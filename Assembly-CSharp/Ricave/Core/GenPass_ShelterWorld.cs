using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_ShelterWorld : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 512148965;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            this.mainRoomMin = default(Vector2Int);
            this.mainRoomMax = default(Vector2Int);
            this.lockedRoomMin = default(Vector2Int);
            this.lockedRoomMax = default(Vector2Int);
            MapFromString.Generate("\r\n        ..............\r\n        .......###....\r\n        .....###c##%..\r\n        .....#pbfin#..\r\n        .####^o    ##.\r\n        .#13 #jh  ge#.\r\n        .#l4mk  d  ##.\r\n        .#25 #  a  #..\r\n        .^###%######..\r\n        ..............", new Action<int, int, char>(this.HandleSymbol), memory, 4);
            if (!Get.Dialogue_Guardian.Ended)
            {
                foreach (Vector3Int vector3Int in memory.playerStartPos.AdjacentCells())
                {
                    if (vector3Int.InBounds() && Get.CellsInfo.CanPassThroughNoActors(vector3Int))
                    {
                        Maker.Make(Get.Entity_GuardianDialogueTrigger, null, false, false, true).Spawn(vector3Int);
                    }
                }
            }
            Entity entity;
            if (Get.World.GetEntitiesOfSpec(Get.Entity_Guardian).TryGetRandomElement<Entity>(out entity))
            {
                ((Actor)entity).Inventory.Add(Maker.Make<Item>(Get.Entity_RoyalKey, null, false, false, true), default(ValueTuple<Vector2Int?, int?, int?>));
            }
            CellCuboid cellCuboid = new CellCuboid(this.lockedRoomMin.x, 0, this.lockedRoomMin.y, this.lockedRoomMax.x - this.lockedRoomMin.x + 1, 4, this.lockedRoomMax.y - this.lockedRoomMin.y + 1);
            Get.World.RetainedRoomInfo.Add(new CellCuboid(this.mainRoomMin.x, 0, this.mainRoomMin.y, this.mainRoomMax.x - this.mainRoomMin.x + 1, 4, this.mainRoomMax.y - this.mainRoomMin.y + 1), Get.Room_Normal, Room.LayoutRole.None, "");
            Get.World.RetainedRoomInfo.Add(cellCuboid, Get.Room_RewardRoom, Room.LayoutRole.LockedBehindSilverDoor, Get.Room_RewardRoom.LabelCap);
            foreach (Vector3Int vector3Int2 in cellCuboid)
            {
                if (vector3Int2.Below().InBounds() && Get.CellsInfo.IsFilled(vector3Int2.Below()) && !Get.CellsInfo.AnyDoorAt(vector3Int2))
                {
                    Get.TiledDecals.SetForced(Get.TiledDecals_Carpet, vector3Int2);
                }
            }
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
            GenPass_ShelterWorld.<> c__DisplayClass8_0 CS$<> 8__locals1;
            Vector3Int vector3Int;
            IEnumerable<Vector3Int> enumerable;
            MapFromString.GetCoordinates(x, z, out CS$<> 8__locals1.floor, out CS$<> 8__locals1.main, out vector3Int, out CS$<> 8__locals1.ceiling, out enumerable, 4);
            if (symbol <= '#')
            {
                if (symbol == ' ')
                {
                    GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
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
                        Vector3Int vector3Int2 = enumerator.Current;
                        Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int2);
                    }
                    return;
                }
            }
            else if (symbol != '%')
            {
                switch (symbol)
                {
                    case '.':
                    case '/':
                    case '0':
                        return;
                    case '1':
                        GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make<Item>(Get.Entity_ThrowingKnife, delegate (Item x)
                        {
                            x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                        }, false, false, true).Spawn(CS$<> 8__locals1.main);
                        return;
                    case '2':
                        GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        Maker.Make<Item>(Get.Entity_EmptyVial, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                        return;
                    case '3':
                        GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        ItemGenerator.Gold(Rand.RangeInclusive(5, 15)).Spawn(CS$<> 8__locals1.main);
                        return;
                    case '4':
                        GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        ItemGenerator.Gold(Rand.RangeInclusive(5, 15)).Spawn(CS$<> 8__locals1.main);
                        return;
                    case '5':
                        GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                        if (!Get.RunConfig.ProgressDisabled)
                        {
                            ItemGenerator.Stardust(Rand.RangeInclusive(5, 15)).Spawn(CS$<> 8__locals1.main);
                            return;
                        }
                        return;
                    default:
                        switch (symbol)
                        {
                            case '^':
                                foreach (Vector3Int vector3Int3 in enumerable)
                                {
                                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int3);
                                }
                                if (this.lockedRoomMin == default(Vector2Int))
                                {
                                    this.lockedRoomMin = new Vector2Int(x, z);
                                    return;
                                }
                                this.lockedRoomMax = new Vector2Int(x, z);
                                return;
                            case '_':
                            case '`':
                                return;
                            case 'a':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_Hatch, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                Get.WorldGenMemory.playerStartPos = CS$<> 8__locals1.main;
                                return;
                            case 'b':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_Torch, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                GenPass_ShelterWorld.< HandleSymbol > g__Guardian | 8_1(ref CS$<> 8__locals1);
                                return;
                            case 'c':
                                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                                Maker.Make(Get.Entity_Staircase, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                Maker.Make(Get.Entity_InvisibleBlocker, null, false, false, true).Spawn(CS$<> 8__locals1.main.Below());
                                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int);
                                return;
                            case 'd':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_Sign, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                return;
                            case 'e':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_StaircaseToLobby, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int);
                                return;
                            case 'f':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_StaircaseRoomSign, null, false, false, true).Spawn(vector3Int);
                                return;
                            case 'g':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_LobbySign, null, false, false, true).Spawn(vector3Int);
                                return;
                            case 'h':
                                {
                                    GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                    Place place = Get.Place;
                                    if (place != null && place.ShelterItemReward != null)
                                    {
                                        Maker.Make<Item>(Get.Place.ShelterItemReward.Value.EntitySpec, delegate (Item x)
                                        {
                                            x.StackCount = Get.Place.ShelterItemReward.Value.Count;
                                            if (Get.Place.ShelterItemRampUp != 0)
                                            {
                                                x.RampUp = Get.Place.ShelterItemRampUp;
                                            }
                                            x.TurnsLeftToIdentify = 0;
                                        }, false, false, true).Spawn(CS$<> 8__locals1.main);
                                        return;
                                    }
                                    return;
                                }
                            case 'i':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                GenPass_ShelterWorld.< HandleSymbol > g__Guardian | 8_1(ref CS$<> 8__locals1);
                                return;
                            case 'j':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int);
                                if (!Get.RunConfig.ProgressDisabled)
                                {
                                    Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                    Item item = Maker.Make<Item>(Get.Entity_Potion_Health, null, false, false, true);
                                    item.PriceTag = new PriceTag(Get.Entity_Diamond, 1, false);
                                    item.CustomLook = Get.ItemLook_ShelterHealthPotion;
                                    item.Spawn(CS$<> 8__locals1.main);
                                    return;
                                }
                                return;
                            case 'k':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_RoyalDoor, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int);
                                return;
                            case 'l':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_Torch, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                ItemGenerator.Gold(Rand.RangeInclusive(5, 15)).Spawn(CS$<> 8__locals1.main);
                                return;
                            case 'm':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_Spikes, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                return;
                            case 'n':
                                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(CS$<> 8__locals1.floor);
                                Maker.Make(Get.Entity_CeilingBarsReinforced, null, false, false, true).Spawn(CS$<> 8__locals1.ceiling);
                                Maker.Make(Get.Entity_AltarOfRedemption, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                return;
                            case 'o':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int);
                                if (!Get.RunConfig.ProgressDisabled)
                                {
                                    Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                    Item item2 = Maker.Make<Item>(Get.Entity_Scroll_CurseRemoval, null, false, false, true);
                                    item2.PriceTag = new PriceTag(Get.Entity_Diamond, 1, false);
                                    item2.CustomLook = Get.ItemLook_ShelterCurseRemovalScroll;
                                    item2.Spawn(CS$<> 8__locals1.main);
                                    return;
                                }
                                return;
                            case 'p':
                                GenPass_ShelterWorld.< HandleSymbol > g__FloorAndCeiling | 8_0(ref CS$<> 8__locals1);
                                Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int);
                                if (!Get.RunConfig.ProgressDisabled)
                                {
                                    Maker.Make(Get.Entity_Display, null, false, false, true).Spawn(CS$<> 8__locals1.main);
                                    Item item3 = Maker.Make<Item>(Get.Entity_Scroll_Identification, null, false, false, true);
                                    item3.PriceTag = new PriceTag(Get.Entity_Diamond, 1, false);
                                    item3.CustomLook = Get.ItemLook_ShelterIdentificationScroll;
                                    item3.Spawn(CS$<> 8__locals1.main);
                                    return;
                                }
                                return;
                            default:
                                return;
                        }
                        break;
                }
            }
            foreach (Vector3Int vector3Int4 in enumerable)
            {
                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int4);
            }
            if (this.mainRoomMin == default(Vector2Int))
            {
                this.mainRoomMin = new Vector2Int(x, z);
                return;
            }
            this.mainRoomMax = new Vector2Int(x, z);
        }

        [CompilerGenerated]
        internal static void <HandleSymbol>g__FloorAndCeiling|8_0(ref GenPass_ShelterWorld.<>c__DisplayClass8_0 A_0)
		{
            Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.floor);
        Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(A_0.ceiling);
    }

    [CompilerGenerated]
    internal static void <HandleSymbol>g__Guardian|8_1(ref GenPass_ShelterWorld.<>c__DisplayClass8_0 A_0)
		{
			Actor actor = Maker.Make<Actor>(Get.Entity_Guardian, delegate (Actor x)
            {
                x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
            }, false, false, true);
    DifficultyUtility.AddConditionsForDifficulty(actor);
			actor.CalculateInitialHPManaAndStamina();
			actor.Inventory.Add(ItemGenerator.Gold(50), default(ValueTuple<Vector2Int?, int?, int?>));
			actor.Spawn(A_0.main);
		}

private Vector2Int mainRoomMin;

private Vector2Int mainRoomMax;

private Vector2Int lockedRoomMin;

private Vector2Int lockedRoomMax;

private const string Map = "\r\n        ..............\r\n        .......###....\r\n        .....###c##%..\r\n        .....#pbfin#..\r\n        .####^o    ##.\r\n        .#13 #jh  ge#.\r\n        .#l4mk  d  ##.\r\n        .#25 #  a  #..\r\n        .^###%######..\r\n        ..............";
	}
}