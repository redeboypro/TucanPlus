namespace TucanEngine.Common.Drawables
{
    public static class PrimitiveData
    {
        #region [ Constants ]
        private const int GridSize = 3;
        private const float GridCellSize = 1f / GridSize;
        #endregion
        
        #region [ 3D Primitives ]
        //TODO: 3d primitive shapes
        #endregion
        
        #region [ 2D Primitives ]
        public static readonly float[] QuadPositions = { 
                -1.0f, 1.0f,
                -1.0f, -1.0f,
                1.0f, 1.0f,
                1.0f, -1.0f
            };
        public static readonly float[] QuadTextureCoordinates = { 
            0.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 0.0f,
            1.0f, 1.0f
        };
        #endregion
    }
}