using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class ActorProps : ISaveableEventsReceiver
    {
        public AISpec AI
        {
            get
            {
                if (this.entitySpec == Get.Entity_Ghost && Get.Trait_Scary.IsChosen())
                {
                    return Get.AI_PlayerFollower;
                }
                return this.ai;
            }
        }

        public List<BodyPartPlacement> BodyParts
        {
            get
            {
                return this.bodyParts;
            }
        }

        public List<NativeWeaponProps> NativeWeapons
        {
            get
            {
                return this.nativeWeapons;
            }
        }

        public List<SpellSpec> DefaultSpells
        {
            get
            {
                return this.defaultSpells;
            }
        }

        public List<SpellSpec> RandomSpell
        {
            get
            {
                return this.randomSpell;
            }
        }

        public Conditions DefaultConditions
        {
            get
            {
                return this.defaultConditions;
            }
        }

        public int KilledExperience
        {
            get
            {
                return this.killedExperience;
            }
        }

        public int MoveNoiseRadius
        {
            get
            {
                return this.moveNoiseRadius;
            }
        }

        public float MaxHPRampUpFactor
        {
            get
            {
                if (!this.maxHPUsesRampUp)
                {
                    return 1f;
                }
                return 1.2500001f;
            }
        }

        public float GenerateSelectionWeight
        {
            get
            {
                return this.generateSelectionWeight;
            }
        }

        public int GenerateMinFloor
        {
            get
            {
                return this.generateMinFloor;
            }
        }

        public int MaxPerMap
        {
            get
            {
                return this.maxPerMap;
            }
        }

        public bool CanBeBoss
        {
            get
            {
                return this.canBeBoss;
            }
        }

        public bool CanBeBaby
        {
            get
            {
                return this.canBeBaby;
            }
        }

        public bool AlwaysBoss
        {
            get
            {
                return this.alwaysBoss;
            }
        }

        public bool UsingSpellsRequiresArms
        {
            get
            {
                return this.usingSpellsRequiresArms;
            }
        }

        public bool UsingSpellsRequiresEyes
        {
            get
            {
                return this.usingSpellsRequiresEyes;
            }
        }

        public int MaxMana
        {
            get
            {
                return this.maxMana;
            }
        }

        public bool CanFlyByDefault
        {
            get
            {
                return this.canFlyByDefault;
            }
        }

        public GenericUsableSpec UsableOnDeath
        {
            get
            {
                return this.usableOnDeath;
            }
        }

        public GenericUsableSpec UsableOnTalk
        {
            get
            {
                return this.usableOnTalk;
            }
        }

        public GenericUsableSpec UsableOnTalk2
        {
            get
            {
                return this.usableOnTalk2;
            }
        }

        public int ExtraKillScore
        {
            get
            {
                return this.extraKillScore;
            }
        }

        public FactionSpec DefaultFaction
        {
            get
            {
                FactionSpec factionSpec;
                if (!this.noDefaultFaction)
                {
                    if ((factionSpec = this.defaultFaction) == null)
                    {
                        return Get.Faction_Monsters;
                    }
                }
                else
                {
                    factionSpec = null;
                }
                return factionSpec;
            }
        }

        public bool Immobile
        {
            get
            {
                return this.immobile;
            }
        }

        public bool CanFlee
        {
            get
            {
                return this.canFlee;
            }
        }

        public bool CanBleed
        {
            get
            {
                return this.canBleed;
            }
        }

        public bool DisableFallDamage
        {
            get
            {
                return this.disableFallDamage;
            }
        }

        public bool PlayerLevelAffectsMaxHP
        {
            get
            {
                return this.playerLevelAffectsMaxHP;
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

        public Rect OccupiedTextureRect
        {
            get
            {
                return this.occupiedTextureRect;
            }
            set
            {
                this.occupiedTextureRect = value;
            }
        }

        public Rect NoBodyPartOccupiedTextureRect
        {
            get
            {
                return this.noBodyPartOccupiedTextureRect;
            }
            set
            {
                this.noBodyPartOccupiedTextureRect = value;
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

        public float ShatterForceMultiplier
        {
            get
            {
                return this.shatterForceMultiplier;
            }
        }

        public Texture2D BodyMap
        {
            get
            {
                return this.bodyMap;
            }
        }

        public FloatRange ScaleRange
        {
            get
            {
                return this.scaleRange;
            }
        }

        public Color SplatColor
        {
            get
            {
                return this.splatColor;
            }
        }

        public bool LayDirectionUp
        {
            get
            {
                return this.layDirectionUp;
            }
        }

        public SoundSpec CallSound
        {
            get
            {
                return this.callSound;
            }
        }

        public SoundSpec MoveSound
        {
            get
            {
                return this.moveSound;
            }
        }

        public bool UseMoveSoundOnlyIfCanFly
        {
            get
            {
                return this.useMoveSoundOnlyIfCanFly;
            }
        }

        public void OnLoaded()
        {
            if (!this.bodyMapPath.NullOrEmpty())
            {
                this.bodyMap = Assets.Get<Texture2D>(this.bodyMapPath);
            }
            if (!this.autoCreatePrefabFromTexture.NullOrEmpty())
            {
                this.autoCreatePrefabFromTextureTexture = Assets.Get<Texture2D>(this.autoCreatePrefabFromTexture);
                this.texture = this.autoCreatePrefabFromTextureTexture;
            }
            bool flag;
            if (this.defaultConditions != null)
            {
                flag = this.defaultConditions.All.Any<Condition>((Condition x) => x.CanFly);
            }
            else
            {
                flag = false;
            }
            this.canFlyByDefault = flag;
        }

        public void OnSaved()
        {
        }

        public void SetEntitySpec(EntitySpec entitySpec)
        {
            this.entitySpec = entitySpec;
        }

        private EntitySpec entitySpec;

        [Saved]
        private AISpec ai;

        [Saved(Default.New, false)]
        private List<BodyPartPlacement> bodyParts = new List<BodyPartPlacement>();

        [Saved(Default.New, false)]
        private List<NativeWeaponProps> nativeWeapons = new List<NativeWeaponProps>();

        [Saved(Default.New, true)]
        private List<SpellSpec> defaultSpells = new List<SpellSpec>();

        [Saved(Default.New, true)]
        private List<SpellSpec> randomSpell = new List<SpellSpec>();

        [Saved]
        private Conditions defaultConditions;

        [Saved]
        private int killedExperience;

        [Saved(1, false)]
        private int moveNoiseRadius = 1;

        [Saved]
        private bool maxHPUsesRampUp;

        [Saved]
        private float generateSelectionWeight;

        [Saved]
        private int generateMinFloor;

        [Saved(-1, false)]
        private int maxPerMap = -1;

        [Saved]
        private bool canBeBoss;

        [Saved(true, false)]
        private bool canBeBaby = true;

        [Saved]
        private bool alwaysBoss;

        [Saved]
        private bool usingSpellsRequiresArms;

        [Saved]
        private bool usingSpellsRequiresEyes;

        [Saved(10, false)]
        private int maxMana = 10;

        [Saved]
        private GenericUsableSpec usableOnDeath;

        [Saved]
        private GenericUsableSpec usableOnTalk;

        [Saved]
        private GenericUsableSpec usableOnTalk2;

        [Saved]
        private int extraKillScore;

        [Saved]
        private FactionSpec defaultFaction;

        [Saved]
        private bool noDefaultFaction;

        [Saved]
        private bool immobile;

        [Saved(true, false)]
        private bool canFlee = true;

        [Saved(true, false)]
        private bool canBleed = true;

        [Saved]
        private bool disableFallDamage;

        [Saved]
        private bool playerLevelAffectsMaxHP;

        [Saved]
        private string autoCreatePrefabFromTexture;

        [Saved(1f, false)]
        private float shatterForceMultiplier = 1f;

        [Saved]
        private string bodyMapPath;

        [Saved(Default.FloatRange_One, false)]
        private FloatRange scaleRange = new FloatRange(1f, 1f);

        [Saved(Default.Color_White, false)]
        private Color splatColor = Color.white;

        [Saved]
        private bool layDirectionUp;

        [Saved]
        private SoundSpec callSound;

        [Saved]
        private SoundSpec moveSound;

        [Saved]
        private bool useMoveSoundOnlyIfCanFly;

        private Texture2D bodyMap;

        private Texture2D autoCreatePrefabFromTextureTexture;

        private Texture2D texture;

        private Rect occupiedTextureRect;

        private Rect noBodyPartOccupiedTextureRect;

        private bool canFlyByDefault;
    }
}