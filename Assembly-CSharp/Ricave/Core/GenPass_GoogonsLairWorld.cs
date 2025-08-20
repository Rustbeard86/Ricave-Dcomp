using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_GoogonsLairWorld : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 713243667;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            this.mainRoomMin = default(Vector2Int);
            this.mainRoomMax = default(Vector2Int);
            this.endRoomMin = default(Vector2Int);
            this.endRoomMax = default(Vector2Int);
            MapFromString.Generate("\r\n        .............\r\n        ....####^....\r\n        ....# l #....\r\n        ....#knm#....\r\n        ....#   #....\r\n        .###^#h####%.\r\n        .# fq   qf #.\r\n        .#    p    #.\r\n        .#  i   j e#.\r\n        .#r   i    #.\r\n        .##g     g##.\r\n        .###bbbbb###.\r\n        .###c d c###.\r\n        .####e o####.\r\n        .####   ####.\r\n        .#### a ####.\r\n        .%##########.\r\n        .............", new Action<int, int, char>(this.HandleSymbol), memory, 4);
            Get.World.RetainedRoomInfo.Add(new CellCuboid(this.mainRoomMin.x, 0, this.mainRoomMin.y, this.mainRoomMax.x - this.mainRoomMin.x + 1, 4, this.mainRoomMax.y - this.mainRoomMin.y + 1), Get.Room_BossRoom, Room.LayoutRole.None, Get.Room_BossRoom.LabelCap);
            Get.World.RetainedRoomInfo.Add(new CellCuboid(this.endRoomMin.x, 0, this.endRoomMin.y, this.endRoomMax.x - this.endRoomMin.x + 1, 4, this.endRoomMax.y - this.endRoomMin.y + 1), Get.Room_RewardRoom, Room.LayoutRole.LockedBehindSilverDoor, Get.Room_RewardRoom.LabelCap);
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
            GenPass_GoogonsLairWorld.<> c__DisplayClass8_0 CS$<> 8__locals1;
            Vector3Int vector3Int;
            Vector3Int vector3Int2;
            IEnumerable<Vector3Int> enumerable;
            MapFromString.GetCoordinates(x, z, out CS$<> 8__locals1.floor, out vector3Int, out vector3Int2, out CS$<> 8__locals1.ceiling, out enumerable, 4);
            if (symbol <= '#')
            {
                if (symbol == ' ')
                {
                    GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
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
            else if (symbol != '%')
            {
                if (symbol == '.')
                {
                    return;
                }
                switch (symbol)
                {
                    case '^':
                        foreach (Vector3Int vector3Int4 in enumerable)
                        {
                            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int4);
                        }
                        if (this.endRoomMin == default(Vector2Int))
                        {
                            this.endRoomMin = new Vector2Int(x, z);
                            return;
                        }
                        this.endRoomMax = new Vector2Int(x, z);
                        return;
                    case '_':
                    case '`':
                        return;
                    case 'a':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Hatch, null, false, false, true).Spawn(vector3Int);
                        Get.WorldGenMemory.playerStartPos = vector3Int;
                        return;
                    case 'b':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Gate, null, false, false, true).Spawn(vector3Int);
                        Maker.Make(Get.Entity_Gate, null, false, false, true).Spawn(vector3Int2);
                        return;
                    case 'c':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int2);
                        return;
                    case 'd':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Lever, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'e':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_GreenWallLamp, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'f':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Vines, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'g':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Grass, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'h':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_GoldenDoor, null, false, false, true).Spawn(vector3Int);
                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                        return;
                    case 'i':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_GooPuddle, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'j':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_Pillar, null, false, false, true).Spawn(vector3Int);
                        Maker.Make(Get.Entity_Pillar, null, false, false, true).Spawn(vector3Int2);
                        return;
                    case 'k':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_HealingCrystal, null, false, false, true).Spawn(vector3Int);
                        return;
                    case 'l':
                        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                        Maker.Make(Get.Entity_Torch, null, false, false, true).Spawn(vector3Int);
                        if (Get.RunSpec.FloorCount != null)
                        {
                            int floor = Get.Floor;
                            int? floorCount = Get.RunSpec.FloorCount;
                            if ((floor == floorCount.GetValueOrDefault()) & (floorCount != null))
                            {
                                Maker.Make(Get.Entity_RunEndPortal, null, false, false, true).Spawn(vector3Int);
                                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int.Below());
                                return;
                            }
                        }
                        Maker.Make(Get.Entity_Staircase, null, false, false, true).Spawn(vector3Int);
                        Maker.Make(Get.Entity_InvisibleBlocker, null, false, false, true).Spawn(vector3Int.Below());
                        return;
                    case 'm':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        ItemGenerator.Potion(false).Spawn(vector3Int);
                        return;
                    case 'n':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        if (!Get.RunConfig.ProgressDisabled)
                        {
                            ItemGenerator.Stardust(50).Spawn(vector3Int);
                            return;
                        }
                        return;
                    case 'o':
                        {
                            GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                            if (Get.RunConfig.ProgressDisabled)
                            {
                                return;
                            }
                            int count = Get.TotalLobbyItems.GetCount(Get.Entity_PuzzlePiece);
                            if (count < 10 && Get.Floor == count + 1)
                            {
                                Maker.Make(Get.Entity_PuzzlePiece, null, false, false, true).Spawn(vector3Int);
                                return;
                            }
                            return;
                        }
                    case 'p':
                        {
                            GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                            Actor actor = BossGenerator.Boss(Get.Entity_Googon, null, new int?(0));
                            if (!Get.Trait_BossHunter.IsChosen())
                            {
                                actor.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
                            }
                            actor.Inventory.Add(Maker.Make<Item>(Get.Entity_GoldenKey, null, false, false, true), default(ValueTuple<Vector2Int?, int?, int?>));
                            actor.Spawn(vector3Int, new Quaternion?(Quaternion.identity), new Vector3(1.3f, 1.3f, 1.3f));
                            return;
                        }
                    case 'q':
                        {
                            GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_Vines, null, false, false, true).Spawn(vector3Int);
                            Actor actor2 = Maker.Make<Actor>(Get.Entity_Roothealer, delegate (Actor x)
                            {
                                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                            }, false, false, true);
                            DifficultyUtility.AddConditionsForDifficulty(actor2);
                            actor2.CalculateInitialHPManaAndStamina();
                            actor2.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
                            actor2.Spawn(vector3Int2);
                            return;
                        }
                    case 'r':
                        GenPass_GoogonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make(Get.Entity_SmallHealingCrystal, null, false, false, true).Spawn(vector3Int);
                        return;
                    default:
                        return;
                }
            }
            foreach (Vector3Int vector3Int5 in enumerable)
            {
                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int5);
            }
            if (this.mainRoomMin == default(Vector2Int))
            {
                this.mainRoomMin = new Vector2Int(x, z);
                return;
            }
            this.mainRoomMax = new Vector2Int(x, z);
        }

        [CompilerGenerated]
        internal static void <HandleSymbol>g__FloorAndCeiling|8_0(ref GenPass_GoogonsLairWorld.<>c__DisplayClass8_0 A_0)
		{
            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.floor);
        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.ceiling);
		}

		private Vector2Int mainRoomMin;

        private Vector2Int mainRoomMax;

        private Vector2Int endRoomMin;

        private Vector2Int endRoomMax;

        private const string Map = "\r\n        .............\r\n        ....####^....\r\n        ....# l #....\r\n        ....#knm#....\r\n        ....#   #....\r\n        .###^#h####%.\r\n        .# fq   qf #.\r\n        .#    p    #.\r\n        .#  i   j e#.\r\n        .#r   i    #.\r\n        .##g     g##.\r\n        .###bbbbb###.\r\n        .###c d c###.\r\n        .####e o####.\r\n        .####   ####.\r\n        .#### a ####.\r\n        .%##########.\r\n        .............";
    }
}