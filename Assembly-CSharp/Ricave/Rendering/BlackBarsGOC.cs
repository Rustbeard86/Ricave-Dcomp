using System;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class BlackBarsGOC : MonoBehaviour
    {
        private void OnGUI()
        {
            Widgets.ApplySkin();
            GUI.depth = -300;
            float num = Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.timeStart, 0.3f, 2.3f, 0.3f);
            if (Clock.UnscaledTime - this.timeStart > 2.8999999f)
            {
                base.gameObject.SetActive(false);
                return;
            }
            if (num > 0f)
            {
                Rect rect = Widgets.ScreenRect.TopPartPct(0.08f * num).ExpandedBy(1f);
                Rect rect2 = Widgets.ScreenRect.BottomPartPct(0.08f * num).ExpandedBy(1f);
                GUIExtra.DrawRect(rect, Color.black);
                GUIExtra.DrawRect(rect2, Color.black);
            }
        }

        public void ShowBlackBars()
        {
            this.timeStart = Clock.UnscaledTime;
            base.gameObject.SetActive(true);
        }

        public void StopShowing()
        {
            this.timeStart = -99999f;
            base.gameObject.SetActive(false);
        }

        private float timeStart = -99999f;

        private const float FadeTime = 0.3f;

        private const float StayTime = 2.3f;

        private const float SizePct = 0.08f;
    }
}