using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class Condition_Drunk : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override bool AnyForcedAction
        {
            get
            {
                return true;
            }
        }

        public override bool CanJumpOffLedge
        {
            get
            {
                return true;
            }
        }

        public override bool AllowPathingIntoAIAvoids
        {
            get
            {
                return true;
            }
        }

        protected Condition_Drunk()
        {
        }

        public Condition_Drunk(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_Drunk(ConditionSpec spec, int turnsLeft)
            : base(spec, turnsLeft)
        {
        }

        public override void CopyFieldsTo(Condition clone)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override Action GetForcedAction(Actor actor)
        {
            return Condition_Drunk.GetForcedDrunkAction(actor);
        }

        public static Action GetForcedDrunkAction(Actor actor)
        {
            foreach (Vector3Int vector3Int in actor.Position.AdjacentCardinalCells().InRandomOrder<Vector3Int>())
            {
                if (Get.World.CanMoveFromTo(actor.Position, vector3Int, actor))
                {
                    return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, vector3Int, new float?(0.5f), true);
                }
            }
            return new Action_Wait(Get.Action_Wait, actor, actor.IsNowControlledActor ? new float?(0.5f) : null, "WaitsDrunk".Translate());
        }
    }
}