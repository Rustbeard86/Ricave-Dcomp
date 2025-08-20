using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class AnimatedSmallCrystalGOC : MonoBehaviour
    {
        private void Start()
        {
            this.SetPosition();
        }

        private void Update()
        {
            this.SetPosition();
        }

        private void SetPosition()
        {
            if (this.first)
            {
                this.first = false;
                this.radius = base.gameObject.transform.localPosition.WithY(0f).magnitude;
                this.startAngle = base.gameObject.transform.localPosition.ToXZAngle();
            }
            float num = this.startAngle + Clock.Time * 50f % 360f;
            base.gameObject.transform.localPosition = new Vector3(Calc.Sin(num * 0.017453292f) * this.radius, base.gameObject.transform.localPosition.y, Calc.Cos(num * 0.017453292f) * this.radius);
        }

        private bool first = true;

        private float radius;

        private float startAngle;
    }
}