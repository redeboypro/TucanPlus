using System;
using System.Collections.Generic;
using Assimp;
using OpenTK;
using Quaternion = OpenTK.Quaternion;

namespace TucanEngine.Common.Math
{
    public enum Axis { X, Y, Z }
    public static class MathBindings
    {
        public static readonly Vector3 EpsilonVector = new Vector3(float.Epsilon);
        #region [ Matrix4 bindings ]
        public static Matrix4 CreateRotation(this Matrix4 matrix, float pitch, float yaw, float roll) {
            return Matrix4.CreateRotationX(pitch) + Matrix4.CreateRotationY(yaw) * Matrix4.CreateRotationZ(roll);
        }
        #endregion

        #region [ Matrix4x4 bindings ]
        public static Vector3 MultiplyPoint3x4(this Matrix4 matrix, Vector3 vector) {
            Vector3 transformedVector;
            transformedVector.X = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14;
            transformedVector.Y = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24;
            transformedVector.Z = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34;
            return transformedVector;
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
                temporaryEuler.Y = 2.0f * (float) System.Math.Atan2(quaternion.Y, quaternion.X);
                temporaryEuler.X = (float) System.Math.PI / 2.0f;
                temporaryEuler.Z = 0.0f;
                return temporaryEuler;
            }
            
            if (test < -edge * unit) {
                temporaryEuler.Y = -2.0f * (float)System.Math.Atan2(quaternion.Y, quaternion.X);
                temporaryEuler.X = (float) -System.Math.PI / 2.0f;
                temporaryEuler.Z = 0.0f;
                return temporaryEuler;
            }
            
            temporaryEuler.Y = (float)System.Math.Atan2(2.0f * quaternion.X * quaternion.W + 2.0f * quaternion.Y * quaternion.Z,
                1.0f - 2.0f * (quaternion.Z * quaternion.Z + quaternion.W * quaternion.W));
            
            temporaryEuler.X = (float)System.Math.Asin(2.0f * (quaternion.X * quaternion.Z - quaternion.W * quaternion.Y));
            
            temporaryEuler.Z = (float)System.Math.Atan2(2.0f * quaternion.X * quaternion.Y + 2.0f * quaternion.Z * quaternion.W,
                1.0f - 2.0f * (quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z));
            
            return temporaryEuler;
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > System.Math.PI * 2.0f) {
                angle -= (float)System.Math.PI * 2.0f;
            }

            while (angle < 0.0f) {
                angle += (float)System.Math.PI * 2.0f;
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

        public static Quaternion GetLookRotation(Vector3 forward, Vector3 up) {
            forward.Normalize();
            var vec1 = Vector3.Normalize(forward);
            var vec2 = Vector3.Normalize(Vector3.Cross(up, vec1));
            var vec3 = Vector3.Cross(vec1, vec2);
            
            var m00 = vec2.X;
            var m01 = vec2.Y;
            var m02 = vec2.Z;
            
            var m10 = vec3.X;
            var m11 = vec3.Y;
            var m12 = vec3.Z;
            
            var m20 = vec1.X;
            var m21 = vec1.Y;
            var m22 = vec1.Z;

            var num8 = (m00 + m11) + m22;
            var quaternion = new Quaternion();
            if (num8 > 0.0f) {
                var num = (float)System.Math.Sqrt(num8 + 1.0f);
                quaternion.W = num * 0.5f;
                num = 0.5f / num;
                quaternion.X = (m12 - m21) * num;
                quaternion.Y = (m20 - m02) * num;
                quaternion.Z = (m01 - m10) * num;
                return quaternion;
            }

            if (m00 >= m11 && m00 >= m22) {
                var num7 = (float)System.Math.Sqrt(1.0f + m00 - m11 - m22);
                var num4 = 0.5f / num7;
                quaternion.X = 0.5f * num7;
                quaternion.Y = (m01 + m10) * num4;
                quaternion.Z = (m02 + m20) * num4;
                quaternion.W = (m12 - m21) * num4;
                return quaternion;
            }

            if (m11 > m22) { 
                var num6 = (float)System.Math.Sqrt(1.0f + m11 - m00 - m22);
                var num3 = 0.5f / num6;
                quaternion.X = (m10 + m01) * num3;
                quaternion.Y = 0.5f * num6;
                quaternion.Z = (m21 + m12) * num3;
                quaternion.W = (m20 - m02) * num3;
                return quaternion;
            }
            
            var num5 = (float)System.Math.Sqrt(1.0f + m22 - m00 - m11);
            var num2 = 0.5f / num5;
            quaternion.X = (m20 + m02) * num2;
            quaternion.Y = (m21 + m12) * num2;
            quaternion.Z = 0.5f * num5;
            quaternion.W = (m01 - m10) * num2;
            return quaternion;
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
            directions[0].X = (float)System.Math.Cos(pitch) * (float)System.Math.Cos(yaw);
            directions[0].Y = (float)System.Math.Sin(pitch);
            directions[0].Z = (float)System.Math.Cos(pitch) * (float)System.Math.Sin(yaw);
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

        public static (Vector3, Vector3) GetBounds(IEnumerable<Vector3> points) {
            var min = Vector3.One * float.PositiveInfinity;
            var max = Vector3.One * float.NegativeInfinity;
            foreach (var point in points) {
                if (point.X < min.X) {
                    min.X = point.X;
                }
                if (point.Y < min.Y) {
                    min.Y = point.Y;
                }
                if (point.Z < min.Z) {
                    min.Z = point.Z;
                }
                if (point.X > max.X) {
                    min.X = point.X;
                }
                if (point.Y > max.Y) {
                    min.Y = point.Y;
                }
                if (point.Z > max.Z) {
                    min.Z = point.Z;
                }
            }
            
            return (min, max);
        }
        #endregion
    }
}