using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class SignPlaceIconGOC : MonoBehaviour
    {
        private void Start()
        {
            base.gameObject.SetActive(Get.PlaceSpec == Get.Place_Shelter);
        }
    }
}