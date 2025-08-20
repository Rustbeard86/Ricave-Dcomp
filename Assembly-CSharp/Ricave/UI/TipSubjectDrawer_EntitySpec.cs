using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class TipSubjectDrawer_EntitySpec
    {
        public static void DoEntitySpecSpecificParts(EntitySpec entitySpec, float width, ref float curY, bool dontDraw = false)
        {
            TipSubjectDrawer_EntitySpec.<> c__DisplayClass0_0 CS$<> 8__locals1;
            CS$<> 8__locals1.width = width;
            CS$<> 8__locals1.dontDraw = dontDraw;
            TipSubjectDrawer_EntitySpec.DoEntitySpecBigLabelAndIcon(entitySpec, 0f, CS$<> 8__locals1.width, ref curY, CS$<> 8__locals1.dontDraw);
            if (!entitySpec.Description.NullOrEmpty())
            {
                TipSubjectDrawer.DoLabel(RichText.Italics(entitySpec.Description), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
            if (entitySpec.IsItem)
            {
                TipSubjectDrawer.DoManaAndStaminaCost(entitySpec.Item.ManaCost, entitySpec.Item.StaminaCost, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                if (entitySpec.Item.DefaultUseEffects != null && entitySpec.Item.DefaultUseEffects.Any)
                {
                    TipSubjectDrawer.DoMissAndCritChance(entitySpec.Item.MissChance, entitySpec.Item.CritChance, null, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                    TipSubjectDrawer.DoUseEffects(entitySpec.Item.DefaultUseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                }
                if (entitySpec.Item.DefaultConditionsEquipped != null)
                {
                    TipSubjectDrawer.DoConditions(entitySpec.Item.DefaultConditionsEquipped.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, false, true);
                }
                if (entitySpec.Item.DefaultUseEffects != null && entitySpec.Item.DefaultUseEffects.Any && entitySpec.Item.UseRange != 0)
                {
                    TipSubjectDrawer.DoUseRange(entitySpec.Item.UseRange, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                }
            }
            else if (entitySpec.IsActor)
            {
                if (entitySpec.Actor.DefaultConditions != null)
                {
                    List<ConditionDrawRequest> allDrawRequests = entitySpec.Actor.DefaultConditions.AllDrawRequests;
                    if (allDrawRequests.Count != 0)
                    {
                        TipSubjectDrawer.DoSection("Conditions".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        TipSubjectDrawer.DoConditions(allDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, false, true);
                    }
                }
                if (entitySpec.Actor.NativeWeapons.Count != 0)
                {
                    TipSubjectDrawer.DoSection("Weapon".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                    foreach (NativeWeaponProps nativeWeaponProps in entitySpec.Actor.NativeWeapons)
                    {
                        TipSubjectDrawer.DoBigLabelAndIcon(nativeWeaponProps.LabelCap, NativeWeapon.NonPlayerNativeWeaponIcon, 0f, CS$<> 8__locals1.width, ref curY, CS$<> 8__locals1.dontDraw);
                        if (nativeWeaponProps.DefaultUseEffects != null)
                        {
                            if (nativeWeaponProps.DefaultUseEffects.Any)
                            {
                                TipSubjectDrawer.DoMissAndCritChance(nativeWeaponProps.MissChance, nativeWeaponProps.CritChance, null, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                            }
                            TipSubjectDrawer.DoUseEffects(nativeWeaponProps.DefaultUseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                            if (nativeWeaponProps.DefaultUseEffects.Any && nativeWeaponProps.UseRange != 0)
                            {
                                TipSubjectDrawer.DoUseRange(nativeWeaponProps.UseRange, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                            }
                        }
                        TipSubjectDrawer.DoSequencePerUseMultiplier(nativeWeaponProps.SequencePerUseMultiplier, TipSubjectDrawer.SequencePerUseUsableType.NativeWeapon, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                    }
                }
                if (entitySpec.Actor.DefaultSpells.Count != 0)
                {
                    TipSubjectDrawer.DoSection("Spells".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                    foreach (SpellSpec spellSpec in entitySpec.Actor.DefaultSpells)
                    {
                        TipSubjectDrawer.DoBigLabelAndIcon(spellSpec, 0f, CS$<> 8__locals1.width, ref curY, CS$<> 8__locals1.dontDraw, false, false, null, true);
                        TipSubjectDrawer.DoManaAndStaminaCost(spellSpec.ManaCost, spellSpec.StaminaCost, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        if (spellSpec.DefaultUseEffects != null)
                        {
                            TipSubjectDrawer.DoUseEffects(spellSpec.DefaultUseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                        }
                        if (spellSpec.DefaultUseEffects != null && spellSpec.DefaultUseEffects.Any && spellSpec.UseRange != 0)
                        {
                            TipSubjectDrawer.DoUseRange(spellSpec.UseRange, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        }
                    }
                }
                if (entitySpec.Actor.UsableOnTalk != null && entitySpec.Actor.UsableOnTalk.DefaultUseEffects.Any)
                {
                    TipSubjectDrawer.DoSection("Talk".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                    TipSubjectDrawer.DoUseEffects(entitySpec.Actor.UsableOnTalk.DefaultUseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                }
                if (entitySpec.Actor.UsableOnDeath != null && entitySpec.Actor.UsableOnDeath.DefaultUseEffects.Any)
                {
                    TipSubjectDrawer.DoSection("WhenDies".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                    TipSubjectDrawer.DoUseEffects(entitySpec.Actor.UsableOnDeath.DefaultUseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                }
                TipSubjectDrawer_EntitySpec.<> c__DisplayClass0_1 CS$<> 8__locals2;
                CS$<> 8__locals2.miscSectionDone = false;
                if (Get.Player.Faction == null || entitySpec.Actor.DefaultFaction != Get.Player.Faction.Spec)
                {
                    TipSubjectDrawer_EntitySpec.< DoEntitySpecSpecificParts > g__DoMiscSection | 0_0(ref curY, ref CS$<> 8__locals1, ref CS$<> 8__locals2);
                    TipSubjectDrawer.DoLabel("KilledExperienceTip".Translate(entitySpec.Actor.KilledExperience), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                }
                if (entitySpec.Actor.AI != null)
                {
                    TipSubjectDrawer_EntitySpec.< DoEntitySpecSpecificParts > g__DoMiscSection | 0_0(ref curY, ref CS$<> 8__locals1, ref CS$<> 8__locals2);
                    TipSubjectDrawer.DoLabel(RichText.AI("AITip".Translate(entitySpec.Actor.AI.LabelCap)), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                }
            }
            else if (entitySpec.IsStructure)
            {
                TipSubjectDrawer.DoManaAndStaminaCost(entitySpec.Structure.ManaCost, entitySpec.Structure.StaminaCost, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                if (entitySpec.Structure.DefaultUseEffects != null && entitySpec.Structure.DefaultUseEffects.Any)
                {
                    TipSubjectDrawer.DoUseEffects(entitySpec.Structure.DefaultUseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                }
            }
            if (entitySpec.IsItem && entitySpec.Item.CooldownTurns != 0)
            {
                TipSubjectDrawer.DoCooldown(entitySpec.Item.CooldownTurns, null, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
            }
            else if (entitySpec.IsStructure && entitySpec.Structure.CooldownTurns != 0)
            {
                TipSubjectDrawer.DoCooldown(entitySpec.Structure.CooldownTurns, null, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
            }
            if (entitySpec.IsItem)
            {
                TipSubjectDrawer.DoSequencePerUseMultiplier(entitySpec.Item.SequencePerUseMultiplier, TipSubjectDrawer.SequencePerUseUsableType.Item, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
            }
            else if (entitySpec.IsStructure)
            {
                TipSubjectDrawer.DoSequencePerUseMultiplier(entitySpec.Structure.SequencePerUseMultiplier, TipSubjectDrawer.SequencePerUseUsableType.Structure, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
            }
            if ((entitySpec.IsItem && entitySpec.Item.AllowUseOnlyIfNotSeenByHostiles) || (entitySpec.IsStructure && entitySpec.Structure.AllowUseOnlyIfNotSeenByHostiles))
            {
                TipSubjectDrawer.DoLabel(RichText.Grayed("AllowUseOnlyIfNotSeenByHostilesTip".Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
            if (entitySpec.IsStructure && entitySpec.Structure.Flammable)
            {
                TipSubjectDrawer.DoLabel(RichText.Grayed("FlammableEntityTip".Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
            if (entitySpec.IsStructure && entitySpec.Structure.Fragile)
            {
                TipSubjectDrawer.DoLabel(RichText.Grayed("FragileEntityTip".Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
        }

        public static void DoEntitySpecBigLabelAndIcon(EntitySpec entitySpec, float x, float width, ref float curY, bool dontDraw = false)
        {
            float num = 20f;
            bool flag = entitySpec.MaxHP != 0;
            bool flag2 = flag && TipSubjectDrawer_EntitySpec.ShouldShowManaBar(entitySpec);
            bool flag3 = flag && TipSubjectDrawer_EntitySpec.ShouldShowStaminaBar(entitySpec);
            ValueTuple<string, Color> labelAndColor = ItemRarityUtility.GetLabelAndColor(entitySpec, false);
            bool flag4 = !flag && labelAndColor.Item1.NullOrEmpty();
            if (!dontDraw)
            {
                Rect rect = new Rect(x, curY + (flag2 ? (num / 2f) : 0f) + (flag3 ? (num / 2f) : 0f), 40f, 40f);
                Rect rect2 = rect;
                rect2.x = x + width - 40f;
                object obj = entitySpec.IsActor && entitySpec.Actor.DefaultFaction != null;
                float num2 = width - rect.width - 10f;
                object obj2 = obj;
                if (obj2 != null)
                {
                    num2 -= 50f;
                }
                string text = RichText.Label(entitySpec, true);
                if (!flag4)
                {
                    text = text.Truncate(num2);
                }
                float num3 = (labelAndColor.Item1.NullOrEmpty() ? 0f : Widgets.CalcSize(labelAndColor.Item1).x);
                Rect rect3 = new Rect(x, curY, 50f + Math.Min(width, Math.Max(Widgets.CalcSize(text).x, num3)) + 3f, 40f);
                Get.Tooltips.RegisterTip(rect3, entitySpec, null, null);
                GUIExtra.DrawHighlightIfMouseover(rect3, true, true, true, true, true);
                GUI.color = entitySpec.IconColorAdjustedIfIdentified;
                GUIExtra.DrawTexture(rect, entitySpec.IconAdjustedIfIdentified);
                GUI.color = Color.white;
                if (obj2 != null)
                {
                    Faction firstOfSpec = Get.FactionManager.GetFirstOfSpec(entitySpec.Actor.DefaultFaction);
                    if (firstOfSpec != null)
                    {
                        GUI.color = firstOfSpec.IconColor;
                        GUIExtra.DrawTexture(rect2, firstOfSpec.Icon);
                        GUI.color = Color.white;
                    }
                    else
                    {
                        GUIExtra.DrawTexture(rect2, entitySpec.Actor.DefaultFaction.Icon);
                    }
                }
                if (flag4)
                {
                    Widgets.Align = TextAnchor.MiddleLeft;
                    Widgets.Label(new Rect(rect.xMax + 10f, curY, num2, 40f), text, true, null, null, false);
                    Widgets.ResetAlign();
                }
                else
                {
                    Widgets.Label(new Rect(rect.xMax + 10f, curY + ((flag || !labelAndColor.Item1.NullOrEmpty()) ? 0f : 11f), num2, 30f), text, true, null, null, false);
                }
                if (!labelAndColor.Item1.NullOrEmpty())
                {
                    TipSubjectDrawer.DoRarity(labelAndColor.Item1, labelAndColor.Item2, rect.xMax + 10f, curY + 20f, num2, dontDraw);
                }
                if (flag)
                {
                    bool flag5 = !entitySpec.IsActor || (Get.Player.Faction != null && entitySpec.Actor.DefaultFaction != null && (entitySpec.Actor.DefaultFaction.DefaultHostileTo.Contains(Get.Player.Faction.Spec) || Get.Player.Faction.Spec.DefaultHostileTo.Contains(entitySpec.Actor.DefaultFaction)));
                    bool flag6 = entitySpec.IsActor && !flag5 && (Get.Player.Faction == null || entitySpec.Actor.DefaultFaction != Get.Player.Faction.Spec);
                    Rect rect4 = new Rect(rect.xMax + 10f, curY + 20f, num2, num);
                    ProgressBarDrawer progressBarDrawer = Get.ProgressBarDrawer;
                    Rect rect5 = rect4;
                    int maxHP = entitySpec.MaxHP;
                    int maxHP2 = entitySpec.MaxHP;
                    Color color = (flag5 ? Faction.DefaultHostileColor : (flag6 ? Faction.DefaultNeutralColor : Faction.DefaultAllyColor));
                    float num4 = 1f;
                    float num5 = 1f;
                    bool flag7 = true;
                    bool flag8 = true;
                    bool flag9 = !flag2 && !flag3;
                    bool flag10 = !flag2 && !flag3;
                    progressBarDrawer.Draw(rect5, maxHP, maxHP2, color, num4, num5, flag7, flag8, null, ProgressBarDrawer.ValueChangeDirection.Constant, null, true, true, flag9, flag10, true, false, null);
                    if (flag2)
                    {
                        rect4.y += rect4.height;
                        ProgressBarDrawer progressBarDrawer2 = Get.ProgressBarDrawer;
                        Rect rect6 = rect4;
                        int maxMana = entitySpec.Actor.MaxMana;
                        int maxMana2 = entitySpec.Actor.MaxMana;
                        Color manaBarColor = TipSubjectDrawer_Entity.ManaBarColor;
                        float num6 = 1f;
                        float num7 = 1f;
                        bool flag11 = true;
                        bool flag12 = true;
                        flag10 = !flag3;
                        flag9 = !flag3;
                        progressBarDrawer2.Draw(rect6, maxMana, maxMana2, manaBarColor, num6, num7, flag11, flag12, null, ProgressBarDrawer.ValueChangeDirection.Constant, null, false, false, flag10, flag9, true, false, null);
                    }
                    if (flag3)
                    {
                        rect4.y += rect4.height;
                        Get.ProgressBarDrawer.Draw(rect4, 10, 10, TipSubjectDrawer_Entity.StaminaBarColor, 1f, 1f, true, true, null, ProgressBarDrawer.ValueChangeDirection.Constant, null, false, false, true, true, true, false, null);
                    }
                }
            }
            curY += 40f;
            if (flag2)
            {
                curY += num;
            }
            if (flag3)
            {
                curY += num;
            }
        }

        public static bool ShouldShowManaBar(EntitySpec actorSpec)
        {
            if (!actorSpec.IsActor)
            {
                return false;
            }
            using (List<SpellSpec>.Enumerator enumerator = actorSpec.Actor.DefaultSpells.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.ManaCost != 0)
                    {
                        return true;
                    }
                }
            }
            using (List<SpellSpec>.Enumerator enumerator = actorSpec.Actor.RandomSpell.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.ManaCost != 0)
                    {
                        return true;
                    }
                }
            }
            using (List<NativeWeaponProps>.Enumerator enumerator2 = actorSpec.Actor.NativeWeapons.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.ManaCost != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ShouldShowStaminaBar(EntitySpec actorSpec)
        {
            if (!actorSpec.IsActor)
            {
                return false;
            }
            using (List<SpellSpec>.Enumerator enumerator = actorSpec.Actor.DefaultSpells.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.StaminaCost != 0)
                    {
                        return true;
                    }
                }
            }
            if (actorSpec.Actor.RandomSpell.Count != 0)
            {
                bool flag = true;
                using (List<SpellSpec>.Enumerator enumerator = actorSpec.Actor.RandomSpell.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.StaminaCost == 0)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    return true;
                }
            }
            using (List<NativeWeaponProps>.Enumerator enumerator2 = actorSpec.Actor.NativeWeapons.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.StaminaCost != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [CompilerGenerated]
        internal static void <DoEntitySpecSpecificParts>g__DoMiscSection|0_0(ref float curY2, ref TipSubjectDrawer_EntitySpec.<>c__DisplayClass0_0 A_1, ref TipSubjectDrawer_EntitySpec.<>c__DisplayClass0_1 A_2)
		{
			if (!A_2.miscSectionDone)
			{
				A_2.miscSectionDone = true;
				TipSubjectDrawer.DoSection("ActorMiscSection".Translate(), 0f, ref curY2, A_1.width, A_1.dontDraw);
			}
}
	}
}