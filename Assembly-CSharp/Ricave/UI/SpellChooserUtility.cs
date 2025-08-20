using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class SpellChooserUtility
    {
        public static int AvailableSpellCountAtCurrentLevel
        {
            get
            {
                return Math.Min((Get.Player.Level - 1) / 1, 7);
            }
        }

        public static int NonAbilityLikeSpellCount
        {
            get
            {
                Actor mainActor = Get.MainActor;
                if (((mainActor != null) ? mainActor.Spells : null) == null)
                {
                    return 0;
                }
                List<Spell> all = Get.MainActor.Spells.All;
                int num = 0;
                for (int i = 0; i < all.Count; i++)
                {
                    if (!all[i].Spec.IsAbilityLike)
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        public static List<SpellSpec> ChoicesToShow
        {
            get
            {
                SpellChooserUtility.tmpChoicesToShow.Clear();
                if (Get.NowControlledActor != Get.MainActor)
                {
                    return SpellChooserUtility.tmpChoicesToShow;
                }
                if (Get.MainActor.Spells.Count >= 7)
                {
                    return SpellChooserUtility.tmpChoicesToShow;
                }
                if (SpellChooserUtility.NonAbilityLikeSpellCount >= SpellChooserUtility.AvailableSpellCountAtCurrentLevel)
                {
                    return SpellChooserUtility.tmpChoicesToShow;
                }
                Rand.PushState(Calc.CombineHashes<int, int, int>(Get.RunSeed, SpellChooserUtility.NonAbilityLikeSpellCount, 802143658));
                try
                {
                    SpellChooserUtility.tmpPossibilities.Clear();
                    foreach (SpellSpec spellSpec in Get.Specs.GetAll<SpellSpec>())
                    {
                        if (spellSpec.CanAppearAsChoiceForPlayer && !Get.MainActor.Spells.AnyOfSpec(spellSpec))
                        {
                            SpellChooserUtility.tmpPossibilities.Add(spellSpec);
                        }
                    }
                    SpellSpec spellSpec2;
                    if (SpellChooserUtility.tmpPossibilities.TryGetRandomElement<SpellSpec>(out spellSpec2))
                    {
                        SpellChooserUtility.tmpChoicesToShow.Add(spellSpec2);
                        SpellChooserUtility.tmpPossibilities.Remove(spellSpec2);
                        SpellSpec spellSpec3;
                        if (SpellChooserUtility.tmpPossibilities.TryGetRandomElement<SpellSpec>(out spellSpec3))
                        {
                            SpellChooserUtility.tmpChoicesToShow.Add(spellSpec3);
                        }
                    }
                }
                finally
                {
                    Rand.PopState();
                }
                return SpellChooserUtility.tmpChoicesToShow;
            }
        }

        public static void DoChooseSpellButton(Rect rect)
        {
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            GUI.DrawTexture(rect.ContractedBy(18f), SpellChooserUtility.AddSpellIcon);
            if (Mouse.Over(rect))
            {
                Get.Tooltips.RegisterTip(rect, "AddSpellButtonTip".Translate(1), null);
            }
            bool flag = Get.WheelSelector != null && Get.WheelSelector.IsSpellChooser;
            if (flag)
            {
                ItemSlotDrawer.DrawEquippedOutline(rect, false, false);
            }
            else
            {
                GUIExtra.DrawHighlight(rect, true, true, true, true, Calc.PulseUnscaled(2f, 0.8f));
            }
            if (Widgets.ButtonInvisible(rect, false, false))
            {
                if (flag)
                {
                    Get.UI.CloseWheelSelector(true);
                    return;
                }
                SpellChooserUtility.TryShowWheelSelectorWithChoices();
            }
        }

        public static void TryShowWheelSelectorWithChoices()
        {
            List<SpellSpec> choicesToShow = SpellChooserUtility.ChoicesToShow;
            if (choicesToShow.Count == 0)
            {
                return;
            }
            List<WheelSelector.Option> list = new List<WheelSelector.Option>();
            foreach (SpellSpec spellSpec in choicesToShow)
            {
                SpellSpec choiceLocal = spellSpec;
                Func<Action> <> 9__1;
                list.Add(new WheelSelector.Option(spellSpec.LabelCap, delegate
                {
                    Get.Sound_ChooseSpell.PlayOneShot(null, 1f, 1f);
                    Func<Action> func;
                    if ((func = <> 9__1) == null)
                    {
                        func = (<> 9__1 = () => new Action_ChooseSpell(Get.Action_ChooseSpell, choiceLocal));
                    }
                    ActionViaInterfaceHelper.TryDo(func);
                }, null, spellSpec));
            }
            Get.UI.OpenWheelSelector(list, null, "ChooseSpellWheelSelectorText".Translate(1), true, false, false);
        }

        private static List<SpellSpec> tmpChoicesToShow = new List<SpellSpec>();

        private static List<SpellSpec> tmpPossibilities = new List<SpellSpec>();

        public const int SpellEveryLevels = 1;

        private static readonly Texture2D AddSpellIcon = Assets.Get<Texture2D>("Textures/UI/AddSpell");
    }
}