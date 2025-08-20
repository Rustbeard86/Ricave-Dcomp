using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_PlayerHitMarker : Instruction
    {
        public Vector3 Position
        {
            get
            {
                return this.position;
            }
        }

        public Color Color
        {
            get
            {
                return this.color;
            }
        }

        protected Instruction_PlayerHitMarker()
        {
        }

        public Instruction_PlayerHitMarker(Vector3 position, Color color)
        {
            this.position = position;
            this.color = color;
        }

        protected override void DoImpl()
        {
            Get.PlayerHitMarkers.Add(this.position, this.color);
        }

        protected override void UndoImpl()
        {
        }

        [Saved]
        private Vector3 position;

        [Saved]
        private Color color;
    }
}