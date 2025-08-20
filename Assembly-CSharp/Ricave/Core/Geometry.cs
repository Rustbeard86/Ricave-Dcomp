using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class Geometry
    {
        public static Vector2? GetLineIntersection(Vector2 fromA, Vector2 toA, Vector2 fromB, Vector2 toB)
        {
            double num = (double)fromA.x;
            double num2 = (double)fromA.y;
            double num3 = (double)toA.x;
            double num4 = (double)toA.y;
            double num5 = (double)fromB.x;
            double num6 = (double)fromB.y;
            double num7 = (double)toB.x;
            double num8 = (double)toB.y;
            if (Math.Abs(num - num3) < 0.0010000000474974513 && Math.Abs(num5 - num7) < 0.0010000000474974513 && Math.Abs(num - num5) < 0.0010000000474974513)
            {
                return null;
            }
            if (Math.Abs(num2 - num4) < 0.0010000000474974513 && Math.Abs(num6 - num8) < 0.0010000000474974513 && Math.Abs(num2 - num6) < 0.0010000000474974513)
            {
                return null;
            }
            if (Math.Abs(num - num3) < 0.0010000000474974513 && Math.Abs(num5 - num7) < 0.0010000000474974513)
            {
                return null;
            }
            if (Math.Abs(num2 - num4) < 0.0010000000474974513 && Math.Abs(num6 - num8) < 0.0010000000474974513)
            {
                return null;
            }
            double num11;
            double num12;
            if (Math.Abs(num - num3) < 0.0010000000474974513)
            {
                double num9 = (num8 - num6) / (num7 - num5);
                double num10 = -num9 * num5 + num6;
                num11 = num;
                num12 = num10 + num9 * num;
            }
            else if (Math.Abs(num5 - num7) < 0.0010000000474974513)
            {
                double num13 = (num4 - num2) / (num3 - num);
                double num14 = -num13 * num + num2;
                num11 = num5;
                num12 = num14 + num13 * num5;
            }
            else
            {
                double num15 = (num4 - num2) / (num3 - num);
                double num16 = -num15 * num + num2;
                double num17 = (num8 - num6) / (num7 - num5);
                double num18 = -num17 * num5 + num6;
                num11 = (num16 - num18) / (num17 - num15);
                num12 = num18 + num17 * num11;
                if (Math.Abs(-num15 * num11 + num12 - num16) >= 0.0010000000474974513 || Math.Abs(-num17 * num11 + num12 - num18) >= 0.0010000000474974513)
                {
                    return null;
                }
            }
            if (Geometry.IsInsideLine(fromA, toA, num11, num12) && Geometry.IsInsideLine(fromB, toB, num11, num12))
            {
                return new Vector2?(new Vector2((float)num11, (float)num12));
            }
            return null;
        }

        public static ValueTuple<Vector2?, Vector2?> GetLineEllipseIntersection(Vector2 lineA, Vector2 lineB, Rect ellipse)
        {
            lineA -= ellipse.center;
            lineB -= ellipse.center;
            Rect rect = default(Rect);
            rect.xMin = Math.Min(lineA.x, lineB.x);
            rect.xMax = Math.Max(lineA.x, lineB.x);
            rect.yMin = Math.Min(lineA.y, lineB.y);
            rect.yMax = Math.Max(lineA.y, lineB.y);
            float num = (lineB.y - lineA.y) / (lineB.x - lineA.x);
            float num2 = lineB.y - num * lineB.x;
            float num3 = ellipse.width / 2f;
            float num4 = ellipse.height / 2f;
            float num5 = num4 * num4 + num3 * num3 * num * num;
            float num6 = 2f * num3 * num3 * num2 * num;
            float num7 = num3 * num3 * num2 * num2 - num3 * num3 * num4 * num4;
            float num8 = Calc.Sqrt(num6 * num6 - 4f * num5 * num7);
            Vector2 vector;
            vector.x = (-num6 - num8) / (2f * num5);
            Vector2 vector2;
            vector2.x = (-num6 + num8) / (2f * num5);
            vector.y = num * vector.x + num2;
            vector2.y = num * vector2.x + num2;
            if (rect.Contains(vector))
            {
                if (rect.Contains(vector2))
                {
                    return new ValueTuple<Vector2?, Vector2?>(new Vector2?(vector + ellipse.center), new Vector2?(vector2 + ellipse.center));
                }
                return new ValueTuple<Vector2?, Vector2?>(new Vector2?(vector + ellipse.center), null);
            }
            else
            {
                if (rect.Contains(vector2))
                {
                    return new ValueTuple<Vector2?, Vector2?>(new Vector2?(vector2 + ellipse.center), null);
                }
                return new ValueTuple<Vector2?, Vector2?>(null, null);
            }
        }

        private static bool IsInsideLine(Vector2 from, Vector2 to, double x, double y)
        {
            return ((x >= (double)from.x && x <= (double)to.x) || (x >= (double)to.x && x <= (double)from.x)) && ((y >= (double)from.y && y <= (double)to.y) || (y >= (double)to.y && y <= (double)from.y));
        }

        public static Vector3 ClosestCubeSide(Vector3 cubeCenter, Vector3 cubeSize, Vector3 point)
        {
            float num;
            return Geometry.ClosestCubeSide(cubeCenter, cubeSize, point, out num);
        }

        public static Vector3 ClosestCubeSide(Vector3 cubeCenter, Vector3 cubeSize, Vector3 point, out float dist)
        {
            float num = Math.Abs(point.x - (cubeCenter.x + cubeSize.x / 2f));
            float num2 = Math.Abs(point.x - (cubeCenter.x - cubeSize.x / 2f));
            float num3 = Math.Abs(point.z - (cubeCenter.z + cubeSize.z / 2f));
            float num4 = Math.Abs(point.z - (cubeCenter.z - cubeSize.z / 2f));
            float num5 = Math.Abs(point.y - (cubeCenter.y + cubeSize.y / 2f));
            float num6 = Math.Abs(point.y - (cubeCenter.y - cubeSize.y / 2f));
            float num7 = Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(num, num2), num3), num4), num5), num6);
            dist = num7;
            if (num7 == num)
            {
                return Vector3.right;
            }
            if (num7 == num2)
            {
                return Vector3.left;
            }
            if (num7 == num3)
            {
                return Vector3.forward;
            }
            if (num7 == num4)
            {
                return Vector3.back;
            }
            if (num7 == num5)
            {
                return Vector3.up;
            }
            return Vector3.down;
        }

        public static float GetDistanceToLineFrom(Vector2 lineA, Vector2 lineB, Vector2 point)
        {
            Vector2 vector = lineB - lineA;
            float num = Vector2.Dot(point - lineA, vector) / vector.sqrMagnitude;
            num = Calc.Clamp01(num);
            return (lineA + vector * num - point).magnitude;
        }
    }
}