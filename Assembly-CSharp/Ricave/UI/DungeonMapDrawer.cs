using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class DungeonMapDrawer
    {
        public static void DrawMap(Rect inRect, ref bool scrolled)
        {
            float num = 0f;
            float num2 = 0f;
            foreach (Place place in Get.PlaceManager.Places)
            {
                num = Math.Max(num, DungeonMapDrawer.GetPlaceRect(place, inRect.width).yMax);
                num2 = Math.Max(num2, place.PositionOnMap.y);
            }
            if (Get.DungeonMapDrawer.Showing)
            {
                num += 25f;
            }
            Widgets.BeginScrollView(inRect, null);
            if (Get.DungeonMapDrawer.Showing)
            {
                Widgets.FontBold = true;
                Widgets.FontSizeScalable = 30;
                Widgets.LabelCentered(inRect.TopPart(150f), "DungeonMapHeader".Translate(), true, null, null);
                Widgets.ResetFontSize();
                Widgets.FontBold = false;
            }
            else if (!Get.RunSpec.LabelCap.NullOrEmpty())
            {
                Widgets.FontBold = true;
                Widgets.FontSizeScalable = 22;
                Widgets.Align = TextAnchor.UpperCenter;
                Rect rect = inRect.TopPart(50f);
                string text = (Mouse.Over(rect) ? Window_NewRun.GetTip(Get.RunSpec) : null);
                Widgets.Label(rect, Get.RunSpec.LabelCap, true, text, null, false);
                Widgets.ResetAlign();
                Widgets.ResetFontSize();
                Widgets.FontBold = false;
            }
            if (Get.DungeonMapDrawer.Showing)
            {
                Widgets.FontSizeScalable = 45;
                GUI.color = new Color(0.25f, 0.25f, 0.25f);
                int num3 = 0;
                List<Place> list = Get.PlaceManager.Places.ToTemporaryList<Place>();
                list.Sort(DungeonMapDrawer.ByPosition);
                foreach (Place place2 in list)
                {
                    if (place2.Floor != num3)
                    {
                        num3 = place2.Floor;
                        Rect placeRect = DungeonMapDrawer.GetPlaceRect(new Vector2(0f, place2.PositionOnMap.y), inRect.width);
                        Widgets.LabelCentered(new Vector2(placeRect.x - 70f, placeRect.center.y), (-place2.Floor).ToStringCached(), true, null, null, false, false, false, null);
                    }
                }
                GUI.color = Color.white;
                Widgets.ResetFontSize();
            }
            PlaceLink placeLink = null;
            foreach (PlaceLink placeLink2 in Get.PlaceManager.Links)
            {
                Vector2 center = DungeonMapDrawer.GetPlaceRect(placeLink2.From, inRect.width).center;
                Vector2 center2 = DungeonMapDrawer.GetPlaceRect(placeLink2.To, inRect.width).center;
                Rect rect2 = RectUtility.BoundingBox(center, center2).ExpandedBy(19f);
                if (Widgets.VisibleInScrollView(rect2))
                {
                    float num4 = Math.Min(DungeonMapDrawer.GetColorFactor(placeLink2.From), DungeonMapDrawer.GetColorFactor(placeLink2.To));
                    if (placeLink2 == DungeonMapDrawer.lastHoverLink && DungeonMapDrawer.LinkHasTip(placeLink2))
                    {
                        float num5 = Widgets.AccumulatedHover(rect2, false);
                        num4 = Calc.Lerp(num4, 1f, num5);
                        Get.Tooltips.RegisterTip(rect2, "{0}:\n\n{1}".Formatted("PlaceLinkTip".Translate(), placeLink2.RewardsDetails), null);
                    }
                    else
                    {
                        float num6 = Widgets.AccumulatedHover(rect2, true);
                        num4 = Calc.Lerp(num4, 1f, num6);
                    }
                    GUIExtra.DrawLine(center, center2, DungeonMapDrawer.PlaceBackgroundColor.MultipliedColor(num4), 4f);
                    bool flag;
                    DungeonMapDrawer.DoPlaceLinkRewards((center + center2) / 2f, placeLink2, num4, out flag);
                    if (flag)
                    {
                        placeLink = placeLink2;
                    }
                }
            }
            bool flag2 = false;
            foreach (Place place3 in Get.PlaceManager.Places)
            {
                Rect placeRect2 = DungeonMapDrawer.GetPlaceRect(place3, inRect.width);
                if (!scrolled && place3 == Get.Place && Event.current.type == EventType.Repaint)
                {
                    scrolled = true;
                    Widgets.ScrollCurrentScrollViewTo(placeRect2.center.y - 300f, true);
                }
                if (Widgets.VisibleInScrollView(placeRect2))
                {
                    bool flag3;
                    DungeonMapDrawer.DoPlace(placeRect2, place3, out flag3);
                    if (flag3)
                    {
                        flag2 = true;
                    }
                }
            }
            DungeonMapDrawer.lastHoverLink = null;
            if (placeLink != null)
            {
                DungeonMapDrawer.lastHoverLink = placeLink;
            }
            else if (!flag2)
            {
                foreach (PlaceLink placeLink3 in Get.PlaceManager.Links)
                {
                    Vector2 center3 = DungeonMapDrawer.GetPlaceRect(placeLink3.From, inRect.width).center;
                    Vector2 center4 = DungeonMapDrawer.GetPlaceRect(placeLink3.To, inRect.width).center;
                    if (Geometry.GetDistanceToLineFrom(center3, center4, Event.current.mousePosition) <= 11f)
                    {
                        DungeonMapDrawer.lastHoverLink = placeLink3;
                        break;
                    }
                }
            }
            Widgets.EndScrollView(inRect, num, false, false);
        }

        private static float GetCenteringOffsetX(float width)
        {
            return (width - 860f + 55f) / 2f;
        }

        private static void DoPlace(Rect rect, Place place, out bool hover)
        {
            float colorFactor = DungeonMapDrawer.GetColorFactor(place);
            GUIExtra.DrawRoundedRectBump(rect, DungeonMapDrawer.PlaceBackgroundColor.MultipliedColor(colorFactor), false, true, true, true, true, null);
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            Get.Tooltips.RegisterTip(rect, place, null, null);
            Rect rect2 = rect.ContractedBy(6f);
            string modifiersDetailsShort = place.ModifiersDetailsShort;
            float num = ((modifiersDetailsShort.NullOrEmpty() && place.ShelterItemReward == null && place.Enemies.NullOrEmpty<EntitySpec>()) ? 30f : 0f);
            GUI.color = place.IconColor.MultipliedColor(colorFactor);
            GUI.DrawTexture(rect2.TopPart(40f).ContractedToSquare().MovedBy(0f, num), place.Icon);
            GUI.color = Color.white;
            GUI.color = Color.white.MultipliedColor(colorFactor);
            Widgets.FontBold = true;
            Widgets.LabelCentered(rect2.CutFromTop(40f).TopPart(40f).MovedBy(0f, num), place.LabelCap, true, null, null);
            Widgets.FontBold = false;
            List<EntitySpec> enemies = place.Enemies;
            if (!enemies.NullOrEmpty<EntitySpec>())
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    Rect rect3 = rect2.CutFromTop(84f).StackHorizontally(i, enemies.Count).ContractedToSquare();
                    if (Get.Progress.SeenActorSpecs.Contains(enemies[i]) || Get.Player.SeenActorSpecs.Contains(enemies[i]))
                    {
                        GUI.color = enemies[i].IconColorAdjusted.MultipliedColor(colorFactor);
                        GUI.DrawTexture(rect3, enemies[i].IconAdjusted);
                    }
                    else
                    {
                        GUI.color = Color.white.MultipliedColor(colorFactor);
                        GUI.DrawTexture(rect3, DungeonMapDrawer.UndiscoveredEnemyIcon);
                    }
                    GUI.color = Color.white;
                }
            }
            if (!modifiersDetailsShort.NullOrEmpty())
            {
                GUI.color = Color.white.MultipliedColor(colorFactor);
                Widgets.Align = TextAnchor.UpperCenter;
                Widgets.Label(rect2.CutFromTop(84f), modifiersDetailsShort, true, null, null, false);
                Widgets.ResetAlign();
            }
            GUI.color = Color.white;
            if (place.ShelterItemReward != null)
            {
                EntitySpec entitySpec = place.ShelterItemReward.Value.EntitySpec;
                Rect rect4 = rect2.BottomHalf().ContractedToSquare().ContractedByPct(0.1f);
                GUI.color = entitySpec.IconColorAdjusted.MultipliedColor(colorFactor);
                GUI.DrawTexture(rect4, entitySpec.IconAdjusted);
                GUI.color = Color.white;
                if (place.ShelterItemReward.Value.Count != 1)
                {
                    GUI.color = Color.white.MultipliedColor(colorFactor);
                    Widgets.LabelCentered(rect2.BottomHalf().center.MovedBy(30f, 15f), "x{0}".Formatted(place.ShelterItemReward.Value.Count), true, null, null, false, false, false, null);
                    GUI.color = Color.white;
                }
            }
            if (place == Get.Place)
            {
                Rect rect5 = rect2.TopPart(32f).LeftPart(32f);
                GUIExtra.DrawHighlightIfMouseover(rect5, true, true, true, true, true);
                Get.Tooltips.RegisterTip(rect5, "YouAreHereTip".Translate(), null);
                GUI.color = Get.MainActor.IconColor;
                GUI.DrawTexture(rect5, Get.MainActor.Icon);
                GUI.color = Color.white;
            }
            if (Get.DungeonMapDrawer.Showing && Get.PlaceManager.LinkExists(Get.Place, place) && !Root.ChangingScene && !Get.ScreenFader.AnyActionQueued && Widgets.ButtonInvisible(rect, true, false))
            {
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                DungeonMapDrawer.OnChosenPlace(place);
            }
            hover = Mouse.Over(rect);
        }

        private static Rect GetPlaceRect(Place place, float totalWidth)
        {
            return DungeonMapDrawer.GetPlaceRect(place.PositionOnMap, totalWidth);
        }

        private static Rect GetPlaceRect(Vector2 placePos, float totalWidth)
        {
            Rect rect = RectUtility.CenteredAt(placePos * 215f, 160f);
            rect.position += new Vector2(80f, 80f);
            rect.position += new Vector2(DungeonMapDrawer.GetCenteringOffsetX(totalWidth), 0f);
            if (Get.DungeonMapDrawer.Showing)
            {
                rect.position += new Vector2(0f, 150f);
            }
            else
            {
                rect.position += new Vector2(0f, 50f);
            }
            return rect;
        }

        private static float GetColorFactor(Place place)
        {
            if (place == Get.Place)
            {
                return 1f;
            }
            if (Get.PlaceManager.LinkExists(Get.Place, place))
            {
                return 0.75f + (Get.DungeonMapDrawer.Showing ? Calc.PulseUnscaled(2f, 0.2f) : 0f);
            }
            return 0.45f;
        }

        private static void DoPlaceLinkRewards(Vector2 pos, PlaceLink link, float colorFactor, out bool mouseover)
        {
            int num = 0;
            if (link.ItemReward != null)
            {
                num++;
            }
            if (link.HealHP != 0)
            {
                num++;
            }
            mouseover = false;
            if (num == 0)
            {
                return;
            }
            float num2 = pos.x - 26f * (float)num / 2f;
            if (link.ItemReward != null)
            {
                EntitySpec entitySpec = link.ItemReward.Value.EntitySpec;
                Rect rect = RectUtility.CenteredAt(pos.WithX(num2 + 13f), 26f);
                GUI.color = entitySpec.IconColorAdjusted.MultipliedColor(colorFactor);
                GUI.DrawTexture(rect, entitySpec.Icon);
                GUI.color = Color.white;
                if (Mouse.Over(rect))
                {
                    mouseover = true;
                }
                num2 += 26f;
            }
            if (link.HealHP != 0)
            {
                Rect rect2 = RectUtility.CenteredAt(pos.WithX(num2 + 13f), 26f);
                GUI.color = Color.white.MultipliedColor(colorFactor);
                GUI.DrawTexture(rect2, Get.UseEffect_Heal.Icon);
                GUI.color = Color.white;
                if (Mouse.Over(rect2))
                {
                    mouseover = true;
                }
                num2 += 26f;
            }
        }

        private static bool LinkHasTip(PlaceLink link)
        {
            return link.ItemReward != null || link.HealHP != 0;
        }

        private static void OnChosenPlace(Place place)
        {
            int worldSeed = WorldGenUtility.CreateSeedForWorld(Get.RunSeed, place);
            Get.ScreenFader.FadeOutAndExecute(delegate
            {
                Run run = Get.Run;
                Place place2 = place;
                run.ReloadScene(RunOnSceneChanged.GenerateWorld(new WorldConfig(((place2 != null) ? place2.Spec.WorldSpec : null) ?? Get.RunSpec.DefaultWorldSpec, worldSeed, place, Get.PlaceManager.GetLink(Get.Place, place), null, false)), true);
            }, null, false, true, false);
        }

        private static PlaceLink lastHoverLink;

        private const float PlaceIconSize = 160f;

        private const float GapBetweenPlaces = 55f;

        private const int AssumePlaceCountWidth = 4;

        private static readonly Color PlaceBackgroundColor = new Color(0.34f, 0.34f, 0.34f);

        private const float LinkMouseoverRadius = 11f;

        private const float PlayerIconSize = 32f;

        private const float HeaderHeightSmall = 50f;

        private const float HeaderHeightLarge = 150f;

        private const float ExtraSpaceAtBottom = 25f;

        private static readonly Texture2D UndiscoveredEnemyIcon = Assets.Get<Texture2D>("Textures/UI/UnexploredRoom");

        private static readonly Comparison<Place> ByPosition = delegate (Place a, Place b)
        {
            Vector2 vector = a.PositionOnMap;
            int num = vector.y.CompareTo(b.PositionOnMap.y);
            if (num != 0)
            {
                return num;
            }
            vector = a.PositionOnMap;
            return vector.x.CompareTo(b.PositionOnMap.x);
        };
    }
}