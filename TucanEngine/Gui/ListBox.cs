using TucanEngine.Rendering;

namespace TucanEngine.Gui
{
    public class ListBox : Image2D {
        
        
        public ListBox(Texture2D textureData, bool isStretched = false) : base(textureData, isStretched) {
            IsMasked = true;
        }
    }
}