using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_SetSpawnerActive : UseEffect
    {
        public bool SetActive
        {
            get
            {
                return this.setActive;
            }
        }

        protected UseEffect_SetSpawnerActive()
        {
        }

        public UseEffect_SetSpawnerActive(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_SetSpawnerActive)clone).setActive = this.setActive;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity entity = usable as Entity;
            SpawnerComp spawnerComp = ((entity != null) ? entity.GetComp<SpawnerComp>() : null);
            if (spawnerComp == null || spawnerComp.Active == this.setActive)
            {
                yield break;
            }
            yield return new Instruction_Spawner_SetActive(spawnerComp, this.setActive);
            yield break;
        }

        [Saved(true, false)]
        private bool setActive = true;
    }
}