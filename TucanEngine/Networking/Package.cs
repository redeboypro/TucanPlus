using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TucanEngine.Networking
{
    public delegate void ReceiveDataEvent(Dictionary<byte, object> data);
    
    public class Package : IDisposable {
        private bool isDisposed;
        public string SerializedData { get; private set; }

        public void Add(byte id, object obj) {
            var objectType = obj.GetType();
            switch (obj) {
                case int asInteger:
                case float asSingle:
                case string asString:
                    SerializedData += $"{id} {obj.GetType().FullName} {obj}\n"; 
                    break;
            }
        }

        public static Dictionary<byte, object> Deserialize(string data) {
            var deserializedData = new Dictionary<byte, object>();
            foreach (var serializedUnit in data.Split('\n')) {
                if (!serializedUnit.Any(char.IsLetter)) {
                    continue;
                }
                var separatedUnitData = serializedUnit.Split(' ');
                var dataType = Type.GetType(separatedUnitData[1]);
                object unit = null;

                if (dataType is null) {
                    continue;
                }
                
                if (dataType.IsAssignableFrom(typeof(int))) {
                    unit = int.Parse(separatedUnitData[2]);
                }
                else if (dataType.IsAssignableFrom(typeof(float))) {
                    unit = float.Parse(separatedUnitData[2]);
                }
                else if (dataType.IsAssignableFrom(typeof(string))) {
                    unit = separatedUnitData[2].Trim();
                }

                var key = byte.Parse(separatedUnitData[0].Trim());
                
                if (unit != null && !deserializedData.ContainsKey(key)) {
                    deserializedData.Add(key, unit);
                }
            }

            return deserializedData;
        }
            
        protected virtual void Dispose(bool disposing) {
            if (!isDisposed) {
                if (disposing) {
                    SerializedData = null;
                }
                isDisposed = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}