using System;
using System.Collections;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class FakeActorGOC : MonoBehaviour
    {
        private void Start()
        {
            if (!this.actorSpec.NullOrEmpty())
            {
                this.actorSpecResolved = Get.Specs.Get<EntitySpec>(this.actorSpec);
            }
            else
            {
                StructureGOC componentInParent = base.GetComponentInParent<StructureGOC>();
                if (componentInParent != null)
                {
                    this.actorSpecResolved = null;
                    using (List<Entity>.Enumerator enumerator = componentInParent.Structure.InnerEntities.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Entity entity = enumerator.Current;
                            Actor actor = entity as Actor;
                            if (actor != null)
                            {
                                this.actorSpecResolved = actor.Spec;
                                break;
                            }
                        }
                        goto IL_008D;
                    }
                }
                this.actorSpecResolved = null;
            }
        IL_008D:
            this.CopyActorRendererData();
            this.initialLocalPosition = base.transform.localPosition;
        }

        private void CopyActorRendererData()
        {
            MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
            MeshFilter component2 = base.gameObject.GetComponent<MeshFilter>();
            if (this.actorSpecResolved != null)
            {
                if (this.actorSpecResolved.Actor.Texture != null)
                {
                    MeshRenderer component3 = this.actorSpecResolved.PrefabAdjusted.GetComponent<MeshRenderer>();
                    MeshFilter component4 = this.actorSpecResolved.PrefabAdjusted.GetComponent<MeshFilter>();
                    component.sharedMaterial = component3.sharedMaterial;
                    component2.sharedMesh = component4.sharedMesh;
                    using (IEnumerator enumerator = base.transform.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            object obj = enumerator.Current;
                            Object.Destroy(((Transform)obj).gameObject);
                        }
                        return;
                    }
                }
                component.sharedMaterial = null;
                component2.sharedMesh = null;
                GameObject gameObject = Object.Instantiate<GameObject>(this.actorSpecResolved.PrefabAdjusted, base.transform);
                EntityGOC entityGOC;
                if (gameObject.TryGetComponent<EntityGOC>(out entityGOC))
                {
                    Object.DestroyImmediate(entityGOC);
                }
                foreach (Component component5 in gameObject.GetComponentsInChildren<Component>())
                {
                    if (!(component5 is Transform) && !(component5 is MeshRenderer) && !(component5 is MeshFilter))
                    {
                        Object.Destroy(component5);
                    }
                }
                return;
            }
            component.sharedMaterial = null;
            component2.sharedMesh = null;
            foreach (object obj2 in base.transform)
            {
                Object.Destroy(((Transform)obj2).gameObject);
            }
        }

        private void LateUpdate()
        {
            this.LookAtPlayer();
        }

        private void FixedUpdate()
        {
            if (this.actorSpecResolved == null)
            {
                return;
            }
            Vector3 vector = this.initialLocalPosition + Vector3.up * DanceAnimationUtility.GetPosOffset(0, 1f);
            Transform transform = base.transform;
            transform.localPosition = Vector3.Lerp(transform.localPosition, vector, 0.13f);
        }

        private void LookAtPlayer()
        {
            if (this.actorSpecResolved == null)
            {
                return;
            }
            Transform transform = base.transform;
            float num;
            float num2;
            ActorGOC.GetLookAtPlayerRot(transform.position, transform.localScale, out num, out num2);
            Quaternion quaternion = Quaternion.Euler(num2, num, DanceAnimationUtility.GetRotation(0, 1f));
            transform.rotation = quaternion;
        }

        [SerializeField]
        private string actorSpec;

        private Vector3 initialLocalPosition;

        private EntitySpec actorSpecResolved;
    }
}