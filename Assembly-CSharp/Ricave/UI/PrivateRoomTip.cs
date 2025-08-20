using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class PrivateRoomTip
    {
        public void OnGUI()
        {
            if (!Get.InLobby)
            {
                return;
            }
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (!this.shown && Get.NowControlledActor.Position.InPrivateRoom())
            {
                this.shown = true;
                this.startTime = Clock.UnscaledTime;
            }
            float num = Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.startTime, 1.2f, 1.8f, 1.2f);
            if (num <= 0f)
            {
                return;
            }
            Widgets.FontSizeScalable = 19;
            GUI.color = new Color(1f, 1f, 1f, num);
            Widgets.LabelCentered(new Vector2(Widgets.ScreenCenter.x, Widgets.VirtualHeight - 60f), "PrivateRoomInfo".Translate().FormattedKeyBindings(), true, null, null, false, false, false, null);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
        }

        private float startTime = -9999f;

        private bool shown;

        private const float FadeInTime = 1.2f;

        private const float StayTime = 1.8f;

        private const float FadeOutTime = 1.2f;
    }
}