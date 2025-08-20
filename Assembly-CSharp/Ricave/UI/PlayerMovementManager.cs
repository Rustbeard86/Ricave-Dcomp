using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class PlayerMovementManager
    {
        public void Update()
        {
            this.UpdateCurrentDirectionArrow();
            if (this.keepRotatingToFace != null && Clock.Time < this.keepRotatingToFaceUntil && this.keepRotatingToFace.Spawned && this.keepRotatingToFace.Position != Get.NowControlledActor.Position)
            {
                Get.FPPControllerGOC.RotateToFace(this.keepRotatingToFace.RenderPositionComputedCenter);
            }
        }

        private void UpdateCurrentDirectionArrow()
        {
            if (this.arrow == null)
            {
                this.arrow = Object.Instantiate<GameObject>(PlayerMovementManager.ArrowPrefab, Get.RuntimeSpecialContainer.transform);
                this.arrow.SetActive(false);
            }
            if (Get.UI.WorldInputBlocked || DebugUI.HideUI)
            {
                this.arrow.SetActive(false);
                return;
            }
            float actorTargetRotation = Get.FPPControllerGOC.ActorTargetRotation;
            Vector3Int position = Get.NowControlledActor.Position;
            Vector3Int offsetToMoveInDir = this.GetOffsetToMoveInDir(actorTargetRotation);
            if (Get.World.CanMoveFromTo(Get.NowControlledActor.Position, Get.NowControlledActor.Position + offsetToMoveInDir, Get.NowControlledActor))
            {
                float rotFromOffset = this.GetRotFromOffset(offsetToMoveInDir);
                Vector3 vector = new Vector3((float)position.x + Calc.Cos((-rotFromOffset + 90f) * 0.017453292f) * 0.5f, (float)position.y + -0.36f, (float)position.z + Calc.Sin((-rotFromOffset + 90f) * 0.017453292f) * 0.5f);
                Transform transform = this.arrow.transform;
                transform.position = vector;
                transform.rotation = Quaternion.Euler(0f, rotFromOffset, 0f);
                transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                this.arrow.SetActive(true);
                return;
            }
            this.arrow.SetActive(false);
        }

        public void OnGUI()
        {
            if (!Get.UI.WorldInputBlocked)
            {
                float actorTargetRotation = Get.FPPControllerGOC.ActorTargetRotation;
                bool flyKeyModifierHeldDown = KeyCodeUtility.FlyKeyModifierHeldDown;
                bool flag = Get.LessonManager.CurrentLesson == Get.Lesson_Moving;
                if (!Get.KeyBinding_StationaryActionsOnly.HeldDown)
                {
                    if (Get.KeyBinding_MoveForward.JustPressed || Get.KeyBinding_MoveForwardAlt.JustPressed)
                    {
                        if (flyKeyModifierHeldDown)
                        {
                            this.TryChangeAltitude(1);
                        }
                        else if (this.TryMoveInDir(actorTargetRotation) && ControllerUtility.InControllerMode && !this.TryRotateToTargetInFront(true, false) && Get.CellsInfo.AnyLadderAt(Get.NowControlledActor.Position))
                        {
                            Vector3Int offsetToMoveInDirRaw = PlayerMovementManager.GetOffsetToMoveInDirRaw(actorTargetRotation);
                            Vector3Int vector3Int = Get.NowControlledActor.Position + offsetToMoveInDirRaw;
                            if (!vector3Int.InBounds() || !Get.CellsInfo.CanPassThroughNoActors(vector3Int))
                            {
                                if (Get.NowControlledActor.Position.Above().InBounds() && Get.CellsInfo.CanPassThroughNoActors(Get.NowControlledActor.Position.Above()))
                                {
                                    Get.FPPControllerGOC.SetCameraTargetRotation(-50f);
                                    this.keepRotatingToFace = null;
                                }
                                else if (Get.NowControlledActor.Position.Below().InBounds() && Get.CellsInfo.CanPassThroughNoActors(Get.NowControlledActor.Position.Below()) && Get.CellsInfo.AnyLadderAt(Get.NowControlledActor.Position.Below()))
                                {
                                    Get.FPPControllerGOC.SetCameraTargetRotation(50f);
                                    this.keepRotatingToFace = null;
                                }
                            }
                        }
                    }
                    else if (Get.KeyBinding_MoveBack.JustPressed || Get.KeyBinding_MoveBackAlt.JustPressed)
                    {
                        if (flyKeyModifierHeldDown)
                        {
                            this.TryChangeAltitude(-1);
                        }
                        else
                        {
                            this.TryMoveInDir(actorTargetRotation + 180f);
                        }
                    }
                    else if (Get.KeyBinding_MoveLeft.JustPressed || Get.KeyBinding_MoveLeftAlt.JustPressed)
                    {
                        if (this.TryMoveInDir(actorTargetRotation - 90f) && !flag)
                        {
                            Get.LessonManager.FinishIfCurrent(Get.Lesson_Strafing);
                        }
                    }
                    else if ((Get.KeyBinding_MoveRight.JustPressed || Get.KeyBinding_MoveRightAlt.JustPressed) && this.TryMoveInDir(actorTargetRotation + 90f) && !flag)
                    {
                        Get.LessonManager.FinishIfCurrent(Get.Lesson_Strafing);
                    }
                }
                if (Get.KeyBinding_Wait.JustPressed)
                {
                    if (Get.PlannedPlayerActions.ShouldStopOnSpaceKeyPressed)
                    {
                        Get.PlannedPlayerActions.Stop();
                    }
                    else
                    {
                        this.TryWait();
                    }
                }
                else if (Get.KeyBinding_Rewind.JustPressed)
                {
                    this.TryRewind();
                }
                if (!Get.KeyBinding_StationaryActionsOnly.HeldDown)
                {
                    if (Get.KeyBinding_RotateLeft.JustPressed || Get.KeyBinding_RotateLeftAlt.JustPressed)
                    {
                        Get.Sound_Rotation.PlayOneShot(null, 1f, 1f);
                        Get.FPPControllerGOC.SetActorTargetRotation(this.GetRotSnappedToNextDir(false));
                        this.< OnGUI > g__AfterRotating | 10_0();
                        Get.LessonManager.FinishIfCurrent(Get.Lesson_Rotating);
                    }
                    if (Get.KeyBinding_RotateRight.JustPressed || Get.KeyBinding_RotateRightAlt.JustPressed)
                    {
                        Get.Sound_Rotation.PlayOneShot(null, 1f, 1f);
                        Get.FPPControllerGOC.SetActorTargetRotation(this.GetRotSnappedToNextDir(true));
                        this.< OnGUI > g__AfterRotating | 10_0();
                        Get.LessonManager.FinishIfCurrent(Get.Lesson_Rotating);
                    }
                }
                if (Get.KeyBinding_SnapToCurrentRotation.JustPressed)
                {
                    Get.Sound_Rotation.PlayOneShot(null, 1f, 1f);
                    Get.FPPControllerGOC.SetActorTargetRotation(PlayerMovementManager.GetRotSnappedToCurDir());
                    Get.FPPControllerGOC.SetCameraTargetRotation(0f);
                    this.keepRotatingToFace = null;
                }
                if (Get.KeyBinding_RotateToNearestTarget.JustPressed)
                {
                    Entity nearestTargetToMouse = this.GetNearestTargetToMouse();
                    if (nearestTargetToMouse != null)
                    {
                        Get.Sound_Rotation.PlayOneShot(null, 1f, 1f);
                        Get.FPPControllerGOC.RotateToFace(nearestTargetToMouse.RenderPositionComputedCenter);
                    }
                    this.keepRotatingToFace = null;
                }
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.DoWaitingLabel();
                this.DoPlayerForcedActionLabel();
            }
        }

        private void TryWait()
        {
            if (ActionViaInterfaceHelper.TryDo(() => new Action_Wait(Get.Action_Wait, Get.NowControlledActor, null, null)))
            {
                this.lastWaitTime = Clock.Time;
                Get.LessonManager.FinishIfCurrent(Get.Lesson_Waiting);
                if (ControllerUtility.InControllerMode)
                {
                    this.TryRotateToTargetInFront(true, true);
                }
            }
        }

        private void TryRewind()
        {
            if (!Get.Player.HasWatch)
            {
                return;
            }
            if (Get.Player.TurnsCanRewind <= 0)
            {
                Get.Sound_CantRewindTime.PlayOneShot(null, 1f, 1f);
                return;
            }
            Get.TurnManager.TryDoAllNextForcedOrNonPlayerActionsNow();
            int toRewind = 0;
            List<Action> recentActions = Get.TurnManager.RecentActions;
            for (int i = recentActions.Count - 1; i >= 0; i--)
            {
                if (recentActions[i].IsRewindPoint)
                {
                    int toRewind2 = toRewind;
                    toRewind = toRewind2 + 1;
                    if (!recentActions[i].WasForced)
                    {
                        break;
                    }
                }
            }
            toRewind = Math.Min(toRewind, Get.Player.TurnsCanRewind);
            if (toRewind > 0)
            {
                Get.PlannedPlayerActions.Stop();
                ActionViaInterfaceHelper.TryDo(() => new Action_RewindTime(Get.Action_RewindTime, toRewind));
            }
        }

        private void DoWaitingLabel()
        {
            float num;
            if (Get.PlannedPlayerActions.LeftWaitActions > 0)
            {
                num = 0.62f;
            }
            else
            {
                num = Calc.ResolveFadeInStayOut(Clock.Time - this.lastWaitTime, 0.1f, 0f, 0.42999998f) * 0.62f;
            }
            if (num > 0f)
            {
                GUI.color = new Color(1f, 1f, 1f, num);
                Widgets.FontSizeScalable = 24;
                Widgets.LabelCentered(Widgets.ScreenCenter + new Vector2(0f, 55f), "Waiting".Translate(), true, null, null, false, false, false, null);
                Widgets.ResetFontSize();
                GUI.color = Color.white;
            }
        }

        private void DoPlayerForcedActionLabel()
        {
            string forcedActionReasonLabel = ForcedActionsHelper.GetForcedActionReasonLabel(Get.NowControlledActor);
            if (forcedActionReasonLabel != null)
            {
                Widgets.FontSizeScalable = 24;
                Widgets.LabelCentered(Widgets.ScreenCenter + new Vector2(0f, 55f), forcedActionReasonLabel, true, null, null, false, false, false, null);
                Widgets.ResetFontSize();
            }
        }

        private float GetRotSnappedToNextDir(bool clockwise)
        {
            return Calc.Round(Get.FPPControllerGOC.ActorTargetRotation / 45f) * 45f + (clockwise ? 45f : (-45f)) + (clockwise ? (-0.02f) : 0.02f);
        }

        public static float GetRotSnappedToCurDir()
        {
            return Calc.Round(Get.FPPControllerGOC.ActorTargetRotation / 45f) * 45f;
        }

        private float GetRotFromOffset(Vector3Int offset)
        {
            if (offset.x == 0 && offset.z == 1)
            {
                return 0f;
            }
            if (offset.x == 1 && offset.z == 1)
            {
                return 45f;
            }
            if (offset.x == 1 && offset.z == 0)
            {
                return 90f;
            }
            if (offset.x == 1 && offset.z == -1)
            {
                return 135f;
            }
            if (offset.x == 0 && offset.z == -1)
            {
                return 180f;
            }
            if (offset.x == -1 && offset.z == -1)
            {
                return 225f;
            }
            if (offset.x == -1 && offset.z == 0)
            {
                return 270f;
            }
            if (offset.x == -1 && offset.z == 1)
            {
                return 315f;
            }
            return 0f;
        }

        private Vector3Int GetOffsetToMoveInDir(float dir)
        {
            PlayerMovementManager.<> c__DisplayClass18_0 CS$<> 8__locals1;
            CS$<> 8__locals1.world = Get.World;
            CS$<> 8__locals1.cellsInfo = Get.CellsInfo;
            CS$<> 8__locals1.playerActor = Get.NowControlledActor;
            CS$<> 8__locals1.playerPos = Get.NowControlledActor.Position;
            dir = Calc.NormalizeDir(dir);
            Vector3Int offsetToMoveInDirRaw = PlayerMovementManager.GetOffsetToMoveInDirRaw(dir);
            CS$<> 8__locals1.preferUsingStairs = (!CS$<> 8__locals1.playerActor.CanFly || !KeyCodeUtility.FlyKeyModifierHeldDown) && (CS$<> 8__locals1.cellsInfo.IsFloorUnder(CS$<> 8__locals1.playerPos, CS$<> 8__locals1.playerActor.Gravity) || (CS$<> 8__locals1.playerActor.CanUseLadders && CS$<> 8__locals1.cellsInfo.IsLadderUnder(CS$<> 8__locals1.playerPos) && !CS$<> 8__locals1.cellsInfo.AnyLadderAt(CS$<> 8__locals1.playerPos)));
            Vector3Int vector3Int;
            if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(offsetToMoveInDirRaw, out vector3Int, ref CS$<> 8__locals1))
            {
                return vector3Int;
            }
            if (offsetToMoveInDirRaw.IsDiagonalDir())
            {
                if (dir >= 22.5f && dir <= 67.5f)
                {
                    if (dir < 45f)
                    {
                        Vector3Int vector3Int2;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(0, 0, 1)), out vector3Int2, ref CS$<> 8__locals1))
                        {
                            return vector3Int2;
                        }
                        Vector3Int vector3Int3;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(1, 0, 0)), out vector3Int3, ref CS$<> 8__locals1))
                        {
                            return vector3Int3;
                        }
                    }
                    else
                    {
                        Vector3Int vector3Int4;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(1, 0, 0)), out vector3Int4, ref CS$<> 8__locals1))
                        {
                            return vector3Int4;
                        }
                        Vector3Int vector3Int5;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(0, 0, 1)), out vector3Int5, ref CS$<> 8__locals1))
                        {
                            return vector3Int5;
                        }
                    }
                }
                else if (dir >= 112.5f && dir <= 157.5f)
                {
                    if (dir < 135f)
                    {
                        Vector3Int vector3Int6;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(1, 0, 0)), out vector3Int6, ref CS$<> 8__locals1))
                        {
                            return vector3Int6;
                        }
                        Vector3Int vector3Int7;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(0, 0, -1)), out vector3Int7, ref CS$<> 8__locals1))
                        {
                            return vector3Int7;
                        }
                    }
                    else
                    {
                        Vector3Int vector3Int8;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(0, 0, -1)), out vector3Int8, ref CS$<> 8__locals1))
                        {
                            return vector3Int8;
                        }
                        Vector3Int vector3Int9;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(1, 0, 0)), out vector3Int9, ref CS$<> 8__locals1))
                        {
                            return vector3Int9;
                        }
                    }
                }
                else if (dir >= 202.5f && dir <= 247.5f)
                {
                    if (dir < 225f)
                    {
                        Vector3Int vector3Int10;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(0, 0, -1)), out vector3Int10, ref CS$<> 8__locals1))
                        {
                            return vector3Int10;
                        }
                        Vector3Int vector3Int11;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(-1, 0, 0)), out vector3Int11, ref CS$<> 8__locals1))
                        {
                            return vector3Int11;
                        }
                    }
                    else
                    {
                        Vector3Int vector3Int12;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(-1, 0, 0)), out vector3Int12, ref CS$<> 8__locals1))
                        {
                            return vector3Int12;
                        }
                        Vector3Int vector3Int13;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(0, 0, -1)), out vector3Int13, ref CS$<> 8__locals1))
                        {
                            return vector3Int13;
                        }
                    }
                }
                else if (dir <= 337.5f)
                {
                    if (dir < 315f)
                    {
                        Vector3Int vector3Int14;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(-1, 0, 0)), out vector3Int14, ref CS$<> 8__locals1))
                        {
                            return vector3Int14;
                        }
                        Vector3Int vector3Int15;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(0, 0, 1)), out vector3Int15, ref CS$<> 8__locals1))
                        {
                            return vector3Int15;
                        }
                    }
                    else
                    {
                        Vector3Int vector3Int16;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(0, 0, 1)), out vector3Int16, ref CS$<> 8__locals1))
                        {
                            return vector3Int16;
                        }
                        Vector3Int vector3Int17;
                        if (PlayerMovementManager.< GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(PlayerMovementManager.ConvertToGravityLocal(new Vector3Int(-1, 0, 0)), out vector3Int17, ref CS$<> 8__locals1))
                        {
                            return vector3Int17;
                        }
                    }
                }
            }
            return default(Vector3Int);
        }

        public static Vector3Int GetOffsetToMoveInDirRaw(float dir)
        {
            dir = Calc.NormalizeDir(dir);
            Vector3Int vector3Int;
            if (dir < 22.5f)
            {
                vector3Int = new Vector3Int(0, 0, 1);
            }
            else if (dir < 67.5f)
            {
                vector3Int = new Vector3Int(1, 0, 1);
            }
            else if (dir < 112.5f)
            {
                vector3Int = new Vector3Int(1, 0, 0);
            }
            else if (dir < 157.5f)
            {
                vector3Int = new Vector3Int(1, 0, -1);
            }
            else if (dir < 202.5f)
            {
                vector3Int = new Vector3Int(0, 0, -1);
            }
            else if (dir < 247.5f)
            {
                vector3Int = new Vector3Int(-1, 0, -1);
            }
            else if (dir < 292.5f)
            {
                vector3Int = new Vector3Int(-1, 0, 0);
            }
            else if (dir < 337.5f)
            {
                vector3Int = new Vector3Int(-1, 0, 1);
            }
            else
            {
                vector3Int = new Vector3Int(0, 0, 1);
            }
            return PlayerMovementManager.ConvertToGravityLocal(vector3Int);
        }

        private static Vector3Int ConvertToGravityLocal(Vector3Int dirAssumeGravDown)
        {
            Vector3Int cameraGravity = Get.FPPControllerGOC.CameraGravity;
            if (cameraGravity.y == -1)
            {
                return dirAssumeGravDown;
            }
            if (cameraGravity.y == 1)
            {
                return new Vector3Int(dirAssumeGravDown.x, dirAssumeGravDown.y, -dirAssumeGravDown.z);
            }
            if (cameraGravity.x == -1)
            {
                return new Vector3Int(dirAssumeGravDown.y, -dirAssumeGravDown.x, dirAssumeGravDown.z);
            }
            if (cameraGravity.x == 1)
            {
                return new Vector3Int(-dirAssumeGravDown.y, dirAssumeGravDown.x, dirAssumeGravDown.z);
            }
            if (cameraGravity.z == -1)
            {
                return new Vector3Int(dirAssumeGravDown.x, -dirAssumeGravDown.z, dirAssumeGravDown.y);
            }
            if (cameraGravity.z == 1)
            {
                return new Vector3Int(dirAssumeGravDown.x, dirAssumeGravDown.z, dirAssumeGravDown.y);
            }
            return dirAssumeGravDown;
        }

        private void TryChangeAltitude(int diff)
        {
            Vector3Int vector3Int = Get.NowControlledActor.Gravity * -diff;
            if (this.< TryChangeAltitude > g__TryGo | 21_0(vector3Int))
            {
                return;
            }
            float actorTargetRotation = Get.FPPControllerGOC.ActorTargetRotation;
            Vector3Int vector3Int2 = PlayerMovementManager.GetOffsetToMoveInDirRaw(actorTargetRotation);
            vector3Int2 += vector3Int;
            if (this.< TryChangeAltitude > g__TryGo | 21_0(vector3Int2))
            {
                return;
            }
            Vector3Int vector3Int3 = this.GetOffsetToMoveInDir(actorTargetRotation);
            vector3Int3 += vector3Int;
            if (this.< TryChangeAltitude > g__TryGo | 21_0(vector3Int3))
            {
                return;
            }
            for (int i = 0; i < Vector3IntUtility.DirectionsXZCardinal.Length; i++)
            {
                Vector3Int vector3Int4 = Vector3IntUtility.DirectionsXZCardinal[i];
                vector3Int4 += vector3Int;
                if (this.< TryChangeAltitude > g__TryGo | 21_0(vector3Int4))
                {
                    return;
                }
            }
            for (int j = 0; j < Vector3IntUtility.DirectionsXZDiagonal.Length; j++)
            {
                Vector3Int vector3Int5 = Vector3IntUtility.DirectionsXZDiagonal[j];
                vector3Int5 += vector3Int;
                if (this.< TryChangeAltitude > g__TryGo | 21_0(vector3Int5))
                {
                    return;
                }
            }
            if (Get.NowControlledActor.CanFly)
            {
                Get.CameraEffects.BumpIntoObstacle(vector3Int);
            }
        }

        private bool TryMoveInDir(float dir)
        {
            if (!this.TryMoveBy(this.GetOffsetToMoveInDir(dir)))
            {
                Get.CameraEffects.BumpIntoObstacle(PlayerMovementManager.GetOffsetToMoveInDirRaw(dir));
                return false;
            }
            Get.LessonManager.FinishIfCurrent(Get.Lesson_Moving);
            return true;
        }

        private bool TryMoveBy(Vector3Int offset)
        {
            PlayerMovementManager.<> c__DisplayClass23_0 CS$<> 8__locals1 = new PlayerMovementManager.<> c__DisplayClass23_0();
            CS$<> 8__locals1.offset = offset;
            if (!Get.TurnManager.CanDoActionsAtAllNow)
            {
                return false;
            }
            if (!CS$<> 8__locals1.< TryMoveBy > g__CanMove | 0())
			{
                return false;
            }
            Get.TurnManager.TryDoAllNextForcedOrNonPlayerActionsNow();
            if (!Get.TurnManager.IsPlayerTurn_CanChooseNextAction || !CS$<> 8__locals1.< TryMoveBy > g__CanMove | 0())
			{
                return false;
            }
            if (!Get.TurnManager.CanDoActionsAtAllNow)
            {
                return false;
            }
            Get.PlannedPlayerActions.Stop();
            if (this.WillFallIntoDescendTriggerAt(Get.NowControlledActor.Position + CS$<> 8__locals1.offset))
            {
                Get.WindowManager.OpenConfirmationWindow("FallIntoDescendTriggerConfirmation".Translate(), delegate
                {
                    if (Get.TurnManager.IsPlayerTurn_CanChooseNextAction && base.< TryMoveBy > g__CanMove | 0() && Get.TurnManager.CanDoActionsAtAllNow)
                    {
                        new Action_MoveSelf(Get.Action_MoveSelf, Get.NowControlledActor, Get.NowControlledActor.Position, Get.NowControlledActor.Position + CS$<> 8__locals1.offset, null, false).Do(false);
                    }
                }, false, null, null);
            }
            else if (this.WillMoveIntoRunEndPortal(Get.NowControlledActor.Position + CS$<> 8__locals1.offset))
            {
                Get.WindowManager.OpenConfirmationWindow("MoveIntoRunEndPortalConfirmation".Translate(), delegate
                {
                    if (Get.TurnManager.IsPlayerTurn_CanChooseNextAction && base.< TryMoveBy > g__CanMove | 0() && Get.TurnManager.CanDoActionsAtAllNow)
                    {
                        new Action_MoveSelf(Get.Action_MoveSelf, Get.NowControlledActor, Get.NowControlledActor.Position, Get.NowControlledActor.Position + CS$<> 8__locals1.offset, null, false).Do(false);
                    }
                }, false, null, null);
            }
            else
            {
                new Action_MoveSelf(Get.Action_MoveSelf, Get.NowControlledActor, Get.NowControlledActor.Position, Get.NowControlledActor.Position + CS$<> 8__locals1.offset, null, false).Do(false);
            }
            return true;
        }

        public Entity GetNearestTargetToMouse()
        {
            Entity targetInFrontToRotateTo = this.GetTargetInFrontToRotateTo(true, false);
            if (targetInFrontToRotateTo != null)
            {
                return targetInFrontToRotateTo;
            }
            using (IEnumerator<Entity> enumerator = (from x in Get.VisibilityCache.EntitiesSeen_Unordered
                                                     where this.IsTargetToAutoRotateTo(x)
                                                     orderby x != Get.InteractionManager.HighlightedEntity, Calc.SphericalDistance(Get.CameraTransform.forward, (x.RenderPositionComputedCenter - Get.CameraPosition).normalized)
                                                     select x).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }
            return null;
        }

        private bool WillFallIntoDescendTriggerAt(Vector3Int at)
        {
            if (!at.InBounds())
            {
                return false;
            }
            using (List<Entity>.Enumerator enumerator = Get.World.GetEntitiesAt(at).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (PlayerMovementManager.< WillFallIntoDescendTriggerAt > g__IsDescendTrigger | 25_0(enumerator.Current))
                    {
                        return true;
                    }
                }
            }
            Vector3Int vector3Int = at;
            while (Get.CellsInfo.IsFallingAt(vector3Int, Get.NowControlledActor, true))
            {
                vector3Int += Get.NowControlledActor.Gravity;
                if (!vector3Int.InBounds())
                {
                    break;
                }
                using (List<Entity>.Enumerator enumerator = Get.World.GetEntitiesAt(vector3Int).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (PlayerMovementManager.< WillFallIntoDescendTriggerAt > g__IsDescendTrigger | 25_0(enumerator.Current))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool WillMoveIntoRunEndPortal(Vector3Int at)
        {
            return at.InBounds() && Get.World.AnyEntityOfSpecAt(at, Get.Entity_RunEndPortal);
        }

        private bool IsTargetToAutoRotateTo(Entity entity)
        {
            if (!entity.Spawned)
            {
                return false;
            }
            if (entity.IsNowControlledActor)
            {
                return false;
            }
            if (entity.Spec == Get.Entity_SecretPassage || entity.Spec == Get.Entity_WallWithCompartment)
            {
                return false;
            }
            if (entity.Spec.IsStructure && entity.Spec.Structure.IsWater)
            {
                return false;
            }
            if (entity.Spec == Get.Entity_Torch || entity.Spec == Get.Entity_VioletTorch || entity.Spec == Get.Entity_PressurePlate || entity.Spec == Get.Entity_CeilingSupport || entity.Spec == Get.Entity_CeilingSpikes)
            {
                return false;
            }
            Actor actor = entity as Actor;
            if (actor != null && actor.Faction == Get.NowControlledActor.Faction && !actor.IsHostile(Get.NowControlledActor) && actor.UsableOnTalk == null)
            {
                return false;
            }
            InteractionManager.PossibleInteraction? possibleInteraction;
            if (!SeenEntitiesDrawer.ShouldList(entity) && entity.MaxHP == 0 && (!entity.EntityGOC.HasAnyNonInspectModeOnlyCollider || Get.InteractionManager.GetInteraction(entity, Vector3IntUtility.Forward, null) == null || !possibleInteraction.GetValueOrDefault().highlightEntity))
            {
                return false;
            }
            if (entity.Spec != Get.Entity_Staircase && entity.Spec != Get.Entity_NewRunStaircase && entity.Spec != Get.Entity_TrainingRoomStaircase && entity.Spec != Get.Entity_NewRunWithSeedStaircase)
            {
                Vector3Int vector3Int;
                Vector3 vector;
                GameObject gameObject = RaycastUtility.Raycast(Get.CameraPosition, (entity.RenderPositionComputedCenter - Get.CameraPosition).normalized, 8f, out vector3Int, out vector, false, true, false, true, null);
                if (gameObject == null || gameObject.GetComponentInParent<EntityGOC>() != entity.EntityGOC)
                {
                    return false;
                }
            }
            return true;
        }

        private bool TryRotateToTargetInFront(bool allowCurrentlyHighlightedEntity = true, bool actorsOnly = false)
        {
            Entity targetInFrontToRotateTo = this.GetTargetInFrontToRotateTo(allowCurrentlyHighlightedEntity, actorsOnly);
            if (targetInFrontToRotateTo != null)
            {
                Get.FPPControllerGOC.RotateToFace(targetInFrontToRotateTo.RenderPositionComputedCenter);
                this.keepRotatingToFace = targetInFrontToRotateTo;
                this.keepRotatingToFaceUntil = Clock.Time + 0.21f;
                return true;
            }
            return false;
        }

        private Entity GetTargetInFrontToRotateTo(bool allowCurrentlyHighlightedEntity = true, bool actorsOnly = false)
        {
            Vector3Int offsetToMoveInDirRaw = PlayerMovementManager.GetOffsetToMoveInDirRaw(Get.FPPControllerGOC.ActorTargetRotation);
            Vector3Int vector3Int = Get.NowControlledActor.Position + offsetToMoveInDirRaw;
            if (!vector3Int.InBounds())
            {
                return null;
            }
            this.tmpTargetsInFront.Clear();
            for (int i = -1; i <= 1; i++)
            {
                Vector3Int vector3Int2 = vector3Int.WithAddedY(i);
                if (vector3Int2.InBounds())
                {
                    foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int2))
                    {
                        if (!actorsOnly || entity is Actor)
                        {
                            if (entity == Get.InteractionManager.HighlightedEntity)
                            {
                                if (allowCurrentlyHighlightedEntity)
                                {
                                    return entity;
                                }
                            }
                            else if (this.IsTargetToAutoRotateTo(entity))
                            {
                                this.tmpTargetsInFront.Add(entity);
                            }
                        }
                    }
                }
            }
            if (this.tmpTargetsInFront.Count != 0)
            {
                Entity entity2;
                if (this.tmpTargetsInFront.TryGetMinBy<Entity, float>((Entity x) => Calc.SphericalDistance(Get.CameraTransform.forward, (x.RenderPositionComputedCenter - Get.CameraPosition).normalized), out entity2))
                {
                    return entity2;
                }
            }
            return null;
        }

        [CompilerGenerated]
        private void <OnGUI>g__AfterRotating|10_0()
		{
			if (ControllerUtility.InControllerMode && !this.TryRotateToTargetInFront(false, false))
			{
				Get.FPPControllerGOC.SetCameraTargetRotation(0f);
				this.keepRotatingToFace = null;
			}
}

[CompilerGenerated]
internal static bool < GetOffsetToMoveInDir > g__CheckCanMoveBy | 18_0(Vector3Int offsetArg, out Vector3Int adjustedOffset, ref PlayerMovementManager.<> c__DisplayClass18_0 A_2)

        {
    if (A_2.preferUsingStairs && A_2.cellsInfo.IsFloorUnder(A_2.playerPos + offsetArg - A_2.playerActor.Gravity, A_2.playerActor.Gravity) && A_2.world.CanMoveFromTo(A_2.playerPos, A_2.playerPos + offsetArg - A_2.playerActor.Gravity, A_2.playerActor))
    {
        adjustedOffset = offsetArg - A_2.playerActor.Gravity;
        return true;
    }
    if (A_2.preferUsingStairs && A_2.cellsInfo.IsFloorUnder(A_2.playerPos + offsetArg + A_2.playerActor.Gravity, A_2.playerActor.Gravity) && A_2.world.CanMoveFromTo(A_2.playerPos, A_2.playerPos + offsetArg + A_2.playerActor.Gravity, A_2.playerActor))
    {
        adjustedOffset = offsetArg + A_2.playerActor.Gravity;
        return true;
    }
    if (A_2.world.CanMoveFromTo(A_2.playerPos, A_2.playerPos + offsetArg, A_2.playerActor))
    {
        adjustedOffset = offsetArg;
        return true;
    }
    if (!A_2.preferUsingStairs && A_2.cellsInfo.IsFloorUnder(A_2.playerPos + offsetArg - A_2.playerActor.Gravity, A_2.playerActor.Gravity) && A_2.world.CanMoveFromTo(A_2.playerPos, A_2.playerPos + offsetArg - A_2.playerActor.Gravity, A_2.playerActor))
    {
        adjustedOffset = offsetArg - A_2.playerActor.Gravity;
        return true;
    }
    if (!A_2.preferUsingStairs && A_2.cellsInfo.IsFloorUnder(A_2.playerPos + offsetArg + A_2.playerActor.Gravity, A_2.playerActor.Gravity) && A_2.world.CanMoveFromTo(A_2.playerPos, A_2.playerPos + offsetArg + A_2.playerActor.Gravity, A_2.playerActor))
    {
        adjustedOffset = offsetArg + A_2.playerActor.Gravity;
        return true;
    }
    adjustedOffset = offsetArg;
    return false;
}

[CompilerGenerated]
private bool < TryChangeAltitude > g__TryGo | 21_0(Vector3Int offset)

        {
    if (Get.World.CanMoveFromTo(Get.NowControlledActor.Position, Get.NowControlledActor.Position + offset, Get.NowControlledActor))
    {
        if (this.TryMoveBy(offset))
        {
            Get.LessonManager.FinishIfCurrent(Get.Lesson_ClimbingLadders);
        }
        return true;
    }
    return false;
}

[CompilerGenerated]
internal static bool < WillFallIntoDescendTriggerAt > g__IsDescendTrigger | 25_0(Entity entity)

        {
    Structure structure = entity as Structure;
    if (structure == null)
    {
        return false;
    }
    if (!structure.Spec.Structure.AutoUseOnActorsPassing)
    {
        return false;
    }
    using (List<UseEffect>.Enumerator enumerator = structure.UseEffects.All.GetEnumerator())
    {
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is UseEffect_Descend)
            {
                return true;
            }
        }
    }
    return false;
}

private GameObject arrow;

private float lastWaitTime = -9999f;

private Entity keepRotatingToFace;

private float keepRotatingToFaceUntil;

private static readonly GameObject ArrowPrefab = Assets.Get<GameObject>("Prefabs/Misc/ArrowBig");

private const float ArrowYOffset = -0.36f;

private const float ArrowOffset = 0.5f;

private const float ArrowScale = 0.6f;

private List<Entity> tmpTargetsInFront = new List<Entity>();
	}
}