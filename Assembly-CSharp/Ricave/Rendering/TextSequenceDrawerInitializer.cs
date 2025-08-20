using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class TextSequenceDrawerInitializer
    {
        public static List<TextSequenceDrawerGOC.Node> GetNodesFor(TextSequence textSequence, string singleText = null)
        {
            List<TextSequenceDrawerGOC.Node> list = new List<TextSequenceDrawerGOC.Node>();
            if (textSequence == TextSequence.Intro)
            {
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Intro1".Translate(),
                    fadeInDuration = new float?(8f),
                    extraDelay = 0.5f
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Intro2".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Intro3".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Intro4".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Intro5".Translate(),
                    fadeInDuration = new float?(0.1f),
                    askForPlayerName = true
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Intro6".Translate(),
                    fadeInDuration = new float?(0.1f),
                    sound = Get.Sound_IntroAttention
                });
            }
            else if (textSequence == TextSequence.Ending)
            {
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending1".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending2".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending3".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending4".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending5".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending6".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending7".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending8".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending9".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending10".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending11".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending12".Translate()
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "Ending13".Translate(),
                    showCredits = true
                });
            }
            else if (textSequence == TextSequence.SingleText)
            {
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = singleText
                });
            }
            else if (textSequence == TextSequence.DemoEnd)
            {
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "ThanksForPlayingDemo".Translate(),
                    goToMainMenu = true
                });
            }
            else if (textSequence == TextSequence.AfterFirstDungeonCompleted)
            {
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "TextAfterFirstDungeonCompleted1".Translate(),
                    showCutsceneImage = true,
                    textColor = new Color?(new Color(1f, 0.3f, 0.3f))
                });
                list.Add(new TextSequenceDrawerGOC.Node
                {
                    text = "TextAfterFirstDungeonCompleted2".Translate()
                });
            }
            return list;
        }
    }
}