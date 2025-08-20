using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class SkyGOC : MonoBehaviour
    {
        private void Start()
        {
            base.gameObject.GetComponent<MeshRenderer>().sharedMaterial = Get.WeatherManager.CurrentSkyMaterial;
        }
    }
}