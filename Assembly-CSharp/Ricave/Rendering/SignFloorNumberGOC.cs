using System;
using Ricave.Core;
using TMPro;
using UnityEngine;

namespace Ricave.Rendering
{
    public class SignFloorNumberGOC : MonoBehaviour
    {
        private void Start()
        {
            if (Get.PlaceSpec == Get.Place_Shelter)
            {
                base.gameObject.SetActive(false);
                return;
            }
            base.GetComponent<TextMeshPro>().text = Get.Floor.ToStringCached();
        }
    }
}