using System;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Credits : Window
    {
        protected override Color BackgroundColor
        {
            get
            {
                return Color.black;
            }
        }

        public Window_Credits(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            float x = Widgets.ScreenCenter.x;
            float num = 40f;
            Widgets.FontSizeScalable = 40;
            Widgets.LabelCentered(new Vector2(x, num), App.Name, true, null, null, false, false, false, null);
            Widgets.ResetFontSize();
            num += 60f;
            Widgets.LabelCentered(new Vector2(x, num), "GameCreatedBy".Translate(), true, null, null, false, false, false, null);
            num += 35f;
            Widgets.FontSizeScalable = 26;
            Widgets.LabelCentered(new Vector2(x, num), "Piotr Walczak (ison)", true, null, null, false, false, false, null);
            Widgets.ResetFontSize();
            num += 30f;
            Widgets.LabelCentered(new Vector2(x, num), "twitter.com/isononly", true, null, null, false, false, false, null);
            num += 60f;
            Widgets.LabelCentered(new Vector2(x, num), "MusicBy".Translate(), true, null, null, false, false, false, null);
            num += 35f;
            Widgets.FontSizeScalable = 26;
            Widgets.LabelCentered(new Vector2(x, num), "Misterbates", true, null, null, false, false, false, null);
            Widgets.ResetFontSize();
            num += 60f;
            Widgets.LabelCentered(new Vector2(x, num), "MainMenuArtBy".Translate(), true, null, null, false, false, false, null);
            num += 35f;
            Widgets.FontSizeScalable = 26;
            Widgets.LabelCentered(new Vector2(x, num), "Ádám Pásztor (Adam Scythe)", true, null, null, false, false, false, null);
            Widgets.ResetFontSize();
            num += 60f;
            Widgets.LabelCentered(new Vector2(x, num), "LogoBy".Translate(), true, null, null, false, false, false, null);
            num += 35f;
            Widgets.FontSizeScalable = 26;
            Widgets.LabelCentered(new Vector2(x, num), "Game Graphics05", true, null, null, false, false, false, null);
            Widgets.ResetFontSize();
            num += 60f;
            Widgets.LabelCentered(new Vector2(x, num), "CharacterModelsBy".Translate(), true, null, null, false, false, false, null);
            num += 35f;
            Widgets.FontSizeScalable = 26;
            Widgets.LabelCentered(new Vector2(x, num), "Csongor Dobesch", true, null, null, false, false, false, null);
            Widgets.ResetFontSize();
            num += 60f;
            Widgets.LabelCentered(new Vector2(x, num), "SoundEffectsBy".Translate(), true, null, null, false, false, false, null);
            num += 35f;
            Rect rect = new Rect(x - 470f, num, 940f, 9999f);
            Widgets.Align = TextAnchor.UpperCenter;
            Widgets.Label(rect, "AgentDD, AlanCat, aphexx_, ady_vornicu, afrosoundBr, animatik, angelkunev, Audionautics, Aesterial-Arts, AlaskaRobotics, ani_music, alec_mackay, apolloaiello, adgawrhbshbffsfgvsrf, Aurelon, Bash360, Benboncan, bigal13, bajko, bogenseeberg, bsperan, berglindsi, Bpianoholic, Beetlemuse, BeezleFM, Bertsz, Bradovic, carrieedick, columbia23, cameronmusic, complex_waveform, craigglenday, cetsoundcrew, D4XX, Denys, dheming, Darsycho, dersuperanton, discokingmusic, ericnorcross81, earth_cord, esperar, eardeer, EugeneEverett, EminYILDIRIM, Euphrosyyn, Envywolf, Eponn, Erdie, Fabrizio84, fastson, FireFlame74, FK-Phantom, Freed, Garuda1982, gladkiy, Guz99, Groadr, goldendiaphragm, Gr0paru, Glaneur de sons, gneube, grunz, Halgrimm, HorrorAudio, henriquedesa, Headphaze, iwanPlays, InspectorJ, JonnyRuss01, j1987, JoelAudio, julianmateo_, JustInvoke, Jeffreys2, klankbeeld, KevinHilt, kiddpark, kjartan_abel, Kukensius, kyles, lipalearning, Litruv, Leady, laggyluk, lala_davis554, launemax, lesaucisson, Legnalegna55, LG, LittleRobotSoundFactory, martin-bn, Metzik, mateusboga, malexmedia, MATRIXXX_, mucky_pete7, MeijstroAudio, Mega-X-stream, MLaudio, Mortifreshman, Migfus20, mrickey13, Nakhas, Nerdwizard78, Nanakisan, Ndheger, Notarget, NXRT, newagesoup, NikPlaymostories, nezuai, newlocknew, NillyPlays, Nox_Sound, original_sound, othercee, oggraphics, Osiruswaltz, pauliep83, Peacewaves, plasterbrain, PaulOcone, pugaeme, PhreaKsAccount, pikachu09, Planet-Leader, plucinskicasey, proolsen, Ranner, Rickplayer, ryanconway, Rudmer_Rotteveel, RyanPBaskett, renatalmar, rhodesmas, sickfin, Slave2theLight, sandyrb, scorpion67890, silencyo, SilverIllusionist, Simon Spiers, SpliceSound, Shamewap, Sirkoto51, shiversmedia, sijam11, sjturia, SoundFlakes, Soughtaftersounds, spookymodem, suntemple, szymalix, timgormly, ToastHatter, Thimblerig, TMFKSOFT, tm1000, TheScarlettWitch89, Tomlija, Turrus, therover, toefur, tbsounddesigns, unfa, waveplaySFX, Wakerone, wertstahl, WeeJee_vdH, wly, Weak_Hero, zatar, vrodge, Volvion, yadronoff, 14GPanskaLetko_Dominik, xkeril", true, null, null, false);
            Widgets.ResetAlign();
            num += Widgets.CalcHeight("AgentDD, AlanCat, aphexx_, ady_vornicu, afrosoundBr, animatik, angelkunev, Audionautics, Aesterial-Arts, AlaskaRobotics, ani_music, alec_mackay, apolloaiello, adgawrhbshbffsfgvsrf, Aurelon, Bash360, Benboncan, bigal13, bajko, bogenseeberg, bsperan, berglindsi, Bpianoholic, Beetlemuse, BeezleFM, Bertsz, Bradovic, carrieedick, columbia23, cameronmusic, complex_waveform, craigglenday, cetsoundcrew, D4XX, Denys, dheming, Darsycho, dersuperanton, discokingmusic, ericnorcross81, earth_cord, esperar, eardeer, EugeneEverett, EminYILDIRIM, Euphrosyyn, Envywolf, Eponn, Erdie, Fabrizio84, fastson, FireFlame74, FK-Phantom, Freed, Garuda1982, gladkiy, Guz99, Groadr, goldendiaphragm, Gr0paru, Glaneur de sons, gneube, grunz, Halgrimm, HorrorAudio, henriquedesa, Headphaze, iwanPlays, InspectorJ, JonnyRuss01, j1987, JoelAudio, julianmateo_, JustInvoke, Jeffreys2, klankbeeld, KevinHilt, kiddpark, kjartan_abel, Kukensius, kyles, lipalearning, Litruv, Leady, laggyluk, lala_davis554, launemax, lesaucisson, Legnalegna55, LG, LittleRobotSoundFactory, martin-bn, Metzik, mateusboga, malexmedia, MATRIXXX_, mucky_pete7, MeijstroAudio, Mega-X-stream, MLaudio, Mortifreshman, Migfus20, mrickey13, Nakhas, Nerdwizard78, Nanakisan, Ndheger, Notarget, NXRT, newagesoup, NikPlaymostories, nezuai, newlocknew, NillyPlays, Nox_Sound, original_sound, othercee, oggraphics, Osiruswaltz, pauliep83, Peacewaves, plasterbrain, PaulOcone, pugaeme, PhreaKsAccount, pikachu09, Planet-Leader, plucinskicasey, proolsen, Ranner, Rickplayer, ryanconway, Rudmer_Rotteveel, RyanPBaskett, renatalmar, rhodesmas, sickfin, Slave2theLight, sandyrb, scorpion67890, silencyo, SilverIllusionist, Simon Spiers, SpliceSound, Shamewap, Sirkoto51, shiversmedia, sijam11, sjturia, SoundFlakes, Soughtaftersounds, spookymodem, suntemple, szymalix, timgormly, ToastHatter, Thimblerig, TMFKSOFT, tm1000, TheScarlettWitch89, Tomlija, Turrus, therover, toefur, tbsounddesigns, unfa, waveplaySFX, Wakerone, wertstahl, WeeJee_vdH, wly, Weak_Hero, zatar, vrodge, Volvion, yadronoff, 14GPanskaLetko_Dominik, xkeril", rect.width);
            num += 60f;
            Widgets.FontSizeScalable = 26;
            Widgets.LabelCentered(new Vector2(x, num), "ThanksForPlaying".Translate(), true, null, null, false, false, false, null);
            Widgets.ResetFontSize();
        }

        public override void SetInitialRect()
        {
            Rect screenRect = Widgets.ScreenRect;
            screenRect.yMin -= 30f;
            base.Rect = screenRect.ExpandedBy(1f);
        }

        private const int VeryBigFont = 40;

        private const int BigFont = 26;

        public const string SoundAuthors = "AgentDD, AlanCat, aphexx_, ady_vornicu, afrosoundBr, animatik, angelkunev, Audionautics, Aesterial-Arts, AlaskaRobotics, ani_music, alec_mackay, apolloaiello, adgawrhbshbffsfgvsrf, Aurelon, Bash360, Benboncan, bigal13, bajko, bogenseeberg, bsperan, berglindsi, Bpianoholic, Beetlemuse, BeezleFM, Bertsz, Bradovic, carrieedick, columbia23, cameronmusic, complex_waveform, craigglenday, cetsoundcrew, D4XX, Denys, dheming, Darsycho, dersuperanton, discokingmusic, ericnorcross81, earth_cord, esperar, eardeer, EugeneEverett, EminYILDIRIM, Euphrosyyn, Envywolf, Eponn, Erdie, Fabrizio84, fastson, FireFlame74, FK-Phantom, Freed, Garuda1982, gladkiy, Guz99, Groadr, goldendiaphragm, Gr0paru, Glaneur de sons, gneube, grunz, Halgrimm, HorrorAudio, henriquedesa, Headphaze, iwanPlays, InspectorJ, JonnyRuss01, j1987, JoelAudio, julianmateo_, JustInvoke, Jeffreys2, klankbeeld, KevinHilt, kiddpark, kjartan_abel, Kukensius, kyles, lipalearning, Litruv, Leady, laggyluk, lala_davis554, launemax, lesaucisson, Legnalegna55, LG, LittleRobotSoundFactory, martin-bn, Metzik, mateusboga, malexmedia, MATRIXXX_, mucky_pete7, MeijstroAudio, Mega-X-stream, MLaudio, Mortifreshman, Migfus20, mrickey13, Nakhas, Nerdwizard78, Nanakisan, Ndheger, Notarget, NXRT, newagesoup, NikPlaymostories, nezuai, newlocknew, NillyPlays, Nox_Sound, original_sound, othercee, oggraphics, Osiruswaltz, pauliep83, Peacewaves, plasterbrain, PaulOcone, pugaeme, PhreaKsAccount, pikachu09, Planet-Leader, plucinskicasey, proolsen, Ranner, Rickplayer, ryanconway, Rudmer_Rotteveel, RyanPBaskett, renatalmar, rhodesmas, sickfin, Slave2theLight, sandyrb, scorpion67890, silencyo, SilverIllusionist, Simon Spiers, SpliceSound, Shamewap, Sirkoto51, shiversmedia, sijam11, sjturia, SoundFlakes, Soughtaftersounds, spookymodem, suntemple, szymalix, timgormly, ToastHatter, Thimblerig, TMFKSOFT, tm1000, TheScarlettWitch89, Tomlija, Turrus, therover, toefur, tbsounddesigns, unfa, waveplaySFX, Wakerone, wertstahl, WeeJee_vdH, wly, Weak_Hero, zatar, vrodge, Volvion, yadronoff, 14GPanskaLetko_Dominik, xkeril";
    }
}