using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class SeenEntitiesDrawer
    {
        public Entity MouseoverEntity
        {
            get
            {
                return this.mouseoverEntity;
            }
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseDown && Event.current.type != EventType.MouseUp)
            {
                return;
            }
            if (this.entries.Count == 0)
            {
                return;
            }
            float num = Widgets.VirtualWidth - 220f - 7f;
            Actor nextSeenActorTurnForUI = Get.TurnManager.NextSeenActorTurnForUI;
            this.mouseoverEntity = null;
            int i = this.entries.Count - 1;
            while (i >= 0)
            {
                SeenEntitiesDrawer.Entry entry = this.entries[i];
                if (Event.current.type == EventType.Repaint)
                {
                    if (entry.visible)
                    {
                        entry.pct += Clock.UnscaledDeltaTime * 2f;
                        if (entry.pct >= 0.98f)
                        {
                            entry.anyAbove = i > 0 && this.entries[i - 1].pct >= 0.98f;
                            entry.anyBelow = i < this.entries.Count - 1 && this.entries[i + 1].pct >= 0.98f;
                        }
                    }
                    else
                    {
                        entry.pct -= Clock.UnscaledDeltaTime * 2f;
                        if (entry.pct < 0f)
                        {
                            this.entries.RemoveAt(i);
                            goto IL_01C5;
                        }
                    }
                    entry.pct = Calc.Clamp01(entry.pct);
                    goto IL_0161;
                }
                goto IL_0161;
            IL_01C5:
                i--;
                continue;
            IL_0161:
                if (nextSeenActorTurnForUI == entry.entity)
                {
                    entry.lastTurnUnscaledTime = Clock.UnscaledTime;
                }
                this.DoRow(num, entry.y.CurrentValue, 220f, entry.entity, Calc.Smooth(entry.pct), nextSeenActorTurnForUI, entry.visible, entry.anyAbove, entry.anyBelow, entry.lastTurnUnscaledTime);
                this.entries[i] = entry;
                goto IL_01C5;
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < this.entries.Count; i++)
            {
                SeenEntitiesDrawer.Entry entry = this.entries[i];
                entry.y.SetTarget(Calc.Lerp(entry.y.Target, this.GetTargetY(i), 10f * Clock.FixedUnscaledDeltaTime));
                this.entries[i] = entry;
            }
        }

        private void DoRow(float x, float y, float width, Entity entity, float alpha, Actor nextActorTurn, bool visible, bool anyAbove, bool anyBelow, float lastTurnUnscaledTime)
        {
            x += (1f - alpha) * 150f;
            x -= Calc.ResolveFadeInStayOut(Clock.UnscaledTime - lastTurnUnscaledTime, 0.07f, 0f, 0.23f) * 22f;
            Rect rect = new Rect(x, y, width + 7f, 30f);
            rect = rect.ExpandedByPct(Calc.ResolveFadeInStayOut(Clock.UnscaledTime - lastTurnUnscaledTime, 0.07f, 0f, 0.23f) * 0.05f);
            if (Event.current.type == EventType.Repaint)
            {
                Color color = this.GetColor(entity).WithAlphaFactor(0.4f);
                float newEntityHighlightFactor = Get.IconOverlayDrawer.GetNewEntityHighlightFactor(entity);
                if (newEntityHighlightFactor > 0f)
                {
                    color = Color.Lerp(color, color.Lighter(0.25f).WithAlpha(1f), newEntityHighlightFactor);
                }
                color = GUIExtra.HighlightedColorIfMouseover(rect, color, true, 0.15f);
                if (Mouse.Over(rect))
                {
                    this.mouseoverEntity = entity;
                }
                Get.Tooltips.RegisterTip(rect, entity, null, null);
                if (entity.MaxHP != 0)
                {
                    Get.ProgressBarDrawer.Draw(rect, entity.HP, entity.MaxHP, color.WithAlpha(1f), color.a, 1f, true, false, new int?(entity.InstanceID), ProgressBarDrawer.ValueChangeDirection.Constant, Get.InteractionManager.GetLostHPRangeForUI(entity), !anyAbove, false, false, !anyBelow, true, false, null);
                }
                else
                {
                    GUIExtra.DrawRoundedRectBump(rect, color, false, !anyAbove, false, false, !anyBelow, null);
                }
                if (entity.Spawned)
                {
                    Rect rect2 = rect.LeftPart(rect.height).ContractedBy(2f);
                    float angleXZFromCamera_Simple = Vector3Utility.GetAngleXZFromCamera_Simple(entity.RenderPositionComputedCenter);
                    GUI.color = new Color(0.8f, 0.8f, 0.8f, 0.32f);
                    GUIExtra.DrawTextureRotated(rect2, SeenEntitiesDrawer.ArrowTex, angleXZFromCamera_Simple + 180f, null);
                    GUI.color = Color.white;
                }
                Actor actor = entity as Actor;
                if (actor != null)
                {
                    this.DoConditionIcons(actor, rect.x - 4f, rect.y, alpha);
                }
                if (Get.InteractionManager.PointedAtEntity == entity)
                {
                    GUIExtra.DrawRoundedRectOutline(rect.ContractedBy(1f), SeenEntitiesDrawer.PointedAtEntityOutlineColor, 2f, !anyAbove, false, false, !anyBelow);
                }
                Rect rect3 = new Rect(x + width - 30f, y, 30f, 30f).ContractedBy(1f);
                if (entity == nextActorTurn && !entity.IsNowControlledActor)
                {
                    GUI.color = new Color(1f, 1f, 1f, alpha);
                    TimeDrawer.DrawResolvingTurnsIcon(rect3, 1f);
                }
                else
                {
                    GUI.color = entity.IconColor.WithAlphaFactor(alpha);
                    GUI.DrawTexture(rect3, entity.Icon);
                }
                Item item = entity as Item;
                if (item != null && !item.Identified)
                {
                    GUI.color = new Color(0.59f, 0.69f, 1f, alpha);
                }
                else
                {
                    GUI.color = new Color(0.85f, 0.85f, 0.85f, alpha);
                }
                Rect rect4 = new Rect(x, y, width - 30f, 30f);
                Widgets.Align = TextAnchor.MiddleRight;
                Widgets.Label(rect4, RichText.Bold(entity.LabelCap).Truncate(rect4.width), true, null, null, false);
                Widgets.ResetAlign();
                GUI.color = Color.white;
                Actor actor2 = entity as Actor;
                if (actor2 != null)
                {
                    float num = Clock.UnscaledTime - actor2.LastTimeCausedPlayerToLoseHP;
                    float num2 = 1f - Calc.ResolveFadeIn(num, 1f);
                    if (num2 > 0f)
                    {
                        GUIExtra.DrawRoundedRectOutline(rect, new Color(1f, 0.18f, 0.18f, num2), 6f, !anyAbove, false, false, !anyBelow);
                    }
                }
                if (visible && entity == this.lastEntityScrolledTo)
                {
                    float num3 = Clock.UnscaledTime - this.lastEntityScrolledToTime;
                    float num4 = Calc.ResolveFadeInStayOut(num3, 0f, 1f, 0.1f);
                    if (num4 > 0f)
                    {
                        float num5 = ((num3 < 0.13f) ? (-(1f - num3 / 0.13f) * 7f) : 0f);
                        GUI.color = new Color(1f, 1f, 1f, num4);
                        GUIExtra.DrawTextureRotated(new Rect(x - 30f + num5, y, 30f, 30f), SeenEntitiesDrawer.ArrowTex, -90f, null);
                        GUI.color = Color.white;
                    }
                }
            }
            if (visible)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.Over(rect))
                {
                    List<ValueTuple<string, Action, string>> contextMenuOptions = SeenEntitiesDrawer.GetContextMenuOptions(entity);
                    Get.WindowManager.OpenContextMenu(contextMenuOptions, entity.LabelCap);
                    Event.current.Use();
                }
                if (Widgets.ButtonInvisible(rect, false, false) && Event.current.button == 0 && !entity.IsNowControlledActor)
                {
                    Get.FPPControllerGOC.RotateToFace(entity.RenderPositionComputedCenter);
                    Get.Sound_Rotation.PlayOneShot(null, 1f, 1f);
                }
            }
        }

        private void DoConditionIcons(Actor actor, float startX, float y, float alpha)
        {
            float num = startX;
            foreach (ConditionDrawRequest conditionDrawRequest in actor.ConditionsAccumulated.AllConditionDrawRequestsPlusExtra)
            {
                Rect rect = new Rect(num - 30f, y, 30f, 30f);
                ExpandingIconAnimation.Do(rect, conditionDrawRequest.Icon, conditionDrawRequest.IconColor, conditionDrawRequest.TimeStartedAffectingActor, 1f, 0.6f, 0.55f);
                if (Mouse.Over(rect))
                {
                    Get.Tooltips.RegisterTip(rect, conditionDrawRequest, null, null);
                }
                GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
                GUI.color = conditionDrawRequest.IconColor.WithAlphaFactor(alpha);
                GUI.DrawTexture(rect, conditionDrawRequest.Icon);
                GUI.color = Color.white;
                num -= 34f;
            }
        }

        public void OnEntitiesVisibilityChanged(List<Entity> entitiesNoLongerSeen, List<Entity> entitiesNewlySeen)
        {
            for (int i = 0; i < entitiesNoLongerSeen.Count; i++)
            {
                if (SeenEntitiesDrawer.ShouldList(entitiesNoLongerSeen[i]))
                {
                    for (int j = 0; j < this.entries.Count; j++)
                    {
                        if (this.entries[j].entity == entitiesNoLongerSeen[i])
                        {
                            SeenEntitiesDrawer.Entry entry = this.entries[j];
                            entry.visible = false;
                            this.entries[j] = entry;
                            break;
                        }
                    }
                }
            }
            for (int k = 0; k < entitiesNewlySeen.Count; k++)
            {
                if (SeenEntitiesDrawer.ShouldList(entitiesNewlySeen[k]))
                {
                    bool flag = false;
                    for (int l = 0; l < this.entries.Count; l++)
                    {
                        if (this.entries[l].entity == entitiesNewlySeen[k])
                        {
                            SeenEntitiesDrawer.Entry entry2 = this.entries[l];
                            entry2.visible = true;
                            this.entries[l] = entry2;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        SeenEntitiesDrawer.Entry entry3 = new SeenEntitiesDrawer.Entry
                        {
                            entity = entitiesNewlySeen[k],
                            visible = true
                        };
                        float num = this.GetTargetY(this.entries.Count) + 100f;
                        entry3.y.SetTarget(num, num);
                        this.entries.Add(entry3);
                    }
                }
            }
        }

        private Color GetColor(Entity entity)
        {
            if (entity.Spec.SpecialSeenEntitiesDrawerColor != null)
            {
                return entity.Spec.SpecialSeenEntitiesDrawerColor.Value;
            }
            Actor actor = entity as Actor;
            if (actor != null)
            {
                return actor.HostilityColor.Lighter(0.2f);
            }
            if (entity is Item)
            {
                return SeenEntitiesDrawer.ItemColor;
            }
            if (entity is Structure && entity.Spec.Structure.IsSpecial)
            {
                return SeenEntitiesDrawer.SpecialStructureColor;
            }
            return Color.white;
        }

        private float GetTargetY(int index)
        {
            return (float)(420 + index * 29);
        }

        public static bool ShouldList(Entity entity)
        {
            return !entity.IsNowControlledActor && (entity is Actor || entity is Item || (entity is Structure && entity.Spec.Structure.IsSpecial) || entity.Spec.SpecialSeenEntitiesDrawerColor != null);
        }

        public void OnWorldAboutToRegenerate()
        {
            this.entries.Clear();
        }

        public void OnScrolledToEntity(Entity entity)
        {
            this.lastEntityScrolledTo = entity;
            this.lastEntityScrolledToTime = Clock.UnscaledTime;
        }

        public void OnActorTookTurn(Actor actor)
        {
            for (int i = 0; i < this.entries.Count; i++)
            {
                if (this.entries[i].entity == actor)
                {
                    SeenEntitiesDrawer.Entry entry = this.entries[i];
                    entry.lastTurnUnscaledTime = Clock.UnscaledTime;
                    this.entries[i] = entry;
                    return;
                }
            }
        }

        public Entity GetNextEntity(Entity current, bool next)
        {
            if (next)
            {
                if (current != null)
                {
                    bool flag = false;
                    for (int i = 0; i < this.entries.Count; i++)
                    {
                        if (this.entries[i].visible)
                        {
                            if (flag)
                            {
                                return this.entries[i].entity;
                            }
                            if (this.entries[i].entity == current)
                            {
                                flag = true;
                            }
                        }
                    }
                }
                for (int j = 0; j < this.entries.Count; j++)
                {
                    if (this.entries[j].visible)
                    {
                        return this.entries[j].entity;
                    }
                }
            }
            else
            {
                if (current != null)
                {
                    bool flag2 = false;
                    for (int k = this.entries.Count - 1; k >= 0; k--)
                    {
                        if (this.entries[k].visible)
                        {
                            if (flag2)
                            {
                                return this.entries[k].entity;
                            }
                            if (this.entries[k].entity == current)
                            {
                                flag2 = true;
                            }
                        }
                    }
                }
                for (int l = this.entries.Count - 1; l >= 0; l--)
                {
                    if (this.entries[l].visible)
                    {
                        return this.entries[l].entity;
                    }
                }
            }
            return null;
        }

        public static List<ValueTuple<string, Action, string>> GetContextMenuOptions(Entity entity)
        {
            List<ValueTuple<string, Action, string>> list = new List<ValueTuple<string, Action, string>>();
            list.Add(new ValueTuple<string, Action, string>("LookAt".Translate(), delegate
            {
                Get.FPPControllerGOC.RotateToFace(entity.RenderPositionComputedCenter);
                Get.Sound_Rotation.PlayOneShot(null, 1f, 1f);
            }, null));
            InteractionManager.PossibleInteraction? interaction = Get.InteractionManager.GetInteraction(entity, Vector3IntUtility.Up, null);
            if (interaction != null)
            {
                Actor actor2 = entity as Actor;
                GenericUsable genericUsable = ((actor2 != null) ? actor2.UsableOnTalk : null);
                string text = ((interaction.Value.usable == null || interaction.Value.usable == entity || interaction.Value.usable == genericUsable) ? "InteractWith".Translate() : "UseOn".Translate(interaction.Value.usable));
                if (interaction.Value.finalActionGetter == null && interaction.Value.pathMode == PathFinder.Request.Mode.None)
                {
                    string text2 = (interaction.Value.failReason.NullOrEmpty() ? "Disabled".Translate() : interaction.Value.failReason);
                    list.Add(new ValueTuple<string, Action, string>(text, null, text2));
                }
                else
                {
                    list.Add(new ValueTuple<string, Action, string>(text, delegate
                    {
                        Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                        Get.InteractionManager.PerformInteraction(interaction.Value, entity);
                    }, null));
                }
            }
            list.Add(new ValueTuple<string, Action, string>("MoveTo".Translate(), delegate
            {
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                SeenEntitiesDrawer.MoveTo(entity);
            }, null));
            Actor actor = entity as Actor;
            if (actor != null && Get.World.CanTouch(Get.NowControlledActor.Position, actor.Position, Get.NowControlledActor) && !actor.IsHostile(Get.NowControlledActor) && actor.Inventory.EquippedWeapon == null)
            {
                using (List<Item>.Enumerator enumerator = Get.NowControlledActor.Inventory.AllNonEquippedItems.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Item item = enumerator.Current;
                        if (item.Spec.Item.IsEquippableWeapon)
                        {
                            Func<Action> <> 9__4;
                            list.Add(new ValueTuple<string, Action, string>("GiveItem".Translate(item), delegate
                            {
                                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                                Func<Action> func;
                                if ((func = <> 9__4) == null)
                                {
                                    func = (<> 9__4 = () => new Action_Transfer(Get.Action_Transfer, Get.NowControlledActor, actor, item));
                                }
                                ActionViaInterfaceHelper.TryDo(func);
                            }, null));
                        }
                    }
                }
            }
            return list;
        }

        public static void MoveTo(Target target)
        {
            int num = Calc.Clamp(Get.NowControlledActor.Position.GetGridDistance(target.Position) + 6, 15, 40);
            List<Vector3Int> list = Get.PathFinder.FindPath(Get.NowControlledActor.Position, target.Position, new PathFinder.Request(PathFinder.Request.Mode.ToCell, Get.NowControlledActor), num, null);
            if (list == null)
            {
                list = Get.PathFinder.FindPath(Get.NowControlledActor.Position, target.Position, new PathFinder.Request(PathFinder.Request.Mode.Touch, Get.NowControlledActor), num, null);
            }
            if (list != null)
            {
                Get.PlannedPlayerActions.Set(Get.InteractionManager.MakeMoveToActions(list, Get.KeyBinding_OneStep.HeldDown), null);
            }
        }

        public void OnSwitchedNowControlledActor(Actor prevActor)
        {
            for (int i = this.entries.Count - 1; i >= 0; i--)
            {
                if (this.entries[i].entity.IsNowControlledActor)
                {
                    this.entries.RemoveAt(i);
                    break;
                }
            }
            if (prevActor != null && Get.NowControlledActor.Sees(prevActor, null))
            {
                this.OnEntitiesVisibilityChanged(new List<Entity>(), new List<Entity> { prevActor });
            }
        }

        private List<SeenEntitiesDrawer.Entry> entries = new List<SeenEntitiesDrawer.Entry>();

        private Entity mouseoverEntity;

        private Entity lastEntityScrolledTo;

        private float lastEntityScrolledToTime;

        private const int MarginTop = 420;

        private const int MarginRight = 7;

        private const int Width = 220;

        private const int RowHeight = 30;

        private const int AnimationOffset = 150;

        private const int AnimationJumpOffset = 100;

        private const float FadeInOutSpeed = 2f;

        private const float PosLerpSpeed = 10f;

        private const float ConditionIconGap = 4f;

        private static readonly Color ItemColor = new Color(0.81f, 0.65f, 0f);

        private static readonly Color SpecialStructureColor = new Color(0.6f, 0.2f, 0.8f);

        public static readonly Color PointedAtEntityOutlineColor = new Color(1f, 1f, 1f, 0.8f);

        private static readonly Texture2D ArrowTex = Assets.Get<Texture2D>("Textures/UI/Arrow");

        private struct Entry
        {
            public Entity entity;

            public bool visible;

            public float pct;

            public InterpolatedFloat y;

            public bool anyAbove;

            public bool anyBelow;

            public float lastTurnUnscaledTime;
        }
    }
}