using System;

namespace Ricave.Core
{
    public class DungeonModifierSpec : Spec
    {
        public DungeonModifierCategory Category
        {
            get
            {
                return this.category;
            }
        }

        public float UIOrder
        {
            get
            {
                return this.uiOrder;
            }
        }

        [Saved]
        private DungeonModifierCategory category;

        [Saved]
        private float uiOrder;
    }
}