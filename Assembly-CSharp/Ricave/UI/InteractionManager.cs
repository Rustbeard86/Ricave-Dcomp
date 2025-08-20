using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class InteractionManager
    {
        public Entity PointedAtEntity
        {
            get
            {
                return this.pointedAtEntity;
            }
        }

        public BodyPart PointedAtBodyPart
        {
            get
            {
                return this.pointedAtBodyPart;
            }
        }

        public Vector3? PointedAtExactPos
        {
            get
            {
                return this.pointedAtExactPos;
            }
        }

        public Entity HighlightedEntity
        {
            get
            {
                return this.highlightedEntity;
            }
        }

        public BodyPart HighlightedBodyPart
        {
            get
            {
                return this.highlightedBodyPart;
            }
        }

        public PlannedPlayerActions PlannedPlayerActions
        {
            get
            {
                return this.plannedPlayerActions;
            }
        }

        public InteractionManager.PossibleInteraction? LastPossibleInteraction
        {
            get
            {
                return this.lastPossibleInteraction;
            }
        }

        public HashSet<ValueTuple<Entity, IUsable>> EntitiesAffectedByAoE
        {
            get
            {
                return this.entitiesAffectedByAnyAoE;
            }
        }

        public static KeyBindingSpec TargetBodyPartKeyBinding
        {
            get
            {
                return Get.KeyBinding_Fly;
            }
        }

        public float LastTimeTriedDisabledAction
        {
            get
            {
                return this.lastTimeTriedDisabledAction;
            }
        }

        public string LastTriedDisabledActionReason
        {
            get
            {
                return this.lastTriedDisabledActionReason;
            }
        }

        public InteractionManager()
        {
            this.RaycastShouldIgnoreCached = new Predicate<GameObject>(this.RaycastShouldIgnore);
        }

        public void Update()
        {
            try
            {
                Profiler.Begin("plannedPlayerActions.Update()");
                this.plannedPlayerActions.Update();
            }
            catch (Exception ex)
            {
                Log.Error("Error in plannedPlayerActions.Update().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("CheckWhereMousePointsAt()");
                this.CheckWhereMousePointsAt();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in CheckWhereMousePointsAt().", ex2);
            }
            finally
            {
                Profiler.End();
            }
        }

        private void CheckWhereMousePointsAt()
        {
            Entity entity = this.pointedAtEntity;
            BodyPart bodyPart = this.pointedAtBodyPart;
            this.pointedAtLedge = null;
            this.pointedAtEntity = null;
            this.pointedAtExactPos = null;
            this.pointedAtBodyPart = null;
            this.raycastHitDirection = default(Vector3Int);
            this.highlightedEntity = null;
            this.highlightedBodyPart = null;
            this.lastPossibleInteraction = null;
            this.entitiesAffectedByAnyAoE.Clear();
            this.lastPointedAtEntityCalcHadInspectMode = KeyCodeUtility.InspectHeldDown;
            if (!Get.UI.WorldInputBlocked)
            {
                bool flag = !Get.NowControlledActor.CanFly && !KeyCodeUtility.InspectHeldDown && Get.NowControlledActor.CanJumpOffLedge && Get.UseOnTargetUI.TargetingUsable == null;
                Profiler.Begin("RaycastUtility.RaycastFromCamera()");
                Vector3 vector;
                GameObject gameObject = RaycastUtility.RaycastFromCamera(8f, out this.raycastHitDirection, out vector, !flag, true, true, this.RaycastShouldIgnoreCached);
                Profiler.End();
                if (gameObject != null)
                {
                    this.pointedAtExactPos = new Vector3?(vector);
                    if (gameObject.layer == Get.Layers.FloorMarkersLayer)
                    {
                        Vector3Int vector3Int = gameObject.transform.position.RoundToVector3Int();
                        List<Vector3Int> list = Get.PathFinder.FindPath(Get.NowControlledActor.Position, vector3Int, new PathFinder.Request(PathFinder.Request.Mode.ToCell, Get.NowControlledActor), 15, this.tmpListForPathFinder);
                        if (list != null)
                        {
                            this.pointedAtLedge = new Vector3Int?(vector3Int);
                            if (!KeyCodeUtility.InspectHeldDown)
                            {
                                Get.CellHighlighter.HighlightPath(list);
                                return;
                            }
                        }
                    }
                    else
                    {
                        EntityGOC cachedEntityGOCInParent = this.GetCachedEntityGOCInParent(gameObject);
                        if (cachedEntityGOCInParent != null)
                        {
                            Entity entity2 = cachedEntityGOCInParent.Entity;
                            this.pointedAtEntity = entity2;
                            Actor actor = entity2 as Actor;
                            if (actor != null && InteractionManager.TargetBodyPartKeyBinding.HeldDown)
                            {
                                this.pointedAtBodyPart = BodyPartUtility.GetBodyPartFromRaycastHit(actor, gameObject, vector, false);
                            }
                            if (!KeyCodeUtility.InspectHeldDown)
                            {
                                InteractionManager.PossibleInteraction? interaction = this.GetInteraction(entity2, this.raycastHitDirection, this.pointedAtBodyPart);
                                if (interaction != null)
                                {
                                    this.lastPossibleInteraction = interaction;
                                    InteractionManager.PossibleInteraction value = interaction.Value;
                                    bool flag2 = false;
                                    if (value.pathMode != PathFinder.Request.Mode.None)
                                    {
                                        Vector3Int vector3Int2 = value.pathDestinationOverride ?? entity2.Position;
                                        List<Vector3Int> list2 = Get.PathFinder.FindPath(Get.NowControlledActor.Position, vector3Int2, new PathFinder.Request(value.pathMode, Get.NowControlledActor), 15, this.tmpListForPathFinder);
                                        if (list2 != null)
                                        {
                                            Get.CellHighlighter.HighlightPath(list2);
                                        }
                                        else
                                        {
                                            flag2 = true;
                                            if (value.pathMode == PathFinder.Request.Mode.ToCell)
                                            {
                                                Get.CellHighlighter.HighlightCell(vector3Int2, CellHighlighter.Red);
                                            }
                                        }
                                    }
                                    if (value.highlightEntity)
                                    {
                                        if (value.highlightBodyPart && actor != null)
                                        {
                                            if (entity != entity2 || bodyPart != this.pointedAtBodyPart)
                                            {
                                                Get.Sound_HighlightedEntity.PlayOneShot(null, 1f, 1f);
                                            }
                                            actor.ActorGOC.HighlightBodyPart(this.pointedAtBodyPart);
                                            this.highlightedBodyPart = this.pointedAtBodyPart;
                                        }
                                        else if (entity != entity2)
                                        {
                                            Get.Sound_HighlightedEntity.PlayOneShot(null, 1f, 1f);
                                        }
                                        Get.GameObjectHighlighter.Highlight(entity2, ((value.finalActionGetter == null && value.pathMode == PathFinder.Request.Mode.None) || flag2) ? new Color?(Color.red) : null);
                                        this.highlightedEntity = entity2;
                                    }
                                    Vector3Int? vector3Int3 = null;
                                    if (value.locationTargetingIcon != null)
                                    {
                                        bool flag3;
                                        if (Get.CellsInfo.IsFloorUnder(value.pathDestinationOverride.Value))
                                        {
                                            Get.CellHighlighter.HighlightCell(value.pathDestinationOverride.Value, CellHighlighter.Violet);
                                            flag3 = true;
                                            vector3Int3 = new Vector3Int?(value.pathDestinationOverride.Value);
                                        }
                                        else
                                        {
                                            flag3 = false;
                                        }
                                        Get.CellHighlighter.HighlightIcon(value.pathDestinationOverride.Value, value.locationTargetingIcon, value.locationTargetingIconColor, flag3);
                                    }
                                    if (value.drawAoEEffectsFor != null)
                                    {
                                        this.DrawAoEEffects(value.drawAoEEffectsFor, value.pathDestinationOverride ?? entity2.Position, vector3Int3);
                                    }
                                    if (value.drawAoEEffectsFor2 != null)
                                    {
                                        this.DrawAoEEffects(value.drawAoEEffectsFor2, value.pathDestinationOverride ?? entity2.Position, vector3Int3);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool RaycastShouldIgnore(GameObject gameObject)
        {
            if (gameObject.layer == Get.Layers.FloorMarkersLayer)
            {
                Vector3Int vector3Int = gameObject.transform.position.RoundToVector3Int();
                return Get.FogOfWar.IsFogged(vector3Int) || !GravityUtility.IsAltitudeLowerOrEqual(vector3Int, Get.NowControlledActor.Position, Get.NowControlledActor.Gravity) || Get.PathFinder.FindPath(Get.NowControlledActor.Position, vector3Int, new PathFinder.Request(PathFinder.Request.Mode.ToCell, Get.NowControlledActor), 15, this.tmpListForPathFinder) == null;
            }
            if (gameObject.layer != Get.Layers.InspectModeOnlyLayer || KeyCodeUtility.InspectHeldDown)
            {
                return false;
            }
            EntityGOC componentInParent = gameObject.GetComponentInParent<EntityGOC>();
            InteractionManager.PossibleInteraction? possibleInteraction;
            return !(componentInParent != null) || componentInParent.HasAnyNonInspectModeOnlyCollider || (this.GetInteraction(componentInParent.Entity, Vector3IntUtility.Forward, null) == null || !possibleInteraction.GetValueOrDefault().highlightEntity);
        }

        public void OnGUI()
        {
            this.DoInterruptedLabel();
            if (Event.current.type == EventType.Layout)
            {
                return;
            }
            if (Get.UI.WorldInputBlocked)
            {
                return;
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.hasInteractionToDraw = false;
                this.interactionExtraHint = null;
            }
            bool flag = false;
            if (this.pointedAtLedge != null)
            {
                List<Vector3Int> list = Get.PathFinder.FindPath(Get.NowControlledActor.Position, this.pointedAtLedge.Value, new PathFinder.Request(PathFinder.Request.Mode.ToCell, Get.NowControlledActor), 15, this.tmpListForPathFinder);
                if (list != null)
                {
                    if (!KeyCodeUtility.InspectHeldDown)
                    {
                        Texture2D jumpOffLedgeIcon = InteractionManager.JumpOffLedgeIcon1;
                        Color white = Color.white;
                        Texture2D jumpOffLedgeIcon2 = InteractionManager.JumpOffLedgeIcon2;
                        Color white2 = Color.white;
                        int num = (list.Count - 1) * Get.NowControlledActor.SequencePerMove;
                        bool flag2 = false;
                        string text = null;
                        bool flag3 = Get.NowControlledActor.SequencePerMove > 12;
                        IntRange? intRange = null;
                        int? num2 = null;
                        this.< OnGUI > g__DrawInteractionAfterUI | 89_0(jumpOffLedgeIcon, white, jumpOffLedgeIcon2, white2, num, flag2, text, intRange, num2, false, null, flag3);
                    }
                    if ((Get.KeyBinding_Interact.JustPressed || Get.KeyBinding_Accept.JustPressed) && !Get.KeyBinding_StationaryActionsOnly.HeldDown && !this.ShouldIgnoreClick(null))
                    {
                        this.plannedPlayerActions.Set(this.MakeMoveToActions(list, Get.KeyBinding_OneStep.HeldDown), null);
                    }
                }
            }
            else if (this.pointedAtEntity != null)
            {
                InteractionManager.PossibleInteraction? interaction = this.GetInteraction(this.pointedAtEntity, this.raycastHitDirection, this.pointedAtBodyPart);
                if (interaction != null)
                {
                    flag = true;
                    InteractionManager.PossibleInteraction value = interaction.Value;
                    if (value.finalActionGetter != null || value.pathMode != PathFinder.Request.Mode.None)
                    {
                        if (value.pathMode == PathFinder.Request.Mode.None)
                        {
                            if (!KeyCodeUtility.InspectHeldDown)
                            {
                                Texture2D icon = value.icon1;
                                Color icon1Color = value.icon1Color;
                                Texture2D icon2 = value.icon2;
                                Color icon2Color = value.icon2Color;
                                int finalActionSequence = value.finalActionSequence;
                                bool flag4 = false;
                                string text2 = null;
                                IntRange? dealtDamageRange = value.dealtDamageRange;
                                int? num3 = new int?(this.pointedAtEntity.HP);
                                bool flag5 = InteractionManager.HasUsePrompt(value);
                                bool flag3 = value.finalActionSequence > 12;
                                this.< OnGUI > g__DrawInteractionAfterUI | 89_0(icon, icon1Color, icon2, icon2Color, finalActionSequence, flag4, text2, dealtDamageRange, num3, flag5, null, flag3);
                            }
                            if (Get.KeyBinding_Interact.JustPressed || Get.KeyBinding_Accept.JustPressed)
                            {
                                this.TryPerformInteraction(interaction.Value, this.pointedAtEntity);
                            }
                        }
                        else
                        {
                            List<Vector3Int> list2 = Get.PathFinder.FindPath(Get.NowControlledActor.Position, value.pathDestinationOverride ?? this.pointedAtEntity.Position, new PathFinder.Request(value.pathMode, Get.NowControlledActor), 15, this.tmpListForPathFinder);
                            if (list2 != null)
                            {
                                if (!KeyCodeUtility.InspectHeldDown)
                                {
                                    Texture2D icon3 = value.icon1;
                                    Color icon1Color2 = value.icon1Color;
                                    Texture2D icon4 = value.icon2;
                                    Color icon2Color2 = value.icon2Color;
                                    int num4 = (list2.Count - 1) * Get.NowControlledActor.SequencePerMove + value.finalActionSequence;
                                    bool flag6 = false;
                                    string text3 = null;
                                    IntRange? dealtDamageRange2 = value.dealtDamageRange;
                                    int? num5 = new int?(this.pointedAtEntity.HP);
                                    bool flag7 = InteractionManager.HasUsePrompt(value);
                                    bool flag3 = value.finalActionSequence > 12 || (list2.Count - 1 >= 1 && Get.NowControlledActor.SequencePerMove > 12);
                                    this.< OnGUI > g__DrawInteractionAfterUI | 89_0(icon3, icon1Color2, icon4, icon2Color2, num4, flag6, text3, dealtDamageRange2, num5, flag7, null, flag3);
                                }
                                if ((Get.KeyBinding_Interact.JustPressed || Get.KeyBinding_Accept.JustPressed) && (!Get.KeyBinding_StationaryActionsOnly.HeldDown || list2.Count <= 1))
                                {
                                    this.TryPerformInteraction(interaction.Value, this.pointedAtEntity);
                                }
                            }
                            else
                            {
                                if (!KeyCodeUtility.InspectHeldDown)
                                {
                                    Texture2D icon5 = value.icon1;
                                    Color icon1Color3 = value.icon1Color;
                                    Texture2D icon6 = value.icon2;
                                    Color icon2Color3 = value.icon2Color;
                                    int num6 = 0;
                                    bool flag8 = true;
                                    string text4 = "CantReach".Translate();
                                    int? num2 = new int?(this.pointedAtEntity.HP);
                                    bool flag3 = InteractionManager.HasUsePrompt(value);
                                    this.< OnGUI > g__DrawInteractionAfterUI | 89_0(icon5, icon1Color3, icon6, icon2Color3, num6, flag8, text4, null, num2, flag3, null, false);
                                }
                                if (Get.KeyBinding_Interact.JustPressed || Get.KeyBinding_Accept.JustPressed)
                                {
                                    this.TriedToDoDisabledAction("CantReach".Translate());
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!KeyCodeUtility.InspectHeldDown)
                        {
                            Texture2D icon7 = value.icon1;
                            Color icon1Color4 = value.icon1Color;
                            Texture2D icon8 = value.icon2;
                            Color icon2Color4 = value.icon2Color;
                            int num7 = 0;
                            bool flag9 = true;
                            string failReason = value.failReason;
                            int? num2 = new int?(this.pointedAtEntity.HP);
                            bool flag3 = InteractionManager.HasUsePrompt(value);
                            this.< OnGUI > g__DrawInteractionAfterUI | 89_0(icon7, icon1Color4, icon8, icon2Color4, num7, flag9, failReason, null, num2, flag3, null, false);
                        }
                        if (Get.KeyBinding_Interact.JustPressed || Get.KeyBinding_Accept.JustPressed)
                        {
                            this.TriedToDoDisabledAction(value.failReason);
                        }
                    }
                }
                else
                {
                    Structure structure = this.pointedAtEntity as Structure;
                    if (structure != null && !KeyCodeUtility.InspectHeldDown)
                    {
                        if (structure.Spec.Structure.IsDiggable)
                        {
                            if (Get.NowControlledActor.Inventory.HasAnyItemOfSpec(Get.Entity_Shovel))
                            {
                                this.interactionExtraHint = "UseShovel".Translate();
                            }
                            else
                            {
                                this.interactionExtraHint = "NeedShovel".Translate();
                            }
                        }
                        else if (structure.Spec.Structure.IsMineable)
                        {
                            if (Get.NowControlledActor.Inventory.HasAnyItemOfSpec(Get.Entity_Pickaxe))
                            {
                                this.interactionExtraHint = "UsePickaxe".Translate();
                            }
                            else
                            {
                                this.interactionExtraHint = "NeedPickaxe".Translate();
                            }
                        }
                    }
                }
            }
            if (ControllerUtility.InControllerMode && this.pointedAtLedge == null && this.highlightedEntity == null && !flag && (Get.KeyBinding_Interact.JustPressed || Get.KeyBinding_Accept.JustPressed))
            {
                Entity nearestTargetToMouse = Get.PlayerMovementManager.GetNearestTargetToMouse();
                if (nearestTargetToMouse != null && Vector3.Dot(Get.CameraTransform.forward, (nearestTargetToMouse.RenderPositionComputedCenter - Get.CameraPosition).normalized) >= 0.65f)
                {
                    Get.Sound_Rotation.PlayOneShot(null, 1f, 1f);
                    Get.FPPControllerGOC.RotateToFace(nearestTargetToMouse.RenderPositionComputedCenter);
                }
            }
            if (Get.KeyBinding_AttackNearest.JustPressed)
            {
                this.AttackNearest();
            }
            if (Event.current.type == EventType.ScrollWheel && !Get.KeyBinding_Minimap.HeldDown && !Get.KeyBinding_MinimapAlt.HeldDown)
            {
                this.entityScrollAccumulatedDelta += Event.current.delta.y;
                if (this.entityScrollAccumulatedDelta > 0.95f)
                {
                    this.RotateToFaceNextEntity(true);
                    this.entityScrollAccumulatedDelta = 0f;
                }
                else if (this.entityScrollAccumulatedDelta < -0.95f)
                {
                    this.RotateToFaceNextEntity(false);
                    this.entityScrollAccumulatedDelta = 0f;
                }
            }
            if (Event.current.type == EventType.KeyDown)
            {
                this.entityScrollAccumulatedDelta = 0f;
            }
        }

        public void OnGUIAfterUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (Get.UI.WorldInputBlocked)
            {
                return;
            }
            if (!Get.KeyBinding_Minimap.HeldDown && !Get.KeyBinding_MinimapAlt.HeldDown)
            {
                if (this.hasInteractionToDraw)
                {
                    InteractionDrawer.Draw(this.interactionIcon1, this.interactionIcon1Color, this.interactionIcon2, this.interactionIcon2Color, this.interactionSequence, this.interactionDisabled, this.interactionDisabledReason, this.interactionDealtDamageRange, this.interactionTargetHP, this.interactionHasUsePrompt, this.interactionArrowDirection, this.interactionAnyActionTakesMoreThan1Turn);
                }
                else if (!this.interactionExtraHint.NullOrEmpty())
                {
                    Vector2 vector = new Vector2(Calc.Round(Widgets.VirtualWidth / 2f + 12f + 5f), Calc.Round(Widgets.VirtualHeight / 2f));
                    Widgets.FontSizeScalable = 19;
                    GUI.color = new Color(0.55f, 0.55f, 0.55f, 0.62f);
                    Widgets.LabelCenteredV(vector, this.interactionExtraHint, true, null, null, false);
                    GUI.color = Color.white;
                    Widgets.ResetFontSize();
                }
            }
            if (this.pointedAtEntity != null && KeyCodeUtility.InspectHeldDown)
            {
                Vector2 size = TipSubjectDrawer.GetSize(this.pointedAtEntity, null);
                TipSubjectDrawer.Draw(this.pointedAtEntity, Widgets.ScreenCenter + new Vector2(20f, -size.y / 2f), null);
                Get.Player.OnInspectedSomething();
                if (this.lastPointedAtEntityCalcHadInspectMode)
                {
                    Get.GameObjectHighlighter.Highlight(this.pointedAtEntity, new Color?(new Color(0.6f, 0.6f, 0.6f)));
                    Actor actor = this.pointedAtEntity as Actor;
                    if (actor != null)
                    {
                        this.HighlightBestWeaponRadius(actor);
                    }
                }
                Get.LessonManager.FinishIfCurrent(Get.Lesson_Inspecting);
            }
            if (this.pointedAtEntity != null && InteractionManager.TargetBodyPartKeyBinding.HeldDown && this.pointedAtBodyPart != null && !KeyCodeUtility.InspectHeldDown)
            {
                ConditionDrawRequest? conditionDrawRequest = this.GetConditionActorWouldGetAfterDestroyingBodyPart(this.pointedAtBodyPart).ToDrawRequestOrNull();
                if (conditionDrawRequest != null)
                {
                    Vector2 size2 = TipSubjectDrawer.GetSize(conditionDrawRequest, null);
                    TipSubjectDrawer.Draw(conditionDrawRequest, Widgets.ScreenCenter + new Vector2(185f, -size2.y / 2f), null);
                }
            }
        }

        private void TriedToDoDisabledAction(string failReason)
        {
            this.lastTimeTriedDisabledAction = Clock.UnscaledTime;
            this.lastTriedDisabledActionReason = failReason;
        }

        private Condition GetConditionActorWouldGetAfterDestroyingBodyPart(BodyPart bodyPart)
        {
            if (bodyPart == null)
            {
                return null;
            }
            if (bodyPart.Spec.ConditionOnAllDestroyed != null)
            {
                bool flag = false;
                foreach (BodyPart bodyPart2 in bodyPart.Actor.BodyParts)
                {
                    if (bodyPart2.Spec == bodyPart.Spec && bodyPart2 != bodyPart && !bodyPart2.IsMissing)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return bodyPart.Spec.ConditionOnAllDestroyed;
                }
            }
            return bodyPart.Spec.ConditionOnDestroyed;
        }

        private void DoInterruptedLabel()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            float num = Calc.ResolveFadeInStayOut(Clock.Time - this.plannedPlayerActions.LastInterruptTime, 0.12f, 0.2f, 0.83f) * 1f;
            if (num > 0f)
            {
                GUI.color = new Color(1f, 0.15f, 0.15f, num);
                Widgets.FontSizeScalable = 24;
                Widgets.LabelCentered(Widgets.ScreenCenter + new Vector2(0f, 100f), "Interrupted".Translate(), true, null, null, false, false, false, null);
                Widgets.ResetFontSize();
                GUI.color = Color.white;
            }
        }

        private void AttackNearest()
        {
            InteractionManager.<> c__DisplayClass94_0 CS$<> 8__locals1 = new InteractionManager.<> c__DisplayClass94_0();
            CS$<> 8__locals1.<> 4__this = this;
            InteractionManager.<> c__DisplayClass94_0 CS$<> 8__locals2 = CS$<> 8__locals1;
            IUsable equippedWeapon = Get.NowControlledActor.Inventory.EquippedWeapon;
            CS$<> 8__locals2.weaponToUse = equippedWeapon ?? Get.NowControlledActor.NativeWeapons.ElementAtOrDefault<NativeWeapon>(0);
            if (CS$<> 8__locals1.weaponToUse != null && CS$<> 8__locals1.weaponToUse.UseEffects.Any)
			{
                IEnumerable<Entity> entitiesSeen_Unordered = Get.VisibilityCache.EntitiesSeen_Unordered;
                Func<Entity, bool> func;
                if ((func = CS$<> 8__locals1.<> 9__0) == null)
				{
                    func = (CS$<> 8__locals1.<> 9__0 = delegate (Entity x)
                    {
                        if (x.Spec.AutoAttackable)
                        {
                            Actor actor = x as Actor;
                            if ((actor == null || actor.IsHostile(Get.NowControlledActor)) && CS$<> 8__locals1.weaponToUse.UseFilter.Allows(x, Get.NowControlledActor))
							{
                                return x != Get.NowControlledActor;
                            }
                        }
                        return false;
                    });
                }
                IEnumerable<Entity> enumerable = entitiesSeen_Unordered.Where<Entity>(func);
                Func<Entity, bool> func2;
                if ((func2 = CS$<> 8__locals1.<> 9__1) == null)
				{
                    func2 = (CS$<> 8__locals1.<> 9__1 = (Entity x) => x != CS$<> 8__locals1.<> 4__this.pointedAtEntity);
                }
                using (List<Entity>.Enumerator enumerator = enumerable.OrderBy<Entity, bool>(func2).ThenBy<Entity, float>((Entity x) => Calc.SphericalDistance(Get.CameraTransform.forward, (x.RenderPositionComputedCenter - Get.CameraPosition).normalized)).ToTemporaryList<Entity>()
                    .GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Entity entity = enumerator.Current;
                        if (Get.NowControlledActor.CanUseOn(CS$<> 8__locals1.weaponToUse, entity, null, false, null) && ActionViaInterfaceHelper.TryDo(() => new Action_Use(Get.Action_Use, Get.NowControlledActor, CS$<> 8__locals1.weaponToUse, entity, null)))
                        {
                            if (entity.Spec == Get.Entity_Crates)
                            {
                                Get.LessonManager.FinishIfCurrent(Get.Lesson_QuickAttack);
                                break;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private EntityGOC GetCachedEntityGOCInParent(GameObject gameObject)
        {
            if (gameObject == this.cachedEntityGOCFor)
            {
                return this.cachedEntityGOC;
            }
            this.cachedEntityGOCFor = gameObject;
            this.cachedEntityGOC = gameObject.GetComponentInParent<EntityGOC>();
            return this.cachedEntityGOC;
        }

        private void RotateToFaceNextEntity(bool next)
        {
            this.currentScrollTarget = Get.SeenEntitiesDrawer.GetNextEntity(this.currentScrollTarget, next);
            if (this.currentScrollTarget == null)
            {
                return;
            }
            Get.SeenEntitiesDrawer.OnScrolledToEntity(this.currentScrollTarget);
            Get.FPPControllerGOC.RotateToFace(this.currentScrollTarget.RenderPositionComputedCenter);
            Get.Sound_Rotation.PlayOneShot(null, 1f, 1f);
        }

        private void HighlightBestWeaponRadius(Actor actor)
        {
            IUsable weaponToUse = AIUtility_Attack.GetWeaponToUse(actor, true);
            if (weaponToUse == null)
            {
                return;
            }
            foreach (Vector3Int vector3Int in actor.Position.GetCellsWithin(weaponToUse.UseRange).ClipToWorld())
            {
                if (vector3Int != actor.Position && Get.CellsInfo.IsFloorUnderNoActors(vector3Int) && !Get.CellsInfo.AnyFilledCantSeeThroughAt(vector3Int) && !Get.FogOfWar.IsFogged(vector3Int) && vector3Int.Below().InBounds() && Get.CellsInfo.AnySupportsTargetingLocations(vector3Int.Below()) && actor.Sees(vector3Int, null) && LineOfSight.IsLineOfFire(actor.Position, vector3Int))
                {
                    Get.CellHighlighter.HighlightCell(vector3Int, CellHighlighter.DeepRed);
                }
            }
        }

        public List<Action> MakeMoveToActions(List<Vector3Int> path, bool oneStep)
        {
            List<Action> list = new List<Action>();
            for (int i = path.Count - 2; i >= 0; i--)
            {
                list.Add(new Action_MoveSelf(Get.Action_MoveSelf, Get.NowControlledActor, path[i + 1], path[i], null, false));
                if (oneStep)
                {
                    break;
                }
            }
            return list;
        }

        public InteractionManager.PossibleInteraction? GetInteraction(Entity entity, Vector3Int raycastHitDirection, BodyPart bodyPart)
        {
            if (entity.IsNowControlledActor)
            {
                InteractionManager.PossibleInteraction? possibleInteraction = null;
                return possibleInteraction;
            }
            this.tmpFailReason.Clear();
            bool flag = !(entity is Actor) || Get.KeyBinding_OneStep.HeldDown;
            Vector3Int? vector3Int;
            bool flag2;
            if (entity is Structure && (entity.Spec.Structure.TargetingLocationToSelf || entity.Spec.Structure.IsStairs))
            {
                vector3Int = new Vector3Int?(entity.Position);
                flag2 = true;
            }
            else if (!entity.Spec.CanPassThrough && entity is Structure && entity.Spec.Structure.SupportsTargetingLocation && (entity.Position + raycastHitDirection).InBounds() && !Get.CellsInfo.IsFilled(entity.Position + raycastHitDirection))
            {
                vector3Int = new Vector3Int?(entity.Position + raycastHitDirection);
                flag2 = raycastHitDirection == -Get.NowControlledActor.Gravity;
            }
            else
            {
                vector3Int = null;
                flag2 = false;
            }
            IUsable targetingUsable = Get.UseOnTargetUI.TargetingUsable;
            if (targetingUsable != null && targetingUsable.UseEffects.Any)
            {
                bool flag3 = flag && targetingUsable.UseRange <= 1;
                Spell spell = targetingUsable as Spell;
                if (spell != null && spell.Spec == Get.Spell_JumpAbility)
                {
                    flag3 = false;
                }
                if (targetingUsable.UseFilter.Allows(entity, Get.NowControlledActor))
                {
                    Actor nowControlledActor = Get.NowControlledActor;
                    IUsable usable = targetingUsable;
                    Target target = entity;
                    bool flag4 = flag3;
                    StringSlot stringSlot = this.tmpFailReason;
                    IUsable usable4;
                    IntRange? intRange;
                    if (nowControlledActor.CanUseOn(usable, target, null, flag4, stringSlot))
                    {
                        PathFinder.Request.Mode mode = (flag3 ? this.ResolvePathMode(targetingUsable.UseRange) : PathFinder.Request.Mode.None);
                        Func<Action> func = this.GetCachedUseActionDelegate(targetingUsable, entity, bodyPart);
                        int num = Action_Use.SequencePerUse(Get.NowControlledActor, targetingUsable);
                        Texture2D icon = targetingUsable.Icon;
                        Color iconColor = targetingUsable.IconColor;
                        Texture2D texture2D = null;
                        Color white = Color.white;
                        flag4 = this.CanTargetBodyPart(targetingUsable);
                        IUsable usable2 = targetingUsable;
                        IUsable usable3 = targetingUsable;
                        Structure structure = entity as Structure;
                        usable4 = ((structure != null && structure.Spec.Structure.AutoUseOnDestroyed) ? structure : null);
                        intRange = UsableUtility.GetDealtDamageRangeForUI(Get.NowControlledActor, targetingUsable, entity, bodyPart);
                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode, func, num, icon, iconColor, texture2D, white, null, true, flag4, null, null, usable2, usable3, usable4, null, intRange));
                    }
                    PathFinder.Request.Mode mode2 = PathFinder.Request.Mode.None;
                    Func<Action> func2 = null;
                    int num2 = 0;
                    Texture2D icon2 = targetingUsable.Icon;
                    Color iconColor2 = targetingUsable.IconColor;
                    Texture2D texture2D2 = null;
                    Color white2 = Color.white;
                    flag4 = this.CanTargetBodyPart(targetingUsable);
                    string text = this.tmpFailReason.String;
                    usable4 = targetingUsable;
                    intRange = UsableUtility.GetDealtDamageRangeForUI(Get.NowControlledActor, targetingUsable, entity, bodyPart);
                    return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode2, func2, num2, icon2, iconColor2, texture2D2, white2, null, true, flag4, null, null, usable4, null, null, text, intRange));
                }
                else if (vector3Int != null && targetingUsable.UseFilter.Allows(vector3Int.Value, Get.NowControlledActor))
                {
                    Actor nowControlledActor2 = Get.NowControlledActor;
                    IUsable usable5 = targetingUsable;
                    Target target2 = vector3Int.Value;
                    bool flag4 = flag3;
                    StringSlot stringSlot = this.tmpFailReason;
                    IUsable usable4;
                    if (nowControlledActor2.CanUseOn(usable5, target2, null, flag4, stringSlot))
                    {
                        PathFinder.Request.Mode mode3 = (flag3 ? this.ResolvePathMode(targetingUsable.UseRange) : PathFinder.Request.Mode.None);
                        Func<Action> func3 = this.GetCachedUseActionDelegate(targetingUsable, vector3Int.Value, null);
                        int num3 = Action_Use.SequencePerUse(Get.NowControlledActor, targetingUsable);
                        Texture2D icon3 = targetingUsable.Icon;
                        Color iconColor3 = targetingUsable.IconColor;
                        Texture2D texture2D3 = null;
                        Color white3 = Color.white;
                        Vector3Int? vector3Int2 = vector3Int;
                        bool flag5 = false;
                        bool flag6 = false;
                        usable4 = targetingUsable;
                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode3, func3, num3, icon3, iconColor3, texture2D3, white3, vector3Int2, flag5, flag6, targetingUsable.Icon, new Color?(targetingUsable.IconColor), usable4, targetingUsable, null, null, null));
                    }
                    PathFinder.Request.Mode mode4 = PathFinder.Request.Mode.None;
                    Func<Action> func4 = null;
                    int num4 = 0;
                    Texture2D icon4 = targetingUsable.Icon;
                    Color iconColor4 = targetingUsable.IconColor;
                    Texture2D texture2D4 = null;
                    Color white4 = Color.white;
                    Vector3Int? vector3Int3 = vector3Int;
                    bool flag7 = false;
                    bool flag8 = false;
                    Texture2D texture2D5 = null;
                    string text = this.tmpFailReason.String;
                    usable4 = targetingUsable;
                    return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode4, func4, num4, icon4, iconColor4, texture2D4, white4, vector3Int3, flag7, flag8, texture2D5, null, usable4, null, null, text, null));
                }
            }
            Actor actor = entity as Actor;
            if (actor != null && !InteractionManager.TargetBodyPartKeyBinding.HeldDown && actor.UsableOnTalk != null && actor.UsableOnTalk.UseEffects.Any && actor.UsableOnTalk.UseFilter.Allows(Get.NowControlledActor, Get.NowControlledActor) && (!actor.UsableOnTalk.Spec.DisallowIfHostile || !actor.IsHostile(Get.NowControlledActor)))
            {
                Actor nowControlledActor3 = Get.NowControlledActor;
                IUsable usableOnTalk = actor.UsableOnTalk;
                Target target3 = Get.NowControlledActor;
                StringSlot stringSlot = this.tmpFailReason;
                IUsable usable3;
                if (nowControlledActor3.CanUseOn(usableOnTalk, target3, null, true, stringSlot))
                {
                    PathFinder.Request.Mode mode5 = this.ResolvePathMode(actor.UsableOnTalk.UseRange);
                    Func<Action> func5 = this.GetCachedUseActionDelegate(actor.UsableOnTalk, Get.NowControlledActor, null);
                    int num5 = Action_Use.SequencePerUse(Get.NowControlledActor, actor.UsableOnTalk);
                    Texture2D talkIcon = InteractionManager.TalkIcon1;
                    Color white5 = Color.white;
                    Texture2D talkIcon2 = InteractionManager.TalkIcon2;
                    Color white6 = Color.white;
                    IUsable usable4 = actor.UsableOnTalk;
                    usable3 = actor.UsableOnTalk;
                    return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode5, func5, num5, talkIcon, white5, talkIcon2, white6, null, true, false, null, null, usable4, usable3, null, null, null));
                }
                PathFinder.Request.Mode mode6 = PathFinder.Request.Mode.None;
                Func<Action> func6 = null;
                int num6 = 0;
                Texture2D talkIcon3 = InteractionManager.TalkIcon1;
                Color white7 = Color.white;
                Texture2D talkIcon4 = InteractionManager.TalkIcon2;
                Color white8 = Color.white;
                string text = this.tmpFailReason.String;
                usable3 = actor.UsableOnTalk;
                return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode6, func6, num6, talkIcon3, white7, talkIcon4, white8, null, true, false, null, null, usable3, null, null, text, null));
            }
            else
            {
                if (!Get.InLobby && InteractionManager.TargetBodyPartKeyBinding.HeldDown)
                {
                    Structure structure2 = entity as Structure;
                    if (structure2 != null && structure2.Spec == Get.Entity_Door && Get.NowControlledActor.KickUsable != null && Get.NowControlledActor.KickUsable.UseEffects.Any && Get.NowControlledActor.KickUsable.UseFilter.Allows(structure2, Get.NowControlledActor))
                    {
                        Actor nowControlledActor4 = Get.NowControlledActor;
                        IUsable kickUsable = Get.NowControlledActor.KickUsable;
                        Target target4 = structure2;
                        StringSlot stringSlot = this.tmpFailReason;
                        IUsable usable4;
                        if (nowControlledActor4.CanUseOn(kickUsable, target4, null, true, stringSlot))
                        {
                            PathFinder.Request.Mode mode7 = this.ResolvePathMode(Get.NowControlledActor.KickUsable.UseRange);
                            Func<Action> func7 = this.GetCachedUseActionDelegate(Get.NowControlledActor.KickUsable, structure2, null);
                            int num7 = Action_Use.SequencePerUse(Get.NowControlledActor, Get.NowControlledActor.KickUsable);
                            Texture2D kickIcon = InteractionManager.KickIcon1;
                            Color white9 = Color.white;
                            Texture2D kickIcon2 = InteractionManager.KickIcon2;
                            Color white10 = Color.white;
                            IUsable usable3 = Get.NowControlledActor.KickUsable;
                            usable4 = Get.NowControlledActor.KickUsable;
                            return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode7, func7, num7, kickIcon, white9, kickIcon2, white10, null, true, false, null, null, usable3, usable4, null, null, null));
                        }
                        PathFinder.Request.Mode mode8 = PathFinder.Request.Mode.None;
                        Func<Action> func8 = null;
                        int num8 = 0;
                        Texture2D kickIcon3 = InteractionManager.KickIcon1;
                        Color white11 = Color.white;
                        Texture2D kickIcon4 = InteractionManager.KickIcon2;
                        Color white12 = Color.white;
                        string text = this.tmpFailReason.String;
                        usable4 = Get.NowControlledActor.KickUsable;
                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode8, func8, num8, kickIcon3, white11, kickIcon4, white12, null, true, false, null, null, usable4, null, null, text, null));
                    }
                }
                Item equippedWeapon = Get.NowControlledActor.Inventory.EquippedWeapon;
                if (equippedWeapon != null && equippedWeapon.UseEffects.Any && Get.UseOnTargetUI.TargetingUsable == null)
                {
                    bool flag9 = flag && equippedWeapon.UseRange <= 1;
                    if (equippedWeapon.UseFilter.Allows(entity, Get.NowControlledActor))
                    {
                        Actor nowControlledActor5 = Get.NowControlledActor;
                        IUsable usable6 = equippedWeapon;
                        Target target5 = entity;
                        bool flag4 = flag9;
                        StringSlot stringSlot = this.tmpFailReason;
                        IUsable usable2;
                        IntRange? intRange;
                        if (nowControlledActor5.CanUseOn(usable6, target5, null, flag4, stringSlot))
                        {
                            PathFinder.Request.Mode mode9 = (flag9 ? this.ResolvePathMode(equippedWeapon.UseRange) : PathFinder.Request.Mode.None);
                            Func<Action> func9 = this.GetCachedUseActionDelegate(equippedWeapon, entity, bodyPart);
                            int num9 = Action_Use.SequencePerUse(Get.NowControlledActor, equippedWeapon);
                            Texture2D icon5 = equippedWeapon.Icon;
                            Color iconColor5 = equippedWeapon.IconColor;
                            Texture2D texture2D6 = null;
                            Color white13 = Color.white;
                            flag4 = this.CanTargetBodyPart(equippedWeapon);
                            IUsable usable4 = equippedWeapon;
                            IUsable usable3 = equippedWeapon;
                            Structure structure3 = entity as Structure;
                            usable2 = ((structure3 != null && structure3.Spec.Structure.AutoUseOnDestroyed) ? structure3 : null);
                            intRange = UsableUtility.GetDealtDamageRangeForUI(Get.NowControlledActor, equippedWeapon, entity, bodyPart);
                            return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode9, func9, num9, icon5, iconColor5, texture2D6, white13, null, true, flag4, null, null, usable4, usable3, usable2, null, intRange));
                        }
                        PathFinder.Request.Mode mode10 = PathFinder.Request.Mode.None;
                        Func<Action> func10 = null;
                        int num10 = 0;
                        Texture2D icon6 = equippedWeapon.Icon;
                        Color iconColor6 = equippedWeapon.IconColor;
                        Texture2D texture2D7 = null;
                        Color white14 = Color.white;
                        flag4 = this.CanTargetBodyPart(equippedWeapon);
                        string text = this.tmpFailReason.String;
                        usable2 = equippedWeapon;
                        intRange = UsableUtility.GetDealtDamageRangeForUI(Get.NowControlledActor, equippedWeapon, entity, bodyPart);
                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode10, func10, num10, icon6, iconColor6, texture2D7, white14, null, true, flag4, null, null, usable2, null, null, text, intRange));
                    }
                    else if (vector3Int != null && equippedWeapon.UseFilter.Allows(vector3Int.Value, Get.NowControlledActor))
                    {
                        Actor nowControlledActor6 = Get.NowControlledActor;
                        IUsable usable7 = equippedWeapon;
                        Target target6 = vector3Int.Value;
                        bool flag4 = flag9;
                        StringSlot stringSlot = this.tmpFailReason;
                        if (nowControlledActor6.CanUseOn(usable7, target6, null, flag4, stringSlot))
                        {
                            return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(flag9 ? this.ResolvePathMode(equippedWeapon.UseRange) : PathFinder.Request.Mode.None, this.GetCachedUseActionDelegate(equippedWeapon, vector3Int.Value, null), Action_Use.SequencePerUse(Get.NowControlledActor, equippedWeapon), equippedWeapon.Icon, equippedWeapon.IconColor, null, Color.white, vector3Int, false, false, equippedWeapon.Icon, new Color?(equippedWeapon.IconColor), equippedWeapon, equippedWeapon, null, null, null));
                        }
                        PathFinder.Request.Mode mode11 = PathFinder.Request.Mode.None;
                        Func<Action> func11 = null;
                        int num11 = 0;
                        Texture2D icon7 = equippedWeapon.Icon;
                        Color iconColor7 = equippedWeapon.IconColor;
                        Texture2D texture2D8 = null;
                        Color white15 = Color.white;
                        Vector3Int? vector3Int4 = vector3Int;
                        bool flag10 = false;
                        bool flag11 = false;
                        Texture2D texture2D9 = null;
                        string text = this.tmpFailReason.String;
                        IUsable usable2 = equippedWeapon;
                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode11, func11, num11, icon7, iconColor7, texture2D8, white15, vector3Int4, flag10, flag11, texture2D9, null, usable2, null, null, text, null));
                    }
                }
                Structure structure4 = entity as Structure;
                if (structure4 != null && structure4.Spec.Structure.AllowUseViaInterface && structure4.UseEffects.Any && structure4.UseFilter.Allows(Get.NowControlledActor, Get.NowControlledActor))
                {
                    Actor nowControlledActor7 = Get.NowControlledActor;
                    IUsable usable8 = structure4;
                    Target target7 = Get.NowControlledActor;
                    StringSlot stringSlot = this.tmpFailReason;
                    IUsable usable3;
                    if (nowControlledActor7.CanUseOn(usable8, target7, null, true, stringSlot))
                    {
                        PathFinder.Request.Mode mode12 = this.ResolvePathMode(structure4.UseRange);
                        Func<Action> func12 = this.GetCachedUseActionDelegate(structure4, Get.NowControlledActor, null);
                        int num12 = (structure4.Spec.Structure.InteractingIsFreeUIHint ? 0 : Action_Use.SequencePerUse(Get.NowControlledActor, structure4));
                        Texture2D useStructureIcon = InteractionManager.UseStructureIcon1;
                        Color white16 = Color.white;
                        Texture2D useStructureIcon2 = InteractionManager.UseStructureIcon2;
                        Color white17 = Color.white;
                        IUsable usable2 = structure4;
                        usable3 = structure4;
                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode12, func12, num12, useStructureIcon, white16, useStructureIcon2, white17, null, true, false, null, null, usable2, usable3, null, null, null));
                    }
                    PathFinder.Request.Mode mode13 = PathFinder.Request.Mode.None;
                    Func<Action> func13 = null;
                    int num13 = 0;
                    Texture2D useStructureIcon3 = InteractionManager.UseStructureIcon1;
                    Color white18 = Color.white;
                    Texture2D useStructureIcon4 = InteractionManager.UseStructureIcon2;
                    Color white19 = Color.white;
                    string text = this.tmpFailReason.String;
                    usable3 = structure4;
                    return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode13, func13, num13, useStructureIcon3, white18, useStructureIcon4, white19, null, true, false, null, null, usable3, null, null, text, null));
                }
                else
                {
                    Item item = entity as Item;
                    if (item != null)
                    {
                        if (InstructionSets_Actor.HasSpaceToPickUp(Get.NowControlledActor, item))
                        {
                            return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(PathFinder.Request.Mode.ToCell, null, 0, item.ForSale ? InteractionManager.BuyIcon1 : InteractionManager.PickUpIcon1, Color.white, item.ForSale ? InteractionManager.BuyIcon2 : InteractionManager.PickUpIcon2, Color.white, null, true, false, null, null, null, null, null, null, null));
                        }
                        PathFinder.Request.Mode mode14 = PathFinder.Request.Mode.None;
                        Func<Action> func14 = null;
                        int num14 = 0;
                        Texture2D texture2D10 = (item.ForSale ? InteractionManager.BuyIcon1 : InteractionManager.PickUpIcon1);
                        Color white20 = Color.white;
                        Texture2D texture2D11 = (item.ForSale ? InteractionManager.BuyIcon2 : InteractionManager.PickUpIcon2);
                        Color white21 = Color.white;
                        string text = "NotEnoughSpaceToPickUp".Translate();
                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode14, func14, num14, texture2D10, white20, texture2D11, white21, null, true, false, null, null, null, null, null, text, null));
                    }
                    else if (equippedWeapon == null && Get.UseOnTargetUI.TargetingUsable == null && Get.NowControlledActor.NativeWeapons.Count != 0 && Get.NowControlledActor.NativeWeapons[0].UseEffects.Any && Get.NowControlledActor.Spec.Actor.NativeWeapons[0].UseFilter.Allows(entity, Get.NowControlledActor))
                    {
                        NativeWeapon nativeWeapon = Get.NowControlledActor.NativeWeapons[0];
                        bool flag12 = flag && nativeWeapon.UseRange <= 1;
                        Actor nowControlledActor8 = Get.NowControlledActor;
                        IUsable usable9 = nativeWeapon;
                        Target target8 = entity;
                        bool flag4 = flag12;
                        StringSlot stringSlot = this.tmpFailReason;
                        IUsable usable4;
                        IntRange? intRange;
                        if (nowControlledActor8.CanUseOn(usable9, target8, null, flag4, stringSlot))
                        {
                            PathFinder.Request.Mode mode15 = (flag12 ? this.ResolvePathMode(nativeWeapon.UseRange) : PathFinder.Request.Mode.None);
                            Func<Action> func15 = this.GetCachedUseActionDelegate(nativeWeapon, entity, bodyPart);
                            int num15 = Action_Use.SequencePerUse(Get.NowControlledActor, nativeWeapon);
                            Texture2D icon8 = nativeWeapon.Icon;
                            Color white22 = Color.white;
                            Texture2D texture2D12 = null;
                            Color white23 = Color.white;
                            flag4 = this.CanTargetBodyPart(nativeWeapon);
                            IUsable usable3 = nativeWeapon;
                            IUsable usable2 = nativeWeapon;
                            Structure structure5 = entity as Structure;
                            usable4 = ((structure5 != null && structure5.Spec.Structure.AutoUseOnDestroyed) ? structure5 : null);
                            intRange = UsableUtility.GetDealtDamageRangeForUI(Get.NowControlledActor, nativeWeapon, entity, bodyPart);
                            return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode15, func15, num15, icon8, white22, texture2D12, white23, null, true, flag4, null, null, usable3, usable2, usable4, null, intRange));
                        }
                        PathFinder.Request.Mode mode16 = PathFinder.Request.Mode.None;
                        Func<Action> func16 = null;
                        int num16 = 0;
                        Texture2D icon9 = nativeWeapon.Icon;
                        Color white24 = Color.white;
                        Texture2D texture2D13 = null;
                        Color white25 = Color.white;
                        flag4 = this.CanTargetBodyPart(nativeWeapon);
                        string text = this.tmpFailReason.String;
                        usable4 = nativeWeapon;
                        intRange = UsableUtility.GetDealtDamageRangeForUI(Get.NowControlledActor, nativeWeapon, entity, bodyPart);
                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode16, func16, num16, icon9, white24, texture2D13, white25, null, true, flag4, null, null, usable4, null, null, text, intRange));
                    }
                    else if (entity is Structure && entity.Spec.Structure.IsLadder)
                    {
                        Vector3Int? vector3Int5 = null;
                        Vector3Int vector3Int6 = entity.Position;
                        Vector3Int vector3Int7;
                        bool flag13;
                        if (entity.Position.y >= Get.NowControlledActor.Position.y)
                        {
                            vector3Int7 = -Get.NowControlledActor.Gravity;
                            flag13 = true;
                        }
                        else
                        {
                            vector3Int7 = Get.NowControlledActor.Gravity;
                            flag13 = false;
                        }
                        while (vector3Int6.InBounds() && (Get.CellsInfo.CanPassThrough(vector3Int6) || !(vector3Int6 != Get.NowControlledActor.Position)) && (Get.CellsInfo.AnyLadderAt(vector3Int6) || Get.CellsInfo.IsLadderUnder(vector3Int6, Get.NowControlledActor.Gravity)))
                        {
                            vector3Int5 = new Vector3Int?(vector3Int6);
                            vector3Int6 += vector3Int7;
                        }
                        if (vector3Int5 != null && vector3Int5.Value != Get.NowControlledActor.Position)
                        {
                            return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(PathFinder.Request.Mode.ToCell, null, 0, flag13 ? InteractionManager.UseLadderUpIcon1 : InteractionManager.UseLadderDownIcon1, Color.white, flag13 ? InteractionManager.UseLadderUpIcon2 : InteractionManager.UseLadderDownIcon2, Color.white, new Vector3Int?(vector3Int5.Value), true, false, null, null, null, null, null, null, null));
                        }
                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(PathFinder.Request.Mode.None, null, 0, flag13 ? InteractionManager.UseLadderUpIcon1 : InteractionManager.UseLadderDownIcon1, Color.white, flag13 ? InteractionManager.UseLadderUpIcon2 : InteractionManager.UseLadderDownIcon2, Color.white, null, true, false, null, null, null, null, null, null, null));
                    }
                    else
                    {
                        if (!(entity is Structure) || !entity.Spec.Structure.IsStairs || entity.Spec.Structure.TargetingLocationToSelf)
                        {
                            foreach (GenericUsable genericUsable in Get.NowControlledActor.Abilities)
                            {
                                if (genericUsable.UseEffects.Any)
                                {
                                    bool flag14 = flag && genericUsable.UseRange <= 1;
                                    if (genericUsable.UseFilter.Allows(entity, Get.NowControlledActor))
                                    {
                                        Actor nowControlledActor9 = Get.NowControlledActor;
                                        IUsable usable10 = genericUsable;
                                        Target target9 = entity;
                                        bool flag4 = flag14;
                                        StringSlot stringSlot = this.tmpFailReason;
                                        IUsable usable3;
                                        IntRange? intRange;
                                        if (nowControlledActor9.CanUseOn(usable10, target9, null, flag4, stringSlot))
                                        {
                                            PathFinder.Request.Mode mode17 = (flag14 ? this.ResolvePathMode(genericUsable.UseRange) : PathFinder.Request.Mode.None);
                                            Func<Action> func17 = this.GetCachedUseActionDelegate(genericUsable, entity, bodyPart);
                                            int num17 = Action_Use.SequencePerUse(Get.NowControlledActor, genericUsable);
                                            Texture2D useStructureIcon5 = InteractionManager.UseStructureIcon1;
                                            Color white26 = Color.white;
                                            Texture2D useStructureIcon6 = InteractionManager.UseStructureIcon2;
                                            Color white27 = Color.white;
                                            flag4 = this.CanTargetBodyPart(genericUsable);
                                            IUsable usable4 = genericUsable;
                                            IUsable usable2 = genericUsable;
                                            Structure structure6 = entity as Structure;
                                            usable3 = ((structure6 != null && structure6.Spec.Structure.AutoUseOnDestroyed) ? structure6 : null);
                                            intRange = UsableUtility.GetDealtDamageRangeForUI(Get.NowControlledActor, genericUsable, entity, bodyPart);
                                            return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode17, func17, num17, useStructureIcon5, white26, useStructureIcon6, white27, null, true, flag4, null, null, usable4, usable2, usable3, null, intRange));
                                        }
                                        PathFinder.Request.Mode mode18 = PathFinder.Request.Mode.None;
                                        Func<Action> func18 = null;
                                        int num18 = 0;
                                        Texture2D useStructureIcon7 = InteractionManager.UseStructureIcon1;
                                        Color white28 = Color.white;
                                        Texture2D useStructureIcon8 = InteractionManager.UseStructureIcon2;
                                        Color white29 = Color.white;
                                        flag4 = this.CanTargetBodyPart(genericUsable);
                                        string text = this.tmpFailReason.String;
                                        usable3 = genericUsable;
                                        intRange = UsableUtility.GetDealtDamageRangeForUI(Get.NowControlledActor, genericUsable, entity, bodyPart);
                                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode18, func18, num18, useStructureIcon7, white28, useStructureIcon8, white29, null, true, flag4, null, null, usable3, null, null, text, intRange));
                                    }
                                    else if (vector3Int != null && genericUsable.UseFilter.Allows(vector3Int.Value, Get.NowControlledActor))
                                    {
                                        Actor nowControlledActor10 = Get.NowControlledActor;
                                        IUsable usable11 = genericUsable;
                                        Target target10 = vector3Int.Value;
                                        bool flag4 = flag14;
                                        StringSlot stringSlot = this.tmpFailReason;
                                        IUsable usable3;
                                        if (nowControlledActor10.CanUseOn(usable11, target10, null, flag4, stringSlot))
                                        {
                                            PathFinder.Request.Mode mode19 = (flag14 ? this.ResolvePathMode(genericUsable.UseRange) : PathFinder.Request.Mode.None);
                                            Func<Action> func19 = this.GetCachedUseActionDelegate(genericUsable, vector3Int.Value, null);
                                            int num19 = Action_Use.SequencePerUse(Get.NowControlledActor, genericUsable);
                                            Texture2D useStructureIcon9 = InteractionManager.UseStructureIcon1;
                                            Color white30 = Color.white;
                                            Texture2D useStructureIcon10 = InteractionManager.UseStructureIcon2;
                                            Color white31 = Color.white;
                                            Vector3Int? vector3Int8 = vector3Int;
                                            bool flag15 = false;
                                            bool flag16 = false;
                                            usable3 = genericUsable;
                                            return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode19, func19, num19, useStructureIcon9, white30, useStructureIcon10, white31, vector3Int8, flag15, flag16, InteractionManager.UseStructureIcon1, new Color?(Color.white), usable3, genericUsable, null, null, null));
                                        }
                                        PathFinder.Request.Mode mode20 = PathFinder.Request.Mode.None;
                                        Func<Action> func20 = null;
                                        int num20 = 0;
                                        Texture2D useStructureIcon11 = InteractionManager.UseStructureIcon1;
                                        Color white32 = Color.white;
                                        Texture2D useStructureIcon12 = InteractionManager.UseStructureIcon2;
                                        Color white33 = Color.white;
                                        Vector3Int? vector3Int9 = vector3Int;
                                        bool flag17 = false;
                                        bool flag18 = false;
                                        Texture2D texture2D14 = null;
                                        string text = this.tmpFailReason.String;
                                        usable3 = genericUsable;
                                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(mode20, func20, num20, useStructureIcon11, white32, useStructureIcon12, white33, vector3Int9, flag17, flag18, texture2D14, null, usable3, null, null, text, null));
                                    }
                                }
                            }
                            bool flag19;
                            if (vector3Int != null)
                            {
                                Vector3Int? vector3Int10 = vector3Int;
                                Vector3Int position = Get.NowControlledActor.Position;
                                flag19 = vector3Int10 == null || (vector3Int10 != null && vector3Int10.GetValueOrDefault() != position);
                            }
                            else
                            {
                                flag19 = false;
                            }
                            if (flag19 && flag2)
                            {
                                return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(PathFinder.Request.Mode.ToCell, null, 0, InteractionManager.MoveIcon1, Color.white, InteractionManager.MoveIcon2, Color.white, vector3Int, false, false, null, null, null, null, null, null, null));
                            }
                            return null;
                        }
                        Vector3Int vector3Int11 = entity.Position;
                        bool flag20;
                        if (Get.NowControlledActor.Position.y <= entity.Position.y)
                        {
                            flag20 = true;
                            Vector3Int vector3Int12 = entity.Position.Above() + entity.DirectionCardinal;
                            if (vector3Int12.InBounds() && Get.CellsInfo.CanPassThrough(vector3Int12) && !Get.CellsInfo.IsFallingAt(vector3Int12, Get.NowControlledActor, false))
                            {
                                vector3Int11 = vector3Int12;
                            }
                        }
                        else
                        {
                            flag20 = false;
                        }
                        if (vector3Int11 != Get.NowControlledActor.Position)
                        {
                            return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(PathFinder.Request.Mode.ToCell, null, 0, flag20 ? InteractionManager.UseStairsUpIcon1 : InteractionManager.UseStairsDownIcon1, Color.white, flag20 ? InteractionManager.UseStairsUpIcon2 : InteractionManager.UseStairsDownIcon2, Color.white, new Vector3Int?(vector3Int11), true, false, null, null, null, null, null, null, null));
                        }
                        return new InteractionManager.PossibleInteraction?(new InteractionManager.PossibleInteraction(PathFinder.Request.Mode.None, null, 0, flag20 ? InteractionManager.UseStairsUpIcon1 : InteractionManager.UseStairsDownIcon1, Color.white, flag20 ? InteractionManager.UseStairsUpIcon2 : InteractionManager.UseStairsDownIcon2, Color.white, null, true, false, null, null, null, null, null, null, null));
                    }
                }
            }
        }

        private PathFinder.Request.Mode ResolvePathMode(int maxDist)
        {
            if (maxDist != 0)
            {
                return PathFinder.Request.Mode.Touch;
            }
            return PathFinder.Request.Mode.ToCell;
        }

        private Func<Action> GetCachedUseActionDelegate(IUsable usable, Target on, BodyPart onBodyPart = null)
        {
            if (usable == this.cachedUseActionDelegateForUsable && on == this.cachedUseActionDelegateForTarget && onBodyPart == this.cachedUseActionDelegateForTargetBodyPart)
            {
                return this.cachedUseActionDelegate;
            }
            this.cachedUseActionDelegate = this.CreateUseActionDelegate(usable, on, onBodyPart);
            this.cachedUseActionDelegateForUsable = usable;
            this.cachedUseActionDelegateForTarget = on;
            this.cachedUseActionDelegateForTargetBodyPart = onBodyPart;
            return this.cachedUseActionDelegate;
        }

        private Func<Action> CreateUseActionDelegate(IUsable usable, Target on, BodyPart onBodyPart)
        {
            return () => new Action_Use(Get.Action_Use, Get.NowControlledActor, usable, on, onBodyPart);
        }

        private void DrawAoEEffects(IUsable usable, Vector3Int pos, Vector3Int? cellAlreadyHighlighted)
        {
            InteractionManager.<> c__DisplayClass111_0 CS$<> 8__locals1;
            CS$<> 8__locals1.usable = usable;
            CS$<> 8__locals1.<> 4__this = this;
            CS$<> 8__locals1.cellAlreadyHighlighted = cellAlreadyHighlighted;
            Item item = CS$<> 8__locals1.usable as Item;
            if (item != null && !item.Identified)
            {
                IdentificationGroupSpec identificationGroup = Get.IdentificationGroups.GetIdentificationGroup(item.Spec);
                if (identificationGroup == null || !identificationGroup.ShowAoEEffectsEvenIfUnidentified)
                {
                    return;
                }
            }
            foreach (UseEffect useEffect in CS$<> 8__locals1.usable.UseEffects.All)
			{
                if (!useEffect.Hidden)
                {
                    UseEffect_CheckCollapseCeiling useEffect_CheckCollapseCeiling = useEffect as UseEffect_CheckCollapseCeiling;
                    if (useEffect_CheckCollapseCeiling != null)
                    {
                        Entity entity = CS$<> 8__locals1.usable as Entity;
                        if (entity != null)
                        {
                            useEffect_CheckCollapseCeiling.GetAffectedCellsForUIAssumingChainedPillarDestroy(entity, InteractionManager.tmpCollapsedCeiling);
                            using (List<Vector3Int>.Enumerator enumerator2 = InteractionManager.tmpCollapsedCeiling.GetEnumerator())
                            {
                                while (enumerator2.MoveNext())
                                {
                                    Vector3Int vector3Int = enumerator2.Current;
                                    this.< DrawAoEEffects > g__ProcessCell | 111_0(vector3Int, ref CS$<> 8__locals1);
                                }
                                continue;
                            }
                        }
                    }
                    UseEffect_CheckCollapseCeilingSingle useEffect_CheckCollapseCeilingSingle = useEffect as UseEffect_CheckCollapseCeilingSingle;
                    if (useEffect_CheckCollapseCeilingSingle != null)
                    {
                        Entity entity2 = CS$<> 8__locals1.usable as Entity;
                        if (entity2 != null)
                        {
                            useEffect_CheckCollapseCeilingSingle.GetAffectedCellsForUI(entity2, InteractionManager.tmpCollapsedCeiling);
                            using (List<Vector3Int>.Enumerator enumerator2 = InteractionManager.tmpCollapsedCeiling.GetEnumerator())
                            {
                                while (enumerator2.MoveNext())
                                {
                                    Vector3Int vector3Int2 = enumerator2.Current;
                                    this.< DrawAoEEffects > g__ProcessCell | 111_0(vector3Int2, ref CS$<> 8__locals1);
                                }
                                continue;
                            }
                        }
                    }
                    if (useEffect.AoERadius != null || useEffect.HasCustomAoEArea)
                    {
                        InteractionManager.tmpAoEArea.Clear();
                        AoEUtility.GetAoEArea(pos, useEffect, CS$<> 8__locals1.usable, InteractionManager.tmpAoEArea, null);
                        foreach (Vector3Int vector3Int3 in InteractionManager.tmpAoEArea)
                        {
                            this.< DrawAoEEffects > g__ProcessCell | 111_0(vector3Int3, ref CS$<> 8__locals1);
                        }
                    }
                }
            }
        }

        private bool CanTargetBodyPart(IUsable usable)
        {
            using (List<UseEffect>.Enumerator enumerator = usable.UseEffects.All.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is UseEffect_Damage)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public IntRange? GetLostHPRangeForUI(Entity entity)
        {
            if (entity.IsNowControlledActor)
            {
                Actor actor = this.PointedAtEntity as Actor;
                if (actor != null)
                {
                    return AIUtility_Attack.GetPredictedDealtDamageRangeToPlayerForUI(actor);
                }
            }
            else if (this.PointedAtEntity == entity && this.PointedAtBodyPart == null && this.LastPossibleInteraction != null && this.LastPossibleInteraction.Value.usable != null && !(this.LastPossibleInteraction.Value.usable is Structure))
            {
                return this.LastPossibleInteraction.Value.dealtDamageRange;
            }
            IntRange? intRange = null;
            foreach (ValueTuple<Entity, IUsable> valueTuple in this.entitiesAffectedByAnyAoE)
            {
                Entity item = valueTuple.Item1;
                IUsable item2 = valueTuple.Item2;
                if (item == entity)
                {
                    Item item3 = item2 as Item;
                    if (item3 == null || item3.Identified)
                    {
                        Actor actor2 = ((item2 is Structure) ? null : Get.NowControlledActor);
                        IUsable usable = item2;
                        Entity entity2 = item;
                        Vector3Int? pathDestinationOverride = this.LastPossibleInteraction.Value.pathDestinationOverride;
                        IntRange? aoEDealtDamageRangeForUI = UsableUtility.GetAoEDealtDamageRangeForUI(actor2, usable, entity2, (pathDestinationOverride != null) ? pathDestinationOverride.GetValueOrDefault() : this.PointedAtEntity);
                        if (aoEDealtDamageRangeForUI != null)
                        {
                            if (intRange == null)
                            {
                                intRange = aoEDealtDamageRangeForUI;
                            }
                            else
                            {
                                intRange = new IntRange?(new IntRange(intRange.Value.from + aoEDealtDamageRangeForUI.Value.from, intRange.Value.to + aoEDealtDamageRangeForUI.Value.to));
                            }
                        }
                    }
                }
            }
            return intRange;
        }

        private bool ShouldIgnoreClick(Entity targetEntity)
        {
            return Clock.UnscaledTime - this.lastActorInteractionUnscaledTime < 0.2f && Vector3.Dot(this.lastActorInteractionCameraForwardVector, Get.Camera.transform.forward) >= 0.9986f && Get.NowControlledActor.Position == this.lastActorInteractionPlayerPos && this.lastActorInteractionActor != targetEntity;
        }

        private void TryPerformInteraction(InteractionManager.PossibleInteraction interaction, Entity entity)
        {
            if (this.ShouldIgnoreClick(entity))
            {
                return;
            }
            this.PerformInteraction(interaction, entity);
        }

        public void PerformInteraction(InteractionManager.PossibleInteraction interaction, Entity entity)
        {
            if (interaction.finalActionGetter != null || interaction.pathMode != PathFinder.Request.Mode.None)
            {
                if (interaction.pathMode == PathFinder.Request.Mode.None)
                {
                    List<Action> list = new List<Action>();
                    Action action = interaction.finalActionGetter();
                    if (action != null)
                    {
                        list.Add(action);
                    }
                    else
                    {
                        Log.Error("Final action getter returned null.", false);
                    }
                    this.plannedPlayerActions.Set(list, null);
                    Actor actor = entity as Actor;
                    if (actor != null)
                    {
                        this.lastActorInteractionUnscaledTime = Clock.UnscaledTime;
                        this.lastActorInteractionCameraForwardVector = Get.CameraTransform.forward;
                        this.lastActorInteractionPlayerPos = Get.NowControlledActor.Position;
                        this.lastActorInteractionActor = actor;
                        return;
                    }
                }
                else
                {
                    List<Vector3Int> list2 = Get.PathFinder.FindPath(Get.NowControlledActor.Position, interaction.pathDestinationOverride ?? entity.Position, new PathFinder.Request(interaction.pathMode, Get.NowControlledActor), 15, this.tmpListForPathFinder);
                    if (list2 != null)
                    {
                        List<Action> list3 = this.MakeMoveToActions(list2, Get.KeyBinding_OneStep.HeldDown);
                        if (interaction.finalActionGetter != null && (!Get.KeyBinding_OneStep.HeldDown || list2.Count <= 1))
                        {
                            Action action2 = interaction.finalActionGetter();
                            if (action2 != null)
                            {
                                list3.Add(action2);
                            }
                            else
                            {
                                Log.Error("Final action getter returned null.", false);
                            }
                        }
                        this.plannedPlayerActions.Set(list3, null);
                    }
                }
            }
        }

        private static bool HasUsePrompt(InteractionManager.PossibleInteraction interaction)
        {
            IUsable usable = interaction.usable;
            UsePrompt usePrompt = ((usable != null) ? usable.UsePrompt : null);
            return usePrompt != null && !(usePrompt is UsePrompt_NewRun);
        }

        [CompilerGenerated]
        private void <OnGUI>g__DrawInteractionAfterUI|89_0(Texture2D icon1, Color icon1Color, Texture2D icon2, Color icon2Color, int sequence, bool disabled, string disabledReason, IntRange? dealtDamageRange, int? targetHP, bool hasUsePrompt, float? arrowDirection, bool anyActionTakesMoreThan1Turn)
		{
			this.hasInteractionToDraw = true;
			this.interactionIcon1 = icon1;
			this.interactionIcon1Color = icon1Color;
			this.interactionIcon2 = icon2;
			this.interactionIcon2Color = icon2Color;
			this.interactionSequence = sequence;
			this.interactionDisabled = disabled;
			this.interactionDisabledReason = disabledReason;
			this.interactionDealtDamageRange = dealtDamageRange;
			this.interactionTargetHP = targetHP;
			this.interactionHasUsePrompt = hasUsePrompt;
			this.interactionArrowDirection = arrowDirection;
			this.interactionAnyActionTakesMoreThan1Turn = anyActionTakesMoreThan1Turn;
		}

    [CompilerGenerated]
    private void <DrawAoEEffects>g__ProcessCell|111_0(Vector3Int c, ref InteractionManager.<>c__DisplayClass111_0 A_2)
		{
			foreach (Entity entity in Get.World.GetEntitiesAt(c))
			{
				if (A_2.usable.UseFilterAoE.Allows(entity, Get.NowControlledActor))
				{
                    this.entitiesAffectedByAnyAoE.Add(new ValueTuple<Entity, IUsable>(entity, A_2.usable));
					Actor actor = entity as Actor;
					if (actor != null && !actor.IsNowControlledActor)
					{
						actor.ActorGOC.MarkForAffectedByAoEHighlight();
					}
				}
			}
			if (c != A_2.cellAlreadyHighlighted && Get.CellsInfo.IsFloorUnderNoActors(c) && !Get.CellsInfo.IsFilled(c) && !Get.FogOfWar.IsFogged(c) && c.Below().InBounds() && Get.CellsInfo.AnySupportsTargetingLocations(c.Below()))
{
    Get.CellHighlighter.HighlightCell(c, CellHighlighter.DeepRed);
}
		}

		[Saved(Default.New, false)]
private PlannedPlayerActions plannedPlayerActions = new PlannedPlayerActions();

private Vector3Int? pointedAtLedge;

private Entity pointedAtEntity;

private BodyPart pointedAtBodyPart;

private Vector3? pointedAtExactPos;

private Entity highlightedEntity;

private BodyPart highlightedBodyPart;

private Vector3Int raycastHitDirection;

private InteractionManager.PossibleInteraction? lastPossibleInteraction;

private bool lastPointedAtEntityCalcHadInspectMode;

private List<Vector3Int> tmpListForPathFinder = new List<Vector3Int>();

private StringSlot tmpFailReason = new StringSlot();

private HashSet<ValueTuple<Entity, IUsable>> entitiesAffectedByAnyAoE = new HashSet<ValueTuple<Entity, IUsable>>();

private Entity currentScrollTarget;

private float entityScrollAccumulatedDelta;

private float lastActorInteractionUnscaledTime = -99999f;

private Vector3 lastActorInteractionCameraForwardVector;

private Vector3Int lastActorInteractionPlayerPos;

private Actor lastActorInteractionActor;

private float lastTimeTriedDisabledAction;

private string lastTriedDisabledActionReason;

private bool hasInteractionToDraw;

private Texture2D interactionIcon1;

private Color interactionIcon1Color;

private Texture2D interactionIcon2;

private Color interactionIcon2Color;

private int interactionSequence;

private bool interactionDisabled;

private string interactionDisabledReason;

private IntRange? interactionDealtDamageRange;

private int? interactionTargetHP;

private bool interactionHasUsePrompt;

private float? interactionArrowDirection;

private bool interactionAnyActionTakesMoreThan1Turn;

private string interactionExtraHint;

private readonly Predicate<GameObject> RaycastShouldIgnoreCached;

private static readonly Texture2D JumpOffLedgeIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/JumpOffLedge1");

private static readonly Texture2D JumpOffLedgeIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/JumpOffLedge2");

public static readonly Texture2D MoveIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/Move1");

private static readonly Texture2D MoveIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/Move2");

private static readonly Texture2D AttackIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/Attack1");

private static readonly Texture2D AttackIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/Attack2");

private static readonly Texture2D KickIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/Kick1");

private static readonly Texture2D KickIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/Kick2");

private static readonly Texture2D UseStructureIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/UseStructure1");

private static readonly Texture2D UseStructureIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/UseStructure2");

private static readonly Texture2D TalkIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/Talk1");

private static readonly Texture2D TalkIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/Talk2");

private static readonly Texture2D PickUpIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/PickUp1");

private static readonly Texture2D PickUpIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/PickUp2");

private static readonly Texture2D BuyIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/Buy1");

private static readonly Texture2D BuyIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/Buy2");

private static readonly Texture2D UseLadderUpIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/UseLadderUp1");

private static readonly Texture2D UseLadderUpIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/UseLadderUp2");

private static readonly Texture2D UseLadderDownIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/UseLadderDown1");

private static readonly Texture2D UseLadderDownIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/UseLadderDown2");

private static readonly Texture2D UseStairsUpIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/UseStairsUp1");

private static readonly Texture2D UseStairsUpIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/UseStairsUp2");

private static readonly Texture2D UseStairsDownIcon1 = Assets.Get<Texture2D>("Textures/UI/Interaction/UseStairsDown1");

private static readonly Texture2D UseStairsDownIcon2 = Assets.Get<Texture2D>("Textures/UI/Interaction/UseStairsDown2");

public const float MaxRaycastDistance = 8f;

private const int MaxTraversalDistance = 15;

private GameObject cachedEntityGOCFor;

private EntityGOC cachedEntityGOC;

private Func<Action> cachedUseActionDelegate;

private IUsable cachedUseActionDelegateForUsable;

private Target cachedUseActionDelegateForTarget;

private BodyPart cachedUseActionDelegateForTargetBodyPart;

private static List<Vector3Int> tmpCollapsedCeiling = new List<Vector3Int>();

private static List<Vector3Int> tmpAoEArea = new List<Vector3Int>();

public struct PossibleInteraction
{
    public PossibleInteraction(PathFinder.Request.Mode pathMode, Func<Action> finalActionGetter, int finalActionSequence, Texture2D icon1, Color icon1Color, Texture2D icon2, Color icon2Color, Vector3Int? pathDestinationOverride = null, bool highlightEntity = true, bool highlightBodyPart = false, Texture2D locationTargetingIcon = null, Color? locationTargetingIconColor = null, IUsable usable = null, IUsable drawAoEEffectsFor = null, IUsable drawAoEEffectsFor2 = null, string failReason = null, IntRange? dealtDamageRange = null)
    {
        this.pathMode = pathMode;
        this.finalActionGetter = finalActionGetter;
        this.finalActionSequence = finalActionSequence;
        this.icon1 = icon1;
        this.icon1Color = icon1Color;
        this.icon2 = icon2;
        this.icon2Color = icon2Color;
        this.pathDestinationOverride = pathDestinationOverride;
        this.highlightEntity = highlightEntity;
        this.highlightBodyPart = highlightBodyPart;
        this.locationTargetingIcon = locationTargetingIcon;
        this.locationTargetingIconColor = locationTargetingIconColor ?? Color.white;
        this.usable = usable;
        this.drawAoEEffectsFor = drawAoEEffectsFor;
        this.drawAoEEffectsFor2 = drawAoEEffectsFor2;
        this.failReason = failReason;
        this.dealtDamageRange = dealtDamageRange;
    }

    public PathFinder.Request.Mode pathMode;

    public Func<Action> finalActionGetter;

    public int finalActionSequence;

    public Texture2D icon1;

    public Color icon1Color;

    public Texture2D icon2;

    public Color icon2Color;

    public Vector3Int? pathDestinationOverride;

    public bool highlightEntity;

    public bool highlightBodyPart;

    public Texture2D locationTargetingIcon;

    public Color locationTargetingIconColor;

    public IUsable usable;

    public IUsable drawAoEEffectsFor;

    public IUsable drawAoEEffectsFor2;

    public string failReason;

    public IntRange? dealtDamageRange;
}
	}
}