using System;
using UnityEngine;

namespace Ricave.Rendering
{
    public class DownsamplingGOC : MonoBehaviour
    {
        public float Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                this.scale = value;
            }
        }

        private void Start()
        {
            this.me = base.GetComponent<Camera>();
        }

        private void OnPreRender()
        {
            this.me.rect = new Rect(0f, 0f, this.scale, this.scale);
        }

        private void OnPostRender()
        {
            this.me.rect = new Rect(0f, 0f, 1f, 1f);
        }

        private float scale = 1f;

        private Camera me;
    }
}