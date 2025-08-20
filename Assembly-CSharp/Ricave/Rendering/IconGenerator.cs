using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ricave.Rendering
{
    public static class IconGenerator
    {
        public static Texture2D GenerateIcon(GameObject gameObject, bool sideView = false, bool frontView = false)
        {
            Color ambientLight = RenderSettings.ambientLight;
            Color globalColor = Shader.GetGlobalColor(Get.ShaderPropertyIDs.CustomFogColorID);
            float globalFloat = Shader.GetGlobalFloat(Get.ShaderPropertyIDs.CustomFogIntensityID);
            bool fog = RenderSettings.fog;
            int antiAliasing = QualitySettings.antiAliasing;
            RenderTexture active = RenderTexture.active;
            RenderSettings.ambientLight = Color.white;
            Shader.SetGlobalColor(Get.ShaderPropertyIDs.CustomFogColorID, Color.black);
            Shader.SetGlobalFloat(Get.ShaderPropertyIDs.CustomFogIntensityID, 0.001f);
            RenderSettings.fog = false;
            GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
            Texture2D texture2D;
            try
            {
                Camera camera = IconGenerator.GetIconCamera();
                Bounds? bounds = null;
                gameObject2.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                gameObject2.transform.localScale = Vector3.one;
                IconGenerator.tmpMeshRenderers.Clear();
                gameObject2.GetComponentsInChildren<MeshRenderer>(IconGenerator.tmpMeshRenderers);
                foreach (MeshRenderer meshRenderer in IconGenerator.tmpMeshRenderers)
                {
                    if (bounds == null)
                    {
                        bounds = new Bounds?(meshRenderer.bounds);
                    }
                    else
                    {
                        Bounds value = bounds.Value;
                        value.Encapsulate(meshRenderer.bounds);
                        bounds = new Bounds?(value);
                    }
                }
                if (bounds == null)
                {
                    bounds = new Bounds?(new Bounds(Vector3.zero, Vector3.zero));
                }
                if (sideView)
                {
                    float num = Math.Max(bounds.Value.extents.z, bounds.Value.extents.y) * 2f;
                    camera.transform.position = bounds.Value.center + new Vector3(bounds.Value.extents.x + num + 0.02f, 0f, 0f);
                }
                else
                {
                    float num2 = Math.Max(bounds.Value.extents.x, bounds.Value.extents.y) * 2f;
                    if (frontView)
                    {
                        camera.transform.position = bounds.Value.center - new Vector3(0f, 0f, bounds.Value.extents.z + num2 + 0.02f);
                    }
                    else
                    {
                        camera.transform.position = bounds.Value.center + new Vector3(0f, 0f, bounds.Value.extents.z + num2 + 0.02f);
                    }
                }
                camera.transform.LookAt(bounds.Value.center);
                EntityGOC entityGOC;
                if (gameObject2.TryGetComponent<EntityGOC>(out entityGOC))
                {
                    Object.DestroyImmediate(entityGOC);
                }
                foreach (ParticleSystem particleSystem in gameObject2.GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>())
                {
                    if (particleSystem.name.Contains("Glitter"))
                    {
                        Object.DestroyImmediate(particleSystem);
                    }
                }
                IconGenerator.tmpRenderers.Clear();
                gameObject2.GetComponentsInChildren<Renderer>(IconGenerator.tmpRenderers);
                foreach (Renderer renderer in IconGenerator.tmpRenderers)
                {
                    renderer.gameObject.layer = Get.Layers.ForIconRendererLayer;
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                }
                foreach (ParticleSystem particleSystem2 in gameObject2.GetComponentsInChildren<ParticleSystem>())
                {
                    particleSystem2.Simulate(2f);
                    particleSystem2.Play();
                }
                gameObject2.SetActive(true);
                camera.Render();
                if (IconGenerator.downsampleRT == null)
                {
                    IconGenerator.downsampleRT = new RenderTexture(256, 256, 24)
                    {
                        name = "IconGeneratorDownsampleRT",
                        filterMode = FilterMode.Trilinear,
                        anisoLevel = 2,
                        wrapMode = TextureWrapMode.Clamp
                    };
                }
                RenderTexture active2 = RenderTexture.active;
                RenderTexture.active = IconGenerator.downsampleRT;
                GL.Clear(true, true, Color.clear);
                RenderTexture.active = active2;
                float num3 = 7f / (float)IconGenerator.iconCamera.targetTexture.width;
                IconGenerator.IconGeneratorOutline.SetFloat("_ColorFactor", 0f);
                IconGenerator.IconGeneratorOutline.SetVector("_Offset", new Vector2(-num3, -num3));
                Graphics.Blit(IconGenerator.iconCamera.targetTexture, IconGenerator.downsampleRT, IconGenerator.IconGeneratorOutline);
                IconGenerator.IconGeneratorOutline.SetVector("_Offset", new Vector2(num3, -num3));
                Graphics.Blit(IconGenerator.iconCamera.targetTexture, IconGenerator.downsampleRT, IconGenerator.IconGeneratorOutline);
                IconGenerator.IconGeneratorOutline.SetVector("_Offset", new Vector2(-num3, num3));
                Graphics.Blit(IconGenerator.iconCamera.targetTexture, IconGenerator.downsampleRT, IconGenerator.IconGeneratorOutline);
                IconGenerator.IconGeneratorOutline.SetVector("_Offset", new Vector2(num3, num3));
                Graphics.Blit(IconGenerator.iconCamera.targetTexture, IconGenerator.downsampleRT, IconGenerator.IconGeneratorOutline);
                IconGenerator.IconGeneratorOutline.SetVector("_Offset", new Vector2(-num3, 0f));
                Graphics.Blit(IconGenerator.iconCamera.targetTexture, IconGenerator.downsampleRT, IconGenerator.IconGeneratorOutline);
                IconGenerator.IconGeneratorOutline.SetVector("_Offset", new Vector2(num3, 0f));
                Graphics.Blit(IconGenerator.iconCamera.targetTexture, IconGenerator.downsampleRT, IconGenerator.IconGeneratorOutline);
                IconGenerator.IconGeneratorOutline.SetVector("_Offset", new Vector2(0f, -num3));
                Graphics.Blit(IconGenerator.iconCamera.targetTexture, IconGenerator.downsampleRT, IconGenerator.IconGeneratorOutline);
                IconGenerator.IconGeneratorOutline.SetVector("_Offset", new Vector2(0f, num3));
                Graphics.Blit(IconGenerator.iconCamera.targetTexture, IconGenerator.downsampleRT, IconGenerator.IconGeneratorOutline);
                IconGenerator.IconGeneratorOutline.SetFloat("_ColorFactor", 1f);
                IconGenerator.IconGeneratorOutline.SetVector("_Offset", Vector2.zero);
                Graphics.Blit(IconGenerator.iconCamera.targetTexture, IconGenerator.downsampleRT, IconGenerator.IconGeneratorOutline);
                texture2D = IconGenerator.downsampleRT.ToTexture2D();
            }
            finally
            {
                Object.DestroyImmediate(gameObject2);
                RenderSettings.ambientLight = ambientLight;
                Shader.SetGlobalColor(Get.ShaderPropertyIDs.CustomFogColorID, globalColor);
                Shader.SetGlobalFloat(Get.ShaderPropertyIDs.CustomFogIntensityID, globalFloat);
                RenderSettings.fog = fog;
                QualitySettings.antiAliasing = antiAliasing;
                RenderTexture.active = active;
            }
            return texture2D;
        }

        private static Camera GetIconCamera()
        {
            if (IconGenerator.iconCamera != null)
            {
                return IconGenerator.iconCamera;
            }
            GameObject gameObject = new GameObject("IconCamera");
            gameObject.SetActive(false);
            Object.DontDestroyOnLoad(gameObject);
            IconGenerator.iconCamera = gameObject.AddComponent<Camera>();
            IconGenerator.iconCamera.transform.position = new Vector3(0f, 0f, -1f);
            IconGenerator.iconCamera.fieldOfView = 60f;
            IconGenerator.iconCamera.clearFlags = CameraClearFlags.Color;
            IconGenerator.iconCamera.backgroundColor = Color.clear;
            IconGenerator.iconCamera.nearClipPlane = 0.1f;
            IconGenerator.iconCamera.farClipPlane = 25f;
            IconGenerator.iconCamera.cullingMask = Get.Layers.ForIconRendererMask;
            IconGenerator.iconCamera.renderingPath = RenderingPath.Forward;
            IconGenerator.iconCamera.allowMSAA = false;
            IconGenerator.iconCamera.targetTexture = new RenderTexture(512, 512, 24)
            {
                name = "IconGeneratorRenderTexture",
                filterMode = FilterMode.Trilinear,
                anisoLevel = 2,
                wrapMode = TextureWrapMode.Clamp
            };
            return IconGenerator.iconCamera;
        }

        private static Camera iconCamera;

        private static RenderTexture downsampleRT;

        private static List<MeshRenderer> tmpMeshRenderers = new List<MeshRenderer>();

        private static List<Renderer> tmpRenderers = new List<Renderer>();

        private static readonly Material IconGeneratorOutline = Assets.Get<Material>("Materials/UI/IconGeneratorOutline");

        private const float OutlineThickness = 7f;
    }
}