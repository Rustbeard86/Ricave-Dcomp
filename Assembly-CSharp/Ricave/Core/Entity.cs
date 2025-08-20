using System;
using System.Collections.Generic;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public abstract class Entity : ITipSubject
    {
        public EntitySpec Spec
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

        public bool Spawned
        {
            get
            {
                return this.spawned;
            }
        }

        public GameObject GameObject
        {
            get
            {
                return this.gameObject;
            }
            set
            {
                this.gameObject = value;
            }
        }

        public EntityGOC EntityGOC
        {
            get
            {
                return this.cachedEntityGOC;
            }
        }

        public List<EntityComp> AllComps
        {
            get
            {
                return this.comps.AllComps;
            }
        }

        public int HP
        {
            get
            {
                return this.hp;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.hp = value;
            }
        }

        public virtual int MaxHP
        {
            get
            {
                return this.Spec.MaxHP;
            }
        }

        public Vector3 RenderPosition
        {
            get
            {
                if (!(this.EntityGOC != null))
                {
                    return this.Position;
                }
                return this.EntityGOC.transform.position;
            }
        }

        public Vector3 RenderPositionComputedCenter
        {
            get
            {
                if (!(this.EntityGOC != null))
                {
                    return this.Position;
                }
                return EntityGameObjectCenterCalculator.CalculateBoundingBox(this.EntityGOC).center;
            }
        }

        public bool IsMainActor
        {
            get
            {
                return this == Get.MainActor;
            }
        }

        public bool IsNowControlledActor
        {
            get
            {
                return this == Get.NowControlledActor;
            }
        }

        public bool IsPlayerParty
        {
            get
            {
                Actor actor = this as Actor;
                return actor != null && Get.Player.Party.Contains(actor);
            }
        }

        public bool IsNowControlledPlayerParty
        {
            get
            {
                return this.IsNowControlledActor && this.IsPlayerParty;
            }
        }

        public bool EverChangedPositionOrBecameActiveSinceSpawningUnsaved
        {
            get
            {
                return this.everChangedPositionOrBecameActiveSinceSpawningUnsaved;
            }
        }

        public Vector3Int Position
        {
            get
            {
                return this.position;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                if (this.position == value)
                {
                    return;
                }
                bool flag = this.CheckRemoveFromCombinedMeshes();
                Vector3Int vector3Int = this.position;
                this.position = value;
                this.everChangedPositionOrBecameActiveSinceSpawningUnsaved = true;
                if (this.Spawned)
                {
                    try
                    {
                        Profiler.Begin("World.OnEntityMoved()");
                        Get.World.OnEntityMoved(this, vector3Int);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error in World.OnEntityMoved().", ex);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("RetainedRoomInfo.OnEntityMoved()");
                        Get.RetainedRoomInfo.OnEntityMoved(this, vector3Int);
                    }
                    catch (Exception ex2)
                    {
                        Log.Error("Error in RetainedRoomInfo.OnEntityMoved().", ex2);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("Minimap.OnEntityMoved()");
                        Get.Minimap.OnEntityMoved(this, vector3Int);
                    }
                    catch (Exception ex3)
                    {
                        Log.Error("Error in Minimap.OnEntityMoved().", ex3);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    if (this.IsNowControlledActor)
                    {
                        try
                        {
                            Profiler.Begin("Player.OnPlayerMoved()");
                            Get.Player.OnPlayerMoved(vector3Int);
                        }
                        catch (Exception ex4)
                        {
                            Log.Error("Error in Player.OnPlayerMoved().", ex4);
                        }
                        finally
                        {
                            Profiler.End();
                        }
                        try
                        {
                            Profiler.Begin("ShopkeeperGreetingManager.OnPlayerMoved()");
                            Get.ShopkeeperGreetingManager.OnPlayerMoved();
                        }
                        catch (Exception ex5)
                        {
                            Log.Error("Error in ShopkeeperGreetingManager.OnPlayerMoved().", ex5);
                        }
                        finally
                        {
                            Profiler.End();
                        }
                        try
                        {
                            Profiler.Begin("FPPControllerGOC.OnPlayerMoved()");
                            Get.FPPControllerGOC.OnPlayerMoved(vector3Int);
                        }
                        catch (Exception ex6)
                        {
                            Log.Error("Error in FPPControllerGOC.OnPlayerMoved().", ex6);
                        }
                        finally
                        {
                            Profiler.End();
                        }
                    }
                    try
                    {
                        Profiler.Begin("UpdateGameObjectAppearance()");
                        this.UpdateGameObjectAppearance();
                    }
                    catch (Exception ex7)
                    {
                        Log.Error("Error in UpdateGameObjectAppearance().", ex7);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("UpdateGameObjectActiveStatus()");
                        this.UpdateGameObjectActiveStatus();
                    }
                    catch (Exception ex8)
                    {
                        Log.Error("Error in UpdateGameObjectActiveStatus().", ex8);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("EntityGOC.OnEntityChangedPosition()");
                        this.EntityGOC.OnEntityChangedPosition(vector3Int);
                    }
                    catch (Exception ex9)
                    {
                        Log.Error("Error in EntityGOC.OnEntityChangedPosition().", ex9);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    this.CheckReAddToCombinedMeshes(flag);
                    Get.ModsEventsManager.CallEvent(ModEventType.EntityMoved, this);
                }
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                if (this.rotation == value)
                {
                    return;
                }
                bool flag = this.CheckRemoveFromCombinedMeshes();
                this.rotation = value;
                if (this.Spawned)
                {
                    try
                    {
                        Profiler.Begin("UpdateGameObjectAppearance()");
                        this.UpdateGameObjectAppearance();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error in UpdateGameObjectAppearance().", ex);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    this.CheckReAddToCombinedMeshes(flag);
                }
            }
        }

        public Vector3 Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                if (this.scale == value)
                {
                    return;
                }
                bool flag = this.CheckRemoveFromCombinedMeshes();
                this.scale = value;
                if (this.Spawned)
                {
                    try
                    {
                        Profiler.Begin("UpdateGameObjectAppearance()");
                        this.UpdateGameObjectAppearance();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error in UpdateGameObjectAppearance().", ex);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    this.CheckReAddToCombinedMeshes(flag);
                }
            }
        }

        public Vector3Int DirectionCardinal
        {
            get
            {
                Quaternion? quaternion = this.cachedCardinalDirForRotation;
                Quaternion quaternion2 = this.rotation;
                if (quaternion == null || (quaternion != null && quaternion.GetValueOrDefault() != quaternion2))
                {
                    this.cachedCardinalDirForRotation = new Quaternion?(this.rotation);
                    this.cachedCardinalDir = this.rotation.ToCardinalDir();
                }
                return this.cachedCardinalDir;
            }
        }

        public virtual string Label
        {
            get
            {
                return this.Spec.LabelAdjusted;
            }
        }

        public string LabelCap
        {
            get
            {
                return this.Label.CapitalizeFirst();
            }
        }

        public virtual string Description
        {
            get
            {
                return this.Spec.DescriptionAdjusted;
            }
        }

        public virtual GameObject Prefab
        {
            get
            {
                return this.Spec.PrefabAdjusted;
            }
        }

        public virtual Texture2D Icon
        {
            get
            {
                return this.Spec.IconAdjusted;
            }
        }

        public virtual Color IconColor
        {
            get
            {
                return this.Spec.IconColorAdjusted;
            }
        }

        protected Entity()
        {
        }

        public Entity(EntitySpec spec)
        {
            this.spec = spec;
            if (spec.EntityClass != base.GetType())
            {
                string[] array = new string[5];
                array[0] = "Created an Entity with spec ";
                array[1] = ((spec != null) ? spec.ToString() : null);
                array[2] = ", but the created Entity type is ";
                int num = 3;
                Type type = base.GetType();
                array[num] = ((type != null) ? type.ToString() : null);
                array[4] = ".";
                Log.Warning(string.Concat(array), false);
            }
            this.instanceID = Get.UniqueIDsManager.NextEntityID();
            if (Get.World == null)
            {
                this.stableID = Rand.UniqueID(EmptyLists<Entity>.Get());
            }
            else
            {
                this.stableID = Rand.UniqueID(Get.World.AllEntities);
            }
            this.hp = this.Spec.MaxHP;
            this.CreateComps();
        }

        public Entity(string specID, int instanceID, int stableID, Vector3Int pos, Quaternion rot, Vector3 scale)
        {
            this.spec = Get.Specs.Get<EntitySpec>(specID);
            this.instanceID = instanceID;
            this.stableID = stableID;
            this.position = pos;
            this.rotation = rot;
            this.scale = scale;
        }

        public T GetComp<T>() where T : EntityComp
        {
            return this.comps.GetComp<T>();
        }

        public bool HasComp<T>() where T : EntityComp
        {
            return this.comps.HasComp<T>();
        }

        public void Spawn(Vector3Int pos)
        {
            this.Spawn(pos, null, Vector3.one);
        }

        public void Spawn(Vector3Int pos, Vector3Int dir)
        {
            this.Spawn(pos, new Quaternion?(dir.CardinalDirToQuaternion()), Vector3.one);
        }

        public void Spawn(Vector3Int pos, Quaternion? rot, Vector3 scale)
        {
            Get.World.Spawn(this, pos, rot, scale);
        }

        public void DeSpawn(bool fadeOut = false)
        {
            Get.World.DeSpawn(this, fadeOut, false);
        }

        public void OnSpawned(GameObject gameObject, bool canAutoRotate)
        {
            this.spawned = true;
            this.gameObject = gameObject;
            this.cachedEntityGOC = gameObject.GetComponent<EntityGOC>();
            if (this.cachedEntityGOC != null)
            {
                this.cachedEntityGOC.Entity = this;
            }
            else
            {
                Log.Error("Tried to spawn " + ((this != null) ? this.ToString() : null) + " but the game object provided does not have an EntityGOC component.", false);
            }
            gameObject.name = this.Spec.LabelCap;
            this.everChangedPositionOrBecameActiveSinceSpawningUnsaved = false;
            try
            {
                this.OnSpawned(canAutoRotate);
            }
            catch (Exception ex)
            {
                Log.Error("Error in Entity.OnSpawned().", ex);
            }
            List<EntityComp> allComps = this.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                try
                {
                    allComps[i].OnSpawned();
                }
                catch (Exception ex2)
                {
                    Log.Error("Error in EntityComp.OnSpawned().", ex2);
                }
            }
            try
            {
                Profiler.Begin("EntityGOC.OnEntitySpawned()");
                this.cachedEntityGOC.OnEntitySpawned();
            }
            catch (Exception ex3)
            {
                Log.Error("Error in EntityGOC.OnEntitySpawned().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            this.UpdateGameObjectAppearance();
            this.UpdateGameObjectActiveStatus();
        }

        public virtual void OnSpawned(bool canAutoRotate)
        {
        }

        public virtual void OnDeSpawned(bool fadeOut = false)
        {
            this.spawned = false;
            List<EntityComp> allComps = this.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                try
                {
                    allComps[i].OnDeSpawned();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in EntityComp.OnDeSpawned().", ex);
                }
            }
            if (this.gameObject != null)
            {
                if (this.gameObject.activeInHierarchy && fadeOut && this.gameObject.GetComponentInChildren<MeshRenderer>() != null)
                {
                    if (CombinedMeshes.ShouldEntityBeCombined(this.Spec))
                    {
                        Get.SectionsManager.GetSectionRenderer(this.Position).CombinedMeshes.TryRemove(this.GameObject);
                    }
                    Get.GameObjectFader.FadeOut(this.gameObject);
                }
                if (!(this is Item))
                {
                    Get.ParticleSystemFinisher.DetachAndFinishParticles(this.gameObject);
                }
                GameObjectUtility.DestroyImmediate(this.gameObject);
                this.gameObject = null;
            }
            this.cachedEntityGOC = null;
        }

        public void OnFogged()
        {
            this.UpdateGameObjectAppearance();
            this.UpdateGameObjectActiveStatus();
        }

        public void OnUnfogged()
        {
            this.UpdateGameObjectAppearance();
            this.UpdateGameObjectActiveStatus();
        }

        public void OnNewlySeenByPlayer()
        {
            if (!this.Spec.RenderIfPlayerCantSee)
            {
                this.UpdateGameObjectAppearance();
            }
            this.UpdateGameObjectActiveStatus();
        }

        public void OnNoLongerSeenByPlayer()
        {
            if (!this.Spec.RenderIfPlayerCantSee)
            {
                this.UpdateGameObjectAppearance();
            }
            this.UpdateGameObjectActiveStatus();
        }

        protected virtual void OnChangedGameObjectActiveStatus()
        {
            List<EntityComp> allComps = this.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                try
                {
                    allComps[i].OnChangedGameObjectActiveStatus();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in EntityComp.OnChangedGameObjectActiveStatus().", ex);
                }
            }
        }

        private bool CheckRemoveFromCombinedMeshes()
        {
            return this.Spawned && Get.SectionsManager.GetSectionRenderer(this.Position).CombinedMeshes.TryRemove(this.gameObject);
        }

        private void CheckReAddToCombinedMeshes(bool wasCombined)
        {
            if (wasCombined && this.gameObject.activeSelf && !Get.GameObjectFader.IsFading(this.gameObject))
            {
                Get.SectionsManager.GetSectionRenderer(this.Position).CombinedMeshes.Add(this.gameObject, true);
            }
        }

        public virtual void Update()
        {
            try
            {
                this.cachedEntityGOC.OnEntityUpdate();
            }
            catch (Exception ex)
            {
                Log.Error("Error in EntityGOC.OnEntityUpdate().", ex);
            }
            List<EntityComp> allComps = this.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                try
                {
                    allComps[i].Update();
                }
                catch (Exception ex2)
                {
                    Log.Error("Error in EntityComp.Update().", ex2);
                }
            }
        }

        public virtual void FixedUpdate()
        {
            try
            {
                this.cachedEntityGOC.OnEntityFixedUpdate();
            }
            catch (Exception ex)
            {
                Log.Error("Error in EntityGOC.OnEntityFixedUpdate().", ex);
            }
            List<EntityComp> allComps = this.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                try
                {
                    allComps[i].FixedUpdate();
                }
                catch (Exception ex2)
                {
                    Log.Error("Error in EntityComp.FixedUpdate().", ex2);
                }
            }
        }

        protected void UpdateGameObjectActiveStatus()
        {
            if (this.gameObject != null)
            {
                bool flag;
                if (this.IsNowControlledActor)
                {
                    flag = true;
                }
                else if (Get.ScreenFader.ShouldFadeOut(this))
                {
                    flag = false;
                }
                else
                {
                    flag = Get.FogOfWar.IsUnfogged(this.Position);
                    if (flag && !this.Spec.RenderIfPlayerCantSee && !Get.VisibilityCache.PlayerSees(this) && !Get.DeathScreenDrawer.ShouldShow)
                    {
                        flag = false;
                    }
                }
                if (this.gameObject.activeSelf != flag)
                {
                    if (flag)
                    {
                        if (!this.everChangedPositionOrBecameActiveSinceSpawningUnsaved)
                        {
                            this.everChangedPositionOrBecameActiveSinceSpawningUnsaved = true;
                            this.EntityGOC.SetTransformToDesiredInstantly();
                        }
                        Get.GameObjectFader.FadeIn(this.gameObject);
                    }
                    else
                    {
                        if (CombinedMeshes.ShouldEntityBeCombined(this.Spec))
                        {
                            Profiler.Begin("Entity remove from combined meshes");
                            try
                            {
                                Get.SectionsManager.GetSectionRenderer(this.Position).CombinedMeshes.TryRemove(this.GameObject);
                            }
                            finally
                            {
                                Profiler.End();
                            }
                        }
                        Profiler.Begin("Entity FadeOut()");
                        try
                        {
                            Get.GameObjectFader.FadeOut(this.gameObject);
                        }
                        finally
                        {
                            Profiler.End();
                        }
                    }
                    this.gameObject.SetActive(flag);
                    this.OnChangedGameObjectActiveStatus();
                }
            }
        }

        public void UpdateGameObjectAppearance()
        {
            if (this.cachedEntityGOC == null)
            {
                return;
            }
            if (this.cachedEntityGOC.UpdateAppearance() && this.cachedEntityGOC.IsMeshCombined)
            {
                CombinedMeshes combinedMeshes = Get.SectionsManager.GetSectionRenderer(this.Position).CombinedMeshes;
                combinedMeshes.Remove(this.gameObject, true);
                combinedMeshes.Add(this.gameObject, true);
            }
        }

        public void CheckFadeOutOnScreenFadeOut()
        {
            this.UpdateGameObjectActiveStatus();
        }

        public void SetGameObjectAfterLoading(GameObject gameObject)
        {
            this.spawned = true;
            this.gameObject = gameObject;
            this.cachedEntityGOC = gameObject.GetComponent<EntityGOC>();
            if (this.cachedEntityGOC != null)
            {
                this.cachedEntityGOC.Entity = this;
            }
            else
            {
                Log.Error("Tried to set game object after loading for " + ((this != null) ? this.ToString() : null) + " but the game object provided does not have an EntityGOC component.", false);
            }
            gameObject.name = this.Spec.LabelCap;
            List<EntityComp> allComps = this.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                try
                {
                    allComps[i].OnSetGameObjectAfterLoading();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in EntityComp.OnSetGameObjectAfterLoading().", ex);
                }
            }
            try
            {
                Profiler.Begin("EntityGOC.OnSetGameObjectAfterLoading()");
                this.cachedEntityGOC.OnSetGameObjectAfterLoading();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in EntityGOC.OnSetGameObjectAfterLoading().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            this.UpdateGameObjectAppearance();
            this.UpdateGameObjectActiveStatus();
        }

        public override string ToString()
        {
            string text;
            try
            {
                if (this.Spec == null)
                {
                    text = base.GetType().Name + " (null spec)";
                }
                else
                {
                    string[] array = new string[6];
                    array[0] = this.LabelCap;
                    array[1] = " (";
                    array[2] = base.GetType().Name;
                    array[3] = ", ";
                    int num = 4;
                    EntitySpec entitySpec = this.Spec;
                    array[num] = ((entitySpec != null) ? entitySpec.ToString() : null);
                    array[5] = ")";
                    text = string.Concat(array);
                }
            }
            catch (Exception)
            {
                text = "[ToString() exception, " + base.GetType().Name + "]";
            }
            return text;
        }

        private void CreateComps()
        {
            this.comps.Clear();
            List<EntityCompProps> allCompProps = this.Spec.AllCompProps;
            for (int i = 0; i < allCompProps.Count; i++)
            {
                EntityCompProps entityCompProps = allCompProps[i];
                try
                {
                    EntityComp entityComp = (EntityComp)Activator.CreateInstance(entityCompProps.CompClass, new object[] { this });
                    this.comps.Add(entityComp);
                }
                catch (Exception ex)
                {
                    Log.Error(string.Concat(new string[]
                    {
                        "Could not create component of class \"",
                        (entityCompProps != null && entityCompProps.CompClass != null) ? entityCompProps.CompClass.ToString() : "null",
                        "\" for ",
                        (this != null) ? this.ToString() : null,
                        "."
                    }), ex);
                }
            }
        }

        public void OnWorldDiscarded()
        {
            this.spawned = false;
            this.position = Vector3Int.zero;
        }

        [Saved]
        private EntitySpec spec;

        [Saved(-1, false)]
        private int instanceID = -1;

        [Saved(-1, false)]
        private int stableID = -1;

        [Saved]
        private int hp;

        [Saved(Default.New, false)]
        private CompsCollection<EntityComp> comps = new CompsCollection<EntityComp>();

        [Saved]
        private Vector3Int position;

        [Saved(Default.Quaternion_Identity, false)]
        private Quaternion rotation = Quaternion.identity;

        [Saved(Default.Vector3_One, false)]
        private Vector3 scale = Vector3.one;

        [Saved]
        private bool spawned;

        private bool everChangedPositionOrBecameActiveSinceSpawningUnsaved;

        private Quaternion? cachedCardinalDirForRotation;

        private Vector3Int cachedCardinalDir;

        private GameObject gameObject;

        private EntityGOC cachedEntityGOC;
    }
}