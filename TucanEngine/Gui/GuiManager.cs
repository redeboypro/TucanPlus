using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TucanEngine.Common.Drawables;
using TucanEngine.Main.GameLogic;
using TucanEngine.Rendering;
using TucanEngine.Rendering.Tools;
using TucanEngine.Rendering.Tools.Common.Bridges;

namespace TucanEngine.Gui
{
// ReSharper disable InconsistentNaming
    public class GuiManager : IBehaviour
    {
        private static GuiManager currentManagerInstance;
        private GuiSkin skin;
        private List<GuiElement> guiElements = new List<GuiElement>();
        
        private readonly ShaderProgram shaderProgram;
        public readonly GlArrayData QuadVAO = new GlArrayData();

        public GuiManager(GuiSkin skin, ShaderProgram shaderProgram) {
            this.skin = skin;
            this.shaderProgram = shaderProgram;
            currentManagerInstance = this;
            BindVAOs();
        }

        private void BindVAOs() {
            QuadVAO.Push(0, 2, PrimitiveData.QuadPositions, BufferTarget.ArrayBuffer);
            QuadVAO.Push(1, 2, PrimitiveData.QuadTextureCoordinates, BufferTarget.ArrayBuffer);
            QuadVAO.Create();
        }

        public Image2D Image(Texture2D textureData, bool isStretched = false) {
            var guiElement = new Image2D(textureData, isStretched);
            guiElements.Add(guiElement);
            return guiElement;
        }
        
        public Slider Slider(float min, float max, Orientation orientation = Orientation.Horizontal) {
            var guiElement = new Slider(min, max, orientation, skin);
            guiElement.GetThumb().LocalSpaceLocation = -Vector3.UnitX;
            guiElements.Add(guiElement);
            return guiElement;
        }
        
        public Image2D Button(GuiElement content, Action e) {
            var guiElement = Image(skin.GetThumbTexture(), true);
            content.SetParent(guiElement);
            guiElement.AddPressEvent(e);
            return guiElement;
        }
        
        public Text2D Text(string text) {
            var guiElement = new Text2D(text);
            guiElements.Add(guiElement);
            return guiElement;
        }

        public static GuiManager GetCurrentManagerInstance() {
            return currentManagerInstance;
        }

        public GuiSkin GetSkin() {
            return skin;
        }

        public ShaderProgram GetShaderProgram() {
            return shaderProgram;
        }

        public void OnKeyDown(KeyboardKeyEventArgs e) {
            foreach (var element in guiElements) element?.OnKeyDown(e);
        }

        public void OnKeyUp(KeyboardKeyEventArgs e) {
            foreach (var element in guiElements) element?.OnKeyUp(e);
        }

        public void OnKeyPress(KeyPressEventArgs e) {
            foreach (var element in guiElements) element?.OnKeyPress(e);
        }

        public void OnMouseDown(MouseButtonEventArgs e) {
            foreach (var element in guiElements) element?.OnMouseDown(e);
        }

        public void OnMouseUp(MouseButtonEventArgs e) {
            foreach (var element in guiElements) element?.OnMouseUp(e);
        }

        public void OnMouseMove(MouseMoveEventArgs e) {
            foreach (var element in guiElements) element?.OnMouseMove(e);
        }

        public void OnLoad(EventArgs e) {
            foreach (var element in guiElements) element?.OnLoad(e);
        }

        public void OnUpdateFrame(FrameEventArgs e) {
            foreach (var element in guiElements) element?.OnUpdateFrame(e);
        }

        public void OnRenderFrame(FrameEventArgs e) {
            GL.Disable(EnableCap.DepthTest);
            for (var index = guiElements.Count - 1; index >= 0; index--) {
                var element = guiElements[index];
                
                if (element != null && element.GetParent() == null) {
                    element.OnRenderFrame(e);
                    element.OnPostRender(e);
                }
            }
            GL.Enable(EnableCap.DepthTest);
        }
    }
}