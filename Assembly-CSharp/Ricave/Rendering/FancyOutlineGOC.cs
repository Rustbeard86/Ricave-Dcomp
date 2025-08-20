using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ricave.Rendering
{
    public class FancyOutlineGOC : MonoBehaviour
    {
        private void Start()
        {
            if (FancyOutlineGOC.PostProcMat == null)
            {
                FancyOutlineGOC.PostProcMat = Assets.Get<Material>("Materials/Misc/FancyOutlinePostProc");
            }
            if (FancyOutlineGOC.CutoutForFancyOutline == null)
            {
                FancyOutlineGOC.CutoutForFancyOutline = Assets.Get<Material>("Materials/Misc/CutoutForFancyOutline");
            }
            if (FancyOutlineGOC.ActorWithMissingBodyPartsCutoutForFancyOutline == null)
            {
                FancyOutlineGOC.ActorWithMissingBodyPartsCutoutForFancyOutline = Assets.Get<Material>("Materials/Actors/ActorWithMissingBodyPartsCutoutForFancyOutline");
            }
            if (FancyOutlineGOC.BodyPartHighlightPrefab == null)
            {
                FancyOutlineGOC.BodyPartHighlightPrefab = Assets.Get<GameObject>("Prefabs/Actors/BodyPartHighlight");
                FancyOutlineGOC.BodyPartHighlightPrefabShader = FancyOutlineGOC.BodyPartHighlightPrefab.GetComponent<Renderer>().sharedMaterial.shader;
            }
            if (FancyOutlineGOC.ActorWithMissingBodyParts == null)
            {
                FancyOutlineGOC.ActorWithMissingBodyParts = Assets.Get<Material>("Materials/Actors/ActorWithMissingBodyParts");
            }
            if (FancyOutlineGOC.AnimatedGrass == null)
            {
                FancyOutlineGOC.AnimatedGrass = Assets.Get<Material>("Materials/Structures/AnimatedGrass");
            }
            if (FancyOutlineGOC.Fluid == null)
            {
                FancyOutlineGOC.Fluid = Assets.Get<Shader>("Shaders/Structures/Fluid");
            }
            this.cachedCamera = base.GetComponent<Camera>();
            this.cachedCamera.targetTexture = this.CreateRenderTexture();
            this.cachedCamera.depthTextureMode = DepthTextureMode.DepthNormals;
        }

        private void OnDestroy()
        {
            if (this.cachedCamera != null)
            {
                RenderTexture targetTexture = this.cachedCamera.targetTexture;
                if (targetTexture != null)
                {
                    this.cachedCamera.targetTexture = null;
                    Object.Destroy(targetTexture);
                }
            }
        }

        private void LateUpdate()
        {
            Profiler.Begin("FancyOutlineGOC calculations");
            try
            {
                if (this.cachedCamera.targetTexture.width != Sys.Resolution.x || this.cachedCamera.targetTexture.height != Sys.Resolution.y)
                {
                    Object targetTexture = this.cachedCamera.targetTexture;
                    this.cachedCamera.targetTexture = null;
                    Object.Destroy(targetTexture);
                    this.cachedCamera.targetTexture = this.CreateRenderTexture();
                }
                ValueTuple<float, float, float, float> valueTuple = this.CalculateScreenSpaceBounds();
                float item = valueTuple.Item1;
                float item2 = valueTuple.Item2;
                float item3 = valueTuple.Item3;
                float item4 = valueTuple.Item4;
                this.clippedRectTexCoords = new Rect(item, item2, item3 - item, item4 - item2);
                this.clippedRectTexCoords.x = (float)((int)(this.clippedRectTexCoords.x * (float)Sys.Resolution.x)) / (float)Sys.Resolution.x;
                this.clippedRectTexCoords.y = (float)((int)(this.clippedRectTexCoords.y * (float)Sys.Resolution.y)) / (float)Sys.Resolution.y;
                this.clippedRectTexCoords.width = (float)((int)(this.clippedRectTexCoords.width * (float)Sys.Resolution.x)) / (float)Sys.Resolution.x;
                this.clippedRectTexCoords.height = (float)((int)(this.clippedRectTexCoords.height * (float)Sys.Resolution.y)) / (float)Sys.Resolution.y;
                float num = Math.Max(this.clippedRectTexCoords.width, this.clippedRectTexCoords.height);
                this.clippedRectTexCoords.width = num;
                this.clippedRectTexCoords.height = num;
                if (this.clippedRectTexCoords.xMax > 1f)
                {
                    this.clippedRectTexCoords.x = this.clippedRectTexCoords.x - (this.clippedRectTexCoords.xMax - 1f);
                }
                if (this.clippedRectTexCoords.yMax > 1f)
                {
                    this.clippedRectTexCoords.y = this.clippedRectTexCoords.y - (this.clippedRectTexCoords.yMax - 1f);
                }
            }
            finally
            {
                Profiler.End();
            }
        }

        private ValueTuple<float, float, float, float> CalculateScreenSpaceBounds()
        {
            Bounds bounds = default(Bounds);
            bool flag = true;
            List<Renderer> allHighlightedRenderers = Get.GameObjectHighlighter.AllHighlightedRenderers;
            for (int i = 0; i < allHighlightedRenderers.Count; i++)
            {
                Renderer renderer = allHighlightedRenderers[i];
                if (flag)
                {
                    flag = false;
                    bounds = renderer.bounds;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
            FancyOutlineGOC.<> c__DisplayClass18_0 CS$<> 8__locals1;
            CS$<> 8__locals1.minX = 1f;
            CS$<> 8__locals1.maxX = 0f;
            CS$<> 8__locals1.minY = 1f;
            CS$<> 8__locals1.maxY = 0f;
            FancyOutlineGOC.< CalculateScreenSpaceBounds > g__CheckPoint | 18_0(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), ref CS$<> 8__locals1);
            FancyOutlineGOC.< CalculateScreenSpaceBounds > g__CheckPoint | 18_0(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), ref CS$<> 8__locals1);
            FancyOutlineGOC.< CalculateScreenSpaceBounds > g__CheckPoint | 18_0(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), ref CS$<> 8__locals1);
            FancyOutlineGOC.< CalculateScreenSpaceBounds > g__CheckPoint | 18_0(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), ref CS$<> 8__locals1);
            FancyOutlineGOC.< CalculateScreenSpaceBounds > g__CheckPoint | 18_0(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), ref CS$<> 8__locals1);
            FancyOutlineGOC.< CalculateScreenSpaceBounds > g__CheckPoint | 18_0(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), ref CS$<> 8__locals1);
            FancyOutlineGOC.< CalculateScreenSpaceBounds > g__CheckPoint | 18_0(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), ref CS$<> 8__locals1);
            FancyOutlineGOC.< CalculateScreenSpaceBounds > g__CheckPoint | 18_0(new Vector3(bounds.max.x, bounds.max.y, bounds.max.z), ref CS$<> 8__locals1);
            CS$<> 8__locals1.minX -= 0.01f;
            CS$<> 8__locals1.minY -= 0.01f;
            CS$<> 8__locals1.maxX += 0.01f;
            CS$<> 8__locals1.maxY += 0.01f;
            CS$<> 8__locals1.minX = Math.Max(CS$<> 8__locals1.minX, 0f);
            CS$<> 8__locals1.minY = Math.Max(CS$<> 8__locals1.minY, 0f);
            CS$<> 8__locals1.maxX = Math.Min(CS$<> 8__locals1.maxX, 1f);
            CS$<> 8__locals1.maxY = Math.Min(CS$<> 8__locals1.maxY, 1f);
            return new ValueTuple<float, float, float, float>(CS$<> 8__locals1.minX, CS$<> 8__locals1.minY, CS$<> 8__locals1.maxX, CS$<> 8__locals1.maxY);
        }

        public void DrawOnGUI()
        {
            if (base.gameObject.activeSelf && this.cachedCamera != null && this.lastDrawnFrame >= Clock.Frame - 1)
            {
                GUI.DrawTexture(new Rect(this.clippedRectTexCoords.x * Widgets.VirtualWidth, (1f - this.clippedRectTexCoords.y - this.clippedRectTexCoords.height) * Widgets.VirtualHeight, this.clippedRectTexCoords.width * Widgets.VirtualWidth, this.clippedRectTexCoords.height * Widgets.VirtualHeight), this.cachedCamera.targetTexture);
            }
        }

        private void OnPreCull()
        {
            this.temporarilyChangedGameObjects.Clear();
            List<Renderer> allHighlightedRenderers = Get.GameObjectHighlighter.AllHighlightedRenderers;
            for (int i = 0; i < allHighlightedRenderers.Count; i++)
            {
                Renderer renderer = allHighlightedRenderers[i];
                this.temporarilyChangedGameObjects.Add(new FancyOutlineGOC.TemporarilyChangedGameObject
                {
                    renderer = renderer,
                    prevRendererEnabled = renderer.enabled,
                    prevLayer = renderer.gameObject.layer,
                    prevMaterial = renderer.sharedMaterial,
                    prevShadowCastingMode = renderer.shadowCastingMode,
                    prevReceiveShadows = renderer.receiveShadows
                });
            }
            for (int j = 0; j < this.temporarilyChangedGameObjects.Count; j++)
            {
                FancyOutlineGOC.TemporarilyChangedGameObject temporarilyChangedGameObject = this.temporarilyChangedGameObjects[j];
                if (!this.Supported(temporarilyChangedGameObject.renderer))
                {
                    temporarilyChangedGameObject.renderer.enabled = false;
                }
                else
                {
                    temporarilyChangedGameObject.renderer.enabled = true;
                    temporarilyChangedGameObject.renderer.gameObject.layer = Get.ForHighlightLayer;
                    temporarilyChangedGameObject.renderer.sharedMaterial = this.GetMaterialFor(temporarilyChangedGameObject.renderer.sharedMaterial);
                    temporarilyChangedGameObject.renderer.shadowCastingMode = ShadowCastingMode.Off;
                    temporarilyChangedGameObject.renderer.receiveShadows = false;
                }
            }
        }

        private void OnPostRender()
        {
            for (int i = 0; i < this.temporarilyChangedGameObjects.Count; i++)
            {
                FancyOutlineGOC.TemporarilyChangedGameObject temporarilyChangedGameObject = this.temporarilyChangedGameObjects[i];
                Renderer renderer = temporarilyChangedGameObject.renderer;
                renderer.enabled = temporarilyChangedGameObject.prevRendererEnabled;
                renderer.gameObject.layer = temporarilyChangedGameObject.prevLayer;
                renderer.sharedMaterial = temporarilyChangedGameObject.prevMaterial;
                renderer.shadowCastingMode = temporarilyChangedGameObject.prevShadowCastingMode;
                renderer.receiveShadows = temporarilyChangedGameObject.prevReceiveShadows;
            }
            this.temporarilyChangedGameObjects.Clear();
            this.lastDrawnFrame = Clock.Frame;
        }

        private void OnPreRender()
        {
            this.SetClippingRect(this.clippedRectTexCoords);
            FancyOutlineGOC.PostProcMat.color = Get.GameObjectHighlighter.CustomColor ?? new Color(1f, 0.6f, 0f);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Vector2 vector = new Vector2(0.0006510417f, 0.0006510417f);
            Rect rect = this.clippedRectTexCoords;
            vector.x /= rect.width;
            vector.y /= rect.height;
            vector.y *= (float)Sys.Resolution.x / (float)Sys.Resolution.y;
            if (vector.x < 1.5f / (float)source.width)
            {
                vector.x = 1.5f / (float)source.width;
            }
            if (vector.y < 1.5f / (float)source.height)
            {
                vector.y = 1.5f / (float)source.height;
            }
            FancyOutlineGOC.PostProcMat.SetVector(Get.ShaderPropertyIDs.SampleStepID, vector);
            Graphics.Blit(source, destination, FancyOutlineGOC.PostProcMat);
        }

        private RenderTexture CreateRenderTexture()
        {
            return new RenderTexture(Sys.Resolution.x, Sys.Resolution.y, 24);
        }

        private Material GetMaterialFor(Material originalMaterial)
        {
            if (originalMaterial == null)
            {
                return null;
            }
            if (originalMaterial.shader == FancyOutlineGOC.AnimatedGrass.shader)
            {
                return originalMaterial;
            }
            if (originalMaterial.shader == FancyOutlineGOC.Fluid)
            {
                return originalMaterial;
            }
            Material material;
            if (!this.cutoutMaterials.TryGetValue(originalMaterial, out material))
            {
                if (originalMaterial.shader == FancyOutlineGOC.ActorWithMissingBodyParts.shader)
                {
                    material = Object.Instantiate<Material>(FancyOutlineGOC.ActorWithMissingBodyPartsCutoutForFancyOutline);
                    material.mainTexture = originalMaterial.mainTexture;
                    material.SetTexture(Get.ShaderPropertyIDs.BodyMapID, originalMaterial.GetTexture(Get.ShaderPropertyIDs.BodyMapID));
                }
                else
                {
                    material = Object.Instantiate<Material>(FancyOutlineGOC.CutoutForFancyOutline);
                    material.mainTexture = originalMaterial.mainTexture;
                }
                this.cutoutMaterials.Add(originalMaterial, material);
            }
            if (originalMaterial.shader == FancyOutlineGOC.ActorWithMissingBodyParts.shader)
            {
                for (int i = 0; i < 7; i++)
                {
                    material.SetFloat(Get.ShaderPropertyIDs.PartXExistsID[i], originalMaterial.GetFloat(Get.ShaderPropertyIDs.PartXExistsID[i]));
                }
            }
            return material;
        }

        private bool Supported(Renderer renderer)
        {
            if (renderer.CompareTag("ExcludeFromFancyOutline"))
            {
                return false;
            }
            if (renderer.sharedMaterial == null)
            {
                return false;
            }
            if (renderer.shadowCastingMode == ShadowCastingMode.ShadowsOnly)
            {
                return false;
            }
            if (renderer.sharedMaterial.shader == FancyOutlineGOC.BodyPartHighlightPrefabShader)
            {
                return false;
            }
            ParticleSystemRenderer particleSystemRenderer = renderer as ParticleSystemRenderer;
            if (particleSystemRenderer != null)
            {
                if (particleSystemRenderer.trailMaterial != null)
                {
                    return false;
                }
                ParticleSystem particleSystem;
                if (renderer.TryGetComponent<ParticleSystem>(out particleSystem))
                {
                    float num;
                    if (particleSystemRenderer.sharedMaterial.mainTexture == this.DefaultParticleTexture)
                    {
                        num = 0.5f;
                    }
                    else
                    {
                        num = 0.32f;
                    }
                    if (particleSystem.main.startColor.Evaluate(0.4f).a < num && particleSystem.main.startColor.Evaluate(0.6f).a < num)
                    {
                        return false;
                    }
                    if (particleSystem.colorOverLifetime.enabled && particleSystem.colorOverLifetime.color.Evaluate(0.4f).a < num && particleSystem.colorOverLifetime.color.Evaluate(0.6f).a < num)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void SetClippingRect(Rect rect)
        {
            if (rect.x < 0f)
            {
                rect.width += rect.x;
                rect.x = 0f;
            }
            if (rect.y < 0f)
            {
                rect.height += rect.y;
                rect.y = 0f;
            }
            rect.width = Math.Min(1f - rect.x, rect.width);
            rect.height = Math.Min(1f - rect.y, rect.height);
            this.cachedCamera.ResetProjectionMatrix();
            Matrix4x4 projectionMatrix = this.cachedCamera.projectionMatrix;
            this.cachedCamera.rect = rect;
            Matrix4x4.TRS(new Vector3(rect.x, rect.y, 0f), Quaternion.identity, new Vector3(rect.width, rect.height, 1f));
            Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(1f / rect.width - 1f, 1f / rect.height - 1f, 0f), Quaternion.identity, new Vector3(1f / rect.width, 1f / rect.height, 1f));
            Matrix4x4 matrix4x2 = Matrix4x4.TRS(new Vector3(-rect.x * 2f / rect.width, -rect.y * 2f / rect.height, 0f), Quaternion.identity, Vector3.one);
            this.cachedCamera.projectionMatrix = matrix4x2 * matrix4x * projectionMatrix;
        }

        [CompilerGenerated]
        internal static void <CalculateScreenSpaceBounds>g__CheckPoint|18_0(Vector3 point, ref FancyOutlineGOC.<>c__DisplayClass18_0 A_1)
		{
            Vector3 vector = Get.Camera.WorldToScreenPoint(point);
			if (vector.z< 0f)
			{
				A_1.minX = 0f;
				A_1.minY = 0f;
				A_1.maxX = 1f;
				A_1.maxY = 1f;
				return;
			}
    vector.x /= (float) Sys.Resolution.x;
    vector.y /= (float) Sys.Resolution.y;
			if (vector.x<A_1.minX)
			{
				A_1.minX = vector.x;
			}
if (vector.y < A_1.minY)
{
    A_1.minY = vector.y;
}
if (vector.x > A_1.maxX)
{
    A_1.maxX = vector.x;
}
if (vector.y > A_1.maxY)
{
    A_1.maxY = vector.y;
}
		}

		private Camera cachedCamera;

private List<FancyOutlineGOC.TemporarilyChangedGameObject> temporarilyChangedGameObjects = new List<FancyOutlineGOC.TemporarilyChangedGameObject>();

private Dictionary<Material, Material> cutoutMaterials = new Dictionary<Material, Material>();

private int lastDrawnFrame;

private Rect clippedRectTexCoords;

private static Material PostProcMat;

private static Material CutoutForFancyOutline;

private static Material ActorWithMissingBodyPartsCutoutForFancyOutline;

private static GameObject BodyPartHighlightPrefab;

private static Shader BodyPartHighlightPrefabShader;

private static Material ActorWithMissingBodyParts;

private static Material AnimatedGrass;

private static Shader Fluid;

[SerializeField]
private Texture DefaultParticleTexture;

private struct TemporarilyChangedGameObject
{
    public Renderer renderer;

    public bool prevRendererEnabled;

    public int prevLayer;

    public Material prevMaterial;

    public ShadowCastingMode prevShadowCastingMode;

    public bool prevReceiveShadows;
}
	}
}