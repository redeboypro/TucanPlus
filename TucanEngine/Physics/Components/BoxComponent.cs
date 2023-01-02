using System;
using System.Diagnostics;
using OpenTK;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Physics.Shapes;

namespace TucanEngine.Physics.Components
{
    public class BoxComponent : Behaviour
    {
        private Box boxShape;
        private GameObject gameObject;
        
        [Obsolete("Physics: Ignoring the pair collision (crutch implementation)")]
        public bool IgnoreMtd { get; set; }
        
        [Obsolete("Physics: Flag is deprecated")]
        public bool EnableAABBCollisionDebugging { get; set; }

        public Box GetBoxShape() {
            return boxShape;
        }

        public void SetBounds(Vector3 min, Vector3 max) {
            boxShape = new Box(min, max);
            Transform();
        }

        public override void OnLoad(EventArgs e) {
            gameObject = GetAssignedObject();
            var halfExtent = gameObject.WorldSpaceScale;
            SetBounds(-halfExtent, halfExtent);
            Transform();
            Physics.Add(boxShape);
        }

        public override void OnUpdateFrame(FrameEventArgs e)
        {
            if (boxShape == null || IgnoreMtd) {
                return;
            }
            Transform();

            for (var i = 0; i < Physics.GetShapeCount(); i++) {
                var collisionShape = Physics.GetShapeByIndex(i);
                if(collisionShape == boxShape) continue;
                
                switch (collisionShape.GetType().Name) {
                    case nameof(Box) :
                        if (Physics.BoxBoxIntersection(boxShape, (Box) collisionShape, out var boxBoxMinTranslation)) {
                            gameObject.WorldSpaceLocation += boxBoxMinTranslation;
                            var collisionBox = (Box)collisionShape;
                            if(EnableAABBCollisionDebugging) { 
                                Console.Write($@"
Collision:
  A:
    Min:{boxShape.GetMin()}
    Max:{boxShape.GetMax()}

  B:
    Min:{collisionBox.GetMin()}
    Max:{collisionBox.GetMax()}
");
                            }
                        }
                        break;
                    case nameof(Triangle) :
                        if (Physics.BoxTriangleIntersection(boxShape, (Triangle) collisionShape, out var boxTriangleMinTranslation)) {
                            gameObject.WorldSpaceLocation += boxTriangleMinTranslation;
                        }
                        break;
                    case nameof(Terrain) :
                        if (Physics.BoxTerrainIntersection(boxShape, (Terrain) collisionShape, out var boxTerrainMinTranslation)) {
                            gameObject.WorldSpaceLocation += boxTerrainMinTranslation;
                        }
                        break;
                }
            }
        }

        private void Transform() {
            boxShape.Transform(gameObject);
        }
    }
}