using System;
using System.Collections.Generic;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class WorldEventNotification
    {
        public bool IsShowing
        {
            get
            {
                return this.showingEvent != null && Clock.UnscaledTime - this.startTime < 11f;
            }
        }

        private float Alpha
        {
            get
            {
                return Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.startTime, 3f, 5f, 3f);
            }
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            SoundHandle.DisposeIfFinished(ref this.soundHandle);
            if (this.waitingQueue.Count != 0 && !this.IsShowing && !Get.BossSlainText.IsShowing && !Get.QuestCompletedText.IsShowing)
            {
                this.StartShowing(this.waitingQueue[0]);
                this.waitingQueue.RemoveAt(0);
            }
            if (this.showingEvent == null)
            {
                return;
            }
            float alpha = this.Alpha;
            if (alpha <= 0f)
            {
                return;
            }
            Vector2 vector = new Vector2(Widgets.ScreenCenter.x, 0.25f * Widgets.VirtualHeight);
            GUI.color = new Color(0.9f, 0.9f, 0.9f, alpha);
            GUI.DrawTexture(vector.WithAddedY(-52f).CenteredRect(50f), this.showingEvent.Icon);
            Widgets.FontSizeScalable = 35;
            Widgets.FontBold = true;
            Widgets.LabelCentered(vector, this.showingEvent.LabelCap, true, null, null, false, false, false, null);
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            string text;
            if (!this.showingEvent.Description.NullOrEmpty())
            {
                text = this.showingEvent.Description;
            }
            else
            {
                WorldSituationSpec addSituation = this.showingEvent.AddSituation;
                text = ((addSituation != null) ? addSituation.Description : null) ?? "";
            }
            Widgets.Align = TextAnchor.UpperCenter;
            Widgets.Label(new Rect(vector.x - 310f, vector.y + 34f, 620f, 200f), text, true, null, null, false);
            Widgets.ResetAlign();
            GUI.color = Color.white;
        }

        public void ShowOrQueueFor(WorldEventSpec eventSpec)
        {
            if (!this.IsShowing && !Get.BossSlainText.IsShowing && !Get.QuestCompletedText.IsShowing)
            {
                this.StartShowing(eventSpec);
                return;
            }
            if (!this.waitingQueue.Contains(eventSpec))
            {
                this.waitingQueue.Add(eventSpec);
            }
        }

        public void StopShowingFor(WorldEventSpec eventSpec)
        {
            if (this.showingEvent == eventSpec)
            {
                this.showingEvent = null;
                SoundHandle.DisposeIfNotNull(ref this.soundHandle);
                return;
            }
            this.waitingQueue.Remove(eventSpec);
        }

        public void StartShowing(WorldEventSpec eventSpec)
        {
            this.showingEvent = eventSpec;
            this.startTime = Clock.UnscaledTime;
            SoundHandle.DisposeIfNotNull(ref this.soundHandle);
            this.soundHandle = Get.Sound_WorldEvent.PlayWithHandle(null, false);
            Get.MusicManager.ForceSilenceFor(3f);
            Get.AmbientSoundsManager.ForceSilenceFor(3f);
        }

        private float startTime = -99999f;

        private WorldEventSpec showingEvent;

        private SoundHandle soundHandle;

        private List<WorldEventSpec> waitingQueue = new List<WorldEventSpec>();

        private const float Duration_FadingIn = 3f;

        private const float Duration = 5f;

        private const float Duration_FadingOut = 3f;
    }
}