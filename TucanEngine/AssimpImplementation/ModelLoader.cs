using System.Collections.Generic;
using System.Linq;
using Assimp;
using Assimp.Configs;
using OpenTK;
using TucanEngine.Common.Math;
using TucanEngine.Physics.Shapes;
using Mesh = TucanEngine.Rendering.Mesh;

namespace TucanEngine.AssimpImplementation
{
    public class ModelLoader 
    {
        public static Terrain LoadTerrainFromFile(string fileName) {
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var scene = importer.ImportFile(fileName, PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
            var mesh = scene.Meshes[0];
            var faces = mesh.Faces;
            var triangles = new List<Triangle>();
            
            foreach (var face in faces) {
                const int vertexCount = 3;
                
                var points = new Vector3[vertexCount];
                var normal = mesh.Normals[face.Indices[0]].ToOpenTK();

                for (var i = 0; i < vertexCount; i++) {
                    points[i] = mesh.Vertices[face.Indices[i]].ToOpenTK();
                }
                
                triangles.Add(new Triangle(points, normal));
            }

            return new Terrain(triangles.ToArray());
        }
        
        public static Mesh LoadMeshFromFile(string fileName) {
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var scene = importer.ImportFile(fileName, PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
            var mesh = scene.Meshes[0];
            var faces = mesh.Faces;

            var vertices = new List<Vector3>();
            var textureCoordinates = new List<Vector2>();
            var elements = new List<int>();
            
            var boundsMin = Vector3.One * float.PositiveInfinity;
            var boundsMax = Vector3.One * float.NegativeInfinity;

            foreach (var face in faces) {
                for (var i = 0; i < face.IndexCount; i++) {
                    var index = face.Indices[i];
                    var uv = mesh.TextureCoordinateChannels[0][index].ToOpenTK();
                    var position = mesh.Vertices[index].ToOpenTK();
                    
                    for (var axisIndex = 0; axisIndex < 3; axisIndex++) {
                        if (position[axisIndex] < boundsMin[axisIndex]) {
                            boundsMin[axisIndex] = position[axisIndex];
                        }
                    
                        if (position[axisIndex] > boundsMax[axisIndex]) {
                            boundsMax[axisIndex] = position[axisIndex];
                        }
                    }

                    vertices.Add(position);
                    textureCoordinates.Add(uv.Xy);
                    elements.Add(index);
                }
            }

            return new Mesh(vertices.ToArray(), textureCoordinates.ToArray(), elements.ToArray(), (boundsMin, boundsMax));
        }
    }
}