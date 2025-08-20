using System;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class Item : Entity, IUsable, ITipSubject
    {
        public Conditions ConditionsEquipped
        {
            get
            {
                return this.conditionsEquipped;
            }
        }

        public UseEffects UseEffects
        {
            get
            {
                return this.useEffects;
            }
        }

        public UseEffects MissUseEffects
        {
            get
            {
                return this.missUseEffects;
            }
        }

        public int IdentifyOrder
        {
            get
            {
                return this.identifyOrder;
            }
            set
            {
                this.identifyOrder = value;
            }
        }

        public int TurnsLeftToIdentify
        {
            get
            {
                return this.turnsLeftToIdentify;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.turnsLeftToIdentify = value;
            }
        }

        public Inventory ParentInventory
        {
            get
            {
                return this.parentInventory;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.parentInventory = value;
            }
        }

        public bool Cursed
        {
            get
            {
                return this.cursed;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.cursed = value;
            }
        }

        public int StackCount
        {
            get
            {
                return this.stackCount;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.stackCount = value;
            }
        }

        public int UsesLeft
        {
            get
            {
                return this.usesLeft;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.usesLeft = value;
            }
        }

        public int ChargesLeft
        {
            get
            {
                return this.chargesLeft;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.chargesLeft = value;
            }
        }

        public PriceTag PriceTag
        {
            get
            {
                return this.priceTag;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.priceTag = value;
            }
        }

        public bool ForSale
        {
            get
            {
                return this.priceTag != null;
            }
        }

        public int RampUp
        {
            get
            {
                return this.rampUp;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.rampUp = value;
            }
        }

        public bool AffectedByRampUp
        {
            get
            {
                return RampUpUtility.AffectedByRampUp(this);
            }
        }

        public TitleSpec Title
        {
            get
            {
                return this.title;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.title = value;
            }
        }

        public ItemGOC ItemGOC
        {
            get
            {
                return (ItemGOC)base.EntityGOC;
            }
        }

        public float TimeAddedToInventory
        {
            get
            {
                return this.timeAddedToInventory;
            }
        }

        public ItemLookSpec CustomLook
        {
            get
            {
                return this.customLook;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.customLook = value;
            }
        }

        public virtual int MarketValue
        {
            get
            {
                return base.Spec.Item.MarketValue;
            }
        }

        public override string Label
        {
            get
            {
                string text;
                if (this.title == null || !this.Identified)
                {
                    ItemLookSpec itemLookSpec = this.CustomLook;
                    text = ((itemLookSpec != null) ? itemLookSpec.Label : null) ?? base.Label;
                }
                else
                {
                    string labelFormat = this.title.LabelFormat;
                    ItemLookSpec itemLookSpec2 = this.CustomLook;
                    text = labelFormat.Formatted(((itemLookSpec2 != null) ? itemLookSpec2.Label : null) ?? base.Label);
                }
                return StringUtility.Concat(text, (this.rampUp != 0 && this.Identified) ? " {0}".Formatted(this.rampUp.ToStringOffset(true)) : "", (this.stackCount >= 2) ? " x{0}".Formatted(this.stackCount.ToStringCached()) : "", (this.usesLeft > 0) ? RichText.Uses(" ({0})".Formatted(StringUtility.UsesString(this.usesLeft))) : "", (this.chargesLeft > 0) ? RichText.Charges(" ({0})".Formatted(StringUtility.ChargesString(this.chargesLeft))) : "", LastUsedToRewindTimeUtility.LastUsedToRewindTimeRichString(this));
            }
        }

        public override string Description
        {
            get
            {
                ItemLookSpec itemLookSpec = this.CustomLook;
                return ((itemLookSpec != null) ? itemLookSpec.Description : null) ?? base.Description;
            }
        }

        public override GameObject Prefab
        {
            get
            {
                ItemLookSpec itemLookSpec = this.CustomLook;
                return ((itemLookSpec != null) ? itemLookSpec.Prefab : null) ?? base.Prefab;
            }
        }

        public override Texture2D Icon
        {
            get
            {
                ItemLookSpec itemLookSpec = this.CustomLook;
                return ((itemLookSpec != null) ? itemLookSpec.Icon : null) ?? base.Icon;
            }
        }

        public override Color IconColor
        {
            get
            {
                ItemLookSpec itemLookSpec = this.CustomLook;
                if (itemLookSpec == null)
                {
                    return base.IconColor;
                }
                return itemLookSpec.IconColor;
            }
        }

        public string UseLabel_Self
        {
            get
            {
                return UsableUtility.CheckAppendDotsToUseLabel(base.Spec.Item.UseLabelKey_Self.Translate(), this);
            }
        }

        public string UseLabel_Other
        {
            get
            {
                return base.Spec.Item.UseLabelKey_Other.Translate();
            }
        }

        public string UseDescriptionFormat_Self
        {
            get
            {
                return base.Spec.Item.UseDescriptionFormatKey_Self.Translate();
            }
        }

        public string UseDescriptionFormat_Other
        {
            get
            {
                return base.Spec.Item.UseDescriptionFormatKey_Other.Translate();
            }
        }

        public TargetFilter UseFilter
        {
            get
            {
                TargetFilter targetFilter;
                if (!this.Identified)
                {
                    IdentificationGroupSpec identificationGroup = Get.IdentificationGroups.GetIdentificationGroup(base.Spec);
                    if ((targetFilter = ((identificationGroup != null) ? identificationGroup.UseFilter : null)) == null)
                    {
                        return base.Spec.Item.UseFilter;
                    }
                }
                else
                {
                    targetFilter = base.Spec.Item.UseFilter;
                }
                return targetFilter;
            }
        }

        public TargetFilter UseFilterAoE
        {
            get
            {
                return base.Spec.Item.UseFilterAoE ?? base.Spec.Item.UseFilter;
            }
        }

        public int UseRange
        {
            get
            {
                return base.Spec.Item.UseRange;
            }
        }

        int? IUsable.LastUsedToRewindTimeSequence
        {
            get
            {
                return this.lastUsedToRewindTimeSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastUsedToRewindTimeSequence = value;
            }
        }

        int? IUsable.LastUseSequence
        {
            get
            {
                return this.lastUseSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastUseSequence = value;
            }
        }

        int? IUsable.CanRewindTimeEveryTurns
        {
            get
            {
                return base.Spec.Item.CanRewindTimeEveryTurns;
            }
        }

        public float SequencePerUseMultiplier
        {
            get
            {
                return base.Spec.Item.SequencePerUseMultiplier;
            }
        }

        public int ManaCost
        {
            get
            {
                return base.Spec.Item.ManaCost;
            }
        }

        public int StaminaCost
        {
            get
            {
                if (Get.Skill_ShieldStamina.IsUnlocked() && base.Spec.IsShield)
                {
                    Inventory inventory = this.ParentInventory;
                    if (inventory != null && inventory.Owner.IsMainActor)
                    {
                        return Calc.RoundToIntHalfUp((float)base.Spec.Item.StaminaCost / 2f);
                    }
                }
                return base.Spec.Item.StaminaCost;
            }
        }

        public int CooldownTurns
        {
            get
            {
                return base.Spec.Item.CooldownTurns;
            }
        }

        public float MissChance
        {
            get
            {
                return base.Spec.Item.MissChance;
            }
        }

        public float CritChance
        {
            get
            {
                return base.Spec.Item.CritChance;
            }
        }

        UsePrompt IUsable.UsePrompt
        {
            get
            {
                if (!this.Identified)
                {
                    return null;
                }
                return base.Spec.Item.UsePrompt;
            }
        }

        public bool Identified
        {
            get
            {
                return Get.DeathScreenDrawer.ShouldShow || this.turnsLeftToIdentify <= 0 || Get.IdentificationGroups.IsIdentified(base.Spec) || (this.CustomLook != null && Get.IdentificationGroups.GetIdentificationGroup(base.Spec) != null) || Get.DungeonModifier_ItemsStartIdentified.IsActiveAndAppliesToCurrentRun();
            }
        }

        protected Item()
        {
        }

        public Item(EntitySpec spec)
            : base(spec)
        {
            this.conditionsEquipped = new Conditions(this);
            this.useEffects = new UseEffects(this);
            this.missUseEffects = new UseEffects(this);
            this.turnsLeftToIdentify = spec.Item.TurnsToIdentify;
            this.usesLeft = spec.Item.DefaultUsesLeft;
            this.chargesLeft = spec.Item.DefaultChargesLeft;
            UseEffects defaultUseEffects = spec.Item.DefaultUseEffects;
            if (defaultUseEffects != null)
            {
                this.useEffects.AddClonedFrom(defaultUseEffects);
            }
            UseEffects useEffects = spec.Item.MissUseEffects;
            if (useEffects != null)
            {
                this.missUseEffects.AddClonedFrom(useEffects);
            }
            Conditions defaultConditionsEquipped = spec.Item.DefaultConditionsEquipped;
            if (defaultConditionsEquipped != null)
            {
                this.conditionsEquipped.AddClonedFrom(defaultConditionsEquipped);
            }
        }

        public void StartJumpAnimation()
        {
            if (this.ItemGOC != null)
            {
                this.ItemGOC.StartJumpAnimation();
            }
        }

        public void StopJumpAnimation()
        {
            if (this.ItemGOC != null)
            {
                this.ItemGOC.StopJumpAnimation();
            }
        }

        public bool CanUse_ExtraInstanceSpecificChecks(Actor user, Vector3Int? assumeUserPos = null, bool assumeAnyUserPos = false, StringSlot outReason = null)
        {
            if (!user.Inventory.Contains(this))
            {
                return false;
            }
            if (!user.HasArm)
            {
                if (outReason != null)
                {
                    outReason.Set("NoArms".Translate());
                }
                return false;
            }
            if (base.Spec.Item.AllowUseOnlyIfNotSeenByHostiles && UsableUtility.SeenByAnyHostile(user))
            {
                if (outReason != null)
                {
                    outReason.Set("SeenByHostiles".Translate());
                }
                return false;
            }
            return true;
        }

        public void OnRemovedFromInventory(Inventory inventory)
        {
            this.lastRemovedFromInventory = inventory;
            this.lastRemovedFromInventoryFrame = Clock.Frame;
        }

        public void OnAddedToInventory()
        {
            if (!Maker.MakingEntity && (this.lastRemovedFromInventory != this.parentInventory || this.lastRemovedFromInventoryFrame != Clock.Frame))
            {
                this.timeAddedToInventory = Clock.UnscaledTime;
            }
        }

        public bool CanStackWith(Item other)
        {
            return base.Spec.Item.Stackable && (base.Spec == other.Spec && this.RampUp == other.RampUp && this.Cursed == other.Cursed) && this.Title == other.Title;
        }

        public Item SplitOff(int count)
        {
            if (count <= 0)
            {
                return null;
            }
            if (count >= this.stackCount)
            {
                return this;
            }
            Item item = this.CloneOne();
            item.stackCount = count;
            return item;
        }

        public Item CloneOne()
        {
            return Maker.Make<Item>(base.Spec, delegate (Item x)
            {
                x.TurnsLeftToIdentify = this.turnsLeftToIdentify;
                x.Cursed = this.cursed;
                x.UsesLeft = this.usesLeft;
                x.ChargesLeft = this.chargesLeft;
                x.RampUp = this.RampUp;
                x.Title = this.title;
                x.lastUsedToRewindTimeSequence = this.lastUsedToRewindTimeSequence;
                x.lastUseSequence = this.lastUseSequence;
                x.ConditionsEquipped.Clear();
                x.ConditionsEquipped.AddClonedFrom(this.conditionsEquipped);
                x.UseEffects.Clear();
                x.UseEffects.AddClonedFrom(this.useEffects);
            }, false, false, true);
        }

        [Saved]
        private Conditions conditionsEquipped;

        [Saved]
        private UseEffects useEffects;

        [Saved]
        private UseEffects missUseEffects;

        [Saved]
        private int identifyOrder;

        [Saved]
        private int turnsLeftToIdentify;

        [Saved]
        private Inventory parentInventory;

        [Saved]
        private bool cursed;

        [Saved(1, false)]
        private int stackCount = 1;

        [Saved]
        private int? lastUsedToRewindTimeSequence;

        [Saved]
        private int? lastUseSequence;

        [Saved]
        private int usesLeft;

        [Saved]
        private int chargesLeft;

        [Saved]
        private PriceTag priceTag;

        [Saved]
        private int rampUp;

        [Saved]
        private TitleSpec title;

        [Saved]
        private ItemLookSpec customLook;

        private float timeAddedToInventory = -99999f;

        private Inventory lastRemovedFromInventory;

        private int lastRemovedFromInventoryFrame = -99999;
    }
}