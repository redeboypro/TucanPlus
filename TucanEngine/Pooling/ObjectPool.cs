using System;
using TucanEngine.Main.GameLogic;

namespace TucanEngine.Pooling
{
    public class ObjectPool<T>
    {
        private readonly string tag;
        private readonly T source;
        private readonly int capacity;
        private bool isContained;

        public void MarkAsContained() {
            isContained = true;
        }

        public bool IsContained() {
            return isContained;
        }

        public ObjectPool(string tag, T source, int capacity) {
            this.tag = tag;
            this.source = source;
            this.capacity = capacity;
        }

        public string GetTag() {
            return tag;
        }
        
        public T GetSource() {
            return source;
        }
        
        public int GetCapacity() {
            return capacity;
        }
    }
}