using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class InspectOverlaysDrawer
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (Get.UI.WorldInputBlocked)
            {
                return;
            }
            bool inspectHeldDown = KeyCodeUtility.InspectHeldDown;
            Item item = Get.InteractionManager.PointedAtEntity as Item;
            Actor actor = Get.InteractionManager.PointedAtEntity as Actor;
            if (!inspectHeldDown && item == null && actor == null)
            {
                return;
            }
            this.tmpEntities.Clear();
            this.tmpActorsInSequencePriorityOrder.Clear();
            if (inspectHeldDown)
            {
                List<Actor> actorsSeen_Unordered = Get.VisibilityCache.ActorsSeen_Unordered;
                for (int i = 0; i < actorsSeen_Unordered.Count; i++)
                {
                    if (!actorsSeen_Unordered[i].IsNowControlledActor)
                    {
                        this.tmpEntities.Add(actorsSeen_Unordered[i]);
                        if (AIUtility.WillMakeMoveBeforeMe(actorsSeen_Unordered[i], Get.NowControlledActor))
                        {
                            this.tmpActorsInSequencePriorityOrder.Add(actorsSeen_Unordered[i]);
                        }
                    }
                }
                List<Item> itemsSeen_Unordered = Get.VisibilityCache.ItemsSeen_Unordered;
                for (int j = 0; j < itemsSeen_Unordered.Count; j++)
                {
                    this.tmpEntities.Add(itemsSeen_Unordered[j]);
                }
            }
            else if (item != null && item.Spawned)
            {
                this.tmpEntities.Add(item);
            }
            else if (actor != null && actor.Spawned)
            {
                this.tmpEntities.Add(actor);
            }
            if (this.tmpEntities.Count == 0)
            {
                return;
            }
            if (this.tmpEntities.Count >= 2)
            {
                this.tmpEntities.Sort(InspectOverlaysDrawer.ByDistToCamera);
            }
            if (this.tmpActorsInSequencePriorityOrder.Count >= 2)
            {
                this.tmpActorsInSequencePriorityOrder.Sort(InspectOverlaysDrawer.BySequencePriority);
            }
            for (int k = 0; k < this.tmpEntities.Count; k++)
            {
                this.DoOverlays(this.tmpEntities[k]);
            }
        }

        private void DoOverlays(Entity entity)
        {
            Vector3 vector = HPBarOverlayDrawer.GetPosAboveHead(entity);
            Actor actor = entity as Actor;
            if (actor != null)
            {
                int drawnRowsCountFor = Get.HPBarOverlayDrawer.GetDrawnRowsCountFor(actor);
                vector = vector.WithAddedY((float)drawnRowsCountFor * HPBarOverlayDrawer.SizeInWorldUnits.y + 0.03f);
            }
            this.DoText(entity.LabelCap, vector);
            Actor actor2 = entity as Actor;
            if (actor2 != null && KeyCodeUtility.InspectHeldDown)
            {
                this.DoSequenceAndDistance(actor2);
            }
        }

        private void DoSequenceAndDistance(Actor actor)
        {
            Rect rect;
            if (!WorldRectToUIRectUtility.GetUIRect(HPBarOverlayDrawer.GetPosAboveHead(actor).WithAddedY(-0.1f), new Vector2(0.12f, 0.12f), out rect))
            {
                return;
            }
            float num = rect.height / 2f;
            float num2 = 0.12f * num;
            Widgets.FontSize = Widgets.GetFontSizeToFitInHeight(num);
            Widgets.Align = TextAnchor.MiddleLeft;
            if (Get.TurnManager.IsPlayerTurn_CanChooseNextAction)
            {
                int num3 = this.tmpActorsInSequencePriorityOrder.IndexOf(actor);
                string text;
                if (num3 >= 0)
                {
                    text = "{0}/{1}".Formatted((num3 + 1).ToStringCached(), this.tmpActorsInSequencePriorityOrder.Count.ToStringCached());
                    int num4 = AIUtility.CountMovesBeforeMe(actor, Get.NowControlledActor);
                    if (num4 > 1)
                    {
                        text = "{0} (x{1})".Formatted(text, num4.ToStringCached());
                    }
                }
                else
                {
                    text = "-";
                }
                float x = Widgets.CalcSize(text).x;
                float num5 = num + x + num2;
                float num6 = rect.center.x - num5 / 2f;
                GUI.color = new Color(0.8f, 0.8f, 0.8f);
                GUI.DrawTexture(new Rect(num6, rect.y, num, num), InspectOverlaysDrawer.SequenceIcon);
                Widgets.Label(new Rect(num6 + num + num2, rect.y, x, num), text, true, null, null, false);
            }
            int gridDistance = actor.Position.GetGridDistance(Get.NowControlledActor.Position);
            string text2 = gridDistance.ToStringCached();
            float x2 = Widgets.CalcSize(text2).x;
            float num7 = num + x2 + num2;
            float num8 = rect.center.x - num7 / 2f;
            if (gridDistance <= 1)
            {
                GUI.color = new Color(1f, 0.2f, 0.2f);
            }
            else
            {
                GUI.color = new Color(0.8f, 0.8f, 0.8f);
            }
            GUI.DrawTexture(new Rect(num8, rect.y + num, num, num), InspectOverlaysDrawer.DistanceIcon);
            Widgets.Label(new Rect(num8 + num + num2, rect.y + num, x2, num), text2, true, null, null, false);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
            Widgets.ResetAlign();
        }

        private void DoText(string text, Vector3 at)
        {
            float num = 0.06f;
            Rect rect;
            if (!WorldRectToUIRectUtility.GetUIRect(at, new Vector2(num, num), out rect))
            {
                return;
            }
            Widgets.FontSize = Widgets.GetFontSizeToFitInHeight(rect.height);
            GUI.color = new Color(0.8f, 0.8f, 0.8f);
            Widgets.LabelCentered(rect.center, text, true, null, null, false, true, false, null);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
        }

        private List<Entity> tmpEntities = new List<Entity>();

        private List<Actor> tmpActorsInSequencePriorityOrder = new List<Actor>();

        private const float SizeInWorldUnits = 0.12f;

        private const float SpaceBetweenIconAndLabel = 0.12f;

        private static readonly Texture2D SequenceIcon = Assets.Get<Texture2D>("Textures/UI/Sequence");

        private static readonly Texture2D DistanceIcon = Assets.Get<Texture2D>("Textures/UI/Distance");

        private static readonly Comparison<Entity> ByDistToCamera = (Entity a, Entity b) => (b.RenderPosition - Get.CameraPosition).sqrMagnitude.CompareTo((a.RenderPosition - Get.CameraPosition).sqrMagnitude);

        private static readonly Comparison<Actor> BySequencePriority = delegate (Actor a, Actor b)
        {
            if (a == b)
            {
                return 0;
            }
            if (!Get.TurnManager.HasSequencePriorityOver(a, b))
            {
                return 1;
            }
            return -1;
        };
    }
}