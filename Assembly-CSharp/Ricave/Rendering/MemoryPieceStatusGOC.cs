using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class MemoryPieceStatusGOC : MonoBehaviour
    {
        private void Start()
        {
            EntitySpec entitySpec = Get.Specs.Get<EntitySpec>(this.memoryPieceSpec);
            bool flag = Get.TotalLobbyItems.Any(entitySpec);
            MeshRenderer component = base.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>();
            if (flag)
            {
                component.material.mainTexture = entitySpec.IconAdjusted;
                component.material.color = entitySpec.IconColorAdjusted;
            }
            base.GetComponent<MeshRenderer>().material.color = (flag ? new Color(0.72f, 0.72f, 0.72f) : new Color(0.1f, 0.1f, 0.1f));
        }

        [SerializeField]
        private string memoryPieceSpec;
    }
}