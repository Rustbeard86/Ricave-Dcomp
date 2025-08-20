using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Mods : Window
    {
        public Window_Mods(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.modsToShow.AddRange(Get.ModManager.ActiveAndInactiveMods);
            this.SortModsToShow();
            if (this.modsToShow.Count != 0)
            {
                this.selectedMod = this.modsToShow[0];
            }
        }

        public override void OnClosed()
        {
            base.OnClosed();
            Get.ModManager.ModsPlayerWantsActive.Save();
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            Rect rect2 = rect.LeftPart(286f);
            Rect rect3 = rect.CutFromLeft(286f);
            this.DoModList(rect2);
            GUIExtra.DrawVerticalLineBump(rect2.UpperRight(), rect2.height, new Color(0.3f, 0.3f, 0.3f));
            this.DoModDetails(rect3);
            GUIExtra.DrawHorizontalLineBump(rect.BottomLeft(), rect.width, new Color(0.3f, 0.3f, 0.3f));
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private void DoModList(Rect rect)
        {
            if (this.modsToShow.Count == 0)
            {
                GUI.color = new Color(0.75f, 0.75f, 0.75f);
                Widgets.LabelCentered(new Vector2(rect.center.x, 40f), "({0})".Formatted("NoneLower".Translate()), true, null, null, false, false, false, null);
                GUI.color = Color.white;
                return;
            }
            float num = 0f;
            ValueTuple<string, bool>? valueTuple = null;
            ValueTuple<Mod, int>? valueTuple2 = null;
            Rect rect2 = Widgets.BeginScrollView(rect, null);
            foreach (Mod mod in this.modsToShow)
            {
                Rect rect3 = new Rect(0f, num, rect2.width, 50f);
                this.DoModRow(mod, rect3, ref valueTuple, ref valueTuple2);
                num += 50f;
            }
            Widgets.EndScrollView(rect, num, false, false);
            ValueTuple<string, bool>? valueTuple3 = valueTuple;
            if (valueTuple3 != null)
            {
                Get.ModManager.ModsPlayerWantsActive.SetWantsActive(valueTuple.Value.Item1, valueTuple.Value.Item2);
                this.SortModsToShow();
            }
            if (valueTuple2 != null)
            {
                Get.ModManager.ModsPlayerWantsActive.Reorder(valueTuple2.Value.Item1.ModId, valueTuple2.Value.Item2);
                this.SortModsToShow();
            }
        }

        private void DoModRow(Mod mod, Rect rect, ref ValueTuple<string, bool>? changed, ref ValueTuple<Mod, int>? reordered)
        {
            ModsPlayerWantsActive modsPlayerWantsActive = Get.ModManager.ModsPlayerWantsActive;
            GUIExtra.DrawRoundedRectBump(rect, this.BackgroundColor.Lighter(0.04f), false, true, true, true, true, null);
            if (!Get.DragAndDrop.Dragging)
            {
                GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            }
            Rect rect2 = rect.ContractedBy(8f);
            bool flag = Widgets.CheckboxCenteredV(rect2.LeftPart(0f).center, modsPlayerWantsActive.WantsActive(mod.ModId));
            if (flag != modsPlayerWantsActive.WantsActive(mod.ModId))
            {
                changed = new ValueTuple<string, bool>?(new ValueTuple<string, bool>(mod.ModId, flag));
            }
            Widgets.Align = TextAnchor.MiddleLeft;
            Rect rect3 = rect2;
            rect3.xMin += 33f;
            if (!modsPlayerWantsActive.WantsActive(mod.ModId))
            {
                GUI.color = new Color(0.6f, 0.6f, 0.6f);
            }
            Widgets.Label(rect3, mod.Info.Name.Truncate(rect3.width), true, null, null, false);
            GUI.color = Color.white;
            Widgets.ResetAlign();
            if (Get.DragAndDrop.IsDragging(mod))
            {
                GUIExtra.DrawRoundedRect(rect, new Color(0f, 0f, 0f, 0.3f), true, true, true, true, null);
            }
            if (modsPlayerWantsActive.WantsActive(mod.ModId))
            {
                ValueTuple<Mod, int>? valueTuple = Reorderable.RegisterVerticalReorderable<Mod>(mod, rect, this.modsToShow, DragAndDrop.DragSource.Unspecified, null);
                if (valueTuple != null)
                {
                    reordered = valueTuple;
                }
            }
            if (!Get.DragAndDrop.DraggingOrDroppedJustNow && Widgets.ButtonInvisible(rect, false, false))
            {
                this.selectedMod = mod;
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
            }
        }

        private void DoModDetails(Rect rect)
        {
            if (this.selectedMod == null)
            {
                GUI.color = new Color(0.75f, 0.75f, 0.75f);
                Widgets.LabelCentered(new Vector2(rect.center.x, 40f), "({0})".Formatted("NoModSelected".Translate()), true, null, null, false, false, false, null);
                GUI.color = Color.white;
                return;
            }
            Rect rect2 = rect.ContractedBy(20f);
            float num = 0f;
            Rect rect3 = Widgets.BeginScrollView(rect2, null);
            Texture2D previewImage = this.selectedMod.Info.PreviewImage;
            if (!previewImage.IsNullOrMissing())
            {
                Rect rect4 = new Rect(0f, num, rect3.width, rect3.width * ((float)previewImage.height / (float)previewImage.width));
                GUI.DrawTexture(rect4, previewImage);
                num += rect4.height + 5f;
            }
            Widgets.FontBold = true;
            Widgets.FontSizeScalable = 20;
            Widgets.Align = TextAnchor.MiddleLeft;
            Widgets.Label(new Rect(0f, num, rect3.width, 30f), this.selectedMod.Info.Name.Truncate(rect3.width), true, null, null, false);
            num += 30f;
            Widgets.ResetAlign();
            Widgets.ResetFontSize();
            Widgets.FontBold = false;
            GUI.color = new Color(0.75f, 0.75f, 0.75f);
            Widgets.Label(0f, rect3.width, ref num, this.selectedMod.Info.Author, true);
            GUI.color = Color.white;
            num += 13f;
            Widgets.Label(0f, rect3.width, ref num, this.selectedMod.Info.Description, true);
            if (SteamManager.Initialized && !this.selectedMod.Info.SteamWorkshopItemData.DifferentAuthor)
            {
                num += 13f;
                Rect rect5 = new Rect(0f, num, rect3.width, 40f).RightPart(200f);
                string text = "UploadToSteam".Translate();
                if (SteamWorkshop.Uploading)
                {
                    text = text.AppendedWithSpace("({0})".Formatted("UploadingToSteam".Translate()));
                }
                if (Widgets.Button(rect5, text, true, null, null, true, true, true, true, null, true, true, null, false, null, null))
                {
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    if (!SteamWorkshop.Uploading)
                    {
                        SteamWorkshop.UploadOrUpdate(this.selectedMod.Info);
                    }
                }
            }
            Widgets.EndScrollView(rect2, num, false, false);
        }

        private void SortModsToShow()
        {
            this.modsToShow.StableSort<Mod, int>((Mod x) => Get.ModManager.ModsPlayerWantsActive.GetOrderOrInfinite(x.ModId));
        }

        private List<Mod> modsToShow = new List<Mod>();

        private Mod selectedMod;

        private const float ModRowHeight = 50f;
    }
}