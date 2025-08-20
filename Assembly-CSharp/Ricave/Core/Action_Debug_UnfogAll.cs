using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Debug_UnfogAll : Action
    {
        protected override int RandSeedPart
        {
            get
            {
                return 284019461;
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose;
            }
        }

        protected Action_Debug_UnfogAll()
        {
        }

        public Action_Debug_UnfogAll(ActionSpec spec)
            : base(spec)
        {
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            List<Vector3Int> list = (from x in Get.RetainedRoomInfo.Rooms.SelectMany<RetainedRoomInfo.RoomInfo, Vector3Int>((RetainedRoomInfo.RoomInfo x) => x.Shape).Distinct<Vector3Int>()
                                     where Get.FogOfWar.IsFogged(x)
                                     select x).ToList<Vector3Int>();
            yield return new Instruction_Unfog(list);
            yield break;
        }
    }
}