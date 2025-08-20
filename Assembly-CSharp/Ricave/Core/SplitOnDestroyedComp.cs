using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class SplitOnDestroyedComp : EntityComp
    {
        public new SplitOnDestroyedCompProps Props
        {
            get
            {
                return (SplitOnDestroyedCompProps)base.Props;
            }
        }

        protected SplitOnDestroyedComp()
        {
        }

        public SplitOnDestroyedComp(Entity parent)
            : base(parent)
        {
        }

        public override IEnumerable<Instruction> MakeParentDestroyedInstructions(Vector3Int? posBeforeDestroy)
        {
            if (posBeforeDestroy != null)
            {
                int num;
                for (int i = 0; i < this.Props.Count; i = num + 1)
                {
                    Actor actor = Maker.Make<Actor>(this.Props.ActorSpec, delegate (Actor x)
                    {
                        Actor actor2 = base.Parent as Actor;
                        if (actor2 != null)
                        {
                            x.RampUp = actor2.RampUp;
                        }
                        else
                        {
                            Item item = base.Parent as Item;
                            if (item != null)
                            {
                                x.RampUp = item.RampUp;
                            }
                            else
                            {
                                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, false);
                            }
                        }
                        Actor actor3 = base.Parent as Actor;
                        if (actor3 != null)
                        {
                            x.IsBaby = actor3.IsBaby;
                        }
                        DifficultyUtility.AddConditionsForDifficulty(x);
                        x.CalculateInitialHPManaAndStamina();
                    }, false, false, true);
                    Vector3Int vector3Int = SpawnPositionFinder.Near(posBeforeDestroy.Value, actor, false, false, null);
                    foreach (Instruction instruction in InstructionSets_Entity.Spawn(actor, vector3Int, null))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    num = i;
                }
            }
            yield break;
            yield break;
        }
    }
}