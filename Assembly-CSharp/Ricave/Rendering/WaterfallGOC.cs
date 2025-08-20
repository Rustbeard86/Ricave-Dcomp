using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class WaterfallGOC : MonoBehaviour
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

        public Vector3Int DirectionCardinal
        {
            get
            {
                return base.transform.rotation.ToCardinalDir();
            }
        }

        private void Start()
        {
            this.waterfall = base.GetComponent<ParticleSystem>();
            this.entity = base.GetComponentInParent<EntityGOC>();
            this.instanceID = base.GetInstanceID();
            this.initialWaterfallParticleLifetime = this.waterfall.main.startLifetime.constant;
            ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i] != this.waterfall)
                {
                    this.foam = componentsInChildren[i];
                    break;
                }
            }
            this.initialFoamLocalZ = this.foam.transform.localPosition.z;
            this.FindFloor();
            this.waterfall.Simulate(1.5f, true);
            this.waterfall.Play();
            this.foam.Simulate(1.5f, true);
            this.foam.Play();
        }

        private void Update()
        {
            if ((Clock.Frame + this.instanceID) % 5 == 0)
            {
                this.FindFloor();
            }
        }

        private void FindFloor()
        {
            Vector3Int vector3Int = this.Position;
            if (this.straight)
            {
                vector3Int += Vector3IntUtility.Down;
            }
            else
            {
                vector3Int += this.DirectionCardinal;
            }
            while (vector3Int.InBounds() && Get.CellsInfo.CanProjectilesPassThrough(vector3Int))
            {
                vector3Int += Vector3IntUtility.Down;
            }
            int num = this.Position.y - vector3Int.y;
            float num2 = this.initialWaterfallParticleLifetime / 2f * (float)num;
            Vector3 localPosition = this.foam.transform.localPosition;
            localPosition.y = (float)(-(float)(num - 1)) - 0.404f;
            if (!this.straight)
            {
                localPosition.z = this.initialFoamLocalZ + (float)(num - 2) * 0.32f;
            }
            if (vector3Int.Above().InBounds() && Get.CellsInfo.AnyWaterAt(vector3Int.Above()))
            {
                localPosition.y += 0.404f;
            }
            else
            {
                num2 += this.initialWaterfallParticleLifetime / 4f;
                if (!this.straight)
                {
                    localPosition.z += 0.18f;
                }
            }
            this.foam.transform.localPosition = localPosition;
            this.waterfall.main.startLifetime = num2;
        }

        [SerializeField]
        private bool straight;

        private ParticleSystem waterfall;

        private ParticleSystem foam;

        private EntityGOC entity;

        private int instanceID;

        private float initialWaterfallParticleLifetime;

        private float initialFoamLocalZ;

        private const float WaterHeight = 0.404f;
    }
}