using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class RichText
    {
        public static string Label(Item item)
        {
            return RichText.Label(item);
        }

        public static string Label(Entity entity)
        {
            if (entity.IsPlayerParty)
            {
                return RichText.Bold(RichText.CreateColorTag(entity.LabelCap, "#5599ff"));
            }
            if (entity.Spec.SpecialRichTextColor != null)
            {
                return RichText.Bold(RichText.CreateColorTag(entity.LabelCap, entity.Spec.SpecialRichTextColor.Value));
            }
            Actor actor = entity as Actor;
            if (actor != null)
            {
                return RichText.Bold(RichText.CreateColorTag(entity.LabelCap, actor.HostilityColor.Lighter(0.2f)));
            }
            Item item = entity as Item;
            if (item != null)
            {
                if (!item.Identified)
                {
                    return RichText.Bold(RichText.Identification(item.LabelCap));
                }
                if (item.Cursed)
                {
                    return RichText.Bold(RichText.Cursed(item.LabelCap));
                }
                return RichText.Bold(RichText.CreateColorTag(item.LabelCap, RichText.ResolveColorForItem(item.Spec)));
            }
            else
            {
                if (entity is Structure && entity.Spec.Structure.IsSpecial)
                {
                    return RichText.Bold(RichText.CreateColorTag(entity.LabelCap, "#bb44ff"));
                }
                return RichText.Bold(RichText.CreateColorTag(entity.LabelCap, "#ffffff"));
            }
        }

        public static string Label(EntitySpec entitySpec, bool assumeIdentified = false)
        {
            string text = (assumeIdentified ? entitySpec.LabelCap : entitySpec.LabelAdjustedCap);
            if (entitySpec.SpecialRichTextColor != null)
            {
                return RichText.Bold(RichText.CreateColorTag(text, entitySpec.SpecialRichTextColor.Value));
            }
            if (entitySpec.IsActor)
            {
                if (Get.Player.Faction != null && entitySpec.Actor.DefaultFaction != null && (entitySpec.Actor.DefaultFaction.DefaultHostileTo.Contains(Get.Player.Faction.Spec) || Get.Player.Faction.Spec.DefaultHostileTo.Contains(entitySpec.Actor.DefaultFaction)))
                {
                    return RichText.Bold(RichText.CreateColorTag(text, Faction.DefaultHostileColor.Lighter(0.2f)));
                }
                if (Get.Player.Faction != null && entitySpec.Actor.DefaultFaction == Get.Player.Faction.Spec)
                {
                    return RichText.Bold(RichText.CreateColorTag(text, Faction.DefaultAllyColor.Lighter(0.2f)));
                }
                return RichText.Bold(RichText.CreateColorTag(text, Faction.DefaultNeutralColor.Lighter(0.2f)));
            }
            else if (entitySpec.IsItem)
            {
                if (Get.IdentificationGroups.GetIdentificationGroup(entitySpec) == null || assumeIdentified || Get.IdentificationGroups.IsIdentified(entitySpec))
                {
                    return RichText.Bold(RichText.CreateColorTag(text, RichText.ResolveColorForItem(entitySpec)));
                }
                return RichText.Bold(RichText.Identification(text));
            }
            else
            {
                if (entitySpec.IsStructure && entitySpec.Structure.IsSpecial)
                {
                    return RichText.Bold(RichText.CreateColorTag(text, "#bb44ff"));
                }
                return RichText.Bold(RichText.CreateColorTag(text, "#ffffff"));
            }
        }

        public static string Label(Faction faction)
        {
            return RichText.Bold(RichText.CreateColorTag(faction.LabelCap, faction.Color.Lighter(0.2f)));
        }

        public static string Label(IUsable usable)
        {
            Entity entity = usable as Entity;
            if (entity != null)
            {
                return RichText.Label(entity);
            }
            return RichText.Bold(RichText.CreateColorTag(usable.LabelCap, "#ffffff"));
        }

        public static string Label(Target target)
        {
            if (target.IsEntity)
            {
                return RichText.Label(target.Entity);
            }
            if (target.IsLocation)
            {
                return "Location".Translate();
            }
            return "";
        }

        public static string Label(ITipSubject tipSubject)
        {
            Entity entity = tipSubject as Entity;
            if (entity != null)
            {
                return RichText.Label(entity);
            }
            EntitySpec entitySpec = tipSubject as EntitySpec;
            if (entitySpec != null)
            {
                return RichText.Label(entitySpec, false);
            }
            return RichText.Bold(RichText.CreateColorTag(tipSubject.LabelCap, "#ffffff"));
        }

        public static string Label(BodyPart bodyPart)
        {
            return RichText.Bold(bodyPart.LabelCap);
        }

        public static string HP(int hp)
        {
            return RichText.CreateColorTag("HP".Translate(hp), "#ff0000");
        }

        public static string HP(string hp)
        {
            return RichText.CreateColorTag(hp, "#ff0000");
        }

        public static string Mana(int mana)
        {
            return RichText.CreateColorTag("Mana".Translate(mana), "#87bbff");
        }

        public static string Mana(string mana)
        {
            return RichText.CreateColorTag(mana, "#87bbff");
        }

        public static string Stamina(int stamina)
        {
            return RichText.CreateColorTag("Stamina".Translate(stamina), "#ffee33");
        }

        public static string Stamina(string stamina)
        {
            return RichText.CreateColorTag(stamina, "#ffee33");
        }

        public static string Identification(string text)
        {
            return RichText.CreateColorTag(text, "#8888ff");
        }

        public static string Identification(int number)
        {
            return RichText.CreateColorTag(number.ToStringCached(), "#8888ff");
        }

        public static string Cursed(string text)
        {
            return RichText.CreateColorTag(text, "#ff8888");
        }

        public static string Turns(string text)
        {
            return RichText.CreateColorTag(text, "#c0c0c0");
        }

        public static string Uses(string text)
        {
            return RichText.CreateColorTag(text, "#c0c0c0");
        }

        public static string Charges(string text)
        {
            return RichText.CreateColorTag(text, "#c0c0c0");
        }

        public static string Chance(string text)
        {
            return RichText.CreateColorTag(text, "#c0c0c0");
        }

        public static string AoERadius(string text)
        {
            return RichText.CreateColorTag(text, "#c0c0c0");
        }

        public static string Grayed(string text)
        {
            return RichText.CreateColorTag(text, "#c0c0c0");
        }

        public static string SubtleRed(string text)
        {
            return RichText.CreateColorTag(text, "#bdababff");
        }

        public static string Warning(string text)
        {
            return RichText.CreateColorTag(text, "#ffff80");
        }

        public static string Error(string text)
        {
            return RichText.CreateColorTag(text, "#ff8080");
        }

        public static string LogDate(string text)
        {
            return RichText.CreateColorTag(text, "#a3a3a3");
        }

        public static string Gold(string text)
        {
            return RichText.CreateColorTag(text, "#ffff7f");
        }

        public static string Yellow(string text)
        {
            return RichText.CreateColorTag(text, "#ffff7f");
        }

        public static string Diamond(string text)
        {
            return RichText.CreateColorTag(text, "#63ccff");
        }

        public static string Stardust(string text)
        {
            return RichText.CreateColorTag(text, "#63ccff");
        }

        public static string AI(string text)
        {
            return RichText.CreateColorTag(text, "#dbce64");
        }

        public static string Red(string text)
        {
            return RichText.CreateColorTag(text, "#ff3333");
        }

        public static string LightRed(string text)
        {
            return RichText.CreateColorTag(text, "#ff7777");
        }

        public static string LightGreen(string text)
        {
            return RichText.CreateColorTag(text, "#77ff77");
        }

        public static string Blue(string text)
        {
            return RichText.CreateColorTag(text, "#78a8ff");
        }

        public static string Miss(string text)
        {
            return RichText.CreateColorTag(text, "#ade5ff");
        }

        public static string Bold(string text)
        {
            return RichText.boldCache.Get(text);
        }

        public static string Italics(string text)
        {
            return RichText.italicsCache.Get(text);
        }

        public static string CreateColorTag(string inner, Color color)
        {
            return RichText.CreateColorTag(inner, "#".Concatenated(RichText.toHTMLStringCache.Get(color.RoundTo255())));
        }

        public static string CreateColorTag(string inner, string color)
        {
            return RichText.colorCache.Get(new ValueTuple<string, string>(inner, color));
        }

        public static string FontSize(string text, int fontSize)
        {
            return RichText.fontSizeCache.Get(new ValueTuple<string, int>(text, fontSize));
        }

        public static string StripColorTags(this string str)
        {
            if (str == null || !str.Contains("<color="))
            {
                return str;
            }
            return RichText.stripColorTagsCache.Get(str);
        }

        private static string ResolveColorForItem(EntitySpec itemSpec)
        {
            if (itemSpec.Item.IsGoodConsumable)
            {
                return "#33ff33";
            }
            if (itemSpec.Item.IsBadConsumable)
            {
                return "#ff3333";
            }
            return "#ffffff";
        }

        public static string MultiplyColorTagsByGUIColor(this string text)
        {
            if (text == null || GUI.color == Color.white)
            {
                return text;
            }
            if (!text.Contains("<color="))
            {
                return text;
            }
            return RichText.multiplyColorTagsByGUIColorCache.Get(new ValueTuple<string, Color>(text, GUI.color.RoundTo255()));
        }

        private static string MultiplyColorTagsByGUIColorImpl(string text, Color guiColor)
        {
            RichText.<> c__DisplayClass86_0 CS$<> 8__locals1;
            CS$<> 8__locals1.text = text;
            Profiler.Begin("MultiplyColorTagsByGUIColor");
            RichText.tmpSb.Length = 0;
            for (int i = 0; i < CS$<> 8__locals1.text.Length; i++)
            {
                if (RichText.< MultiplyColorTagsByGUIColorImpl > g__IsTagAt | 86_0(i, ref CS$<> 8__locals1))
                {
                    RichText.tmpSb.Append("<color=");
                    i += "<color=".Length;
                    RichText.tmpColor.Length = 0;
                    long num = 0L;
                    while (i < CS$<> 8__locals1.text.Length && CS$<> 8__locals1.text[i] != '>')
					{
                RichText.tmpColor.Append(CS$<> 8__locals1.text[i]);
                num = (num << 8) | (long)((ulong)CS$<> 8__locals1.text[i]);
                i++;
            }
            Color? color = RichText.parseHtmlStringCache.Get(num);
            if (color != null)
            {
                Color color2 = color.Value;
                color2 *= guiColor;
                RichText.tmpSb.Append("#");
                ColorUtility.AppendColorInHex(RichText.tmpSb, color2);
            }
            else
            {
                RichText.tmpSb.Append(RichText.tmpColor);
            }
            RichText.tmpSb.Append('>');
        }
				else
				{
					RichText.tmpSb.Append(CS$<>8__locals1.text[i]);
				}
}
string text2 = RichText.tmpSb.ToString();
Profiler.End();
return text2;
		}

		public static string TableRowR(string label, string value, int labelMaxLength)
{
    return "{0}: {1}".Formatted(RichText.Grayed(label), value);
}

public static string TableRowL(string label, string value, int labelMaxLength)
{
    return "{0}: {1}".Formatted(RichText.Grayed(label), value);
}

[CompilerGenerated]
internal static bool < MultiplyColorTagsByGUIColorImpl > g__IsTagAt | 86_0(int at, ref RichText.<> c__DisplayClass86_0 A_1)

        {
    return A_1.text[at] == '<' && A_1.text.IsSubstringAt("<color=", at);
}

private static readonly CalculationCache<string, string> boldCache = new CalculationCache<string, string>((string x) => "<b>" + x + "</b>", 300);

private static readonly CalculationCache<string, string> italicsCache = new CalculationCache<string, string>((string x) => "<i>" + x + "</i>", 300);

private static readonly CalculationCache<ValueTuple<string, string>, string> colorCache = new CalculationCache<ValueTuple<string, string>, string>((ValueTuple<string, string> x) => string.Concat(new string[] { "<color=", x.Item2, ">", x.Item1, "</color>" }), 300);

private static readonly CalculationCache<ValueTuple<string, int>, string> fontSizeCache = new CalculationCache<ValueTuple<string, int>, string>((ValueTuple<string, int> x) => string.Concat(new string[]
{
            "<size=",
            x.Item2.ToString(),
            ">",
            x.Item1,
            "</size>"
}), 300);

private static readonly CalculationCache<string, string> stripColorTagsCache = new CalculationCache<string, string>((string x) => RichText.ColorTagsRegex.Replace(x, "").Replace("</color>", ""), 300);

private static readonly CalculationCache<Color, string> toHTMLStringCache = new CalculationCache<Color, string>((Color x) => ColorUtility.ToHtmlStringRGBA(x), 50);

private static readonly CalculationCache<long, Color?> parseHtmlStringCache = new CalculationCache<long, Color?>(delegate (long x)
{
    Color color;
    if (!ColorUtility.TryParseHtmlString(RichText.tmpColor.ToString(), out color))
    {
        return null;
    }
    return new Color?(color);
}, 50);

private static readonly CalculationCache<ValueTuple<string, Color>, string> multiplyColorTagsByGUIColorCache = new CalculationCache<ValueTuple<string, Color>, string>((ValueTuple<string, Color> x) => RichText.MultiplyColorTagsByGUIColorImpl(x.Item1, x.Item2), 20);

private const string Label_Player = "#5599ff";

private const string Label_Item = "#ffffff";

private const string Label_ItemBad = "#ff3333";

private const string Label_ItemGood = "#33ff33";

private const string Label_SpecialStructure = "#bb44ff";

private const string Label_UnimportantEntity = "#ffffff";

private const string HPColor = "#ff0000";

private const string ManaColor = "#87bbff";

private const string StaminaColor = "#ffee33";

private const string IdentificationColor = "#8888ff";

private const string CursedColor = "#ff8888";

private const string GrayedColor = "#c0c0c0";

private const string SubtleRedColor = "#bdababff";

private const string TurnsColor = "#c0c0c0";

private const string UsesColor = "#c0c0c0";

private const string ChanceColor = "#c0c0c0";

private const string AoERadiusColor = "#c0c0c0";

private const string WarningColor = "#ffff80";

private const string ErrorColor = "#ff8080";

private const string LogDateColor = "#a3a3a3";

private const string GoldColor = "#ffff7f";

private const string YellowColor = "#ffff7f";

private const string DiamondColor = "#63ccff";

private const string StardustColor = "#63ccff";

private const string AIColor = "#dbce64";

private const string RedColor = "#ff3333";

private const string LightRedColor = "#ff7777";

private const string LightGreenColor = "#77ff77";

private const string BlueColor = "#78a8ff";

private const string MissColor = "#ade5ff";

private static readonly Regex ColorTagsRegex = new Regex("<color=(.*?)>");

private static StringBuilder tmpSb = new StringBuilder();

private static StringBuilder tmpColor = new StringBuilder();
	}
}