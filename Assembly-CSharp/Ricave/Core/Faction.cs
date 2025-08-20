using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class Faction : ITipSubject
    {
        public FactionSpec Spec
        {
            get
            {
                return this.spec;
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

        public bool IsPlayer
        {
            get
            {
                return this == Get.Player.Faction;
            }
        }

        public bool IsPlayerFactionSpec
        {
            get
            {
                return this.Spec == Get.RunSpec.PlayerFaction;
            }
        }

        public Color Color
        {
            get
            {
                if (Get.MainActor == null)
                {
                    return Faction.DefaultNeutralColor;
                }
                if (this == Get.Player.Faction)
                {
                    return this.Color_Ally;
                }
                if (this.IsHostile(Get.Player.Faction))
                {
                    return this.Color_Hostile;
                }
                return this.Color_Neutral;
            }
        }

        public Color Color_Hostile
        {
            get
            {
                if (this.colorIndex_hostile == -1)
                {
                    for (int i = 0; i < Faction.Colors_Hostile.Length; i++)
                    {
                        bool flag = false;
                        using (List<Faction>.Enumerator enumerator = Get.FactionManager.Factions.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                if (enumerator.Current.colorIndex_hostile == i)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (!flag)
                        {
                            this.colorIndex_hostile = i;
                            break;
                        }
                    }
                    if (this.colorIndex_hostile == -1)
                    {
                        this.colorIndex_hostile = Calc.HashToRangeInclusive(Calc.CombineHashes<int, int>(this.spec.MyStableHash, 815341657), 0, Faction.Colors_Hostile.Length - 1);
                    }
                }
                return Faction.Colors_Hostile[this.colorIndex_hostile];
            }
        }

        public Color Color_Neutral
        {
            get
            {
                if (this.colorIndex_neutral == -1)
                {
                    for (int i = 0; i < Faction.Colors_Neutral.Length; i++)
                    {
                        bool flag = false;
                        using (List<Faction>.Enumerator enumerator = Get.FactionManager.Factions.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                if (enumerator.Current.colorIndex_neutral == i)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (!flag)
                        {
                            this.colorIndex_neutral = i;
                            break;
                        }
                    }
                    if (this.colorIndex_neutral == -1)
                    {
                        this.colorIndex_neutral = Calc.HashToRangeInclusive(Calc.CombineHashes<int, int>(this.spec.MyStableHash, 713255689), 0, Faction.Colors_Neutral.Length - 1);
                    }
                }
                return Faction.Colors_Neutral[this.colorIndex_neutral];
            }
        }

        public Color Color_Ally
        {
            get
            {
                return Faction.DefaultAllyColor;
            }
        }

        public Color IconColor
        {
            get
            {
                return this.Color;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.spec.Icon;
            }
        }

        public bool Hidden
        {
            get
            {
                if (this.spec.DefaultHidden)
                {
                    return true;
                }
                if (this.spec.HideIfNoMemberSpawned)
                {
                    bool flag = false;
                    using (List<Actor>.Enumerator enumerator = Get.World.Actors.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.Faction == this)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        protected Faction()
        {
        }

        public Faction(FactionSpec spec, string name = null)
        {
            this.spec = spec;
            this.name = name;
        }

        public bool IsHostile(Faction other)
        {
            return this != other && Get.FactionManager.HostilityExists(this, other);
        }

        [Saved]
        private FactionSpec spec;

        [Saved]
        private string name;

        [Saved(-1, false)]
        private int colorIndex_hostile = -1;

        [Saved(-1, false)]
        private int colorIndex_neutral = -1;

        private static readonly Color[] Colors_Hostile = new Color[]
        {
            new Color(0.8f, 0.05f, 0.05f),
            new Color32(224, 50, 82, byte.MaxValue),
            new Color32(224, 50, 121, byte.MaxValue),
            new Color32(231, 55, 40, byte.MaxValue),
            new Color32(229, 80, 18, byte.MaxValue)
        };

        private static readonly Color[] Colors_Neutral = new Color[]
        {
            new Color(0.8f, 0.8f, 0.8f),
            new Color32(139, 164, 166, byte.MaxValue),
            new Color32(131, 158, 171, byte.MaxValue),
            new Color32(130, 144, 190, byte.MaxValue),
            new Color32(135, 130, 190, byte.MaxValue),
            new Color32(149, 178, 168, byte.MaxValue),
            new Color32(149, 178, 158, byte.MaxValue),
            new Color32(154, 178, 149, byte.MaxValue)
        };

        public static readonly Color DefaultHostileColor = Faction.Colors_Hostile[0];

        public static readonly Color DefaultNeutralColor = Faction.Colors_Neutral[0];

        public static readonly Color DefaultAllyColor = new Color(0.05f, 0.7f, 0.05f);
    }
}