using System;
using OpenTK;
using TucanEngine.Common.Math;
using TucanEngine.Main.GameLogic.Common;

namespace TucanEngine.Main.GameLogic
{
    public class Camera : GameObject
    {
        private const float NearClip = 0.01f;
        private const float FarClip = 1000.0f;
        private Matrix4 projection;

        public Camera(float fieldOfView = MathHelper.PiOver4) { 
            var display = Display.Display.GetCurrentDisplay(); 
            projection = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, display.Width / (float)display.Height, NearClip, FarClip);
        }

        public Matrix4 GetProjectionMatrix() {
            return projection;
        }

        private Vector3 GetWorldCoordinates(Vector4 eyeCoords) {
            var invertedView = GetViewMatrix().Inverted();
            var rayWorld = eyeCoords * invertedView;
            var mouseRay = new Vector3(rayWorld.X, rayWorld.Y, rayWorld.Z);
            mouseRay.Normalize();
            return mouseRay;
        }
        
        private Vector4 GetEyeCoordinates(Vector4 clipCoords) {
            var invertedProjection = projection.Inverted();
            var eyeCoords = clipCoords * invertedProjection;
            return new Vector4(eyeCoords.X, eyeCoords.Y, -1f, 0f);
        }
        
        public Vector3 RectToWorldCoordinates(float x, float y) {
            var normalizedCoords = Ortho.ToGlCoordinates((int)x, (int)y);
            var clipCoords = new Vector4(normalizedCoords.X, normalizedCoords.Y, -1.0f, 1.0f);
            var eyeCoords = GetEyeCoordinates(clipCoords);
            var worldRay = GetWorldCoordinates(eyeCoords);
            return worldRay;
        }

        public Vector3 WorldCoordinatesToRect(Vector3 vector) {
            var display = Display.Display.GetCurrentDisplay();
            var viewMatrix = GetViewMatrix();
            var worldSpaceLocation = vector.Transform(viewMatrix);
            var transformedLocation = Vector3.TransformPerspective(worldSpaceLocation, projection);
            return new Vector3((0.5f + 0.5f * transformedLocation.X) * display.Width,
                (0.5f + 0.5f * -transformedLocation.Y) * display.Height, transformedLocation.Z);
        }

        public Matrix4 GetViewMatrix() {
            return Matrix4.LookAt(WorldSpaceLocation, WorldSpaceLocation + Forward(Space.Global), Up(Space.Global)).Normalized();
        }
    }
}