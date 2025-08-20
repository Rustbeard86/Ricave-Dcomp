using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_TrainingRoom : Window
    {
        public Action<EntitySpec, bool> OnAccept { get; set; }

        public Window_TrainingRoom(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            List<EntitySpec> all = Get.Specs.GetAll<EntitySpec>();
            HashSet<EntitySpec> seenActorSpecs = Get.Progress.SeenActorSpecs;
            this.knownActorSpecs.Clear();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].IsActor && (Get.Player.Faction == null || all[i].Actor.DefaultFaction != Get.Player.Faction.Spec) && seenActorSpecs.Contains(all[i]))
                {
                    this.knownActorSpecs.Add(all[i]);
                }
            }
            this.knownActorSpecsCanBeBoss.Clear();
            for (int j = 0; j < this.knownActorSpecs.Count; j++)
            {
                if (this.knownActorSpecs[j].Actor.CanBeBoss)
                {
                    this.knownActorSpecsCanBeBoss.Add(this.knownActorSpecs[j]);
                }
            }
            for (int k = this.knownActorSpecs.Count - 1; k >= 0; k--)
            {
                if (this.knownActorSpecs[k].Actor.AlwaysBoss)
                {
                    this.knownActorSpecs.RemoveAt(k);
                }
            }
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            Widgets.Tabs(rect.TopPart(32f), this.tabs, ref this.currentTab, true, null, null);
            Rect rect2 = rect.CutFromTop(32f);
            Rect rect3 = Widgets.BeginScrollView(rect2, new int?(81325477));
            float num = this.DoList(rect3);
            Widgets.EndScrollView(rect2, num, false, false);
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private float DoList(Rect viewRect)
        {
            float num = 0f;
            List<EntitySpec> list = ((this.currentTab == 0) ? this.knownActorSpecs : this.knownActorSpecsCanBeBoss);
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0)
                {
                    num -= 1f;
                }
                EntitySpec entitySpec = list[i];
                Rect rect = new Rect(viewRect.x, num, viewRect.width, 110f);
                if (rect.VisibleInScrollView())
                {
                    this.DoRow(rect, entitySpec, i > 0, i < list.Count - 1);
                }
                num += 110f;
            }
            return num;
        }

        private void DoRow(Rect rowRect, EntitySpec actorSpec, bool anyAbove, bool anyBelow)
        {
            GUIExtra.DrawRoundedRectBump(rowRect, this.BackgroundColor.Lighter(0.09f), false, !anyAbove, !anyAbove, !anyBelow, !anyBelow, null);
            GUIExtra.DrawHighlightIfMouseover(rowRect, true, !anyAbove, !anyAbove, !anyBelow, !anyBelow);
            Rect rect = rowRect.ContractedBy(20f);
            Rect rect2 = new Rect(rect.xMax - 120f, rect.y + (rect.height - 50f) / 2f, 120f, 50f);
            Rect rect3 = rect;
            rect3.xMax = rect2.x - 10f;
            string text = ((this.currentTab == 1 && !actorSpec.Actor.AlwaysBoss) ? "BossLabel".Translate(actorSpec.LabelAdjusted).CapitalizeFirst() : actorSpec.LabelAdjustedCap);
            GUI.color = actorSpec.IconColorAdjusted;
            GUI.DrawTexture(rect3.LeftPart(42f).ContractedToSquare(), actorSpec.IconAdjusted);
            GUI.color = Color.white;
            Widgets.FontSizeScalable = 19;
            Widgets.FontBold = true;
            Widgets.LabelCenteredV(rect3.CutFromLeft(53f).LeftCenter(), text, true, null, null, false);
            Widgets.CalcSize(text);
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            if (Widgets.Button(rect2, "Train".Translate(), true, null, null, true, true, true, true, null, true, true, null, false, null, null))
            {
                this.OnAccept(actorSpec, this.currentTab == 1);
                Get.WindowManager.Close(this, true);
            }
            if (!Mouse.Over(rect2))
            {
                Get.Tooltips.RegisterTip(rowRect, actorSpec, null, null);
            }
        }

        private int currentTab;

        private readonly List<string> tabs = new List<string>
        {
            "Enemies".Translate(),
            "Bosses".Translate()
        };

        private List<EntitySpec> knownActorSpecs = new List<EntitySpec>();

        private List<EntitySpec> knownActorSpecsCanBeBoss = new List<EntitySpec>();

        private const int RowHeight = 110;

        private const int Gap = 10;
    }
}