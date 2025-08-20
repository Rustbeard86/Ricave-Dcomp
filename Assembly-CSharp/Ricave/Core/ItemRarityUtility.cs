using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class ItemRarityUtility
    {
        [return: TupleElementNames(new string[] { "label", "color" })]
        public static ValueTuple<string, Color> GetLabelAndColor(this ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Unspecified:
                    return new ValueTuple<string, Color>(null, Color.white);
                case ItemRarity.Common:
                    return new ValueTuple<string, Color>("Common".Translate(), new Color(0.65f, 0.65f, 0.65f));
                case ItemRarity.Uncommon:
                    return new ValueTuple<string, Color>("Uncommon".Translate(), new Color(0.6f, 1f, 0.6f));
                case ItemRarity.Rare:
                    return new ValueTuple<string, Color>("Rare".Translate(), new Color(0.5f, 0.75f, 1f));
                case ItemRarity.Epic:
                    return new ValueTuple<string, Color>("Epic".Translate(), new Color(0.78f, 0.38f, 0.89f));
                default:
                    return new ValueTuple<string, Color>(null, Color.white);
            }
        }

        [return: TupleElementNames(new string[] { "label", "color" })]
        public static ValueTuple<string, Color> GetLabelAndColor(Item item)
        {
            if (item == null)
            {
                return new ValueTuple<string, Color>(null, Color.white);
            }
            if (!item.Identified)
            {
                return new ValueTuple<string, Color>("Unidentified".Translate(), new Color32(136, 136, byte.MaxValue, byte.MaxValue));
            }
            if (item.Cursed)
            {
                return new ValueTuple<string, Color>("Cursed".Translate(), new Color(1f, 0.533f, 0.533f));
            }
            return ItemRarityUtility.GetLabelAndColor(item.Spec, item.Title != null);
        }

        [return: TupleElementNames(new string[] { "label", "color" })]
        public static ValueTuple<string, Color> GetLabelAndColor(EntitySpec itemSpec, bool hasTitle = false)
        {
            if (!itemSpec.IsItem)
            {
                return new ValueTuple<string, Color>(null, Color.white);
            }
            if (!itemSpec.IsLobbyItemOrLobbyRelated)
            {
                return ItemRarityUtility.GetRarity(itemSpec, hasTitle).GetLabelAndColor();
            }
            if (Get.InLobby)
            {
                return new ValueTuple<string, Color>(null, Color.white);
            }
            return new ValueTuple<string, Color>("LobbyItem".Translate(), ItemRarityUtility.LobbyItemColor);
        }

        private static ItemRarity GetRarity(EntitySpec itemSpec, bool hasTitle)
        {
            ItemRarity itemRarity = itemSpec.Item.Rarity;
            if (itemRarity == ItemRarity.Common && hasTitle)
            {
                itemRarity = ItemRarity.Uncommon;
            }
            return itemRarity;
        }

        public static GameObject GetItemBackgroundPrefab(Item item)
        {
            if (ItemRarityUtility.ItemBackgroundPrefab == null)
            {
                ItemRarityUtility.ItemBackgroundPrefab = Assets.Get<GameObject>("Prefabs/Misc/ItemBackground_Normal");
            }
            if (ItemRarityUtility.ItemBackgroundPrefab_Lobby == null)
            {
                ItemRarityUtility.ItemBackgroundPrefab_Lobby = Assets.Get<GameObject>("Prefabs/Misc/ItemBackground_Lobby");
            }
            if (ItemRarityUtility.ItemBackgroundPrefab_Unidentified == null)
            {
                ItemRarityUtility.ItemBackgroundPrefab_Unidentified = Assets.Get<GameObject>("Prefabs/Misc/ItemBackground_Unidentified");
            }
            if (ItemRarityUtility.ItemBackgroundPrefab_Cursed == null)
            {
                ItemRarityUtility.ItemBackgroundPrefab_Cursed = Assets.Get<GameObject>("Prefabs/Misc/ItemBackground_Cursed");
            }
            if (ItemRarityUtility.ItemBackgroundPrefab_Uncommon == null)
            {
                ItemRarityUtility.ItemBackgroundPrefab_Uncommon = Assets.Get<GameObject>("Prefabs/Misc/ItemBackground_Uncommon");
            }
            if (ItemRarityUtility.ItemBackgroundPrefab_Rare == null)
            {
                ItemRarityUtility.ItemBackgroundPrefab_Rare = Assets.Get<GameObject>("Prefabs/Misc/ItemBackground_Rare");
            }
            if (ItemRarityUtility.ItemBackgroundPrefab_Epic == null)
            {
                ItemRarityUtility.ItemBackgroundPrefab_Epic = Assets.Get<GameObject>("Prefabs/Misc/ItemBackground_Epic");
            }
            if (!item.Identified)
            {
                return ItemRarityUtility.ItemBackgroundPrefab_Unidentified;
            }
            if (item.Cursed)
            {
                return ItemRarityUtility.ItemBackgroundPrefab_Cursed;
            }
            if (item.Spec.IsLobbyItemOrLobbyRelated)
            {
                if (Get.InLobby)
                {
                    return ItemRarityUtility.ItemBackgroundPrefab;
                }
                return ItemRarityUtility.ItemBackgroundPrefab_Lobby;
            }
            else
            {
                switch (ItemRarityUtility.GetRarity(item.Spec, item.Title != null))
                {
                    case ItemRarity.Unspecified:
                        return ItemRarityUtility.ItemBackgroundPrefab;
                    case ItemRarity.Common:
                        return ItemRarityUtility.ItemBackgroundPrefab;
                    case ItemRarity.Uncommon:
                        return ItemRarityUtility.ItemBackgroundPrefab_Uncommon;
                    case ItemRarity.Rare:
                        return ItemRarityUtility.ItemBackgroundPrefab_Rare;
                    case ItemRarity.Epic:
                        return ItemRarityUtility.ItemBackgroundPrefab_Epic;
                    default:
                        return ItemRarityUtility.ItemBackgroundPrefab;
                }
            }
        }

        public static readonly Color32 LobbyItemColor = new Color32(120, 168, byte.MaxValue, byte.MaxValue);

        private static GameObject ItemBackgroundPrefab;

        private static GameObject ItemBackgroundPrefab_Lobby;

        private static GameObject ItemBackgroundPrefab_Unidentified;

        private static GameObject ItemBackgroundPrefab_Cursed;

        private static GameObject ItemBackgroundPrefab_Uncommon;

        private static GameObject ItemBackgroundPrefab_Rare;

        private static GameObject ItemBackgroundPrefab_Epic;
    }
}