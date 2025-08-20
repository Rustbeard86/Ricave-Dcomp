using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Inventory : Window
    {
        public Window_Inventory(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Item[,] backpackItems = Get.NowControlledActor.Inventory.BackpackItems;
            int width = Get.NowControlledActor.Inventory.Backpack.Width;
            int height = Get.NowControlledActor.Inventory.Backpack.Height;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    ItemSlotDrawer.DoItemSlot(new Rect(inRect.x + (float)i * (ItemSlotDrawer.SlotSize.x + 5f), inRect.y + (float)j * (ItemSlotDrawer.SlotSize.y + 5f), ItemSlotDrawer.SlotSize.x, ItemSlotDrawer.SlotSize.y), backpackItems[i, j], Window_Inventory.GetCachedItemMoveActionGetter(i, j), true, null, true);
                }
            }
        }

        protected override void DoExtraTitleGUI(Rect titleRect)
        {
            Rect rect = titleRect.LeftPart(titleRect.height);
            string text = "";
            bool flag = true;
            Texture2D sortButtonIcon = Window_Inventory.SortButtonIcon;
            string text2 = "InventorySortButtonTip".Translate();
            if (Widgets.Button(rect, text, flag, null, null, true, true, true, true, sortButtonIcon, false, true, null, false, text2, null))
            {
                ActionViaInterfaceHelper.TryDo(() => new Action_SortInventory(Get.Action_SortInventory, Get.NowControlledActor));
            }
            if (Window_Inventory.goldStrCachedForGold != Get.Player.Gold)
            {
                Window_Inventory.goldStrCachedForGold = Get.Player.Gold;
                Window_Inventory.goldStrCached = RichText.Bold(RichText.Gold("Gold".Translate() + " x" + Get.Player.Gold.ToStringCached()));
            }
            Widgets.Align = TextAnchor.MiddleRight;
            Widgets.Label(titleRect, Window_Inventory.goldStrCached, true, "PlayerGoldTip".Translate(), null, false);
            Widgets.ResetAlign();
            this.DoRat(true);
        }

        public override void ExtraOnGUI()
        {
            this.DoRat(false);
            base.ExtraOnGUI();
        }

        private void DoRat(bool subtractWindowPos)
        {
            if (!Get.RunConfig.HasPetRat)
            {
                return;
            }
            if (!Get.NowControlledActor.IsPlayerParty)
            {
                return;
            }
            Rect rect = new Rect(base.Rect.xMax - 140f - 60f, base.Rect.y - 106.4f, 140f, 140f);
            if (subtractWindowPos)
            {
                rect.x -= base.Rect.x;
                rect.y -= base.Rect.y;
            }
            else
            {
                this.mouseoverRat = Mouse.Over(rect) && !Mouse.Over(base.Rect);
                if (this.mouseoverRat)
                {
                    Get.Tooltips.RegisterTip(rect, "PetRatTip".Translate(), null);
                }
            }
            if (this.mouseoverRat)
            {
                GUI.color = new Color(1f, 1f, 0.35f);
            }
            GUI.DrawTexture(rect, Window_Inventory.RatRotated);
            if (this.mouseoverRat)
            {
                GUI.color = Color.white;
            }
        }

        private static Func<Item, Action> GetCachedItemMoveActionGetter(int x, int y)
        {
            if (Window_Inventory.CachedItemMoveActionGetters[x, y] == null)
            {
                Window_Inventory.CachedItemMoveActionGetters[x, y] = Window_Inventory.CreateItemMoveActionGetter(x, y);
            }
            return Window_Inventory.CachedItemMoveActionGetters[x, y];
        }

        private static Func<Item, Action> CreateItemMoveActionGetter(int x, int y)
        {
            return (Item item) => new Action_MoveItemInInventory(Get.Action_MoveItemInInventory, Get.NowControlledActor, item, new Vector2Int?(new Vector2Int(x, y)), null);
        }

        private bool mouseoverRat;

        private static string goldStrCached;

        private static int goldStrCachedForGold = -1;

        private static readonly Func<Item, Action>[,] CachedItemMoveActionGetters = new Func<Item, Action>[5, 5];

        private static readonly Texture2D RatRotated = Assets.Get<Texture2D>("Textures/UI/RatRotated");

        private static readonly Texture2D SortButtonIcon = Assets.Get<Texture2D>("Textures/UI/SortButton");
    }
}