using System;
using UnityEngine;

namespace Ricave.Core
{
    public struct ExtrapolatedVector3
    {
        public Vector3 LastReal
        {
            get
            {
                return this.anticipated.Start;
            }
        }

        public Vector3 AnticipatedTarget
        {
            get
            {
                return this.anticipated.Target;
            }
        }

        public Vector3 CurrentExtrapolated
        {
            get
            {
                return this.anticipated.CurrentValue;
            }
        }

        public void Set(Vector3 currentReal, Vector3 nextAnticipated)
        {
            this.anticipated.SetTarget(nextAnticipated, currentReal);
        }

        public void TransformFromFixedUpdate(Func<Vector3, Vector3> fixedUpdate)
        {
            Vector3 vector = fixedUpdate(this.LastReal);
            Vector3 vector2 = fixedUpdate(vector);
            this.Set(vector, vector2);
        }

        private InterpolatedVector3 anticipated;
    }
}