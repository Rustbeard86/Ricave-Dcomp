using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Traits : Window
    {
        public Window_Traits(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.allTraitsInOrder.Clear();
            this.allTraitsInOrder.AddRange(from x in Get.Specs.GetAll<TraitSpec>()
                                           orderby x.UIOrder
                                           select x);
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            Rect rect2 = rect.LeftPartPct(0.6f);
            Rect rect3 = rect.RightPartPct(0.4f);
            Rect rect4 = rect3.TopPartPct(0.7f);
            rect4.xMin += 12f;
            Rect rect5 = rect3.BottomPartPct(0.3f);
            GUIExtra.DrawVerticalLineBump(rect3.position, rect3.height, new Color(0.3f, 0.3f, 0.3f));
            GUIExtra.DrawHorizontalLineBump(rect5.position, rect5.width, new Color(0.3f, 0.3f, 0.3f));
            TraitSpec traitSpec = null;
            foreach (TraitSpec traitSpec2 in Get.Specs.GetAll<TraitSpec>())
            {
                Rect iconRect = this.GetIconRect(traitSpec2);
                Window_Traits.DoTrait(iconRect, traitSpec2, false, true);
                if (traitSpec2.IsUnlocked() && Widgets.ButtonInvisible(iconRect, true, false))
                {
                    if (traitSpec2.IsChosen())
                    {
                        Get.TraitManager.Unchoose(traitSpec2);
                        Get.Sound_Unwear.PlayOneShot(null, 1f, 1f);
                    }
                    else if (Get.TraitManager.Chosen.Count >= 2)
                    {
                        Get.PlayLog.Add("TraitsLimitReached".Translate());
                        Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    }
                    else
                    {
                        Get.TraitManager.Choose(traitSpec2);
                        Get.Sound_Wear.PlayOneShot(null, 1f, 1f);
                    }
                }
                if (Mouse.Over(iconRect))
                {
                    if (traitSpec2.IsUnlocked())
                    {
                        traitSpec = traitSpec2;
                    }
                    else
                    {
                        Widgets.Label(rect4, "LockedTraitTip".Translate(), true, null, null, false);
                    }
                }
            }
            if (Get.TraitManager.Unlocked.Count < Get.Specs.GetAll<TraitSpec>().Count)
            {
                int price = this.GetNextTraitPrice();
                Widgets.FontSizeScalable = 20;
                Widgets.FontBold = true;
                Widgets.LabelCentered(rect2.BottomCenter().MovedBy(0f, -98f), "{0}: {1}".Formatted("NextTraitsPrice".Translate(), RichText.Stardust(StringUtility.StardustString(price))), true, null, null, false, false, false, null);
                Widgets.FontBold = false;
                Widgets.ResetFontSize();
                if (Widgets.Button(rect2.BottomPart(70f).ResizedTo(250f, 70f), "BuyRandomTraits".Translate(2), false, null, null, true, true, true, true, null, true, true, null, false, null, null))
                {
                    if (Get.TotalLobbyItems.Stardust >= price)
                    {
                        Get.WindowManager.OpenConfirmationWindow("ConfirmBuyRandomTraits".Translate(StringUtility.StardustString(price), 2.ToStringCached()), delegate
                        {
                            if (Get.TotalLobbyItems.Stardust >= price)
                            {
                                Get.TotalLobbyItems.ChangeCount(Get.Entity_Stardust, -price);
                                this.UnlockRandomTraits();
                            }
                        }, false, null, null);
                    }
                    else
                    {
                        Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                        Get.PlayLog.Add("CantAffordTrait".Translate());
                    }
                }
            }
            Widgets.LabelCentered(rect5.TopCenter().MovedBy(0f, 25f), "{0}:".Formatted("SelectedTraits".Translate()), true, null, null, false, false, false, null);
            Vector2 vector = rect5.center.MovedBy(0f, 25f);
            if (Get.TraitManager.Chosen.Count != 0)
            {
                float num = (float)Get.TraitManager.Chosen.Count * 104f - 14f;
                float num2 = vector.x - num / 2f;
                using (HashSet<TraitSpec>.Enumerator enumerator2 = Get.TraitManager.Chosen.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        TraitSpec traitSpec3 = enumerator2.Current;
                        Rect rect6 = new Rect(num2, vector.y - 45f, 90f, 90f);
                        Window_Traits.DoTrait(rect6, traitSpec3, false, false);
                        if (Mouse.Over(rect6))
                        {
                            traitSpec = traitSpec3;
                        }
                        num2 += 104f;
                    }
                    goto IL_0484;
                }
            }
            Widgets.LabelCentered(vector, "None".Translate(), true, null, null, false, false, false, null);
        IL_0484:
            if (traitSpec != null)
            {
                GUIExtra.BeginGroup(rect4);
                float num3;
                TipSubjectDrawer.DoContents(traitSpec, rect4.width, out num3, (ControllerUtility.InControllerMode ? "ClickToSelectTraitController" : "ClickToSelectTrait").Translate(), false);
                Rect rect7 = rect4.AtZero().TopPart(24f).RightPart(24f);
                GUI.color = Get.Entity_Demon.IconColorAdjusted.WithAlphaFactor(0.5f);
                GUI.DrawTexture(rect7, Get.Entity_Demon.IconAdjusted);
                GUI.color = Color.white.WithAlphaFactor(0.5f);
                Widgets.LabelCentered(rect7.BottomCenter().MovedBy(0f, 9f), Get.TraitManager.GetDemonsKilledWithTrait(traitSpec).ToStringCached(), true, null, null, false, false, false, null);
                GUI.color = Color.white;
                GUIExtra.EndGroup();
            }
            foreach (TraitSpec traitSpec4 in Get.Specs.GetAll<TraitSpec>())
            {
                ExpandingIconAnimation.Do(this.GetIconRect(traitSpec4), traitSpec4.Icon, traitSpec4.IconColor, Get.TraitManager.GetTimeUnlocked(traitSpec4), 1f, 0.6f, 0.55f);
            }
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private void UnlockRandomTraits()
        {
            bool flag = false;
            for (int i = 0; i < 2; i++)
            {
                TraitSpec traitSpec;
                if ((from x in Get.Specs.GetAll<TraitSpec>()
                     where !x.IsUnlocked()
                     select x).TryGetRandomElement<TraitSpec>(out traitSpec, (TraitSpec x) => this.GetSelectionWeight(x)))
                {
                    Get.TraitManager.Unlock(traitSpec, -1);
                    flag = true;
                }
            }
            if (flag)
            {
                Get.Sound_BoughtItem.PlayOneShot(null, 1f, 1f);
            }
        }

        public static void DoTrait(Rect rect, TraitSpec trait, bool doTip = true, bool canDoSelectedOutline = true)
        {
            float colorFactor = Window_Traits.GetColorFactor(trait);
            float num = Widgets.AccumulatedHover(rect, false);
            if (doTip && (Get.TraitManager.IsUnlocked(trait) || Get.TraitManager.IsChosen(trait)))
            {
                Get.Tooltips.RegisterTip(rect, trait, null, null);
            }
            GUI.color = new Color(0f, 0f, 0f, 0.8f);
            GUI.DrawTexture(rect.MovedBy(0f, 2f).ContractedBy(1f, 0f), Window_Traits.TraitBackground);
            if (canDoSelectedOutline && trait.IsChosen())
            {
                GUI.color = new Color(0.33f, 0.53f, 1f).MultipliedColor(colorFactor).Lighter(num * 0.1f);
                GUI.DrawTexture(rect.ExpandedByPct(0.07f), Window_Traits.TraitBackground);
            }
            GUI.color = Color.white.MultipliedColor(colorFactor * 0.5f).Lighter(num * 0.1f);
            GUI.DrawTexture(rect, Window_Traits.TraitBackground);
            GUI.color = Color.white;
            if (Get.TraitManager.IsUnlocked(trait) || Get.TraitManager.IsChosen(trait))
            {
                GUI.color = trait.IconColor.MultipliedColor(colorFactor);
                GUI.DrawTexture(rect.ContractedByPct(0.14f).MovedByPct(0f, 0.05f), trait.Icon);
                GUI.color = Color.white;
                if (canDoSelectedOutline)
                {
                    GUI.color = new Color(1f, 1f, 0.6f);
                    Widgets.LabelCentered(rect.BottomCenter().MovedBy(0f, -11f), Get.TraitManager.GetMaxFloorReachedWithTrait(trait).ToStringCached(), true, null, null, false, true, false, null);
                    GUI.color = Color.white;
                    return;
                }
            }
            else
            {
                GUI.color = new Color(0.65f, 0.65f, 0.65f).MultipliedColor(colorFactor);
                Widgets.FontSize = 50;
                Widgets.FontBold = true;
                Widgets.LabelCentered(rect.MovedByPct(0f, 0.038f).center, "?", false, null, null, false, false, false, null);
                Widgets.FontBold = false;
                Widgets.ResetFontSize();
                GUI.color = Color.white;
            }
        }

        private Rect GetIconRect(TraitSpec trait)
        {
            int num = 0;
            for (int i = 0; i < this.allTraitsInOrder.Count; i++)
            {
                if (this.allTraitsInOrder[i].Rarity == trait.Rarity)
                {
                    if (this.allTraitsInOrder[i] == trait)
                    {
                        break;
                    }
                    num++;
                }
            }
            Rect rect = RectUtility.CenteredAt(new Vector2Int(num, (int)(TraitRarity.Epic - trait.Rarity)) * 104f, 90f);
            rect.position += new Vector2(45f, 45f);
            rect.position += new Vector2(47f, 0f);
            return rect;
        }

        private static float GetColorFactor(TraitSpec trait)
        {
            if (Get.TraitManager.IsUnlocked(trait) || Get.TraitManager.IsChosen(trait))
            {
                return 1f;
            }
            return 0.45f;
        }

        private int GetNextTraitPrice()
        {
            float num = (float)Get.TraitManager.Unlocked.Count / (float)(Get.Specs.GetAll<TraitSpec>().Count - 1);
            return Calc.RoundToInt(Calc.RoundTo(Calc.Lerp(50f, 150f, num), 5f));
        }

        private float GetSelectionWeight(TraitSpec traitSpec)
        {
            switch (traitSpec.Rarity)
            {
                case TraitRarity.Common:
                    return 1f;
                case TraitRarity.Uncommon:
                    return 0.85f;
                case TraitRarity.Rare:
                    return 0.7f;
                case TraitRarity.Epic:
                    return 0.5f;
                default:
                    return 1f;
            }
        }

        private List<TraitSpec> allTraitsInOrder = new List<TraitSpec>();

        private const int TraitsPerBuy = 2;

        private const float TraitIconSize = 90f;

        private const float GapBetweenTraits = 14f;

        private static readonly Texture2D TraitBackground = Assets.Get<Texture2D>("Textures/UI/TraitBackground");
    }
}