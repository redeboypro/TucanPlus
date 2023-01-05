using System;
using System.Collections;
using OpenTK;
using TucanEngine.AssimpImplementation;
using TucanEngine.Common.Math;
using TucanEngine.Display;
using TucanEngine.Gui;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.BasicComponents;
using TucanEngine.Networking.Components;
using TucanEngine.Physics.Components;
using TucanEngine.Physics.Shapes;
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
            var command = Console.ReadLine();
            var display = new Display(800, 600, "Tucan Display", () => {
                var scene = new Scene();
                var camera = scene.GetCamera();

                var poolSource = new GameObject();
                
                var mesh = ModelLoader.LoadMeshFromFile("ak47.obj");
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
                instance.SetParent(scene.GetCamera());
                
                var instance2 = scene.InstantiateFromPool(TemporaryTag, Vector3.UnitZ * 5 + Vector3.UnitX, Quaternion.Identity, Vector3.One);
                instance2.AddBehaviour<BoxComponent>();
                
                var boxComponent = instance2.GetBehaviour<BoxComponent>();
                boxComponent.IgnoreMtd = true;
                
                camera.AddBehaviour<BoxComponent>();
                
                var addressComponents = command.Split(' ')[1].Split(':');
                
                if (command.Contains("host")) {
                    camera.AddBehaviour<Server>();
                    
                    var server = camera.GetBehaviour<Server>();
                    server.Start(addressComponents[0], int.Parse(addressComponents[1]));
                    instance2.AddBehaviour<NetTransformReceiver>();

                    instance2.WorldSpaceLocation = Vector3.Zero;
                    camera.WorldSpaceLocation = Vector3.UnitZ * 5 + Vector3.UnitX;
                    
                    server.ReceiveData = data => {
                        instance2.GetBehaviour<NetTransformReceiver>().UpdateTarget(data);
                    };
                }
                else if (command.Contains("connect")) {
                    camera.AddBehaviour<FreeCameraController>();
                    camera.AddBehaviour<Client>();
                    
                    var client = camera.GetBehaviour<Client>();
                    client.Connect(addressComponents[0], int.Parse(addressComponents[1]));
                    instance2.AddBehaviour<NetTransformReceiver>();
                    
                    client.ReceiveData = data => {
                        instance2.GetBehaviour<NetTransformReceiver>().UpdateTarget(data);
                    };
                }
                
                camera.AddBehaviour<NetTransformSender>();

                instance.LocalSpaceRotation = Quaternion.FromEulerAngles(
                    MathHelper.DegreesToRadians(90),
                    MathHelper.DegreesToRadians(-90),
                    MathHelper.DegreesToRadians(90));
            });
        }
    }
}