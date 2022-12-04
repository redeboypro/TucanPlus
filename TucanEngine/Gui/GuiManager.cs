using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TucanEngine.Common.Drawables;
using TucanEngine.Main;
using TucanEngine.Rendering;
using TucanEngine.Rendering.Tools;
using TucanEngine.Rendering.Tools.Common;

namespace TucanEngine.Gui
{
// ReSharper disable InconsistentNaming
    public class GuiManager : IBehaviour
    {
        public const int GridSize = 3;
        private const float CellSize = 1f / GridSize;
        
        private static GuiManager currentManagerInstance;
        private List<GuiElement> guiElements = new List<GuiElement>();
        private readonly ShaderProgram shaderProgram;

        #region [ VAOs ]
        public readonly ArrayData QuadVAO = new ArrayData();
        #endregion

        public GuiManager(ShaderProgram shaderProgram) {
            this.shaderProgram = shaderProgram;
            currentManagerInstance = this;
            BindVAOs();
        }

        private void BindVAOs()
        {
            #region [ Bind quad vao ]
            QuadVAO.Push(0, 2, PrimitiveData.QuadPositions, BufferTarget.ArrayBuffer);
            QuadVAO.Push(1, 2, PrimitiveData.QuadTextureCoordinates, BufferTarget.ArrayBuffer);
            QuadVAO.Create();
            #endregion
        }

        public T CreateGuiElement<T>(object[] parameters) where T : GuiElement {
            var guiElement = (T)Activator.CreateInstance(typeof(T), parameters);
            guiElements.Add(guiElement);
            return guiElement;
        }

        public static GuiManager GetCurrentManagerInstance() => currentManagerInstance;
        public ShaderProgram GetShaderProgram() => shaderProgram;
        
        public void OnKeyDown(KeyboardKeyEventArgs e) {
            foreach (var element in guiElements) element.OnKeyDown(e);
        }

        public void OnKeyUp(KeyboardKeyEventArgs e) {
            foreach (var element in guiElements) element.OnKeyUp(e);
        }

        public void OnKeyPress(KeyPressEventArgs e) {
            foreach (var element in guiElements) element.OnKeyPress(e);
        }

        public void OnMouseDown(MouseButtonEventArgs e) {
            foreach (var element in guiElements) element.OnMouseDown(e);
        }

        public void OnMouseUp(MouseButtonEventArgs e) {
            foreach (var element in guiElements) element.OnMouseUp(e);
        }

        public void OnMouseMove(MouseMoveEventArgs e) {
            foreach (var element in guiElements) element.OnMouseMove(e);
        }

        public void OnLoad(EventArgs e) {
            foreach (var element in guiElements) element.OnLoad(e);
        }

        public void OnUpdateFrame(FrameEventArgs e) {
            foreach (var element in guiElements) element.OnUpdateFrame(e);
        }

        public void OnRenderFrame(FrameEventArgs e) {
            GL.Disable(EnableCap.DepthTest);
            for (var index = guiElements.Count - 1; index >= 0; index--) {
                var element = guiElements[index];
                element.OnRenderFrame(e);
            }
            GL.Enable(EnableCap.DepthTest);
        }
    }
}