using System;
using OpenTK;
using TucanEngine.Common.EventTranslation;

namespace TucanEngine.Main.GameLogic
{
    public class Camera : GameObject
    {
        private const float NearClip = 0.01f;
        private const float FarClip = 1000.0f;
        private static Camera current;
        
        private Vector3 forwardDirection = -Vector3.UnitZ;
        private Vector3 upDirection = Vector3.UnitY;
        private Vector3 rightDirection = Vector3.UnitX;

        private float pitch;
        private float yaw = MathHelper.DegreesToRadians(90);

        private Matrix4 viewMatrix;
        private Matrix4 projection;

        public Camera(float fieldOfView = MathHelper.PiOver4) {
            current = this;
            var display = Display.Display.GetCurrent();
            projection = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, display.Width / (float)display.Height, NearClip, FarClip);
            UpdateDirections();
        }

        public Matrix4 GetViewMatrix() {
            return viewMatrix;
        }
        
        public Matrix4 GetProjectionMatrix() {
            return projection;
        }

        public Vector3 GetForwardDirection() {
            return forwardDirection;
        }
        
        public Vector3 GetRightDirection() {
            return rightDirection;
        }
        
        public Vector3 GetUpDirection() {
            return upDirection;
        }

        public float Pitch {
            get => MathHelper.RadiansToDegrees(pitch);
            set {
                pitch = MathHelper.DegreesToRadians(value);
                UpdateDirections();
            }
        }
        
        public float Yaw {
            get => MathHelper.RadiansToDegrees(yaw);
            set {
                yaw = MathHelper.DegreesToRadians(value);
                UpdateDirections();
            }
        }
        
        private Vector3 GetWorldCoordinates(Vector4 eyeCoords) {
            var invertedView = viewMatrix.Inverted();
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
        
        private void UpdateDirections() {
            forwardDirection.X = (float)Math.Cos(pitch) * (float)Math.Cos(yaw);
            forwardDirection.Y = (float)Math.Sin(pitch);
            forwardDirection.Z = (float)Math.Cos(pitch) * (float)Math.Sin(yaw);
            forwardDirection.Normalize();
            rightDirection = Vector3.Normalize(Vector3.Cross(forwardDirection, Vector3.UnitY));
            upDirection = Vector3.Normalize(Vector3.Cross(rightDirection, forwardDirection));
            UpdateViewMatrix();
        }
        
        private void UpdateViewMatrix() {
            viewMatrix = Matrix4.LookAt(WorldSpaceLocation, WorldSpaceLocation + forwardDirection, upDirection);
        }

        public override void OnTransformMatrices() {
            base.OnTransformMatrices();
            UpdateViewMatrix();
        }

        public static Camera GetCurrentCameraInstance() {
            return current;
        }
    }
}