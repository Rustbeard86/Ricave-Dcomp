using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class RainSplashGOC : MonoBehaviour
    {
        public Vector3Int Position
        {
            get
            {
                if (!(this.entity != null))
                {
                    return base.transform.position.RoundToVector3Int();
                }
                return this.entity.Entity.Position;
            }
        }

        private void Start()
        {
            this.rain = base.GetComponent<ParticleSystem>();
            this.entity = base.GetComponentInParent<EntityGOC>();
            this.killPlane = this.rain.collision.GetPlane(0);
            this.splashTriggerCollider = this.rain.trigger.GetCollider(0);
            this.instanceID = base.GetInstanceID();
            ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i] != this.rain)
                {
                    this.splash = componentsInChildren[i];
                    return;
                }
            }
        }

        private void OnParticleTrigger()
        {
            if (this.cameraClose)
            {
                this.rain.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, this.tmpParticles);
                if (this.tmpParticles.Count != 0)
                {
                    Vector3 position = this.tmpParticles[0].position;
                    position.z = this.splashTriggerCollider.transform.localPosition.z;
                    this.splash.transform.localPosition = position;
                    this.splash.Play();
                    Get.Sound_Droplet.PlayOneShot(new Vector3?(this.splash.transform.position), 1f, 1f);
                }
            }
        }

        private void Update()
        {
            if ((Clock.Frame + this.instanceID) % 5 == 0)
            {
                Vector3Int vector3Int = this.Position;
                while (vector3Int.InBounds() && !Get.CellsInfo.AnyFilledImpassableAt(vector3Int))
                {
                    vector3Int += Vector3IntUtility.Down;
                }
                Vector3 position = this.killPlane.transform.position;
                position.y = (float)vector3Int.y - 0.5f;
                this.killPlane.transform.position = position;
                Vector3 position2 = this.splashTriggerCollider.transform.position;
                position2.y = (float)vector3Int.y + 0.5f;
                this.splashTriggerCollider.transform.position = position2;
                Vector3 localPosition = this.splashTriggerCollider.transform.localPosition;
                localPosition.x = 0f;
                localPosition.y = 0f;
                this.splashTriggerCollider.transform.localPosition = localPosition;
                this.cameraClose = (Get.CameraPosition - base.transform.position).WithY(0f).sqrMagnitude < 25f;
            }
        }

        private ParticleSystem rain;

        private ParticleSystem splash;

        private EntityGOC entity;

        private Transform killPlane;

        private Component splashTriggerCollider;

        private int instanceID;

        private bool cameraClose;

        private List<ParticleSystem.Particle> tmpParticles = new List<ParticleSystem.Particle>();

        private const float PlaySoundWhenCameraCloserThan = 5f;
    }
}