using System;
using OpenTK;
using OpenTK.Input;

namespace TucanEngine.Main
{
    public interface IBehaviour
    {
        #region [ Input events implementation ]
        void OnKeyDown(KeyboardKeyEventArgs e);
        void OnKeyUp(KeyboardKeyEventArgs e);
        void OnKeyPress(KeyPressEventArgs e);
        void OnMouseDown(MouseButtonEventArgs e);
        void OnMouseUp(MouseButtonEventArgs e);
        void OnMouseMove(MouseMoveEventArgs e);
        #endregion
        
        #region [ Game loop events implementation ]
        void OnLoad(EventArgs e);
        void OnUpdateFrame(FrameEventArgs e);
        void OnRenderFrame(FrameEventArgs e);
        #endregion
    }
}