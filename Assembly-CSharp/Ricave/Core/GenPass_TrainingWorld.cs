using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_TrainingWorld : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 465589412;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            MapFromString.Generate("\r\n        .......\r\n        .#####.\r\n        .#bj #.\r\n        .# ^ #.\r\n        .#   #.\r\n        .#   #.\r\n        .##i##.\r\n        .#f c#.\r\n        .#g d#.\r\n        .#hae#.\r\n        .##k##.\r\n        ..###..\r\n        .......", new Action<int, int, char>(this.HandleSymbol), memory, 4);
            foreach (Structure structure in Get.World.Structures)
            {
                if (!structure.SpawnedWithSpecificRotationUnsaved)
                {
                    structure.AutoRotate();
                }
            }
        }

        private void HandleSymbol(int x, int z, char symbol)
        {
            GenPass_TrainingWorld.<> c__DisplayClass4_0 CS$<> 8__locals1;
            Vector3Int vector3Int;
            Vector3Int vector3Int2;
            IEnumerable<Vector3Int> enumerable;
            MapFromString.GetCoordinates(x, z, out CS$<> 8__locals1.floor, out vector3Int, out vector3Int2, out CS$<> 8__locals1.ceiling, out enumerable, 4);
            if (symbol <= '#')
            {
                if (symbol == ' ')
                {
                    GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                    return;
                }
                if (symbol != '#')
                {
                    return;
                }
                using (IEnumerator<Vector3Int> enumerator = enumerable.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Vector3Int vector3Int3 = enumerator.Current;
                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int3);
                    }
                    return;
                }
            }
            else
            {
                if (symbol == '.')
                {
                    return;
                }
                switch (symbol)
                {
                    case '^':
                        GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int);
                        return;
                    case '_':
                    case '`':
                        return;
                    case 'a':
                        break;
                    case 'b':
                        GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Torch, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'c':
                        GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        GenPass_TrainingWorld.< HandleSymbol > g__Identified | 4_1(Maker.Make<Item>(Get.Entity_Axe, null, false, false, true)).Spawn(vector3Int);
                        return;
                    case 'd':
                        GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        GenPass_TrainingWorld.< HandleSymbol > g__Identified | 4_1(Maker.Make<Item>(Get.Entity_Sword, null, false, false, true)).Spawn(vector3Int);
                        return;
                    case 'e':
                        GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        GenPass_TrainingWorld.< HandleSymbol > g__Identified | 4_1(Maker.Make<Item>(Get.Entity_Mace, null, false, false, true)).Spawn(vector3Int);
                        return;
                    case 'f':
                        GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        GenPass_TrainingWorld.< HandleSymbol > g__Identified | 4_1(Maker.Make<Item>(Get.Entity_MagicRobe, null, false, false, true)).Spawn(vector3Int);
                        Maker.Make(Get.Entity_Torch, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'g':
                        GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        GenPass_TrainingWorld.< HandleSymbol > g__Identified | 4_1(Maker.Make<Item>(Get.Entity_LeatherArmor, null, false, false, true)).Spawn(vector3Int);
                        return;
                    case 'h':
                        GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        GenPass_TrainingWorld.< HandleSymbol > g__Identified | 4_1(Maker.Make<Item>(Get.Entity_PlateArmor, null, false, false, true)).Spawn(vector3Int);
                        return;
                    case 'i':
                        GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                        Maker.Make(Get.Entity_Door, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'j':
                        Maker.Make(Get.Entity_FloorBarsReinforced, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                        this.GenerateEnemy().Spawn(vector3Int);
                        return;
                    case 'k':
                        GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_TrainingRoomEndPortal, null, false, false, true).Spawn(vector3Int);
                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                        return;
                    default:
                        return;
                }
            }
            GenPass_TrainingWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
            Get.WorldGenMemory.playerStartPos = vector3Int;
        }

        private Actor GenerateEnemy()
        {
            if (Get.RunConfig.TrainingBoss)
            {
                if (Get.RunConfig.TrainingActorSpec != null)
                {
                    return BossGenerator.Boss(Get.RunConfig.TrainingActorSpec, null, null);
                }
                return BossGenerator.Boss(true);
            }
            else
            {
                if (Get.RunConfig.TrainingActorSpec != null)
                {
                    return Maker.Make<Actor>(Get.RunConfig.TrainingActorSpec, null, false, false, true);
                }
                EntitySpec entitySpec;
                if ((from x in Get.Specs.GetAll<EntitySpec>()
                     where x.IsActor && x.Actor.KilledExperience > 0 && Get.Floor >= x.Actor.GenerateMinFloor && x.Actor.GenerateSelectionWeight > 0f && !x.Actor.AlwaysBoss
                     select x).TryGetRandomElement<EntitySpec>(out entitySpec, (EntitySpec x) => x.Actor.GenerateSelectionWeight))
                {
                    return Maker.Make<Actor>(entitySpec, null, false, false, true);
                }
                return Maker.Make<Actor>(Get.Entity_Skeleton, null, false, false, true);
            }
        }

        [CompilerGenerated]
        internal static void <HandleSymbol>g__FloorAndCeiling|4_0(ref GenPass_TrainingWorld.<>c__DisplayClass4_0 A_0)
		{
            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.floor);
        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.ceiling);
		}

		[CompilerGenerated]
        internal static Item<HandleSymbol> g__Identified|4_1(Item item)
		{
			item.TurnsLeftToIdentify = 0;
			return item;
		}

private const string Map = "\r\n        .......\r\n        .#####.\r\n        .#bj #.\r\n        .# ^ #.\r\n        .#   #.\r\n        .#   #.\r\n        .##i##.\r\n        .#f c#.\r\n        .#g d#.\r\n        .#hae#.\r\n        .##k##.\r\n        ..###..\r\n        .......";
	}
}