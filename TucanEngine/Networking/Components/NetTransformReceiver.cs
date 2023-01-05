using System;
using System.Collections.Generic;
using OpenTK;
using TucanEngine.Common.Math;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Networking.Common;

namespace TucanEngine.Networking.Components
{
    public class NetTransformReceiver : Behaviour
    {
        public static float InterpolationFactor { get; set; } = 0.1f;
        
        private Vector3 targetLocation = Vector3.Zero;
        private Quaternion targetRotation = Quaternion.Identity;
        private Vector3 targetScale = Vector3.One;
        
        private GameObject gameObject;
        private float interpolation;

        public override void OnLoad(EventArgs e) {
            gameObject = GetAssignedObject();
        }

        public override void OnUpdateFrame(FrameEventArgs e) {
            interpolation += (float)e.Time * InterpolationFactor;
            gameObject.WorldSpaceLocation = Vector3.Lerp(gameObject.WorldSpaceLocation, targetLocation, interpolation);
            gameObject.WorldSpaceRotation = Quaternion.Slerp(gameObject.WorldSpaceRotation, targetRotation, interpolation);
            gameObject.WorldSpaceScale = Vector3.Lerp(gameObject.WorldSpaceScale, targetScale, interpolation);
        }

        public void UpdateTarget(Dictionary<byte, object> data) {
            foreach (var dataUnit in data) {
                switch (dataUnit.Key) {
                    case NetworkConstants.X_Location_Id:
                        targetLocation.X = (float)dataUnit.Value;
                        break;
                    case NetworkConstants.Y_Location_Id:
                        targetLocation.Y = (float)dataUnit.Value;
                        break;
                    case NetworkConstants.Z_Location_Id:
                        targetLocation.Z = (float)dataUnit.Value;
                        break;
                    case NetworkConstants.X_Rotation_Id:
                        targetRotation.X = (float)dataUnit.Value;
                        break;
                    case NetworkConstants.Y_Rotation_Id:
                        targetRotation.Y = (float)dataUnit.Value;
                        break;
                    case NetworkConstants.Z_Rotation_Id:
                        targetRotation.Z = (float)dataUnit.Value;
                        break;
                    case NetworkConstants.W_Rotation_Id:
                        targetRotation.W = (float)dataUnit.Value;
                        break;
                    case NetworkConstants.X_Scale_Id:
                        targetScale.X = (float)dataUnit.Value;
                        break;
                    case NetworkConstants.Y_Scale_Id:
                        targetScale.Y = (float)dataUnit.Value;
                        break;
                    case NetworkConstants.Z_Scale_Id:
                        targetScale.Z = (float)dataUnit.Value;
                        break;
                }
                interpolation = 0.0f;
            }
        }
    }
}