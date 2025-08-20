using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class GameObjectFader
    {
        public List<ValueTuple<Entity, List<Renderer>>> FadingEntities
        {
            get
            {
                this.tmpFadingEntities.Clear();
                for (int i = 0; i < this.fadingGameObjects.Count; i++)
                {
                    if (this.fadingGameObjects[i].cachedEntityGOC != null && this.fadingGameObjects[i].cachedEntityGOC.Entity != null)
                    {
                        this.tmpFadingEntities.Add(new ValueTuple<Entity, List<Renderer>>(this.fadingGameObjects[i].cachedEntityGOC.Entity, this.fadingGameObjects[i].cachedRenderers));
                    }
                }
                return this.tmpFadingEntities;
            }
        }

        public bool IsFading(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }
            int instanceID = gameObject.GetInstanceID();
            return this.fadingGameObjects_fadingIn.Contains(instanceID) || this.fadingGameObjects_fadingOut.Contains(instanceID);
        }

        public bool IsFadingIn(GameObject gameObject)
        {
            return !(gameObject == null) && this.fadingGameObjects_fadingIn.Contains(gameObject.GetInstanceID());
        }

        public bool IsFadingOut(GameObject gameObject)
        {
            return !(gameObject == null) && this.fadingGameObjects_fadingOut.Contains(gameObject.GetInstanceID());
        }

        private bool IsFadingImpl(GameObject gameObject, bool fadingIn)
        {
            if (gameObject == null)
            {
                return false;
            }
            for (int i = 0; i < this.fadingGameObjects.Count; i++)
            {
                if (this.fadingGameObjects[i].fadingIn == fadingIn && (this.fadingGameObjects[i].original == gameObject || this.fadingGameObjects[i].fadedCopy == gameObject))
                {
                    return true;
                }
            }
            return false;
        }

        public void FadeIn(GameObject gameObject)
        {
            if (this.IsFadingIn(gameObject))
            {
                return;
            }
            if (!this.EnsureNotCombinedMesh(gameObject))
            {
                return;
            }
            bool flag = this.IsFadingOut(gameObject);
            bool flag2;
            float num = (flag ? Calc.InverseSmooth(1f - Calc.Smooth(this.GetFadePct(gameObject, out flag2))) : 0f);
            GameObjectFader.FadingGameObject fadingGameObject = this.StopFading(gameObject, false);
            EntityGOC component = gameObject.GetComponent<EntityGOC>();
            GameObjectFader.FadingGameObject.AnimationType intendedAnimationType = this.GetIntendedAnimationType(gameObject);
            int instanceID = gameObject.GetInstanceID();
            MaterialPropertyBlock materialPropertyBlock = MaterialPropertyBlockPool.Get();
            MaterialPropertyBlock materialPropertyBlock2 = MaterialPropertyBlockPool.Get();
            List<Renderer> list = Pool<List<Renderer>>.Get();
            list.Clear();
            list.AddRange(gameObject.GetRenderersInChildren(true));
            Transform transform = gameObject.transform;
            if (!flag)
            {
                if (intendedAnimationType == GameObjectFader.FadingGameObject.AnimationType.Opaque)
                {
                    foreach (Renderer renderer in gameObject.GetRenderersInChildren(true))
                    {
                        renderer.enabled = false;
                    }
                    using (List<Collider>.Enumerator enumerator2 = gameObject.GetCollidersInChildren(true).GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            Collider collider = enumerator2.Current;
                            collider.enabled = false;
                        }
                        goto IL_0136;
                    }
                }
                if (intendedAnimationType == GameObjectFader.FadingGameObject.AnimationType.Alpha)
                {
                    this.SetAlpha(list, 0f, materialPropertyBlock, materialPropertyBlock2);
                }
            }
            else if (intendedAnimationType == GameObjectFader.FadingGameObject.AnimationType.Alpha)
            {
                this.SetAlpha(list, 0f, materialPropertyBlock, materialPropertyBlock2);
            }
        IL_0136:
            bool flag3 = (component != null && (component.Entity.Spec == Get.Entity_Skybox || component.Entity.Spec == Get.Entity_TemporarilyOpenedDoor)) || gameObject.GetComponentInChildren<MeshRenderer>() == null || (Get.World.RecreatingGameObjectAfterLoad && (transform.position - Get.NowControlledActor.Position).sqrMagnitude > 26.009998f);
            bool flag4 = false;
            if (component != null && (component.Entity.Spec == Get.Entity_TemporarilyOpenedDoor || component.Entity.Spec == Get.Entity_Door))
            {
                flag4 = true;
            }
            this.fadingGameObjects.Add(new GameObjectFader.FadingGameObject
            {
                original = gameObject,
                cachedEntityGOC = component,
                cachedRenderers = list,
                fadingIn = true,
                duration = (flag3 ? 0.001f : ((intendedAnimationType == GameObjectFader.FadingGameObject.AnimationType.Opaque) ? GameObjectFader.DurationRange.RandomInRange : 1f)),
                startTime = Clock.Time + ((flag || intendedAnimationType != GameObjectFader.FadingGameObject.AnimationType.Opaque || flag3 || flag4 || !(component is StructureGOC)) ? 0f : Rand.Range(0f, 0.4f)),
                animationType = intendedAnimationType,
                pct = num,
                randSeed = instanceID,
                basePos = (flag ? fadingGameObject.basePos : transform.position),
                baseRot = (flag ? fadingGameObject.baseRot : transform.rotation),
                baseScale = (flag ? fadingGameObject.baseScale : transform.localScale),
                propertyBlock1 = materialPropertyBlock,
                propertyBlock2 = materialPropertyBlock2
            });
            this.fadingGameObjects_fadingIn.Add(gameObject.GetInstanceID());
            if (!flag && intendedAnimationType == GameObjectFader.FadingGameObject.AnimationType.ManualScale && component != null)
            {
                component.SetScaleToDesiredInstantly();
            }
            this.UpdateFadingGameObjectTransformAndAlpha(this.fadingGameObjects[this.fadingGameObjects.Count - 1], this.GetFadePct(this.fadingGameObjects.Count - 1));
        }

        public void FadeOut(GameObject gameObject)
        {
            if (this.IsFadingOut(gameObject))
            {
                return;
            }
            if (!this.EnsureNotCombinedMesh(gameObject))
            {
                return;
            }
            bool flag = this.IsFadingIn(gameObject);
            bool flag2;
            float num = (flag ? Calc.InverseSmooth(1f - Calc.Smooth(this.GetFadePct(gameObject, out flag2))) : 0f);
            GameObjectFader.FadingGameObject fadingGameObject = this.StopFading(gameObject, false);
            EntityGOC component = gameObject.GetComponent<EntityGOC>();
            ActorGOC actorGOC = component as ActorGOC;
            if (actorGOC != null)
            {
                actorGOC.OnPreFadeOut();
            }
            GameObjectFader.FadingGameObject.AnimationType intendedAnimationType = this.GetIntendedAnimationType(gameObject);
            bool flag3 = !gameObject.AnyRendererInCameraFrustumOrCombined();
            GameObject gameObject2;
            if (flag3)
            {
                gameObject2 = null;
            }
            else
            {
                Profiler.Begin("MakeFadedCopy()");
                try
                {
                    gameObject2 = this.MakeFadedCopy(gameObject);
                }
                finally
                {
                    Profiler.End();
                }
            }
            List<Renderer> list = Pool<List<Renderer>>.Get();
            list.Clear();
            if (gameObject2 != null)
            {
                list.AddRange(gameObject2.GetRenderersInChildren(true));
            }
            List<GameObjectFader.FadingGameObject> list2 = this.fadingGameObjects;
            GameObjectFader.FadingGameObject fadingGameObject2 = default(GameObjectFader.FadingGameObject);
            fadingGameObject2.original = gameObject;
            fadingGameObject2.cachedEntityGOC = component;
            fadingGameObject2.cachedRenderers = list;
            fadingGameObject2.fadedCopy = gameObject2;
            fadingGameObject2.fadingIn = false;
            fadingGameObject2.duration = (flag3 ? 0.001f : ((intendedAnimationType == GameObjectFader.FadingGameObject.AnimationType.Opaque) ? GameObjectFader.DurationRange.RandomInRange : 1f));
            float time = Clock.Time;
            float num2;
            if (!flag && intendedAnimationType == GameObjectFader.FadingGameObject.AnimationType.Opaque && !Get.ScreenFader.AnyActionQueued)
            {
                StructureGOC structureGOC = component as StructureGOC;
                if (structureGOC != null && structureGOC.Structure.Spec.Structure.IsFilled)
                {
                    num2 = Rand.Range(0f, 0.4f);
                    goto IL_017B;
                }
            }
            num2 = 0f;
        IL_017B:
            fadingGameObject2.startTime = time + num2;
            fadingGameObject2.animationType = intendedAnimationType;
            fadingGameObject2.pct = num;
            fadingGameObject2.randSeed = gameObject.GetInstanceID();
            fadingGameObject2.basePos = (flag ? fadingGameObject.basePos : gameObject.transform.position);
            fadingGameObject2.baseRot = (flag ? fadingGameObject.baseRot : gameObject.transform.rotation);
            fadingGameObject2.baseScale = (flag ? fadingGameObject.baseScale : gameObject.transform.localScale);
            fadingGameObject2.propertyBlock1 = MaterialPropertyBlockPool.Get();
            fadingGameObject2.propertyBlock2 = MaterialPropertyBlockPool.Get();
            list2.Add(fadingGameObject2);
            this.fadingGameObjects_fadingOut.Add(gameObject.GetInstanceID());
            if (gameObject2 != null)
            {
                this.fadingGameObjects_fadingOut.Add(gameObject2.GetInstanceID());
            }
            this.UpdateFadingGameObjectTransformAndAlpha(this.fadingGameObjects[this.fadingGameObjects.Count - 1], this.GetFadePct(this.fadingGameObjects.Count - 1));
        }

        public GameObjectFader.FadingGameObject StopFading(GameObject gameObject, bool success)
        {
            if (!this.IsFading(gameObject))
            {
                return default(GameObjectFader.FadingGameObject);
            }
            int num = -1;
            for (int i = 0; i < this.fadingGameObjects.Count; i++)
            {
                if (this.fadingGameObjects[i].original == gameObject || this.fadingGameObjects[i].fadedCopy == gameObject)
                {
                    num = i;
                    break;
                }
            }
            if (num < 0)
            {
                return default(GameObjectFader.FadingGameObject);
            }
            GameObjectFader.FadingGameObject fadingGameObject = this.fadingGameObjects[num];
            if (fadingGameObject.animationType == GameObjectFader.FadingGameObject.AnimationType.Alpha)
            {
                List<Renderer> renderersInChildren = gameObject.GetRenderersInChildren(true);
                for (int j = 0; j < renderersInChildren.Count; j++)
                {
                    renderersInChildren[j].SetPropertyBlock(null);
                }
            }
            EntityGOC entityGOC = null;
            if (fadingGameObject.fadingIn)
            {
                entityGOC = fadingGameObject.cachedEntityGOC;
            }
            if (fadingGameObject.original != null)
            {
                this.fadingGameObjects_fadingIn.Remove(fadingGameObject.original.GetInstanceID());
                this.fadingGameObjects_fadingOut.Remove(fadingGameObject.original.GetInstanceID());
            }
            if (fadingGameObject.fadedCopy != null)
            {
                this.fadingGameObjects_fadingIn.Remove(fadingGameObject.fadedCopy.GetInstanceID());
                this.fadingGameObjects_fadingOut.Remove(fadingGameObject.fadedCopy.GetInstanceID());
                GameObjectUtility.Destroy(fadingGameObject.fadedCopy);
            }
            fadingGameObject.cachedRenderers.Clear();
            Pool<List<Renderer>>.Return(fadingGameObject.cachedRenderers);
            MaterialPropertyBlockPool.Return(fadingGameObject.propertyBlock1);
            MaterialPropertyBlockPool.Return(fadingGameObject.propertyBlock2);
            this.fadingGameObjects.RemoveAt(num);
            if (entityGOC != null)
            {
                if (success)
                {
                    entityGOC.OnFadeInComplete();
                }
                else
                {
                    entityGOC.OnFadeInInterrupted();
                }
                Get.TiledDecals.OnEntityNoLongerFadingIn(entityGOC.Entity);
            }
            return fadingGameObject;
        }

        private bool EnsureNotCombinedMesh(GameObject gameObject)
        {
            EntityGOC entityGOC;
            if (gameObject.TryGetComponent<EntityGOC>(out entityGOC) && entityGOC.IsMeshCombined)
            {
                Log.Error("Tried to fade in or out a game object which has its mesh combined currently. This would make this game object render twice (once combined, and once on its own, since GameObjectFader enables renderer).", false);
                return false;
            }
            return true;
        }

        private float GetFadePct(GameObject gameObject, out bool fadingIn)
        {
            if (gameObject == null)
            {
                fadingIn = false;
                return 0f;
            }
            for (int i = 0; i < this.fadingGameObjects.Count; i++)
            {
                if (this.fadingGameObjects[i].original == gameObject || this.fadingGameObjects[i].fadedCopy == gameObject)
                {
                    fadingIn = this.fadingGameObjects[i].fadingIn;
                    return this.GetFadePct(i);
                }
            }
            fadingIn = false;
            return 0f;
        }

        private float GetFadePct(int index)
        {
            return this.fadingGameObjects[index].pct;
        }

        public void Update()
        {
            for (int i = this.fadingGameObjects.Count - 1; i >= 0; i--)
            {
                GameObjectFader.FadingGameObject fadingGameObject = this.fadingGameObjects[i];
                if (Clock.Time > fadingGameObject.startTime)
                {
                    GameObjectFader.FadingGameObject fadingGameObject2 = fadingGameObject;
                    fadingGameObject2.pct = Math.Min(fadingGameObject2.pct + Clock.DeltaTime / fadingGameObject.duration, 1f);
                    this.fadingGameObjects[i] = fadingGameObject2;
                }
                float fadePct = this.GetFadePct(i);
                this.UpdateFadingGameObjectTransformAndAlpha(fadingGameObject, fadePct);
                if (fadePct >= 1f)
                {
                    if (fadingGameObject.original != null)
                    {
                        this.StopFading(fadingGameObject.original, true);
                    }
                    else if (fadingGameObject.fadedCopy != null)
                    {
                        this.StopFading(fadingGameObject.fadedCopy, true);
                    }
                    else
                    {
                        fadingGameObject.cachedRenderers.Clear();
                        Pool<List<Renderer>>.Return(fadingGameObject.cachedRenderers);
                        MaterialPropertyBlockPool.Return(fadingGameObject.propertyBlock1);
                        MaterialPropertyBlockPool.Return(fadingGameObject.propertyBlock2);
                        this.fadingGameObjects.RemoveAt(i);
                    }
                }
            }
            if (this.fadingGameObjects.Count == 0)
            {
                this.fadingGameObjects_fadingIn.Clear();
                this.fadingGameObjects_fadingOut.Clear();
            }
        }

        private void UpdateFadingGameObjectTransformAndAlpha(GameObjectFader.FadingGameObject fading, float pct)
        {
            pct = this.GetSmoothedPct(pct, fading.fadingIn);
            if (pct > 0f)
            {
                if (fading.fadingIn)
                {
                    if (!(fading.original != null))
                    {
                        goto IL_00C4;
                    }
                    foreach (Renderer renderer in fading.original.GetRenderersInChildren(true))
                    {
                        renderer.enabled = true;
                    }
                    using (List<Collider>.Enumerator enumerator2 = fading.original.GetCollidersInChildren(true).GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            Collider collider = enumerator2.Current;
                            collider.enabled = true;
                        }
                        goto IL_00C4;
                    }
                }
                if (fading.fadedCopy != null)
                {
                    fading.fadedCopy.SetActive(true);
                }
            }
        IL_00C4:
            if (fading.animationType == GameObjectFader.FadingGameObject.AnimationType.Alpha)
            {
                if (!fading.fadingIn)
                {
                    if (fading.original != null && fading.fadedCopy != null)
                    {
                        Transform transform = fading.fadedCopy.transform;
                        Transform transform2 = fading.original.transform;
                        transform.position = transform2.position;
                        transform.rotation = transform2.rotation;
                        transform.localScale = transform2.localScale;
                    }
                    this.SetAlpha(fading.cachedRenderers, pct, fading.propertyBlock1, fading.propertyBlock2);
                    return;
                }
                if (fading.original != null)
                {
                    this.SetAlpha(fading.cachedRenderers, pct, fading.propertyBlock1, fading.propertyBlock2);
                    return;
                }
            }
            else if (fading.animationType == GameObjectFader.FadingGameObject.AnimationType.ManualScale)
            {
                if (!fading.fadingIn)
                {
                    if (fading.original != null && fading.fadedCopy != null)
                    {
                        Transform transform3 = fading.fadedCopy.transform;
                        Transform transform4 = fading.original.transform;
                        transform3.position = transform4.position;
                        transform3.rotation = transform4.rotation;
                        transform3.localScale = transform4.localScale;
                        return;
                    }
                    if (fading.fadedCopy != null)
                    {
                        fading.fadedCopy.transform.localScale = fading.baseScale * Math.Max(pct, 0.01f);
                        return;
                    }
                }
            }
            else if (fading.animationType == GameObjectFader.FadingGameObject.AnimationType.Opaque)
            {
                if (fading.fadingIn)
                {
                    if (fading.original != null)
                    {
                        this.UpdateTransform(fading.original, fading.basePos, fading.baseRot, fading.baseScale, fading.randSeed, pct, fading.fadingIn);
                        return;
                    }
                }
                else if (fading.fadedCopy != null)
                {
                    this.UpdateTransform(fading.fadedCopy, fading.basePos, fading.baseRot, fading.baseScale, fading.randSeed, pct, fading.fadingIn);
                }
            }
        }

        private float GetSmoothedPct(float pct, bool fadingIn)
        {
            pct = Calc.Smooth(pct);
            if (!fadingIn)
            {
                pct = 1f - pct;
            }
            return pct;
        }

        private GameObject MakeFadedCopy(GameObject gameObject)
        {
            if (gameObject == null)
            {
                Log.Error("Tried to make a faded copy of a null game object.", false);
                return null;
            }
            GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, Get.RuntimeSpecialContainer.transform);
            gameObject2.SetActive(true);
            EntityGOC entityGOC;
            if (gameObject2.TryGetComponent<EntityGOC>(out entityGOC))
            {
                Object.DestroyImmediate(entityGOC);
            }
            GameObjectFader.tmpComponents.Clear();
            gameObject2.GetComponentsInChildren<Component>(GameObjectFader.tmpComponents);
            for (int i = 0; i < GameObjectFader.tmpComponents.Count; i++)
            {
                Component component = GameObjectFader.tmpComponents[i];
                if (component is Light || component is CullParticlesGOC || component is WaterfallGOC || component is RainSplashGOC || component is BossTrophyPortraitGOC || component is RetractableAnimationGOC || component is PistonAnimationGOC)
                {
                    Object.Destroy(component);
                }
                else
                {
                    ParticleSystem particleSystem = component as ParticleSystem;
                    if (particleSystem != null)
                    {
                        if (particleSystem.gameObject.activeInHierarchy)
                        {
                            particleSystem.Simulate(2f);
                            particleSystem.Play();
                        }
                    }
                    else
                    {
                        StackCountNumberGOC stackCountNumberGOC = component as StackCountNumberGOC;
                        if (stackCountNumberGOC != null && entityGOC != null)
                        {
                            Item item = entityGOC.Entity as Item;
                            if (item != null)
                            {
                                stackCountNumberGOC.SetSpecificStackCount(item.StackCount);
                            }
                        }
                    }
                }
            }
            GameObjectFader.tmpComponents.Clear();
            Get.ParticleSystemFinisher.DetachAndFinishParticles(gameObject2);
            foreach (Renderer renderer in gameObject2.GetRenderersInChildren(true))
            {
                renderer.enabled = true;
            }
            foreach (Collider collider in gameObject2.GetCollidersInChildren(true))
            {
                collider.enabled = true;
            }
            return gameObject2;
        }

        private GameObjectFader.FadingGameObject.AnimationType GetIntendedAnimationType(GameObject gameObject)
        {
            EntityGOC componentInChildren = gameObject.GetComponentInChildren<EntityGOC>();
            if (componentInChildren == null)
            {
                return GameObjectFader.FadingGameObject.AnimationType.Opaque;
            }
            return this.GetIntendedAnimationType(componentInChildren.Entity);
        }

        private GameObjectFader.FadingGameObject.AnimationType GetIntendedAnimationType(Entity entity)
        {
            if (entity is Actor)
            {
                return GameObjectFader.FadingGameObject.AnimationType.ManualScale;
            }
            if (entity is Item)
            {
                return GameObjectFader.FadingGameObject.AnimationType.ManualScale;
            }
            return GameObjectFader.FadingGameObject.AnimationType.Opaque;
        }

        public bool IsTransformCurrentlyHandledByFader(Entity entity)
        {
            return this.GetIntendedAnimationType(entity) == GameObjectFader.FadingGameObject.AnimationType.Opaque && this.IsFadingIn(entity.GameObject);
        }

        private void SetAlpha(List<Renderer> renderers, float alpha, MaterialPropertyBlock propertyBlock1, MaterialPropertyBlock propertyBlock2)
        {
            if (renderers == null || renderers.Count == 0)
            {
                return;
            }
            for (int i = 0; i < renderers.Count; i++)
            {
                if (!(renderers[i] == null) && !(renderers[i] is ParticleSystemRenderer))
                {
                    MaterialPropertyBlock materialPropertyBlock;
                    if (i == 0)
                    {
                        materialPropertyBlock = propertyBlock1;
                    }
                    else if (i == 1)
                    {
                        materialPropertyBlock = propertyBlock2;
                    }
                    else
                    {
                        materialPropertyBlock = null;
                        for (int j = 0; j < this.temporaryPropertyBlocks.Count; j++)
                        {
                            if (Clock.Frame - this.temporaryPropertyBlocks[j].Item2 > 5)
                            {
                                materialPropertyBlock = this.temporaryPropertyBlocks[j].Item1;
                                this.temporaryPropertyBlocks[j] = new ValueTuple<MaterialPropertyBlock, int>(materialPropertyBlock, Clock.Frame);
                                break;
                            }
                        }
                        if (materialPropertyBlock == null)
                        {
                            materialPropertyBlock = new MaterialPropertyBlock();
                            this.temporaryPropertyBlocks.Add(new ValueTuple<MaterialPropertyBlock, int>(materialPropertyBlock, Clock.Frame));
                        }
                    }
                    Color color = new Color(1f, 1f, 1f, alpha);
                    bool flag = false;
                    if (renderers[i].sharedMaterial != null)
                    {
                        if (renderers[i].sharedMaterial.HasProperty(Get.ShaderPropertyIDs.ColorID))
                        {
                            color *= renderers[i].sharedMaterial.color;
                        }
                        else if (renderers[i].sharedMaterial.HasProperty(Get.ShaderPropertyIDs.FaceColorID))
                        {
                            color *= renderers[i].sharedMaterial.GetColor(Get.ShaderPropertyIDs.FaceColorID);
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        materialPropertyBlock.SetColor(Get.ShaderPropertyIDs.FaceColorID, color);
                        if (renderers[i].sharedMaterial.HasProperty(Get.ShaderPropertyIDs.OutlineColorID))
                        {
                            materialPropertyBlock.SetColor(Get.ShaderPropertyIDs.OutlineColorID, new Color(0f, 0f, 0f, alpha));
                        }
                    }
                    else
                    {
                        materialPropertyBlock.SetColor(Get.ShaderPropertyIDs.ColorID, color);
                    }
                    renderers[i].SetPropertyBlock(materialPropertyBlock);
                }
            }
        }

        public float GetFilledEntitySmoothedPctAt(Vector3Int pos, out Vector3 basePos, out Quaternion baseRot, out Vector3 baseScale, out int randSeed, out bool fadingIn)
        {
            for (int i = 0; i < this.fadingGameObjects.Count; i++)
            {
                EntityGOC cachedEntityGOC = this.fadingGameObjects[i].cachedEntityGOC;
                if (cachedEntityGOC != null && cachedEntityGOC.Entity.Position == pos && cachedEntityGOC.Entity.Spec.IsStructure && cachedEntityGOC.Entity.Spec.Structure.IsFilled)
                {
                    basePos = this.fadingGameObjects[i].basePos;
                    baseRot = this.fadingGameObjects[i].baseRot;
                    baseScale = this.fadingGameObjects[i].baseScale;
                    randSeed = this.fadingGameObjects[i].randSeed;
                    fadingIn = this.fadingGameObjects[i].fadingIn;
                    return this.GetSmoothedPct(this.fadingGameObjects[i].pct, this.fadingGameObjects[i].fadingIn);
                }
            }
            basePos = pos;
            baseRot = Quaternion.identity;
            baseScale = Vector3.one;
            randSeed = 0;
            fadingIn = false;
            return 1f;
        }

        public float GetSmoothedFadePct(Entity entity)
        {
            if (!this.IsFading(entity.GameObject))
            {
                return 1f;
            }
            bool flag;
            float fadePct = this.GetFadePct(entity.GameObject, out flag);
            return this.GetSmoothedPct(fadePct, flag);
        }

        private void UpdateTransform(GameObject gameObject, Vector3 basePos, Quaternion baseRot, Vector3 baseScale, int randSeed, float pct, bool fadeIn)
        {
            gameObject.transform.position = GameObjectFader.GetAnimatedPos(basePos, pct, randSeed);
            gameObject.transform.rotation = GameObjectFader.GetAnimatedRot(baseRot, pct, randSeed);
            gameObject.transform.localScale = GameObjectFader.GetAnimatedScale(baseScale, pct, randSeed, fadeIn);
        }

        private static Vector3 GetAnimatedPos(Vector3 basePos, float pct, int randSeed)
        {
            if (pct >= 1f)
            {
                return basePos;
            }
            Vector3 vector;
            if (Calc.RoundToInt(basePos.y) < Get.NowControlledActor.Position.y)
            {
                vector = Vector3.down;
            }
            else
            {
                vector = Get.CameraTransform.forward;
            }
            return basePos + vector * 0.35f * (1f - pct);
        }

        private static Quaternion GetAnimatedRot(Quaternion baseRot, float pct, int randSeed)
        {
            if (pct >= 1f)
            {
                return baseRot;
            }
            Vector3 eulerAngles = baseRot.eulerAngles;
            Rand.PushState(randSeed);
            Quaternion quaternion = Quaternion.Euler(eulerAngles.x + Rand.Range(-7f, 7f), eulerAngles.y + Rand.Range(-7f, 7f), eulerAngles.z + Rand.Range(-7f, 7f));
            Rand.PopState();
            return Quaternion.Lerp(quaternion, baseRot, pct);
        }

        private static Vector3 GetAnimatedScale(Vector3 baseScale, float pct, int randSeed, bool fadingIn)
        {
            if (pct >= 1f)
            {
                return baseScale;
            }
            if (fadingIn)
            {
                return baseScale * Calc.Lerp(0.7f, 1f, pct);
            }
            return baseScale * Calc.Lerp(0f, 1f, Calc.Pow(pct, 0.4761905f));
        }

        public static Matrix4x4 GetAnimatedRelativeMatrix(Vector3 basePos, Quaternion baseRot, Vector3 baseScale, float pct, int randSeed, bool fadingIn)
        {
            Matrix4x4 matrix4x = Matrix4x4.TRS(basePos, baseRot, baseScale);
            Matrix4x4 matrix4x2 = Matrix4x4.TRS(GameObjectFader.GetAnimatedPos(basePos, pct, randSeed), GameObjectFader.GetAnimatedRot(baseRot, pct, randSeed), GameObjectFader.GetAnimatedScale(baseScale, pct, randSeed, fadingIn));
            return Matrix4x4.Inverse(matrix4x) * matrix4x2;
        }

        private List<GameObjectFader.FadingGameObject> fadingGameObjects = new List<GameObjectFader.FadingGameObject>(100);

        private HashSet<int> fadingGameObjects_fadingIn = new HashSet<int>();

        private HashSet<int> fadingGameObjects_fadingOut = new HashSet<int>();

        private List<ValueTuple<Entity, List<Renderer>>> tmpFadingEntities = new List<ValueTuple<Entity, List<Renderer>>>(100);

        private List<ValueTuple<MaterialPropertyBlock, int>> temporaryPropertyBlocks = new List<ValueTuple<MaterialPropertyBlock, int>>();

        private static readonly FloatRange DurationRange = new FloatRange(0.8f, 1f);

        private const float DurationNonOpaque = 1f;

        private const float MaxRandomStartTimeOffset = 0.4f;

        private static List<Component> tmpComponents = new List<Component>();

        public struct FadingGameObject
        {
            public GameObject original;

            public GameObject fadedCopy;

            public EntityGOC cachedEntityGOC;

            public List<Renderer> cachedRenderers;

            public bool fadingIn;

            public float duration;

            public float startTime;

            public GameObjectFader.FadingGameObject.AnimationType animationType;

            public float pct;

            public int randSeed;

            public Vector3 basePos;

            public Quaternion baseRot;

            public Vector3 baseScale;

            public MaterialPropertyBlock propertyBlock1;

            public MaterialPropertyBlock propertyBlock2;

            public enum AnimationType
            {
                Alpha,

                Opaque,

                ManualScale
            }
        }
    }
}