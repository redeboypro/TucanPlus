using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TucanEngine.Gui;
using TucanEngine.Main;
using TucanEngine.Main.GameLogic;
using TucanEngine.Scene;
using TucanEngine.Rendering;

namespace TucanEngine.Display
{
    public class Display : GameWindow
    {
        private static Display currentDisplayInstance;

        private readonly string title;
        private readonly Action loadEvent;
        private GuiManager currentGuiManager;
        private Scene.Scene currentScene;
        private MeshShader meshShader;
        private float currentFrameTime = 0.0f;
        
        public int FramesPerSecond { get; set; }

        public Display(int width, int height, string title, Action loadEvent = null) {
            Width = width; Height = height; 
            this.title = title;
            currentDisplayInstance = this;
            this.loadEvent = loadEvent;
            VSync = VSyncMode.Off;
            Run();
        }

        public static Display GetCurrentDisplay() {
            return currentDisplayInstance;
        }
        
        #region [ Game loop events implementation ]
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            #region [ Enable caps ]
            {
                GL.Enable(EnableCap.Blend);
                GL.Enable(EnableCap.CullFace);
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.DepthTest);
            }
            #endregion
            
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            meshShader = new MeshShader();
            loadEvent?.Invoke();
            currentScene = Scene.Scene.GetCurrentScene();
            currentScene?.OnLoad(e);
            currentGuiManager = GuiManager.GetCurrentManagerInstance();
            currentGuiManager?.OnLoad(e);
            Input.OnLoad();
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            
            currentFrameTime += (float)e.Time;
            FramesPerSecond++;
            if (currentFrameTime >= 1.0f) {
                Title = $"{title} FPS:{FramesPerSecond}";
                currentFrameTime = 0.0f;
                FramesPerSecond = 0;
            }
            
            Input.OnUpdateFrame();
            currentScene?.OnUpdateFrame(e);
            currentGuiManager?.OnUpdateFrame(e);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            meshShader.Start();
            currentScene?.OnRenderFrame(e);
            currentGuiManager?.GetShaderProgram().Start();
            currentGuiManager?.OnRenderFrame(e);
            SwapBuffers();
        }
        #endregion

        #region [ Input events implementation ]
        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            currentScene?.OnKeyPress(e);
            currentGuiManager?.OnKeyPress(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            base.OnKeyUp(e);
            currentScene?.OnKeyUp(e);
            currentGuiManager?.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);
            currentScene?.OnKeyDown(e);
            currentGuiManager?.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            currentScene?.OnMouseDown(e);
            currentGuiManager?.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            currentScene?.OnMouseUp(e);
            currentGuiManager?.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e) {
            base.OnMouseMove(e);
            currentScene?.OnMouseMove(e);
            currentGuiManager?.OnMouseMove(e);
        }
        #endregion
    }
}