using System;
using UnityEngine;

namespace Ricave.Core
{
    public struct InterpolatedQuaternion
    {
        public Quaternion Start
        {
            get
            {
                return this.start;
            }
        }

        public Quaternion Target
        {
            get
            {
                return this.target;
            }
        }

        public Quaternion CurrentValue
        {
            get
            {
                if (Clock.Time == this.targetSetTime)
                {
                    return this.start;
                }
                float num = Calc.Clamp01((Clock.Time - this.targetSetTime) / this.timeToNextFixedUpdateWhenTargetSet);
                return Quaternion.Lerp(this.start, this.target, num);
            }
        }

        public InterpolatedQuaternion(Quaternion value)
        {
            this = default(InterpolatedQuaternion);
            this.target = value;
            this.start = value;
        }

        public void SetTarget(Quaternion target)
        {
            this.SetTarget(target, this.CurrentValue);
        }

        public void SetTarget(Quaternion target, Quaternion newStart)
        {
            this.target = target;
            this.start = newStart;
            this.targetSetTime = Clock.Time;
            this.timeToNextFixedUpdateWhenTargetSet = InterpolationUtility.TimeToNextFixedUpdate;
        }

        private Quaternion target;

        private Quaternion start;

        private float targetSetTime;

        private float timeToNextFixedUpdateWhenTargetSet;
    }
}