using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public static class DungeonModifiersUtility
    {
        public static int ActiveCount
        {
            get
            {
                List<DungeonModifierSpec> dungeonModifiers = Get.RunConfig.DungeonModifiers;
                if (dungeonModifiers == null)
                {
                    return 0;
                }
                int num = 0;
                for (int i = 0; i < dungeonModifiers.Count; i++)
                {
                    if (DungeonModifiersUtility.AppliesToCurrentRun(dungeonModifiers[i]))
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        public static void GetStardustAndScoreFactors(List<DungeonModifierSpec> selectedModifiers, out float stardustFactor, out float scoreFactor, bool filterOutNotApply = false)
        {
            stardustFactor = 1f;
            scoreFactor = 1f;
            if (selectedModifiers == null)
            {
                return;
            }
            foreach (DungeonModifierSpec dungeonModifierSpec in selectedModifiers)
            {
                if (!filterOutNotApply || DungeonModifiersUtility.AppliesToCurrentRun(dungeonModifierSpec))
                {
                    if (dungeonModifierSpec.Category == DungeonModifierCategory.Bad)
                    {
                        stardustFactor *= 1.15f;
                        scoreFactor *= 1.15f;
                    }
                    else if (dungeonModifierSpec.Category == DungeonModifierCategory.Good)
                    {
                        stardustFactor /= 1.15f;
                        scoreFactor /= 1.15f;
                    }
                }
            }
            if (Calc.Approximately(stardustFactor, 1f))
            {
                stardustFactor = 1f;
            }
            if (Calc.Approximately(scoreFactor, 1f))
            {
                scoreFactor = 1f;
            }
        }

        public static float GetCurrentRunStardustFactor()
        {
            float num;
            float num2;
            DungeonModifiersUtility.GetStardustAndScoreFactors(Get.RunConfig.DungeonModifiers, out num, out num2, true);
            return num;
        }

        public static float GetCurrentRunScoreFactor()
        {
            float num;
            float num2;
            DungeonModifiersUtility.GetStardustAndScoreFactors(Get.RunConfig.DungeonModifiers, out num, out num2, true);
            return num2;
        }

        public static bool AppliesToCurrentRun(DungeonModifierSpec modifier)
        {
            return (modifier != Get.DungeonModifier_NoRandomEvents || Get.RunSpec.HasRandomEvents) && (modifier != Get.DungeonModifier_NoMiniBosses || Get.RunSpec.HasMiniBosses) && (modifier != Get.DungeonModifier_ExtraMiniBoss || Get.RunSpec.HasMiniBosses) && (modifier != Get.DungeonModifier_NoShelters || Get.RunSpec.HasShelters);
        }

        public static bool IsActiveAndAppliesToCurrentRun(this DungeonModifierSpec modifier)
        {
            List<DungeonModifierSpec> dungeonModifiers = Get.RunConfig.DungeonModifiers;
            return dungeonModifiers != null && dungeonModifiers.Contains(modifier) && DungeonModifiersUtility.AppliesToCurrentRun(modifier);
        }

        private const float FactorPerModifier = 1.15f;
    }
}