using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class VisibilityCache
    {
        public VisibilityRenderer Renderer
        {
            get
            {
                return this.renderer;
            }
        }

        public List<Vector3Int> CellsSeen_Unordered
        {
            get
            {
                return this.cellsSeen_unordered;
            }
        }

        public List<Entity> EntitiesSeen_Unordered
        {
            get
            {
                return this.entitiesSeen_unordered;
            }
        }

        public List<Actor> ActorsSeen_Unordered
        {
            get
            {
                return this.actorsSeen_unordered;
            }
        }

        public List<Item> ItemsSeen_Unordered
        {
            get
            {
                return this.itemsSeen_unordered;
            }
        }

        public List<Entity> EntitiesSeen_Ordered
        {
            get
            {
                if (this.entitiesSeen_ordered_dirty)
                {
                    this.entitiesSeen_ordered_dirty = false;
                    this.entitiesSeen_ordered.Clear();
                    this.entitiesSeen_ordered.AddRange(this.entitiesSeen_unordered);
                    this.entitiesSeen_ordered.Sort(VisibilityCache.ByEntityID);
                }
                return this.entitiesSeen_ordered;
            }
        }

        public List<Actor> ActorsSeen_Ordered
        {
            get
            {
                if (this.actorsSeen_ordered_dirty)
                {
                    this.actorsSeen_ordered_dirty = false;
                    this.actorsSeen_ordered.Clear();
                    this.actorsSeen_ordered.AddRange(this.actorsSeen_unordered);
                    this.actorsSeen_ordered.Sort(VisibilityCache.ByEntityIDActor);
                }
                return this.actorsSeen_ordered;
            }
        }

        public List<Item> ItemsSeen_Ordered
        {
            get
            {
                if (this.itemsSeen_ordered_dirty)
                {
                    this.itemsSeen_ordered_dirty = false;
                    this.itemsSeen_ordered.Clear();
                    this.itemsSeen_ordered.AddRange(this.itemsSeen_unordered);
                    this.itemsSeen_ordered.Sort(VisibilityCache.ByEntityIDItem);
                }
                return this.itemsSeen_ordered;
            }
        }

        public bool AnyNonNowControlledActorSeen
        {
            get
            {
                for (int i = 0; i < this.actorsSeen_unordered.Count; i++)
                {
                    if (!this.actorsSeen_unordered[i].IsNowControlledActor)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public VisibilityCache(World world)
        {
            this.world = world;
            Vector3Int size = world.Size;
            this.LOSCheckWorkerDelegate = new Action<int>(this.LOSCheckWorker);
            this.seen = new bool[size.x, size.y, size.z];
            this.renderer = new VisibilityRenderer(world, this);
            this.Recalculate(null, null);
        }

        public bool PlayerSees(Vector3Int pos)
        {
            return this.seen[pos.x, pos.y, pos.z];
        }

        public bool PlayerSees(Entity entity)
        {
            return this.entitiesSeenHashSet.Contains(entity);
        }

        public void OnEntityMoved(Entity entity, Vector3Int prev)
        {
            if (entity.IsNowControlledActor)
            {
                this.Recalculate(null, null);
            }
            else
            {
                this.ProcessEntityChangedAt(entity, entity.Position, new Vector3Int?(prev));
            }
            this.renderer.OnEntityMoved(entity, prev);
        }

        public void OnEntitySpawned(Entity entity)
        {
            if (entity.IsNowControlledActor)
            {
                this.Recalculate(null, null);
            }
            else
            {
                this.ProcessEntityChangedAt(entity, entity.Position, null);
            }
            this.renderer.OnEntitySpawned(entity);
        }

        public void OnEntityDeSpawned(Entity entity)
        {
            if (entity.IsNowControlledActor)
            {
                this.Recalculate(null, null);
            }
            else
            {
                this.ProcessEntityChangedAt(entity, entity.Position, null);
            }
            this.renderer.OnEntityDeSpawned(entity);
        }

        public void OnSeeRangeChanged(Actor actor)
        {
            if (actor.IsNowControlledActor)
            {
                this.Recalculate(null, null);
            }
        }

        public void OnSwitchedNowControlledActor()
        {
            this.Recalculate(null, null);
        }

        public void Update()
        {
            this.renderer.Update();
        }

        private bool ShouldRecalculateWhenEntityChangedAt(Vector3Int pos)
        {
            return Get.NowControlledActor != null && Get.NowControlledActor.Spawned && !Get.World.NotifyingUnsavedComponentsOfEntitiesAfterLoad && pos.GetGridDistance(Get.NowControlledActor.Position) <= Get.NowControlledActor.SeeRange;
        }

        private void ProcessEntityChangedAt(Entity entity, Vector3Int pos, Vector3Int? prev = null)
        {
            if (!this.ShouldRecalculateWhenEntityChangedAt(pos) && (prev == null || !this.ShouldRecalculateWhenEntityChangedAt(prev.Value)))
            {
                return;
            }
            if (entity is Structure || !entity.Spec.CanSeeThrough)
            {
                this.Recalculate(null, null);
                return;
            }
            this.Recalculate(new Vector3Int?(pos), prev);
        }

        private void Recalculate(Vector3Int? losOnlyChangedAt1 = null, Vector3Int? losOnlyChangedAt2 = null)
        {
            Profiler.Begin("VisibilityCache.Recalculate()");
            try
            {
                if (losOnlyChangedAt1 != null)
                {
                    this.seen[losOnlyChangedAt1.Value.x, losOnlyChangedAt1.Value.y, losOnlyChangedAt1.Value.z] = false;
                    if (losOnlyChangedAt2 != null)
                    {
                        this.seen[losOnlyChangedAt2.Value.x, losOnlyChangedAt2.Value.y, losOnlyChangedAt2.Value.z] = false;
                    }
                }
                else
                {
                    for (int i = 0; i < this.cellsSeen_unordered.Count; i++)
                    {
                        Vector3Int vector3Int = this.cellsSeen_unordered[i];
                        this.seen[vector3Int.x, vector3Int.y, vector3Int.z] = false;
                    }
                }
                this.tmpPrevCellsSeen.Clear();
                this.tmpPrevCellsSeen.AddRange(this.cellsSeen_unordered);
                this.tmpPrevEntitiesSeen.Clear();
                this.tmpPrevEntitiesSeen.AddRange(this.entitiesSeen_unordered);
                this.cellsSeen_unordered.Clear();
                this.entitiesSeen_unordered.Clear();
                this.entitiesSeenHashSet.Clear();
                this.actorsSeen_unordered.Clear();
                this.itemsSeen_unordered.Clear();
                this.entitiesSeen_ordered_dirty = true;
                this.actorsSeen_ordered_dirty = true;
                this.itemsSeen_ordered_dirty = true;
                if (Get.NowControlledActor != null && Get.NowControlledActor.Spawned && !Get.World.NotifyingUnsavedComponentsOfEntitiesAfterLoad)
                {
                    Vector3Int position = Get.NowControlledActor.Position;
                    int seeRange = Get.NowControlledActor.SeeRange;
                    CellCuboid cellCuboid = position.GetCellsWithin(seeRange).ClipToWorld();
                    if (losOnlyChangedAt1 != null)
                    {
                        using (CellCuboid.Enumerator enumerator = cellCuboid.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Vector3Int vector3Int2 = enumerator.Current;
                                if (vector3Int2 == losOnlyChangedAt1 || vector3Int2 == losOnlyChangedAt2)
                                {
                                    if (LineOfSight.IsLineOfSight(position, vector3Int2))
                                    {
                                        this.cellsSeen_unordered.Add(vector3Int2);
                                    }
                                }
                                else if (this.seen[vector3Int2.x, vector3Int2.y, vector3Int2.z])
                                {
                                    this.cellsSeen_unordered.Add(vector3Int2);
                                }
                            }
                            goto IL_034C;
                        }
                    }
                    this.LOSCheckWorker_width = cellCuboid.width;
                    this.LOSCheckWorker_widthTimesHeight = cellCuboid.width * cellCuboid.height;
                    this.LOSCheckWorker_playerPos = position;
                    this.LOSCheckWorker_offsetX = cellCuboid.x;
                    this.LOSCheckWorker_offsetY = cellCuboid.y;
                    this.LOSCheckWorker_offsetZ = cellCuboid.z;
                    Profiler.Begin("VisibilityCache Parallel.For()");
                    try
                    {
                        Parallel.For(0, cellCuboid.Volume, this.LOSCheckWorkerDelegate);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                    int j = 0;
                    int count = VisibilityCache.allThreadLocalCellsSeen.Count;
                    while (j < count)
                    {
                        List<Vector3Int> list = VisibilityCache.allThreadLocalCellsSeen[j];
                        this.cellsSeen_unordered.AddRange(list);
                        list.Clear();
                        j++;
                    }
                    if (VisibilityCache.allThreadLocalCellsSeen.Count > 200)
                    {
                        Log.Warning("Too many thread local cells seen lists. Clearing to avoid using too much memory.", false);
                        VisibilityCache.ClearThreadLocalCellsSeenLists();
                    }
                IL_034C:
                    Profiler.Begin("Apply VisibilityCache Parallel.For() results");
                    try
                    {
                        for (int k = 0; k < this.cellsSeen_unordered.Count; k++)
                        {
                            Vector3Int vector3Int3 = this.cellsSeen_unordered[k];
                            this.seen[vector3Int3.x, vector3Int3.y, vector3Int3.z] = true;
                            List<Entity> entitiesAt = this.world.GetEntitiesAt(vector3Int3);
                            for (int l = 0; l < entitiesAt.Count; l++)
                            {
                                this.entitiesSeen_unordered.Add(entitiesAt[l]);
                                this.entitiesSeenHashSet.Add(entitiesAt[l]);
                                this.entitiesSeen_ordered_dirty = true;
                                Actor actor = entitiesAt[l] as Actor;
                                if (actor != null)
                                {
                                    this.actorsSeen_unordered.Add(actor);
                                    this.actorsSeen_ordered_dirty = true;
                                }
                                else
                                {
                                    Item item = entitiesAt[l] as Item;
                                    if (item != null)
                                    {
                                        this.itemsSeen_unordered.Add(item);
                                        this.itemsSeen_ordered_dirty = true;
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
                this.NotifyOtherSystems();
                this.tmpPrevCellsSeen.Clear();
                this.tmpPrevEntitiesSeen.Clear();
            }
            finally
            {
                Profiler.End();
            }
        }

        private void LOSCheckWorker(int index)
        {
            int num = index / this.LOSCheckWorker_widthTimesHeight;
            index -= num * this.LOSCheckWorker_widthTimesHeight;
            int num2 = index / this.LOSCheckWorker_width;
            int num3 = index - num2 * this.LOSCheckWorker_width + this.LOSCheckWorker_offsetX;
            num2 += this.LOSCheckWorker_offsetY;
            num += this.LOSCheckWorker_offsetZ;
            Vector3Int vector3Int = new Vector3Int(num3, num2, num);
            if (LineOfSight.IsLineOfSight(this.LOSCheckWorker_playerPos, vector3Int))
            {
                VisibilityCache.threadLocalCellsSeen.Value.Add(vector3Int);
            }
        }

        private void NotifyOtherSystems()
        {
            this.tmpCellsSeenHashSet.Clear();
            this.tmpCellsSeenHashSet.AddRange<Vector3Int>(this.cellsSeen_unordered);
            this.tmpPrevCellsSeenHashSet.Clear();
            this.tmpPrevCellsSeenHashSet.AddRange<Vector3Int>(this.tmpPrevCellsSeen);
            this.tmpPrevEntitiesSeenHashSet.Clear();
            this.tmpPrevEntitiesSeenHashSet.AddRange<Entity>(this.tmpPrevEntitiesSeen);
            this.tmpCellsNoLongerSeen.Clear();
            for (int i = 0; i < this.tmpPrevCellsSeen.Count; i++)
            {
                if (!this.tmpCellsSeenHashSet.Contains(this.tmpPrevCellsSeen[i]))
                {
                    this.tmpCellsNoLongerSeen.Add(this.tmpPrevCellsSeen[i]);
                }
            }
            this.tmpCellsNewlySeen.Clear();
            for (int j = 0; j < this.cellsSeen_unordered.Count; j++)
            {
                if (!this.tmpPrevCellsSeenHashSet.Contains(this.cellsSeen_unordered[j]))
                {
                    this.tmpCellsNewlySeen.Add(this.cellsSeen_unordered[j]);
                }
            }
            this.tmpEntitiesNoLongerSeen.Clear();
            for (int k = 0; k < this.tmpPrevEntitiesSeen.Count; k++)
            {
                if (!this.entitiesSeenHashSet.Contains(this.tmpPrevEntitiesSeen[k]))
                {
                    this.tmpEntitiesNoLongerSeen.Add(this.tmpPrevEntitiesSeen[k]);
                }
            }
            this.tmpEntitiesNewlySeen.Clear();
            for (int l = 0; l < this.entitiesSeen_unordered.Count; l++)
            {
                if (!this.tmpPrevEntitiesSeenHashSet.Contains(this.entitiesSeen_unordered[l]))
                {
                    this.tmpEntitiesNewlySeen.Add(this.entitiesSeen_unordered[l]);
                }
            }
            if (this.tmpCellsNoLongerSeen.Count != 0 || this.tmpCellsNewlySeen.Count != 0)
            {
                try
                {
                    Profiler.Begin("renderer.OnVisibilityChanged()");
                    this.renderer.OnVisibilityChanged(this.tmpCellsNoLongerSeen, this.tmpCellsNewlySeen);
                }
                catch (Exception ex)
                {
                    Log.Error("Error in renderer.OnVisibilityChanged().", ex);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("Minimap.OnVisibilityChanged()");
                    Get.Minimap.OnVisibilityChanged(this.tmpCellsNoLongerSeen, this.tmpCellsNewlySeen);
                }
                catch (Exception ex2)
                {
                    Log.Error("Error in Minimap.OnVisibilityChanged().", ex2);
                }
                finally
                {
                    Profiler.End();
                }
            }
            if (this.tmpEntitiesNoLongerSeen.Count != 0 || this.tmpEntitiesNewlySeen.Count != 0)
            {
                try
                {
                    Profiler.Begin("SeenEntitiesDrawer.OnEntitiesVisibilityChanged()");
                    Get.SeenEntitiesDrawer.OnEntitiesVisibilityChanged(this.tmpEntitiesNoLongerSeen, this.tmpEntitiesNewlySeen);
                }
                catch (Exception ex3)
                {
                    Log.Error("Error in SeenEntitiesDrawer.OnEntitiesVisibilityChanged().", ex3);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("IconOverlayDrawer.OnEntitiesVisibilityChanged()");
                    Get.IconOverlayDrawer.OnEntitiesVisibilityChanged(this.tmpEntitiesNoLongerSeen, this.tmpEntitiesNewlySeen);
                }
                catch (Exception ex4)
                {
                    Log.Error("Error in IconOverlayDrawer.OnEntitiesVisibilityChanged().", ex4);
                }
                finally
                {
                    Profiler.End();
                }
                try
                {
                    Profiler.Begin("StaticTextOverlays.OnEntitiesVisibilityChanged()");
                    Get.StaticTextOverlays.OnEntitiesVisibilityChanged(this.tmpEntitiesNoLongerSeen, this.tmpEntitiesNewlySeen);
                }
                catch (Exception ex5)
                {
                    Log.Error("Error in StaticTextOverlays.OnEntitiesVisibilityChanged().", ex5);
                }
                finally
                {
                    Profiler.End();
                }
                if (this.tmpEntitiesNewlySeen.Count != 0)
                {
                    try
                    {
                        Profiler.Begin("Player.OnEntitiesSeen()");
                        Get.Player.OnEntitiesSeen(this.tmpEntitiesNewlySeen);
                    }
                    catch (Exception ex6)
                    {
                        Log.Error("Error in Player.OnEntitiesSeen().", ex6);
                    }
                    finally
                    {
                        Profiler.End();
                    }
                }
            }
            Profiler.Begin("Entity.OnNoLongerSeenByPlayer()");
            try
            {
                for (int m = 0; m < this.tmpEntitiesNoLongerSeen.Count; m++)
                {
                    try
                    {
                        this.tmpEntitiesNoLongerSeen[m].OnNoLongerSeenByPlayer();
                    }
                    catch (Exception ex7)
                    {
                        Log.Error("Error in Entity.OnNoLongerSeenByPlayer().", ex7);
                    }
                }
            }
            finally
            {
                Profiler.End();
            }
            Profiler.Begin("Entity.OnNewlySeenByPlayer()");
            try
            {
                for (int n = 0; n < this.tmpEntitiesNewlySeen.Count; n++)
                {
                    try
                    {
                        this.tmpEntitiesNewlySeen[n].OnNewlySeenByPlayer();
                    }
                    catch (Exception ex8)
                    {
                        Log.Error("Error in Entity.OnNewlySeenByPlayer().", ex8);
                    }
                }
            }
            finally
            {
                Profiler.End();
            }
            if (this.tmpCellsNoLongerSeen.Count != 0)
            {
                Get.ModsEventsManager.CallEvent(ModEventType.CellsNoLongerSeen, this.tmpCellsNoLongerSeen);
            }
            if (this.tmpCellsNewlySeen.Count != 0)
            {
                Get.ModsEventsManager.CallEvent(ModEventType.CellsNewlySeen, this.tmpCellsNewlySeen);
            }
            if (this.tmpEntitiesNoLongerSeen.Count != 0)
            {
                Get.ModsEventsManager.CallEvent(ModEventType.EntitiesNoLongerSeen, this.tmpEntitiesNoLongerSeen);
            }
            if (this.tmpEntitiesNewlySeen.Count != 0)
            {
                Get.ModsEventsManager.CallEvent(ModEventType.EntitiesNewlySeen, this.tmpEntitiesNewlySeen);
            }
            this.tmpCellsSeenHashSet.Clear();
            this.tmpPrevCellsSeenHashSet.Clear();
            this.tmpPrevEntitiesSeenHashSet.Clear();
            this.tmpCellsNoLongerSeen.Clear();
            this.tmpCellsNewlySeen.Clear();
            this.tmpEntitiesNoLongerSeen.Clear();
            this.tmpEntitiesNewlySeen.Clear();
        }

        public void OnFinishedNotifyingUnsavedComponentsOfEntitiesAfterLoad()
        {
            this.Recalculate(null, null);
        }

        public static void ResetStaticDataOnSceneChanged()
        {
            VisibilityCache.ClearThreadLocalCellsSeenLists();
        }

        private static List<Vector3Int> CreateThreadLocalCellsSeenList()
        {
            List<Vector3Int> list = new List<Vector3Int>(20);
            object obj = VisibilityCache.allThreadLocalCellsSeenLock;
            lock (obj)
            {
                VisibilityCache.allThreadLocalCellsSeen.Add(list);
            }
            return list;
        }

        private static void ClearThreadLocalCellsSeenLists()
        {
            VisibilityCache.threadLocalCellsSeen.Dispose();
            VisibilityCache.allThreadLocalCellsSeen.Clear();
            VisibilityCache.threadLocalCellsSeen = new ThreadLocal<List<Vector3Int>>(new Func<List<Vector3Int>>(VisibilityCache.CreateThreadLocalCellsSeenList));
        }

        private World world;

        private bool[,,] seen;

        private List<Vector3Int> cellsSeen_unordered = new List<Vector3Int>(250);

        private List<Entity> entitiesSeen_unordered = new List<Entity>(120);

        private HashSet<Entity> entitiesSeenHashSet = new HashSet<Entity>(120);

        private List<Actor> actorsSeen_unordered = new List<Actor>(30);

        private List<Item> itemsSeen_unordered = new List<Item>(30);

        private List<Entity> entitiesSeen_ordered = new List<Entity>(120);

        private bool entitiesSeen_ordered_dirty = true;

        private List<Actor> actorsSeen_ordered = new List<Actor>(20);

        private bool actorsSeen_ordered_dirty = true;

        private List<Item> itemsSeen_ordered = new List<Item>(20);

        private bool itemsSeen_ordered_dirty = true;

        private static ThreadLocal<List<Vector3Int>> threadLocalCellsSeen = new ThreadLocal<List<Vector3Int>>(new Func<List<Vector3Int>>(VisibilityCache.CreateThreadLocalCellsSeenList));

        private static List<List<Vector3Int>> allThreadLocalCellsSeen = new List<List<Vector3Int>>(30);

        private static object allThreadLocalCellsSeenLock = new object();

        private VisibilityRenderer renderer;

        private List<Vector3Int> tmpPrevCellsSeen = new List<Vector3Int>(250);

        private List<Entity> tmpPrevEntitiesSeen = new List<Entity>(120);

        private HashSet<Vector3Int> tmpCellsSeenHashSet = new HashSet<Vector3Int>(250);

        private HashSet<Vector3Int> tmpPrevCellsSeenHashSet = new HashSet<Vector3Int>(250);

        private HashSet<Entity> tmpPrevEntitiesSeenHashSet = new HashSet<Entity>(120);

        private List<Vector3Int> tmpCellsNoLongerSeen = new List<Vector3Int>(250);

        private List<Vector3Int> tmpCellsNewlySeen = new List<Vector3Int>(250);

        private List<Entity> tmpEntitiesNoLongerSeen = new List<Entity>(120);

        private List<Entity> tmpEntitiesNewlySeen = new List<Entity>(120);

        private Action<int> LOSCheckWorkerDelegate;

        private int LOSCheckWorker_width;

        private int LOSCheckWorker_widthTimesHeight;

        private Vector3Int LOSCheckWorker_playerPos;

        private int LOSCheckWorker_offsetX;

        private int LOSCheckWorker_offsetY;

        private int LOSCheckWorker_offsetZ;

        private static readonly Comparison<Entity> ByEntityID = (Entity a, Entity b) => a.MyStableHash.CompareTo(b.MyStableHash);

        private static readonly Comparison<Actor> ByEntityIDActor = (Actor a, Actor b) => a.MyStableHash.CompareTo(b.MyStableHash);

        private static readonly Comparison<Item> ByEntityIDItem = (Item a, Item b) => a.MyStableHash.CompareTo(b.MyStableHash);
    }
}