using System;
using Ricave.Rendering;
using UnityEngine;

namespace CoreEditor
{
    public class LightCookieGenerator : MonoBehaviour
    {
        private void Start()
        {
            CubemapMaker.MakeCubemap((Color x) => new Color(x.r, x.g, x.b, x.r));
        }

        private void LateUpdate()
        {
            CubemapMaker.LateUpdate();
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
            {
                CubemapMaker.MakeCubemap((Color x) => new Color(x.r, x.g, x.b, x.r));
            }
        }
    }
}