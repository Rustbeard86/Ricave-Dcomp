using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class MainMenuUI
    {
        public WindowManager WindowManager
        {
            get
            {
                return this.windowManager;
            }
        }

        public Tooltips Tooltips
        {
            get
            {
                return this.tooltips;
            }
        }

        public DragAndDrop DragAndDrop
        {
            get
            {
                return this.dragAndDrop;
            }
        }

        private float MainButtonsStartY
        {
            get
            {
                return Widgets.ScreenCenter.y - 177.5f;
            }
        }

        public RootGOC RootGOC
        {
            get
            {
                return this.rootGOC;
            }
        }

        public ScreenFaderGOC ScreenFader
        {
            get
            {
                return this.screenFader;
            }
        }

        public MouseAttachmentDrawerGOC MouseAttachmentDrawerGOC
        {
            get
            {
                return this.mouseAttachmentDrawerGOC;
            }
        }

        public DevConsoleGOC DevConsole
        {
            get
            {
                return this.devConsole;
            }
        }

        public Camera Camera
        {
            get
            {
                return this.camera;
            }
        }

        public void Init()
        {
            Mouse.SetCursorLocked(false);
            this.rootGOC = GameObject.Find("Root").GetComponent<RootGOC>();
            this.screenFader = GameObject.Find("ScreenFader").GetComponent<ScreenFaderGOC>();
            this.mouseAttachmentDrawerGOC = GameObject.Find("MouseAttachment").GetComponent<MouseAttachmentDrawerGOC>();
            this.mouseAttachmentDrawerGOC.gameObject.SetActive(false);
            GameObject gameObject = GameObject.Find("DevConsole");
            this.devConsole = ((gameObject == null) ? null : gameObject.GetComponent<DevConsoleGOC>());
            this.camera = Camera.main;
            Get.CacheReferences();
            Shader.SetGlobalColor(Get.ShaderPropertyIDs.SkyColorID, Color.black);
            Shader.SetGlobalFloat(Get.ShaderPropertyIDs.LightningStrikeID, 0f);
        }

        public void OnGUI()
        {
            Get.ModsEventsManager.CallEvent(ModEventType.MainMenuOnGUIEarly, null);
            try
            {
                Profiler.Begin("tooltips.OnGUI()");
                this.tooltips.OnGUI();
            }
            catch (Exception ex)
            {
                Log.Error("Error in tooltips.OnGUI().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("windowManager.CheckCloseWindowsOnClickedOutside()");
                this.windowManager.CheckCloseWindowsOnClickedOutside();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in windowManager.CheckCloseWindowsOnClickedOutside().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("dragAndDrop.OnGUI()");
                this.dragAndDrop.OnGUI();
            }
            catch (Exception ex3)
            {
                Log.Error("Error in dragAndDrop.OnGUI().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("windowManager.OnGUI()");
                this.windowManager.OnGUI();
            }
            catch (Exception ex4)
            {
                Log.Error("Error in windowManager.OnGUI().", ex4);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("DoMainMenu()");
                this.DoMainMenu();
            }
            catch (Exception ex5)
            {
                Log.Error("Error in DoMainMenu().", ex5);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("FPSCounter.OnGUI()");
                FPSCounter.OnGUI();
            }
            catch (Exception ex6)
            {
                Log.Error("Error in FPSCounter.OnGUI().", ex6);
            }
            finally
            {
                Profiler.End();
            }
            Get.ModsEventsManager.CallEvent(ModEventType.MainMenuOnGUILate, null);
            try
            {
                Profiler.Begin("windowManager.LateOnGUI()");
                this.windowManager.LateOnGUI();
            }
            catch (Exception ex7)
            {
                Log.Error("Error in windowManager.LateOnGUI().", ex7);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void Update()
        {
            Get.Camera.transform.eulerAngles = new Vector3(0f, Clock.Time * 0.8f, 0f);
        }

        public void FixedUpdate()
        {
            Vector2 vector = Input.mousePosition / Widgets.UIScale;
            vector = new Vector2(Calc.Clamp(vector.x, 0f, Widgets.VirtualWidth), Calc.Clamp(vector.y, 0f, Widgets.VirtualHeight));
            Vector2 vector2 = (vector - Widgets.ScreenCenter) / (Math.Max(Widgets.VirtualWidth, Widgets.VirtualHeight) / 2f);
            vector2 = new Vector2(Calc.Clamp(vector2.x, -1f, 1f), Calc.Clamp(vector2.y, -1f, 1f));
            vector2 = new Vector2(-vector2.x, vector2.y);
            this.curParallaxPos = Vector2.Lerp(this.curParallaxPos, vector2, 0.1f);
        }

        private void DoMainMenu()
        {
            this.DoBackground();
            VersionUI.DrawVersion();
            if (!this.AnyWindowExceptContextMenuOpen())
            {
                GUIExtra.DrawRect(new Rect(80f, 0f, 610f, Widgets.VirtualHeight), new Color(0f, 0f, 0f, 0.6f));
                this.DoMainButtons();
                this.DoSocialMedia();
                Vector2 screenCenter = Widgets.ScreenCenter;
                GUIExtra.DrawTexture(new Vector2(360f, screenCenter.y - 280f), 500f, MainMenuUI.LogoWithName);
                if (ControllerUtility.InControllerMode)
                {
                    Widgets.FontBold = true;
                    Widgets.FontSizeScalable = 16;
                    if (Get.KeyBinding_Accept.Glyph != null)
                    {
                        Widgets.IconAndLabelCentered(Widgets.ScreenCenter.MovedBy(-100f, Widgets.VirtualHeight / 2f - 50f), Get.KeyBinding_Accept.Glyph, Color.white, Get.KeyBinding_Accept.LabelCap, Color.white, 30f, 6f, null);
                    }
                    if (Get.KeyBinding_RotateRight.Glyph != null)
                    {
                        Widgets.IconAndLabelCentered(Widgets.ScreenCenter.MovedBy(100f, Widgets.VirtualHeight / 2f - 50f), Get.KeyBinding_RotateRight.Glyph, Color.white, "Back".Translate(), Color.white, 30f, 6f, null);
                    }
                    Widgets.ResetFontSize();
                    Widgets.FontBold = false;
                }
                if (Widgets.Button(new Rect(Widgets.VirtualWidth - 210f, Widgets.VirtualHeight - 50f, 200f, 40f), Get.ActiveLanguage.Info.EnglishAndNativeName, true, null, null, true, true, true, true, null, true, true, null, false, null, null))
                {
                    this.windowManager.OpenContextMenu(this.GetLanguageChoices(), "ChooseLanguage".Translate());
                }
            }
        }

        private void DoBackground()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            this.CheckLoadMenuBackground();
            float num = Math.Max(Widgets.VirtualWidth, Widgets.VirtualHeight) * 0.007f;
            float num2 = 0f;
            for (int i = 0; i < MainMenuUI.BackgroundImages.Length; i++)
            {
                if (MainMenuUI.BackgroundImages[i].strength > num2)
                {
                    num2 = MainMenuUI.BackgroundImages[i].strength;
                }
            }
            Vector2 vector = new Vector2(num, num);
            if (Widgets.VirtualWidth > Widgets.VirtualHeight)
            {
                vector.y /= Widgets.VirtualWidth / Widgets.VirtualHeight;
            }
            else
            {
                vector.x /= Widgets.VirtualHeight / Widgets.VirtualWidth;
            }
            vector *= num2;
            Rect rect = RectUtility.CenteredAt(Widgets.ScreenCenter, (float)MainMenuUI.BackgroundImages[0].image.width, (float)MainMenuUI.BackgroundImages[0].image.height).ScaledToCover_MaintainAspectRatio(Widgets.VirtualWidth + vector.x * 2f, Widgets.VirtualHeight + vector.y * 2f);
            for (int j = 0; j < MainMenuUI.BackgroundImages.Length; j++)
            {
                Rect rect2 = rect.MovedBy(num * this.curParallaxPos.x * MainMenuUI.BackgroundImages[j].strength, num * this.curParallaxPos.y * MainMenuUI.BackgroundImages[j].strength);
                if (MainMenuUI.BackgroundImages[j].flicker)
                {
                    GUI.color = new Color(1f, 1f, 1f, Noise.PerlinNoise(Clock.UnscaledTime * 1.9f, 4817f + (float)j * 2384f) * 0.33f + 0.67f);
                }
                GUI.DrawTexture(rect2, MainMenuUI.BackgroundImages[j].image);
                GUI.color = Color.white;
            }
        }

        private void DoMainButtons()
        {
            float num = this.MainButtonsStartY;
            Rect rect = new Rect(120f, num, 250f, 55f);
            string text;
            if (Get.LessonManager.TutorialAreaPassed && Get.Progress.PlayerName != null)
            {
                text = "Play".Translate();
                Get.Tooltips.RegisterTip(rect, Get.Progress.PlayerName.AppendedInNewLine("Level".Translate().AppendedWithSpace(Get.Progress.ScoreLevel.ToStringCached())), null);
            }
            else
            {
                text = "Play".Translate();
            }
            this.curButtonIndex = 0;
            if (this.Button(rect, text, MainMenuUI.PlayIcon) && !Root.ChangingScene && !Get.ScreenFader.AnyActionQueued)
            {
                delegate
                {
                    if (!Root.ChangingScene && !Get.ScreenFader.AnyActionQueued)
                    {
                        if (Get.Progress.FileExistsButCouldNotRead)
                        {
                            Log.Error("Can't start the game because could not read the progress file. Otherwise proceeding would overwrite the progress file.", false);
                            return;
                        }
                        Get.Progress.OnPressedPlayInMainMenu();
                        Get.MusicManager.ForceSilenceFor(1f);
                        Get.MusicManager.ForciblyFadeOutCurrentMusic();
                        if (File.Exists(FilePaths.Savefile("Current")))
                        {
                            Get.ScreenFader.FadeOutAndExecute(delegate
                            {
                                Get.ModManager.ModsPlayerWantsActive.ActivateAndDeactiveModsAsRequested();
                                Root.LoadPlayScene(RootOnSceneChanged.LoadRun("Current"));
                            }, null, false, true, false);
                            return;
                        }
                        if (!Get.LessonManager.TutorialAreaPassed)
                        {
                            Get.ScreenFader.FadeOutAndExecute(delegate
                            {
                                Get.ModManager.ModsPlayerWantsActive.ActivateAndDeactiveModsAsRequested();
                                Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(new RunConfig(Get.Run_Tutorial, Rand.Int, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null)));
                            }, null, false, true, false);
                            return;
                        }
                        Get.ScreenFader.FadeOutAndExecute(delegate
                        {
                            Get.ModManager.ModsPlayerWantsActive.ActivateAndDeactiveModsAsRequested();
                            Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(new RunConfig(Get.Run_Lobby, Rand.Int, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null)));
                        }, null, false, true, false);
                    }
                }();
            }
            num += 75f;
            if (this.Button(new Rect(120f, num, rect.width, 55f), "Options".Translate(), MainMenuUI.OptionsIcon))
            {
                Get.WindowManager.Open(Get.Window_Options, false);
            }
            num += 75f;
            if (this.Button(new Rect(120f, num, rect.width, 55f), "Mods".Translate(), MainMenuUI.ModsIcon))
            {
                Get.WindowManager.Open(Get.Window_Mods, false);
            }
            num += 75f;
            if (this.Button(new Rect(120f, num, rect.width, 55f), "Credits".Translate(), MainMenuUI.CreditsIcon))
            {
                Get.WindowManager.Open(Get.Window_Credits, false);
            }
            num += 75f;
            if (this.Button(new Rect(120f, num, rect.width, 55f), "Quit".Translate(), MainMenuUI.ExitIcon))
            {
                Root.Quit();
            }
            if (ControllerUtility.InControllerMode && !Get.WindowManager.AnyWindowOpen)
            {
                if (Get.KeyBinding_MoveForward.JustPressed || Get.KeyBinding_MoveForwardAlt.JustPressed)
                {
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    this.selectedButtonForController = Math.Max(this.selectedButtonForController - 1, 0);
                }
                if (Get.KeyBinding_MoveBack.JustPressed || Get.KeyBinding_MoveBackAlt.JustPressed)
                {
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    this.selectedButtonForController = Math.Min(this.selectedButtonForController + 1, this.curButtonIndex - 1);
                }
            }
        }

        private bool Button(Rect rect, string label, Texture2D icon)
        {
            if (ControllerUtility.InControllerMode && this.curButtonIndex == this.selectedButtonForController)
            {
                float num = Calc.PulseUnscaled(5f, 10f);
                GUIExtra.DrawTextureRotated(rect.MovedBy(-38f - num, 0f).LeftPart(rect.height).ContractedByPct(0.12f), MainMenuUI.ArrowTex, 270f, null);
                Widgets.FontBold = true;
            }
            Rect rect2 = rect;
            bool flag = true;
            int? num2 = new int?(32);
            bool flag2 = Widgets.Button(rect2, label, flag, null, null, true, true, true, true, null, false, true, num2, true, null, null);
            if (ControllerUtility.InControllerMode && this.curButtonIndex == this.selectedButtonForController)
            {
                Widgets.FontBold = false;
                if (!Get.WindowManager.AnyWindowOpen && Get.KeyBinding_Accept.JustPressed)
                {
                    flag2 = true;
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                }
            }
            this.curButtonIndex++;
            return flag2;
        }

        private void DoSocialMedia()
        {
            float num = this.MainButtonsStartY;
            float num2 = 375f;
            for (int i = 0; i < MainMenuUI.SocialMedia.Length; i++)
            {
                ValueTuple<Texture2D, string, string, string, Color> valueTuple = MainMenuUI.SocialMedia[i];
                Texture2D item = valueTuple.Item1;
                string item2 = valueTuple.Item2;
                string item3 = valueTuple.Item3;
                string item4 = valueTuple.Item4;
                Color item5 = valueTuple.Item5;
                SocialMediaDrawer.DrawAt(item, item2, item3, item4, item5, new Vector2(num2, num));
                num += 75f;
            }
        }

        private bool AnyWindowExceptContextMenuOpen()
        {
            using (List<Window>.Enumerator enumerator = this.windowManager.Windows.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (!(enumerator.Current is Window_ContextMenu))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private List<Window_ContextMenu.Option> GetLanguageChoices()
        {
            return Get.Languages.AllLanguages.Select<LanguageInfo, Window_ContextMenu.Option>((LanguageInfo x) => new Window_ContextMenu.Option(x.EnglishAndNativeName, delegate
            {
                this.ChooseLanguage(x);
            }, null)).ToList<Window_ContextMenu.Option>();
        }

        private void ChooseLanguage(LanguageInfo language)
        {
            Log.Message("Reloading all content because language changed.");
            Get.Languages.SetActiveLanguageInPrefs(language.EnglishName);
            Get.ModManager.ReloadAllContent();
        }

        private void CheckLoadMenuBackground()
        {
            for (int i = 0; i < MainMenuUI.BackgroundImages.Length; i++)
            {
                if (!(MainMenuUI.BackgroundImages[i].image != null))
                {
                    MainMenuUI.ParallaxImage parallaxImage = MainMenuUI.BackgroundImages[i];
                    parallaxImage.image = Assets.Get<Texture2D>(MainMenuUI.BackgroundImages[i].imagePath);
                    MainMenuUI.BackgroundImages[i] = parallaxImage;
                }
            }
        }

        public static void UnloadMenuBackground()
        {
            for (int i = 0; i < MainMenuUI.BackgroundImages.Length; i++)
            {
                if (!(MainMenuUI.BackgroundImages[i].image == null))
                {
                    Resources.UnloadAsset(MainMenuUI.BackgroundImages[i].image);
                    MainMenuUI.ParallaxImage parallaxImage = MainMenuUI.BackgroundImages[i];
                    parallaxImage.image = null;
                    MainMenuUI.BackgroundImages[i] = parallaxImage;
                }
            }
        }

        private Vector2 curParallaxPos;

        private int selectedButtonForController;

        private int curButtonIndex;

        private WindowManager windowManager = new WindowManager();

        private Tooltips tooltips = new Tooltips();

        private DragAndDrop dragAndDrop = new DragAndDrop();

        private RootGOC rootGOC;

        private ScreenFaderGOC screenFader;

        private MouseAttachmentDrawerGOC mouseAttachmentDrawerGOC;

        private DevConsoleGOC devConsole;

        private Camera camera;

        public static readonly string DiscordServerAddress = "https://discord.gg/u3zwufW4f4";

        private static readonly ValueTuple<Texture2D, string, string, string, Color>[] SocialMedia = new ValueTuple<Texture2D, string, string, string, Color>[]
        {
            new ValueTuple<Texture2D, string, string, string, Color>(Assets.Get<Texture2D>("Textures/UI/SocialMedia/Discord"), "Discord", "Ricave community", MainMenuUI.DiscordServerAddress, new Color(0.48f, 0.65f, 0.92f)),
            new ValueTuple<Texture2D, string, string, string, Color>(Assets.Get<Texture2D>("Textures/UI/SocialMedia/Twitter"), "Twitter", "@rogueatic", "https://twitter.com/rogueatic", new Color(0.31f, 0.67f, 0.93f)),
            new ValueTuple<Texture2D, string, string, string, Color>(Assets.Get<Texture2D>("Textures/UI/SocialMedia/Reddit"), "Reddit", "r/Ricave", "https://reddit.com/r/Ricave", new Color(0.93f, 0.66f, 0.46f)),
            new ValueTuple<Texture2D, string, string, string, Color>(Assets.Get<Texture2D>("Textures/UI/Logo"), "Blog", "ricave.com/blog", "https://ricave.com/blog", new Color(0.96f, 0.79f, 0.42f)),
            new ValueTuple<Texture2D, string, string, string, Color>(Assets.Get<Texture2D>("Textures/UI/SocialMedia/YouTube"), "YouTube", "Ricave videos", "https://www.youtube.com/@Rogueatic", new Color(0.96f, 0.49f, 0.42f))
        };

        private const float ParallaxLerpSpeed = 0.1f;

        private const float MaxParallaxMovePct = 0.007f;

        private static readonly MainMenuUI.ParallaxImage[] BackgroundImages = new MainMenuUI.ParallaxImage[]
        {
            new MainMenuUI.ParallaxImage
            {
                imagePath = "Textures/UI/MenuBackground/MenuBackground1",
                strength = 1f
            },
            new MainMenuUI.ParallaxImage
            {
                imagePath = "Textures/UI/MenuBackground/MenuBackground2",
                strength = 1.035f,
                flicker = true
            },
            new MainMenuUI.ParallaxImage
            {
                imagePath = "Textures/UI/MenuBackground/MenuBackground3",
                strength = 1.17f,
                flicker = true
            },
            new MainMenuUI.ParallaxImage
            {
                imagePath = "Textures/UI/MenuBackground/MenuBackground4",
                strength = 1.23f,
                flicker = true
            },
            new MainMenuUI.ParallaxImage
            {
                imagePath = "Textures/UI/MenuBackground/MenuBackground5",
                strength = 1.38f,
                flicker = true
            },
            new MainMenuUI.ParallaxImage
            {
                imagePath = "Textures/UI/MenuBackground/MenuBackground6",
                strength = 1.8f
            },
            new MainMenuUI.ParallaxImage
            {
                imagePath = "Textures/UI/MenuBackground/MenuBackground7",
                strength = 2f
            }
        };

        private static readonly Texture2D PlayIcon = Assets.Get<Texture2D>("Textures/UI/MenuIcons/Resume");

        private static readonly Texture2D OptionsIcon = Assets.Get<Texture2D>("Textures/UI/MenuIcons/Options");

        private static readonly Texture2D ModsIcon = Assets.Get<Texture2D>("Textures/UI/MenuIcons/Mods");

        private static readonly Texture2D CreditsIcon = Assets.Get<Texture2D>("Textures/UI/MenuIcons/Credits");

        private static readonly Texture2D ExitIcon = Assets.Get<Texture2D>("Textures/UI/MenuIcons/Exit");

        private static readonly Texture2D ArrowTex = Assets.Get<Texture2D>("Textures/UI/Arrow");

        private const int ButtonHeight = 55;

        private const int SpaceBetweenButtons = 20;

        private const int SpaceBetweenSocialMedia = 20;

        private const int ButtonCount = 5;

        private const float ButtonsLeftOffset = 120f;

        private static readonly Texture2D LogoWithName = Assets.Get<Texture2D>("Textures/UI/LogoWithName");

        private struct ParallaxImage
        {
            public string imagePath;

            public float strength;

            public bool flicker;

            public Texture2D image;
        }
    }
}