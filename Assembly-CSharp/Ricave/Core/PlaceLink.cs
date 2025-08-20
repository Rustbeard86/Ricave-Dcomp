using System;

namespace Ricave.Core
{
    public class PlaceLink
    {
        public Place From
        {
            get
            {
                return this.from;
            }
        }

        public Place To
        {
            get
            {
                return this.to;
            }
        }

        public EntitySpecWithCount? ItemReward
        {
            get
            {
                return this.itemReward;
            }
        }

        public int HealHP
        {
            get
            {
                return this.healHP;
            }
        }

        public int ItemRampUp
        {
            get
            {
                return this.itemRampUp;
            }
        }

        public string RewardsDetails
        {
            get
            {
                if (this.cachedRewardsDetails == null)
                {
                    this.cachedRewardsDetails = "";
                    if (this.itemReward != null)
                    {
                        this.cachedRewardsDetails = this.cachedRewardsDetails + "- {0}".Formatted((this.itemRampUp == 0) ? this.itemReward.Value.LabelCap : this.itemReward.Value.LabelCap.AppendedWithSpace("+{0}".Formatted(this.itemRampUp))) + "\n";
                    }
                    if (this.healHP != 0)
                    {
                        this.cachedRewardsDetails = this.cachedRewardsDetails + "- {0}".Formatted("HealHPReward".Translate(this.healHP)) + "\n";
                    }
                    this.cachedRewardsDetails = this.cachedRewardsDetails.TrimEnd();
                    if (this.cachedRewardsDetails.Length == 0)
                    {
                        this.cachedRewardsDetails = "None".Translate();
                    }
                }
                return this.cachedRewardsDetails;
            }
        }

        protected PlaceLink()
        {
        }

        public PlaceLink(Place from, Place to)
        {
            this.from = from;
            this.to = to;
        }

        public void PlaceGen_AssignReward(EntitySpecWithCount? itemReward, int healHP, int itemRampUp = 0)
        {
            this.itemReward = itemReward;
            this.healHP = healHP;
            this.itemRampUp = itemRampUp;
        }

        public void PlaceGen_RelinkFrom(Place newFrom)
        {
            this.from = newFrom;
        }

        [Saved]
        private Place from;

        [Saved]
        private Place to;

        [Saved]
        private EntitySpecWithCount? itemReward;

        [Saved]
        private int healHP;

        [Saved]
        private int itemRampUp;

        private string cachedRewardsDetails;
    }
}