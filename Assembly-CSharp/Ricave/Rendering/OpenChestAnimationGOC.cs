using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class OpenChestAnimationGOC : MonoBehaviour
    {
        public EntitySpec ChestSpec { get; set; }

        private void Start()
        {
            this.entityGameObject = Object.Instantiate<GameObject>(this.ChestSpec.PrefabAdjusted, base.gameObject.transform);
            EntityGOC entityGOC;
            if (this.entityGameObject.TryGetComponent<EntityGOC>(out entityGOC))
            {
                Object.DestroyImmediate(entityGOC);
            }
            this.lidGameObject = this.entityGameObject.GetComponentInChildren<ChestLidForAnimationGOC>().gameObject;
            this.startTime = Clock.Time;
        }

        private void FixedUpdate()
        {
            this.lidGameObject.transform.localRotation = Quaternion.Lerp(this.lidGameObject.transform.localRotation, Quaternion.Euler(-70f, 0f, 0f), 0.14f);
            if (Clock.Time - this.startTime > 0.1f)
            {
                Transform transform = base.transform;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.1f);
                transform.localPosition += new Vector3(0f, -0.018f, 0f);
            }
        }

        private GameObject entityGameObject;

        private GameObject lidGameObject;

        private float startTime;
    }
}