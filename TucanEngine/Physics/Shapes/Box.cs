using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using OpenTK;
using TucanEngine.Common.Math;
using TucanEngine.Main.GameLogic;
using TucanEngine.Rendering;

namespace TucanEngine.Physics.Shapes
{
    public class Box : IShape
    {
        public const int VertexCount = 8;
        
        private readonly Vector3[] vertices = new Vector3[VertexCount];
        private readonly Vector3[] sharedVertices = new Vector3[VertexCount];
        private Vector3 center = Vector3.Zero;
        private Vector3 min;
        private Vector3 max;
        private Vector3 sharedMin;
        private Vector3 sharedMax;

        public bool IgnoreRotation { get; set; }
        public Transform AssignedTransform { get; set; }

        public Box(Vector3 min, Vector3 max) {
            SetBounds(min, max);
        }
        
        public Box(float scaleFactor, Vector3 center) {
            var extents = Vector3.One * scaleFactor;
            SetBounds(center + extents, center - extents);
        }

        public Vector3 GetVertexByIndex(int index) {
            return sharedVertices[index];
        }

        public Vector3 GetMin() {
            return sharedMin;
        }
        
        public Vector3 GetMax() {
            return sharedMax;
        }
        
        public Vector3 GetCenter() {
            return center;
        }
        
        public Vector3 GetHalfExtent() {
            return sharedMax - GetCenter();
        }
        
        public void SetBounds(Vector3 min, Vector3 max) {
            this.min = sharedMin = min;
            this.max = sharedMax = max;
            ReturnToOriginalState();
        }
        
        public void SetBoundsFromMesh(Mesh mesh) {
            SetBounds(mesh.Min, mesh.Max);
        }
        
        public void ReturnToOriginalState() {
            vertices[0] = min;
            vertices[1] = new Vector3(min.X, min.Y, max.Z);
            vertices[2] = new Vector3(min.X, max.Y, min.Z);
            vertices[3] = new Vector3(max.X, min.Y, min.Z);
            vertices[4] = new Vector3(min.X, max.Y, max.Z);
            vertices[5] = new Vector3(max.X, min.Y, max.Z);
            vertices[6] = new Vector3(max.X, max.Y, min.Z);
            vertices[7] = max;
        }
        
        public void Transform(Transform transform) {
            center = transform.WorldSpaceLocation;

            if (IgnoreRotation) {
                sharedMin = min + center;
                sharedMax = max + center;
                return;
            }
            
            ReturnToOriginalState();
            
            sharedMin = Vector3.One * float.PositiveInfinity;
            sharedMax = Vector3.One * float.NegativeInfinity;

            for(var i = 0; i < VertexCount; i++) {
                sharedVertices[i] = transform.WorldSpaceRotation * (vertices[i] * transform.WorldSpaceScale) + transform.WorldSpaceLocation;
            }

            foreach (var transformedPoint in sharedVertices) {
                var tempMin = sharedMin;
                var tempMax = sharedMax;

                for (var axisIndex = 0; axisIndex < 3; axisIndex++) {
                    if (transformedPoint[axisIndex] < tempMin[axisIndex]) {
                        tempMin[axisIndex] = transformedPoint[axisIndex];
                    }
                    
                    if (transformedPoint[axisIndex] > tempMax[axisIndex]) {
                        tempMax[axisIndex] = transformedPoint[axisIndex];
                    }
                }

                sharedMin = tempMin;
                sharedMax = tempMax;
            }
        }

        public bool Raycast(Vector3 start, Vector3 direction, out Vector3 hitPoint) {
            hitPoint = start + direction;
            
            Vector3 fracDirection;
            fracDirection.X = 1.0f / direction.X;
            fracDirection.Y = 1.0f / direction.Y;
            fracDirection.Z = 1.0f / direction.Z;
            
            var distanceToMinX = (sharedMin.X - start.X) * fracDirection.X;
            var distanceToMaxX = (sharedMax.X - start.X) * fracDirection.X;
            
            var distanceToMinY = (sharedMin.Y - start.Y) * fracDirection.Y;
            var distanceToMaxY = (sharedMax.Y - start.Y) * fracDirection.Y;
            
            var distanceToMinZ = (sharedMin.Z - start.Z) * fracDirection.Z;
            var distanceToMaxZ = (sharedMax.Z - start.Z) * fracDirection.Z;

            var distanceToMin = Math.Max(Math.Max(
                    Math.Min(distanceToMinX, distanceToMaxX), 
                    Math.Min(distanceToMinY, distanceToMaxY)), 
                    Math.Min(distanceToMinZ, distanceToMaxZ));
            
            var distanceToMax = Math.Min(Math.Min(
                Math.Max(distanceToMinX, distanceToMaxX),
                Math.Max(distanceToMinY, distanceToMaxY)),
                Math.Max(distanceToMinZ, distanceToMaxZ));

            if (distanceToMax < 0) {
                return false;
            }
            
            if (distanceToMin > distanceToMax) {
                return false;
            }

            hitPoint = start + distanceToMin * direction;
            return true;
        }
    }
}