using System;

namespace Ricave.Core
{
    public struct InterpolatedFloat
    {
        public float Start
        {
            get
            {
                return this.start;
            }
        }

        public float Target
        {
            get
            {
                return this.target;
            }
        }

        public float CurrentValue
        {
            get
            {
                if (Clock.Time == this.targetSetTime)
                {
                    return this.start;
                }
                float num = Calc.Clamp01((Clock.Time - this.targetSetTime) / this.timeToNextFixedUpdateWhenTargetSet);
                return Calc.Lerp(this.start, this.target, num);
            }
        }

        public InterpolatedFloat(float value)
        {
            this = default(InterpolatedFloat);
            this.target = value;
            this.start = value;
        }

        public void SetTarget(float target)
        {
            this.SetTarget(target, this.CurrentValue);
        }

        public void SetTarget(float target, float newStart)
        {
            this.target = target;
            this.start = newStart;
            this.targetSetTime = Clock.Time;
            this.timeToNextFixedUpdateWhenTargetSet = InterpolationUtility.TimeToNextFixedUpdate;
        }

        private float target;

        private float start;

        private float targetSetTime;

        private float timeToNextFixedUpdateWhenTargetSet;
    }
}