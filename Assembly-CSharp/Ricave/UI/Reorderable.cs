using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class Reorderable
    {
        public static ValueTuple<T, int>? RegisterVerticalReorderable<T>(T obj, Rect rect, IEnumerable<T> all, DragAndDrop.DragSource source = DragAndDrop.DragSource.Unspecified, Vector2? customSize = null)
        {
            ValueTuple<T, Reorderable.ReorderPlace>? valueTuple = Reorderable.RegisterVerticalReorderable<T>(obj, rect, source, customSize);
            if (valueTuple == null)
            {
                return null;
            }
            int num = all.IndexOf(obj);
            if (num < 0)
            {
                return null;
            }
            int num2 = all.IndexOf(valueTuple.Value.Item1);
            if (num2 < 0)
            {
                return null;
            }
            if (num == num2)
            {
                return null;
            }
            if (valueTuple.Value.Item2 == Reorderable.ReorderPlace.Before)
            {
                return new ValueTuple<T, int>?(new ValueTuple<T, int>(valueTuple.Value.Item1, (num2 < num) ? (num - 1) : num));
            }
            return new ValueTuple<T, int>?(new ValueTuple<T, int>(valueTuple.Value.Item1, (num2 > num) ? (num + 1) : num));
        }

        public static ValueTuple<T, Reorderable.ReorderPlace>? RegisterVerticalReorderable<T>(T obj, Rect rect, DragAndDrop.DragSource source = DragAndDrop.DragSource.Unspecified, Vector2? customSize = null)
        {
            Get.DragAndDrop.RegisterDraggable<T>(obj, rect, source, customSize);
            DragAndDrop dragAndDrop = Get.DragAndDrop;
            Rect rect2 = new Rect(rect.x, rect.y, rect.width, 0f).ExpandedBy(rect.height / 2f);
            if (dragAndDrop.HoveringDragged<T>(rect2, null))
            {
                GUIExtra.DrawHorizontalLine(rect.position, rect.width, Color.white, 1f);
                GUIExtra.DrawHorizontalLine(rect.position + Vector2.up, rect.width, Color.white, 1f);
            }
            Rect rect3 = new Rect(rect.x, rect.yMax, rect.width, 0f).ExpandedBy(rect.height / 2f);
            if (dragAndDrop.HoveringDragged<T>(rect3, null))
            {
                Vector2 vector = new Vector2(rect.x, rect.yMax);
                GUIExtra.DrawHorizontalLine(vector, rect.width, Color.white, 1f);
                GUIExtra.DrawHorizontalLine(vector + Vector2.up, rect.width, Color.white, 1f);
            }
            T t = dragAndDrop.ConsumeDropped<T>(rect2, null);
            if (t != null)
            {
                return new ValueTuple<T, Reorderable.ReorderPlace>?(new ValueTuple<T, Reorderable.ReorderPlace>(t, Reorderable.ReorderPlace.Before));
            }
            T t2 = dragAndDrop.ConsumeDropped<T>(rect3, null);
            if (t2 != null)
            {
                return new ValueTuple<T, Reorderable.ReorderPlace>?(new ValueTuple<T, Reorderable.ReorderPlace>(t2, Reorderable.ReorderPlace.After));
            }
            return null;
        }

        public enum ReorderPlace
        {
            Before,

            After
        }
    }
}