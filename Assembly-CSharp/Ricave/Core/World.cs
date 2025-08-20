using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class World : ISaveableEventsReceiver
    {
        public Vector3Int Size
        {
            get
            {
                return this.size;
            }
        }

        public CellCuboid Bounds
        {
            get
            {
                return new CellCuboid(0, 0, 0, this.size.x, this.size.y, this.size.z);
            }
        }

        public List<Entity> AllEntities
        {
            get
            {
                return this.entities.All;
            }
        }

        public List<Item> Items
        {
            get
            {
                return this.entities.GetOfType<Item>();
            }
        }

        public List<Structure> Structures
        {
            get
            {
                return this.entities.GetOfType<Structure>();
            }
        }

        public List<SequenceableStructure> SequenceableStructures
        {
            get
            {
                return this.entities.GetOfType<SequenceableStructure>();
            }
        }

        public List<Actor> Actors
        {
            get
            {
                return this.entities.GetOfType<Actor>();
            }
        }

        public List<Ethereal> Ethereal
        {
            get
            {
                return this.entities.GetOfType<Ethereal>();
            }
        }

        public List<Entity> AllEntitiesWithSoundLoop
        {
            get
            {
                return this.entities.AllWithSoundLoop;
            }
        }

        public WorldInfo WorldInfo
        {
            get
            {
                return this.worldInfo;
            }
        }

        public WorldConfig Config
        {
            get
            {
                return this.worldInfo.Config;
            }
        }

        public WorldSpec Spec
        {
            get
            {
                return this.Config.Spec;
            }
        }

        public WorldSequenceable WorldSequenceable
        {
            get
            {
                return this.worldSequenceable;
            }
        }

        public FogOfWar FogOfWar
        {
            get
            {
                return this.fogOfWar;
            }
        }

        public RetainedRoomInfo RetainedRoomInfo
        {
            get
            {
                return this.retainedRoomInfo;
            }
        }

        public CellsInfo CellsInfo
        {
            get
            {
                return this.cellsInfo;
            }
        }

        public PathFinder PathFinder
        {
            get
            {
                return this.pathFinder;
            }
        }

        public ShortestPathsCache ShortestPathsCache
        {
            get
            {
                return this.shortestPathsCache;
            }
        }

        public BFSCache BFSCache
        {
            get
            {
                return this.bfsCache;
            }
        }

        public SectionsManager SectionsManager
        {
            get
            {
                return this.sectionsManager;
            }
        }

        public VisibilityCache VisibilityCache
        {
            get
            {
                return this.visibilityCache;
            }
        }

        public VisualEffectsManager VisualEffectsManager
        {
            get
            {
                return this.visualEffectsManager;
            }
        }

        public GameObjectHighlighter GameObjectHighlighter
        {
            get
            {
                return this.gameObjectHighlighter;
            }
        }

        public GameObjectFader GameObjectFader
        {
            get
            {
                return this.gameObjectFader;
            }
        }

        public TiledDecals TiledDecals
        {
            get
            {
                return this.tiledDecals;
            }
        }

        public WorldEventsManager WorldEventsManager
        {
            get
            {
                return this.worldEventsManager;
            }
        }

        public WorldSituationsManager WorldSituationsManager
        {
            get
            {
                return this.worldSituationsManager;
            }
        }

        public ShopkeeperGreetingManager ShopkeeperGreetingManager
        {
            get
            {
                return this.shopkeeperGreetingManager;
            }
        }

        public BurrowManager BurrowManager
        {
            get
            {
                return this.burrowManager;
            }
        }

        public CellHighlighter CellHighlighter
        {
            get
            {
                return this.cellHighlighter;
            }
        }

        public ShatterManager ShatterManager
        {
            get
            {
                return this.shatterManager;
            }
        }

        public VolumeShatterManager VolumeShatterManager
        {
            get
            {
                return this.volumeShatterManager;
            }
        }

        public LightManager LightManager
        {
            get
            {
                return this.lightManager;
            }
        }

        public ParticleSystemFinisher ParticleSystemFinisher
        {
            get
            {
                return this.particleSystemFinisher;
            }
        }

        public WeatherManager WeatherManager
        {
            get
            {
                return this.weatherManager;
            }
        }

        public bool RecreatingGameObjectAfterLoad
        {
            get
            {
                return this.recreatingGameObjectAfterLoad;
            }
        }

        public bool NotifyingUnsavedComponentsOfEntitiesAfterLoad
        {
            get
            {
                return this.notifyingUnsavedComponentsOfEntitiesAfterLoad;
            }
        }

        public void InitAsNew(Vector3Int size, WorldInfo worldInfo)
        {
            this.size = size;
            this.worldInfo = worldInfo;
            this.worldSequenceable = new WorldSequenceable(this);
            this.entities = new EntitiesCollection(this);
            this.cellsInfo = new CellsInfo(this);
            Get.CacheReferences();
            this.pathFinder = new PathFinder(this);
            this.shortestPathsCache = new ShortestPathsCache(this);
            this.bfsCache = new BFSCache(this);
            this.fogOfWar = new FogOfWar(this);
            this.retainedRoomInfo = new RetainedRoomInfo(this);
            this.sectionsManager = new SectionsManager(this);
            this.visibilityCache = new VisibilityCache(this);
            this.floorGameObjectsForRaycasts = new FloorGameObjectsForRaycasts(this);
            this.tiledDecals = new TiledDecals(this);
            this.worldEventsManager = new WorldEventsManager(this);
            this.worldSituationsManager = new WorldSituationsManager(this);
            this.shatterManager = new ShatterManager();
            this.volumeShatterManager = new VolumeShatterManager();
            this.shopkeeperGreetingManager = new ShopkeeperGreetingManager();
            this.burrowManager = new BurrowManager();
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            this.cellsInfo = new CellsInfo(this);
            Get.CacheReferences();
            this.pathFinder = new PathFinder(this);
            this.shortestPathsCache = new ShortestPathsCache(this);
            this.bfsCache = new BFSCache(this);
            this.sectionsManager = new SectionsManager(this);
            this.visibilityCache = new VisibilityCache(this);
            this.floorGameObjectsForRaycasts = new FloorGameObjectsForRaycasts(this);
            this.shatterManager = new ShatterManager();
            this.volumeShatterManager = new VolumeShatterManager();
            this.tiledDecals.CheckAddDataAll();
            foreach (Entity entity in this.entities.All.ToList<Entity>())
            {
                GameObject gameObject = null;
                try
                {
                    GameObject prefab = entity.Prefab;
                    bool activeSelf = prefab.activeSelf;
                    prefab.SetActive(false);
                    try
                    {
                        this.recreatingGameObjectAfterLoad = true;
                        gameObject = Object.Instantiate<GameObject>(prefab, entity.Position, entity.Rotation, Get.SectionsManager.GetSection(entity.Position).transform);
                        entity.SetGameObjectAfterLoading(gameObject);
                    }
                    finally
                    {
                        this.recreatingGameObjectAfterLoad = false;
                        prefab.SetActive(activeSelf);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error while recreating a game object for an entity after loading. Will try to despawn and clean up.", ex);
                    if (gameObject != null)
                    {
                        GameObjectUtility.DestroyImmediate(gameObject);
                    }
                    try
                    {
                        this.DeSpawn(entity, false, true);
                    }
                    catch (Exception ex2)
                    {
                        Log.Error("Error while trying to clean up after failing to spawn an entity after loading.", ex2);
                    }
                }
            }
            this.notifyingUnsavedComponentsOfEntitiesAfterLoad = true;
            this.tiledDecals.TemporarilyIgnoreEntityChanges = true;
            try
            {
                HashSet<Entity> hashSet = new HashSet<Entity>();
                foreach (Entity entity2 in this.entities.All)
                {
                    try
                    {
                        this.NotifyUnsavedComponentsOfEntitySpawned(entity2, true, hashSet);
                    }
                    catch (Exception ex3)
                    {
                        Log.Error("Error in NotifyUnsavedComponentsOfEntitySpawned() after loading.", ex3);
                    }
                }
            }
            finally
            {
                this.notifyingUnsavedComponentsOfEntitiesAfterLoad = false;
                this.tiledDecals.TemporarilyIgnoreEntityChanges = false;
            }
            this.visibilityCache.OnFinishedNotifyingUnsavedComponentsOfEntitiesAfterLoad();
        }

        public void Update()
        {
            try
            {
                Profiler.Begin("UpdateEntities()");
                this.UpdateEntities();
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("floorGameObjectsForRaycasts.Update()");
                this.floorGameObjectsForRaycasts.Update();
            }
            catch (Exception ex)
            {
                Log.Error("Error in floorGameObjectsForRaycasts.Update().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("visibilityCache.Update()");
                this.visibilityCache.Update();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in visibilityCache.Update().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("visualEffectsManager.Update()");
                this.visualEffectsManager.Update();
            }
            catch (Exception ex3)
            {
                Log.Error("Error in visualEffectsManager.Update().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("gameObjectHighlighter.Update()");
                this.gameObjectHighlighter.Update();
            }
            catch (Exception ex4)
            {
                Log.Error("Error in gameObjectHighlighter.Update().", ex4);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("cellHighlighter.Update()");
                this.cellHighlighter.Update();
            }
            catch (Exception ex5)
            {
                Log.Error("Error in cellHighlighter.Update().", ex5);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("gameObjectFader.Update()");
                this.gameObjectFader.Update();
            }
            catch (Exception ex6)
            {
                Log.Error("Error in gameObjectFader.Update().", ex6);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("tiledDecals.Update()");
                this.tiledDecals.Update();
            }
            catch (Exception ex7)
            {
                Log.Error("Error in tiledDecals.Update().", ex7);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("shatterManager.Update()");
                this.shatterManager.Update();
            }
            catch (Exception ex8)
            {
                Log.Error("Error in shatterManager.Update().", ex8);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("volumeShatterManager.Update()");
                this.volumeShatterManager.Update();
            }
            catch (Exception ex9)
            {
                Log.Error("Error in volumeShatterManager.Update().", ex9);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("lightManager.Update()");
                this.lightManager.Update();
            }
            catch (Exception ex10)
            {
                Log.Error("Error in lightManager.Update().", ex10);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("particleSystemFinisher.Update()");
                this.particleSystemFinisher.Update();
            }
            catch (Exception ex11)
            {
                Log.Error("Error in particleSystemFinisher.Update().", ex11);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("weatherManager.Update()");
                this.weatherManager.Update();
            }
            catch (Exception ex12)
            {
                Log.Error("Error in weatherManager.Update().", ex12);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void FixedUpdate()
        {
            try
            {
                Profiler.Begin("FixedUpdateEntities()");
                this.FixedUpdateEntities();
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("visualEffectsManager.FixedUpdate()");
                this.visualEffectsManager.FixedUpdate();
            }
            catch (Exception ex)
            {
                Log.Error("Error in visualEffectsManager.FixedUpdate().", ex);
            }
            finally
            {
                Profiler.End();
            }
        }

        public List<Entity> GetEntitiesAt(Vector3Int pos)
        {
            return this.entities.GetAt(pos);
        }

        public bool AnyEntityAt(Vector3Int pos)
        {
            return this.GetEntitiesAt(pos).Count != 0;
        }

        public bool AnyEntityOfSpecAt(Vector3Int pos, EntitySpec spec)
        {
            return this.GetFirstEntityOfSpecAt(pos, spec) != null;
        }

        public Entity GetFirstEntityOfSpecAt(Vector3Int pos, EntitySpec spec)
        {
            List<Entity> entitiesAt = this.GetEntitiesAt(pos);
            int i = 0;
            int count = entitiesAt.Count;
            while (i < count)
            {
                Entity entity = entitiesAt[i];
                if (entity.Spec == spec)
                {
                    return entity;
                }
                i++;
            }
            return null;
        }

        public bool AnyAdjacentEntityOfSpecAt(Vector3Int pos, EntitySpec spec)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsAndInside[i];
                if (this.InBounds(vector3Int) && this.AnyEntityOfSpecAt(vector3Int, spec))
                {
                    return true;
                }
            }
            return false;
        }

        public bool AnyAdjacentXZEntityOfSpecAt(Vector3Int pos, EntitySpec spec)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsXZAndInside.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsXZAndInside[i];
                if (this.InBounds(vector3Int) && this.AnyEntityOfSpecAt(vector3Int, spec))
                {
                    return true;
                }
            }
            return false;
        }

        public bool AnyCardinalAdjacentEntityOfSpecAt(Vector3Int pos, EntitySpec spec)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsCardinalAndInside.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsCardinalAndInside[i];
                if (this.InBounds(vector3Int) && this.AnyEntityOfSpecAt(vector3Int, spec))
                {
                    return true;
                }
            }
            return false;
        }

        public bool AnyEntityOfSpec(EntitySpec spec)
        {
            return this.GetEntitiesOfSpec(spec).Count != 0;
        }

        public List<Entity> GetEntitiesOfSpec(EntitySpec spec)
        {
            return this.entities.GetOfSpec(spec);
        }

        public List<Entity> GetEntitiesWithComp<CompType>()
        {
            return this.entities.GetWithComp<CompType>();
        }

        public bool AnyEntityNearby(Vector3Int cell, int radius, EntitySpec spec)
        {
            if (radius < 0)
            {
                return false;
            }
            if (radius == 0)
            {
                return this.AnyEntityOfSpecAt(cell, spec);
            }
            CellCuboid cellCuboid = new CellCuboid(cell, radius).ClipToWorld();
            List<Entity> entitiesOfSpec = this.GetEntitiesOfSpec(spec);
            if (entitiesOfSpec.Count > cellCuboid.Volume)
            {
                foreach (Vector3Int vector3Int in cellCuboid)
                {
                    if (this.AnyEntityOfSpecAt(vector3Int, spec))
                    {
                        return true;
                    }
                }
                return false;
            }
            int i = 0;
            int count = entitiesOfSpec.Count;
            while (i < count)
            {
                if (cellCuboid.Contains(entitiesOfSpec[i].Position))
                {
                    return true;
                }
                i++;
            }
            return false;
        }

        public void Spawn(Entity entity, Vector3Int pos, Quaternion? rot, Vector3 scale)
        {
            Instruction.ThrowIfNotExecuting();
            Profiler.Begin("World.Spawn()");
            try
            {
                if (entity == null)
                {
                    Log.Error("Tried to spawn null entity.", false);
                }
                else
                {
                    if (!pos.InBounds())
                    {
                        string text = "Trying to spawn entity out of bounds (";
                        Vector3Int vector3Int = pos;
                        Log.Error(text + vector3Int.ToString() + ").", false);
                    }
                    if (this.entities.Contains(entity))
                    {
                        Log.Error("Tried to spawn already spawned entity: " + entity.ToStringSafe() + ".", false);
                    }
                    else
                    {
                        Item item = entity as Item;
                        if (item != null && item.ParentInventory != null)
                        {
                            Log.Error("Tried to spawn an item which is still in an inventory.", false);
                        }
                        else
                        {
                            GameObject gameObject = null;
                            try
                            {
                                entity.Position = pos;
                                entity.Rotation = rot ?? Quaternion.identity;
                                entity.Scale = scale;
                                GameObject prefab = entity.Prefab;
                                bool activeSelf = prefab.activeSelf;
                                prefab.SetActive(false);
                                try
                                {
                                    gameObject = Object.Instantiate<GameObject>(prefab, entity.Position, entity.Rotation, Get.SectionsManager.GetSection(entity.Position).transform);
                                    this.Spawn(entity, gameObject, rot == null);
                                }
                                finally
                                {
                                    prefab.SetActive(activeSelf);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(string.Concat(new string[]
                                {
                                    "Error while spawning entity ",
                                    entity.ToStringSafe(),
                                    " at ",
                                    pos.ToStringSafe(),
                                    ". Will try to despawn and clean up."
                                }), ex);
                                if (gameObject != null)
                                {
                                    GameObjectUtility.DestroyImmediate(gameObject);
                                }
                                try
                                {
                                    this.DeSpawn(entity, false, false);
                                }
                                catch (Exception ex2)
                                {
                                    Log.Error("Error while trying to clean up after failing to spawn an entity.", ex2);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                Profiler.End();
            }
        }

        private void Spawn(Entity entity, GameObject gameObject, bool canAutoRotate)
        {
            if (this.entities.Contains(entity))
            {
                Log.Error("Tried to spawn already spawned entity: " + entity.ToStringSafe() + ".", false);
                return;
            }
            try
            {
                Profiler.Begin("entities.AddEntity()");
                this.entities.AddEntity(entity);
            }
            catch (Exception ex)
            {
                Log.Error("Error in entities.AddEntity().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("entity.OnSpawned()");
                entity.OnSpawned(gameObject, canAutoRotate);
            }
            catch (Exception ex2)
            {
                Log.Error("Error in entity.OnSpawned().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            this.NotifyUnsavedComponentsOfEntitySpawned(entity, false, null);
            Get.ModsEventsManager.CallEvent(ModEventType.EntitySpawned, entity);
        }

        private void NotifyUnsavedComponentsOfEntitySpawned(Entity entity, bool afterLoading, HashSet<Entity> notifiedEntitiesForGameObjectAppearance = null)
        {
            try
            {
                Profiler.Begin("cellsInfo.OnEntitySpawned()");
                this.cellsInfo.OnEntitySpawned(entity);
            }
            catch (Exception ex)
            {
                Log.Error("Error in cellsInfo.OnEntitySpawned().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("floorGameObjectsForRaycasts.OnEntitySpawned()");
                this.floorGameObjectsForRaycasts.OnEntitySpawned(entity);
            }
            catch (Exception ex2)
            {
                Log.Error("Error in floorGameObjectsForRaycasts.OnEntitySpawned().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("visibilityCache.OnEntitySpawned()");
                this.visibilityCache.OnEntitySpawned(entity);
            }
            catch (Exception ex3)
            {
                Log.Error("Error in visibilityCache.OnEntitySpawned().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("RetainedRoomInfo.OnEntitySpawned()");
                Get.RetainedRoomInfo.OnEntitySpawned(entity);
            }
            catch (Exception ex4)
            {
                Log.Error("Error in RetainedRoomInfo.OnEntitySpawned().", ex4);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("Minimap.OnEntitySpawned()");
                Get.Minimap.OnEntitySpawned(entity);
            }
            catch (Exception ex5)
            {
                Log.Error("Error in Minimap.OnEntitySpawned().", ex5);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("BossHPBar.OnEntitySpawned()");
                Get.BossHPBar.OnEntitySpawned(entity);
            }
            catch (Exception ex6)
            {
                Log.Error("Error in BossHPBar.OnEntitySpawned().", ex6);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("sectionsManager.OnEntitySpawned()");
                this.sectionsManager.OnEntitySpawned(entity);
            }
            catch (Exception ex7)
            {
                Log.Error("Error in sectionsManager.OnEntitySpawned().", ex7);
            }
            finally
            {
                Profiler.End();
            }
            if (entity.IsNowControlledActor)
            {
                try
                {
                    Profiler.Begin("FPPControllerGOC.OnPlayerActorSpawned()");
                    Get.FPPControllerGOC.OnPlayerActorSpawned(afterLoading);
                }
                catch (Exception ex8)
                {
                    Log.Error("Error in FPPControllerGOC.OnPlayerActorSpawned().", ex8);
                }
                finally
                {
                    Profiler.End();
                }
            }
            try
            {
                Profiler.Begin("tiledDecals.OnEntitySpawned()");
                this.tiledDecals.OnEntitySpawned(entity);
            }
            catch (Exception ex9)
            {
                Log.Error("Error in tiledDecals.OnEntitySpawned().", ex9);
            }
            finally
            {
                Profiler.End();
            }
            if (EntityGOC.CanEverAffectAdjacentEntitiesGameObjectAppearance(entity.Spec) && !WorldGen.DoingGenPasses)
            {
                try
                {
                    Profiler.Begin("NotifyAdjacentEntitiesGameObjectAppearance()");
                    this.NotifyAdjacentEntitiesGameObjectAppearance(entity.Position, notifiedEntitiesForGameObjectAppearance);
                }
                catch (Exception ex10)
                {
                    Log.Error("Error in NotifyAdjacentEntitiesGameObjectAppearance().", ex10);
                }
                finally
                {
                    Profiler.End();
                }
            }
        }

        public void DeSpawn(Entity entity, bool fadeOut = false, bool cleaningUpAfterFailedLoad = false)
        {
            if (!cleaningUpAfterFailedLoad)
            {
                Instruction.ThrowIfNotExecuting();
            }
            Profiler.Begin("World.DeSpawn()");
            try
            {
                if (entity == null)
                {
                    Log.Error("Tried to despawn null entity.", false);
                }
                else if (!this.entities.Contains(entity))
                {
                    Log.Error("Tried to despawn not spawned entity: " + entity.ToStringSafe() + ".", false);
                }
                else
                {
                    try
                    {
                        Profiler.Begin("entities.Remove()");
                        this.entities.Remove(entity);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error in entities.Remove().", ex);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("entity.OnDeSpawned()");
                        entity.OnDeSpawned(fadeOut);
                    }
                    catch (Exception ex2)
                    {
                        Log.Error("Error in entity.OnDeSpawned().", ex2);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("cellsInfo.OnEntityDeSpawned()");
                        this.cellsInfo.OnEntityDeSpawned(entity);
                    }
                    catch (Exception ex3)
                    {
                        Log.Error("Error in cellsInfo.OnEntityDeSpawned().", ex3);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("floorGameObjectsForRaycasts.OnEntityDeSpawned()");
                        this.floorGameObjectsForRaycasts.OnEntityDeSpawned(entity);
                    }
                    catch (Exception ex4)
                    {
                        Log.Error("Error in floorGameObjectsForRaycasts.OnEntityDeSpawned().", ex4);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("visibilityCache.OnEntityDeSpawned()");
                        this.visibilityCache.OnEntityDeSpawned(entity);
                    }
                    catch (Exception ex5)
                    {
                        Log.Error("Error in visibilityCache.OnEntityDeSpawned().", ex5);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("Minimap.OnEntityDeSpawned()");
                        Get.Minimap.OnEntityDeSpawned(entity);
                    }
                    catch (Exception ex6)
                    {
                        Log.Error("Error in Minimap.OnEntityDeSpawned().", ex6);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("BossHPBar.OnEntityDeSpawned()");
                        Get.BossHPBar.OnEntityDeSpawned(entity);
                    }
                    catch (Exception ex7)
                    {
                        Log.Error("Error in BossHPBar.OnEntityDeSpawned().", ex7);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("tiledDecals.OnEntityDeSpawned()");
                        this.tiledDecals.OnEntityDeSpawned(entity);
                    }
                    catch (Exception ex8)
                    {
                        Log.Error("Error in tiledDecals.OnEntityDeSpawned().", ex8);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    try
                    {
                        Profiler.Begin("NextTurnsUI.OnEntityDeSpawned()");
                        Get.NextTurnsUI.OnEntityDeSpawned(entity);
                    }
                    catch (Exception ex9)
                    {
                        Log.Error("Error in NextTurnsUI.OnEntityDeSpawned().", ex9);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    if (EntityGOC.CanEverAffectAdjacentEntitiesGameObjectAppearance(entity.Spec))
                    {
                        try
                        {
                            Profiler.Begin("NotifyAdjacentEntitiesGameObjectAppearance()");
                            this.NotifyAdjacentEntitiesGameObjectAppearance(entity.Position, null);
                        }
                        catch (Exception ex10)
                        {
                            Log.Error("Error in NotifyAdjacentEntitiesGameObjectAppearance().", ex10);
                        }
                        finally
                        {
                            Profiler.End();
                        }
                    }
                    GameObject gameObject = entity.GameObject;
                    if (gameObject != null)
                    {
                        GameObjectUtility.DestroyImmediate(gameObject);
                    }
                    Get.ModsEventsManager.CallEvent(ModEventType.EntityDeSpawned, entity);
                }
            }
            finally
            {
                Profiler.End();
            }
        }

        private void UpdateEntities()
        {
            this.tempForUpdate.Clear();
            this.tempForUpdate.AddRange(this.entities.AllWantUpdate);
            int i = 0;
            int count = this.tempForUpdate.Count;
            while (i < count)
            {
                try
                {
                    Entity entity = this.tempForUpdate[i];
                    if (this.entities.Contains_ProbablyAtByWhetherWantsUpdate(entity, i))
                    {
                        entity.Update();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error while updating entity " + this.tempForUpdate[i].ToStringSafe() + ".", ex);
                }
                i++;
            }
        }

        private void FixedUpdateEntities()
        {
            this.tempForUpdate.Clear();
            this.tempForUpdate.AddRange(this.entities.AllWantUpdate);
            int i = 0;
            int count = this.tempForUpdate.Count;
            while (i < count)
            {
                try
                {
                    Entity entity = this.tempForUpdate[i];
                    if (this.entities.Contains_ProbablyAtByWhetherWantsUpdate(entity, i))
                    {
                        entity.FixedUpdate();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error while updating (fixed update) entity " + this.tempForUpdate[i].ToStringSafe() + ".", ex);
                }
                i++;
            }
        }

        public void OnEntityMoved(Entity entity, Vector3Int prev)
        {
            try
            {
                Profiler.Begin("entities.OnMoved()");
                this.entities.OnMoved(entity, prev);
            }
            catch (Exception ex)
            {
                Log.Error("Error in entities.OnMoved().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("cellsInfo.OnEntityMoved()");
                this.cellsInfo.OnEntityMoved(entity, prev);
            }
            catch (Exception ex2)
            {
                Log.Error("Error in cellsInfo.OnEntityMoved().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("floorGameObjectsForRaycasts.OnEntityMoved()");
                this.floorGameObjectsForRaycasts.OnEntityMoved(entity, prev);
            }
            catch (Exception ex3)
            {
                Log.Error("Error in floorGameObjectsForRaycasts.OnEntityMoved().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("fogOfWar.OnEntityMoved()");
                this.fogOfWar.OnEntityMoved(entity, prev);
            }
            catch (Exception ex4)
            {
                Log.Error("Error in fogOfWar.OnEntityMoved().", ex4);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("visibilityCache.OnEntityMoved()");
                this.visibilityCache.OnEntityMoved(entity, prev);
            }
            catch (Exception ex5)
            {
                Log.Error("Error in visibilityCache.OnEntityMoved().", ex5);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("sectionsManager.OnEntityMoved()");
                this.sectionsManager.OnEntityMoved(entity, prev);
            }
            catch (Exception ex6)
            {
                Log.Error("Error in sectionsManager.OnEntityMoved().", ex6);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("tiledDecals.OnEntityMoved()");
                this.tiledDecals.OnEntityMoved(entity, prev);
            }
            catch (Exception ex7)
            {
                Log.Error("Error in tiledDecals.OnEntityMoved().", ex7);
            }
            finally
            {
                Profiler.End();
            }
            if (EntityGOC.CanEverAffectAdjacentEntitiesGameObjectAppearance(entity.Spec))
            {
                try
                {
                    Profiler.Begin("NotifyAdjacentEntitiesGameObjectAppearance()");
                    this.NotifyAdjacentEntitiesGameObjectAppearance(prev, null);
                }
                catch (Exception ex8)
                {
                    Log.Error("Error in NotifyAdjacentEntitiesGameObjectAppearance().", ex8);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("NotifyAdjacentEntitiesGameObjectAppearance()");
                    this.NotifyAdjacentEntitiesGameObjectAppearance(entity.Position, null);
                }
                catch (Exception ex9)
                {
                    Log.Error("Error in NotifyAdjacentEntitiesGameObjectAppearance().", ex9);
                }
                finally
                {
                    Profiler.End();
                }
            }
        }

        public void OnFogChanged(List<Vector3Int> cells)
        {
            HashSet<Entity> hashSet = FrameLocalPool<HashSet<Entity>>.Get();
            for (int i = 0; i < cells.Count; i++)
            {
                this.NotifyAdjacentEntitiesGameObjectAppearance(cells[i], hashSet);
            }
        }

        public bool InBounds(Vector3Int pos)
        {
            return pos.x < this.size.x && pos.y < this.size.y && pos.z < this.size.z;
        }

        public bool InBounds(int x, int y, int z)
        {
            return x < this.size.x && y < this.size.y && z < this.size.z;
        }

        private void NotifyAdjacentEntitiesGameObjectAppearance(Vector3Int pos, HashSet<Entity> notifiedEntities = null)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsCardinalAndInside.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsCardinalAndInside[i];
                if (this.InBounds(vector3Int))
                {
                    List<Entity> entitiesAt = this.GetEntitiesAt(vector3Int);
                    int j = 0;
                    int count = entitiesAt.Count;
                    while (j < count)
                    {
                        if (notifiedEntities == null || notifiedEntities.Add(entitiesAt[j]))
                        {
                            try
                            {
                                entitiesAt[j].UpdateGameObjectAppearance();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Error in UpdateGameObjectAppearance().", ex);
                            }
                        }
                        j++;
                    }
                }
            }
        }

        public bool CanMoveFromTo(Vector3Int from, Vector3Int to, Actor actor)
        {
            World.CanMoveFromToParams canMoveFromToParams = new World.CanMoveFromToParams(actor);
            return this.CanMoveFromTo(from, to, ref canMoveFromToParams);
        }

        public bool CanMoveFromTo(Vector3Int from, Vector3Int to, ref World.CanMoveFromToParams parms)
        {
            return !(from == to) && this.InBounds(to) && from.IsAdjacent(to) && this.cellsInfo.CanPassThrough(to) && this.CanMoveFromTo_AssumeAdjacentAndCanPassThrough(from, to, ref parms);
        }

        public bool CanMoveFromTo_AssumeAdjacentAndCanPassThrough(Vector3Int from, Vector3Int to, ref World.CanMoveFromToParams parms)
        {
            if (!parms.MovingAllowed)
            {
                return false;
            }
            if (parms.ChainPostPos != null && !to.IsAdjacentOrInside(parms.ChainPostPos.Value))
            {
                return false;
            }
            if (!parms.CanJumpOffLedge && this.cellsInfo.IsFallingAt(to, parms.Gravity, parms.CanFly, parms.CanUseLadders, false))
            {
                return false;
            }
            if (!GravityUtility.IsAltitudeEqual(from, to, parms.Gravity) && !this.CanUseStairsFromTo(from, to, parms.Gravity, parms.CanUseLadders))
            {
                if (parms.CanFly)
                {
                    Vector3Int vector3Int;
                    Vector3Int vector3Int2;
                    GravityUtility.GetForwardAndRight(parms.Gravity, out vector3Int, out vector3Int2);
                    Vector3Int vector3Int3 = (from - to) * vector3Int;
                    Vector3Int vector3Int4 = (from - to) * vector3Int2;
                    if (vector3Int3.Sum() != 0 || vector3Int4.Sum() != 0)
                    {
                        return false;
                    }
                }
                else if (!parms.CanUseLadders || !from.IsAdjacentCardinal(to) || (!this.cellsInfo.AnyLadderAt(from) && (!this.cellsInfo.IsLadderUnder(from) || !(to == from + parms.Gravity))) || (!this.cellsInfo.AnyLadderAt(to) && !this.cellsInfo.IsLadderUnder(to, parms.Gravity) && !(to == from + parms.Gravity)))
                {
                    return false;
                }
            }
            return !this.cellsInfo.IsFallingAt(from, parms.Gravity, parms.CanFly, parms.CanUseLadders, false) && (from.y != to.y || from.x == to.x || from.z == to.z || (!this.cellsInfo.BlocksDiagonalMovement(new Vector3Int(from.x, from.y, to.z)) && !this.cellsInfo.BlocksDiagonalMovement(new Vector3Int(to.x, from.y, from.z)))) && (parms.AllowAIAvoids || !this.cellsInfo.AnyAIAvoidsAt(to));
        }

        public bool CanUseStairsFromTo(Vector3Int fromSt, Vector3Int toSt, Vector3Int gravity, bool canUseLadders)
        {
            return this.CanGoUpTheStairsFromTo(fromSt, toSt, gravity, canUseLadders) || this.CanGoUpTheStairsFromTo(toSt, fromSt, gravity, canUseLadders);
        }

        public bool CanGoUpTheStairsFromTo(Vector3Int fromSt, Vector3Int toSt, Vector3Int gravity, bool canUseLadders)
        {
            return this.cellsInfo.AnyStairsAt(fromSt) && gravity.y == -1 && toSt.y == fromSt.y + 1 && (fromSt.x != toSt.x || fromSt.z != toSt.z) && this.cellsInfo.CanPassThrough(fromSt + Vector3Int.up) && !this.cellsInfo.IsFallingAt(toSt, gravity, false, canUseLadders, false) && (fromSt.x == toSt.x || fromSt.z == toSt.z || (!this.cellsInfo.BlocksDiagonalMovement(new Vector3Int(fromSt.x, toSt.y, toSt.z)) && !this.cellsInfo.BlocksDiagonalMovement(new Vector3Int(toSt.x, toSt.y, fromSt.z))));
        }

        public bool CanTouch(Vector3Int from, Vector3Int target, Actor actor)
        {
            return this.CanTouch(from, target, actor.Gravity, actor.CanFly, actor.CanUseLadders);
        }

        public bool CanTouch(Vector3Int from, Vector3Int target, Vector3Int gravity, bool canFly, bool canUseLadders)
        {
            return from.IsAdjacentOrInside(target) && !this.cellsInfo.IsFallingAt(from, gravity, canFly, canUseLadders, false) && (from == target || (LineOfSight.IsLineOfFire(from, target) && LineOfSight.IsLineOfSight(from, target)));
        }

        public void Discard()
        {
            Get.TurnManager.OnWorldDiscarded();
            foreach (Entity entity in this.entities.All)
            {
                try
                {
                    entity.OnWorldDiscarded();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in entity.OnWorldDiscarded().", ex);
                }
            }
            foreach (Actor actor in Get.PlayerParty.ToTemporaryList<Actor>())
            {
                actor.OnWorldDiscarded();
            }
            Get.ModsEventsManager.CallEvent(ModEventType.WorldDiscarded, this);
        }

        public T GetOrCreateModState<T>(string modId) where T : ModStatePerWorld, new()
        {
            for (int i = 0; i < this.modsState.Count; i++)
            {
                if (this.modsState[i].ModId == modId)
                {
                    T t = this.modsState[i] as T;
                    if (t != null)
                    {
                        return t;
                    }
                }
            }
            T t2 = new T();
            t2.ModId = modId;
            t2.World = this;
            this.modsState.Add(t2);
            return t2;
        }

        [Saved]
        private Vector3Int size;

        [Saved]
        private WorldSequenceable worldSequenceable;

        [Saved]
        private EntitiesCollection entities;

        [Saved]
        private FogOfWar fogOfWar;

        [Saved]
        private RetainedRoomInfo retainedRoomInfo;

        [Saved]
        private WorldInfo worldInfo;

        [Saved]
        private TiledDecals tiledDecals;

        [Saved]
        private WorldEventsManager worldEventsManager;

        [Saved]
        private WorldSituationsManager worldSituationsManager;

        [Saved]
        private ShopkeeperGreetingManager shopkeeperGreetingManager;

        [Saved]
        private BurrowManager burrowManager;

        [Saved(Default.New, true)]
        private List<ModStatePerWorld> modsState = new List<ModStatePerWorld>();

        private CellsInfo cellsInfo;

        private PathFinder pathFinder;

        private ShortestPathsCache shortestPathsCache;

        private BFSCache bfsCache;

        private FloorGameObjectsForRaycasts floorGameObjectsForRaycasts;

        private SectionsManager sectionsManager;

        private VisibilityCache visibilityCache;

        private VisualEffectsManager visualEffectsManager = new VisualEffectsManager();

        private GameObjectHighlighter gameObjectHighlighter = new GameObjectHighlighter();

        private GameObjectFader gameObjectFader = new GameObjectFader();

        private CellHighlighter cellHighlighter = new CellHighlighter();

        private ShatterManager shatterManager;

        private VolumeShatterManager volumeShatterManager;

        private LightManager lightManager = new LightManager();

        private ParticleSystemFinisher particleSystemFinisher = new ParticleSystemFinisher();

        private WeatherManager weatherManager = new WeatherManager();

        private bool recreatingGameObjectAfterLoad;

        private bool notifyingUnsavedComponentsOfEntitiesAfterLoad;

        private List<Entity> tempForUpdate = new List<Entity>();

        public struct CanMoveFromToParams
        {
            public CanMoveFromToParams(Vector3Int gravity, bool canFly, bool canJumpOffLedge, bool canUseLadders, bool movingAllowed, bool allowAIAvoids, Vector3Int? chainPostPos)
            {
                this.Gravity = gravity;
                this.CanFly = canFly;
                this.CanJumpOffLedge = canJumpOffLedge;
                this.CanUseLadders = canUseLadders;
                this.MovingAllowed = movingAllowed;
                this.AllowAIAvoids = allowAIAvoids;
                this.ChainPostPos = chainPostPos;
            }

            public CanMoveFromToParams(Actor actor)
            {
                this.Gravity = actor.Gravity;
                this.CanFly = actor.CanFly;
                this.CanJumpOffLedge = actor.CanJumpOffLedge;
                this.CanUseLadders = actor.CanUseLadders;
                this.MovingAllowed = actor.MovingAllowed;
                this.AllowAIAvoids = actor.AllowPathingIntoAIAvoids;
                ChainPost attachedToChainPost = actor.AttachedToChainPost;
                this.ChainPostPos = ((attachedToChainPost != null) ? new Vector3Int?(attachedToChainPost.Position) : null);
            }

            public readonly Vector3Int Gravity;

            public readonly bool CanFly;

            public readonly bool CanJumpOffLedge;

            public readonly bool CanUseLadders;

            public readonly bool MovingAllowed;

            public readonly bool AllowAIAvoids;

            public readonly Vector3Int? ChainPostPos;
        }
    }
}