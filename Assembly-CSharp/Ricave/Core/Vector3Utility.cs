using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public static class Vector3Utility
    {
        public static Vector3 WithAddedY(this Vector3 v, float offset)
        {
            return new Vector3(v.x, v.y + offset, v.z);
        }

        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 RotateY(this Vector3 v, float degrees)
        {
            return Quaternion.Euler(0f, degrees, 0f) * v;
        }

        public static Vector3 RotateY(this Vector3 v, float degrees, Vector3 pivot)
        {
            return (v - pivot).RotateY(degrees) + pivot;
        }

        public static float ToXZAngle(this Vector3 v)
        {
            if (v.x == 0f && v.z == 0f)
            {
                return 0f;
            }
            return Calc.Atan2(v.x, v.z) * 57.29578f;
        }

        public static Quaternion QuatFromCamera(this Vector3 v)
        {
            return Quaternion.LookRotation(v - Get.CameraPosition).normalized;
        }

        public static Vector3 Multiply(this Vector3 v, Vector3 other)
        {
            return new Vector3(v.x * other.x, v.y * other.y, v.z * other.z);
        }

        public static float GetAngleXZFromCamera_Simple(Vector3 target)
        {
            Vector3 cameraPosition = Get.CameraPosition;
            if (cameraPosition.x != target.x || cameraPosition.z != target.z)
            {
                return (target - cameraPosition).ToXZAngle() - Get.CameraTransform.rotation.ToXZAngle();
            }
            if (target.y >= cameraPosition.y)
            {
                return 0f;
            }
            return 180f;
        }

        public static float GetAngleXZFromCamera_Accurate(Vector3 targetPosition)
        {
            Vector3 vector = Get.Camera.WorldToScreenPoint(targetPosition) / Widgets.UIScale;
            vector.y = Widgets.VirtualHeight - vector.y;
            Vector2 vector2 = new Vector2(vector.x, vector.y) - Widgets.ScreenCenter;
            if (vector2 == Vector2.zero)
            {
                return 0f;
            }
            vector2.Normalize();
            if (vector.z <= 0f)
            {
                vector2 = -vector2;
            }
            return Calc.Atan2(vector2.y, vector2.x) * 57.29578f + 90f;
        }

        public static Vector3 LerpWithDeltaTime(Vector3 a, Vector3 b, float lerpSpeed)
        {
            return Vector3.Lerp(a, b, 1f - Calc.Exp(-lerpSpeed * Clock.DeltaTime));
        }
    }
}