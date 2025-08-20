using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class RoomFeatureSpec : Spec, ISaveableEventsReceiver
    {
        public RoomFeature RoomFeature
        {
            get
            {
                return this.roomFeature;
            }
        }

        public RoomFeatureCategory Category
        {
            get
            {
                return this.category;
            }
        }

        public float Priority
        {
            get
            {
                return this.priority;
            }
        }

        public float SelectionWeight
        {
            get
            {
                return this.selectionWeight;
            }
        }

        public int MaxPerRoom
        {
            get
            {
                return this.maxPerRoom;
            }
        }

        public int MaxPerMap
        {
            get
            {
                return this.maxPerMap;
            }
        }

        public QuestSpec OnlyIfQuestActive
        {
            get
            {
                return this.onlyIfQuestActive;
            }
        }

        public int? RequiredFloor
        {
            get
            {
                return this.requiredFloor;
            }
        }

        public int MinFloor
        {
            get
            {
                return this.minFloor;
            }
        }

        public List<RunSpec> ExcludedRunSpecs
        {
            get
            {
                return this.excludedRunSpecs;
            }
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            this.roomFeature.OnRoomFeatureSpecLoaded(this);
        }

        [Saved]
        private RoomFeature roomFeature;

        [Saved]
        private RoomFeatureCategory category;

        [Saved(1f, false)]
        private float priority = 1f;

        [Saved(1f, false)]
        private float selectionWeight = 1f;

        [Saved(1, false)]
        private int maxPerRoom = 1;

        [Saved(1, false)]
        private int maxPerMap = 1;

        [Saved]
        private QuestSpec onlyIfQuestActive;

        [Saved]
        private int? requiredFloor;

        [Saved]
        private int minFloor;

        [Saved]
        private List<RunSpec> excludedRunSpecs;
    }
}