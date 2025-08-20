using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_TutorialWorld : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 869416033;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            MapFromString.Generate("\r\n        #####\r\n        #bl #\r\n        #   #\r\n        ##d##\r\n        # k #\r\n        #   #\r\n        ##d##\r\n        #   #\r\n        #bj #\r\n        #   #\r\n        ##d##\r\n        #   #\r\n        # i #\r\n        #   #\r\n        ##d##\r\n        #bhh#\r\n        #hh #\r\n        #  h#\r\n        ##d##\r\n        #   #\r\n        # g #\r\n        #   #\r\n        ##d##\r\n        #   #\r\n        #bf #\r\n        #   #\r\n        ##e##\r\n        #   #\r\n        #^^^#\r\n        # c #\r\n        ##d##\r\n        #m m#\r\n        #   #\r\n        #mam#\r\n        #####", new Action<int, int, char>(this.HandleSymbol), memory, 4);
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
            GenPass_TutorialWorld.<> c__DisplayClass4_0 CS$<> 8__locals1;
            Vector3Int vector3Int;
            Vector3Int vector3Int2;
            IEnumerable<Vector3Int> enumerable;
            MapFromString.GetCoordinates(x, z, out CS$<> 8__locals1.floor, out vector3Int, out vector3Int2, out CS$<> 8__locals1.ceiling, out enumerable, 4);
            if (symbol <= '#')
            {
                if (symbol == ' ')
                {
                    GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
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
                        GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int);
                        return;
                    case '_':
                    case '`':
                        return;
                    case 'a':
                        break;
                    case 'b':
                        GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Torch, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'c':
                        GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Ladder, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'd':
                        GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                        Maker.Make(Get.Entity_Gate, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'e':
                        GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                        Maker.Make(Get.Entity_Door, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'f':
                        GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Sword, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'g':
                        GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Skeleton, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'h':
                        {
                            GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                            Structure structure = Maker.Make<Structure>(Get.Entity_Crates, null, false, false, true);
                            structure.InnerEntities.Add(ItemGenerator.Gold(Rand.RangeInclusive(5, 10)));
                            structure.Spawn(vector3Int);
                            return;
                        }
                    case 'i':
                        {
                            GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                            Structure structure2 = Maker.Make<Structure>(Get.Entity_Chest, null, false, false, true);
                            structure2.InnerEntities.Add(Maker.Make<Item>(Get.Entity_Potion_Health, null, false, false, true));
                            structure2.Spawn(vector3Int, Vector3IntUtility.Back);
                            return;
                        }
                    case 'j':
                        GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Skeleton, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'k':
                        GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Lever, null, false, false, true).Spawn(vector3Int);
                        Maker.Make(Get.Entity_StaircaseRoomSign, null, false, false, true).Spawn(vector3Int2);
                        return;
                    case 'l':
                        GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_TutorialEndPortal, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'm':
                        Maker.Make(Get.Entity_FloorBarsReinforced, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                        Maker.Make(Rand.Bool ? Get.Entity_Floor : Get.Entity_CeilingBars, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                        return;
                    default:
                        return;
                }
            }
            GenPass_TutorialWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
            Get.WorldGenMemory.playerStartPos = vector3Int;
        }

        [CompilerGenerated]
        internal static void <HandleSymbol>g__FloorAndCeiling|4_0(ref GenPass_TutorialWorld.<>c__DisplayClass4_0 A_0)
		{
            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.floor);
        Maker.Make(Rand.Bool? Get.Entity_Floor : Get.Entity_CeilingBars, null, false, false, true).Spawn(A_0.ceiling);
		}

		private const string Map = "\r\n        #####\r\n        #bl #\r\n        #   #\r\n        ##d##\r\n        # k #\r\n        #   #\r\n        ##d##\r\n        #   #\r\n        #bj #\r\n        #   #\r\n        ##d##\r\n        #   #\r\n        # i #\r\n        #   #\r\n        ##d##\r\n        #bhh#\r\n        #hh #\r\n        #  h#\r\n        ##d##\r\n        #   #\r\n        # g #\r\n        #   #\r\n        ##d##\r\n        #   #\r\n        #bf #\r\n        #   #\r\n        ##e##\r\n        #   #\r\n        #^^^#\r\n        # c #\r\n        ##d##\r\n        #m m#\r\n        #   #\r\n        #mam#\r\n        #####";
    }
}