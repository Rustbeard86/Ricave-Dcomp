using System;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class FPPControllerGOC : MonoBehaviour
    {
        public Vector3 TargetPosition
        {
            get
            {
                return this.targetPosition;
            }
            set
            {
                if (this.targetPosition == value)
                {
                    return;
                }
                this.targetPosition = value;
            }
        }

        public float ActorRotation
        {
            get
            {
                return base.transform.localRotation.eulerAngles.y;
            }
        }

        public float ActorTargetRotation
        {
            get
            {
                return this.actorTargetRot.eulerAngles.y;
            }
        }

        public float CameraUpDownRotation
        {
            get
            {
                return this.mainCamera.transform.localRotation.eulerAngles.x;
            }
        }

        public float CameraTargetUpDownRotation
        {
            get
            {
                return this.cameraTargetRot.eulerAngles.x;
            }
        }

        public Vector3Int CameraGravity
        {
            get
            {
                if (Get.NowControlledActor == null)
                {
                    return Vector3Int.down;
                }
                return Get.NowControlledActor.Gravity;
            }
        }

        public bool CameraSwitchingBetweenActorsNow
        {
            get
            {
                return this.cameraSwitchingBetweenActorsNow;
            }
        }

        private float PositionLerpSpeed
        {
            get
            {
                if (Get.NowControlledActor == null)
                {
                    return 0.19f;
                }
                int sequencePerMove = Get.NowControlledActor.SequencePerMove;
                if (sequencePerMove == 12)
                {
                    return 0.19f;
                }
                if (sequencePerMove < 12)
                {
                    return 0.2337f;
                }
                if (Get.NowControlledActor.Limping)
                {
                    return 0.19f;
                }
                return 0.13870001f;
            }
        }

        private void Start()
        {
            this.mainCamera = Camera.main;
            if (Get.SavedCameraPosition.ActorTargetRot != null)
            {
                base.transform.localRotation = Get.SavedCameraPosition.ActorTargetRot.Value;
            }
            if (Get.SavedCameraPosition.CameraTargetRot != null)
            {
                this.mainCamera.transform.localRotation = Get.SavedCameraPosition.CameraTargetRot.Value;
            }
            this.SetActorTargetRotation(base.transform.localRotation);
            this.SetCameraTargetRotation(this.mainCamera.transform.localRotation);
        }

        private void Update()
        {
            this.ProcessMouseInput();
            Transform transform = base.transform;
            Vector3Int vector3Int;
            bool flag = this.WouldClipIntoAnyObjectAt(transform.position, out vector3Int);
            transform.position = this.PositionMoveTowards(transform.position, this.targetPosition);
            if (!flag && this.WouldClipIntoAnyObjectAt(transform.position, out vector3Int))
            {
                this.MoveOutOfClippedIntoObject();
            }
            transform.localRotation = QuaternionUtility.SlerpWithDeltaTime(transform.localRotation, this.actorTargetRot, 8.125945f);
            this.mainCamera.transform.localRotation = QuaternionUtility.SlerpWithDeltaTime(this.mainCamera.transform.localRotation, this.cameraTargetRot, 8.125945f);
            Get.PlayerGravity.transform.localRotation = QuaternionUtility.SlerpWithDeltaTime(Get.PlayerGravity.transform.localRotation, this.GetTargetGravityRot(), 8.125945f);
            if (this.cameraSwitchingBetweenActorsNow && this.lastSwitchedNowControlledActorFrame != Time.frameCount && (transform.position - this.targetPosition).sqrMagnitude < 0.25f)
            {
                this.cameraSwitchingBetweenActorsNow = false;
                if (Get.NowControlledActor.ActorGOC != null)
                {
                    Get.NowControlledActor.ActorGOC.UpdateMissingBodyPartsActiveStatus();
                }
            }
        }

        private Vector3 PositionMoveTowards(Vector3 from, Vector3 to)
        {
            if (this.cameraSwitchingBetweenActorsNow)
            {
                return Vector3Utility.LerpWithDeltaTime(from, to, Calc.ConvertConstantFixedUpdateLerpSpeedToNewLerpWithDeltaTime(0.2f));
            }
            if ((from - to).sqrMagnitude < 2.25f && !Get.CameraEffects.WalkingStaircase)
            {
                return Vector3.MoveTowards(from, to, this.PositionLerpSpeed * 20f * Clock.DeltaTime);
            }
            if ((from - to).sqrMagnitude < 6.25f && !Get.CameraEffects.WalkingStaircase)
            {
                return Vector3.MoveTowards(from, to, this.PositionLerpSpeed * 25f * Clock.DeltaTime);
            }
            return Vector3Utility.LerpWithDeltaTime(from, to, Calc.ConvertConstantFixedUpdateLerpSpeedToNewLerpWithDeltaTime(this.PositionLerpSpeed));
        }

        public void SetActorTargetRotation(float aroundY)
        {
            this.SetActorTargetRotation(Quaternion.Euler(0f, aroundY, 0f));
        }

        private void SetActorTargetRotation(Quaternion rot)
        {
            this.actorTargetRot = rot;
            Get.SavedCameraPosition.ActorTargetRot = new Quaternion?(this.actorTargetRot);
        }

        public void SetCameraTargetRotation(float aroundX)
        {
            this.SetCameraTargetRotation(Quaternion.Euler(aroundX, 0f, 0f));
        }

        private void SetCameraTargetRotation(Quaternion rot)
        {
            this.cameraTargetRot = rot;
            Get.SavedCameraPosition.CameraTargetRot = new Quaternion?(this.cameraTargetRot);
        }

        public void RotateToFace(Vector3 position)
        {
            Vector3 position2 = this.mainCamera.transform.position;
            if (position2 == position)
            {
                return;
            }
            if (position.x != position2.x || position.z != position2.z)
            {
                this.SetActorTargetRotation((position - position2).ToXZAngle());
            }
            if (position.x == position2.x && position.z == position2.z)
            {
                if (position.y < position2.y)
                {
                    this.SetCameraTargetRotation(90f);
                    return;
                }
                if (position.y > position2.y)
                {
                    this.SetCameraTargetRotation(-90f);
                    return;
                }
            }
            else
            {
                float num = Calc.Asin((position.y - position2.y) / (position - position2).magnitude);
                this.SetCameraTargetRotation(-57.29578f * num);
            }
        }

        public void OnPlayerActorSpawned(bool afterLoading)
        {
            base.transform.position = Get.NowControlledActor.Position;
            this.TargetPosition = base.transform.position;
            if (!afterLoading)
            {
                base.transform.localRotation = Quaternion.Euler(0f, Get.NowControlledActor.Rotation.ToXZAngle(), 0f);
                this.SetActorTargetRotation(base.transform.localRotation);
            }
        }

        private void ProcessMouseInput()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                this.waitForMouseMoveFrames = 5;
                return;
            }
            if (Get.TextSequenceDrawer.Showing)
            {
                return;
            }
            float num = (SteamDeckUtility.IsSteamDeck ? 3f : (ControllerUtility.InControllerMode ? 1.5f : 1f));
            float num2 = Input.GetAxisRaw("Mouse X") * 1f * PrefsHelper.MouseSensitivity * num * Clock.TrailerFakeAdjustment;
            float num3 = Input.GetAxisRaw("Mouse Y") * 1f * PrefsHelper.MouseSensitivity * num * Clock.TrailerFakeAdjustment;
            if (num3 == 0f && num2 == 0f)
            {
                return;
            }
            if (PrefsHelper.InvertY)
            {
                num3 = -num3;
            }
            if (this.waitForMouseMoveFrames != 0 && (num3 != 0f || num2 != 0f))
            {
                this.waitForMouseMoveFrames--;
                if ((double)Math.Abs(num3) > 1.2 || (double)Math.Abs(num2) > 1.2)
                {
                    return;
                }
            }
            if (num2 != 0f)
            {
                base.transform.localRotation *= Quaternion.Euler(0f, num2, 0f);
                this.SetActorTargetRotation(this.actorTargetRot * Quaternion.Euler(0f, num2, 0f));
            }
            if (num3 != 0f)
            {
                this.mainCamera.transform.localRotation = this.ClampXAxisRotation(this.mainCamera.transform.localRotation * Quaternion.Euler(-num3, 0f, 0f));
                this.SetCameraTargetRotation(this.ClampXAxisRotation(this.cameraTargetRot * Quaternion.Euler(-num3, 0f, 0f)));
            }
        }

        private Quaternion ClampXAxisRotation(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1f;
            float num = 114.59156f * Calc.Atan(q.x);
            num = Calc.Clamp(num, -90f, 90f);
            q.x = Calc.Tan(0.008726646f * num);
            return q;
        }

        private Quaternion GetTargetGravityRot()
        {
            Vector3Int cameraGravity = this.CameraGravity;
            if (cameraGravity.y == -1)
            {
                return Quaternion.identity;
            }
            if (cameraGravity.y == 1)
            {
                return Quaternion.Euler(180f, 0f, 0f);
            }
            if (cameraGravity.x == -1)
            {
                return Quaternion.Euler(0f, 0f, -90f);
            }
            if (cameraGravity.x == 1)
            {
                return Quaternion.Euler(0f, 0f, 90f);
            }
            if (cameraGravity.z == -1)
            {
                return Quaternion.Euler(90f, 0f, 0f);
            }
            return Quaternion.Euler(-90f, 0f, 0f);
        }

        private bool WouldClipIntoAnyObjectAt(Vector3 at, out Vector3Int blockingCell)
        {
            if (Clock.Time - this.lastPlayerTeleportTime < 2f)
            {
                blockingCell = default(Vector3Int);
                return false;
            }
            if ((this.targetPosition - at).sqrMagnitude > 9.61f)
            {
                blockingCell = default(Vector3Int);
                return false;
            }
            if (Get.CellsInfo.AnyFilledImpassableAt(Get.NowControlledActor.Position))
            {
                blockingCell = default(Vector3Int);
                return false;
            }
            if (Get.CameraEffects.WalkingStaircase)
            {
                blockingCell = default(Vector3Int);
                return false;
            }
            Vector3Int vector3Int = at.RoundToVector3Int();
            if (vector3Int.InBounds() && Get.CellsInfo.AnyFilledImpassableAt(vector3Int))
            {
                blockingCell = vector3Int;
                return true;
            }
            if (!vector3Int.InRoomsBounds())
            {
                blockingCell = vector3Int;
                return false;
            }
            for (int i = 0; i < Vector3IntUtility.DirectionsCardinal.Length; i++)
            {
                Vector3Int vector3Int2 = (at + Vector3IntUtility.DirectionsCardinal[i] * 0.13f).RoundToVector3Int();
                if (vector3Int2.InBounds() && Get.CellsInfo.AnyFilledImpassableAt(vector3Int2))
                {
                    blockingCell = vector3Int2;
                    return true;
                }
            }
            blockingCell = default(Vector3Int);
            return false;
        }

        private void MoveOutOfClippedIntoObject()
        {
            Vector3Int vector3Int;
            if (!this.WouldClipIntoAnyObjectAt(base.transform.position, out vector3Int))
            {
                return;
            }
            float num;
            Vector3 vector = Geometry.ClosestCubeSide(vector3Int, Vector3.one, base.transform.position, out num);
            base.transform.position += vector * (num + 0.13f + 0.0001f);
        }

        public void OnPlayerMoved(Vector3Int prev)
        {
            if (Get.NowControlledActor.Position.GetGridDistance(prev) >= 2)
            {
                this.lastPlayerTeleportTime = Clock.Time;
            }
        }

        public void OnSwitchedNowControlledActor()
        {
            this.lastPlayerTeleportTime = Clock.Time;
            this.cameraSwitchingBetweenActorsNow = true;
            this.lastSwitchedNowControlledActorFrame = Time.frameCount;
        }

        private Vector3 targetPosition;

        private Quaternion actorTargetRot;

        private Quaternion cameraTargetRot;

        private int waitForMouseMoveFrames = 5;

        private float lastPlayerTeleportTime = -99999f;

        private bool cameraSwitchingBetweenActorsNow;

        private int lastSwitchedNowControlledActorFrame = -99999;

        private Camera mainCamera;

        [SerializeField]
        public AnimationCurve StaircaseAnimation_xOffset;

        [SerializeField]
        public AnimationCurve StaircaseAnimation_yOffset;

        [SerializeField]
        public AnimationCurve StaircaseAnimation_zOffset;

        [SerializeField]
        public AnimationCurve StaircaseUpAnimation_xOffset;

        [SerializeField]
        public AnimationCurve StaircaseUpAnimation_yOffset;

        [SerializeField]
        public AnimationCurve StaircaseUpAnimation_zOffset;

        private const float BasePositionLerpSpeed = 0.19f;

        private const float SensitivityX = 1f;

        private const float SensitivityY = 1f;

        private const float MinimumRotationX = -90f;

        private const float MaximumRotationX = 90f;

        private const float RotationLerpSpeed = 8.125945f;

        private const int DefaultWaitForMouseMoveFrames = 5;

        private const float ObjectClippingPad = 0.13f;
    }
}