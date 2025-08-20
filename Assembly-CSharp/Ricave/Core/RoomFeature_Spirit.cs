using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Spirit : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (Get.Place == null || !Get.Place.HasSpirit)
            {
                return false;
            }
            if (Get.Player.SpiritsSetFree >= Get.RunSpec.SpiritsCount)
            {
                return false;
            }
            if (room.Role == Room.LayoutRole.End)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.TryGetRandomElementWhere<Vector3Int>(delegate (Vector3Int x)
            {
                Vector3Int vector3Int3;
                return !Get.World.AnyEntityNearby(x, 2, Get.Entity_Portal) && this.TryFindAdjacentPosForSpirit(x, out vector3Int3);
            }, out vector3Int))
            {
                return false;
            }
            Vector3Int vector3Int2;
            if (!this.TryFindAdjacentPosForSpirit(vector3Int, out vector3Int2))
            {
                return false;
            }
            ChainPost chainPost = Maker.Make<ChainPost>(Get.Entity_ChainPost, null, false, false, true);
            chainPost.Spawn(vector3Int);
            Actor actor = Maker.Make<Actor>(Get.Entity_Spirit, delegate (Actor x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
            DifficultyUtility.AddConditionsForDifficulty(actor);
            actor.CalculateInitialHPManaAndStamina();
            actor.Name = NameGenerator.Name(true);
            actor.Spawn(vector3Int2);
            actor.AttachedToChainPostDirect = chainPost;
            chainPost.AttachedActorDirect = actor;
            return true;
        }

        private bool TryFindAdjacentPosForSpirit(Vector3Int chainPostPos, out Vector3Int spiritPos)
        {
            return (from x in chainPostPos.AdjacentCardinalCellsXZ()
                    where x.InBounds() && !Get.World.AnyEntityAt(x) && Get.CellsInfo.IsFloorUnderNoActors(x)
                    select x).TryGetRandomElement<Vector3Int>(out spiritPos);
        }
    }
}