using OpenTK;
using TucanEngine.AssimpImplementation;
using TucanEngine.Display;
using TucanEngine.Gui;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.BasicComponents;
using TucanEngine.Pooling;
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
                var guiManager = GuiManager.GetCurrentManagerInstance();

                var poolSource = new GameObject();
                var mesh = ModelLoader.LoadFromFile("ak47.obj");
                var texture = new Texture2D("ak47.png");
                var meshRenderer = new MeshRenderer();
                meshRenderer.AssignObject(poolSource);
                meshRenderer.SetMesh(mesh);
                meshRenderer.SetTexture(texture);
                poolSource.AddBehaviour(meshRenderer);
                
                var pool = new ObjectPool<GameObject>(TemporaryTag, poolSource, 10);
                scene.PushPool(pool);
                scene.FillPools();
                var instance = scene.InstantiateFromPool(TemporaryTag, new Vector3(0.8f, -0.8f, -2f), Quaternion.Identity, Vector3.One);
                var instance2 = scene.InstantiateFromPool(TemporaryTag, Vector3.UnitZ * 5 + Vector3.UnitX, Quaternion.Identity, Vector3.One);
                scene.GetCamera().AddBehaviour<FreeCameraController>();
                instance.SetParent(scene.GetCamera());

                instance.LocalSpaceRotation = Quaternion.FromEulerAngles(
                    MathHelper.DegreesToRadians(90),
                    MathHelper.DegreesToRadians(-90),
                    MathHelper.DegreesToRadians(90));
            });
        }
    }
}