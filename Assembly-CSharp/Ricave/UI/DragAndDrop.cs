using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class DragAndDrop
    {
        public object DraggedObject
        {
            get
            {
                if (!this.Dragging)
                {
                    return null;
                }
                return this.dragged;
            }
        }

        public bool InitiatedDragging
        {
            get
            {
                return this.dragged != null;
            }
        }

        public bool Dragging
        {
            get
            {
                return this.InitiatedDragging && this.startedDragging;
            }
        }

        public bool DraggingOrDroppedJustNow
        {
            get
            {
                return this.Dragging || this.droppedFrame == Clock.Frame;
            }
        }

        public bool IsDragging(object obj)
        {
            return this.Dragging && this.dragged == obj;
        }

        public void RegisterDraggable<T>(T obj, Rect rect, DragAndDrop.DragSource source = DragAndDrop.DragSource.Unspecified, Vector2? customSize = null)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Mouse.Over(rect) && obj != null)
            {
                this.InitiateDrag(obj, source, rect, customSize);
            }
        }

        public bool HoveringDragged<T>(Rect toRect, DragAndDrop.DragSource? requiredSource = null)
        {
            if (this.Dragging && this.dragged is T)
            {
                if (requiredSource != null)
                {
                    DragAndDrop.DragSource? dragSource = requiredSource;
                    DragAndDrop.DragSource dragSource2 = this.source;
                    if (!((dragSource.GetValueOrDefault() == dragSource2) & (dragSource != null)))
                    {
                        return false;
                    }
                }
                return Mouse.Over(toRect);
            }
            return false;
        }

        public T ConsumeDropped<T>(Rect toRect, DragAndDrop.DragSource? requiredSource = null)
        {
            object obj = this.dropped;
            if (obj is T)
            {
                T t = (T)((object)obj);
                if (requiredSource != null)
                {
                    DragAndDrop.DragSource? dragSource = requiredSource;
                    DragAndDrop.DragSource dragSource2 = this.droppedSource;
                    if (!((dragSource.GetValueOrDefault() == dragSource2) & (dragSource != null)))
                    {
                        goto IL_006A;
                    }
                }
                if (toRect.Contains(GUIUtility.ScreenToGUIPoint(this.droppedAt)) && Widgets.VisibleInScrollView(GUIUtility.ScreenToGUIPoint(this.droppedAt)))
                {
                    T t2 = t;
                    this.ClearDropped();
                    return t2;
                }
            }
        IL_006A:
            return default(T);
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.Repaint && Clock.Frame > this.droppedFrame)
            {
                this.ClearDropped();
            }
            if (!Input.GetMouseButton(0) && this.InitiatedDragging)
            {
                this.Drop();
            }
            if (this.InitiatedDragging && !this.startedDragging && Cursor.lockState == CursorLockMode.None && (Event.current.mousePosition - this.dragStartMousePos).sqrMagnitude >= 9f)
            {
                this.startedDragging = true;
                if (this.dragged is Item)
                {
                    Get.Sound_DragStartItem.PlayOneShot(null, 1f, 1f);
                }
                else
                {
                    Get.Sound_DragStart.PlayOneShot(null, 1f, 1f);
                }
                Get.MouseAttachmentDrawerGOC.CheckEnableDisable();
            }
        }

        public void DrawMouseAttachment()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (!this.Dragging)
            {
                return;
            }
            ITipSubject tipSubject = this.dragged as ITipSubject;
            if (tipSubject != null)
            {
                Rect rect = new Rect(Event.current.mousePosition.x - this.draggedOffset.x, Event.current.mousePosition.y - this.draggedOffset.y, this.draggedSize.x, this.draggedSize.y);
                GUI.color = tipSubject.IconColor;
                GUI.DrawTexture(rect, tipSubject.Icon);
                GUI.color = Color.white;
                return;
            }
            Mod mod = this.dragged as Mod;
            if (mod != null)
            {
                Widgets.LabelCentered(Event.current.mousePosition, mod.Info.Name, true, null, null, false, false, false, null);
            }
        }

        private void InitiateDrag(object obj, DragAndDrop.DragSource source, Rect fromRect, Vector2? customSize)
        {
            this.ClearDragged();
            this.dragged = obj;
            this.source = source;
            this.draggedOffset = ((customSize != null) ? new Vector2(0f, -30f) : (Event.current.mousePosition - fromRect.position));
            this.draggedSize = customSize ?? fromRect.size;
            this.dragStartMousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition / Widgets.UIScale);
            Get.MouseAttachmentDrawerGOC.CheckEnableDisable();
        }

        private void Drop()
        {
            if (!this.InitiatedDragging)
            {
                return;
            }
            if (this.Dragging)
            {
                this.dropped = this.dragged;
                this.droppedAt = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                this.droppedFrame = Clock.Frame;
                this.droppedSource = this.source;
                if (this.dropped is Item)
                {
                    Get.Sound_DragEndItem.PlayOneShot(null, 1f, 1f);
                }
                else
                {
                    Get.Sound_DragEnd.PlayOneShot(null, 1f, 1f);
                }
            }
            else
            {
                this.ClearDropped();
            }
            this.ClearDragged();
        }

        private void ClearDragged()
        {
            this.dragged = null;
            this.source = DragAndDrop.DragSource.Unspecified;
            this.draggedOffset = default(Vector2);
            this.draggedSize = default(Vector2);
            this.dragStartMousePos = default(Vector2);
            this.startedDragging = false;
            Get.MouseAttachmentDrawerGOC.CheckEnableDisable();
        }

        private void ClearDropped()
        {
            this.dropped = null;
            this.droppedAt = default(Vector2);
            this.droppedSource = DragAndDrop.DragSource.Unspecified;
        }

        public void Interrupt()
        {
            this.ClearDragged();
            this.ClearDropped();
        }

        public void OnCursorLockStateChanged()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                this.ClearDragged();
            }
        }

        private object dragged;

        private Vector2 draggedOffset;

        private Vector2 draggedSize;

        private Vector2 dragStartMousePos;

        private DragAndDrop.DragSource source;

        private bool startedDragging;

        private object dropped;

        private Vector2 droppedAt;

        private int droppedFrame = -1;

        private DragAndDrop.DragSource droppedSource;

        private const float MinDistToStartDragging = 3f;

        public enum DragSource
        {
            Unspecified,

            Inventory,

            IdentificationQueue,

            SpellsBar
        }
    }
}