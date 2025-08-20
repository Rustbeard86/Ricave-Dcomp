using System;
using System.Linq;
using Ricave.Rendering;

namespace Ricave.Core
{
    public class BodyPart
    {
        public string Label
        {
            get
            {
                return this.Placement.Label;
            }
        }

        public string LabelCap
        {
            get
            {
                return this.Placement.LabelCap;
            }
        }

        public int PlacementIndex
        {
            get
            {
                return this.placementIndex;
            }
        }

        public BodyPartPlacement Placement
        {
            get
            {
                return this.actor.Spec.Actor.BodyParts[this.placementIndex];
            }
        }

        public BodyPartSpec Spec
        {
            get
            {
                return this.Placement.Spec;
            }
        }

        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public int HP
        {
            get
            {
                return this.hp;
            }
            set
            {
                if (this.hp == value)
                {
                    return;
                }
                Instruction.ThrowIfNotExecuting();
                this.hp = value;
                ActorGOC actorGOC = this.actor.ActorGOC;
                if (actorGOC == null)
                {
                    return;
                }
                actorGOC.OnBodyPartChanged();
            }
        }

        public int MaxHP
        {
            get
            {
                return this.maxHP;
            }
        }

        public bool IsMissing
        {
            get
            {
                return this.hp <= 0;
            }
        }

        public bool Harmed
        {
            get
            {
                return this.hp < this.maxHP;
            }
        }

        public int StableID
        {
            get
            {
                return this.stableID;
            }
        }

        public int MyStableHash
        {
            get
            {
                return this.stableID;
            }
        }

        protected BodyPart()
        {
        }

        public BodyPart(Actor actor, int placementIndex)
        {
            this.actor = actor;
            this.placementIndex = placementIndex;
            if (Get.World == null)
            {
                this.stableID = Rand.UniqueID(actor.BodyParts.Select<BodyPart, int>((BodyPart x) => x.StableID));
            }
            else
            {
                this.stableID = Rand.UniqueID(from x in Get.World.Actors.SelectMany<Actor, BodyPart>((Actor x) => x.BodyParts).Concat<BodyPart>(actor.BodyParts)
                                              select x.StableID);
            }
            this.CalculateInitialMaxHP();
        }

        public void CalculateInitialMaxHP()
        {
            Instruction.ThrowIfNotExecuting();
            this.maxHP = Math.Max(Calc.RoundToIntHalfUp((float)this.actor.MaxHP * (this.Placement.MaxHPPctOverride ?? this.Spec.MaxHPPct)), 1);
            this.hp = this.maxHP;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private int placementIndex;

        [Saved]
        private int hp;

        [Saved]
        private int maxHP;

        [Saved(-1, false)]
        private int stableID = -1;
    }
}