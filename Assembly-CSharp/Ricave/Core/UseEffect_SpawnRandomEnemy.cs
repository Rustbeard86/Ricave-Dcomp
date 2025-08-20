using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_SpawnRandomEnemy : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public bool AddPlayLog
        {
            get
            {
                return this.addPlayLog;
            }
        }

        protected UseEffect_SpawnRandomEnemy()
        {
        }

        public UseEffect_SpawnRandomEnemy(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_SpawnRandomEnemy)clone).addPlayLog = this.addPlayLog;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            EntitySpec entitySpec;
            if (!(from x in Get.Specs.GetAll<EntitySpec>()
                  where x.IsActor && x.Actor.KilledExperience > 0 && Get.Floor >= x.Actor.GenerateMinFloor && x.Actor.GenerateSelectionWeight > 0f && !x.Actor.AlwaysBoss
                  select x).TryGetRandomElement<EntitySpec>(out entitySpec, (EntitySpec x) => x.Actor.GenerateSelectionWeight))
            {
                yield break;
            }
            Actor actor = Maker.Make<Actor>(entitySpec, delegate (Actor x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, false);
                DifficultyUtility.AddConditionsForDifficulty(x);
                x.CalculateInitialHPManaAndStamina();
            }, false, false, true);
            Vector3Int vector3Int = SpawnPositionFinder.Near(target.Position, actor, false, false, null);
            foreach (Instruction instruction in InstructionSets_Entity.Spawn(actor, vector3Int, null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (this.addPlayLog)
            {
                yield return new Instruction_PlayLog("EnemySpawnedNearby".Translate(actor));
            }
            yield break;
            yield break;
        }

        [Saved]
        private bool addPlayLog;
    }
}