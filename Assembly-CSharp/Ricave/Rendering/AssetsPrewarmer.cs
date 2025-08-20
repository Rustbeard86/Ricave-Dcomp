using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class AssetsPrewarmer
    {
        public bool Finished
        {
            get
            {
                return this.finished;
            }
        }

        public int FinishedFrame
        {
            get
            {
                return this.finishedFrame;
            }
        }

        public void Update()
        {
            if (!this.started)
            {
                this.started = true;
                this.CreateGameObjects();
                return;
            }
            if (!this.finished)
            {
                this.finished = true;
                this.finishedFrame = Clock.Frame;
                this.DestroyGameObjects();
            }
        }

        private void CreateGameObjects()
        {
            this.container = new GameObject("AssetsPrewarmer");
            this.container.transform.position = Get.CameraPosition + Get.CameraTransform.forward * 1f;
            this.container.transform.LookAt(Get.CameraPosition);
            this.container.transform.forward = -this.container.transform.forward;
            int num = (int)Calc.Sqrt((float)Calc.NextPerfectSquare(Assets.AllLoadedMaterials.Count));
            int num2 = 0;
            int num3 = 0;
            float num4 = 1f / (float)num;
            foreach (Material material in Assets.AllLoadedMaterials)
            {
                GameObject gameObject = new GameObject();
                gameObject.AddComponent<MeshFilter>().sharedMesh = PrimitiveMeshes.Get(PrimitiveType.Quad);
                gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
                gameObject.transform.SetParent(this.container.transform, false);
                gameObject.transform.localPosition = new Vector3((float)num2 * num4 - 0.5f, (float)num3 * num4 - 0.5f, 0f);
                gameObject.transform.localScale = new Vector3(num4, num4, num4);
                num2++;
                if (num2 >= num)
                {
                    num2 = 0;
                    num3++;
                }
            }
            foreach (Material material2 in Assets.AllLoadedMaterials)
            {
                GameObject gameObject2 = new GameObject();
                gameObject2.AddComponent<MeshFilter>().sharedMesh = PrimitiveMeshes.Get(PrimitiveType.Quad);
                gameObject2.AddComponent<MeshRenderer>().sharedMaterial = material2;
                gameObject2.transform.SetParent(this.container.transform, false);
                gameObject2.transform.localPosition = new Vector3(Rand.Range(-0.001f, 0.001f), Rand.Range(-0.001f, 0.001f), 0.05f);
            }
        }

        private void DestroyGameObjects()
        {
            Object.DestroyImmediate(this.container);
            this.container = null;
        }

        private bool started;

        private bool finished;

        private int finishedFrame;

        private GameObject container;

        private const float DistFromCamera = 1f;

        private const float Size = 1f;
    }
}