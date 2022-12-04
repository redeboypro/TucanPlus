using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using TucanEngine.Rendering;

namespace TucanEngine.Gui
{
    public class Button : Image2D
    {
        private GuiElement content;
        
        public Button(GuiElement content, Texture2D textureData, bool isStretched = false) : base(textureData, isStretched) {
            this.content = content;
            content?.SetParent(this);
        }

        #region [ Events test ]
        public override void OnLoad(EventArgs e) {
            SetColor(Color4.LightGray);
        }

        public override void OnFocus() {
            SetColor(Color4.White);
        }

        public override void OnOutOfFocus() {
            SetColor(Color4.LightGray);
        }

        public override void OnPress() {
            SetColor(Color4.Gray);
        }

        public override void OnRelease() {
            SetColor(IsHighlighted() ? Color4.White : Color4.LightGray);
        }
        #endregion
    }
}