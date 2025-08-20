using System;
using UnityEngine;

namespace Ricave.Core
{
    public class UnlockableAsItem : Item
    {
        public UnlockableSpec UnlockableSpec
        {
            get
            {
                return this.unlockableSpec;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.unlockableSpec = value;
            }
        }

        public override string Label
        {
            get
            {
                UnlockableSpec unlockableSpec = this.unlockableSpec;
                return ((unlockableSpec != null) ? unlockableSpec.Label : null) ?? base.Label;
            }
        }

        public override string Description
        {
            get
            {
                UnlockableSpec unlockableSpec = this.unlockableSpec;
                return ((unlockableSpec != null) ? unlockableSpec.Description : null) ?? base.Description;
            }
        }

        public override Texture2D Icon
        {
            get
            {
                UnlockableSpec unlockableSpec = this.unlockableSpec;
                return ((unlockableSpec != null) ? unlockableSpec.Icon : null) ?? base.Icon;
            }
        }

        public override GameObject Prefab
        {
            get
            {
                UnlockableSpec unlockableSpec = this.unlockableSpec;
                return ((unlockableSpec != null) ? unlockableSpec.ItemPrefab : null) ?? base.Prefab;
            }
        }

        protected UnlockableAsItem()
        {
        }

        public UnlockableAsItem(EntitySpec spec)
            : base(spec)
        {
        }

        [Saved]
        private UnlockableSpec unlockableSpec;
    }
}