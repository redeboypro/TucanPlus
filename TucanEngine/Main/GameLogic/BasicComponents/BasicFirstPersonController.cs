using System;
using OpenTK;
using OpenTK.Input;
using TucanEngine.Common.Math;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Physics;
using TucanEngine.Physics.Components;

namespace TucanEngine.Main.GameLogic.BasicComponents
{
    public class BasicFirstPersonController : Behaviour
    {
        private static readonly Vector3 forwardCorrection = Vector3.One.SetUnit(0.0f, Axis.Y);

        private GameObject gameObject;
        private BoxComponent boxComponent;
        
        public float MovementSpeed { get; set; } = 5.0f;
        public float Sensitivity { get; set; } = 50.0f;

        public override void OnLoad(EventArgs e) {
            gameObject = GetAssignedObject();
            boxComponent = gameObject.GetBehaviour<BoxComponent>();
            boxComponent.GetBoxShape().IgnoreRotation = true;
            boxComponent.IgnoreGravity = false;
            
            Display.Display.GetCurrentDisplay().CursorGrabbed = true;
            Display.Display.GetCurrentDisplay().CursorVisible = false;
        }

        public override void OnUpdateFrame(FrameEventArgs e) {
            if (Input.IsAnyKeyDown()) {
                gameObject.LocalSpaceLocation += gameObject.Forward(Space.Local) * forwardCorrection * (float)e.Time * MovementSpeed * Input.GetAxis("Vertical");
                gameObject.LocalSpaceLocation += gameObject.Right(Space.Local) * (float)e.Time * MovementSpeed * Input.GetAxis("Horizontal");

                var (collide, face) = boxComponent.CollideOther();
                if (Input.IsKeyDown(Key.Space) && collide && face is Face.Up) {
                    boxComponent.TossUp(5);
                }
            }

            var pitch = Input.GetMouseDeltaY() * (float)e.Time * Sensitivity;
            var yaw = -Input.GetMouseDeltaX() * (float)e.Time * Sensitivity;

            gameObject.Rotate(Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(yaw)), Space.Local);
            gameObject.Rotate(Quaternion.FromAxisAngle(gameObject.Right(Space.Local), MathHelper.DegreesToRadians(pitch)), Space.Local);
        }
    }
}