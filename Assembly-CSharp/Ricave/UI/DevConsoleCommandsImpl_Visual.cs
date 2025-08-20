using System;
using Ricave.Core;
using Ricave.Rendering;

namespace Ricave.UI
{
    public static class DevConsoleCommandsImpl_Visual
    {
        public static void Shake(string[] args)
        {
            float num = float.Parse(args[0]);
            Get.CameraEffects.Shake(num);
        }

        public static void Wobble(string[] args)
        {
            float num = float.Parse(args[0]);
            Get.CameraEffects.Wobble(num);
        }

        public static void Vignette(string[] args)
        {
            float num = float.Parse(args[0]);
            Get.CameraEffects.DoVignette(num);
        }

        public static void Strike(string[] args)
        {
            Get.StrikeOverlays.Strike(false);
        }

        public static void Fall(string[] args)
        {
            Get.CameraEffects.DoFallEffect();
        }

        public static void Effect(string[] args)
        {
            string text = args[0];
            VisualEffectSpec visualEffectSpec = Get.Specs.Get<VisualEffectSpec>(text);
            if (visualEffectSpec == null)
            {
                return;
            }
            Get.VisualEffectsManager.DoOneShot(visualEffectSpec, Get.NowControlledActor.Position);
        }

        public static void MakeCubemap(string[] args)
        {
            CubemapMaker.MakeCubemap(null);
        }
    }
}