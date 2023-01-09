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
        private Box boxShape;
        private GameObject gameObject;
        private (bool, Face) collideOther;
        private float fallingVelocity;

        [SerializedField]
        public bool IgnoreGravity = true;
        
        [SerializedField]
        public bool IgnoreMtd;
        
        public MtdCorrectionEvent MtdCorrection { get; set; }
        public MtdCorrectionEvent CollisionEnter { get; set; }
        public MtdCorrectionEvent CollisionExit { get; set; }

        public (bool, Face) CollideOther() {
            return collideOther;
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
            Transform();
            
            if (boxShape == null || IgnoreMtd) {
                return;
            }

            if (!IgnoreGravity) {
                fallingVelocity += Physics.Gravity * (float)e.Time;
                gameObject.Move(0, fallingVelocity * (float)e.Time, 0);
            }
            
            var tempCollision = (false, Face.None);
            for (var i = 0; i < Physics.GetShapeCount(); i++) {
                var collisionShape = Physics.GetShapeByIndex(i);
                if (collisionShape == boxShape || !((GameObject)collisionShape.AssignedTransform).IsActive()) {
                    continue;
                }
                switch (collisionShape) {
                    case Box collisionBox:
                        if (Physics.BoxBoxIntersection(boxShape, collisionBox, out var boxBoxMinTranslation, out var translationDirection)) {
                            gameObject.LocalSpaceLocation += boxBoxMinTranslation;

                            if (translationDirection is Face.Up) {
                                fallingVelocity = 0.0f;
                            }

                            if (!collideOther.Item1) {
                                CollisionEnter?.Invoke(collisionBox.AssignedTransform, translationDirection);
                            }
                            
                            tempCollision = (true, translationDirection);
                            MtdCorrection?.Invoke(collisionBox.AssignedTransform, translationDirection);
                        }
                        break;
                    case Triangle collisionTriangle :
                        if (Physics.BoxTriangleIntersection(boxShape, collisionTriangle, out var boxTriangleMinTranslation)) {
                            gameObject.LocalSpaceLocation += boxTriangleMinTranslation;
                            
                            if (!collideOther.Item1) {
                                CollisionEnter?.Invoke(collisionTriangle.AssignedTransform, Face.Up);
                            }
                            
                            fallingVelocity = 0.0f;
                            tempCollision = (true, Face.Up);
                            MtdCorrection?.Invoke(collisionTriangle.AssignedTransform, Face.Up);
                        }
                        break;
                    case Terrain collisionTerrain :
                        if (Physics.BoxTerrainIntersection(boxShape, collisionTerrain, out var boxTerrainMinTranslation)) {
                            gameObject.LocalSpaceLocation += boxTerrainMinTranslation;
                            
                            if (!collideOther.Item1) {
                                CollisionEnter?.Invoke(collisionTerrain.AssignedTransform, Face.Up);
                            }
                            
                            fallingVelocity = 0.0f;
                            tempCollision = (true, Face.Up);
                            MtdCorrection?.Invoke(collisionTerrain.AssignedTransform, Face.Up);
                        }
                        break;
                }
            }

            if (!tempCollision.Item1 && collideOther.Item1) {
                CollisionExit?.Invoke(null, collideOther.Item2);
            }
            
            collideOther = tempCollision;
        }

        private void Transform() {
            boxShape.Transform(gameObject);
        }
    }
}