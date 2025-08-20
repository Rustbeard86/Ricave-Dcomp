using System;
using System.Linq;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class LessonDrawerGOC : MonoBehaviour
    {
        private bool ShouldShowWatchTip
        {
            get
            {
                return !Get.Progress.EverUsedWatch && !Get.Player.UsedWatch && Get.NowControlledActor.Spawned && Get.Player.HasWatch && Get.Player.TurnsCanRewind >= 1 && (float)Get.NowControlledActor.HP / (float)Get.NowControlledActor.MaxHP < 0.5f && !Get.InLobby && !Get.WindowManager.AnyWindowOpen;
            }
        }

        private bool ShouldShowInspectTip
        {
            get
            {
                return !Get.Progress.EverInspectedAnythingInMainRun && !Get.Player.InspectedAnything && Get.Player.Playtime > 120.0 && Get.NowControlledActor.Spawned && !Get.InLobby && Get.RunSpec == Get.Run_Main1 && !Get.WindowManager.AnyWindowOpen;
            }
        }

        public void CheckEnableDisable()
        {
            bool flag = Get.LessonManager.CurrentLesson != null || this.ShouldShowWatchTip || this.ShouldShowInspectTip || (Get.Quest_Introduction.IsCompleted() && !Get.Quest_Introduction.IsCompletedAndClaimed());
            base.gameObject.SetActive(flag);
            if (!flag && this.staircaseArrow != null)
            {
                Object.Destroy(this.staircaseArrow);
                this.staircaseArrow = null;
            }
        }

        private void OnGUI()
        {
            Widgets.ApplySkin();
            GUI.depth = -50;
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (Get.TextSequenceDrawer.Showing)
            {
                return;
            }
            this.HandleArrow();
            LessonSpec currentLesson = Get.LessonManager.CurrentLesson;
            if (currentLesson != null)
            {
                float num = PlayerActorStatusControls.LevelLabelOffset.x + 47f;
                float num2 = PlayerActorStatusControls.LevelLabelOffset.y;
                float num3 = Clock.UnscaledTime - this.startTime;
                float num4 = Calc.ResolveFadeIn(num3, 0.28f);
                Rect rect = new Rect(num, num2 + 20f, 430f, 400f);
                string text;
                if (ControllerUtility.InControllerMode)
                {
                    if (ControllerUtility.ControllerType == ControllerType.SteamDeck && !currentLesson.TextSteamDeck.NullOrEmpty())
                    {
                        text = currentLesson.TextSteamDeck;
                    }
                    else if (!currentLesson.TextController.NullOrEmpty())
                    {
                        text = currentLesson.TextController;
                    }
                    else
                    {
                        text = currentLesson.Text;
                    }
                }
                else
                {
                    text = currentLesson.Text;
                }
                text = text.FormattedKeyBindings();
                Widgets.FontSizeScalable = 17;
                Rect rect2 = new Rect(rect.position, Widgets.CalcSize(text, rect.width));
                Widgets.ResetFontSize();
                if (currentLesson == Get.Lesson_Inventory || currentLesson == Get.Lesson_Tooltips)
                {
                    num -= 315f;
                    rect.x -= 315f;
                    rect2.x -= 315f;
                }
                float num9;
                if (currentLesson == Get.Lesson_Moving)
                {
                    float num5 = Widgets.ScreenCenter.x - rect2.x / 2f;
                    float num6 = Widgets.ScreenCenter.y - rect2.y / 2f;
                    float num7 = Calc.Lerp(num5, num, Clock.UnscaledTime - (this.startTime + 1.9f));
                    float num8 = Calc.Lerp(num6, num2, Clock.UnscaledTime - (this.startTime + 1.9f));
                    rect.x = num7;
                    rect.y = num8 + 20f;
                    rect2.x = num7;
                    rect2.y = num8 + 20f;
                    num = num7;
                    num2 = num8;
                    num9 = this.startTime + 4.5f;
                }
                else
                {
                    num9 = this.startTime + 2.5f;
                }
                string text2 = ((ControllerUtility.InControllerMode && !currentLesson.FlashingTextController.NullOrEmpty()) ? currentLesson.FlashingTextController : currentLesson.FlashingText);
                if (!text2.NullOrEmpty() && !Get.WindowManager.AnyWindowOpen)
                {
                    Widgets.FontSizeScalable = 52;
                    GUI.color = new Color(0.95f, 0.95f, 0.95f, (0.25f + (Calc.Sin(Clock.UnscaledTime * 4f) + 1f) / 2f * 0.75f) * Calc.ResolveFadeIn(Clock.UnscaledTime - num9, 0.3f));
                    Widgets.LabelCentered(Widgets.ScreenCenter.WithAddedY(-50f), text2.FormattedKeyBindings(), true, null, null, false, false, false, null);
                    GUI.color = Color.white;
                    Widgets.ResetFontSize();
                }
                string labelCap = currentLesson.LabelCap;
                Widgets.FontSizeScalable = 28;
                float x = Widgets.CalcSize(labelCap).x;
                Widgets.ResetFontSize();
                Rect rect3 = RectUtility.BoundingBox(rect2, new Rect(num, num2 - 17f, x, 1f), true).ExpandedBy(6f);
                GUIExtra.DrawRoundedRect(rect3, new Color(0.1f, 0.1f, 0.1f, 0.75f * num4), true, true, true, true, null);
                GUI.color = new Color(1f, 1f, 0.5f, num4);
                Widgets.FontSizeScalable = 28;
                Widgets.LabelCenteredV(new Vector2(num, num2), labelCap, true, null, null, false);
                GUI.color = Color.white;
                Widgets.ResetFontSize();
                GUI.color = new Color(1f, 1f, 1f, num4);
                Widgets.FontSizeScalable = 17;
                Widgets.Label(rect, text, true, null, null, false);
                Widgets.ResetFontSize();
                GUI.color = Color.white;
                float num10 = 1f - Calc.ResolveFadeIn(num3, 1f);
                if (num10 > 0f)
                {
                    GUIExtra.DrawRoundedRect(rect3, new Color(1f, 1f, 1f, num10 * 0.7f * num4), true, true, true, true, null);
                }
                if (currentLesson == Get.Lesson_ClimbingLadders)
                {
                    Entity entity = Get.World.GetEntitiesOfSpec(Get.Entity_Ladder).FirstOrDefault<Entity>();
                    if (entity != null)
                    {
                        Get.CellHighlighter.HighlightCell(entity.Position, new Color(1f, 1f, 0f));
                    }
                }
                return;
            }
            if (this.ShouldShowInspectTip)
            {
                Widgets.FontSizeScalable = 28;
                GUI.color = new Color(0.95f, 0.95f, 0.95f, (0.25f + Calc.Sin(Clock.UnscaledTime * 4f) + 1f) / 2f * 0.75f);
                Widgets.LabelCentered(Widgets.ScreenCenter.WithAddedY(-50f), "NeverInspectedAnythingTip".Translate().FormattedKeyBindings(), true, null, null, false, false, false, null);
                GUI.color = Color.white;
                Widgets.ResetFontSize();
                return;
            }
            if (this.ShouldShowWatchTip)
            {
                Widgets.FontSizeScalable = 52;
                GUI.color = new Color(0.95f, 0.95f, 0.95f, (0.25f + Calc.Sin(Clock.UnscaledTime * 4f) + 1f) / 2f * 0.75f);
                Widgets.LabelCentered(Widgets.ScreenCenter.WithAddedY(-50f), "NeverUsedWatchRewindTip".Translate().FormattedKeyBindings(), true, null, null, false, false, false, null);
                GUI.color = Color.white;
                Widgets.ResetFontSize();
            }
        }

        private void HandleArrow()
        {
            if (Get.InLobby)
            {
                float num = 0f;
                Entity entity;
                if (Get.Quest_Introduction.IsCompleted() && !Get.Quest_Introduction.IsCompletedAndClaimed())
                {
                    entity = Get.World.GetEntitiesOfSpec(Get.Entity_QuestLog).FirstOrDefault<Entity>();
                    num = 0.3f;
                }
                else
                {
                    entity = null;
                }
                if (entity != null)
                {
                    if (this.staircaseArrow == null)
                    {
                        if (LessonDrawerGOC.ArrowPrefab == null)
                        {
                            LessonDrawerGOC.ArrowPrefab = Assets.Get<GameObject>("Prefabs/Misc/TutorialArrow");
                        }
                        this.staircaseArrow = Object.Instantiate<GameObject>(LessonDrawerGOC.ArrowPrefab, entity.Position, Quaternion.identity, Get.RuntimeSpecialContainer.transform);
                        this.staircaseArrow.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    }
                    this.staircaseArrow.transform.position = entity.Position + new Vector3(0f, 0.26f + num + Calc.Pulse(2.5f, 0.18f), 0f);
                    this.staircaseArrow.transform.rotation = Quaternion.Euler(0f, Clock.Time * 150f, 0f);
                    return;
                }
                if (this.staircaseArrow != null)
                {
                    Object.Destroy(this.staircaseArrow);
                    this.staircaseArrow = null;
                }
            }
        }

        public void OnUIInit()
        {
            this.startTime = Clock.UnscaledTime;
        }

        public void OnLessonFinished()
        {
            this.startTime = Clock.UnscaledTime;
            Get.Sound_LessonCompleted.PlayOneShot(null, 1f, 1f);
        }

        public void OnTextSequenceDrawerFinished()
        {
            this.startTime = Clock.UnscaledTime;
        }

        private float startTime;

        private GameObject staircaseArrow;

        private const float FadeInTime = 0.28f;

        private const int FontSize = 17;

        private static GameObject ArrowPrefab;
    }
}