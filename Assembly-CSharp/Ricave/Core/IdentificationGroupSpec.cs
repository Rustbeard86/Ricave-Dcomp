using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class IdentificationGroupSpec : Spec
    {
        public List<EntitySpec> ItemSpecs
        {
            get
            {
                return this.itemSpecs;
            }
        }

        public List<ItemLookSpec> ItemLookSpecs
        {
            get
            {
                return this.itemLookSpecs;
            }
        }

        public int PossibilitiesCountIfUnidentified
        {
            get
            {
                return this.possibilitiesCountIfUnidentified;
            }
        }

        public TargetFilter UseFilter
        {
            get
            {
                return this.useFilter;
            }
        }

        public bool ShowAoEEffectsEvenIfUnidentified
        {
            get
            {
                return this.showAoEEffectsEvenIfUnidentified;
            }
        }

        [Saved]
        private List<EntitySpec> itemSpecs;

        [Saved]
        private List<ItemLookSpec> itemLookSpecs;

        [Saved(3, false)]
        private int possibilitiesCountIfUnidentified = 3;

        [Saved(Default.New, false)]
        private TargetFilter useFilter = new TargetFilter();

        [Saved]
        private bool showAoEEffectsEvenIfUnidentified;

        private const int DefaultPossibilitiesCountIfUnidentified = 3;
    }
}