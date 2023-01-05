using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TucanEngine.Common.Serialization
{
    public class SerializeUtil
    {
        public static byte[] SerializeToByteArray(object obj) {
            if (obj is null) { 
                return null;
            }
            
            var binaryFormatter = new BinaryFormatter();
            
            using (var memoryStream = new MemoryStream()) {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
        
        public static object DeserializeFromByteArray(byte[] param) {
            using (var memoryStream = new MemoryStream(param)) {
                var binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}