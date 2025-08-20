using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class CachedGUI
    {
        public static Vector2 RepaintOffset
        {
            get
            {
                return CachedGUI.repaintOffset;
            }
            set
            {
                CachedGUI.repaintOffset = value;
            }
        }

        public static bool BeginCachedGUI(Rect rect, int ID, bool warnAboutSizeChange = true)
        {
            if (Event.current.type != EventType.Repaint)
            {
                CachedGUI.beginRenderTextureStack.Add(false);
                CachedGUI.partsStack.Add(default(CachedGUI.CachedPart));
                return true;
            }
            int i = 0;
            while (i < CachedGUI.cachedParts.Count)
            {
                if (CachedGUI.cachedParts[i].ID == ID)
                {
                    CachedGUI.CachedPart cachedPart = CachedGUI.cachedParts[i];
                    bool dirty = cachedPart.dirty;
                    bool flag;
                    if (rect.size != cachedPart.rect.size)
                    {
                        if (warnAboutSizeChange)
                        {
                            Log.Warning("Cached GUI rect size changed. This may be fine, but doing it every frame can degrade performance.", false);
                        }
                        flag = true;
                        Object.Destroy(cachedPart.renderTexture);
                        cachedPart.renderTexture = new RenderTexture(Calc.CeilToInt(rect.width * Widgets.UIScale), Calc.CeilToInt(rect.height * Widgets.UIScale), 0);
                    }
                    else
                    {
                        flag = false;
                    }
                    cachedPart.lastUsedFrame = Clock.Frame;
                    cachedPart.rect = rect;
                    cachedPart.dirty = false;
                    CachedGUI.cachedParts[i] = cachedPart;
                    if (flag || dirty)
                    {
                        GUIExtra.BeginRenderTexture(cachedPart.renderTexture);
                        CachedGUI.beginRenderTextureStack.Add(true);
                        CachedGUI.partsStack.Add(cachedPart);
                        CachedGUI.repaintOffset = -rect.position;
                        return true;
                    }
                    CachedGUI.beginRenderTextureStack.Add(false);
                    CachedGUI.partsStack.Add(cachedPart);
                    return false;
                }
                else
                {
                    i++;
                }
            }
            CachedGUI.cachedParts.Add(new CachedGUI.CachedPart
            {
                rect = rect,
                lastUsedFrame = Clock.Frame,
                ID = ID,
                renderTexture = new RenderTexture(Calc.CeilToInt(rect.width * Widgets.UIScale), Calc.CeilToInt(rect.height * Widgets.UIScale), 0)
            });
            GUIExtra.BeginRenderTexture(CachedGUI.cachedParts[CachedGUI.cachedParts.Count - 1].renderTexture);
            CachedGUI.beginRenderTextureStack.Add(true);
            CachedGUI.partsStack.Add(CachedGUI.cachedParts[CachedGUI.cachedParts.Count - 1]);
            CachedGUI.repaintOffset = -rect.position;
            return true;
        }

        public static void EndCachedGUI(float alpha = 1f, float scale = 1f)
        {
            bool flag = CachedGUI.beginRenderTextureStack[CachedGUI.beginRenderTextureStack.Count - 1];
            CachedGUI.beginRenderTextureStack.RemoveAt(CachedGUI.beginRenderTextureStack.Count - 1);
            CachedGUI.CachedPart cachedPart = CachedGUI.partsStack[CachedGUI.partsStack.Count - 1];
            CachedGUI.partsStack.RemoveAt(CachedGUI.partsStack.Count - 1);
            if (flag)
            {
                GUIExtra.EndRenderTexture();
                CachedGUI.repaintOffset = Vector2.zero;
            }
            if (Event.current.type == EventType.Repaint && alpha > 0f)
            {
                if (GUI.color != Color.white)
                {
                    Log.Warning("Drawing cached GUI render texture with non-white GUI.color. Resetting.", true);
                    GUI.color = Color.white;
                }
                Rect rect = new Rect(cachedPart.rect.x, cachedPart.rect.y, (float)cachedPart.renderTexture.width / Widgets.UIScale, (float)cachedPart.renderTexture.height / Widgets.UIScale);
                if (scale != 1f)
                {
                    rect = rect.ExpandedByPct(scale - 1f);
                }
                if (alpha < 1f)
                {
                    GUI.color = new Color(1f, 1f, 1f, alpha);
                }
                GUI.DrawTexture(rect, cachedPart.renderTexture);
                if (alpha < 1f)
                {
                    GUI.color = Color.white;
                }
            }
        }

        public static void SetDirty(int ID)
        {
            for (int i = 0; i < CachedGUI.cachedParts.Count; i++)
            {
                if (CachedGUI.cachedParts[i].ID == ID)
                {
                    CachedGUI.CachedPart cachedPart = CachedGUI.cachedParts[i];
                    cachedPart.dirty = true;
                    CachedGUI.cachedParts[i] = cachedPart;
                    return;
                }
            }
        }

        public static void Clear()
        {
            for (int i = CachedGUI.cachedParts.Count - 1; i >= 0; i--)
            {
                Object.Destroy(CachedGUI.cachedParts[i].renderTexture);
                CachedGUI.cachedParts.RemoveAt(i);
            }
        }

        public static void OnGUI()
        {
            if (CachedGUI.beginRenderTextureStack.Count != 0)
            {
                Log.Error("beginRenderTextureStack is not empty. Clearing.", false);
                CachedGUI.beginRenderTextureStack.Clear();
                RenderTexture.active = null;
            }
            if (CachedGUI.partsStack.Count != 0)
            {
                Log.Error("partsStack is not empty. Clearing.", false);
                CachedGUI.partsStack.Clear();
            }
            for (int i = CachedGUI.cachedParts.Count - 1; i >= 0; i--)
            {
                if (Clock.Frame - CachedGUI.cachedParts[i].lastUsedFrame > 60)
                {
                    Object.Destroy(CachedGUI.cachedParts[i].renderTexture);
                    CachedGUI.cachedParts.RemoveAt(i);
                }
            }
        }

        private static List<CachedGUI.CachedPart> cachedParts = new List<CachedGUI.CachedPart>();

        private static List<bool> beginRenderTextureStack = new List<bool>();

        private static List<CachedGUI.CachedPart> partsStack = new List<CachedGUI.CachedPart>();

        private static Vector2 repaintOffset;

        private struct CachedPart
        {
            public RenderTexture renderTexture;

            public Rect rect;

            public int ID;

            public int lastUsedFrame;

            public bool dirty;
        }
    }
}