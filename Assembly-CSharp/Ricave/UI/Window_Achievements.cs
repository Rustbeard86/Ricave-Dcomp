using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Achievements : Window
    {
        public Window_Achievements(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.allAchievementsInOrder.Clear();
            this.allAchievementsInOrder.AddRange(from x in Get.Specs.GetAll<AchievementSpec>()
                                                 orderby x.LabelCap
                                                 select x);
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            Widgets.Label(rect, RichText.TableRowL("UnlockedAchievements".Translate(), RichText.Bold("{0}/{1}".Formatted(AchievementsUtility.CompletedAchievementsCount.ToStringCached(), AchievementsUtility.AchievementsCount.ToStringCached())), 23), true, null, null, false);
            this.DoAchievementsGrid(rect.CutFromTop(35f));
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private void DoAchievementsGrid(Rect rect)
        {
            for (int i = 0; i < this.allAchievementsInOrder.Count; i++)
            {
                AchievementSpec achievementSpec = this.allAchievementsInOrder[i];
                int num = i % 6;
                int num2 = i / 6;
                float num3 = rect.x + (float)num * 117f;
                float num4 = rect.y + (float)num2 * 117f;
                Rect rect2 = new Rect(num3, num4, 103f, 103f);
                this.DoAchievement(rect2, achievementSpec);
            }
        }

        private void DoAchievement(Rect rect, AchievementSpec achievement)
        {
            GUI.DrawTexture(rect, achievement.IsCompleted ? achievement.CompletedIcon : achievement.LockedIcon);
            if (Mouse.Over(rect))
            {
                Get.Tooltips.RegisterTip(rect, achievement.Description, null);
            }
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
        }

        private List<AchievementSpec> allAchievementsInOrder = new List<AchievementSpec>();

        private const float AchievementIconSize = 103f;

        private const float GapBetweenIcons = 14f;
    }
}