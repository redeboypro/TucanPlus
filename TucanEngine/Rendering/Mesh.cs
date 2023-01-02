using OpenTK;
using OpenTK.Graphics.OpenGL;
using TucanEngine.Rendering.Tools.Common.Bridges;

namespace TucanEngine.Rendering
{
    public class Mesh
    {
        private GlArrayData arrayData;
        private int vertexCount;

        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }

        public Mesh(Vector3[] vertices, Vector2[] textureCoordinates, int[] indices, (Vector3, Vector3) bounds) {
            (Min, Max) = bounds;
            vertexCount = vertices.Length;
            arrayData = new GlArrayData();
            arrayData.Push(0, 3, vertices, BufferTarget.ArrayBuffer);
            arrayData.Push(1, 2, textureCoordinates, BufferTarget.ArrayBuffer);
            arrayData.Push(2, 1, indices, BufferTarget.ElementArrayBuffer);
            arrayData.Create();
        }

        public GlArrayData GetArrayData() {
            return arrayData;
        }

        public int GetVertexCount() {
            return vertexCount;
        }
    }
}