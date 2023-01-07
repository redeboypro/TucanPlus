using System;
using OpenTK;
using TucanEngine.AssimpImplementation;
using TucanEngine.Display;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.BasicComponents;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Physics;
using TucanEngine.Physics.Components;
using TucanEngine.Rendering;
using TucanEngine.Rendering.Components;
using TucanEngine.Scene;

namespace TucanPlus
{
    internal class Program
    {
        private const string TemporaryTag = "Temp";
        private const int PlatformCount = 10;
        
        public static void Main(string[] args) {
            var display = new Display(800, 600, "Tucan Display", () => {
                var scene = new Scene();
                var camera = scene.GetCamera();

                Physics.Gravity = -10;
                
                camera.AddBehaviour<BoxComponent>();
                var box = camera.GetBehaviour<BoxComponent>();
                camera.AddBehaviour<BasicFirstPersonController>();
                camera.AddBehaviour<GameController>();

                var meshRenderer = new MeshRenderer();
                meshRenderer.SetMesh(ModelLoader.LoadMeshFromFile("grassPlatform.obj"));
                meshRenderer.SetTexture(new Texture2D("grassPlatform.png"));

                var model = new GameObject();
                model.AddBehaviour(meshRenderer);
                model.AddBehaviour<BoxComponent>();
                var platformCollision = model.GetBehaviour<BoxComponent>();
                platformCollision.IgnoreMtd = true;

                scene.PushPool(TemporaryTag, model, 10);
                scene.FillPools();

                var ran = new Random();
                for (var i = 0; i < PlatformCount; i++) {
                    scene.InstantiateFromPool(TemporaryTag, -3 * Vector3.UnitY + Vector3.UnitZ * i * 8, 
                        Quaternion.FromEulerAngles(0, MathHelper.DegreesToRadians(ran.Next(0,180)), 0), Vector3.One);
                }
            });
        }

        public class GameController : Behaviour
        {
            private GameObject gameObject;
            
            public override void OnLoad(EventArgs e) {
                base.OnLoad(e);
                gameObject = GetAssignedObject();
            }

            public override void OnUpdateFrame(FrameEventArgs e) {
                base.OnUpdateFrame(e);
                if (gameObject.WorldSpaceLocation.Y < -7) {
                    gameObject.WorldSpaceLocation = Vector3.UnitY;
                }
            }
        }
    }
}