using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Cosmetics : Window
    {
        public CosmeticSpec HoverCosmetic
        {
            get
            {
                return this.hoverCosmetic;
            }
        }

        public CosmeticSpec LastUnchosen
        {
            get
            {
                return this.lastUnchosen;
            }
        }

        public Window_Cosmetics(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.allCosmeticsInOrder.Clear();
            this.allCosmeticsInOrder.AddRange(from x in Get.Specs.GetAll<CosmeticSpec>()
                                              orderby x.UIOrder
                                              select x);
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            Rect rect2 = rect.LeftPartPct(0.41f);
            this.DoPlayerPreview(rect2);
            Rect rect3 = rect.RightPartPct(0.59f);
            this.DoCosmetics(rect3.CutFromTop(10f));
            GUI.color = Color.gray;
            Widgets.LabelCentered(rect.BottomPart(40f).center, "CosmeticsInfo".Translate(), true, null, null, false, false, false, null);
            GUI.color = Color.white;
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private void DoPlayerPreview(Rect rect)
        {
            GUI.DrawTexture(rect.ContractedToSquare().ExpandedByPct(0.1f), Get.PlayerModel.Preview);
        }

        private void DoCosmetics(Rect rect)
        {
            this.hoverCosmetic = null;
            for (int i = 0; i < this.allCosmeticsInOrder.Count; i++)
            {
                CosmeticSpec cosmeticSpec = this.allCosmeticsInOrder[i];
                int num = i % 4;
                int num2 = i / 4;
                float num3 = rect.x + (float)num * 104f;
                float num4 = rect.y + (float)num2 * 104f;
                Rect rect2 = new Rect(num3, num4, 90f, 90f);
                this.DoCosmetic(rect2, cosmeticSpec);
            }
            if (this.lastUnchosen != null && this.hoverCosmetic != this.lastUnchosen)
            {
                this.lastUnchosen = null;
            }
        }

        private void DoCosmetic(Rect rect, CosmeticSpec cosmetic)
        {
            bool flag = cosmetic.IsUnlocked();
            bool flag2 = cosmetic.IsChosen();
            float num = (flag ? 1f : 0.5f);
            if (flag2)
            {
                GUIExtra.DrawRoundedRect(rect.ExpandedByPct(0.06f), new Color(0.4f, 0.65f, 1f), true, true, true, true, null);
            }
            GUIExtra.DrawRoundedRectBump(rect, new Color(0.4f, 0.4f, 0.4f).MultipliedColor(num), false, true, true, true, true, null);
            GUI.color = Color.white.MultipliedColor(num);
            GUI.DrawTexture(rect, cosmetic.Icon);
            GUI.color = Color.white;
            if (Mouse.Over(rect))
            {
                Get.Tooltips.RegisterTip(rect, this.GetTip(cosmetic), null);
                this.hoverCosmetic = cosmetic;
            }
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            if (!flag)
            {
                Widgets.Align = TextAnchor.LowerRight;
                Widgets.Label(rect.ContractedByPct(0.05f), cosmetic.Price.PriceShortRichText, true, null, null, false);
                Widgets.ResetAlign();
            }
            if (Widgets.ButtonInvisible(rect, true, false))
            {
                if (!flag)
                {
                    if (cosmetic.Price.CanAfford(Get.MainActor))
                    {
                        Get.WindowManager.OpenConfirmationWindow("ConfirmBuyCosmetic".Translate(cosmetic.LabelCap, cosmetic.Price.PriceRichText), delegate
                        {
                            if (cosmetic.Price.CanAfford(Get.MainActor))
                            {
                                foreach (Instruction instruction in cosmetic.Price.MakePayInstructions(Get.MainActor, null))
                                {
                                    instruction.Do();
                                }
                                Get.CosmeticsManager.Unlock(cosmetic);
                                Get.Sound_BoughtItem.PlayOneShot(null, 1f, 1f);
                                this.UnchooseCollidingWith(cosmetic);
                                Get.CosmeticsManager.Choose(cosmetic);
                            }
                        }, false, null, null);
                        return;
                    }
                    Get.PlayLog.Add("CantAffordCosmetic".Translate());
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    return;
                }
                else
                {
                    if (flag2)
                    {
                        Get.CosmeticsManager.Unchoose(cosmetic);
                        Get.Sound_Unwear.PlayOneShot(null, 1f, 1f);
                        this.lastUnchosen = cosmetic;
                        return;
                    }
                    this.UnchooseCollidingWith(cosmetic);
                    Get.CosmeticsManager.Choose(cosmetic);
                    Get.Sound_Wear.PlayOneShot(null, 1f, 1f);
                }
            }
        }

        private string GetTip(CosmeticSpec cosmetic)
        {
            if (cosmetic == this.cachedTipForCosmetic)
            {
                return this.cachedTip;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(cosmetic.LabelCap);
            stringBuilder.Append("\n\n");
            stringBuilder.Append(cosmetic.Description);
            if (cosmetic == Get.Cosmetic_DevilHorns)
            {
                stringBuilder.Append("\n\n");
                stringBuilder.Append(RichText.LightRed("CosmeticWillMakeFactionHostile".Translate(Get.Faction_Guardians)));
            }
            if (!cosmetic.IsUnlocked())
            {
                stringBuilder.Append("\n\n");
                stringBuilder.Append("Price".Translate());
                stringBuilder.Append(": ");
                stringBuilder.Append(cosmetic.Price.PriceRichText);
            }
            bool flag = false;
            foreach (CosmeticSpec cosmeticSpec in Get.Specs.GetAll<CosmeticSpec>())
            {
                if (cosmeticSpec != cosmetic)
                {
                    bool flag2 = false;
                    foreach (string text in cosmetic.CollisionTags)
                    {
                        if (cosmeticSpec.CollisionTags.Contains(text))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (flag2)
                    {
                        if (!flag)
                        {
                            stringBuilder.Append("\n\n");
                            stringBuilder.Append(RichText.Grayed("CosmeticCollidesWith".Translate()));
                            stringBuilder.Append(RichText.Grayed(": "));
                            stringBuilder.Append(RichText.Grayed(cosmeticSpec.LabelCap));
                            flag = true;
                        }
                        else
                        {
                            stringBuilder.Append(RichText.Grayed(", "));
                            stringBuilder.Append(RichText.Grayed(cosmeticSpec.Label));
                        }
                    }
                }
            }
            if (cosmetic.IsUnlocked())
            {
                stringBuilder.Append("\n\n");
                stringBuilder.Append(RichText.Grayed((ControllerUtility.InControllerMode ? "ClickToSelectController" : "ClickToSelect").Translate()));
            }
            this.cachedTip = stringBuilder.ToString();
            this.cachedTipForCosmetic = cosmetic;
            return this.cachedTip;
        }

        private bool CollidesWithChosen(CosmeticSpec candidate)
        {
            foreach (CosmeticSpec cosmeticSpec in Get.CosmeticsManager.Chosen)
            {
                foreach (string text in candidate.CollisionTags)
                {
                    if (cosmeticSpec.CollisionTags.Contains(text))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void UnchooseCollidingWith(CosmeticSpec cosmetic)
        {
            foreach (CosmeticSpec cosmeticSpec in Get.CosmeticsManager.Chosen.ToTemporaryList<CosmeticSpec>())
            {
                foreach (string text in cosmetic.CollisionTags)
                {
                    if (cosmeticSpec.CollisionTags.Contains(text))
                    {
                        Get.CosmeticsManager.Unchoose(cosmeticSpec);
                    }
                }
            }
        }

        private List<CosmeticSpec> allCosmeticsInOrder = new List<CosmeticSpec>();

        private CosmeticSpec hoverCosmetic;

        private CosmeticSpec lastUnchosen;

        private const float CosmeticIconSize = 90f;

        private const float GapBetweenIcons = 14f;

        private CosmeticSpec cachedTipForCosmetic;

        private string cachedTip;
    }
}