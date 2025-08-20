using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_KeyBindings : Window
    {
        protected override bool CloseOnEscapeKey
        {
            get
            {
                return this.readingKeyFor == null && this.newKeyBindings.Count == 0;
            }
        }

        public Window_KeyBindings(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.keyBindingsInUIOrder.Clear();
            this.keyBindingsInUIOrder.AddRange(Get.Specs.GetAll<KeyBindingSpec>());
            this.keyBindingsInUIOrder.Sort((KeyBindingSpec a, KeyBindingSpec b) => a.UIOrder.CompareTo(b.UIOrder));
        }

        protected override void DoWindowContents(Rect inRect)
        {
            if (this.readingKeyFor != null)
            {
                this.DoReadingKeyUI(inRect);
                return;
            }
            Rect rect = base.MainArea(inRect);
            Rect rect2 = Widgets.BeginScrollView(rect, new int?(269355109));
            float num = rect2.y;
            foreach (KeyBindingSpec keyBindingSpec in this.keyBindingsInUIOrder)
            {
                Rect rect3 = new Rect(rect2.x, num, rect2.width, 50f);
                if (rect3.VisibleInScrollView())
                {
                    this.DoKeyBinding(keyBindingSpec, rect3.ContractedBy(6f));
                }
                num += 50f;
            }
            Widgets.EndScrollView(rect, num, false, false);
            if (base.DoBottomButton("Cancel".Translate(), inRect, 0, 3, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
            if (base.DoBottomButton("Accept".Translate(), inRect, 1, 3, true, null) || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return))
            {
                foreach (KeyValuePair<KeyBindingSpec, KeyCode> keyValuePair in this.newKeyBindings)
                {
                    keyValuePair.Key.SetKeyCode(keyValuePair.Value, false);
                }
                KeyBindingSpec.Save();
                Get.WindowManager.Close(this, true);
            }
            if (base.DoBottomButton("RestoreDefaults".Translate(), inRect, 2, 3, true, null))
            {
                foreach (KeyBindingSpec keyBindingSpec2 in Get.Specs.GetAll<KeyBindingSpec>())
                {
                    keyBindingSpec2.RestoreDefaultKeyCode(false);
                }
                KeyBindingSpec.Save();
                this.newKeyBindings.Clear();
            }
        }

        private void DoReadingKeyUI(Rect rect)
        {
            Widgets.Align = TextAnchor.MiddleCenter;
            Widgets.Label(rect.center.CenteredRect(rect.width, 300f), "{0}\n{1}".Formatted("PressKeyToAssignTo".Translate(), this.readingKeyFor.LabelCap), true, null, null, false);
            Widgets.ResetAlign();
            KeyCode keyCode;
            if (Event.current.type == EventType.KeyDown)
            {
                keyCode = Event.current.keyCode;
            }
            else if (Event.current.type == EventType.MouseDown)
            {
                keyCode = KeyCodeUtility.MouseButtonToKeyCode(Event.current.button);
            }
            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                keyCode = KeyCode.LeftAlt;
            }
            else if (Input.GetKey(KeyCode.RightAlt))
            {
                keyCode = KeyCode.RightAlt;
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                keyCode = KeyCode.LeftShift;
            }
            else if (Input.GetKey(KeyCode.RightShift))
            {
                keyCode = KeyCode.RightShift;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                keyCode = KeyCode.LeftControl;
            }
            else if (Input.GetKey(KeyCode.RightControl))
            {
                keyCode = KeyCode.RightControl;
            }
            else if (Input.GetKey(KeyCode.LeftMeta))
            {
                keyCode = KeyCode.LeftMeta;
            }
            else if (Input.GetKey(KeyCode.RightMeta))
            {
                keyCode = KeyCode.RightMeta;
            }
            else if (Input.GetKey(KeyCode.LeftWindows))
            {
                keyCode = KeyCode.LeftWindows;
            }
            else if (Input.GetKey(KeyCode.RightWindows))
            {
                keyCode = KeyCode.RightWindows;
            }
            else if (Input.GetKey(KeyCode.LeftMeta))
            {
                keyCode = KeyCode.LeftMeta;
            }
            else if (Input.GetKey(KeyCode.RightMeta))
            {
                keyCode = KeyCode.RightMeta;
            }
            else
            {
                keyCode = KeyCode.None;
            }
            if (keyCode != KeyCode.None)
            {
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                if (keyCode != KeyCode.Escape)
                {
                    if (keyCode == this.readingKeyFor.KeyCode)
                    {
                        this.newKeyBindings.Remove(this.readingKeyFor);
                    }
                    else
                    {
                        this.newKeyBindings[this.readingKeyFor] = keyCode;
                    }
                }
                this.readingKeyFor = null;
                if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown)
                {
                    Event.current.Use();
                }
            }
        }

        private void DoKeyBinding(KeyBindingSpec keyBinding, Rect rect)
        {
            Widgets.Align = TextAnchor.MiddleLeft;
            Widgets.Label(rect.LeftHalf(), keyBinding.LabelCap, true, null, null, false);
            Widgets.ResetAlign();
            KeyCode keyCode;
            Color? color;
            if (this.newKeyBindings.TryGetValue(keyBinding, out keyCode))
            {
                color = new Color?(new Color(0.5f, 1f, 0.5f));
            }
            else
            {
                keyCode = keyBinding.KeyCode;
                color = null;
            }
            Rect rect2 = rect.RightHalf();
            string text = keyCode.ToStringCached();
            bool flag = true;
            Color? color2 = color;
            if (Widgets.Button(rect2, text, flag, null, color2, true, true, true, true, null, true, true, null, false, null, null))
            {
                this.readingKeyFor = keyBinding;
            }
        }

        private KeyBindingSpec readingKeyFor;

        private Dictionary<KeyBindingSpec, KeyCode> newKeyBindings = new Dictionary<KeyBindingSpec, KeyCode>();

        private List<KeyBindingSpec> keyBindingsInUIOrder = new List<KeyBindingSpec>();

        private const float RowHeight = 50f;
    }
}