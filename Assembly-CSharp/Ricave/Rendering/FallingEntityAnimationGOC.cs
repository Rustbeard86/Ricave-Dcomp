using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class FallingEntityAnimationGOC : MonoBehaviour
    {
        public Vector3 Destination { get; set; }

        private void Start()
        {
            EntitySpec entitySpec = Get.Specs.Get<EntitySpec>(this.entitySpecID);
            this.entityGameObject = Object.Instantiate<GameObject>(entitySpec.PrefabAdjusted, base.gameObject.transform);
            this.entityGameObject.transform.rotation = Vector3IntUtility.Down.CardinalDirToQuaternion();
            EntityGOC entityGOC;
            if (this.entityGameObject.TryGetComponent<EntityGOC>(out entityGOC))
            {
                Object.DestroyImmediate(entityGOC);
            }
        }

        private void Update()
        {
            Transform transform = this.entityGameObject.transform;
            transform.position = Vector3.MoveTowards(transform.position, this.Destination, 8f * Clock.DeltaTime);
        }

        [SerializeField]
        private string entitySpecID;

        private GameObject entityGameObject;

        private const float PositionChangeSpeed = 8f;
    }
}