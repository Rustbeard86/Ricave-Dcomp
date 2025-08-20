using System;

namespace Ricave.Core
{
    public class BlueprintSpec : Spec
    {
        public Blueprint Blueprint
        {
            get
            {
                return this.blueprint;
            }
        }

        [Saved]
        private Blueprint blueprint;
    }
}