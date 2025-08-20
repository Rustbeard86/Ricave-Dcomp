using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Blueprint : ISaveableEventsReceiver
    {
        public int Width
        {
            get
            {
                return this.cachedWidth;
            }
        }

        public int Height
        {
            get
            {
                return this.mapStrings.Count;
            }
        }

        public int Depth
        {
            get
            {
                return this.cachedDepth;
            }
        }

        protected Blueprint()
        {
        }

        public Blueprint(List<string> mapStrings, Dictionary<char, EntitySpec> mapCharToEntity, Dictionary<char, List<EntitySpec>> mapCharToEntities = null)
        {
            this.mapStrings = mapStrings;
            this.mapCharToEntity = mapCharToEntity;
            this.mapCharToEntities = mapCharToEntities ?? new Dictionary<char, List<EntitySpec>>();
            this.TrimAndInit();
            this.LogConfigErrors();
        }

        private void TrimAndInit()
        {
            for (int i = 0; i < this.mapStrings.Count; i++)
            {
                this.mapStrings[i] = this.mapStrings[i].Trim();
            }
            if (this.mapStrings.Count == 0)
            {
                this.cachedWidth = 0;
                this.cachedDepth = 0;
                return;
            }
            string[] array = this.mapStrings[0].Split('\n', StringSplitOptions.None);
            this.cachedWidth = array[0].Trim().Length;
            this.cachedDepth = array.Length;
        }

        public void LogConfigErrors()
        {
            for (int i = 1; i < this.mapStrings.Count; i++)
            {
                if (this.mapStrings[i].Length != this.mapStrings[0].Length)
                {
                    Log.Error("Blueprint has errors: Different Y level strings have different lengths.", false);
                    break;
                }
            }
            for (int j = 0; j < this.mapStrings.Count; j++)
            {
                for (int k = 0; k < this.mapStrings[j].Length; k++)
                {
                    if (this.mapStrings[j][k] != ' ' && this.mapStrings[j][k] != '.' && this.mapStrings[j][k] != '\t' && this.mapStrings[j][k] != '\r' && this.mapStrings[j][k] != '\n')
                    {
                        char c = this.mapStrings[j][k];
                        if (!this.mapCharToEntity.ContainsKey(c) && !this.mapCharToEntities.ContainsKey(c))
                        {
                            Log.Error(string.Format("Blueprint has errors: Unknown character '{0}' in string.", c), false);
                            break;
                        }
                    }
                }
            }
            for (int l = 0; l < this.mapStrings.Count; l++)
            {
                if (this.mapStrings[l].Split('\n', StringSplitOptions.None).Length != this.cachedDepth)
                {
                    Log.Error("Blueprint has errors: Different Y level strings have different depths (line count).", false);
                    return;
                }
            }
        }

        public void OnLoaded()
        {
            this.TrimAndInit();
            this.LogConfigErrors();
        }

        public void OnSaved()
        {
        }

        private Entity MakeEntityFromSpec(EntitySpec entitySpec)
        {
            if (entitySpec == Get.Entity_Stardust && Get.RunConfig.ProgressDisabled)
            {
                return null;
            }
            if (entitySpec == Get.Entity_Gold)
            {
                return ItemGenerator.Gold(Rand.RangeInclusive(5, 15));
            }
            if (entitySpec == Get.Entity_Stardust)
            {
                return ItemGenerator.Stardust(Rand.RangeInclusive(5, 15));
            }
            return Maker.Make(entitySpec, delegate (Entity x)
            {
                Actor actor = x as Actor;
                if (actor != null)
                {
                    actor.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
                }
                Structure structure = x as Structure;
                if (structure != null && entitySpec == Get.Entity_Grave)
                {
                    RoomFeature_Grave.PopulateGrave(structure);
                }
            }, true, true, true);
        }

        public void Spawn(Vector3Int position)
        {
            if (!Get.World.InBounds(position))
            {
                Log.Error(string.Format("Cannot spawn blueprint at position {0}: Position is out of world bounds.", position), false);
                return;
            }
            CellCuboid cellCuboid = new CellCuboid(position, this.Width, this.Height, this.Depth);
            if (!cellCuboid.InBounds())
            {
                Log.Error(string.Format("Cannot spawn blueprint at position {0}: Blueprint bounds {1} exceed world bounds.", position, cellCuboid), false);
                return;
            }
            List<Entity> list = new List<Entity>();
            for (int i = 0; i < this.mapStrings.Count; i++)
            {
                string[] array = this.mapStrings[i].Split('\n', StringSplitOptions.None);
                for (int j = array.Length - 1; j >= 0; j--)
                {
                    string text = array[j].Trim();
                    for (int k = 0; k < text.Length; k++)
                    {
                        char c = text[k];
                        if (c != ' ' && c != '.' && c != '\t' && c != '\r' && c != '\n')
                        {
                            Vector3Int vector3Int = position + new Vector3Int(k, i, array.Length - j - 1);
                            List<EntitySpec> list2;
                            if (this.mapCharToEntities.TryGetValue(c, out list2))
                            {
                                using (List<EntitySpec>.Enumerator enumerator = list2.GetEnumerator())
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        EntitySpec entitySpec = enumerator.Current;
                                        Entity entity = this.MakeEntityFromSpec(entitySpec);
                                        if (entity != null)
                                        {
                                            entity.Spawn(vector3Int);
                                            list.Add(entity);
                                        }
                                    }
                                    goto IL_0182;
                                }
                            }
                            EntitySpec entitySpec2;
                            if (this.mapCharToEntity.TryGetValue(c, out entitySpec2))
                            {
                                Entity entity2 = this.MakeEntityFromSpec(entitySpec2);
                                if (entity2 != null)
                                {
                                    entity2.Spawn(vector3Int);
                                    list.Add(entity2);
                                }
                            }
                        }
                    IL_0182:;
                    }
                }
            }
            for (int l = 0; l < list.Count; l++)
            {
                Structure structure = list[l] as Structure;
                if (structure != null && !structure.SpawnedWithSpecificRotationUnsaved)
                {
                    structure.AutoRotate();
                }
            }
        }

        public IEnumerable<Instruction> SpawnInstructions(Vector3Int position)
        {
            if (!Get.World.InBounds(position))
            {
                Log.Error(string.Format("Cannot spawn blueprint at position {0}: Position is out of world bounds.", position), false);
                yield break;
            }
            CellCuboid cellCuboid = new CellCuboid(position, this.Width, this.Height, this.Depth);
            if (!cellCuboid.InBounds())
            {
                Log.Error(string.Format("Cannot spawn blueprint at position {0}: Blueprint bounds {1} exceed world bounds.", position, cellCuboid), false);
                yield break;
            }
            List<ValueTuple<Entity, Vector3Int>> entitiesToSpawn = new List<ValueTuple<Entity, Vector3Int>>();
            for (int j = 0; j < this.mapStrings.Count; j++)
            {
                string[] array = this.mapStrings[j].Split('\n', StringSplitOptions.None);
                for (int k = array.Length - 1; k >= 0; k--)
                {
                    string text = array[k].Trim();
                    for (int l = 0; l < text.Length; l++)
                    {
                        char c = text[l];
                        if (c != ' ' && c != '.' && c != '\t' && c != '\r' && c != '\n')
                        {
                            Vector3Int vector3Int = position + new Vector3Int(l, j, array.Length - k - 1);
                            List<EntitySpec> list;
                            if (this.mapCharToEntities.TryGetValue(c, out list))
                            {
                                using (List<EntitySpec>.Enumerator enumerator = list.GetEnumerator())
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        EntitySpec entitySpec = enumerator.Current;
                                        Entity entity = this.MakeEntityFromSpec(entitySpec);
                                        if (entity != null)
                                        {
                                            entitiesToSpawn.Add(new ValueTuple<Entity, Vector3Int>(entity, vector3Int));
                                        }
                                    }
                                    goto IL_01E7;
                                }
                            }
                            EntitySpec entitySpec2;
                            if (this.mapCharToEntity.TryGetValue(c, out entitySpec2))
                            {
                                Entity entity2 = this.MakeEntityFromSpec(entitySpec2);
                                if (entity2 != null)
                                {
                                    entitiesToSpawn.Add(new ValueTuple<Entity, Vector3Int>(entity2, vector3Int));
                                }
                            }
                        }
                    IL_01E7:;
                    }
                }
            }
            int num;
            for (int i = 0; i < entitiesToSpawn.Count; i = num + 1)
            {
                ValueTuple<Entity, Vector3Int> valueTuple = entitiesToSpawn[i];
                Entity item = valueTuple.Item1;
                Vector3Int item2 = valueTuple.Item2;
                Structure structure = item as Structure;
                if (structure != null && structure.Spec.Structure.IsFilled)
                {
                    foreach (Instruction instruction in InstructionSets_Entity.Spawn(item, item2, null))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                }
                num = i;
            }
            for (int i = 0; i < entitiesToSpawn.Count; i = num + 1)
            {
                ValueTuple<Entity, Vector3Int> valueTuple2 = entitiesToSpawn[i];
                Entity item3 = valueTuple2.Item1;
                Vector3Int item4 = valueTuple2.Item2;
                Structure structure2 = item3 as Structure;
                if (structure2 == null || !structure2.Spec.Structure.IsFilled)
                {
                    foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(item3, item4, null))
                    {
                        yield return instruction2;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                }
                num = i;
            }
            yield break;
            yield break;
        }

        [Saved(Default.New, true)]
        private List<string> mapStrings = new List<string>();

        [Saved(Default.New, true)]
        private Dictionary<char, EntitySpec> mapCharToEntity = new Dictionary<char, EntitySpec>();

        [Saved(Default.New, true)]
        private Dictionary<char, List<EntitySpec>> mapCharToEntities = new Dictionary<char, List<EntitySpec>>();

        private int cachedWidth;

        private int cachedDepth;
    }
}