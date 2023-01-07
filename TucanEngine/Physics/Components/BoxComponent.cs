using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTK;
using TucanEngine.Common.Serialization;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Physics.Shapes;
using TucanEngine.Rendering.Components;

namespace TucanEngine.Physics.Components
{
    public delegate void MtdCorrectionEvent(Transform transform, Face direction);
    
    public class BoxComponent : Behaviour, IPhysicsComponent
    {
        private readonly List<IShape> contacts = new List<IShape>();
        private Box boxShape;
        private GameObject gameObject;
        private bool isGrounded;
        private float fallingVelocity;

        [SerializedField]
        public bool IgnoreGravity = true;
        
        [SerializedField]
        public bool IgnoreMtd;
        
        public MtdCorrectionEvent MtdCorrection { get; set; }
        public MtdCorrectionEvent CollisionEnter { get; set; }
        public MtdCorrectionEvent CollisionExit { get; set; }

        public bool IsGrounded() {
            return isGrounded;
        }

        public Box GetBoxShape() {
            return boxShape;
        }

        public void SetBounds(Vector3 min, Vector3 max) {
            boxShape = new Box(min, max);
            Transform();
        }

        public void TossUp(float force) {
            if (!IgnoreGravity) {
                fallingVelocity = force;
            }
        }

        public override void OnLoad(EventArgs e) {
            gameObject = GetAssignedObject();
            
            var scale = gameObject.WorldSpaceScale;
            var (min, max) = (-scale * 0.5f, scale * 0.5f);
            
            var meshRenderer = gameObject.GetBehaviour<MeshRenderer>();
            if (meshRenderer != null) {
                var mesh = meshRenderer.GetMesh();
                (min, max) = (mesh.Min, mesh.Max);
            }

            SetBounds(min, max);
            boxShape.AssignedTransform = gameObject;
            
            Transform();
            Physics.Add(boxShape);
        }

        public override void OnUpdateFrame(FrameEventArgs e) {
            if (boxShape == null || IgnoreMtd) {
                return;
            }
            Transform();

            isGrounded = false;

            if (!IgnoreGravity) {
                fallingVelocity += Physics.Gravity * (float)e.Time;
                gameObject.Move(0, fallingVelocity * (float)e.Time, 0);
            }

            var currentContacts = new List<IShape>();
            for (var i = 0; i < Physics.GetShapeCount(); i++) {
                var collisionShape = Physics.GetShapeByIndex(i);
                if (collisionShape == boxShape || !((GameObject)collisionShape.AssignedTransform).IsActive()) {
                    continue;
                }
                
                switch (collisionShape) {
                    case Box collisionBox:
                        if (Physics.BoxBoxIntersection(boxShape, collisionBox, out var boxBoxMinTranslation, out var translationDirection)) {
                            gameObject.WorldSpaceLocation += boxBoxMinTranslation;
                            
                            currentContacts.Add(collisionBox);
                            if (!contacts.Contains(collisionBox)) {
                                CollisionEnter?.Invoke(collisionBox.AssignedTransform, translationDirection);
                                contacts.Add(collisionBox);
                            }

                            if (translationDirection is Face.Up) {
                                fallingVelocity = 0.0f;
                                isGrounded = true;
                            }
                            
                            MtdCorrection?.Invoke(collisionBox.AssignedTransform, translationDirection);
                        }
                        break;
                    case Triangle collisionTriangle :
                        if (Physics.BoxTriangleIntersection(boxShape, collisionTriangle, out var boxTriangleMinTranslation)) {
                            gameObject.WorldSpaceLocation += boxTriangleMinTranslation;
                            
                            currentContacts.Add(collisionTriangle);
                            if (!contacts.Contains(collisionTriangle)) {
                                CollisionEnter?.Invoke(collisionTriangle.AssignedTransform, Face.Up);
                                contacts.Add(collisionTriangle);
                            }
                            
                            fallingVelocity = 0.0f;
                            isGrounded = true;
                            MtdCorrection?.Invoke(collisionTriangle.AssignedTransform, Face.Up);
                        }
                        break;
                    case Terrain collisionTerrain :
                        if (Physics.BoxTerrainIntersection(boxShape, collisionTerrain, out var boxTerrainMinTranslation)) {
                            gameObject.WorldSpaceLocation += boxTerrainMinTranslation;
                            
                            currentContacts.Add(collisionTerrain);
                            if (!contacts.Contains(collisionTerrain)) {
                                CollisionEnter?.Invoke(collisionTerrain.AssignedTransform, Face.Up);
                                contacts.Add(collisionTerrain);
                            }
                            
                            fallingVelocity = 0.0f;
                            isGrounded = true;
                            MtdCorrection?.Invoke(collisionTerrain.AssignedTransform, Face.Up);
                        }
                        break;
                }
            }
            
            foreach (var contact in contacts.ToArray()) {
                if (!currentContacts.Contains(contact)) {
                    CollisionExit?.Invoke(contact.AssignedTransform, Face.None);
                    contacts.Remove(contact);
                }
            }
        }

        private void Transform() {
            boxShape.Transform(gameObject);
        }
    }
}