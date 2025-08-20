using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class GUIExtra
    {
        public static Texture2D GradientTexture
        {
            get
            {
                if (GUIExtra.gradientTex == null)
                {
                    GUIExtra.gradientTex = new Texture2D(2, 2);
                    GUIExtra.gradientTex.filterMode = FilterMode.Bilinear;
                    GUIExtra.gradientTex.wrapMode = TextureWrapMode.Clamp;
                }
                GUIExtra.gradientTex.SetPixel(0, 0, Color.HSVToRGB(Calc.PingPong(Clock.UnscaledTime / 10f, 0.2f) + 0.6f, 0.78f, 0.25f));
                GUIExtra.gradientTex.SetPixel(1, 0, Color.HSVToRGB(Calc.PingPong((Clock.UnscaledTime + 0.4f) / 10f, 0.2f) + 0.6f, 0.78f, 0.25f));
                GUIExtra.gradientTex.SetPixel(0, 1, Color.HSVToRGB(Calc.PingPong((Clock.UnscaledTime + 0.8f) / 10f, 0.2f) + 0.6f, 0.78f, 0.25f));
                GUIExtra.gradientTex.SetPixel(1, 1, Color.HSVToRGB(Calc.PingPong((Clock.UnscaledTime + 1.2f) / 10f, 0.2f) + 0.6f, 0.78f, 0.25f + Calc.PingPong(Clock.UnscaledTime / 6.2f, 0.1f)));
                GUIExtra.gradientTex.Apply();
                return GUIExtra.gradientTex;
            }
        }

        public static bool AnyTextFieldIsFocused
        {
            get
            {
                TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                return textEditor != null && textEditor.position != default(Rect) && GUIUtility.keyboardControl != 0 && Widgets.DrawnTextFieldThisOrLastFrame(textEditor.controlID);
            }
        }

        public static void OnGUI()
        {
            if (GUIExtra.repaintOffsetStack.Count != 0)
            {
                Log.Error("repaintOffsetStack is not empty. Clearing.", false);
                GUIExtra.repaintOffsetStack.Clear();
            }
        }

        public static void DrawTexture(Rect rect, Texture texture)
        {
            rect.position += CachedGUI.RepaintOffset;
            GUI.DrawTexture(rect, texture);
        }

        public static void DrawTexture(Vector2 center, float width, Texture texture)
        {
            GUIExtra.DrawTexture(RectUtility.CenteredAt(center, width, width * ((float)texture.height / (float)texture.width)), texture);
        }

        public static void DrawTexture(Rect rect, Texture texture, Material material)
        {
            if (Event.current.type == EventType.Repaint)
            {
                rect.position += CachedGUI.RepaintOffset;
                Graphics.DrawTexture(rect, texture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, GUI.color * 0.5f, material);
            }
        }

        public static void DrawTextureWithTexCoords(Rect rect, Texture texture, Rect texCoords)
        {
            rect.position += CachedGUI.RepaintOffset;
            GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
        }

        public static void DrawTextureRotated(Rect rect, Texture texture, float angle, Vector2? pivot = null)
        {
            rect.position += CachedGUI.RepaintOffset;
            if (angle != 0f)
            {
                if (pivot != null)
                {
                    pivot += CachedGUI.RepaintOffset;
                }
                Matrix4x4 matrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(angle, (pivot ?? rect.center) * Widgets.UIScale);
                GUI.DrawTexture(rect, texture);
                GUI.matrix = matrix;
                SteamDeckUtility.CheckFixMousePosition();
                return;
            }
            GUI.DrawTexture(rect, texture);
        }

        public static void DrawTextureWithTexCoordsRotated(Rect rect, Texture texture, Rect texCoords, float angle, Vector2? pivot = null)
        {
            rect.position += CachedGUI.RepaintOffset;
            if (angle != 0f)
            {
                if (pivot != null)
                {
                    pivot += CachedGUI.RepaintOffset;
                }
                Matrix4x4 matrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(angle, (pivot ?? rect.center) * Widgets.UIScale);
                GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
                GUI.matrix = matrix;
                SteamDeckUtility.CheckFixMousePosition();
                return;
            }
            GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
        }

        public static void DrawTextureXFlipped(Rect rect, Texture texture)
        {
            GUIExtra.DrawTextureWithTexCoords(rect, texture, new Rect(1f, 0f, -1f, 1f));
        }

        public static void DrawTextureYFlipped(Rect rect, Texture texture)
        {
            GUIExtra.DrawTextureWithTexCoords(rect, texture, new Rect(0f, 1f, 1f, -1f));
        }

        public static void DrawMultipartTexture(Rect rect, Texture2D atlas, float edgeSize = 15f, float atlasEdgeUVSize = 0.25f, bool drawCenter = true)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            edgeSize = Math.Min(edgeSize, rect.width / 2f);
            edgeSize = Math.Min(edgeSize, rect.height / 2f);
            if (drawCenter)
            {
                GUIExtra.DrawTextureWithTexCoords(rect.ContractedBy(edgeSize), atlas, new Rect(atlasEdgeUVSize, atlasEdgeUVSize, 1f - atlasEdgeUVSize * 2f, 1f - atlasEdgeUVSize * 2f));
            }
            GUIExtra.DrawTextureWithTexCoords(rect.TopPart(edgeSize).ContractedBy(edgeSize, 0f), atlas, new Rect(atlasEdgeUVSize, 1f - atlasEdgeUVSize, 1f - atlasEdgeUVSize * 2f, atlasEdgeUVSize));
            GUIExtra.DrawTextureWithTexCoords(rect.BottomPart(edgeSize).ContractedBy(edgeSize, 0f), atlas, new Rect(atlasEdgeUVSize, 0f, 1f - atlasEdgeUVSize * 2f, atlasEdgeUVSize));
            GUIExtra.DrawTextureWithTexCoords(rect.LeftPart(edgeSize).ContractedBy(0f, edgeSize), atlas, new Rect(0f, atlasEdgeUVSize, atlasEdgeUVSize, 1f - atlasEdgeUVSize * 2f));
            GUIExtra.DrawTextureWithTexCoords(rect.RightPart(edgeSize).ContractedBy(0f, edgeSize), atlas, new Rect(1f - atlasEdgeUVSize, atlasEdgeUVSize, atlasEdgeUVSize, 1f - atlasEdgeUVSize * 2f));
            GUIExtra.DrawTextureWithTexCoords(rect.TopPart(edgeSize).LeftPart(edgeSize), atlas, new Rect(0f, 1f - atlasEdgeUVSize, atlasEdgeUVSize, atlasEdgeUVSize));
            GUIExtra.DrawTextureWithTexCoords(rect.TopPart(edgeSize).RightPart(edgeSize), atlas, new Rect(1f - atlasEdgeUVSize, 1f - atlasEdgeUVSize, atlasEdgeUVSize, atlasEdgeUVSize));
            GUIExtra.DrawTextureWithTexCoords(rect.BottomPart(edgeSize).LeftPart(edgeSize), atlas, new Rect(0f, 0f, atlasEdgeUVSize, atlasEdgeUVSize));
            GUIExtra.DrawTextureWithTexCoords(rect.BottomPart(edgeSize).RightPart(edgeSize), atlas, new Rect(1f - atlasEdgeUVSize, 0f, atlasEdgeUVSize, atlasEdgeUVSize));
        }

        public static void DrawHighlight(Rect rect, bool tl = true, bool tr = true, bool br = true, bool bl = true, float strength = 1f)
        {
            GUIExtra.DrawRoundedRect(rect, new Color(1f, 1f, 1f, 0.1f * strength), tl, tr, br, bl, null);
        }

        public static void DrawHighlightIfMouseover(Rect rect, bool animated = true, bool tl = true, bool tr = true, bool br = true, bool bl = true)
        {
            if (animated)
            {
                float num = Widgets.AccumulatedHover(rect, false);
                if (num > 0f)
                {
                    GUIExtra.DrawRoundedRect(rect, new Color(1f, 1f, 1f, 0.1f * num), tl, tr, br, bl, null);
                    return;
                }
            }
            else if (Mouse.Over(rect))
            {
                GUIExtra.DrawRoundedRect(rect, new Color(1f, 1f, 1f, 0.1f), tl, tr, br, bl, null);
            }
        }

        public static Color HighlightedColorIfMouseover(Rect rect, Color color, bool animated = true, float strength = 0.15f)
        {
            if (animated)
            {
                float num = Widgets.AccumulatedHover(rect, false);
                return color.Lighter(strength * num);
            }
            if (Mouse.Over(rect))
            {
                return color.Lighter(strength);
            }
            return color;
        }

        public static void DrawHighlightRotated(Rect rect, float angle)
        {
            GUIExtra.CheckCreateWhiteTex();
            GUI.color = new Color(1f, 1f, 1f, 0.1f);
            GUIExtra.DrawTextureRotated(rect, GUIExtra.whiteTex, angle, null);
            GUI.color = Color.white;
        }

        public static void DrawRect(Rect rect, Color color)
        {
            GUIExtra.CheckCreateWhiteTex();
            rect.position += CachedGUI.RepaintOffset;
            Color color2 = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, GUIExtra.whiteTex);
            GUI.color = color2;
        }

        public static void DrawRoundedRect(Rect rect, Color color, bool tl = true, bool tr = true, bool br = true, bool bl = true, Texture customTexture = null)
        {
            if (customTexture == null)
            {
                GUIExtra.CheckCreateWhiteTex();
                customTexture = GUIExtra.whiteTex;
            }
            rect.position += CachedGUI.RepaintOffset;
            float num = Math.Max(rect.width, rect.height) / 2f + 1f;
            GUI.DrawTexture(rect, customTexture, ScaleMode.StretchToFill, true, 0f, color, new Vector4(num, num, num, num), new Vector4(10f * (tl ? 1f : 0f), 10f * (tr ? 1f : 0f), 10f * (br ? 1f : 0f), 10f * (bl ? 1f : 0f)));
        }

        public static void DrawRectWithOutline(Rect rect, Color color, Color outlineColor)
        {
            if (color.a >= 1f)
            {
                GUIExtra.DrawRect(rect, outlineColor);
            }
            GUIExtra.DrawRect(rect.ContractedBy(1f), color);
            if (color.a < 1f)
            {
                GUIExtra.DrawRectOutline(rect, outlineColor, 1f);
            }
        }

        public static void DrawRoundedRectWithOutline(Rect rect, Color color, Color outlineColor, bool tl = true, bool tr = true, bool br = true, bool bl = true, Texture customTexture = null)
        {
            GUIExtra.DrawRoundedRect(rect, color, tl, tr, br, bl, customTexture);
            GUIExtra.DrawRoundedRectOutline(rect, outlineColor, 1f, tl, tr, br, bl);
        }

        public static void DrawRoundedRectWithDoubleOutline(Rect rect, Color color, Color innerOutlineColor, Color outerOutlineColor, bool tl = true, bool tr = true, bool br = true, bool bl = true, Texture customTexture = null)
        {
            GUIExtra.DrawRoundedRect(rect, color, tl, tr, br, bl, customTexture);
            GUIExtra.DrawRoundedRectOutline(rect.ContractedBy(1f), innerOutlineColor, 1f, tl, tr, br, bl);
            GUIExtra.DrawRoundedRectOutline(rect, outerOutlineColor, 1f, tl, tr, br, bl);
        }

        public static void DrawRectBump(Rect rect, Color color, bool inverted = false)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            GUIExtra.CheckCreateWhiteTex();
            if (color.a >= 1f)
            {
                Color color2 = color.Lighter(0.2f);
                Color color3 = color.Darker(0.4f);
                if (inverted)
                {
                    Calc.Swap<Color>(ref color2, ref color3);
                }
                Color color4 = GUI.color;
                GUI.color = color3;
                Rect rect2 = rect;
                rect2.position += CachedGUI.RepaintOffset;
                GUI.DrawTexture(rect2, GUIExtra.whiteTex);
                GUI.color = color;
                Rect rect3 = rect;
                rect3.position += CachedGUI.RepaintOffset;
                float num = rect3.xMin;
                rect3.xMin = num + 1f;
                num = rect3.xMax;
                rect3.xMax = num - 1f;
                num = rect3.yMax;
                rect3.yMax = num - 1f;
                GUI.DrawTexture(rect3, GUIExtra.whiteTex);
                GUI.color = color4;
                GUIExtra.DrawHorizontalLine(rect.position + new Vector2(1f, 0f), rect.width - 2f, color2, 1f);
                return;
            }
            Color color5 = GUI.color;
            GUI.color = color;
            Rect rect4 = rect;
            rect4.position += CachedGUI.RepaintOffset;
            GUI.DrawTexture(rect4, GUIExtra.whiteTex);
            GUI.color = color5;
            GUIExtra.DrawRectBumpOutline(rect, color, inverted);
        }

        public static void DrawRoundedRectBump(Rect rect, Color color, bool inverted = false, bool tl = true, bool tr = true, bool br = true, bool bl = true, Texture customTexture = null)
        {
            if (inverted)
            {
                GUIExtra.DrawRoundedRectWithOutline(rect, color, color.Lighter(0.2f), tl, tr, br, bl, customTexture);
                return;
            }
            GUIExtra.DrawRoundedRectWithDoubleOutline(rect, color, color.Lighter(0.2f), color.Darker(0.4f), tl, tr, br, bl, customTexture);
        }

        public static void DrawRectBumpOutline(Rect rect, Color color, bool inverted = false)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            Color color2 = color.Lighter(0.2f);
            Color color3 = color.Darker(0.4f);
            if (inverted)
            {
                Calc.Swap<Color>(ref color2, ref color3);
            }
            GUIExtra.DrawHorizontalLine(rect.position, rect.width, color2, 1f);
            GUIExtra.DrawVerticalLine(new Vector2(rect.xMax - 1f, rect.y), rect.height, color3, 1f);
            GUIExtra.DrawHorizontalLine(new Vector2(rect.x, rect.yMax - 1f), rect.width, color3, 1f);
            GUIExtra.DrawVerticalLine(rect.position, rect.height, color3, 1f);
        }

        public static void DrawRectOutline(Rect rect, Color color, float width = 1f)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            GUIExtra.DrawHorizontalLine(rect.position, rect.width, color, width);
            GUIExtra.DrawVerticalLine(new Vector2(rect.xMax - 1f, rect.y), rect.height, color, width);
            GUIExtra.DrawHorizontalLine(new Vector2(rect.x, rect.yMax - 1f), rect.width, color, width);
            GUIExtra.DrawVerticalLine(rect.position, rect.height, color, width);
        }

        public static void DrawRoundedRectOutline(Rect rect, Color color, float width = 1f, bool tl = true, bool tr = true, bool br = true, bool bl = true)
        {
            GUIExtra.CheckCreateWhiteTex();
            rect.position += CachedGUI.RepaintOffset;
            GUI.DrawTexture(rect, GUIExtra.whiteTex, ScaleMode.StretchToFill, true, 0f, color, new Vector4(width, width, width, width), new Vector4(10f * (tl ? 1f : 0f), 10f * (tr ? 1f : 0f), 10f * (br ? 1f : 0f), 10f * (bl ? 1f : 0f)));
        }

        public static void DrawCircle(Vector2 pos, float radius, Color color)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            GUIExtra.CheckCreateWhiteTex();
            pos += CachedGUI.RepaintOffset;
            float num = radius + 1f;
            GUI.DrawTexture(new Rect(pos.x - radius, pos.y - radius, radius * 2f, radius * 2f), GUIExtra.whiteTex, ScaleMode.StretchToFill, true, 0f, color, new Vector4(num, num, num, num), new Vector4(999f, 999f, 999f, 999f));
        }

        public static void DrawHorizontalLine(Vector2 pos, float length, Color color, float width = 1f)
        {
            GUIExtra.CheckCreateWhiteTex();
            pos += CachedGUI.RepaintOffset;
            Color color2 = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(pos.x, pos.y - (width - 1f) / 2f, length, width), GUIExtra.whiteTex);
            GUI.color = color2;
        }

        public static void DrawVerticalLine(Vector2 pos, float length, Color color, float width = 1f)
        {
            GUIExtra.CheckCreateWhiteTex();
            pos += CachedGUI.RepaintOffset;
            Color color2 = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(pos.x - (width - 1f) / 2f, pos.y, width, length), GUIExtra.whiteTex);
            GUI.color = color2;
        }

        public static void DrawLine(Vector2 a, Vector2 b, Color color, float width = 1f)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            GUIExtra.CheckCreateWhiteTex();
            Vector2 vector = b - a;
            float magnitude = vector.magnitude;
            if (magnitude < 0.001f)
            {
                return;
            }
            Vector2 vector2 = Vector2.Perpendicular(vector / magnitude);
            a -= vector2 * (width / 2f);
            b -= vector2 * (width / 2f);
            float num = -Calc.Atan2(-vector.y, vector.x) * 57.29578f;
            Matrix4x4 matrix4x = Matrix4x4.TRS(a, Quaternion.Euler(0f, 0f, num), Vector3.one) * Matrix4x4.Translate(-a);
            Rect rect = new Rect(a.x, a.y, magnitude, width);
            rect.position += CachedGUI.RepaintOffset;
            Color color2 = GUI.color;
            GUI.color = color;
            GL.PushMatrix();
            GL.MultMatrix(matrix4x);
            GUI.DrawTexture(rect, GUIExtra.whiteTex);
            GL.PopMatrix();
            GUI.color = color2;
        }

        public static void DrawHorizontalLineBump(Vector2 pos, float length, Color color)
        {
            GUIExtra.DrawHorizontalLine(pos, length, color, 1f);
            GUIExtra.DrawHorizontalLine(pos + new Vector2(0f, 1f), length, color.Darker(0.25f), 1f);
        }

        public static void DrawVerticalLineBump(Vector2 pos, float length, Color color)
        {
            GUIExtra.DrawVerticalLine(pos, length, color, 1f);
            GUIExtra.DrawVerticalLine(pos + new Vector2(1f, 0f), length, color.Darker(0.25f), 1f);
        }

        public static void DrawShadowAround(Rect rect, float alpha = 0.15f, float size = 25f, bool drawCenter = false)
        {
            Color color = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, alpha);
            GUIExtra.DrawMultipartTexture(rect.ExpandedBy(size - 1f), GUIExtra.ShadowTex, size * 1.2f, 0.25f, drawCenter);
            GUI.color = color;
        }

        public static void DrawLessonHint(Rect rect)
        {
            GUIExtra.DrawRoundedRect(rect, new Color(1f, 0.35f, 0.35f, 0.6f * (0.25f + (Calc.Sin(Clock.UnscaledTime * 4f) + 1f) / 2f * 0.75f)), true, true, true, true, null);
        }

        private static void CheckCreateWhiteTex()
        {
            if (GUIExtra.whiteTex == null)
            {
                GUIExtra.whiteTex = new Texture2D(2, 2);
                GUIExtra.whiteTex.filterMode = FilterMode.Point;
                GUIExtra.whiteTex.SetPixel(0, 0, Color.white);
                GUIExtra.whiteTex.SetPixel(1, 0, Color.white);
                GUIExtra.whiteTex.SetPixel(0, 1, Color.white);
                GUIExtra.whiteTex.SetPixel(1, 1, Color.white);
                GUIExtra.whiteTex.Apply();
            }
        }

        public static void DeselectText(int textLength)
        {
            TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            if (textEditor != null)
            {
                textEditor.OnFocus();
                textEditor.selectIndex = textLength;
                textEditor.cursorIndex = textLength;
            }
        }

        public static Vector2 GUIToScreenPoint(this Vector2 point)
        {
            return GUIUtility.GUIToScreenPoint(point / Widgets.UIScale);
        }

        public static void BeginRenderTexture(RenderTexture renderTexture)
        {
            if (renderTexture == null)
            {
                return;
            }
            if (Event.current.type == EventType.Repaint)
            {
                RenderTexture.active = renderTexture;
                Widgets.ApplyUIScale();
                GL.Clear(false, true, new Color(0f, 0f, 0f, 0f));
            }
        }

        public static void EndRenderTexture()
        {
            if (Event.current.type == EventType.Repaint)
            {
                RenderTexture.active = null;
                Widgets.ApplyUIScale();
            }
        }

        public static void BeginGroup(Rect rect)
        {
            if (RenderTexture.active != null)
            {
                GUIExtra.repaintOffsetStack.Add(CachedGUI.RepaintOffset);
                CachedGUI.RepaintOffset += rect.position;
                return;
            }
            GUI.BeginGroup(rect);
            SteamDeckUtility.CheckFixMousePosition();
        }

        public static void EndGroup()
        {
            if (RenderTexture.active != null)
            {
                CachedGUI.RepaintOffset = GUIExtra.repaintOffsetStack[GUIExtra.repaintOffsetStack.Count - 1];
                GUIExtra.repaintOffsetStack.RemoveAt(GUIExtra.repaintOffsetStack.Count - 1);
                return;
            }
            GUI.EndGroup();
            SteamDeckUtility.CheckFixMousePosition();
        }

        public static void DrawPie(Vector2 pos, float radius, float startAngle, float angleSpan, float innerRadius = 0f)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            Rect rect = RectUtility.CenteredAt(pos, radius * 2f);
            GUIExtra.CheckCreateWhiteTex();
            GUIExtra.PieMaterial.color = GUI.color;
            GUIExtra.PieMaterial.SetFloat(Get.ShaderPropertyIDs.StartAngleID, Calc.NormalizeDir(startAngle));
            GUIExtra.PieMaterial.SetFloat(Get.ShaderPropertyIDs.AngleSpanID, Calc.Clamp(angleSpan, 0f, 360f));
            GUIExtra.PieMaterial.SetFloat(Get.ShaderPropertyIDs.InnerRadiusPctID, Calc.Clamp01(innerRadius / radius));
            GUIExtra.DrawTexture(rect, GUIExtra.whiteTex, GUIExtra.PieMaterial);
        }

        private static Texture2D whiteTex;

        private static Texture2D gradientTex;

        private static List<Vector2> repaintOffsetStack = new List<Vector2>();

        public const float RectBumpStrength_Darker = 0.4f;

        public const float RectBumpStrength_Lighter = 0.2f;

        private const float RoundedCornersRadius = 10f;

        private static readonly Texture2D ShadowTex = Assets.Get<Texture2D>("Textures/UI/Shadow");

        private static readonly Material PieMaterial = Assets.Get<Material>("Materials/UI/Pie");
    }
}