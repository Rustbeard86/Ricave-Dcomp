using System;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class AnimatedLineGOC : MonoBehaviour
    {
        private void Start()
        {
            this.lineRenderer = base.gameObject.GetComponent<LineRenderer>();
            this.initialWidth = this.lineRenderer.startWidth;
            this.initialColor = this.lineRenderer.startColor;
            this.duration = base.gameObject.GetComponentInParent<ParticleSystem>().main.duration;
            if (this.animateColor)
            {
                this.lineRenderer.startColor = this.initialColor.WithAlphaFactor(0f);
                this.lineRenderer.endColor = this.lineRenderer.startColor;
                return;
            }
            this.lineRenderer.startWidth = 0f;
            this.lineRenderer.endWidth = this.lineRenderer.startWidth;
        }

        private void OnEnable()
        {
            this.startTime = Clock.Time;
            if (this.lineRenderer != null)
            {
                if (this.animateColor)
                {
                    this.lineRenderer.startColor = this.initialColor.WithAlphaFactor(0f);
                    this.lineRenderer.endColor = this.lineRenderer.startColor;
                    return;
                }
                this.lineRenderer.startWidth = 0f;
                this.lineRenderer.endWidth = this.lineRenderer.startWidth;
            }
        }

        private void LateUpdate()
        {
            float num = Calc.Clamp01((Clock.Time - this.startTime) / this.duration);
            float num2;
            if (num < 0.5f)
            {
                num2 = num * 2f;
            }
            else
            {
                num2 = (1f - num) * 2f;
            }
            if (this.animateColor)
            {
                this.lineRenderer.startColor = this.initialColor.WithAlphaFactor(num2);
                this.lineRenderer.endColor = this.lineRenderer.startColor;
                return;
            }
            this.lineRenderer.startWidth = this.initialWidth * num2;
            this.lineRenderer.endWidth = this.lineRenderer.startWidth;
        }

        [SerializeField]
        private bool animateColor;

        private LineRenderer lineRenderer;

        private float startTime;

        private float duration;

        private float initialWidth;

        private Color initialColor;
    }
}