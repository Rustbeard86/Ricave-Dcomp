using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_CutsceneWorld : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 876890767;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            MapFromString.Generate("\r\n        .....\r\n        .###.\r\n        .#b#.\r\n        .# #.\r\n        .#c#.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .#a#.\r\n        .###.\r\n        .....", new Action<int, int, char>(this.HandleSymbol), memory, 3);
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
            GenPass_CutsceneWorld.<> c__DisplayClass4_0 CS$<> 8__locals1;
            Vector3Int vector3Int;
            Vector3Int vector3Int2;
            IEnumerable<Vector3Int> enumerable;
            MapFromString.GetCoordinates(x, z, out CS$<> 8__locals1.floor, out vector3Int, out vector3Int2, out CS$<> 8__locals1.ceiling, out enumerable, 3);
            if (symbol <= '#')
            {
                if (symbol == ' ')
                {
                    GenPass_CutsceneWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
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
                    case 'a':
                        break;
                    case 'b':
                        GenPass_CutsceneWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_SurgicalBed, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'c':
                        GenPass_CutsceneWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_CutsceneTrigger, null, false, false, true).Spawn(vector3Int);
                        return;
                    default:
                        return;
                }
            }
            GenPass_CutsceneWorld.< HandleSymbol > g__FloorAndCeiling | 4_0(ref CS$<> 8__locals1);
            Get.WorldGenMemory.playerStartPos = vector3Int;
        }

        [CompilerGenerated]
        internal static void <HandleSymbol>g__FloorAndCeiling|4_0(ref GenPass_CutsceneWorld.<>c__DisplayClass4_0 A_0)
		{
            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.floor);
        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.ceiling);
		}

		private const string Map = "\r\n        .....\r\n        .###.\r\n        .#b#.\r\n        .# #.\r\n        .#c#.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .# #.\r\n        .#a#.\r\n        .###.\r\n        .....";
    }
}