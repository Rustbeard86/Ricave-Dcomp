using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_ResolveDescendTriggerPlayerStartPos : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 613950441;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            if (!memory.config.UsedDescendTrigger)
            {
                return;
            }
            World world = Get.World;
            IEnumerable<Entity> enumerable = world.GetEntitiesOfSpec(Get.Entity_CeilingBars).Concat<Entity>(world.GetEntitiesOfSpec(Get.Entity_CeilingBarsReinforced)).Where<Entity>(delegate (Entity x)
            {
                Vector3Int vector3Int = x.Position.Below();
                if (!world.InBounds(vector3Int) || world.AnyEntityAt(vector3Int))
                {
                    return false;
                }
                List<RetainedRoomInfo.RoomInfo> rooms = vector3Int.GetRooms();
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].Role == Room.LayoutRole.LockedBehindGate || rooms[i].Role == Room.LayoutRole.LockedBehindSilverDoor || rooms[i].Role == Room.LayoutRole.OptionalChallenge || rooms[i].Role == Room.LayoutRole.Secret)
                    {
                        return false;
                    }
                }
                return true;
            });
            Entity entity;
            if (enumerable.Where<Entity>(delegate (Entity x)
            {
                Vector3Int vector3Int2 = x.Position;
                int num = 0;
                for (; ; )
                {
                    vector3Int2 += Vector3IntUtility.Down;
                    if (!world.InBounds(vector3Int2) || !world.CellsInfo.CanPassThrough(vector3Int2))
                    {
                        break;
                    }
                    num++;
                }
                return num <= 2;
            }).TryGetRandomElement<Entity>(out entity) || enumerable.TryGetRandomElement<Entity>(out entity))
            {
                memory.playerStartPos = entity.Position.Below();
                entity.DeSpawn(false);
                Maker.Make((entity.Spec == Get.Entity_CeilingBarsReinforced) ? Get.Entity_DamagedCeilingBarsReinforced : Get.Entity_DamagedCeilingBars, null, false, false, true).Spawn(entity.Position);
            }
        }
    }
}