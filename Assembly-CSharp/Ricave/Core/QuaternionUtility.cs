using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class QuaternionUtility
    {
        public static Vector3Int ToCardinalDir(this Quaternion quaternion)
        {
            return (quaternion * Vector3.forward).ToCardinalVector3IntDir();
        }

        public static Quaternion CardinalDirToQuaternion(this Vector3Int cardinalDir)
        {
            if (!cardinalDir.IsCardinalDir())
            {
                Log.Warning("Called CardinalDirToQuaternion() using an argument which is not a cardinal direction.", false);
                return Quaternion.identity;
            }
            if (cardinalDir.z == 1)
            {
                return Quaternion.identity;
            }
            if (cardinalDir.z == -1)
            {
                return QuaternionUtility.Back;
            }
            if (cardinalDir.x == -1)
            {
                return QuaternionUtility.Left;
            }
            if (cardinalDir.x == 1)
            {
                return QuaternionUtility.Right;
            }
            if (cardinalDir.y == 1)
            {
                return QuaternionUtility.Up;
            }
            return QuaternionUtility.Down;
        }

        public static float ToXZAngle(this Quaternion quaternion)
        {
            return (quaternion * Vector3.forward).ToXZAngle();
        }

        public static Quaternion SlerpWithDeltaTime(Quaternion a, Quaternion b, float lerpSpeed)
        {
            return Quaternion.Slerp(a, b, 1f - Calc.Exp(-lerpSpeed * Clock.DeltaTime));
        }

        public static readonly Quaternion Identity = Quaternion.identity;

        public static readonly Quaternion Forward = QuaternionUtility.Identity;

        public static readonly Quaternion Back = Quaternion.Euler(0f, 180f, 0f);

        public static readonly Quaternion Left = Quaternion.Euler(0f, 270f, 0f);

        public static readonly Quaternion Right = Quaternion.Euler(0f, 90f, 0f);

        public static readonly Quaternion Up = Quaternion.Euler(270f, 0f, 0f);

        public static readonly Quaternion Down = Quaternion.Euler(90f, 0f, 0f);
    }
}