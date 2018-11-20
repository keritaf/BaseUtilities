﻿/*
 * Copyright © 2015 - 2018 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */
using OpenTK;
using System;
using System.Diagnostics;

namespace OpenTKUtils.Common
{
    public class MatrixCalc
    {
        public bool InPerspectiveMode { get; set; } = true;
        public Matrix4 ModelMatrix { get; private set; }
        public Matrix4 ProjectionMatrix { get; private set; }
        public Matrix4 ProjectionModelMatrix { get; private set; }

        public Matrix4 InvEyeRotate { get; private set; }

        public float ZoomDistance { get; set; } = 1000F;       // distance that zoom=1 will be from the Position, in the direction of the camera.
        public float PerspectiveFarZDistance { get; set; } = 1000000.0f;        // perspective, set Z's for clipping
        public float PerspectiveNearZDistance { get; set; } = 1f;
        public float OrthographicDistance { get; set; } = 5000.0f;              // Orthographic, give scale

        public float CalcEyeDistance(float zoom) { return ZoomDistance / zoom; }    // distance of eye from target position

        public Vector3 TargetPosition { get; private set; }                     // after ModelMatrix
        public Vector3 EyePosition { get; private set; }                        // after ModelMatrix
        public float EyeDistance { get; private set; }                          // after ModelMatrix

        // Calculate the model matrix, which is the view onto the model
        // model matrix rotates and scales the model to the eye position
        // Model matrix does not have any Y inversion..

        public void CalculateModelMatrix(Vector3 position, Vector3 cameraDir, float zoom)       // We compute the model matrix, not opengl, because we need it before we do a Paint for other computations
        {
            TargetPosition = position;      // record for shader use

            if (InPerspectiveMode)
            {
                Vector3 eye, normal;
                CalculateEyePosition(position, cameraDir, zoom, out eye, out normal);
                EyePosition = eye;
                ModelMatrix = Matrix4.LookAt(eye, position, normal);   // from eye, look at target, with up giving the rotation of the look
            }
            else
            {                                                               // replace open gl computation with our own.
                Matrix4 scale = Matrix4.CreateScale(zoom);
                Matrix4 offset = Matrix4.CreateTranslation(-position.X, -position.Y, -position.Z);
                Matrix4 rotcam = Matrix4.Identity;
                rotcam *= Matrix4.CreateRotationY((float)(-cameraDir.Y * Math.PI / 180.0f));
                rotcam *= Matrix4.CreateRotationX((float)((cameraDir.X - 90) * Math.PI / 180.0f));
                rotcam *= Matrix4.CreateRotationZ((float)(cameraDir.Z * Math.PI / 180.0f));

                Matrix4 preinverted = Matrix4.Mult(offset, scale);
                EyePosition = new Vector3(preinverted.Row0.X, preinverted.Row1.Y, preinverted.Row2.Z);          // TBD.. 
                preinverted = Matrix4.Mult(preinverted, rotcam);
                ModelMatrix = preinverted;
            }

            ProjectionModelMatrix = Matrix4.Mult(ModelMatrix, ProjectionMatrix);        // order order order ! so important.
            EyeDistance = CalcEyeDistance(zoom);


            Matrix4 transform = Matrix4.Identity;                   // identity nominal matrix, dir is in degrees
            //transform *= Matrix4.CreateRotationX(90f.Radians());
            //transform *= Matrix4.CreateRotationX((-cameraDir.X).Radians());
            //transform *= Matrix4.CreateRotationY((-cameraDir.Y).Radians());
            //transform *= Matrix4.CreateRotationZ((-cameraDir.Y).Radians());
            //transform *= Matrix4.CreateRotationZ((float)(-cameraDir.Z * Math.PI / 180.0f));
            //System.Diagnostics.Debug.WriteLine("Dir " + cameraDir);
            InvEyeRotate = transform;
        }

        // used for calculating positions on the screen from pixel positions
        public Matrix4 GetResMat
        {
            get
            {
                return ProjectionModelMatrix;
            }
        }

        // Projection matrix - projects the 3d model space to the 2D screen
        // Has a Y flip so that +Y is going up.

        public void CalculateProjectionMatrix(float fov, int w, int h, out float znear)
        {
            if (InPerspectiveMode)
            {                                                                   // Fov, perspective, znear, zfar
                znear = 1.0F;
                ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fov, (float)w / h, PerspectiveNearZDistance, PerspectiveFarZDistance);
            }
            else
            {
                znear = -OrthographicDistance;
                float orthoheight = (OrthographicDistance / 5.0f) * h / w;
                ProjectionMatrix = Matrix4.CreateOrthographic(OrthographicDistance*2.0f/5.0f, orthoheight * 2.0F, -OrthographicDistance, OrthographicDistance);
            }

            Matrix4 flipy = Matrix4.CreateScale(new Vector3(1, -1, 1));
            ProjectionMatrix = flipy * ProjectionMatrix;                                // Flip Y to correct for openGL Y orientation

            ProjectionModelMatrix = Matrix4.Mult(ModelMatrix, ProjectionMatrix);
        }

        // calculate pos of eye, given position, CameraDir, zoom.

        private void CalculateEyePosition(Vector3 position, Vector3 cameraDir, float zoom, out Vector3 eye, out Vector3 normal)
        {
            Matrix3 transform = Matrix3.Identity;                   // identity nominal matrix, dir is in degrees
            transform *= Matrix3.CreateRotationZ((float)(cameraDir.Z * Math.PI / 180.0f));
            transform *= Matrix3.CreateRotationX((float)(cameraDir.X * Math.PI / 180.0f));
            transform *= Matrix3.CreateRotationY((float)(cameraDir.Y * Math.PI / 180.0f));
            // transform ends as the camera direction vector
            // calculate where eye is, relative to target. its ZoomDistance/zoom, rotated by camera rotation.  This is based on looking at 0,0,0.  
            // the 0,-1,0 means (0,0,0) is looking down on the target pos, (90,0,0) is on the plane looking towards +Z.

            Vector3 eyerel = Vector3.Transform(new Vector3(0.0f, -CalcEyeDistance(zoom), 0.0f), transform);

            // rotate the up vector (0,0,1) by the eye camera dir to get a vector upwards from the current camera dir
            normal = Vector3.Transform(new Vector3(0.0f, 0.0f, 1.0f), transform);

            eye = position + eyerel;              // eye is here, the target pos, plus the eye relative position
            System.Diagnostics.Debug.WriteLine("Eye " + eye + " target " + position + " dir " + cameraDir + " camera dist " + CalcEyeDistance(zoom) + " zoom " + zoom);
        }

    }
}
