using System;

namespace Ricave.Core
{
    public struct ExtrapolatedFloat
    {
        public float LastReal
        {
            get
            {
                return this.anticipated.Start;
            }
        }

        public float AnticipatedTarget
        {
            get
            {
                return this.anticipated.Target;
            }
        }

        public float CurrentExtrapolated
        {
            get
            {
                return this.anticipated.CurrentValue;
            }
        }

        public void Set(float currentReal, float nextAnticipated)
        {
            this.anticipated.SetTarget(nextAnticipated, currentReal);
        }

        public void TransformFromFixedUpdate(Func<float, float> fixedUpdate)
        {
            float num = fixedUpdate(this.LastReal);
            float num2 = fixedUpdate(num);
            this.Set(num, num2);
        }

        private InterpolatedFloat anticipated;
    }
}