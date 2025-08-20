using System;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class BossSlainText
    {
        public bool IsShowing
        {
            get
            {
                return !this.bossName.NullOrEmpty() && Clock.UnscaledTime - this.startTime < 10f;
            }
        }

        public void ShowTextFor(string bossName)
        {
            this.bossName = bossName;
            this.startTime = Clock.UnscaledTime;
            SoundHandle.DisposeIfNotNull(ref this.soundHandle);
            this.soundHandle = Get.Sound_BossSlain.PlayWithHandle(null, false);
            Get.MusicManager.ForceSilenceFor(6f);
            Get.AmbientSoundsManager.ForceSilenceFor(6f);
        }

        public void StopShowingText()
        {
            this.bossName = null;
            SoundHandle.DisposeIfNotNull(ref this.soundHandle);
            Get.BlackBars.StopShowing();
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            SoundHandle.DisposeIfFinished(ref this.soundHandle);
            if (this.bossName.NullOrEmpty())
            {
                return;
            }
            float num = Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.startTime, 3f, 4f, 3f);
            if (num <= 0f)
            {
                return;
            }
            Vector2 vector = new Vector2(Widgets.ScreenCenter.x, 0.25f * Widgets.VirtualHeight);
            if (Get.QuestCompletedText.IsShowing)
            {
                vector.y += 100f;
            }
            GUI.color = new Color(0.9f, 0.9f, 0.9f, num);
            if (Clock.UnscaledTime - this.startTime > 1f)
            {
                float num2 = Math.Min((Clock.UnscaledTime - this.startTime - 1f) / 0.3f, 1f);
                float num3 = 100f * (1f - num2);
                float num4 = Calc.Lerp(135f, 45f, num2);
                GUIExtra.DrawTextureRotated(RectUtility.CenteredAt(vector.WithAddedY(65f) + Vector2.left * num3, 100f), BossSlainText.SwordIcon, num4, null);
                GUIExtra.DrawTextureRotated(RectUtility.CenteredAt(vector.WithAddedY(65f) + Vector2.right * num3, 100f), BossSlainText.SwordIcon, -num4, null);
            }
            Widgets.FontSizeScalable = 35;
            Widgets.FontBold = true;
            Widgets.LabelCentered(vector, "BossSlain".Translate(this.bossName), true, null, null, false, false, false, null);
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            GUI.color = Color.white;
        }

        private float startTime = -99999f;

        private string bossName;

        private SoundHandle soundHandle;

        private const float Duration_FadingIn = 3f;

        private const float Duration = 4f;

        private const float Duration_FadingOut = 3f;

        private static readonly Texture2D SwordIcon = Assets.Get<Texture2D>("Textures/UI/BossSlainSword");

        private const float SwordIconSize = 100f;

        private const float SwordAnimationDelay = 1f;

        private const float SwordAnimationDuration = 0.3f;

        private const float SwordXOffset = 100f;

        private const float SwordYOffset = 65f;
    }
}