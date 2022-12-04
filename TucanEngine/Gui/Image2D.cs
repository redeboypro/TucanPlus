using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TucanEngine.Common.Drawables;
using TucanEngine.Rendering;
using TucanEngine.Rendering.Tools.Common;

namespace TucanEngine.Gui {
    public class Image2D : GuiElement {
        private Texture2D textureData;
        private bool isStretched;

        public Image2D(Texture2D textureData, bool isStretched = false) {
            this.textureData = textureData;
            this.isStretched = isStretched;
        }

        public override void OnRenderFrame(FrameEventArgs e)
        {
            var guiManager = GuiManager.GetCurrentManagerInstance();
            var shaderProgram = guiManager.GetShaderProgram();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureData.Texture);

            shaderProgram.SetUniform(ShaderNamingConstants.ModelMatrix, ModelMatrix);
            shaderProgram.SetUniform(ShaderNamingConstants.Dimensions, RelativeSize);
            shaderProgram.SetUniform(ShaderNamingConstants.IsStretched, isStretched);
            shaderProgram.SetUniform(ShaderNamingConstants.MainColor, GetColor());
            GL.BindVertexArray(guiManager.QuadVAO.Id);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.BindVertexArray(0);
        }
    }
}