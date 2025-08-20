using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class CrosshairDrawer
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (Get.UI.InventoryOpen)
            {
                return;
            }
            if (Get.UI.IsWheelSelectorOpen)
            {
                return;
            }
            if (Get.KeyBinding_Minimap.HeldDown || Get.KeyBinding_MinimapAlt.HeldDown)
            {
                return;
            }
            if (Get.NowControlledActor == null || !Get.NowControlledActor.Spawned)
            {
                return;
            }
            if (Get.NowControlledActor.Spawned && !Get.TurnManager.IsPlayerTurn_CanChooseNextAction && Get.VisibilityCache.AnyNonNowControlledActorSeen)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.55f);
            }
            float num = 20f;
            GUIExtra.DrawTextureRotated(new Rect(Calc.Round((Widgets.VirtualWidth - num) / 2f), Calc.Round((Widgets.VirtualHeight - num) / 2f), num, num), CrosshairDrawer.CrosshairTex, this.angle.CurrentValue, null);
            GUI.color = Color.white;
            if (Get.TimeDrawer.ResolvingTurnsIconAlpha > 0f)
            {
                TimeDrawer.DrawResolvingTurnsIcon(new Rect(Widgets.VirtualWidth / 2f - 43f, (Widgets.VirtualHeight - 28f) / 2f, 28f, 28f), Get.TimeDrawer.ResolvingTurnsIconAlpha);
            }
            if (Get.NowControlledActor.ChargedAttack != 0)
            {
                float num2 = ((Get.NowControlledActor.ChargedAttack == 1) ? 1.2f : 1.4f);
                string text = "x{0}".Formatted(num2.ToStringCached());
                float x = Widgets.CalcSize(text).x;
                float num3 = 28f + x;
                Rect rect = new Rect((Widgets.VirtualWidth - num3) / 2f, Widgets.VirtualHeight / 2f + 10f + 6f, 24f, 24f);
                float num4 = ((Get.NowControlledActor.ChargedAttack == 1) ? Calc.ResolveFadeIn(Clock.UnscaledTime - Get.Player.LastChargedAttackChangeTime, 0.09f) : 1f);
                float num5 = 1f + Calc.ResolveFadeInStayOut(Clock.UnscaledTime - Get.Player.LastChargedAttackChangeTime, 0.1f, 0f, 0.1f) * 0.089f;
                CachedGUI.SetDirty(12);
                CachedGUI.BeginCachedGUI(new Rect(rect.x, rect.y, num3, rect.height).ExpandedBy(2f), 12, true);
                GUI.color = new Color(0.9f, 0.9f, 0.9f);
                GUIExtra.DrawTexture(rect, Get.UseEffect_Damage.Icon);
                Widgets.LabelCenteredV(rect.RightCenter().MovedBy(4f, 0f), text, true, null, null, false);
                GUI.color = Color.white;
                CachedGUI.EndCachedGUI(num4, num5);
            }
        }

        public void FixedUpdate()
        {
            float num = (float)(Get.NowControlledActor.Sequence - Get.TurnManager.InceptionSequence) / 12f * 90f - 90f;
            if (!this.angleEverSet)
            {
                this.angleEverSet = true;
                this.angle = new InterpolatedFloat(num);
                return;
            }
            this.angle.SetTarget(Calc.Lerp(this.angle.Target, num, 0.16f));
        }

        public void Reset()
        {
            this.angleEverSet = false;
        }

        private InterpolatedFloat angle;

        private bool angleEverSet;

        private static readonly Texture2D CrosshairTex = Assets.Get<Texture2D>("Textures/UI/Crosshair");

        private const int CrosshairSize = 20;

        private const int ResolvingTurnsIconSize = 28;
    }
}