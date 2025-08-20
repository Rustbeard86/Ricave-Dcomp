using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class CollectedNotesVisualGOC : MonoBehaviour
    {
        private void Start()
        {
            this.noteMesh = base.GetComponent<MeshFilter>().sharedMesh;
            this.noteMaterial = base.GetComponent<MeshRenderer>().sharedMaterial;
            base.gameObject.SetActive(Get.Progress.CollectedNoteSpecs.Count > 0);
        }

        private void Update()
        {
            int count = Get.Progress.CollectedNoteSpecs.Count;
            float num = 0f;
            float num2 = 0f;
            num += 0.08f;
            for (int i = 1; i < count; i++)
            {
                Graphics.DrawMesh(this.noteMesh, base.transform.parent.localToWorldMatrix * Matrix4x4.TRS(base.transform.localPosition + new Vector3(num, num2, 0f), base.transform.localRotation, base.transform.localScale), this.noteMaterial, 0);
                if ((i + 1) % 6 == 0)
                {
                    num = 0f;
                    num2 -= 0.08f;
                }
                else
                {
                    num += 0.08f;
                }
            }
        }

        private Mesh noteMesh;

        private Material noteMaterial;
    }
}