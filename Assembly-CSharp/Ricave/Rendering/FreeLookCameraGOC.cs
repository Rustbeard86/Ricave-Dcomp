using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class FreeLookCameraGOC : MonoBehaviour
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            this.rotX += Input.GetAxis("Mouse X") * 1.8f;
            this.rotY -= Input.GetAxis("Mouse Y") * 1.8f;
            this.rotX = Calc.Repeat(this.rotX, 360f);
            this.rotY = Calc.Clamp(this.rotY, -90f, 90f);
            base.transform.rotation = Quaternion.Euler(this.rotY, this.rotX, 0f);
        }

        private float rotX;

        private float rotY;

        private const float Sensitivity = 1.8f;
    }
}