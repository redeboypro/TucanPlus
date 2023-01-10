using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TucanEngine.Common.Math;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.Common;

namespace TucanEngine.Gui
{
    public enum Orientation { Horizontal, Vertical }
    public enum GuiEventType { Press, Release, Drag }
    public delegate void GuiEvent(object args);
    public abstract class GuiElement : Transform, IBehaviour
    {
        private readonly Dictionary<GuiEventType, List<GuiEvent>> GuiEvents = new Dictionary<GuiEventType, List<GuiEvent>> {
            { GuiEventType.Press , new List<GuiEvent>() },
            { GuiEventType.Release, new List<GuiEvent>() },
            { GuiEventType.Drag, new List<GuiEvent>() }
        };

        private Color4 color = Color4.White;
        private Vector3 boundsMin;
        private Vector3 boundsMax;
        
        private bool isPressed;
        private bool isHighlighted;

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

        protected override void OnTransformMatrices() {
            base.OnTransformMatrices();
            RecalculateBounds();
        }

        public virtual void OnKeyDown(KeyboardKeyEventArgs e) {
            InvokeInChildren(nameof(OnKeyDown), e);
        }

        public virtual void OnKeyUp(KeyboardKeyEventArgs e) {
            InvokeInChildren(nameof(OnKeyUp), e);
        }

        public virtual void OnKeyPress(KeyPressEventArgs e) {
            InvokeInChildren(nameof(OnKeyPress), e);
        }
        
        public virtual void OnMouseDown(MouseButtonEventArgs e) {
            if (!MouseIsInsideBounds(e)) return;
            OnPress();
            isPressed = true;
            InvokeInChildren(nameof(OnMouseDown), e);
        }
        
        public virtual void OnMouseUp(MouseButtonEventArgs e) {
            isPressed = false;
            if (!MouseIsInsideBounds(e)) {
                return;
            }
            OnRelease();
            InvokeInChildren(nameof(OnMouseUp), e);
        }
        
        public virtual void OnMouseMove(MouseMoveEventArgs e) {
            if (isPressed) {
                OnDrag(e);
            }
            
            if (MouseIsInsideBounds(e)) {
                if (isHighlighted) {
                    return;
                }
                OnFocus();
                isHighlighted = true;
            }
            else {
                if (!isHighlighted) {
                    return;
                }
                isHighlighted = false;
                OnOutOfFocus();
            }
            
            InvokeInChildren(nameof(OnMouseMove), e);
        }

        public virtual void OnPress() {
            foreach (var action in GuiEvents[GuiEventType.Press]) {
                action?.Invoke(null);
            }
            InvokeInChildren(nameof(OnPress));
        }
        
        public virtual void OnRelease() {
            foreach (var action in GuiEvents[GuiEventType.Release]) {
                action?.Invoke(null);
            }
        }
        
        public virtual void OnDrag(MouseMoveEventArgs moveEventArgs) {
            foreach (var action in GuiEvents[GuiEventType.Drag]) {
                action?.Invoke(moveEventArgs);
            }
        }

        public virtual void OnLoad(EventArgs e) {
            InvokeInChildren(nameof(OnLoad), e);
        }

        public virtual void OnUpdateFrame(FrameEventArgs e) {
            InvokeInChildren(nameof(OnUpdateFrame), e);
        }

        public virtual void OnRenderFrame(FrameEventArgs e)
        {
            var scissorLocation = Ortho.ToScreenCoordinates(LocalSpaceLocation.X, LocalSpaceLocation.Y);
            var scissorSize = Ortho.ToScreenCoordinates(LocalSpaceScale.X - 1,  LocalSpaceScale.Y - 1);
            
            if (IsMasked) {
                GL.Scissor((int) scissorLocation.X, (int) scissorLocation.Y, (int) scissorSize.X, (int) scissorSize.Y);
                GL.Enable(EnableCap.ScissorTest);
            }

            InvokeInChildren(nameof(OnRenderFrame), e);
            
            if (IsMasked) {
                GL.Disable(EnableCap.ScissorTest);
            }
        }
        
        public void AddExpandingEvent(GuiEvent guiEvent, GuiEventType guiEventType) {
            GuiEvents[guiEventType].Add(guiEvent);
        }
        
        public void RemoveExpandingEvent(GuiEvent guiEvent, GuiEventType guiEventType) {
            GuiEvents[guiEventType].Add(guiEvent);
        }
        
        public virtual void OnFocus() { }

        public virtual void OnOutOfFocus() { }

        public void InvokeInChildren(string methodName, object eventArgs = null) {
            for (var index = 0; index < GetChildCount(); index++) {
                var child = (GuiElement)GetChild(index);
                child.GetType().GetMethod(methodName)?.Invoke(child, eventArgs != null ? new [] { eventArgs } : null);
                child.InvokeInChildren(methodName, eventArgs);
            }
        }
        
        private void RecalculateBounds() {
            var angle = GetRotationAngle();
            var angleSin = Math.Sin(angle);
            var angleCos = Math.Cos(angle);
            
            if (angleSin < 0) angleSin = -angleSin;
            if (angleCos < 0) angleCos = -angleCos;
            
            var width = WorldSpaceScale.Y * angleSin + WorldSpaceScale.X * angleCos;
            var height = WorldSpaceScale.Y * angleCos + WorldSpaceScale.X * angleSin;
            
            var halfExtent = new Vector3((float)width, (float)height, 0);
            boundsMin = WorldSpaceLocation - halfExtent;
            boundsMax = WorldSpaceLocation + halfExtent;
        }
        
        public float GetRotationAngle() {
            var t1 = 2.0f * (WorldSpaceRotation.W * WorldSpaceRotation.Z + WorldSpaceRotation.X * WorldSpaceRotation.Y);
            var t2 = 1.0f - 2.0f * (WorldSpaceRotation.Y * WorldSpaceRotation.Y + WorldSpaceRotation.Z * WorldSpaceRotation.Z);
            return (float) Math.Atan2(t1, t2);
        }
        
        private bool MouseIsInsideBounds(MouseEventArgs e) {
            var processedMouseCoordinates = Ortho.ToGlCoordinates(e.X, e.Y);
            return processedMouseCoordinates.X > boundsMin.X &&
                   processedMouseCoordinates.X < boundsMax.X &&
                   processedMouseCoordinates.Y > boundsMin.Y &&
                   processedMouseCoordinates.Y < boundsMax.Y;
        }
    }
}