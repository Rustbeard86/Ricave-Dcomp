using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_LobbyWorld : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 109427325;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            this.privateRoomMin = default(Vector2Int);
            this.privateRoomMax = default(Vector2Int);
            MapFromString.Generate("\r\n        ....############\r\n        ....#z#kbs#123e#\r\n        ....# xy^cd   F#\r\n        ######%KaJ#456G#\r\n        #  tE ##f#######\r\n        #     #i  # n #.\r\n        B     hwjug  m#.\r\n        B D   #  I#ln #.\r\n        B   C #H H#####.\r\n        %BBB###rop#.....\r\n        ......@,A,@.....\r\n        ......@,q,@.....\r\n        ......@@@@@.....", new Action<int, int, char>(this.HandleSymbol), memory, 4);
            World world = Get.World;
            world.RetainedRoomInfo.Add(new CellCuboid(this.privateRoomMin.x, 0, this.privateRoomMin.y, this.privateRoomMax.x - this.privateRoomMin.x + 1, 4, this.privateRoomMax.y - this.privateRoomMin.y + 1), Get.Room_PrivateRoom, Room.LayoutRole.None, Get.Room_PrivateRoom.LabelCap);
            this.SpawnPrivateRoomStructures();
            foreach (Structure structure in world.Structures)
            {
                if (!structure.SpawnedWithSpecificRotationUnsaved)
                {
                    structure.AutoRotate();
                }
            }
            Vector3Int? preferredRespawnPos = Get.Progress.PreferredRespawnPos;
            if (preferredRespawnPos != null && preferredRespawnPos.Value.InBounds() && world.CellsInfo.CanPassThrough(preferredRespawnPos.Value) && !world.CellsInfo.IsFallingAt(preferredRespawnPos.Value, Vector3IntUtility.Down, false, true, false))
            {
                memory.playerStartPos = preferredRespawnPos.Value;
                memory.playerStartDir = Get.Progress.PreferredRespawnDir;
            }
        }

        private void HandleSymbol(int x, int z, char symbol)
        {
            GenPass_LobbyWorld.<> c__DisplayClass6_0 CS$<> 8__locals1;
            Vector3Int vector3Int;
            Vector3Int vector3Int2;
            IEnumerable<Vector3Int> enumerable;
            MapFromString.GetCoordinates(x, z, out CS$<> 8__locals1.floor, out vector3Int, out vector3Int2, out CS$<> 8__locals1.ceiling, out enumerable, 4);
            switch (symbol)
            {
                case ' ':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    goto IL_1258;
                case '!':
                case '"':
                case '$':
                case '&':
                case '\'':
                case '(':
                case ')':
                case '*':
                case '+':
                case '-':
                case '.':
                case '/':
                case '0':
                case '7':
                case '8':
                case '9':
                case ':':
                case ';':
                case '<':
                case '=':
                case '>':
                case '?':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '[':
                case '\\':
                case ']':
                case '_':
                case '`':
                case 'v':
                    goto IL_1258;
                case '#':
                    {
                        using (IEnumerator<Vector3Int> enumerator = enumerable.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Vector3Int vector3Int3 = enumerator.Current;
                                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int3);
                            }
                            goto IL_1258;
                        }
                        break;
                    }
                case '%':
                    break;
                case ',':
                    goto IL_0E6F;
                case '1':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Chain, null, false, false, true).Spawn(vector3Int2);
                    Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(vector3Int);
                    if (Get.Quest_RescueLobbyShopkeeper.IsCompleted() && !Get.UnlockableManager.IsUnlocked(Get.Unlockable_StardustBoost, null))
                    {
                        UnlockableAsItem unlockableAsItem = Maker.Make<UnlockableAsItem>(Get.Entity_UnlockableAsItem, null, false, false, true);
                        unlockableAsItem.UnlockableSpec = Get.Unlockable_StardustBoost;
                        unlockableAsItem.PriceTag = new PriceTag(Get.Entity_Stardust, unlockableAsItem.UnlockableSpec.StardustPrice, false);
                        unlockableAsItem.Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case '2':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Chain, null, false, false, true).Spawn(vector3Int2);
                    Maker.Make(Get.Entity_VioletWallLamp, null, false, false, true).Spawn(vector3Int);
                    Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(vector3Int);
                    if (Get.Quest_RescueLobbyShopkeeper.IsCompleted() && !Get.UnlockableManager.IsUnlocked(Get.Unlockable_GoldBoost, null))
                    {
                        UnlockableAsItem unlockableAsItem2 = Maker.Make<UnlockableAsItem>(Get.Entity_UnlockableAsItem, null, false, false, true);
                        unlockableAsItem2.UnlockableSpec = Get.Unlockable_GoldBoost;
                        unlockableAsItem2.PriceTag = new PriceTag(Get.Entity_Stardust, unlockableAsItem2.UnlockableSpec.StardustPrice, false);
                        unlockableAsItem2.Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case '3':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Chain, null, false, false, true).Spawn(vector3Int2);
                    Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(vector3Int);
                    if (Get.Quest_RescueLobbyShopkeeper.IsCompleted() && !Get.UnlockableManager.IsUnlocked(Get.Unlockable_ScoreBoost, null))
                    {
                        UnlockableAsItem unlockableAsItem3 = Maker.Make<UnlockableAsItem>(Get.Entity_UnlockableAsItem, null, false, false, true);
                        unlockableAsItem3.UnlockableSpec = Get.Unlockable_ScoreBoost;
                        unlockableAsItem3.PriceTag = new PriceTag(Get.Entity_Stardust, unlockableAsItem3.UnlockableSpec.StardustPrice, false);
                        unlockableAsItem3.Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case '4':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Chain, null, false, false, true).Spawn(vector3Int2);
                    Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(vector3Int);
                    if (Get.Quest_RescueLobbyShopkeeper.IsCompleted() && !Get.UnlockableManager.IsUnlocked(Get.Unlockable_ExtraHealthPotion, null))
                    {
                        UnlockableAsItem unlockableAsItem4 = Maker.Make<UnlockableAsItem>(Get.Entity_UnlockableAsItem, null, false, false, true);
                        unlockableAsItem4.UnlockableSpec = Get.Unlockable_ExtraHealthPotion;
                        unlockableAsItem4.PriceTag = new PriceTag(Get.Entity_Stardust, unlockableAsItem4.UnlockableSpec.StardustPrice, false);
                        unlockableAsItem4.Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case '5':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Chain, null, false, false, true).Spawn(vector3Int2);
                    Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(vector3Int);
                    if (Get.Quest_RescueLobbyShopkeeper.IsCompleted() && !Get.UnlockableManager.IsUnlocked(Get.Unlockable_ExtraScroll, null))
                    {
                        UnlockableAsItem unlockableAsItem5 = Maker.Make<UnlockableAsItem>(Get.Entity_UnlockableAsItem, null, false, false, true);
                        unlockableAsItem5.UnlockableSpec = Get.Unlockable_ExtraScroll;
                        unlockableAsItem5.PriceTag = new PriceTag(Get.Entity_Stardust, unlockableAsItem5.UnlockableSpec.StardustPrice, false);
                        unlockableAsItem5.Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case '6':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Chain, null, false, false, true).Spawn(vector3Int2);
                    Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(vector3Int);
                    if (Get.Quest_RescueLobbyShopkeeper.IsCompleted() && !Get.UnlockableManager.IsUnlocked(Get.Unlockable_ExtraSword, null))
                    {
                        UnlockableAsItem unlockableAsItem6 = Maker.Make<UnlockableAsItem>(Get.Entity_UnlockableAsItem, null, false, false, true);
                        unlockableAsItem6.UnlockableSpec = Get.Unlockable_ExtraSword;
                        unlockableAsItem6.PriceTag = new PriceTag(Get.Entity_Stardust, unlockableAsItem6.UnlockableSpec.StardustPrice, false);
                        unlockableAsItem6.Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case '@':
                    {
                        using (IEnumerator<Vector3Int> enumerator = enumerable.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Vector3Int vector3Int4 = enumerator.Current;
                                Maker.Make(Get.Entity_Skybox, null, false, false, true).Spawn(vector3Int4);
                            }
                            goto IL_1258;
                        }
                        goto IL_0E6F;
                    }
                case 'A':
                    Maker.Make(Get.Entity_Skybox, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                    Maker.Make(Get.Entity_Skybox, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                    if (!Get.Dialogue_Final.Ended)
                    {
                        Maker.Make(Get.Entity_FinalDialogueTrigger, null, false, false, true).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case 'B':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                    Maker.Make(Get.Entity_Window, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'C':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    if (!Get.Unlockable_TrashPile1.IsUnlocked())
                    {
                        GenPass_LobbyWorld.< HandleSymbol > g__MakeTrashPile | 6_1(Get.Unlockable_TrashPile1).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case 'D':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    if (!Get.Unlockable_TrashPile2.IsUnlocked())
                    {
                        GenPass_LobbyWorld.< HandleSymbol > g__MakeTrashPile | 6_1(Get.Unlockable_TrashPile2).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case 'E':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    if (!Get.Unlockable_TrashPile3.IsUnlocked())
                    {
                        GenPass_LobbyWorld.< HandleSymbol > g__MakeTrashPile | 6_1(Get.Unlockable_TrashPile3).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case 'F':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_WoodenWall, null, false, false, true).Spawn(vector3Int2);
                    Maker.Make(Get.Entity_MagicMirror, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'G':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_WoodenWall, null, false, false, true).Spawn(vector3Int2);
                    Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(vector3Int);
                    if (Get.Quest_RescueLobbyShopkeeper.IsCompleted() && !Get.UnlockableManager.IsUnlocked(Get.Unlockable_ExtraAmuletOfYerdon, null))
                    {
                        UnlockableAsItem unlockableAsItem7 = Maker.Make<UnlockableAsItem>(Get.Entity_UnlockableAsItem, null, false, false, true);
                        unlockableAsItem7.UnlockableSpec = Get.Unlockable_ExtraAmuletOfYerdon;
                        unlockableAsItem7.PriceTag = new PriceTag(Get.Entity_Stardust, unlockableAsItem7.UnlockableSpec.StardustPrice, false);
                        unlockableAsItem7.Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case 'H':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_FlowerVase, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'I':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_CyanWallLamp, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'J':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_CosmeticsWardrobe, null, false, false, true).Spawn(vector3Int, Vector3IntUtility.Left);
                    if (Get.Progress.GetRunStats(Get.Run_Main1).Completed)
                    {
                        Maker.Make(Get.Entity_PetRat, null, false, false, true).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case 'K':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Soulforge, null, false, false, true).Spawn(vector3Int, Vector3IntUtility.Right);
                    goto IL_1258;
                case '^':
                    Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                    Maker.Make(Get.Entity_CeilingBars, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                    if (Get.WeatherManager.IsRainingOutside)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Maker.Make(Get.Entity_Rain, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                        }
                    }
                    if (Get.Progress.GetRunStats(Get.Run_Main3).Completed && !Get.Progress.DemonNotePickedUp)
                    {
                        Maker.Make<Note>(Get.Entity_Note, null, false, false, true).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case 'a':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Get.WorldGenMemory.playerStartPos = vector3Int;
                    Maker.Make(Get.Entity_HallwaySign, null, false, false, true).Spawn(vector3Int2);
                    goto IL_1258;
                case 'b':
                    Maker.Make(Get.Entity_CeilingBars, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                    Maker.Make(Get.Entity_WallLamp, null, false, false, true).Spawn(vector3Int);
                    if (!Get.Quest_Introduction.IsActive() && !Get.Quest_Introduction.IsCompleted())
                    {
                        Maker.Make(Get.Entity_StaircaseLocked, null, false, false, true).Spawn(vector3Int);
                    }
                    else
                    {
                        Maker.Make(Get.Entity_NewRunStaircase, null, false, false, true).Spawn(vector3Int);
                    }
                    Maker.Make(Get.Entity_InvisibleBlocker, null, false, false, true).Spawn(vector3Int.Below());
                    goto IL_1258;
                case 'c':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_ShopRoomSign, null, false, false, true).Spawn(vector3Int2);
                    goto IL_1258;
                case 'd':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                    if (!Get.Quest_RescueLobbyShopkeeper.IsCompleted())
                    {
                        Maker.Make(Get.Entity_LockedDoor, null, false, false, true).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    if (!Get.Quest_RescueLobbyShopkeeper.IsCompletedAndClaimed())
                    {
                        Maker.Make(Get.Entity_TemporarilyOpenedDoor, null, false, false, true).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    Maker.Make(Get.Entity_Door, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'e':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_WoodenWall, null, false, false, true).Spawn(vector3Int2);
                    if (Get.Quest_RescueLobbyShopkeeper.IsCompleted())
                    {
                        Maker.Make(Get.Entity_LobbyShopkeeper, null, false, false, true).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case 'f':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                    if (Get.Unlockable_Hallway.IsUnlocked())
                    {
                        Maker.Make(Get.Entity_Door, null, false, false, true).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    Maker.Make(Get.Entity_LockedDoorHallway, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'g':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                    if (Get.TotalLobbyItems.GetCount(Get.Entity_PuzzlePiece) >= 10)
                    {
                        Maker.Make(Get.Entity_PuzzleDoor, null, false, false, true).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    Maker.Make(Get.Entity_LockedPuzzleDoor, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'h':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                    if (Get.UnlockableManager.IsUnlocked(Get.Unlockable_PrivateRoom, null))
                    {
                        Maker.Make(Get.Entity_Door, null, false, false, true).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    Maker.Make(Get.Entity_LockedDoor, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'i':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    if (!Get.UnlockableManager.IsUnlocked(Get.Unlockable_PrivateRoom, null))
                    {
                        UnlockableAsItem unlockableAsItem8 = Maker.Make<UnlockableAsItem>(Get.Entity_UnlockableAsItem, null, false, false, true);
                        unlockableAsItem8.PriceTag = new PriceTag(Get.Entity_Stardust, Get.Unlockable_PrivateRoom.StardustPrice, false);
                        unlockableAsItem8.UnlockableSpec = Get.Unlockable_PrivateRoom;
                        unlockableAsItem8.Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case 'j':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Chandelier, null, false, false, true).Spawn(vector3Int2);
                    goto IL_1258;
                case 'k':
                    {
                        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                        Maker.Make(Get.Entity_CeilingBars, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                        Maker.Make(Get.Entity_QuestLog, null, false, false, true).Spawn(vector3Int);
                        int num = 0;
                        using (IEnumerator<TotalKillCounter.KilledBoss> enumerator2 = this.BossesForTrophies().GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                TotalKillCounter.KilledBoss killedBoss = enumerator2.Current;
                                BossTrophy bossTrophy = Maker.Make<BossTrophy>(Get.Entity_BossTrophy, null, false, false, true);
                                bossTrophy.Boss = killedBoss;
                                if (!killedBoss.TrophyHasStardust)
                                {
                                    bossTrophy.UseEffects.Clear();
                                }
                                bossTrophy.Spawn(vector3Int2 + new Vector3Int(num, 0, 0), Vector3IntUtility.South);
                                num++;
                            }
                            goto IL_0743;
                        }
                    IL_070B:
                        BossTrophy bossTrophy2 = Maker.Make<BossTrophy>(Get.Entity_BossTrophy, null, false, false, true);
                        bossTrophy2.UseEffects.Clear();
                        bossTrophy2.Spawn(vector3Int2 + new Vector3Int(num, 0, 0), Vector3IntUtility.South);
                        num++;
                    IL_0743:
                        if (num < 3)
                        {
                            goto IL_070B;
                        }
                        if (SteamManager.Initialized)
                        {
                            Maker.Make(Get.Entity_DailyChallengeBoard, null, false, false, true).Spawn(vector3Int2, Vector3IntUtility.Right);
                            goto IL_1258;
                        }
                        goto IL_1258;
                    }
                case 'l':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    if (Get.PrivateRoom.GetPlacedAndInventoryCount(Get.Entity_PuzzleSculpture) <= 0)
                    {
                        PrivateRoomStructureAsItem privateRoomStructureAsItem = Maker.Make<PrivateRoomStructureAsItem>(Get.Entity_PrivateRoomStructureAsItem, null, false, false, true);
                        privateRoomStructureAsItem.PriceTag = new PriceTag(Get.Entity_Stardust, 60, false);
                        privateRoomStructureAsItem.PrivateRoomStructure = Get.Entity_PuzzleSculpture;
                        privateRoomStructureAsItem.Spawn(vector3Int);
                        goto IL_1258;
                    }
                    goto IL_1258;
                case 'm':
                    Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                    Maker.Make(Get.Entity_WallLamp, null, false, false, true).Spawn(vector3Int);
                    Maker.Make(Get.Entity_NewRunWithSeedStaircase, null, false, false, true).Spawn(vector3Int);
                    Maker.Make(Get.Entity_InvisibleBlocker, null, false, false, true).Spawn(vector3Int.Below());
                    goto IL_1258;
                case 'n':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_BannerPuzzle, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'o':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_WallPiece3, null, false, false, true).Spawn(vector3Int2);
                    if (Get.TotalLobbyItems.Any(Get.Entity_MemoryPiece1) && Get.TotalLobbyItems.Any(Get.Entity_MemoryPiece2) && Get.TotalLobbyItems.Any(Get.Entity_MemoryPiece3) && Get.TotalLobbyItems.Any(Get.Entity_MemoryPiece4))
                    {
                        Maker.Make(Get.Entity_MemoryDoor, null, false, false, true).Spawn(vector3Int);
                    }
                    else
                    {
                        Maker.Make(Get.Entity_LockedMemoryDoor, null, false, false, true).Spawn(vector3Int);
                    }
                    Maker.Make(Get.Entity_MemoryPiece1Status, null, false, false, true).Spawn(vector3Int, Vector3IntUtility.Right);
                    Maker.Make(Get.Entity_MemoryPiece2Status, null, false, false, true).Spawn(vector3Int, Vector3IntUtility.Right);
                    Maker.Make(Get.Entity_MemoryPiece3Status, null, false, false, true).Spawn(vector3Int, Vector3IntUtility.Right);
                    Maker.Make(Get.Entity_MemoryPiece4Status, null, false, false, true).Spawn(vector3Int, Vector3IntUtility.Right);
                    goto IL_1258;
                case 'p':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_WallPiece1, null, false, false, true).Spawn(vector3Int);
                    Maker.Make(Get.Entity_WallPiece2, null, false, false, true).Spawn(vector3Int2);
                    goto IL_1258;
                case 'q':
                    Maker.Make(Get.Entity_Skybox, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                    Maker.Make(Get.Entity_Skybox, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                    Maker.Make(Get.Entity_EndingPortal, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'r':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_WallPiece5, null, false, false, true).Spawn(vector3Int);
                    Maker.Make(Get.Entity_WallPiece4, null, false, false, true).Spawn(vector3Int2);
                    goto IL_1258;
                case 's':
                    Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                    Maker.Make(Get.Entity_CeilingBars, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                    Maker.Make(Get.Entity_StatsBoard, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 't':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_WallLamp, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'u':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_PuzzleRoomSign, null, false, false, true).Spawn(vector3Int2);
                    goto IL_1258;
                case 'w':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_PrivateRoomSign, null, false, false, true).Spawn(vector3Int2);
                    goto IL_1258;
                case 'x':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                    if (Get.Unlockable_TrainingRoom.IsUnlocked())
                    {
                        Maker.Make(Get.Entity_Door, null, false, false, true).Spawn(vector3Int);
                        goto IL_1258;
                    }
                    Maker.Make(Get.Entity_LockedDoorTrainingRoom, null, false, false, true).Spawn(vector3Int);
                    goto IL_1258;
                case 'y':
                    GenPass_LobbyWorld.< HandleSymbol > g__FloorAndCeiling | 6_0(ref CS$<> 8__locals1);
                    Maker.Make(Get.Entity_TrainingRoomSign, null, false, false, true).Spawn(vector3Int2);
                    goto IL_1258;
                case 'z':
                    Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                    Maker.Make(Get.Entity_PinkWallLamp, null, false, false, true).Spawn(vector3Int);
                    Maker.Make(Get.Entity_TrainingRoomStaircase, null, false, false, true).Spawn(vector3Int);
                    Maker.Make(Get.Entity_InvisibleBlocker, null, false, false, true).Spawn(vector3Int.Below());
                    goto IL_1258;
                default:
                    goto IL_1258;
            }
            foreach (Vector3Int vector3Int5 in enumerable)
            {
                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int5);
            }
            if (this.privateRoomMin == default(Vector2Int))
            {
                this.privateRoomMin = new Vector2Int(x, z);
                goto IL_1258;
            }
            this.privateRoomMax = new Vector2Int(x, z);
            goto IL_1258;
        IL_0E6F:
            Maker.Make(Get.Entity_Skybox, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
            Maker.Make(Get.Entity_Skybox, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
        IL_1258:
            if (!Get.CellsInfo.IsFilled(vector3Int) && (Get.CellsInfo.CanSeeThrough(vector3Int) || Get.CellsInfo.AnyDoorAt(vector3Int)) && Get.CellsInfo.IsFloorUnder(vector3Int))
            {
                Get.TiledDecals.SetForced(Get.TiledDecals_Carpet, vector3Int);
            }
        }

        private void SpawnPrivateRoomStructures()
        {
            foreach (PrivateRoom.PlacedStructure placedStructure in Get.PrivateRoom.PlacedStructures)
            {
                Structure structure = Maker.Make<Structure>(placedStructure.StructureSpec, null, false, false, true);
                structure.Spawn(placedStructure.Position);
                placedStructure.OnSpawned(structure);
            }
            try
            {
                foreach (Instruction instruction in Get.PrivateRoom.RevalidateAllPlacedStructuresAndSyncWithWorld())
                {
                    instruction.Do();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while revalidating placed private room structures.", ex);
            }
        }

        private IEnumerable<TotalKillCounter.KilledBoss> BossesForTrophies()
        {
            return (from x in Get.TotalKillCounter.KilledBosses
                    where x.CanBeTrophy
                    orderby x.KilledOnFloor descending, x.KillTime descending
                    select x).Take<TotalKillCounter.KilledBoss>(3);
        }

        [CompilerGenerated]
        internal static void <HandleSymbol>g__FloorAndCeiling|6_0(ref GenPass_LobbyWorld.<>c__DisplayClass6_0 A_0)
		{
            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.floor);
        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.ceiling);
		}

		[CompilerGenerated]
        internal static Structure<HandleSymbol> g__MakeTrashPile|6_1(UnlockableSpec unlockableSpec)
		{
			Structure structure = Maker.Make<Structure>(Get.Entity_TrashPile, null, false, false, true);
    structure.UseEffects.RemoveUseEffect(structure.UseEffects.GetFirstOfSpec(Get.UseEffect_UnlockUnlockable));
			structure.UseEffects.AddUseEffect(new UseEffect_UnlockUnlockable(Get.UseEffect_UnlockUnlockable, unlockableSpec, true), -1);
			return structure;
		}

private Vector2Int privateRoomMin;

private Vector2Int privateRoomMax;

private const string Map = "\r\n        ....############\r\n        ....#z#kbs#123e#\r\n        ....# xy^cd   F#\r\n        ######%KaJ#456G#\r\n        #  tE ##f#######\r\n        #     #i  # n #.\r\n        B     hwjug  m#.\r\n        B D   #  I#ln #.\r\n        B   C #H H#####.\r\n        %BBB###rop#.....\r\n        ......@,A,@.....\r\n        ......@,q,@.....\r\n        ......@@@@@.....";
	}
}