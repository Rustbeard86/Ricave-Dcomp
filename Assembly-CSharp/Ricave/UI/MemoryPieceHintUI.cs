using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class MemoryPieceHintUI
    {
        private float Alpha
        {
            get
            {
                return Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.startShowingTime, 0.6f, 2.7f, 4f);
            }
        }

        public bool AllowedToShow
        {
            get
            {
                return this.canShow;
            }
            set
            {
                this.canShow = value;
            }
        }

        public bool IsShowing
        {
            get
            {
                return Clock.UnscaledTime - this.startShowingTime < 7.3f;
            }
        }

        private EntitySpec MemoryPieceHere
        {
            get
            {
                if (Get.RunConfig.ProgressDisabled)
                {
                    return null;
                }
                if (Get.PlaceSpec != Get.Place_Normal)
                {
                    return null;
                }
                if (Get.Floor == 3 && Get.Quest_MemoryPiece1.IsActive())
                {
                    return Get.Entity_MemoryPiece1;
                }
                if (Get.Floor == 4 && Get.Quest_MemoryPiece2.IsActive())
                {
                    return Get.Entity_MemoryPiece2;
                }
                if (Get.Floor == 6 && Get.Quest_MemoryPiece3.IsActive())
                {
                    return Get.Entity_MemoryPiece3;
                }
                if (Get.Floor == 8 && Get.Quest_MemoryPiece4.IsActive())
                {
                    return Get.Entity_MemoryPiece4;
                }
                return null;
            }
        }

        private void StartShowing()
        {
            this.startShowingTime = Clock.UnscaledTime + 0.75f;
        }

        public void OnGUI()
        {
            this.DrawIfShowing();
        }

        public void CheckStartShowing()
        {
            if (!this.canShow || this.IsShowing)
            {
                return;
            }
            if (this.MemoryPieceHere == null)
            {
                return;
            }
            this.StartShowing();
        }

        private void DrawIfShowing()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (!this.IsShowing)
            {
                return;
            }
            float alpha = this.Alpha;
            if (alpha <= 0f)
            {
                return;
            }
            EntitySpec memoryPieceHere = this.MemoryPieceHere;
            if (memoryPieceHere == null)
            {
                return;
            }
            Vector2 vector = Widgets.ScreenCenter.WithY(Widgets.VirtualHeight - 299f);
            float num = 0.65f + Calc.PulseUnscaled(2.8f, 0.35f);
            GUI.color = memoryPieceHere.IconColorAdjusted.WithAlphaFactor(alpha * num);
            GUI.DrawTexture(RectUtility.CenteredAt(vector.MovedBy(0f, -78f), 80f, 80f), memoryPieceHere.IconAdjusted);
            GUI.color = Color.white;
            GUI.color = new Color(0.9f, 0.9f, 0.53f, alpha).MultipliedColor(num);
            Widgets.FontSizeScalable = 32;
            Widgets.FontBold = true;
            Widgets.LabelCentered(vector, "MemoryPieceHereHint".Translate(), true, null, null, false, true, false, null);
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            Widgets.FontSizeScalable = 26;
            Widgets.LabelCentered(vector.WithAddedY(48f), "MemoryPieceHereHint2".Translate(), true, null, null, false, true, false, null);
            Widgets.ResetFontSize();
            GUI.color = Color.white;
        }

        private float startShowingTime = -99999f;

        private bool canShow;

        private const float Duration_FadingIn = 0.6f;

        private const float Duration = 2.7f;

        private const float Duration_FadingOut = 4f;
    }
}