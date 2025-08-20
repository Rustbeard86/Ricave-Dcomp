using System;
using System.Collections.Generic;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class CellHighlighter
    {
        public void HighlightCell(Vector3Int pos, Color color)
        {
            if (DebugUI.HideUI)
            {
                return;
            }
            bool flag = false;
            for (int i = 0; i < this.cellHighlights.Count; i++)
            {
                if (this.cellHighlights[i].pos == pos && this.cellHighlights[i].color == color)
                {
                    CellHighlighter.CellHighlight cellHighlight = this.cellHighlights[i];
                    cellHighlight.frame = Clock.Frame;
                    this.cellHighlights[i] = cellHighlight;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                this.cellHighlights.Add(new CellHighlighter.CellHighlight
                {
                    pos = pos,
                    frame = Clock.Frame,
                    color = color,
                    gameObject = this.GetNewCellHighlightGameObject(color, pos)
                });
            }
        }

        public void ArrowBetween(Vector3Int from, Vector3Int to)
        {
            if (DebugUI.HideUI)
            {
                return;
            }
            if (from.y != to.y)
            {
                return;
            }
            bool flag = false;
            for (int i = 0; i < this.arrows.Count; i++)
            {
                if (this.arrows[i].from == from && this.arrows[i].to == to)
                {
                    CellHighlighter.Arrow arrow = this.arrows[i];
                    arrow.frame = Clock.Frame;
                    this.arrows[i] = arrow;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                this.arrows.Add(new CellHighlighter.Arrow
                {
                    from = from,
                    to = to,
                    frame = Clock.Frame,
                    gameObject = this.GetNewArrowGameObject(from, to)
                });
            }
        }

        public void HighlightArea(CellCuboid cuboid)
        {
            if (DebugUI.HideUI)
            {
                return;
            }
            bool flag = false;
            for (int i = 0; i < this.areaHighlights.Count; i++)
            {
                if (this.areaHighlights[i].cuboid == cuboid)
                {
                    CellHighlighter.AreaHighlight areaHighlight = this.areaHighlights[i];
                    areaHighlight.frame = Clock.Frame;
                    this.areaHighlights[i] = areaHighlight;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                this.areaHighlights.Add(new CellHighlighter.AreaHighlight
                {
                    cuboid = cuboid,
                    frame = Clock.Frame,
                    gameObject = this.GetNewAreaHighlightGameObject(cuboid)
                });
            }
        }

        public void HighlightIcon(Vector3Int pos, Texture2D icon, Color iconColor, bool atTheBottom)
        {
            if (DebugUI.HideUI)
            {
                return;
            }
            bool flag = false;
            for (int i = 0; i < this.icons.Count; i++)
            {
                if (this.icons[i].pos == pos && this.icons[i].icon == icon && this.icons[i].iconColor == iconColor)
                {
                    CellHighlighter.Icon icon2 = this.icons[i];
                    icon2.frame = Clock.Frame;
                    this.icons[i] = icon2;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                this.icons.Add(new CellHighlighter.Icon
                {
                    pos = pos,
                    icon = icon,
                    iconColor = iconColor,
                    frame = Clock.Frame,
                    gameObject = this.GetNewIconGameObject(pos, icon, iconColor, atTheBottom)
                });
            }
        }

        public void HighlightAreaWithIcons(Vector3Int pos, int radius)
        {
            if (DebugUI.HideUI)
            {
                return;
            }
            foreach (Vector3Int vector3Int in pos.GetCellsWithin(radius).ClipToWorld())
            {
                this.HighlightIcon(vector3Int, CellHighlighter.AreaIcon, Color.white, false);
            }
        }

        public void HighlightPath(List<Vector3Int> positions)
        {
            if (DebugUI.HideUI)
            {
                return;
            }
            if (positions.Count != 0)
            {
                this.HighlightCell(positions[0], CellHighlighter.Yellow);
            }
            for (int i = 0; i < positions.Count - 1; i++)
            {
                this.ArrowBetween(positions[i + 1], positions[i]);
                this.HighlightCell(positions[i + 1], CellHighlighter.White);
            }
        }

        public void Update()
        {
            for (int i = this.cellHighlights.Count - 1; i >= 0; i--)
            {
                if (Clock.Frame > this.cellHighlights[i].frame + 1)
                {
                    this.ReleaseCellHighlightGameObject(this.cellHighlights[i].gameObject);
                    this.cellHighlights.RemoveAt(i);
                }
            }
            for (int j = this.arrows.Count - 1; j >= 0; j--)
            {
                if (Clock.Frame > this.arrows[j].frame + 1)
                {
                    this.ReleaseArrowGameObject(this.arrows[j].gameObject);
                    this.arrows.RemoveAt(j);
                }
            }
            for (int k = this.areaHighlights.Count - 1; k >= 0; k--)
            {
                if (Clock.Frame > this.areaHighlights[k].frame + 1)
                {
                    this.ReleaseAreaHighlightGameObject(this.areaHighlights[k].gameObject);
                    this.areaHighlights.RemoveAt(k);
                }
            }
            for (int l = this.icons.Count - 1; l >= 0; l--)
            {
                if (Clock.Frame > this.icons[l].frame + 1)
                {
                    this.ReleaseIconGameObject(this.icons[l].gameObject);
                    this.icons.RemoveAt(l);
                }
            }
            for (int m = 0; m < this.cellHighlights.Count; m++)
            {
                this.cellHighlights[m].gameObject.transform.localScale = this.GetAnimatedScaleForCellHighlight(this.cellHighlights[m].gameObject.transform.position);
            }
            for (int n = 0; n < this.arrows.Count; n++)
            {
                this.arrows[n].gameObject.transform.position = this.GetAnimatedArrowPosition(this.arrows[n].from, this.arrows[n].to, this.arrows[n].gameObject.transform.rotation);
            }
        }

        private void ReleaseCellHighlightGameObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            this.cellHighlightsPool.Add(gameObject);
        }

        private void ReleaseArrowGameObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            this.arrowsPool.Add(gameObject);
        }

        private void ReleaseAreaHighlightGameObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            this.areaHighlightsPool.Add(gameObject);
        }

        private void ReleaseIconGameObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            this.iconsPool.Add(gameObject);
        }

        private GameObject GetNewCellHighlightGameObject(Color color, Vector3Int pos)
        {
            GameObject gameObject;
            if (this.cellHighlightsPool.Count != 0)
            {
                gameObject = this.cellHighlightsPool[this.cellHighlightsPool.Count - 1];
                this.cellHighlightsPool.RemoveAt(this.cellHighlightsPool.Count - 1);
                gameObject.SetActive(true);
            }
            else
            {
                gameObject = Object.Instantiate<GameObject>(CellHighlighter.CellHighlightPrefab, Get.RuntimeSpecialContainer.transform);
            }
            gameObject.transform.position = pos + new Vector3(0f, -0.4945f, 0f);
            gameObject.transform.localScale = this.GetAnimatedScaleForCellHighlight(gameObject.transform.position);
            Material material;
            if (!this.cellHighlightMaterials.TryGetValue(color, out material))
            {
                material = Object.Instantiate<Material>(CellHighlighter.CellHighlightPrefab.GetComponent<Renderer>().sharedMaterial);
                material.color = color;
                this.cellHighlightMaterials.Add(color, material);
            }
            gameObject.GetComponent<Renderer>().sharedMaterial = material;
            return gameObject;
        }

        private GameObject GetNewArrowGameObject(Vector3Int from, Vector3Int to)
        {
            GameObject gameObject;
            if (this.arrowsPool.Count != 0)
            {
                gameObject = this.arrowsPool[this.arrowsPool.Count - 1];
                this.arrowsPool.RemoveAt(this.arrowsPool.Count - 1);
                gameObject.SetActive(true);
            }
            else
            {
                gameObject = Object.Instantiate<GameObject>(CellHighlighter.ArrowPrefab, Get.RuntimeSpecialContainer.transform);
            }
            float num = Calc.Atan2((float)(to.z - from.z), (float)(to.x - from.x)) * 57.29578f;
            gameObject.transform.rotation = Quaternion.Euler(0f, -num + 90f, 0f);
            gameObject.transform.position = this.GetAnimatedArrowPosition(from, to, gameObject.transform.rotation);
            gameObject.transform.localScale = new Vector3(0.3f, 0.15f, 0.3f);
            return gameObject;
        }

        private GameObject GetNewAreaHighlightGameObject(CellCuboid cuboid)
        {
            GameObject gameObject;
            if (this.areaHighlightsPool.Count != 0)
            {
                gameObject = this.areaHighlightsPool[this.areaHighlightsPool.Count - 1];
                this.areaHighlightsPool.RemoveAt(this.areaHighlightsPool.Count - 1);
                gameObject.SetActive(true);
            }
            else
            {
                gameObject = Object.Instantiate<GameObject>(CellHighlighter.AreaHighlightPrefab, Get.RuntimeSpecialContainer.transform);
            }
            gameObject.transform.position = cuboid.CenterFloat;
            gameObject.transform.localScale = new Vector3((float)cuboid.width, (float)cuboid.height, (float)cuboid.depth) - Vector3.one * 0.01f;
            return gameObject;
        }

        private GameObject GetNewIconGameObject(Vector3Int pos, Texture2D icon, Color iconColor, bool atTheBottom)
        {
            GameObject gameObject;
            if (this.iconsPool.Count != 0)
            {
                gameObject = this.iconsPool[this.iconsPool.Count - 1];
                this.iconsPool.RemoveAt(this.iconsPool.Count - 1);
                gameObject.SetActive(true);
            }
            else
            {
                gameObject = Object.Instantiate<GameObject>(CellHighlighter.IconPrefab, Get.RuntimeSpecialContainer.transform);
            }
            float num = Get.CellsInfo.OffsetFromStairsAt(pos);
            gameObject.transform.position = pos.WithAddedY(atTheBottom ? (-0.25f + num) : 0f) + new Vector3(0.001f, 0f, 0.001f);
            Material material;
            if (!this.iconMaterials.TryGetValue(new ValueTuple<Texture2D, Color>(icon, iconColor), out material))
            {
                material = Object.Instantiate<Material>(CellHighlighter.IconPrefab.GetComponent<Renderer>().sharedMaterial);
                material.mainTexture = icon;
                material.color *= iconColor;
                this.iconMaterials.Add(new ValueTuple<Texture2D, Color>(icon, iconColor), material);
            }
            gameObject.GetComponent<Renderer>().sharedMaterial = material;
            return gameObject;
        }

        private Vector3 GetAnimatedScaleForCellHighlight(Vector3 pos)
        {
            float num = Calc.Pulse(3f, 0.06f) + 0.94f;
            return new Vector3(num, num, 1f);
        }

        private Vector3 GetAnimatedArrowPosition(Vector3Int from, Vector3Int to, Quaternion rot)
        {
            float num = -Calc.Pulse(3f, 0.05f) + 0.025f;
            Vector3 vector = Vector3.Lerp(to, from, 0.55f);
            return new Vector3(vector.x, vector.y + -0.45f, vector.z) + rot * (Vector3.forward * num);
        }

        private List<CellHighlighter.CellHighlight> cellHighlights = new List<CellHighlighter.CellHighlight>();

        private List<CellHighlighter.Arrow> arrows = new List<CellHighlighter.Arrow>();

        private List<CellHighlighter.AreaHighlight> areaHighlights = new List<CellHighlighter.AreaHighlight>();

        private List<CellHighlighter.Icon> icons = new List<CellHighlighter.Icon>();

        private List<GameObject> cellHighlightsPool = new List<GameObject>();

        private List<GameObject> arrowsPool = new List<GameObject>();

        private List<GameObject> areaHighlightsPool = new List<GameObject>();

        private List<GameObject> iconsPool = new List<GameObject>();

        private Dictionary<Color, Material> cellHighlightMaterials = new Dictionary<Color, Material>();

        private Dictionary<ValueTuple<Texture2D, Color>, Material> iconMaterials = new Dictionary<ValueTuple<Texture2D, Color>, Material>();

        private static readonly GameObject CellHighlightPrefab = Assets.Get<GameObject>("Prefabs/Misc/CellHighlight");

        private static readonly GameObject AreaHighlightPrefab = Assets.Get<GameObject>("Prefabs/Misc/AreaHighlight");

        private static readonly GameObject ArrowPrefab = Assets.Get<GameObject>("Prefabs/Misc/Arrow");

        private static readonly GameObject IconPrefab = Assets.Get<GameObject>("Prefabs/Misc/CellHighlightIcon");

        private static readonly Texture2D AreaIcon = Assets.Get<Texture2D>("Textures/Misc/Area");

        private const float CellHighlightOffsetY = -0.4945f;

        private const float ArrowOffsetY = -0.45f;

        private const float ArrowsLerpFactor = 0.55f;

        private const float ArrowScale = 0.3f;

        private const float AreaHighlightEps = 0.01f;

        private const float IconOffsetY = -0.25f;

        public static readonly Color White = new Color(1f, 1f, 1f, 0.4f);

        public static readonly Color Yellow = new Color(1f, 0.83f, 0.43f, 0.6f);

        public static readonly Color Red = new Color(1f, 0.5f, 0.5f, 0.4f);

        public static readonly Color BrightRed = new Color(1f, 0.2f, 0.2f, 0.82f);

        public static readonly Color DeepRed = new Color(1f, 0f, 0f, 0.4f);

        public static readonly Color Violet = new Color(0.68f, 0.28f, 1f, 0.4f);

        private struct CellHighlight
        {
            public Vector3Int pos;

            public Color color;

            public int frame;

            public GameObject gameObject;
        }

        private struct Arrow
        {
            public Vector3Int from;

            public Vector3Int to;

            public int frame;

            public GameObject gameObject;
        }

        private struct AreaHighlight
        {
            public CellCuboid cuboid;

            public int frame;

            public GameObject gameObject;
        }

        private struct Icon
        {
            public Vector3Int pos;

            public int frame;

            public Texture2D icon;

            public Color iconColor;

            public GameObject gameObject;
        }
    }
}