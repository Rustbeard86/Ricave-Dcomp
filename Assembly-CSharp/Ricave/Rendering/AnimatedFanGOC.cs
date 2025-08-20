using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class AnimatedFanGOC : MonoBehaviour
    {
        private void LateUpdate()
        {
            base.gameObject.transform.localRotation = Quaternion.Euler(0f, Clock.Time * 370f * this.speedFactor % 360f, 0f);
        }

        [SerializeField]
        private float speedFactor = 1f;
    }
}