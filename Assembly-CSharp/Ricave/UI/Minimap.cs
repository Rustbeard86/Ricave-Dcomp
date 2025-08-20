using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class Minimap
    {
        private int MinimapHighResSize
        {
            get
            {
                return Calc.RoundToInt((float)Math.Min(Sys.Resolution.x, Sys.Resolution.y) * (((float)Sys.Resolution.x > 1920f) ? 0.6666667f : 1f));
            }
        }

        private float MinimapRotation
        {
            get
            {
                if (!this.RotateMinimap)
                {
                    return 0f;
                }
                return Get.FPPControllerGOC.ActorRotation;
            }
        }

        private bool RotateMinimap
        {
            get
            {
                return true;
            }
        }

        public float Zoom
        {
            get
            {
                return this.zoom;
            }
            set
            {
                if (value == this.zoom)
                {
                    return;
                }
                this.zoom = value;
                this.highResDirty = true;
            }
        }

        private float ZoomAccountForMapSize
        {
            get
            {
                return this.Zoom * ((float)Minimap.pixels.width / 41f);
            }
        }

        public void Regenerate()
        {
            if (Get.World == null)
            {
                return;
            }
            Vector3Int size = Get.World.Size;
            int num = Math.Max(size.x, size.z);
            if (this.minimapObjectChars == null || this.minimapObjectChars.GetLength(0) != size.x || this.minimapObjectChars.GetLength(1) != size.z)
            {
                this.minimapObjectChars = new ValueTuple<char, Color>[size.x, size.z];
            }
            if (Minimap.pixels == null || Minimap.pixels.width != num || Minimap.pixels.height != num)
            {
                if (Minimap.pixels != null)
                {
                    Object.Destroy(Minimap.pixels);
                }
                Minimap.pixels = new Texture2D(num, num);
                Minimap.pixels.name = "Minimap";
                Minimap.pixels.filterMode = FilterMode.Point;
                Minimap.pixels.wrapMode = TextureWrapMode.Clamp;
            }
            if (Minimap.minimapHighRes == null || Minimap.minimapHighRes.width != this.MinimapHighResSize || Minimap.minimapHighRes.height != this.MinimapHighResSize)
            {
                if (Minimap.minimapHighRes != null)
                {
                    Object.Destroy(Minimap.minimapHighRes);
                }
                Minimap.minimapHighRes = new RenderTexture(this.MinimapHighResSize, this.MinimapHighResSize, 0);
            }
            if (this.tmpColors == null || this.tmpColors.Length != num * num)
            {
                this.tmpColors = new Color[num * num];
            }
            this.generatedForSize = num;
            this.generatedForPlayerSpawned = Get.NowControlledActor != null && Get.NowControlledActor.Spawned;
            if (this.generatedForPlayerSpawned)
            {
                this.generatedForY = Get.NowControlledActor.Position.y;
            }
            else
            {
                this.generatedForY = -1;
            }
            for (int i = 0; i < this.minimapObjectChars.GetLength(0); i++)
            {
                for (int j = 0; j < this.minimapObjectChars.GetLength(1); j++)
                {
                    this.minimapObjectChars[i, j] = new ValueTuple<char, Color>('\0', Color.white);
                }
            }
            for (int k = 0; k < num; k++)
            {
                for (int l = 0; l < num; l++)
                {
                    if (this.generatedForPlayerSpawned && l < size.x && k < size.z)
                    {
                        Entity entity;
                        ValueTuple<Color, char, Color> desiredPixelAt = this.GetDesiredPixelAt(new Vector2Int(l, k), out entity);
                        this.tmpColors[k * num + l] = desiredPixelAt.Item1;
                        this.minimapObjectChars[l, k] = new ValueTuple<char, Color>(desiredPixelAt.Item2, desiredPixelAt.Item3);
                    }
                    else
                    {
                        this.tmpColors[k * num + l] = new Color(0f, 0f, 0f, 0f);
                    }
                }
            }
            Minimap.pixels.SetPixels(this.tmpColors);
            Minimap.pixels.Apply();
            this.highResDirty = true;
        }

        public void Update()
        {
            if (!this.highResDirty && (new Vector2(this.highResCalculatedForCamPos.x, this.highResCalculatedForCamPos.z) - new Vector2(Get.CameraPosition.x, Get.CameraPosition.z)).sqrMagnitude > 1.6000002E-05f)
            {
                this.highResDirty = true;
            }
            if (this.highResDirty)
            {
                this.UpdateHighResTexture();
            }
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.ScrollWheel && (Get.KeyBinding_Minimap.HeldDown || Get.KeyBinding_MinimapAlt.HeldDown))
            {
                this.extraScrollWheelZoom /= Calc.Pow(1.05f, Event.current.delta.y);
                this.extraScrollWheelZoom = Calc.Clamp(this.extraScrollWheelZoom, 0.5f, 2f);
            }
            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseDown && Event.current.type != EventType.MouseUp)
            {
                return;
            }
            if (Get.InLobby)
            {
                return;
            }
            if (this.darkenBackgroundAlpha > 0f)
            {
                GUIExtra.DrawRect(Widgets.ScreenRect, Color.black.WithAlpha(this.darkenBackgroundAlpha * 0.3f));
            }
            if (Get.KeyBinding_Minimap.HeldDown || Get.KeyBinding_MinimapAlt.HeldDown)
            {
                if (!this.prevShownBigMinimap)
                {
                    this.prevShownBigMinimap = true;
                    this.dontDisplayUntilNotDirty = true;
                }
                this.Zoom = 1f * this.extraScrollWheelZoom;
                if (Widgets.VirtualHeight <= Widgets.VirtualWidth)
                {
                    this.Draw(new Rect((Widgets.VirtualWidth - Widgets.VirtualHeight) / 2f, 0f, Widgets.VirtualHeight, Widgets.VirtualHeight), false, 1f * this.darkenBackgroundAlpha, true);
                }
                else
                {
                    this.Draw(new Rect(0f, (Widgets.VirtualHeight - Widgets.VirtualWidth) / 2f, Widgets.VirtualWidth, Widgets.VirtualWidth), false, 1f * this.darkenBackgroundAlpha, true);
                }
                if (Event.current.type == EventType.Repaint)
                {
                    this.darkenBackgroundAlpha = Math.Min(this.darkenBackgroundAlpha + Clock.UnscaledDeltaTime * 10f, 1f);
                }
                Get.LessonManager.FinishIfCurrent(Get.Lesson_BigMap);
                return;
            }
            if (this.prevShownBigMinimap)
            {
                this.prevShownBigMinimap = false;
                this.dontDisplayUntilNotDirty = true;
            }
            this.Zoom = 1.7f;
            if (!Get.DeathScreenDrawer.ShouldShow)
            {
                this.Draw(new Rect(Widgets.VirtualWidth - 350f, 0f, 350f, 350f), true, 1f, false);
            }
            this.DoDungeonFloor();
            if (Get.WorldInfo.EscapingMode)
            {
                this.DoEscapeLabel();
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.darkenBackgroundAlpha = Math.Max(this.darkenBackgroundAlpha - Clock.UnscaledDeltaTime * 10f, 0f);
            }
        }

        private void DoDungeonFloor()
        {
            Widgets.FontSizeScalable = 28;
            Vector2 vector = new Vector2(Widgets.VirtualWidth - Minimap.FloorLabelOffset.x, Minimap.FloorLabelOffset.y);
            Rect rect = new Rect(vector.x - 30f, vector.y - 15f, 30f, 30f);
            string text = ((Get.RunSpec.FloorCount != null) ? "{0}/{1}".Formatted(Get.Floor, Get.RunSpec.FloorCount.Value) : Get.Floor.ToStringCached());
            Vector2 vector2 = new Vector2(rect.x - 5f - Widgets.CalcSize(text).x, vector.y);
            float num = Widgets.CalcHeight(text, 9999f);
            Rect rect2 = new Rect(vector2.x, vector2.y - num / 2f, vector.x - vector2.x, num).ExpandedBy(3f);
            GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            if (Mouse.Over(rect2))
            {
                Get.Tooltips.RegisterTip(rect2, "DungeonFloorTip".Translate(), null);
            }
            if (Get.Floor != this.cachedDungeonFloorForFloor)
            {
                this.cachedDungeonFloorForFloor = Get.Floor;
                CachedGUI.SetDirty(9);
            }
            if (CachedGUI.BeginCachedGUI(rect2.ExpandedBy(2f), 9, false))
            {
                GUI.color = PlayerActorStatusControls.LevelColor;
                GUIExtra.DrawTexture(rect, Minimap.DungeonFloorIcon);
                Widgets.LabelCenteredV(vector2, text, true, null, null, false);
                GUI.color = Color.white;
            }
            CachedGUI.EndCachedGUI(1f, 1f);
            Widgets.ResetFontSize();
        }

        private void DoEscapeLabel()
        {
            Widgets.FontSizeScalable = 28;
            Vector2 vector = new Vector2(Widgets.VirtualWidth - Minimap.FloorLabelOffset.x, Minimap.FloorLabelOffset.y + 40f);
            string text = "EscapingMode".Translate();
            Vector2 vector2 = new Vector2(vector.x - Widgets.CalcSize(text).x, vector.y);
            float num = Widgets.CalcHeight(text, 9999f);
            Rect rect = new Rect(vector2.x, vector2.y - num / 2f, vector.x - vector2.x, num).ExpandedBy(3f);
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            if (Mouse.Over(rect))
            {
                Get.Tooltips.RegisterTip(rect, "EscapingModeTip".Translate(), null);
            }
            GUI.color = new Color(0.95f, 0.25f, 0.25f).Lighter(Calc.PulseUnscaled(6f, 0.16f));
            Widgets.LabelCenteredV(vector2, text, true, null, null, false);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
        }

        private void Draw(Rect rect, bool canUseCache, float minimapAlpha, bool pulseTargetRooms)
        {
            if (this.highResDirty && this.dontDisplayUntilNotDirty)
            {
                return;
            }
            List<RetainedRoomInfo.RoomInfo> rooms = Get.RetainedRoomInfo.Rooms;
            if (Event.current.type == EventType.Repaint)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (Minimap.< Draw > g__ShouldDrawRoomLabel | 57_0(rooms[i]))
                    {
                        float num;
                        if (!this.roomLabelAlpha.TryGetValue(rooms[i], out num))
                        {
                            num = 0f;
                        }
                        float num2 = num;
                        if (rooms[i].AnyNonFilledCellUnfogged)
                        {
                            Vector3 centerFloat = rooms[i].Shape.CenterFloat;
                            Vector2 vector = this.WorldToMinimapPos(new Vector2(centerFloat.x, centerFloat.z), rect);
                            if (rect.Contains(vector))
                            {
                                num += 1.3f * Clock.UnscaledDeltaTime;
                            }
                            else
                            {
                                num -= 1.3f * Clock.UnscaledDeltaTime;
                            }
                        }
                        else
                        {
                            num -= 1.3f * Clock.UnscaledDeltaTime;
                        }
                        num = Calc.Clamp01(num);
                        if (num2 != num)
                        {
                            this.roomLabelAlpha[rooms[i]] = num;
                            CachedGUI.SetDirty(2);
                        }
                    }
                }
            }
            float minimapRotation = this.MinimapRotation;
            GUI.color = Color.black.WithAlpha(minimapAlpha);
            GUIExtra.DrawTextureRotated(rect.MovedBy(-1f, 1f), Minimap.minimapHighRes, -minimapRotation, null);
            GUIExtra.DrawTextureRotated(rect.MovedBy(1f, -1f), Minimap.minimapHighRes, -minimapRotation, null);
            GUIExtra.DrawTextureRotated(rect.MovedBy(-1f, -1f), Minimap.minimapHighRes, -minimapRotation, null);
            GUIExtra.DrawTextureRotated(rect.MovedBy(1f, 1f), Minimap.minimapHighRes, -minimapRotation, null);
            GUI.color = Color.white.WithAlpha(minimapAlpha);
            GUIExtra.DrawTextureRotated(rect, Minimap.minimapHighRes, -minimapRotation, null);
            GUI.color = Color.white;
            if (!this.RotateMinimap)
            {
                Vector3Int position = Get.NowControlledActor.Position;
                GUIExtra.DrawTextureRotated(RectUtility.CenteredAt(this.WorldToMinimapPos(new Vector2((float)position.x, (float)position.z), rect), this.PixelSize(rect) * 1.1f), Minimap.ArrowTex, PlayerMovementManager.GetRotSnappedToCurDir() + 180f, null);
            }
            Widgets.FontSize = Widgets.GetFontSizeToFitInHeight(this.PixelSize(rect) * 1.1f);
            int j = 0;
            int length = this.minimapObjectChars.GetLength(1);
            while (j < length)
            {
                int k = 0;
                int length2 = this.minimapObjectChars.GetLength(0);
                while (k < length2)
                {
                    if (this.minimapObjectChars[k, j].Item1 != '\0')
                    {
                        Vector2 vector2 = this.WorldToMinimapPos(new Vector2((float)k, (float)j), rect);
                        GUI.color = this.minimapObjectChars[k, j].Item2;
                        Widgets.LabelCentered(vector2, this.minimapObjectChars[k, j].Item1.ToStringCached(), false, null, null, false, false, false, null);
                        GUI.color = Color.white;
                    }
                    k++;
                }
                j++;
            }
            Widgets.ResetFontSize();
            if (Math.Abs(minimapRotation - Minimap.cachedForCameraRot) > 0.15f)
            {
                Minimap.cachedForCameraRot = minimapRotation;
                CachedGUI.SetDirty(2);
            }
            bool flag = !canUseCache || CachedGUI.BeginCachedGUI(rect.ExpandedBy(50f), 2, true);
            if (flag)
            {
                for (int l = 0; l < rooms.Count; l++)
                {
                    float num3;
                    if (Minimap.< Draw > g__ShouldDrawRoomLabel | 57_0(rooms[l]) && this.roomLabelAlpha.TryGetValue(rooms[l], out num3) && num3 > 0f)
                    {
                        Vector3 centerFloat2 = rooms[l].Shape.CenterFloat;
                        Vector2 vector3 = this.WorldToMinimapPos(new Vector2(centerFloat2.x, centerFloat2.z), rect);
                        if (rooms[l].EverVisitedOrKnown)
                        {
                            if (!rooms[l].Name.NullOrEmpty())
                            {
                                bool flag2;
                                if (pulseTargetRooms)
                                {
                                    if (Get.World.AnyEntityOfSpec(Get.Entity_Lever))
                                    {
                                        flag2 = rooms[l].Role == Room.LayoutRole.LeverRoom;
                                    }
                                    else
                                    {
                                        flag2 = rooms[l].Role == Room.LayoutRole.End;
                                    }
                                }
                                else
                                {
                                    flag2 = false;
                                }
                                GUI.color = new Color(1f, 1f, 1f, minimapAlpha * num3);
                                if (flag2)
                                {
                                    Widgets.FontSizeScalable = 15 + Calc.RoundToInt(Calc.PulseUnscaled(5f, 5f));
                                }
                                Widgets.FontBold = true;
                                Widgets.LabelCentered(vector3, rooms[l].Name, true, null, null, false, false, false, null);
                                Widgets.FontBold = false;
                                if (flag2)
                                {
                                    Widgets.ResetFontSize();
                                }
                            }
                        }
                        else if (Minimap.CanShowUnexploredSymbolFor(rooms[l]))
                        {
                            GUI.color = new Color(1f, 1f, 1f, minimapAlpha * num3);
                            Widgets.FontSizeScalable = 40;
                            Widgets.FontBold = true;
                            Widgets.LabelCentered(vector3, "?", true, null, null, false, false, false, null);
                            Widgets.FontBold = false;
                            Widgets.ResetFontSize();
                        }
                    }
                }
                GUI.color = Color.white;
            }
            if (canUseCache)
            {
                CachedGUI.EndCachedGUI(1f, 1f);
            }
            if (Mouse.Over(rect) && Get.UI.WantsMouseUnlocked)
            {
                Vector2 vector4 = this.MinimapToWorldPos(Event.current.mousePosition, rect);
                Vector2Int vector2Int = new Vector2Int(Calc.RoundToInt(vector4.x), Calc.RoundToInt(vector4.y));
                Vector3Int vector3Int = new Vector3Int(vector2Int.x, this.generatedForY, vector2Int.y);
                if (vector3Int.InBounds())
                {
                    Entity entity;
                    this.GetDesiredPixelAt(vector2Int, out entity);
                    float num4 = rect.width / ((float)this.generatedForSize / this.ZoomAccountForMapSize);
                    Vector2 vector5 = this.WorldToMinimapPos(vector2Int, rect);
                    Rect rect2 = RectUtility.CenteredAt(vector5, num4 * 2f);
                    if (entity != null)
                    {
                        GUIExtra.DrawHighlightRotated(RectUtility.CenteredAt(vector5, num4), -minimapRotation);
                        Get.Tooltips.RegisterTip(rect2, entity, null, new int?(Calc.CombineHashes<int, int>(entity.MyStableHash, 5132995)));
                    }
                    if (vector3Int != Get.NowControlledActor.Position && Get.FogOfWar.IsUnfogged(vector3Int))
                    {
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && entity != null)
                        {
                            List<ValueTuple<string, Action, string>> contextMenuOptions = SeenEntitiesDrawer.GetContextMenuOptions(entity);
                            Get.WindowManager.OpenContextMenu(contextMenuOptions, entity.LabelCap);
                            Event.current.Use();
                        }
                        if (Widgets.ButtonInvisible(rect2, false, false) && Event.current.button == 0)
                        {
                            Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                            SeenEntitiesDrawer.MoveTo(vector3Int);
                        }
                    }
                }
            }
        }

        private float PixelSize(Rect rect)
        {
            return rect.width / ((float)this.generatedForSize / this.ZoomAccountForMapSize);
        }

        private Vector2 WorldToMinimapPos(Vector2 pos, Rect rect)
        {
            float minimapRotation = this.MinimapRotation;
            Vector3 cameraPosition = Get.CameraPosition;
            float num = (float)this.generatedForSize / this.ZoomAccountForMapSize;
            float num2 = (cameraPosition.x + 0.5f) / (float)this.generatedForSize;
            float num3 = (cameraPosition.z + 0.5f) / (float)this.generatedForSize;
            float num4 = (pos.x + 0.5f) / (float)this.generatedForSize;
            float num5 = (pos.y + 0.5f) / (float)this.generatedForSize;
            float num6 = num4 - num2;
            float num7 = num5 - num3;
            float num8 = num6 * (float)this.generatedForSize;
            float num9 = num7 * (float)this.generatedForSize;
            num8 /= num;
            num9 /= num;
            num8 += 0.5f;
            num9 += 0.5f;
            Vector2 vector = new Vector2(num8, num9).Rotate(minimapRotation, new Vector2(0.5f, 0.5f));
            return new Vector2(rect.x + vector.x * rect.width, rect.y + (1f - vector.y) * rect.height);
        }

        private Vector2 MinimapToWorldPos(Vector2 pos, Rect rect)
        {
            float minimapRotation = this.MinimapRotation;
            Vector3 cameraPosition = Get.CameraPosition;
            float num = (float)this.generatedForSize / this.ZoomAccountForMapSize;
            float num2 = (cameraPosition.x + 0.5f) / (float)this.generatedForSize;
            float num3 = (cameraPosition.z + 0.5f) / (float)this.generatedForSize;
            Vector2 vector = new Vector2((pos.x - rect.x) / rect.width, 1f - (pos.y - rect.y) / rect.height).Rotate(-minimapRotation, new Vector2(0.5f, 0.5f));
            float num4 = vector.x;
            float y = vector.y;
            num4 -= 0.5f;
            float num5 = y - 0.5f;
            num4 *= num;
            float num6 = num5 * num;
            float num7 = num4 / (float)this.generatedForSize;
            float num8 = num6 / (float)this.generatedForSize;
            float num9 = num7 + num2;
            float num10 = num8 + num3;
            float num11 = num9 * (float)this.generatedForSize - 0.5f;
            float num12 = num10 * (float)this.generatedForSize - 0.5f;
            return new Vector2(num11, num12);
        }

        private void UpdateHighResTexture()
        {
            Vector2 vector = new Vector2(1f / this.ZoomAccountForMapSize, 1f / this.ZoomAccountForMapSize);
            Vector3 cameraPosition = Get.CameraPosition;
            float num = (cameraPosition.x + 0.5f) / (float)this.generatedForSize;
            float num2 = (cameraPosition.z + 0.5f) / (float)this.generatedForSize;
            Vector2 vector2 = new Vector2(num - vector.x / 2f, num2 - vector.y / 2f);
            Minimap.MinimapMaterial.SetVector(Get.ShaderPropertyIDs.OffsetID, vector2);
            Minimap.MinimapMaterial.SetVector(Get.ShaderPropertyIDs.ScaleID, vector);
            Graphics.Blit(Minimap.pixels, Minimap.minimapHighRes, Minimap.MinimapMaterial);
            this.highResDirty = false;
            this.dontDisplayUntilNotDirty = false;
            this.highResCalculatedForCamPos = cameraPosition;
            this.highResCalculatedForZoom = this.ZoomAccountForMapSize;
            CachedGUI.SetDirty(2);
        }

        public void OnEntityMoved(Entity entity, Vector3Int prev)
        {
            if (entity.IsNowControlledActor && entity.Position.y != this.generatedForY)
            {
                this.Regenerate();
                return;
            }
            this.ProcessEntityChangedAt(prev);
            this.ProcessEntityChangedAt(entity.Position);
        }

        public void OnEntitySpawned(Entity entity)
        {
            if (entity.IsNowControlledActor)
            {
                this.Regenerate();
                return;
            }
            this.ProcessEntityChangedAt(entity.Position);
        }

        public void OnEntityDeSpawned(Entity entity)
        {
            if (entity.IsNowControlledActor)
            {
                this.Regenerate();
                return;
            }
            this.ProcessEntityChangedAt(entity.Position);
        }

        public void OnFactionHostilityChanged()
        {
            this.Regenerate();
        }

        public void OnFogChanged(List<Vector3Int> unfoggedOrFogged)
        {
            if (!this.generatedForPlayerSpawned)
            {
                return;
            }
            for (int i = 0; i < unfoggedOrFogged.Count; i++)
            {
                if (this.AltitudeCurrentlyIncludedInMinimap(unfoggedOrFogged[i].y))
                {
                    this.UpdatePixelAt(new Vector2Int(unfoggedOrFogged[i].x, unfoggedOrFogged[i].z), false);
                }
            }
            for (int j = 0; j < unfoggedOrFogged.Count; j++)
            {
                foreach (Vector3Int vector3Int in Vector3IntUtility.DirectionsCardinal)
                {
                    Vector3Int vector3Int2 = unfoggedOrFogged[j] + vector3Int;
                    if (vector3Int2.InBounds() && this.AltitudeCurrentlyIncludedInMinimap(vector3Int2.y))
                    {
                        this.UpdatePixelAt(new Vector2Int(vector3Int2.x, vector3Int2.z), false);
                    }
                }
            }
            Minimap.pixels.Apply();
        }

        public void OnVisibilityChanged(List<Vector3Int> noLongerSeen, List<Vector3Int> newlySeen)
        {
            if (!this.generatedForPlayerSpawned)
            {
                return;
            }
            for (int i = 0; i < noLongerSeen.Count; i++)
            {
                if (this.AltitudeCurrentlyIncludedInMinimap(noLongerSeen[i].y))
                {
                    this.UpdatePixelAt(new Vector2Int(noLongerSeen[i].x, noLongerSeen[i].z), false);
                }
            }
            for (int j = 0; j < newlySeen.Count; j++)
            {
                if (this.AltitudeCurrentlyIncludedInMinimap(newlySeen[j].y))
                {
                    this.UpdatePixelAt(new Vector2Int(newlySeen[j].x, newlySeen[j].z), false);
                }
            }
            Minimap.pixels.Apply();
        }

        private void ProcessEntityChangedAt(Vector3Int pos)
        {
            if (!this.AltitudeCurrentlyIncludedInMinimap(pos.y))
            {
                return;
            }
            this.UpdatePixelAt(new Vector2Int(pos.x, pos.z), true);
        }

        private void UpdatePixelAt(Vector2Int pos, bool apply = true)
        {
            Entity entity;
            ValueTuple<Color, char, Color> desiredPixelAt = this.GetDesiredPixelAt(pos, out entity);
            Minimap.pixels.SetPixel(pos.x, pos.y, desiredPixelAt.Item1);
            this.minimapObjectChars[pos.x, pos.y] = new ValueTuple<char, Color>(desiredPixelAt.Item2, desiredPixelAt.Item3);
            if (apply)
            {
                Minimap.pixels.Apply();
            }
            this.highResDirty = true;
        }

        private bool AltitudeCurrentlyIncludedInMinimap(int y)
        {
            return this.generatedForPlayerSpawned && (y == this.generatedForY || y == this.generatedForY - 1 || y == this.generatedForY + 1);
        }

        private ValueTuple<Color, char, Color> GetDesiredPixelAt(Vector2Int pos, out Entity mainEntity)
        {
            mainEntity = null;
            Vector3Int vector3Int = new Vector3Int(pos.x, this.generatedForY, pos.y);
            if (Get.FogOfWar.IsFogged(vector3Int))
            {
                bool flag = false;
                foreach (Vector3Int vector3Int2 in Vector3IntUtility.DirectionsCardinal)
                {
                    Vector3Int vector3Int3 = vector3Int + vector3Int2;
                    if (vector3Int3.InBounds() && Get.FogOfWar.IsUnfogged(vector3Int3) && Get.CellsInfo.CanSeeThrough(vector3Int3))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    return new ValueTuple<Color, char, Color>(Minimap.FoggedColor, '\0', Color.white);
                }
                return new ValueTuple<Color, char, Color>(Minimap.FarFoggedColor, '\0', Color.white);
            }
            else
            {
                Color color = Minimap.NothingInterestingColor;
                if (Get.CellsInfo.AnyFilledImpassableAt(vector3Int) || Get.World.AnyEntityOfSpecAt(vector3Int, Get.Entity_Window) || Get.World.AnyEntityOfSpecAt(vector3Int, Get.Entity_SecretPassage))
                {
                    foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int))
                    {
                        Structure structure = entity as Structure;
                        if (structure != null && !structure.Spec.CanPassThrough)
                        {
                            mainEntity = entity;
                            break;
                        }
                    }
                    return new ValueTuple<Color, char, Color>(Minimap.ImpassableColor, '\0', Color.white);
                }
                if (Get.CellsInfo.AnyDoorAt(vector3Int) && !Get.World.AnyEntityOfSpecAt(vector3Int, Get.Entity_SecretPassage) && !Get.World.AnyEntityOfSpecAt(vector3Int, Get.Entity_TemporarilyOpenedDoor))
                {
                    return new ValueTuple<Color, char, Color>(Minimap.DoorColor, '\0', Color.white);
                }
                if (!Get.VisibilityCache.PlayerSees(vector3Int))
                {
                    return new ValueTuple<Color, char, Color>(Minimap.OutOfSightColor, '\0', Color.white);
                }
                if (Get.CellsInfo.IsFloorUnderNoActors(vector3Int))
                {
                    foreach (Entity entity2 in Get.World.GetEntitiesAt(vector3Int.Below()))
                    {
                        Structure structure2 = entity2 as Structure;
                        if (structure2 != null && !structure2.Spec.CanPassThrough)
                        {
                            mainEntity = entity2;
                            break;
                        }
                    }
                    color = Minimap.PassableColor;
                }
                List<Entity> entitiesAt = Get.World.GetEntitiesAt(vector3Int);
                for (int j = entitiesAt.Count - 1; j >= 0; j--)
                {
                    if (entitiesAt[j] is Actor)
                    {
                        ValueTuple<char, Color>? importantEntityChar = this.GetImportantEntityChar(entitiesAt[j]);
                        if (importantEntityChar != null)
                        {
                            mainEntity = entitiesAt[j];
                            return new ValueTuple<Color, char, Color>(color, importantEntityChar.Value.Item1, importantEntityChar.Value.Item2);
                        }
                    }
                }
                for (int k = entitiesAt.Count - 1; k >= 0; k--)
                {
                    ValueTuple<char, Color>? importantEntityChar2 = this.GetImportantEntityChar(entitiesAt[k]);
                    if (importantEntityChar2 != null)
                    {
                        mainEntity = entitiesAt[k];
                        return new ValueTuple<Color, char, Color>(color, importantEntityChar2.Value.Item1, importantEntityChar2.Value.Item2);
                    }
                }
                return new ValueTuple<Color, char, Color>(color, '\0', Color.white);
            }
        }

        private ValueTuple<char, Color>? GetImportantEntityChar(Entity entity)
        {
            if (!entity.Spec.RenderIfPlayerCantSee && !Get.VisibilityCache.PlayerSees(entity))
            {
                return null;
            }
            if (entity.IsNowControlledActor)
            {
                if (!this.RotateMinimap)
                {
                    return null;
                }
                return new ValueTuple<char, Color>?(new ValueTuple<char, Color>('●', Minimap.PlayerColor));
            }
            else
            {
                if (entity.IsPlayerParty)
                {
                    return new ValueTuple<char, Color>?(new ValueTuple<char, Color>('●', Minimap.PlayerColor));
                }
                if (entity.Spec.SpecialMinimapColor != null)
                {
                    return new ValueTuple<char, Color>?(new ValueTuple<char, Color>('●', entity.Spec.SpecialMinimapColor.Value));
                }
                Actor actor = entity as Actor;
                if (actor != null)
                {
                    return new ValueTuple<char, Color>?(new ValueTuple<char, Color>('●', actor.HostilityColor.MultipliedColor(1.43f)));
                }
                if (entity is Item)
                {
                    return new ValueTuple<char, Color>?(new ValueTuple<char, Color>('●', Minimap.ItemColor));
                }
                if (entity is Structure && entity.Spec.Structure.IsDoor && entity.Spec != Get.Entity_SecretPassage && entity.Spec != Get.Entity_TemporarilyOpenedDoor)
                {
                    return new ValueTuple<char, Color>?(new ValueTuple<char, Color>('●', Minimap.DoorColor));
                }
                if (entity is Structure && entity.Spec.Structure.IsSpecial)
                {
                    return new ValueTuple<char, Color>?(new ValueTuple<char, Color>('●', Minimap.SpecialStructureColor));
                }
                if (entity is Structure && (!entity.Spec.CanPassThrough || entity.MaxHP != 0) && entity.Spec != Get.Entity_Torch && entity.Spec != Get.Entity_VioletTorch)
                {
                    return new ValueTuple<char, Color>?(new ValueTuple<char, Color>('●', Minimap.MiscStructureColor));
                }
                return null;
            }
        }

        public void OnWorldAboutToRegenerate()
        {
            this.roomLabelAlpha.Clear();
        }

        public static bool CanShowUnexploredSymbolFor(RetainedRoomInfo.RoomInfo room)
        {
            return room.Role != Room.LayoutRole.Secret && room.Role != Room.LayoutRole.LockedBehindSilverDoor && room.Role != Room.LayoutRole.OptionalChallenge;
        }

        [CompilerGenerated]
        internal static bool <Draw>g__ShouldDrawRoomLabel|57_0(RetainedRoomInfo.RoomInfo room)
		{
			return Get.NowControlledActor.Position.y >= room.Shape.y && Get.NowControlledActor.Position.y <= room.Shape.yMax - 1;
		}

		private int generatedForY = -1;

        private int generatedForSize = -1;

        private bool generatedForPlayerSpawned;

        private Color[] tmpColors;

        private ValueTuple<char, Color>[,] minimapObjectChars;

        private bool highResDirty = true;

        private float zoom = 1.7f;

        private Vector3 highResCalculatedForCamPos;

        private float highResCalculatedForZoom = -1f;

        private Dictionary<RetainedRoomInfo.RoomInfo, float> roomLabelAlpha = new Dictionary<RetainedRoomInfo.RoomInfo, float>();

        private static float cachedForCameraRot = -1f;

        private float darkenBackgroundAlpha;

        private float extraScrollWheelZoom = 1f;

        private bool dontDisplayUntilNotDirty = true;

        private bool prevShownBigMinimap;

        private static Texture2D pixels;

        private static RenderTexture minimapHighRes;

        private static readonly Material MinimapMaterial = Assets.Get<Material>("Materials/Misc/Minimap");

        private static readonly Texture2D DungeonFloorIcon = Assets.Get<Texture2D>("Textures/UI/DungeonFloor");

        private static readonly Texture2D ArrowTex = Assets.Get<Texture2D>("Textures/UI/Arrow");

        public const float Size = 350f;

        private const float ZoomSmall = 1.7f;

        private const float ZoomBig = 1f;

        private const float CamPosEps = 0.004f;

        private const float RoomLabelFadeSpeed = 1.3f;

        private static readonly Vector2 FloorLabelOffset = new Vector2(324f, 41f);

        private const float RecacheCameraRotEps = 0.15f;

        private const float MinimapAlpha = 1f;

        private const float BigMinimapAlpha = 1f;

        private static readonly Color ImpassableColor = new Color(0f, 0f, 0f);

        private static readonly Color PassableColor = new Color(0.69f, 0.69f, 0.69f);

        private static readonly Color OutOfSightColor = new Color(0.42f, 0.42f, 0.42f);

        private static readonly Color NothingInterestingColor = new Color(0f, 0f, 0f, 0f);

        private static readonly Color FarFoggedColor = new Color(0f, 0f, 0f, 0f);

        private static readonly Color FoggedColor = new Color(0.28f, 0.28f, 0.48f);

        private static readonly Color DoorColor = new Color(1f, 1f, 0.7f);

        private static readonly Color PlayerColor = new Color(0.1f, 0.1f, 1f);

        private static readonly Color ItemColor = new Color(1f, 1f, 0f);

        private static readonly Color SpecialStructureColor = new Color(0.8f, 0f, 1f);

        private static readonly Color MiscStructureColor = new Color(1f, 1f, 1f);

        private int cachedDungeonFloorForFloor = -1;
    }
}