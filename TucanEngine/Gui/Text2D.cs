﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using TucanEngine.Common.EventTranslation;
using TucanEngine.Rendering.Tools.Common;

namespace TucanEngine.Gui
{
    public class Text2D : GuiElement
    {
        private string text;

        public Text2D(string text) {
            this.text = text;
        }

        public override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            var guiManager = GuiManager.GetCurrentManagerInstance();
            var shaderProgram = guiManager.GetShaderProgram();
            var font = guiManager.GetSkin().GetFont();
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, font.GetTexture().Texture);
            
            for (var i = 0; i < text.Length; i++) {
                if (!Font.CharSheet.Contains(text[i].ToString())) continue;
                var charWidth = LocalSpaceScale.X / text.Length;
                var matrix = Matrix4.CreateScale(LocalSpaceScale.ScaleBy(charWidth, Axis.X)) *
                             Matrix4.CreateFromQuaternion(LocalSpaceRotation) * 
                             Matrix4.CreateTranslation(LocalSpaceLocation.AddUnit(charWidth * i, Axis.X))
                             * GetModelMatrix();
                
                shaderProgram.SetUniform(ShaderNamingConstants.ModelMatrix, matrix);
                shaderProgram.SetUniform(ShaderNamingConstants.IsStretched, false);
                shaderProgram.SetUniform(ShaderNamingConstants.MainColor, GetColor());

                GL.BindVertexArray(font.GetCharArrayData(text[i]).Id);
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);

                GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

                GL.DisableVertexAttribArray(0);
                GL.DisableVertexAttribArray(1);
            }
            
            GL.BindVertexArray(0);
        }

        public string GetText() {
            return text;
        }

        public void SetText(string text) {
            this.text = text;
        }
    }
}