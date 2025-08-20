using System;
using UnityEngine;

namespace Ricave.Core
{
    public struct InterpolatedVector2
    {
        public Vector2 Start
        {
            get
            {
                return this.start;
            }
        }

        public Vector2 Target
        {
            get
            {
                return this.target;
            }
        }

        public Vector2 CurrentValue
        {
            get
            {
                if (Clock.Time == this.targetSetTime)
                {
                    return this.start;
                }
                float num = Calc.Clamp01((Clock.Time - this.targetSetTime) / this.timeToNextFixedUpdateWhenTargetSet);
                return Vector2.Lerp(this.start, this.target, num);
            }
        }

        public InterpolatedVector2(Vector2 value)
        {
            this = default(InterpolatedVector2);
            this.target = value;
            this.start = value;
        }

        public void SetTarget(Vector2 target)
        {
            this.SetTarget(target, this.CurrentValue);
        }

        public void SetTarget(Vector2 target, Vector2 newStart)
        {
            this.target = target;
            this.start = newStart;
            this.targetSetTime = Clock.Time;
            this.timeToNextFixedUpdateWhenTargetSet = InterpolationUtility.TimeToNextFixedUpdate;
        }

        private Vector2 target;

        private Vector2 start;

        private float targetSetTime;

        private float timeToNextFixedUpdateWhenTargetSet;
    }
}