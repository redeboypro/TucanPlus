using System;
using OpenTK;
using OpenTK.Input;
using TucanEngine.Main.GameLogic.Common;

namespace TucanEngine.Main.GameLogic.BasicComponents
{
    public class FreeCameraController : Behaviour
    {
        private const float MovementSpeed = 5.0f;
        private const float Sensitivity = 10.0f;

        private float pitch;
        private float yaw;

        private GameObject cameraGameObject;

        public override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Display.Display.GetCurrent().CursorGrabbed = true;
            cameraGameObject = GetAssignedObject();
            Display.Display.GetCurrent().CursorVisible = false;
        }

        public override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            if (Input.IsAnyKeyDown()) {
                if (Input.IsKeyDown(Key.W)) {
                    cameraGameObject.WorldSpaceLocation += cameraGameObject.Forward() * (float)e.Time * MovementSpeed;
                }

                if (Input.IsKeyDown(Key.S)) {
                    cameraGameObject.WorldSpaceLocation -= cameraGameObject.Forward() * (float)e.Time * MovementSpeed;
                }

                if (Input.IsKeyDown(Key.A)) {
                    cameraGameObject.WorldSpaceLocation += cameraGameObject.Right() * (float)e.Time * MovementSpeed;
                }

                if (Input.IsKeyDown(Key.D)) {
                    cameraGameObject.WorldSpaceLocation -= cameraGameObject.Right() * (float)e.Time * MovementSpeed;
                }
            }

            pitch += MathHelper.DegreesToRadians(Input.GetMouseDeltaY() * (float)e.Time * Sensitivity);
            yaw += MathHelper.DegreesToRadians(-Input.GetMouseDeltaX() * (float)e.Time * Sensitivity);
            var pitchQuaternion = Quaternion.FromAxisAngle(cameraGameObject.Right(), pitch);
            var yawQuaternion = Quaternion.FromAxisAngle(Vector3.UnitY, yaw);
            cameraGameObject.WorldSpaceRotation = pitchQuaternion * yawQuaternion;
        }
    }
}