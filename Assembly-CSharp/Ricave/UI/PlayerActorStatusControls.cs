using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class PlayerActorStatusControls
    {
        private static Condition_HPRegen NativeHPRegen
        {
            get
            {
                foreach (Condition condition in Get.NowControlledActor.Conditions.All)
                {
                    Condition_HPRegen condition_HPRegen = condition as Condition_HPRegen;
                    if (condition_HPRegen != null && condition_HPRegen.Native)
                    {
                        return condition_HPRegen;
                    }
                }
                return null;
            }
        }

        private static Condition_StaminaRegen NativeStaminaRegen
        {
            get
            {
                foreach (Condition condition in Get.NowControlledActor.Conditions.All)
                {
                    Condition_StaminaRegen condition_StaminaRegen = condition as Condition_StaminaRegen;
                    if (condition_StaminaRegen != null && condition_StaminaRegen.Native)
                    {
                        return condition_StaminaRegen;
                    }
                }
                return null;
            }
        }

        public static string PlayerLevelTip
        {
            get
            {
                return "PlayerLevelTip".Translate(LevelUtility.MaxHPPerLevelAdjusted.ToStringOffset(true), 1, "{0} / {1}".Formatted(Get.Player.ExperienceSinceLeveling.ToStringCached(), Get.Player.ExperienceBetweenLevels.ToStringCached()), Get.Player.Experience.ToStringCached());
            }
        }

        public static string PlayerExperienceTip
        {
            get
            {
                return "PlayerExperienceTip".Translate("{0} / {1}".Formatted(Get.Player.ExperienceSinceLeveling.ToStringCached(), Get.Player.ExperienceBetweenLevels.ToStringCached()), Get.Player.Level.ToStringCached(), Get.Player.Experience.ToStringCached());
            }
        }

        public static bool IsLowHP
        {
            get
            {
                return Get.NowControlledActor != null && PlayerActorStatusControls.IsActorOnLowHP(Get.NowControlledActor);
            }
        }

        public static float LowHPFlash
        {
            get
            {
                return Calc.PulseUnscaled(6f, 0.28f);
            }
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseDown && Event.current.type != EventType.MouseUp)
            {
                return;
            }
            if (Get.NowControlledActor == null)
            {
                return;
            }
            if (Get.InLobby)
            {
                PlayerActorStatusControls.DoLobbyItems();
                PlayerActorStatusControls.DoScoreLevel();
                return;
            }
            float num = 20f;
            Rect rect = new Rect(20f, num, PlayerActorStatusControls.ProgressBarsSize.x, PlayerActorStatusControls.ProgressBarsSize.y);
            float num2 = (PlayerActorStatusControls.IsLowHP ? PlayerActorStatusControls.LowHPFlash : 0f);
            Get.ProgressBarDrawer.Draw(rect, Get.NowControlledActor.HP, Get.NowControlledActor.MaxHP, PlayerActorStatusControls.PlayerHPBarColor.Lighter(num2), 1f, 1f, true, true, new int?(Get.NowControlledActor.InstanceID), TipSubjectDrawer_Entity.GetHPBarValueChangeDirection(Get.NowControlledActor), Get.InteractionManager.GetLostHPRangeForUI(Get.NowControlledActor), true, true, false, false, true, true, null);
            GUI.color = new Color(1f, 1f, 1f, 0.65f);
            GUI.DrawTexture(new Rect(20f, num, PlayerActorStatusControls.ProgressBarsSize.y, PlayerActorStatusControls.ProgressBarsSize.y).ContractedBy(2f), PlayerActorStatusControls.HPBarIcon);
            GUI.color = Color.white;
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            if (Mouse.Over(rect))
            {
                string text = "PlayerHealthTip".Translate(LevelUtility.MaxHPPerLevelAdjusted.ToStringOffset(true));
                Condition_HPRegen nativeHPRegen = PlayerActorStatusControls.NativeHPRegen;
                if (nativeHPRegen != null)
                {
                    text = text.AppendedInDoubleNewLine("PlayerHealthRegenTip".Translate(RichText.HP("HP".Translate(nativeHPRegen.Amount)), RichText.Turns(StringUtility.TurnsString(nativeHPRegen.IntervalTurns))));
                    if (Get.NowControlledActor.HP < Get.NowControlledActor.MaxHP && !nativeHPRegen.Disabled)
                    {
                        text = text.AppendedInDoubleNewLine("PlayerHealthRegenHealHPInTip".Translate(RichText.Turns(StringUtility.TurnsString(nativeHPRegen.TurnsToNextHeal))));
                    }
                }
                if (Get.NowControlledActor.HP > Get.NowControlledActor.MaxHP)
                {
                    text = text.AppendedInDoubleNewLine("LosingHPBecauseAboveMaxTip".Translate());
                }
                Get.Tooltips.RegisterTip(rect, text, null);
            }
            num += rect.height - 1f;
            if (TipSubjectDrawer_Entity.ShouldShowManaBar(Get.NowControlledActor))
            {
                Get.Player.HadManaBar = true;
                Rect rect2 = new Rect(20f, num, PlayerActorStatusControls.ProgressBarsSize.x, PlayerActorStatusControls.ProgressBarsSize.y);
                ProgressBarDrawer progressBarDrawer = Get.ProgressBarDrawer;
                Rect rect3 = rect2;
                int mana = Get.NowControlledActor.Mana;
                int maxMana = Get.NowControlledActor.MaxMana;
                Color manaBarColor = TipSubjectDrawer_Entity.ManaBarColor;
                float num3 = 1f;
                float num4 = 1f;
                bool flag = true;
                bool flag2 = true;
                int? num5 = new int?(100000000 + Get.NowControlledActor.InstanceID);
                ProgressBarDrawer.ValueChangeDirection manaBarValueChangeDirection = TipSubjectDrawer_Entity.GetManaBarValueChangeDirection(Get.NowControlledActor);
                InteractionManager.PossibleInteraction? possibleInteraction = Get.InteractionManager.LastPossibleInteraction;
                int? num6;
                if (possibleInteraction == null)
                {
                    num6 = null;
                }
                else
                {
                    IUsable usable = possibleInteraction.GetValueOrDefault().usable;
                    num6 = ((usable != null) ? new int?(usable.ManaCost) : null);
                }
                int? num7 = num6;
                progressBarDrawer.Draw(rect3, mana, maxMana, manaBarColor, num3, num4, flag, flag2, num5, manaBarValueChangeDirection, (num7 != null) ? new IntRange?((IntRange)num7.GetValueOrDefault()) : null, false, false, false, false, true, true, null);
                GUI.color = new Color(1f, 1f, 1f, 0.65f);
                GUI.DrawTexture(new Rect(20f, num, PlayerActorStatusControls.ProgressBarsSize.y, PlayerActorStatusControls.ProgressBarsSize.y).ContractedBy(2f), PlayerActorStatusControls.ManaBarIcon);
                GUI.color = Color.white;
                GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
                if (Mouse.Over(rect2))
                {
                    string text2 = "PlayerManaTip".Translate(1);
                    if (Get.NowControlledActor.Mana > Get.NowControlledActor.MaxMana)
                    {
                        text2 = text2.AppendedInDoubleNewLine("LosingManaBecauseAboveMaxTip".Translate());
                    }
                    Get.Tooltips.RegisterTip(rect2, text2, null);
                }
                num += rect2.height - 1f;
            }
            bool flag3 = Get.MainActor == Get.NowControlledActor;
            bool flag4 = Get.NowControlledActor.ConditionsAccumulated.AnyOfSpec(Get.Condition_Hunger);
            if (TipSubjectDrawer_Entity.ShouldShowStaminaBar(Get.NowControlledActor))
            {
                Get.Player.HadStaminaBar = true;
                Rect rect4 = new Rect(20f, num, PlayerActorStatusControls.ProgressBarsSize.x, PlayerActorStatusControls.ProgressBarsSize.y);
                ProgressBarDrawer progressBarDrawer2 = Get.ProgressBarDrawer;
                Rect rect5 = rect4;
                int stamina = Get.NowControlledActor.Stamina;
                int maxStamina = Get.NowControlledActor.MaxStamina;
                Color staminaBarColor = TipSubjectDrawer_Entity.StaminaBarColor;
                float num8 = 1f;
                float num9 = 1f;
                bool flag5 = true;
                bool flag6 = true;
                int? num10 = new int?(200000000 + Get.NowControlledActor.InstanceID);
                ProgressBarDrawer.ValueChangeDirection staminaBarValueChangeDirection = TipSubjectDrawer_Entity.GetStaminaBarValueChangeDirection(Get.NowControlledActor);
                InteractionManager.PossibleInteraction? possibleInteraction = Get.InteractionManager.LastPossibleInteraction;
                int? num11;
                if (possibleInteraction == null)
                {
                    num11 = null;
                }
                else
                {
                    IUsable usable2 = possibleInteraction.GetValueOrDefault().usable;
                    num11 = ((usable2 != null) ? new int?(usable2.StaminaCost) : null);
                }
                int? num7 = num11;
                progressBarDrawer2.Draw(rect5, stamina, maxStamina, staminaBarColor, num8, num9, flag5, flag6, num10, staminaBarValueChangeDirection, (num7 != null) ? new IntRange?((IntRange)num7.GetValueOrDefault()) : null, false, false, !flag3 && !flag4, !flag3 && !flag4, true, true, null);
                GUI.color = new Color(1f, 1f, 1f, 0.65f);
                GUI.DrawTexture(new Rect(20f, num, PlayerActorStatusControls.ProgressBarsSize.y, PlayerActorStatusControls.ProgressBarsSize.y).ContractedBy(2f), PlayerActorStatusControls.StaminaBarIcon);
                GUI.color = Color.white;
                GUIExtra.DrawHighlightIfMouseover(rect4, true, true, true, true, true);
                if (Mouse.Over(rect4))
                {
                    string text3 = "PlayerStaminaTip".Translate();
                    Condition_StaminaRegen nativeStaminaRegen = PlayerActorStatusControls.NativeStaminaRegen;
                    if (nativeStaminaRegen != null)
                    {
                        text3 = text3.AppendedInDoubleNewLine("PlayerStaminaRegenTip".Translate(RichText.Stamina("Stamina".Translate(nativeStaminaRegen.Amount)), RichText.Turns(StringUtility.TurnsString(nativeStaminaRegen.IntervalTurns))));
                        if (Get.NowControlledActor.Stamina < Get.NowControlledActor.MaxStamina && !nativeStaminaRegen.Disabled)
                        {
                            text3 = text3.AppendedInDoubleNewLine("PlayerStaminaRegenNextRegenInTip".Translate(RichText.Turns(StringUtility.TurnsString(nativeStaminaRegen.TurnsToNextRegen))));
                        }
                        if (nativeStaminaRegen.DisabledBecauseNotTouchingGround)
                        {
                            text3 = text3.AppendedInDoubleNewLine("StaminaRegenDisabledBecausePlayerNotTouchingGround".Translate());
                        }
                    }
                    if (Get.NowControlledActor.Stamina > Get.NowControlledActor.MaxStamina)
                    {
                        text3 = text3.AppendedInDoubleNewLine("LosingStaminaBecauseAboveMaxTip".Translate());
                    }
                    Get.Tooltips.RegisterTip(rect4, text3, null);
                }
                num += rect4.height - 1f;
            }
            if (flag4)
            {
                Rect rect6 = new Rect(20f, num, PlayerActorStatusControls.ProgressBarsSize.x, 21f);
                ProgressBarDrawer progressBarDrawer3 = Get.ProgressBarDrawer;
                Rect rect7 = rect6;
                int satiation = Get.Player.Satiation;
                int num12 = 600;
                Color color = new Color(0.4f, 0.4f, 0f);
                float num13 = 1f;
                float num14 = 1f;
                bool flag7 = true;
                bool flag8 = false;
                int? num15 = new int?(-2);
                ProgressBarDrawer.ValueChangeDirection valueChangeDirection = ProgressBarDrawer.ValueChangeDirection.Constant;
                bool flag9 = !flag3;
                bool flag10 = !flag3;
                progressBarDrawer3.Draw(rect7, satiation, num12, color, num13, num14, flag7, flag8, num15, valueChangeDirection, null, false, false, flag9, flag10, true, false, null);
                GUI.color = new Color(1f, 1f, 1f, 0.65f);
                GUI.DrawTexture(new Rect(20f, num, PlayerActorStatusControls.ProgressBarsSize.y, 21f).ContractedToSquare(), PlayerActorStatusControls.HungerBarIcon);
                GUI.color = Color.white;
                GUIExtra.DrawHighlightIfMouseover(rect6, true, true, true, true, true);
                if (Mouse.Over(rect6))
                {
                    Get.Tooltips.RegisterTip(rect6, "PlayerHungerTip".Translate(Get.Player.Satiation.ToStringCached(), 600.ToStringCached(), 250.ToStringCached(), 0.ToStringCached()), null);
                }
                num += rect6.height;
            }
            if (flag3)
            {
                Rect rect8 = new Rect(20f, num, PlayerActorStatusControls.ProgressBarsSize.x, 21f);
                Get.ProgressBarDrawer.Draw(rect8, Get.Player.ExperienceSinceLeveling, Get.Player.ExperienceBetweenLevels, PlayerActorStatusControls.GetAnimatedExperienceBarColor(), 1f, 1f, true, false, new int?(-1), ProgressBarDrawer.ValueChangeDirection.Constant, null, false, false, true, true, true, false, null);
                GUI.color = new Color(1f, 1f, 1f, 0.65f);
                GUI.DrawTexture(new Rect(20f, num, PlayerActorStatusControls.ProgressBarsSize.y, 21f).ContractedToSquare(), PlayerActorStatusControls.ExperienceBarIcon);
                GUI.color = Color.white;
                GUIExtra.DrawHighlightIfMouseover(rect8, true, true, true, true, true);
                if (Mouse.Over(rect8))
                {
                    Get.Tooltips.RegisterTip(rect8, PlayerActorStatusControls.PlayerExperienceTip, null);
                }
                num += rect8.height;
            }
            bool inventoryOpen = Get.UI.InventoryOpen;
            TipSubjectDrawer.DoConditions(Get.NowControlledActor.ConditionsAccumulated.ActorConditionDrawRequestsPlusExtra, 20f, ref num, Widgets.VirtualWidth - 20f, false, 20f, 1.5f, true, true, inventoryOpen);
            if (Get.NowControlledActor.Inventory.EquippedWeapon != null)
            {
                num += 20f;
                float num16 = num;
                Rect rect9 = TipSubjectDrawer_Entity.DoEntityBigLabelAndIcon(Get.NowControlledActor.Inventory.EquippedWeapon, 20f, Widgets.VirtualWidth - 20f, ref num, false, !Get.NowControlledActor.CanUse(Get.NowControlledActor.Inventory.EquippedWeapon, null, true, null), true, inventoryOpen);
                TipSubjectDrawer.DoUseEffects(Get.NowControlledActor.Inventory.EquippedWeapon.UseEffects.AllDrawRequests, 20f, ref num, Widgets.VirtualWidth - 20f, false, 9f, 1.5f, true, inventoryOpen);
                TipSubjectDrawer.DoConditions(Get.NowControlledActor.Inventory.EquippedWeapon.ConditionsEquipped.AllDrawRequests, 20f, ref num, Widgets.VirtualWidth - 20f, false, 9f, 1.5f, false, true, inventoryOpen);
                PlayerActorStatusControls.DoSection(num16, num);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.Over(rect9))
                {
                    bool flag10;
                    List<ValueTuple<string, Action, string>> possibleInteractions = Get.NowControlledActor.Inventory.GetPossibleInteractions(Get.NowControlledActor.Inventory.EquippedWeapon, out flag10, false);
                    Get.WindowManager.OpenContextMenu(possibleInteractions, Get.NowControlledActor.Inventory.EquippedWeapon.LabelCap);
                    Event.current.Use();
                }
                ItemSlotDrawer.HandleInventoryManagementSpecialClicks(rect9, Get.NowControlledActor.Inventory.EquippedWeapon);
            }
            else
            {
                foreach (NativeWeapon nativeWeapon in Get.NowControlledActor.NativeWeapons)
                {
                    num += 20f;
                    float num17 = num;
                    ITipSubject tipSubject = nativeWeapon;
                    float num18 = 20f;
                    float num19 = Widgets.VirtualWidth - 20f;
                    bool flag11 = false;
                    bool flag12 = !Get.NowControlledActor.CanUse(nativeWeapon, null, true, null);
                    bool flag13 = true;
                    bool flag10 = inventoryOpen;
                    TipSubjectDrawer.DoBigLabelAndIcon(tipSubject, num18, num19, ref num, flag11, flag12, flag13, null, flag10);
                    TipSubjectDrawer.DoUseEffects(nativeWeapon.UseEffects.AllDrawRequests, 20f, ref num, Widgets.VirtualWidth - 20f, false, 9f, 1.5f, true, inventoryOpen);
                    PlayerActorStatusControls.DoSection(num17, num);
                }
            }
            foreach (Item item in Get.NowControlledActor.Inventory.EquippedItems.ToTemporaryList<Item>())
            {
                if (item != Get.NowControlledActor.Inventory.EquippedWeapon)
                {
                    num += 20f;
                    float num20 = num;
                    Rect rect10 = TipSubjectDrawer_Entity.DoEntityBigLabelAndIcon(item, 20f, Widgets.VirtualWidth - 20f, ref num, false, false, true, inventoryOpen);
                    TipSubjectDrawer.DoConditions(item.ConditionsEquipped.AllDrawRequests, 20f, ref num, Widgets.VirtualWidth - 20f, false, 9f, 1.5f, false, true, inventoryOpen);
                    PlayerActorStatusControls.DoSection(num20, num);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.Over(rect10))
                    {
                        bool flag10;
                        List<ValueTuple<string, Action, string>> possibleInteractions2 = Get.NowControlledActor.Inventory.GetPossibleInteractions(item, out flag10, false);
                        Get.WindowManager.OpenContextMenu(possibleInteractions2, item.LabelCap);
                        Event.current.Use();
                    }
                    ItemSlotDrawer.HandleInventoryManagementSpecialClicks(rect10, item);
                }
            }
            if (flag3)
            {
                PlayerActorStatusControls.DoLevelUpAnimation();
                GUI.color = PlayerActorStatusControls.LevelColor;
                Widgets.FontSizeScalable = 28;
                Rect rect11 = Widgets.LabelCenteredV(PlayerActorStatusControls.LevelLabelOffset, Get.Player.Level.ToStringCached(), true, null, null, true).ExpandedBy(3f);
                if (Mouse.Over(rect11))
                {
                    Get.Tooltips.RegisterTip(rect11, PlayerActorStatusControls.PlayerLevelTip, null);
                }
                GUI.color = Color.white;
                Widgets.ResetFontSize();
            }
            if (Event.current.type == EventType.Repaint)
            {
                float num21 = ((Clock.UnscaledTime - Get.Player.LastExpGainTime < 3.2f) ? 1f : 0f);
                this.centerExpBarAlpha = Calc.StepTowards(this.centerExpBarAlpha, num21, Clock.UnscaledDeltaTime * 2.5f);
            }
            if (this.centerExpBarAlpha > 0f)
            {
                GUI.color = new Color(1f, 1f, 1f, this.centerExpBarAlpha);
                Widgets.FontBold = true;
                Widgets.LabelCentered(Widgets.ScreenRect.TopCenter().WithAddedY(87f), "{0}: {1}".Formatted("Level".Translate(), Get.Player.Level.ToStringCached()), true, null, null, false, false, false, null);
                Widgets.FontBold = false;
                GUI.color = Color.white;
                ProgressBarDrawer progressBarDrawer4 = Get.ProgressBarDrawer;
                Rect rect12 = Widgets.ScreenRect.CutFromTop(105f).TopPart(26f).ResizedToWidth(420f);
                int experienceSinceLeveling = Get.Player.ExperienceSinceLeveling;
                int experienceBetweenLevels = Get.Player.ExperienceBetweenLevels;
                Color animatedExperienceBarColor = PlayerActorStatusControls.GetAnimatedExperienceBarColor();
                float num22 = this.centerExpBarAlpha;
                float num23 = 1f;
                bool flag14 = true;
                bool flag15 = false;
                int? num24 = new int?(-1);
                ProgressBarDrawer.ValueChangeDirection valueChangeDirection2 = ProgressBarDrawer.ValueChangeDirection.Constant;
                string text4 = "{0} {1}/{2}".Formatted("ExperienceShort".Translate(), Get.Player.ExperienceSinceLeveling.ToStringCached(), Get.Player.ExperienceBetweenLevels.ToStringCached());
                progressBarDrawer4.Draw(rect12, experienceSinceLeveling, experienceBetweenLevels, animatedExperienceBarColor, num22, num23, flag14, flag15, num24, valueChangeDirection2, null, true, true, true, true, true, true, text4);
            }
        }

        private static void DoSection(float fromY, float toY)
        {
            if (fromY == toY)
            {
                return;
            }
            GUIExtra.DrawRect(new Rect(9f, fromY, 5f, toY - fromY), new Color(1f, 1f, 1f, 0.2f));
        }

        private static void DoLobbyItems()
        {
            float num = 20f;
            PlayerActorStatusControls.DoLobbyItem(Get.Entity_Stardust, Get.TotalLobbyItems.Stardust, "StardustTip".Translate(), ref num);
            if (Get.TotalLobbyItems.Diamonds > 0)
            {
                PlayerActorStatusControls.DoLobbyItem(Get.Entity_Diamond, Get.TotalLobbyItems.Diamonds, Get.Entity_Diamond.DescriptionAdjusted, ref num);
            }
            int count = Get.TotalLobbyItems.GetCount(Get.Entity_PuzzlePiece);
            if (count > 0 && count < 10)
            {
                string text = "PuzzlePiecesTip".Translate(10);
                if (count < 10)
                {
                    text = text.AppendedInDoubleNewLine("PuzzlePiecesTip_NextPuzzlePiece".Translate(count + 1));
                }
                PlayerActorStatusControls.DoLobbyItem(Get.Entity_PuzzlePiece, count, text, ref num);
            }
            int count2 = Get.TotalLobbyItems.GetCount(Get.Entity_PetRatCheese);
            if (count2 > 0)
            {
                PlayerActorStatusControls.DoLobbyItem(Get.Entity_PetRatCheese, count2, Get.Entity_PetRatCheese.DescriptionAdjusted, ref num);
            }
            if (Clock.TimeSinceSceneLoad > 2.6f && Get.TotalLobbyItems.LastRunLobbyItemsForAnimation.Count != 0)
            {
                Get.TotalLobbyItems.OnLastRunLobbyItemsAnimationPlayed();
            }
        }

        private static void DoScoreLevel()
        {
            Rect rect = new Rect(Widgets.VirtualWidth - 90f - 25f, 25f, 90f, 90f);
            Rect rect2 = rect.ExpandedBy(3f);
            GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            if (Mouse.Over(rect2))
            {
                Get.Tooltips.RegisterTip(rect2, "ScoreLevelTip".Translate("{0} / {1}".Formatted(Get.Progress.ScoreSinceLeveling.ToStringCached(), Get.Progress.ScoreBetweenLevels.ToStringCached()), Get.Progress.Score.ToStringCached()), null);
            }
            PlayerActorStatusControls.scoreProgressBar.FinishInstantly();
            PlayerActorStatusControls.scoreProgressBar.Do(rect, 1f);
        }

        private static void DoLobbyItem(EntitySpec itemSpec, int count, string tip, ref float curY)
        {
            Rect rect = new Rect(20f, curY, 60f, 60f);
            Rect rect2 = rect;
            rect2.width += 90f;
            rect2 = rect2.ExpandedBy(3f);
            GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            if (Mouse.Over(rect2))
            {
                Get.Tooltips.RegisterTip(rect2, tip, null);
            }
            GUI.color = itemSpec.IconColorAdjusted;
            GUI.DrawTexture(rect, itemSpec.IconAdjusted);
            GUI.color = Color.white;
            float num = Calc.ResolveFadeIn(Clock.TimeSinceSceneLoad - 0.28f, 1.05f);
            int num2 = count - Calc.RoundToInt((float)PlayerActorStatusControls.GetNewlyCollectedLobbyItemsCount(itemSpec) * (1f - num));
            string text = ((itemSpec == Get.Entity_PuzzlePiece) ? "{0}/{1}".Formatted(num2, 10) : num2.ToStringCached());
            Widgets.FontSizeScalable = 22;
            Widgets.LabelCenteredV(new Vector2(rect.xMax + 10f, rect.center.y), text, true, null, null, false);
            Widgets.ResetFontSize();
            ExpandingIconAnimation.Do(rect, itemSpec.IconAdjusted, itemSpec.IconColorAdjusted, Get.TotalLobbyItems.GetLastCountChangedTime(itemSpec), 0.9f, 0.6f, 0.55f);
            PlayerActorStatusControls.DoNewlyCollectedLobbyItemsAnimation(itemSpec, rect);
            curY += rect.height + 8f;
        }

        private static int GetNewlyCollectedLobbyItemsCount(EntitySpec itemSpec)
        {
            foreach (ValueTuple<EntitySpec, int> valueTuple in Get.TotalLobbyItems.LastRunLobbyItemsForAnimation)
            {
                if (valueTuple.Item1 == itemSpec)
                {
                    return Math.Max(valueTuple.Item2, 0);
                }
            }
            return 0;
        }

        private static void DoNewlyCollectedLobbyItemsAnimation(EntitySpec itemSpec, Rect rect)
        {
            int num = PlayerActorStatusControls.GetNewlyCollectedLobbyItemsCount(itemSpec);
            if (num <= 0)
            {
                return;
            }
            if (itemSpec == Get.Entity_Stardust)
            {
                num /= 5;
            }
            num = Math.Min(num, 23);
            for (int i = 0; i < num; i++)
            {
                float num2 = 0.28f + (float)i * 1.05f / (float)num;
                float num3 = Calc.ResolveFadeIn(Clock.TimeSinceSceneLoad - num2, 0.55f);
                if (num3 > 0f)
                {
                    if (num3 >= 1f)
                    {
                        Dictionary<EntitySpec, int> lastRunLobbyItemsAnimationSoundsPlayed = Get.TotalLobbyItems.LastRunLobbyItemsAnimationSoundsPlayed;
                        if (lastRunLobbyItemsAnimationSoundsPlayed.GetOrDefault(itemSpec, 0) <= i)
                        {
                            Get.Sound_Counter.PlayOneShot(null, 1f, 1f);
                            lastRunLobbyItemsAnimationSoundsPlayed.SetOrIncrement(itemSpec, 1);
                        }
                    }
                    else
                    {
                        float num4 = num3 * 0.87f;
                        if (num4 > 0f)
                        {
                            Vector2 vector = (rect.center + new Vector2(300f, 150f) * (1f - num3)).WithAddedY(150f * Calc.Pow(1f - num3, 2f));
                            GUI.color = itemSpec.IconColorAdjusted.WithAlphaFactor(num4);
                            GUI.DrawTexture(RectUtility.CenteredAt(vector, rect.width + rect.width * (1f - num3) * 0.72f), itemSpec.IconAdjusted);
                            GUI.color = Color.white;
                        }
                    }
                }
            }
        }

        private static void DoLevelUpAnimation()
        {
            if (Clock.UnscaledTime - Get.Player.LastLevelUpTime > 1.2f)
            {
                return;
            }
            string text = Get.Player.Level.ToStringCached();
            Vector2 levelLabelOffset = PlayerActorStatusControls.LevelLabelOffset;
            Widgets.FontSizeScalable = 28;
            levelLabelOffset.x += Widgets.CalcSize(text).x / 2f;
            Widgets.ResetFontSize();
            ExpandingLabelAnimation.Do(levelLabelOffset, text, 28, PlayerActorStatusControls.LevelColor, Get.Player.LastLevelUpTime, 1f, 1.2f, 1f, false);
        }

        private static Color GetAnimatedExperienceBarColor()
        {
            float num = Calc.ResolveFadeInStayOut(Clock.UnscaledTime - Get.CameraEffects.LastExperienceParticleArrivedTime, 0.3f, 0f, 0.3f);
            return PlayerActorStatusControls.ExperienceBarColor.Lighter(num * 0.8f);
        }

        public static bool IsActorOnLowHP(Actor actor)
        {
            return (float)actor.HP <= (float)actor.MaxHP * 0.2f;
        }

        private float centerExpBarAlpha;

        private static readonly Vector2 ProgressBarsSize = new Vector3(250f, 32f);

        public static readonly Vector2 LevelLabelOffset = new Vector2(283f, 41f);

        private const float ExperienceBarHeight = 21f;

        public const int Margin = 20;

        private const int Gap = 20;

        private const int SmallGap = 9;

        public static readonly Color ExperienceBarColor = new Color(0.65f, 0.65f, 0f);

        public const int LevelFontSize = 28;

        public static readonly Color LevelColor = new Color(0.7f, 0.7f, 0.7f);

        public static readonly Color PlayerHPBarColor = Faction.DefaultHostileColor;

        private const float NewLobbyItemsAnimation_AllDuration = 1.05f;

        private const float NewLobbyItemsAnimation_StartDelay = 0.28f;

        private const float NewLobbyItemsAnimation_SingleDuration = 0.55f;

        private static readonly Texture2D HPBarIcon = Assets.Get<Texture2D>("Textures/UI/Bars/HPBar");

        private static readonly Texture2D ManaBarIcon = Assets.Get<Texture2D>("Textures/UI/Bars/ManaBar");

        private static readonly Texture2D StaminaBarIcon = Assets.Get<Texture2D>("Textures/UI/Bars/StaminaBar");

        private static readonly Texture2D HungerBarIcon = Assets.Get<Texture2D>("Textures/UI/Bars/HungerBar");

        private static readonly Texture2D ExperienceBarIcon = Assets.Get<Texture2D>("Textures/UI/Bars/ExperienceBar");

        public const float LowHPPctThreshold = 0.2f;

        private static ScoreProgressBar scoreProgressBar = new ScoreProgressBar(11);
    }
}