using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Descend : UseEffect
    {
        protected UseEffect_Descend()
        {
        }

        public UseEffect_Descend(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_Descend useEffect_Descend = (UseEffect_Descend)clone;
            useEffect_Descend.usedDescendTrigger = this.usedDescendTrigger;
            useEffect_Descend.showDungeonMap = this.showDungeonMap;
            useEffect_Descend.disallowIfAllPartyCantReach = this.disallowIfAllPartyCantReach;
        }

        public override bool PreventEntireUse(Actor user, IUsable usable, Target target, StringSlot outReason = null)
        {
            if (user != null && user.IsPlayerParty && this.disallowIfAllPartyCantReach)
            {
                Structure structure = usable as Structure;
                if (structure != null && !this.AllPartyCanReach(user, structure))
                {
                    if (outReason != null)
                    {
                        outReason.Set("CantDescendBecausePartyCantReach".Translate());
                    }
                    return true;
                }
            }
            return false;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            if (actor == null)
            {
                yield break;
            }
            if (actor.IsPlayerParty)
            {
                if (this.disallowIfAllPartyCantReach)
                {
                    Structure structure = usable as Structure;
                    if (structure != null && !this.AllPartyCanReach(user, structure))
                    {
                        yield return new Instruction_PlayLog("CantDescendBecausePartyCantReach".Translate());
                        yield break;
                    }
                }
                if (this.usedDescendTrigger && !Get.Achievement_JumpIntoPit.IsCompleted)
                {
                    yield return new Instruction_CompleteAchievement(Get.Achievement_JumpIntoPit);
                }
                if (this.showDungeonMap && Get.PlaceManager.Places.Count != 0 && Get.PlaceManager.GetNextPlaces(Get.Place).Any<Place>())
                {
                    yield return new Instruction_Immediate(delegate
                    {
                        Get.ScreenFader.FadeOutAndExecute(delegate
                        {
                            Get.DungeonMapDrawer.Show();
                        }, new float?(2.5f), false, false, false);
                    });
                }
                else
                {
                    Place randomNextNonShelterPlaceOrNull = Get.PlaceManager.GetRandomNextNonShelterPlaceOrNull(Get.Place);
                    yield return new Instruction_Descend(randomNextNonShelterPlaceOrNull, null, this.usedDescendTrigger);
                }
            }
            else
            {
                foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(actor, false))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        private bool AllPartyCanReach(Actor user, Structure staircase)
        {
            foreach (Actor actor in Get.PlayerParty)
            {
                if (actor != user)
                {
                    if (!actor.Spawned)
                    {
                        if (actor.HP > 0)
                        {
                            return false;
                        }
                    }
                    else if ((!actor.Position.IsAdjacentOrInside(staircase.Position) || !LineOfSight.IsLineOfFire(actor.Position, staircase.Position)) && (!actor.Position.IsAdjacentOrInside(user.Position) || !LineOfSight.IsLineOfFire(actor.Position, user.Position)))
                    {
                        if (actor.Position.GetGridDistance(staircase.Position) <= 3)
                        {
                            List<Vector3Int> list = Get.PathFinder.FindPath(actor.Position, staircase.Position, new PathFinder.Request(PathFinder.Request.Mode.Touch, actor), 5, this.tmpListForPathFinder);
                            if (list != null && list.Count <= 4)
                            {
                                continue;
                            }
                        }
                        if (actor.Position.GetGridDistance(user.Position) <= 3)
                        {
                            List<Vector3Int> list2 = Get.PathFinder.FindPath(actor.Position, user.Position, new PathFinder.Request(PathFinder.Request.Mode.Touch, actor), 5, this.tmpListForPathFinder);
                            if (list2 != null && list2.Count <= 4)
                            {
                                continue;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        [Saved]
        private bool usedDescendTrigger;

        [Saved]
        private bool showDungeonMap;

        [Saved]
        private bool disallowIfAllPartyCantReach;

        private List<Vector3Int> tmpListForPathFinder = new List<Vector3Int>();
    }
}