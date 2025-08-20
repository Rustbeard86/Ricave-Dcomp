using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class VisualEffectsManager
    {
        public Vector3 ParticlesFlyingTowardsCameraDestination
        {
            get
            {
                if (Get.MainActor != Get.NowControlledActor)
                {
                    return Get.MainActor.RenderPosition;
                }
                return Get.CameraPosition.WithAddedY(-0.22f);
            }
        }

        public void DoOneShot(VisualEffectSpec spec, Vector3 pos)
        {
            this.DoOneShot(spec, pos, Quaternion.identity, Vector3.one, null, null, null);
        }

        public void DoOneShot(VisualEffectSpec spec, Vector3 pos, Quaternion rotation, Vector3 scale, Vector3? fallingEntityAnimationDestination = null, EntitySpec openChestAnimationChestSpec = null, EntitySpec buttonPressAnimationButtonSpec = null)
        {
            VisualEffectsManager.VisualEffect visualEffect = default(VisualEffectsManager.VisualEffect);
            this.InitAndStart(ref visualEffect, spec, pos, rotation, scale, fallingEntityAnimationDestination, openChestAnimationChestSpec, buttonPressAnimationButtonSpec);
            this.oneShots.Add(visualEffect);
        }

        public void Update()
        {
            for (int i = this.oneShots.Count - 1; i >= 0; i--)
            {
                if (!this.IsStillPlaying(this.oneShots[i]))
                {
                    this.StopAndCleanup(this.oneShots[i]);
                    this.oneShots.RemoveAt(i);
                }
                else if (!this.everWarnedAboutDuration && Clock.Time - this.oneShots[i].timeStarted > 15f)
                {
                    this.everWarnedAboutDuration = true;
                    Log.Warning("Some visual effects are playing for more than " + 15f.ToString() + " seconds. It's possible they never end.", false);
                }
            }
        }

        public void FixedUpdate()
        {
            for (int i = this.oneShots.Count - 1; i >= 0; i--)
            {
                if (this.oneShots[i].spec.MoveTowardsPlayer && this.oneShots[i].particleSystem != null)
                {
                    GameObject particleSystem = this.oneShots[i].particleSystem;
                    float num = Rand.RangeSeeded(0.2f, 0.5f, particleSystem.GetInstanceID());
                    if (Clock.Time > this.oneShots[i].timeStarted + num)
                    {
                        float num2 = (particleSystem.transform.position - this.oneShots[i].pos).magnitude * 0.09f;
                        num2 = Math.Max(num2, 0.009f);
                        particleSystem.transform.position = Vector3.MoveTowards(particleSystem.transform.position, this.ParticlesFlyingTowardsCameraDestination, num2);
                    }
                    if ((particleSystem.transform.position - this.ParticlesFlyingTowardsCameraDestination).sqrMagnitude < 0.010000001f)
                    {
                        this.StopAndCleanup(this.oneShots[i]);
                        this.oneShots.RemoveAt(i);
                        Get.Sound_ExperienceParticleArrived.PlayOneShot(null, 1f, 1f);
                        Get.CameraEffects.OnExperienceParticleArrived();
                    }
                }
            }
        }

        public void StopAllOfSpec(VisualEffectSpec spec)
        {
            for (int i = this.oneShots.Count - 1; i >= 0; i--)
            {
                if (this.oneShots[i].spec == spec)
                {
                    this.StopAndCleanup(this.oneShots[i]);
                    this.oneShots.RemoveAt(i);
                }
            }
        }

        private bool IsStillPlaying(VisualEffectsManager.VisualEffect effect)
        {
            return effect.particleSystemMainCompCached != null && effect.particleSystemMainCompCached.isPlaying;
        }

        private void InitAndStart(ref VisualEffectsManager.VisualEffect effect, VisualEffectSpec spec, Vector3 pos, Quaternion rotation, Vector3 scale, Vector3? fallingEntityAnimationDestination = null, EntitySpec openChestAnimationChestSpec = null, EntitySpec buttonPressAnimationButtonSpec = null)
        {
            effect.timeStarted = Clock.Time;
            effect.spec = spec;
            effect.pos = pos;
            effect.rotation = rotation;
            effect.scale = scale;
            if (spec.ParticleSystemPrefab != null)
            {
                effect.particleSystem = this.GetPooledParticleSystem(spec, pos, rotation, scale, fallingEntityAnimationDestination, openChestAnimationChestSpec, buttonPressAnimationButtonSpec);
                effect.particleSystemMainCompCached = effect.particleSystem.GetComponent<ParticleSystem>();
            }
            if (spec.CameraWobbleAmount != 0f)
            {
                Get.CameraEffects.Wobble(spec.CameraWobbleAmount);
            }
            if (spec.CameraWobbleAmountRange != default(FloatRange))
            {
                Get.CameraEffects.Wobble(spec.CameraWobbleAmountRange.RandomInRange);
            }
            if (spec.VignetteAmount != 0f)
            {
                float num = (((float)Get.NowControlledActor.HP < (float)Get.NowControlledActor.MaxHP * 0.5f) ? (spec.VignetteAmount * 1.8f) : spec.VignetteAmount);
                Get.CameraEffects.DoVignette(num);
            }
            if (spec.ShakeAmount != 0f)
            {
                Get.CameraEffects.Shake(spec.ShakeAmount);
            }
            if (spec.StrikeOverlay)
            {
                Get.StrikeOverlays.Strike(false);
            }
            if (spec.CameraFallEffect)
            {
                Get.CameraEffects.DoFallEffect();
            }
            if (spec.Sound != null)
            {
                spec.Sound.PlayOneShot(new Vector3?(pos), 1f, 1f);
            }
        }

        private void StopAndCleanup(VisualEffectsManager.VisualEffect effect)
        {
            if (effect.particleSystem != null)
            {
                if (effect.particleSystemMainCompCached != null)
                {
                    effect.particleSystemMainCompCached.Stop();
                }
                this.ReturnParticleSystemToPool(effect.particleSystem, effect.spec);
            }
        }

        private void SetFallingEntityAnimationDestination(GameObject particleSystem, Vector3 destination)
        {
            FallingEntityAnimationGOC componentInChildren = particleSystem.GetComponentInChildren<FallingEntityAnimationGOC>();
            if (componentInChildren == null)
            {
                Log.Error("Tried to set falling entity animation destination, but there's no such comp.", false);
                return;
            }
            componentInChildren.Destination = destination;
        }

        private void SetOpenChestAnimationChestSpec(GameObject particleSystem, EntitySpec chestSpec)
        {
            OpenChestAnimationGOC componentInChildren = particleSystem.GetComponentInChildren<OpenChestAnimationGOC>();
            if (componentInChildren == null)
            {
                Log.Error("Tried to set open chest animation chest spec, but there's no such comp.", false);
                return;
            }
            componentInChildren.ChestSpec = chestSpec;
        }

        private void SetButtonPressAnimationButtonSpec(GameObject particleSystem, EntitySpec buttonSpec)
        {
            ButtonPressAnimationGOC componentInChildren = particleSystem.GetComponentInChildren<ButtonPressAnimationGOC>();
            if (componentInChildren == null)
            {
                Log.Error("Tried to set button press animation button spec, but there's no such comp.", false);
                return;
            }
            componentInChildren.ButtonSpec = buttonSpec;
        }

        private void ReturnParticleSystemToPool(GameObject particleSystem, VisualEffectSpec spec)
        {
            ParticleSystem particleSystem2;
            if (particleSystem.TryGetComponent<ParticleSystem>(out particleSystem2))
            {
                particleSystem2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            particleSystem.SetActive(false);
            if (!spec.DisablePooling)
            {
                List<GameObject> list;
                if (!this.particleSystemsPool.TryGetValue(spec, out list))
                {
                    list = new List<GameObject>();
                    this.particleSystemsPool.Add(spec, list);
                }
                list.Add(particleSystem);
                return;
            }
            Object.Destroy(particleSystem);
        }

        private GameObject GetPooledParticleSystem(VisualEffectSpec spec, Vector3 pos, Quaternion rotation, Vector3 scale, Vector3? fallingEntityAnimationDestination = null, EntitySpec openChestAnimationChestSpec = null, EntitySpec buttonPressAnimationButtonSpec = null)
        {
            List<GameObject> list;
            GameObject gameObject;
            if (!spec.DisablePooling && this.particleSystemsPool.TryGetValue(spec, out list) && list.Count != 0)
            {
                gameObject = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                gameObject.transform.localPosition = pos;
                gameObject.transform.localRotation = rotation;
            }
            else
            {
                bool activeSelf = spec.ParticleSystemPrefab.activeSelf;
                spec.ParticleSystemPrefab.SetActive(false);
                try
                {
                    gameObject = Object.Instantiate<GameObject>(spec.ParticleSystemPrefab, pos, rotation, Get.RuntimeSpecialContainer.transform);
                    if (!spec.DisablePooling)
                    {
                        this.RememberOriginalLocalScalesForChildren(gameObject);
                    }
                }
                finally
                {
                    spec.ParticleSystemPrefab.SetActive(activeSelf);
                }
            }
            this.SetScale(gameObject, scale);
            ParticleSystem particleSystem;
            if (gameObject.TryGetComponent<ParticleSystem>(out particleSystem))
            {
                particleSystem.randomSeed = (uint)Calc.CombineHashes<int, Vector3, int>(spec.MyStableHash, pos, 846124295);
            }
            if (fallingEntityAnimationDestination != null)
            {
                this.SetFallingEntityAnimationDestination(gameObject, fallingEntityAnimationDestination.Value);
            }
            if (openChestAnimationChestSpec != null)
            {
                this.SetOpenChestAnimationChestSpec(gameObject, openChestAnimationChestSpec);
            }
            if (buttonPressAnimationButtonSpec != null)
            {
                this.SetButtonPressAnimationButtonSpec(gameObject, buttonPressAnimationButtonSpec);
            }
            gameObject.gameObject.SetActive(true);
            if (particleSystem != null && !particleSystem.isPlaying && particleSystem.main.playOnAwake)
            {
                particleSystem.Play();
            }
            return gameObject;
        }

        private void SetScale(GameObject particleSystem, Vector3 scale)
        {
            particleSystem.transform.localScale = scale;
            this.tmpChildParticleSystems.Clear();
            particleSystem.GetComponentsInChildren<ParticleSystem>(this.tmpChildParticleSystems);
            for (int i = 0; i < this.tmpChildParticleSystems.Count; i++)
            {
                ParticleSystem particleSystem2 = this.tmpChildParticleSystems[i];
                if (!(particleSystem2.gameObject == particleSystem))
                {
                    particleSystem2.transform.localScale = this.GetOriginalLocalScale(particleSystem2.gameObject).Multiply(scale);
                }
            }
            this.tmpChildParticleSystems.Clear();
        }

        private void RememberOriginalLocalScalesForChildren(GameObject pooledRootParticleSystem)
        {
            this.tmpChildParticleSystems.Clear();
            pooledRootParticleSystem.GetComponentsInChildren<ParticleSystem>(this.tmpChildParticleSystems);
            for (int i = 0; i < this.tmpChildParticleSystems.Count; i++)
            {
                ParticleSystem particleSystem = this.tmpChildParticleSystems[i];
                if (!(particleSystem.gameObject == pooledRootParticleSystem))
                {
                    this.pooledParticleSystemsOriginalLocalScale.Add(particleSystem.gameObject, particleSystem.transform.localScale);
                }
            }
            this.tmpChildParticleSystems.Clear();
        }

        private Vector3 GetOriginalLocalScale(GameObject possiblyPooledParticleSystem)
        {
            Vector3 vector;
            if (this.pooledParticleSystemsOriginalLocalScale.TryGetValue(possiblyPooledParticleSystem, out vector))
            {
                return vector;
            }
            return possiblyPooledParticleSystem.transform.localScale;
        }

        private List<VisualEffectsManager.VisualEffect> oneShots = new List<VisualEffectsManager.VisualEffect>();

        private bool everWarnedAboutDuration;

        private Dictionary<GameObject, Vector3> pooledParticleSystemsOriginalLocalScale = new Dictionary<GameObject, Vector3>();

        private Dictionary<VisualEffectSpec, List<GameObject>> particleSystemsPool = new Dictionary<VisualEffectSpec, List<GameObject>>();

        private const float ParticlesFlyingTowardsCameraRemoveAtDist = 0.1f;

        private const float WarnAfterDuration = 15f;

        private List<ParticleSystem> tmpChildParticleSystems = new List<ParticleSystem>();

        private struct VisualEffect
        {
            public VisualEffectSpec spec;

            public Vector3 pos;

            public Quaternion rotation;

            public Vector3 scale;

            public float timeStarted;

            public GameObject particleSystem;

            public ParticleSystem particleSystemMainCompCached;
        }
    }
}