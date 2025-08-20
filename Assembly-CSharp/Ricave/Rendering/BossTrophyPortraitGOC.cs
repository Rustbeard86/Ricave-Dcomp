using System;
using Ricave.Core;
using TMPro;
using UnityEngine;

namespace Ricave.Rendering
{
    public class BossTrophyPortraitGOC : MonoBehaviour
    {
        private void Start()
        {
            BossTrophy bossTrophy = (BossTrophy)base.GetComponentInParent<EntityGOC>().Entity;
            MeshRenderer component = base.GetComponent<MeshRenderer>();
            if (bossTrophy.Boss == null)
            {
                component.sharedMaterial = MaterialsFromTexture.InvisibleMaterial;
                component.enabled = false;
                this.killedOnFloorText.GetComponent<TextMeshPro>().text = "";
                return;
            }
            GameObject prefabAdjusted = bossTrophy.Boss.ActorSpec.PrefabAdjusted;
            if (bossTrophy.Boss.ActorSpec.Actor.Texture != null)
            {
                component.sharedMaterial = prefabAdjusted.GetComponent<MeshRenderer>().sharedMaterial;
                Material material = component.material;
                int i = material.renderQueue;
                material.renderQueue = i - 1;
            }
            else
            {
                component.sharedMaterial = MaterialsFromTexture.InvisibleMaterial;
                component.enabled = false;
                GameObject gameObject = Object.Instantiate<GameObject>(prefabAdjusted, base.transform);
                EntityGOC entityGOC;
                if (gameObject.TryGetComponent<EntityGOC>(out entityGOC))
                {
                    Object.DestroyImmediate(entityGOC);
                }
                foreach (Component component2 in gameObject.GetComponentsInChildren<Component>())
                {
                    if (!(component2 is Transform) && !(component2 is MeshRenderer) && !(component2 is MeshFilter))
                    {
                        Object.Destroy(component2);
                    }
                }
                gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z - 0.1f);
            }
            this.killedOnFloorText.GetComponent<TextMeshPro>().text = bossTrophy.Boss.KilledOnFloor.ToStringCached();
        }

        [SerializeField]
        private GameObject killedOnFloorText;
    }
}