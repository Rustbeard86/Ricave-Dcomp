using System;

namespace Ricave.Core
{
    public class BodyPartSpec : Spec
    {
        public float MaxHPPct
        {
            get
            {
                return this.maxHPPct;
            }
        }

        public Condition ConditionOnDestroyed
        {
            get
            {
                return this.conditionOnDestroyed;
            }
        }

        public Condition ConditionOnAllDestroyed
        {
            get
            {
                return this.conditionOnAllDestroyed;
            }
        }

        public bool CausesActorToLimp
        {
            get
            {
                return this.causesActorToLimp;
            }
        }

        public bool IsArm
        {
            get
            {
                return this.isArm;
            }
        }

        public bool AreEyes
        {
            get
            {
                return this.areEyes;
            }
        }

        [Saved]
        private float maxHPPct;

        [Saved]
        private Condition conditionOnDestroyed;

        [Saved]
        private Condition conditionOnAllDestroyed;

        [Saved]
        private bool causesActorToLimp;

        [Saved]
        private bool isArm;

        [Saved]
        private bool areEyes;
    }
}