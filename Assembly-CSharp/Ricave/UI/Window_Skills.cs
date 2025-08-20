using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Skills : Window
    {
        private float CenteringOffset
        {
            get
            {
                return (base.Spec.Size.x - 40f - 738f + 47f) / 2f;
            }
        }

        public Window_Skills(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Widgets.Label(inRect, RichText.TableRowL("AvailableSkillPoints".Translate(), RichText.Bold(Get.Player.SkillPoints.ToStringCached()), 23), true, null, null, false);
            Widgets.Label(inRect.CutFromTop(20f), RichText.TableRowL("UnlockedSkills".Translate(), RichText.Bold("{0}/{1}".Formatted(Get.SkillManager.UnlockedSkills.Count.ToStringCached(), Get.Specs.GetAll<SkillSpec>().Count.ToStringCached())), 23), true, null, null, false);
            float num = 246f;
            Widgets.SectionBackground(new Rect(inRect.center.x - num - 115f, 50f, 230f, 580f));
            Widgets.SectionBackground(new Rect(inRect.center.x - 115f, 50f, 230f, 580f));
            Widgets.SectionBackground(new Rect(inRect.center.x + num - 115f, 50f, 230f, 580f));
            foreach (SkillSpec skillSpec in Get.Specs.GetAll<SkillSpec>())
            {
                foreach (SkillSpec skillSpec2 in skillSpec.Prerequisites)
                {
                    float num2 = Math.Min(Window_Skills.GetColorFactor(skillSpec), Window_Skills.GetColorFactor(skillSpec2));
                    Vector2 center = this.GetIconRect(skillSpec).center;
                    Vector2 center2 = this.GetIconRect(skillSpec2).center;
                    GUIExtra.DrawLine(center, center2, skillSpec.Category.GetColor().MultipliedColor(num2), 4f);
                }
            }
            using (List<SkillSpec>.Enumerator enumerator = Get.Specs.GetAll<SkillSpec>().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    SkillSpec skill = enumerator.Current;
                    Rect iconRect = this.GetIconRect(skill);
                    Window_Skills.DoSkill(iconRect, skill);
                    if (Mouse.OverCircle(iconRect.center, iconRect.width / 2f) && Widgets.ButtonInvisible(iconRect, true, false) && Window_Skills.CanUnlockSkill(skill))
                    {
                        ActionViaInterfaceHelper.TryDo(() => new Action_UnlockSkill(Get.Action_UnlockSkill, skill));
                    }
                }
            }
            foreach (SkillSpec skillSpec3 in Get.Specs.GetAll<SkillSpec>())
            {
                ExpandingIconAnimation.Do(this.GetIconRect(skillSpec3), skillSpec3.Icon, skillSpec3.IconColor, Get.SkillManager.GetTimeUnlocked(skillSpec3), 1f, 0.6f, 0.55f);
            }
            GUI.color = new Color(0.55f, 0.55f, 0.55f);
            Widgets.LabelCentered(inRect.BottomPart(72f).center.MovedBy(-num, 0f), SkillCategory.Defense.GetLabelCap(), true, null, null, false, false, false, null);
            Widgets.LabelCentered(inRect.BottomPart(72f).center, SkillCategory.Attack.GetLabelCap(), true, null, null, false, false, false, null);
            Widgets.LabelCentered(inRect.BottomPart(72f).center.MovedBy(num, 0f), SkillCategory.Misc.GetLabelCap(), true, null, null, false, false, false, null);
            GUI.color = Color.white;
        }

        public static void DoSkill(Rect rect, SkillSpec skill)
        {
            float colorFactor = Window_Skills.GetColorFactor(skill);
            float num;
            if (Mouse.OverCircle(rect.center, rect.width / 2f))
            {
                Get.Tooltips.RegisterTip(rect, skill, null, null);
                num = Widgets.AccumulatedHover(rect, false);
            }
            else
            {
                num = Widgets.AccumulatedHover(rect, true);
            }
            GUIExtra.DrawCircle(rect.center.MovedBy(0f, 3f), rect.width / 2f - 1f, new Color(0f, 0f, 0f, 0.8f));
            GUIExtra.DrawCircle(rect.center, rect.width / 2f, skill.Category.GetColor().MultipliedColor(colorFactor).Lighter(num * 0.1f));
            if (!Get.SkillManager.IsUnlocked(skill))
            {
                GUIExtra.DrawCircle(rect.center, rect.width / 2f * 0.9f, new Color(0.5f, 0.5f, 0.5f).MultipliedColor(colorFactor).Lighter(num * 0.1f));
            }
            GUI.color = skill.IconColor.MultipliedColor(colorFactor);
            GUI.DrawTexture(rect.ContractedByPct(0.15f), skill.Icon);
            GUI.color = Color.white;
        }

        private Rect GetIconRect(SkillSpec skill)
        {
            Rect rect = RectUtility.CenteredAt(skill.Position * 123f, 76f);
            rect.position += new Vector2(38f, 38f);
            rect.position += new Vector2(this.CenteringOffset, 0f);
            rect.position += new Vector2(0f, 68f);
            return rect;
        }

        private static float GetColorFactor(SkillSpec skill)
        {
            if (Get.SkillManager.IsUnlocked(skill))
            {
                return 1f;
            }
            if (Window_Skills.CanUnlockSkill(skill))
            {
                return 0.65f + Calc.PulseUnscaled(2f, 0.3f);
            }
            return 0.5f;
        }

        private static bool CanUnlockSkill(SkillSpec skill)
        {
            return Get.MainActor.Spawned && !Get.SkillManager.IsUnlocked(skill) && Get.Player.SkillPoints > 0 && skill.PrerequisiteUnlocked;
        }

        private const float SkillIconSize = 76f;

        private const float GapBetweenSkills = 47f;

        private const int AssumeSkillCountWidth = 6;
    }
}