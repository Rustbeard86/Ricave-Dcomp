using System;
using UnityEngine;

namespace Ricave.Core
{
    public struct InterpolatedVector3
    {
        public Vector3 Start
        {
            get
            {
                return this.start;
            }
        }

        public Vector3 Target
        {
            get
            {
                return this.target;
            }
        }

        public Vector3 CurrentValue
        {
            get
            {
                if (Clock.Time == this.targetSetTime)
                {
                    return this.start;
                }
                float num = Calc.Clamp01((Clock.Time - this.targetSetTime) / this.timeToNextFixedUpdateWhenTargetSet);
                return Vector3.Lerp(this.start, this.target, num);
            }
        }

        public InterpolatedVector3(Vector3 value)
        {
            this = default(InterpolatedVector3);
            this.target = value;
            this.start = value;
        }

        public void SetTarget(Vector3 target)
        {
            this.SetTarget(target, this.CurrentValue);
        }

        public void SetTarget(Vector3 target, Vector3 newStart)
        {
            this.target = target;
            this.start = newStart;
            this.targetSetTime = Clock.Time;
            this.timeToNextFixedUpdateWhenTargetSet = InterpolationUtility.TimeToNextFixedUpdate;
        }

        private Vector3 target;

        private Vector3 start;

        private float targetSetTime;

        private float timeToNextFixedUpdateWhenTargetSet;
    }
}