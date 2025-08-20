using System;
using System.Collections.Generic;
using Ricave.Core;
using Steamworks;
using UnityEngine;

namespace Ricave.UI
{
    public static class SteamDeckUtility
    {
        public static bool IsSteamDeck
        {
            get
            {
                return SteamDeckUtility.isSteamDeck;
            }
        }

        private static bool IsSteamDeckOrLinux
        {
            get
            {
                return SteamDeckUtility.IsSteamDeck || Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor;
            }
        }

        public static void Init()
        {
            SteamDeckUtility.isSteamDeck = SteamUtils.IsSteamRunningOnSteamDeck();
        }

        public static void Shutdown()
        {
        }

        public static void OnGUISkinApplied()
        {
            SteamDeckUtility.CheckFixMousePosition();
        }

        public static void OnGUI()
        {
        }

        public static void CheckFixMousePosition()
        {
            if (SteamDeckUtility.IsSteamDeckOrLinux)
            {
                Vector3 vector = Input.mousePosition / Widgets.UIScale;
                vector.y = Widgets.VirtualHeight - vector.y;
                vector = GUIUtility.ScreenToGUIPoint(vector * Widgets.UIScale);
                Event.current.mousePosition = vector;
            }
        }

        public static void HandleScrollViewTouch(Rect scrollViewRect, ref Vector2 scrollPos)
        {
            if (!SteamDeckUtility.isSteamDeck || Get.DragAndDrop.Dragging)
            {
                return;
            }
            Rect rect = new Rect(scrollViewRect.x, scrollViewRect.y, scrollViewRect.width - 15f - 1f, scrollViewRect.height - 15f - 1f);
            Vector2 vector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (Input.GetMouseButtonDown(0) && Mouse.Over(rect))
            {
                SteamDeckUtility.draggingScrollView = new Rect?(scrollViewRect);
                SteamDeckUtility.draggingScrollViewStartPos = vector;
                SteamDeckUtility.scrollViewDragStarted = false;
                SteamDeckUtility.scrollVelocityHistory.Clear();
                if (SteamDeckUtility.scrollViewForVelocity == SteamDeckUtility.draggingScrollView)
                {
                    SteamDeckUtility.scrollViewForVelocity = null;
                    SteamDeckUtility.scrollVelocity = default(Vector2);
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    Rect? rect2 = SteamDeckUtility.draggingScrollView;
                    Rect rect3 = scrollViewRect;
                    if (rect2 != null && (rect2 == null || rect2.GetValueOrDefault() == rect3))
                    {
                        if (!SteamDeckUtility.scrollViewDragStarted && (vector - SteamDeckUtility.draggingScrollViewStartPos).sqrMagnitude > 25f)
                        {
                            SteamDeckUtility.scrollViewDragStarted = true;
                        }
                        if (!SteamDeckUtility.scrollViewDragStarted)
                        {
                            goto IL_02AA;
                        }
                        scrollPos -= Event.current.delta;
                        SteamDeckUtility.scrollVelocityHistory.Add(new ValueTuple<float, Vector2>(Time.time, -Event.current.delta));
                        if (scrollPos.x < 0f)
                        {
                            scrollPos.x = 0f;
                        }
                        if (scrollPos.y < 0f)
                        {
                            scrollPos.y = 0f;
                            goto IL_02AA;
                        }
                        goto IL_02AA;
                    }
                }
                if (!Input.GetMouseButton(0))
                {
                    Rect? rect2 = SteamDeckUtility.draggingScrollView;
                    Rect rect3 = scrollViewRect;
                    if (rect2 != null && (rect2 == null || rect2.GetValueOrDefault() == rect3))
                    {
                        if (SteamDeckUtility.scrollViewDragStarted)
                        {
                            SteamDeckUtility.scrollViewForVelocity = SteamDeckUtility.draggingScrollView;
                            SteamDeckUtility.scrollVelocity = default(Vector2);
                            for (int i = 0; i < SteamDeckUtility.scrollVelocityHistory.Count; i++)
                            {
                                if (Time.time - SteamDeckUtility.scrollVelocityHistory[i].Item1 < 0.05f)
                                {
                                    SteamDeckUtility.scrollVelocity += SteamDeckUtility.scrollVelocityHistory[i].Item2;
                                }
                            }
                            SteamDeckUtility.consumeMouseUpFrame = Time.frameCount;
                        }
                        SteamDeckUtility.draggingScrollView = null;
                    }
                }
            }
        IL_02AA:
            if (Event.current.type == EventType.Repaint)
            {
                Rect? rect2 = SteamDeckUtility.scrollViewForVelocity;
                Rect rect3 = scrollViewRect;
                if (rect2 != null && (rect2 == null || rect2.GetValueOrDefault() == rect3) && SteamDeckUtility.scrollVelocity != default(Vector2))
                {
                    scrollPos += SteamDeckUtility.scrollVelocity;
                    SteamDeckUtility.scrollVelocity = Vector2.MoveTowards(SteamDeckUtility.scrollVelocity, Vector2.zero, Time.deltaTime * 100f);
                    if (scrollPos.x < 0f)
                    {
                        scrollPos.x = 0f;
                    }
                    if (scrollPos.y < 0f)
                    {
                        scrollPos.y = 0f;
                    }
                }
            }
            if (SteamDeckUtility.consumeMouseUpFrame == Time.frameCount && Event.current.type == EventType.MouseUp)
            {
                Event.current.Use();
            }
        }

        private static bool isSteamDeck;

        private static Rect? draggingScrollView;

        private static Vector2 draggingScrollViewStartPos;

        private static bool scrollViewDragStarted;

        private static List<ValueTuple<float, Vector2>> scrollVelocityHistory = new List<ValueTuple<float, Vector2>>();

        private static Rect? scrollViewForVelocity;

        private static Vector2 scrollVelocity;

        private static int consumeMouseUpFrame = -1;
    }
}