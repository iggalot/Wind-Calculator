using System;
using System.Numerics;

namespace WindCalculator
{
    public static class ExtensionMethods
    {
        public static Vector3 Normalize(this Vector3 vec)
        {
            double mag = Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y) + (vec.Z * vec.Z));
            return new Vector3((float)(vec.X / mag), (float)(vec.Y / mag), (float)(vec.Z / mag));
        }

        public static Vector3 Cross(this Vector3 l, Vector3 r)
        {
            return new Vector3(l.Y * r.Z - l.Z * r.Y, l.Z * r.X - l.X * r.Z, l.X * r.Y - l.Y * r.X);
        }
        public static float Dot(this Vector3 l, Vector3 r)
        {
            return ((l.X * r.X + l.Y * r.Y) + l.Z * r.Z);
        }

        public static float ToRadians(this float i)
        {
            return ((float)(i * Math.PI / 180.0));
        }

        /// <summary>
        /// Builds a translation matrix by components x, y, z
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Matrix4x4 Translate(this Matrix4x4 mat, float x, float y, float z)
        {
            Matrix4x4 m  = Matrix4x4.Identity;
            m.M14 = x;
            m.M24 = y;
            m.M34 = z;

            return m;
        }

        public static Matrix4x4 ScaleBy(this Matrix4x4 mat, float val)
        {
            return Matrix4x4.Multiply(mat, val);
        }

        /// <summary>
        /// Builds a trasnlation matrix by a Vector3 vector
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Matrix4x4 Translate(this Matrix4x4 mat, Vector3 vec)
        {
            return mat.Translate(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Builds a trasnlation matrix by a Vector4 vector
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Matrix4x4 Translate(this Matrix4x4 mat, Vector4 vec)
        {
            return mat.Translate(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Multiply a Matrix4x4 by a Vector4
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="vec"></param>
        /// <returns></returns>

        public static Vector4 MatrixVectorProduct(this Matrix4x4 mat, Vector4 vec)
        {
            Vector4 v = new Vector4();

            v.X = mat.M11 * vec.X + mat.M12 * vec.Y + mat.M13 * vec.Z + mat.M14 * vec.W;
            v.Y = mat.M21 * vec.X + mat.M22 * vec.Y + mat.M23 * vec.Z + mat.M24 * vec.W;
            v.Z = mat.M31 * vec.X + mat.M32 * vec.Y + mat.M33 * vec.Z + mat.M34 * vec.W;
            v.W = mat.M41 * vec.X + mat.M42 * vec.Y + mat.M43 * vec.Z + mat.M44 * vec.W;

            return v;
        }
    }
}
