using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class PrivateRoom : ISaveableEventsReceiver
    {
        public List<PrivateRoom.PlacedStructure> PlacedStructures
        {
            get
            {
                return this.placedStructures;
            }
        }

        public List<ValueTuple<EntitySpec, int>> StructuresInInventoryInDisplayOrder
        {
            get
            {
                this.tmpStructuresInInventoryInDisplayOrder.Clear();
                foreach (KeyValuePair<EntitySpec, int> keyValuePair in this.structuresInInventory)
                {
                    if (keyValuePair.Value > 0)
                    {
                        this.tmpStructuresInInventoryInDisplayOrder.Add(new ValueTuple<EntitySpec, int>(keyValuePair.Key, keyValuePair.Value));
                    }
                }
                this.tmpStructuresInInventoryInDisplayOrder.Sort(PrivateRoom.ByDisplayOrder);
                return this.tmpStructuresInInventoryInDisplayOrder;
            }
        }

        public void ChangeInventoryCount(EntitySpec structureSpec, int offset)
        {
            if (structureSpec == null)
            {
                Log.Error("Tried to offset null structure spec count in private room inventory.", false);
                return;
            }
            this.structuresInInventory.SetOrIncrement(structureSpec, offset);
        }

        public int GetInventoryCount(EntitySpec structureSpec)
        {
            if (structureSpec == null)
            {
                return 0;
            }
            return this.structuresInInventory.GetOrDefault(structureSpec, 0);
        }

        public int GetPlacedCount(EntitySpec structureSpec)
        {
            if (structureSpec == null)
            {
                return 0;
            }
            int num = 0;
            using (List<PrivateRoom.PlacedStructure>.Enumerator enumerator = this.placedStructures.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.StructureSpec == structureSpec)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public int GetPlacedAndInventoryCount(EntitySpec structureSpec)
        {
            return this.GetPlacedCount(structureSpec) + this.GetInventoryCount(structureSpec);
        }

        public void OnPlaced(EntitySpec structureSpec, Vector3Int at, Structure spawnedStructure)
        {
            if (structureSpec == null)
            {
                Log.Error("Called OnPlaced() with null structure spec.", false);
                return;
            }
            if (this.GetInventoryCount(structureSpec) <= 0)
            {
                Log.Error("Called OnPlaced() but this structure spec is not in the inventory.", false);
                return;
            }
            this.ChangeInventoryCount(structureSpec, -1);
            this.placedStructures.Add(new PrivateRoom.PlacedStructure(structureSpec, at, spawnedStructure));
        }

        public void OnMovedToInventory(Structure structure)
        {
            if (structure == null)
            {
                Log.Error("Called OnMovedToInventory() with null structure.", false);
                return;
            }
            PrivateRoom.PlacedStructure placedStructure = this.FindPlacedStructureFor(structure);
            if (placedStructure == null)
            {
                Log.Error("Called OnMovedToInventory() but couldn't find this structure in placed structures.", false);
                return;
            }
            this.OnMovedToInventory(placedStructure);
        }

        private void OnMovedToInventory(PrivateRoom.PlacedStructure placedStructure)
        {
            if (placedStructure == null)
            {
                Log.Error("Called OnMovedToInventory() with null placedStructure.", false);
                return;
            }
            if (!this.placedStructures.Contains(placedStructure))
            {
                Log.Error("Called OnMovedToInventory() but this placed structure is not here.", false);
                return;
            }
            this.placedStructures.Remove(placedStructure);
            this.ChangeInventoryCount(placedStructure.StructureSpec, 1);
        }

        public bool IsPlacedStructure(Structure structure)
        {
            return structure != null && this.FindPlacedStructureFor(structure) != null;
        }

        private PrivateRoom.PlacedStructure FindPlacedStructureFor(Structure structure)
        {
            foreach (PrivateRoom.PlacedStructure placedStructure in this.placedStructures)
            {
                if (placedStructure.SpawnedStructure == structure)
                {
                    return placedStructure;
                }
            }
            return null;
        }

        public void Apply(ThisRunPrivateRoomStructures thisRunPrivateRoomStructures)
        {
            foreach (KeyValuePair<EntitySpec, int> keyValuePair in thisRunPrivateRoomStructures.CollectedStructures)
            {
                this.ChangeInventoryCount(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public IEnumerable<Instruction> RevalidateAllPlacedStructuresAndSyncWithWorld()
        {
            int num;
            for (int i = 0; i < 10000; i = num + 1)
            {
                bool flag = false;
                using (List<PrivateRoom.PlacedStructure>.Enumerator enumerator = this.PlacedStructures.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        PrivateRoom.<> c__DisplayClass19_0 CS$<> 8__locals1 = new PrivateRoom.<> c__DisplayClass19_0();
                        CS$<> 8__locals1.<> 4__this = this;
                        CS$<> 8__locals1.structure = enumerator.Current;
                        PrivateRoom.<> c__DisplayClass19_1 CS$<> 8__locals2 = new PrivateRoom.<> c__DisplayClass19_1();
                        CS$<> 8__locals2.CS$<> 8__locals1 = CS$<> 8__locals1;
                        CS$<> 8__locals2.spawned = CS$<> 8__locals2.CS$<> 8__locals1.structure.SpawnedStructure;
                        if (CS$<> 8__locals2.spawned == null)
						{
                yield return new Instruction_Immediate(delegate
                {
                    CS$<> 8__locals2.CS$<> 8__locals1.<> 4__this.OnMovedToInventory(CS$<> 8__locals2.CS$<> 8__locals1.structure);
                });
                flag = true;
                break;
            }
            if (!CS$<> 8__locals2.spawned.Spawned || CS$<> 8__locals2.spawned.Position != CS$<> 8__locals2.CS$<> 8__locals1.structure.Position || !ItemOrStructureFallUtility.HasSupport(CS$<> 8__locals2.spawned))
						{
                yield return new Instruction_Immediate(delegate
                {
                    CS$<> 8__locals2.CS$<> 8__locals1.<> 4__this.OnMovedToInventory(CS$<> 8__locals2.spawned);
                });
                foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(CS$<> 8__locals2.spawned, false))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator2 = null;
                flag = true;
                break;
            }
            CS$<> 8__locals2 = null;
        }
				}
				List<PrivateRoom.PlacedStructure>.Enumerator enumerator = default(List<PrivateRoom.PlacedStructure>.Enumerator);
				if (!flag)
				{
					break;
				}
if (i == 9999)
{
    Log.Error("Hit iterations limit in RevalidateAllPlacedStructuresAndSyncWithWorld().", false);
}
num = i;
			}
			yield break;
yield break;
		}

		public void OnSaved()
{
}

public void OnLoaded()
{
    if (this.placedStructures.RemoveAll((PrivateRoom.PlacedStructure x) => x.StructureSpec == null) != 0)
    {
        Log.Error("Removed some private room placed structures with null spec.", false);
    }
}

[Saved(Default.New, false)]
private Dictionary<EntitySpec, int> structuresInInventory = new Dictionary<EntitySpec, int>();

[Saved(Default.New, true)]
private List<PrivateRoom.PlacedStructure> placedStructures = new List<PrivateRoom.PlacedStructure>();

private List<ValueTuple<EntitySpec, int>> tmpStructuresInInventoryInDisplayOrder = new List<ValueTuple<EntitySpec, int>>();

private static readonly Comparison<ValueTuple<EntitySpec, int>> ByDisplayOrder = (ValueTuple<EntitySpec, int> a, ValueTuple<EntitySpec, int> b) => a.Item1.Label.CompareTo(b.Item1.Label);

public class PlacedStructure
{
    public EntitySpec StructureSpec
    {
        get
        {
            return this.structureSpec;
        }
    }

    public Vector3Int Position
    {
        get
        {
            return this.position;
        }
    }

    public Structure SpawnedStructure
    {
        get
        {
            return this.spawnedStructure;
        }
    }

    protected PlacedStructure()
    {
    }

    public PlacedStructure(EntitySpec structureSpec, Vector3Int position, Structure spawnedStructure)
    {
        this.structureSpec = structureSpec;
        this.position = position;
        this.spawnedStructure = spawnedStructure;
    }

    public void OnSpawned(Structure spawnedStructure)
    {
        this.spawnedStructure = spawnedStructure;
    }

    [Saved]
    private EntitySpec structureSpec;

    [Saved]
    private Vector3Int position;

    private Structure spawnedStructure;
}
	}
}