using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class RootGOC : MonoBehaviour
    {
        private void Start()
        {
            Root.OnSceneChanged();
        }

        private void FixedUpdate()
        {
            Root.FixedUpdate();
        }

        private void Update()
        {
            Root.Update();
        }

        private void LateUpdate()
        {
            Root.LateUpdate();
        }

        private void OnGUI()
        {
            Root.OnGUI();
        }
    }
}