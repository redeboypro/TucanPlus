using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
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
        private Dictionary<string, Queue<GameObject>> root = new Dictionary<string, Queue<GameObject>>();

        public Scene() {
            currentScene = this;
        }

        public void PushPool(ObjectPool<GameObject> pool) {
            pools.Add(pool);
        }

        public void FillPools() {
            foreach (var pool in pools) {
                if(pool.IsContained()) continue;
                var queue = new Queue<GameObject>();
                for (var i = 0; i < pool.GetCapacity(); i++) {
                    var objectInstance = pool.GetSource().Clone();
                    objectInstance.SetActive(false);
                    objectInstance.SetIndex(queue.Count);
                    queue.Enqueue(objectInstance);
                }
                root.Add(pool.GetTag(), queue);
                pool.MarkAsContained();
            }
        }

        public GameObject InstantiateFromPool(string tag, Vector3 location, Quaternion rotation, Vector3 scale) {
            if (!root.ContainsKey(tag)) {
                Console.WriteLine($"Pool \"{tag}\" doesn't exist");
                return null;
            }
            var queue = root[tag];
            var objectInstance = queue.Dequeue();
            objectInstance.SetActive(true);
            objectInstance.WorldSpaceLocation = location;
            objectInstance.WorldSpaceRotation = rotation;
            objectInstance.WorldSpaceScale = scale;
            objectInstance.OnAwake();
            queue.Enqueue(objectInstance);
            return objectInstance;
        }

        public Queue<GameObject> FindPoolByTag(string tag) {
            return root[tag];
        }

        public void OnKeyDown(KeyboardKeyEventArgs e) {
            foreach (var obj in root
                .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                obj.OnKeyDown(e);
            }
        }

        public void OnKeyUp(KeyboardKeyEventArgs e) {
            foreach (var obj in root
                .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                obj.OnKeyUp(e);
            }
        }

        public void OnKeyPress(KeyPressEventArgs e) {
            foreach (var obj in root
                .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                obj.OnKeyPress(e);
            }
        }

        public void OnMouseDown(MouseButtonEventArgs e) {
            foreach (var obj in root
                .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                obj.OnMouseDown(e);
            }
        }

        public void OnMouseUp(MouseButtonEventArgs e) {
            foreach (var obj in root
                .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                obj.OnMouseUp(e);
            }
        }

        public void OnMouseMove(MouseMoveEventArgs e) {
            foreach (var obj in root
                .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                obj.OnMouseMove(e);
            }
        }

        public void OnLoad(EventArgs e) {
            foreach (var obj in root
                .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                obj.OnLoad(e);
            }
        }

        public void OnUpdateFrame(FrameEventArgs e) {
            foreach (var obj in root
                .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                obj.OnUpdateFrame(e);
            }
        }

        public void OnRenderFrame(FrameEventArgs e) {
            foreach (var obj in root
                .SelectMany(queue => queue.Value.Where(obj => obj.IsActive()))) {
                obj.OnRenderFrame(e);
            }
        }

        public static Scene GetCurrentScene() {
            return currentScene;
        }
    }
}