using System;
using OpenTK;
using TucanEngine.AssimpImplementation;
using TucanEngine.Main.GameLogic.Common;
using TucanEngine.Physics.Shapes;

namespace TucanEngine.Physics.Components
{
    public class TerrainComponent : Behaviour, IPhysicsComponent
    {
        private Terrain terrainShape;

        public void SetTerrainShape(Terrain terrain) {
            if (terrainShape != null && Physics.Contains(terrainShape)) {
                Physics.Remove(terrainShape);
            }
            terrainShape = terrain;
            terrainShape.AssignedTransform = GetAssignedObject();
            Physics.Add(terrainShape);
        }
        
        public void LoadFromFile(string fileName) {
            SetTerrainShape(ModelLoader.LoadTerrainFromFile(fileName));
        }
    }
}