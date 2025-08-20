using System;
using System.Collections.Generic;
using System.IO;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class DebugUI
    {
        public static bool HideUI { get; set; }

        public static bool TrailerMode { get; set; }

        public static bool LogEvents { get; set; }

        public void OnGUI()
        {
            if (!PrefsHelper.DevMode)
            {
                return;
            }
            if (DebugUI.LogEvents)
            {
                if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout && Event.current.type != EventType.MouseMove)
                {
                    Log.Message(string.Concat(new string[]
                    {
                        "Event: ",
                        Event.current.type.ToString(),
                        ", keyCode=",
                        Event.current.keyCode.ToString(),
                        ", character=",
                        Event.current.character.ToString(),
                        ", modifiers=",
                        Event.current.modifiers.ToString(),
                        ", delta=",
                        Event.current.delta.ToString(),
                        ", mousePosition=",
                        Event.current.mousePosition.ToString(),
                        ", button=",
                        Event.current.button.ToString(),
                        ", clickCount=",
                        Event.current.clickCount.ToString()
                    }));
                }
                if (Input.GetAxis("Fire1") != 0f)
                {
                    Log.Message("Axis: Fire1 " + Input.GetAxis("Fire1").ToString());
                }
                if (Input.GetAxis("Fire2") != 0f)
                {
                    Log.Message("Axis: Fire2 " + Input.GetAxis("Fire2").ToString());
                }
                if (Input.GetAxis("Horizontal") != 0f)
                {
                    Log.Message("Axis: Horizontal " + Input.GetAxis("Horizontal").ToString());
                }
                if (Input.GetAxis("Vertical") != 0f)
                {
                    Log.Message("Axis: Vertical " + Input.GetAxis("Vertical").ToString());
                }
            }
            if (Clock.Time > this.waitUntil)
            {
                while (!this.toReplay.NullOrEmpty<Instruction>())
                {
                    bool causesRewindPoint = this.toReplay[this.toReplay.Count - 1].CausesRewindPoint;
                    this.toReplay[this.toReplay.Count - 1].Do();
                    this.toReplay.RemoveAt(this.toReplay.Count - 1);
                    if (causesRewindPoint)
                    {
                        this.waitUntil = Clock.Time + 0.1f;
                        break;
                    }
                }
            }
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F12)
            {
                string text = "C:/Users/ison/Desktop/screenshot";
                int num = 1;
                string text2;
                for (; ; )
                {
                    text2 = text + "_" + num.ToString() + ".png";
                    if (!File.Exists(text2))
                    {
                        break;
                    }
                    num++;
                }
                text = text2;
                ScreenCapture.CaptureScreenshot(text);
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
            }
        }

        public void Replay(List<Instruction> instructions)
        {
            this.toReplay = instructions;
        }

        private List<Instruction> toReplay;

        private float waitUntil = -99999f;
    }
}