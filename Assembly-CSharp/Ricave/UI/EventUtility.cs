using System;
using UnityEngine;

namespace Ricave.UI
{
    public static class EventUtility
    {
        public static Vector2 MousePositionOrCenter(this Event ev)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                return Widgets.ScreenCenter;
            }
            return ev.mousePosition;
        }
    }
}