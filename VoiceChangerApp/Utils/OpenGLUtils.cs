﻿using SharpGL;
using System;
using System.Text;

namespace VoiceChangerApp.Utils
{
    public static class OpenGLUtils
    {
        //public static uint GL_DEBUG_OUTPUT = 
        public static uint NO_BUFFER = 0;
        public static uint NO_SHADER = 0;
        public static uint NO_PROGRAM = 0;

        public static void InitDebug(this OpenGL gl)
        {
            //gl.Enable(OpenGL.Debug);            
            //glDebugMessageCallback(MessageCallback, 0);
        }

        public static uint CompileProgram(this OpenGL gl, byte[] vertexSource, byte[] fragmentSource)
        {
            uint vertexShaderID = gl.CompileShader(vertexSource, OpenGL.GL_VERTEX_SHADER);
            uint fragmentShaderID = gl.CompileShader(fragmentSource, OpenGL.GL_FRAGMENT_SHADER);
            uint programID = gl.CreateProgram();
            gl.AttachShader(programID, vertexShaderID);
            gl.AttachShader(programID, fragmentShaderID);
            gl.DeleteShader(vertexShaderID);
            gl.DeleteShader(fragmentShaderID);
            gl.LinkProgram(programID);
            return programID;
        }

        public static uint CompileShader(this OpenGL gl, byte[] source, uint shaderType)
        {
            var sourceString = Encoding.ASCII.GetString(source);
            return CompileShader(gl, sourceString, shaderType);
        }

        public static unsafe uint CompileShader(this OpenGL gl, string source, uint shaderType)
        {
            var shaderID = gl.CreateShader(shaderType);
            gl.ShaderSource(shaderID, source);
            gl.CompileShader(shaderID);

            var status = new int[10];
            gl.GetShader(shaderID, OpenGL.GL_COMPILE_STATUS, status);
            if (status[0] != OpenGL.GL_TRUE)
            {
                int[] logLength = new int[1];
                gl.GetShader(shaderID, OpenGL.GL_INFO_LOG_LENGTH, logLength);

                var log = new StringBuilder(logLength[0]);
                gl.GetShaderInfoLog(shaderID, logLength[0], IntPtr.Zero, log);

                throw new Exception($"Failed to compile shader. Error:\n {log} \nSource:\n{source}");
            }

            return shaderID;
        }

        public static void CheckGLError(this OpenGL gl)
        {
            while (true)
            {
                var errorCode = gl.GetError();
                if (errorCode == OpenGL.GL_NO_ERROR)
                {
                    return;
                }

                throw new Exception($"OpenGL error: {errorCode}");
            }
        }
    }
}
