using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class AnimatedRopeGOC : MonoBehaviour
    {
        private void Start()
        {
            this.instanceID = base.GetInstanceID();
            this.SetTransform();
        }

        private void LateUpdate()
        {
            this.SetTransform();
        }

        private void SetTransform()
        {
            Transform transform = base.transform;
            Vector3Int vector3Int = transform.parent.position.RoundToVector3Int().Above();
            if (vector3Int.InBounds())
            {
                EntityGOC componentInParent = transform.GetComponentInParent<EntityGOC>();
                EntitySpec entitySpec = ((componentInParent != null) ? componentInParent.Entity.Spec : null) ?? Get.Entity_HangingRope;
                Entity firstEntityOfSpecAt = Get.World.GetFirstEntityOfSpecAt(vector3Int, entitySpec);
                if (firstEntityOfSpecAt != null)
                {
                    AnimatedRopeEndMarkerGOC componentInChildren = firstEntityOfSpecAt.GameObject.GetComponentInChildren<AnimatedRopeEndMarkerGOC>();
                    transform.position = Vector3.MoveTowards(transform.position, componentInChildren.transform.position, Clock.DeltaTime * 0.75f);
                }
            }
            float num = Calc.Sin((float)(this.instanceID % 500) * 3.2f + Clock.Time * 1.35f) * 3.5f * this.animationStrength;
            float num2 = Calc.Sin((float)(this.instanceID % 523) * 2.7f + Clock.Time * 1.35f * 0.81f) * 3.5f * 0.716f * this.animationStrength;
            transform.localRotation = Quaternion.Euler(num, 0f, num2);
        }

        private int instanceID;

        [SerializeField]
        private float animationStrength = 1f;
    }
}