using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class HPBarOverlayDrawer
    {
        public bool ShouldHideDueToInspectMode
        {
            get
            {
                return !Get.UI.WantsMouseUnlocked && KeyCodeUtility.InspectHeldDown;
            }
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (this.ShouldHideDueToInspectMode)
            {
                return;
            }
            this.tmpEntitiesWithHPBar.Clear();
            List<Entity> entitiesSeen_Unordered = Get.VisibilityCache.EntitiesSeen_Unordered;
            int i = 0;
            int count = entitiesSeen_Unordered.Count;
            while (i < count)
            {
                Entity entity = entitiesSeen_Unordered[i];
                if (!entity.IsNowControlledActor && (entity is Actor || entity.HP != entity.MaxHP))
                {
                    this.tmpEntitiesWithHPBar.Add(entity);
                }
                i++;
            }
            if (this.tmpEntitiesWithHPBar.Count == 0)
            {
                return;
            }
            if (this.tmpEntitiesWithHPBar.Count >= 2)
            {
                this.tmpEntitiesWithHPBar.Sort(HPBarOverlayDrawer.ByDistToCamera);
            }
            for (int j = 0; j < this.tmpEntitiesWithHPBar.Count; j++)
            {
                this.DoBar(this.tmpEntitiesWithHPBar[j]);
            }
        }

        public int GetDrawnRowsCountFor(Entity entity)
        {
            if (this.ShouldHideDueToInspectMode)
            {
                return 0;
            }
            if (entity.IsNowControlledActor)
            {
                return 0;
            }
            Actor actor = entity as Actor;
            if (actor != null)
            {
                int num = 0;
                if (this.ShouldShowHPBar(actor))
                {
                    num++;
                }
                if (TipSubjectDrawer_Entity.ShouldShowManaBar(actor))
                {
                    num++;
                }
                if (TipSubjectDrawer_Entity.ShouldShowStaminaBar(actor))
                {
                    num++;
                }
                if (actor.ConditionsAccumulated.AllConditionDrawRequestsPlusExtra.Count != 0)
                {
                    num++;
                }
                return num;
            }
            if (entity.HP != entity.MaxHP)
            {
                return 1;
            }
            return 0;
        }

        private void DoBar(Entity entity)
        {
            Vector3 posAboveHead = HPBarOverlayDrawer.GetPosAboveHead(entity);
            Vector2 sizeInWorldUnits = HPBarOverlayDrawer.SizeInWorldUnits;
            Rect rect;
            if (!WorldRectToUIRectUtility.GetUIRect(posAboveHead, sizeInWorldUnits, out rect))
            {
                this.alpha.Remove(entity);
                return;
            }
            float num;
            if (!this.alpha.TryGetValue(entity, out num))
            {
                num = 0f;
            }
            float num2 = num;
            if (Widgets.ScreenRect.Contains(rect.center))
            {
                num += 4.5f * Clock.UnscaledDeltaTime;
            }
            else
            {
                num -= 4.5f * Clock.UnscaledDeltaTime;
            }
            num = Calc.Clamp01(num);
            this.alpha[entity] = num;
            if (num <= 0f)
            {
                this.alpha.Remove(entity);
                return;
            }
            float num3;
            if (!this.labelAlpha.TryGetValue(entity, out num3))
            {
                num3 = 0f;
            }
            if (rect.height < 16f)
            {
                num3 -= 0.5f * Clock.UnscaledDeltaTime;
            }
            else
            {
                num3 += 0.5f * Clock.UnscaledDeltaTime;
            }
            num3 = Calc.Clamp01(num3);
            this.labelAlpha[entity] = num3;
            this.DrawBar(entity, rect, num2, num3);
        }

        private void DrawBar(Entity entity, Rect barRect, float alphaResolved, float labelAlphaResolved)
        {
            if (alphaResolved <= 0f)
            {
                return;
            }
            Widgets.FontSize = Widgets.GetFontSizeToFitInHeight(barRect.height - 3f);
            Actor actor = entity as Actor;
            bool flag = actor == null || this.ShouldShowHPBar(actor);
            bool flag2 = actor != null && TipSubjectDrawer_Entity.ShouldShowManaBar(actor);
            bool flag3 = actor != null && TipSubjectDrawer_Entity.ShouldShowStaminaBar(actor);
            if (flag)
            {
                Get.ProgressBarDrawer.Draw(barRect, entity.HP, entity.MaxHP, TipSubjectDrawer_Entity.GetHPBarColor(entity), alphaResolved, labelAlphaResolved, true, true, new int?(entity.InstanceID), (actor == null) ? ProgressBarDrawer.ValueChangeDirection.Constant : TipSubjectDrawer_Entity.GetHPBarValueChangeDirection(actor), Get.InteractionManager.GetLostHPRangeForUI(entity), !flag2 && !flag3, !flag2 && !flag3, true, true, true, false, null);
                barRect.y -= barRect.height;
            }
            if (flag2)
            {
                ProgressBarDrawer progressBarDrawer = Get.ProgressBarDrawer;
                Rect rect = barRect;
                int mana = actor.Mana;
                int maxMana = actor.MaxMana;
                Color manaBarColor = TipSubjectDrawer_Entity.ManaBarColor;
                bool flag4 = true;
                bool flag5 = true;
                int? num = new int?(100000000 + entity.InstanceID);
                ProgressBarDrawer.ValueChangeDirection manaBarValueChangeDirection = TipSubjectDrawer_Entity.GetManaBarValueChangeDirection(actor);
                bool flag6 = !flag3;
                bool flag7 = !flag3;
                bool flag8 = !flag;
                bool flag9 = !flag;
                progressBarDrawer.Draw(rect, mana, maxMana, manaBarColor, alphaResolved, labelAlphaResolved, flag4, flag5, num, manaBarValueChangeDirection, null, flag6, flag7, flag8, flag9, true, false, null);
                barRect.y -= barRect.height;
            }
            if (flag3)
            {
                ProgressBarDrawer progressBarDrawer2 = Get.ProgressBarDrawer;
                Rect rect2 = barRect;
                int stamina = actor.Stamina;
                int maxStamina = actor.MaxStamina;
                Color staminaBarColor = TipSubjectDrawer_Entity.StaminaBarColor;
                bool flag10 = true;
                bool flag11 = true;
                int? num2 = new int?(200000000 + entity.InstanceID);
                ProgressBarDrawer.ValueChangeDirection staminaBarValueChangeDirection = TipSubjectDrawer_Entity.GetStaminaBarValueChangeDirection(actor);
                bool flag9 = !flag && !flag2;
                bool flag8 = !flag && !flag2;
                progressBarDrawer2.Draw(rect2, stamina, maxStamina, staminaBarColor, alphaResolved, labelAlphaResolved, flag10, flag11, num2, staminaBarValueChangeDirection, null, true, true, flag9, flag8, true, false, null);
                barRect.y -= barRect.height;
            }
            barRect.y += barRect.height;
            Widgets.ResetFontSize();
            if (actor != null)
            {
                this.DoConditionIcons(actor, barRect.height, alphaResolved, barRect.x, barRect.y);
            }
        }

        private bool ShouldShowHPBar(Actor actor)
        {
            return !actor.IsBoss;
        }

        private void DoConditionIcons(Actor actor, float hpBarHeight, float alpha, float startX, float startBotY)
        {
            float num = hpBarHeight * 1.5f;
            float num2 = startX;
            int num3 = 0;
            foreach (ConditionDrawRequest conditionDrawRequest in actor.ConditionsAccumulated.AllConditionDrawRequestsPlusExtra)
            {
                Rect rect = new Rect(num2, startBotY - num, num, num);
                ExpandingIconAnimation.Do(rect, conditionDrawRequest.Icon, conditionDrawRequest.IconColor, conditionDrawRequest.TimeStartedAffectingActor, 1f, 0.6f, 0.55f);
                GUI.color = conditionDrawRequest.IconColor.WithAlphaFactor(alpha);
                GUI.DrawTexture(rect.ContractedBy(rect.width * 0.05f), conditionDrawRequest.Icon);
                GUI.color = Color.white;
                if (Mouse.Over(rect))
                {
                    GUIExtra.DrawHighlight(rect, true, true, true, true, 1f);
                    Get.Tooltips.RegisterTip(rect, conditionDrawRequest, null, new int?(Calc.CombineHashes<int, int, int>(actor.MyStableHash, num3, 91209812)));
                }
                num2 += num;
                num3++;
            }
        }

        public static Vector3 GetPosAboveHead(Entity entity)
        {
            if (entity.EntityGOC == null)
            {
                return entity.RenderPosition;
            }
            float num = ((entity is Item) ? 0f : 0.1f);
            return entity.RenderPosition.WithY(entity.EntityGOC.BoundingBox.max.y + num);
        }

        private Dictionary<Entity, float> alpha = new Dictionary<Entity, float>();

        private Dictionary<Entity, float> labelAlpha = new Dictionary<Entity, float>();

        private List<Entity> tmpEntitiesWithHPBar = new List<Entity>();

        private const float FadeInOutSpeed = 4.5f;

        private const float LabelFadeInOutSpeed = 0.5f;

        private const float MinBarHeightToShowLabel = 16f;

        public static readonly Vector2 SizeInWorldUnits = new Vector2(0.4f, 0.05f);

        private const float ConditionIconSizeFactor = 1.5f;

        private static readonly Comparison<Entity> ByDistToCamera = (Entity a, Entity b) => (b.RenderPosition - Get.CameraPosition).sqrMagnitude.CompareTo((a.RenderPosition - Get.CameraPosition).sqrMagnitude);
    }
}