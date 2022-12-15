using System;
using OpenTK;
using TucanEngine.AssimpImplementation;
using TucanEngine.Common.EventTranslation;
using TucanEngine.Display;
using TucanEngine.Gui;
using TucanEngine.Main.GameLogic;
using TucanEngine.Pooling;
using TucanEngine.Rendering;
using TucanEngine.Rendering.Components;
using TucanEngine.Scene;

namespace TucanPlus
{
    internal class Program
    {
        private const string TemporaryTag = "Temp";
        public static void Main(string[] args)
        {
            var display = new Display(800, 600, "Tucan Display", () => {
                var scene = Scene.GetCurrentScene();
                var guiManager = GuiManager.GetCurrentManagerInstance();
                var camera = new Camera();
                
                var poolSource = new GameObject();
                var mesh = ModelLoader.LoadFromFile("test.obj");
                var texture = new Texture2D("test.png");
                var meshRenderer = new MeshRenderer();
                meshRenderer.AssignObject(poolSource);
                meshRenderer.SetMesh(mesh);
                meshRenderer.SetTexture(texture);
                poolSource.AddBehaviour(meshRenderer);
                
                var pool = new ObjectPool<GameObject>(TemporaryTag, poolSource, 10);
                scene.PushPool(pool);
                scene.FillPools();
                var instance = scene.InstantiateFromPool(TemporaryTag, Vector3.UnitZ * 5, Quaternion.Identity, Vector3.One);

                var slider = guiManager.Slider(0, 100);
                slider.LocalSpaceScale = new Vector3(0.5f, 0.05f, 1);
                slider.SetValueChangingEvent(() => {
                    instance.LocalSpaceLocation = instance.LocalSpaceLocation.SetUnit(slider.GetValue() * -.1f, Axis.X);
                });
            });
        }
    }
}