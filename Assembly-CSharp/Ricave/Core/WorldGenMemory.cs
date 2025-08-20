using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class WorldGenMemory
    {
        public IEnumerable<Room> AllRooms
        {
            get
            {
                return this.storeys.SelectMany<Storey, Room>((Storey x) => x.Rooms);
            }
        }

        public IEnumerable<Item> UnusedBaseMiscItems
        {
            get
            {
                return this.unusedBaseItems.Where<Item>((Item x) => !x.Spec.IsLobbyItemOrLobbyRelated && !x.Spec.Item.IsEquippableNonRing);
            }
        }

        public IEnumerable<Item> UnusedBaseGear
        {
            get
            {
                return this.unusedBaseItems.Where<Item>((Item x) => !x.Spec.IsLobbyItemOrLobbyRelated && x.Spec.Item.IsEquippableNonRing);
            }
        }

        public IEnumerable<Item> UnusedBaseLobbyItems
        {
            get
            {
                return this.unusedBaseItems.Where<Item>((Item x) => x.Spec.IsLobbyItemOrLobbyRelated);
            }
        }

        public WorldConfig config;

        public List<RetainedRoomInfo.RoomInfo> retainedRoomInfo;

        public List<Storey> storeys;

        public Vector3Int playerStartPos;

        public Vector3Int? playerStartDir;

        public List<Item> baseItems;

        public List<Actor> baseActors;

        public List<Item> unusedBaseItems;

        public List<Actor> unusedBaseActors;

        public List<RoomFeatureSpec> roomFeaturesGenerated = new List<RoomFeatureSpec>();

        public bool debugStopFillingRooms;

        public int generatedGold;

        public List<Entity> goldWanters = new List<Entity>();

        public int typicalWallsWithPipe;

        public int roomsWithPillars;

        public bool generatedGoldInWindow;

        public bool generatedWindowsWall;

        public bool generatedBigCrate;

        public bool generatedBarrel;

        public bool generatedCeilingSupports;

        public bool generatedMimicChest;

        public bool generatedChains;

        public bool generatedCrateWithGold;

        public bool generatedGrassWithVineSeed;

        public Dictionary<object, object> custom = new Dictionary<object, object>();
    }
}