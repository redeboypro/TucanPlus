using System.Collections.Generic;
using Assimp;
using Assimp.Configs;
using OpenTK;
using TucanEngine.Common.EventTranslation;
using Mesh = TucanEngine.Rendering.Mesh;

namespace TucanEngine.AssimpImplementation
{
    public class ModelLoader 
    {
        public static Mesh LoadFromFile(string filename)
        {
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var scene = importer.ImportFile(filename, PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
            var mesh = scene.Meshes[0];
            var faces = mesh.Faces;

            var vertices = new List<Vector3>();
            var textureCoordinates = new List<Vector2>();
            var elements = new List<int>();

            foreach (var face in faces) {
                for (var i = 0; i < face.IndexCount; i++) {
                    var index = face.Indices[i];
                    var uv = mesh.TextureCoordinateChannels[0][index].ToOpenTK();
                    var position = mesh.Vertices[index].ToOpenTK();

                    vertices.Add(position);
                    textureCoordinates.Add(uv.Xy);
                    elements.Add(index);
                }
            }

            return new Mesh(vertices.ToArray(), textureCoordinates.ToArray(), elements.ToArray());
        }
    }
}