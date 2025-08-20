using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class WorldSituation : ITipSubject
    {
        public string Label
        {
            get
            {
                return this.spec.Label;
            }
        }

        public string LabelCap
        {
            get
            {
                return this.spec.LabelCap;
            }
        }

        public string Description
        {
            get
            {
                return this.spec.Description;
            }
        }

        public WorldSituationSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public int MyStableHash
        {
            get
            {
                return this.spec.MyStableHash;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.spec.Icon;
            }
        }

        public Color IconColor
        {
            get
            {
                return this.spec.IconColor;
            }
        }

        public float TimeAdded
        {
            get
            {
                return this.timeAdded;
            }
            set
            {
                this.timeAdded = value;
            }
        }

        public int? SequenceWhenAdded
        {
            get
            {
                return this.sequenceWhenAdded;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.sequenceWhenAdded = value;
            }
        }

        public virtual ValueTuple<Color, float>? FogOverride
        {
            get
            {
                return null;
            }
        }

        public virtual float AmbientLightFactor
        {
            get
            {
                return 1f;
            }
        }

        protected WorldSituation()
        {
        }

        public WorldSituation(WorldSituationSpec spec)
        {
            this.spec = spec;
            if (spec.WorldSituationClass != base.GetType())
            {
                string[] array = new string[5];
                array[0] = "Created a WorldSituation with spec ";
                array[1] = ((spec != null) ? spec.ToString() : null);
                array[2] = ", but the created WorldSituation type is ";
                int num = 3;
                Type type = base.GetType();
                array[num] = ((type != null) ? type.ToString() : null);
                array[4] = ".";
                Log.Warning(string.Concat(array), false);
            }
        }

        public virtual IEnumerable<Instruction> MakeResolveWorldInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public virtual IEnumerable<Instruction> MakePostAddInstructions()
        {
            yield return new Instruction_SetWorldSituationSequenceWhenAdded(this, Get.TurnManager.CurrentSequence);
            if (this.spec.EveryoneConditions != null && this.spec.EveryoneConditions.Any)
            {
                foreach (Actor actor in Get.World.Actors.ToTemporaryList<Actor>())
                {
                    if (actor.Spawned)
                    {
                        foreach (Condition condition in this.spec.EveryoneConditions.All)
                        {
                            Condition condition2 = condition.Clone();
                            condition2.OriginWorldSituation = this;
                            foreach (Instruction instruction in InstructionSets_Misc.AddCondition(condition2, actor.Conditions, false, false))
                            {
                                yield return instruction;
                            }
                            IEnumerator<Instruction> enumerator3 = null;
                        }
                        List<Condition>.Enumerator enumerator2 = default(List<Condition>.Enumerator);
                        actor = null;
                    }
                }
                List<Actor>.Enumerator enumerator = default(List<Actor>.Enumerator);
            }
            yield break;
            yield break;
        }

        public virtual IEnumerable<Instruction> MakePostRemoveInstructions()
        {
            if (this.spec.EveryoneConditions != null && this.spec.EveryoneConditions.Any)
            {
                foreach (Actor actor in Get.World.Actors.ToTemporaryList<Actor>())
                {
                    if (actor.Spawned)
                    {
                        foreach (Condition condition in actor.Conditions.All.ToTemporaryList<Condition>())
                        {
                            if (condition.OriginWorldSituation == this)
                            {
                                foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(condition, false, false))
                                {
                                    yield return instruction;
                                }
                                IEnumerator<Instruction> enumerator3 = null;
                            }
                        }
                        List<Condition>.Enumerator enumerator2 = default(List<Condition>.Enumerator);
                    }
                }
                List<Actor>.Enumerator enumerator = default(List<Actor>.Enumerator);
            }
            yield break;
            yield break;
        }

        [Saved]
        private WorldSituationSpec spec;

        [Saved]
        private int? sequenceWhenAdded;

        private float timeAdded = -99999f;
    }
}