using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class RoomLabelDrawer
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            List<RetainedRoomInfo.RoomInfo> rooms = Get.NowControlledActor.Position.GetRooms();
            this.SetCurrentRoom((rooms.Count == 0) ? null : rooms[0]);
            if (Get.NextTurnsUI.LastFrameDisplayed >= Clock.Frame - 1)
            {
                return;
            }
            Rect rect = new Rect(Widgets.VirtualWidth - RoomLabelDrawer.Offset.x - 300f, Widgets.VirtualHeight - RoomLabelDrawer.Offset.y - 30f, 300f, 30f);
            if (Math.Abs(this.lastTimeDrawnForAnimationPct - this.animationPct) >= 0.001f)
            {
                CachedGUI.SetDirty(3);
            }
            if (CachedGUI.BeginCachedGUI(rect.ExpandedBy(20f), 3, true))
            {
                this.lastTimeDrawnForAnimationPct = this.animationPct;
                if (this.previousRoom != null)
                {
                    this.DrawLabel(rect.MovedBy(20f * this.animationPct, 0f), this.previousRoom, 1f - this.animationPct);
                }
                if (this.currentRoom != null)
                {
                    this.DrawLabel(rect.MovedBy(-20f + 20f * this.animationPct, 0f), this.currentRoom, this.animationPct);
                }
            }
            CachedGUI.EndCachedGUI(1f, 1f);
        }

        public void FixedUpdate()
        {
            this.animationPct = Calc.Lerp(this.animationPct, 1f, 0.2f);
        }

        public void SetCurrentRoom(RetainedRoomInfo.RoomInfo room)
        {
            if (this.currentRoom == room)
            {
                return;
            }
            string text = ((room != null) ? room.Name : null);
            RetainedRoomInfo.RoomInfo roomInfo = this.currentRoom;
            bool flag = text != ((roomInfo != null) ? roomInfo.Name : null);
            if (flag)
            {
                this.previousRoom = this.currentRoom;
            }
            this.currentRoom = room;
            if (flag)
            {
                this.animationPct = 0f;
            }
            CachedGUI.SetDirty(3);
        }

        private void DrawLabel(Rect rect, RetainedRoomInfo.RoomInfo room, float alpha)
        {
            if (room.Name.NullOrEmpty())
            {
                return;
            }
            Widgets.Align = TextAnchor.UpperRight;
            Widgets.FontSizeScalable = 21;
            Widgets.FontBold = true;
            GUI.color = new Color(RoomLabelDrawer.Color.r, RoomLabelDrawer.Color.g, RoomLabelDrawer.Color.b, RoomLabelDrawer.Color.a * alpha);
            Widgets.Label(rect, room.Name, true, null, null, false);
            GUI.color = Color.white;
            Widgets.ResetAlign();
            Widgets.ResetFontSize();
            Widgets.FontBold = false;
        }

        private RetainedRoomInfo.RoomInfo currentRoom;

        private RetainedRoomInfo.RoomInfo previousRoom;

        private float animationPct;

        private float lastTimeDrawnForAnimationPct = -1f;

        private static readonly Vector2 Offset = new Vector2(30f, 10f);

        private const float Width = 300f;

        private const float Height = 30f;

        private const int FontSize = 21;

        private static readonly Color Color = new Color(1f, 1f, 1f, 0.6f);

        private const float AnimationLerpSpeed = 0.2f;

        private const float AnimationOffset = 20f;
    }
}