using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class WindowManager
    {
        public List<Window> Windows
        {
            get
            {
                return this.windows;
            }
        }

        public bool AnyWindowOpen
        {
            get
            {
                return this.windows.Count != 0;
            }
        }

        public Window FocusedWindow
        {
            get
            {
                return this.focusedWindow;
            }
        }

        public bool IsContextMenuOpen
        {
            get
            {
                return this.IsOpen(Get.Window_ContextMenu);
            }
        }

        public bool IsOpen(WindowSpec spec)
        {
            if (spec == null)
            {
                return false;
            }
            for (int i = 0; i < this.windows.Count; i++)
            {
                if (this.windows[i].Spec == spec)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOpen(Window window)
        {
            return window != null && this.windows.Contains(window);
        }

        public Window Open(WindowSpec spec, bool playSound = true)
        {
            if (spec == null)
            {
                Log.Error("Tried to open a window with null spec.", false);
                return null;
            }
            Type windowType = spec.WindowType;
            object[] array = new object[2];
            array[0] = spec;
            int num = 1;
            int num2 = this.nextWindowID;
            this.nextWindowID = num2 + 1;
            array[num] = num2;
            Window window = (Window)Activator.CreateInstance(windowType, array);
            this.Open(window, playSound);
            return window;
        }

        public void Open(Window window, bool playSound = true)
        {
            if (window == null)
            {
                Log.Error("Tried to open a null window.", false);
                return;
            }
            if (this.IsOpen(window))
            {
                return;
            }
            if (window.Spec.MaxOne)
            {
                this.CloseAll(window.Spec);
            }
            this.windows.Add(window);
            this.FocusWindow(window);
            window.OnOpen();
            if (playSound)
            {
                Get.Sound_OpenWindow.PlayOneShot(null, 1f, 1f);
            }
        }

        public void FocusWindow(Window window)
        {
            GUI.FocusWindow(window.ID);
            this.focusedWindow = window;
        }

        public Window_ContextMenu OpenContextMenu(List<ValueTuple<string, Action>> options, string title)
        {
            List<Window_ContextMenu.Option> list = new List<Window_ContextMenu.Option>();
            for (int i = 0; i < options.Count; i++)
            {
                list.Add(new Window_ContextMenu.Option(options[i].Item1, options[i].Item2, null));
            }
            return this.OpenContextMenu(list, title);
        }

        public Window_ContextMenu OpenContextMenu(List<ValueTuple<string, Action, string>> options, string title)
        {
            List<Window_ContextMenu.Option> list = new List<Window_ContextMenu.Option>();
            for (int i = 0; i < options.Count; i++)
            {
                list.Add(new Window_ContextMenu.Option(options[i].Item1, options[i].Item2, options[i].Item3));
            }
            return this.OpenContextMenu(list, title);
        }

        public Window_ContextMenu OpenContextMenu(List<Window_ContextMenu.Option> options, string title)
        {
            Window_ContextMenu window_ContextMenu = (Window_ContextMenu)this.Open(Get.Window_ContextMenu, true);
            window_ContextMenu.Init(title, options);
            return window_ContextMenu;
        }

        public Window_Confirmation OpenConfirmationWindow(string text, Action onConfirmed, bool onlyOKButton = false, string customTitle = null, Action onRejected = null)
        {
            Window_Confirmation window_Confirmation = (Window_Confirmation)this.Open(Get.Window_Confirmation, true);
            window_Confirmation.Init(text, onConfirmed, onlyOKButton, customTitle, onRejected);
            return window_Confirmation;
        }

        public Window_Confirmation OpenMessageWindow(string text, string customTitle = null)
        {
            return this.OpenConfirmationWindow(text, null, true, customTitle ?? "MessageTitle".Translate(), null);
        }

        public void Close(Window window, bool playSound = true)
        {
            if (window == null)
            {
                Log.Error("Tried to close a null window.", false);
                return;
            }
            if (!this.IsOpen(window))
            {
                return;
            }
            this.windows.Remove(window);
            if (this.focusedWindow == window)
            {
                this.focusedWindow = ((this.windows.Count != 0) ? this.windows[this.windows.Count - 1] : null);
            }
            window.OnClosed();
            if (playSound)
            {
                Get.Sound_CloseWindow.PlayOneShot(null, 1f, 1f);
            }
        }

        public void CloseAll()
        {
            for (int i = this.windows.Count - 1; i >= 0; i--)
            {
                this.Close(this.windows[i], false);
            }
        }

        public void CloseAll(WindowSpec spec)
        {
            if (spec == null)
            {
                Log.Error("Tried to close all windows of null spec.", false);
                return;
            }
            for (int i = this.windows.Count - 1; i >= 0; i--)
            {
                if (this.windows[i].Spec == spec)
                {
                    this.Close(this.windows[i], false);
                }
            }
        }

        public Window GetWindowAt(Vector2 pos)
        {
            for (int i = this.windows.Count - 1; i >= 0; i--)
            {
                if (this.windows[i].Rect.Contains(pos))
                {
                    return this.windows[i];
                }
            }
            return null;
        }

        public Window GetFirstWindow(WindowSpec spec)
        {
            for (int i = 0; i < this.windows.Count; i++)
            {
                if (this.windows[i].Spec == spec)
                {
                    return this.windows[i];
                }
            }
            return null;
        }

        public void OnGUI()
        {
            if (this.focusedWindow != null)
            {
                this.windows.Remove(this.focusedWindow);
                this.windows.Add(this.focusedWindow);
                this.FocusWindow(this.focusedWindow);
            }
            this.tmpWindows.Clear();
            this.tmpWindows.AddRange(this.windows);
            for (int i = 0; i < this.tmpWindows.Count; i++)
            {
                if (this.tmpWindows[i].Spec.CloseOnLostFocus && this.focusedWindow != this.tmpWindows[i])
                {
                    this.Close(this.tmpWindows[i], true);
                }
            }
            for (int j = 0; j < this.tmpWindows.Count; j++)
            {
                if (this.IsOpen(this.tmpWindows[j]))
                {
                    try
                    {
                        this.tmpWindows[j].OnGUI();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error in Window.OnGUI().", ex);
                    }
                }
            }
            for (int k = this.tmpWindows.Count - 1; k >= 0; k--)
            {
                if (this.IsOpen(this.tmpWindows[k]))
                {
                    try
                    {
                        this.tmpWindows[k].ExtraOnGUI();
                    }
                    catch (Exception ex2)
                    {
                        Log.Error("Error in Window.ExtraOnGUI().", ex2);
                    }
                }
            }
        }

        public void LateOnGUI()
        {
            for (int i = 0; i < this.tmpWindows.Count; i++)
            {
                GUIExtra.DrawShadowAround(this.tmpWindows[i].Rect, 0.15f, 25f, false);
            }
        }

        public void CheckCloseWindowsOnClickedOutside()
        {
            if (Event.current.type == EventType.MouseDown)
            {
                Window firstWindow = this.GetFirstWindow(Get.Window_ContextMenu);
                if (firstWindow != null)
                {
                    Vector2 vector = GUIUtility.GUIToScreenPoint(Event.current.mousePosition / Widgets.UIScale);
                    if (!firstWindow.Rect.Contains(vector))
                    {
                        this.Close(firstWindow, true);
                    }
                }
            }
        }

        public void OnUIScaleChanged()
        {
            for (int i = 0; i < this.windows.Count; i++)
            {
                this.windows[i].SetInitialRect();
            }
        }

        public void OnResolutionChanged()
        {
            for (int i = 0; i < this.windows.Count; i++)
            {
                this.windows[i].SetInitialRect();
            }
        }

        public void OnWindowFunctionCalled(Window window)
        {
            if (Event.current.type == EventType.Layout && this.lastFocusedWindowCheckFrame != Clock.Frame)
            {
                this.lastFocusedWindowCheckFrame = Clock.Frame;
                this.focusedWindow = window;
            }
        }

        private List<Window> windows = new List<Window>();

        private int nextWindowID;

        private Window focusedWindow;

        private List<Window> tmpWindows = new List<Window>();

        private int lastFocusedWindowCheckFrame = -1;
    }
}