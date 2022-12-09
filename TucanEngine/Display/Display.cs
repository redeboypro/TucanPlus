using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TucanEngine.Gui;
using TucanEngine.Rendering;
using TucanEngine.Main.GameLogic;

namespace TucanEngine.Display
{
    public class Display : GameWindow
    {
        private static Display currentDisplayInstance;
        private GuiManager guiManager;
        private GuiSkin guiSkin;

        public Display() {
            Width = 800; Height = 600; 
            Title = "TucanPlus (Game Display)";
            currentDisplayInstance = this;
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
            guiSkin.SetFont(new Texture2D("font.png"));
            guiSkin.SetBoxTexture(new Texture2D("test.png"));
            guiSkin.SetThumbTexture(new Texture2D("btn.png"));
            guiManager = new GuiManager(guiSkin, new GuiShader());
            
            var slider = guiManager.Slider(30, 100, Orientation.Vertical);
            slider.LocalSpaceScale = new Vector3(0.5f, 0.1f, 1f);
            slider.SetMovingEvent(args => {
                Console.WriteLine("Slider value:" + slider.GetValue());
            });
            slider.LocalSpaceRotation = Quaternion.FromEulerAngles(0,0,MathHelper.DegreesToRadians(90));

            guiManager.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            guiManager.OnUpdateFrame(e);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            guiManager.GetShaderProgram().Start();
            guiManager.OnRenderFrame(e);
            SwapBuffers();
        }
        #endregion

        #region [ Input events implementation ]
        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            guiManager.OnKeyPress(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            base.OnKeyUp(e);
            guiManager.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);
            guiManager.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            guiManager.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            guiManager.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e) {
            base.OnMouseMove(e);
            guiManager.OnMouseMove(e);
        }
        #endregion
    }
}