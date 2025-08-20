using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class RetractableAnimationGOC : MonoBehaviour
    {
        private void Start()
        {
            this.initialPosition = base.transform.localPosition;
            EntityGOC componentInParent = base.GetComponentInParent<EntityGOC>();
            RetractableComp retractableComp;
            if (componentInParent == null)
            {
                retractableComp = null;
            }
            else
            {
                Entity entity = componentInParent.Entity;
                retractableComp = ((entity != null) ? entity.GetComp<RetractableComp>() : null);
            }
            this.retractable = retractableComp;
        }

        private void Update()
        {
            if (this.retractable == null || this.retractable.Active)
            {
                base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, this.initialPosition, 3.65f * Clock.DeltaTime);
                return;
            }
            base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, this.initialPosition + Vector3.down * this.distance, 1.25f * Clock.DeltaTime);
        }

        [SerializeField]
        private float distance = 0.37f;

        private Vector3 initialPosition;

        private RetractableComp retractable;
    }
}