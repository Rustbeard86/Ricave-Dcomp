using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public static class Widgets
    {
        public static float VirtualWidth
        {
            get
            {
                return Widgets.virtualWidth;
            }
        }

        public static float VirtualHeight
        {
            get
            {
                return Widgets.virtualHeight;
            }
        }

        private static float ResolutionDiagonal
        {
            get
            {
                return Calc.Sqrt((float)(Sys.Resolution.x * Sys.Resolution.x + Sys.Resolution.y * Sys.Resolution.y));
            }
        }

        public static float UIScale
        {
            get
            {
                return Math.Max(Widgets.ResolutionDiagonal / Widgets.Resolution1080Diagonal * 0.9f, SteamDeckUtility.IsSteamDeck ? 1f : 0.9f) * Widgets.uiScaleFactor;
            }
        }

        public static float UIScaleFactor
        {
            get
            {
                return Widgets.uiScaleFactor;
            }
            set
            {
                Widgets.uiScaleFactor = value;
            }
        }

        public static float FontScale
        {
            get
            {
                return Widgets.fontScale;
            }
            set
            {
                if (Widgets.fontScale == value)
                {
                    return;
                }
                Widgets.fontScale = value;
                Widgets.ResetFontSize();
                Widgets.skin.label.fontSize = Widgets.FontSize;
                Widgets.skin.button.fontSize = Widgets.FontSize;
                Widgets.skin.textField.fontSize = Widgets.FontSize;
                CachedGUI.Clear();
            }
        }

        public static Rect ScreenRect
        {
            get
            {
                return new Rect(0f, 0f, Widgets.VirtualWidth, Widgets.VirtualHeight);
            }
        }

        public static Vector2 ScreenCenter
        {
            get
            {
                return new Vector2(Widgets.VirtualWidth / 2f, Widgets.VirtualHeight / 2f);
            }
        }

        public static bool DraggingAnySlider
        {
            get
            {
                return Widgets.draggingSlider != default(Rect);
            }
        }

        public static float LineHeight
        {
            get
            {
                return Widgets.CalcSize("ABC").y;
            }
        }

        public static float SpaceBetweenLines
        {
            get
            {
                return Widgets.CalcSize("ABC\nABC").y - Widgets.CalcSize("ABC").y * 2f;
            }
        }

        public static bool WordWrap
        {
            get
            {
                return Widgets.skin.label.wordWrap;
            }
            set
            {
                Widgets.skin.label.wordWrap = value;
            }
        }

        public static int FontSize
        {
            get
            {
                return Widgets.skin.label.fontSize;
            }
            set
            {
                Widgets.skin.label.fontSize = value;
            }
        }

        public static int FontSizeScalable
        {
            get
            {
                return Calc.RoundToInt((float)Widgets.FontSize / Widgets.fontScale);
            }
            set
            {
                Widgets.FontSize = Calc.RoundToInt((float)value * Widgets.fontScale);
            }
        }

        public static bool FontBold
        {
            get
            {
                return Widgets.skin.label.fontStyle == FontStyle.Bold;
            }
            set
            {
                Widgets.skin.label.fontStyle = (value ? FontStyle.Bold : FontStyle.Normal);
            }
        }

        public static TextAnchor Align
        {
            get
            {
                return Widgets.skin.label.alignment;
            }
            set
            {
                Widgets.skin.label.alignment = value;
            }
        }

        public static bool IsTextCurrentlyLeftAligned
        {
            get
            {
                return GUI.skin.label.alignment == TextAnchor.LowerLeft || GUI.skin.label.alignment == TextAnchor.MiddleLeft || GUI.skin.label.alignment == TextAnchor.UpperLeft;
            }
        }

        public static bool IsTextCurrentlyRightAligned
        {
            get
            {
                return GUI.skin.label.alignment == TextAnchor.LowerRight || GUI.skin.label.alignment == TextAnchor.MiddleRight || GUI.skin.label.alignment == TextAnchor.UpperRight;
            }
        }

        public static bool IsTextCurrentlyCenterAligned
        {
            get
            {
                return GUI.skin.label.alignment == TextAnchor.LowerCenter || GUI.skin.label.alignment == TextAnchor.MiddleCenter || GUI.skin.label.alignment == TextAnchor.UpperCenter;
            }
        }

        public static bool IsTextCurrentlyLowerAligned
        {
            get
            {
                return GUI.skin.label.alignment == TextAnchor.LowerLeft || GUI.skin.label.alignment == TextAnchor.LowerRight || GUI.skin.label.alignment == TextAnchor.LowerCenter;
            }
        }

        public static bool IsTextCurrentlyMiddleAligned
        {
            get
            {
                return GUI.skin.label.alignment == TextAnchor.MiddleLeft || GUI.skin.label.alignment == TextAnchor.MiddleRight || GUI.skin.label.alignment == TextAnchor.MiddleCenter;
            }
        }

        public static bool IsTextCurrentlyUpperAligned
        {
            get
            {
                return GUI.skin.label.alignment == TextAnchor.UpperLeft || GUI.skin.label.alignment == TextAnchor.UpperRight || GUI.skin.label.alignment == TextAnchor.UpperCenter;
            }
        }

        public static void OnGUI()
        {
            Widgets.ApplySkin();
            if (Event.current.type == EventType.Repaint)
            {
                for (int i = Widgets.activeScrollViews.Count - 1; i >= 0; i--)
                {
                    if (Widgets.activeScrollViews[i].lastRegisteredFrame < Clock.Frame - 1)
                    {
                        Widgets.activeScrollViews.RemoveAt(i);
                    }
                }
                if (Widgets.scrollViewsStack.Count != 0)
                {
                    Log.Error("scrollViewsStack is not empty. Clearing.", false);
                    Widgets.scrollViewsStack.Clear();
                }
                for (int j = Widgets.accumulatedHovers.Count - 1; j >= 0; j--)
                {
                    if (Widgets.accumulatedHovers[j].Item3 < Clock.Frame - 1)
                    {
                        Widgets.accumulatedHovers.RemoveAt(j);
                    }
                }
            }
            if (Widgets.draggingSlider != default(Rect) && Event.current.rawType == EventType.MouseUp)
            {
                Widgets.draggingSlider = default(Rect);
                Get.Sound_DragEnd.PlayOneShot(null, 1f, 1f);
            }
            if (Math.Abs(Widgets.lastKnownResolutionWidth - (float)Sys.Resolution.x) > 1f || Math.Abs(Widgets.lastKnownResolutionHeight - (float)Sys.Resolution.y) > 1f)
            {
                Widgets.lastKnownResolutionWidth = (float)Sys.Resolution.x;
                Widgets.lastKnownResolutionHeight = (float)Sys.Resolution.y;
                Widgets.OnVirtualScreenSizeChanged();
                Get.WindowManager.OnResolutionChanged();
            }
            if (GUI.color != Color.white)
            {
                Log.Error("GUI.color not white at the beginning of the frame. Resetting.", false);
                GUI.color = Color.white;
            }
            if (GUI.skin.label.alignment != TextAnchor.UpperLeft)
            {
                Log.Error("Text align not UpperLeft at the beginning of the frame. Resetting.", false);
                GUI.skin.label.alignment = TextAnchor.UpperLeft;
            }
            if (!GUI.skin.label.wordWrap)
            {
                Log.Error("WordWrap not true at the beginning of the frame. Resetting.", false);
                GUI.skin.label.wordWrap = true;
            }
        }

        public static void ApplySkin()
        {
            if (Widgets.skin == null)
            {
                Widgets.skin = ScriptableObject.CreateInstance<GUISkin>();
                Widgets.ResetFontSize();
                Widgets.skin.label.fontSize = Widgets.FontSize;
                Widgets.skin.label.normal.textColor = Color.white;
                Widgets.skin.label.hover.textColor = Color.white;
                Widgets.skin.label.wordWrap = true;
                Widgets.skin.button.fontSize = Widgets.FontSize;
                Widgets.skin.button.alignment = TextAnchor.MiddleCenter;
                Widgets.skin.button.normal.textColor = Color.white;
                Widgets.skin.button.hover.textColor = Color.white;
                Widgets.skin.button.active.textColor = Color.white;
                Widgets.skin.font = Assets.Get<Font>("Fonts/NotoSans-Regular");
                Widgets.skin.verticalScrollbar.normal.background = Assets.Get<Texture2D>("Textures/UI/ScrollBar");
                Widgets.skin.verticalScrollbarThumb.normal.background = Assets.Get<Texture2D>("Textures/UI/ScrollBarThumb");
                Widgets.skin.verticalScrollbarDownButton.normal.background = Assets.Get<Texture2D>("Textures/UI/Transparent");
                Widgets.skin.verticalScrollbarUpButton.normal.background = Assets.Get<Texture2D>("Textures/UI/Transparent");
                Widgets.skin.verticalScrollbar.fixedWidth = 15f;
                Widgets.skin.horizontalScrollbar.normal.background = Assets.Get<Texture2D>("Textures/UI/ScrollBar");
                Widgets.skin.horizontalScrollbarThumb.normal.background = Assets.Get<Texture2D>("Textures/UI/ScrollBarThumb");
                Widgets.skin.horizontalScrollbarLeftButton.normal.background = Assets.Get<Texture2D>("Textures/UI/Transparent");
                Widgets.skin.horizontalScrollbarRightButton.normal.background = Assets.Get<Texture2D>("Textures/UI/Transparent");
                Widgets.skin.horizontalScrollbar.fixedHeight = 15f;
                Widgets.skin.textField.focused.textColor = Color.white;
                Widgets.skin.textField.focused.background = Assets.Get<Texture2D>("Textures/UI/Transparent");
                Widgets.skin.textField.normal.textColor = Color.white;
                Widgets.skin.textField.normal.background = Assets.Get<Texture2D>("Textures/UI/Transparent");
                Widgets.skin.textField.padding.left = 10;
                Widgets.skin.textField.padding.right = 10;
                Widgets.skin.textField.padding.bottom = 3;
                Widgets.skin.textField.padding.top = 6;
                Widgets.skin.textField.fontSize = Widgets.FontSize;
                Cursor.SetCursor(Widgets.CursorTex, default(Vector2), CursorMode.Auto);
                Widgets.ApplyUIScale();
                for (int i = 1; i < 250; i++)
                {
                    Widgets.GetFontSizeToFitInHeight((float)i);
                }
                string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_+=~`[]{}|\\:;\"'<>,.?/ ";
                Widgets.skin.font.RequestCharactersInTexture(text);
                Widgets.skin.font.RequestCharactersInTexture(text, 15);
                Widgets.skin.font.RequestCharactersInTexture(text, 15, FontStyle.Bold);
                Widgets.skin.font.RequestCharactersInTexture(text, Widgets.FontSize);
                Widgets.skin.font.RequestCharactersInTexture(text, Widgets.FontSize, FontStyle.Bold);
                Widgets.skin.font.RequestCharactersInTexture(text, 16);
                Widgets.skin.font.RequestCharactersInTexture(text, 16, FontStyle.Bold);
            }
            GUI.skin = Widgets.skin;
            Widgets.ApplyUIScale();
            SteamDeckUtility.OnGUISkinApplied();
        }

        public static float AccumulatedHover(Rect rect, bool forceNotMouseover = false)
        {
            float num = 0f;
            int num2 = -1;
            for (int i = 0; i < Widgets.accumulatedHovers.Count; i++)
            {
                if (Widgets.accumulatedHovers[i].Item1 == Window.Current && Widgets.accumulatedHovers[i].Item2 == rect)
                {
                    num = Widgets.accumulatedHovers[i].Item4;
                    num2 = i;
                    break;
                }
            }
            if (Event.current.type == EventType.Repaint)
            {
                if (!forceNotMouseover && Mouse.Over(rect))
                {
                    num = Math.Min(num + Clock.UnscaledDeltaTime * 20f, 1f);
                }
                else
                {
                    num = Math.Max(num - Clock.UnscaledDeltaTime * 10f, 0f);
                }
                if (num2 >= 0)
                {
                    if (num <= 0f)
                    {
                        Widgets.accumulatedHovers.RemoveAt(num2);
                    }
                    else
                    {
                        ValueTuple<Window, Rect, int, float> valueTuple = Widgets.accumulatedHovers[num2];
                        valueTuple.Item3 = Clock.Frame;
                        valueTuple.Item4 = num;
                        Widgets.accumulatedHovers[num2] = valueTuple;
                    }
                }
                else if (num > 0f)
                {
                    Widgets.accumulatedHovers.Add(new ValueTuple<Window, Rect, int, float>(Window.Current, rect, Clock.Frame, num));
                }
            }
            return num;
        }

        public static void IconAndLabelCentered(Vector2 pos, Texture2D icon, Color iconColor, string label, Color labelColor, float iconSize, float gap = 6f, float? truncateToWidth = null)
        {
            float num = Widgets.CalcSize(label).x;
            float num2 = iconSize + gap + num;
            if (truncateToWidth != null)
            {
                float num3 = num2;
                float? num4 = truncateToWidth;
                if ((num3 > num4.GetValueOrDefault()) & (num4 != null))
                {
                    label = label.Truncate(truncateToWidth.Value - iconSize - gap);
                    num = Widgets.CalcSize(label).x;
                    num2 = iconSize + gap + num;
                }
            }
            Color color = GUI.color;
            GUI.color = iconColor;
            GUIExtra.DrawTexture(RectUtility.CenteredAt(pos.MovedBy(-num2 / 2f + iconSize / 2f, 0f), iconSize), icon);
            GUI.color = labelColor;
            Widgets.LabelCentered(pos.MovedBy(num2 / 2f - num / 2f, 0f), label, true, null, null, false, false, false, null);
            GUI.color = color;
        }

        public static bool Button(Rect rect, string text, bool clickSound = true, Color? backgroundColor = null, Color? labelColor = null, bool tl = true, bool tr = true, bool br = true, bool bl = true, Texture2D image = null, bool doBackground = true, bool bump = true, int? fontSize = null, bool leftAligned = false, string tip = null, Color? imageColor = null)
        {
            Widgets.DoHoverSoundArea(rect);
            float num = Widgets.AccumulatedHover(rect, false);
            if (doBackground)
            {
                Color color;
                if (Mouse.Over(rect) && Input.GetMouseButton(0))
                {
                    color = (backgroundColor ?? Widgets.DefaultButtonColor).Lighter(0.15f);
                }
                else
                {
                    color = (backgroundColor ?? Widgets.DefaultButtonColor).Lighter(num * 0.1f);
                }
                if (bump)
                {
                    GUIExtra.DrawRoundedRectBump(rect, color, false, tl, tr, br, bl, null);
                }
                else
                {
                    GUIExtra.DrawRoundedRect(rect, color, tl, tr, br, bl, null);
                }
            }
            else
            {
                GUIExtra.DrawHighlight(rect, true, true, true, true, num);
                if (Mouse.Over(rect) && Input.GetMouseButton(0))
                {
                    GUIExtra.DrawHighlight(rect, true, true, true, true, 1f);
                }
            }
            if (!text.NullOrEmpty())
            {
                if (fontSize != null)
                {
                    Widgets.FontSizeScalable = fontSize.Value;
                }
                if (image != null)
                {
                    Color color2 = (imageColor ?? Color.white).MultipliedColor(0.9f).Lighter(num * 0.1f);
                    float num2 = Math.Min(rect.height - 4f, 22f);
                    Widgets.IconAndLabelCentered(rect.center, image, color2, text, labelColor ?? GUI.color, num2, 6f, new float?(rect.width));
                }
                else if (labelColor != null)
                {
                    Color color3 = GUI.color;
                    GUI.color = labelColor.Value;
                    if (leftAligned)
                    {
                        Widgets.Align = TextAnchor.MiddleLeft;
                        Widgets.Label(rect.CutFromLeft(10f), text, true, null, null, false);
                        Widgets.ResetAlign();
                    }
                    else
                    {
                        Widgets.LabelCentered(rect, text, true, null, null);
                    }
                    GUI.color = color3;
                }
                else if (leftAligned)
                {
                    Widgets.Align = TextAnchor.MiddleLeft;
                    Widgets.Label(rect.CutFromLeft(10f), text, true, null, null, false);
                    Widgets.ResetAlign();
                }
                else
                {
                    Widgets.LabelCentered(rect, text, true, null, null);
                }
                if (fontSize != null)
                {
                    Widgets.ResetFontSize();
                }
            }
            else if (image != null)
            {
                Rect rect2 = rect.ContractedByPct(0.1f).ContractedToSquare();
                Color color4 = GUI.color;
                GUI.color = (imageColor ?? Color.white).MultipliedColor(0.9f).Lighter(num * 0.1f);
                GUIExtra.DrawTexture(rect2, image);
                GUI.color = color4;
            }
            bool flag = GUI.Button(rect, "", Widgets.EmptyStyle);
            if (flag && clickSound)
            {
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
            }
            if (!tip.NullOrEmpty())
            {
                Get.Tooltips.RegisterTip(rect, tip, null);
            }
            return flag;
        }

        public static string TextField(Rect rect, string text, bool doBackground = true, bool centered = false)
        {
            Color color;
            if (Mouse.Over(rect))
            {
                color = new Color(0.28f, 0.28f, 0.28f);
            }
            else
            {
                color = new Color(0.23f, 0.23f, 0.23f);
            }
            if (doBackground)
            {
                GUIExtra.DrawRoundedRectBump(rect, color, true, true, true, true, true, null);
            }
            GUI.BeginClip(rect);
            TextAnchor alignment = GUI.skin.textField.alignment;
            if (centered)
            {
                GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
            }
            string text2 = GUI.TextField(rect.AtZero(), text);
            int num = GUIUtility.GetControlID(FocusType.Keyboard, rect.AtZero()) - 1;
            GUI.skin.textField.alignment = alignment;
            GUI.EndClip();
            SteamDeckUtility.CheckFixMousePosition();
            for (int i = Widgets.lastTextFieldDrawFrames.Count - 1; i >= 0; i--)
            {
                if (Widgets.lastTextFieldDrawFrames[i].Item2 < Clock.Frame - 10)
                {
                    Widgets.lastTextFieldDrawFrames.RemoveAt(i);
                }
            }
            bool flag = false;
            for (int j = 0; j < Widgets.lastTextFieldDrawFrames.Count; j++)
            {
                if (Widgets.lastTextFieldDrawFrames[j].Item1 == num)
                {
                    Widgets.lastTextFieldDrawFrames[j] = new ValueTuple<int, int>(num, Clock.Frame);
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                Widgets.lastTextFieldDrawFrames.Add(new ValueTuple<int, int>(num, Clock.Frame));
            }
            return text2;
        }

        public static void DoHoverSoundArea(Rect rect)
        {
            if (Widgets.lastHoverButtonFrame < Clock.Frame - 1)
            {
                Widgets.lastHoverButton = null;
            }
            bool flag = Widgets.lastHoverButton != null && Widgets.lastHoverButton.Value.Item1 == Window.Current && Widgets.lastHoverButton.Value.Item2 == rect;
            if (Mouse.Over(rect))
            {
                bool flag2;
                if (Widgets.lastHoverButton != null && Widgets.lastHoverButton.Value.Item1 == Window.Current)
                {
                    ValueTuple<Window, Rect> valueTuple = Widgets.lastHoverButton.Value;
                    if (valueTuple.Item2.Overlaps(rect))
                    {
                        if (Widgets.lastHoverButton.Value.Item2.Area() < rect.Area())
                        {
                            flag2 = true;
                            goto IL_010E;
                        }
                        if (Widgets.lastHoverButton.Value.Item2.Area() == rect.Area())
                        {
                            valueTuple = Widgets.lastHoverButton.Value;
                            flag2 = valueTuple.Item2.GetHashCode() < rect.GetHashCode();
                            goto IL_010E;
                        }
                        flag2 = false;
                        goto IL_010E;
                    }
                }
                flag2 = false;
            IL_010E:
                if (!flag2)
                {
                    Widgets.lastHoverButton = new ValueTuple<Window, Rect>?(new ValueTuple<Window, Rect>(Window.Current, rect));
                    Widgets.lastHoverButtonFrame = Clock.Frame;
                    if (!flag)
                    {
                        Get.Sound_Hover.PlayOneShot(null, 1f, 1f);
                        return;
                    }
                }
            }
            else if (flag)
            {
                Widgets.lastHoverButton = null;
            }
        }

        public static void DisabledButton(Rect rect, string text)
        {
            bool flag = false;
            Color? color = new Color?(new Color(0.6f, 0.6f, 0.6f));
            Widgets.Button(rect, text, flag, null, color, true, true, true, true, null, true, true, null, false, null, null);
        }

        public static bool ButtonInvisible(Rect rect, bool hoverSound = false, bool clickSound = false)
        {
            if (hoverSound)
            {
                Widgets.DoHoverSoundArea(rect);
            }
            bool flag = GUI.Button(rect, "", Widgets.EmptyStyle);
            if (flag && clickSound)
            {
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
            }
            return flag;
        }

        public static void DoLabelBackground(Rect rect)
        {
            GUIExtra.DrawRoundedRect(rect, new Color(0f, 0f, 0f, GUI.color.a * 0.15f), true, true, true, true, null);
        }

        public static void Label(Rect rect, string label, bool outline = true, string tip = null, ITipSubject tipSubject = null, bool drawBackground = false)
        {
            Widgets.<> c__DisplayClass97_0 CS$<> 8__locals1;
            CS$<> 8__locals1.label = label;
            CS$<> 8__locals1.rect = rect;
            if (!Widgets.VisibleInScrollView(CS$<> 8__locals1.rect))
            {
                return;
            }
            CS$<> 8__locals1.boundingRectCached = null;
            if (!tip.NullOrEmpty() || tipSubject != null)
            {
                Rect rect2 = Widgets.< Label > g__GetBoundingRect | 97_0(ref CS$<> 8__locals1);
                Get.Tooltips.RegisterTip(rect2, tipSubject, tip, null);
                GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            }
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (drawBackground)
            {
                Widgets.DoLabelBackground(Widgets.< Label > g__GetBoundingRect | 97_0(ref CS$<> 8__locals1));
            }
            CS$<> 8__locals1.rect.position = CS$<> 8__locals1.rect.position + CachedGUI.RepaintOffset;
            if (outline)
            {
                Color color = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, 0.1f * Calc.Pow(color.a, 1.6f));
                GUI.Label(CS$<> 8__locals1.rect.MovedBy(3f, 3f), CS$<> 8__locals1.label.MultiplyColorTagsByGUIColor());
                GUI.color = new Color(0f, 0f, 0f, 0.3f * Calc.Pow(color.a, 1.6f));
                GUI.Label(CS$<> 8__locals1.rect.MovedBy(2f, 2f), CS$<> 8__locals1.label.MultiplyColorTagsByGUIColor());
                GUI.color = new Color(0f, 0f, 0f, 0.6f * Calc.Pow(color.a, 1.6f));
                GUI.Label(CS$<> 8__locals1.rect.MovedBy(1f, 1f), CS$<> 8__locals1.label.MultiplyColorTagsByGUIColor());
                GUI.color = color;
            }
            GUI.Label(CS$<> 8__locals1.rect, CS$<> 8__locals1.label.MultiplyColorTagsByGUIColor());
        }

        public static void Label(float x, float width, ref float curY, string label, bool outline = true)
        {
            if (label.NullOrEmpty())
            {
                return;
            }
            float num = Widgets.CalcHeight(label, width);
            Widgets.Label(new Rect(x, curY, width, num + 10f), label, outline, null, null, false);
            curY += num;
        }

        public static void LabelCentered(float x, float width, ref float curY, string label, bool outline = true)
        {
            if (label.NullOrEmpty())
            {
                return;
            }
            float num = Widgets.CalcHeight(label, width);
            Rect rect = new Rect(x, curY, width, num + 10f);
            Widgets.Align = TextAnchor.UpperCenter;
            Widgets.Label(rect, label, outline, null, null, false);
            Widgets.ResetAlign();
            curY += num;
        }

        public static Rect LabelCenteredH(Vector2 pos, string label, bool outline = true, string tip = null, ITipSubject tipSubject = null, bool doHighlightEvenIfNoTip = false)
        {
            Vector2 vector = Widgets.CalcSize(label);
            Rect rect = new Rect(pos.x - vector.x / 2f, pos.y, vector.x, vector.y);
            if (!tip.NullOrEmpty() || tipSubject != null || doHighlightEvenIfNoTip)
            {
                Rect rect2 = rect.ExpandedBy(3f);
                if (!tip.NullOrEmpty() || tipSubject != null)
                {
                    Get.Tooltips.RegisterTip(rect2, tipSubject, tip, null);
                }
                GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            }
            Widgets.Label(rect, label, outline, null, null, false);
            return rect;
        }

        public static Rect LabelCenteredV(Vector2 pos, string label, bool outline = true, string tip = null, ITipSubject tipSubject = null, bool doHighlightEvenIfNoTip = false)
        {
            Vector2 vector = Widgets.CalcSize(label);
            Rect rect = new Rect(pos.x, pos.y - vector.y / 2f, vector.x, vector.y);
            if (!tip.NullOrEmpty() || tipSubject != null || doHighlightEvenIfNoTip)
            {
                Rect rect2 = rect.ExpandedBy(3f);
                if (!tip.NullOrEmpty() || tipSubject != null)
                {
                    Get.Tooltips.RegisterTip(rect2, tipSubject, tip, null);
                }
                GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            }
            Widgets.Label(rect, label, outline, null, null, false);
            return rect;
        }

        public static Rect LabelCentered(Vector2 pos, string label, bool outline = true, string tip = null, ITipSubject tipSubject = null, bool doHighlightEvenIfNoTip = false, bool drawBackground = false, bool clampToScreen = false, float? maxWidth = null)
        {
            Vector2 vector = ((maxWidth != null) ? Widgets.CalcSize(label, maxWidth.Value) : Widgets.CalcSize(label));
            Rect rect = new Rect(pos.x - vector.x / 2f, pos.y - vector.y / 2f, vector.x, vector.y);
            if (clampToScreen)
            {
                if (rect.x < 0f)
                {
                    rect.x = 0f;
                }
                else if (rect.xMax > Widgets.VirtualWidth)
                {
                    rect.x = Widgets.VirtualWidth - rect.width;
                }
                if (rect.y < 0f)
                {
                    rect.y = 0f;
                }
                else if (rect.yMax > Widgets.VirtualHeight)
                {
                    rect.y = Widgets.VirtualHeight - rect.height;
                }
            }
            if (!tip.NullOrEmpty() || tipSubject != null || doHighlightEvenIfNoTip)
            {
                Rect rect2 = rect.ExpandedBy(3f);
                if (!tip.NullOrEmpty() || tipSubject != null)
                {
                    Get.Tooltips.RegisterTip(rect2, tipSubject, tip, null);
                }
                GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            }
            Widgets.Align = TextAnchor.MiddleCenter;
            Widgets.Label(rect, label, outline, null, null, drawBackground);
            Widgets.ResetAlign();
            return rect;
        }

        public static void LabelCentered(Rect rect, string label, bool outline = true, string tip = null, ITipSubject tipSubject = null)
        {
            if (!tip.NullOrEmpty() || tipSubject != null)
            {
                float num = Widgets.CalcHeight(label, rect.width);
                Vector2 vector = Widgets.CalcSize(label);
                Rect rect2 = RectUtility.CenteredAt(rect.center, Math.Min(vector.x, rect.width), num).ExpandedBy(3f);
                Get.Tooltips.RegisterTip(rect2, tipSubject, tip, null);
                GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            }
            Widgets.Align = TextAnchor.MiddleCenter;
            Widgets.Label(rect, label, outline, null, null, false);
            Widgets.ResetAlign();
        }

        public static void LabelScrollable(Rect rect, string label, bool outline = true, string tip = null, ITipSubject tipSubject = null, bool drawBackground = false)
        {
            Rect rect2 = Widgets.BeginScrollView(rect, null);
            Widgets.Label(rect2, label, outline, tip, tipSubject, drawBackground);
            float num = Widgets.CalcHeight(label, rect2.width);
            Widgets.EndScrollView(rect, num, false, false);
        }

        public static float Slider(Rect rect, float value, float min, float max, float roundTo = 0.01f, bool showValueAsPct = false)
        {
            float num = Calc.InverseLerp(min, max, value);
            float num2 = Widgets.Slider(rect, num, showValueAsPct ? value.ToStringPercent(false) : value.ToString("0.##"));
            return Calc.RoundTo(Calc.Lerp(min, max, num2), roundTo);
        }

        public static float Slider(Rect rect, float value, string label = null)
        {
            bool flag = Mouse.Over(rect);
            float num = rect.x + 6.5f;
            float num2 = rect.xMax - 6.5f;
            bool flag2 = ((Widgets.draggingSlider != default(Rect)) ? (Widgets.draggingSlider == rect) : flag);
            GUIExtra.DrawRoundedRectBump(new Rect(rect.x, rect.center.y - 2.5f, rect.width, 5f), new Color(0.46f, 0.49f, 0.56f).Lighter(flag2 ? 0.15f : 0f), false, true, true, true, true, null);
            if (!label.NullOrEmpty())
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                Widgets.LabelCentered(rect.center.WithAddedY(-17.5f), label, true, null, null, false, false, false, null);
                GUI.color = Color.white;
            }
            Rect rect2 = new Rect(num + (num2 - num) * value - 6.5f, rect.center.y - 11f, 13f, 22f);
            GUIExtra.DrawRoundedRectBump(rect2, new Color(0.26f, 0.29f, 0.36f).Lighter((Widgets.draggingSlider == rect) ? 0.25f : (flag2 ? 0.15f : 0f)), false, true, true, true, true, null);
            bool flag3 = false;
            if (flag && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Event.current.Use();
                Widgets.draggingSlider = rect;
                Widgets.draggingSliderMouseOffset = (Mouse.Over(rect2) ? (rect2.center.x - Event.current.mousePosition.x) : 0f);
                flag3 = true;
                Get.Sound_DragStart.PlayOneShot(null, 1f, 1f);
            }
            else if (Event.current.type == EventType.Repaint && Widgets.draggingSlider == rect)
            {
                flag3 = true;
            }
            if (flag3)
            {
                value = Calc.InverseLerp(num, num2, Event.current.mousePosition.x + Widgets.draggingSliderMouseOffset);
            }
            if (Widgets.draggingSlider != default(Rect) && (Event.current.rawType == EventType.MouseUp || !Input.GetMouseButton(0)))
            {
                Widgets.draggingSlider = default(Rect);
                Get.Sound_DragEnd.PlayOneShot(null, 1f, 1f);
            }
            return value;
        }

        public static bool Checkbox(Vector2 pos, bool check)
        {
            if (Widgets.Button(new Rect(pos.x, pos.y, 26f, 26f), check ? "✓" : "", true, null, null, true, true, true, true, null, true, true, null, false, null, null))
            {
                check = !check;
            }
            return check;
        }

        public static bool CheckboxCentered(Vector2 pos, bool check)
        {
            return Widgets.Checkbox(new Vector2(pos.x - 13f, pos.y - 13f), check);
        }

        public static bool CheckboxCenteredV(Vector2 pos, bool check)
        {
            return Widgets.Checkbox(new Vector2(pos.x, pos.y - 13f), check);
        }

        public static bool CheckboxCenteredH(Vector2 pos, bool check)
        {
            return Widgets.Checkbox(new Vector2(pos.x - 13f, pos.y), check);
        }

        public static bool CheckboxLabeled(Rect rect, string label, bool check, Color? labelColor = null, string tip = null)
        {
            Rect rect2 = new Rect(rect.x, rect.y + (rect.height - 26f) / 2f, 26f, 26f);
            GUIExtra.DrawRoundedRectBump(rect2, Widgets.DefaultButtonColor, false, true, true, true, true, null);
            Widgets.LabelCentered(rect2, check ? "✓" : "", true, null, null);
            if (labelColor != null)
            {
                GUI.color = labelColor.Value;
            }
            Widgets.LabelCenteredV(rect2.center.MovedBy(rect2.width / 2f + 5f, 0f), label, true, null, null, false);
            if (labelColor != null)
            {
                GUI.color = Color.white;
            }
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            if (!tip.NullOrEmpty())
            {
                Get.Tooltips.RegisterTip(rect, tip, null);
            }
            if (Widgets.ButtonInvisible(rect, true, true))
            {
                check = !check;
            }
            return check;
        }

        public static bool Radio(Vector2 pos, bool check, string label)
        {
            Rect rect;
            return Widgets.Radio(pos, check, label, out rect);
        }

        public static bool Radio(Vector2 pos, bool check, string label, out Rect entireRect)
        {
            Rect rect = new Rect(pos.x, pos.y, 26f, 26f);
            float x = Widgets.CalcSize(label).x;
            entireRect = new Rect(rect.x, rect.y, rect.width + 5f + x, rect.height);
            GUIExtra.DrawRoundedRectBump(rect, Widgets.DefaultButtonColor, false, true, true, true, true, null);
            Widgets.LabelCentered(rect, check ? "✓" : "", true, null, null);
            Widgets.LabelCenteredV(rect.center.MovedBy(rect.width / 2f + 5f, 0f), label, true, null, null, false);
            GUIExtra.DrawHighlightIfMouseover(entireRect, true, true, true, true, true);
            if (Widgets.ButtonInvisible(entireRect, true, true))
            {
                check = !check;
            }
            return check;
        }

        public static bool Radio(Vector2 pos, bool check)
        {
            if (Widgets.Button(new Rect(pos.x, pos.y, 26f, 26f), check ? "✓" : "", true, null, null, true, true, true, true, null, true, true, null, false, null, null))
            {
                check = !check;
            }
            return check;
        }

        public static bool RadioCentered(Vector2 pos, bool check)
        {
            return Widgets.Radio(new Vector2(pos.x - 13f, pos.y - 13f), check);
        }

        public static bool RadioCenteredV(Vector2 pos, bool check)
        {
            return Widgets.Radio(new Vector2(pos.x, pos.y - 13f), check);
        }

        public static bool RadioCenteredH(Vector2 pos, bool check)
        {
            return Widgets.Radio(new Vector2(pos.x - 13f, pos.y), check);
        }

        public static void Section(string title, float x, float width, ref float curY, bool gray = false, float spaceAfter = 7f)
        {
            Widgets.FontBold = true;
            if (gray)
            {
                GUI.color = new Color(0.6f, 0.6f, 0.6f);
            }
            Widgets.Label(x, width, ref curY, title, true);
            if (gray)
            {
                GUI.color = Color.white;
            }
            Widgets.FontBold = false;
            curY += 1f;
            GUIExtra.DrawHorizontalLineBump(new Vector2(x, curY), width, new Color(0.45f, 0.45f, 0.45f));
            curY += spaceAfter;
        }

        public static void SectionBackground(Rect rect)
        {
            GUIExtra.DrawRoundedRect(rect, new Color(0f, 0f, 0f, 0.2f), true, true, true, true, null);
        }

        public static void AltRowHighlight(Rect rect)
        {
            GUIExtra.DrawHighlight(rect, true, true, true, true, 0.2f);
        }

        public static void ResetFontSize()
        {
            Widgets.FontSizeScalable = 15;
        }

        public static void ResetAlign()
        {
            Widgets.Align = TextAnchor.UpperLeft;
        }

        public static Vector2 CalcSize(string label)
        {
            if (Widgets.tmpGUIContent == null)
            {
                Widgets.tmpGUIContent = new GUIContent(label);
            }
            else
            {
                Widgets.tmpGUIContent.text = label;
            }
            Vector2 vector = Widgets.skin.label.CalcSize(Widgets.tmpGUIContent);
            vector.x += 0.71f;
            return vector;
        }

        public static float CalcHeight(string label, float width)
        {
            if (Widgets.tmpGUIContent == null)
            {
                Widgets.tmpGUIContent = new GUIContent(label);
            }
            else
            {
                Widgets.tmpGUIContent.text = label;
            }
            return Widgets.skin.label.CalcHeight(Widgets.tmpGUIContent, width);
        }

        public static Vector2 CalcSize(string label, float width)
        {
            return new Vector2(Math.Min(Widgets.CalcSize(label).x, width), Widgets.CalcHeight(label, width));
        }

        public static Rect CalcTrimmedBounds(string label, Rect rect)
        {
            float num = Widgets.CalcHeight(label, rect.width);
            Vector2 vector = Widgets.CalcSize(label);
            Rect rect2;
            if (Widgets.IsTextCurrentlyRightAligned)
            {
                float num2 = Math.Min(vector.x, rect.width);
                rect2 = new Rect(rect.xMax - num2, rect.y, num2, num);
            }
            else if (Widgets.IsTextCurrentlyCenterAligned)
            {
                float num3 = Math.Min(vector.x, rect.width);
                rect2 = new Rect(rect.x + (rect.width - num3) / 2f, rect.y, num3, num);
            }
            else
            {
                rect2 = new Rect(rect.x, rect.y, Math.Min(vector.x, rect.width), num);
            }
            if (Widgets.IsTextCurrentlyLowerAligned)
            {
                rect2.y = rect.yMax - rect2.height;
            }
            else if (Widgets.IsTextCurrentlyMiddleAligned)
            {
                rect2.y = rect.y + (rect.height - rect2.height) / 2f;
            }
            return rect2;
        }

        public static int GetFontSizeToFitInHeight(float height)
        {
            int num = Calc.CeilToInt(height);
            int num2;
            if (Widgets.fontSizeToFitCache.TryGetValue(num, out num2))
            {
                return num2;
            }
            for (int i = 150; i >= 8; i--)
            {
                float y;
                if (!Widgets.fontHeightCache.TryGetValue(i, out y))
                {
                    Widgets.FontSize = i;
                    y = Widgets.CalcSize("ABC").y;
                    Widgets.fontHeightCache.Add(i, y);
                    Widgets.ResetFontSize();
                }
                if (y <= height)
                {
                    Widgets.fontSizeToFitCache.Add(num, i);
                    return i;
                }
            }
            Widgets.fontSizeToFitCache.Add(num, 8);
            return 8;
        }

        public static void ApplyUIScale()
        {
            float uiscale = Widgets.UIScale;
            if (uiscale != Widgets.prevAppliedUIScale)
            {
                Widgets.fontSizeToFitCache.Clear();
                Widgets.fontHeightCache.Clear();
                CachedGUI.Clear();
                Widgets.prevAppliedUIScale = uiscale;
                Widgets.cachedGUIMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(uiscale, uiscale, 1f));
                Widgets.OnVirtualScreenSizeChanged();
                Get.WindowManager.OnUIScaleChanged();
            }
            GUI.matrix = Widgets.cachedGUIMatrix;
            SteamDeckUtility.CheckFixMousePosition();
        }

        private static void OnVirtualScreenSizeChanged()
        {
            float uiscale = Widgets.UIScale;
            if (uiscale == 1f)
            {
                Widgets.virtualWidth = (float)Sys.Resolution.x;
                Widgets.virtualHeight = (float)Sys.Resolution.y;
                return;
            }
            Widgets.virtualWidth = (float)Sys.Resolution.x / uiscale;
            Widgets.virtualHeight = (float)Sys.Resolution.y / uiscale;
        }

        public static Rect BeginScrollView(Rect rect, int? uniqueID = null)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(rect.position / Widgets.UIScale);
            Rect rect2 = new Rect(vector, rect.size);
            int num = -1;
            Widgets.ActiveScrollView activeScrollView = default(Widgets.ActiveScrollView);
            int i = 0;
            while (i < Widgets.activeScrollViews.Count)
            {
                if (uniqueID == null)
                {
                    Widgets.ActiveScrollView activeScrollView2 = Widgets.activeScrollViews[i];
                    if (activeScrollView2.uniqueID == null && Widgets.activeScrollViews[i].rect == rect2)
                    {
                        goto IL_00B8;
                    }
                }
                if (uniqueID != null)
                {
                    int? uniqueID2 = Widgets.activeScrollViews[i].uniqueID;
                    int? num2 = uniqueID;
                    if ((uniqueID2.GetValueOrDefault() == num2.GetValueOrDefault()) & (uniqueID2 != null == (num2 != null)))
                    {
                        goto IL_00B8;
                    }
                }
                i++;
                continue;
            IL_00B8:
                activeScrollView = Widgets.activeScrollViews[i];
                activeScrollView.lastRegisteredFrame = Clock.Frame;
                activeScrollView.rect = rect2;
                Widgets.activeScrollViews[i] = activeScrollView;
                num = i;
                break;
            }
            if (num < 0)
            {
                Widgets.ActiveScrollView activeScrollView2 = new Widgets.ActiveScrollView
                {
                    rect = rect2,
                    lastRegisteredFrame = Clock.Frame,
                    firstRegisteredFrame = Clock.Frame,
                    uniqueID = uniqueID
                };
                activeScrollView = activeScrollView2;
                num = Widgets.activeScrollViews.Count;
                Widgets.activeScrollViews.Add(activeScrollView);
            }
            bool flag = activeScrollView.height > activeScrollView.rect.height;
            if (!flag)
            {
                activeScrollView.scrollPos = default(Vector2);
                activeScrollView.smoothScrollPos_start = default(Vector2);
                activeScrollView.smoothScrollPos_end = default(Vector2);
                activeScrollView.smoothScrollPos_animation = 0f;
            }
            Rect rect3 = new Rect(0f, 0f, rect.width - (flag ? 15f : 0f), activeScrollView.height);
            Vector2 vector2 = GUI.BeginScrollView(rect, activeScrollView.scrollPos, rect3);
            SteamDeckUtility.CheckFixMousePosition();
            if (vector2 != activeScrollView.scrollPos)
            {
                activeScrollView.scrollPos = vector2;
                activeScrollView.smoothScrollPos_start = vector2;
                activeScrollView.smoothScrollPos_end = vector2;
                activeScrollView.smoothScrollPos_animation = 0f;
            }
            if (Event.current.type == EventType.Repaint && activeScrollView.smoothScrollPos_animation > 0f)
            {
                activeScrollView.smoothScrollPos_animation = Math.Max(activeScrollView.smoothScrollPos_animation - Clock.UnscaledDeltaTime * 1.95f, 0f);
                activeScrollView.scrollPos = Vector2Utility.SmoothStep(activeScrollView.smoothScrollPos_start, activeScrollView.smoothScrollPos_end, 1f - activeScrollView.smoothScrollPos_animation);
            }
            activeScrollView.lastViewRect = rect3;
            Widgets.activeScrollViews[num] = activeScrollView;
            Widgets.scrollViewsStack.Add(num);
            return rect3;
        }

        public static void EndScrollView(Rect rect, float height, bool startAtBottom = false, bool startAtBottomSmoothScroll = false)
        {
            GUI.EndScrollView();
            SteamDeckUtility.CheckFixMousePosition();
            int num = Widgets.scrollViewsStack[Widgets.scrollViewsStack.Count - 1];
            Widgets.ActiveScrollView activeScrollView = Widgets.activeScrollViews[num];
            Widgets.scrollViewsStack.RemoveAt(Widgets.scrollViewsStack.Count - 1);
            if (Event.current.type == EventType.Repaint || height > activeScrollView.height)
            {
                if (startAtBottom && activeScrollView.height != height)
                {
                    Widgets.ScrollTo(ref activeScrollView, height - rect.height, startAtBottomSmoothScroll);
                }
                activeScrollView.height = height;
                Widgets.activeScrollViews[num] = activeScrollView;
            }
        }

        public static void ScrollCurrentScrollViewTo(float toY, bool smooth = false)
        {
            if (Widgets.scrollViewsStack.Count == 0)
            {
                Log.Error("Tried to scroll scroll view but there's none shown currently.", false);
                return;
            }
            int num = Widgets.scrollViewsStack[Widgets.scrollViewsStack.Count - 1];
            Widgets.ActiveScrollView activeScrollView = Widgets.activeScrollViews[num];
            Widgets.ScrollTo(ref activeScrollView, toY, smooth);
            Widgets.activeScrollViews[num] = activeScrollView;
        }

        private static void ScrollTo(ref Widgets.ActiveScrollView scrollView, float toY, bool smooth = false)
        {
            if (smooth)
            {
                scrollView.smoothScrollPos_start = scrollView.scrollPos;
                scrollView.smoothScrollPos_end = new Vector2(0f, toY);
                scrollView.smoothScrollPos_animation = 1f;
                return;
            }
            scrollView.scrollPos = new Vector2(0f, toY);
            scrollView.smoothScrollPos_start = scrollView.scrollPos;
            scrollView.smoothScrollPos_end = scrollView.scrollPos;
            scrollView.smoothScrollPos_animation = 0f;
        }

        public static bool IsInNewScrollView()
        {
            return Widgets.scrollViewsStack.Count != 0 && Widgets.activeScrollViews[Widgets.scrollViewsStack[Widgets.scrollViewsStack.Count - 1]].firstRegisteredFrame == Clock.Frame;
        }

        public static bool VisibleInScrollView(Rect rect)
        {
            if (Widgets.scrollViewsStack.Count == 0)
            {
                return true;
            }
            Widgets.ActiveScrollView activeScrollView = Widgets.activeScrollViews[Widgets.scrollViewsStack[Widgets.scrollViewsStack.Count - 1]];
            return rect.yMax >= activeScrollView.lastViewRect.y + activeScrollView.scrollPos.y && rect.yMin <= activeScrollView.lastViewRect.y + activeScrollView.scrollPos.y + activeScrollView.rect.height;
        }

        public static bool VisibleInScrollView(Vector2 pos)
        {
            if (Widgets.scrollViewsStack.Count == 0)
            {
                return true;
            }
            Widgets.ActiveScrollView activeScrollView = Widgets.activeScrollViews[Widgets.scrollViewsStack[Widgets.scrollViewsStack.Count - 1]];
            Rect rect = new Rect(activeScrollView.lastViewRect.x + activeScrollView.scrollPos.x, activeScrollView.lastViewRect.y + activeScrollView.scrollPos.y, activeScrollView.rect.width, activeScrollView.rect.height);
            return rect.Contains(pos);
        }

        public static void Tabs(Rect rect, List<string> titles, ref int selected, bool rounded = true, List<string> tips = null, List<Texture2D> icons = null)
        {
            if (titles.Count == 0)
            {
                selected = -1;
                return;
            }
            if (selected < 0 || selected >= titles.Count)
            {
                selected = 0;
            }
            float num = rect.width / (float)titles.Count;
            for (int i = 0; i < titles.Count; i++)
            {
                Rect rect2 = rect.CutFromLeft(num * (float)i).LeftPart(num);
                if (tips != null && i < tips.Count)
                {
                    Get.Tooltips.RegisterTip(rect2, tips[i], null);
                }
                if (selected == i)
                {
                    Rect rect3 = rect2;
                    string text = titles[i];
                    bool flag = true;
                    Color? color = new Color?(Widgets.ActiveTabColor);
                    Texture2D texture2D = ((icons != null && i < icons.Count) ? icons[i] : null);
                    Widgets.Button(rect3, text, flag, color, null, true, true, true, true, texture2D, true, true, null, false, null, null);
                }
                else
                {
                    Rect rect4 = rect2;
                    string text2 = titles[i];
                    bool flag2 = true;
                    Texture2D texture2D = ((icons != null && i < icons.Count) ? icons[i] : null);
                    if (Widgets.Button(rect4, text2, flag2, null, null, true, true, true, true, texture2D, false, true, null, false, null, null))
                    {
                        selected = i;
                    }
                }
            }
        }

        public static float GetLinesHeight(int lines)
        {
            if (lines > 0)
            {
                return (float)lines * Widgets.LineHeight + (float)(lines - 1) * Widgets.SpaceBetweenLines;
            }
            return 0f;
        }

        public static void DoCheck(Rect rect, bool on, Color color)
        {
            if (on)
            {
                GUI.color = color * new Color(0.2f, 1f, 0.2f);
                GUIExtra.DrawTexture(rect, Widgets.CheckOn);
            }
            else
            {
                GUI.color = color * new Color(1f, 0.2f, 0.2f);
                GUIExtra.DrawTexture(rect, Widgets.CheckOff);
            }
            GUI.color = Color.white;
        }

        public static void AbsorbClicks(Rect rect)
        {
            if (Event.current.type == EventType.MouseDown && Mouse.Over(rect))
            {
                Event.current.Use();
            }
        }

        public static bool DrawnTextFieldThisOrLastFrame(int textFieldID)
        {
            for (int i = 0; i < Widgets.lastTextFieldDrawFrames.Count; i++)
            {
                if (Widgets.lastTextFieldDrawFrames[i].Item1 == textFieldID)
                {
                    return Widgets.lastTextFieldDrawFrames[i].Item2 >= Clock.Frame - 1;
                }
            }
            return false;
        }

        public static Rect TableRowL(string label, string text, float x, ref float curY, List<string> allLabels)
        {
            float num = 0f;
            if (allLabels != null)
            {
                for (int i = 0; i < allLabels.Count; i++)
                {
                    if (!allLabels[i].NullOrEmpty())
                    {
                        num = Math.Max(num, Widgets.CalcSize(allLabels[i]).x);
                    }
                }
            }
            num = Math.Max(num, Widgets.CalcSize(label).x);
            Rect rect = Widgets.LabelCenteredV(new Vector2(x, curY), label, true, null, null, false);
            Rect rect2 = Widgets.LabelCenteredV(new Vector2(x + num + 9f, curY), text, true, null, null, false);
            Rect rect3 = new Rect(rect.x, rect.y, rect2.xMax - rect.x, rect2.height).ContractedBy(0f, 1f);
            curY += 20f;
            GUIExtra.DrawHighlightIfMouseover(rect3, true, true, true, true, true);
            return rect3;
        }

        [CompilerGenerated]
        internal static Rect<Label> g__GetBoundingRect|97_0(ref Widgets.<>c__DisplayClass97_0 A_0)
		{
			if (A_0.boundingRectCached == null)
			{
				A_0.boundingRectCached = new Rect? (Widgets.CalcTrimmedBounds(A_0.label, A_0.rect).ExpandedBy(3f));
			}
			return A_0.boundingRectCached.Value;
		}

private static float virtualWidth = (float)Sys.Resolution.x;

private static float virtualHeight = (float)Sys.Resolution.y;

private static GUISkin skin;

private static GUIContent tmpGUIContent;

private static float uiScaleFactor = 1f;

private static float fontScale = 1f;

private static Dictionary<int, int> fontSizeToFitCache = new Dictionary<int, int>(250);

private static Dictionary<int, float> fontHeightCache = new Dictionary<int, float>(250);

private static List<Widgets.ActiveScrollView> activeScrollViews = new List<Widgets.ActiveScrollView>(10);

private static List<int> scrollViewsStack = new List<int>(10);

private static ValueTuple<Window, Rect>? lastHoverButton;

private static int lastHoverButtonFrame = -1;

private static float prevAppliedUIScale;

private static Matrix4x4 cachedGUIMatrix;

private static Rect draggingSlider;

private static float draggingSliderMouseOffset;

private static float lastKnownResolutionWidth;

private static float lastKnownResolutionHeight;

private static List<ValueTuple<Window, Rect, int, float>> accumulatedHovers = new List<ValueTuple<Window, Rect, int, float>>();

private static List<ValueTuple<int, int>> lastTextFieldDrawFrames = new List<ValueTuple<int, int>>();

public const int DefaultFontSize = 15;

private const int MinVisibleFontSize = 8;

private const float DefaultUIScalePer1080 = 0.9f;

public const float ScrollBarWidth = 15f;

public const int LabelTipExpandedBy = 3;

private const float SmoothScrollSpeed = 1.95f;

public const float CheckboxSize = 26f;

public const float RadioSize = 26f;

private static readonly Texture2D CursorTex = Assets.Get<Texture2D>("Textures/UI/Cursor");

private static readonly Texture2D CheckOn = Assets.Get<Texture2D>("Textures/UI/CheckOn");

private static readonly Texture2D CheckOff = Assets.Get<Texture2D>("Textures/UI/CheckOff");

private static readonly GUIStyle EmptyStyle = new GUIStyle();

public static readonly Color DefaultButtonColor = new Color(0.184f, 0.184f, 0.184f);

public static readonly Color ActiveTabColor = new Color(0.59f, 0.54f, 0.43f);

private static readonly float Resolution1080Diagonal = Calc.Sqrt(4852800f);

private struct ActiveScrollView
{
    public Rect rect;

    public Vector2 scrollPos;

    public Vector2 smoothScrollPos_start;

    public Vector2 smoothScrollPos_end;

    public float smoothScrollPos_animation;

    public int lastRegisteredFrame;

    public int firstRegisteredFrame;

    public float height;

    public int? uniqueID;

    public Rect lastViewRect;
}
	}
}