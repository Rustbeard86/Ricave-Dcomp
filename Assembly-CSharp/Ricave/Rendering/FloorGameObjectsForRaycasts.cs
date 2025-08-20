using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class FloorGameObjectsForRaycasts
    {
        public FloorGameObjectsForRaycasts(World world)
        {
            this.world = world;
            Vector3Int size = world.Size;
            this.gameObjects = new GameObject[size.x, size.y, size.z];
        }

        public void Update()
        {
            if (this.resolveEntityChangedAtQueueOnFrame != -1 && Clock.Frame >= this.resolveEntityChangedAtQueueOnFrame)
            {
                Profiler.Begin("Resolve queue for ledge markers");
                try
                {
                    this.resolveEntityChangedAtQueueOnFrame = -1;
                    foreach (Vector3Int vector3Int in this.entityChangedAtQueue)
                    {
                        this.ProcessEntityChangedAt(vector3Int);
                    }
                    this.entityChangedAtQueue.Clear();
                }
                finally
                {
                    Profiler.End();
                }
            }
        }

        public void OnEntityMoved(Entity entity, Vector3Int prev)
        {
            this.entityChangedAtQueue.Add(prev);
            this.entityChangedAtQueue.Add(entity.Position);
            this.resolveEntityChangedAtQueueOnFrame = Clock.Frame + 1;
        }

        public void OnEntitySpawned(Entity entity)
        {
            this.entityChangedAtQueue.Add(entity.Position);
            this.resolveEntityChangedAtQueueOnFrame = Clock.Frame + 1;
        }

        public void OnEntityDeSpawned(Entity entity)
        {
            this.entityChangedAtQueue.Add(entity.Position);
            this.resolveEntityChangedAtQueueOnFrame = Clock.Frame + 1;
        }

        private void ProcessEntityChangedAt(Vector3Int pos)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsAndInside[i];
                if (this.world.InBounds(vector3Int))
                {
                    this.RecalculateAt(vector3Int);
                }
            }
        }

        private void RecalculateAt(Vector3Int pos)
        {
            if (this.ShouldBeLedge(pos))
            {
                if (this.gameObjects[pos.x, pos.y, pos.z] == null)
                {
                    GameObject section = Get.SectionsManager.GetSection(pos);
                    if (this.ledgeGameObjectsPool.Count != 0)
                    {
                        GameObject gameObject = this.ledgeGameObjectsPool[this.ledgeGameObjectsPool.Count - 1];
                        this.gameObjects[pos.x, pos.y, pos.z] = gameObject;
                        this.ledgeGameObjectsPool.RemoveAt(this.ledgeGameObjectsPool.Count - 1);
                        gameObject.transform.SetParent(section.transform);
                        gameObject.SetActive(true);
                    }
                    else
                    {
                        this.gameObjects[pos.x, pos.y, pos.z] = Object.Instantiate<GameObject>(FloorGameObjectsForRaycasts.LedgePrefab, pos, Quaternion.identity, section.transform);
                        this.gameObjects[pos.x, pos.y, pos.z].name = "LedgeMarker";
                    }
                    this.gameObjects[pos.x, pos.y, pos.z].transform.position = pos + new Vector3(0f, -0.49f, 0f);
                    return;
                }
            }
            else if (this.gameObjects[pos.x, pos.y, pos.z] != null)
            {
                GameObject gameObject2 = this.gameObjects[pos.x, pos.y, pos.z];
                gameObject2.SetActive(false);
                this.ledgeGameObjectsPool.Add(gameObject2);
                this.gameObjects[pos.x, pos.y, pos.z] = null;
            }
        }

        private bool ShouldBeLedge(Vector3Int pos)
        {
            CellsInfo cellsInfo = this.world.CellsInfo;
            if (!cellsInfo.CanPassThrough(pos))
            {
                return false;
            }
            Vector3Int cameraGravity = Get.FPPControllerGOC.CameraGravity;
            if (!cellsInfo.IsFallingAt(pos, cameraGravity, false, true, false))
            {
                return false;
            }
            Vector3Int vector3Int = pos + cameraGravity;
            if (this.world.InBounds(vector3Int) && cellsInfo.AnyStairsAt(vector3Int))
            {
                return false;
            }
            for (int i = 0; i < Vector3IntUtility.DirectionsXZ.Length; i++)
            {
                Vector3Int vector3Int2 = pos + Vector3IntUtility.DirectionsXZ[i];
                if (this.world.InBounds(vector3Int2) && cellsInfo.CanPassThroughNoActors(vector3Int2) && !cellsInfo.IsFallingAt(vector3Int2, cameraGravity, false, true, false) && (!cellsInfo.AnyLadderAt(vector3Int2) || cellsInfo.IsFloorUnder(vector3Int2, cameraGravity)))
                {
                    return true;
                }
            }
            return false;
        }

        private World world;

        private GameObject[,,] gameObjects;

        private List<GameObject> ledgeGameObjectsPool = new List<GameObject>(64);

        private HashSet<Vector3Int> entityChangedAtQueue = new HashSet<Vector3Int>();

        private int resolveEntityChangedAtQueueOnFrame = -1;

        private static readonly GameObject LedgePrefab = Assets.Get<GameObject>("Prefabs/Misc/LedgeMarker");

        public const string LedgeName = "LedgeMarker";

        private const float OffsetY = -0.49f;
    }
}