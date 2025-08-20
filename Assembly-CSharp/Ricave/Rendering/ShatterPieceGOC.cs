using System;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class ShatterPieceGOC : MonoBehaviour
    {
        public void OnReturnedToPool()
        {
            this.splatColor = null;
            this.splatCreated = false;
            this.createdAtSequence = -1;
        }

        private void OnCollisionEnter(Collision collision)
        {
            this.TryPlaySound(collision);
            this.TryCreateSplat(collision);
        }

        private void TryPlaySound(Collision collision)
        {
            if (Clock.UnscaledTime - ShatterPieceGOC.lastSoundPlayTime < 0.05f)
            {
                return;
            }
            float magnitude = collision.relativeVelocity.magnitude;
            if (magnitude < 1f || collision.contactCount <= 0)
            {
                return;
            }
            ShatterPieceGOC shatterPieceGOC;
            if (collision.collider.TryGetComponent<ShatterPieceGOC>(out shatterPieceGOC))
            {
                return;
            }
            Vector3 vector = Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                vector += collision.GetContact(i).point;
            }
            vector /= (float)collision.contactCount;
            ShatterPieceGOC.lastSoundPlayTime = Clock.UnscaledTime + Rand.Range(-0.02f, 0.02f);
            Rand.Element<SoundSpec>(Get.Sound_PieceHit1, Get.Sound_PieceHit2, Get.Sound_PieceHit3, Get.Sound_PieceHit4, Get.Sound_PieceHit5).PlayOneShot(new Vector3?(vector), Calc.Pow(Math.Min(magnitude / 8f, 1f), 2f), 1f);
        }

        private void TryCreateSplat(Collision collision)
        {
            if (this.splatColor == null)
            {
                return;
            }
            if (this.splatCreated)
            {
                return;
            }
            float magnitude = collision.relativeVelocity.magnitude;
            if (magnitude < 1f || collision.contactCount <= 0)
            {
                return;
            }
            EntityGOC componentInParent = collision.collider.GetComponentInParent<EntityGOC>();
            if (!(componentInParent == null))
            {
                Structure structure = componentInParent.Entity as Structure;
                if (structure != null)
                {
                    if (!structure.Spec.Structure.IsFilled)
                    {
                        return;
                    }
                    if (structure.Spec == Get.Entity_Piston || structure.Spec == Get.Entity_BrickWall || structure.Spec == Get.Entity_Dirt)
                    {
                        return;
                    }
                    ContactPoint contact = collision.GetContact(0);
                    Vector3 point = contact.point;
                    Vector3 normal = contact.normal;
                    if (((Mathf.Abs(point.x - ((float)structure.Position.x - 0.5f)) < 0.1f) ? 1 : 0) + ((Mathf.Abs(point.y - ((float)structure.Position.y - 0.5f)) < 0.1f) ? 1 : 0) + ((Mathf.Abs(point.z - ((float)structure.Position.z - 0.5f)) < 0.1f) ? 1 : 0) + ((Mathf.Abs(point.x - ((float)structure.Position.x + 0.5f)) < 0.1f) ? 1 : 0) + ((Mathf.Abs(point.y - ((float)structure.Position.y + 0.5f)) < 0.1f) ? 1 : 0) + ((Mathf.Abs(point.z - ((float)structure.Position.z + 0.5f)) < 0.1f) ? 1 : 0) >= 2)
                    {
                        return;
                    }
                    SectionRendererGOC sectionRenderer = Get.SectionsManager.GetSectionRenderer(structure.Position);
                    if (sectionRenderer == null)
                    {
                        return;
                    }
                    float num = Calc.Lerp(0.014f, 0.07f, Calc.Clamp01((magnitude - 1f) / 6f));
                    sectionRenderer.SplatsManager.AddSplat(point + normal * 0.0011f, normal, this.splatColor.Value.WithAlphaFactor(Rand.Range(0.9f, 1f)), num * Rand.Range(0.65f, 1.35f), structure.Position, this.createdAtSequence);
                    this.splatCreated = true;
                    return;
                }
            }
        }

        public void InitSplatColor(Color color)
        {
            this.splatColor = new Color?(color.WithAlpha(0.27f));
            this.createdAtSequence = Get.TurnManager.CurrentSequence;
        }

        private static float lastSoundPlayTime = -99999f;

        private Color? splatColor;

        private bool splatCreated;

        private int createdAtSequence = -1;
    }
}