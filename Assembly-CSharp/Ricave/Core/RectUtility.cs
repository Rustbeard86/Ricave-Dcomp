using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public static class RectUtility
    {
        public static Rect AtZero(this Rect rect)
        {
            return new Rect(0f, 0f, rect.width, rect.height);
        }

        public static Rect MovedBy(this Rect rect, Vector2 offset)
        {
            return new Rect(rect.x + offset.x, rect.y + offset.y, rect.width, rect.height);
        }

        public static Rect MovedBy(this Rect rect, float x, float y)
        {
            return new Rect(rect.x + x, rect.y + y, rect.width, rect.height);
        }

        public static Rect MovedByPct(this Rect rect, float xPct, float yPct)
        {
            return new Rect(rect.x + xPct * rect.width, rect.y + yPct * rect.height, rect.width, rect.height);
        }

        public static bool Empty(this Rect rect)
        {
            return rect.Area() <= 0f;
        }

        public static float Area(this Rect rect)
        {
            return rect.width * rect.height;
        }

        public static Rect ResizedTo(this Rect rect, float size)
        {
            return rect.ResizedTo(size, size);
        }

        public static Rect ResizedTo(this Rect rect, float width, float height)
        {
            Rect rect2 = rect;
            rect2.width = width;
            rect2.height = height;
            rect2.center = rect.center;
            return rect2;
        }

        public static Rect ResizedToWidth(this Rect rect, float width)
        {
            return rect.ResizedTo(width, rect.height);
        }

        public static Rect ResizedToHeight(this Rect rect, float height)
        {
            return rect.ResizedTo(rect.width, height);
        }

        public static Rect ResizedToPct(this Rect rect, float pct)
        {
            return rect.ResizedToPct(pct, pct);
        }

        public static Rect ResizedToPct(this Rect rect, float widthPct, float heightPct)
        {
            return rect.ResizedTo(rect.width * widthPct, rect.height * heightPct);
        }

        public static Rect ExpandedBy(this Rect rect, float padding)
        {
            return rect.ExpandedBy(padding, padding);
        }

        public static Rect ExpandedBy(this Rect rect, float widthPadding, float heightPadding)
        {
            return rect.ResizedTo(rect.width + widthPadding * 2f, rect.height + heightPadding * 2f);
        }

        public static Rect ExpandedBy(this Rect rect, float top, float right, float bottom, float left)
        {
            rect.yMin -= top;
            rect.xMax += right;
            rect.yMax += bottom;
            rect.xMin -= left;
            return rect;
        }

        public static Rect ExpandedByPct(this Rect rect, float paddingPct)
        {
            return rect.ExpandedByPct(paddingPct, paddingPct);
        }

        public static Rect ExpandedByPct(this Rect rect, float widthPaddingPct, float heightPaddingPct)
        {
            return rect.ResizedTo(rect.width + widthPaddingPct * rect.width * 2f, rect.height + heightPaddingPct * rect.height * 2f);
        }

        public static Rect ContractedBy(this Rect rect, float padding)
        {
            return rect.ContractedBy(padding, padding);
        }

        public static Rect ContractedBy(this Rect rect, float widthPadding, float heightPadding)
        {
            return rect.ResizedTo(rect.width - widthPadding * 2f, rect.height - heightPadding * 2f);
        }

        public static Rect ContractedByPct(this Rect rect, float paddingPct)
        {
            return rect.ContractedByPct(paddingPct, paddingPct);
        }

        public static Rect ContractedByPct(this Rect rect, float widthPaddingPct, float heightPaddingPct)
        {
            return rect.ResizedTo(rect.width - widthPaddingPct * rect.width * 2f, rect.height - heightPaddingPct * rect.height * 2f);
        }

        public static Rect CenteredAt(Vector2 pos, float size)
        {
            return new Rect(pos.x - size / 2f, pos.y - size / 2f, size, size);
        }

        public static Rect CenteredAt(Vector2 pos, Vector2 size)
        {
            return RectUtility.CenteredAt(pos, size.x, size.y);
        }

        public static Rect CenteredAt(Vector2 pos, float width, float height)
        {
            return new Rect(pos.x - width / 2f, pos.y - height / 2f, width, height);
        }

        public static Rect CenteredAt(float x, float y, float width, float height)
        {
            return new Rect(x - width / 2f, y - height / 2f, width, height);
        }

        public static Rect RightPart(this Rect rect, float size)
        {
            if (size < 0f)
            {
                size = 0f;
            }
            if (rect.width <= size)
            {
                return rect;
            }
            Rect rect2 = rect;
            rect2.xMin = rect2.xMax - size;
            return rect2;
        }

        public static Rect CutFromLeft(this Rect rect, float toCut)
        {
            return rect.RightPart(rect.width - toCut);
        }

        public static Rect CutFromLeftPct(this Rect rect, float toCutPct)
        {
            return rect.RightPart(rect.width - toCutPct * rect.width);
        }

        public static Rect RightPartPct(this Rect rect, float pct)
        {
            return rect.RightPart(rect.width * pct);
        }

        public static Rect RightHalf(this Rect rect)
        {
            return rect.RightPart(rect.width * 0.5f);
        }

        public static Rect LeftPart(this Rect rect, float size)
        {
            if (size < 0f)
            {
                size = 0f;
            }
            if (rect.width <= size)
            {
                return rect;
            }
            Rect rect2 = rect;
            rect2.width = size;
            return rect2;
        }

        public static Rect CutFromRight(this Rect rect, float toCut)
        {
            return rect.LeftPart(rect.width - toCut);
        }

        public static Rect CutFromRightPct(this Rect rect, float toCutPct)
        {
            return rect.LeftPart(rect.width - toCutPct * rect.width);
        }

        public static Rect LeftPartPct(this Rect rect, float pct)
        {
            return rect.LeftPart(rect.width * pct);
        }

        public static Rect LeftHalf(this Rect rect)
        {
            return rect.LeftPart(rect.width * 0.5f);
        }

        public static Rect TopPart(this Rect rect, float size)
        {
            if (size < 0f)
            {
                size = 0f;
            }
            if (rect.height <= size)
            {
                return rect;
            }
            Rect rect2 = rect;
            rect2.height = size;
            return rect2;
        }

        public static Rect CutFromBottom(this Rect rect, float toCut)
        {
            return rect.TopPart(rect.height - toCut);
        }

        public static Rect CutFromBottomPct(this Rect rect, float toCutPct)
        {
            return rect.TopPart(rect.height - toCutPct * rect.height);
        }

        public static Rect TopPartPct(this Rect rect, float pct)
        {
            return rect.TopPart(rect.height * pct);
        }

        public static Rect TopHalf(this Rect rect)
        {
            return rect.TopPart(rect.height * 0.5f);
        }

        public static Rect BottomPart(this Rect rect, float size)
        {
            if (size < 0f)
            {
                size = 0f;
            }
            if (rect.height <= size)
            {
                return rect;
            }
            Rect rect2 = rect;
            rect2.yMin = rect2.yMax - size;
            return rect2;
        }

        public static Rect CutFromTop(this Rect rect, float toCut)
        {
            return rect.BottomPart(rect.height - toCut);
        }

        public static Rect CutFromTopPct(this Rect rect, float toCutPct)
        {
            return rect.BottomPart(rect.height - toCutPct * rect.height);
        }

        public static Rect BottomPartPct(this Rect rect, float pct)
        {
            return rect.BottomPart(rect.height * pct);
        }

        public static Rect BottomHalf(this Rect rect)
        {
            return rect.BottomPart(rect.height * 0.5f);
        }

        public static Vector2 UpperLeft(this Rect rect)
        {
            return rect.position;
        }

        public static Vector2 BottomLeft(this Rect rect)
        {
            return new Vector2(rect.x, rect.y + rect.height);
        }

        public static Vector2 UpperRight(this Rect rect)
        {
            return new Vector2(rect.x + rect.width, rect.y);
        }

        public static Vector2 BottomRight(this Rect rect)
        {
            return new Vector2(rect.x + rect.width, rect.y + rect.height);
        }

        public static Vector2 TopCenter(this Rect rect)
        {
            return new Vector2(rect.x + rect.width / 2f, rect.y);
        }

        public static Vector2 BottomCenter(this Rect rect)
        {
            return new Vector2(rect.x + rect.width / 2f, rect.y + rect.height);
        }

        public static Vector2 LeftCenter(this Rect rect)
        {
            return new Vector2(rect.x, rect.y + rect.height / 2f);
        }

        public static Vector2 RightCenter(this Rect rect)
        {
            return new Vector2(rect.x + rect.width, rect.y + rect.height / 2f);
        }

        public static Rect ContractedToSquare(this Rect rect)
        {
            return RectUtility.CenteredAt(rect.center, Math.Min(rect.width, rect.height));
        }

        public static Rect ScaledToCover_MaintainAspectRatio(this Rect rect, Vector2 minResultingSize)
        {
            return rect.ScaledToCover_MaintainAspectRatio(minResultingSize.x, minResultingSize.y);
        }

        public static Rect ScaledToCover_MaintainAspectRatio(this Rect rect, float minResultingWidth, float minResultingHeight)
        {
            float num = rect.width / rect.height;
            if (minResultingWidth / minResultingHeight > num)
            {
                return rect.ResizedTo(minResultingWidth, minResultingWidth / num);
            }
            return rect.ResizedTo(minResultingHeight * num, minResultingHeight);
        }

        public static Rect StackHorizontally(this Rect rect, int columnIndex, int maxColumns)
        {
            return rect.StackHorizontally(columnIndex, maxColumns, rect.width / (float)maxColumns);
        }

        public static Rect StackHorizontally(this Rect rect, int columnIndex, int maxColumns, float elementWidth)
        {
            if (maxColumns == 1)
            {
                return new Rect(rect.center.x - elementWidth / 2f, rect.y, elementWidth, rect.height);
            }
            float num = (rect.width - (float)maxColumns * elementWidth) / (float)(maxColumns - 1);
            return new Rect(rect.x + (float)columnIndex * (elementWidth + num), rect.y, elementWidth, rect.height);
        }

        public static Rect StackVertically(this Rect rect, int rowIndex, int maxRows)
        {
            return rect.StackVertically(rowIndex, maxRows, rect.height / (float)maxRows);
        }

        public static Rect StackVertically(this Rect rect, int rowIndex, int maxRows, float elementHeight)
        {
            if (maxRows == 1)
            {
                return new Rect(rect.x, rect.center.y - elementHeight / 2f, rect.width, elementHeight);
            }
            float num = (rect.height - (float)maxRows * elementHeight) / (float)(maxRows - 1);
            return new Rect(rect.x, rect.y + (float)rowIndex * (elementHeight + num), rect.width, elementHeight);
        }

        public static bool VisibleInScrollView(this Rect rect)
        {
            return Widgets.VisibleInScrollView(rect);
        }

        public static Rect BoundingBox(Rect a, Rect b, bool ignoreEmpty = true)
        {
            if (ignoreEmpty)
            {
                if (a.Empty())
                {
                    return b;
                }
                if (b.Empty())
                {
                    return a;
                }
            }
            float num = Math.Min(a.xMin, b.xMin);
            float num2 = Math.Min(a.yMin, b.yMin);
            float num3 = Math.Max(a.xMax, b.xMax);
            float num4 = Math.Max(a.yMax, b.yMax);
            return new Rect(num, num2, num3 - num, num4 - num2);
        }

        public static Rect BoundingBox(Vector2 a, Vector2 b)
        {
            float num = Math.Min(a.x, b.x);
            float num2 = Math.Min(a.y, b.y);
            float num3 = Math.Max(a.x, b.x);
            float num4 = Math.Max(a.y, b.y);
            return new Rect(num, num2, num3 - num, num4 - num2);
        }

        public static Vector2 TopLeftWorldPos(this Rect rect)
        {
            return new Vector2(rect.x, rect.yMax);
        }

        public static Vector2 TopRightWorldPos(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMax);
        }

        public static Vector2 BotRightWorldPos(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.y);
        }

        public static Vector2 BotLeftWorldPos(this Rect rect)
        {
            return new Vector2(rect.x, rect.y);
        }

        public static Vector2 TopLeftUIPos(this Rect rect)
        {
            return new Vector2(rect.x, rect.y);
        }

        public static Vector2 TopRightUIPos(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.y);
        }

        public static Vector2 BotRightUIPos(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMax);
        }

        public static Vector2 BotLeftUIPos(this Rect rect)
        {
            return new Vector2(rect.x, rect.yMax);
        }

        public static bool EqualsApproximately(this Rect a, Rect b, float? eps = null)
        {
            if (eps == null)
            {
                return Calc.Approximately(a.x, b.x) && Calc.Approximately(a.y, b.y) && Calc.Approximately(a.width, b.width) && Calc.Approximately(a.height, b.height);
            }
            float num = Math.Abs(a.x - b.x);
            float? num2 = eps;
            if ((num <= num2.GetValueOrDefault()) & (num2 != null))
            {
                float num3 = Math.Abs(a.y - b.y);
                num2 = eps;
                if ((num3 <= num2.GetValueOrDefault()) & (num2 != null))
                {
                    float num4 = Math.Abs(a.width - b.width);
                    num2 = eps;
                    if ((num4 <= num2.GetValueOrDefault()) & (num2 != null))
                    {
                        float num5 = Math.Abs(a.height - b.height);
                        num2 = eps;
                        return (num5 <= num2.GetValueOrDefault()) & (num2 != null);
                    }
                }
            }
            return false;
        }

        public static IEnumerable<Rect> GetPackedRects(Rect outerRect, int count, float eachRectSize, float spaceBetweenRects)
        {
            if (count <= 0)
            {
                yield break;
            }
            int num = Calc.FloorToInt(outerRect.width / (eachRectSize + spaceBetweenRects) * (outerRect.height / (eachRectSize + spaceBetweenRects)));
            if (count > num)
            {
                float num2 = (float)num / (float)count;
                eachRectSize *= num2;
                spaceBetweenRects *= num2;
            }
            int perRow = count;
            float num3 = 0f;
            for (int k = 0; k < count; k++)
            {
                float num4 = ((num3 == 0f) ? eachRectSize : (spaceBetweenRects + eachRectSize));
                if (num3 + num4 > outerRect.width)
                {
                    perRow = k;
                    break;
                }
                num3 += num4;
            }
            perRow = Math.Max(perRow, 1);
            int lastRow = count % perRow;
            int num5 = count / perRow;
            int rowCount = num5 + ((lastRow > 0) ? 1 : 0);
            float num6 = (float)rowCount * eachRectSize + (float)(rowCount - 1) * spaceBetweenRects;
            float curY = outerRect.center.y - num6 / 2f;
            int num8;
            for (int i = 0; i < rowCount; i = num8 + 1)
            {
                int perThisRow = ((i == rowCount - 1) ? ((lastRow == 0) ? perRow : lastRow) : perRow);
                float num7 = (float)perThisRow * eachRectSize + (float)(perThisRow - 1) * spaceBetweenRects;
                float curX = outerRect.center.x - num7 / 2f;
                for (int j = 0; j < perThisRow; j = num8 + 1)
                {
                    yield return new Rect(curX, curY, eachRectSize, eachRectSize);
                    curX += eachRectSize + spaceBetweenRects;
                    num8 = j;
                }
                curY += eachRectSize + spaceBetweenRects;
                num8 = i;
            }
            yield break;
        }
    }
}