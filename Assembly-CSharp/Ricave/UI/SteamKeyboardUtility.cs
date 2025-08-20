using System;
using Ricave.Core;
using Steamworks;
using UnityEngine;

namespace Ricave.UI
{
    public static class SteamKeyboardUtility
    {
        public static bool ShowingKeyboard
        {
            get
            {
                return SteamKeyboardUtility.showingKeyboard;
            }
        }

        public static void Init()
        {
            SteamKeyboardUtility.keyboardDismissedCallback = Callback<FloatingGamepadTextInputDismissed_t>.Create(new Callback<FloatingGamepadTextInputDismissed_t>.DispatchDelegate(SteamKeyboardUtility.KeyboardDismissedCallback));
        }

        public static bool TryShowKeyboard(Rect textFieldRect)
        {
            if (!ControllerUtility.InControllerMode || !SteamManager.Initialized || SteamKeyboardUtility.showingKeyboard)
            {
                return false;
            }
            Rect rect = GUIUtility.GUIToScreenRect(textFieldRect);
            if (SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height))
            {
                SteamKeyboardUtility.showingKeyboard = true;
                return true;
            }
            return false;
        }

        public static void HideKeyboard()
        {
            if (SteamKeyboardUtility.showingKeyboard)
            {
                SteamUtils.DismissFloatingGamepadTextInput();
            }
        }

        private static void KeyboardDismissedCallback(FloatingGamepadTextInputDismissed_t data)
        {
            SteamKeyboardUtility.showingKeyboard = false;
            SteamKeyboardUtility.unfocusCurrentTextField = true;
        }

        public static void OnGUI()
        {
            if (!ControllerUtility.InControllerMode || !SteamManager.Initialized)
            {
                return;
            }
            if (SteamKeyboardUtility.unfocusCurrentTextField && Event.current.type == EventType.Repaint)
            {
                GUI.SetNextControlName("dummy");
                GUI.TextField(default(Rect), "");
                GUI.FocusControl("dummy");
                TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                if (textEditor != null)
                {
                    SteamKeyboardUtility.lastFocusedTextFieldID = textEditor.controlID;
                    SteamKeyboardUtility.lastFocusedTextFieldCursorIndex = textEditor.cursorIndex;
                }
                SteamKeyboardUtility.unfocusCurrentTextField = false;
            }
            if (Event.current.type == EventType.Repaint)
            {
                TextEditor textEditor2 = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                if (textEditor2 != null && (textEditor2.controlID != SteamKeyboardUtility.lastFocusedTextFieldID || textEditor2.cursorIndex != SteamKeyboardUtility.lastFocusedTextFieldCursorIndex) && textEditor2.position != default(Rect))
                {
                    SteamKeyboardUtility.lastFocusedTextFieldID = textEditor2.controlID;
                    SteamKeyboardUtility.lastFocusedTextFieldCursorIndex = textEditor2.cursorIndex;
                    SteamKeyboardUtility.TryShowKeyboard(textEditor2.position);
                    return;
                }
                if (SteamKeyboardUtility.showingKeyboard && (textEditor2 == null || textEditor2.position == default(Rect)))
                {
                    SteamKeyboardUtility.HideKeyboard();
                }
            }
        }

        public static void OnTextFieldLikelyNoLongerFocused()
        {
            if (!ControllerUtility.InControllerMode || !SteamManager.Initialized)
            {
                return;
            }
            if (SteamKeyboardUtility.showingKeyboard)
            {
                SteamKeyboardUtility.HideKeyboard();
            }
        }

        private static bool showingKeyboard;

        private static int lastFocusedTextFieldID;

        private static int lastFocusedTextFieldCursorIndex;

        private static bool unfocusCurrentTextField;

        private static Callback<FloatingGamepadTextInputDismissed_t> keyboardDismissedCallback;
    }
}