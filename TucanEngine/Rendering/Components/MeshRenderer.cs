using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TucanEngine.Gui;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Rendering.Tools.Common;
using TucanEngine.Serialization;

namespace TucanEngine.Rendering.Components
{
    public class MeshRenderer : Behaviour
    {
        [SerializedField]
        private Mesh mesh;
        
        [SerializedField]
        private Texture2D textureData;
        
        [SerializedField]
        private bool ignoreCameraTransformation;

        public Mesh GetMesh() {
            return mesh;
        }
        
        public Texture2D GetTexture() {
            return textureData;
        }
        
        public bool IgnoreCameraTransformation() {
            return ignoreCameraTransformation;
        }

        public void SetMesh(Mesh mesh) {
            this.mesh = mesh;
        }
        
        public void SetTexture(Texture2D textureData) {
            this.textureData = textureData;
        }
        
        public void SetToIgnoreCameraTransformation(bool state) {
            ignoreCameraTransformation = state;
        }
        
        public override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            if (mesh == null) return;

            var scene = Scene.Scene.GetCurrentScene();
            var shaderProgram = MeshShader.GetCurrentShader();
            var gameObject = GetAssignedObject();
            var camera = scene.GetCamera();
            
            GL.BindVertexArray(mesh.GetArrayData().Id);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            if (textureData != null) {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textureData.Texture);
            }

            shaderProgram.SetUniform(ShaderNamingConstants.ProjectionMatrix, camera.GetProjectionMatrix());
            shaderProgram.SetUniform(ShaderNamingConstants.ViewMatrix, camera.GetViewMatrix());
            shaderProgram.SetUniform(ShaderNamingConstants.ModelMatrix, gameObject.GetModelMatrix());

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.DrawElements(PrimitiveType.Triangles, mesh.GetVertexCount(), DrawElementsType.UnsignedInt,
                IntPtr.Zero);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.BindVertexArray(0);
        }
    }
}