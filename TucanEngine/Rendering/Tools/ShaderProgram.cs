using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace TucanEngine.Rendering.Tools
{
    public abstract class ShaderProgram
    {
        private readonly int programId;
        private readonly int vertexShaderId, fragmentShaderId;
        private readonly Dictionary<string, int> uniforms = new Dictionary<string, int>();
        private static readonly List<ShaderProgram> shaders = new List<ShaderProgram>();

        protected ShaderProgram(string vertex, string fragment) {
            vertexShaderId = LoadShaderFromSource(vertex, ShaderType.VertexShader);
            fragmentShaderId = LoadShaderFromSource(fragment, ShaderType.FragmentShader);
            programId = GL.CreateProgram();
            
            GL.AttachShader(programId, vertexShaderId);
            GL.AttachShader(programId, fragmentShaderId);
            
            // ReSharper disable once VirtualMemberCallInConstructor
            BindAttributes();
            
            GL.LinkProgram(programId);
            GL.ValidateProgram(programId);
        }

        public void Start() => GL.UseProgram(programId);
        
        public void Clear() {
            GL.UseProgram(0);
            GL.DetachShader(programId, vertexShaderId);
            GL.DetachShader(programId, fragmentShaderId);
            GL.DeleteShader(vertexShaderId);
            GL.DeleteShader(fragmentShaderId);
            GL.DeleteProgram(programId);
        }

        public static void DeleteShaders() {
            foreach (var shader in shaders) shader.Clear();
            shaders.Clear();
        }

        protected abstract void BindAttributes();
        protected void BindAttribute(int attribute, string variableName) => GL.BindAttribLocation(programId, attribute, variableName);
        
        private int GetUniformLocation(string uniformName) => GL.GetUniformLocation(programId, uniformName);

        #region [ Uniform data bindings ]
        public void SetUniform(string location, float value) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform1(uniforms[location], value);
        }
        
        public void SetUniform(string location, int value) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform1(uniforms[location], value);
        }
        
        public void SetUniform(string location, double value) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform1(uniforms[location], value);
        }
        
        public void SetUniform(string location, Vector3 value) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform3(uniforms[location], value);
        }
        
        public void SetUniform(string location, float x, float y, float z) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform3(uniforms[location], x, y, z);
        }
        
        public void SetUniform(string location, Vector4 value) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform4(uniforms[location], value);
        }
        
        public void SetUniform(string location, float x, float y, float z, float w) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform4(uniforms[location], x, y, z, w);
        }
        
        public void SetUniform(string location, Vector2 value) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform2(uniforms[location], value);
        }
        
        public void SetUniform(string location, float x, float y) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform2(uniforms[location], x, y);
        }
        
        public void SetUniform(string location, Color4 value) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform4(uniforms[location], value);
        }
        
        public void SetUniform(string location, Matrix4 value) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.UniformMatrix4(uniforms[location], false, ref value);
        }
        
        public void SetUniform(string location, Matrix3 value) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.UniformMatrix3(uniforms[location], false, ref value);
        }
        
        public void SetUniform(string location, bool value) {
            if (!uniforms.ContainsKey(location)) uniforms.Add(location, GetUniformLocation(location));
            GL.Uniform1(uniforms[location], Convert.ToInt32(value));
        }
        #endregion

        private static int LoadShaderFromSource(string source, ShaderType type)
        {
            var shaderId = GL.CreateShader(type);
            GL.ShaderSource(shaderId, source);
            GL.CompileShader(shaderId);
            var log = GL.GetShaderInfoLog(shaderId);
            if (!string.IsNullOrEmpty(log)) throw new Exception(log);
            return shaderId;
        }
    }
}