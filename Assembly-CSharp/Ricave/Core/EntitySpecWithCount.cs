using System;
using Ricave.UI;

namespace Ricave.Core
{
    public struct EntitySpecWithCount
    {
        public EntitySpec EntitySpec
        {
            get
            {
                return this.entitySpec;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public string RichLabel
        {
            get
            {
                string text = RichText.Label(this.entitySpec, false);
                if (this.count >= 2)
                {
                    text = "{0} {1}".Formatted(text, RichText.Bold("x{0}".Formatted(this.count)));
                }
                return text;
            }
        }

        public string Label
        {
            get
            {
                string text = this.entitySpec.LabelAdjusted;
                if (this.count >= 2)
                {
                    text = "{0} x{1}".Formatted(text, this.count.ToStringCached());
                }
                return text;
            }
        }

        public string LabelCap
        {
            get
            {
                return this.Label.CapitalizeFirst();
            }
        }

        public EntitySpecWithCount(EntitySpec entitySpec, int count)
        {
            this.entitySpec = entitySpec;
            this.count = count;
        }

        [Saved]
        private EntitySpec entitySpec;

        [Saved]
        private int count;
    }
}