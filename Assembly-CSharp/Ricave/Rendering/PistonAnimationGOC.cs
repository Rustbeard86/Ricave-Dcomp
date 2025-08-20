using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class PistonAnimationGOC : MonoBehaviour
    {
        private void Start()
        {
            this.initialPosition = base.transform.localPosition;
            this.handleAnchorTransform = base.transform.Find("Anchor");
            this.anchorInitialScale = this.handleAnchorTransform.localScale;
            EntityGOC componentInParent = base.GetComponentInParent<EntityGOC>();
            PistonComp pistonComp;
            if (componentInParent == null)
            {
                pistonComp = null;
            }
            else
            {
                Entity entity = componentInParent.Entity;
                pistonComp = ((entity != null) ? entity.GetComp<PistonComp>() : null);
            }
            this.piston = pistonComp;
        }

        private void Update()
        {
            base.transform.localPosition = Vector3.Lerp(this.initialPosition, this.initialPosition.WithAddedY(1f), this.pct);
            this.handleAnchorTransform.localScale = Vector3.Lerp(this.anchorInitialScale, this.anchorInitialScale.WithAddedY(0.5f), this.pct);
            if (this.piston != null && this.piston.Active)
            {
                this.pct = Calc.StepTowards(this.pct, 1f, 4f * Clock.DeltaTime);
                return;
            }
            this.pct = Calc.StepTowards(this.pct, 0f, 4f * Clock.DeltaTime);
        }

        private Vector3 initialPosition;

        private Transform handleAnchorTransform;

        private PistonComp piston;

        private Vector3 anchorInitialScale;

        private float pct;

        private const float Speed = 4f;
    }
}