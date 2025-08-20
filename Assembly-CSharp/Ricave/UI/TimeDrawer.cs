using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class TimeDrawer
    {
        public float ResolvingTurnsIconAlpha
        {
            get
            {
                return this.resolvingTurnsIconAlpha;
            }
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseDown && Event.current.type != EventType.MouseUp)
            {
                return;
            }
            if (Get.InLobby)
            {
                return;
            }
            Rect rect = new Rect(Widgets.VirtualWidth - 104f - 30f, 350f, 104f, 31f);
            string text = GameTime.TimeToString(true);
            if (text != this.lastTimeStr)
            {
                CachedGUI.SetDirty(4);
            }
            Rect rect2 = rect.ExpandedBy(3f);
            GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            if (Mouse.Over(rect2))
            {
                string text2 = "TimeTip".Translate(GameTime.TimeToString(false), Get.DayNightCycleManager.DayPart.GetLabelCap(), (Get.TurnManager.CurrentSequence / 12).ToStringCached(), ((Get.TurnManager.CurrentSequence - Get.TurnManager.InceptionSequence) / 12).ToStringCached());
                text2 = text2.AppendedInDoubleNewLine("{0}: {1}-{2}".Formatted(DayPart.Morning.GetLabelCap(), GameTime.TimeToString(5, 0, false), GameTime.TimeToString(7, 59, false)));
                text2 = text2.AppendedInNewLine("{0}: {1}-{2}".Formatted(DayPart.Day.GetLabelCap(), GameTime.TimeToString(8, 0, false), GameTime.TimeToString(17, 59, false)));
                text2 = text2.AppendedInNewLine("{0}: {1}-{2}".Formatted(DayPart.Evening.GetLabelCap(), GameTime.TimeToString(18, 0, false), GameTime.TimeToString(20, 59, false)));
                text2 = text2.AppendedInNewLine("{0}: {1}-{2}".Formatted(DayPart.Night.GetLabelCap(), GameTime.TimeToString(21, 0, false), GameTime.TimeToString(4, 59, false)));
                Get.Tooltips.RegisterTip(rect2, text2, new int?(835366480));
            }
            if (CachedGUI.BeginCachedGUI(rect, 4, true))
            {
                this.lastTimeStr = text;
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                Widgets.FontSizeScalable = 22;
                Widgets.Align = TextAnchor.UpperLeft;
                Widgets.Label(rect, text, true, null, null, false);
                Widgets.ResetAlign();
                Widgets.ResetFontSize();
                GUI.color = Color.white;
                GUIExtra.DrawTexture(rect.RightPart(rect.height).ContractedBy(3f), Get.DayNightCycleManager.DayPart.GetIcon());
            }
            CachedGUI.EndCachedGUI(1f, 1f);
            if (Event.current.type == EventType.Repaint)
            {
                if (Get.NowControlledActor.Spawned && !Get.TurnManager.IsPlayerTurn_CanChooseNextAction && Get.VisibilityCache.AnyNonNowControlledActorSeen)
                {
                    this.resolvingTurnsIconAlpha = Math.Min(this.resolvingTurnsIconAlpha + Clock.UnscaledDeltaTime * 10f, 1f);
                }
                else
                {
                    this.resolvingTurnsIconAlpha = Math.Max(this.resolvingTurnsIconAlpha - Clock.UnscaledDeltaTime * 10f, 0f);
                }
            }
            if (this.resolvingTurnsIconAlpha > 0f)
            {
                TimeDrawer.DrawResolvingTurnsIcon(new Rect(rect.x - 26f - 8f, rect.y + 1f, 26f, 26f), this.resolvingTurnsIconAlpha);
            }
            if (this.extraFlashingTextAlpha > 0f)
            {
                GUI.color = new Color(1f, 1f, 1f, this.extraFlashingTextAlpha);
                Widgets.FontSizeScalable = 22;
                Widgets.Align = TextAnchor.UpperRight;
                Rect rect3 = rect;
                rect3.xMax = rect3.xMin;
                rect3.xMin -= 300f;
                rect3.x -= 49f;
                Widgets.Label(rect3, this.extraFlashingText, true, null, null, false);
                Widgets.ResetAlign();
                Widgets.ResetFontSize();
                GUI.color = Color.white;
                if (Event.current.type == EventType.Repaint)
                {
                    this.extraFlashingTextAlpha = Math.Max(this.extraFlashingTextAlpha - Clock.UnscaledDeltaTime * 0.47f, 0f);
                }
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.Over(rect))
            {
                TimeDrawer.OpenContextMenu();
                Event.current.Use();
            }
            if (Get.UI.InventoryOpen)
            {
                string text3 = "Wait".Translate();
                int num = TimeDrawer.SimulateHPOffsetAfterTurns(Get.NowControlledActor, TimeDrawer.WaitTurnsOptions[TimeDrawer.WaitTurnsOptions.Length - 1]);
                if (num > 0)
                {
                    text3 = text3.AppendedWithSpace("({0})".Formatted("HP".Translate(num.ToStringOffset(true))));
                }
                float num2 = Widgets.CalcSize(text3).x + 20f;
                if (Widgets.Button(new Rect(rect.x - 26f - 8f - num2 - 2f, rect.y, num2, 31f), text3, true, null, null, true, true, true, true, null, true, true, null, false, null, null))
                {
                    TimeDrawer.OpenContextMenu();
                }
            }
            float num3 = Widgets.VirtualWidth - 30f - 31f;
            foreach (WorldSituation worldSituation in Get.WorldSituationsManager.Situations)
            {
                Rect rect4 = new Rect(num3, rect.y + 33f, 31f, 31f);
                ExpandingIconAnimation.Do(rect4, worldSituation.Icon, worldSituation.IconColor, worldSituation.TimeAdded, 1f, 0.6f, 0.55f);
                if (Mouse.Over(rect4))
                {
                    Get.Tooltips.RegisterTip(rect4, worldSituation, null, null);
                }
                GUIExtra.DrawHighlightIfMouseover(rect4, true, true, true, true, true);
                GUI.color = worldSituation.IconColor;
                GUIExtra.DrawTexture(rect4, worldSituation.Icon);
                GUI.color = Color.white;
                num3 -= 34f;
            }
        }

        public static void DrawResolvingTurnsIcon(Rect rect, float alpha = 1f)
        {
            if (alpha <= 0f)
            {
                return;
            }
            GUI.color = new Color(0.9f, 0.9f, 0.9f, alpha);
            GUIExtra.DrawTextureRotated(rect, TimeDrawer.ResolvingTurnsIcon, (float)((int)(Clock.UnscaledTime / 0.05f) * 45 % 360), null);
            GUI.color = Color.white;
        }

        private static void OpenContextMenu()
        {
            Get.WindowManager.OpenContextMenu(TimeDrawer.WaitTurnsOptions.Select<int, ValueTuple<string, Action>>(delegate (int x)
            {
                string text = "WaitTurns".Translate(StringUtility.TurnsString(x));
                int num = TimeDrawer.SimulateHPOffsetAfterTurns(Get.NowControlledActor, x);
                if (num <= -Get.NowControlledActor.HP)
                {
                    text = text.AppendedWithSpace("({0})".Formatted("WaitOptionDeath".Translate()));
                }
                else
                {
                    text = text.AppendedWithSpace("({0})".Formatted("HP".Translate(num.ToStringOffset(true))));
                }
                return new ValueTuple<string, Action>(text, delegate
                {
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    Get.PlannedPlayerActions.SetMultipleWait(x, 0.04f);
                });
            }).ToList<ValueTuple<string, Action>>(), "Wait".Translate());
        }

        public static int SimulateHPOffsetAfterTurns(Actor actor, int turns)
        {
            TimeDrawer.<> c__DisplayClass16_0 CS$<> 8__locals1;
            CS$<> 8__locals1.actor = actor;
            CS$<> 8__locals1.hasHunger = CS$<> 8__locals1.actor.ConditionsAccumulated.AnyOfSpec(Get.Condition_Hunger);
            CS$<> 8__locals1.initiallyHungry = CS$<> 8__locals1.actor.ConditionsAccumulated.AnyOfSpec(Get.Condition_Hungry);
            CS$<> 8__locals1.initiallyStarving = CS$<> 8__locals1.actor.ConditionsAccumulated.AnyOfSpec(Get.Condition_Starving);
            TimeDrawer.tmpRelevantConditions.Clear();
            foreach (Condition condition in CS$<> 8__locals1.actor.ConditionsAccumulated.AllConditions)
			{
                if (condition is Condition_DamageOverTimeBase || condition is Condition_HPRegen || condition.DisableNativeHPRegen)
                {
                    TimeDrawer.tmpRelevantConditions.Add(condition);
                }
            }
            int num = 0;
            int maxHP = CS$<> 8__locals1.actor.MaxHP;
            for (int i = 0; i < turns; i++)
            {
                int num2 = i + 1;
                bool flag = TimeDrawer.< SimulateHPOffsetAfterTurns > g__NativeHPRegenDisabledAfterTurns | 16_2(num2, ref CS$<> 8__locals1);
                for (int j = 0; j < TimeDrawer.tmpRelevantConditions.Count; j++)
                {
                    Condition condition2 = TimeDrawer.tmpRelevantConditions[j];
                    if (TimeDrawer.< SimulateHPOffsetAfterTurns > g__ExistsAfterTurns | 16_0(condition2, num2))
                    {
                        Condition_DamageOverTimeBase condition_DamageOverTimeBase = condition2 as Condition_DamageOverTimeBase;
                        if (condition_DamageOverTimeBase != null)
                        {
                            if (condition_DamageOverTimeBase.IntervalTurns == 0 || (condition_DamageOverTimeBase.TurnsPassed + num2) % condition_DamageOverTimeBase.IntervalTurns == 0)
                            {
                                int damage = condition_DamageOverTimeBase.Damage;
                                int num3 = damage;
                                int num4 = damage;
                                DamageUtility.ApplyDamageProtectionAndClamp(CS$<> 8__locals1.actor, null, condition_DamageOverTimeBase.DamageType, ref damage, ref num3, ref num4);
                                int num5 = num4;
                                num = Math.Max(num - num5, -CS$<> 8__locals1.actor.HP);
                                if (num == -CS$<> 8__locals1.actor.HP)
								{
                return num;
            }
        }
    }
						else
						{
							Condition_HPRegen condition_HPRegen = condition2 as Condition_HPRegen;
							if (condition_HPRegen != null && (!condition_HPRegen.Native || !flag) && (condition_HPRegen.IntervalTurns == 0 || (condition_HPRegen.Progress + num2) % condition_HPRegen.IntervalTurns == 0))
							{
								int amount = condition_HPRegen.Amount;
    num = Math.Min(num + amount, maxHP - CS$<>8__locals1.actor.HP);
							}
						}
					}
				}
			}
			TimeDrawer.tmpRelevantConditions.Clear();
return num;
		}

		public void OnSequenceAdded(ISequenceable sequenceable, int sequenceOffset)
{
    if (sequenceable == Get.NowControlledActor && sequenceOffset >= 24)
    {
        int num = Calc.RoundToInt((float)sequenceOffset / 12f);
        this.extraFlashingText = "TurnsCount".Translate(num.ToStringOffset(true));
        this.extraFlashingTextAlpha = 1f;
    }
}

[CompilerGenerated]
internal static bool < SimulateHPOffsetAfterTurns > g__ExistsAfterTurns | 16_0(Condition condition, int afterTurns)

        {
    return condition.TurnsLeft <= 0 || condition.TurnsLeft - afterTurns >= 0;
}

[CompilerGenerated]
internal static bool < SimulateHPOffsetAfterTurns > g__IsHungryOrStarvingAfterTurns | 16_1(int afterTurns, ref TimeDrawer.<> c__DisplayClass16_0 A_1)

        {
    return (A_1.initiallyHungry | A_1.initiallyStarving) || (A_1.hasHunger && A_1.actor.IsMainActor && Get.Player.Satiation - afterTurns <= 250);
}

[CompilerGenerated]
internal static bool < SimulateHPOffsetAfterTurns > g__NativeHPRegenDisabledAfterTurns | 16_2(int afterTurns, ref TimeDrawer.<> c__DisplayClass16_0 A_1)

        {
    if (TimeDrawer.< SimulateHPOffsetAfterTurns > g__IsHungryOrStarvingAfterTurns | 16_1(afterTurns, ref A_1))
    {
        return true;
    }
    for (int i = 0; i < TimeDrawer.tmpRelevantConditions.Count; i++)
    {
        if (TimeDrawer.< SimulateHPOffsetAfterTurns > g__ExistsAfterTurns | 16_0(TimeDrawer.tmpRelevantConditions[i], afterTurns) && TimeDrawer.tmpRelevantConditions[i].DisableNativeHPRegen)
        {
            return true;
        }
    }
    return false;
}

private string lastTimeStr;

private float resolvingTurnsIconAlpha;

private string extraFlashingText;

private float extraFlashingTextAlpha;

private static readonly Texture2D ResolvingTurnsIcon = Assets.Get<Texture2D>("Textures/UI/ResolvingTurns");

private const int Width = 104;

private const int FontSize = 22;

private const int Pad = 30;

private const int ResolvingTurnsIconSize = 26;

private static readonly int[] WaitTurnsOptions = new int[] { 1, 10, 25, 50 };

private static List<Condition> tmpRelevantConditions = new List<Condition>();
	}
}