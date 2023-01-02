using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TucanEngine.Main.GameLogic;
using TucanEngine.Physics.Shapes;

namespace TucanEngine.Physics
{
    public interface IShape {
        void Transform(Transform transform);
        bool Raycast(Vector3 start, Vector3 direction, out Vector3 hitPoint);
    }
    
    public static class Physics
    {
        private static readonly List<IShape> shapes = new List<IShape>();

        public static void Add(IShape shape) {
            shapes.Add(shape);
        }
        
        public static IShape GetShapeByIndex(int index) {
            return shapes[index];
        }
        
        public static int GetShapeCount() {
            return shapes.Count;
        }

        public static bool BoxBoxIntersection(Box boxA, Box boxB, out Vector3 minTranslation) {
            var aCenter = boxA.GetCenter();
            var bCenter = boxB.GetCenter();
            var distance = bCenter - aCenter;
            minTranslation = Vector3.Zero;
            
            distance.X = Math.Abs(distance.X);
            distance.Y = Math.Abs(distance.Y);
            distance.Z = Math.Abs(distance.Z);
		
            distance -= boxA.GetHalfExtent() + boxB.GetHalfExtent();
            
            if (distance.X < 0 && distance.Y < 0 && distance.Z < 0) {
                var correctionDistance = boxB.GetCenter() - boxA.GetCenter();
                
                if (distance.X > distance.Y && distance.X > distance.Z) {
                    minTranslation.X = distance.X * Math.Sign(correctionDistance.X);
                }

                if(distance.Y > distance.X && distance.Y > distance.Z){
                    minTranslation.Y = distance.Y * Math.Sign(correctionDistance.Y);
                }
                
                if (distance.Z > distance.Y && distance.Z > distance.X){
                    minTranslation.Z = distance.Z * Math.Sign(correctionDistance.Z);
                }
                return true;
            }

            return false;
        }

        public static bool BoxTriangleIntersection(Box box, Triangle triangle, out Vector3 minTranslation) {
            const int vertexCount = Box.VertexCount;
            minTranslation = Vector3.Zero;

            for (var a = 0; a < vertexCount; a++) {
                var aPoint = box.GetVertexByIndex(a);
                var isInsideProjection = triangle.IsPointInsideTriangleProjection(aPoint);
                var boxIsIntersectsTriangle = false;

                if (isInsideProjection) {
                    var trianglePointProjectionHeight = GetPointHeightInsideTriangle(triangle, aPoint);
                    var planeNormalSignMultiplier = Math.Sign(aPoint.Y - trianglePointProjectionHeight);

                    for (var b = 0; b < vertexCount; b++) {
                        
                        if (b == a) {
                            continue;
                        }
                        
                        var bPoint = box.GetVertexByIndex(b);

                        var rayStart = planeNormalSignMultiplier >= 0 ? aPoint : bPoint;
                        var rayDirection = (aPoint - bPoint).Normalized() * planeNormalSignMultiplier;
                        
                        var lineIsIntersectsTriangle = RaycastPlane(rayStart, rayDirection, triangle[0], triangle.GetNormal(), out var hitPoint);
                        
                        if (lineIsIntersectsTriangle) {
                            boxIsIntersectsTriangle = true;
                            var unitTestMinTranslation = aPoint - hitPoint;
                            if (unitTestMinTranslation.Length > minTranslation.Length) {
                                minTranslation = unitTestMinTranslation;
                            }
                        }
                        break;
                    }
                }
                return boxIsIntersectsTriangle;
            }
            
            return false;
        }

        public static bool BoxTerrainIntersection(Box box, Terrain terrain, out Vector3 minTranslation) {
            minTranslation = Vector3.Zero;
            var intersects = false;
            
            foreach (var triangle in terrain.GetTriangles()) {
                if (BoxTriangleIntersection(box, triangle, out var unitTestMinTranslation)) {
                    intersects = true;
                    if (unitTestMinTranslation.Length > minTranslation.Length) {
                        minTranslation = unitTestMinTranslation;
                    }
                }
            }

            return intersects;
        }
        
        public static float GetPointHeightInsideTriangle(Triangle triangle, Vector3 point) {
            var a = -(triangle[2].Z * triangle[1].Y
                      - triangle[0].Z * triangle[1].Y
                      - triangle[2].Z * triangle[0].Y
                      + triangle[0].Y * triangle[1].Z
                      + triangle[2].Y * triangle[0].Z
                      - triangle[1].Z * triangle[2].Y);

            var b = triangle[0].Z * triangle[2].X
                     + triangle[1].Z * triangle[0].X
                     + triangle[2].Z * triangle[1].X 
                     - triangle[1].Z * triangle[2].X 
                     - triangle[0].Z * triangle[1].X 
                     - triangle[2].Z * triangle[0].X;

            var c = triangle[1].Y * triangle[2].X
                     + triangle[0].Y * triangle[1].X
                     + triangle[2].Y * triangle[0].X
                     - triangle[0].Y * triangle[2].X
                     - triangle[1].Y * triangle[0].X
                     - triangle[1].X * triangle[2].Y;

            var d = 
                - a * triangle[0].X
                - b * triangle[0].Y
                - c * triangle[0].Z;

            return -(a * point.X + c * point.Z + d) / b;
        }

        public static bool RaycastPlane(Vector3 start, Vector3 direction, Vector3 planeOrigin, Vector3 planeNormal, out Vector3 hitPoint) {
            const float minimalDotBetweenDirections = 0.0001f;
            var planeNormalDotRayDirection = Vector3.Dot(planeNormal, direction);
            hitPoint = start + direction;
            
            if (!(Math.Abs(planeNormalDotRayDirection) > minimalDotBetweenDirections)) {
                return false;
            }
            
            var distanceToIntersectionPoint = Vector3.Dot(planeOrigin - start, planeNormal) / planeNormalDotRayDirection;
            return distanceToIntersectionPoint >= 0;
        }

        public static bool Raycast(Vector3 start, Vector3 direction, out (Vector3, IShape) hitInfo) {
            hitInfo = (start + direction, null);
            var intersects = false;
            
            foreach (var shape in shapes) {
                if (shape.Raycast(start, direction, out var hitPoint)) {
                    intersects = true;
                    if (Vector3.Distance(start, hitPoint) < Vector3.Distance(start, hitInfo.Item1)) {
                        hitInfo = (hitPoint, shape);
                    }
                }
            }

            return intersects;
        }
    }
}