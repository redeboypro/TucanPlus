using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenTK;
using OpenTK.Input;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Serialization;

namespace TucanEngine.Main.GameLogic
{
    public class GameObject : Transform, IBehaviour, IDisposable
    {
        private readonly List<Behaviour> behaviours = new List<Behaviour>();
        private bool isActive;
        private int index;

        public int GetIndex() {
            return index;
        }

        public bool IsActive() {
            return isActive;
        }

        public void SetIndex(int index) {
            this.index = index;
        }

        public T GetBehaviour<T>() where T : Behaviour {
            foreach (var behaviour in behaviours
                .Where(behaviour => behaviour.GetType().IsAssignableFrom(typeof(T)))) {
                return (T) behaviour;
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
            }
        }

        public void OnUpdateFrame(FrameEventArgs e) {
            foreach (var behaviour in behaviours.Where(behaviour => behaviour.IsActive())) {
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
            var objectClone = new GameObject();
            objectClone.SetActive(isActive);
            objectClone.CopyFrom(this);
            foreach (var behaviour in behaviours) {
                var fields = behaviour.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(field => Attribute.IsDefined(field, typeof(SerializedField)))
                    .ToDictionary(field => field.Name, field => field.GetValue(behaviour));
                
                var behaviourClone = (Behaviour) Activator.CreateInstance(behaviour.GetType());
                foreach (var field in behaviourClone.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)) {
                    if (Attribute.IsDefined(field, typeof(SerializedField))) {
                        field.SetValue(behaviourClone, fields[field.Name]);
                    }
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