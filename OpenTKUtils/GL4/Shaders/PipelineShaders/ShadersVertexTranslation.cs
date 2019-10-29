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

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKUtils.GL4
{
    // Pipeline shader, Translation
    // Requires:
    //      location 0 : position: vec4 vertex array of positions
    //      uniform 0 : GL MatrixCalc
    //      uniform 4 : transform: mat4 array of transforms
    // Out:
    //      vs_color is based on instance ID mostly for debugging
    //      gl_Position

    public class GLVertexShaderMatrixTranslation : GLShaderPipelineShadersBase
    {
        public string Code()
        {
            return
@"
#version 450 core
" + GLMatrixCalcUniformBlock.GLSL + @"
layout (location = 0) in vec4 position;
layout (location = 4) in mat4 transform;

out gl_PerVertex {
        vec4 gl_Position;
        float gl_PointSize;
        float gl_ClipDistance[];
    };

out vec4 vs_color;

void main(void)
{
    vs_color = vec4(gl_InstanceID*0.2+0.2,gl_InstanceID*0.2+0.2,0.5+gl_VertexID*0.1,1.0);       // colour may be thrown away if required..
	gl_Position = mc.ProjectionModelMatrix * transform * position;        // order important
}
";
        }

        public GLVertexShaderMatrixTranslation()
        {
            Program = GLProgram.CompileLink(ShaderType.VertexShader, Code(), GetType().Name);
        }
    }

    // Pipeline shader, Translation, Texture
    // Requires:
    //      location 0 : position: vec4 vertex array of positions
    //      location 1 : vec2 texture co-ordinates
    //      uniform 0 : GL MatrixCalc
    //      uniform 4 : transform: mat4 array of transforms
    // Out:
    //      vs_textureCoordinate
    //      gl_Position

    public class GLVertexShaderTextureMatrixTranslation : GLShaderPipelineShadersBase
    {
        public string Code()
        {
            return
@"
#version 450 core
" + GLMatrixCalcUniformBlock.GLSL + @"
layout (location = 0) in vec4 position;
layout (location = 4) in mat4 transform;

out gl_PerVertex {
        vec4 gl_Position;
        float gl_PointSize;
        float gl_ClipDistance[];
    };

layout(location = 1) in vec2 texco;

out VS_OUT
{
    flat int vs_instanced;
} vs_out;

out vec2 vs_textureCoordinate;

void main(void)
{
	gl_Position = mc.ProjectionModelMatrix * transform * position;        // order important
    vs_textureCoordinate = texco;
    vs_out.vs_instanced = gl_InstanceID;
}
";
        }

        public GLVertexShaderTextureMatrixTranslation()
        {
            Program = GLProgram.CompileLink(ShaderType.VertexShader, Code(), GetType().Name);
        }
    }

    // Pipeline shader, Translation, Color, Common transform, Object transform
    // Requires:
    //      location 0 : position: vec4 vertex array of positions
    //      location 1 : vec4 colours of vertexs
    //      uniform 0 : GL MatrixCalc
    //      uniform 22 : objecttransform: mat4 array of transforms
    //      uniform 23 : commontransform: mat4 array of transforms
    // Out:
    //      gl_Position
    //      vs_textureCoordinate

    public class GLVertexShaderColorTransformWithCommonTransform : GLShaderPipelineShadersBase
    {
        public GLObjectDataTranslationRotation Transform { get; set; }           // only use this for rotation - position set by object data

        public string Code()
        {
            return

@"
#version 450 core
" + GLMatrixCalcUniformBlock.GLSL + @"
layout (location = 0) in vec4 position;
out gl_PerVertex {
        vec4 gl_Position;
        float gl_PointSize;
        float gl_ClipDistance[];
    };

layout(location = 1) in vec4 color;
out vec4 vs_color;

layout (location = 22) uniform  mat4 objecttransform;
layout (location = 23) uniform  mat4 commontransform;

void main(void)
{
	gl_Position = mc.ProjectionModelMatrix * objecttransform *  commontransform * position;        // order important
	vs_color = color;                                                   // pass to fragment shader
}
";
        }

        public GLVertexShaderColorTransformWithCommonTransform()
        {
            Transform = new GLObjectDataTranslationRotation();
            Program = GLProgram.CompileLink(ShaderType.VertexShader, Code(), GetType().Name);
        }

        public override void Start()
        {
            base.Start();
            Matrix4 t = Transform.Transform;
            GL.ProgramUniformMatrix4(Id, 23, false, ref t);
            OpenTKUtils.GLStatics.Check();
        }
    }

    // Pipeline shader, Translation, Texture, Common transform, Object transform
    // Requires:
    //      location 0 : position: vec4 vertex array of positions
    //      location 1 : vec2 texture co-ords
    //      uniform 0 : GL MatrixCalc
    //      uniform 22 : objecttransform: mat4 array of transforms
    //      uniform 23 : commontransform: mat4 array of transforms
    // Out:
    //      gl_Position
    //      vs_textureCoordinate

    public class GLVertexShaderTextureTransformWithCommonTransform : GLShaderPipelineShadersBase
    {
        public GLObjectDataTranslationRotation Transform { get; set; }           // only use this for rotation - position set by object data

        public string Code()
        {
            return

@"
#version 450 core
" + GLMatrixCalcUniformBlock.GLSL + @"
layout (location = 0) in vec4 position;
out gl_PerVertex {
        vec4 gl_Position;
        float gl_PointSize;
        float gl_ClipDistance[];
    };

layout(location = 1) in vec2 texco;
out vec2 vs_textureCoordinate;

layout (location = 22) uniform  mat4 objecttransform;
layout (location = 23) uniform  mat4 commontransform;

void main(void)
{
	gl_Position = mc.ProjectionModelMatrix * objecttransform *  commontransform * position;        // order important
    vs_textureCoordinate = texco;
}
";
        }

        public GLVertexShaderTextureTransformWithCommonTransform()
        {
            Transform = new GLObjectDataTranslationRotation();
            Program = GLProgram.CompileLink(ShaderType.VertexShader, Code(), GetType().Name);
        }

        public override void Start()
        {
            base.Start();
            Matrix4 t = Transform.Transform;
            GL.ProgramUniformMatrix4(Id, 23, false, ref t);
            OpenTKUtils.GLStatics.Check();
        }
    }
}

