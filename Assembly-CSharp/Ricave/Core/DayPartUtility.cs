using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class DayPartUtility
    {
        public static Texture2D GetIcon(this DayPart dayPart)
        {
            switch (dayPart)
            {
                case DayPart.Morning:
                    return DayPartUtility.MorningIcon;
                case DayPart.Day:
                    return DayPartUtility.DayIcon;
                case DayPart.Evening:
                    return DayPartUtility.EveningIcon;
                case DayPart.Night:
                    return DayPartUtility.NightIcon;
                default:
                    return null;
            }
        }

        public static string GetLabel(this DayPart dayPart)
        {
            switch (dayPart)
            {
                case DayPart.Morning:
                    return "DayPart_Morning".Translate();
                case DayPart.Day:
                    return "DayPart_Day".Translate();
                case DayPart.Evening:
                    return "DayPart_Evening".Translate();
                case DayPart.Night:
                    return "DayPart_Night".Translate();
                default:
                    return "";
            }
        }

        public static string GetLabelCap(this DayPart dayPart)
        {
            return dayPart.GetLabel().CapitalizeFirst();
        }

        private static readonly Texture2D MorningIcon = Assets.Get<Texture2D>("Textures/UI/DayPart/Morning");

        private static readonly Texture2D DayIcon = Assets.Get<Texture2D>("Textures/UI/DayPart/Day");

        private static readonly Texture2D EveningIcon = Assets.Get<Texture2D>("Textures/UI/DayPart/Evening");

        private static readonly Texture2D NightIcon = Assets.Get<Texture2D>("Textures/UI/DayPart/Night");
    }
}