using System;
using Assimp;
using OpenTK;
using Quaternion = OpenTK.Quaternion;

namespace TucanEngine.Common.EventTranslation
{
    public enum Axis { X, Y, Z }
    public static class MathBindings
    {
        #region [ Quaternion bindings ]
        public static Vector3 ToEulerAngles(this Quaternion quaternion) {
            const float edge = 0.4995f;
            var sqrtW = quaternion.W * quaternion.W;
            var sqrtX = quaternion.X * quaternion.X;
            var sqrtY = quaternion.Y * quaternion.Y;
            var sqrtZ = quaternion.Z * quaternion.Z;
            var unit = sqrtX + sqrtY + sqrtZ + sqrtW;
            var test = quaternion.X * quaternion.W - quaternion.Y * quaternion.Z;
            Vector3 temporaryEuler;

            if (test > edge * unit) {
                temporaryEuler.Y = 2.0f * (float) Math.Atan2(quaternion.Y, quaternion.X);
                temporaryEuler.X = (float) Math.PI / 2.0f;
                temporaryEuler.Z = 0.0f;
                return temporaryEuler;
            }
            
            if (test < -edge * unit) {
                temporaryEuler.Y = -2.0f * (float)Math.Atan2(quaternion.Y, quaternion.X);
                temporaryEuler.X = (float) -Math.PI / 2.0f;
                temporaryEuler.Z = 0.0f;
                return temporaryEuler;
            }
            
            temporaryEuler.Y = (float)Math.Atan2(2.0f * quaternion.X * quaternion.W + 2.0f * quaternion.Y * quaternion.Z,
                1.0f - 2.0f * (quaternion.Z * quaternion.Z + quaternion.W * quaternion.W));
            
            temporaryEuler.X = (float)Math.Asin(2.0f * (quaternion.X * quaternion.Z - quaternion.W * quaternion.Y));
            
            temporaryEuler.Z = (float)Math.Atan2(2.0f * quaternion.X * quaternion.Y + 2.0f * quaternion.Z * quaternion.W,
                1.0f - 2.0f * (quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z));
            
            return temporaryEuler;
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > Math.PI * 2.0f) {
                angle -= (float)Math.PI * 2.0f;
            }

            while (angle < 0.0f) {
                angle += (float)Math.PI * 2.0f;
            }
            
            return angle;
        }
        #endregion
        
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
        
        public static Vector3 ToOpenTK(this Vector3D vector) {
            Vector3 tmpVector;
            tmpVector.X = vector.X;
            tmpVector.Y = vector.Y;
            tmpVector.Z = vector.Z;
            return tmpVector;
        }
        #endregion
    }
}