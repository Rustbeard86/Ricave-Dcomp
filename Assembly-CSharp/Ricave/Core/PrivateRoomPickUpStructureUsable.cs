using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class PrivateRoomPickUpStructureUsable : IUsable, ITipSubject
    {
        string ITipSubject.LabelCap
        {
            get
            {
                return "PickUpPrivateRoomStructure_Other".Translate();
            }
        }

        string ITipSubject.Description
        {
            get
            {
                return "PickUpPrivateRoomStructureTip".Translate();
            }
        }

        Texture2D ITipSubject.Icon
        {
            get
            {
                return PrivateRoomPickUpStructureUsable.PickUpPrivateRoomStructureIcon;
            }
        }

        Color ITipSubject.IconColor
        {
            get
            {
                return Color.white;
            }
        }

        UseEffects IUsable.UseEffects
        {
            get
            {
                return this.useEffects;
            }
        }

        UseEffects IUsable.MissUseEffects
        {
            get
            {
                return null;
            }
        }

        TargetFilter IUsable.UseFilter
        {
            get
            {
                return TargetFilter.ForPrivateRoomPickUpStructure();
            }
        }

        TargetFilter IUsable.UseFilterAoE
        {
            get
            {
                return TargetFilter.ForPrivateRoomPickUpStructure();
            }
        }

        int IUsable.UseRange
        {
            get
            {
                return 10;
            }
        }

        string IUsable.UseLabel_Self
        {
            get
            {
                return UsableUtility.CheckAppendDotsToUseLabel("UseLabel_Self".Translate(), this);
            }
        }

        string IUsable.UseLabel_Other
        {
            get
            {
                return "PickUpPrivateRoomStructure_Other".Translate();
            }
        }

        string IUsable.UseDescriptionFormat_Self
        {
            get
            {
                return "UseDescription_Self".Translate();
            }
        }

        string IUsable.UseDescriptionFormat_Other
        {
            get
            {
                return "PickUpPrivateRoomStructureDescription_Other".Translate();
            }
        }

        int? IUsable.LastUsedToRewindTimeSequence
        {
            get
            {
                return this.lastUsedToRewindTimeSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastUsedToRewindTimeSequence = value;
            }
        }

        int? IUsable.LastUseSequence
        {
            get
            {
                return this.lastUseSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastUseSequence = value;
            }
        }

        int? IUsable.CanRewindTimeEveryTurns
        {
            get
            {
                return null;
            }
        }

        float IUsable.SequencePerUseMultiplier
        {
            get
            {
                return 1f;
            }
        }

        int IUsable.MyStableHash
        {
            get
            {
                return 853243121;
            }
        }

        int IUsable.ManaCost
        {
            get
            {
                return 0;
            }
        }

        int IUsable.StaminaCost
        {
            get
            {
                return 0;
            }
        }

        int IUsable.CooldownTurns
        {
            get
            {
                return 0;
            }
        }

        float IUsable.MissChance
        {
            get
            {
                return 0f;
            }
        }

        float IUsable.CritChance
        {
            get
            {
                return 0f;
            }
        }

        UsePrompt IUsable.UsePrompt
        {
            get
            {
                return null;
            }
        }

        public PrivateRoomPickUpStructureUsable()
        {
            this.useEffects = new UseEffects(this);
        }

        bool IUsable.CanUse_ExtraInstanceSpecificChecks(Actor user, Vector3Int? assumeUserPos, bool assumeAnyUserPos, StringSlot outReason)
        {
            return Get.InLobby && user.IsMainActor && user.Position.InPrivateRoom();
        }

        public void PostMakePlayerActor()
        {
            new Instruction_AddUseEffect(new UseEffect_PickUpPrivateRoomStructure(Get.UseEffect_PickUpPrivateRoomStructure), this.useEffects).Do();
        }

        public void OnPlayerMoved(Vector3Int prev)
        {
            if (Get.UseOnTargetUI.TargetingUsable == this && !Get.NowControlledActor.Position.InPrivateRoom())
            {
                Get.UseOnTargetUI.StopTargeting();
            }
        }

        [Saved]
        private UseEffects useEffects;

        [Saved]
        private int? lastUsedToRewindTimeSequence;

        [Saved]
        private int? lastUseSequence;

        private static readonly Texture2D PickUpPrivateRoomStructureIcon = Assets.Get<Texture2D>("Textures/UI/PickUpPrivateRoomStructure");
    }
}