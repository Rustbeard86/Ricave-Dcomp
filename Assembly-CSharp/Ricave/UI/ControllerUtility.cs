using System;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Steamworks;
using UnityEngine;

namespace Ricave.UI
{
    public static class ControllerUtility
    {
        public static bool InControllerMode
        {
            get
            {
                return ControllerUtility.inControllerMode;
            }
        }

        public static ControllerType ControllerType
        {
            get
            {
                if (!ControllerUtility.inControllerMode)
                {
                    return ControllerType.None;
                }
                return ControllerUtility.controllerType;
            }
        }

        public static void OnGUI()
        {
            bool flag = ControllerUtility.inControllerMode;
            if (SteamDeckUtility.IsSteamDeck)
            {
                ControllerUtility.inControllerMode = true;
            }
            else if (SteamManager.InitializedAndNotShuttingDown)
            {
                int connectedControllers = SteamInput.GetConnectedControllers(ControllerUtility.tmpConnectedControllers);
                if (ControllerUtility.IsXboxControllerConnected(ControllerUtility.tmpConnectedControllers, connectedControllers) || ControllerUtility.IsPSControllerConnected(ControllerUtility.tmpConnectedControllers, connectedControllers))
                {
                    ControllerUtility.inControllerMode = true;
                    ControllerUtility.everDetectedControllerFromSteamInput = true;
                }
                else if (ControllerUtility.everDetectedControllerFromSteamInput)
                {
                    ControllerUtility.inControllerMode = false;
                }
            }
            else if (Event.current.type == EventType.KeyDown && ControllerUtility.IsControllerButton(Event.current.keyCode))
            {
                ControllerUtility.inControllerMode = true;
            }
            if (ControllerUtility.inControllerMode != flag)
            {
                ControllerUtility.OnControllerModeChanged();
            }
        }

        private static bool IsControllerButton(KeyCode keyCode)
        {
            return keyCode >= KeyCode.JoystickButton0 && keyCode <= KeyCode.Joystick8Button19;
        }

        public static KeyCode RemapControllerToKeyboard(KeyCode keyCode)
        {
            if (ControllerUtility.ControllerType == ControllerType.SteamDeck)
            {
                switch (keyCode)
                {
                    case KeyCode.JoystickButton0:
                        return KeyCode.Return;
                    case KeyCode.JoystickButton1:
                        return KeyCode.E;
                    case KeyCode.JoystickButton2:
                        return KeyCode.Q;
                    case KeyCode.JoystickButton3:
                        return KeyCode.Space;
                    case KeyCode.JoystickButton4:
                        return KeyCode.Z;
                    case KeyCode.JoystickButton5:
                        return KeyCode.Tab;
                    case KeyCode.JoystickButton6:
                        return KeyCode.Escape;
                    case KeyCode.JoystickButton7:
                        return KeyCode.Escape;
                    case KeyCode.JoystickButton8:
                        return KeyCode.Mouse1;
                    case KeyCode.JoystickButton9:
                        return KeyCode.Mouse0;
                    case KeyCode.JoystickButton10:
                        return KeyCode.J;
                    case KeyCode.JoystickButton11:
                        return KeyCode.LeftShift;
                    case KeyCode.JoystickButton12:
                        return KeyCode.M;
                    case KeyCode.JoystickButton13:
                        return KeyCode.H;
                    default:
                        return KeyCode.None;
                }
            }
            else
            {
                switch (keyCode)
                {
                    case KeyCode.JoystickButton0:
                        return KeyCode.Return;
                    case KeyCode.JoystickButton1:
                        return KeyCode.E;
                    case KeyCode.JoystickButton2:
                        return KeyCode.Q;
                    case KeyCode.JoystickButton3:
                        return KeyCode.Space;
                    case KeyCode.JoystickButton4:
                        return KeyCode.Z;
                    case KeyCode.JoystickButton5:
                        return KeyCode.Tab;
                    case KeyCode.JoystickButton6:
                        return KeyCode.J;
                    case KeyCode.JoystickButton7:
                        return KeyCode.Escape;
                    case KeyCode.JoystickButton8:
                        return KeyCode.M;
                    case KeyCode.JoystickButton9:
                        return KeyCode.LeftShift;
                    default:
                        return KeyCode.None;
                }
            }
        }

        public static KeyCode RemapKeyboardToController(KeyCode keyCode, out KeyCode alt1, out KeyCode alt2)
        {
            alt1 = KeyCode.None;
            alt2 = KeyCode.None;
            if (ControllerUtility.ControllerType == ControllerType.SteamDeck)
            {
                if (keyCode <= KeyCode.J)
                {
                    if (keyCode <= KeyCode.Escape)
                    {
                        if (keyCode == KeyCode.Tab)
                        {
                            return KeyCode.JoystickButton5;
                        }
                        if (keyCode == KeyCode.Return)
                        {
                            return KeyCode.JoystickButton0;
                        }
                        if (keyCode == KeyCode.Escape)
                        {
                            alt1 = KeyCode.JoystickButton6;
                            return KeyCode.JoystickButton7;
                        }
                    }
                    else if (keyCode <= KeyCode.E)
                    {
                        if (keyCode == KeyCode.Space)
                        {
                            return KeyCode.JoystickButton3;
                        }
                        if (keyCode == KeyCode.E)
                        {
                            return KeyCode.JoystickButton1;
                        }
                    }
                    else
                    {
                        if (keyCode == KeyCode.H)
                        {
                            return KeyCode.JoystickButton13;
                        }
                        if (keyCode == KeyCode.J)
                        {
                            return KeyCode.JoystickButton10;
                        }
                    }
                }
                else if (keyCode <= KeyCode.Z)
                {
                    if (keyCode == KeyCode.M)
                    {
                        return KeyCode.JoystickButton12;
                    }
                    if (keyCode == KeyCode.Q)
                    {
                        return KeyCode.JoystickButton2;
                    }
                    if (keyCode == KeyCode.Z)
                    {
                        return KeyCode.JoystickButton4;
                    }
                }
                else if (keyCode <= KeyCode.LeftShift)
                {
                    if (keyCode == KeyCode.RightShift)
                    {
                        return KeyCode.JoystickButton11;
                    }
                    if (keyCode == KeyCode.LeftShift)
                    {
                        return KeyCode.JoystickButton11;
                    }
                }
                else
                {
                    if (keyCode == KeyCode.Mouse0)
                    {
                        return KeyCode.JoystickButton9;
                    }
                    if (keyCode == KeyCode.Mouse1)
                    {
                        return KeyCode.JoystickButton8;
                    }
                }
                return KeyCode.None;
            }
            if (keyCode <= KeyCode.E)
            {
                if (keyCode <= KeyCode.Return)
                {
                    if (keyCode == KeyCode.Tab)
                    {
                        return KeyCode.JoystickButton5;
                    }
                    if (keyCode == KeyCode.Return)
                    {
                        return KeyCode.JoystickButton0;
                    }
                }
                else
                {
                    if (keyCode == KeyCode.Escape)
                    {
                        return KeyCode.JoystickButton7;
                    }
                    if (keyCode == KeyCode.Space)
                    {
                        return KeyCode.JoystickButton3;
                    }
                    if (keyCode == KeyCode.E)
                    {
                        return KeyCode.JoystickButton1;
                    }
                }
            }
            else if (keyCode <= KeyCode.Q)
            {
                if (keyCode == KeyCode.J)
                {
                    return KeyCode.JoystickButton6;
                }
                if (keyCode == KeyCode.M)
                {
                    return KeyCode.JoystickButton8;
                }
                if (keyCode == KeyCode.Q)
                {
                    return KeyCode.JoystickButton2;
                }
            }
            else
            {
                if (keyCode == KeyCode.Z)
                {
                    return KeyCode.JoystickButton4;
                }
                if (keyCode == KeyCode.RightShift)
                {
                    return KeyCode.JoystickButton9;
                }
                if (keyCode == KeyCode.LeftShift)
                {
                    return KeyCode.JoystickButton9;
                }
            }
            return KeyCode.None;
        }

        public static string GetControllerKeyLabel(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.JoystickButton0:
                    if (ControllerUtility.controllerType != ControllerType.PlayStation)
                    {
                        return "A";
                    }
                    return "X";
                case KeyCode.JoystickButton1:
                    if (ControllerUtility.controllerType != ControllerType.PlayStation)
                    {
                        return "B";
                    }
                    return "O";
                case KeyCode.JoystickButton2:
                    if (ControllerUtility.controllerType != ControllerType.PlayStation)
                    {
                        return "X";
                    }
                    return "□";
                case KeyCode.JoystickButton3:
                    if (ControllerUtility.controllerType != ControllerType.PlayStation)
                    {
                        return "Y";
                    }
                    return "△";
                case KeyCode.JoystickButton4:
                    return "ControllerKey_LB".Translate();
                case KeyCode.JoystickButton5:
                    return "ControllerKey_RB".Translate();
                case KeyCode.JoystickButton6:
                    return "ControllerKey_View".Translate();
                case KeyCode.JoystickButton7:
                    return "ControllerKey_Menu".Translate();
                case KeyCode.JoystickButton8:
                    return "ControllerKey_LSB".Translate();
                case KeyCode.JoystickButton9:
                    return "ControllerKey_RSB".Translate();
                case KeyCode.JoystickButton10:
                    return "L4";
                case KeyCode.JoystickButton11:
                    return "L5";
                case KeyCode.JoystickButton12:
                    return "R4";
                case KeyCode.JoystickButton13:
                    return "R5";
                default:
                    return "?";
            }
        }

        public static Texture2D GetGlyph(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Mouse0:
                    return ControllerUtility.Common_RT;
                case KeyCode.Mouse1:
                    return ControllerUtility.Common_LT;
                case KeyCode.JoystickButton0:
                    return ControllerUtility.< GetGlyph > g__Get | 45_0(ControllerUtility.Xbox_A, ControllerUtility.PlayStation_X, ControllerUtility.SteamDeck_A);
                case KeyCode.JoystickButton1:
                    return ControllerUtility.< GetGlyph > g__Get | 45_0(ControllerUtility.Xbox_B, ControllerUtility.PlayStation_O, ControllerUtility.SteamDeck_B);
                case KeyCode.JoystickButton2:
                    return ControllerUtility.< GetGlyph > g__Get | 45_0(ControllerUtility.Xbox_X, ControllerUtility.PlayStation_Square, ControllerUtility.SteamDeck_X);
                case KeyCode.JoystickButton3:
                    return ControllerUtility.< GetGlyph > g__Get | 45_0(ControllerUtility.Xbox_Y, ControllerUtility.PlayStation_Triangle, ControllerUtility.SteamDeck_Y);
                case KeyCode.JoystickButton4:
                    return ControllerUtility.Common_LB;
                case KeyCode.JoystickButton5:
                    return ControllerUtility.Common_RB;
                case KeyCode.JoystickButton6:
                    return ControllerUtility.< GetGlyph > g__Get | 45_0(ControllerUtility.Xbox_Back, ControllerUtility.PlayStation_Select, ControllerUtility.SteamDeck_View);
                case KeyCode.JoystickButton7:
                    return ControllerUtility.< GetGlyph > g__Get | 45_0(ControllerUtility.Xbox_Start, ControllerUtility.PlayStation_Start, ControllerUtility.SteamDeck_Menu);
                case KeyCode.JoystickButton8:
                    return ControllerUtility.Common_LSB;
                case KeyCode.JoystickButton9:
                    return ControllerUtility.Common_RSB;
                case KeyCode.JoystickButton10:
                    return ControllerUtility.SteamDeck_L4;
                case KeyCode.JoystickButton11:
                    return ControllerUtility.SteamDeck_L5;
                case KeyCode.JoystickButton12:
                    return ControllerUtility.SteamDeck_R4;
                case KeyCode.JoystickButton13:
                    return ControllerUtility.SteamDeck_R5;
            }
            return null;
        }

        private static void OnControllerModeChanged()
        {
            KeyCodeUtility.ResetKeyCodesStrings();
            CachedGUI.SetDirty(1);
            if (!ControllerUtility.inControllerMode)
            {
                ControllerUtility.controllerType = ControllerType.None;
                return;
            }
            if (SteamDeckUtility.IsSteamDeck)
            {
                ControllerUtility.controllerType = ControllerType.SteamDeck;
                return;
            }
            ControllerUtility.controllerType = ControllerType.Xbox;
            int num = (SteamManager.InitializedAndNotShuttingDown ? SteamInput.GetConnectedControllers(ControllerUtility.tmpConnectedControllers) : 0);
            if (ControllerUtility.IsXboxControllerConnected(ControllerUtility.tmpConnectedControllers, num))
            {
                ControllerUtility.controllerType = ControllerType.Xbox;
                return;
            }
            if (ControllerUtility.IsPSControllerConnected(ControllerUtility.tmpConnectedControllers, num))
            {
                ControllerUtility.controllerType = ControllerType.PlayStation;
                return;
            }
            string[] joystickNames = Input.GetJoystickNames();
            if (joystickNames != null)
            {
                foreach (string text in joystickNames)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        if (text.Contains("xbox", StringComparison.InvariantCultureIgnoreCase) || text.Contains("microsoft", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ControllerUtility.controllerType = ControllerType.Xbox;
                            return;
                        }
                        if (text.Contains("ps4", StringComparison.InvariantCultureIgnoreCase) || text.Contains("playstation", StringComparison.InvariantCultureIgnoreCase) || text.Contains("play station", StringComparison.InvariantCultureIgnoreCase) || text.Contains("sony", StringComparison.InvariantCultureIgnoreCase) || text.Contains("dualshock", StringComparison.InvariantCultureIgnoreCase) || text.Contains("dualsense", StringComparison.InvariantCultureIgnoreCase) || text == "Wireless Controller")
                        {
                            ControllerUtility.controllerType = ControllerType.PlayStation;
                            return;
                        }
                    }
                }
            }
        }

        private static bool IsXboxControllerConnected(InputHandle_t[] connectedControllers, int count)
        {
            if (count <= 0 || connectedControllers == null)
            {
                return false;
            }
            for (int i = 0; i < count; i++)
            {
                ESteamInputType inputTypeForHandle = SteamInput.GetInputTypeForHandle(connectedControllers[i]);
                if (inputTypeForHandle == ESteamInputType.k_ESteamInputType_XBox360Controller || inputTypeForHandle == ESteamInputType.k_ESteamInputType_XBoxOneController || inputTypeForHandle == ESteamInputType.k_ESteamInputType_SteamController)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsPSControllerConnected(InputHandle_t[] connectedControllers, int count)
        {
            if (count <= 0 || connectedControllers == null)
            {
                return false;
            }
            for (int i = 0; i < count; i++)
            {
                ESteamInputType inputTypeForHandle = SteamInput.GetInputTypeForHandle(connectedControllers[i]);
                if (inputTypeForHandle == ESteamInputType.k_ESteamInputType_PS3Controller || inputTypeForHandle == ESteamInputType.k_ESteamInputType_PS4Controller || inputTypeForHandle == ESteamInputType.k_ESteamInputType_PS5Controller)
                {
                    return true;
                }
            }
            return false;
        }

        [CompilerGenerated]
        internal static Texture2D<GetGlyph> g__Get|45_0(Texture2D xbox, Texture2D ps, Texture2D steamDeck)
		{
			switch (ControllerUtility.controllerType)
			{
			case ControllerType.Xbox:
				return xbox;
			case ControllerType.PlayStation:
				return ps;
			case ControllerType.SteamDeck:
				return steamDeck;
			default:
				return xbox;
			}
}

private static bool inControllerMode;

private static bool everDetectedControllerFromSteamInput;

private static ControllerType controllerType;

private static InputHandle_t[] tmpConnectedControllers = new InputHandle_t[16];

private static readonly Texture2D Xbox_A = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Xbox/A");

private static readonly Texture2D Xbox_B = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Xbox/B");

private static readonly Texture2D Xbox_X = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Xbox/X");

private static readonly Texture2D Xbox_Y = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Xbox/Y");

private static readonly Texture2D Xbox_Back = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Xbox/Back");

private static readonly Texture2D Xbox_Start = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Xbox/Start");

private static readonly Texture2D PlayStation_X = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/PlayStation/X");

private static readonly Texture2D PlayStation_O = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/PlayStation/O");

private static readonly Texture2D PlayStation_Square = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/PlayStation/Square");

private static readonly Texture2D PlayStation_Triangle = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/PlayStation/Triangle");

private static readonly Texture2D PlayStation_Select = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/PlayStation/Select");

private static readonly Texture2D PlayStation_Start = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/PlayStation/Start");

private static readonly Texture2D SteamDeck_A = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/SteamDeck/A");

private static readonly Texture2D SteamDeck_B = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/SteamDeck/B");

private static readonly Texture2D SteamDeck_X = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/SteamDeck/X");

private static readonly Texture2D SteamDeck_Y = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/SteamDeck/Y");

private static readonly Texture2D SteamDeck_View = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/SteamDeck/View");

private static readonly Texture2D SteamDeck_Menu = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/SteamDeck/Menu");

private static readonly Texture2D SteamDeck_L4 = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/SteamDeck/L4");

private static readonly Texture2D SteamDeck_L5 = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/SteamDeck/L5");

private static readonly Texture2D SteamDeck_R4 = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/SteamDeck/R4");

private static readonly Texture2D SteamDeck_R5 = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/SteamDeck/R5");

private static readonly Texture2D Common_LB = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Common/LB");

private static readonly Texture2D Common_RB = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Common/RB");

private static readonly Texture2D Common_LT = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Common/LT");

private static readonly Texture2D Common_RT = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Common/RT");

private static readonly Texture2D Common_Up = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Common/Up");

private static readonly Texture2D Common_Down = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Common/Down");

private static readonly Texture2D Common_Left = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Common/Left");

private static readonly Texture2D Common_Right = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Common/Right");

private static readonly Texture2D Common_LSB = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Common/LSB");

private static readonly Texture2D Common_RSB = Assets.Get<Texture2D>("Textures/UI/ControllerGlyphs/Common/RSB");
	}
}