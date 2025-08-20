using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class WorldRectToUIRectUtility
    {
        public static bool GetUIRect(Vector3 worldPos, Vector2 sizeInWorldUnits, out Rect uiRect)
        {
            Vector3 vector = Get.Camera.WorldToScreenPoint(worldPos) / Widgets.UIScale;
            if (vector.z <= 0f)
            {
                uiRect = default(Rect);
                return false;
            }
            Vector3 up = Get.CameraTransform.up;
            Vector3 vector2 = worldPos + up * sizeInWorldUnits.y / 2f;
            Vector3 vector3 = worldPos - up * sizeInWorldUnits.y / 2f;
            Vector3 vector4 = Get.Camera.WorldToScreenPoint(vector2) / Widgets.UIScale;
            Vector3 vector5 = Get.Camera.WorldToScreenPoint(vector3) / Widgets.UIScale;
            if (vector4.z <= 0f || vector5.z <= 0f)
            {
                uiRect = default(Rect);
                return false;
            }
            vector4.y = Widgets.VirtualHeight - vector4.y;
            vector5.y = Widgets.VirtualHeight - vector5.y;
            vector.y = Widgets.VirtualHeight - vector.y;
            float num = vector5.y - vector4.y;
            float num2 = num * (sizeInWorldUnits.x / sizeInWorldUnits.y);
            uiRect = RectUtility.CenteredAt(vector, num2, num);
            return true;
        }
    }
}