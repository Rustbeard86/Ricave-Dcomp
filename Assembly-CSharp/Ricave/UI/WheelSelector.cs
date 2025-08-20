using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class WheelSelector
    {
        public ITipSubject ForSubject
        {
            get
            {
                return this.subject;
            }
        }

        public bool IsSpellChooser
        {
            get
            {
                return this.isSpellChooser;
            }
        }

        private float HotAreaAngleSpan
        {
            get
            {
                return Math.Min(360f / (float)this.options.Count, 90f);
            }
        }

        private float OptionSize
        {
            get
            {
                return 970f * (this.HotAreaAngleSpan / 360f);
            }
        }

        private int OptionFontSize
        {
            get
            {
                return Math.Min(Widgets.GetFontSizeToFitInHeight(this.OptionSize / 2f), 35);
            }
        }

        public WheelSelector(List<WheelSelector.Option> options, ITipSubject subject, string extraText = null, bool isSpellChooser = false, bool closeIfItemNotInInventory = false, bool isQuickbarMenu = false)
        {
            if (options.Count == 0)
            {
                options.Add(new WheelSelector.Option("Leave".Translate(), delegate
                {
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                }, null, null));
            }
            this.options = options;
            this.subject = subject;
            this.extraText = extraText;
            this.isSpellChooser = isSpellChooser;
            this.closeIfItemNotInInventory = closeIfItemNotInInventory;
            this.isQuickbarMenu = isQuickbarMenu;
            this.mouseoverAnimationPct = new List<float>();
            for (int i = 0; i < options.Count; i++)
            {
                this.mouseoverAnimationPct.Add(0f);
            }
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }
            if (Event.current.type == EventType.MouseMove)
            {
                return;
            }
            if (this.closeIfItemNotInInventory)
            {
                Item item = this.subject as Item;
                if (item != null && !Get.NowControlledActor.Inventory.Contains(item))
                {
                    Get.UI.CloseWheelSelector(false);
                    return;
                }
            }
            if (this.firstMouseUpFrame == null && !Input.GetMouseButton(0))
            {
                this.firstMouseUpFrame = new int?(Clock.Frame);
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.distToOption = Calc.LerpWithUnscaledDeltaTime(this.distToOption, 190f, 17.83374f);
                this.fadeInAlpha = Calc.LerpWithUnscaledDeltaTime(this.fadeInAlpha, 1f, 11.15718f);
                this.animatedAngle = Calc.LerpWithUnscaledDeltaTime(this.animatedAngle, 0f, 11.15718f);
                for (int i = 0; i < this.mouseoverAnimationPct.Count; i++)
                {
                    float num = ((i == this.mouseoverIndex) ? 1f : 0f);
                    this.mouseoverAnimationPct[i] = Calc.LerpWithUnscaledDeltaTime(this.mouseoverAnimationPct[i], num, 11.15718f);
                }
            }
            Vector2 screenCenter = Widgets.ScreenCenter;
            float hotAreaAngleSpan = this.HotAreaAngleSpan;
            GUIExtra.DrawCircle(screenCenter, 305f, new Color(0f, 0f, 0f, 0.15f).WithAlphaFactor(this.fadeInAlpha));
            if (this.isQuickbarMenu)
            {
                for (int j = 0; j < this.options.Count; j++)
                {
                    Item item2 = this.options[j].TipSubject as Item;
                    if (item2 != null)
                    {
                        Color? backgroundColor = ItemSlotDrawer.GetBackgroundColor(item2);
                        if (backgroundColor != null)
                        {
                            float baseAngle = this.GetBaseAngle(j);
                            GUI.color = backgroundColor.Value.WithAlphaFactor(0.6f * this.fadeInAlpha);
                            GUIExtra.DrawPie(screenCenter, 305f, baseAngle - hotAreaAngleSpan / 2f + 90f, hotAreaAngleSpan, 30f);
                            GUI.color = Color.white;
                        }
                    }
                }
            }
            int num2 = this.mouseoverIndex;
            this.mouseoverIndex = -1;
            for (int k = 0; k < this.options.Count; k++)
            {
                float baseAngle2 = this.GetBaseAngle(k);
                float sqrMagnitude = (Event.current.mousePosition - screenCenter).sqrMagnitude;
                float num3 = Calc.NormalizeDir(screenCenter.Atan2Deg(Event.current.mousePosition) + 90f);
                if (sqrMagnitude >= 900f && sqrMagnitude <= 93025f && Math.Abs(Calc.DeltaAngle(baseAngle2, num3)) < hotAreaAngleSpan / 2f && this.mouseoverIndex == -1)
                {
                    if (num2 != k && !this.options[k].Disabled)
                    {
                        Get.Sound_Hover.PlayOneShot(null, 1f, 1f);
                    }
                    this.mouseoverIndex = k;
                }
                float num4 = Math.Min(this.mouseoverAnimationPct[k] * 0.85f + ((this.mouseoverIndex == k) ? 0.15f : 0f), 1f);
                if (num4 > 0f)
                {
                    GUI.color = (this.options[k].Disabled ? new Color(1f, 0.1f, 0.1f, 0.83f) : new Color(1f, 1f, 1f)).WithAlphaFactor(0.1f * num4);
                    GUIExtra.DrawPie(screenCenter, 305f, baseAngle2 - hotAreaAngleSpan / 2f + 90f, hotAreaAngleSpan, 30f);
                    GUI.color = Color.white;
                }
            }
            if (this.subject != null)
            {
                Rect rect = new Rect(screenCenter.x - 70f, screenCenter.y - 70f, 140f, 140f);
                GUI.color = this.subject.IconColor.WithAlphaFactor(this.fadeInAlpha);
                GUIExtra.DrawTextureRotated(rect, this.subject.Icon, this.animatedAngle, null);
                GUI.color = new Color(1f, 1f, 1f, this.fadeInAlpha);
                Widgets.FontSizeScalable = 20;
                Widgets.LabelCentered(screenCenter + new Vector2(0f, 84f), RichText.Label(this.subject), true, null, this.subject, false, false, false, null);
                Widgets.ResetFontSize();
                Item item3 = this.subject as Item;
                if (item3 != null)
                {
                    if (!item3.Identified && item3.Spec.CanBeCursed)
                    {
                        Widgets.LabelCentered(screenCenter + new Vector2(0f, 144f), "ItemMayBeCursedWarning".Translate(RichText.Cursed("CursedLower".Translate())), true, null, null, false, false, false, null);
                        Widgets.LabelCentered(screenCenter + new Vector2(0f, 166f), "CantUnequipCursedItems".Translate(RichText.Cursed("CursedLower".Translate())), true, null, null, false, false, false, null);
                    }
                    else if (item3.Identified && item3.Cursed)
                    {
                        Widgets.LabelCentered(screenCenter + new Vector2(0f, 144f), "ItemCursedWarning".Translate(RichText.Cursed("CursedLower".Translate())), true, null, null, false, false, false, null);
                        Widgets.LabelCentered(screenCenter + new Vector2(0f, 166f), "CantUnequipCursedItems".Translate(RichText.Cursed("CursedLower".Translate())), true, null, null, false, false, false, null);
                    }
                }
                GUI.color = Color.white;
            }
            Widgets.FontSizeScalable = this.OptionFontSize;
            Widgets.FontBold = true;
            for (int l = 0; l < this.options.Count; l++)
            {
                float num5 = this.GetBaseAngle(l) + this.animatedAngle;
                bool flag = this.mouseoverIndex == l;
                Vector2 vector = screenCenter + new Vector2(Calc.Cos((num5 - 90f) * 0.017453292f), Calc.Sin((num5 - 90f) * 0.017453292f)) * this.distToOption;
                if (!this.options[l].Disabled)
                {
                    vector.y -= this.mouseoverAnimationPct[l] * 10f;
                }
                if (flag)
                {
                    Get.Tooltips.RegisterTip(Widgets.ScreenRect, this.options[l].TipSubject, this.options[l].Tip, null);
                }
                if (this.options[l].Disabled)
                {
                    GUI.color = new Color(0.5f, 0.5f, 0.5f, this.fadeInAlpha);
                }
                else if (flag)
                {
                    GUI.color = new Color(0.95f, 0.95f, 0.95f, this.fadeInAlpha);
                }
                else
                {
                    GUI.color = new Color(0.85f, 0.85f, 0.85f, this.fadeInAlpha);
                }
                if (this.options[l].TipSubject != null)
                {
                    Rect rect2 = RectUtility.CenteredAt(vector, Math.Min(this.OptionSize * 0.99f, 150f));
                    GUI.color *= this.options[l].TipSubject.IconColor;
                    GUIExtra.DrawTextureRotated(rect2, this.options[l].TipSubject.Icon, this.animatedAngle, null);
                    Item item4 = this.options[l].TipSubject as Item;
                    if (item4 != null && Get.NowControlledActor.Inventory.Equipped.IsEquipped(item4))
                    {
                        ItemSlotDrawer.DrawEquippedOutline(rect2, item4.Cursed && item4.Identified, true);
                    }
                }
                else
                {
                    Widgets.LabelCentered(RectUtility.CenteredAt(vector, this.OptionSize * 1.9f), this.options[l].Label, true, null, null);
                }
                GUI.color = Color.white;
            }
            Widgets.ResetFontSize();
            Widgets.FontBold = false;
            if (!this.extraText.NullOrEmpty())
            {
                GUI.color = new Color(1f, 1f, 1f, Calc.Pow(this.fadeInAlpha, 10f));
                Widgets.Align = TextAnchor.MiddleCenter;
                Widgets.FontSizeScalable = 19;
                float num6 = Widgets.VirtualWidth * 0.5f;
                float y = Widgets.CalcSize(this.extraText, num6).y;
                Vector2 vector2 = screenCenter;
                vector2.y -= 311f + y / 2f;
                Widgets.LabelCentered(vector2, this.extraText, true, null, null, false, true, true, new float?(num6));
                Widgets.ResetFontSize();
                Widgets.ResetAlign();
                GUI.color = Color.white;
            }
            bool flag2 = false;
            if (Event.current.type == EventType.MouseUp)
            {
                int? num7 = this.firstMouseUpFrame;
                int frame = Clock.Frame;
                if (!((num7.GetValueOrDefault() == frame) & (num7 != null)))
                {
                    if (Event.current.button == 0 && this.mouseoverIndex >= 0)
                    {
                        flag2 = true;
                        goto IL_0A3F;
                    }
                    Get.UI.CloseWheelSelector(true);
                    goto IL_0A3F;
                }
            }
            if (ControllerUtility.InControllerMode && Get.KeyBinding_Accept.JustPressed && this.mouseoverIndex >= 0)
            {
                flag2 = true;
                if (Event.current.type == EventType.KeyDown)
                {
                    Event.current.Use();
                }
            }
            else if (Event.current.type == EventType.KeyDown && (FPPQuickbarControls.IsQuickbarOrSpellHotkey(Event.current.keyCode) || KeyCodeUtility.CancelJustPressed))
            {
                Get.UI.CloseWheelSelector(true);
                Event.current.Use();
            }
        IL_0A3F:
            if (flag2 && this.mouseoverIndex >= 0 && !this.options[this.mouseoverIndex].Disabled)
            {
                try
                {
                    this.options[this.mouseoverIndex].Action();
                }
                catch (Exception ex)
                {
                    Log.Error("Error while executing wheel selector option action.", ex);
                }
                if (Get.WheelSelector == this)
                {
                    Get.UI.CloseWheelSelector(false);
                }
            }
        }

        private float GetBaseAngle(int index)
        {
            return 360f / (float)this.options.Count * (float)index - 90f;
        }

        private List<WheelSelector.Option> options;

        private ITipSubject subject;

        private string extraText;

        private bool isSpellChooser;

        private bool closeIfItemNotInInventory;

        private bool isQuickbarMenu;

        private List<float> mouseoverAnimationPct;

        private float distToOption = 50f;

        private float fadeInAlpha;

        private float animatedAngle = 20f;

        private int mouseoverIndex = -1;

        private int? firstMouseUpFrame;

        private const float InitialDistToOption = 50f;

        private const float TargetDistToOption = 190f;

        private const float DistToOptionLerpSpeed = 17.83374f;

        private const float AlphaLerpSpeed = 11.15718f;

        private const float AnimatedAngle = 20f;

        private const float AnimatedAngleLerpSpeed = 11.15718f;

        private const float MouseoverAnimationLerpSpeed = 11.15718f;

        private const float MouseoverBumpDist = 10f;

        private const int SubjectIconSize = 140;

        private const float InnerRadius = 30f;

        private const float Radius = 305f;

        public struct Option
        {
            public string Label
            {
                get
                {
                    return this.label;
                }
            }

            public Action Action
            {
                get
                {
                    return this.action;
                }
            }

            public string Tip
            {
                get
                {
                    return this.tip;
                }
            }

            public ITipSubject TipSubject
            {
                get
                {
                    return this.tipSubject;
                }
            }

            public bool Disabled
            {
                get
                {
                    return this.action == null;
                }
            }

            public Option(string label, Action action, string tip = null, ITipSubject tipSubject = null)
            {
                this.label = label;
                this.action = action;
                this.tip = tip;
                this.tipSubject = tipSubject;
            }

            private string label;

            private Action action;

            private string tip;

            private ITipSubject tipSubject;
        }
    }
}