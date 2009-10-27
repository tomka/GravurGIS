using System;
using System.Collections.Generic;
using System.Text;

namespace GravurGIS.Topology
{
    public struct Vector2
    {
        #region Fields

        public double X;
        public double Y;

        #endregion

        #region Constructors

        public Vector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        #endregion

        #region Operators

        public static Vector2 operator *(Vector2 vec, double multiplikator)
        {
            return new Vector2(vec.X * multiplikator, vec.Y * multiplikator);
        }
        public static Vector2 operator -(Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(vec1.X - vec2.X, vec1.Y - vec2.Y);
        }
        public static Vector2 operator +(Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(vec1.X + vec2.X, vec1.Y + vec2.Y);
        }
        public static Vector2 operator /(Vector2 vec1, double divisor)
        {
            return new Vector2(vec1.X / divisor, vec1.Y / divisor); ;
        }

        #endregion

        #region Methods

        public static Vector2 Add(Vector2 vec1, Vector2 vec2)
        {
            return vec1 + vec2;
        }

        public static Vector2 Divide(Vector2 vector2, double divisor)
        {
            return vector2 / divisor;
        }
        public static Vector2 Div(Vector2 a, Vector2 b) { return new Vector2(a.X / b.X, a.Y / b.Y); }

        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }
        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }
        public static Vector2 Zero()
        {
            return new Vector2(0d, 0d);
        }
        #endregion
    }
}
