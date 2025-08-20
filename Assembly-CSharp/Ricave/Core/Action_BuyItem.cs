using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_BuyItem : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int, int>(this.actor.MyStableHash, this.item.MyStableHash, 153850923);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, this.item);
            }
        }

        protected Action_BuyItem()
        {
        }

        public Action_BuyItem(ActionSpec spec, Actor actor, Item item)
            : base(spec)
        {
            this.actor = actor;
            this.item = item;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (!this.item.Spawned)
            {
                return false;
            }
            if (!this.item.ForSale)
            {
                return false;
            }
            if (!ignoreActorState)
            {
                if (!this.actor.Spawned)
                {
                    return false;
                }
                if (this.actor.Position != this.item.Position)
                {
                    return false;
                }
                if (!this.item.PriceTag.CanAfford(this.actor))
                {
                    return false;
                }
                if (!InstructionSets_Actor.HasSpaceToPickUp(this.actor, this.item))
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            foreach (Instruction instruction in this.item.PriceTag.MakePayInstructions(this.actor, null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_RemovePriceTag(this.item);
            if (this.actor.IsNowControlledActor)
            {
                yield return new Instruction_Sound(Get.Sound_BoughtItem, null, 1f, 1f);
                yield return new Instruction_PlayLog("YouBoughtItem".Translate(this.item));
            }
            UnlockableAsItem unlockableAsItem = this.item as UnlockableAsItem;
            if (unlockableAsItem != null && unlockableAsItem.UnlockableSpec == Get.Unlockable_PrivateRoom)
            {
                foreach (Instruction instruction2 in this.UnlockPrivateRoomDoor(this.item.Position))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            if (Get.PlaceSpec == Get.Place_Shelter && this.item.CustomLook != null)
            {
                yield return new Instruction_SetCustomLook(this.item, null);
                if (!this.item.Identified)
                {
                    foreach (Instruction instruction3 in InstructionSets_Entity.Identify(this.item, this.item.TurnsLeftToIdentify, false))
                    {
                        yield return instruction3;
                    }
                    enumerator = null;
                }
            }
            foreach (Instruction instruction4 in InstructionSets_Entity.DeSpawn(this.item, false))
            {
                yield return instruction4;
            }
            enumerator = null;
            foreach (Instruction instruction5 in InstructionSets_Actor.AddToInventory(this.actor, this.item))
            {
                yield return instruction5;
            }
            enumerator = null;
            yield break;
            yield break;
        }

        private IEnumerable<Instruction> UnlockPrivateRoomDoor(Vector3Int near)
        {
            if (!Get.InLobby)
            {
                yield break;
            }
            foreach (Vector3Int vector3Int in near.AdjacentCellsXZ())
            {
                if (vector3Int.InBounds())
                {
                    Entity door = Get.World.GetFirstEntityOfSpecAt(vector3Int, Get.Entity_LockedDoor);
                    if (door != null)
                    {
                        foreach (Instruction instruction in InstructionSets_Entity.Spawn(Maker.Make(Get.Entity_Door, null, false, false, true), vector3Int, null))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                        foreach (Instruction instruction2 in InstructionSets_Entity.DeSpawn(door, false))
                        {
                            yield return instruction2;
                        }
                        enumerator2 = null;
                        break;
                    }
                }
            }
            IEnumerator<Vector3Int> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Item item;
    }
}