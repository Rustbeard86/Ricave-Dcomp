using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class DialogueDrawer
    {
        public static void Draw(DialogueSpec dialogueSpec, float x, float width, ref float curY, ref float letterShowingTimer)
        {
            if ((Event.current.type == EventType.Layout && !Widgets.IsInNewScrollView()) || Event.current.type == EventType.MouseMove)
            {
                return;
            }
            Widgets.FontSizeScalable = 18;
            Dialogue orCreateDialogue = Get.DialoguesManager.GetOrCreateDialogue(dialogueSpec);
            List<DialogueEntry> history = orCreateDialogue.History;
            for (int i = 0; i < history.Count; i++)
            {
                DialogueDrawer.DoEntry(history[i].Speaker, history[i].Text, history[i].Text.Length, x, width, ref curY, false);
            }
            DialogueNode curNode = orCreateDialogue.CurNode;
            if (curNode != null)
            {
                int num = (int)letterShowingTimer;
                DialogueDrawer.DoEntry(curNode.Speaker, curNode.Text, num, x, width, ref curY, true);
                if (num >= curNode.Text.Length)
                {
                    if (curNode.HasResponses)
                    {
                        List<DialogueNode.Response> responses = curNode.Responses;
                        for (int j = 0; j < responses.Count; j++)
                        {
                            DialogueDrawer.DoResponseOption(responses[j], orCreateDialogue, j, x + 20f, width - 20f, ref curY, ref letterShowingTimer);
                        }
                    }
                    else
                    {
                        Rect rect = new Rect(x, curY, width, 20f);
                        if (orCreateDialogue.IsInLastNode)
                        {
                            GUI.color = new Color(0.6f, 0.7f, 1f);
                        }
                        else
                        {
                            GUI.color = new Color(0.65f, 0.65f, 0.65f);
                        }
                        Widgets.LabelCentered(rect, "PressSpaceToContinue".Translate().FormattedKeyBindings(), true, null, null);
                        GUI.color = Color.white;
                        curY += 20f;
                    }
                }
                if (Get.KeyBinding_Wait.JustPressed || Get.KeyBinding_Accept.JustPressed)
                {
                    if (num < curNode.Text.Length)
                    {
                        letterShowingTimer = (float)curNode.Text.Length;
                        Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    }
                    else if (!curNode.HasResponses)
                    {
                        if (orCreateDialogue.IsInLastNode)
                        {
                            Get.Sound_TelephoneHangUp.PlayOneShot(null, 1f, 1f);
                        }
                        else
                        {
                            Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                        }
                        orCreateDialogue.OnContinuedDialogue();
                        letterShowingTimer = 0f;
                    }
                }
            }
            Widgets.ResetFontSize();
            if (Event.current.type == EventType.Repaint)
            {
                letterShowingTimer += 100f * Clock.DeltaTime;
            }
        }

        private static void DoEntry(SpeakerSpec speaker, string text, int letters, float x, float width, ref float curY, bool current)
        {
            if (letters < text.Length)
            {
                DialogueDrawer.animatedText.Clear();
                for (int i = 0; i < letters; i++)
                {
                    DialogueDrawer.animatedText.Append(text[i]);
                }
                for (int j = letters; j < text.Length; j++)
                {
                    DialogueDrawer.animatedText.Append(' ');
                }
                DialogueDrawer.animatedText.Append("\u200b");
                text = DialogueDrawer.animatedText.ToString();
                if (Clock.Time - DialogueDrawer.lastSpeakSoundTime > 0.07f)
                {
                    speaker.Voice.PlayOneShot(null, 1f, 1f);
                    DialogueDrawer.lastSpeakSoundTime = Clock.Time;
                }
            }
            float num = x;
            DialogueDrawer.DoPortraitAndName(speaker, ref num, curY, !current);
            text = StringUtility.FillWidthWithSpaces(num - x - 45f) + "- " + text;
            float num2 = Widgets.CalcHeight(text, width - 45f);
            GUI.color = Color.white.Darker(current ? 0f : 0.21f);
            Widgets.Label(new Rect(x + 45f, curY, width - 45f, num2), text, true, null, null, false);
            GUI.color = Color.white;
            curY += num2 + 20f;
        }

        private static void DoPortraitAndName(SpeakerSpec speaker, ref float curX, float y, bool darker = false)
        {
            Rect rect = new Rect(curX, y, 24f, 24f);
            GUI.color = Color.white.Darker(darker ? 0.21f : 0f);
            GUI.DrawTexture(rect, speaker.Portrait);
            GUI.color = Color.white;
            GUI.color = speaker.Color.Darker(darker ? 0.21f : 0f);
            Widgets.FontBold = true;
            Rect rect2 = new Rect(rect.xMax + 10f, y, 999f, 50f);
            Widgets.Label(rect2, speaker.LabelAdjustedUppercase, true, null, null, false);
            curX = rect2.x + Widgets.CalcSize(speaker.LabelAdjustedUppercase).x + 10f;
            GUI.color = Color.white;
            Widgets.FontBold = false;
        }

        private static void DoResponseOption(DialogueNode.Response response, Dialogue dialogue, int index, float x, float width, ref float curY, ref float letterShowingTimer)
        {
            DialogueDrawer.<> c__DisplayClass10_0 CS$<> 8__locals1;
            CS$<> 8__locals1.index = index;
            CS$<> 8__locals1.response = response;
            Widgets.FontBold = true;
            float num = Widgets.CalcHeight(DialogueDrawer.< DoResponseOption > g__GetResponseText | 10_0(false, ref CS$<> 8__locals1), width);
            Rect rect = new Rect(x, curY, width, num);
            if (Mouse.Over(rect))
            {
                if (DialogueDrawer.lastResponseHover != CS$<> 8__locals1.index || DialogueDrawer.lastResponseHoverFrame < Clock.Frame - 1)
				{
                    Get.Sound_Hover.PlayOneShot(null, 1f, 1f);
                }
                DialogueDrawer.lastResponseHover = CS$<> 8__locals1.index;
                DialogueDrawer.lastResponseHoverFrame = Clock.Frame;
            }
            if (Widgets.ButtonInvisible(rect, false, false) || (Event.current.type == EventType.KeyDown && Event.current.keyCode == (CS$<> 8__locals1.index + 1).DigitToKeyCode()))
			{
                dialogue.OnResponseChosen(CS$<> 8__locals1.index);
                letterShowingTimer = 0f;
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
            }
            Widgets.Label(rect, DialogueDrawer.< DoResponseOption > g__GetResponseText | 10_0(Mouse.Over(rect), ref CS$<> 8__locals1), true, null, null, false);
            GUI.color = Color.white;
            Widgets.FontBold = false;
            curY += num;
        }

        public static string FormattedWithPlayerName(this string str)
        {
            if (str == null)
            {
                return str;
            }
            if (!str.Contains("["))
            {
                return str;
            }
            return DialogueDrawer.formattedWithPlayerNameCache.Get(str);
        }

        public static void OnPlayerNameChanged()
        {
            DialogueDrawer.formattedWithPlayerNameCache.Clear();
        }

        [CompilerGenerated]
        internal static string <DoResponseOption>g__GetResponseText|10_0(bool mouseover, ref DialogueDrawer.<>c__DisplayClass10_0 A_1)
		{
			return "{0}. {1}".Formatted((A_1.index + 1).ToStringCached(), mouseover? A_1.response.Text : RichText.CreateColorTag(A_1.response.Text, new Color(0.83f, 0.21f, 0.21f)));
		}

		private static StringBuilder animatedText = new StringBuilder();

        private static float lastSpeakSoundTime = -99999f;

        private static readonly CalculationCache<string, string> formattedWithPlayerNameCache = new CalculationCache<string, string>((string x) => x.Replace("[PlayerName]", Get.Progress.PlayerName), 30);

        private const float LetterShowingSpeed = 100f;

        private const float PortraitSize = 24f;

        private static int lastResponseHover = -1;

        private static int lastResponseHoverFrame = -1;
    }
}