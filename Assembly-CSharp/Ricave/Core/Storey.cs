using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class Storey
    {
        public WorldGenMemory Memory
        {
            get
            {
                return this.memory;
            }
        }

        public int Index
        {
            get
            {
                int num = this.memory.storeys.IndexOf(this);
                if (num == -1)
                {
                    Log.Error("Asking for Storey index but not added to storeys list.", false);
                }
                return num;
            }
        }

        public Storey.Type StoreyType
        {
            get
            {
                return this.type;
            }
        }

        public IEnumerable<Room> Rooms
        {
            get
            {
                return this.roomSlots.Where<Room>((Room x) => x != null);
            }
        }

        public List<Room> RoomSlots
        {
            get
            {
                return this.roomSlots;
            }
        }

        public bool IsFirstStorey
        {
            get
            {
                return this.Index == 0;
            }
        }

        public bool IsLastStorey
        {
            get
            {
                return this.Index == this.memory.storeys.Count - 1;
            }
        }

        public bool IsUndergroundStorey
        {
            get
            {
                return this.type == Storey.Type.Underground;
            }
        }

        public bool IsMainStorey
        {
            get
            {
                return this.type == Storey.Type.Main;
            }
        }

        public bool IsUpperStorey
        {
            get
            {
                return this.type == Storey.Type.Upper;
            }
        }

        public bool IsExtrasAboveStorey
        {
            get
            {
                return this.type == Storey.Type.ExtrasAbove;
            }
        }

        public HashSet<Vector3Int> Sewers
        {
            get
            {
                return this.sewers;
            }
        }

        public Storey StoreyAbove
        {
            get
            {
                if (!this.IsLastStorey)
                {
                    return this.memory.storeys[this.Index + 1];
                }
                return null;
            }
        }

        public Storey StoreyBelow
        {
            get
            {
                if (!this.IsFirstStorey)
                {
                    return this.memory.storeys[this.Index - 1];
                }
                return null;
            }
        }

        public Storey(Storey.Type type, WorldGenMemory memory)
        {
            this.type = type;
            this.memory = memory;
        }

        private WorldGenMemory memory;

        private Storey.Type type;

        private List<Room> roomSlots = new List<Room>();

        private HashSet<Vector3Int> sewers = new HashSet<Vector3Int>();

        public enum Type
        {
            Underground,

            Main,

            Upper,

            ExtrasAbove
        }
    }
}