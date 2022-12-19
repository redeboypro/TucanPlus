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
        private readonly Action loadEvent;
        
        private GuiManager guiManager;
        private GuiSkin guiSkin;
        private Scene.Scene scene;

        private MeshShader meshShader;

        public Display(int width, int height, string title, Action loadEvent = null) {
            Width = width; Height = height; 
            Title = title;
            currentDisplayInstance = this;
            this.loadEvent = loadEvent;
            Run();
        }

        public static Display GetCurrent() {
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

            guiSkin = new GuiSkin();
            guiSkin.SetFont(new Texture2D("resources\\font.png"));
            guiSkin.SetBoxTexture(new Texture2D("resources\\box.png"));
            guiSkin.SetThumbTexture(new Texture2D("resources\\thumb.png"));
            guiManager = new GuiManager(guiSkin, new GuiShader());
            meshShader = new MeshShader();
            loadEvent?.Invoke();
            scene = Scene.Scene.GetCurrentScene();
            scene.OnLoad(e);
            guiManager.OnLoad(e);
            Input.OnLoad();
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            Input.OnUpdateFrame();
            scene.OnUpdateFrame(e);
            guiManager.OnUpdateFrame(e);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            meshShader.Start();
            scene.OnRenderFrame(e);
            
            guiManager.GetShaderProgram().Start();
            guiManager.OnRenderFrame(e);
            SwapBuffers();
        }
        #endregion

        #region [ Input events implementation ]
        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            scene.OnKeyPress(e);
            guiManager.OnKeyPress(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            base.OnKeyUp(e);
            scene.OnKeyUp(e);
            guiManager.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);
            scene.OnKeyDown(e);
            guiManager.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            scene.OnMouseDown(e);
            guiManager.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            scene.OnMouseUp(e);
            guiManager.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e) {
            base.OnMouseMove(e);
            scene.OnMouseMove(e);
            guiManager.OnMouseMove(e);
        }
        #endregion

        public Scene.Scene GetScene() {
            return scene;
        }
        public GuiManager GetGuiManager() {
            return guiManager;
        }
    }
}