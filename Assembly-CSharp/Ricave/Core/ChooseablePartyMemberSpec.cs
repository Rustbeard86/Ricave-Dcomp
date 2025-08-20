using System;

namespace Ricave.Core
{
    public class ChooseablePartyMemberSpec : Spec
    {
        public EntitySpec ActorSpec
        {
            get
            {
                return this.actorSpec;
            }
        }

        public int RequiredSpirits
        {
            get
            {
                return this.requiredSpirits;
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
        private EntitySpec actorSpec;

        [Saved]
        private int requiredSpirits;

        [Saved]
        private float uiOrder;
    }
}