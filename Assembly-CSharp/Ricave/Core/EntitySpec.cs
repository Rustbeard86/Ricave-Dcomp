using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class EntitySpec : Spec, ITipSubject, ISaveableEventsReceiver
    {
        public Type EntityClass
        {
            get
            {
                return this.entityClass;
            }
        }

        public string PrefabName
        {
            get
            {
                return this.prefabName;
            }
        }

        public GameObject Prefab
        {
            get
            {
                return this.prefab;
            }
        }

        public bool CanPassThrough
        {
            get
            {
                return this.canPassThrough;
            }
        }

        public bool CanProjectilesPassThrough
        {
            get
            {
                return this.canProjectilesPassThrough;
            }
        }

        public bool CanSeeThrough
        {
            get
            {
                return this.canSeeThrough;
            }
        }

        public bool CanSeeThroughToImpassable
        {
            get
            {
                return this.canSeeThrough || this.allowSeeThroughToImpassable;
            }
        }

        public bool BlocksDiagonalMovement
        {
            get
            {
                return this.blocksDiagonalMovement;
            }
        }

        public bool UsePositionOffsetFromStairs
        {
            get
            {
                return this.usePositionOffsetFromStairs;
            }
        }

        public bool RenderIfPlayerCantSee
        {
            get
            {
                return this.renderIfPlayerCantSee;
            }
        }

        public bool WantsUpdate
        {
            get
            {
                return this.wantsUpdate || this.IsActor || this.IsItem || (this.IsStructure && this.Structure.FallBehavior == StructureFallBehavior.Fall);
            }
        }

        public int MaxHP
        {
            get
            {
                return this.maxHP;
            }
        }

        public bool AutoAttackable
        {
            get
            {
                return this.autoAttackable;
            }
        }

        public bool DestroyOnFilledEntityDespawned
        {
            get
            {
                return this.destroyOnFilledEntityDespawned;
            }
        }

        public List<EntitySpec> DestroyOnEntityOfSpecDespawned
        {
            get
            {
                return this.destroyOnEntityOfSpecDespawned;
            }
        }

        public bool HasLifespanComp
        {
            get
            {
                return this.hasLifespanCompCached;
            }
        }

        public bool IsPermanent
        {
            get
            {
                return this.IsStructure && this.Structure.IsPermanent;
            }
        }

        public bool IsPermanentFilledImpassable
        {
            get
            {
                return this.isPermanentFilledImpassableCached;
            }
        }

        public bool IsSemiPermanent
        {
            get
            {
                return this.isSemiPermanentCached;
            }
        }

        public bool IsSemiPermanentFilledImpassable
        {
            get
            {
                return this.isSemiPermanentFilledImpassableCached;
            }
        }

        public bool IsLobbyItemOrLobbyRelated
        {
            get
            {
                return this.isLobbyItemOrLobbyRelated;
            }
        }

        public bool IsShield
        {
            get
            {
                return this.isShield;
            }
        }

        public string IconPath
        {
            get
            {
                return this.iconPath;
            }
        }

        public Color IconColor
        {
            get
            {
                ItemProps itemProps = this.Item;
                if (itemProps == null)
                {
                    return Color.white;
                }
                return itemProps.AutoCreatePrefabFromTexture_Color;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
        }

        public VisualEffectSpec DestroyEffect
        {
            get
            {
                return this.destroyEffect;
            }
        }

        public bool DisableDestroyEffect
        {
            get
            {
                return this.disableDestroyEffect;
            }
        }

        public SoundSpec DestroySound
        {
            get
            {
                return this.destroySound;
            }
        }

        public SoundSpec SoundLoop
        {
            get
            {
                return this.soundLoop;
            }
        }

        public float SoundLoopMaxDistance
        {
            get
            {
                return this.soundLoopMaxDistance;
            }
        }

        public Color? SpecialMinimapColor
        {
            get
            {
                return this.specialMinimapColor;
            }
        }

        public Color? SpecialSeenEntitiesDrawerColor
        {
            get
            {
                return this.specialSeenEntitiesDrawerColor;
            }
        }

        public Color? SpecialRichTextColor
        {
            get
            {
                return this.specialRichTextColor;
            }
        }

        public List<EntityCompProps> AllCompProps
        {
            get
            {
                return this.compProps;
            }
        }

        public ActorProps Actor
        {
            get
            {
                return this.actor;
            }
        }

        public ItemProps Item
        {
            get
            {
                return this.item;
            }
        }

        public StructureProps Structure
        {
            get
            {
                return this.structure;
            }
        }

        public EtherealProps Ethereal
        {
            get
            {
                return this.ethereal;
            }
        }

        public bool IsActor
        {
            get
            {
                return this.isActorCached;
            }
        }

        public bool IsItem
        {
            get
            {
                return this.isItemCached;
            }
        }

        public bool IsStructure
        {
            get
            {
                return this.isStructureCached;
            }
        }

        public bool IsEthereal
        {
            get
            {
                return this.isEtherealCached;
            }
        }

        public string LabelAdjusted
        {
            get
            {
                if (!this.IsItem || Get.IdentificationGroups.IsIdentified(this))
                {
                    return base.Label;
                }
                ItemLookSpec itemLook = Get.IdentificationGroups.GetItemLook(this);
                return ((itemLook != null) ? itemLook.Label : null) ?? base.Label;
            }
        }

        public string LabelAdjustedCap
        {
            get
            {
                return this.LabelAdjusted.CapitalizeFirst();
            }
        }

        public string DescriptionAdjusted
        {
            get
            {
                if (!this.IsItem || Get.IdentificationGroups.IsIdentified(this))
                {
                    return base.Description;
                }
                ItemLookSpec itemLook = Get.IdentificationGroups.GetItemLook(this);
                return ((itemLook != null) ? itemLook.Description : null) ?? base.Description;
            }
        }

        public GameObject PrefabAdjusted
        {
            get
            {
                ItemLookSpec itemLook = Get.IdentificationGroups.GetItemLook(this);
                return ((itemLook != null) ? itemLook.Prefab : null) ?? this.Prefab;
            }
        }

        public Texture2D IconAdjusted
        {
            get
            {
                ItemLookSpec itemLook = Get.IdentificationGroups.GetItemLook(this);
                return ((itemLook != null) ? itemLook.Icon : null) ?? this.Icon;
            }
        }

        public Color IconColorAdjusted
        {
            get
            {
                ItemLookSpec itemLook = Get.IdentificationGroups.GetItemLook(this);
                if (itemLook == null)
                {
                    return this.IconColor;
                }
                return itemLook.IconColor;
            }
        }

        public Texture2D IconAdjustedIfIdentified
        {
            get
            {
                if (Get.IdentificationGroups.IsIdentified(this))
                {
                    return this.IconAdjusted;
                }
                return this.Icon;
            }
        }

        public Color IconColorAdjustedIfIdentified
        {
            get
            {
                if (Get.IdentificationGroups.IsIdentified(this))
                {
                    return this.IconColorAdjusted;
                }
                return this.IconColor;
            }
        }

        public bool CanBeCursed
        {
            get
            {
                return this.IsItem && this.Item.IsEquippable && this.Item.TurnsToIdentify > 0 && Get.IdentificationGroups.GetIdentificationGroup(this) == null;
            }
        }

        public void OnLoaded()
        {
            this.isActorCached = typeof(Actor).IsAssignableFrom(this.entityClass);
            this.isItemCached = typeof(Item).IsAssignableFrom(this.entityClass);
            this.isStructureCached = typeof(Structure).IsAssignableFrom(this.entityClass);
            this.isEtherealCached = typeof(Ethereal).IsAssignableFrom(this.entityClass);
            if (this.IsActor && this.actor == null)
            {
                this.actor = new ActorProps();
            }
            if (this.IsItem && this.item == null)
            {
                this.item = new ItemProps();
            }
            if (this.IsStructure && this.structure == null)
            {
                this.structure = new StructureProps();
            }
            if (this.IsEthereal && this.ethereal == null)
            {
                this.ethereal = new EtherealProps();
            }
            if (this.actor != null)
            {
                this.actor.SetEntitySpec(this);
            }
            if (!this.prefabName.NullOrEmpty())
            {
                this.prefab = Assets.Get<GameObject>(this.prefabName);
            }
            else if (this.IsItem && this.Item.AutoCreatePrefabFromTextureTexture != null)
            {
                this.prefab = ItemPrefabCreator.CreatePrefab(this.Item.AutoCreatePrefabFromTextureTexture, base.SpecID, new Color?(this.Item.AutoCreatePrefabFromTexture_Color));
                this.destroyPrefabOnSpecDestroyed = true;
            }
            else if (this.IsStructure && this.Structure.AutoCreatePrefabFrom != null)
            {
                this.prefab = this.Structure.AutoCreatePrefabFrom.CreatePrefab(base.SpecID);
                this.destroyPrefabOnSpecDestroyed = true;
            }
            else if (this.IsActor && this.Actor.AutoCreatePrefabFromTextureTexture != null)
            {
                this.prefab = ActorPrefabCreator.CreatePrefab(this.Actor.AutoCreatePrefabFromTextureTexture, base.SpecID);
                this.destroyPrefabOnSpecDestroyed = true;
            }
            else
            {
                Log.Error("EntitySpec " + base.SpecID + " has no prefab.", false);
            }
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
            }
            else if (!this.IsEthereal)
            {
                Log.Error("EntitySpec " + base.SpecID + " has no icon.", false);
            }
            MeshRenderer meshRenderer;
            if (this.IsItem && this.prefab != null && this.prefab.TryGetComponent<MeshRenderer>(out meshRenderer) && meshRenderer.sharedMaterial != null)
            {
                Texture2D texture2D = meshRenderer.sharedMaterial.mainTexture as Texture2D;
                if (texture2D != null)
                {
                    this.item.Texture = texture2D;
                    goto IL_035C;
                }
            }
            MeshRenderer meshRenderer2;
            if (this.IsActor && this.prefab != null && this.prefab.TryGetComponent<MeshRenderer>(out meshRenderer2) && meshRenderer2.sharedMaterial != null)
            {
                Texture2D texture2D2 = meshRenderer2.sharedMaterial.mainTexture as Texture2D;
                if (texture2D2 != null)
                {
                    this.actor.Texture = texture2D2;
                    this.actor.OccupiedTextureRect = TextureBoundsFinder.FindBounds(texture2D2);
                    if (this.actor.OccupiedTextureRect.Empty())
                    {
                        this.actor.OccupiedTextureRect = new Rect(0.5f, 0.5f, 0f, 0f);
                    }
                    if (texture2D2.isReadable)
                    {
                        ShatterMeshes.CacheVisibilityMaskFor(texture2D2, new Rect?(this.actor.OccupiedTextureRect));
                    }
                }
            }
        IL_035C:
            if (this.IsActor)
            {
                if (this.actor.BodyMap != null)
                {
                    Rect[] array = BodyMapBoundsFinder.FindBounds(this.actor.BodyMap);
                    this.actor.NoBodyPartOccupiedTextureRect = array[0];
                    for (int i = 0; i < this.actor.BodyParts.Count; i++)
                    {
                        this.actor.BodyParts[i].OccupiedTextureRect = array[i + 1];
                    }
                }
                else
                {
                    this.actor.NoBodyPartOccupiedTextureRect = this.actor.OccupiedTextureRect;
                }
            }
            this.hasLifespanCompCached = false;
            for (int j = 0; j < this.compProps.Count; j++)
            {
                if (this.compProps[j] is LifespanCompProps)
                {
                    this.hasLifespanCompCached = true;
                    break;
                }
            }
            bool flag = false;
            if (this.IsStructure && this.structure.DefaultUseEffects != null)
            {
                foreach (UseEffect useEffect in this.structure.DefaultUseEffects.All)
                {
                    if (useEffect is UseEffect_DestroySelf || useEffect is UseEffect_DeSpawnSelf || useEffect is UseEffect_DestroyConnected || useEffect is UseEffect_OpenDoor || useEffect is UseEffect_CloseDoor)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            this.isSemiPermanentCached = this.IsStructure && !this.Structure.Flammable && !this.Structure.Fragile && !this.Structure.IsMineable && !this.Structure.IsDiggable && !this.Structure.IsGlass && this.Structure.FallBehavior == StructureFallBehavior.None && this.MaxHP == 0 && !this.HasLifespanComp && !flag && !this.destroyOnFilledEntityDespawned;
            this.isSemiPermanentFilledImpassableCached = this.isSemiPermanentCached && !this.canPassThrough && this.Structure.IsFilled;
            this.isPermanentFilledImpassableCached = this.IsPermanent && !this.canPassThrough && this.Structure.IsFilled;
            this.isLobbyItemOrLobbyRelated = this.IsItem && (this.Item.LobbyItem || typeof(UnlockableAsItem).IsAssignableFrom(this.entityClass) || typeof(PrivateRoomStructureAsItem).IsAssignableFrom(this.entityClass));
            bool flag2;
            if (this.IsItem && this.Item.DefaultUseEffects != null)
            {
                flag2 = this.Item.DefaultUseEffects.All.Any<UseEffect>(delegate (UseEffect x)
                {
                    UseEffect_AddCondition useEffect_AddCondition = x as UseEffect_AddCondition;
                    return useEffect_AddCondition != null && useEffect_AddCondition.Condition is Condition_Shield;
                });
            }
            else
            {
                flag2 = false;
            }
            this.isShield = flag2;
            if (this.canSeeThrough && this.allowSeeThroughToImpassable)
            {
                Log.Warning("Entity " + base.SpecID + " has both canSeeThrough and allowSeeThroughToImpassable set to true. Only canSeeThrough would be enough.", false);
            }
            if (this.IsStructure && this.Structure.AttachesToAnything && this.Structure.FallBehavior == StructureFallBehavior.None)
            {
                Log.Warning("Structure " + base.SpecID + " attaches to ceiling or wall but its FallBehavior is None, so it's not affected by gravity anyway and can be placed anywhere. This is probably wrong.", false);
            }
            if (this.usePositionOffsetFromStairs && this.IsStructure && this.Structure.AttachesToAnything)
            {
                Log.Warning("Structure " + base.SpecID + " has usePositionOffsetFromStairs set to true but it attaches to ceiling or wall. This is probably wrong.", false);
            }
            if (!this.canPassThrough && this.IsStructure && this.Structure.IsFilled && !this.blocksDiagonalMovement)
            {
                Log.Warning("Filled structure " + base.SpecID + " has canPassThrough set to false but it doesn't block diagonal movement. This is probably wrong.", false);
            }
            if (!this.canPassThrough && this.IsStructure && this.Structure.IsLadder)
            {
                Log.Warning("Structure " + base.SpecID + " is a ladder but it's impassable. This is probably wrong.", false);
            }
            if (!this.canPassThrough && this.IsStructure && this.Structure.IsStairs)
            {
                Log.Warning("Structure " + base.SpecID + " has isStairs set to true but it's impassable. This is probably wrong.", false);
            }
            if (this.IsPermanent)
            {
                if (this.Structure.Flammable)
                {
                    Log.Warning("Structure " + base.SpecID + " is both permanent and flammable.", false);
                }
                if (this.Structure.Fragile)
                {
                    Log.Warning("Structure " + base.SpecID + " is both permanent and fragile.", false);
                }
                if (this.Structure.IsMineable)
                {
                    Log.Warning("Structure " + base.SpecID + " is both permanent and mineable.", false);
                }
                if (this.Structure.IsDiggable)
                {
                    Log.Warning("Structure " + base.SpecID + " is both permanent and diggable.", false);
                }
                if (this.Structure.IsGlass)
                {
                    Log.Warning("Structure " + base.SpecID + " is both permanent and glass.", false);
                }
                if (this.Structure.FallBehavior != StructureFallBehavior.None)
                {
                    Log.Warning("Structure " + base.SpecID + " is both permanent and affected by gravity.", false);
                }
                if (this.MaxHP != 0)
                {
                    Log.Warning("Structure " + base.SpecID + " is permanent and has max HP defined.", false);
                }
                if (this.HasLifespanComp)
                {
                    Log.Warning("Structure " + base.SpecID + " is permanent and has lifespan comp.", false);
                }
                if (flag)
                {
                    Log.Warning("Structure " + base.SpecID + " is permanent and has despawn use effect.", false);
                }
                if (this.destroyOnFilledEntityDespawned)
                {
                    Log.Warning("Structure " + base.SpecID + " is permanent and uses destroyOnFilledEntityDespawned.", false);
                }
                if (!this.IsSemiPermanent)
                {
                    Log.Warning("Structure " + base.SpecID + " is permanent but at the same time non-semi-permanent.", false);
                }
            }
            if (this.IsActor)
            {
                if (!this.Actor.CanBeBoss && this.Actor.AlwaysBoss)
                {
                    Log.Warning("Actor " + base.SpecID + " has alwaysBoss set to true but canBeBoss set to false.", false);
                }
                if (this.Actor.Immobile)
                {
                    if (this.Actor.BodyParts.Any<BodyPartPlacement>((BodyPartPlacement x) => x.Spec == Get.BodyPart_Leg))
                    {
                        Log.Warning("Actor " + base.SpecID + " is immobile but has legs.", false);
                    }
                }
            }
        }

        public void OnSaved()
        {
        }

        public override void OnActiveLanguageChanged()
        {
            base.OnActiveLanguageChanged();
            if (this.item != null)
            {
                this.item.OnActiveLanguageChanged();
            }
            if (this.structure != null)
            {
                this.structure.OnActiveLanguageChanged();
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            if (this.destroyPrefabOnSpecDestroyed && this.prefab != null)
            {
                Object.Destroy(this.prefab);
            }
        }

        [Saved(typeof(Entity), false)]
        private Type entityClass = typeof(Entity);

        [Saved]
        private string prefabName;

        [Saved]
        private bool canPassThrough;

        [Saved]
        private bool canProjectilesPassThrough;

        [Saved]
        private bool canSeeThrough;

        [Saved]
        private bool allowSeeThroughToImpassable;

        [Saved]
        private bool blocksDiagonalMovement;

        [Saved]
        private bool usePositionOffsetFromStairs;

        [Saved]
        private bool renderIfPlayerCantSee;

        [Saved]
        private bool wantsUpdate;

        [Saved]
        private int maxHP;

        [Saved(true, false)]
        private bool autoAttackable = true;

        [Saved]
        private bool destroyOnFilledEntityDespawned;

        [Saved]
        private List<EntitySpec> destroyOnEntityOfSpecDespawned;

        [Saved]
        private string iconPath;

        [Saved]
        private VisualEffectSpec destroyEffect;

        [Saved]
        private bool disableDestroyEffect;

        [Saved]
        private SoundSpec destroySound;

        [Saved]
        private SoundSpec soundLoop;

        [Saved(10f, false)]
        private float soundLoopMaxDistance = 10f;

        [Saved]
        private Color? specialMinimapColor;

        [Saved]
        private Color? specialSeenEntitiesDrawerColor;

        [Saved]
        private Color? specialRichTextColor;

        [Saved(Default.New, false)]
        private List<EntityCompProps> compProps = new List<EntityCompProps>();

        [Saved]
        private ActorProps actor;

        [Saved]
        private ItemProps item;

        [Saved]
        private StructureProps structure;

        [Saved]
        private EtherealProps ethereal;

        private bool destroyPrefabOnSpecDestroyed;

        private GameObject prefab;

        private Texture2D icon;

        private bool isActorCached;

        private bool isItemCached;

        private bool isStructureCached;

        private bool isEtherealCached;

        private bool hasLifespanCompCached;

        private bool isPermanentFilledImpassableCached;

        private bool isSemiPermanentCached;

        private bool isSemiPermanentFilledImpassableCached;

        private bool isLobbyItemOrLobbyRelated;

        private bool isShield;
    }
}