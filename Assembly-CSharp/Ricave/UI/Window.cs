using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public abstract class Window
    {
        public static Window Current
        {
            get
            {
                return Window.current;
            }
        }

        public WindowSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public int ID
        {
            get
            {
                return this.myID;
            }
        }

        public Rect Rect { get; protected set; }

        public bool IsOpen
        {
            get
            {
                return Get.WindowManager.IsOpen(this);
            }
        }

        protected virtual string Title
        {
            get
            {
                return this.spec.LabelCap;
            }
        }

        protected virtual Color BackgroundColor
        {
            get
            {
                return Window.DefaultBackgroundColor;
            }
        }

        protected virtual bool CloseOnEscapeKey
        {
            get
            {
                return this.spec.CloseOnEscapeKey;
            }
        }

        public float OpenAnimationOffset
        {
            get
            {
                return this.openAnimationOffset;
            }
        }

        public Window(WindowSpec spec, int ID)
        {
            this.spec = spec;
            this.myID = ID;
            this.cachedWindowFunction = new GUI.WindowFunction(this.WindowFunction);
        }

        public void OnGUI()
        {
            this.Rect = GUI.Window(this.ID, this.Rect, this.cachedWindowFunction, "");
        }

        protected abstract void DoWindowContents(Rect inRect);

        protected virtual void DoExtraTitleGUI(Rect titleRect)
        {
        }

        public void StartOpenAnimation()
        {
            this.openAnimationOffset = 8f;
            this.SetInitialRect();
            this.Rect = this.Rect.MovedBy(0f, this.openAnimationOffset);
        }

        public virtual void ExtraOnGUI()
        {
            if (Event.current.type == EventType.Repaint && this.openAnimationOffset > 0f)
            {
                this.openAnimationOffset -= Clock.UnscaledDeltaTime * 220f;
                if (this.openAnimationOffset < 0f)
                {
                    this.openAnimationOffset = 0f;
                }
                this.SetInitialRect();
                this.Rect = this.Rect.MovedBy(0f, this.openAnimationOffset);
            }
        }

        public virtual void OnOpen()
        {
            this.SetInitialRect();
            if (!Get.InMainMenu)
            {
                Get.UI.OnWindowOpened();
            }
        }

        public virtual void SetInitialRect()
        {
            this.Rect = new Rect((float)Calc.RoundToInt((Widgets.VirtualWidth - this.spec.Size.x) / 2f + this.spec.Offset.x), (float)Calc.RoundToInt((Widgets.VirtualHeight - this.spec.Size.y) / 2f + this.spec.Offset.y), this.spec.Size.x, this.spec.Size.y);
        }

        public virtual void OnClosed()
        {
            if (!Get.InMainMenu)
            {
                Get.UI.OnWindowClosed();
            }
        }

        private void WindowFunction(int unused)
        {
            Window.current = this;
            SteamDeckUtility.CheckFixMousePosition();
            Profiler.Begin(this.Spec.LabelCap);
            try
            {
                if ((this.CloseOnEscapeKey && KeyCodeUtility.CancelJustPressed) || (this.spec.CloseOnEnterKey && Get.KeyBinding_Accept.JustPressed))
                {
                    Get.WindowManager.Close(this, true);
                    if (Event.current.type == EventType.KeyDown)
                    {
                        Event.current.Use();
                    }
                }
                else
                {
                    Get.WindowManager.OnWindowFunctionCalled(this);
                    GUIExtra.DrawRoundedRectBump(this.Rect.AtZero(), this.BackgroundColor, false, true, true, true, true, null);
                    Rect rect = new Rect(this.spec.TitlePadding, 0f, this.Rect.width - this.spec.TitlePadding * 2f, 30f);
                    GUIExtra.DrawRoundedRectBump(new Rect(0f, 0f, this.Rect.width, 30f), new Color(0.39f, 0.37f, 0.33f), false, true, true, false, false, null);
                    Widgets.FontBold = true;
                    Widgets.Align = TextAnchor.MiddleCenter;
                    Widgets.WordWrap = false;
                    Widgets.Label(rect, this.Title, true, this.Spec.Description, null, false);
                    Widgets.WordWrap = true;
                    Widgets.ResetAlign();
                    Widgets.FontBold = false;
                    this.DoExtraTitleGUI(rect);
                    Rect rect2 = new Rect(0f, 30f, this.Rect.width, this.Rect.height - 30f).ContractedBy(this.spec.Padding);
                    GUIExtra.BeginGroup(rect2);
                    try
                    {
                        this.DoWindowContents(rect2.AtZero());
                    }
                    catch (Exception ex)
                    {
                        string text = "Error while filling \"";
                        Type type = base.GetType();
                        Log.Error(text + ((type != null) ? type.ToString() : null) + "\" window contents.", ex);
                    }
                    GUIExtra.EndGroup();
                    if (this.spec.CanDragWindow)
                    {
                        GUI.DragWindow();
                    }
                }
            }
            finally
            {
                Window.current = null;
                Profiler.End();
            }
        }

        protected Rect MainArea(Rect windowRect)
        {
            Rect rect = new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height - Window.BottomButtonSize.y - this.spec.Padding);
            if (this.spec.Padding == 0f)
            {
                rect.height -= 40f;
            }
            return rect;
        }

        protected bool DoBottomButton(string text, Rect windowRect, int index, int count, bool clickSound = true, Color? labelColor = null)
        {
            return Widgets.Button(this.GetBottomButtonRect(windowRect, index, count), text, clickSound, null, labelColor, true, true, true, true, null, true, true, null, false, null, null);
        }

        protected Rect GetBottomButtonRect(Rect windowRect, int index, int count)
        {
            float num = (float)count * Window.BottomButtonSize.x + (float)(count - 1) * 20f;
            float num2 = windowRect.center.x - num / 2f + (float)index * (Window.BottomButtonSize.x + 20f);
            float num3 = windowRect.yMax - Window.BottomButtonSize.y;
            if (this.spec.Padding == 0f)
            {
                num3 -= 20f;
            }
            return new Rect(num2, num3, Window.BottomButtonSize.x, Window.BottomButtonSize.y);
        }

        private WindowSpec spec;

        private int myID;

        private GUI.WindowFunction cachedWindowFunction;

        private float openAnimationOffset;

        private static Window current;

        protected const int TitleBarHeight = 30;

        public const float InitialOpenAnimationOffset = 8f;

        private static readonly Vector2 BottomButtonSize = new Vector2(160f, 45f);

        public static readonly Color DefaultBackgroundColor = new Color(0.118f, 0.118f, 0.118f);

        public const float OpenAnimationSpeed = 220f;
    }
}