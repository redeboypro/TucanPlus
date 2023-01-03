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

        private GameObject cameraGameObject;

        public override void OnLoad(EventArgs e) {
            cameraGameObject = GetAssignedObject();
            Display.Display.GetCurrentDisplay().CursorGrabbed = true;
            Display.Display.GetCurrentDisplay().CursorVisible = false;
        }

        public override void OnUpdateFrame(FrameEventArgs e) {
            if (Input.IsAnyKeyDown()) {
                cameraGameObject.WorldSpaceLocation += cameraGameObject.Forward() * (float)e.Time * MovementSpeed * Input.GetAxis("Vertical");
                cameraGameObject.WorldSpaceLocation += cameraGameObject.Right() * (float)e.Time * MovementSpeed * Input.GetAxis("Horizontal");
                
                if (Input.IsKeyDown(Key.Space)) {
                    cameraGameObject.WorldSpaceLocation += Vector3.UnitY * (float)e.Time * MovementSpeed;
                }

                if (Input.IsKeyDown(Key.ShiftLeft)) {
                    cameraGameObject.WorldSpaceLocation -= Vector3.UnitY * (float)e.Time * MovementSpeed;
                }
            }

            var pitch = Input.GetMouseDeltaY() * (float)e.Time * Sensitivity;
            var yaw = -Input.GetMouseDeltaX() * (float)e.Time * Sensitivity;

            cameraGameObject.Rotate(Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(yaw)));
            cameraGameObject.Rotate(Quaternion.FromAxisAngle(cameraGameObject.Right(), MathHelper.DegreesToRadians(pitch)));
        }
    }
}