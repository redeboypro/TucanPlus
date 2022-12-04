using OpenTK;
using OpenTK.Graphics.OpenGL;
using TucanEngine.Rendering.Tools.Common;

namespace TucanEngine.Gui
{
    public class Text2D : GuiElement
    {
        private Font font;
        private string text;

        public Text2D(Font font, string text) {
            this.font = font;
            this.text = text;
        }

        public override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            var guiManager = GuiManager.GetCurrentManagerInstance();
            var shaderProgram = guiManager.GetShaderProgram();
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, font.GetTexture().Texture);
            
            for (var i = 0; i < text.Length; i++)
            {
                if (!Font.CharSheet.Contains(text[i].ToString())) continue;
                var charWidth = RelativeSize.X / text.Length;
                var matrix = 
                    Matrix4.CreateScale(charWidth, RelativeSize.Y, 1) * 
                    Matrix4.CreateTranslation(RelativeLocation.X + charWidth * i, RelativeLocation.Y, 0) * ModelMatrix;
                
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

        public Font GetFont() {
            return font;
        }
        
        public string GetText() {
            return text;
        }
        
        public void SetFont(Font font) {
            this.font = font;
        }
        
        public void SetText(string text) {
            this.text = text;
        }
    }
}