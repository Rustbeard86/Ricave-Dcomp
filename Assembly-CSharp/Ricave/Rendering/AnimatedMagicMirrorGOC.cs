using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class AnimatedMagicMirrorGOC : MonoBehaviour
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
            base.gameObject.transform.localPosition = new Vector3(0f, Calc.Sin(Clock.Time * 0.6f) * 0.027f, 0f);
        }
    }
}