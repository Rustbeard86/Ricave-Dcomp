using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_ContextMenu : Window
    {
        protected override string Title
        {
            get
            {
                return this.title;
            }
        }

        private float HeightPerOption
        {
            get
            {
                return (float)((this.options.Count > 10) ? 25 : 50);
            }
        }

        public Window_ContextMenu(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.openTime = Clock.UnscaledTime;
        }

        protected override void DoWindowContents(Rect inRect)
        {
            bool flag = false;
            for (int i = 0; i < this.options.Count; i++)
            {
                Rect rect = new Rect(inRect.x, inRect.y + (float)i * this.HeightPerOption, base.Rect.width, this.HeightPerOption);
                if (Event.current.type == EventType.Repaint && Mouse.Over(rect) && !this.options[i].Disabled)
                {
                    if (this.lastMouseoverOption != i)
                    {
                        Get.Sound_Hover.PlayOneShot(null, 1f, 1f);
                    }
                    this.lastMouseoverOption = i;
                    flag = true;
                }
                float num = Calc.Clamp01((Clock.UnscaledTime - this.openTime - (float)i / (float)this.options.Count * 0.14f + 0.02f) / 0.18f);
                Color color;
                Color color2;
                if (this.options[i].Disabled)
                {
                    color = new Color(0.3f, 0.15f, 0.15f, 0.5f * num);
                    color2 = new Color(0.8f, 0.8f, 0.8f, num);
                }
                else
                {
                    color = GUIExtra.HighlightedColorIfMouseover(rect, Widgets.DefaultButtonColor.WithAlpha(0.5f * num), true, 0.4f);
                    color2 = new Color(1f, 1f, 1f, num);
                }
                GUIExtra.DrawRoundedRectBump(rect, color, false, i == 0, i == 0, i == this.options.Count - 1, i == this.options.Count - 1, null);
                GUI.color = color2;
                Widgets.LabelCenteredV(new Vector2(rect.x + 20f, rect.center.y), this.options[i].Label, true, null, null, false);
                GUI.color = Color.white;
                if (!this.options[i].Tip.NullOrEmpty())
                {
                    Get.Tooltips.RegisterTip(rect, this.options[i].Tip, null);
                }
                if (Widgets.ButtonInvisible(rect, false, false) && Event.current.button == 0 && !this.options[i].Disabled)
                {
                    try
                    {
                        this.options[i].Action();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error while executing context menu option action.", ex);
                    }
                    Get.WindowManager.Close(this, false);
                    return;
                }
            }
            if (Event.current.type == EventType.Repaint && !flag)
            {
                this.lastMouseoverOption = -1;
            }
        }

        public void Init(string title, List<Window_ContextMenu.Option> options)
        {
            if (options.Count == 0)
            {
                options.Add(new Window_ContextMenu.Option(RichText.Grayed("(" + "NoOptions".Translate() + ")"), delegate
                {
                }, null));
            }
            this.title = title;
            this.options = options;
            this.initialMousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition / Widgets.UIScale);
            this.SetInitialRect();
        }

        public override void SetInitialRect()
        {
            Vector2 vector = new Vector2(this.initialMousePos.x + 1f, this.initialMousePos.y - 30f - 4f);
            float num = base.Spec.Size.x;
            Widgets.FontBold = true;
            num = Math.Max(num, Widgets.CalcSize(this.Title).x + base.Spec.TitlePadding * 2f + 1f);
            Widgets.FontBold = false;
            for (int i = 0; i < this.options.Count; i++)
            {
                num = Math.Max(num, Widgets.CalcSize(this.options[i].Label).x + 40f + 1f);
            }
            Vector2 vector2 = new Vector2(num, 30f + (float)this.options.Count * this.HeightPerOption);
            bool flag = false;
            if (vector.x + vector2.x > Widgets.VirtualWidth)
            {
                vector.x = this.initialMousePos.x - vector2.x;
                flag = true;
            }
            if (vector.y + vector2.y > Widgets.VirtualHeight)
            {
                if (this.initialMousePos.y - vector2.y >= 0f)
                {
                    vector.y = this.initialMousePos.y - vector2.y;
                    if (!flag)
                    {
                        vector.x = this.initialMousePos.x;
                    }
                }
                else
                {
                    vector.y = Widgets.VirtualHeight - vector2.y;
                }
            }
            base.Rect = new Rect(vector, vector2);
        }

        private Vector2 initialMousePos;

        private List<Window_ContextMenu.Option> options = new List<Window_ContextMenu.Option>();

        private string title;

        private int lastMouseoverOption = -1;

        private float openTime;

        private const int HeightPerOptionNormal = 50;

        private const int HeightPerOptionCompact = 25;

        private const int Padding = 20;

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

            public bool Disabled
            {
                get
                {
                    return this.action == null;
                }
            }

            public Option(string label, Action action, string tip = null)
            {
                this.label = label;
                this.action = action;
                this.tip = tip;
            }

            private string label;

            private Action action;

            private string tip;
        }
    }
}