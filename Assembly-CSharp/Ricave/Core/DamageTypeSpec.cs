using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class DamageTypeSpec : Spec
    {
        public string Adjective
        {
            get
            {
                return this.adjective;
            }
        }

        public bool DestroyNPCRandomBodyPart
        {
            get
            {
                return this.destroyNPCRandomBodyPart;
            }
        }

        public bool CanDestroyPlayerBodyPart
        {
            get
            {
                return this.canDestroyPlayerBodyPart;
            }
        }

        public float SpreadOntoBodyPartChance
        {
            get
            {
                return this.spreadOntoBodyPartChance;
            }
        }

        public List<DamageTypeSpec.ConditionFromDamage> ConditionsFromDamage
        {
            get
            {
                return this.conditionsFromDamage;
            }
        }

        public string AdjectiveCap
        {
            get
            {
                if (this.adjectiveCapCached == null && this.adjective != null)
                {
                    this.adjectiveCapCached = this.adjective.CapitalizeFirst();
                }
                return this.adjectiveCapCached;
            }
        }

        [Saved]
        [Translatable]
        private string adjective;

        [Saved]
        private bool destroyNPCRandomBodyPart;

        [Saved]
        private bool canDestroyPlayerBodyPart;

        [Saved]
        private float spreadOntoBodyPartChance;

        [Saved(Default.New, false)]
        private List<DamageTypeSpec.ConditionFromDamage> conditionsFromDamage = new List<DamageTypeSpec.ConditionFromDamage>();

        private string adjectiveCapCached;

        public class ConditionFromDamage
        {
            public Condition Condition
            {
                get
                {
                    return this.condition;
                }
            }

            public float Chance
            {
                get
                {
                    return this.chance;
                }
            }

            protected ConditionFromDamage()
            {
            }

            [Saved]
            private Condition condition;

            [Saved]
            private float chance;
        }
    }
}