using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Shop : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            if (memory.config.Floor == 0 || memory.config.Floor % 2 != 0)
            {
                return false;
            }
            if (!room.Storey.IsMainStorey)
            {
                return false;
            }
            CellCuboid cellCuboid;
            Vector3Int vector3Int;
            Func<Vector3Int, Vector3Int> func;
            if (!BiggestRectFinder.TryFindRotatedRectOfSize(room.Shape.InnerCuboid(1).BottomSurfaceCuboid, (Vector3Int x) => !world.AnyEntityAt(x) && world.CellsInfo.IsFloorUnderNoActors(x) && world.CellsInfo.AnyPermanentImpassableAt(x.Below()) && !room.IsEntranceCellToAvoidBlockingOnlyFromBelow(x), 5, 4, out cellCuboid, out vector3Int, out func))
            {
                return false;
            }
            Vector3Int vector3Int2 = func(new Vector3Int(2, 0, 1));
            Vector3Int vector3Int3 = func(new Vector3Int(1, 0, 2));
            Vector3Int vector3Int4 = func(new Vector3Int(2, 0, 2));
            Vector3Int vector3Int5 = func(new Vector3Int(3, 0, 2));
            Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(vector3Int3);
            Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(vector3Int4);
            Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(vector3Int5);
            Structure structure = Maker.Make<Structure>(Get.Entity_Shop, null, false, false, true);
            Actor actor = Maker.Make<Actor>(Get.Entity_Shopkeeper, delegate (Actor x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
            DifficultyUtility.AddConditionsForDifficulty(actor);
            actor.CalculateInitialHPManaAndStamina();
            structure.InnerEntities.Add(actor);
            structure.Spawn(vector3Int2, vector3Int);
            Item item;
            if (Rand.Chance(0.3f))
            {
                item = ItemGenerator.Amulet(true);
                item.PriceTag = new PriceTag(Get.Entity_Gold, 300, true);
            }
            else
            {
                item = ItemGenerator.GoodReward();
                item.PriceTag = new PriceTag(Get.Entity_Gold, 400, true);
            }
            item.Spawn(vector3Int3);
            Item item2 = ItemGenerator.GoodReward();
            item2.PriceTag = new PriceTag(Get.Entity_Gold, 400, true);
            item2.Spawn(vector3Int4);
            Item item3;
            if (Get.Player.PartyHasAnyConditionOfSpec(Get.Condition_Disease) && !Get.Player.PartyHasAnyItemOfSpec(Get.Entity_FishOil))
            {
                item3 = Maker.Make<Item>(Get.Entity_FishOil, null, false, false, true);
                item3.PriceTag = new PriceTag(Get.Entity_Gold, 200, true);
            }
            else
            {
                item3 = Maker.Make<Item>(Get.Entity_Bread, delegate (Item x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                }, false, false, true);
                item3.PriceTag = new PriceTag(Get.Entity_Gold, 200, true);
            }
            item3.Spawn(vector3Int5);
            return true;
        }
    }
}