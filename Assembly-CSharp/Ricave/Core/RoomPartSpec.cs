using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class RoomPartSpec : Spec, ISaveableEventsReceiver
    {
        public RoomPart RoomPart
        {
            get
            {
                return this.roomPart;
            }
        }

        public float Order
        {
            get
            {
                return this.order;
            }
        }

        public List<RoomSpec> AddToRoomSpecs
        {
            get
            {
                return this.addToRoomSpecs;
            }
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            this.roomPart.OnRoomPartSpecLoaded(this);
        }

        [Saved]
        private RoomPart roomPart;

        [Saved]
        private float order;

        [Saved(Default.New, false)]
        private List<RoomSpec> addToRoomSpecs = new List<RoomSpec>();
    }
}