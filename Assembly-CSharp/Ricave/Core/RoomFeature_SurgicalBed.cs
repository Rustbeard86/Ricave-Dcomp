using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_SurgicalBed : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!this.PlayerPartyHasAnyMissingBodyPart() || !room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_SurgicalBed, null, false, false, true).Spawn(vector3Int);
            return true;
        }

        private bool PlayerPartyHasAnyMissingBodyPart()
        {
            foreach (Actor actor in Get.PlayerParty)
            {
                using (List<BodyPart>.Enumerator enumerator2 = actor.BodyParts.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        if (enumerator2.Current.IsMissing)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}