using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class BossHPBar
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (this.showingFor == null)
            {
                for (int i = 0; i < this.spawnedBosses.Count; i++)
                {
                    if (Get.VisibilityCache.PlayerSees(this.spawnedBosses[i]))
                    {
                        this.showingFor = this.spawnedBosses[i];
                        break;
                    }
                }
                if (this.showingFor == null)
                {
                    return;
                }
            }
            bool flag = Get.VisibilityCache.PlayerSees(this.showingFor);
            if (flag)
            {
                this.lastSeenTime = Clock.Time;
            }
            if (flag || (Clock.Time - this.lastSeenTime < 1f && this.showingFor.Spawned))
            {
                this.alpha = Math.Min(this.alpha + Clock.DeltaTime * 2f, 1f);
            }
            else
            {
                this.alpha = Math.Max(this.alpha - Clock.DeltaTime * 0.5f, 0f);
                if (this.alpha <= 0f)
                {
                    this.showingFor = null;
                }
            }
            if (this.alpha <= 0f)
            {
                return;
            }
            float num = Math.Min(Widgets.VirtualWidth - 840f, 1000f);
            Rect rect = new Rect((Widgets.VirtualWidth - num) / 2f, 42f, num, 38f);
            Get.ProgressBarDrawer.Draw(rect, this.showingFor.HP, this.showingFor.MaxHP, TipSubjectDrawer_Entity.GetHPBarColor(this.showingFor), this.alpha, 1f, true, true, new int?(this.showingFor.InstanceID), ProgressBarDrawer.ValueChangeDirection.Constant, Get.InteractionManager.GetLostHPRangeForUI(this.showingFor), true, true, true, true, true, false, null);
            Widgets.FontSizeScalable = 24;
            Widgets.FontBold = true;
            GUI.color = new Color(1f, 1f, 1f, this.alpha);
            Widgets.LabelCentered(rect.center.WithAddedY(-37f), this.showingFor.LabelCap, true, null, null, false, false, false, null);
            GUI.color = Color.white;
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
        }

        public void OnEntitySpawned(Entity entity)
        {
            Actor actor = entity as Actor;
            if (actor != null && actor.IsBoss)
            {
                this.spawnedBosses.Add(actor);
            }
        }

        public void OnEntityDeSpawned(Entity entity)
        {
            Actor actor = entity as Actor;
            if (actor != null && actor.IsBoss)
            {
                this.spawnedBosses.Remove(actor);
            }
        }

        private List<Actor> spawnedBosses = new List<Actor>();

        private float lastSeenTime = -99999f;

        private Actor showingFor;

        private float alpha;

        private const float FadeOutAfter = 1f;

        private const float FadeInSpeed = 2f;

        private const float FadeOutSpeed = 0.5f;

        private const float Margin = 420f;
    }
}