using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class Place : ITipSubject
    {
        public PlaceSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public int InstanceID
        {
            get
            {
                return this.instanceID;
            }
        }

        public int StableID
        {
            get
            {
                return this.stableID;
            }
        }

        public int MyStableHash
        {
            get
            {
                return this.stableID;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string Label
        {
            get
            {
                if (!this.name.NullOrEmpty())
                {
                    return this.name;
                }
                return this.spec.Label;
            }
        }

        public string LabelCap
        {
            get
            {
                return this.Label.CapitalizeFirst();
            }
        }

        public string Description
        {
            get
            {
                return this.spec.Description;
            }
        }

        public int Floor
        {
            get
            {
                return this.floor;
            }
        }

        public Color IconColor
        {
            get
            {
                return Color.white;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.spec.Icon;
            }
        }

        public Vector2 PositionOnMap
        {
            get
            {
                return this.positionOnMap;
            }
        }

        public float EnemyCountFactor
        {
            get
            {
                return this.enemyCountFactor;
            }
        }

        public float GoldFactor
        {
            get
            {
                return this.goldFactor;
            }
        }

        public Place.CrystalType? Crystal
        {
            get
            {
                return this.crystal;
            }
        }

        public List<EntitySpec> Enemies
        {
            get
            {
                return this.enemies;
            }
        }

        public bool HasSpirit
        {
            get
            {
                return this.hasSpirit;
            }
        }

        public EntitySpecWithCount? ShelterItemReward
        {
            get
            {
                return this.shelterItemReward;
            }
        }

        public int ShelterItemRampUp
        {
            get
            {
                return this.shelterItemRampUp;
            }
        }

        public string ModifiersDetailsShort
        {
            get
            {
                if (this.cachedModifiersDetailsShort == null)
                {
                    this.cachedModifiersDetailsShort = "";
                    if (this.enemyCountFactor != 1f || this.spec != Get.Place_Normal)
                    {
                        if (this.enemyCountFactor > 1f)
                        {
                            this.cachedModifiersDetailsShort = this.cachedModifiersDetailsShort + RichText.CreateColorTag("MoreEnemies".Translate(), new Color(1f, 0.7f, 0.7f)) + "\n";
                        }
                        else if (this.enemyCountFactor < 1f)
                        {
                            this.cachedModifiersDetailsShort = this.cachedModifiersDetailsShort + RichText.CreateColorTag("LessEnemies".Translate(), new Color(0.7f, 1f, 0.7f)) + "\n";
                        }
                    }
                    if (this.goldFactor != 1f || this.spec != Get.Place_Normal)
                    {
                        if (this.goldFactor > 1f)
                        {
                            this.cachedModifiersDetailsShort = this.cachedModifiersDetailsShort + RichText.CreateColorTag("MoreGold".Translate(), new Color(0.7f, 1f, 0.7f)) + "\n";
                        }
                        else if (this.goldFactor < 1f)
                        {
                            this.cachedModifiersDetailsShort = this.cachedModifiersDetailsShort + RichText.CreateColorTag("LessGold".Translate(), new Color(1f, 0.7f, 0.7f)) + "\n";
                        }
                    }
                    if (this.crystal != null)
                    {
                        this.cachedModifiersDetailsShort = this.cachedModifiersDetailsShort + "PlaceCrystalShort".Translate(this.crystal.Value.GetLabelCap()) + "\n";
                    }
                    this.cachedModifiersDetailsShort = this.cachedModifiersDetailsShort.TrimEnd();
                }
                return this.cachedModifiersDetailsShort;
            }
        }

        public string ModifiersDetails
        {
            get
            {
                if (this.cachedModifiersDetails == null)
                {
                    this.cachedModifiersDetails = "";
                    if (this.enemyCountFactor != 1f)
                    {
                        this.cachedModifiersDetails = string.Concat(new string[]
                        {
                            this.cachedModifiersDetails,
                            "EnemyCountFactor".Translate(),
                            ": ",
                            this.enemyCountFactor.ToStringFactor(),
                            "\n"
                        });
                    }
                    if (this.goldFactor != 1f)
                    {
                        this.cachedModifiersDetails = string.Concat(new string[]
                        {
                            this.cachedModifiersDetails,
                            "GoldFactor".Translate(),
                            ": ",
                            this.goldFactor.ToStringFactor(),
                            "\n"
                        });
                    }
                    if (this.crystal != null)
                    {
                        this.cachedModifiersDetails = string.Concat(new string[]
                        {
                            this.cachedModifiersDetails,
                            "PlaceCrystal".Translate(),
                            ": ",
                            this.crystal.Value.GetLabelCap(),
                            "\n"
                        });
                    }
                    this.cachedModifiersDetails = this.cachedModifiersDetails.TrimEnd();
                }
                return this.cachedModifiersDetails;
            }
        }

        protected Place()
        {
        }

        public Place(PlaceSpec spec, Vector2 positionOnMap, int floor, string name = null)
        {
            this.spec = spec;
            this.positionOnMap = positionOnMap;
            this.name = name;
            this.floor = floor;
            this.instanceID = Get.UniqueIDsManager.NextPlaceID();
            this.stableID = Rand.UniqueID(Get.PlaceManager.Places.Select<Place, int>((Place x) => x.StableID));
        }

        public void PlaceGen_AssignDanger(float enemyCountFactor, float goldFactor, Place.CrystalType? crystal)
        {
            this.enemyCountFactor = enemyCountFactor;
            this.goldFactor = goldFactor;
            this.crystal = crystal;
        }

        public void PlaceGen_ChangePosition(Vector2 positionOnMap)
        {
            this.positionOnMap = positionOnMap;
        }

        public void PlaceGen_AssignShelterReward(EntitySpec itemSpec, int count, int itemRampUp)
        {
            this.shelterItemReward = new EntitySpecWithCount?(new EntitySpecWithCount(itemSpec, count));
            this.shelterItemRampUp = itemRampUp;
        }

        public void PlaceGen_AssignEnemies(List<EntitySpec> enemies)
        {
            this.enemies = new List<EntitySpec>();
            this.enemies.AddRange(enemies);
        }

        public void PlaceGen_SetHasSpirit()
        {
            this.hasSpirit = true;
        }

        [Saved]
        private PlaceSpec spec;

        [Saved(-1, false)]
        private int instanceID = -1;

        [Saved(-1, false)]
        private int stableID = -1;

        [Saved]
        private Vector2 positionOnMap;

        [Saved]
        private string name;

        [Saved(1, false)]
        private int floor = 1;

        [Saved(1f, false)]
        private float enemyCountFactor = 1f;

        [Saved(1f, false)]
        private float goldFactor = 1f;

        [Saved]
        private Place.CrystalType? crystal;

        [Saved]
        private List<EntitySpec> enemies;

        [Saved]
        private bool hasSpirit;

        [Saved]
        private EntitySpecWithCount? shelterItemReward;

        [Saved]
        private int shelterItemRampUp;

        private string cachedModifiersDetailsShort;

        private string cachedModifiersDetails;

        public enum CrystalType
        {
            Red,

            Green,

            Blue
        }
    }
}