using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TucanEngine.Gui;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.Common;

namespace TucanEngine.Rendering.Components
{
    public class MeshRenderer : Behaviour
    {
        private Mesh mesh;
        private Texture2D textureData;

        public Mesh GetMesh() {
            return mesh;
        }
        
        public Texture2D GetTexture() {
            return textureData;
        }

        public void SetMesh(Mesh mesh) {
            this.mesh = mesh;
        }
        
        public void SetTexture(Texture2D textureData) {
            this.textureData = textureData;
        }
        
        public override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            if (mesh == null) return;
            
            var guiManager = GuiManager.GetCurrentManagerInstance();
            var shaderProgram = guiManager.GetShaderProgram();
            var gameObject = GetAssignedObject();
            
            GL.BindVertexArray(mesh.GetArrayData().Id);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureData.Texture);
            
            if (gameObject.GetParent() is Camera) {
                //Not implemented
            }
            else {
                //Not implemented
            }

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