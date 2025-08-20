using System;

namespace Ricave.Core
{
    public class MonumentSpec : Spec
    {
        public BlueprintSpec BlueprintSpec
        {
            get
            {
                return this.blueprintSpec;
            }
        }

        public float SelectionWeight
        {
            get
            {
                return this.selectionWeight;
            }
        }

        public int MinFloor
        {
            get
            {
                return this.minFloor;
            }
        }

        [Saved]
        private BlueprintSpec blueprintSpec;

        [Saved(1f, false)]
        private float selectionWeight = 1f;

        [Saved]
        private int minFloor;
    }
}