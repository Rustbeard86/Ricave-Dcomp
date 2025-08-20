using System;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_AddFloatingText : Instruction
    {
        public Target Above
        {
            get
            {
                return this.above;
            }
        }

        public string Text
        {
            get
            {
                return this.text;
            }
        }

        public Color Color
        {
            get
            {
                return this.color;
            }
        }

        public float Scale
        {
            get
            {
                return this.scale;
            }
        }

        public float Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_AddFloatingText()
        {
        }

        public Instruction_AddFloatingText(Target above, string text, Color color, float scale = 1f, float offset = 0f, float worldPosOffset = 0f, BodyPart bodyPart = null)
        {
            this.above = above;
            this.text = text;
            this.color = color;
            this.scale = scale;
            this.offset = offset;
            this.worldPosOffset = worldPosOffset;
            this.bodyPart = bodyPart;
        }

        protected override void DoImpl()
        {
            this.AddText();
        }

        protected override void UndoImpl()
        {
            this.AddText();
        }

        private void AddText()
        {
            Vector3 vector;
            if (this.bodyPart != null)
            {
                ActorGOC actorGOC = (this.above.Entity as Actor).ActorGOC;
                if (actorGOC != null)
                {
                    vector = actorGOC.GetBodyPartCenterWorldPos(this.bodyPart);
                    goto IL_005B;
                }
            }
            vector = this.above.RenderPosition.WithAddedY(this.worldPosOffset) + Vector3.up * 0.37f;
        IL_005B:
            Get.FloatingTexts.Add(this.text, vector, this.color, this.scale, this.offset);
        }

        [Saved]
        private Target above;

        [Saved]
        private string text;

        [Saved]
        private Color color;

        [Saved]
        private float scale;

        [Saved]
        private float offset;

        [Saved]
        private float worldPosOffset;

        [Saved]
        private BodyPart bodyPart;

        private const float OffsetY = 0.37f;
    }
}