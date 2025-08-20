using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class StrikeAnimationUtility
    {
        public static Vector3 GetPosOffset(float startTime, Vector3 dir, bool isNowControlledActor, float extraDistFactor = 1f)
        {
            float num = (isNowControlledActor ? 0.19f : 0.5f);
            float num2 = (Clock.Time - startTime) / num;
            if (isNowControlledActor)
            {
                return Calc.ResolveFadeInStayOut(num2, 0.5f, 0f, 0.5f) * dir * 0.18f * extraDistFactor;
            }
            if (num2 < 0.5f)
            {
                return dir * 0.54f * extraDistFactor;
            }
            return Vector3.zero;
        }

        public static float GetRotOffset(float startTime, Vector3 dir)
        {
            float num = (Clock.Time - startTime) / 0.5f;
            if (num < 0.2f)
            {
                return Calc.LerpDouble(0.05f, 0.2f, 0f, -7f, num);
            }
            return Calc.LerpDouble(0.2f, 1f, -7f, 0f, (num - 0.2f) / 0.8f);
        }

        public static float GetArmlessAnimationRotation(float startTime, Vector3 dir)
        {
            float num = (Clock.Time - startTime) / 0.5f;
            if (num < 0.2f)
            {
                return Calc.LerpDouble(0.05f, 0.2f, 0f, -23f, num);
            }
            return Calc.LerpDouble(0.2f, 1f, -23f, 0f, (num - 0.2f) / 0.8f);
        }

        private const float Duration = 0.5f;

        private const float Duration_Player = 0.19f;

        private const float Dist = 0.54f;

        private const float Dist_Player = 0.18f;
    }
}