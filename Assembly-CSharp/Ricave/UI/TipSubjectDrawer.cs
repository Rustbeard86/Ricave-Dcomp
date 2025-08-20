using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class TipSubjectDrawer
    {
        public static Vector2 GetSize(ITipSubject subject, string extraText = null)
        {
            float num;
            TipSubjectDrawer.DoContents(subject, 330f, out num, extraText, true);
            num += 20f;
            return new Vector2(350f, Calc.Ceil(num));
        }

        public static void Draw(ITipSubject subject, Vector2 pos, string extraText = null)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            Vector2 size = TipSubjectDrawer.GetSize(subject, extraText);
            Rect rect = new Rect(pos, size);
            Item item = subject as Item;
            if (item == null || !item.Identified || (item.Spec.Item.Rarity != ItemRarity.Rare && item.Spec.Item.Rarity != ItemRarity.Epic))
            {
                EntitySpec entitySpec = subject as EntitySpec;
                if (entitySpec == null || !entitySpec.IsItem || (Get.IdentificationGroups.GetIdentificationGroup(entitySpec) != null && !Get.IdentificationGroups.IsIdentified(entitySpec)) || (entitySpec.Item.Rarity != ItemRarity.Rare && entitySpec.Item.Rarity != ItemRarity.Epic))
                {
                    Actor actor = subject as Actor;
                    if (actor == null || !actor.IsBoss)
                    {
                        GUIExtra.DrawRoundedRectBump(rect, TipSubjectDrawer.BackgroundColor, false, true, true, true, true, null);
                        goto IL_00E6;
                    }
                }
            }
            CachedGUI.SetDirty(7);
            GUIExtra.DrawRoundedRectWithOutline(rect, Color.white, Color.black, true, true, true, true, GUIExtra.GradientTexture);
        IL_00E6:
            Rect rect2 = rect.ContractedBy(10f);
            GUIExtra.BeginGroup(rect2);
            float num;
            TipSubjectDrawer.DoContents(subject, rect2.width, out num, extraText, false);
            GUIExtra.EndGroup();
        }

        public static void DoContents(ITipSubject subject, float width, out float height, string extraText = null, bool dontDraw = false)
        {
            float num = 0f;
            Entity entity = subject as Entity;
            if (entity != null)
            {
                TipSubjectDrawer_Entity.DoEntitySpecificParts(entity, width, ref num, dontDraw);
            }
            else
            {
                EntitySpec entitySpec = subject as EntitySpec;
                if (entitySpec != null)
                {
                    TipSubjectDrawer_EntitySpec.DoEntitySpecSpecificParts(entitySpec, width, ref num, dontDraw);
                }
                else
                {
                    PrivateRoomPlaceStructureUsable privateRoomPlaceStructureUsable = subject as PrivateRoomPlaceStructureUsable;
                    if (privateRoomPlaceStructureUsable != null)
                    {
                        TipSubjectDrawer_EntitySpec.DoEntitySpecSpecificParts(privateRoomPlaceStructureUsable.PlacingStructure, width, ref num, dontDraw);
                    }
                    else
                    {
                        TraitSpec traitSpec = subject as TraitSpec;
                        if (traitSpec != null)
                        {
                            TipSubjectDrawer.DoBigLabelAndIcon(subject, 0f, width, ref num, dontDraw, false, false, new ValueTuple<string, Color>?(traitSpec.Rarity.GetLabelAndColor()), true);
                        }
                        else
                        {
                            TipSubjectDrawer.DoBigLabelAndIcon(subject, 0f, width, ref num, dontDraw, false, false, null, true);
                        }
                        if (!subject.Description.NullOrEmpty())
                        {
                            TipSubjectDrawer.DoLabel(RichText.Italics(subject.Description), 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                        }
                        IUsable usable = subject as IUsable;
                        if (usable != null)
                        {
                            TipSubjectDrawer.DoManaAndStaminaCost(usable.ManaCost, usable.StaminaCost, ref num, width, dontDraw);
                            if (usable.UseEffects.Any)
                            {
                                float missChance = usable.MissChance;
                                float critChance = usable.CritChance;
                                UseEffects useEffects = usable.UseEffects;
                                TipSubjectDrawer.DoMissAndCritChance(missChance, critChance, (useEffects != null) ? useEffects.GetWieldingActor() : null, ref num, width, dontDraw);
                            }
                            TipSubjectDrawer.DoUseEffects(usable.UseEffects.AllDrawRequests, 0f, ref num, width, dontDraw, 5f, 1f, false, true);
                            if (usable.UseEffects.Any && usable.UseRange != 0)
                            {
                                TipSubjectDrawer.DoUseRange(usable, 0f, ref num, width, dontDraw);
                            }
                            if (usable.CooldownTurns != 0)
                            {
                                TipSubjectDrawer.DoCooldown(usable.CooldownTurns, usable.LastUseSequence, ref num, width, dontDraw);
                            }
                            TipSubjectDrawer.DoSequencePerUseMultiplier(usable.SequencePerUseMultiplier, (usable is Spell) ? TipSubjectDrawer.SequencePerUseUsableType.Spell : ((usable is NativeWeapon) ? TipSubjectDrawer.SequencePerUseUsableType.NativeWeapon : TipSubjectDrawer.SequencePerUseUsableType.Item), ref num, width, dontDraw);
                        }
                        else
                        {
                            SpellSpec spellSpec = subject as SpellSpec;
                            if (spellSpec != null)
                            {
                                TipSubjectDrawer.DoManaAndStaminaCost(spellSpec.ManaCost, spellSpec.StaminaCost, ref num, width, dontDraw);
                                TipSubjectDrawer.DoUseEffects(spellSpec.DefaultUseEffects.AllDrawRequests, 0f, ref num, width, dontDraw, 5f, 1f, false, true);
                                if (spellSpec.DefaultUseEffects.Any && spellSpec.UseRange != 0)
                                {
                                    TipSubjectDrawer.DoUseRange(spellSpec.UseRange, 0f, ref num, width, dontDraw);
                                }
                                if (spellSpec.CooldownTurns != 0)
                                {
                                    TipSubjectDrawer.DoCooldown(spellSpec.CooldownTurns, null, ref num, width, dontDraw);
                                }
                                TipSubjectDrawer.DoSequencePerUseMultiplier(spellSpec.SequencePerUseMultiplier, TipSubjectDrawer.SequencePerUseUsableType.Spell, ref num, width, dontDraw);
                            }
                            else
                            {
                                WorldSituationSpec worldSituationSpec = subject as WorldSituationSpec;
                                if (worldSituationSpec != null)
                                {
                                    if (worldSituationSpec.EveryoneConditions != null && worldSituationSpec.EveryoneConditions.Any)
                                    {
                                        TipSubjectDrawer.DoConditions(worldSituationSpec.EveryoneConditions.AllDrawRequests, 0f, ref num, width, dontDraw, 5f, 1f, false, false, true);
                                    }
                                }
                                else
                                {
                                    WorldSituation worldSituation = subject as WorldSituation;
                                    if (worldSituation != null)
                                    {
                                        if (worldSituation.Spec.EveryoneConditions != null && worldSituation.Spec.EveryoneConditions.Any)
                                        {
                                            TipSubjectDrawer.DoConditions(worldSituation.Spec.EveryoneConditions.AllDrawRequests, 0f, ref num, width, dontDraw, 5f, 1f, false, false, true);
                                        }
                                        if (worldSituation.SequenceWhenAdded != null)
                                        {
                                            int num2 = (Get.TurnManager.CurrentSequence - worldSituation.SequenceWhenAdded.Value) / 12;
                                            TipSubjectDrawer.DoLabel("{0}: {1}".Formatted("WorldSituationPresentSince".Translate(), RichText.Turns(StringUtility.TurnsString(num2))), 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                        }
                                    }
                                    else
                                    {
                                        SkillSpec skillSpec = subject as SkillSpec;
                                        if (skillSpec != null)
                                        {
                                            if (!Get.SkillManager.IsUnlocked(skillSpec))
                                            {
                                                if (!skillSpec.PrerequisiteUnlocked)
                                                {
                                                    TipSubjectDrawer.DoLabel(RichText.Grayed("UnlockSkillFirst".Translate(skillSpec.PrerequisitesLabel)), 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                                }
                                                else if (Get.Player.SkillPoints <= 0)
                                                {
                                                    TipSubjectDrawer.DoLabel(RichText.Grayed("YouDontHaveAnySkillPoints".Translate()), 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                                }
                                                else
                                                {
                                                    TipSubjectDrawer.DoLabel(RichText.Grayed((ControllerUtility.InControllerMode ? "ClickToUnlockSkillController" : "ClickToUnlockSkill").Translate()), 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Place place = subject as Place;
                                            if (place != null)
                                            {
                                                if (!place.ModifiersDetails.NullOrEmpty())
                                                {
                                                    TipSubjectDrawer.DoLabel(place.ModifiersDetails, 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                                }
                                                if (place.ShelterItemReward != null)
                                                {
                                                    string text = place.ShelterItemReward.Value.EntitySpec.LabelCap;
                                                    if (place.ShelterItemRampUp != 0)
                                                    {
                                                        text = "{0} +{1}".Formatted(text, place.ShelterItemRampUp.ToStringCached());
                                                    }
                                                    if (place.ShelterItemReward.Value.Count != 1)
                                                    {
                                                        text = "{0} x{1}".Formatted(text, place.ShelterItemReward.Value.Count.ToStringCached());
                                                    }
                                                    TipSubjectDrawer.DoLabel("{0}: {1}".Formatted("ShelterReward".Translate(), text), 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                                }
                                                if (!place.Enemies.NullOrEmpty<EntitySpec>())
                                                {
                                                    string text2 = "{0}:".Formatted("Enemies".Translate());
                                                    foreach (EntitySpec entitySpec2 in place.Enemies)
                                                    {
                                                        bool flag = Get.Progress.SeenActorSpecs.Contains(entitySpec2) || Get.Player.SeenActorSpecs.Contains(entitySpec2);
                                                        text2 = text2.AppendedInNewLine("- {0}".Formatted(flag ? entitySpec2.LabelCap : "UndiscoveredEnemy".Translate()));
                                                    }
                                                    TipSubjectDrawer.DoLabel(text2, 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                                }
                                            }
                                            else
                                            {
                                                TraitSpec traitSpec2 = subject as TraitSpec;
                                                if (traitSpec2 != null)
                                                {
                                                    if (!traitSpec2.EffectsString.NullOrEmpty())
                                                    {
                                                        TipSubjectDrawer.DoLabel(traitSpec2.EffectsString, 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                                    }
                                                    if (Get.InLobby)
                                                    {
                                                        TipSubjectDrawer.DoLabel("MaxFloorReachedWithTraitTip".Translate(Get.TraitManager.GetMaxFloorReachedWithTrait(traitSpec2)), 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                                    }
                                                }
                                                else
                                                {
                                                    ClassSpec classSpec = subject as ClassSpec;
                                                    if (classSpec != null)
                                                    {
                                                        if (!classSpec.EffectsString.NullOrEmpty())
                                                        {
                                                            TipSubjectDrawer.DoLabel(classSpec.EffectsString, 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                                        }
                                                        if (Get.InLobby)
                                                        {
                                                            TipSubjectDrawer.DoLabel("MaxFloorReachedWithClassTip".Translate(Get.Progress.GetMaxFloorReachedWithClass(classSpec)), 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!extraText.NullOrEmpty())
            {
                TipSubjectDrawer.DoLabel(extraText, 0f, ref num, width, dontDraw, TextAnchor.UpperLeft, null);
            }
            height = num;
        }

        public static void DoBigLabelAndIcon(ITipSubject subject, float x, float width, ref float curY, bool dontDraw = false, bool drawInRed = false, bool doBackground = false, ValueTuple<string, Color>? secondLine = null, bool showLabels = true)
        {
            bool flag = secondLine == null;
            if (!dontDraw)
            {
                Rect rect = new Rect(x, curY, 40f, 40f);
                float num = width - rect.width - 10f;
                string text = (drawInRed ? RichText.Bold(subject.LabelCap) : RichText.Label(subject));
                if (!flag)
                {
                    text = text.Truncate(num);
                }
                float num2 = ((secondLine == null) ? 0f : Widgets.CalcSize(secondLine.Value.Item1).x);
                Rect rect2 = (showLabels ? new Rect(x, curY, 50f + Math.Min(width, Math.Max(Widgets.CalcSize(text).x, num2)) + 3f, 40f) : rect);
                if (doBackground)
                {
                    Widgets.DoLabelBackground(rect2);
                }
                Get.Tooltips.RegisterTip(rect2, subject, null, null);
                GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
                if (drawInRed)
                {
                    GUI.color = new Color(1f, 0.2f, 0.2f);
                }
                else
                {
                    GUI.color = subject.IconColor;
                }
                GUIExtra.DrawTexture(rect, subject.Icon);
                if (!drawInRed)
                {
                    GUI.color = Color.white;
                }
                if (showLabels)
                {
                    if (flag)
                    {
                        Widgets.Align = TextAnchor.MiddleLeft;
                        Widgets.Label(new Rect(rect.xMax + 10f, curY, num, 40f), text, true, null, null, false);
                        Widgets.ResetAlign();
                    }
                    else
                    {
                        Widgets.Label(new Rect(rect.xMax + 10f, curY + ((secondLine != null) ? 0f : 11f), num, 30f), text, true, null, null, false);
                    }
                }
                GUI.color = Color.white;
                if (showLabels && secondLine != null)
                {
                    TipSubjectDrawer.DoRarity(secondLine.Value.Item1, secondLine.Value.Item2, rect.xMax + 10f, curY + 20f, num, dontDraw);
                }
            }
            curY += 40f;
        }

        public static void DoBigLabelAndIcon(string label, Texture2D icon, float x, float width, ref float curY, bool dontDraw = false)
        {
            if (!dontDraw)
            {
                Rect rect = new Rect(x, curY, 40f, 40f);
                GUIExtra.DrawTexture(rect, icon);
                Widgets.Label(new Rect(rect.xMax + 10f, curY + 11f, width - rect.width - 10f, 30f), label, true, null, null, false);
            }
            curY += 40f;
        }

        public static void DoUseEffects(List<UseEffectDrawRequest> useEffects, float x, ref float curY, float width, bool dontDraw = false, float initialGap = 0f, float iconSizeFactor = 1f, bool doBackground = false, bool showLabels = true)
        {
            if (useEffects == null || useEffects.Count == 0)
            {
                return;
            }
            curY += initialGap;
            float num = 20f * iconSizeFactor;
            foreach (UseEffectDrawRequest useEffectDrawRequest in useEffects)
            {
                string labelCap = useEffectDrawRequest.LabelCap;
                Rect rect = new Rect(x, curY, num, num);
                Rect rect2 = new Rect(rect.x + num + 10f, rect.y, width - num - 10f, 999f);
                Rect rect3 = (showLabels ? new Rect(rect.x, rect.y, rect.width + (rect2.x - rect.xMax) + Math.Min(Widgets.CalcSize(labelCap).x, rect2.width), rect.height) : rect);
                float num2 = (showLabels ? Widgets.CalcHeight(labelCap, rect2.width) : 0f);
                if (showLabels && iconSizeFactor > 1f)
                {
                    rect2.y += (num - num2) / 2f;
                }
                if (!dontDraw)
                {
                    if (doBackground)
                    {
                        Widgets.DoLabelBackground(rect3);
                    }
                    if (Mouse.Over(rect3))
                    {
                        Get.Tooltips.RegisterTip(rect3, useEffectDrawRequest, null, null);
                    }
                    GUIExtra.DrawHighlightIfMouseover(rect3, true, true, true, true, true);
                    GUI.color = useEffectDrawRequest.IconColor;
                    GUIExtra.DrawTexture(rect, useEffectDrawRequest.Icon);
                    GUI.color = Color.white;
                    if (showLabels)
                    {
                        Widgets.Label(rect2, labelCap, true, null, null, false);
                    }
                }
                curY += (showLabels ? Math.Max(num, num2) : num);
                if (!useEffectDrawRequest.InnerUseEffects.NullOrEmpty<UseEffectDrawRequest>())
                {
                    TipSubjectDrawer.DoUseEffects(useEffectDrawRequest.InnerUseEffects, x + 10f, ref curY, width - 10f, dontDraw, 5f, iconSizeFactor, false, showLabels);
                }
            }
        }

        public static string GetMissAndCritChanceText(float missChance, float critChance, Actor forActor)
        {
            string text = "";
            if (missChance > 0f)
            {
                if (forActor != null)
                {
                    missChance *= forActor.ConditionsAccumulated.MissChanceFactor;
                }
                if (forActor != null && forActor.ConditionsAccumulated.MinMissChanceOverride > missChance && missChance > 0f)
                {
                    Condition minMissChanceOverrideCondition = forActor.ConditionsAccumulated.MinMissChanceOverrideCondition;
                    text = RichText.Miss("{0} ({1})".Formatted("MissChance".Translate(forActor.ConditionsAccumulated.MinMissChanceOverride.ToStringPercent(false)), minMissChanceOverrideCondition.LabelBase));
                }
                else
                {
                    text = RichText.Miss("MissChance".Translate(missChance.ToStringPercent(false)));
                }
            }
            if (critChance > 0f)
            {
                if (forActor != null)
                {
                    critChance *= forActor.ConditionsAccumulated.CritChanceFactor;
                }
                text = text.AppendedWithComma(RichText.LightRed("CritChance".Translate(critChance.ToStringPercent(false))));
            }
            return text;
        }

        public static void DoMissAndCritChance(float missChance, float critChance, Actor forActor, ref float curY, float width, bool dontDraw = false)
        {
            if (missChance <= 0f && critChance <= 0f)
            {
                return;
            }
            TipSubjectDrawer.DoLabel(TipSubjectDrawer.GetMissAndCritChanceText(missChance, critChance, forActor), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
        }

        public static void DoConditions(List<ConditionDrawRequest> conditions, float x, ref float curY, float width, bool dontDraw = false, float initialGap = 0f, float iconSizeFactor = 1f, bool doNewConditionAnimations = false, bool doBackground = false, bool showLabels = true)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return;
            }
            curY += initialGap;
            float num = 20f * iconSizeFactor;
            foreach (ConditionDrawRequest conditionDrawRequest in conditions)
            {
                bool flag = showLabels;
                if (doNewConditionAnimations && Clock.UnscaledTime - conditionDrawRequest.TimeStartedAffectingActor < 2.9f)
                {
                    flag = true;
                }
                string labelCap = conditionDrawRequest.LabelCap;
                Rect rect = new Rect(x, curY, num, num);
                Rect rect2 = new Rect(rect.x + num + 10f, rect.y, width - num - 10f, 999f);
                Rect rect3 = (flag ? new Rect(rect.x, rect.y, rect.width + (rect2.x - rect.xMax) + Math.Min(Widgets.CalcSize(labelCap).x, rect2.width), rect.height) : rect);
                float num2 = (flag ? Widgets.CalcHeight(labelCap, rect2.width) : 0f);
                if (flag && iconSizeFactor > 1f)
                {
                    rect2.y += (num - num2) / 2f;
                }
                if (!dontDraw)
                {
                    if (doBackground)
                    {
                        Widgets.DoLabelBackground(rect3);
                    }
                    if (doNewConditionAnimations)
                    {
                        ExpandingIconAnimation.Do(rect, conditionDrawRequest.Icon, conditionDrawRequest.IconColor, conditionDrawRequest.TimeStartedAffectingActor, 5f, 0.6f, 0.55f);
                    }
                    if (Mouse.Over(rect3))
                    {
                        Get.Tooltips.RegisterTip(rect3, conditionDrawRequest, null, null);
                    }
                    GUIExtra.DrawHighlightIfMouseover(rect3, true, true, true, true, true);
                    GUI.color = conditionDrawRequest.IconColor;
                    GUIExtra.DrawTexture(rect, conditionDrawRequest.Icon);
                    GUI.color = Color.white;
                    if (flag)
                    {
                        Widgets.Label(rect2, labelCap, true, null, null, false);
                    }
                }
                curY += (flag ? Math.Max(num, num2) : num);
            }
        }

        public static void DoLabel(string label, float x, ref float curY, float width, bool dontDraw = false, TextAnchor align = TextAnchor.UpperLeft, Color? background = null)
        {
            curY += 10f;
            float num = Widgets.CalcHeight(label, width);
            if (!dontDraw)
            {
                if (align != TextAnchor.UpperLeft)
                {
                    Widgets.Align = align;
                }
                if (background != null)
                {
                    GUIExtra.DrawRect(new Rect(x, curY, width, num + 1f), background.Value);
                }
                Widgets.Label(new Rect(x, curY, width, num + 10f), label, true, null, null, false);
                if (align != TextAnchor.UpperLeft)
                {
                    Widgets.ResetAlign();
                }
            }
            curY += num;
        }

        public static void DoUseRange(IUsable usable, float x, ref float curY, float width, bool dontDraw = false)
        {
            int useRange = usable.UseRange;
            string text = ((useRange <= 1) ? "Melee".Translate() : useRange.ToStringCached());
            if (useRange > 1)
            {
                Entity wieldingActorOrStructure = usable.UseEffects.GetWieldingActorOrStructure();
                if (wieldingActorOrStructure != null && !wieldingActorOrStructure.IsNowControlledActor && wieldingActorOrStructure.Spawned)
                {
                    if (Get.NowControlledActor.Position.GetGridDistance(wieldingActorOrStructure.Position) <= useRange)
                    {
                        text = text.AppendedWithSpace("({0})".Formatted("YouAreInRange".Translate()));
                    }
                    else
                    {
                        text = text.AppendedWithSpace("({0})".Formatted("YouAreOutOfRange".Translate()));
                    }
                }
            }
            TipSubjectDrawer.DoLabel(RichText.Grayed("UseRange".Translate(text)), x, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
        }

        public static void DoUseRange(int range, float x, ref float curY, float width, bool dontDraw = false)
        {
            string text = ((range <= 1) ? "Melee".Translate() : range.ToStringCached());
            TipSubjectDrawer.DoLabel(RichText.Grayed("UseRange".Translate(text)), x, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
        }

        public static void DoRarity(string label, Color color, float x, float y, float width, bool dontDraw = false)
        {
            if (dontDraw)
            {
                return;
            }
            GUI.color = color;
            Widgets.Label(new Rect(x, y, width, 20f), label, true, null, null, false);
            GUI.color = Color.white;
        }

        public static void DoHorizontalLine(float x, ref float curY, float width, bool dontDraw = false)
        {
            curY += 1f;
            if (!dontDraw)
            {
                GUIExtra.DrawHorizontalLine(new Vector2(x, curY), width, new Color(0.42f, 0.42f, 0.42f), 1f);
            }
            curY += 1f;
        }

        public static void DoSection(string label, float x, ref float curY, float width, bool dontDraw = false)
        {
            curY += 2f;
            GUI.color = new Color(0.45f, 0.45f, 0.45f);
            Widgets.FontBold = true;
            TipSubjectDrawer.DoLabel(label, x, ref curY, width, dontDraw, TextAnchor.UpperCenter, new Color?(new Color(1f, 1f, 1f, 0.1f)));
            Widgets.FontBold = false;
            GUI.color = Color.white;
            curY += 5f;
        }

        public static void DoManaAndStaminaCost(int manaCost, int staminaCost, ref float curY, float width, bool dontDraw = false)
        {
            if (manaCost != 0)
            {
                TipSubjectDrawer.DoLabel("CostsMana".Translate(RichText.Mana(manaCost)), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
            }
            if (staminaCost != 0)
            {
                TipSubjectDrawer.DoLabel("CostsStamina".Translate(RichText.Stamina(staminaCost)), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
            }
        }

        public static void DoCooldown(int cooldownTurns, int? lastUseSequence, ref float curY, float width, bool dontDraw = false)
        {
            if (cooldownTurns <= 0)
            {
                return;
            }
            string text = "CooldownTip".Translate(StringUtility.TurnsString(cooldownTurns));
            if (lastUseSequence != null && Get.TurnManager.CurrentSequence - lastUseSequence.Value < cooldownTurns * 12)
            {
                int num = (cooldownTurns * 12 - (Get.TurnManager.CurrentSequence - lastUseSequence.Value)) / 12;
                text = "{0} ({1})".Formatted(text, "CooldownTurnsRemaining".Translate(num));
            }
            TipSubjectDrawer.DoLabel(RichText.Grayed(text), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
        }

        public static void DoSequencePerUseMultiplier(float mul, TipSubjectDrawer.SequencePerUseUsableType type, ref float curY, float width, bool dontDraw = false)
        {
            if (mul == 1f)
            {
                return;
            }
            if (type == TipSubjectDrawer.SequencePerUseUsableType.Structure)
            {
                if (mul == 0f)
                {
                    TipSubjectDrawer.DoLabel(RichText.Grayed("InteractingWithThisStructureCostsNoTurn".Translate()), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                    return;
                }
                if (mul < 1f)
                {
                    TipSubjectDrawer.DoLabel(RichText.Grayed("InteractingWithThisStructureCostsXOfATurn".Translate(mul.ToStringPercent(false))), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                    return;
                }
                TipSubjectDrawer.DoLabel(RichText.Grayed("InteractingWithThisStructureCostsXTurns".Translate(mul.ToStringPercent(false))), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                return;
            }
            else if (type == TipSubjectDrawer.SequencePerUseUsableType.Spell)
            {
                if (mul == 0f)
                {
                    TipSubjectDrawer.DoLabel(RichText.Grayed("UsingThisSpellCostsNoTurn".Translate()), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                    return;
                }
                if (mul < 1f)
                {
                    TipSubjectDrawer.DoLabel(RichText.Grayed("UsingThisSpellCostsXOfATurn".Translate(mul.ToStringPercent(false))), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                    return;
                }
                TipSubjectDrawer.DoLabel(RichText.Grayed("UsingThisSpellCostsXTurns".Translate(mul.ToStringPercent(false))), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                return;
            }
            else if (type == TipSubjectDrawer.SequencePerUseUsableType.NativeWeapon)
            {
                if (mul == 0f)
                {
                    TipSubjectDrawer.DoLabel(RichText.Grayed("UsingThisNativeWeaponCostsNoTurn".Translate()), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                    return;
                }
                if (mul < 1f)
                {
                    TipSubjectDrawer.DoLabel(RichText.Grayed("UsingThisNativeWeaponCostsXOfATurn".Translate(mul.ToStringPercent(false))), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                    return;
                }
                TipSubjectDrawer.DoLabel(RichText.Grayed("UsingThisNativeWeaponCostsXTurns".Translate(mul.ToStringPercent(false))), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                return;
            }
            else
            {
                if (mul == 0f)
                {
                    TipSubjectDrawer.DoLabel(RichText.Grayed("UsingThisItemCostsNoTurn".Translate()), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                    return;
                }
                if (mul < 1f)
                {
                    TipSubjectDrawer.DoLabel(RichText.Grayed("UsingThisItemCostsXOfATurn".Translate(mul.ToStringPercent(false))), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                    return;
                }
                TipSubjectDrawer.DoLabel(RichText.Grayed("UsingThisItemCostsXTurns".Translate(mul.ToStringPercent(false))), 0f, ref curY, width, dontDraw, TextAnchor.UpperLeft, null);
                return;
            }
        }

        public const int Pad = 10;

        public const int Gap = 10;

        public const int GapSmall = 5;

        public const int BigIconSize = 40;

        public const int SmallIconSize = 20;

        public const int DefaultWidth = 350;

        public static readonly Color BackgroundColor = new Color(0.15f, 0.15f, 0.15f);

        public enum SequencePerUseUsableType
        {
            Item,

            Structure,

            Spell,

            NativeWeapon
        }
    }
}