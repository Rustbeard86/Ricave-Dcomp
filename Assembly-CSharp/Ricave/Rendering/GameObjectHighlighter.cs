using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class GameObjectHighlighter
    {
        public Color? CustomColor
        {
            get
            {
                return this.customColor;
            }
        }

        public List<Renderer> AllHighlightedRenderers
        {
            get
            {
                this.tmpAllHighlightedRenderers.Clear();
                for (int i = 0; i < this.highlightedRenderers.Count; i++)
                {
                    if (this.highlightedRenderers[i].renderer != null)
                    {
                        this.tmpAllHighlightedRenderers.Add(this.highlightedRenderers[i].renderer);
                    }
                }
                return this.tmpAllHighlightedRenderers;
            }
        }

        public bool IsHighlighted(Renderer renderer)
        {
            if (renderer == null)
            {
                return false;
            }
            for (int i = 0; i < this.highlightedRenderers.Count; i++)
            {
                if (this.highlightedRenderers[i].renderer == renderer)
                {
                    return true;
                }
            }
            return false;
        }

        public void Highlight(GameObject gameObject, Color? customColor = null)
        {
            if (gameObject == null)
            {
                return;
            }
            this.customColor = customColor;
            foreach (Renderer renderer in gameObject.GetRenderersInChildren(false))
            {
                bool flag = false;
                for (int i = 0; i < this.highlightedRenderers.Count; i++)
                {
                    if (this.highlightedRenderers[i].renderer == renderer)
                    {
                        GameObjectHighlighter.HighlightedRenderer highlightedRenderer = this.highlightedRenderers[i];
                        highlightedRenderer.lastRegisteredFrame = Clock.Frame;
                        this.highlightedRenderers[i] = highlightedRenderer;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    this.highlightedRenderers.Add(new GameObjectHighlighter.HighlightedRenderer
                    {
                        renderer = renderer,
                        lastRegisteredFrame = Clock.Frame
                    });
                    Get.HighlightCamera.gameObject.SetActive(true);
                }
            }
        }

        public void Highlight(Entity entity, Color? customColor = null)
        {
            if (entity == null || !entity.Spawned)
            {
                return;
            }
            GameObject gameObject = entity.GameObject;
            if (gameObject != null)
            {
                this.Highlight(gameObject, customColor);
            }
        }

        public void Update()
        {
            for (int i = this.highlightedRenderers.Count - 1; i >= 0; i--)
            {
                if (this.highlightedRenderers[i].renderer == null || this.highlightedRenderers[i].lastRegisteredFrame < Clock.Frame - 1)
                {
                    this.highlightedRenderers.RemoveAt(i);
                }
            }
            Get.HighlightCamera.gameObject.SetActive(this.highlightedRenderers.Count != 0);
        }

        private List<GameObjectHighlighter.HighlightedRenderer> highlightedRenderers = new List<GameObjectHighlighter.HighlightedRenderer>();

        private Color? customColor;

        private List<Renderer> tmpAllHighlightedRenderers = new List<Renderer>();

        private struct HighlightedRenderer
        {
            public Renderer renderer;

            public int lastRegisteredFrame;
        }
    }
}