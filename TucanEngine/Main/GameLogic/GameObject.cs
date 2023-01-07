using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenTK;
using OpenTK.Input;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Common.Serialization;
using TucanEngine.Physics.Components;

namespace TucanEngine.Main.GameLogic
{
    public class GameObject : Transform, IBehaviour, IDisposable
    {
        private readonly List<Behaviour> behaviours = new List<Behaviour>();
        private readonly int layer;
        private bool isActive;
        private int index;
        private string name;
        private string tag;

        public IPhysicsComponent PhysicsComponent { get; private set; }

        public GameObject(int layer = 0) {
            this.layer = layer;
        }

        public int GetIndex() {
            return index;
        }
        
        public string GetName() {
            return name;
        }
        
        public string GetTag() {
            return tag;
        }
        
        public int GetLayer() {
            return layer;
        }

        public bool IsActive() {
            return isActive;
        }

        public void SetIndex(int index) {
            this.index = index;
        }
        
        public void SetName(string name) {
            this.name = name;
        }
        
        public void SetTag(string tag) {
            this.tag = tag;
        }

        public T GetBehaviour<T>() where T : Behaviour {
            foreach (var behaviour in behaviours
                .Where(behaviour => behaviour.GetType().IsAssignableFrom(typeof(T)))) {
                return (T) behaviour;
            }
            return null;
        }
        
        public Behaviour GetBehaviourWithInterface<T>() {
            foreach (var behaviour in behaviours
                .Where(behaviour => behaviour.GetType().GetInterface(typeof(T).Name) != null)) {
                return behaviour;
            }
            throw new Exception("Behavior cannot be found!");
        }
        
        [Obsolete("Method is deprecated, please use GetBehaviour<T>()")]
        public Behaviour GetBehaviour(Type type) {
            foreach (var behaviour in behaviours
                .Where(behaviour => behaviour.GetType().IsAssignableFrom(type))) {
                return behaviour;
            }
            throw new Exception("Behavior cannot be found!");
        }
        
        public void AddBehaviour<T>() where T : Behaviour {
            var behaviourInstance = Activator.CreateInstance<T>();
            behaviourInstance.AssignObject(this);
            behaviours.Add(behaviourInstance);
        }
        
        public void AddBehaviour(Behaviour behaviour) {
            behaviour.AssignObject(this);
            behaviours.Add(behaviour);
        }

        public void SetActive(bool isActive) {
            this.isActive = isActive;
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
                if (isActive) behaviour.OnObjectActivated(); 
                else behaviour.OnObjectDeactivated();
            }
        }

        public void OnKeyDown(KeyboardKeyEventArgs e) {
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
                behaviour.OnKeyDown(e);
            }
        }

        public void OnKeyUp(KeyboardKeyEventArgs e) {
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
                behaviour.OnKeyUp(e);
            }
        }

        public void OnKeyPress(KeyPressEventArgs e) {
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
                behaviour.OnKeyPress(e);
            }
        }

        public void OnMouseDown(MouseButtonEventArgs e) {
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
                behaviour.OnMouseDown(e);
            }
        }

        public void OnMouseUp(MouseButtonEventArgs e) {
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
                behaviour.OnMouseUp(e);
            }
        }

        public void OnMouseMove(MouseMoveEventArgs e) {
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
                behaviour.OnMouseMove(e);
            }
        }

        public void OnLoad(EventArgs e) {
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
                behaviour.OnLoad(e);
                
                if (behaviour is IPhysicsComponent physicsComponent) {
                    PhysicsComponent = physicsComponent;
                }
            }
        }

        public void OnUpdateFrame(FrameEventArgs e) {
            ((Behaviour)PhysicsComponent).OnUpdateFrame(e);
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive() && behaviour != (Behaviour)PhysicsComponent)) {
                behaviour.OnUpdateFrame(e);
            }
        }

        public void OnRenderFrame(FrameEventArgs e) {
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
                behaviour.OnRenderFrame(e);
            }
        }
        
        public void OnAwake() {
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
                behaviour.OnAwake();
            }
        }

        public GameObject Clone() {
            var objectClone = new GameObject(layer);
            objectClone.SetActive(isActive);
            objectClone.CopyFrom(this);
            objectClone.SetName(name);
            foreach (var behaviour in behaviours) {
                const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
                
                var fields = behaviour.GetType().GetFields(flags)
                    .Where(field => Attribute.IsDefined(field, typeof(SerializedField)))
                    .ToDictionary(field => field.Name, field => field.GetValue(behaviour));
                
                var behaviourClone = (Behaviour) Activator.CreateInstance(behaviour.GetType());
                var behaviourType = behaviourClone.GetType();
                
                foreach (var field in fields) {
                    behaviourType.GetField(field.Key, flags)?.SetValue(behaviourClone, field.Value);
                }
                objectClone.AddBehaviour(behaviourClone);
            }
            for (var i = 0; i < GetChildCount(); i++) {
                objectClone.AddChild(((GameObject) GetChild(i)).Clone());
            }
            return objectClone;
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}