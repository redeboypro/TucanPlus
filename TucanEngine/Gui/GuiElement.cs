using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TucanEngine.Common.EventTranslation;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.Common;

namespace TucanEngine.Gui
{
    public enum Orientation { Horizontal, Vertical }
    public delegate void MouseMovingEvent(MouseMoveEventArgs e);
    public abstract class GuiElement : Transform, IBehaviour
    {
        private List<MouseMovingEvent> dragEvents = new List<MouseMovingEvent>();
        private List<Action> pressEvents = new List<Action>();
        private List<Action> releaseEvents = new List<Action>();

        private Color4 color = Color4.White;
        private bool isPressed;
        private bool isHighlighted;

        private Vector3 min; 
        private Vector3 max;

        public bool IsMasked { get; set; }

        public Color4 GetColor() {
            return color;
        }

        public void SetColor(Color4 color) {
            this.color = color;
        }
        
        public bool IsPressed() {
            return isPressed;
        }

        public bool IsHighlighted() {
            return isHighlighted;
        }
        
        public override void OnMoving() { 
            base.OnMoving();
        }
        
        public override void OnRotating() { 
            base.OnRotating();
        }
        
        public override void OnScaling() { 
            base.OnScaling();
        }

        public override void OnTransformMatrices()
        {
            base.OnTransformMatrices();
            RecalculateBounds();
        }

        public virtual void OnKeyDown(KeyboardKeyEventArgs e) { }
        public virtual void OnKeyUp(KeyboardKeyEventArgs e) { }
        public virtual void OnKeyPress(KeyPressEventArgs e) { }
        public virtual void OnMouseDown(MouseButtonEventArgs e) {
            if (!MouseIsInsideBounds(e)) return;
            OnPress();
            isPressed = true;
        }
        
        public virtual void OnMouseUp(MouseButtonEventArgs e) {
            isPressed = false;
            if (!MouseIsInsideBounds(e)) return;
            OnRelease();
        }
        
        public virtual void OnMouseMove(MouseMoveEventArgs e) {
            if (isPressed) OnDrag(e);
            if (MouseIsInsideBounds(e)) {
                if (isHighlighted) return;
                OnFocus();
                isHighlighted = true;
            }
            else {
                if (!isHighlighted) return;
                isHighlighted = false;
                OnOutOfFocus();
            }
        }
        
        public void AddPressEvent(Action e) {
            pressEvents.Add(e);
        }
        
        public void AddReleaseEvent(Action e) {
            releaseEvents.Add(e);
        }
        
        public void AddDragEvent(MouseMovingEvent e) {
            dragEvents.Add(e);
        }
        
        public void RemovePressEvent(Action e) {
            pressEvents.Remove(e);
        }
        
        public void RemoveReleaseEvent(Action e) {
            releaseEvents.Remove(e);
        }
        
        public void RemoveDragEvent(MouseMovingEvent e) {
            dragEvents.Remove(e);
        }
        
        public virtual void OnPress() {
            foreach (var action in pressEvents) {
                action.Invoke();
            }
        }
        
        public virtual void OnRelease() {
            foreach (var action in releaseEvents) {
                action.Invoke();
            }
        }
        
        public virtual void OnDrag(MouseMoveEventArgs e) {
            foreach (var action in dragEvents) {
                action.Invoke(e);
            }
        }
        
        public virtual void OnFocus() { }
        
        public virtual void OnOutOfFocus() { }
        
        private bool MouseIsInsideBounds(MouseEventArgs e) {
            var processedMouseCoordinates = Ortho.ToGlCoordinates(e.X, e.Y);
            return processedMouseCoordinates.X > min.X &&
                   processedMouseCoordinates.X < max.X &&
                   processedMouseCoordinates.Y > min.Y &&
                   processedMouseCoordinates.Y < max.Y;
        }
        
        private void RecalculateBounds()
        {
            var angle = GetRotationAngle();
            var angleSin = Math.Sin(angle);
            var angleCos = Math.Cos(angle);
            if (angleSin < 0) angleSin = -angleSin;
            if (angleCos < 0) angleCos = -angleCos;
            var width = WorldSpaceScale.Y * angleSin + WorldSpaceScale.X * angleCos;
            var height = WorldSpaceScale.Y * angleCos + WorldSpaceScale.X * angleSin;
            var halfExtent = new Vector3((float)width, (float)height, 0);
            min = WorldSpaceLocation - halfExtent;
            max = WorldSpaceLocation + halfExtent;
        }
        
        public float GetRotationAngle() {
            var t1 = 2.0f * (WorldSpaceRotation.W * WorldSpaceRotation.Z + WorldSpaceRotation.X * WorldSpaceRotation.Y);
            var t2 = 1.0f - 2.0f * (WorldSpaceRotation.Y * WorldSpaceRotation.Y + WorldSpaceRotation.Z * WorldSpaceRotation.Z);
            return (float)Math.Atan2(t1, t2);
        }
        
        public virtual void OnLoad(EventArgs e) { }
        public virtual void OnUpdateFrame(FrameEventArgs e) { }
        public virtual void OnRenderFrame(FrameEventArgs e) { }

        public virtual void OnPostRender(FrameEventArgs e) {
            var scissorLocation = Ortho.ToScreenCoordinates(LocalSpaceLocation.X, LocalSpaceLocation.Y);
            var scissorSize = Ortho.ToScreenCoordinates(LocalSpaceScale.X - 1,  LocalSpaceScale.Y - 1);
            
            if (IsMasked) {
                GL.Scissor((int) scissorLocation.X, (int) scissorLocation.Y, (int) scissorSize.X, (int) scissorSize.Y);
                GL.Enable(EnableCap.ScissorTest);
            }

            for (var index = 0; index < GetChildCount(); index++) {
                ((GuiElement) GetChild(index)).OnRenderFrame(e);
            }

            if (IsMasked) GL.Disable(EnableCap.ScissorTest);
        }
    }
}