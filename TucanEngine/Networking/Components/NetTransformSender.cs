using System;
using OpenTK;
using TucanEngine.Main.GameLogic;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Networking.Common;

namespace TucanEngine.Networking.Components
{
    public class NetTransformSender : Behaviour
    {
        private INetworkComponent networkComponentInstance;
        private GameObject gameObject;
        private float countDown;

        public override void OnLoad(EventArgs e) {
            gameObject = GetAssignedObject();
            networkComponentInstance = (INetworkComponent)gameObject.GetBehaviourWithInterface<INetworkComponent>();
            countDown = Server.Latency;
        }

        public override void OnUpdateFrame(FrameEventArgs e) {
            if (countDown > 0) {
                countDown -= (float)e.Time;
                return;
            }

            countDown = Server.Latency;
            
            var package = new Package();
            try {
                var location = gameObject.WorldSpaceLocation;
                var rotation = gameObject.WorldSpaceRotation;
                var scale = gameObject.WorldSpaceScale;
                
                package.Add(NetworkConstants.X_Location_Id, location.X);
                package.Add(NetworkConstants.Y_Location_Id, location.Y);
                package.Add(NetworkConstants.Z_Location_Id, location.Z);
                
                package.Add(NetworkConstants.X_Rotation_Id, rotation.X);
                package.Add(NetworkConstants.Y_Rotation_Id, rotation.Y);
                package.Add(NetworkConstants.Z_Rotation_Id, rotation.Z);
                package.Add(NetworkConstants.W_Rotation_Id, rotation.W);
                
                package.Add(NetworkConstants.X_Scale_Id, scale.X);
                package.Add(NetworkConstants.Y_Scale_Id, scale.Y);
                package.Add(NetworkConstants.Z_Scale_Id, scale.Z);

                networkComponentInstance.SendDataToOther(package.SerializedData);
            }
            catch (Exception ex) {
                Console.WriteLine("Unable to send transform info");
            }
            finally {
                package.Dispose();
            }
        }
    }
}