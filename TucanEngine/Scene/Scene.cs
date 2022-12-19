using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Pooling;

namespace TucanEngine.Scene
{
    public class Scene : IBehaviour
    {
        private static Scene currentScene;
        private List<ObjectPool<GameObject>> pools = new List<ObjectPool<GameObject>>();
        private Dictionary<string, Queue<GameObject>>[] layers;
        private Camera sceneCamera;

        public Scene(int numberOfLayers = 1) {
            currentScene = this;
            sceneCamera = new Camera();
            layers = new Dictionary<string, Queue<GameObject>>[numberOfLayers];
            for (var index = 0; index < layers.Length; index++) {
                layers[index] = new Dictionary<string, Queue<GameObject>>();
            }
        }

        public void PushPool(ObjectPool<GameObject> pool) {
            pools.Add(pool);
        }

        public void FillPools() {
            foreach (var pool in pools) {
                if(pool.IsContained()) continue;
                var queue = new Queue<GameObject>();
                var source = pool.GetSource();
                for (var i = 0; i < pool.GetCapacity(); i++) {
                    var objectInstance = source.Clone();
                    objectInstance.SetActive(false);
                    objectInstance.SetIndex(queue.Count);
                    queue.Enqueue(objectInstance);
                }
                layers[source.GetLayer()].Add(pool.GetTag(), queue);
                pool.MarkAsContained();
            }
        }

        public GameObject InstantiateFromPool(string tag, Vector3 location, Quaternion rotation, Vector3 scale, int layer = 0) {
            if (!layers[layer].ContainsKey(tag)) {
                Console.WriteLine($"Pool \"{tag}\" doesn't exist");
                return null;
            }
            var queue = layers[layer][tag];
            var objectInstance = queue.Dequeue();
            objectInstance.SetActive(true);
            objectInstance.WorldSpaceLocation = location;
            objectInstance.WorldSpaceRotation = rotation;
            objectInstance.WorldSpaceScale = scale;
            objectInstance.OnAwake();
            queue.Enqueue(objectInstance);
            return objectInstance;
        }

        public Queue<GameObject> FindPoolByTag(string tag, int layer = 0) {
            return layers[layer][tag];
        }

        public void OnKeyDown(KeyboardKeyEventArgs e) {
            sceneCamera.OnKeyDown(e);
            foreach (var layer in layers) {
                foreach (var obj in layer
                    .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                    obj.OnKeyDown(e);
                }
            }
        }

        public void OnKeyUp(KeyboardKeyEventArgs e) {
            sceneCamera.OnKeyUp(e);
            foreach (var layer in layers) {
                foreach (var obj in layer
                    .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                    obj.OnKeyUp(e);
                }
            }
        }

        public void OnKeyPress(KeyPressEventArgs e) {
            sceneCamera.OnKeyPress(e);
            foreach (var layer in layers) {
                foreach (var obj in layer
                    .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                    obj.OnKeyPress(e);
                }
            }
        }

        public void OnMouseDown(MouseButtonEventArgs e) {
            sceneCamera.OnMouseDown(e);
            foreach (var layer in layers) {
                foreach (var obj in layer
                    .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                    obj.OnMouseDown(e);
                }
            }
        }

        public void OnMouseUp(MouseButtonEventArgs e) {
            sceneCamera.OnMouseUp(e);
            foreach (var layer in layers) {
                foreach (var obj in layer
                    .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                    obj.OnMouseUp(e);
                }
            }
        }

        public void OnMouseMove(MouseMoveEventArgs e) {
            sceneCamera.OnMouseMove(e);
            foreach (var layer in layers) {
                foreach (var obj in layer
                    .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                    obj.OnMouseMove(e);
                }
            }
        }

        public void OnLoad(EventArgs e) {
            sceneCamera.OnLoad(e);
            foreach (var layer in layers) {
                foreach (var obj in layer
                    .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                    obj.OnLoad(e);
                }
            }
        }

        public void OnUpdateFrame(FrameEventArgs e) {
            sceneCamera.OnUpdateFrame(e);
            foreach (var layer in layers) {
                foreach (var obj in layer
                    .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                    obj.OnUpdateFrame(e);
                }
            }
        }

        public void OnRenderFrame(FrameEventArgs e) {
            sceneCamera.OnRenderFrame(e);
            foreach (var layer in layers) {
                GL.Clear(ClearBufferMask.DepthBufferBit);
                foreach (var obj in layer
                    .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                    obj.OnRenderFrame(e);
                }
            }
        }

        public static Scene GetCurrentScene() {
            return currentScene;
        }
        
        public Camera GetCamera() {
            return sceneCamera;
        }
    }
}