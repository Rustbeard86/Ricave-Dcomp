using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class UnexploredRoomsDrawer
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (!Get.KeyBinding_Minimap.HeldDown && !Get.KeyBinding_MinimapAlt.HeldDown)
            {
                return;
            }
            this.toDraw.Clear();
            List<RetainedRoomInfo.RoomInfo> rooms = Get.RetainedRoomInfo.Rooms;
            Vector3Int position = Get.NowControlledActor.Position;
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].AnyNonFilledCellUnfogged && (position.x < rooms[i].Shape.x || position.x > rooms[i].Shape.xMax || position.z < rooms[i].Shape.z || position.z > rooms[i].Shape.zMax))
                {
                    bool flag = !rooms[i].EverVisitedOrKnown;
                    if (!flag || Minimap.CanShowUnexploredSymbolFor(rooms[i]))
                    {
                        Texture2D iconToShowFor = this.GetIconToShowFor(rooms[i]);
                        if (flag || !(iconToShowFor == null))
                        {
                            Vector3 centerFloat = rooms[i].Shape.CenterFloat;
                            centerFloat.y = (float)(rooms[i].Shape.yMin + 1);
                            Vector3 vector = Get.Camera.WorldToScreenPoint(centerFloat) / Widgets.UIScale;
                            vector.y = Widgets.VirtualHeight - vector.y;
                            if (vector.z > 0f)
                            {
                                Vector3 up = Get.CameraTransform.up;
                                Vector3 vector2 = centerFloat + up * 1.3f / 2f;
                                Vector3 vector3 = Get.Camera.WorldToScreenPoint(vector2) / Widgets.UIScale;
                                vector3.y = Widgets.VirtualHeight - vector3.y;
                                float num = Math.Abs(vector.y - vector3.y) * 2f;
                                Rect rect = new Rect(vector.x - num / 2f, vector.y - num / 2f, num, num);
                                if (rect.Overlaps(Widgets.ScreenRect))
                                {
                                    rect.y += Calc.Sin(Clock.UnscaledTime * 3f) * rect.height * 0.05f;
                                    this.toDraw.Add(new ValueTuple<Vector3, bool, Rect, Texture2D>(centerFloat, flag, rect, iconToShowFor));
                                }
                            }
                        }
                    }
                }
            }
            if (this.toDraw.Count >= 2)
            {
                this.toDraw.Sort(UnexploredRoomsDrawer.ByDistToCamera);
            }
            for (int j = 0; j < this.toDraw.Count; j++)
            {
                ValueTuple<Vector3, bool, Rect, Texture2D> valueTuple = this.toDraw[j];
                bool item = valueTuple.Item2;
                Rect item2 = valueTuple.Item3;
                Texture2D item3 = valueTuple.Item4;
                if (item)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.45f);
                    GUI.DrawTexture(item2.ExpandedBy(item2.width * 0.1f), UnexploredRoomsDrawer.IconBackground);
                    GUI.DrawTexture(item2, UnexploredRoomsDrawer.UnexploredIcon);
                }
                else
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.65f);
                    GUI.DrawTexture(item2.ExpandedBy(item2.width * 0.1f), UnexploredRoomsDrawer.IconBackground);
                    GUI.DrawTexture(item2, item3);
                }
            }
            GUI.color = Color.white;
        }

        private Texture2D GetIconToShowFor(RetainedRoomInfo.RoomInfo room)
        {
            if (room.Role == Room.LayoutRole.End)
            {
                return Get.Entity_Staircase.IconAdjusted;
            }
            if (room.Role == Room.LayoutRole.StoreysTransition)
            {
                return Get.Entity_Ladder.IconAdjusted;
            }
            if (room.Role == Room.LayoutRole.LeverRoom && Get.World.AnyEntityOfSpec(Get.Entity_Lever))
            {
                return Get.Entity_Lever.IconAdjusted;
            }
            if (room.Role == Room.LayoutRole.Start)
            {
                return Get.Entity_Sign.IconAdjusted;
            }
            return null;
        }

        private List<ValueTuple<Vector3, bool, Rect, Texture2D>> toDraw = new List<ValueTuple<Vector3, bool, Rect, Texture2D>>();

        private static readonly Texture2D IconBackground = Assets.Get<Texture2D>("Textures/UI/IconBackground");

        public static readonly Texture2D UnexploredIcon = Assets.Get<Texture2D>("Textures/UI/UnexploredRoom");

        private const float Size = 1.3f;

        private static readonly Comparison<ValueTuple<Vector3, bool, Rect, Texture2D>> ByDistToCamera = (ValueTuple<Vector3, bool, Rect, Texture2D> a, ValueTuple<Vector3, bool, Rect, Texture2D> b) => (b.Item1 - Get.CameraPosition).sqrMagnitude.CompareTo((a.Item1 - Get.CameraPosition).sqrMagnitude);
    }
}