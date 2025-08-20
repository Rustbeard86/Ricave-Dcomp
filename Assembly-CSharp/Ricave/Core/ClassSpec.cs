using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class ClassSpec : Spec, ITipSubject, ISaveableEventsReceiver
    {
        public string IconPath
        {
            get
            {
                return this.iconPath;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
        }

        public Color IconColor
        {
            get
            {
                return ClassSpec.ClassColor;
            }
        }

        public float UIOrder
        {
            get
            {
                return this.uiOrder;
            }
        }

        public List<string> GoodEffects
        {
            get
            {
                return this.goodEffects;
            }
        }

        public List<string> BadEffects
        {
            get
            {
                return this.badEffects;
            }
        }

        public string EffectsString
        {
            get
            {
                if (this.effectsString == null)
                {
                    ClassSpec.tmpSb.Clear();
                    ClassSpec.tmpSb.Append("ClassEffects".Translate());
                    ClassSpec.tmpSb.Append(":");
                    foreach (string text in this.goodEffects)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag(text, GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.spells.Count > 0)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        string text2 = string.Join(", ", this.spells.Select<SpellSpec, string>((SpellSpec s) => RichText.Bold(s.LabelCap)));
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassSpells".Translate(text2), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.maxHPOffset > 0)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("ClassMaxHPOffset".Translate(), this.maxHPOffset.ToStringOffset(true)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.maxManaOffset > 0)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("ClassMaxManaOffset".Translate(), this.maxManaOffset.ToStringOffset(true)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.maxStaminaOffset > 0)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("ClassMaxStaminaOffset".Translate(), this.maxStaminaOffset.ToStringOffset(true)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.damageFactorAgainstUndead > 1f)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassDamageFactorAgainstUndead".Translate(this.damageFactorAgainstUndead.ToStringFactor()), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.hungerRateFactor < 1f)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassHungerRateFactor".Translate(this.hungerRateFactor.ToStringFactor()), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.disablesPressurePlates)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassDisablesPressurePlates".Translate(), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.immuneToPoison)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassImmuneToPoison".Translate(), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.immuneToPushing)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassImmuneToPushing".Translate(), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.fireIncomingDamageFactor < 1f)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassFireIncomingDamageFactor_Less".Translate((1f - this.fireIncomingDamageFactor).ToStringPercent(false)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.walkingNoiseOffset < 0)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassWalkingNoiseOffset".Translate(this.walkingNoiseOffset.ToStringOffset(true)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    foreach (string text3 in this.badEffects)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag(text3, GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.maxHPOffset < 0)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("ClassMaxHPOffset".Translate(), this.maxHPOffset.ToStringOffset(true)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.maxManaOffset < 0)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("ClassMaxManaOffset".Translate(), this.maxManaOffset.ToStringOffset(true)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.maxStaminaOffset < 0)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("ClassMaxStaminaOffset".Translate(), this.maxStaminaOffset.ToStringOffset(true)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.damageFactorAgainstUndead < 1f)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassDamageFactorAgainstUndead".Translate(this.damageFactorAgainstUndead.ToStringFactor()), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.hungerRateFactor > 1f)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassHungerRateFactor".Translate(this.hungerRateFactor.ToStringFactor()), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.dogsAreHostile)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassDogsAreHostile".Translate(), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.fireIncomingDamageFactor > 1f)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassFireIncomingDamageFactor_More".Translate((this.fireIncomingDamageFactor - 1f).ToStringPercent(false)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.walkingNoiseOffset > 0)
                    {
                        ClassSpec.tmpSb.AppendLine();
                        ClassSpec.tmpSb.Append("- ");
                        ClassSpec.tmpSb.Append(RichText.CreateColorTag("ClassWalkingNoiseOffset".Translate(this.walkingNoiseOffset.ToStringOffset(true)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    this.effectsString = ClassSpec.tmpSb.ToString();
                }
                return this.effectsString;
            }
        }

        public List<SpellSpec> Spells
        {
            get
            {
                return this.spells;
            }
        }

        public int MaxHPOffset
        {
            get
            {
                return this.maxHPOffset;
            }
        }

        public int MaxManaOffset
        {
            get
            {
                return this.maxManaOffset;
            }
        }

        public int MaxStaminaOffset
        {
            get
            {
                return this.maxStaminaOffset;
            }
        }

        public float DamageFactorAgainstUndead
        {
            get
            {
                return this.damageFactorAgainstUndead;
            }
        }

        public float HungerRateFactor
        {
            get
            {
                return this.hungerRateFactor;
            }
        }

        public bool DisablesPressurePlates
        {
            get
            {
                return this.disablesPressurePlates;
            }
        }

        public bool DogsAreHostile
        {
            get
            {
                return this.dogsAreHostile;
            }
        }

        public bool ImmuneToPoison
        {
            get
            {
                return this.immuneToPoison;
            }
        }

        public bool ImmuneToPushing
        {
            get
            {
                return this.immuneToPushing;
            }
        }

        public float FireIncomingDamageFactor
        {
            get
            {
                return this.fireIncomingDamageFactor;
            }
        }

        public int WalkingNoiseOffset
        {
            get
            {
                return this.walkingNoiseOffset;
            }
        }

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
                return;
            }
            Log.Error("ClassSpec " + base.SpecID + " has no icon.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string iconPath;

        [Saved]
        private float uiOrder;

        [Saved(Default.New, false)]
        [Translatable]
        private List<string> goodEffects = new List<string>();

        [Saved(Default.New, false)]
        [Translatable]
        private List<string> badEffects = new List<string>();

        [Saved(Default.New, true)]
        private List<SpellSpec> spells = new List<SpellSpec>();

        [Saved]
        private int maxHPOffset;

        [Saved]
        private int maxManaOffset;

        [Saved]
        private int maxStaminaOffset;

        [Saved(1f, false)]
        private float damageFactorAgainstUndead = 1f;

        [Saved(1f, false)]
        private float hungerRateFactor = 1f;

        [Saved]
        private bool disablesPressurePlates;

        [Saved]
        private bool dogsAreHostile;

        [Saved]
        private bool immuneToPoison;

        [Saved]
        private bool immuneToPushing;

        [Saved(1f, false)]
        private float fireIncomingDamageFactor = 1f;

        [Saved]
        private int walkingNoiseOffset;

        private static StringBuilder tmpSb = new StringBuilder();

        private Texture2D icon;

        private string effectsString;

        public static readonly Color ClassColor = new Color(1f, 1f, 0.5f);
    }
}