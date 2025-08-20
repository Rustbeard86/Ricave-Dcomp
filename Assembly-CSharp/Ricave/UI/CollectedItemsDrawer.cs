using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class CollectedItemsDrawer
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (Get.InLobby)
            {
                return;
            }
            this.UpdateEntries();
            int num = (Get.Player.HasWatch ? 130 : 25);
            Rect rect = new Rect((float)num, Widgets.VirtualHeight - 25f - 80f - 42f, (float)(80 * this.entries.Count + 25 * (this.entries.Count - 1)), 122f).ExpandedBy(13f);
            for (int i = 0; i < this.entries.Count; i++)
            {
                if (this.entries[i].lastAnimationPct != this.entries[i].AnimationPct)
                {
                    CachedGUI.SetDirty(8);
                    break;
                }
            }
            float num2 = (float)num;
            for (int j = 0; j < this.entries.Count; j++)
            {
                Rect rect2 = new Rect(num2, Widgets.VirtualHeight - 25f - 80f - 42f, 80f, 122f).ExpandedBy(12f, 0f);
                GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
                Get.Tooltips.RegisterTip(rect2, this.entries[j].itemSpec, null, null);
                num2 += 105f;
            }
            if (CachedGUI.BeginCachedGUI(rect, 8, false))
            {
                num2 = (float)num;
                for (int k = 0; k < this.entries.Count; k++)
                {
                    Rect rect3 = new Rect(num2, Widgets.VirtualHeight - 25f - 80f, 80f, 80f);
                    this.DoEntry(k, rect3);
                    num2 += 105f;
                }
            }
            CachedGUI.EndCachedGUI(1f, 1f);
            num2 = (float)num;
            for (int l = 0; l < this.entries.Count; l++)
            {
                Rect rect4 = new Rect(num2, Widgets.VirtualHeight - 25f - 80f, 80f, 80f);
                CollectedItemsDrawer.Entry entry = this.entries[l];
                ExpandingIconAnimation.Do(rect4, entry.itemSpec.IconAdjusted, entry.itemSpec.IconColorAdjusted, entry.lastCountChangedTime, 0.9f, 0.6f, 0.55f);
                num2 += 105f;
            }
        }

        private void UpdateEntries()
        {
            if (this.GetEntryIndex(Get.Entity_Gold) == -1)
            {
                this.entries.Add(new CollectedItemsDrawer.Entry
                {
                    itemSpec = Get.Entity_Gold
                });
                CachedGUI.SetDirty(8);
            }
            this.< UpdateEntries > g__CheckUpdateCount | 7_0(this.GetEntryIndex(Get.Entity_Gold), Get.Player.Gold);
            foreach (KeyValuePair<EntitySpec, int> keyValuePair in Get.ThisRunLobbyItems.LobbyItems)
            {
                if (!keyValuePair.Key.Item.HideLobbyItem)
                {
                    int num = this.GetEntryIndex(keyValuePair.Key);
                    if (num == -1)
                    {
                        if (keyValuePair.Value == 0)
                        {
                            continue;
                        }
                        this.entries.Add(new CollectedItemsDrawer.Entry
                        {
                            itemSpec = keyValuePair.Key,
                            lastKnownTotalCountForLobbyItems = new int?(Get.TotalLobbyItems.GetCount(keyValuePair.Key))
                        });
                        num = this.entries.Count - 1;
                        CachedGUI.SetDirty(8);
                    }
                    this.< UpdateEntries > g__CheckUpdateCount | 7_0(num, keyValuePair.Value);
                }
            }
            bool flag = false;
            if (Get.PlaceSpec == Get.Place_Shelter && Get.TotalLobbyItems.GetCount(Get.Entity_Diamond) + Get.ThisRunLobbyItems.GetCount(Get.Entity_Diamond) > 0)
            {
                flag = true;
                if (this.GetEntryIndex(Get.Entity_Diamond) == -1)
                {
                    this.entries.Add(new CollectedItemsDrawer.Entry
                    {
                        itemSpec = Get.Entity_Diamond,
                        lastKnownTotalCountForLobbyItems = new int?(Get.TotalLobbyItems.GetCount(Get.Entity_Diamond))
                    });
                    CachedGUI.SetDirty(8);
                }
                this.< UpdateEntries > g__CheckUpdateCount | 7_0(this.GetEntryIndex(Get.Entity_Diamond), Get.ThisRunLobbyItems.GetCount(Get.Entity_Diamond));
            }
            for (int i = this.entries.Count - 1; i >= 0; i--)
            {
                if (this.entries[i].itemSpec != Get.Entity_Gold && (!flag || this.entries[i].itemSpec != Get.Entity_Diamond) && (this.entries[i].lastKnownCount == 0 || (this.entries[i].itemSpec.IsLobbyItemOrLobbyRelated && Get.ThisRunLobbyItems.GetCount(this.entries[i].itemSpec) == 0)))
                {
                    this.entries.RemoveAt(i);
                    CachedGUI.SetDirty(8);
                }
            }
        }

        private int GetEntryIndex(EntitySpec itemSpec)
        {
            for (int i = 0; i < this.entries.Count; i++)
            {
                if (this.entries[i].itemSpec == itemSpec)
                {
                    return i;
                }
            }
            return -1;
        }

        private void DoEntry(int entryIndex, Rect rect)
        {
            CollectedItemsDrawer.Entry entry = this.entries[entryIndex];
            GUI.color = entry.itemSpec.IconColorAdjusted;
            GUIExtra.DrawTexture(rect, entry.itemSpec.IconAdjusted);
            GUI.color = Color.white;
            Widgets.FontSizeScalable = 22;
            Widgets.FontBold = true;
            GUI.color = new Color(0.85f, 0.85f, 0.85f).Lighter(entry.AnimationPct * 0.15f);
            Vector2 vector = rect.center.WithAddedY(-65f);
            vector.y -= entry.AnimationPct * 4f;
            string text = entry.lastKnownCount.ToStringCached();
            if ((entry.lastKnownCount < 0 && entry.lastKnownTotalCountForLobbyItems != null) || (entry.itemSpec == Get.Entity_Diamond && Get.PlaceSpec == Get.Place_Shelter && entry.lastKnownTotalCountForLobbyItems != null))
            {
                text = "{0} {1}".Formatted(text, RichText.Grayed("({0})".Formatted(entry.lastKnownCount + entry.lastKnownTotalCountForLobbyItems.Value)));
            }
            Widgets.LabelCentered(vector, text, true, null, null, false, false, false, null);
            GUI.color = Color.white;
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            if (entry.itemSpec.IsLobbyItemOrLobbyRelated)
            {
                Vector2 vector2 = rect.center.WithAddedY(36f);
                GUI.color = ItemRarityUtility.LobbyItemColor;
                Widgets.LabelCentered(vector2, "LobbyItemShort".Translate(), true, null, null, false, false, false, null);
                GUI.color = Color.white;
            }
            entry.lastAnimationPct = entry.AnimationPct;
            this.entries[entryIndex] = entry;
        }

        [CompilerGenerated]
        private void <UpdateEntries>g__CheckUpdateCount|7_0(int entryIndex, int newCount)
		{
			CollectedItemsDrawer.Entry entry = this.entries[entryIndex];
			if (entry.lastKnownCount != newCount)
			{
				entry.lastKnownCount = newCount;
				entry.lastCountChangedTime = Clock.UnscaledTime;
				this.entries[entryIndex] = entry;
				CachedGUI.SetDirty(8);
			}
}

private List<CollectedItemsDrawer.Entry> entries = new List<CollectedItemsDrawer.Entry>();

private const int IconSize = 80;

private const int Margin = 25;

private const int Spacing = 25;

private const float CountChangedAnimationDuration = 0.3f;

private struct Entry
{
    public float AnimationPct
    {
        get
        {
            return Math.Max(1f - (Clock.UnscaledTime - this.lastCountChangedTime) / 0.3f, 0f);
        }
    }

    public EntitySpec itemSpec;

    public int lastKnownCount;

    public float lastCountChangedTime;

    public float lastAnimationPct;

    public int? lastKnownTotalCountForLobbyItems;
}
	}
}