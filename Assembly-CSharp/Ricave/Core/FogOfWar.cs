using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Ricave.Core
{
    public class FogOfWar
    {
        protected FogOfWar()
        {
            this.UnfogCheckWorkerDelegate = new Action<int>(this.UnfogCheckWorker);
        }

        public FogOfWar(World world)
        {
            this.world = world;
            this.UnfogCheckWorkerDelegate = new Action<int>(this.UnfogCheckWorker);
            Vector3Int size = world.Size;
            this.unfogged = new bool[size.x, size.y, size.z];
        }

        public bool IsUnfogged(Vector3Int pos)
        {
            return this.unfogged[pos.x, pos.y, pos.z];
        }

        public bool IsFogged(Vector3Int pos)
        {
            return !this.unfogged[pos.x, pos.y, pos.z];
        }

        public List<Vector3Int> GetCellsToNewlyUnfogAt(Vector3Int playerPos)
        {
            int seeRange = Get.NowControlledActor.SeeRange;
            CellCuboid cellCuboid = playerPos.GetCellsWithin(seeRange).ClipToWorld();
            this.UnfogCheckWorker_width = cellCuboid.width;
            this.UnfogCheckWorker_widthTimesHeight = cellCuboid.width * cellCuboid.height;
            this.UnfogCheckWorker_playerPos = playerPos;
            this.UnfogCheckWorker_offsetX = cellCuboid.x;
            this.UnfogCheckWorker_offsetY = cellCuboid.y;
            this.UnfogCheckWorker_offsetZ = cellCuboid.z;
            this.UnfogCheckWorker_anyToUnfog = false;
            Profiler.Begin("FogOfWar Parallel.For()");
            try
            {
                Parallel.For(0, cellCuboid.Volume, this.UnfogCheckWorkerDelegate);
            }
            finally
            {
                Profiler.End();
            }
            List<Vector3Int> list;
            if (this.UnfogCheckWorker_anyToUnfog)
            {
                list = new List<Vector3Int>();
                int i = 0;
                int count = FogOfWar.allThreadLocalToUnfog.Count;
                while (i < count)
                {
                    List<Vector3Int> list2 = FogOfWar.allThreadLocalToUnfog[i];
                    list.AddRange(list2);
                    list2.Clear();
                    i++;
                }
            }
            else
            {
                list = EmptyLists<Vector3Int>.Get();
            }
            if (FogOfWar.allThreadLocalToUnfog.Count > 200)
            {
                Log.Warning("Too many thread local cells to unfog lists. Clearing to avoid using too much memory.", false);
                FogOfWar.ClearThreadLocalToUnfogLists();
            }
            return list;
        }

        private void UnfogCheckWorker(int index)
        {
            int num = index / this.UnfogCheckWorker_widthTimesHeight;
            index -= num * this.UnfogCheckWorker_widthTimesHeight;
            int num2 = index / this.UnfogCheckWorker_width;
            int num3 = index - num2 * this.UnfogCheckWorker_width + this.UnfogCheckWorker_offsetX;
            num2 += this.UnfogCheckWorker_offsetY;
            num += this.UnfogCheckWorker_offsetZ;
            if (this.unfogged[num3, num2, num])
            {
                return;
            }
            Vector3Int vector3Int = new Vector3Int(num3, num2, num);
            if (LineOfSight.IsLineOfSight(this.UnfogCheckWorker_playerPos, vector3Int))
            {
                FogOfWar.threadLocalToUnfog.Value.Add(vector3Int);
                this.UnfogCheckWorker_anyToUnfog = true;
            }
        }

        public void Unfog(List<Vector3Int> cells)
        {
            Instruction.ThrowIfNotExecuting();
            if (cells.Count == 0)
            {
                return;
            }
            for (int i = 0; i < cells.Count; i++)
            {
                Vector3Int vector3Int = cells[i];
                if (this.unfogged[vector3Int.x, vector3Int.y, vector3Int.z])
                {
                    Log.Error("Tried to unfog a cell which is already unfogged.", false);
                }
                else
                {
                    this.unfogged[vector3Int.x, vector3Int.y, vector3Int.z] = true;
                    this.CheckDoUnfoggedVisualEffect(vector3Int);
                    List<Entity> entitiesAt = this.world.GetEntitiesAt(vector3Int);
                    for (int j = 0; j < entitiesAt.Count; j++)
                    {
                        try
                        {
                            entitiesAt[j].OnUnfogged();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error in Entity.OnUnfogged().", ex);
                        }
                    }
                }
            }
            try
            {
                Profiler.Begin("world.OnFogChanged()");
                Get.World.OnFogChanged(cells);
            }
            catch (Exception ex2)
            {
                Log.Error("Error in world.OnFogChanged().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("VisibilityRenderer.OnFogChanged()");
                Get.VisibilityCache.Renderer.OnFogChanged(cells);
            }
            catch (Exception ex3)
            {
                Log.Error("Error in VisibilityRenderer.OnFogChanged().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("TiledDecals.OnFogChanged()");
                Get.TiledDecals.OnFogChanged(cells);
            }
            catch (Exception ex4)
            {
                Log.Error("Error in TiledDecals.OnFogChanged().", ex4);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("RetainedRoomInfo.OnFogChanged()");
                Get.RetainedRoomInfo.OnFogChanged(cells);
            }
            catch (Exception ex5)
            {
                Log.Error("Error in RetainedRoomInfo.OnFogChanged().", ex5);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("Minimap.OnFogChanged()");
                Get.Minimap.OnFogChanged(cells);
            }
            catch (Exception ex6)
            {
                Log.Error("Error in Minimap.OnFogChanged().", ex6);
            }
            finally
            {
                Profiler.End();
            }
            Get.ModsEventsManager.CallEvent(ModEventType.UnfoggedCells, cells);
        }

        public void SetFogged(List<Vector3Int> cells)
        {
            Instruction.ThrowIfNotExecuting();
            if (cells.Count == 0)
            {
                return;
            }
            for (int i = 0; i < cells.Count; i++)
            {
                Vector3Int vector3Int = cells[i];
                if (!this.unfogged[vector3Int.x, vector3Int.y, vector3Int.z])
                {
                    Log.Error("Tried to set fog in an already fogged cell.", false);
                }
                else
                {
                    this.unfogged[vector3Int.x, vector3Int.y, vector3Int.z] = false;
                    List<Entity> entitiesAt = this.world.GetEntitiesAt(vector3Int);
                    for (int j = 0; j < entitiesAt.Count; j++)
                    {
                        try
                        {
                            entitiesAt[j].OnFogged();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error in Entity.OnFogged().", ex);
                        }
                    }
                }
            }
            try
            {
                Profiler.Begin("world.OnFogChanged()");
                Get.World.OnFogChanged(cells);
            }
            catch (Exception ex2)
            {
                Log.Error("Error in world.OnFogChanged().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("VisibilityRenderer.OnFogChanged()");
                Get.VisibilityCache.Renderer.OnFogChanged(cells);
            }
            catch (Exception ex3)
            {
                Log.Error("Error in VisibilityRenderer.OnFogChanged().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("TiledDecals.OnFogChanged()");
                Get.TiledDecals.OnFogChanged(cells);
            }
            catch (Exception ex4)
            {
                Log.Error("Error in TiledDecals.OnFogChanged().", ex4);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("RetainedRoomInfo.OnFogChanged()");
                Get.RetainedRoomInfo.OnFogChanged(cells);
            }
            catch (Exception ex5)
            {
                Log.Error("Error in RetainedRoomInfo.OnFogChanged().", ex5);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("Minimap.OnFogChanged()");
                Get.Minimap.OnFogChanged(cells);
            }
            catch (Exception ex6)
            {
                Log.Error("Error in Minimap.OnFogChanged().", ex6);
            }
            finally
            {
                Profiler.End();
            }
            Get.ModsEventsManager.CallEvent(ModEventType.FoggedCells, cells);
        }

        public void OnEntityMoved(Entity entity, Vector3Int prev)
        {
            bool flag = this.IsFogged(prev);
            bool flag2 = this.IsFogged(entity.Position);
            if (flag && !flag2)
            {
                try
                {
                    Profiler.Begin("entity.OnUnfogged()");
                    entity.OnUnfogged();
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error("Error in entity.OnUnfogged().", ex);
                    return;
                }
                finally
                {
                    Profiler.End();
                }
            }
            if (!flag && flag2)
            {
                try
                {
                    Profiler.Begin("entity.OnFogged()");
                    entity.OnFogged();
                }
                catch (Exception ex2)
                {
                    Log.Error("Error in entity.OnFogged().", ex2);
                }
                finally
                {
                    Profiler.End();
                }
            }
        }

        private void CheckDoUnfoggedVisualEffect(Vector3Int cell)
        {
            Rand.PushState(Calc.CombineHashes<int, int, int, int>(cell.x, cell.y, cell.z, 608935132));
            if (Rand.Chance(0.02f) && cell.GetGridDistance(Get.NowControlledActor.Position) >= 4 && (Get.CellsInfo.CanPassThrough(cell) || Get.World.AnyEntityOfSpecAt(cell, Get.Entity_CeilingBars) || Get.World.AnyEntityOfSpecAt(cell, Get.Entity_CeilingBarsReinforced) || Get.World.AnyEntityOfSpecAt(cell, Get.Entity_FloorBars) || Get.World.AnyEntityOfSpecAt(cell, Get.Entity_FloorBarsReinforced)) && !Get.CellsInfo.AnyDoorAt(cell) && !Get.CellsInfo.IsFilled(cell))
            {
                Get.VisualEffectsManager.DoOneShot(Get.VisualEffect_Fireflies, cell);
            }
            Rand.PopState();
        }

        public static void ResetStaticDataOnSceneChanged()
        {
            FogOfWar.ClearThreadLocalToUnfogLists();
        }

        private static List<Vector3Int> CreateThreadLocalToUnfogList()
        {
            List<Vector3Int> list = new List<Vector3Int>(20);
            object obj = FogOfWar.allThreadLocalToUnfogLock;
            lock (obj)
            {
                FogOfWar.allThreadLocalToUnfog.Add(list);
            }
            return list;
        }

        private static void ClearThreadLocalToUnfogLists()
        {
            FogOfWar.threadLocalToUnfog.Dispose();
            FogOfWar.allThreadLocalToUnfog.Clear();
            FogOfWar.threadLocalToUnfog = new ThreadLocal<List<Vector3Int>>(new Func<List<Vector3Int>>(FogOfWar.CreateThreadLocalToUnfogList));
        }

        [Saved]
        private World world;

        [Saved]
        private bool[,,] unfogged;

        private Action<int> UnfogCheckWorkerDelegate;

        private int UnfogCheckWorker_width;

        private int UnfogCheckWorker_widthTimesHeight;

        private Vector3Int UnfogCheckWorker_playerPos;

        private int UnfogCheckWorker_offsetX;

        private int UnfogCheckWorker_offsetY;

        private int UnfogCheckWorker_offsetZ;

        private bool UnfogCheckWorker_anyToUnfog;

        private static ThreadLocal<List<Vector3Int>> threadLocalToUnfog = new ThreadLocal<List<Vector3Int>>(new Func<List<Vector3Int>>(FogOfWar.CreateThreadLocalToUnfogList));

        private static List<List<Vector3Int>> allThreadLocalToUnfog = new List<List<Vector3Int>>(30);

        private static object allThreadLocalToUnfogLock = new object();
    }
}