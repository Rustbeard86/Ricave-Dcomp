using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class RoomSpec : Spec, ISaveableEventsReceiver
    {
        public RoomFiller Filler
        {
            get
            {
                return this.filler;
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

        public int? MaxPerMap
        {
            get
            {
                return this.maxPerMap;
            }
        }

        public string MaxOnePerMapTag
        {
            get
            {
                return this.maxOnePerMapTag;
            }
        }

        public List<Room.LayoutRole> AllowedLayoutRoles
        {
            get
            {
                return this.allowedLayoutRoles;
            }
        }

        public List<RoomPartSpec> PartsInOrder
        {
            get
            {
                if (this.partsFromAllSourcesCached == null)
                {
                    this.partsFromAllSourcesCached = new List<RoomPartSpec>();
                    this.partsFromAllSourcesCached.AddRange(this.parts);
                    foreach (RoomPartSpec roomPartSpec in Get.Specs.GetAll<RoomPartSpec>())
                    {
                        if (roomPartSpec.AddToRoomSpecs.Contains(this))
                        {
                            this.partsFromAllSourcesCached.Add(roomPartSpec);
                        }
                    }
                    this.partsFromAllSourcesCached.StableSort<RoomPartSpec, float>((RoomPartSpec x) => x.Order);
                }
                return this.partsFromAllSourcesCached;
            }
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            this.filler.OnRoomSpecLoaded(this);
        }

        [Saved]
        private RoomFiller filler;

        [Saved(1f, false)]
        private float priority = 1f;

        [Saved(1f, false)]
        private float selectionWeight = 1f;

        [Saved]
        private int? maxPerMap;

        [Saved]
        private string maxOnePerMapTag;

        [Saved(Default.New, false)]
        private List<Room.LayoutRole> allowedLayoutRoles = new List<Room.LayoutRole>();

        [Saved(Default.New, false)]
        private List<RoomPartSpec> parts = new List<RoomPartSpec>();

        private List<RoomPartSpec> partsFromAllSourcesCached;
    }
}