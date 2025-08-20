using System;

namespace Ricave.Core
{
    public class WorldConfig : ISaveableEventsReceiver
    {
        public WorldSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public int WorldSeed
        {
            get
            {
                return this.worldSeed;
            }
        }

        public bool UsedDescendTrigger
        {
            get
            {
                return this.usedDescendTrigger;
            }
        }

        public Place Place
        {
            get
            {
                return this.place;
            }
        }

        public PlaceLink UsedLink
        {
            get
            {
                return this.usedLink;
            }
        }

        public int Floor
        {
            get
            {
                Place place = this.place;
                if (place == null)
                {
                    return 1;
                }
                return place.Floor;
            }
        }

        public RoomSpec OnlyAllowedRoomSpec
        {
            get
            {
                return this.onlyAllowedRoomSpec;
            }
        }

        public WorldConfig()
        {
        }

        public WorldConfig(WorldSpec spec, int worldSeed, Place place, PlaceLink usedLink, RoomSpec onlyAllowedRoomSpec = null, bool usedDescendTrigger = false)
        {
            this.spec = spec;
            this.worldSeed = worldSeed;
            this.place = place;
            this.usedLink = usedLink;
            this.onlyAllowedRoomSpec = onlyAllowedRoomSpec;
            this.usedDescendTrigger = usedDescendTrigger;
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.spec == null)
            {
                Log.Error("World spec was null after loading.", false);
                this.spec = Get.World_Standard;
            }
        }

        [Saved]
        private WorldSpec spec;

        [Saved]
        private int worldSeed;

        [Saved]
        private bool usedDescendTrigger;

        [Saved]
        private Place place;

        [Saved]
        private PlaceLink usedLink;

        [Saved]
        private RoomSpec onlyAllowedRoomSpec;
    }
}