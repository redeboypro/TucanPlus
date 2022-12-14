using System;
using System.Collections.Generic;
using OpenTK;
using TucanEngine.Rendering;

namespace TucanEngine.Main.GameLogic
{
    public enum Space { Local, Global }

    public abstract class Transform {
        private Transform parent;
        private List<Transform> children = new List<Transform>();
        
        private Vector3 globalLocation = Vector3.Zero;
        private Quaternion globalRotation = Quaternion.Identity;
        private Vector3 globalScale = Vector3.One;
        
        private Vector3 localLocation = Vector3.Zero;
        private Quaternion localRotation = Quaternion.Identity;
        private Vector3 localScale = Vector3.One;

        private Matrix4 modelMatrix = Matrix4.Identity;
        
        private Vector3 min, max;

        #region [ World space transformation ]
        public Vector3 WorldSpaceLocation {
            get => globalLocation;
            set {
                globalLocation = value;
                TransformMatrices(true);
                OnMoving();
            }
        }
        
        public Quaternion WorldSpaceRotation {
            get => globalRotation;
            set {
                globalRotation = value;
                TransformMatrices(true);
                OnRotating();
            }
        }
        
        public Vector3 WorldSpaceScale {
            get => globalScale;
            set {
                globalScale = value;
                TransformMatrices(true);
                OnScaling();
            }
        }
        #endregion
        
        #region [ Local space transformation ]
        public Vector3 LocalSpaceLocation {
            get => localLocation;
            set {
                localLocation = value;
                TransformMatrices(false);
                OnMoving();
            }
        }
        
        public Quaternion LocalSpaceRotation {
            get => localRotation;
            set {
                localRotation = value;
                TransformMatrices(false);
                OnRotating();
            }
        }
        
        public Vector3 LocalSpaceScale {
            get => localScale;
            set {
                localScale = value;
                TransformMatrices(false);
                OnScaling();
            }
        }
        #endregion

        public Transform() {
            TransformMatrices(false);
        }
        
        public int GetChildCount() {
            return children.Count;
        }
        
        public Transform GetChild(int index) {
            return children[index];
        }
        
        public void RemoveChild(Transform child) {
            if (children.Contains(child)) {
                children.Remove(child);
            }
        }
        
        public void AddChild(Transform child) {
            if (children.Contains(child)) {
                return;
            }
            children.Add(child);
            child.SetParent(this);
        }

        public void SetParent(Transform parent) {
            if (this.parent != parent) {
                this.parent?.RemoveChild(this);
            }
            this.parent = parent;
            TransformMatrices(false);
            parent?.AddChild(this);
        }
        
        public Transform GetParent() {
            return parent;
        }

        public Matrix4 GetModelMatrix() {
            return modelMatrix;
        }

        public void TransformMatrices(bool inverse) {
            var parentMatrix = parent?.GetModelMatrix() ?? Matrix4.Identity;
            if (!inverse) {
                modelMatrix = Matrix4.CreateScale(localScale)
                              * Matrix4.CreateFromQuaternion(localRotation)
                              * Matrix4.CreateTranslation(localLocation) * parentMatrix;

                globalLocation = modelMatrix.ExtractTranslation();
                globalRotation = modelMatrix.ExtractRotation();
                globalScale = modelMatrix.ExtractScale();
            }
            else {
                var localMatrix = parentMatrix.Inverted() * Matrix4.CreateScale(globalScale)
                                                          * Matrix4.CreateFromQuaternion(globalRotation)
                                                          * Matrix4.CreateTranslation(globalLocation);

                LocalSpaceLocation = localMatrix.ExtractTranslation();
                LocalSpaceRotation = localMatrix.ExtractRotation();
                LocalSpaceScale = localMatrix.ExtractScale();
            }

            foreach (var child in children) {
                child.TransformMatrices(false);
            }
            
            OnTransformMatrices();
        }

        public Quaternion GetLookRotation(Vector3 target, Vector3 upDirection, Space space = Space.Global) {
            return Matrix4.LookAt(space is Space.Global ? globalLocation : localLocation, target, upDirection).ExtractRotation();
        }

        public void LookAt(Vector3 target, Vector3 upDirection, Space space = Space.Global) {
            var lookRotation = GetLookRotation(target, upDirection, space);
            switch (space) {
                case Space.Global: globalRotation = lookRotation; 
                    break;
                case Space.Local: localRotation = lookRotation; 
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(space), space, null);
            }
        }

        public virtual void OnMoving() { }
        public virtual void OnRotating() { }
        public virtual void OnScaling() { }
        public virtual void OnTransformMatrices() { }
    }
}