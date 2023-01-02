using System;
using OpenTK;
using TucanEngine.Main.GameLogic;

namespace TucanEngine.Physics.Shapes
{
    public class Terrain : IShape
    {
        private readonly Triangle[] triangles;

        public Terrain(Triangle[] triangles) {
            this.triangles = triangles;
        }

        public Triangle[] GetTriangles() {
            return triangles;
        }

        public void Transform(Transform transform) {
            foreach (var triangle in triangles) {
                triangle.Transform(transform);
            }
        }

        public bool Raycast(Vector3 start, Vector3 direction, out Vector3 hitPoint) {
            hitPoint = start + direction * float.PositiveInfinity;
            var intersects = false;
            foreach (var triangle in triangles) {
                intersects = triangle.Raycast(start, direction, out var unitIntersectionPoint);
                if (Vector3.Distance(unitIntersectionPoint, start)
                    < Vector3.Distance(hitPoint, start)) {
                    hitPoint = unitIntersectionPoint;
                }
            }

            if (!intersects) {
                hitPoint = start + direction;
            }
            
            return intersects;
        }
    }
}