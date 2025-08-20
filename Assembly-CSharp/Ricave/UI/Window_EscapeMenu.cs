using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_EscapeMenu : Window
    {
        public Window_EscapeMenu(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void SetInitialRect()
        {
            int num;
            if (Get.InLobby)
            {
                num = 3;
            }
            else
            {
                num = 5;
                if (Get.RunConfig.IsDailyChallenge)
                {
                    num--;
                }
                if (Get.RunSpec == Get.Run_Tutorial)
                {
                    num--;
                }
            }
            float num2 = 30f + base.Spec.Padding * 2f + (float)(num * 50) + (float)((num - 1) * 15);
            base.Rect = new Rect(Widgets.ScreenCenter.x - base.Spec.Size.x / 2f, Widgets.ScreenCenter.y - num2 / 2f, base.Spec.Size.x, num2);
        }

        protected override void DoWindowContents(Rect inRect)
        {
            float num = 0f;
            Rect rect = new Rect(0f, num, inRect.width, 50f);
            string text = "Resume".Translate();
            bool flag = true;
            Texture2D texture2D = Window_EscapeMenu.ResumeIcon;
            if (Widgets.Button(rect, text, flag, null, null, true, true, true, true, texture2D, true, true, null, false, null, null))
            {
                Get.WindowManager.Close(this, false);
            }
            num += 65f;
            Rect rect2 = new Rect(0f, num, inRect.width, 50f);
            string text2 = "Options".Translate();
            bool flag2 = true;
            texture2D = Window_EscapeMenu.OptionsIcon;
            if (Widgets.Button(rect2, text2, flag2, null, null, true, true, true, true, texture2D, true, true, null, false, null, null))
            {
                Get.WindowManager.Open(Get.Window_Options, false);
            }
            if (!Get.InLobby)
            {
                if (!Get.RunConfig.IsDailyChallenge)
                {
                    num += 65f;
                    Rect rect3 = new Rect(0f, num, inRect.width, 50f);
                    string text3 = this.TryAddWillLoseBoostsInfo("QuickRestart".Translate());
                    bool flag3 = true;
                    texture2D = Window_EscapeMenu.RestartIcon;
                    if (Widgets.Button(rect3, text3, flag3, null, null, true, true, true, true, texture2D, true, true, null, false, null, null))
                    {
                        Window_EscapeMenu.QuickRestart();
                    }
                }
                if (Get.RunSpec != Get.Run_Tutorial)
                {
                    num += 65f;
                    Rect rect4 = new Rect(0f, num, inRect.width, 50f);
                    string text4 = this.TryAddWillLoseBoostsInfo("QuitToLobby".Translate());
                    bool flag4 = true;
                    texture2D = Window_EscapeMenu.ExitToLobbyIcon;
                    if (Widgets.Button(rect4, text4, flag4, null, null, true, true, true, true, texture2D, true, true, null, false, null, null) && !Root.ChangingScene && !Get.ScreenFader.AnyActionQueued)
                    {
                        Get.ScreenFader.FadeOutAndExecute(delegate
                        {
                            Get.Progress.ApplyCurrentRunProgress();
                            Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(new RunConfig(Get.Run_Lobby, Rand.Int, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null)));
                        }, null, true, true, true);
                    }
                }
            }
            num += 65f;
            Rect rect5 = new Rect(0f, num, inRect.width, 50f);
            string text5 = "SaveAndExitGame".Translate();
            bool flag5 = true;
            texture2D = Window_EscapeMenu.ExitIcon;
            if (Widgets.Button(rect5, text5, flag5, null, null, true, true, true, true, texture2D, true, true, null, false, null, null) && !Root.ChangingScene && !Get.ScreenFader.AnyActionQueued)
            {
                Get.Run.Save();
                Get.Progress.Save();
                Get.WindowManager.Close(this, false);
                Get.UI.OnPressedQuitToMainMenuButton();
                Get.MusicManager.ForceSilenceFor(1f);
                Get.MusicManager.ForciblyFadeOutCurrentMusic();
                Get.ScreenFader.FadeOutAndExecute(new Action(Root.LoadMainMenuScene), null, false, true, false);
            }
        }

        public override void ExtraOnGUI()
        {
            base.ExtraOnGUI();
            VersionUI.DrawVersion();
            this.DoKeyBindings();
        }

        public override void OnClosed()
        {
            Get.WindowManager.CloseAll(Get.Window_Options);
            base.OnClosed();
        }

        private string TryAddWillLoseBoostsInfo(string text)
        {
            if (!Get.UnlockableManager.AnyDirectlyUnlockedSingleUseBoost && !Get.RunConfig.HasPetRat)
            {
                return text;
            }
            if (Get.InLobby || Get.RunConfig.ProgressDisabled)
            {
                return text;
            }
            return text.AppendedInNewLine(RichText.LightRed("({0})".Formatted("WillLoseBoosts".Translate())));
        }

        private void DoKeyBindings()
        {
            Rect rect = base.Rect.MovedBy(base.Rect.width, 0f).CutFromRightPct(0.1f);
            rect.height = this.lastKeyBindingsHeight + 40f;
            GUIExtra.DrawRoundedRectBump(rect, new Color(0f, 0f, 0f, 0.15f), false, true, true, true, true, null);
            rect = rect.ContractedBy(20f);
            float y = rect.y;
            if (ControllerUtility.InControllerMode)
            {
                if (ControllerUtility.ControllerType == ControllerType.SteamDeck)
                {
                    this.DoKeyBinding(Get.KeyBinding_Accept, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_RotateRight, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_RotateLeft, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Wait, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Rewind, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_ShowInventory, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Inspect, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Interact, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Cancel, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_QuickbarMenu, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Fly, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_MinimapAlt, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_InspectAlt, rect.x, rect.width, ref y);
                }
                else
                {
                    this.DoKeyBinding(Get.KeyBinding_Accept, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_RotateRight, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_RotateLeft, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Wait, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Rewind, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_ShowInventory, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Inspect, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Interact, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_QuickbarMenu, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Cancel, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_MinimapAlt, rect.x, rect.width, ref y);
                    this.DoKeyBinding(Get.KeyBinding_Fly, rect.x, rect.width, ref y);
                }
            }
            else
            {
                this.DoKeyBinding(Get.KeyBinding_RotateLeft, rect.x, rect.width, ref y);
                this.DoKeyBinding(Get.KeyBinding_RotateRight, rect.x, rect.width, ref y);
                this.DoKeyBinding(Get.KeyBinding_Wait, rect.x, rect.width, ref y);
                this.DoKeyBinding(Get.KeyBinding_ShowInventory, rect.x, rect.width, ref y);
                this.DoKeyBinding(Get.KeyBinding_Inspect, rect.x, rect.width, ref y);
                this.DoKeyBinding(Get.KeyBinding_AttackNearest, rect.x, rect.width, ref y);
                this.DoKeyBinding(Get.KeyBinding_Minimap, rect.x, rect.width, ref y);
                this.DoKeyBinding(Get.KeyBinding_Fly, rect.x, rect.width, ref y);
                this.DoKeyBinding(Get.KeyBinding_Rewind, rect.x, rect.width, ref y);
            }
            if (Event.current.type == EventType.Layout)
            {
                this.lastKeyBindingsHeight = y - rect.y;
            }
        }

        private void DoKeyBinding(KeyBindingSpec keyBinding, float x, float width, ref float curY)
        {
            this.DoKeyBinding(RichText.Bold(keyBinding.KeyCode.ToStringCached()), keyBinding.LabelCap, x, width, ref curY);
        }

        private void DoKeyBinding(string left, string right, float x, float width, ref float curY)
        {
            string text = "{0} - {1}".Formatted(left, right);
            GUI.color = new Color(0.52f, 0.52f, 0.52f);
            Widgets.Label(x, width, ref curY, text, true);
            GUI.color = Color.white;
        }

        public static void QuickRestart()
        {
            if (!Root.ChangingScene && !Get.ScreenFader.AnyActionQueued)
            {
                Get.ScreenFader.FadeOutAndExecute(delegate
                {
                    bool flag;
                    if (Get.RunConfig.HasPetRat && Get.Progress.PetRatSatiation > 0)
                    {
                        Progress progress = Get.Progress;
                        int petRatSatiation = progress.PetRatSatiation;
                        progress.PetRatSatiation = petRatSatiation - 1;
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                    Get.Progress.ApplyCurrentRunProgress();
                    Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(Get.RunConfig.CopyWithSavefileNameAndSeed("Current", Get.RunConfig.ProgressDisabled ? Get.RunConfig.RunSeed : Rand.Int, flag)));
                }, null, true, true, true);
            }
        }

        private const int ButtonHeight = 50;

        private const int SpaceBetweenButtons = 15;

        private static readonly Texture2D ResumeIcon = Assets.Get<Texture2D>("Textures/UI/MenuIcons/Resume");

        private static readonly Texture2D OptionsIcon = Assets.Get<Texture2D>("Textures/UI/MenuIcons/Options");

        private static readonly Texture2D RestartIcon = Assets.Get<Texture2D>("Textures/UI/MenuIcons/Restart");

        private static readonly Texture2D ExitToLobbyIcon = Assets.Get<Texture2D>("Textures/UI/MenuIcons/ExitToLobby");

        private static readonly Texture2D ExitIcon = Assets.Get<Texture2D>("Textures/UI/MenuIcons/Exit");

        private float lastKeyBindingsHeight;
    }
}