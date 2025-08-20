using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Cultists : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            CellCuboid cellCuboid;
            if (!BiggestRectFinder.TryFindRectOfSize(room.Shape.InnerCuboid(1).BottomSurfaceCuboid, (Vector3Int x) => !world.AnyEntityAt(x) && world.CellsInfo.IsFloorUnderNoActors(x) && world.CellsInfo.IsFilled(x.Below()) && !room.IsEntranceCellToAvoidBlockingOnlyFromBelow(x), 5, out cellCuboid))
            {
                return false;
            }
            Maker.Make(Get.Entity_CultistCircle, null, false, false, true).Spawn(cellCuboid.Center);
            Vector3Int vector3Int = cellCuboid.Center + new Vector3Int(-1, 0, 0);
            Vector3Int vector3Int2 = cellCuboid.Center + new Vector3Int(1, 0, 1);
            Vector3Int vector3Int3 = cellCuboid.Center + new Vector3Int(1, 0, -1);
            Faction cultFaction = new Faction(Get.Faction_Cult, FactionUtility.GenerateFactionName(Get.Faction_Cult));
            Get.FactionManager.AddFaction(cultFaction, -1);
            foreach (Faction faction in FactionUtility.DefaultHostileFactions(cultFaction))
            {
                Get.FactionManager.AddHostility(cultFaction, faction, -1);
            }
            Action<Actor> <> 9__1;
            for (int i = 0; i < 3; i++)
            {
                Vector3Int vector3Int4;
                switch (i)
                {
                    case 0:
                        vector3Int4 = vector3Int;
                        break;
                    case 1:
                        vector3Int4 = vector3Int2;
                        break;
                    case 2:
                        vector3Int4 = vector3Int3;
                        break;
                    default:
                        vector3Int4 = vector3Int;
                        break;
                }
                Item item;
                switch (Rand.RangeInclusive(0, 3))
                {
                    case 0:
                        item = ItemGenerator.Ring(false);
                        break;
                    case 1:
                        item = ItemGenerator.Amulet(false);
                        break;
                    case 2:
                        item = ItemGenerator.Wand();
                        break;
                    case 3:
                        item = ItemGenerator.Scroll(true);
                        break;
                    default:
                        item = null;
                        break;
                }
                EntitySpec entity_Cultist = Get.Entity_Cultist;
                Action<Actor> action;
                if ((action = <> 9__1) == null)
                {
                    action = (<> 9__1 = delegate (Actor x)
                    {
                        x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                        x.Faction = cultFaction;
                    });
                }
                Actor actor = Maker.Make<Actor>(entity_Cultist, action, false, false, true);
                DifficultyUtility.AddConditionsForDifficulty(actor);
                actor.CalculateInitialHPManaAndStamina();
                actor.Inventory.Add(item, default(ValueTuple<Vector2Int?, int?, int?>));
                actor.Spawn(vector3Int4);
            }
            return true;
        }
    }
}