using System;
using System.Collections.Generic;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class ScreenFaderGOC : MonoBehaviour
    {
        public bool AnyActionQueued
        {
            get
            {
                return this.toExecute != null;
            }
        }

        public float Alpha
        {
            get
            {
                return this.alpha;
            }
        }

        public bool ShowYouDiedText
        {
            get
            {
                return this.showYouDiedText;
            }
        }

        private void OnGUI()
        {
            Widgets.ApplySkin();
            GUI.depth = -300;
            if (this.toExecute != null)
            {
                if (this.alpha >= 1f)
                {
                    try
                    {
                        this.toExecute();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error while executing ScreenFader's queued action.", ex);
                    }
                    this.toExecute = null;
                }
                else
                {
                    this.alpha += Clock.UnscaledDeltaTime * ((this.fadeOutSeconds != null) ? (1f / this.fadeOutSeconds.Value) : 2f);
                    this.alpha = Math.Min(this.alpha, 1f);
                }
            }
            else if (this.alpha > 0f)
            {
                if (Get.InMainMenu || ((Get.Run.UnloadUnusedAssetsAsyncOperation == null || Get.Run.UnloadUnusedAssetsAsyncOperation.isDone) && Get.AssetsPrewarmer.Finished && Clock.Frame >= Get.AssetsPrewarmer.FinishedFrame + 3))
                {
                    this.alpha -= Clock.DeltaTime * 0.45f;
                    this.alpha = Math.Max(this.alpha, 0f);
                }
            }
            else
            {
                base.gameObject.SetActive(false);
            }
            if (this.alpha > 0f)
            {
                if (!Get.InMainMenu)
                {
                    this.DoBlinkEffect();
                }
                GUIExtra.DrawRect(Widgets.ScreenRect.ExpandedBy(1f), new Color(0f, 0f, 0f, this.alpha));
                if (this.showLoadingText && this.alpha > 0.85f)
                {
                    GUI.color = new Color(1f, 1f, 1f, Math.Min((this.alpha - 0.85f) / 0.1f, 1f));
                    Widgets.LabelCentered(Widgets.ScreenCenter, "Loading".Translate(), true, null, null, false, false, false, null);
                    GUI.color = Color.white;
                }
            }
        }

        private void Update()
        {
            if (this.AnyActionQueued && this.fadeOutAllEntities && !Get.InMainMenu)
            {
                List<Entity> entitiesSeen_Unordered = Get.VisibilityCache.EntitiesSeen_Unordered;
                if (entitiesSeen_Unordered.Count != 0)
                {
                    using (List<Entity>.Enumerator enumerator = entitiesSeen_Unordered.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Entity entity = enumerator.Current;
                            if ((entity.Position - Get.CameraPosition).sqrMagnitude < 36f)
                            {
                                entity.CheckFadeOutOnScreenFadeOut();
                            }
                        }
                        return;
                    }
                }
                foreach (Vector3Int vector3Int in Get.NowControlledActor.Position.GetCellsWithin(6).ClipToWorld())
                {
                    foreach (Entity entity2 in Get.World.GetEntitiesAt(vector3Int))
                    {
                        entity2.CheckFadeOutOnScreenFadeOut();
                    }
                }
            }
        }

        public void FadeOutAndExecute(Action toExecute, float? fadeOutSeconds = null, bool fadeOutAllEntities = false, bool showLoadingText = true, bool showYouDiedText = false)
        {
            if (this.toExecute != null)
            {
                Log.Error("Called FadeOutAndExecute() but we're already fading out.", false);
                return;
            }
            if (toExecute == null)
            {
                Log.Error("Called FadeOutAndExecute() with null toExecute action.", false);
                return;
            }
            this.toExecute = toExecute;
            this.fadeOutSeconds = fadeOutSeconds;
            this.fadeOutAllEntities = fadeOutAllEntities;
            this.showLoadingText = showLoadingText;
            this.showYouDiedText = showYouDiedText;
            base.gameObject.SetActive(true);
        }

        public void FadeFromBlack()
        {
            this.alpha = 1f;
            this.showLoadingText = false;
            this.showYouDiedText = false;
            base.gameObject.SetActive(true);
        }

        public bool ShouldFadeOut(Entity entity)
        {
            return false;
        }

        private void DoBlinkEffect()
        {
            if (ScreenFaderGOC.gaussGradient == null)
            {
                ScreenFaderGOC.gaussGradient = Assets.Get<Texture2D>("Textures/UI/CurvedGaussGradient");
            }
            float num = Widgets.VirtualHeight * 0.45f;
            float num2 = Calc.Lerp(-num, Widgets.VirtualHeight / 2f, Calc.Pow(this.alpha, 1.6f));
            if (num2 + 2f > 0f)
            {
                GUIExtra.DrawRect(new Rect(-1f, -1f, Widgets.VirtualWidth + 2f, num2 + 2f), Color.black);
            }
            GUI.DrawTexture(new Rect(-1f, num2, Widgets.VirtualWidth + 2f, num), ScreenFaderGOC.gaussGradient);
            float num3 = Widgets.VirtualHeight - num2 - num;
            if (Widgets.VirtualHeight - (num3 + num) + 2f > 0f)
            {
                GUIExtra.DrawRect(new Rect(-1f, num3 + num - 1f, Widgets.VirtualWidth + 2f, Widgets.VirtualHeight - (num3 + num) + 2f), Color.black);
            }
            GUIExtra.DrawTextureYFlipped(new Rect(-1f, num3, Widgets.VirtualWidth + 2f, num), ScreenFaderGOC.gaussGradient);
        }

        private float alpha = 1f;

        private Action toExecute;

        private float? fadeOutSeconds;

        private bool fadeOutAllEntities;

        private bool showLoadingText = true;

        private bool showYouDiedText;

        public const float FadeOutSpeed = 2f;

        private const float FadeInSpeed = 0.45f;

        private static Texture2D gaussGradient;
    }
}