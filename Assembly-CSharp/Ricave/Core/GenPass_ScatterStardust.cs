using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_ScatterStardust : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 417960225;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            if (Get.RunConfig.ProgressDisabled)
            {
                return;
            }
            List<Vector3Int> list = this.GetCellCandidates(memory.storeys).ToList<Vector3Int>();
            if (list.Count > 3)
            {
                list.RemoveRange(3, list.Count - 3);
            }
            int num = Calc.RoundToIntHalfUp((float)Get.RunSpec.StardustPerFloor * Get.Difficulty.StardustFactor * DungeonModifiersUtility.GetCurrentRunStardustFactor());
            if (Get.UnlockableManager.IsUnlocked(Get.Unlockable_StardustBoost, null))
            {
                num = Calc.RoundToIntHalfUp((float)num * 1.5f);
            }
            List<int> list2 = Calc.DistributeRandomly(list.Count, 5, 15, num).ToList<int>();
            for (int i = 0; i < list2.Count; i++)
            {
                if (list2[i] > 0)
                {
                    ItemGenerator.Stardust(list2[i]).Spawn(list[i]);
                }
            }
        }

        private IEnumerable<Vector3Int> GetCellCandidates(List<Storey> storeys)
        {
            foreach (Room room in storeys.SelectMany<Storey, Room>((Storey x) => x.Rooms).InRandomOrder<Room>())
            {
                Vector3Int vector3Int;
                if (room.Role != Room.LayoutRole.LockedBehindSilverDoor && room.Role != Room.LayoutRole.OptionalChallenge && room.Role != Room.LayoutRole.Secret && room.Role != Room.LayoutRole.Start && room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    yield return vector3Int;
                }
            }
            IEnumerator<Room> enumerator = null;
            yield break;
            yield break;
        }

        public const float StardustBoostFactor = 1.5f;
    }
}