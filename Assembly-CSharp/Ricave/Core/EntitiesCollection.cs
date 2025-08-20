using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class EntitiesCollection : ISaveableEventsReceiver
    {
        public List<Entity> All
        {
            get
            {
                return this.all;
            }
        }

        public List<Entity> AllWantUpdate
        {
            get
            {
                return this.byWhetherWantsUpdate;
            }
        }

        public List<Entity> AllWithSoundLoop
        {
            get
            {
                return this.byWhetherHasSoundLoop;
            }
        }

        protected EntitiesCollection()
        {
        }

        public EntitiesCollection(World world)
        {
            this.world = world;
            Vector3Int size = world.Size;
            this.byPosition = new List<Entity>[size.x, size.y, size.z];
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            Vector3Int size = this.world.Size;
            this.byPosition = new List<Entity>[size.x, size.y, size.z];
            if (this.all.RemoveAll((Entity x) => x == null) != 0)
            {
                Log.Error("Removed some null entities from EntitiesCollection.", false);
            }
            if (this.all.RemoveAll((Entity x) => x.Spec == null) != 0)
            {
                Log.Error("Removed some entities with null spec from EntitiesCollection.", false);
            }
            for (int i = 0; i < this.all.Count; i++)
            {
                try
                {
                    this.AddToCache(this.all[i]);
                }
                catch (Exception ex)
                {
                    Log.Error("Error while adding an entity to cache in EntitiesCollection after loading.", ex);
                }
            }
        }

        public List<Entity> GetWithComp<CompType>()
        {
            List<Entity> list;
            if (this.byComp.TryGetValue(typeof(CompType), out list))
            {
                return list;
            }
            return EmptyLists<Entity>.Get();
        }

        public IEnumerable<Entity> GetWithCompBase<CompType>()
        {
            foreach (KeyValuePair<Type, List<Entity>> keyValuePair in this.byComp)
            {
                if (typeof(CompType).IsAssignableFrom(keyValuePair.Key))
                {
                    foreach (Entity entity in keyValuePair.Value)
                    {
                        yield return entity;
                    }
                    List<Entity>.Enumerator enumerator2 = default(List<Entity>.Enumerator);
                }
            }
            Dictionary<Type, List<Entity>>.Enumerator enumerator = default(Dictionary<Type, List<Entity>>.Enumerator);
            yield break;
            yield break;
        }

        public List<Entity> GetOfSpec(EntitySpec spec)
        {
            List<Entity> list;
            if (spec != null && this.bySpec.TryGetValue(spec, out list))
            {
                return list;
            }
            return EmptyLists<Entity>.Get();
        }

        public List<Entity> GetOfType(Type type)
        {
            EntitiesCollection.EntitiesOfType entitiesOfType;
            if (this.byType.TryGetValue(type, out entitiesOfType))
            {
                return entitiesOfType.entities;
            }
            return EmptyLists<Entity>.Get();
        }

        public List<T> GetOfType<T>()
        {
            EntitiesCollection.EntitiesOfType entitiesOfType;
            if (this.byType.TryGetValue(typeof(T), out entitiesOfType))
            {
                if (entitiesOfType.entitiesListCorrectType == null)
                {
                    entitiesOfType.entitiesListCorrectType = new List<T>();
                    for (int i = 0; i < entitiesOfType.entities.Count; i++)
                    {
                        entitiesOfType.entitiesListCorrectType.Add(entitiesOfType.entities[i]);
                    }
                    this.byType[typeof(T)] = entitiesOfType;
                }
                return (List<T>)entitiesOfType.entitiesListCorrectType;
            }
            return EmptyLists<T>.Get();
        }

        public List<Entity> GetAt(Vector3Int at)
        {
            return this.byPosition[at.x, at.y, at.z] ?? EmptyLists<Entity>.Get();
        }

        public bool Contains(Entity entity)
        {
            return this.byInstance.Contains(entity);
        }

        public bool Contains_ProbablyAtByWhetherWantsUpdate(Entity entity, int probablyAt)
        {
            return (probablyAt < this.byWhetherWantsUpdate.Count && this.byWhetherWantsUpdate[probablyAt] == entity) || this.Contains(entity);
        }

        public void AddEntity(Entity entity)
        {
            if (entity == null)
            {
                Log.Error("Tried to add null entity to EntitiesCollection.", false);
                return;
            }
            if (entity.Spec == null)
            {
                Log.Error("Tried to add entity with null spec to EntitiesCollection.", false);
                return;
            }
            if (this.Contains(entity))
            {
                Log.Error("Tried to add the same entity twice to EntitiesCollection.", false);
                return;
            }
            this.all.AddSorted(entity, EntitiesCollection.ByEntityID);
            this.AddToCache(entity);
        }

        private void AddToCache(Entity entity)
        {
            this.byInstance.Add(entity);
            List<EntityComp> allComps = entity.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                foreach (Type type in allComps[i].GetType().GetTypeAndBaseTypesExceptObject())
                {
                    List<Entity> list;
                    if (this.byComp.TryGetValue(type, out list))
                    {
                        list.AddSorted(entity, EntitiesCollection.ByEntityID);
                    }
                    else
                    {
                        List<Entity> list2 = new List<Entity>();
                        list2.Add(entity);
                        this.byComp.Add(type, list2);
                    }
                }
            }
            Vector3Int position = entity.Position;
            List<Entity> list3 = this.byPosition[position.x, position.y, position.z];
            if (list3 == null)
            {
                list3 = new List<Entity>();
                this.byPosition[position.x, position.y, position.z] = list3;
            }
            list3.AddSorted(entity, EntitiesCollection.ByEntityID);
            foreach (Type type2 in entity.GetType().GetTypeAndBaseTypesExceptObject())
            {
                EntitiesCollection.EntitiesOfType entitiesOfType;
                if (this.byType.TryGetValue(type2, out entitiesOfType))
                {
                    entitiesOfType.entities.AddSorted(entity, EntitiesCollection.ByEntityID);
                    if (entitiesOfType.entitiesListCorrectType != null)
                    {
                        entitiesOfType.entitiesListCorrectType.AddSortedNonGeneric(entity, EntitiesCollection.ByEntityIDNonGeneric);
                    }
                }
                else
                {
                    EntitiesCollection.EntitiesOfType entitiesOfType2 = default(EntitiesCollection.EntitiesOfType);
                    entitiesOfType2.entities = new List<Entity>();
                    entitiesOfType2.entities.Add(entity);
                    this.byType.Add(type2, entitiesOfType2);
                }
            }
            List<Entity> list4;
            if (this.bySpec.TryGetValue(entity.Spec, out list4))
            {
                list4.AddSorted(entity, EntitiesCollection.ByEntityID);
            }
            else
            {
                List<Entity> list5 = new List<Entity>();
                list5.Add(entity);
                this.bySpec.Add(entity.Spec, list5);
            }
            if (entity.Spec.WantsUpdate)
            {
                this.byWhetherWantsUpdate.AddSorted(entity, EntitiesCollection.ByEntityID);
            }
            if (entity.Spec.SoundLoop != null || entity is Actor)
            {
                this.byWhetherHasSoundLoop.AddSorted(entity, EntitiesCollection.ByEntityID);
            }
        }

        public void Remove(Entity entity)
        {
            if (!this.Contains(entity))
            {
                Log.Error("Tried to remove entity but it's not here.", false);
                return;
            }
            this.all.RemoveSorted(entity, EntitiesCollection.ByEntityID);
            this.byInstance.Remove(entity);
            List<EntityComp> allComps = entity.AllComps;
            int i = 0;
            while (i < allComps.Count)
            {
                if (allComps[i] != null)
                {
                    using (List<Type>.Enumerator enumerator = allComps[i].GetType().GetTypeAndBaseTypesExceptObject().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Type type = enumerator.Current;
                            this.byComp[type].RemoveSorted(entity, EntitiesCollection.ByEntityID);
                        }
                        goto IL_00A7;
                    }
                    goto IL_009C;
                }
                goto IL_009C;
            IL_00A7:
                i++;
                continue;
            IL_009C:
                Log.Error("Entity has null comp in EntitiesCollection.Remove().", false);
                goto IL_00A7;
            }
            Vector3Int position = entity.Position;
            this.byPosition[position.x, position.y, position.z].RemoveSorted(entity, EntitiesCollection.ByEntityID);
            foreach (Type type2 in entity.GetType().GetTypeAndBaseTypesExceptObject())
            {
                EntitiesCollection.EntitiesOfType entitiesOfType = this.byType[type2];
                entitiesOfType.entities.RemoveSorted(entity, EntitiesCollection.ByEntityID);
                if (entitiesOfType.entitiesListCorrectType != null)
                {
                    entitiesOfType.entitiesListCorrectType.RemoveSortedNonGeneric(entity, EntitiesCollection.ByEntityIDNonGeneric, EqualityComparer<Entity>.Default);
                }
            }
            if (entity.Spec != null)
            {
                this.bySpec[entity.Spec].RemoveSorted(entity, EntitiesCollection.ByEntityID);
            }
            else
            {
                Log.Error("Entity has null spec in EntitiesCollection.Remove().", false);
            }
            this.byWhetherWantsUpdate.RemoveSorted(entity, EntitiesCollection.ByEntityID);
            this.byWhetherHasSoundLoop.RemoveSorted(entity, EntitiesCollection.ByEntityID);
        }

        public void OnMoved(Entity entity, Vector3Int prev)
        {
            if (!this.byPosition[prev.x, prev.y, prev.z].RemoveSorted(entity, EntitiesCollection.ByEntityID))
            {
                Log.Error("Could not find entity in its previous byPosition list after moving.", false);
            }
            Vector3Int position = entity.Position;
            List<Entity> list = this.byPosition[position.x, position.y, position.z];
            if (list == null)
            {
                list = new List<Entity>();
                this.byPosition[position.x, position.y, position.z] = list;
            }
            list.AddSorted(entity, EntitiesCollection.ByEntityID);
        }

        [Saved]
        private World world;

        [Saved(Default.New, false)]
        private List<Entity> all = new List<Entity>();

        private Dictionary<Type, EntitiesCollection.EntitiesOfType> byType = new Dictionary<Type, EntitiesCollection.EntitiesOfType>(16);

        private Dictionary<Type, List<Entity>> byComp = new Dictionary<Type, List<Entity>>(16);

        private Dictionary<EntitySpec, List<Entity>> bySpec = new Dictionary<EntitySpec, List<Entity>>(200);

        private List<Entity> byWhetherWantsUpdate = new List<Entity>(500);

        private List<Entity> byWhetherHasSoundLoop = new List<Entity>(250);

        private HashSet<Entity> byInstance = new HashSet<Entity>(1000);

        private List<Entity>[,,] byPosition;

        private static readonly IComparer<Entity> ByEntityID = new EntitiesCollection.ByEntityIDComparer();

        private static readonly IComparer ByEntityIDNonGeneric = new EntitiesCollection.ByEntityIDComparer();

        private struct EntitiesOfType
        {
            public List<Entity> entities;

            public IList entitiesListCorrectType;
        }

        private class ByEntityIDComparer : IComparer<Entity>, IComparer
        {
            public int Compare(Entity a, Entity b)
            {
                return a.MyStableHash.CompareTo(b.MyStableHash);
            }

            public int Compare(object a, object b)
            {
                return ((Entity)a).MyStableHash.CompareTo(((Entity)b).MyStableHash);
            }
        }
    }
}