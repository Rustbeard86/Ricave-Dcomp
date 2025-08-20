using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Factions : Window
    {
        public Window_Factions(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.factionsInUIOrder = (from x in Get.FactionManager.Factions
                                      where !x.Hidden
                                      orderby x.Spec.UIOrder
                                      select x).ToList<Faction>();
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Widgets.LabelCenteredV(new Vector2(300f, 20f), "FactionEnemies".Translate(), true, null, null, false);
            Widgets.LabelCenteredV(new Vector2(590f, 20f), "FactionKnownMembers".Translate(), true, null, null, false);
            Rect rect = inRect.CutFromTop(40f);
            Rect rect2 = Widgets.BeginScrollView(rect, new int?(713215995));
            float num = this.DoFactionsList(rect2);
            Widgets.EndScrollView(rect, num, false, false);
        }

        private float DoFactionsList(Rect viewRect)
        {
            float num = 0f;
            for (int i = 0; i < this.factionsInUIOrder.Count; i++)
            {
                Faction faction = this.factionsInUIOrder[i];
                Rect rect = new Rect(viewRect.x, num, viewRect.width, 88f);
                if (rect.VisibleInScrollView())
                {
                    if (i % 2 == 1)
                    {
                        Widgets.AltRowHighlight(rect);
                    }
                    this.DoFactionRow(rect, faction);
                }
                num += 88f;
            }
            return num;
        }

        private void DoFactionRow(Rect rowRect, Faction faction)
        {
            GUIExtra.DrawHighlightIfMouseover(rowRect, true, true, true, true, true);
            Rect rect = rowRect.ContractedBy(20f).LeftPart(48f).ContractedToSquare();
            GUI.color = faction.IconColor;
            GUI.DrawTexture(rect, faction.Icon);
            GUI.color = Color.white;
            Widgets.FontBold = true;
            Widgets.LabelCenteredV(rect.RightCenter().MovedBy(12f, 0f), faction.LabelCap.Truncate(200f), true, null, null, false);
            Widgets.FontBold = false;
            float num = 300f;
            bool flag = false;
            int num2 = 0;
            for (int i = 0; i < this.factionsInUIOrder.Count; i++)
            {
                Faction faction2 = this.factionsInUIOrder[i];
                if (faction2.IsHostile(faction))
                {
                    num2++;
                    Rect rect2 = rowRect.CutFromLeft(num).LeftPart(48f).ContractedToSquare();
                    GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
                    if (Mouse.Over(rect2))
                    {
                        Get.Tooltips.RegisterTip(rect2, faction2, null, null);
                        flag = true;
                    }
                    GUI.color = faction2.IconColor;
                    GUI.DrawTexture(rect2, faction2.Icon);
                    GUI.color = Color.white;
                    num += 53f;
                    if (num2 >= 5)
                    {
                        break;
                    }
                }
            }
            if (num2 == 0)
            {
                GUI.color = Color.gray;
                Widgets.LabelCenteredV(new Vector2(num + 2f, rowRect.center.y), "None".Translate(), true, null, null, false);
                GUI.color = Color.white;
            }
            float num3 = 590f;
            int num4 = 0;
            foreach (EntitySpec entitySpec in Get.Player.SeenActorSpecs)
            {
                if (entitySpec.Actor.DefaultFaction == faction.Spec)
                {
                    num4++;
                    Rect rect3 = rowRect.CutFromLeft(num3).LeftPart(48f).ContractedToSquare();
                    GUIExtra.DrawHighlightIfMouseover(rect3, true, true, true, true, true);
                    if (entitySpec == Get.MainActor.Spec)
                    {
                        if (Mouse.Over(rect3))
                        {
                            Get.Tooltips.RegisterTip(rect3, Get.MainActor, null, null);
                            flag = true;
                        }
                        GUI.color = Get.MainActor.IconColor;
                        GUI.DrawTexture(rect3, Get.MainActor.Icon);
                        GUI.color = Color.white;
                    }
                    else
                    {
                        if (Mouse.Over(rect3))
                        {
                            Get.Tooltips.RegisterTip(rect3, entitySpec, null, null);
                            flag = true;
                        }
                        GUI.color = entitySpec.IconColor;
                        GUI.DrawTexture(rect3, entitySpec.Icon);
                        GUI.color = Color.white;
                    }
                    num3 += 53f;
                    if (num4 >= 5)
                    {
                        break;
                    }
                }
            }
            if (num4 == 0)
            {
                GUI.color = Color.gray;
                Widgets.LabelCenteredV(new Vector2(num3 + 2f, rowRect.center.y), "None".Translate(), true, null, null, false);
                GUI.color = Color.white;
            }
            if (!flag)
            {
                Get.Tooltips.RegisterTip(rowRect, faction, null, null);
            }
        }

        private List<Faction> factionsInUIOrder;

        private const int RowHeight = 88;

        private const int IconSize = 48;

        private const float EnemiesX = 300f;

        private const float KnownMembersX = 590f;
    }
}