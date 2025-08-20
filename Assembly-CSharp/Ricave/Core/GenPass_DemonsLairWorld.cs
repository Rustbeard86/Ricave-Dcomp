using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_DemonsLairWorld : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 931553785;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            this.mainRoomMin = default(Vector2Int);
            this.mainRoomMax = default(Vector2Int);
            this.endRoomMin = default(Vector2Int);
            this.endRoomMax = default(Vector2Int);
            this.secretRoomMin = default(Vector2Int);
            this.secretRoomMax = default(Vector2Int);
            MapFromString.Generate("\r\n        ....................\r\n        .........####^......\r\n        .........#kjk#......\r\n        .........#ion#......\r\n        .........#kkk#......\r\n        ....#####^#h######%.\r\n        ....#ggggggggggggg#.\r\n        ....#gf    q    fg#.\r\n        ....#gr  mllll  rg#.\r\n        ....#g           g#.\r\n        ....#g l #bbb# m g#.\r\n        ....#g l becpb l g#.\r\n        ....#g l beaeb l g#.\r\n        ....#g l bd#db l g#.\r\n        ....#g l beeeb l g#.\r\n        .###&g m #bbb# l g#.\r\n        .#vutg           g#.\r\n        .&###gr  llllm  rg#.\r\n        ....#gf         fg#.\r\n        ....#ggggggggggggs#.\r\n        ....%##############.\r\n        ....................", new Action<int, int, char>(this.HandleSymbol), memory, 5);
            Get.World.RetainedRoomInfo.Add(new CellCuboid(this.mainRoomMin.x, 0, this.mainRoomMin.y, this.mainRoomMax.x - this.mainRoomMin.x + 1, 5, this.mainRoomMax.y - this.mainRoomMin.y + 1), Get.Room_BossRoom, Room.LayoutRole.None, Get.Room_BossRoom.LabelCap);
            Get.World.RetainedRoomInfo.Add(new CellCuboid(this.endRoomMin.x, 0, this.endRoomMin.y, this.endRoomMax.x - this.endRoomMin.x + 1, 5, this.endRoomMax.y - this.endRoomMin.y + 1), Get.Room_RewardRoom, Room.LayoutRole.LockedBehindSilverDoor, Get.Room_RewardRoom.LabelCap);
            Get.World.RetainedRoomInfo.Add(new CellCuboid(this.secretRoomMin.x, 0, this.secretRoomMin.y, this.secretRoomMax.x - this.secretRoomMin.x + 1, 5, this.secretRoomMax.y - this.secretRoomMin.y + 1), Get.Room_RewardRoom, Room.LayoutRole.LockedBehindSilverDoor, Get.Room_RewardRoom.LabelCap);
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
            GenPass_DemonsLairWorld.<> c__DisplayClass10_0 CS$<> 8__locals1;
            Vector3Int vector3Int;
            Vector3Int vector3Int2;
            IEnumerable<Vector3Int> enumerable;
            MapFromString.GetCoordinates(x, z, out CS$<> 8__locals1.floor, out vector3Int, out vector3Int2, out CS$<> 8__locals1.ceiling, out enumerable, 5);
            Vector3Int vector3Int3 = vector3Int2.Above();
            switch (symbol)
            {
                case ' ':
                    GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                    return;
                case '!':
                case '"':
                case '$':
                    return;
                case '#':
                    {
                        using (IEnumerator<Vector3Int> enumerator = enumerable.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Vector3Int vector3Int4 = enumerator.Current;
                                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int4);
                            }
                            return;
                        }
                        break;
                    }
                case '%':
                    break;
                case '&':
                    foreach (Vector3Int vector3Int5 in enumerable)
                    {
                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int5);
                    }
                    if (this.secretRoomMin == default(Vector2Int))
                    {
                        this.secretRoomMin = new Vector2Int(x, z);
                        return;
                    }
                    this.secretRoomMax = new Vector2Int(x, z);
                    return;
                default:
                    if (symbol == '.')
                    {
                        return;
                    }
                    switch (symbol)
                    {
                        case '^':
                            foreach (Vector3Int vector3Int6 in enumerable)
                            {
                                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int6);
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
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_Hatch, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            Get.WorldGenMemory.playerStartPos = vector3Int;
                            return;
                        case 'b':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_Gate, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Gate, null, false, false, true).Spawn(vector3Int2);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'c':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_Lever, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'd':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_PinkWallLamp, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'e':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'f':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_MagicFire, null, false, false, true).Spawn(vector3Int);
                            return;
                        case 'g':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'h':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_GoldenDoor, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'i':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_HealingCrystal, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'j':
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
                                    goto IL_04D0;
                                }
                            }
                            Maker.Make(Get.Entity_Staircase, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_InvisibleBlocker, null, false, false, true).Spawn(vector3Int.Below());
                        IL_04D0:
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'k':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'l':
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                            Maker.Make(Get.Entity_Lava, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                            return;
                        case 'm':
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                            Maker.Make(Get.Entity_Lava, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                            Maker.Make(Get.Entity_Ladder, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                            return;
                        case 'n':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            ItemGenerator.Potion(false).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'o':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            if (!Get.RunConfig.ProgressDisabled)
                            {
                                ItemGenerator.Stardust(100).Spawn(vector3Int);
                            }
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'p':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            if (!Get.RunConfig.ProgressDisabled)
                            {
                                int count = Get.TotalLobbyItems.GetCount(Get.Entity_PuzzlePiece);
                                if (count < 10 && Get.Floor == count + 1)
                                {
                                    Maker.Make(Get.Entity_PuzzlePiece, null, false, false, true).Spawn(vector3Int);
                                }
                            }
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'q':
                            {
                                GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                                Actor actor = BossGenerator.Boss(Get.Entity_Demon, Get.Progress.PlayerName.Reversed(), new int?(0));
                                if (!Get.Trait_BossHunter.IsChosen())
                                {
                                    actor.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
                                }
                                if (!Get.RunConfig.ProgressDisabled)
                                {
                                    PrivateRoomStructureAsItem privateRoomStructureAsItem = Maker.Make<PrivateRoomStructureAsItem>(Get.Entity_PrivateRoomStructureAsItem, null, false, false, true);
                                    privateRoomStructureAsItem.PrivateRoomStructure = Get.Entity_MirrorShard;
                                    actor.Inventory.Add(privateRoomStructureAsItem, default(ValueTuple<Vector2Int?, int?, int?>));
                                }
                                actor.Inventory.Add(Maker.Make<Item>(Get.Entity_GoldenKey, null, false, false, true), default(ValueTuple<Vector2Int?, int?, int?>));
                                actor.Spawn(vector3Int, new Quaternion?(Quaternion.identity), new Vector3(1.3f, 1.3f, 1.3f));
                                return;
                            }
                        case 'r':
                            {
                                GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                                Actor actor2 = Maker.Make<Actor>(Get.Entity_Blazekin, delegate (Actor x)
                                {
                                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                                }, false, false, true);
                                DifficultyUtility.AddConditionsForDifficulty(actor2);
                                actor2.CalculateInitialHPManaAndStamina();
                                actor2.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
                                actor2.Spawn(vector3Int);
                                return;
                            }
                        case 's':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_SmallHealingCrystal, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 't':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_RoyalDoor, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'u':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            Maker.Make(Get.Entity_SilverDoor, null, false, false, true).Spawn(vector3Int);
                            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int3);
                            return;
                        case 'v':
                            GenPass_DemonsLairWorld.< HandleSymbol > g__FloorAndCeiling | 10_0(ref CS$<> 8__locals1);
                            if (!Get.RunConfig.ProgressDisabled)
                            {
                                ItemGenerator.Diamonds(3).Spawn(vector3Int);
                            }
                            else
                            {
                                ItemGenerator.Gold(500).Spawn(vector3Int);
                            }
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int2);
                            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int3);
                            return;
                        default:
                            return;
                    }
                    break;
            }
            foreach (Vector3Int vector3Int7 in enumerable)
            {
                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int7);
            }
            if (this.mainRoomMin == default(Vector2Int))
            {
                this.mainRoomMin = new Vector2Int(x, z);
                return;
            }
            this.mainRoomMax = new Vector2Int(x, z);
        }

        [CompilerGenerated]
        internal static void <HandleSymbol>g__FloorAndCeiling|10_0(ref GenPass_DemonsLairWorld.<>c__DisplayClass10_0 A_0)
		{
            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.floor);
        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.ceiling);
		}

		private Vector2Int mainRoomMin;

        private Vector2Int mainRoomMax;

        private Vector2Int endRoomMin;

        private Vector2Int endRoomMax;

        private Vector2Int secretRoomMin;

        private Vector2Int secretRoomMax;

        private const string Map = "\r\n        ....................\r\n        .........####^......\r\n        .........#kjk#......\r\n        .........#ion#......\r\n        .........#kkk#......\r\n        ....#####^#h######%.\r\n        ....#ggggggggggggg#.\r\n        ....#gf    q    fg#.\r\n        ....#gr  mllll  rg#.\r\n        ....#g           g#.\r\n        ....#g l #bbb# m g#.\r\n        ....#g l becpb l g#.\r\n        ....#g l beaeb l g#.\r\n        ....#g l bd#db l g#.\r\n        ....#g l beeeb l g#.\r\n        .###&g m #bbb# l g#.\r\n        .#vutg           g#.\r\n        .&###gr  llllm  rg#.\r\n        ....#gf         fg#.\r\n        ....#ggggggggggggs#.\r\n        ....%##############.\r\n        ....................";
    }
}