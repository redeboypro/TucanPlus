using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using TucanEngine.Common.EventTranslation;
using TucanEngine.Main;

namespace TucanEngine.Gui
{
    public abstract class GuiElement : IBehaviour
    {
        private GuiElement parent;
        private Color4 color = Color4.White;
        private bool isPressed;
        private bool isHighlighted;
        
        public GuiElement GetParent() {
            return parent;
        }
        
        public Color4 GetColor() {
            return color;
        }
        
        public void SetParent(GuiElement parent) {
            this.parent = parent;
            UpdateModelMatrix();
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

        #region [ Transform implementation ]
        private Vector2 relativeLocation = Vector2.Zero;
        private Vector2 relativeSize = Vector2.One;
        
        private Vector2 globalLocation = Vector2.Zero;
        private Vector2 globalSize = Vector2.One;

        public Matrix4 ModelMatrix { get; private set; } = Matrix4.Identity;

        public Vector2 RelativeLocation {
            get => relativeLocation;
            set {
                relativeLocation = value; 
                UpdateModelMatrix();
                OnChangeLocation();
            }
        }
        
        public Vector2 RelativeSize {
            get => relativeSize;
            set { 
                relativeSize = value; 
                UpdateModelMatrix(); 
                OnChangeSize();
            }
        }

        public Vector2 GetGlobalLocation() {
            return globalLocation;
        }
        
        public Vector2 GetGlobalSize() {
            return globalSize;
        }

        private void UpdateModelMatrix() {
            var parentMatrix = parent?.ModelMatrix ?? Matrix4.Identity;
            ModelMatrix = Matrix4.CreateScale(relativeSize.X, relativeSize.Y, 1) *
                          Matrix4.CreateTranslation(relativeLocation.X, relativeLocation.Y, 0) * parentMatrix;

            globalLocation = ModelMatrix.ExtractTranslation().Xy;
            globalSize = ModelMatrix.ExtractScale().Xy;
        }

        protected virtual void OnChangeLocation() { }
        protected virtual void OnChangeSize() { }
        #endregion
        
        #region [ Input events implementation ]
        public virtual void OnKeyDown(KeyboardKeyEventArgs e) { }
        public virtual void OnKeyUp(KeyboardKeyEventArgs e) { }
        public virtual void OnKeyPress(KeyPressEventArgs e) { }
        public virtual void OnMouseDown(MouseButtonEventArgs e) {
            if (!MouseIsInsideBounds(e)) return;
            OnPress();
            isPressed = true;
        }

        public virtual void OnMouseUp(MouseButtonEventArgs e) {
            if (!MouseIsInsideBounds(e)) return;
            OnRelease();
            isPressed = false;
        }
        public virtual void OnMouseMove(MouseMoveEventArgs e) {
            if (MouseIsInsideBounds(e)) {
                if(isPressed) OnDrag();
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
        public virtual void OnPress() { }
        public virtual void OnRelease() { }
        public virtual void OnDrag() { }
        public virtual void OnFocus() { }
        public virtual void OnOutOfFocus() { }
        private bool MouseIsInsideBounds(MouseEventArgs e) {
            var processedMouseCoordinates = Ortho.ToGlCoordinates(e.X, e.Y);
            var higherCorner = globalLocation + globalSize;
            return processedMouseCoordinates.X > globalLocation.X &&
                   processedMouseCoordinates.X < higherCorner.X &&
                   processedMouseCoordinates.Y > globalLocation.Y &&
                   processedMouseCoordinates.Y < higherCorner.Y;
        }
        #endregion

        #region [ Game loop events implementation ]
        public virtual void OnLoad(EventArgs e) => UpdateModelMatrix();
        public virtual void OnUpdateFrame(FrameEventArgs e) { }
        public virtual void OnRenderFrame(FrameEventArgs e) { }
        #endregion
    }
}