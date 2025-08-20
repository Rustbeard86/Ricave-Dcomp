using System;
using System.Collections.Generic;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class StructureProps : ISaveableEventsReceiver
    {
        public bool IsFilled
        {
            get
            {
                return this.isFilled;
            }
        }

        public bool Flammable
        {
            get
            {
                return this.flammable;
            }
        }

        public bool Fragile
        {
            get
            {
                return this.fragile;
            }
        }

        public bool IsPermanent
        {
            get
            {
                return this.isPermanent;
            }
        }

        public bool IsSpecial
        {
            get
            {
                return this.isSpecial;
            }
        }

        public bool IsDoor
        {
            get
            {
                return this.isDoor;
            }
        }

        public bool IsStairs
        {
            get
            {
                return this.isStairs;
            }
        }

        public bool IsLadder
        {
            get
            {
                return this.isLadder;
            }
        }

        public bool IsWater
        {
            get
            {
                return this.isWater;
            }
        }

        public bool IsFluidSource
        {
            get
            {
                return this.isFluidSource;
            }
        }

        public bool IsCollidingFloorEmplacement
        {
            get
            {
                return this.isCollidingFloorEmplacement;
            }
        }

        public bool IsCollidingCeilingEmplacement
        {
            get
            {
                return this.isCollidingCeilingEmplacement;
            }
        }

        public bool Animated
        {
            get
            {
                return this.animated;
            }
        }

        public bool AIAvoids
        {
            get
            {
                return this.aiAvoids;
            }
        }

        public bool Fishingable
        {
            get
            {
                return this.fishingable;
            }
        }

        public bool Extinguishable
        {
            get
            {
                return this.extinguishable;
            }
        }

        public bool IsMineable
        {
            get
            {
                return this.isMineable;
            }
        }

        public bool IsDiggable
        {
            get
            {
                return this.isDiggable;
            }
        }

        public bool IsGlass
        {
            get
            {
                return this.isGlass;
            }
        }

        public UseEffects DefaultUseEffects
        {
            get
            {
                return this.defaultUseEffects;
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

        public bool AutoUseOnActorsPassing
        {
            get
            {
                return this.autoUseOnActorsPassing;
            }
        }

        public bool AutoUseOnDestroyed
        {
            get
            {
                return this.autoUseOnDestroyed;
            }
        }

        public bool AutoUseIfPlayerMovedOutOfTop
        {
            get
            {
                return this.autoUseIfPlayerMovedOutOfTop;
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

        public bool AllowUseViaInterface
        {
            get
            {
                return this.allowUseViaInterface;
            }
        }

        public bool InteractingIsFreeUIHint
        {
            get
            {
                return this.interactingIsFreeUIHint;
            }
        }

        public bool SupportsTargetingLocation
        {
            get
            {
                return this.isFilled || this.supportsTargetingLocation;
            }
        }

        public bool TargetingLocationToSelf
        {
            get
            {
                return this.targetingLocationToSelf;
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

        public bool AllowUseOnlyIfNoBoss
        {
            get
            {
                return this.allowUseOnlyIfNoBoss;
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

        public int CooldownTurns
        {
            get
            {
                return 0;
            }
        }

        public bool HideAdjacentFacesSameSpec
        {
            get
            {
                return this.hideAdjacentFacesSameSpec;
            }
        }

        public bool HideAdjacentFacesUnfoggedOrNotFilled
        {
            get
            {
                return this.hideAdjacentFacesUnfoggedOrNotFilled;
            }
        }

        public bool DisableShatterAnimation
        {
            get
            {
                return this.disableShatterAnimation;
            }
        }

        public bool BlocksBevels
        {
            get
            {
                return this.blocksBevels;
            }
        }

        public SoundSpec WalkThroughSound
        {
            get
            {
                return this.walkThroughSound;
            }
        }

        public SoundSpec WalkOnSound
        {
            get
            {
                return this.walkOnSound;
            }
        }

        public SoundSpec UsedLadderSound
        {
            get
            {
                return this.usedLadderSound;
            }
        }

        public StructureFallBehavior FallBehavior
        {
            get
            {
                return this.fallBehavior;
            }
        }

        public bool AttachesToCeiling
        {
            get
            {
                return this.attachesToCeiling;
            }
        }

        public bool AttachesToSameSpecAbove
        {
            get
            {
                return this.attachesToSameSpecAbove;
            }
        }

        public bool AttachesToBack
        {
            get
            {
                return this.attachesToBack;
            }
        }

        public bool AttachesToAnything
        {
            get
            {
                return this.attachesToCeiling || this.attachesToBack || this.attachesToSameSpecAbove;
            }
        }

        public bool GivesGravitySupportInside
        {
            get
            {
                return this.givesGravitySupportInside;
            }
        }

        public List<EntitySpec> DefaultLoot
        {
            get
            {
                return this.defaultLoot;
            }
        }

        public StructurePrefabCreator AutoCreatePrefabFrom
        {
            get
            {
                return this.autoCreatePrefabFrom;
            }
        }

        public float OffsetFromStairs
        {
            get
            {
                return this.offsetFromStairs;
            }
        }

        public bool DestroyConnectedOnDestroyed
        {
            get
            {
                return this.destroyConnectedOnDestroyed;
            }
        }

        public bool AutoRotateRandomly
        {
            get
            {
                return this.autoRotateRandomly;
            }
        }

        public bool AutoRotateRandomlySnap90
        {
            get
            {
                return this.autoRotateRandomlySnap90;
            }
        }

        public bool AutoRotateTowardsFree
        {
            get
            {
                return this.autoRotateTowardsFree;
            }
        }

        public bool AutoRotateTowardsFree_inRoomBounds
        {
            get
            {
                return this.autoRotateTowardsFree_inRoomBounds;
            }
        }

        public bool AutoRotateAwayFromWall
        {
            get
            {
                return this.autoRotateAwayFromWall;
            }
        }

        public bool AutoRotateTowardsCorner
        {
            get
            {
                return this.autoRotateTowardsCorner;
            }
        }

        public bool AutoRotateConnectToSameSpec
        {
            get
            {
                return this.autoRotateConnectToSameSpec;
            }
        }

        public PriceTag PrivateRoomPriceTag
        {
            get
            {
                return this.privateRoomPriceTag;
            }
        }

        public string PrivateRoomStructureItemPrefabName
        {
            get
            {
                return this.privateRoomStructureItemPrefabName;
            }
        }

        public GameObject PrivateRoomStructureItemPrefab
        {
            get
            {
                return this.privateRoomStructureItemPrefab;
            }
        }

        public string AutoCreatePrivateRoomStructureItemPrefabFromTexture
        {
            get
            {
                return this.autoCreatePrivateRoomStructureItemPrefabFromTexture;
            }
        }

        public Texture2D AutoCreatePrivateRoomStructureItemPrefabFromTextureTexture
        {
            get
            {
                return this.autoCreatePrivateRoomStructureItemPrefabFromTextureTexture;
            }
        }

        public void OnLoaded()
        {
            if (!this.autoCreatePrivateRoomStructureItemPrefabFromTexture.NullOrEmpty())
            {
                this.autoCreatePrivateRoomStructureItemPrefabFromTextureTexture = Assets.Get<Texture2D>(this.autoCreatePrivateRoomStructureItemPrefabFromTexture);
            }
            if (!this.privateRoomStructureItemPrefabName.NullOrEmpty())
            {
                this.privateRoomStructureItemPrefab = Assets.Get<GameObject>(this.privateRoomStructureItemPrefabName);
            }
            else if (this.autoCreatePrivateRoomStructureItemPrefabFromTextureTexture != null)
            {
                this.privateRoomStructureItemPrefab = ItemPrefabCreator.CreatePrefab(this.autoCreatePrivateRoomStructureItemPrefabFromTextureTexture, "PrivateRoomStructureItem", null);
            }
            if (!this.allowUseViaInterface && this.interactingIsFreeUIHint)
            {
                Log.Warning("StructureProps has allowUseViaInterface set to false but interactingIsFreeUIHint is true. This is useless so probably wrong.", false);
            }
        }

        public void OnSaved()
        {
        }

        public void OnActiveLanguageChanged()
        {
            TranslateUtility.ValidateTranslationKey(this.useDescriptionFormatKey_self);
            TranslateUtility.ValidateTranslationKey(this.useDescriptionFormatKey_other);
        }

        [Saved]
        private bool isFilled;

        [Saved]
        private bool flammable;

        [Saved]
        private bool fragile;

        [Saved]
        private bool isPermanent;

        [Saved]
        private bool isSpecial;

        [Saved]
        private bool isDoor;

        [Saved]
        private bool isStairs;

        [Saved]
        private bool isLadder;

        [Saved]
        private bool isWater;

        [Saved]
        private bool isFluidSource;

        [Saved]
        private bool isCollidingFloorEmplacement;

        [Saved]
        private bool isCollidingCeilingEmplacement;

        [Saved]
        private bool animated;

        [Saved]
        private bool aiAvoids;

        [Saved]
        private bool fishingable;

        [Saved]
        private bool extinguishable;

        [Saved]
        private bool isMineable;

        [Saved]
        private bool isDiggable;

        [Saved]
        private bool isGlass;

        [Saved]
        private UseEffects defaultUseEffects;

        [Saved(Default.New, false)]
        private TargetFilter useFilter = new TargetFilter();

        [Saved]
        private TargetFilter useFilterAoE;

        [Saved(1, false)]
        private int useRange = 1;

        [Saved]
        private bool autoUseOnActorsPassing;

        [Saved]
        private bool autoUseOnDestroyed;

        [Saved]
        private bool autoUseIfPlayerMovedOutOfTop;

        [Saved("UseDescription_Self", false)]
        private string useDescriptionFormatKey_self = "UseDescription_Self";

        [Saved("UseDescription_Other", false)]
        private string useDescriptionFormatKey_other = "UseDescription_Other";

        [Saved(true, false)]
        private bool allowUseViaInterface = true;

        [Saved]
        private bool interactingIsFreeUIHint;

        [Saved]
        private bool supportsTargetingLocation;

        [Saved]
        private bool targetingLocationToSelf;

        [Saved]
        private UsePrompt usePrompt;

        [Saved(1f, false)]
        private float sequencePerUseMultiplier = 1f;

        [Saved]
        private int manaCost;

        [Saved]
        private int staminaCost;

        [Saved]
        private bool allowUseOnlyIfNotSeenByHostiles;

        [Saved]
        private bool allowUseOnlyIfNoBoss;

        [Saved]
        private bool hideAdjacentFacesSameSpec;

        [Saved]
        private bool hideAdjacentFacesUnfoggedOrNotFilled;

        [Saved]
        private bool disableShatterAnimation;

        [Saved]
        private bool blocksBevels;

        [Saved]
        private SoundSpec walkThroughSound;

        [Saved]
        private SoundSpec walkOnSound;

        [Saved]
        private SoundSpec usedLadderSound;

        [Saved(StructureFallBehavior.None, false)]
        private StructureFallBehavior fallBehavior;

        [Saved]
        private bool attachesToCeiling;

        [Saved]
        private bool attachesToSameSpecAbove;

        [Saved]
        private bool attachesToBack;

        [Saved]
        private bool givesGravitySupportInside;

        [Saved(Default.Null, true)]
        private List<EntitySpec> defaultLoot;

        [Saved]
        private StructurePrefabCreator autoCreatePrefabFrom;

        [Saved(0.5f, false)]
        private float offsetFromStairs = 0.5f;

        [Saved]
        private bool destroyConnectedOnDestroyed;

        [Saved]
        private bool autoRotateRandomly;

        [Saved]
        private bool autoRotateRandomlySnap90;

        [Saved]
        private bool autoRotateTowardsFree;

        [Saved]
        private bool autoRotateTowardsFree_inRoomBounds;

        [Saved]
        private bool autoRotateAwayFromWall;

        [Saved]
        private bool autoRotateTowardsCorner;

        [Saved]
        private bool autoRotateConnectToSameSpec;

        [Saved]
        private PriceTag privateRoomPriceTag;

        [Saved]
        private string privateRoomStructureItemPrefabName;

        [Saved]
        private string autoCreatePrivateRoomStructureItemPrefabFromTexture;

        private GameObject privateRoomStructureItemPrefab;

        private Texture2D autoCreatePrivateRoomStructureItemPrefabFromTextureTexture;
    }
}