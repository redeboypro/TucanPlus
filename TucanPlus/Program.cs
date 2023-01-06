using System;
using OpenTK;
using TucanEngine.AssimpImplementation;
using TucanEngine.Display;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.BasicComponents;
using TucanEngine.Physics.Components;
using TucanEngine.Rendering;
using TucanEngine.Rendering.Components;
using TucanEngine.Scene;

namespace TucanPlus
{
    internal class Program
    {
        private const string TemporaryTag = "Temp";
        
        public static void Main(string[] args) {
            var display = new Display(800, 600, "Tucan Display", () => {
                var scene = new Scene();
                var camera = scene.GetCamera();
                
                camera.AddBehaviour<BoxComponent>();
                var box = camera.GetBehaviour<BoxComponent>();
                camera.AddBehaviour<BasicFirstPersonController>();

                var meshRenderer = new MeshRenderer();
                meshRenderer.SetMesh(ModelLoader.LoadMeshFromFile("ak47.obj"));
                meshRenderer.SetTexture(new Texture2D("ak47.png"));

                var model = new GameObject();
                model.AddBehaviour(meshRenderer);
                model.AddBehaviour<BoxComponent>();
                model.GetBehaviour<BoxComponent>().IgnoreMtd = true;
                
                scene.PushPool(TemporaryTag, model, 1);
                scene.FillPools();
                scene.InstantiateFromPool(TemporaryTag, -3 * Vector3.UnitY, Quaternion.Identity, Vector3.One);
            });
        }
    }
}