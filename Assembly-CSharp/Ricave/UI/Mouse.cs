using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class Mouse
    {
        public static bool Over(Rect rect)
        {
            return rect.Contains(Event.current.MousePositionOrCenter()) && Window.Current == Get.WindowManager.GetWindowAt(Event.current.MousePositionOrCenter().GUIToScreenPoint()) && Widgets.VisibleInScrollView(Event.current.MousePositionOrCenter());
        }

        public static bool OverCircle(Vector2 center, float radius)
        {
            return (Event.current.MousePositionOrCenter() - center).sqrMagnitude <= radius * radius && Window.Current == Get.WindowManager.GetWindowAt(Event.current.MousePositionOrCenter().GUIToScreenPoint()) && Widgets.VisibleInScrollView(Event.current.MousePositionOrCenter());
        }

        public static void SetCursorLocked(bool locked)
        {
            bool flag = Cursor.lockState == CursorLockMode.Locked;
            if (locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (flag != locked)
            {
                Get.Tooltips.OnCursorLockStateChanged();
                Get.DragAndDrop.OnCursorLockStateChanged();
            }
        }
    }
}