using System;
using Assimp;
using OpenTK;
using Quaternion = OpenTK.Quaternion;

namespace TucanEngine.Common.EventTranslation
{
    public enum Axis { X, Y, Z }
    public static class MathBindings
    {
        #region [ Matrix4 bindings ]
        public static Matrix4 CreateRotation(this Matrix4 matrix, float pitch, float yaw, float roll) {
            return Matrix4.CreateRotationX(pitch) + Matrix4.CreateRotationY(yaw) * Matrix4.CreateRotationZ(roll);
        }
        #endregion
        
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
        
        public static Vector3[] GetDirectionVectors(this Quaternion quaternion) {
            return GetDirectionVectors(quaternion.ToEulerAngles());
        }
        
        public static Vector3 Right(this Quaternion quaternion) {
            return quaternion * Vector3.UnitX;
        }
        
        public static Vector3 Up(this Quaternion quaternion) {
            return quaternion * Vector3.UnitY;
        }
        
        public static Vector3 Forward(this Quaternion quaternion) {
            return quaternion * Vector3.UnitZ;
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
        
        public static Vector3[] GetDirectionVectors(this Vector3 eulerAngles) {
            return CalculateDirectionVectors(eulerAngles.X, eulerAngles.Y);
        }
        
        public static Vector3[] CalculateDirectionVectors(float pitch, float yaw) {
            var directions = new []{ -Vector3.UnitZ, Vector3.UnitX, Vector3.UnitY };
            directions[0].X = (float)Math.Cos(pitch) * (float)Math.Cos(yaw);
            directions[0].Y = (float)Math.Sin(pitch);
            directions[0].Z = (float)Math.Cos(pitch) * (float)Math.Sin(yaw);
            directions[0].Normalize();
            directions[2] = Vector3.Normalize(Vector3.Cross(directions[0], Vector3.UnitY));
            directions[1] = Vector3.Normalize(Vector3.Cross(directions[2], directions[0]));
            return directions;
        }

        public static Vector3 Transform(this Vector3 vector, Matrix4 matrix) {
            var toVector4 = new Vector4(vector.X, vector.Y, vector.Z, 1.0f);
            Vector4.Transform(ref toVector4, ref matrix, out toVector4);
            return toVector4.Xyz;
        }
        #endregion
    }
}