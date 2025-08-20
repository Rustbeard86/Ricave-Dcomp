using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class Vector2Utility
    {
        public static float Atan2(this Vector2 from, Vector2 to)
        {
            return Calc.Atan2(to.y - from.y, to.x - from.x);
        }

        public static float Atan2Deg(this Vector2 from, Vector2 to)
        {
            return Calc.Atan2(to.y - from.y, to.x - from.x) * 57.29578f;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            return Quaternion.Euler(0f, 0f, degrees) * v;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees, Vector2 pivot)
        {
            return (v - pivot).Rotate(degrees) + pivot;
        }

        public static Vector2 MovedBy(this Vector2 v, float x, float y)
        {
            return new Vector2(v.x + x, v.y + y);
        }

        public static Vector2 WithX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        public static Vector2 WithY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        public static Vector2 WithAddedY(this Vector2 v, float offset)
        {
            return new Vector2(v.x, v.y + offset);
        }

        public static Vector2 SmoothStep(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(Calc.SmoothStep(a.x, b.x, t), Calc.SmoothStep(a.y, b.y, t));
        }

        public static Rect CenteredRect(this Vector2 at, float size)
        {
            return RectUtility.CenteredAt(at, size);
        }

        public static Rect CenteredRect(this Vector2 at, float width, float height)
        {
            return RectUtility.CenteredAt(at, width, height);
        }

        public static Rect CenteredRect(this Vector2 at, Vector2 size)
        {
            return RectUtility.CenteredAt(at, size.x, size.y);
        }

        public static Vector2 XZAngleToDirectionalVector(float angle)
        {
            return new Vector2(Calc.Cos((angle - 90f) * 0.017453292f), Calc.Sin((angle - 90f) * 0.017453292f));
        }
    }
}