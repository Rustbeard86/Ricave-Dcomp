using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class InheritFOVGOC : MonoBehaviour
    {
        private void Start()
        {
            this.me = base.GetComponent<Camera>();
        }

        private void OnPreRender()
        {
            this.me.fieldOfView = Get.Camera.fieldOfView;
        }

        private Camera me;
    }
}