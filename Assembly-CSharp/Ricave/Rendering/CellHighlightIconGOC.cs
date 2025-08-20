using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class CellHighlightIconGOC : MonoBehaviour
    {
        private void LateUpdate()
        {
            this.UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            Transform transform = base.transform;
            if (transform.position != this.prevPosition)
            {
                this.prevPosition = transform.position;
                this.basePosition = transform.position;
            }
            transform.position = this.basePosition + Vector3.up * Calc.Sin(Clock.Time * 2f) * 0.03f;
            float num;
            float num2;
            ActorGOC.GetLookAtPlayerRot(transform.position, Vector3.one, out num, out num2);
            transform.rotation = Quaternion.Euler(num2, num, 0f);
            this.prevPosition = transform.position;
        }

        private Vector3 basePosition;

        private Vector3 prevPosition;
    }
}