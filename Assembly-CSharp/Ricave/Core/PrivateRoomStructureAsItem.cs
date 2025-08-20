using System;
using UnityEngine;

namespace Ricave.Core
{
    public class PrivateRoomStructureAsItem : Item
    {
        public EntitySpec PrivateRoomStructure
        {
            get
            {
                return this.privateRoomStructure;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.privateRoomStructure = value;
            }
        }

        public override string Label
        {
            get
            {
                return this.privateRoomStructure.Label;
            }
        }

        public override string Description
        {
            get
            {
                return this.privateRoomStructure.Description;
            }
        }

        public override Texture2D Icon
        {
            get
            {
                return this.privateRoomStructure.Icon;
            }
        }

        public override Color IconColor
        {
            get
            {
                return this.privateRoomStructure.IconColor;
            }
        }

        public override GameObject Prefab
        {
            get
            {
                return this.privateRoomStructure.Structure.PrivateRoomStructureItemPrefab;
            }
        }

        protected PrivateRoomStructureAsItem()
        {
        }

        public PrivateRoomStructureAsItem(EntitySpec spec)
            : base(spec)
        {
        }

        [Saved]
        private EntitySpec privateRoomStructure;
    }
}