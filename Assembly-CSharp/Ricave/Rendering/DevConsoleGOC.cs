using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class DevConsoleGOC : MonoBehaviour
    {
        public bool IsOn
        {
            get
            {
                return this.on;
            }
        }

        public bool IsOnOrStillVisible
        {
            get
            {
                return this.on || this.yOffset.CurrentValue > -299f;
            }
        }

        public Entity SelectedOrMouseoverEntity
        {
            get
            {
                return this.selectedEntity ?? this.mouseoverEntity;
            }
        }

        private IEnumerable<string> Suggestions
        {
            get
            {
                if (this.input.NullOrEmpty())
                {
                    yield break;
                }
                string text = this.input.Trim(' ');
                if (text.Length == 0)
                {
                    yield break;
                }
                bool finishedTypingCommandName = text.Contains(' ') || this.input.EndsWith(" ");
                string typedCommandName = this.GetCommandName(this.input);
                bool flag = false;
                IEnumerable<DevConsoleCommands.Command> commands = DevConsoleCommandsList.Commands;
                Func<DevConsoleCommands.Command, bool> <> 9__0;
                Func<DevConsoleCommands.Command, bool> func;
                if ((func = <> 9__0) == null)
                {
                    func = (<> 9__0 = (DevConsoleCommands.Command x) => x.Name.StartsWith(typedCommandName));
                }
                IOrderedEnumerable<DevConsoleCommands.Command> orderedEnumerable = commands.OrderByDescending<DevConsoleCommands.Command, bool>(func);
                Func<DevConsoleCommands.Command, bool> <> 9__1;
                Func<DevConsoleCommands.Command, bool> func2;
                if ((func2 = <> 9__1) == null)
                {
                    func2 = (<> 9__1 = (DevConsoleCommands.Command x) => x.Name.Contains(typedCommandName));
                }
                foreach (DevConsoleCommands.Command command in orderedEnumerable.ThenByDescending<DevConsoleCommands.Command, bool>(func2))
                {
                    if (finishedTypingCommandName)
                    {
                        if (command.Name != typedCommandName)
                        {
                            continue;
                        }
                    }
                    else if (!command.Name.Contains(typedCommandName))
                    {
                        continue;
                    }
                    yield return command.NameWithArgsNames + " <color=#a3a3a3>(" + command.Description + ")</color>";
                    flag = true;
                }
                IEnumerator<DevConsoleCommands.Command> enumerator = null;
                if (!flag)
                {
                    yield return "<color=#ff8080>Command not found: " + typedCommandName + "</color>";
                }
                yield break;
                yield break;
            }
        }

        private void OnGUI()
        {
            this.CheckToggleDevMode();
            if (!PrefsHelper.DevMode)
            {
                base.useGUILayout = false;
                return;
            }
            Widgets.ApplySkin();
            GUI.depth = -200;
            if (Event.current.type == EventType.KeyDown)
            {
                if ((Event.current.keyCode == KeyCode.BackQuote || Event.current.keyCode == KeyCode.Tilde) && this.tildeHeldDownStartFrame == Clock.Frame)
                {
                    this.Toggle();
                    Event.current.Use();
                }
                if (this.on)
                {
                    if (Event.current.keyCode == KeyCode.Escape)
                    {
                        this.SetOn(false);
                        Event.current.Use();
                    }
                    else if (Event.current.keyCode == KeyCode.UpArrow && this.curHistoryIndex - 1 >= 0)
                    {
                        this.curHistoryIndex--;
                        this.input = DevConsoleGOC.history[this.curHistoryIndex];
                        Event.current.Use();
                    }
                    else if (Event.current.keyCode == KeyCode.DownArrow && this.curHistoryIndex + 1 < DevConsoleGOC.history.Count)
                    {
                        this.curHistoryIndex++;
                        this.input = DevConsoleGOC.history[this.curHistoryIndex];
                        Event.current.Use();
                    }
                    else if (Event.current.keyCode == KeyCode.Tab)
                    {
                        this.Autocomplete();
                        Event.current.Use();
                    }
                }
            }
            base.useGUILayout = this.IsOnOrStillVisible;
            this.DoConsole();
        }

        private void Update()
        {
            this.mouseoverEntity = null;
            if (this.on)
            {
                Vector3Int vector3Int;
                Vector3 vector;
                GameObject gameObject = RaycastUtility.RaycastFromMousePosition(15f, out vector3Int, out vector, true, true, false, null);
                if (gameObject != null)
                {
                    EntityGOC componentInParent = gameObject.GetComponentInParent<EntityGOC>();
                    if (componentInParent != null)
                    {
                        gameObject = componentInParent.gameObject;
                    }
                    if (this.selectedEntity == null)
                    {
                        Get.GameObjectHighlighter.Highlight(gameObject, null);
                    }
                    if (componentInParent != null)
                    {
                        this.mouseoverEntity = componentInParent.Entity;
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    this.selectedEntity = this.mouseoverEntity;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    this.selectedEntity = null;
                }
                if (this.selectedEntity != null && this.selectedEntity.Spawned)
                {
                    Get.GameObjectHighlighter.Highlight(this.selectedEntity.GameObject, null);
                }
            }
        }

        private void FixedUpdate()
        {
            this.yOffset.SetTarget(Calc.Lerp(this.yOffset.Target, this.on ? (-1f) : (-300f), 15f * Clock.FixedUnscaledDeltaTime));
        }

        public void Toggle()
        {
            if (!this.on && !Get.InMainMenu && !Get.InLobby && Get.RunConfig.IsDailyChallenge)
            {
                return;
            }
            this.SetOn(!this.on);
        }

        private void CheckToggleDevMode()
        {
            if (!Get.InMainMenu && !Get.InLobby && Get.RunConfig.IsDailyChallenge)
            {
                return;
            }
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.BackQuote || Event.current.keyCode == KeyCode.Tilde) && !this.tildeHeldDown)
            {
                this.tildeHeldDown = true;
                this.tildeHeldDownTime = 0f;
                this.tildeHeldDownStartFrame = Clock.Frame;
                this.devModeChangedThisTildeKeypress = false;
            }
            else if (Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.BackQuote || Event.current.keyCode == KeyCode.Tilde) && this.tildeHeldDown)
            {
                this.tildeHeldDown = false;
                this.tildeHeldDownTime = 0f;
                this.devModeChangedThisTildeKeypress = false;
            }
            if (Event.current.type == EventType.Repaint && this.tildeHeldDown)
            {
                this.tildeHeldDownTime += Clock.UnscaledDeltaTime;
                if (this.tildeHeldDownTime > 3f && !this.devModeChangedThisTildeKeypress)
                {
                    this.devModeChangedThisTildeKeypress = true;
                    PrefsHelper.DevMode = !PrefsHelper.DevMode;
                    this.SetOn(PrefsHelper.DevMode);
                }
            }
        }

        public void SetOn(bool on)
        {
            if (this.on == on)
            {
                return;
            }
            this.on = on;
            this.mouseoverEntity = null;
            this.selectedEntity = null;
            if (on)
            {
                Mouse.SetCursorLocked(false);
                this.input = "";
                this.cyclingAutocomplete = false;
                this.curHistoryIndex = DevConsoleGOC.history.Count;
                return;
            }
            SteamKeyboardUtility.OnTextFieldLikelyNoLongerFocused();
            if (!Get.InMainMenu && !Get.UI.WantsMouseUnlocked)
            {
                Mouse.SetCursorLocked(true);
            }
        }

        private void DoConsole()
        {
            if (!this.IsOnOrStillVisible)
            {
                return;
            }
            Rect rect = new Rect(0f, this.yOffset.CurrentValue, Widgets.VirtualWidth, 270f);
            GUIExtra.DrawRoundedRectBump(rect, new Color(0.2f, 0.2f, 0.2f, 0.8f), false, true, true, true, true, null);
            float num = 0f;
            Rect rect2 = Widgets.BeginScrollView(rect, new int?(462948500)).ContractedBy(10f, 0f);
            float num2 = 0f;
            List<string> all = Log.All;
            int num3 = Math.Max(all.Count - 100, 0);
            for (int i = num3; i < all.Count; i++)
            {
                num2 += Widgets.CalcHeight(all[i], rect2.width);
            }
            num = Math.Max(rect.height - num2, 0f);
            for (int j = num3; j < all.Count; j++)
            {
                Widgets.Label(rect2.x, rect2.width, ref num, all[j], true);
            }
            Widgets.EndScrollView(rect, num, true, false);
            Rect rect3 = new Rect(rect.x, rect.yMax, Widgets.VirtualWidth, 30f);
            GUIExtra.DrawRoundedRectBump(rect3, new Color(0.3f, 0.3f, 0.3f, 0.85f), false, true, true, true, true, null);
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                this.TryExecuteCommand();
                Event.current.Use();
            }
            GUI.SetNextControlName("DevConsoleTextField");
            string text = Widgets.TextField(rect3, this.input, true, false);
            if (text != null)
            {
                string text2 = this.input;
                this.input = text.Replace("`", "");
                if (text2 != this.input)
                {
                    this.cyclingAutocomplete = false;
                }
            }
            if (this.on)
            {
                if (!SteamDeckUtility.IsSteamDeck && GUI.GetNameOfFocusedControl() != "DevConsoleTextField")
                {
                    GUI.FocusControl("DevConsoleTextField");
                    GUIExtra.DeselectText(this.input.Length);
                }
            }
            else if (GUI.GetNameOfFocusedControl() == "DevConsoleTextField")
            {
                GUI.FocusControl(null);
            }
            if (Event.current.type == EventType.Repaint)
            {
                if (this.on && this.Suggestions.Any<string>())
                {
                    float num4 = rect3.yMax;
                    float num5 = this.Suggestions.Max<string>((string x) => Widgets.CalcSize(x).x) + 20f;
                    foreach (string text3 in this.Suggestions)
                    {
                        Rect rect4 = new Rect(rect3.x, num4, num5, 30f);
                        Rect rect5 = new Rect(rect4.x + 10f, rect4.y + 4f, rect4.width - 20f, rect4.height - 8f);
                        GUIExtra.DrawRoundedRectBump(rect4, new Color(0.3f, 0.3f, 0.3f, 0.85f), false, true, true, true, true, null);
                        Widgets.Label(rect5, text3, true, null, null, false);
                        num4 += 30f;
                    }
                }
                Entity selectedOrMouseoverEntity = this.SelectedOrMouseoverEntity;
                if (selectedOrMouseoverEntity != null)
                {
                    Rect rect6 = new Rect(Widgets.VirtualWidth - 410f, rect3.yMax, 400f, 400f);
                    Widgets.Align = TextAnchor.UpperRight;
                    GUI.color = new Color(0.9f, 0.9f, 0.9f);
                    string[] array = new string[27];
                    array[0] = selectedOrMouseoverEntity.LabelCap;
                    array[1] = "\nClass: ";
                    int num6 = 2;
                    Type type = selectedOrMouseoverEntity.GetType();
                    array[num6] = ((type != null) ? type.ToString() : null);
                    array[3] = "\ninstanceID: ";
                    array[4] = selectedOrMouseoverEntity.InstanceID.ToString();
                    array[5] = "\nstableID: ";
                    array[6] = selectedOrMouseoverEntity.StableID.ToString();
                    array[7] = "\n\nPosition: ";
                    array[8] = selectedOrMouseoverEntity.Position.ToString();
                    array[9] = "\nRotation: ";
                    array[10] = selectedOrMouseoverEntity.Rotation.eulerAngles.ToString();
                    array[11] = "\nScale: ";
                    array[12] = selectedOrMouseoverEntity.Scale.ToString();
                    array[13] = "\nDirectionCardinal: ";
                    array[14] = selectedOrMouseoverEntity.DirectionCardinal.ToString();
                    array[15] = "\n\nSpec: ";
                    array[16] = selectedOrMouseoverEntity.Spec.SpecID;
                    array[17] = "\nSpec label: ";
                    array[18] = selectedOrMouseoverEntity.Spec.LabelCap;
                    array[19] = "\nSpec label (look override): ";
                    array[20] = selectedOrMouseoverEntity.Spec.LabelAdjustedCap;
                    array[21] = "\nSpec prefab: ";
                    int num7 = 22;
                    GameObject prefab = selectedOrMouseoverEntity.Spec.Prefab;
                    array[num7] = ((prefab != null) ? prefab.name : null);
                    array[23] = "\nSpec prefab (look override): ";
                    int num8 = 24;
                    GameObject prefabAdjusted = selectedOrMouseoverEntity.Spec.PrefabAdjusted;
                    array[num8] = ((prefabAdjusted != null) ? prefabAdjusted.name : null);
                    array[25] = "\nWantsUpdate: ";
                    array[26] = selectedOrMouseoverEntity.Spec.WantsUpdate.ToString();
                    string text4 = string.Concat(array);
                    GUIExtra.DrawRoundedRect(Widgets.CalcTrimmedBounds(text4, rect6).ExpandedBy(3f), new Color(0f, 0f, 0f, 0.34f), true, true, true, true, null);
                    Widgets.Label(rect6, text4, true, null, null, false);
                    GUI.color = Color.white;
                    Widgets.ResetAlign();
                }
            }
        }

        private void TryExecuteCommand()
        {
            if (this.input.NullOrEmpty())
            {
                return;
            }
            DevConsoleGOC.history.Add(this.input);
            this.curHistoryIndex = DevConsoleGOC.history.Count;
            DevConsoleCommands.TryExecute(this.input);
            this.input = "";
            this.cyclingAutocomplete = false;
        }

        private void Autocomplete()
        {
            this.input = this.input.TrimStart(' ');
            if (this.input.TrimEnd(' ').Contains(' '))
            {
                return;
            }
            if (this.cyclingAutocomplete)
            {
                if (this.rememberedAutocompleteSuggestions.Count != 0)
                {
                    int num = this.curAutocompleteIndex % this.rememberedAutocompleteSuggestions.Count;
                    if (!this.rememberedAutocompleteSuggestions[num].StartsWith("<color=#ff8080>Command not found: "))
                    {
                        this.input = this.GetCommandName(this.rememberedAutocompleteSuggestions[num]) + " ";
                    }
                    this.curAutocompleteIndex++;
                    return;
                }
            }
            else
            {
                this.cyclingAutocomplete = true;
                this.autocompleteForInput = this.input;
                this.rememberedAutocompleteSuggestions.Clear();
                this.rememberedAutocompleteSuggestions.AddRange(this.Suggestions);
                if (this.rememberedAutocompleteSuggestions.Count != 0 && !this.rememberedAutocompleteSuggestions[0].StartsWith("<color=#ff8080>Command not found: "))
                {
                    this.input = this.GetCommandName(this.rememberedAutocompleteSuggestions[0]) + " ";
                }
                this.curAutocompleteIndex = 1;
            }
        }

        private string GetCommandName(string commandWithArgs)
        {
            commandWithArgs = commandWithArgs.Trim(' ');
            int num = commandWithArgs.IndexOf(' ');
            if (num < 0)
            {
                return commandWithArgs;
            }
            return commandWithArgs.Substring(0, num);
        }

        private bool on;

        private InterpolatedFloat yOffset = new InterpolatedFloat(-300f);

        private string input = "";

        private static List<string> history = new List<string>(10);

        private int curHistoryIndex;

        private Entity mouseoverEntity;

        private Entity selectedEntity;

        private bool tildeHeldDown;

        private float tildeHeldDownTime;

        private int tildeHeldDownStartFrame = -1;

        private bool devModeChangedThisTildeKeypress;

        private bool cyclingAutocomplete;

        private string autocompleteForInput;

        private int curAutocompleteIndex;

        private List<string> rememberedAutocompleteSuggestions = new List<string>();

        private const float Height = 300f;

        private const float PosLerpSpeed = 15f;

        private const int InputHeight = 30;

        private const int SuggestionHeight = 30;

        private const int SuggestionLeftRightPad = 10;

        private const int SuggestionUpDownPad = 4;

        private const int MaxLogEntries = 100;

        private const string CommandNotFoundPrefix = "<color=#ff8080>Command not found: ";

        private const string CommandNotFoundSuffix = "</color>";
    }
}