using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class SectionsManager
    {
        public SectionsManager(World world)
        {
            this.world = world;
            this.CreateSections();
        }

        public GameObject GetSection(Vector3Int pos)
        {
            Vector2Int sectionIndex = this.GetSectionIndex(pos);
            if (sectionIndex.x < 0 || sectionIndex.y < 0 || sectionIndex.x >= this.sections.GetLength(0) || sectionIndex.y >= this.sections.GetLength(1))
            {
                string text = "Tried to get a section for a position out of bounds: ";
                Vector3Int vector3Int = pos;
                Log.Error(text + vector3Int.ToString() + ".", false);
                return null;
            }
            return this.sections[sectionIndex.x, sectionIndex.y];
        }

        public SectionRendererGOC GetSectionRenderer(Vector3Int pos)
        {
            Vector2Int sectionIndex = this.GetSectionIndex(pos);
            if (sectionIndex.x < 0 || sectionIndex.y < 0 || sectionIndex.x >= this.sections.GetLength(0) || sectionIndex.y >= this.sections.GetLength(1))
            {
                string text = "Tried to get a section renderer for a position out of bounds: ";
                Vector3Int vector3Int = pos;
                Log.Error(text + vector3Int.ToString() + ".", false);
                return null;
            }
            return this.sectionRenderers[sectionIndex.x, sectionIndex.y];
        }

        public void OnEntitySpawned(Entity entity)
        {
            entity.GameObject.transform.parent = this.GetSection(entity.Position).transform;
            if (entity.IsNowControlledActor)
            {
                this.UpdateSectionsActiveStatus();
            }
        }

        public void OnEntityMoved(Entity entity, Vector3Int prev)
        {
            entity.GameObject.transform.parent = this.GetSection(entity.Position).transform;
            if (entity.IsNowControlledActor)
            {
                this.UpdateSectionsActiveStatus();
            }
        }

        public void OnSwitchedNowControlledActor()
        {
            this.UpdateSectionsActiveStatus();
        }

        private void CreateSections()
        {
            Vector3Int size = this.world.Size;
            this.sections = new GameObject[this.GetSectionsCount(size.x), this.GetSectionsCount(size.z)];
            this.sectionRenderers = new SectionRendererGOC[this.sections.GetLength(0), this.sections.GetLength(1)];
            this.active = new bool[this.sections.GetLength(0), this.sections.GetLength(1)];
            for (int i = 0; i < this.sections.GetLength(0); i++)
            {
                for (int j = 0; j < this.sections.GetLength(1); j++)
                {
                    GameObject gameObject = this.CreateSectionGameObject(i, j);
                    this.sections[i, j] = gameObject;
                    this.sectionRenderers[i, j] = gameObject.GetComponent<SectionRendererGOC>();
                }
            }
        }

        private void UpdateSectionsActiveStatus()
        {
            if (Get.NowControlledActor == null || !Get.NowControlledActor.Spawned)
            {
                return;
            }
            Vector3Int position = Get.NowControlledActor.Position;
            Vector2Int sectionIndex = this.GetSectionIndex(position);
            Vector2Int vector2Int = sectionIndex;
            Vector2Int vector2Int2 = sectionIndex;
            for (int i = sectionIndex.x - 1; i >= 0; i--)
            {
                int maxCellPosInSection = this.GetMaxCellPosInSection(i);
                if (position.x - maxCellPosInSection > 20)
                {
                    break;
                }
                vector2Int.x = i;
            }
            int length = this.sections.GetLength(0);
            int num = sectionIndex.x + 1;
            while (num < length && this.GetMinCellPosInSection(num) - position.x <= 20)
            {
                vector2Int2.x = num;
                num++;
            }
            for (int j = sectionIndex.y - 1; j >= 0; j--)
            {
                int maxCellPosInSection2 = this.GetMaxCellPosInSection(j);
                if (position.z - maxCellPosInSection2 > 20)
                {
                    break;
                }
                vector2Int.y = j;
            }
            int length2 = this.sections.GetLength(1);
            int num2 = sectionIndex.y + 1;
            while (num2 < length2 && this.GetMinCellPosInSection(num2) - position.z <= 20)
            {
                vector2Int2.y = num2;
                num2++;
            }
            for (int k = 0; k < this.activeSections.Count; k++)
            {
                this.active[this.activeSections[k].x, this.activeSections[k].y] = false;
            }
            this.tmpNewActiveSections.Clear();
            for (int l = vector2Int.x; l <= vector2Int2.x; l++)
            {
                for (int m = vector2Int.y; m <= vector2Int2.y; m++)
                {
                    this.sections[l, m].SetActive(true);
                    this.active[l, m] = true;
                    this.tmpNewActiveSections.Add(new Vector2Int(l, m));
                }
            }
            for (int n = 0; n < this.activeSections.Count; n++)
            {
                if (!this.active[this.activeSections[n].x, this.activeSections[n].y])
                {
                    this.sections[this.activeSections[n].x, this.activeSections[n].y].SetActive(false);
                }
            }
            this.activeSections.Clear();
            this.activeSections.AddRange(this.tmpNewActiveSections);
        }

        private Vector2Int GetSectionIndex(Vector3Int pos)
        {
            return new Vector2Int(pos.x / 5, pos.z / 5);
        }

        private int GetSectionsCount(int size)
        {
            return (size - 1) / 5 + 1;
        }

        private int GetMinCellPosInSection(int index)
        {
            return index * 5;
        }

        private int GetMaxCellPosInSection(int index)
        {
            return (index + 1) * 5 - 1;
        }

        private GameObject CreateSectionGameObject(int x, int z)
        {
            GameObject gameObject = new GameObject("Section " + x.ToString() + " " + z.ToString());
            gameObject.transform.parent = Get.RuntimeSectionsContainer.transform;
            gameObject.isStatic = true;
            gameObject.SetActive(false);
            gameObject.AddComponent(typeof(SectionRendererGOC));
            return gameObject;
        }

        private World world;

        private GameObject[,] sections;

        private SectionRendererGOC[,] sectionRenderers;

        private List<Vector2Int> activeSections = new List<Vector2Int>();

        private List<Vector2Int> tmpNewActiveSections = new List<Vector2Int>();

        private bool[,] active;

        private const int SectionSize = 5;

        private const int DrawDistance = 20;
    }
}