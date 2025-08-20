using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class CullParticlesGOC : MonoBehaviour
    {
        private void Start()
        {
            this.instanceID = base.GetInstanceID();
            this.ps = base.GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if ((Clock.Frame + this.instanceID) % 5 == 0 && this.ps != null)
            {
                if ((Get.CameraPosition - base.transform.position).WithY(0f).sqrMagnitude < 72.25f)
                {
                    if (!this.ps.isEmitting)
                    {
                        this.ps.Play();
                        return;
                    }
                }
                else if (this.ps.isEmitting)
                {
                    this.ps.Stop();
                }
            }
        }

        private int instanceID;

        private ParticleSystem ps;

        private const float CullDistance = 8.5f;
    }
}