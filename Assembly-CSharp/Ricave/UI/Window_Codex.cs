using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Codex : Window
    {
        public Window_Codex(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.CalculateSeenEntities();
        }

        private void CalculateSeenEntities()
        {
            this.allSeenEntities.Clear();
            this.allSeenEntities.AddRange(Get.Progress.SeenItemSpecs);
            foreach (EntitySpec entitySpec in Get.Player.SeenItemSpecs)
            {
                if (!this.allSeenEntities.Contains(entitySpec))
                {
                    this.allSeenEntities.Add(entitySpec);
                }
            }
            this.allSeenEntities.AddRange(Get.Progress.SeenActorSpecs);
            foreach (EntitySpec entitySpec2 in Get.Player.SeenActorSpecs)
            {
                if (!this.allSeenEntities.Contains(entitySpec2))
                {
                    this.allSeenEntities.Add(entitySpec2);
                }
            }
            this.allSeenEntities.RemoveAll((EntitySpec x) => x == Get.Entity_Player || x == Get.Entity_LobbyPlayer || x == Get.Entity_UnlockableAsItem || x == Get.Entity_PrivateRoomStructureAsItem);
            this.allSeenEntities.StableSort<EntitySpec, string>((EntitySpec x) => x.LabelCap);
            this.filteredEntities.Clear();
            this.filteredEntities.AddRange(this.allSeenEntities);
            this.itemsCount = this.allSeenEntities.Count<EntitySpec>((EntitySpec x) => x.IsItem);
            this.actorsCount = this.allSeenEntities.Count<EntitySpec>((EntitySpec x) => x.IsActor);
        }

        protected override void DoWindowContents(Rect inRect)
        {
            this.DoSearchFilter(inRect.TopPart(30f));
            Widgets.Label(inRect.CutFromTop(33f), RichText.TableRowL("DiscoveredItems".Translate(), RichText.Bold(this.itemsCount.ToStringCached()), 23), true, null, null, false);
            Widgets.Label(inRect.CutFromTop(53f), RichText.TableRowL("DiscoveredActors".Translate(), RichText.Bold(this.actorsCount.ToStringCached()), 23), true, null, null, false);
            this.DoSeenEntities(inRect.CutFromTop(82f).CutFromBottom(25f));
            GUI.color = Color.gray;
            Widgets.Align = TextAnchor.LowerCenter;
            Widgets.Label(inRect.BottomPart(25f), "DiscoveredEntitiesTip".Translate(), true, null, null, false);
            Widgets.ResetAlign();
            GUI.color = Color.white;
        }

        private void DoSearchFilter(Rect rect)
        {
            Widgets.LabelCenteredV(rect.LeftPart(50f).LeftCenter(), "SearchFilter".Translate(), true, null, null, false);
            string text = Widgets.TextField(rect.CutFromLeft(55f), this.searchFilter, true, false);
            if (text != this.searchFilter)
            {
                this.searchFilter = text;
                string trimmedSearchFilter = this.searchFilter.Trim();
                this.filteredEntities.Clear();
                if (this.searchFilter.NullOrWhitespace())
                {
                    this.filteredEntities.AddRange(this.allSeenEntities);
                    return;
                }
                this.filteredEntities.AddRange(this.allSeenEntities.Where<EntitySpec>((EntitySpec x) => x.LabelCap.Contains(trimmedSearchFilter, StringComparison.OrdinalIgnoreCase)));
            }
        }

        private void DoSeenEntities(Rect rect)
        {
            Rect rect2 = Widgets.BeginScrollView(rect, null);
            for (int i = 0; i < this.filteredEntities.Count; i++)
            {
                Rect rect3 = new Rect(0f, (float)i * 41f, rect2.width, 41f);
                if (Widgets.VisibleInScrollView(rect3))
                {
                    this.DoRow(rect3, this.filteredEntities[i], i);
                }
            }
            Widgets.EndScrollView(rect, (float)this.filteredEntities.Count * 41f, false, false);
        }

        private void DoRow(Rect rect, EntitySpec spec, int index)
        {
            if (index % 2 == 1)
            {
                Widgets.AltRowHighlight(rect);
            }
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            GUI.color = spec.IconColorAdjustedIfIdentified;
            GUIExtra.DrawTexture(rect.LeftPart(rect.height).ContractedBy(1f), spec.IconAdjustedIfIdentified);
            GUI.color = Color.white;
            Widgets.LabelCenteredV(rect.CutFromLeft(rect.height + 6f).LeftCenter(), RichText.Label(spec, true), true, null, null, false);
            Get.Tooltips.RegisterTip(rect, spec, null, null);
        }

        private List<EntitySpec> allSeenEntities = new List<EntitySpec>();

        private List<EntitySpec> filteredEntities = new List<EntitySpec>();

        private string searchFilter;

        private int itemsCount;

        private int actorsCount;

        private const float FilterHeight = 30f;

        private const float RowHeight = 41f;
    }
}