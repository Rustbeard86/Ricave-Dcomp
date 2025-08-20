using System;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class KeyBindingSpec : Spec
    {
        public float UIOrder
        {
            get
            {
                return this.uiOrder;
            }
        }

        public KeyCode DefaultKeyCode
        {
            get
            {
                if (this.defaultKeyCodeLaptop != null && Sys.IsLaptop)
                {
                    return this.defaultKeyCodeLaptop.Value;
                }
                return this.defaultKeyCode;
            }
        }

        public KeyCode KeyCode
        {
            get
            {
                if (this.cachedKeyCode == null)
                {
                    string @string = Prefs.GetString(this.PrefsKey, null);
                    KeyCode keyCode;
                    if (!@string.NullOrEmpty() && Enum.TryParse<KeyCode>(@string, out keyCode))
                    {
                        this.cachedKeyCode = new KeyCode?(keyCode);
                    }
                    else
                    {
                        this.cachedKeyCode = new KeyCode?(this.DefaultKeyCode);
                    }
                }
                return this.cachedKeyCode.Value;
            }
        }

        public bool JustPressed
        {
            get
            {
                bool flag;
                if (Event.current == null)
                {
                    flag = this.IsOurKeyCodeDown();
                }
                else
                {
                    flag = (Event.current.type == EventType.KeyDown && this.IsOurKeyCode(Event.current.keyCode) && (this.IsOurKeyCodeDown() || GUIExtra.AnyTextFieldIsFocused)) || (Event.current.type == EventType.MouseDown && this.IsOurKeyCode(KeyCodeUtility.MouseButtonToKeyCode(Event.current.button)));
                }
                if (flag && FPPQuickbarControls.IsActivelyUsedSpellHotkey(this.KeyCode))
                {
                    flag = false;
                }
                if (flag)
                {
                    this.lastNewPressTime = Clock.UnscaledTime;
                    this.caughtPress = true;
                    return true;
                }
                if (this.allowRepeatWhenHeldDown)
                {
                    if (!this.IsOurKeyCodeHeld())
                    {
                        this.caughtPress = false;
                    }
                    if (this.caughtPress && this.IsOurKeyCodeHeld() && !this.IsOurKeyCodeDown() && Clock.UnscaledTime - this.lastNewPressTime > 0.5f && Clock.UnscaledTime - this.lastRepeatTime > 0.15f && (Clock.UnscaledTime - this.lastNewPressTime < 4f || Clock.UnscaledTime - this.lastRepeatTime < 4f))
                    {
                        this.lastRepeatTime = Clock.UnscaledTime;
                        return true;
                    }
                }
                return false;
            }
        }

        public bool JustReleased
        {
            get
            {
                bool flag;
                if (Event.current == null)
                {
                    flag = this.IsOurKeyCodeUp();
                }
                else
                {
                    flag = (Event.current.type == EventType.KeyUp && this.IsOurKeyCode(Event.current.keyCode)) || (Event.current.type == EventType.MouseUp && this.IsOurKeyCode(KeyCodeUtility.MouseButtonToKeyCode(Event.current.button)));
                }
                if (flag && FPPQuickbarControls.IsActivelyUsedSpellHotkey(this.KeyCode))
                {
                    flag = false;
                }
                return flag;
            }
        }

        public bool HeldDown
        {
            get
            {
                bool flag = this.IsOurKeyCodeHeld() || ((this.KeyCode == KeyCode.LeftShift || this.KeyCode == KeyCode.RightShift) && Event.current != null && Event.current.shift) || ((this.KeyCode == KeyCode.LeftControl || this.KeyCode == KeyCode.RightControl) && Event.current != null && Event.current.control) || ((this.KeyCode == KeyCode.LeftAlt || this.KeyCode == KeyCode.RightAlt) && Event.current != null && Event.current.alt);
                if (flag && FPPQuickbarControls.IsActivelyUsedSpellHotkey(this.KeyCode))
                {
                    flag = false;
                }
                return flag;
            }
        }

        public Texture2D Glyph
        {
            get
            {
                KeyCode keyCode2;
                KeyCode keyCode3;
                KeyCode keyCode = ControllerUtility.RemapKeyboardToController(this.KeyCode, out keyCode2, out keyCode3);
                if (keyCode != KeyCode.None)
                {
                    return ControllerUtility.GetGlyph(keyCode);
                }
                return ControllerUtility.GetGlyph(this.KeyCode);
            }
        }

        private string PrefsKey
        {
            get
            {
                return "KeyBinding_" + base.SpecID;
            }
        }

        public void SetKeyCode(KeyCode keyCode, bool save = true)
        {
            KeyCode keyCode2 = keyCode;
            KeyCode? keyCode3 = this.cachedKeyCode;
            if ((keyCode2 == keyCode3.GetValueOrDefault()) & (keyCode3 != null))
            {
                return;
            }
            Prefs.SetString(this.PrefsKey, keyCode.ToString());
            if (save)
            {
                KeyBindingSpec.Save();
            }
            this.cachedKeyCode = new KeyCode?(keyCode);
            KeyCodeUtility.OnKeyBindingChanged();
        }

        public void RestoreDefaultKeyCode(bool save = true)
        {
            Prefs.DeleteKey(this.PrefsKey);
            if (save)
            {
                KeyBindingSpec.Save();
            }
            this.cachedKeyCode = new KeyCode?(this.DefaultKeyCode);
            KeyCodeUtility.OnKeyBindingChanged();
        }

        public static void Save()
        {
            Prefs.Save();
        }

        private bool IsOurKeyCode(KeyCode keyCode)
        {
            if (keyCode == KeyCode.None)
            {
                return false;
            }
            if (keyCode == this.KeyCode)
            {
                return true;
            }
            if (ControllerUtility.InControllerMode)
            {
                KeyCode keyCode2 = ControllerUtility.RemapControllerToKeyboard(keyCode);
                if (keyCode2 != KeyCode.None && keyCode2 == this.KeyCode)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsOurKeyCodeDown()
        {
            if (this.KeyCode == KeyCode.None)
            {
                return false;
            }
            if (Input.GetKeyDown(this.KeyCode))
            {
                return true;
            }
            if (ControllerUtility.InControllerMode)
            {
                KeyCode keyCode2;
                KeyCode keyCode3;
                KeyCode keyCode = ControllerUtility.RemapKeyboardToController(this.KeyCode, out keyCode2, out keyCode3);
                if ((keyCode != KeyCode.None && Input.GetKeyDown(keyCode)) || (keyCode2 != KeyCode.None && Input.GetKeyDown(keyCode2)) || (keyCode3 != KeyCode.None && Input.GetKeyDown(keyCode3)))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsOurKeyCodeHeld()
        {
            if (this.KeyCode == KeyCode.None)
            {
                return false;
            }
            if (Input.GetKey(this.KeyCode))
            {
                return true;
            }
            if (ControllerUtility.InControllerMode)
            {
                KeyCode keyCode2;
                KeyCode keyCode3;
                KeyCode keyCode = ControllerUtility.RemapKeyboardToController(this.KeyCode, out keyCode2, out keyCode3);
                if ((keyCode != KeyCode.None && Input.GetKey(keyCode)) || (keyCode2 != KeyCode.None && Input.GetKey(keyCode2)) || (keyCode3 != KeyCode.None && Input.GetKey(keyCode3)))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsOurKeyCodeUp()
        {
            if (this.KeyCode == KeyCode.None)
            {
                return false;
            }
            if (Input.GetKeyUp(this.KeyCode))
            {
                return true;
            }
            if (ControllerUtility.InControllerMode)
            {
                KeyCode keyCode2;
                KeyCode keyCode3;
                KeyCode keyCode = ControllerUtility.RemapKeyboardToController(this.KeyCode, out keyCode2, out keyCode3);
                if ((keyCode != KeyCode.None && Input.GetKeyUp(keyCode)) || (keyCode2 != KeyCode.None && Input.GetKeyUp(keyCode2)) || (keyCode3 != KeyCode.None && Input.GetKeyUp(keyCode3)))
                {
                    return true;
                }
            }
            return false;
        }

        [Saved]
        private KeyCode defaultKeyCode;

        [Saved]
        private KeyCode? defaultKeyCodeLaptop;

        [Saved]
        private bool allowRepeatWhenHeldDown;

        [Saved]
        private float uiOrder;

        private KeyCode? cachedKeyCode;

        private float lastNewPressTime;

        private float lastRepeatTime;

        private bool caughtPress;

        private const float RepeatAfterTime = 0.5f;

        private const float TimeBetweenRepeats = 0.15f;
    }
}