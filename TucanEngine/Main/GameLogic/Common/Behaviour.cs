using System;
using OpenTK;
using OpenTK.Input;

namespace TucanEngine.Main.GameLogic.Common
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
    
    public abstract class Behaviour
    {
        private GameObject assignedObject;
        private bool isActive;

        public GameObject GetAssignedObject() {
            return assignedObject;
        }
        
        public bool IsActive() {
            return isActive;
        }
        
        public void AssignObject(GameObject objectToAssign) {
            assignedObject = objectToAssign;
        }

        public void SetActive(bool isActive) {
            this.isActive = isActive;
            if (isActive) OnActivated(); 
            else OnDeactivated();
        }

        public virtual void OnKeyDown(KeyboardKeyEventArgs e){}
        public virtual void OnKeyUp(KeyboardKeyEventArgs e){}
        public virtual void OnKeyPress(KeyPressEventArgs e){}
        public virtual void OnMouseDown(MouseButtonEventArgs e){}
        public virtual void OnMouseUp(MouseButtonEventArgs e){}
        public virtual void OnMouseMove(MouseMoveEventArgs e){}
        public virtual void OnLoad(EventArgs e){}
        public virtual void OnUpdateFrame(FrameEventArgs e){}
        public virtual void OnRenderFrame(FrameEventArgs e){}
        public virtual void OnAwake(){}
        public virtual void OnActivated(){}
        public virtual void OnDeactivated(){}
        public virtual void OnObjectActivated(){}
        public virtual void OnObjectDeactivated(){}
    }
}