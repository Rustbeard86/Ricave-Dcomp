using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class TipSubjectDrawer_Entity
    {
        public static void DoEntitySpecificParts(Entity entity, float width, ref float curY, bool dontDraw = false)
        {
            TipSubjectDrawer_Entity.<> c__DisplayClass2_0 CS$<> 8__locals1;
            CS$<> 8__locals1.width = width;
            CS$<> 8__locals1.dontDraw = dontDraw;
            TipSubjectDrawer_Entity.DoEntityBigLabelAndIcon(entity, 0f, CS$<> 8__locals1.width, ref curY, CS$<> 8__locals1.dontDraw, false, false, true);
            BossTrophy bossTrophy = entity as BossTrophy;
            if (bossTrophy != null && bossTrophy.Boss != null)
            {
                TipSubjectDrawer.DoLabel(RichText.Italics("\"{0}\n{1}\"".Formatted(bossTrophy.Boss.Name, "BossTrophyDesc".Translate(bossTrophy.Boss.KillTimeString))), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
            if (!entity.Description.NullOrEmpty())
            {
                TipSubjectDrawer.DoLabel(RichText.Italics(entity.Description), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
            Item item = entity as Item;
            if (item != null)
            {
                TipSubjectDrawer.DoManaAndStaminaCost(item.ManaCost, item.StaminaCost, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                if (item.Identified)
                {
                    if (item.Cursed)
                    {
                        TipSubjectDrawer.DoLabel("ItemCursedDesc".Translate(RichText.Cursed("CursedLower".Translate())), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                    }
                    if (item.UseEffects.Any)
                    {
                        float missChance = item.MissChance;
                        float critChance = item.CritChance;
                        UseEffects useEffects = item.UseEffects;
                        TipSubjectDrawer.DoMissAndCritChance(missChance, critChance, (useEffects != null) ? useEffects.GetWieldingActor() : null, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                    }
                    TipSubjectDrawer.DoUseEffects(item.UseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                    TipSubjectDrawer.DoConditions(item.ConditionsEquipped.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, false, true);
                }
                else if (Get.IdentificationGroups.GetIdentificationGroup(entity.Spec) != null)
                {
                    TipSubjectDrawer_Entity.DoIdentificationGroupPossibilities(entity.Spec, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                }
                else
                {
                    TipSubjectDrawer.DoLabel("ItemUnidentifiedDesc".Translate(RichText.Identification("UnidentifiedLower".Translate()), RichText.Cursed("CursedLower".Translate())), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                }
                if (item.UseEffects.Any && item.UseRange != 0)
                {
                    TipSubjectDrawer.DoUseRange(item, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                }
            }
            else
            {
                Actor actor = entity as Actor;
                if (actor != null)
                {
                    List<ConditionDrawRequest> actorConditionDrawRequestsPlusExtra = actor.ConditionsAccumulated.ActorConditionDrawRequestsPlusExtra;
                    if (actorConditionDrawRequestsPlusExtra.Count != 0)
                    {
                        TipSubjectDrawer.DoSection("Conditions".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        TipSubjectDrawer.DoConditions(actorConditionDrawRequestsPlusExtra, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, false, true);
                    }
                    if (actor.Inventory.EquippedWeapon != null)
                    {
                        TipSubjectDrawer.DoSection("Weapon".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        Item equippedWeapon = actor.Inventory.EquippedWeapon;
                        TipSubjectDrawer_Entity.DoEntityBigLabelAndIcon(equippedWeapon, 0f, CS$<> 8__locals1.width, ref curY, CS$<> 8__locals1.dontDraw, !actor.CanUse(actor.Inventory.EquippedWeapon, null, true, null), false, true);
                        if (equippedWeapon.Identified && equippedWeapon.UseEffects.Any)
                        {
                            TipSubjectDrawer.DoMissAndCritChance(equippedWeapon.MissChance, equippedWeapon.CritChance, actor, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        }
                        TipSubjectDrawer.DoUseEffects(equippedWeapon.UseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                        TipSubjectDrawer.DoConditions(equippedWeapon.ConditionsEquipped.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, false, true);
                        if (equippedWeapon.UseEffects.Any && equippedWeapon.UseRange != 0)
                        {
                            TipSubjectDrawer.DoUseRange(equippedWeapon, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        }
                    }
                    else if (actor.NativeWeapons.Count != 0)
                    {
                        TipSubjectDrawer.DoSection("Weapon".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        foreach (NativeWeapon nativeWeapon in actor.NativeWeapons)
                        {
                            TipSubjectDrawer.DoBigLabelAndIcon(nativeWeapon, 0f, CS$<> 8__locals1.width, ref curY, CS$<> 8__locals1.dontDraw, !actor.CanUse(nativeWeapon, null, true, null), false, null, true);
                            if (nativeWeapon.UseEffects.Any)
                            {
                                TipSubjectDrawer.DoMissAndCritChance(nativeWeapon.MissChance, nativeWeapon.CritChance, actor, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                            }
                            TipSubjectDrawer.DoUseEffects(nativeWeapon.UseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                            if (nativeWeapon.UseEffects.Any && nativeWeapon.UseRange != 0)
                            {
                                TipSubjectDrawer.DoUseRange(nativeWeapon, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                            }
                            TipSubjectDrawer.DoSequencePerUseMultiplier(nativeWeapon.SequencePerUseMultiplier, TipSubjectDrawer.SequencePerUseUsableType.NativeWeapon, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        }
                    }
                    if (!actor.IsMainActor && actor.Spells.Any)
                    {
                        TipSubjectDrawer.DoSection("Spells".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        foreach (Spell spell in actor.Spells.All)
                        {
                            TipSubjectDrawer.DoBigLabelAndIcon(spell, 0f, CS$<> 8__locals1.width, ref curY, CS$<> 8__locals1.dontDraw, !actor.CanUse(spell, null, true, null), false, null, true);
                            TipSubjectDrawer.DoManaAndStaminaCost(spell.ManaCost, spell.StaminaCost, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                            TipSubjectDrawer.DoUseEffects(spell.UseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                            if (spell.UseEffects.Any && spell.UseRange != 0)
                            {
                                TipSubjectDrawer.DoUseRange(spell, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                            }
                        }
                    }
                    if (actor.UsableOnTalk != null && actor.UsableOnTalk.UseEffects.Any)
                    {
                        TipSubjectDrawer.DoSection("Talk".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        TipSubjectDrawer.DoUseEffects(actor.UsableOnTalk.UseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                    }
                    if (actor.UsableOnDeath != null && actor.UsableOnDeath.UseEffects.Any)
                    {
                        TipSubjectDrawer.DoSection("WhenDies".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        TipSubjectDrawer.DoUseEffects(actor.UsableOnDeath.UseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                    }
                    bool flag = false;
                    using (List<Item>.Enumerator enumerator3 = actor.Inventory.EquippedItems.GetEnumerator())
                    {
                        while (enumerator3.MoveNext())
                        {
                            if (enumerator3.Current != actor.Inventory.EquippedWeapon)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        TipSubjectDrawer.DoSection("Worn".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        foreach (Item item2 in actor.Inventory.EquippedItems)
                        {
                            if (item2 != actor.Inventory.EquippedWeapon)
                            {
                                TipSubjectDrawer_Entity.DoEntityBigLabelAndIcon(item2, 0f, CS$<> 8__locals1.width, ref curY, CS$<> 8__locals1.dontDraw, false, false, true);
                                TipSubjectDrawer.DoConditions(item2.ConditionsEquipped.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, false, true);
                            }
                        }
                    }
                    if (!actor.IsPlayerParty && actor.Inventory.AllNonEquippedItems.Count != 0 && Get.Player.Faction != null && actor.Faction == Get.Player.Faction)
                    {
                        TipSubjectDrawer.DoSection("Inventory".Translate(), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        foreach (Item item3 in actor.Inventory.AllNonEquippedItems)
                        {
                            TipSubjectDrawer_Entity.DoEntityBigLabelAndIcon(item3, 0f, CS$<> 8__locals1.width, ref curY, CS$<> 8__locals1.dontDraw, false, false, true);
                        }
                    }
                    TipSubjectDrawer_Entity.<> c__DisplayClass2_1 CS$<> 8__locals2;
                    CS$<> 8__locals2.miscSectionDone = false;
                    if (actor.IsHostile(Get.MainActor) || actor.Faction == null || Get.Player.Faction == null || actor.Faction != Get.Player.Faction)
                    {
                        TipSubjectDrawer_Entity.< DoEntitySpecificParts > g__DoMiscSection | 2_0(ref curY, ref CS$<> 8__locals1, ref CS$<> 8__locals2);
                        TipSubjectDrawer.DoLabel("KilledExperienceTip".Translate(actor.KilledExperience), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                    }
                    if (actor.Faction != null && Get.Player.Faction != null && actor.Faction != Get.Player.Faction && !Get.FactionManager.HostilityExists(Get.Player.Faction, actor.Faction))
                    {
                        TipSubjectDrawer_Entity.< DoEntitySpecificParts > g__DoMiscSection | 2_0(ref curY, ref CS$<> 8__locals1, ref CS$<> 8__locals2);
                        TipSubjectDrawer.DoLabel(RichText.LightRed("AngeringFactionTip".Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                    }
                    if (actor.AI != null)
                    {
                        TipSubjectDrawer_Entity.< DoEntitySpecificParts > g__DoMiscSection | 2_0(ref curY, ref CS$<> 8__locals1, ref CS$<> 8__locals2);
                        string text = actor.AI.LabelCap;
                        if (actor.AI.HasShouldFleeNode && AINode_ShouldFlee.CanEverFlee(actor))
                        {
                            text = text.AppendedWithComma("FleesOnLowHP".Translate());
                        }
                        TipSubjectDrawer.DoLabel(RichText.AI("AITip".Translate(text)), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                    }
                    if (Get.Player.IsPlayerFollower(actor) || (actor.IsPlayerParty && !actor.IsMainActor))
                    {
                        TipSubjectDrawer_Entity.< DoEntitySpecificParts > g__DoMiscSection | 2_0(ref curY, ref CS$<> 8__locals1, ref CS$<> 8__locals2);
                        TipSubjectDrawer.DoLabel(RichText.Grayed("FollowerTip".Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                    }
                }
                else
                {
                    Structure structure = entity as Structure;
                    if (structure != null)
                    {
                        if (entity.Spec == Get.Entity_Sign)
                        {
                            if (Get.PlaceSpec == Get.Place_Shelter)
                            {
                                TipSubjectDrawer.DoLabel(RichText.Italics("\"{0}\"".Formatted("Sign_Shelter".Translate())), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                            }
                            else if (Get.Floor == 1)
                            {
                                TipSubjectDrawer.DoLabel(RichText.Italics("\"{0}\"".Formatted("Sign_FloorOne".Translate())), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                            }
                            else
                            {
                                string text2 = "Sign_AnyFloor".Translate(Get.Floor);
                                Rand.PushState(Calc.CombineHashes<int, int>(Get.WorldSeed, 472153980));
                                string text3;
                                "SignTexts".TranslateSplit(";").TryGetRandomElement<string>(out text3);
                                Rand.PopState();
                                text2 = text2.AppendedInDoubleNewLine(text3);
                                TipSubjectDrawer.DoLabel(RichText.Italics("\"{0}\"".Formatted(text2)), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                            }
                        }
                        TipSubjectDrawer.DoManaAndStaminaCost(structure.ManaCost, structure.StaminaCost, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                        if (structure.UseEffects.Any)
                        {
                            TipSubjectDrawer.DoUseEffects(structure.UseEffects.AllDrawRequests, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, 5f, 1f, false, true);
                        }
                    }
                }
            }
            foreach (EntityComp entityComp in entity.AllComps)
            {
                string extraTip = entityComp.ExtraTip;
                if (!extraTip.NullOrEmpty())
                {
                    TipSubjectDrawer.DoLabel(extraTip, 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                }
            }
            IUsable usable = entity as IUsable;
            if (usable != null && usable.CooldownTurns != 0)
            {
                TipSubjectDrawer.DoCooldown(usable.CooldownTurns, usable.LastUseSequence, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
            }
            Item item4 = entity as Item;
            if (item4 != null)
            {
                TipSubjectDrawer.DoSequencePerUseMultiplier(item4.SequencePerUseMultiplier, TipSubjectDrawer.SequencePerUseUsableType.Item, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
            }
            else
            {
                Structure structure2 = entity as Structure;
                if (structure2 != null)
                {
                    TipSubjectDrawer.DoSequencePerUseMultiplier(structure2.SequencePerUseMultiplier, TipSubjectDrawer.SequencePerUseUsableType.Structure, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw);
                }
            }
            Item item5 = entity as Item;
            if (item5 == null || !item5.Spec.Item.AllowUseOnlyIfNotSeenByHostiles)
            {
                Structure structure3 = entity as Structure;
                if (structure3 == null || !structure3.Spec.Structure.AllowUseOnlyIfNotSeenByHostiles)
                {
                    goto IL_0E2A;
                }
            }
            TipSubjectDrawer.DoLabel(RichText.Grayed("AllowUseOnlyIfNotSeenByHostilesTip".Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
        IL_0E2A:
            if (entity is Structure && entity.Spec.Structure.Flammable)
            {
                TipSubjectDrawer.DoLabel(RichText.Grayed("FlammableEntityTip".Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
            if (entity is Structure && entity.Spec.Structure.Fragile)
            {
                TipSubjectDrawer.DoLabel(RichText.Grayed("FragileEntityTip".Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
            if (!(entity is Structure) || !entity.Spec.Structure.IsLadder)
            {
                Item item6 = entity as Item;
                if (item6 == null || !item6.Identified || !TipSubjectDrawer_Entity.AllowsLevitation(item6))
                {
                    goto IL_0F2E;
                }
            }
            TipSubjectDrawer.DoLabel(RichText.Grayed("ChangeAltitudeTip".Translate().FormattedKeyBindings()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
        IL_0F2E:
            if (entity.Spec == Get.Entity_Watch || entity.Spec == Get.Entity_Superwatch)
            {
                TipSubjectDrawer.DoLabel(RichText.Grayed("PressKeyToRewindTip".Translate().FormattedKeyBindings()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
            if (!Get.InLobby && entity.Spec == Get.Entity_Door && entity.Spawned)
            {
                TipSubjectDrawer.DoLabel(RichText.Grayed("KickDoorTip".Translate().FormattedKeyBindings()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
            if (entity.Spawned && !Get.UI.IsWheelSelectorOpen)
            {
                Item item7 = entity as Item;
                if (item7 != null)
                {
                    if (item7.ForSale)
                    {
                        TipSubjectDrawer.DoLabel(RichText.Grayed((ControllerUtility.InControllerMode ? "ClickToBuyTipController" : "ClickToBuyTip").Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                    }
                    else
                    {
                        TipSubjectDrawer.DoLabel(RichText.Grayed((ControllerUtility.InControllerMode ? "ClickToPickUpTipController" : "ClickToPickUpTip").Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                    }
                }
                else
                {
                    Structure structure4 = entity as Structure;
                    if (structure4 != null && ((structure4.Spec.Structure.AllowUseViaInterface && structure4.UseEffects.Any && structure4.UseFilter.Allows(Get.NowControlledActor, Get.NowControlledActor)) || structure4.Spec.Structure.IsStairs || structure4.Spec.Structure.IsLadder))
                    {
                        TipSubjectDrawer.DoLabel(RichText.Grayed((ControllerUtility.InControllerMode ? "ClickToInteractTipController" : "ClickToInteractTip").Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
                    }
                }
            }
            if (entity is Item && entity.Spec.Item.DestroyOnDescend)
            {
                TipSubjectDrawer.DoLabel(RichText.LightRed("DestroyOnDescendItemTip".Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
            Item item8 = entity as Item;
            if (item8 != null && !item8.Identified && !item8.Spawned && item8.Spec.Item.IsEquippable && item8.ParentInventory == Get.NowControlledActor.Inventory && Get.IdentificationGroups.GetIdentificationGroup(item8.Spec) == null && (Get.RunSpec == Get.Run_Tutorial || Get.RunSpec == Get.Run_Main1 || Get.RunSpec == Get.Run_Main2))
            {
                TipSubjectDrawer.DoLabel(RichText.Grayed("CheckInCodexTip".Translate()), 0f, ref curY, CS$<> 8__locals1.width, CS$<> 8__locals1.dontDraw, TextAnchor.UpperLeft, null);
            }
        }

        public static Rect DoEntityBigLabelAndIcon(Entity entity, float x, float width, ref float curY, bool dontDraw = false, bool drawInRed = false, bool doBackground = false, bool showLabels = true)
        {
            float num = 20f;
            Actor actor = entity as Actor;
            bool flag = entity.MaxHP != 0;
            bool flag2 = flag && actor != null && TipSubjectDrawer_Entity.ShouldShowManaBar(actor);
            bool flag3 = flag && actor != null && TipSubjectDrawer_Entity.ShouldShowStaminaBar(actor);
            ValueTuple<string, Color> labelAndColor = ItemRarityUtility.GetLabelAndColor(entity as Item);
            bool flag4 = !flag && labelAndColor.Item1.NullOrEmpty();
            Rect rect3;
            if (!dontDraw)
            {
                Rect rect = new Rect(x, curY + (flag2 ? (num / 2f) : 0f) + (flag3 ? (num / 2f) : 0f), 40f, 40f);
                Rect rect2 = rect;
                rect2.x = x + width - 40f;
                object obj = actor != null && actor.Faction != null;
                float num2 = width - rect.width - 10f;
                object obj2 = obj;
                if (obj2 != null)
                {
                    num2 -= 50f;
                }
                string text = (drawInRed ? RichText.Bold(entity.LabelCap) : RichText.Label(entity));
                if (!flag4)
                {
                    text = text.Truncate(num2);
                }
                float num3 = ((showLabels && !labelAndColor.Item1.NullOrEmpty()) ? Widgets.CalcSize(labelAndColor.Item1).x : 0f);
                rect3 = (showLabels ? new Rect(x, curY, 50f + Math.Min(width, Math.Max(Widgets.CalcSize(text).x, num3)) + 3f, 40f) : rect);
                if (doBackground)
                {
                    Widgets.DoLabelBackground(rect3);
                }
                Get.Tooltips.RegisterTip(rect3, entity, null, null);
                GUIExtra.DrawHighlightIfMouseover(rect3, true, true, true, true, true);
                if (drawInRed)
                {
                    GUI.color = new Color(1f, 0.2f, 0.2f);
                }
                else
                {
                    GUI.color = entity.IconColor;
                }
                GUIExtra.DrawTexture(rect, entity.Icon);
                if (!drawInRed)
                {
                    GUI.color = Color.white;
                }
                if (obj2 != null)
                {
                    GUI.color = actor.Faction.IconColor;
                    GUIExtra.DrawTexture(rect2, actor.Faction.Icon);
                    GUI.color = Color.white;
                }
                if (showLabels)
                {
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
                }
                GUI.color = Color.white;
                if (showLabels && !labelAndColor.Item1.NullOrEmpty())
                {
                    TipSubjectDrawer.DoRarity(labelAndColor.Item1, labelAndColor.Item2, rect.xMax + 10f, curY + 20f, num2, dontDraw);
                }
                if (flag && showLabels)
                {
                    Rect rect4 = new Rect(rect.xMax + 10f, curY + 20f, num2, num);
                    ProgressBarDrawer progressBarDrawer = Get.ProgressBarDrawer;
                    Rect rect5 = rect4;
                    int hp = entity.HP;
                    int maxHP = entity.MaxHP;
                    Color hpbarColor = TipSubjectDrawer_Entity.GetHPBarColor(entity);
                    float num4 = 1f;
                    float num5 = 1f;
                    bool flag5 = true;
                    bool flag6 = true;
                    int? num6 = new int?(entity.InstanceID);
                    ProgressBarDrawer.ValueChangeDirection valueChangeDirection = ProgressBarDrawer.ValueChangeDirection.Constant;
                    bool flag7 = !flag2 && !flag3;
                    bool flag8 = !flag2 && !flag3;
                    progressBarDrawer.Draw(rect5, hp, maxHP, hpbarColor, num4, num5, flag5, flag6, num6, valueChangeDirection, null, true, true, flag7, flag8, true, false, null);
                    if (flag2)
                    {
                        rect4.y += rect4.height;
                        ProgressBarDrawer progressBarDrawer2 = Get.ProgressBarDrawer;
                        Rect rect6 = rect4;
                        int mana = actor.Mana;
                        int maxMana = actor.MaxMana;
                        Color manaBarColor = TipSubjectDrawer_Entity.ManaBarColor;
                        float num7 = 1f;
                        float num8 = 1f;
                        bool flag9 = true;
                        bool flag10 = true;
                        int? num9 = new int?(100000000 + entity.InstanceID);
                        ProgressBarDrawer.ValueChangeDirection valueChangeDirection2 = ProgressBarDrawer.ValueChangeDirection.Constant;
                        flag8 = !flag3;
                        flag7 = !flag3;
                        progressBarDrawer2.Draw(rect6, mana, maxMana, manaBarColor, num7, num8, flag9, flag10, num9, valueChangeDirection2, null, false, false, flag8, flag7, true, false, null);
                    }
                    if (flag3)
                    {
                        rect4.y += rect4.height;
                        Get.ProgressBarDrawer.Draw(rect4, actor.Stamina, actor.MaxStamina, TipSubjectDrawer_Entity.StaminaBarColor, 1f, 1f, true, true, new int?(200000000 + entity.InstanceID), ProgressBarDrawer.ValueChangeDirection.Constant, null, false, false, true, true, true, false, null);
                    }
                }
            }
            else
            {
                rect3 = default(Rect);
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
            return rect3;
        }

        public static void DoIdentificationGroupPossibilities(EntitySpec entitySpec, ref float curY, float width, bool dontDraw = false)
        {
            TipSubjectDrawer.DoLabel("ItemUnidentifiedDescChoices".Translate(RichText.Identification("UnidentifiedLower".Translate())), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
            List<EntitySpec> possibilities = Get.IdentificationGroups.GetPossibilities(entitySpec);
            for (int i = 0; i < possibilities.Count; i++)
            {
                TipSubjectDrawer_Entity.DoEntityLabelAndIcon_AssumeIdentified(possibilities[i], 0f, ref curY, width, dontDraw, 10f);
                if (possibilities[i].Item.DefaultUseEffects != null)
                {
                    TipSubjectDrawer.DoUseEffects(possibilities[i].Item.DefaultUseEffects.AllDrawRequests, 10f, ref curY, width - 10f, dontDraw, 5f, 1f, false, true);
                }
                if (possibilities[i].Item.DefaultConditionsEquipped != null)
                {
                    TipSubjectDrawer.DoConditions(possibilities[i].Item.DefaultConditionsEquipped.AllDrawRequests, 10f, ref curY, width - 10f, dontDraw, 5f, 1f, false, false, true);
                }
            }
            TipSubjectDrawer.DoLabel(RichText.Grayed("IdentificationGroupsBecomeIdentifiedDesc".Translate()), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
        }

        public static void DoEntityLabelAndIcon_AssumeIdentified(EntitySpec entitySpec, float x, ref float curY, float width, bool dontDraw = false, float initialGap = 0f)
        {
            curY += initialGap;
            if (!dontDraw)
            {
                Rect rect = new Rect(x, curY, 20f, 20f);
                GUI.color = entitySpec.IconColor;
                GUIExtra.DrawTexture(rect, entitySpec.Icon);
                GUI.color = Color.white;
                Rect rect2 = new Rect(rect.x + 20f + 10f, rect.y, width, 30f);
                Widgets.Label(rect2, RichText.Label(entitySpec, true).Truncate(rect2.width), true, null, null, false);
            }
            curY += 20f;
        }

        public static bool ShouldShowManaBar(Actor actor)
        {
            if ((actor.IsPlayerParty || actor.IsNowControlledActor) && Get.Player.HadManaBar)
            {
                return true;
            }
            if (actor.IsNowControlledActor && actor.Mana < actor.MaxMana)
            {
                return true;
            }
            using (List<Spell>.Enumerator enumerator = actor.Spells.All.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.ManaCost != 0)
                    {
                        return true;
                    }
                }
            }
            using (List<NativeWeapon>.Enumerator enumerator2 = actor.NativeWeapons.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.ManaCost != 0)
                    {
                        return true;
                    }
                }
            }
            foreach (Item item in actor.Inventory.AllItems)
            {
                if ((actor.IsPlayerParty || actor.IsNowControlledActor || actor.Inventory.Equipped.IsEquipped(item)) && item.ManaCost != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ShouldShowStaminaBar(Actor actor)
        {
            if ((actor.IsPlayerParty || actor.IsNowControlledActor) && Get.Player.HadStaminaBar)
            {
                return true;
            }
            if (actor.IsNowControlledActor && actor.Stamina < actor.MaxStamina)
            {
                return true;
            }
            using (List<Spell>.Enumerator enumerator = actor.Spells.All.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.StaminaCost != 0)
                    {
                        return true;
                    }
                }
            }
            using (List<NativeWeapon>.Enumerator enumerator2 = actor.NativeWeapons.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.StaminaCost != 0)
                    {
                        return true;
                    }
                }
            }
            foreach (Item item in actor.Inventory.AllItems)
            {
                if ((actor.IsMainActor || actor.IsNowControlledActor || actor.Inventory.Equipped.IsEquipped(item)) && item.StaminaCost != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static ProgressBarDrawer.ValueChangeDirection GetHPBarValueChangeDirection(Actor actor)
        {
            if (actor.HP > actor.MaxHP)
            {
                return ProgressBarDrawer.ValueChangeDirection.Down;
            }
            bool flag = false;
            List<Condition> allConditions = actor.ConditionsAccumulated.AllConditions;
            for (int i = 0; i < allConditions.Count; i++)
            {
                Condition condition = allConditions[i];
                Condition_DamageOverTimeBase condition_DamageOverTimeBase = condition as Condition_DamageOverTimeBase;
                if (condition_DamageOverTimeBase != null && condition_DamageOverTimeBase.Damage > 0)
                {
                    return ProgressBarDrawer.ValueChangeDirection.Down;
                }
                Condition_HPRegen condition_HPRegen = condition as Condition_HPRegen;
                if (condition_HPRegen != null && !condition_HPRegen.Disabled && condition_HPRegen.Amount > 0)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                return ProgressBarDrawer.ValueChangeDirection.Up;
            }
            return ProgressBarDrawer.ValueChangeDirection.Constant;
        }

        public static ProgressBarDrawer.ValueChangeDirection GetManaBarValueChangeDirection(Actor actor)
        {
            if (actor.Mana > actor.MaxMana)
            {
                return ProgressBarDrawer.ValueChangeDirection.Down;
            }
            return ProgressBarDrawer.ValueChangeDirection.Constant;
        }

        public static ProgressBarDrawer.ValueChangeDirection GetStaminaBarValueChangeDirection(Actor actor)
        {
            if (actor.Stamina > actor.MaxStamina)
            {
                return ProgressBarDrawer.ValueChangeDirection.Down;
            }
            bool flag = false;
            List<Condition> allConditions = actor.ConditionsAccumulated.AllConditions;
            for (int i = 0; i < allConditions.Count; i++)
            {
                Condition_StaminaRegen condition_StaminaRegen = allConditions[i] as Condition_StaminaRegen;
                if (condition_StaminaRegen != null && !condition_StaminaRegen.Disabled && condition_StaminaRegen.Amount > 0)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                return ProgressBarDrawer.ValueChangeDirection.Up;
            }
            return ProgressBarDrawer.ValueChangeDirection.Constant;
        }

        private static bool AllowsLevitation(Item item)
        {
            if (item.ConditionsEquipped != null && item.ConditionsEquipped.AnyOfSpec(Get.Condition_Levitating))
            {
                return true;
            }
            if (item.UseEffects != null)
            {
                foreach (UseEffect useEffect in item.UseEffects.All)
                {
                    UseEffect_AddCondition useEffect_AddCondition = useEffect as UseEffect_AddCondition;
                    if (useEffect_AddCondition != null && useEffect_AddCondition.Condition.Spec == Get.Condition_Levitating)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public static Color GetHPBarColor(Entity entity)
        {
            Actor actor = entity as Actor;
            if (actor != null)
            {
                return actor.HostilityColor;
            }
            return Faction.DefaultHostileColor;
        }

        [CompilerGenerated]
        internal static void <DoEntitySpecificParts>g__DoMiscSection|2_0(ref float curY2, ref TipSubjectDrawer_Entity.<>c__DisplayClass2_0 A_1, ref TipSubjectDrawer_Entity.<>c__DisplayClass2_1 A_2)
		{
			if (!A_2.miscSectionDone)
			{
				A_2.miscSectionDone = true;
				TipSubjectDrawer.DoSection("ActorMiscSection".Translate(), 0f, ref curY2, A_1.width, A_1.dontDraw);
			}
}

public static readonly Color ManaBarColor = new Color(0.11f, 0.38f, 0.79f);

public static readonly Color StaminaBarColor = new Color(0.78f, 0.57f, 0.11f);
	}
}