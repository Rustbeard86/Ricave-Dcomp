using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class ItemProps : ISaveableEventsReceiver
    {
        public bool Stackable
        {
            get
            {
                return this.stackable;
            }
        }

        public bool IsGoodConsumable
        {
            get
            {
                return this.isGoodConsumable;
            }
        }

        public bool IsBadConsumable
        {
            get
            {
                return this.isBadConsumable;
            }
        }

        public bool LobbyItem
        {
            get
            {
                return this.lobbyItem;
            }
        }

        public ItemRarity Rarity
        {
            get
            {
                return this.rarity;
            }
        }

        public int TurnsToIdentify
        {
            get
            {
                return this.turnsToIdentify;
            }
        }

        public List<string> GeneratorTags
        {
            get
            {
                return this.generatorTags;
            }
        }

        public bool DestroyOnDescend
        {
            get
            {
                return this.destroyOnDescend;
            }
        }

        public EntitySpec CookedItemSpec
        {
            get
            {
                return this.cookedItemSpec;
            }
        }

        public bool IsFood
        {
            get
            {
                return this.isFood;
            }
        }

        public int MarketValue
        {
            get
            {
                return this.marketValue;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return this.texture;
            }
            set
            {
                this.texture = value;
            }
        }

        public string AutoCreatePrefabFromTexture
        {
            get
            {
                return this.autoCreatePrefabFromTexture;
            }
        }

        public Texture2D AutoCreatePrefabFromTextureTexture
        {
            get
            {
                return this.autoCreatePrefabFromTextureTexture;
            }
        }

        public Color AutoCreatePrefabFromTexture_Color
        {
            get
            {
                return this.autoCreatePrefabFromTexture_color;
            }
        }

        public bool HideLobbyItem
        {
            get
            {
                return this.hideLobbyItem;
            }
        }

        public UseEffects DefaultUseEffects
        {
            get
            {
                return this.defaultUseEffects;
            }
        }

        public UseEffects MissUseEffects
        {
            get
            {
                return this.missUseEffects;
            }
        }

        public TargetFilter UseFilter
        {
            get
            {
                return this.useFilter;
            }
        }

        public TargetFilter UseFilterAoE
        {
            get
            {
                return this.useFilterAoE;
            }
        }

        public int UseRange
        {
            get
            {
                return this.useRange;
            }
        }

        public string UseDescriptionFormatKey_Self
        {
            get
            {
                return this.useDescriptionFormatKey_self;
            }
        }

        public string UseDescriptionFormatKey_Other
        {
            get
            {
                return this.useDescriptionFormatKey_other;
            }
        }

        public string UseLabelKey_Self
        {
            get
            {
                return this.useLabelKey_self;
            }
        }

        public string UseLabelKey_Other
        {
            get
            {
                return this.useLabelKey_other;
            }
        }

        public int DefaultUsesLeft
        {
            get
            {
                return this.defaultUsesLeft;
            }
        }

        public int DefaultChargesLeft
        {
            get
            {
                return this.defaultChargesLeft;
            }
        }

        public bool AllowUseOnSelfViaInterface
        {
            get
            {
                return this.allowUseOnSelfViaInterface;
            }
        }

        public int? CanRewindTimeEveryTurns
        {
            get
            {
                return this.canRewindTimeEveryTurns;
            }
        }

        public float SequencePerUseMultiplier
        {
            get
            {
                return this.sequencePerUseMultiplier;
            }
        }

        public int ManaCost
        {
            get
            {
                return this.manaCost;
            }
        }

        public int StaminaCost
        {
            get
            {
                return this.staminaCost;
            }
        }

        public float MissChance
        {
            get
            {
                return this.missChance;
            }
        }

        public float CritChance
        {
            get
            {
                return this.critChance;
            }
        }

        public UsePrompt UsePrompt
        {
            get
            {
                return this.usePrompt;
            }
        }

        public bool AllowUseOnlyIfNotSeenByHostiles
        {
            get
            {
                return this.allowUseOnlyIfNotSeenByHostiles;
            }
        }

        public int CooldownTurns
        {
            get
            {
                return this.cooldownTurns;
            }
        }

        public bool IsEquippableWeapon
        {
            get
            {
                return this.itemSlot == Get.ItemSlot_Weapon;
            }
        }

        public bool IsEquippableNonWeapon
        {
            get
            {
                return this.itemSlot != null && !this.IsEquippableWeapon;
            }
        }

        public bool IsEquippable
        {
            get
            {
                return this.itemSlot != null;
            }
        }

        public bool IsEquippableNonRing
        {
            get
            {
                return this.itemSlot != null && this.itemSlot != Get.ItemSlot_Ring;
            }
        }

        public Conditions DefaultConditionsEquipped
        {
            get
            {
                return this.defaultConditionsEquipped;
            }
        }

        public ItemSlotSpec ItemSlot
        {
            get
            {
                return this.itemSlot;
            }
        }

        public bool HealWearerOnKilled
        {
            get
            {
                return this.healWearerOnKilled;
            }
        }

        public bool Generator_IsCommonGear
        {
            get
            {
                return this.generatorTags.Contains("CommonGear");
            }
        }

        public bool Generator_IsGoodReward
        {
            get
            {
                return this.generatorTags.Contains("GoodReward");
            }
        }

        public bool Generator_IsPotion
        {
            get
            {
                return this.generatorTags.Contains("Potion");
            }
        }

        public bool Generator_IsScroll
        {
            get
            {
                return this.generatorTags.Contains("Scroll");
            }
        }

        public bool Generator_IsTrap
        {
            get
            {
                return this.generatorTags.Contains("Trap");
            }
        }

        public bool Generator_IsWand
        {
            get
            {
                return this.generatorTags.Contains("Wand");
            }
        }

        public bool Generator_IsBomb
        {
            get
            {
                return this.generatorTags.Contains("Bomb");
            }
        }

        public void OnLoaded()
        {
            if (!this.autoCreatePrefabFromTexture.NullOrEmpty())
            {
                this.autoCreatePrefabFromTextureTexture = Assets.Get<Texture2D>(this.autoCreatePrefabFromTexture);
                this.texture = this.autoCreatePrefabFromTextureTexture;
            }
        }

        public void OnSaved()
        {
        }

        public void OnActiveLanguageChanged()
        {
            TranslateUtility.ValidateTranslationKey(this.useDescriptionFormatKey_self);
            TranslateUtility.ValidateTranslationKey(this.useDescriptionFormatKey_other);
            TranslateUtility.ValidateTranslationKey(this.useLabelKey_self);
            TranslateUtility.ValidateTranslationKey(this.useLabelKey_other);
        }

        [Saved]
        private bool stackable;

        [Saved]
        private bool isGoodConsumable;

        [Saved]
        private bool isBadConsumable;

        [Saved]
        private bool lobbyItem;

        [Saved]
        private ItemRarity rarity;

        [Saved]
        private int turnsToIdentify;

        [Saved(Default.New, false)]
        private List<string> generatorTags = new List<string>();

        [Saved]
        private bool destroyOnDescend;

        [Saved]
        private EntitySpec cookedItemSpec;

        [Saved]
        private bool isFood;

        [Saved]
        private int marketValue;

        [Saved]
        private string autoCreatePrefabFromTexture;

        [Saved(Default.Color_White, false)]
        private Color autoCreatePrefabFromTexture_color = Color.white;

        [Saved]
        private bool hideLobbyItem;

        [Saved]
        private UseEffects defaultUseEffects;

        [Saved]
        private UseEffects missUseEffects;

        [Saved(Default.New, false)]
        private TargetFilter useFilter = new TargetFilter();

        [Saved]
        private TargetFilter useFilterAoE;

        [Saved(1, false)]
        private int useRange = 1;

        [Saved("UseDescription_Self", false)]
        private string useDescriptionFormatKey_self = "UseDescription_Self";

        [Saved("UseDescription_Other", false)]
        private string useDescriptionFormatKey_other = "UseDescription_Other";

        [Saved("UseLabel_Self", false)]
        private string useLabelKey_self = "UseLabel_Self";

        [Saved("UseLabel_Other", false)]
        private string useLabelKey_other = "UseLabel_Other";

        [Saved]
        private int defaultUsesLeft;

        [Saved]
        private int defaultChargesLeft;

        [Saved(true, false)]
        private bool allowUseOnSelfViaInterface = true;

        [Saved]
        private int? canRewindTimeEveryTurns;

        [Saved(1f, false)]
        private float sequencePerUseMultiplier = 1f;

        [Saved]
        private int manaCost;

        [Saved]
        private int staminaCost;

        [Saved]
        private float missChance;

        [Saved]
        private float critChance;

        [Saved]
        private UsePrompt usePrompt;

        [Saved]
        private bool allowUseOnlyIfNotSeenByHostiles;

        [Saved]
        private int cooldownTurns;

        [Saved]
        private Conditions defaultConditionsEquipped;

        [Saved]
        private ItemSlotSpec itemSlot;

        [Saved]
        private bool healWearerOnKilled;

        private Texture2D autoCreatePrefabFromTextureTexture;

        private Texture2D texture;
    }
}