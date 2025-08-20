using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public interface IUsable : ITipSubject
    {
        UseEffects UseEffects { get; }

        UseEffects MissUseEffects { get; }

        TargetFilter UseFilter { get; }

        TargetFilter UseFilterAoE { get; }

        int UseRange { get; }

        string UseLabel_Self { get; }

        string UseLabel_Other { get; }

        string UseDescriptionFormat_Self { get; }

        string UseDescriptionFormat_Other { get; }

        int? LastUsedToRewindTimeSequence { get; set; }

        int? LastUseSequence { get; set; }

        int? CanRewindTimeEveryTurns { get; }

        float SequencePerUseMultiplier { get; }

        int MyStableHash { get; }

        int ManaCost { get; }

        int StaminaCost { get; }

        int CooldownTurns { get; }

        float MissChance { get; }

        float CritChance { get; }

        UsePrompt UsePrompt { get; }

        bool CanUse_ExtraInstanceSpecificChecks(Actor user, Vector3Int? assumeUserPos = null, bool assumeAnyUserPos = false, StringSlot outReason = null);
    }
}