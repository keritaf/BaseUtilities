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
    public class GLFragmentShadersBase : IGLShader
    {
        public int Id { get { return program.Id; } }

        public virtual string Code() { return null; }

        private GLProgram program;

        public void CompileLink()
        {
            program = new OpenTKUtils.GL4.GLProgram();
            string ret = program.Compile(OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader, Code());
            System.Diagnostics.Debug.Assert(ret == null, GetType().Name, ret);
            ret = program.Link(separable: true);
            System.Diagnostics.Debug.Assert(ret == null, GetType().Name, ret );
        }

        public virtual void Start(Common.MatrixCalc c) // seperable do not use a program - that is for the pipeline to hook up
        {
        }

        public virtual void Finish()
        {
        }

        public void Dispose()
        {
            program.Dispose();
        }
    }

    // Fragment, requires vs_color
    public class GLFragmentShaderColour : GLFragmentShadersBase
    {
        public override string Code()
        {
            return
@"
#version 450 core
in vec4 vs_color;
out vec4 color;

void main(void)
{
	color = vs_color;
}
";
        }

        public GLFragmentShaderColour()
        {
            CompileLink();
        }
    }

    public class GLFragmentShaderTexture : GLFragmentShadersBase
    {
        public override string Code()
        {
            return
@"
#version 450 core
in vec2 vs_textureCoordinate;
layout (binding=1) uniform sampler2D textureObject;
out vec4 color;

void main(void)
{
    color = texture(textureObject, vs_textureCoordinate);       // vs_texture coords normalised 0 to 1.0f
}
";      }

        public GLFragmentShaderTexture()
        {
            CompileLink();
        }

    }



    public class GLFragmentShader2DCommonBlend : GLFragmentShadersBase
    {
        public float Blend { get; set; } = 0.0f;
        public override string Code()
        {
            return
@"
#version 450 core
in vec2 vs_textureCoordinate;
layout (binding=1) uniform sampler2DArray textureObject;
out vec4 color;
layout (location = 30) uniform float blend;

void main(void)
{
    vec4 col1 = texture(textureObject, vec3(vs_textureCoordinate,0));
    vec4 col2 = texture(textureObject, vec3(vs_textureCoordinate,1));

    //color = vec4(vs_textureCoordinate.x,vs_textureCoordinate.y,0.5,1.0f);
    //color = vec4(blend, blend,0.5,1.0f);
    //color = vec4( mix(col1.x,col2.x,blend), mix(col1.y,col2.y,blend), mix(col1.z,col2.z,blend), 1.0f);
    color = mix(col1,col2,blend);
}
";
        }

        public GLFragmentShader2DCommonBlend()
        {
            CompileLink();
        }

        public override void Start(Common.MatrixCalc c)
        {
            GL.ProgramUniform1(Id, 30, Blend);
        }
    }
}
