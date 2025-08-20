using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class AnimatedTelephoneGOC : MonoBehaviour
    {
        private bool ShouldRing
        {
            get
            {
                if (Get.WindowManager.IsOpen(Get.Window_QuestLog))
                {
                    return false;
                }
                foreach (QuestSpec questSpec in Get.Specs.GetAll<QuestSpec>())
                {
                    if (questSpec.Visible && !questSpec.IsCompleted() && !questSpec.IsActive() && questSpec.Dialogue != null && !questSpec.Dialogue.Ended)
                    {
                        Dialogue dialogue = Get.DialoguesManager.GetDialogue(questSpec.Dialogue);
                        if (dialogue == null || dialogue.CurNodeIndex == 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private void Start()
        {
            this.SetTransform();
        }

        private void Update()
        {
            this.SetTransform();
            if (!this.ShouldRing)
            {
                this.StopSound();
            }
        }

        private void SetTransform()
        {
            if (Clock.TimeSinceSceneLoad >= 3f)
            {
                float num = (Clock.TimeSinceSceneLoad - 3f) % 6f;
                float num2;
                if (num < 2f && this.ShouldRing)
                {
                    num2 = 5f * Calc.Sin(21.99115f * num);
                    if (this.canRing)
                    {
                        this.canRing = false;
                        this.StopSound();
                        this.soundHandle = Get.Sound_TelephoneRinging.PlayWithHandle(new Vector3?(base.transform.position), false);
                    }
                }
                else
                {
                    num2 = 0f;
                    this.canRing = true;
                }
                base.transform.localRotation = Quaternion.Euler(0f, 0f, num2);
            }
        }

        private void StopSound()
        {
            SoundHandle.DisposeIfNotNull(ref this.soundHandle);
        }

        private bool canRing = true;

        private SoundHandle soundHandle;
    }
}