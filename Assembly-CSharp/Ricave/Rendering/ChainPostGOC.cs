using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class ChainPostGOC : MonoBehaviour
    {
        private Vector3? AttachedActorPos
        {
            get
            {
                if (this.entityGOC != null)
                {
                    ChainPost chainPost = this.entityGOC.Entity as ChainPost;
                    if (chainPost != null && chainPost.AttachedActor != null)
                    {
                        return new Vector3?(chainPost.AttachedActor.RenderPositionComputedCenter);
                    }
                }
                return null;
            }
        }

        private void Start()
        {
            this.CheckInit();
            this.SetChainLinePos();
        }

        private void OnEnable()
        {
            this.CheckInit();
            this.SetChainLinePos();
        }

        private void Update()
        {
            this.SetChainLinePos();
        }

        private void CheckInit()
        {
            if (this.entityGOC != null)
            {
                return;
            }
            this.entityGOC = base.GetComponentInParent<EntityGOC>();
            this.lineRenderer = base.GetComponent<LineRenderer>();
            LineRenderer lineRenderer = this.lineRenderer;
            this.initialLineStartOffset = ((lineRenderer != null) ? lineRenderer.GetPosition(0) : Vector3.zero);
            this.lineColliderRotationAnchor = base.transform.GetChild(0).gameObject;
            this.lineColliderComponent = base.GetComponentInChildren<BoxCollider>();
            this.lineCollider = this.lineColliderComponent.gameObject;
        }

        private void SetChainLinePos()
        {
            Vector3? attachedActorPos = this.AttachedActorPos;
            Vector3 vector = base.transform.position + this.initialLineStartOffset;
            if (attachedActorPos == null)
            {
                this.lineRenderer.enabled = false;
                this.lineRenderer.SetPosition(0, vector);
                this.lineRenderer.SetPosition(1, this.lineRenderer.GetPosition(0));
                this.lineColliderComponent.enabled = false;
                return;
            }
            this.lineRenderer.enabled = true;
            this.lineRenderer.SetPosition(0, vector);
            this.lineRenderer.SetPosition(1, attachedActorPos.Value);
            Vector3 vector2 = attachedActorPos.Value - vector;
            this.lineColliderComponent.enabled = true;
            this.lineColliderRotationAnchor.transform.forward = vector2.normalized;
            this.lineCollider.transform.localScale = new Vector3(this.lineCollider.transform.localScale.x, this.lineCollider.transform.localScale.y, vector2.magnitude);
            this.lineCollider.transform.localPosition = new Vector3(this.lineCollider.transform.localPosition.x, this.lineCollider.transform.localPosition.y, vector2.magnitude / 2f);
        }

        private EntityGOC entityGOC;

        private LineRenderer lineRenderer;

        private Vector3 initialLineStartOffset;

        private GameObject lineColliderRotationAnchor;

        private GameObject lineCollider;

        private BoxCollider lineColliderComponent;
    }
}