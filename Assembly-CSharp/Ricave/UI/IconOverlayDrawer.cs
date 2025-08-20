using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class IconOverlayDrawer
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (!Get.UI.WantsMouseUnlocked && KeyCodeUtility.InspectHeldDown)
            {
                return;
            }
            if (Get.KeyBinding_Minimap.HeldDown || Get.KeyBinding_MinimapAlt.HeldDown)
            {
                return;
            }
            if (Get.InLobby)
            {
                return;
            }
            this.SetWorldPosForEntityEntries();
            this.CheckAddDynamicEntries();
            if (this.entries.Count >= 2)
            {
                this.entries.Sort(IconOverlayDrawer.ByDistToCamera);
            }
            for (int i = 0; i < this.entries.Count; i++)
            {
                IconOverlayDrawer.Entry entry = this.entries[i];
                Vector3 worldPos = entry.worldPos;
                Texture2D texture2D;
                if ((texture2D = entry.customIcon) == null)
                {
                    Entity entity = entry.entity;
                    texture2D = ((entity != null) ? entity.Icon : null);
                }
                Color? customIconColor = entry.customIconColor;
                Color color;
                if (customIconColor == null)
                {
                    Entity entity2 = entry.entity;
                    color = ((entity2 != null) ? entity2.IconColor : Color.white);
                }
                else
                {
                    color = customIconColor.GetValueOrDefault();
                }
                bool flag;
                Vector2 vector;
                this.DoIcon(worldPos, texture2D, color, entry.entity, entry.room, entry.alphaPct, entry.screenPosOffset, entry.IconSize, new bool?(entry.drawnAtScreenEdge), out flag, out vector);
                if (!entry.drawnAtScreenEdge && flag)
                {
                    entry.alphaPct = 0f;
                }
                entry.drawnAtScreenEdge = flag;
                entry.screenPos = vector;
                this.entries[i] = entry;
            }
            Entity mouseoverEntity = Get.SeenEntitiesDrawer.MouseoverEntity;
            if (mouseoverEntity != null && mouseoverEntity.Spawned)
            {
                bool flag2 = false;
                for (int j = 0; j < this.entries.Count; j++)
                {
                    if (this.entries[j].entity == mouseoverEntity && this.entries[j].alphaPct > 0f)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    bool flag3;
                    Vector2 vector2;
                    this.DoIcon(mouseoverEntity.RenderPositionComputedCenter, mouseoverEntity.Icon, mouseoverEntity.IconColor, mouseoverEntity, null, 1f, Vector2.zero, 90f, null, out flag3, out vector2);
                }
            }
        }

        private void DoIcon(Vector3 worldPos, Texture2D icon, Color iconColor, Entity entity, RetainedRoomInfo.RoomInfo room, float alphaPct, Vector2 screenPosOffset, float iconSize, bool? prevDrawnAtScreenEdge, out bool drawnAtScreenEdge, out Vector2 screenPos)
        {
            Vector3 vector = Get.Camera.WorldToScreenPoint(worldPos) / Widgets.UIScale;
            vector.y = Widgets.VirtualHeight - vector.y;
            bool flag = vector.z <= 0f;
            float num = iconSize / 2f + 5f;
            float num2;
            if (!Widgets.ScreenRect.ContractedBy(num).Contains(vector) || flag)
            {
                Vector2 vector2 = new Vector2(vector.x, vector.y) - Widgets.ScreenCenter;
                vector2.Normalize();
                if (flag)
                {
                    vector2 = -vector2;
                }
                num2 = Calc.Atan2(vector2.y, vector2.x) * 57.29578f;
                vector2 *= Widgets.VirtualWidth + Widgets.VirtualHeight;
                Vector2 vector3 = Widgets.ScreenCenter + vector2;
                ValueTuple<Vector2?, Vector2?> lineEllipseIntersection = Geometry.GetLineEllipseIntersection(Widgets.ScreenCenter, vector3, Widgets.ScreenRect.ContractedByPct(0.1f));
                if (lineEllipseIntersection.Item1 != null)
                {
                    vector = lineEllipseIntersection.Item1.Value;
                }
                drawnAtScreenEdge = true;
            }
            else
            {
                num2 = 0f;
                drawnAtScreenEdge = false;
            }
            if ((prevDrawnAtScreenEdge != null && !prevDrawnAtScreenEdge.Value) & drawnAtScreenEdge)
            {
                alphaPct = 0f;
            }
            vector.x = Calc.Clamp(vector.x, num, Widgets.VirtualWidth - num);
            vector.y = Calc.Clamp(vector.y, num, Widgets.VirtualHeight - num);
            screenPos = new Vector2(vector.x, vector.y);
            Vector2 vector4 = screenPos + screenPosOffset;
            float newEntityHighlightFactor = this.GetNewEntityHighlightFactor(entity);
            if (newEntityHighlightFactor > 0f)
            {
                GUI.color = this.GetHighlightColor(entity).WithAlpha(newEntityHighlightFactor * 0.65f);
                GUI.DrawTexture(new Rect(Calc.Round(vector4.x - 375f), Calc.Round(vector4.y - 375f), 750f, 750f), IconOverlayDrawer.HighlightTex);
            }
            if (alphaPct > 0f)
            {
                Rect rect = new Rect(Calc.Round(vector4.x - iconSize / 2f), Calc.Round(vector4.y - iconSize / 2f), iconSize, iconSize);
                Actor actor = entity as Actor;
                if (actor != null)
                {
                    GUI.color = actor.HostilityColor.MultipliedColor(1.43f).WithAlphaFactor(alphaPct);
                }
                else if (room != null)
                {
                    GUI.color = new Color(0.6f, 0.6f, 0.6f, 0.65f * alphaPct);
                }
                else
                {
                    GUI.color = new Color(1f, 1f, 1f, alphaPct);
                }
                if (entity != null && Get.SeenEntitiesDrawer.MouseoverEntity == entity)
                {
                    GUI.color = GUI.color.Lighter(0.23f);
                }
                if (drawnAtScreenEdge)
                {
                    GUIExtra.DrawTextureRotated(RectUtility.CenteredAt(rect.center, iconSize * ((float)IconOverlayDrawer.IconArrow.width / (float)IconOverlayDrawer.IconBackground.width)), IconOverlayDrawer.IconArrow, num2 + 90f, null);
                }
                GUI.DrawTexture(rect, IconOverlayDrawer.IconBackground);
                if (icon != null)
                {
                    GUI.color = iconColor.WithAlphaFactor(alphaPct);
                    GUI.DrawTexture(rect.ContractedBy(iconSize * 0.07f), icon);
                    if (icon == Get.Entity_Staircase.IconAdjusted && Get.World.AnyEntityOfSpec(Get.Entity_Lever))
                    {
                        GUI.DrawTexture(rect.ContractedByPct(0.2f), IconOverlayDrawer.LockIcon);
                    }
                }
                if (actor != null && actor.ConditionsAccumulated.AnyOfSpec(Get.Condition_Sleeping))
                {
                    GUI.color = new Color(1f, 1f, 1f, alphaPct * 0.5f);
                    GUI.DrawTexture(rect.ContractedBy(iconSize * 0.17f), Get.Condition_Sleeping.Icon);
                }
                if (actor != null)
                {
                    float num3 = Clock.UnscaledTime - actor.LastTimeCausedPlayerToLoseHP;
                    float num4 = 1f - Calc.ResolveFadeIn(num3, 1f);
                    if (num4 > 0f)
                    {
                        GUIExtra.DrawCircle(rect.center, iconSize / 2f, new Color(1f, 0.18f, 0.18f, num4 * alphaPct));
                    }
                }
            }
            GUI.color = Color.white;
        }

        public void FixedUpdate()
        {
            for (int i = this.entries.Count - 1; i >= 0; i--)
            {
                IconOverlayDrawer.Entry entry = this.entries[i];
                if (entry.wantsPresent && entry.drawnAtScreenEdge)
                {
                    entry.alphaPct += Clock.FixedDeltaTime * 4.2f;
                }
                else
                {
                    entry.alphaPct -= Clock.FixedDeltaTime * 1.1f;
                }
                entry.alphaPct = Calc.Clamp01(entry.alphaPct);
                if (!entry.wantsPresent && (entry.alphaPct <= 0f || (entry.entity != null && !entry.entity.Spawned)))
                {
                    this.entries.RemoveAt(i);
                }
                else
                {
                    this.entries[i] = entry;
                }
            }
            for (int j = 0; j < this.entries.Count; j++)
            {
                IconOverlayDrawer.Entry entry2 = this.entries[j];
                if (entry2.entity != null)
                {
                    if ((Widgets.ScreenCenter - entry2.screenPos).sqrMagnitude < 40000f)
                    {
                        for (int k = 0; k < this.entries.Count; k++)
                        {
                            if (j != k && this.entries[k].entity != null)
                            {
                                Vector2 vector = this.entries[k].ScreenPosWithOffset - entry2.ScreenPosWithOffset;
                                if (vector == Vector2.zero)
                                {
                                    vector = Vector2.right;
                                }
                                float num = entry2.IconSize / 2f + this.entries[k].IconSize / 2f;
                                if (vector.sqrMagnitude < num * num)
                                {
                                    entry2.screenPosOffset -= vector.normalized * 200f * Clock.FixedDeltaTime;
                                }
                            }
                        }
                    }
                    else
                    {
                        entry2.screenPosOffset = Vector2.MoveTowards(entry2.screenPosOffset, default(Vector2), 100f * Clock.FixedDeltaTime);
                    }
                    this.entries[j] = entry2;
                }
            }
        }

        public void OnEntitiesVisibilityChanged(List<Entity> entitiesNoLongerSeen, List<Entity> entitiesNewlySeen)
        {
            for (int i = 0; i < entitiesNoLongerSeen.Count; i++)
            {
                if (this.ShouldEverList(entitiesNoLongerSeen[i]))
                {
                    for (int j = 0; j < this.entries.Count; j++)
                    {
                        if (this.entries[j].entity == entitiesNoLongerSeen[i])
                        {
                            IconOverlayDrawer.Entry entry = this.entries[j];
                            entry.wantsPresent = false;
                            this.entries[j] = entry;
                            break;
                        }
                    }
                    this.lastSeenOnTurn[entitiesNoLongerSeen[i]] = Get.TurnManager.RewindPoint;
                    this.lastSeenTime[entitiesNoLongerSeen[i]] = Clock.UnscaledTime;
                }
            }
            for (int k = 0; k < entitiesNewlySeen.Count; k++)
            {
                if (this.ShouldEverList(entitiesNewlySeen[k]))
                {
                    bool flag = false;
                    for (int l = 0; l < this.entries.Count; l++)
                    {
                        if (this.entries[l].entity == entitiesNewlySeen[k])
                        {
                            IconOverlayDrawer.Entry entry2 = this.entries[l];
                            entry2.wantsPresent = true;
                            entry2.newEnemySeenStartTime = (this.ResolveNewEntitySeen(entitiesNewlySeen[k]) ? Clock.UnscaledTime : entry2.newEnemySeenStartTime);
                            this.entries[l] = entry2;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        this.entries.Add(new IconOverlayDrawer.Entry
                        {
                            entity = entitiesNewlySeen[k],
                            wantsPresent = true,
                            newEnemySeenStartTime = (this.ResolveNewEntitySeen(entitiesNewlySeen[k]) ? Clock.UnscaledTime : (-1f))
                        });
                    }
                }
            }
        }

        public float GetNewEntityHighlightFactor(Entity entity)
        {
            if (entity == null)
            {
                return 0f;
            }
            int i = 0;
            while (i < this.entries.Count)
            {
                if (this.entries[i].entity == entity)
                {
                    float num = Clock.UnscaledTime - this.entries[i].newEnemySeenStartTime;
                    if (num > 1.3f)
                    {
                        return 0f;
                    }
                    if (num < 0.65f)
                    {
                        return num / 1.3f * 2f;
                    }
                    return (1f - num / 1.3f) * 2f;
                }
                else
                {
                    i++;
                }
            }
            return 0f;
        }

        private bool EntityOccluded(Entity entity)
        {
            IconOverlayDrawer.<> c__DisplayClass23_0 CS$<> 8__locals1;
            CS$<> 8__locals1.entity = entity;
            Entity entity2 = CS$<> 8__locals1.entity;
            if (((entity2 != null) ? entity2.GameObject : null) == null)
            {
                return false;
            }
            if (!(CS$<> 8__locals1.entity is Actor))
			{
                return false;
            }
            Vector3 localScale = CS$<> 8__locals1.entity.GameObject.transform.localScale;
            Vector3 renderPositionComputedCenter = CS$<> 8__locals1.entity.RenderPositionComputedCenter;
            Vector3 vector = Vector3.Cross((renderPositionComputedCenter - Get.CameraPosition).normalized, Vector3.up);
            Vector3 vector2 = renderPositionComputedCenter + vector * 0.05f * localScale.x + Vector3.up * 0.065f * localScale.y;
            Vector3 vector3 = renderPositionComputedCenter - vector * 0.05f * localScale.x + Vector3.down * 0.065f * localScale.y;
            return IconOverlayDrawer.< EntityOccluded > g__RaycastOccluded | 23_0(vector2, ref CS$<> 8__locals1) && IconOverlayDrawer.< EntityOccluded > g__RaycastOccluded | 23_0(vector3, ref CS$<> 8__locals1);
        }

        private bool ResolveNewEntitySeen(Entity entity)
        {
            if (this.lastSeenOnTurn.ContainsKey(entity) && Get.TurnManager.RewindPoint - this.lastSeenOnTurn[entity] < 5 && Clock.UnscaledTime - this.lastSeenTime[entity] < 10f)
            {
                return false;
            }
            Actor actor = entity as Actor;
            if (actor != null && !actor.IsNowControlledActor && !actor.IsPlayerParty)
            {
                Get.Sound_NewActorSeen.PlayOneShot(null, 1f, 1f);
            }
            else if (entity.Spec == Get.Entity_Lever)
            {
                Get.Sound_LeverFound.PlayOneShot(new Vector3?(entity.Position), 1f, 1f);
            }
            Actor actor2 = entity as Actor;
            if (actor2 != null)
            {
                if (!actor2.IsHostile(Get.NowControlledActor))
                {
                    return false;
                }
                Get.PlannedPlayerActions.Interrupt();
            }
            return true;
        }

        private bool ShouldEverList(Entity entity)
        {
            return entity.Spec == Get.Entity_Lever || (entity is Actor && !entity.IsNowControlledActor);
        }

        private Color GetHighlightColor(Entity entity)
        {
            Actor actor = entity as Actor;
            if (actor != null && actor.IsHostile(Get.NowControlledActor))
            {
                return Color.red;
            }
            return Color.white;
        }

        public void OnWorldAboutToRegenerate()
        {
            this.entries.Clear();
            this.lastSeenOnTurn.Clear();
            this.lastSeenTime.Clear();
        }

        public bool AnyIconForEntitySpecShowing(EntitySpec spec)
        {
            for (int i = 0; i < this.entries.Count; i++)
            {
                if (this.entries[i].entity != null && this.entries[i].entity.Spec == spec)
                {
                    return true;
                }
            }
            return false;
        }

        private void SetWorldPosForEntityEntries()
        {
            for (int i = 0; i < this.entries.Count; i++)
            {
                if (this.entries[i].entity != null && this.entries[i].entity.Spawned)
                {
                    IconOverlayDrawer.Entry entry = this.entries[i];
                    entry.worldPos = this.entries[i].entity.RenderPositionComputedCenter;
                    this.entries[i] = entry;
                }
            }
        }

        private void CheckAddDynamicEntries()
        {
            for (int i = 0; i < this.entries.Count; i++)
            {
                if (this.entries[i].room != null)
                {
                    IconOverlayDrawer.Entry entry = this.entries[i];
                    entry.wantsPresent = false;
                    this.entries[i] = entry;
                }
            }
            List<RetainedRoomInfo.RoomInfo> rooms = Get.RetainedRoomInfo.Rooms;
            Vector3Int position = Get.NowControlledActor.Position;
            for (int j = 0; j < rooms.Count; j++)
            {
                if (rooms[j].AnyNonFilledCellUnfogged && (position.x < rooms[j].Shape.x || position.x > rooms[j].Shape.xMax || position.z < rooms[j].Shape.z || position.z > rooms[j].Shape.zMax) && (rooms[j].EverVisitedOrKnown || Minimap.CanShowUnexploredSymbolFor(rooms[j])))
                {
                    IconOverlayDrawer.<> c__DisplayClass30_0 CS$<> 8__locals1;
                    Texture2D iconToShowForRoom = this.GetIconToShowForRoom(rooms[j], out CS$<> 8__locals1.flashing);
                    if (!(iconToShowForRoom == null))
                    {
                        bool flag = false;
                        for (int k = 0; k < this.entries.Count; k++)
                        {
                            if (this.entries[k].room == rooms[j])
                            {
                                IconOverlayDrawer.Entry entry2 = this.entries[k];
                                entry2.wantsPresent = true;
                                entry2.customIcon = iconToShowForRoom;
                                if (CS$<> 8__locals1.flashing)
								{
                entry2.customIconColor = new Color?(new Color(1f, 1f, 1f, IconOverlayDrawer.< CheckAddDynamicEntries > g__FlashingAlpha | 30_0(ref CS$<> 8__locals1)));
            }
            this.entries[k] = entry2;
            flag = true;
            break;
        }
    }
						if (!flag)
						{
							Vector3 centerFloat = rooms[j].Shape.CenterFloat;
    centerFloat.y = (float) (rooms[j].Shape.yMin + 1);

                            this.entries.Add(new IconOverlayDrawer.Entry
							{
								worldPos = centerFloat,
								room = rooms[j],
								customIcon = iconToShowForRoom,
								customIconColor = new Color? (new Color(1f, 1f, 1f, IconOverlayDrawer.<CheckAddDynamicEntries>g__FlashingAlpha|30_0(ref CS$<>8__locals1))),
								wantsPresent = true
							});
						}
					}
				}
			}
		}

		private Texture2D GetIconToShowForRoom(RetainedRoomInfo.RoomInfo room, out bool flashing)
{
    flashing = false;
    if (!room.EverVisitedOrKnown)
    {
        return null;
    }
    if (room.Role == Room.LayoutRole.End && !this.AnyIconForEntitySpecShowing(Get.Entity_Staircase))
    {
        flashing = !Get.World.AnyEntityOfSpec(Get.Entity_Lever);
        return Get.Entity_Staircase.IconAdjusted;
    }
    if (room.Role == Room.LayoutRole.LeverRoom && Get.World.AnyEntityOfSpec(Get.Entity_Lever) && !this.AnyIconForEntitySpecShowing(Get.Entity_Lever))
    {
        flashing = true;
        return Get.Entity_Lever.IconAdjusted;
    }
    if (room.Role == Room.LayoutRole.Start)
    {
        return Get.Entity_Sign.IconAdjusted;
    }
    return null;
}

public void OnSwitchedNowControlledActor(Actor prevActor)
{
    for (int i = this.entries.Count - 1; i >= 0; i--)
    {
        if (this.entries[i].entity != null && this.entries[i].entity.IsNowControlledActor)
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

[CompilerGenerated]
internal static bool < EntityOccluded > g__RaycastOccluded | 23_0(Vector3 raycastDest, ref IconOverlayDrawer.<> c__DisplayClass23_0 A_1)

        {
    Vector3 cameraPosition = Get.CameraPosition;
    Vector3Int vector3Int;
    Vector3 vector;
    GameObject gameObject = RaycastUtility.Raycast(cameraPosition, (raycastDest - cameraPosition).normalized, (raycastDest - cameraPosition).magnitude + 1f, out vector3Int, out vector, true, true, false, false, null);
    if (gameObject == null)
    {
        return true;
    }
    EntityGOC componentInParent = gameObject.GetComponentInParent<EntityGOC>();
    return componentInParent == null || componentInParent.Entity != A_1.entity;
}

[CompilerGenerated]
internal static float < CheckAddDynamicEntries > g__FlashingAlpha | 30_0(ref IconOverlayDrawer.<> c__DisplayClass30_0 A_0)

        {
    if (!A_0.flashing)
    {
        return 0.7f;
    }
    return 0.5f + Calc.PulseUnscaled(3f, 0.5f);
}

private List<IconOverlayDrawer.Entry> entries = new List<IconOverlayDrawer.Entry>();

private Dictionary<Entity, int> lastSeenOnTurn = new Dictionary<Entity, int>();

private Dictionary<Entity, float> lastSeenTime = new Dictionary<Entity, float>();

private const int HighlightSize = 750;

private const float BaseIconSize = 90f;

private const float PosLerpSpeed = 0.2f;

private const float MoveIconsApartDistToScreenCenter = 200f;

private const int ConsideredNewEnemyIfSeenTurnsAgo = 5;

private const float ConsideredNewEnemyIfSeenSecondsAgo = 10f;

private const float NewEntityHighlightDuration = 1.3f;

private const float IconFadeInSpeed = 4.2f;

private const float IconFadeOutSpeed = 1.1f;

private static readonly Texture2D HighlightTex = Assets.Get<Texture2D>("Textures/UI/NewEntityHighlight");

private static readonly Texture2D IconBackground = Assets.Get<Texture2D>("Textures/UI/IconBackground");

private static readonly Texture2D IconArrow = Assets.Get<Texture2D>("Textures/UI/IconArrow");

private static readonly Texture2D LockIcon = Assets.Get<Texture2D>("Textures/UI/Lock");

private static readonly Comparison<IconOverlayDrawer.Entry> ByDistToCamera = (IconOverlayDrawer.Entry a, IconOverlayDrawer.Entry b) => (b.worldPos - Get.CameraPosition).sqrMagnitude.CompareTo((a.worldPos - Get.CameraPosition).sqrMagnitude);

private struct Entry
{
    public Vector2 ScreenPosWithOffset
    {
        get
        {
            return this.screenPos + this.screenPosOffset;
        }
    }

    public float IconSize
    {
        get
        {
            if (this.room != null)
            {
                return 45f;
            }
            Actor actor = this.entity as Actor;
            if (actor != null)
            {
                float num;
                if (actor.IsBoss)
                {
                    num = 108.00001f;
                }
                else if (actor.IsBaby)
                {
                    num = 81f;
                }
                else
                {
                    num = 90f;
                }
                if (actor.Spawned && actor.Position.IsAdjacentOrInside(Get.NowControlledActor.Position) && actor.IsHostile(Get.NowControlledActor))
                {
                    num *= 1f + Calc.PulseUnscaled(6f, 0.1f);
                }
                return num;
            }
            return 90f;
        }
    }

    public Entity entity;

    public RetainedRoomInfo.RoomInfo room;

    public Vector3 worldPos;

    public Texture2D customIcon;

    public Color? customIconColor;

    public bool wantsPresent;

    public bool drawnAtScreenEdge;

    public float alphaPct;

    public Vector2 screenPos;

    public Vector2 screenPosOffset;

    public float newEnemySeenStartTime;
}
	}
}