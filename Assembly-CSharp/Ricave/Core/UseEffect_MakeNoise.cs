using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_MakeNoise : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public NoiseType Type
        {
            get
            {
                return this.type;
            }
        }

        public int Radius
        {
            get
            {
                return this.radius;
            }
        }

        public bool AtTarget
        {
            get
            {
                return this.atTarget;
            }
        }

        public bool AtSource
        {
            get
            {
                return this.atSource;
            }
        }

        public bool WholeMap
        {
            get
            {
                return this.wholeMap;
            }
        }

        public bool ThroughWalls
        {
            get
            {
                return this.throughWalls;
            }
        }

        public override string LabelBase
        {
            get
            {
                string text = base.Spec.Label;
                if (this.wholeMap)
                {
                    text = text.Concatenated(RichText.AoERadius(" ({0})".Formatted("WholeMap".Translate())));
                }
                else if (this.throughWalls)
                {
                    text = text.Concatenated(RichText.AoERadius(" ({0} {1})".Formatted("AoERadius".Translate(this.radius), "ThroughWalls".Translate())));
                }
                else
                {
                    text = text.Concatenated(RichText.AoERadius(" ({0})".Formatted("AoERadius".Translate(this.radius))));
                }
                return text;
            }
        }

        protected UseEffect_MakeNoise()
        {
        }

        public UseEffect_MakeNoise(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public UseEffect_MakeNoise(UseEffectSpec spec, NoiseType type, int radius, bool atTarget, bool atSource, bool throughWalls)
            : base(spec, 1f, 0, null, false)
        {
            this.type = type;
            this.radius = radius;
            this.atTarget = atTarget;
            this.atSource = atSource;
            this.throughWalls = throughWalls;
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_MakeNoise useEffect_MakeNoise = (UseEffect_MakeNoise)clone;
            useEffect_MakeNoise.type = this.type;
            useEffect_MakeNoise.radius = this.radius;
            useEffect_MakeNoise.atTarget = this.atTarget;
            useEffect_MakeNoise.atSource = this.atSource;
            useEffect_MakeNoise.wholeMap = this.wholeMap;
            useEffect_MakeNoise.throughWalls = this.throughWalls;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Vector3Int vector3Int;
            if (this.atTarget)
            {
                vector3Int = target.Position;
            }
            else
            {
                vector3Int = UseEffect_Sound.GetSourcePos(user, usable, target);
            }
            foreach (Instruction instruction in InstructionSets_Noise.MakeNoise(vector3Int, this.wholeMap ? Math.Max(Math.Max(Get.World.Size.x, Get.World.Size.y), Get.World.Size.z) : this.radius, this.type, user, this.throughWalls || this.wholeMap, !this.wholeMap))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved(NoiseType.LoudInteraction, false)]
        private NoiseType type = NoiseType.LoudInteraction;

        [Saved]
        private int radius;

        [Saved]
        private bool atTarget;

        [Saved]
        private bool atSource;

        [Saved]
        private bool wholeMap;

        [Saved]
        private bool throughWalls;
    }
}