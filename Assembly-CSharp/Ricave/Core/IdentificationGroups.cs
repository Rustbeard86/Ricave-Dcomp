using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class IdentificationGroups : ISaveableEventsReceiver
    {
        public void InitRandom()
        {
            this.assignedItemLooks.Clear();
            this.assignedPossibilities.Clear();
            this.itemLookSpecToEntitySpec.Clear();
            List<IdentificationGroupSpec> all = Get.Specs.GetAll<IdentificationGroupSpec>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].ItemLookSpecs.Count < all[i].ItemSpecs.Count)
                {
                    string text = "There are more EntitySpecs than ItemLookSpecs in an IdentificationGroupSpec. Some ItemLookSpecs will be assigned to multiple EntitySpecs which will be confusing (2 different items with the same label and icon). The IdentificationGroupSpec is ";
                    IdentificationGroupSpec identificationGroupSpec = all[i];
                    Log.Warning(text + ((identificationGroupSpec != null) ? identificationGroupSpec.ToString() : null) + ".", false);
                }
                foreach (EntitySpec entitySpec in all[i].ItemSpecs)
                {
                    if (this.assignedItemLooks.ContainsKey(entitySpec))
                    {
                        string text2 = "The same EntitySpec is in two IdentificationGroupSpecs. The EntitySpec is ";
                        EntitySpec entitySpec2 = entitySpec;
                        Log.Error(text2 + ((entitySpec2 != null) ? entitySpec2.ToString() : null) + ".", false);
                    }
                    else if (all[i].ItemLookSpecs.Count == 0)
                    {
                        string text3 = "Can't assign any ItemLookSpec to EntitySpec because the ItemLookSpecs list is empty. The EntitySpec is ";
                        EntitySpec entitySpec3 = entitySpec;
                        Log.Error(text3 + ((entitySpec3 != null) ? entitySpec3.ToString() : null) + ".", false);
                    }
                    else
                    {
                        ItemLookSpec itemLookSpec;
                        ItemLookSpec itemLookSpec2;
                        if (all[i].ItemLookSpecs.Where<ItemLookSpec>((ItemLookSpec x) => !this.itemLookSpecToEntitySpec.ContainsKey(x)).TryGetRandomElement<ItemLookSpec>(out itemLookSpec))
                        {
                            this.assignedItemLooks.Add(entitySpec, itemLookSpec);
                            this.itemLookSpecToEntitySpec.Add(itemLookSpec, entitySpec);
                        }
                        else if (all[i].ItemLookSpecs.TryGetRandomElement<ItemLookSpec>(out itemLookSpec2))
                        {
                            this.assignedItemLooks.Add(entitySpec, itemLookSpec2);
                        }
                        List<EntitySpec> possibilities = new List<EntitySpec>();
                        possibilities.Add(entitySpec);
                        Func<EntitySpec, bool> <> 9__1;
                        for (int j = 0; j < all[i].PossibilitiesCountIfUnidentified - 1; j++)
                        {
                            IEnumerable<EntitySpec> itemSpecs = all[i].ItemSpecs;
                            Func<EntitySpec, bool> func;
                            if ((func = <> 9__1) == null)
                            {
                                func = (<> 9__1 = (EntitySpec x) => !possibilities.Contains(x));
                            }
                            EntitySpec entitySpec4;
                            if (!itemSpecs.Where<EntitySpec>(func).TryGetRandomElement<EntitySpec>(out entitySpec4))
                            {
                                break;
                            }
                            possibilities.Add(entitySpec4);
                        }
                        possibilities.Shuffle<EntitySpec>();
                        this.assignedPossibilities.Add(entitySpec, possibilities);
                    }
                }
            }
        }

        public List<EntitySpec> GetPossibilities(EntitySpec entitySpec)
        {
            if (entitySpec == null)
            {
                return null;
            }
            List<EntitySpec> list;
            if (this.assignedPossibilities.TryGetValue(entitySpec, out list))
            {
                return list;
            }
            return null;
        }

        public ItemLookSpec GetItemLook(EntitySpec entitySpec)
        {
            if (entitySpec == null)
            {
                return null;
            }
            ItemLookSpec itemLookSpec;
            if (this.assignedItemLooks.TryGetValue(entitySpec, out itemLookSpec))
            {
                return itemLookSpec;
            }
            return null;
        }

        public EntitySpec GetItemSpecUsingItemLook(ItemLookSpec itemLook)
        {
            if (itemLook == null)
            {
                return null;
            }
            EntitySpec entitySpec;
            if (this.itemLookSpecToEntitySpec.TryGetValue(itemLook, out entitySpec))
            {
                return entitySpec;
            }
            return null;
        }

        public IdentificationGroupSpec GetIdentificationGroup(EntitySpec entitySpec)
        {
            if (entitySpec == null)
            {
                return null;
            }
            List<IdentificationGroupSpec> all = Get.Specs.GetAll<IdentificationGroupSpec>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].ItemSpecs.Contains(entitySpec))
                {
                    return all[i];
                }
            }
            return null;
        }

        public bool ShouldAddToIdentifiedListOnIdentified(EntitySpec entitySpec)
        {
            return entitySpec != null && !this.IsIdentified(entitySpec) && this.GetIdentificationGroup(entitySpec) != null;
        }

        public bool IsIdentified(EntitySpec entitySpec)
        {
            return entitySpec != null && ((Get.DeathScreenDrawer.ShouldShow && this.GetIdentificationGroup(entitySpec) != null) || (Get.DungeonModifier_ItemsStartIdentified.IsActiveAndAppliesToCurrentRun() && this.GetIdentificationGroup(entitySpec) != null) || this.identifiedHashSet.Contains(entitySpec));
        }

        public void AddToIdentified(EntitySpec spec, int insertAt = -1)
        {
            Instruction.ThrowIfNotExecuting();
            if (spec == null)
            {
                Log.Error("Tried to add null EntitySpec to identified items.", false);
                return;
            }
            if (this.IsIdentified(spec))
            {
                Log.Error("Tried to add the same EntitySpec twice to identified items.", false);
                return;
            }
            if (!this.ShouldAddToIdentifiedListOnIdentified(spec))
            {
                Log.Error("Tried to add EntitySpec to identified items, but it doesn't belong to any identification group.", false);
                return;
            }
            if (insertAt >= 0)
            {
                if (insertAt > this.identified.Count)
                {
                    Log.Error(string.Concat(new string[]
                    {
                        "Tried to insert identified EntitySpec at index ",
                        insertAt.ToString(),
                        ", but identified list count is only ",
                        this.identified.Count.ToString(),
                        "."
                    }), false);
                    return;
                }
                this.identified.Insert(insertAt, spec);
            }
            else
            {
                this.identified.Add(spec);
            }
            this.identifiedHashSet.Add(spec);
        }

        public int RemoveFromIdentified(EntitySpec spec)
        {
            Instruction.ThrowIfNotExecuting();
            if (spec == null)
            {
                Log.Error("Tried to remove null EntitySpec from identified items.", false);
                return -1;
            }
            if (!this.IsIdentified(spec))
            {
                Log.Error("Tried to remove EntitySpec from identified items but it's not here.", false);
                return -1;
            }
            int num = this.identified.IndexOf(spec);
            this.identified.Remove(spec);
            this.identifiedHashSet.Remove(spec);
            return num;
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            this.identifiedHashSet.Clear();
            this.identifiedHashSet.AddRange<EntitySpec>(this.identified);
            foreach (KeyValuePair<EntitySpec, List<EntitySpec>> keyValuePair in this.assignedPossibilities)
            {
                if (keyValuePair.Value.RemoveAll((EntitySpec x) => x == null) != 0)
                {
                    Log.Error("Removed some null EntitySpecs from assignedPossibilities.", false);
                }
            }
            this.itemLookSpecToEntitySpec.Clear();
            foreach (KeyValuePair<EntitySpec, ItemLookSpec> keyValuePair2 in this.assignedItemLooks)
            {
                if (!this.itemLookSpecToEntitySpec.ContainsKey(keyValuePair2.Value))
                {
                    this.itemLookSpecToEntitySpec.Add(keyValuePair2.Value, keyValuePair2.Key);
                }
            }
        }

        [Saved(Default.New, true)]
        private List<EntitySpec> identified = new List<EntitySpec>();

        [Saved(Default.New, true)]
        private Dictionary<EntitySpec, ItemLookSpec> assignedItemLooks = new Dictionary<EntitySpec, ItemLookSpec>();

        [Saved(Default.New, true)]
        private Dictionary<EntitySpec, List<EntitySpec>> assignedPossibilities = new Dictionary<EntitySpec, List<EntitySpec>>();

        private HashSet<EntitySpec> identifiedHashSet = new HashSet<EntitySpec>();

        private Dictionary<ItemLookSpec, EntitySpec> itemLookSpecToEntitySpec = new Dictionary<ItemLookSpec, EntitySpec>();
    }
}