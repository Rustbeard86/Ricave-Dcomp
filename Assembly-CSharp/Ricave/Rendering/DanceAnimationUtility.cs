using System;
using System.Collections.Generic;
using Ricave.Core;

namespace Ricave.Rendering
{
    public static class DanceAnimationUtility
    {
        public static float GetPosOffset(Actor actor)
        {
            if (actor.IsNowControlledActor)
            {
                return 0f;
            }
            if (Get.CellsInfo.IsFallingAt(actor.Position, actor, false))
            {
                return Calc.Sin(Clock.Time * 8f) * 0.008f;
            }
            if (!actor.AllowDanceAnimation)
            {
                return 0f;
            }
            float animationSpeedFactor = DanceAnimationUtility.GetAnimationSpeedFactor(actor);
            if (actor.CanFly)
            {
                return Calc.Sin(Clock.Time * 2.3f * animationSpeedFactor) * 0.023f;
            }
            return DanceAnimationUtility.GetPosOffset(actor.InstanceID, animationSpeedFactor);
        }

        public static float GetPosOffset(int actorID, float animationSpeedFactor = 1f)
        {
            DanceAnimationUtility.DanceState danceState;
            float num;
            DanceAnimationUtility.GetCurrentState(actorID, animationSpeedFactor, out danceState, out num);
            switch (danceState)
            {
                case DanceAnimationUtility.DanceState.IdleLeft:
                    return 0f;
                case DanceAnimationUtility.DanceState.IdleRight:
                    return 0f;
                case DanceAnimationUtility.DanceState.ChangeToLeft:
                    if (num >= 0.5f)
                    {
                        return 0f;
                    }
                    return 0.02f;
                case DanceAnimationUtility.DanceState.ChangeToRight:
                    if (num >= 0.5f)
                    {
                        return 0f;
                    }
                    return 0.02f;
                case DanceAnimationUtility.DanceState.JumpOnLeft:
                    if (num >= 0.5f)
                    {
                        return 0f;
                    }
                    return 0.02f;
                case DanceAnimationUtility.DanceState.JumpOnRight:
                    if (num >= 0.5f)
                    {
                        return 0f;
                    }
                    return 0.02f;
                default:
                    return 0f;
            }
        }

        public static float GetRotation(Actor actor)
        {
            if (Get.CellsInfo.IsFallingAt(actor.Position, actor, false))
            {
                return 0f;
            }
            if (!actor.AllowDanceAnimation)
            {
                return 0f;
            }
            if (actor.CanFly)
            {
                return 0f;
            }
            return DanceAnimationUtility.GetRotation(actor.InstanceID, DanceAnimationUtility.GetAnimationSpeedFactor(actor));
        }

        public static float GetRotation(int actorID, float animationSpeedFactor = 1f)
        {
            DanceAnimationUtility.DanceState danceState;
            float num;
            DanceAnimationUtility.GetCurrentState(actorID, animationSpeedFactor, out danceState, out num);
            num = Calc.Smooth(num);
            switch (danceState)
            {
                case DanceAnimationUtility.DanceState.IdleLeft:
                    return -2f;
                case DanceAnimationUtility.DanceState.IdleRight:
                    return 2f;
                case DanceAnimationUtility.DanceState.ChangeToLeft:
                    return Calc.Lerp(2f, -2f, num);
                case DanceAnimationUtility.DanceState.ChangeToRight:
                    return Calc.Lerp(-2f, 2f, num);
                case DanceAnimationUtility.DanceState.JumpOnLeft:
                    return -2f;
                case DanceAnimationUtility.DanceState.JumpOnRight:
                    return 2f;
                default:
                    return 0f;
            }
        }

        private static void GetCurrentState(int actorID, float animationSpeedFactor, out DanceAnimationUtility.DanceState state, out float pct)
        {
            List<DanceAnimationUtility.DanceState> danceStates;
            if (!DanceAnimationUtility.cachedDanceStates.TryGetValue(actorID, out danceStates))
            {
                danceStates = DanceAnimationUtility.GetDanceStates(actorID);
                DanceAnimationUtility.cachedDanceStates.Add(actorID, danceStates);
            }
            float num = Clock.Time * 2f * animationSpeedFactor;
            int num2 = (int)num;
            pct = num - (float)num2;
            state = danceStates[num2 % danceStates.Count];
        }

        private static List<DanceAnimationUtility.DanceState> GetDanceStates(int actorID)
        {
            List<DanceAnimationUtility.DanceMove> danceMoves = DanceAnimationUtility.GetDanceMoves(actorID);
            List<DanceAnimationUtility.DanceState> list = new List<DanceAnimationUtility.DanceState>();
            bool flag = true;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < danceMoves.Count; j++)
                {
                    switch (danceMoves[j])
                    {
                        case DanceAnimationUtility.DanceMove.Idle:
                            list.Add(flag ? DanceAnimationUtility.DanceState.IdleLeft : DanceAnimationUtility.DanceState.IdleRight);
                            break;
                        case DanceAnimationUtility.DanceMove.ChangeFoot:
                            list.Add(flag ? DanceAnimationUtility.DanceState.ChangeToRight : DanceAnimationUtility.DanceState.ChangeToLeft);
                            flag = !flag;
                            break;
                        case DanceAnimationUtility.DanceMove.Jump:
                            list.Add(flag ? DanceAnimationUtility.DanceState.JumpOnLeft : DanceAnimationUtility.DanceState.JumpOnRight);
                            break;
                    }
                }
            }
            return list;
        }

        private static List<DanceAnimationUtility.DanceMove> GetDanceMoves(int actorID)
        {
            Rand.PushState(Calc.CombineHashes<int, int>(actorID, 94907512));
            List<DanceAnimationUtility.DanceMove> list = new List<DanceAnimationUtility.DanceMove>();
            bool flag = false;
            for (int i = 0; i < 4; i++)
            {
                DanceAnimationUtility.DanceMove danceMove = DanceAnimationUtility.GenerateNextDanceMove(list);
                list.Add(danceMove);
                if (danceMove == DanceAnimationUtility.DanceMove.ChangeFoot)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                list[Rand.RangeInclusive(0, list.Count - 1)] = DanceAnimationUtility.DanceMove.ChangeFoot;
            }
            Rand.PopState();
            return list;
        }

        private static DanceAnimationUtility.DanceMove GenerateNextDanceMove(List<DanceAnimationUtility.DanceMove> moves)
        {
            if (moves.Count == 3)
            {
                return DanceAnimationUtility.DanceMove.Idle;
            }
            return DanceAnimationUtility.DanceMove.ChangeFoot;
        }

        private static float GetAnimationSpeedFactor(Actor actor)
        {
            ActorGOC actorGOC = actor.ActorGOC;
            if (!(actorGOC != null))
            {
                return 1f;
            }
            switch (actorGOC.LastMovesPerTurnAnimation)
            {
                case ActorGOC.MovesPerTurnAnimation.Normal:
                    return 1f;
                case ActorGOC.MovesPerTurnAnimation.WontMoveThisTurn:
                    return 0.5f;
                case ActorGOC.MovesPerTurnAnimation.WillMoveAtLeastTwice:
                    return 1.5f;
                default:
                    return 1f;
            }
        }

        private static Dictionary<int, List<DanceAnimationUtility.DanceState>> cachedDanceStates = new Dictionary<int, List<DanceAnimationUtility.DanceState>>();

        public const float MaxRot = 2f;

        public const float DanceMaxYOffset = 0.02f;

        public const float FlyAmplitude = 0.023f;

        private enum DanceMove
        {
            Idle,

            ChangeFoot,

            Jump
        }

        private enum DanceState
        {
            IdleLeft,

            IdleRight,

            ChangeToLeft,

            ChangeToRight,

            JumpOnLeft,

            JumpOnRight
        }
    }
}