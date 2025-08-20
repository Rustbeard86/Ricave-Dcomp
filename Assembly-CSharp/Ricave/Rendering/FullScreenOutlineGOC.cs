using System;
using Ricave.Core;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Ricave.Rendering
{
    public class FullScreenOutlineGOC : MonoBehaviour
    {
        private void Start()
        {
            if (FullScreenOutlineGOC.cheap == null)
            {
                FullScreenOutlineGOC.cheap = Assets.Get<Material>("Materials/Misc/FullScreenOutlinePostProcCheap");
            }
            if (FullScreenOutlineGOC.pass1 == null)
            {
                FullScreenOutlineGOC.pass1 = Assets.Get<Material>("Materials/Misc/FullScreenOutlinePostProc_Pass1");
            }
            if (FullScreenOutlineGOC.pass2 == null)
            {
                FullScreenOutlineGOC.pass2 = Assets.Get<Material>("Materials/Misc/FullScreenOutlinePostProc_Pass2");
            }
            if (FullScreenOutlineGOC.pass3 == null)
            {
                FullScreenOutlineGOC.pass3 = Assets.Get<Material>("Materials/Misc/FullScreenOutlinePostProc_Pass3");
            }
            if (FullScreenOutlineGOC.pass2Round == null)
            {
                FullScreenOutlineGOC.pass2Round = Assets.Get<Material>("Materials/Misc/FullScreenOutlinePostProc_Pass2Round");
            }
            if (FullScreenOutlineGOC.pass3Round == null)
            {
                FullScreenOutlineGOC.pass3Round = Assets.Get<Material>("Materials/Misc/FullScreenOutlinePostProc_Pass3Round");
            }
            base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
        }

        [ImageEffectOpaque]
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (PrefsHelper.Outline == "Multi" || PrefsHelper.Outline == "MultiRounded")
            {
                RenderTexture renderTexture = RenderTexturePool.Get(source.width, source.height, 0, RenderTextureFormat.ARGB64);
                Graphics.Blit(source, renderTexture, FullScreenOutlineGOC.pass1);
                RenderTexture renderTexture2 = RenderTexturePool.Get(source.width, source.height, 0, RenderTextureFormat.ARGB64);
                float num = Math.Max((float)source.width * 0.0006510417f, 1f);
                Material material = ((PrefsHelper.Outline == "MultiRounded") ? FullScreenOutlineGOC.pass2Round : FullScreenOutlineGOC.pass2);
                material.SetInt(Get.ShaderPropertyIDs.IterationsCountID, Calc.CeilToInt(num) + 1);
                material.SetFloat(Get.ShaderPropertyIDs.MaxThicknessInPixelsID, num);
                Graphics.Blit(renderTexture, renderTexture2, material);
                Material material2 = ((PrefsHelper.Outline == "MultiRounded") ? FullScreenOutlineGOC.pass3Round : FullScreenOutlineGOC.pass3);
                material2.SetInt(Get.ShaderPropertyIDs.IterationsCountID, Calc.CeilToInt(num) + 1);
                material2.SetFloat(Get.ShaderPropertyIDs.MaxThicknessInPixelsID, num);
                material2.SetTexture(Get.ShaderPropertyIDs.SceneTexID, source);
                Graphics.Blit(renderTexture2, destination, material2);
                RenderTexturePool.Return(renderTexture);
                RenderTexturePool.Return(renderTexture2);
                return;
            }
            int num2 = Calc.RoundToInt(((float)source.width + 1f) / 1920f);
            FullScreenOutlineGOC.cheap.SetFloat(Get.ShaderPropertyIDs.MaxThicknessInPixelsID, (float)num2);
            Graphics.Blit(source, destination, FullScreenOutlineGOC.cheap);
        }

        private static Material cheap;

        private static Material pass1;

        private static Material pass2;

        private static Material pass3;

        private static Material pass2Round;

        private static Material pass3Round;
    }
}