using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class EyesGOC : MonoBehaviour
    {
        private void Start()
        {
            this.UpdateEyes();
        }

        private void OnEnable()
        {
            this.UpdateEyes();
        }

        private void Update()
        {
            this.UpdateEyes();
        }

        private void UpdateEyes()
        {
            if (!this.CheckInit())
            {
                return;
            }
            this.eyesRenderer.sharedMaterial = this.GetCurrentEyesMaterial();
        }

        private Material GetCurrentEyesMaterial()
        {
            if (this.actor.Actor.HP <= 0)
            {
                return EyesGOC.Eyes_Dead;
            }
            bool flag = Get.NowControlledActor != null && this.actor.Actor.IsHostile(Get.NowControlledActor);
            if (this.actor.Actor.ConditionsAccumulated.AnyOfSpec(Get.Condition_Sleeping))
            {
                if (!flag)
                {
                    return EyesGOC.Eyes_Closed;
                }
                return EyesGOC.Eyes_ClosedAngry;
            }
            else if (this.actor.Actor.ConditionsAccumulated.AnyOfSpec(Get.Condition_Frozen))
            {
                if (!flag)
                {
                    return EyesGOC.Eyes_Open;
                }
                return EyesGOC.Eyes_OpenAngry;
            }
            else
            {
                float num = 4f * ((float)this.actor.Actor.SequencePerTurn / 12f);
                float num2 = Calc.HashToRange(this.actor.Actor.StableID, 0f, num);
                if ((Clock.Time + num2) % num < 0.12f)
                {
                    if (!flag)
                    {
                        return EyesGOC.Eyes_Closed;
                    }
                    return EyesGOC.Eyes_ClosedAngry;
                }
                else
                {
                    if (!flag)
                    {
                        return EyesGOC.Eyes_Open;
                    }
                    return EyesGOC.Eyes_OpenAngry;
                }
            }
        }

        private bool CheckInit()
        {
            if (EyesGOC.Eyes_Open == null)
            {
                EyesGOC.Eyes_Open = Assets.Get<Material>("Materials/Actors/Eyes_Open");
            }
            if (EyesGOC.Eyes_Closed == null)
            {
                EyesGOC.Eyes_Closed = Assets.Get<Material>("Materials/Actors/Eyes_Closed");
            }
            if (EyesGOC.Eyes_Dead == null)
            {
                EyesGOC.Eyes_Dead = Assets.Get<Material>("Materials/Actors/Eyes_Dead");
            }
            if (EyesGOC.Eyes_OpenAngry == null)
            {
                EyesGOC.Eyes_OpenAngry = Assets.Get<Material>("Materials/Actors/Eyes_OpenAngry");
            }
            if (EyesGOC.Eyes_ClosedAngry == null)
            {
                EyesGOC.Eyes_ClosedAngry = Assets.Get<Material>("Materials/Actors/Eyes_ClosedAngry");
            }
            if (this.eyesRenderer == null)
            {
                this.eyesRenderer = base.GetComponent<MeshRenderer>();
                if (this.eyesRenderer == null)
                {
                    return false;
                }
            }
            if (this.actor == null)
            {
                this.actor = base.GetComponentInParent<ActorGOC>();
                if (this.actor == null)
                {
                    return false;
                }
            }
            return this.actor.Actor != null;
        }

        public void OnPreShatter()
        {
            this.UpdateEyes();
        }

        private MeshRenderer eyesRenderer;

        private ActorGOC actor;

        private static Material Eyes_Open;

        private static Material Eyes_Closed;

        private static Material Eyes_Dead;

        private static Material Eyes_OpenAngry;

        private static Material Eyes_ClosedAngry;
    }
}