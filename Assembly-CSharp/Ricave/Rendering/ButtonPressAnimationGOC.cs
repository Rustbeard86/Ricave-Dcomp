using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class ButtonPressAnimationGOC : MonoBehaviour
    {
        public EntitySpec ButtonSpec { get; set; }

        private void Start()
        {
            this.entityGameObject = Object.Instantiate<GameObject>(this.ButtonSpec.PrefabAdjusted, base.gameObject.transform);
            EntityGOC entityGOC;
            if (this.entityGameObject.TryGetComponent<EntityGOC>(out entityGOC))
            {
                Object.DestroyImmediate(entityGOC);
            }
            this.animatedButtonPartGameObject = this.entityGameObject.GetComponentInChildren<AnimatedButtonPartGOC>().gameObject;
            this.startTime = Clock.Time;
            this.startLocalPos = this.animatedButtonPartGameObject.transform.localPosition;
        }

        private void FixedUpdate()
        {
            float num = Calc.ResolveFadeInStayOut(Clock.Time - this.startTime, 0.095f, 0f, 0.095f) * 0.074f;
            this.animatedButtonPartGameObject.transform.localPosition = this.startLocalPos + new Vector3(0f, 0f, -num);
            if (Clock.Time - this.startTime > 0.25f)
            {
                Transform transform = base.transform;
                transform.ScaleAround(Vector3.Lerp(transform.localScale, Vector3.zero, 0.1f), this.animatedButtonPartGameObject.transform.position);
            }
        }

        private GameObject entityGameObject;

        private GameObject animatedButtonPartGameObject;

        private float startTime;

        private Vector3 startLocalPos;
    }
}