using System;
using System.Collections.Generic;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class QuestCompletedText
    {
        private float Alpha
        {
            get
            {
                return Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.questCompletedTime, 3f, 4f, 3f);
            }
        }

        public bool IsShowing
        {
            get
            {
                return this.showingLabelFor != null && Clock.UnscaledTime - this.questCompletedTime < 10f;
            }
        }

        public void OnQuestCompleted(QuestSpec questSpec)
        {
            if (!this.IsShowing && !Get.BossSlainText.IsShowing && !Get.WorldEventNotification.IsShowing)
            {
                this.StartShowing(questSpec);
                return;
            }
            if (!this.waitingQueue.Contains(questSpec))
            {
                this.waitingQueue.Add(questSpec);
            }
        }

        public void OnUndoQuestCompleted(QuestSpec questSpec)
        {
            if (this.showingLabelFor == questSpec)
            {
                this.showingLabelFor = null;
                SoundHandle.DisposeIfNotNull(ref this.soundHandle);
                return;
            }
            this.waitingQueue.Remove(questSpec);
        }

        private void StartShowing(QuestSpec questSpec)
        {
            this.questCompletedTime = Clock.UnscaledTime;
            this.showingLabelFor = questSpec;
            SoundHandle.DisposeIfNotNull(ref this.soundHandle);
            this.soundHandle = Get.Sound_QuestCompleted.PlayWithHandle(null, false);
            Get.MusicManager.ForceSilenceFor(5f);
            Get.AmbientSoundsManager.ForceSilenceFor(5f);
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            SoundHandle.DisposeIfFinished(ref this.soundHandle);
            if (this.waitingQueue.Count != 0 && !this.IsShowing && !Get.BossSlainText.IsShowing && !Get.WorldEventNotification.IsShowing)
            {
                this.StartShowing(this.waitingQueue[0]);
                this.waitingQueue.RemoveAt(0);
            }
            if (this.showingLabelFor == null)
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
            Widgets.FontSizeScalable = 35;
            Widgets.FontBold = true;
            Widgets.LabelCentered(vector, "{0}: {1}".Formatted("QuestCompleted".Translate(), this.showingLabelFor.LabelCap), true, null, null, false, false, false, null);
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            Widgets.LabelCentered(vector.WithAddedY(35f), "QuestCompletedTip".Translate(), true, null, null, false, false, false, null);
            GUI.color = Color.white;
        }

        private float questCompletedTime = -99999f;

        private QuestSpec showingLabelFor;

        private SoundHandle soundHandle;

        private List<QuestSpec> waitingQueue = new List<QuestSpec>();

        private const float Duration_FadingIn = 3f;

        private const float Duration = 4f;

        private const float Duration_FadingOut = 3f;
    }
}