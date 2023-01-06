using System;
using OpenTK;
using TucanEngine.Main.GameLogic;
using TucanEngine.Common.Math;

namespace TucanEngine.Physics.Shapes
{
    public class Triangle : IShape
    {
        public const int VertexCount = 3;
        
        private readonly Vector3[] vertices;
        private readonly Vector3[] sharedVertices;
        private Vector3 normal;
        
        public Transform AssignedTransform { get; set; }

        public Triangle(Vector3[] vertices, Vector3 normal) {
            this.normal = normal;
            this.vertices = vertices;
            sharedVertices = (Vector3[]) vertices.Clone();
        }

        public Vector3 this[int index] => vertices[index];

        public Vector3 GetVertexByIndex(int index) {
            return sharedVertices[index];
        }
        
        public Vector3 GetNormal() {
            return normal;
        }

        public bool IsPointInsideTriangleProjection(Vector3 point) {
            var velocity0 = (vertices[0].X - vertices[2].X) * (point.Z - vertices[2].Z) - (vertices[0].Z - vertices[2].Z) * (point.X - vertices[2].X);
            var velocity1 = (vertices[1].X - vertices[0].X) * (point.Z - vertices[0].Z) - (vertices[1].Z - vertices[0].Z) * (point.X - vertices[0].X);

            if (velocity0 < 0 != velocity1 < 0 && velocity0 != 0 && velocity1 != 0) {
                return false;
            }

            var resultVelocity = (vertices[2].X - vertices[1].X) * (point.Y - vertices[1].Y) - (vertices[2].Y - vertices[1].Y) * (point.X - vertices[1].X);
            return resultVelocity == 0 || resultVelocity < 0 == velocity0 + velocity1 <= 0;
        }

        public void Transform(Transform transform) {
            for (var i = 0; i < VertexCount; i++) {
                sharedVertices[i] = transform.WorldSpaceRotation * (vertices[i] * transform.WorldSpaceScale) + transform.WorldSpaceLocation;;
            }
            
            var triangleEdge1 = sharedVertices[1] - sharedVertices[0];
            var triangleEdge2 = sharedVertices[2] - sharedVertices[0];
            
            normal.X = triangleEdge1.Y * triangleEdge2.Z - triangleEdge1.Z * triangleEdge2.Y;
            normal.Y = triangleEdge1.Z * triangleEdge2.X - triangleEdge1.X * triangleEdge2.Z;
            normal.Z = triangleEdge1.X * triangleEdge2.Y - triangleEdge1.Y * triangleEdge2.X;
        }

        public bool Raycast(Vector3 start, Vector3 direction, out Vector3 hitPoint) {

            var normalDotRayDirection = Vector3.Dot(normal, direction);
            hitPoint = start + direction;
            
            if (Math.Abs(normalDotRayDirection) < float.Epsilon) {
                return false;
            }

            var normalDotPoint = -Vector3.Dot(normal, vertices[0]);
            var distanceToHitPoint = -(Vector3.Dot(normal, start) + normalDotPoint) / normalDotRayDirection;

            if (distanceToHitPoint < 0) {
                return false;
            }

            hitPoint = start + distanceToHitPoint * direction;

            var triangleEdge1 = vertices[1] - vertices[0];
            var rayTriangleDelta1 = hitPoint - vertices[0];
            var crossProduct = Vector3.Cross(triangleEdge1, rayTriangleDelta1);
            if (Vector3.Dot(normal, crossProduct) < 0) {
                return false;
            }

            var triangleEdge2 = vertices[2] - vertices[1];
            var rayTriangleDelta2 = hitPoint - vertices[1];
            crossProduct = Vector3.Cross(triangleEdge2, rayTriangleDelta2);
            if (Vector3.Dot(normal, crossProduct) < 0) {
                return false;
            }

            var triangleEdge3 = vertices[0] - vertices[2];
            var rayTriangleDelta3 = hitPoint - vertices[2];
            crossProduct = Vector3.Cross(triangleEdge3, rayTriangleDelta3);
            return !(Vector3.Dot(normal, crossProduct) < 0);
        }
    }
}