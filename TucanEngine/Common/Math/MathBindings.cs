using System;
using OpenTK;

namespace TucanEngine.Common.EventTranslation
{
    public enum Axis { X, Y, Z }
    public static class MathBindings
    {
        #region [ Vector bindings ]
        public static Vector3 AddUnit(this Vector3 vector, float value, Axis axis) {
            switch (axis) {
                case Axis.X: vector.X += value;
                    break;
                case Axis.Y: vector.Y += value; 
                    break;
                case Axis.Z: vector.Z += value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
            return vector;
        }
        
        public static Vector3 ScaleBy(this Vector3 vector, float value, Axis axis) {
            switch (axis) {
                case Axis.X: vector.X *= value;
                    break;
                case Axis.Y: vector.Y *= value; 
                    break;
                case Axis.Z: vector.Z *= value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
            return vector;
        }
        
        public static Vector3 SetUnit(this Vector3 vector, float value, Axis axis) {
            switch (axis) {
                case Axis.X: vector.X = value;
                    break;
                case Axis.Y: vector.Y = value; 
                    break;
                case Axis.Z: vector.Z = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
            return vector;
        }
        #endregion
    }
}