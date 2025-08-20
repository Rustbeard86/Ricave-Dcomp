using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class DifficultySpec : Spec, ISaveableEventsReceiver
    {
        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
        }

        public float Order
        {
            get
            {
                return this.order;
            }
        }

        public float ScoreFactor
        {
            get
            {
                return this.scoreFactor;
            }
        }

        public float EnemyHPFactor
        {
            get
            {
                return this.enemyHPFactor;
            }
        }

        public int PlayerHPOffset
        {
            get
            {
                return this.playerHPOffset;
            }
        }

        public Conditions PlayerConditions
        {
            get
            {
                return this.playerConditions;
            }
        }

        public int ExtraBabies
        {
            get
            {
                return this.extraBabies;
            }
        }

        public string PlayerConditionsExplanation
        {
            get
            {
                return this.playerConditionsExplanation;
            }
        }

        public int BreadCountPerFloor
        {
            get
            {
                return this.breadCountPerFloor;
            }
        }

        public int UpToHealthPotionsPerFloor
        {
            get
            {
                return this.upToHealthPotionsPerFloor;
            }
        }

        public float RampUpMultiplier
        {
            get
            {
                return this.rampUpMultiplier;
            }
        }

        public float GoldPerFloorMultiplier
        {
            get
            {
                return this.goldPerFloorMultiplier;
            }
        }

        public int HealingCrystalsPerFloor
        {
            get
            {
                return this.healingCrystalsPerFloor;
            }
        }

        public int SmallHealingCrystalsPerFloor
        {
            get
            {
                return this.smallHealingCrystalsPerFloor;
            }
        }

        public int ManaCrystalsPerFloor
        {
            get
            {
                return this.manaCrystalsPerFloor;
            }
        }

        public int SmallManaCrystalsPerFloor
        {
            get
            {
                return this.smallManaCrystalsPerFloor;
            }
        }

        public float EnemyCountPerFloorMultiplier
        {
            get
            {
                return this.enemyCountPerFloorMultiplier;
            }
        }

        public int StartingHealthPotions
        {
            get
            {
                return this.startingHealthPotions;
            }
        }

        public float StardustFactor
        {
            get
            {
                return this.stardustFactor;
            }
        }

        public string Details
        {
            get
            {
                if (this.cachedDetails == null)
                {
                    this.cachedDetails = base.Description + "\n";
                    int num = Get.Entity_Player.MaxHP + this.PlayerHPOffset;
                    this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("StartingPlayerHP".Translate(), RichText.Bold(num.ToString()), 25);
                    if (this.enemyHPFactor != 1f)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("EnemyHPFactor".Translate(), RichText.Bold(this.enemyHPFactor.ToStringFactor()), 25);
                    }
                    if (this.rampUpMultiplier != 1f)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("DifficultyRampUpMultiplier".Translate(), RichText.Bold(this.rampUpMultiplier.ToStringFactor()), 25);
                    }
                    if (this.enemyCountPerFloorMultiplier != 1f)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("EnemyCountPerFloorMultiplier".Translate(), RichText.Bold(this.enemyCountPerFloorMultiplier.ToStringFactor()), 25);
                    }
                    if (this.extraBabies != 0)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("ExtraEnemies".Translate(), RichText.Bold(this.extraBabies.ToString()), 25);
                    }
                    if (this.healingCrystalsPerFloor != 0)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("HealingCrystalsPerFloor".Translate(), RichText.Bold(this.healingCrystalsPerFloor.ToString()), 25);
                    }
                    if (this.smallHealingCrystalsPerFloor != 0)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("SmallHealingCrystalsPerFloor".Translate(), RichText.Bold(this.smallHealingCrystalsPerFloor.ToString()), 25);
                    }
                    if (this.manaCrystalsPerFloor != 0)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("ManaCrystalsPerFloor".Translate(), RichText.Bold(this.manaCrystalsPerFloor.ToString()), 25);
                    }
                    if (this.smallManaCrystalsPerFloor != 0)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("SmallManaCrystalsPerFloor".Translate(), RichText.Bold(this.smallManaCrystalsPerFloor.ToString()), 25);
                    }
                    this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("UpToHealthPotionsPerFloor".Translate(), RichText.Bold(StringUtility.RangeToString(0, this.upToHealthPotionsPerFloor)), 25);
                    if (this.breadCountPerFloor != 3)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("BreadCountPerFloor".Translate(), RichText.Bold(this.breadCountPerFloor.ToString()), 25);
                    }
                    if (this.startingHealthPotions != 0)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("StartingHealthPotions".Translate(), RichText.Bold(this.startingHealthPotions.ToString()), 25);
                    }
                    if (this.goldPerFloorMultiplier != 1f)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("GoldPerFloorMultiplier".Translate(), RichText.Bold(this.goldPerFloorMultiplier.ToStringFactor()), 25);
                    }
                    if (this.stardustFactor != 1f)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("StardustFactor".Translate(), RichText.Bold(this.stardustFactor.ToStringFactor()), 25);
                    }
                    if (this.scoreFactor != 1f)
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + RichText.TableRowL("ScoreFactor".Translate(), RichText.Bold(this.scoreFactor.ToStringFactor()), 25);
                    }
                    if (!this.PlayerConditionsExplanation.NullOrEmpty())
                    {
                        this.cachedDetails = this.cachedDetails + "\n" + this.PlayerConditionsExplanation;
                    }
                }
                return this.cachedDetails;
            }
        }

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
                return;
            }
            Log.Error("DifficultySpec " + base.SpecID + " has no icon.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string iconPath;

        [Saved]
        private float order;

        [Saved(1f, false)]
        private float scoreFactor = 1f;

        [Saved(1f, false)]
        private float enemyHPFactor = 1f;

        [Saved]
        private int playerHPOffset;

        [Saved]
        private Conditions playerConditions;

        [Saved]
        private int extraBabies;

        [Saved]
        [Translatable]
        private string playerConditionsExplanation;

        [Saved]
        private int breadCountPerFloor;

        [Saved]
        private int upToHealthPotionsPerFloor;

        [Saved(1f, false)]
        private float rampUpMultiplier = 1f;

        [Saved(1f, false)]
        private float goldPerFloorMultiplier = 1f;

        [Saved]
        private int healingCrystalsPerFloor;

        [Saved]
        private int smallHealingCrystalsPerFloor;

        [Saved]
        private int manaCrystalsPerFloor;

        [Saved]
        private int smallManaCrystalsPerFloor;

        [Saved(1f, false)]
        private float enemyCountPerFloorMultiplier = 1f;

        [Saved]
        private int startingHealthPotions;

        [Saved(1f, false)]
        private float stardustFactor = 1f;

        private Texture2D icon;

        private string cachedDetails;
    }
}