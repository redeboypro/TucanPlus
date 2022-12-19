using OpenTK;
using TucanEngine.Main.GameLogic;
using TucanEngine.Common.Math;

namespace TucanEngine.Physics.Shapes
{
    public class Triangle
    {
        private readonly Vector3[] vertices;
        private readonly Vector3[] sharedVertices;

        public Vector3 GetVertexByIndex(int index) {
            return sharedVertices[index];
        }

        public Triangle(Vector3[] vertices) {
            this.vertices = vertices;
            sharedVertices = (Vector3[]) vertices.Clone();
        }
        
        public void Transform(Transform transform) {
            for (var i = 0; i < 3; i++) {
                sharedVertices[i] = vertices[i].Transform(transform.GetModelMatrix());
            }
        }
    }
}