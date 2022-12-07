using TucanEngine.Rendering;

namespace TucanEngine.Gui
{
    public class GuiSkin
    {
        private Texture2D boxTexture;
        private Texture2D thumbTexture;
        private Font font;

        public Texture2D GetBoxTexture() {
            return boxTexture;
        }
        
        public Texture2D GetThumbTexture() {
            return thumbTexture;
        }
        
        public Font GetFont() {
            return font;
        }

        public void SetBoxTexture(Texture2D textureData) {
            boxTexture = textureData;
        }
        
        public void SetThumbTexture(Texture2D textureData) {
            thumbTexture = textureData;
        }
        
        public void SetFont(Texture2D textureData) {
            font = new Font(textureData);
        }
    }
}