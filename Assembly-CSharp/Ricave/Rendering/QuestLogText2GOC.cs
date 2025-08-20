using System;
using Ricave.Core;
using TMPro;
using UnityEngine;

namespace Ricave.Rendering
{
    public class QuestLogText2GOC : MonoBehaviour
    {
        private void Start()
        {
            this.SetText();
        }

        private void Update()
        {
            this.SetText();
        }

        private void SetText()
        {
            if (this.setForCount_visible != Get.QuestManager.VisibleQuestsCount || this.setForCount_historical != Get.QuestManager.HistoricalQuestsCount)
            {
                this.setForCount_visible = Get.QuestManager.VisibleQuestsCount;
                this.setForCount_historical = Get.QuestManager.HistoricalQuestsCount;
                base.GetComponent<TextMeshPro>().text = Get.QuestManager.HistoricalQuestsCount.ToStringCached() + "/" + Get.QuestManager.VisibleQuestsCount.ToStringCached();
            }
        }

        private int setForCount_visible;

        private int setForCount_historical;
    }
}