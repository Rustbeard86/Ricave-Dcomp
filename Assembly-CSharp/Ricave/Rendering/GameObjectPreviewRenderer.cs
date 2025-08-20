using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ricave.Rendering
{
    public static class GameObjectPreviewRenderer
    {
        public static RenderTexture GetPreview(GameObject gameObject, float rotAroundY = 0f, float rotAroundX = 0f, Bounds? specificBounds = null)
        {
            Color ambientLight = RenderSettings.ambientLight;
            Color globalColor = Shader.GetGlobalColor(Get.ShaderPropertyIDs.CustomFogColorID);
            float globalFloat = Shader.GetGlobalFloat(Get.ShaderPropertyIDs.CustomFogIntensityID);
            bool fog = RenderSettings.fog;
            int antiAliasing = QualitySettings.antiAliasing;
            RenderTexture active = RenderTexture.active;
            bool activeSelf = gameObject.activeSelf;
            RenderSettings.ambientLight = Color.white;
            Shader.SetGlobalColor(Get.ShaderPropertyIDs.CustomFogColorID, Color.black);
            Shader.SetGlobalFloat(Get.ShaderPropertyIDs.CustomFogIntensityID, 0.001f);
            RenderSettings.fog = false;
            Vector3 position = gameObject.transform.position;
            Quaternion rotation = gameObject.transform.rotation;
            Vector3 localScale = gameObject.transform.localScale;
            gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            gameObject.transform.localScale = Vector3.one;
            gameObject.SetActive(true);
            RenderTexture targetTexture;
            try
            {
                Camera camera = GameObjectPreviewRenderer.GetPreviewCamera();
                Bounds? bounds = null;
                if (specificBounds != null)
                {
                    bounds = specificBounds;
                }
                else
                {
                    gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    gameObject.transform.localScale = Vector3.one;
                    GameObjectPreviewRenderer.tmpMeshRenderers.Clear();
                    gameObject.GetComponentsInChildren<MeshRenderer>(GameObjectPreviewRenderer.tmpMeshRenderers);
                    foreach (MeshRenderer meshRenderer in GameObjectPreviewRenderer.tmpMeshRenderers)
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
                }
                float num = Math.Max(bounds.Value.extents.x, bounds.Value.extents.y) * 2f;
                camera.transform.position = bounds.Value.center + new Vector3(0f, 0f, bounds.Value.extents.z + num + 0.02f);
                camera.transform.LookAt(bounds.Value.center);
                gameObject.transform.Rotate(Vector3.up, rotAroundY);
                gameObject.transform.Rotate(Vector3.right, rotAroundX);
                GameObjectPreviewRenderer.tmpRenderers.Clear();
                gameObject.GetComponentsInChildren<Renderer>(GameObjectPreviewRenderer.tmpRenderers);
                GameObjectPreviewRenderer.tmpOldRendererState.Clear();
                foreach (Renderer renderer in GameObjectPreviewRenderer.tmpRenderers)
                {
                    GameObjectPreviewRenderer.tmpOldRendererState.Add(new ValueTuple<int, ShadowCastingMode>(renderer.gameObject.layer, renderer.shadowCastingMode));
                    renderer.gameObject.layer = Get.Layers.ForIconRendererLayer;
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                }
                camera.Render();
                for (int i = 0; i < GameObjectPreviewRenderer.tmpRenderers.Count; i++)
                {
                    ValueTuple<int, ShadowCastingMode> valueTuple = GameObjectPreviewRenderer.tmpOldRendererState[i];
                    int item = valueTuple.Item1;
                    ShadowCastingMode item2 = valueTuple.Item2;
                    GameObjectPreviewRenderer.tmpRenderers[i].gameObject.layer = item;
                    GameObjectPreviewRenderer.tmpRenderers[i].shadowCastingMode = item2;
                }
                targetTexture = camera.targetTexture;
            }
            finally
            {
                RenderSettings.ambientLight = ambientLight;
                Shader.SetGlobalColor(Get.ShaderPropertyIDs.CustomFogColorID, globalColor);
                Shader.SetGlobalFloat(Get.ShaderPropertyIDs.CustomFogIntensityID, globalFloat);
                RenderSettings.fog = fog;
                QualitySettings.antiAliasing = antiAliasing;
                RenderTexture.active = active;
                gameObject.SetActive(activeSelf);
                gameObject.transform.SetPositionAndRotation(position, rotation);
                gameObject.transform.localScale = localScale;
            }
            return targetTexture;
        }

        private static Camera GetPreviewCamera()
        {
            if (GameObjectPreviewRenderer.previewCamera != null)
            {
                return GameObjectPreviewRenderer.previewCamera;
            }
            GameObject gameObject = new GameObject("PreviewCamera");
            gameObject.SetActive(false);
            Object.DontDestroyOnLoad(gameObject);
            GameObjectPreviewRenderer.previewCamera = gameObject.AddComponent<Camera>();
            GameObjectPreviewRenderer.previewCamera.transform.position = new Vector3(0f, 0f, -1f);
            GameObjectPreviewRenderer.previewCamera.fieldOfView = 60f;
            GameObjectPreviewRenderer.previewCamera.clearFlags = CameraClearFlags.Color;
            GameObjectPreviewRenderer.previewCamera.backgroundColor = Color.clear;
            GameObjectPreviewRenderer.previewCamera.nearClipPlane = 0.1f;
            GameObjectPreviewRenderer.previewCamera.farClipPlane = 25f;
            GameObjectPreviewRenderer.previewCamera.cullingMask = Get.Layers.ForIconRendererMask;
            GameObjectPreviewRenderer.previewCamera.renderingPath = RenderingPath.Forward;
            GameObjectPreviewRenderer.previewCamera.allowMSAA = false;
            GameObjectPreviewRenderer.previewCamera.targetTexture = new RenderTexture(512, 512, 24)
            {
                name = "PreviewRendererRenderTexture",
                filterMode = FilterMode.Trilinear,
                anisoLevel = 2,
                wrapMode = TextureWrapMode.Clamp
            };
            return GameObjectPreviewRenderer.previewCamera;
        }

        private static Camera previewCamera;

        private static List<MeshRenderer> tmpMeshRenderers = new List<MeshRenderer>();

        private static List<Renderer> tmpRenderers = new List<Renderer>();

        private static List<ValueTuple<int, ShadowCastingMode>> tmpOldRendererState = new List<ValueTuple<int, ShadowCastingMode>>();
    }
}