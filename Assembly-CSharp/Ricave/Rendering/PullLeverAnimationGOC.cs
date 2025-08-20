using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class PullLeverAnimationGOC : MonoBehaviour
    {
        private void Start()
        {
            this.entityGameObject = Object.Instantiate<GameObject>(Get.Entity_Lever.PrefabAdjusted, base.gameObject.transform);
            EntityGOC entityGOC;
            if (this.entityGameObject.TryGetComponent<EntityGOC>(out entityGOC))
            {
                Object.DestroyImmediate(entityGOC);
            }
            this.handleGameObject = this.entityGameObject.GetComponentInChildren<LeverHandleForAnimationGOC>().gameObject;
            this.startTime = Clock.Time;
        }

        private void FixedUpdate()
        {
            this.handleGameObject.transform.localRotation = Quaternion.Lerp(this.handleGameObject.transform.localRotation, Quaternion.Euler(0f, 0f, -70f), 0.11f);
            if (Clock.Time - this.startTime > 0.12f)
            {
                Transform transform = base.transform;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.1f);
                transform.localPosition += new Vector3(0f, -0.02f, 0f);
            }
        }

        private GameObject entityGameObject;

        private GameObject handleGameObject;

        private float startTime;
    }
}