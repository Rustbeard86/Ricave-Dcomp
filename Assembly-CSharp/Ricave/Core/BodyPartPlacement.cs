using System;
using UnityEngine;

namespace Ricave.Core
{
    public class BodyPartPlacement
    {
        public BodyPartSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public string Label
        {
            get
            {
                return this.customLabel ?? this.spec.Label;
            }
        }

        public Vector2 Offset
        {
            get
            {
                return this.offset;
            }
        }

        public BodyPartPlacement.BracketType Bracket
        {
            get
            {
                return this.bracketType;
            }
        }

        public float? MaxHPPctOverride
        {
            get
            {
                return this.maxHPPctOverride;
            }
        }

        public Rect OccupiedTextureRect
        {
            get
            {
                return this.occupiedTextureRect;
            }
            set
            {
                this.occupiedTextureRect = value;
            }
        }

        public string LabelCap
        {
            get
            {
                if (this.labelCapCached == null && this.Label != null)
                {
                    this.labelCapCached = this.Label.CapitalizeFirst();
                }
                return this.labelCapCached;
            }
        }

        [Saved]
        private BodyPartSpec spec;

        [Saved]
        [Translatable]
        private string customLabel;

        [Saved]
        private Vector2 offset;

        [Saved]
        private BodyPartPlacement.BracketType bracketType;

        [Saved]
        private float? maxHPPctOverride;

        private string labelCapCached;

        private Rect occupiedTextureRect;

        public enum BracketType
        {
            UpperLeft,

            UpperRight,

            BottomLeft,

            BottomRight,

            Top
        }
    }
}