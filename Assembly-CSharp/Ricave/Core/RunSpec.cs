using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class RunSpec : Spec
    {
        public WorldSpec DefaultWorldSpec
        {
            get
            {
                return this.defaultWorldSpec;
            }
        }

        public EntitySpec PlayerActorSpec
        {
            get
            {
                return this.playerActorSpec;
            }
        }

        public FactionSpec PlayerFaction
        {
            get
            {
                return this.PlayerActorSpec.Actor.DefaultFaction;
            }
        }

        public bool IsLobby
        {
            get
            {
                return this.isLobby;
            }
        }

        public bool IsMain
        {
            get
            {
                return this.isMain;
            }
        }

        public bool HasRandomEvents
        {
            get
            {
                return this.hasRandomEvents;
            }
        }

        public List<int> PlaceCounts
        {
            get
            {
                return this.placeCounts;
            }
        }

        public bool HasShelters
        {
            get
            {
                return this.hasShelters;
            }
        }

        public int? FloorCount
        {
            get
            {
                return this.floorCount;
            }
        }

        public int RoomsPerFloor
        {
            get
            {
                return this.roomsPerFloor;
            }
        }

        public int GoldPerFloor
        {
            get
            {
                return this.goldPerFloor;
            }
        }

        public int StardustPerFloor
        {
            get
            {
                return this.stardustPerFloor;
            }
        }

        public int TotalEnemiesExpPerFloor
        {
            get
            {
                return this.totalEnemiesExpPerFloor;
            }
        }

        public float RampUpMultiplier
        {
            get
            {
                return this.rampUpMultiplier;
            }
        }

        public bool HasUpperStorey
        {
            get
            {
                return this.hasUpperStorey;
            }
        }

        public bool AllowMobSpawners
        {
            get
            {
                return this.allowMobSpawners;
            }
        }

        public bool PlayerCanLoseLimbs
        {
            get
            {
                return this.playerCanLoseLimbs;
            }
        }

        public int PotionsCount
        {
            get
            {
                return this.potionsCount;
            }
        }

        public int ScrollsCount
        {
            get
            {
                return this.scrollsCount;
            }
        }

        public bool AllowSpecialEnemies
        {
            get
            {
                return this.allowSpecialEnemies;
            }
        }

        public bool HasMiniBosses
        {
            get
            {
                return this.hasMiniBosses;
            }
        }

        public int SpiritsCount
        {
            get
            {
                return this.spiritsCount;
            }
        }

        public List<EntitySpec> Enemies
        {
            get
            {
                return this.enemies;
            }
        }

        [Saved]
        private WorldSpec defaultWorldSpec;

        [Saved]
        private EntitySpec playerActorSpec;

        [Saved]
        private bool isLobby;

        [Saved]
        private bool isMain;

        [Saved]
        private bool hasRandomEvents;

        [Saved]
        private List<int> placeCounts;

        [Saved]
        private bool hasShelters;

        [Saved]
        private int? floorCount;

        [Saved(10, false)]
        private int roomsPerFloor = 10;

        [Saved(250, false)]
        private int goldPerFloor = 250;

        [Saved(30, false)]
        private int stardustPerFloor = 30;

        [Saved(20, false)]
        private int totalEnemiesExpPerFloor = 20;

        [Saved(1f, false)]
        private float rampUpMultiplier = 1f;

        [Saved(true, false)]
        private bool hasUpperStorey = true;

        [Saved(true, false)]
        private bool allowMobSpawners = true;

        [Saved(true, false)]
        private bool playerCanLoseLimbs = true;

        [Saved(2, false)]
        private int potionsCount = 2;

        [Saved(1, false)]
        private int scrollsCount = 1;

        [Saved(true, false)]
        private bool allowSpecialEnemies = true;

        [Saved]
        private bool hasMiniBosses;

        [Saved]
        private int spiritsCount;

        [Saved]
        private List<EntitySpec> enemies;
    }
}