using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class SkillCategoryUtility
    {
        public static Color GetColor(this SkillCategory skillCategory)
        {
            switch (skillCategory)
            {
                case SkillCategory.Defense:
                    return new Color(0.25f, 0.59f, 0.86f);
                case SkillCategory.Attack:
                    return new Color(1f, 0.36f, 0.34f);
                case SkillCategory.Misc:
                    return new Color(0.8f, 0.81f, 0.55f);
                default:
                    return Color.white;
            }
        }

        public static string GetLabel(this SkillCategory skillCategory)
        {
            switch (skillCategory)
            {
                case SkillCategory.Defense:
                    return "SkillCategory_Defense".Translate();
                case SkillCategory.Attack:
                    return "SkillCategory_Attack".Translate();
                case SkillCategory.Misc:
                    return "SkillCategory_Misc".Translate();
                default:
                    return "";
            }
        }

        public static string GetLabelCap(this SkillCategory skillCategory)
        {
            return skillCategory.GetLabel().CapitalizeFirst();
        }
    }
}