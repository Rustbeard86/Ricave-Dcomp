using System;
using System.Text.RegularExpressions;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class KeyCodeUtility
    {
        public static bool InspectHeldDown
        {
            get
            {
                return Get.KeyBinding_Inspect.HeldDown || Get.KeyBinding_InspectAlt.HeldDown;
            }
        }

        public static bool CancelJustPressed
        {
            get
            {
                return Get.KeyBinding_Cancel.JustPressed || (ControllerUtility.InControllerMode && Get.KeyBinding_RotateRight.JustPressed && !SteamKeyboardUtility.ShowingKeyboard);
            }
        }

        public static bool FlyKeyModifierHeldDown
        {
            get
            {
                return Get.KeyBinding_Fly.HeldDown || (ControllerUtility.InControllerMode && ControllerUtility.ControllerType != ControllerType.SteamDeck && Input.GetMouseButton(1));
            }
        }

        public static int ToMouseButton(this KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Mouse0:
                    return 0;
                case KeyCode.Mouse1:
                    return 1;
                case KeyCode.Mouse2:
                    return 2;
                case KeyCode.Mouse3:
                    return 3;
                case KeyCode.Mouse4:
                    return 4;
                case KeyCode.Mouse5:
                    return 5;
                case KeyCode.Mouse6:
                    return 6;
                default:
                    return -1;
            }
        }

        public static KeyCode MouseButtonToKeyCode(int button)
        {
            switch (button)
            {
                case 0:
                    return KeyCode.Mouse0;
                case 1:
                    return KeyCode.Mouse1;
                case 2:
                    return KeyCode.Mouse2;
                case 3:
                    return KeyCode.Mouse3;
                case 4:
                    return KeyCode.Mouse4;
                case 5:
                    return KeyCode.Mouse5;
                case 6:
                    return KeyCode.Mouse6;
                default:
                    return KeyCode.None;
            }
        }

        public static KeyCode DigitToKeyCode(this int digit)
        {
            switch (digit)
            {
                case 0:
                    return KeyCode.Alpha0;
                case 1:
                    return KeyCode.Alpha1;
                case 2:
                    return KeyCode.Alpha2;
                case 3:
                    return KeyCode.Alpha3;
                case 4:
                    return KeyCode.Alpha4;
                case 5:
                    return KeyCode.Alpha5;
                case 6:
                    return KeyCode.Alpha6;
                case 7:
                    return KeyCode.Alpha7;
                case 8:
                    return KeyCode.Alpha8;
                case 9:
                    return KeyCode.Alpha9;
                default:
                    return KeyCode.None;
            }
        }

        public static bool IsAlpha(this KeyCode keyCode)
        {
            return keyCode == KeyCode.Alpha0 || keyCode == KeyCode.Alpha1 || keyCode == KeyCode.Alpha2 || keyCode == KeyCode.Alpha3 || keyCode == KeyCode.Alpha4 || keyCode == KeyCode.Alpha5 || keyCode == KeyCode.Alpha6 || keyCode == KeyCode.Alpha7 || keyCode == KeyCode.Alpha8 || keyCode == KeyCode.Alpha9;
        }

        public static string ToStringCached(this KeyCode keyCode)
        {
            if (KeyCodeUtility.keyCodesToString == null)
            {
                KeyCodeUtility.keyCodesToString = new string[EnumUtility.GetMaxValue<KeyCode>() + 1];
            }
            bool flag = false;
            if (ControllerUtility.InControllerMode)
            {
                if (keyCode == KeyCode.A || keyCode == KeyCode.LeftArrow)
                {
                    return "◀";
                }
                if (keyCode == KeyCode.D || keyCode == KeyCode.RightArrow)
                {
                    return "▶";
                }
                if (keyCode == KeyCode.W || keyCode == KeyCode.UpArrow)
                {
                    return "▲";
                }
                if (keyCode == KeyCode.S || keyCode == KeyCode.DownArrow)
                {
                    return "▼";
                }
                if (keyCode == KeyCode.Mouse0)
                {
                    return "ControllerKey_RT".Translate();
                }
                if (keyCode == KeyCode.Mouse1)
                {
                    return "ControllerKey_LT".Translate();
                }
                KeyCode keyCode3;
                KeyCode keyCode4;
                KeyCode keyCode2 = ControllerUtility.RemapKeyboardToController(keyCode, out keyCode3, out keyCode4);
                if (keyCode2 != KeyCode.None)
                {
                    keyCode = keyCode2;
                }
                else
                {
                    flag = true;
                }
            }
            int num = (int)keyCode;
            if (num < 0 || num >= KeyCodeUtility.keyCodesToString.Length)
            {
                return "error";
            }
            if (KeyCodeUtility.keyCodesToString[num] == null)
            {
                KeyCodeUtility.keyCodesToString[num] = KeyCodeUtility.GetProcessedString(keyCode);
            }
            string text = KeyCodeUtility.keyCodesToString[num];
            if (flag && keyCode != KeyCode.None)
            {
                text = "{0} ({1})".Formatted("UnassignedKeyShort".Translate(), text);
            }
            return text;
        }

        public static void ResetKeyCodesStrings()
        {
            KeyCodeUtility.formattedWithKeyBindingsCache.Clear();
            if (KeyCodeUtility.keyCodesToString == null)
            {
                return;
            }
            for (int i = 0; i < KeyCodeUtility.keyCodesToString.Length; i++)
            {
                KeyCodeUtility.keyCodesToString[i] = null;
            }
        }

        private static string GetProcessedString(KeyCode keyCode)
        {
            if (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
            {
                return (keyCode - KeyCode.Alpha0).ToStringCached();
            }
            if (keyCode >= KeyCode.JoystickButton0 && keyCode <= KeyCode.JoystickButton13)
            {
                return ControllerUtility.GetControllerKeyLabel(keyCode);
            }
            if (keyCode <= KeyCode.Return)
            {
                if (keyCode == KeyCode.None)
                {
                    return "KeyCode_None".Translate();
                }
                if (keyCode == KeyCode.Return)
                {
                    return "KeyCode_Return".Translate();
                }
            }
            else
            {
                if (keyCode == KeyCode.Escape)
                {
                    return "KeyCode_Escape".Translate();
                }
                if (keyCode == KeyCode.Space)
                {
                    return "KeyCode_Space".Translate();
                }
                switch (keyCode)
                {
                    case KeyCode.RightShift:
                    case KeyCode.LeftShift:
                        return "KeyCode_Shift".Translate();
                    case KeyCode.RightControl:
                    case KeyCode.LeftControl:
                        return "KeyCode_Control".Translate();
                    case KeyCode.RightAlt:
                    case KeyCode.LeftAlt:
                        return "KeyCode_Alt".Translate();
                    case KeyCode.RightMeta:
                    case KeyCode.LeftMeta:
                        return "KeyCode_Command".Translate();
                    case KeyCode.LeftWindows:
                    case KeyCode.RightWindows:
                        return "KeyCode_Windows".Translate();
                    case KeyCode.Mouse0:
                        return "KeyCode_Mouse0".Translate();
                    case KeyCode.Mouse1:
                        return "KeyCode_Mouse1".Translate();
                    case KeyCode.Mouse2:
                        return "KeyCode_Mouse2".Translate();
                }
            }
            return keyCode.ToString();
        }

        public static bool HasKeyBindingSymbols(string str)
        {
            return str != null && str.Contains("[") && KeyCodeUtility.FormattedWithKeyBindingsRegex.IsMatch(str);
        }

        public static string FormattedKeyBindings(this string str)
        {
            if (str == null)
            {
                return str;
            }
            if (!str.Contains("["))
            {
                return str;
            }
            return KeyCodeUtility.formattedWithKeyBindingsCache.Get(str);
        }

        private static string FormattedKeyBindingsImpl(Match match)
        {
            string value = match.Groups[1].Value;
            KeyBindingSpec keyBindingSpec;
            if (Get.Specs.TryGet<KeyBindingSpec>(value, out keyBindingSpec))
            {
                return RichText.Bold(keyBindingSpec.KeyCode.ToStringCached());
            }
            return "None".Translate();
        }

        public static void OnKeyBindingChanged()
        {
            KeyCodeUtility.formattedWithKeyBindingsCache.Clear();
        }

        private static readonly CalculationCache<string, string> formattedWithKeyBindingsCache = new CalculationCache<string, string>((string x) => KeyCodeUtility.FormattedWithKeyBindingsRegex.Replace(x, KeyCodeUtility.FormattedWithKeyBindingsMatchEvaluator), 30);

        private static string[] keyCodesToString;

        private static readonly MatchEvaluator FormattedWithKeyBindingsMatchEvaluator = new MatchEvaluator(KeyCodeUtility.FormattedKeyBindingsImpl);

        private static readonly Regex FormattedWithKeyBindingsRegex = new Regex("\\[([a-zA-Z0-9_-]*)\\]");
    }
}