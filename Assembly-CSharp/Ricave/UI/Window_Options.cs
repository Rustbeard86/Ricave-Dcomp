using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Options : Window
    {
        public Window_Options(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            float num = (base.Rect.ContractedBy(base.Spec.Padding).width - 40f) / 3f;
            float num2 = 0f;
            Widgets.Section("VideoSection".Translate(), 0f, num, ref num2, false, 7f);
            this.Label("Resolution".Translate(), 0f, ref num2, num);
            if (this.Button("{0} x {1}".Formatted(Sys.Resolution.x, Sys.Resolution.y), 0f, ref num2, num, null, null))
            {
                Get.WindowManager.OpenContextMenu((from x in (from x in Screen.resolutions
                                                              where x.width >= 800 && x.height >= 600
                                                              select new ValueTuple<int, int>(x.width, x.height)).Distinct<ValueTuple<int, int>>()
                                                   select new ValueTuple<string, Action>("{0} x {1}".Formatted(x.Item1, x.Item2), delegate
                                                   {
                                                       PrefsHelper.SetResolution(x.Item1, x.Item2);
                                                       Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                                                   })).ToList<ValueTuple<string, Action>>(), "Resolution".Translate());
            }
            this.Label("Fullscreen".Translate(), 0f, ref num2, num);
            PrefsHelper.Fullscreen = this.Checkbox(PrefsHelper.Fullscreen, 0f, ref num2);
            this.Label("FOV".Translate(), 0f, ref num2, num);
            this.tempFOV = this.Slider(this.tempFOV, 60f, 90f, 0f, ref num2, num, 1f, false);
            if (!Widgets.DraggingAnySlider)
            {
                PrefsHelper.FOV = this.tempFOV;
            }
            this.Label("VSync".Translate(), 0f, ref num2, num);
            PrefsHelper.VSync = this.Checkbox(PrefsHelper.VSync, 0f, ref num2);
            if (!PrefsHelper.VSync)
            {
                this.Label("TargetFramerate".Translate(), 0f, ref num2, num);
                if (this.Button(PrefsHelper.TargetFramerate.ToStringCached(), 0f, ref num2, num, null, null))
                {
                    int[] array = new int[] { 30, 60, 90, 120 };
                    Get.WindowManager.OpenContextMenu(array.Select<int, ValueTuple<string, Action>>((int x) => new ValueTuple<string, Action>(x.ToStringCached(), delegate
                    {
                        PrefsHelper.TargetFramerate = x;
                        Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    })).ToList<ValueTuple<string, Action>>(), "TargetFramerate".Translate());
                }
            }
            this.Label("ShowFPS".Translate(), 0f, ref num2, num);
            PrefsHelper.ShowFPS = this.Checkbox(PrefsHelper.ShowFPS, 0f, ref num2);
            float num3 = num + 20f;
            num2 = 0f;
            Widgets.Section("InterfaceSection".Translate(), num3, num, ref num2, false, 7f);
            this.Label("UIScale".Translate(), num3, ref num2, num);
            this.tempUIScale = this.Slider(this.tempUIScale, 0.75f, 1.5f, num3, ref num2, num, 0.01f, true);
            if (!Widgets.DraggingAnySlider)
            {
                PrefsHelper.UIScale = this.tempUIScale;
            }
            this.Label((ControllerUtility.InControllerMode ? "MouseSensitivityController" : "MouseSensitivity").Translate(), num3, ref num2, num);
            this.tempMouseSensitivity = this.Slider(this.tempMouseSensitivity, 0.4f, 2.5f, num3, ref num2, num, 0.01f, true);
            if (!Widgets.DraggingAnySlider)
            {
                PrefsHelper.MouseSensitivity = this.tempMouseSensitivity;
            }
            this.Label("InvertY".Translate(), num3, ref num2, num);
            PrefsHelper.InvertY = this.Checkbox(PrefsHelper.InvertY, num3, ref num2);
            if (this.Button("KeyBindings".Translate(), num3, ref num2, num, null, null))
            {
                Get.WindowManager.Open(Get.Window_KeyBindings, true);
            }
            num2 += 5f;
            Widgets.Section("AudioSection".Translate(), num3, num, ref num2, false, 7f);
            this.Label("Volume".Translate(), num3, ref num2, num);
            this.tempVolume = this.Slider(this.tempVolume, 0f, 1f, num3, ref num2, num, 0.01f, true);
            if (!Widgets.DraggingAnySlider)
            {
                PrefsHelper.Volume = this.tempVolume;
            }
            this.Label("MusicVolume".Translate(), num3, ref num2, num);
            this.tempMusicVolume = this.Slider(this.tempMusicVolume, 0f, 1f, num3, ref num2, num, 0.01f, true);
            if (!Widgets.DraggingAnySlider)
            {
                PrefsHelper.MusicVolume = this.tempMusicVolume;
            }
            if (Get.InMainMenu)
            {
                num2 += 5f;
                Widgets.Section("MiscSection".Translate(), num3, num, ref num2, false, 7f);
                this.Label("ResetGameState".Translate(), num3, ref num2, num);
                if (this.Button("ReturnToLobby".Translate(), num3, ref num2, num, new Color?(new Color(0.6f, 0.1f, 0.1f)), null))
                {
                    Get.WindowManager.OpenConfirmationWindow("ConfirmReturnToLobby".Translate(), new Action(Window_Options.ReturnToLobby), false, null, null);
                }
                if (this.Button("ResetAllProgress".Translate(), num3, ref num2, num, new Color?(new Color(0.6f, 0.1f, 0.1f)), null))
                {
                    Get.WindowManager.OpenConfirmationWindow("ConfirmResetAllProgress".Translate(), delegate
                    {
                        string text2 = FilePaths.Savefile("Current");
                        if (File.Exists(text2))
                        {
                            File.Delete(text2);
                        }
                        string progress = FilePaths.Progress;
                        if (File.Exists(progress))
                        {
                            File.Delete(progress);
                            Root.LoadProgress();
                        }
                    }, false, null, null);
                }
            }
            num3 += num + 20f;
            num2 = 0f;
            Widgets.Section("QualitySection".Translate(), num3, num, ref num2, false, 7f);
            this.Label("SSAOQuality".Translate(), num3, ref num2, num);
            if (this.Button(Window_Options.SSAOQualityLabelKeys[PrefsHelper.SSAOQuality].Translate(), num3, ref num2, num, null, null))
            {
                Get.WindowManager.OpenContextMenu(Window_Options.SSAOQualityLabelKeys.Select<string, ValueTuple<string, Action>>((string x, int index) => new ValueTuple<string, Action>(x.Translate(), delegate
                {
                    PrefsHelper.SSAOQuality = index;
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                })).ToList<ValueTuple<string, Action>>(), "SSAOQuality".Translate());
            }
            this.Label("ShadowQuality".Translate(), num3, ref num2, num);
            if (this.Button(Window_Options.ShadowQualityLabelKeys[PrefsHelper.ShadowQuality].Translate(), num3, ref num2, num, null, null))
            {
                Get.WindowManager.OpenContextMenu(Window_Options.ShadowQualityLabelKeys.Select<string, ValueTuple<string, Action>>((string x, int index) => new ValueTuple<string, Action>(x.Translate(), delegate
                {
                    PrefsHelper.ShadowQuality = index;
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                })).ToList<ValueTuple<string, Action>>(), "ShadowQuality".Translate());
            }
            this.Label("Downsampling".Translate(), num3, ref num2, num);
            string text = null;
            for (int i = 0; i < Window_Options.DownsamplingLevels.Length; i++)
            {
                if (Window_Options.DownsamplingLevels[i].Item2 == PrefsHelper.Downsampling)
                {
                    text = Window_Options.DownsamplingLevels[i].Item1.Translate();
                    break;
                }
            }
            if (text == null)
            {
                text = PrefsHelper.Downsampling.ToStringCached();
            }
            if (this.Button(text, num3, ref num2, num, null, null))
            {
                Get.WindowManager.OpenContextMenu(Window_Options.DownsamplingLevels.Select<ValueTuple<string, float>, ValueTuple<string, Action>>((ValueTuple<string, float> x, int index) => new ValueTuple<string, Action>(x.Item1.Translate(), delegate
                {
                    PrefsHelper.Downsampling = x.Item2;
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                })).ToList<ValueTuple<string, Action>>(), "Downsampling".Translate());
            }
            this.Label("MaxLights".Translate(), num3, ref num2, num);
            this.tempMaxLights = Calc.RoundToInt(this.Slider((float)this.tempMaxLights, 2f, 5f, num3, ref num2, num, 1f, false));
            if (!Widgets.DraggingAnySlider)
            {
                PrefsHelper.MaxLights = this.tempMaxLights;
            }
            if (base.DoBottomButton("OK".Translate(), inRect, 0, 2, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
            if (base.DoBottomButton("RestoreDefaults".Translate(), inRect, 1, 2, true, null))
            {
                PrefsHelper.ResetToDefault();
                this.tempUIScale = PrefsHelper.UIScale;
                this.tempFOV = PrefsHelper.FOV;
                this.tempVolume = PrefsHelper.Volume;
                this.tempMusicVolume = PrefsHelper.MusicVolume;
                this.tempMouseSensitivity = PrefsHelper.MouseSensitivity;
                this.tempMaxLights = PrefsHelper.MaxLights;
            }
        }

        private void Label(string label, float x, ref float curY, float width)
        {
            Widgets.Label(new Rect(x, curY, width, 31f), label, true, null, null, false);
            curY += 31f;
        }

        private bool Button(string label, float x, ref float curY, float width, Color? backgroundColor = null, Color? labelColor = null)
        {
            bool flag = Widgets.Button(new Rect(x, curY, width, 30f), label, true, backgroundColor, labelColor, true, true, true, true, null, true, true, null, false, null, null);
            curY += 42f;
            return flag;
        }

        private float Slider(float value, float x, ref float curY, float width)
        {
            float num = Widgets.Slider(new Rect(x, curY, width, 30f), value, null);
            curY += 42f;
            return num;
        }

        private float Slider(float value, float min, float max, float x, ref float curY, float width, float roundTo = 0.01f, bool showValueAsPct = false)
        {
            float num = Widgets.Slider(new Rect(x, curY, width, 30f), value, min, max, roundTo, showValueAsPct);
            curY += 42f;
            return num;
        }

        private bool Checkbox(bool check, float x, ref float curY)
        {
            bool flag = Widgets.Checkbox(new Vector2(x, curY), check);
            curY += 42f;
            return flag;
        }

        public static void ReturnToLobby()
        {
            string text = FilePaths.Savefile("Current");
            if (File.Exists(text))
            {
                File.Delete(text);
            }
            Get.Progress.ResetPreferredRespawn();
            Get.Progress.Save();
        }

        private float tempUIScale = PrefsHelper.UIScale;

        private float tempFOV = PrefsHelper.FOV;

        private float tempVolume = PrefsHelper.Volume;

        private float tempMusicVolume = PrefsHelper.MusicVolume;

        private float tempMouseSensitivity = PrefsHelper.MouseSensitivity;

        private int tempMaxLights = PrefsHelper.MaxLights;

        private const float LabelHeight = 31f;

        private const float ButtonHeight = 30f;

        private const float SliderHeight = 30f;

        private const float CheckboxAreaHeight = 30f;

        private const float Gap = 12f;

        private const float SpaceBetweenColumns = 20f;

        private static readonly string[] SSAOQualityLabelKeys = new string[] { "SSAOQuality_Off", "SSAOQuality_Low", "SSAOQuality_Normal", "SSAOQuality_High" };

        private static readonly string[] ShadowQualityLabelKeys = new string[] { "ShadowQuality_Low", "ShadowQuality_Normal", "ShadowQuality_High" };

        private static readonly ValueTuple<string, float>[] DownsamplingLevels = new ValueTuple<string, float>[]
        {
            new ValueTuple<string, float>("DownsamplingLevel_None", 1f),
            new ValueTuple<string, float>("DownsamplingLevel_OneAndHalf", 1.5f),
            new ValueTuple<string, float>("DownsamplingLevel_Two", 2f)
        };

        private static readonly Dictionary<string, string> OutlineLabelKeys = new Dictionary<string, string>
        {
            { "MultiRounded", "Outline_MultiPassRound" },
            { "Multi", "Outline_MultiPass" },
            { "Single", "Outline_SinglePass" }
        };
    }
}