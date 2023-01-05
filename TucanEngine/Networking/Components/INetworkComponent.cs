namespace TucanEngine.Networking.Components
{
    public interface INetworkComponent {
        void SendDataToOther(string serializedData);
    }
}