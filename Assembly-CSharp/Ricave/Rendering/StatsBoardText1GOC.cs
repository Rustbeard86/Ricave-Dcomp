using System;
using Ricave.Core;
using TMPro;
using UnityEngine;

namespace Ricave.Rendering
{
    public class StatsBoardText1GOC : MonoBehaviour
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
            if (this.setForName != Get.Progress.PlayerName)
            {
                this.setForName = Get.Progress.PlayerName;
                base.GetComponent<TextMeshPro>().text = Get.Progress.PlayerName;
            }
        }

        private string setForName;
    }
}