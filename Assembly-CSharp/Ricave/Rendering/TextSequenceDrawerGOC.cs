using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class TextSequenceDrawerGOC : MonoBehaviour
    {
        private TextSequenceDrawerGOC.Node CurNode
        {
            get
            {
                return this.nodes[this.curNodeIndex];
            }
        }

        public bool Showing
        {
            get
            {
                return this.showing != null;
            }
        }

        private void OnGUI()
        {
            Widgets.ApplySkin();
            GUI.depth = -150;
            if (!this.Showing)
            {
                return;
            }
            GUIExtra.DrawRect(Widgets.ScreenRect.ExpandedBy(1f), Color.black);
            float num = Widgets.VirtualHeight * 0.025f;
            float num2 = num * 28f;
            Widgets.FontSize = Widgets.GetFontSizeToFitInHeight(num);
            Widgets.Align = TextAnchor.UpperCenter;
            float num3 = Widgets.ScreenCenter.y - Widgets.CalcHeight(this.CurNode.text, num2) / 2f - this.curScrollY;
            float num4 = num * 0.65f;
            if (Event.current.type == EventType.Repaint)
            {
                if (this.CurNode.showCutsceneImage)
                {
                    this.cutsceneImageAlpha = Calc.StepTowards(this.cutsceneImageAlpha, 1f, Clock.UnscaledDeltaTime * 2f);
                }
                else
                {
                    this.cutsceneImageAlpha = Calc.StepTowards(this.cutsceneImageAlpha, 0f, Clock.UnscaledDeltaTime * 7f);
                }
            }
            if (this.cutsceneImageAlpha > 0f)
            {
                if (TextSequenceDrawerGOC.CutsceneImage == null)
                {
                    TextSequenceDrawerGOC.CutsceneImage = Assets.Get<Texture2D>("Textures/UI/Cutscene");
                }
                GUI.color = new Color(1f, 1f, 1f, this.cutsceneImageAlpha);
                GUI.DrawTexture(Widgets.ScreenRect.ContractedToSquare().ExpandedByPct(0.1f), TextSequenceDrawerGOC.CutsceneImage);
                GUI.color = Color.white;
            }
            this.DoLabel(this.CurNode.text, num2, ref num3, 0f, this.CurNode.textColor, 1f);
            if (this.CurNode.showCredits)
            {
                num3 += num * 3.5f;
                this.DoLabel("GameCreatedBy".Translate(), num2, ref num3, this.startCreditScrollAfterTime, null, 1f);
                num3 += num4;
                this.DoLabel("Piotr Walczak (ison)", num2, ref num3, this.startCreditScrollAfterTime + 0.3f, null, 1f);
                num3 += num4;
                num3 += num * 3.5f;
                this.DoLabel("MusicBy".Translate(), num2, ref num3, this.startCreditScrollAfterTime + 2f, null, 1f);
                num3 += num4;
                this.DoLabel("Misterbates", num2, ref num3, this.startCreditScrollAfterTime + 2.3f, null, 1f);
                num3 += num4;
                num3 += num * 3.5f;
                this.DoLabel("MainMenuArtBy".Translate(), num2, ref num3, this.startCreditScrollAfterTime + 3f, null, 1f);
                num3 += num4;
                this.DoLabel("Ádám Pásztor (Adam Scythe)", num2, ref num3, this.startCreditScrollAfterTime + 3.3f, null, 1f);
                num3 += num4;
                num3 += num * 3.5f;
                this.DoLabel("CharacterModelsBy".Translate(), num2, ref num3, this.startCreditScrollAfterTime + 4f, null, 1f);
                num3 += num4;
                this.DoLabel("Csongor Dobesch", num2, ref num3, this.startCreditScrollAfterTime + 4.3f, null, 1f);
                num3 += num4;
                num3 += num * 3.5f;
                this.DoLabel("SoundEffectsBy".Translate(), num2, ref num3, this.startCreditScrollAfterTime + 5f, null, 1f);
                num3 += num4;
                this.DoLabel("AgentDD, AlanCat, aphexx_, ady_vornicu, afrosoundBr, animatik, angelkunev, Audionautics, Aesterial-Arts, AlaskaRobotics, ani_music, alec_mackay, apolloaiello, adgawrhbshbffsfgvsrf, Aurelon, Bash360, Benboncan, bigal13, bajko, bogenseeberg, bsperan, berglindsi, Bpianoholic, Beetlemuse, BeezleFM, Bertsz, Bradovic, carrieedick, columbia23, cameronmusic, complex_waveform, craigglenday, cetsoundcrew, D4XX, Denys, dheming, Darsycho, dersuperanton, discokingmusic, ericnorcross81, earth_cord, esperar, eardeer, EugeneEverett, EminYILDIRIM, Euphrosyyn, Envywolf, Eponn, Erdie, Fabrizio84, fastson, FireFlame74, FK-Phantom, Freed, Garuda1982, gladkiy, Guz99, Groadr, goldendiaphragm, Gr0paru, Glaneur de sons, gneube, grunz, Halgrimm, HorrorAudio, henriquedesa, Headphaze, iwanPlays, InspectorJ, JonnyRuss01, j1987, JoelAudio, julianmateo_, JustInvoke, Jeffreys2, klankbeeld, KevinHilt, kiddpark, kjartan_abel, Kukensius, kyles, lipalearning, Litruv, Leady, laggyluk, lala_davis554, launemax, lesaucisson, Legnalegna55, LG, LittleRobotSoundFactory, martin-bn, Metzik, mateusboga, malexmedia, MATRIXXX_, mucky_pete7, MeijstroAudio, Mega-X-stream, MLaudio, Mortifreshman, Migfus20, mrickey13, Nakhas, Nerdwizard78, Nanakisan, Ndheger, Notarget, NXRT, newagesoup, NikPlaymostories, nezuai, newlocknew, NillyPlays, Nox_Sound, original_sound, othercee, oggraphics, Osiruswaltz, pauliep83, Peacewaves, plasterbrain, PaulOcone, pugaeme, PhreaKsAccount, pikachu09, Planet-Leader, plucinskicasey, proolsen, Ranner, Rickplayer, ryanconway, Rudmer_Rotteveel, RyanPBaskett, renatalmar, rhodesmas, sickfin, Slave2theLight, sandyrb, scorpion67890, silencyo, SilverIllusionist, Simon Spiers, SpliceSound, Shamewap, Sirkoto51, shiversmedia, sijam11, sjturia, SoundFlakes, Soughtaftersounds, spookymodem, suntemple, szymalix, timgormly, ToastHatter, Thimblerig, TMFKSOFT, tm1000, TheScarlettWitch89, Tomlija, Turrus, therover, toefur, tbsounddesigns, unfa, waveplaySFX, Wakerone, wertstahl, WeeJee_vdH, wly, Weak_Hero, zatar, vrodge, Volvion, yadronoff, 14GPanskaLetko_Dominik, xkeril", num2, ref num3, this.startCreditScrollAfterTime + 5.3f, null, 1f);
                num3 += num4;
                num3 += num * 3.5f;
                this.DoLabel("ThanksForPlaying".Translate(), num2, ref num3, this.startCreditScrollAfterTime + 6f, null, 1f);
                num3 += num4;
                if (Event.current.type == EventType.Repaint && Clock.UnscaledTime - this.CurNode.extraDelay >= this.curNodeStartTime + this.startCreditScrollAfterTime)
                {
                    this.curScrollY += num * 2f * Clock.UnscaledDeltaTime;
                    float num5 = num3 - Widgets.ScreenCenter.y + this.curScrollY;
                    this.curScrollY = Math.Min(this.curScrollY, num5);
                }
            }
            else if (this.CurNode.askForPlayerName)
            {
                this.playerName = this.DoTextField(this.playerName, num2, ref num3);
            }
            else
            {
                num3 += num4;
                this.DoLabel("PressSpaceToContinue".Translate().FormattedKeyBindings(), num2, ref num3, 0f, new Color?(Color.gray), 0.75f);
            }
            Widgets.ResetAlign();
            Widgets.ResetFontSize();
            GUI.color = Color.white;
            Get.WindowManager.CloseAll();
            Get.UI.CloseWheelSelector(false);
            this.HandleGUIEvents();
        }

        private void DoLabel(string label, float width, ref float curY, float fadeInOffset, Color? color = null, float fontSizeFactor = 1f)
        {
            float num = Calc.ResolveFadeIn(Clock.UnscaledTime - this.CurNode.extraDelay - this.curNodeStartTime - fadeInOffset, this.CurNode.fadeInDuration ?? 2f);
            GUI.color = ((color != null) ? color.GetValueOrDefault().WithAlphaFactor(num) : new Color(1f, 1f, 1f, num));
            int fontSize = Widgets.FontSize;
            if (fontSizeFactor != 1f)
            {
                Widgets.FontSize = Calc.RoundToInt((float)fontSize * fontSizeFactor);
            }
            float num2 = Widgets.CalcHeight(label, width);
            Widgets.Label(new Rect(Widgets.ScreenCenter.x - width / 2f, curY, width, num2), label, true, null, null, false);
            curY += num2;
            Widgets.FontSize = fontSize;
        }

        private string DoTextField(string text, float width, ref float curY)
        {
            float num = Calc.ResolveFadeIn(Clock.UnscaledTime - this.CurNode.extraDelay - this.curNodeStartTime, this.CurNode.fadeInDuration ?? 2f);
            GUI.color = new Color(1f, 1f, 1f, num);
            float num2 = Widgets.CalcHeight("ABC", width);
            Rect rect = new Rect(Widgets.ScreenCenter.x - width / 2f, curY, width, num2);
            GUI.SetNextControlName("TextSequenceDrawerTextField");
            text = Widgets.TextField(rect, text, false, true);
            curY += num2;
            if (text.Length > 22)
            {
                text = text.Substring(0, 22);
            }
            if (text.NullOrWhitespace())
            {
                text = "";
            }
            GUI.FocusControl("TextSequenceDrawerTextField");
            return text;
        }

        public void StartFadeOut(TextSequence textSequence)
        {
            if (Root.ChangingScene || Get.ScreenFader.AnyActionQueued)
            {
                return;
            }
            Get.ScreenFader.FadeOutAndExecute(delegate
            {
                this.Show(textSequence, null);
            }, new float?(6f), false, false, false);
            if (textSequence == TextSequence.Ending)
            {
                Get.Sound_Ending.PlayOneShot(null, 1f, 1f);
            }
        }

        public void Show(TextSequence textSequence, string singleText = null)
        {
            this.showing = new TextSequence?(textSequence);
            this.singleText = singleText;
            this.curNodeStartTime = Clock.UnscaledTime;
            this.curScrollY = 0f;
            this.curNodeIndex = 0;
            this.startCreditScrollAfterTime = 6f;
            this.nodes = TextSequenceDrawerInitializer.GetNodesFor(textSequence, singleText);
            this.cutsceneImageAlpha = 0f;
            base.gameObject.SetActive(true);
        }

        public void HandleGUIEvents()
        {
            if (!this.Showing)
            {
                return;
            }
            if (Clock.UnscaledTime - this.curNodeStartTime >= 0.2f)
            {
                if ((Get.KeyBinding_Wait.JustPressed || Get.KeyBinding_Accept.JustPressed) && !this.CurNode.askForPlayerName)
                {
                    this.< HandleGUIEvents > g__Proceed | 21_0();
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    if (Event.current.type == EventType.KeyDown)
                    {
                        Event.current.Use();
                    }
                }
                else if (Get.KeyBinding_Accept.JustPressed && this.CurNode.askForPlayerName && !this.playerName.NullOrWhitespace())
                {
                    this.< HandleGUIEvents > g__Proceed | 21_0();
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    if (Event.current.type == EventType.KeyDown)
                    {
                        Event.current.Use();
                    }
                }
                else if (this.CurNode.showCredits && KeyCodeUtility.CancelJustPressed)
                {
                    this.Finish();
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    if (Event.current.type == EventType.KeyDown)
                    {
                        Event.current.Use();
                    }
                }
            }
            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
            {
                Event.current.Use();
            }
        }

        private void Finish()
        {
            TextSequence? textSequence = this.showing;
            TextSequence textSequence2 = TextSequence.Intro;
            if (!((textSequence.GetValueOrDefault() == textSequence2) & (textSequence != null)))
            {
                textSequence = this.showing;
                textSequence2 = TextSequence.SingleText;
                if (!((textSequence.GetValueOrDefault() == textSequence2) & (textSequence != null)))
                {
                    textSequence = this.showing;
                    textSequence2 = TextSequence.AfterFirstDungeonCompleted;
                    if (!((textSequence.GetValueOrDefault() == textSequence2) & (textSequence != null)))
                    {
                        goto IL_0061;
                    }
                }
            }
            Get.UI.ShowWelcomeTexts();
        IL_0061:
            bool goToMainMenu = this.CurNode.goToMainMenu;
            this.showing = null;
            this.singleText = null;
            base.gameObject.SetActive(false);
            Get.ScreenFader.FadeFromBlack();
            Get.LessonDrawerGOC.OnTextSequenceDrawerFinished();
            if (goToMainMenu && !Root.ChangingScene && !Get.ScreenFader.AnyActionQueued)
            {
                Get.Run.Save();
                Get.Progress.Save();
                Get.MusicManager.ForceSilenceFor(1f);
                Get.MusicManager.ForciblyFadeOutCurrentMusic();
                Get.ScreenFader.FadeOutAndExecute(new Action(Root.LoadMainMenuScene), null, false, true, false);
            }
        }

        [CompilerGenerated]
        private void <HandleGUIEvents>g__Proceed|21_0()
		{
			if (this.CurNode.askForPlayerName)

            {
            Get.Progress.PlayerName = this.playerName.Trim();
            SteamKeyboardUtility.OnTextFieldLikelyNoLongerFocused();
        }
			if (this.curNodeIndex == this.nodes.Count - 1)

            {
            if (!this.CurNode.showCredits)
            {
                this.Finish();
                return;
            }
            if (Clock.UnscaledTime < this.curNodeStartTime + this.startCreditScrollAfterTime)
            {
                this.startCreditScrollAfterTime = Clock.UnscaledTime - this.curNodeStartTime;
                return;
            }
        }
			else

            {
            this.curNodeIndex++;
            this.curNodeStartTime = Clock.UnscaledTime;
            if (this.curNodeIndex < this.nodes.Count && this.CurNode.sound != null)
            {
                this.CurNode.sound.PlayOneShot(null, 1f, 1f);
            }
        }
        }

        private TextSequence? showing;

        private string singleText;

        private float curScrollY;

        private List<TextSequenceDrawerGOC.Node> nodes;

        private int curNodeIndex;

        private float curNodeStartTime;

        private float startCreditScrollAfterTime = 6f;

        private string playerName = "";

        private float cutsceneImageAlpha;

        private const float DefaultStartCreditScrollAfterTime = 6f;

        private static Texture2D CutsceneImage;

        public struct Node
        {
            public string text;

            public bool showCredits;

            public float? fadeInDuration;

            public float extraDelay;

            public bool askForPlayerName;

            public bool goToMainMenu;

            public SoundSpec sound;

            public bool showCutsceneImage;

            public Color? textColor;
        }
    }
}