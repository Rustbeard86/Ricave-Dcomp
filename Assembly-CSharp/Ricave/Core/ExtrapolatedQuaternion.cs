using System;
using UnityEngine;

namespace Ricave.Core
{
    public struct ExtrapolatedQuaternion
    {
        public Quaternion LastReal
        {
            get
            {
                return this.anticipated.Start;
            }
        }

        public Quaternion AnticipatedTarget
        {
            get
            {
                return this.anticipated.Target;
            }
        }

        public Quaternion CurrentExtrapolated
        {
            get
            {
                return this.anticipated.CurrentValue;
            }
        }

        public void Set(Quaternion currentReal, Quaternion nextAnticipated)
        {
            this.anticipated.SetTarget(nextAnticipated, currentReal);
        }

        public void TransformFromFixedUpdate(Func<Quaternion, Quaternion> fixedUpdate)
        {
            Quaternion quaternion = fixedUpdate(this.LastReal);
            Quaternion quaternion2 = fixedUpdate(quaternion);
            this.Set(quaternion, quaternion2);
        }

        private InterpolatedQuaternion anticipated;
    }
}