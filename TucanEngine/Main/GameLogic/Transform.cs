using System;
using System.Collections.Generic;
using OpenTK;
using TucanEngine.Common.Math;

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
        
        public Vector3 WorldSpaceEulerAngles {
            get => globalRotation.ToEulerAngles();
            set {
                WorldSpaceRotation = Quaternion.FromEulerAngles(value);
                TransformMatrices(false);
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
                localRotation = value.Normalized();
                TransformMatrices(false);
                OnRotating();
            }
        }
        
        public Vector3 LocalSpaceEulerAngles {
            get => localRotation.ToEulerAngles();
            set {
                localRotation = Quaternion.FromEulerAngles(value);
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

            var location = globalLocation;
            var rotation = globalRotation;
            var scale = globalScale;
            this.parent = parent;
            TransformMatrices(false);
            
            globalLocation = location;
            globalRotation = rotation;
            globalScale = scale;
            TransformMatrices(true);
            
            parent?.AddChild(this);
        }
        
        public Transform GetParent() {
            return parent;
        }

        public Matrix4 GetModelMatrix() {
            return modelMatrix;
        }

        public Matrix4 GetParentMatrix() {
            switch (parent) {
                case null:
                    return Matrix4.Identity;
                case Camera camera:
                    return camera.GetViewMatrix().Inverted();
                default: 
                    return parent.GetModelMatrix();
            }
        }
        
        public Vector3 Forward(Space space) {
            switch (space) {
                case Space.Global:
                    return WorldSpaceRotation.Forward();
                case Space.Local: 
                    return LocalSpaceRotation.Forward();
                default: throw new Exception("Unknown space");
            }
        }
        
        public Vector3 Up(Space space) {
            switch (space) {
                case Space.Global:
                    return WorldSpaceRotation.Up();
                case Space.Local: 
                    return LocalSpaceRotation.Up();
                default: throw new Exception("Unknown space");
            }
        }
        
        public Vector3 Right(Space space) {
            switch (space) {
                case Space.Global:
                    return WorldSpaceRotation.Right();
                case Space.Local: 
                    return LocalSpaceRotation.Right();
                default: throw new Exception("Unknown space");
            }
        }

        public void TransformMatrices(bool inverse) {
            var parentMatrix = GetParentMatrix();
            if (!inverse) {
                modelMatrix = Matrix4.CreateScale(localScale)
                              * Matrix4.CreateFromQuaternion(localRotation)
                              * Matrix4.CreateTranslation(localLocation) * parentMatrix;

                globalLocation = modelMatrix.ExtractTranslation();
                globalRotation = modelMatrix.ExtractRotation();
                globalScale = modelMatrix.ExtractScale();
            }
            else {
                var localMatrix = Matrix4.CreateScale(globalScale)
                                                          * Matrix4.CreateFromQuaternion(globalRotation)
                                                          * Matrix4.CreateTranslation(globalLocation) * parentMatrix.Inverted();

                localLocation = localMatrix.ExtractTranslation();
                localRotation = localMatrix.ExtractRotation();
                localRotation.Normalize();
                localScale = localMatrix.ExtractScale();
                TransformMatrices(false);
            }
            
            UpdateChildren();
            OnTransformMatrices();
        }

        public void UpdateChildren() {
            foreach (var child in children) {
                child.TransformMatrices(false);
            }
        }

        public void CopyFrom(Transform transform) {
            parent = transform.parent;
            LocalSpaceLocation = transform.LocalSpaceLocation;
            LocalSpaceRotation = transform.LocalSpaceRotation;
            LocalSpaceScale = transform.LocalSpaceScale;
        }

        public void LookAt(Vector3 target, Vector3 up) {
            if (target == Vector3.Zero) {
                target = MathBindings.EpsilonVector;
            }
            WorldSpaceRotation = MathBindings.GetLookRotation((target - WorldSpaceLocation).Normalized(), up);
        }

        public void Rotate(Quaternion rotation, Space space = Space.Global) {
            switch (space) {
                case Space.Global:
                    WorldSpaceRotation = rotation * globalRotation;
                    break;
                case Space.Local: 
                    LocalSpaceRotation = rotation * localRotation;
                    break;
                default: throw new Exception("Unknown space");
            }
        }
        
        public void Rotate(float angle, Vector3 axis, Space space = Space.Global) {
            Rotate(Quaternion.FromAxisAngle(axis, angle), space);
        }
        
        public void Move(float deltaX, float deltaY, float deltaZ) {
            Move(new Vector3(deltaX, deltaY, deltaZ));
        }
        
        public void Move(Vector3 delta) {
            LocalSpaceLocation += delta;
        }

        public virtual void OnMoving() { }
        public virtual void OnRotating() { }
        public virtual void OnScaling() { }
        public virtual void OnTransformMatrices() { }
    }
}