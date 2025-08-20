using System;
using System.Collections.Generic;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class BodyPartOverlayDrawer
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (!Get.UI.WantsMouseUnlocked && KeyCodeUtility.InspectHeldDown)
            {
                return;
            }
            if (!InteractionManager.TargetBodyPartKeyBinding.HeldDown)
            {
                return;
            }
            this.tmpActors.Clear();
            List<Actor> actorsSeen_Unordered = Get.VisibilityCache.ActorsSeen_Unordered;
            for (int i = 0; i < actorsSeen_Unordered.Count; i++)
            {
                if (!actorsSeen_Unordered[i].IsNowControlledActor && (actorsSeen_Unordered[i] == Get.InteractionManager.PointedAtEntity || this.HasAnyNonMissingHarmedBodyPart(actorsSeen_Unordered[i])))
                {
                    this.tmpActors.Add(actorsSeen_Unordered[i]);
                }
            }
            if (this.tmpActors.Count == 0)
            {
                return;
            }
            if (this.tmpActors.Count >= 2)
            {
                this.tmpActors.Sort(BodyPartOverlayDrawer.ByDistToCamera);
            }
            for (int j = 0; j < this.tmpActors.Count; j++)
            {
                this.DoOverlays(this.tmpActors[j]);
            }
        }

        private void DoOverlays(Actor actor)
        {
            ActorGOC actorGOC = actor.ActorGOC;
            bool flag = Get.InteractionManager.PointedAtEntity == actor;
            List<BodyPart> bodyParts = actor.BodyParts;
            for (int i = 0; i < bodyParts.Count; i++)
            {
                BodyPart bodyPart = bodyParts[i];
                Rect rect;
                if (!bodyPart.IsMissing && (flag || bodyPart.Harmed) && WorldRectToUIRectUtility.GetUIRect(actorGOC.GetBodyPartOverlayWorldPos(bodyPart), new Vector2(0.08f, 0.08f), out rect))
                {
                    float num = 0f;
                    float num2 = 0f;
                    switch (bodyPart.Placement.Bracket)
                    {
                        case BodyPartPlacement.BracketType.UpperLeft:
                            GUIExtra.DrawRectBump(new Rect(rect.x, rect.y + rect.height * 0.1f, rect.width * 0.1f, rect.height * 0.9f), BodyPartOverlayDrawer.BracketsColor, false);
                            GUIExtra.DrawRectBump(new Rect(rect.x, rect.y, rect.width, rect.height * 0.1f), BodyPartOverlayDrawer.BracketsColor, false);
                            num = 0.08f;
                            num2 = 0.08f;
                            break;
                        case BodyPartPlacement.BracketType.UpperRight:
                            GUIExtra.DrawRectBump(new Rect(rect.x + rect.width * 0.9f, rect.y + rect.height * 0.1f, rect.width * 0.1f, rect.height * 0.9f), BodyPartOverlayDrawer.BracketsColor, false);
                            GUIExtra.DrawRectBump(new Rect(rect.x, rect.y, rect.width, rect.height * 0.1f), BodyPartOverlayDrawer.BracketsColor, false);
                            num = -0.08f;
                            num2 = 0.08f;
                            break;
                        case BodyPartPlacement.BracketType.BottomLeft:
                            GUIExtra.DrawRectBump(new Rect(rect.x, rect.y + rect.height * 0.1f, rect.width * 0.1f, rect.height * 0.9f), BodyPartOverlayDrawer.BracketsColor, false);
                            GUIExtra.DrawRectBump(new Rect(rect.x, rect.y + rect.height * 0.9f, rect.width, rect.height * 0.1f), BodyPartOverlayDrawer.BracketsColor, false);
                            num = 0.08f;
                            num2 = -0.08f;
                            break;
                        case BodyPartPlacement.BracketType.BottomRight:
                            GUIExtra.DrawRectBump(new Rect(rect.x + rect.width * 0.9f, rect.y + rect.height * 0.1f, rect.width * 0.1f, rect.height * 0.9f), BodyPartOverlayDrawer.BracketsColor, false);
                            GUIExtra.DrawRectBump(new Rect(rect.x, rect.y + rect.height * 0.9f, rect.width, rect.height * 0.1f), BodyPartOverlayDrawer.BracketsColor, false);
                            num = -0.08f;
                            num2 = -0.08f;
                            break;
                        case BodyPartPlacement.BracketType.Top:
                            GUIExtra.DrawRectBump(new Rect(rect.x, rect.y + rect.height * 0.1f, rect.width * 0.1f, rect.height * 0.9f), BodyPartOverlayDrawer.BracketsColor, false);
                            GUIExtra.DrawRectBump(new Rect(rect.x + rect.width * 0.9f, rect.y + rect.height * 0.1f, rect.width * 0.1f, rect.height * 0.9f), BodyPartOverlayDrawer.BracketsColor, false);
                            GUIExtra.DrawRectBump(new Rect(rect.x, rect.y, rect.width, rect.height * 0.1f), BodyPartOverlayDrawer.BracketsColor, false);
                            break;
                    }
                    Widgets.FontSize = Widgets.GetFontSizeToFitInHeight((bodyParts[i].HP >= 10) ? (rect.height * 0.8f) : rect.height);
                    GUI.color = (bodyPart.Harmed ? new Color(0.83f, 0.7f, 0.3f) : new Color(0.8f, 0.8f, 0.8f));
                    Widgets.LabelCentered(rect.ExpandedBy(rect.width * 0.8f).MovedBy(rect.width * num, rect.height * num2), bodyParts[i].HP.ToStringCached(), true, null, null);
                    GUI.color = Color.white;
                    Widgets.ResetFontSize();
                }
            }
        }

        private bool HasAnyNonMissingHarmedBodyPart(Actor actor)
        {
            List<BodyPart> bodyParts = actor.BodyParts;
            for (int i = 0; i < bodyParts.Count; i++)
            {
                if (!bodyParts[i].IsMissing && bodyParts[i].Harmed)
                {
                    return true;
                }
            }
            return false;
        }

        private List<Actor> tmpActors = new List<Actor>();

        private const float SizeInWorldUnits = 0.08f;

        private static readonly Color BracketsColor = new Color(0.8f, 0f, 0f);

        private static readonly Comparison<Actor> ByDistToCamera = (Actor a, Actor b) => (b.RenderPosition - Get.CameraPosition).sqrMagnitude.CompareTo((a.RenderPosition - Get.CameraPosition).sqrMagnitude);
    }
}