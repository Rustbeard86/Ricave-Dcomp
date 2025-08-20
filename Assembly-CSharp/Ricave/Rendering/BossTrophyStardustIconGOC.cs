using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class BossTrophyStardustIconGOC : MonoBehaviour
    {
        private void Start()
        {
            this.iconRenderer = base.GetComponent<MeshRenderer>();
            this.initialLocalPos = base.transform.localPosition;
            this.bossTrophy = (BossTrophy)base.GetComponentInParent<EntityGOC>().Entity;
            GameObject prefabAdjusted = Get.Entity_Stardust.PrefabAdjusted;
            this.iconRenderer.sharedMaterial = prefabAdjusted.GetComponent<MeshRenderer>().sharedMaterial;
            this.UpdateIcon();
        }

        private void Update()
        {
            this.UpdateIcon();
        }

        private void UpdateIcon()
        {
            if (this.bossTrophy.Boss != null && this.bossTrophy.Boss.TrophyHasStardust)
            {
                this.iconRenderer.transform.localPosition = this.initialLocalPos.WithAddedY(Calc.Sin(Clock.Time * 2f) * 0.01f);
                this.iconRenderer.enabled = true;
                return;
            }
            this.iconRenderer.enabled = false;
        }

        private MeshRenderer iconRenderer;

        private Vector3 initialLocalPos;

        private BossTrophy bossTrophy;
    }
}