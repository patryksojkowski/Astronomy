using System;
using Decimal;

namespace AstronomyCalculationsLibrary.Common
{
    /// <summary>
    /// Just a (T1 X, T2 Y) pair.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public struct Pair<T1,T2>
    {
        public T1 X;
        public T2 Y;
        public Pair(T1 x, T2 y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Two dimensional Vector of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Vector2<T>
    {
        public T X;
        public T Y;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"X:{X} Y:{Y}";
        }

        /// <summary>
        /// Determines whether two decimal Vector2 instances has close values.
        /// </summary>
        /// <param name="V1"></param>
        /// <param name="V2"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool IsClose(Vector2<decimal> V1, Vector2<decimal> V2, decimal epsilon = 1m)
        {
            return Distance(V1, V2) < epsilon;
        }

        /// <summary>
        /// Returns distances between two decimal Vector2 instances
        /// </summary>
        /// <param name="V1"></param>
        /// <param name="V2"></param>
        /// <returns></returns>
        public static decimal Distance(Vector2<decimal> V1, Vector2<decimal> V2)
        {
            return DecimalMath.Sqrt(DecimalMath.Power(V1.X - V2.X, 2) + DecimalMath.Power(V1.Y - V2.Y, 2));
        }

    }

    /// <summary>
    /// Three dimensional Vector of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Vector3<T>
    {
        public T X;
        public T Y;
        public T Z;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"X:{X} Y:{Y} Z:{Z}";
        }

        /// <summary>
        /// Determines whether two decimal Vector3 instances has close values.
        /// </summary>
        /// <param name="V1"></param>
        /// <param name="V2"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool IsClose(Vector3<decimal> V1, Vector3<decimal> V2, decimal epsilon = 1m)
        {
            return Distance(V1, V2) < epsilon;
        }

        /// <summary>
        /// Calculate Euclidean metric for 3D points
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static decimal Distance(Vector3<decimal> v1, Vector3<decimal> v2)
        {
            return DecimalMath.Sqrt(DecimalMath.Power(v1.X - v2.X, 2) + DecimalMath.Power(v1.Y - v2.Y, 2) + DecimalMath.Power(v1.Z - v2.Z, 2));
        }

    }
}
