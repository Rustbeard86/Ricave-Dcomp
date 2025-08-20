using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class SpawnerComp : EntityComp
    {
        public new SpawnerCompProps Props
        {
            get
            {
                return (SpawnerCompProps)base.Props;
            }
        }

        public int TurnsPassed
        {
            get
            {
                return this.turnsPassed;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.turnsPassed = value;
            }
        }

        public List<Entity> SpawnedEntities
        {
            get
            {
                return this.spawnedEntities;
            }
        }

        public bool Active
        {
            get
            {
                return this.active;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.active = value;
            }
        }

        public override string ExtraTip
        {
            get
            {
                if (!this.active)
                {
                    return null;
                }
                string text = "{0}: ".Formatted("SpawnsNextIn".Translate(RichText.Label(this.Props.EntitySpec, false)));
                if (this.ShouldSpawnMore)
                {
                    text = text.Concatenated(RichText.Turns(StringUtility.TurnsString(this.Props.IntervalTurns - this.turnsPassed)));
                }
                else
                {
                    text = text.Concatenated("WontSpawnMore".Translate());
                }
                return text;
            }
        }

        private bool ShouldSpawnMore
        {
            get
            {
                if (!this.active)
                {
                    return false;
                }
                int num = 0;
                for (int i = this.spawnedEntities.Count - 1; i >= 0; i--)
                {
                    if (this.spawnedEntities[i].Spawned)
                    {
                        num++;
                        if (num >= this.Props.Count)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        protected SpawnerComp()
        {
        }

        public SpawnerComp(Entity parent)
            : base(parent)
        {
            if (!(parent is SequenceableStructure))
            {
                Log.Warning("SpawnerComp requires entity of type SequenceableStructure. Entity: " + ((parent != null) ? parent.ToString() : null), false);
            }
            this.active = this.Props.ActiveByDefault;
        }

        public override IEnumerable<Instruction> MakeResolveStructureInstructions()
        {
            if (!this.ShouldSpawnMore)
            {
                yield break;
            }
            if (this.turnsPassed >= this.Props.IntervalTurns - 1)
            {
                if (this.turnsPassed != 0)
                {
                    yield return new Instruction_Spawner_ChangeTurnsPassed(this, -this.turnsPassed);
                }
                Entity toSpawn = Maker.Make(this.Props.EntitySpec, delegate (Entity x)
                {
                    Actor actor = x as Actor;
                    if (actor != null)
                    {
                        actor.RampUp = RampUpUtility.GenerateRandomRampUpFor(actor, true);
                        DifficultyUtility.AddConditionsForDifficulty(actor);
                        actor.CalculateInitialHPManaAndStamina();
                        return;
                    }
                    Item item = x as Item;
                    if (item != null)
                    {
                        item.RampUp = RampUpUtility.GenerateRandomRampUpFor(item, true);
                    }
                }, false, false, true);
                foreach (Instruction instruction in InstructionSets_Entity.Spawn(toSpawn, SpawnPositionFinder.Near(base.Parent.Position, toSpawn, false, false, null), null))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                yield return new Instruction_Spawner_AddToSpawnedEntities(toSpawn, this);
                toSpawn = null;
            }
            else
            {
                yield return new Instruction_Spawner_ChangeTurnsPassed(this, 1);
            }
            yield break;
            yield break;
        }

        [Saved(Default.New, true)]
        private List<Entity> spawnedEntities = new List<Entity>();

        [Saved]
        private int turnsPassed;

        [Saved]
        private bool active;
    }
}