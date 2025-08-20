using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class Compass
    {
        public static void Do()
        {
            if (Get.InLobby)
            {
                return;
            }
            Rect rect = new Rect(Widgets.VirtualWidth - 130f - Compass.Offset.x, Widgets.VirtualHeight - 130f - Compass.Offset.y, 130f, 130f);
            if (Widgets.ButtonInvisible(rect, false, false))
            {
                Get.Sound_Rotation.PlayOneShot(null, 1f, 1f);
                Get.FPPControllerGOC.SetActorTargetRotation((Event.current.button == 1) ? 180f : 0f);
                Get.FPPControllerGOC.SetCameraTargetRotation(0f);
            }
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            Rect rect2 = rect.ExpandedBy(10f);
            float actorRotation = Get.FPPControllerGOC.ActorRotation;
            Vector3Int offsetToMoveInDirRaw = PlayerMovementManager.GetOffsetToMoveInDirRaw(actorRotation);
            GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            if (Mouse.Over(rect2))
            {
                Get.Tooltips.RegisterTip(rect2, "CompassTip".Translate(offsetToMoveInDirRaw.DirectionToString()).FormattedKeyBindings(), null);
            }
            if (Math.Abs(actorRotation - Compass.cachedForCameraRot) > 0.15f || Compass.cachedForOffsetToMoveInDirRaw != offsetToMoveInDirRaw)
            {
                Compass.cachedForCameraRot = actorRotation;
                Compass.cachedForOffsetToMoveInDirRaw = offsetToMoveInDirRaw;
                CachedGUI.SetDirty(0);
            }
            if (CachedGUI.BeginCachedGUI(rect2, 0, true))
            {
                Compass.DrawCompass(rect);
            }
            CachedGUI.EndCachedGUI(1f, 1f);
            Compass.DrawObjectiveMarker(rect);
            if (Event.current.type == EventType.Repaint)
            {
                if (Get.CellsInfo.AnyLadderAt(Get.NowControlledActor.Position) || (Get.NowControlledActor.Position.Below().InBounds() && Get.CellsInfo.AnyLadderAt(Get.NowControlledActor.Position.Below()) && Get.CellsInfo.CanPassThrough(Get.NowControlledActor.Position.Below())))
                {
                    Compass.moveUpDownIconsAlpha = Math.Min(Compass.moveUpDownIconsAlpha + Clock.UnscaledDeltaTime * 6f, 1f);
                }
                else
                {
                    Compass.moveUpDownIconsAlpha = Math.Max(Compass.moveUpDownIconsAlpha - Clock.UnscaledDeltaTime * 6f, 0f);
                }
            }
            if (Compass.moveUpDownIconsAlpha > 0f)
            {
                float num = ((ControllerUtility.InControllerMode && ControllerUtility.ControllerType != ControllerType.SteamDeck) ? 167f : 102f);
                Rect rect3 = rect.MovedBy(-num, 0f).TopHalf().CutFromBottom(4f)
                    .BottomPart(23f);
                Rect rect4 = rect.MovedBy(-num, 0f).BottomHalf().CutFromTop(4f)
                    .TopPart(23f);
                rect3.width = 999f;
                rect4.width = 999f;
                GUI.color = new Color(0.9f, 0.9f, 0.9f, Compass.moveUpDownIconsAlpha);
                Widgets.Align = TextAnchor.MiddleLeft;
                GUIExtra.DrawTextureRotated(rect3.LeftPart(rect3.height), Compass.ArrowTex, 180f, null);
                Widgets.Label(rect3.CutFromLeft(rect3.height), "[Fly]+[MoveForward]".FormattedKeyBindings(), true, null, null, false);
                GUI.DrawTexture(rect4.LeftPart(rect4.height), Compass.ArrowTex);
                Widgets.Label(rect4.CutFromLeft(rect3.height), "[Fly]+[MoveBack]".FormattedKeyBindings(), true, null, null, false);
                Widgets.ResetAlign();
                GUI.color = Color.white;
            }
        }

        private static void DrawCompass(Rect rect)
        {
            float actorRotation = Get.FPPControllerGOC.ActorRotation;
            GUIExtra.DrawTextureRotated(rect, Compass.CompassTex, -actorRotation, null);
            Compass.DrawCompassLabels(rect);
        }

        private static void DrawCompassLabels(Rect rect)
        {
            float actorRotation = Get.FPPControllerGOC.ActorRotation;
            Vector3Int offsetToMoveInDirRaw = PlayerMovementManager.GetOffsetToMoveInDirRaw(actorRotation);
            Compass.DrawLabel(actorRotation, "N", rect, true, offsetToMoveInDirRaw == new Vector3Int(0, 0, 1));
            Compass.DrawLabel(actorRotation - 45f, "NE", rect, false, offsetToMoveInDirRaw == new Vector3Int(1, 0, 1));
            Compass.DrawLabel(actorRotation - 90f, "E", rect, true, offsetToMoveInDirRaw == new Vector3Int(1, 0, 0));
            Compass.DrawLabel(actorRotation - 135f, "SE", rect, false, offsetToMoveInDirRaw == new Vector3Int(1, 0, -1));
            Compass.DrawLabel(actorRotation - 180f, "S", rect, true, offsetToMoveInDirRaw == new Vector3Int(0, 0, -1));
            Compass.DrawLabel(actorRotation - 225f, "SW", rect, false, offsetToMoveInDirRaw == new Vector3Int(-1, 0, -1));
            Compass.DrawLabel(actorRotation - 270f, "W", rect, true, offsetToMoveInDirRaw == new Vector3Int(-1, 0, 0));
            Compass.DrawLabel(actorRotation - 315f, "NW", rect, false, offsetToMoveInDirRaw == new Vector3Int(-1, 0, 1));
        }

        private static void DrawLabel(float angle, string label, Rect compassRect, bool major, bool active)
        {
            float num = (major ? 0.5f : 0.4f);
            Vector2 vector = compassRect.center + new Vector2(Calc.Cos((-angle - 90f) * 0.017453292f), Calc.Sin((-angle - 90f) * 0.017453292f)) * compassRect.height * num;
            Widgets.FontBold = active;
            Widgets.FontSizeScalable = (major ? 14 : 12);
            GUI.color = (active ? Color.white : new Color(0.4f, 0.4f, 0.4f));
            Widgets.LabelCentered(vector, label, true, null, null, false, false, false, null);
            Widgets.ResetFontSize();
            Widgets.FontBold = false;
            GUI.color = Color.white;
        }

        private static void DrawObjectiveMarker(Rect rect)
        {
            Vector3Int? vector3Int = null;
            if (Get.World.AnyEntityOfSpec(Get.Entity_Lever))
            {
                Entity entity = Get.World.GetEntitiesOfSpec(Get.Entity_Lever)[0];
                RetainedRoomInfo.RoomInfo firstRoomWithRole = Get.RetainedRoomInfo.GetFirstRoomWithRole(Room.LayoutRole.LeverRoom);
                if ((firstRoomWithRole != null && firstRoomWithRole.EverVisitedOrKnown) || Get.FogOfWar.IsUnfogged(entity.Position))
                {
                    vector3Int = new Vector3Int?(entity.Position);
                }
            }
            else if (Get.World.AnyEntityOfSpec(Get.Entity_Staircase))
            {
                Entity entity2 = Get.World.GetEntitiesOfSpec(Get.Entity_Staircase)[0];
                RetainedRoomInfo.RoomInfo firstRoomWithRole2 = Get.RetainedRoomInfo.GetFirstRoomWithRole(Room.LayoutRole.End);
                if ((firstRoomWithRole2 != null && firstRoomWithRole2.EverVisitedOrKnown) || Get.FogOfWar.IsUnfogged(entity2.Position))
                {
                    vector3Int = new Vector3Int?(entity2.Position);
                }
            }
            else if (Get.World.AnyEntityOfSpec(Get.Entity_RunEndPortal))
            {
                Entity entity3 = Get.World.GetEntitiesOfSpec(Get.Entity_RunEndPortal)[0];
                RetainedRoomInfo.RoomInfo firstRoomWithRole3 = Get.RetainedRoomInfo.GetFirstRoomWithRole(Room.LayoutRole.End);
                if ((firstRoomWithRole3 != null && firstRoomWithRole3.EverVisitedOrKnown) || Get.FogOfWar.IsUnfogged(entity3.Position))
                {
                    vector3Int = new Vector3Int?(entity3.Position);
                }
            }
            if (vector3Int == null)
            {
                return;
            }
            float angleXZFromCamera_Simple = Vector3Utility.GetAngleXZFromCamera_Simple(vector3Int.Value);
            Vector2 vector = rect.center + Vector2Utility.XZAngleToDirectionalVector(angleXZFromCamera_Simple) * rect.width * 0.61f;
            GUI.color = new Color(1f, 1f, 1f, 0.3f);
            GUI.DrawTexture(RectUtility.CenteredAt(vector, 10f), Compass.ObjectiveTex);
            GUI.color = Color.white;
        }

        private static float cachedForCameraRot = -1f;

        private static Vector3Int cachedForOffsetToMoveInDirRaw;

        private static float moveUpDownIconsAlpha;

        private static readonly Texture2D CompassTex = Assets.Get<Texture2D>("Textures/UI/Compass");

        private static readonly Texture2D ArrowTex = Assets.Get<Texture2D>("Textures/UI/Arrow");

        private static readonly Texture2D ObjectiveTex = Assets.Get<Texture2D>("Textures/UI/Objective");

        private const float Size = 130f;

        private static readonly Vector2 Offset = new Vector2(30f, 60f);

        private const float RecacheCameraRotEps = 0.15f;
    }
}