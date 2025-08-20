using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class ParticleSystemFinisher
    {
        public void DetachAndFinishParticles(GameObject gameObject)
        {
            if (gameObject == null || !gameObject.activeInHierarchy)
            {
                return;
            }
            if (gameObject.GetComponentInChildren<ParticleSystem>() == null)
            {
                return;
            }
            this.tmpParticleSystems.Clear();
            this.tmpParticleSystemsRaw.Clear();
            gameObject.GetComponentsInChildren<ParticleSystem>(this.tmpParticleSystemsRaw);
            foreach (ParticleSystem particleSystem in this.tmpParticleSystemsRaw)
            {
                if (particleSystem.isPlaying && particleSystem.gameObject.activeInHierarchy)
                {
                    if (particleSystem.gameObject == gameObject)
                    {
                        Log.Error("Tried to finish particle systems but one particle system is attached to the root's game object. This is not supported. It should be a child, so we could detach it.", false);
                    }
                    else
                    {
                        this.tmpParticleSystems.Add(particleSystem);
                    }
                }
            }
            this.tmpParticleSystemsRaw.Clear();
            for (int i = 0; i < this.tmpParticleSystems.Count; i++)
            {
                ParticleSystem particleSystem2 = this.tmpParticleSystems[i];
                Vector3 localScale = particleSystem2.transform.localScale;
                particleSystem2.transform.SetParent(Get.RuntimeSpecialContainer.transform, true);
                particleSystem2.transform.localScale = localScale;
            }
            for (int j = 0; j < this.tmpParticleSystems.Count; j++)
            {
                ParticleSystem particleSystem3 = this.tmpParticleSystems[j];
                this.tmpChildren.Clear();
                foreach (object obj in particleSystem3.gameObject.transform)
                {
                    Transform transform = (Transform)obj;
                    this.tmpChildren.Add(transform);
                }
                for (int k = 0; k < this.tmpChildren.Count; k++)
                {
                    GameObjectUtility.DestroyImmediate(this.tmpChildren[k].gameObject);
                }
                this.tmpComponents.Clear();
                particleSystem3.gameObject.GetComponents<Component>(this.tmpComponents);
                for (int l = 0; l < this.tmpComponents.Count; l++)
                {
                    Component component = this.tmpComponents[l];
                    if (!(component is Transform) && !(component is ParticleSystem) && !(component is ParticleSystemForceField) && !(component is ParticleSystemRenderer))
                    {
                        Object.DestroyImmediate(component);
                    }
                }
                particleSystem3.Stop();
                this.finishing.Add(particleSystem3);
            }
        }

        public void Update()
        {
            for (int i = this.finishing.Count - 1; i >= 0; i--)
            {
                ParticleSystem particleSystem = this.finishing[i];
                if (particleSystem.gameObject == null)
                {
                    this.finishing.RemoveAt(i);
                }
                else
                {
                    if (particleSystem.isEmitting)
                    {
                        particleSystem.Stop();
                    }
                    if (!particleSystem.isPlaying)
                    {
                        GameObjectUtility.Destroy(particleSystem.gameObject);
                        this.finishing.RemoveAt(i);
                    }
                }
            }
        }

        private List<ParticleSystem> finishing = new List<ParticleSystem>();

        private List<ParticleSystem> tmpParticleSystems = new List<ParticleSystem>();

        private List<ParticleSystem> tmpParticleSystemsRaw = new List<ParticleSystem>();

        private List<Transform> tmpChildren = new List<Transform>();

        private List<Component> tmpComponents = new List<Component>();
    }
}