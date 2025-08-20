using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_DropItem : Action
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
                return Calc.CombineHashes<int, int, int>(this.actor.MyStableHash, this.item.MyStableHash, 414260913);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor, this.item);
            }
        }

        protected Action_DropItem()
        {
        }

        public Action_DropItem(ActionSpec spec, Actor actor, Item item)
            : base(spec)
        {
            this.actor = actor;
            this.item = item;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (this.item.Spawned)
            {
                return false;
            }
            if (!ignoreActorState)
            {
                if (!this.actor.Spawned)
                {
                    return false;
                }
                if (!this.actor.Inventory.Contains(this.item))
                {
                    return false;
                }
                if (this.actor.Inventory.Equipped.IsEquipped(this.item) && this.item.Cursed)
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.actor) || ActionUtility.TargetConcernsPlayer(this.item);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(this.item))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(this.item, SpawnPositionFinder.Near(this.actor.Position, this.item, true, false, null), null))
            {
                yield return instruction2;
            }
            enumerator = null;
            yield return new Instruction_Sound(Get.Sound_DroppedItem, new Vector3?(this.item.Position), 1f, 1f);
            if (this.item.Spec == Get.Entity_HolyWater && this.item.Spawned && Get.World.AnyEntityOfSpecAt(this.item.Position, Get.Entity_CultistCircle))
            {
                yield return new Instruction_VisualEffect(Get.VisualEffect_Sacrifice, this.item.Position);
                foreach (Instruction instruction3 in InstructionSets_Entity.DeSpawn(this.item, false))
                {
                    yield return instruction3;
                }
                enumerator = null;
                Actor actor = Get.World.Actors.FirstOrDefault<Actor>(delegate (Actor x)
                {
                    Faction faction = x.Faction;
                    return ((faction != null) ? faction.Spec : null) == Get.Faction_Cult;
                });
                if (actor != null)
                {
                    Faction firstOfSpec = Get.FactionManager.GetFirstOfSpec(Get.Faction_Monsters);
                    if (firstOfSpec != null && !Get.FactionManager.HostilityExists(actor.Faction, firstOfSpec))
                    {
                        yield return new Instruction_AddFactionHostility(actor.Faction, firstOfSpec);
                    }
                }
                if (!Get.Achievement_GiveHolyWaterToCultists.IsCompleted)
                {
                    yield return new Instruction_CompleteAchievement(Get.Achievement_GiveHolyWaterToCultists);
                }
            }
            if (this.item.Spawned && this.actor.IsNowControlledActor)
            {
                foreach (Instruction instruction4 in this.CheckGlyphsComplete(this.item))
                {
                    yield return instruction4;
                }
                enumerator = null;
            }
            yield break;
            yield break;
        }

        private IEnumerable<Instruction> CheckGlyphsComplete(Item item)
        {
            if (!Get.World.AnyEntityOfSpecAt(item.Position, Get.Entity_Glyph1) && !Get.World.AnyEntityOfSpecAt(item.Position, Get.Entity_Glyph2) && !Get.World.AnyEntityOfSpecAt(item.Position, Get.Entity_Glyph3) && !Get.World.AnyEntityOfSpecAt(item.Position, Get.Entity_Glyph4))
            {
                yield break;
            }
            if (this.AreAllGlyphsComplete())
            {
                yield return new Instruction_VisualEffect(Get.VisualEffect_Sacrifice, item.Position);
                Rand.PushState(Calc.CombineHashes<int, int>(Get.WorldSeed, 743580098));
                Item rewardItem = ItemGenerator.Reward(true);
                Rand.PopState();
                foreach (Instruction instruction in InstructionSets_Actor.AddToInventoryOrSpawnNear(Get.NowControlledActor, rewardItem))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                yield return new Instruction_PlayLog("GlyphsCompleted".Translate(rewardItem));
                foreach (Structure structure in this.GetAllGlyphs().ToTemporaryList<Structure>())
                {
                    foreach (Instruction instruction2 in InstructionSets_Entity.Destroy(structure, null, null))
                    {
                        yield return instruction2;
                    }
                    enumerator = null;
                }
                List<Structure>.Enumerator enumerator2 = default(List<Structure>.Enumerator);
                rewardItem = null;
            }
            yield break;
            yield break;
        }

        private bool IsValidItemForGlyph(Item item, Structure glyph)
        {
            if (glyph.Spec == Get.Entity_Glyph1)
            {
                return item.Spec.Item.IsEquippableWeapon;
            }
            if (glyph.Spec == Get.Entity_Glyph2)
            {
                return item.Spec.Item.ItemSlot == Get.ItemSlot_Armor;
            }
            if (glyph.Spec == Get.Entity_Glyph3)
            {
                return item.Spec.Item.Generator_IsScroll;
            }
            return glyph.Spec == Get.Entity_Glyph4 && item.Spec.Item.Generator_IsPotion;
        }

        private IEnumerable<Structure> GetAllGlyphs()
        {
            return Get.World.GetEntitiesOfSpec(Get.Entity_Glyph1).Concat<Entity>(Get.World.GetEntitiesOfSpec(Get.Entity_Glyph2)).Concat<Entity>(Get.World.GetEntitiesOfSpec(Get.Entity_Glyph3))
                .Concat<Entity>(Get.World.GetEntitiesOfSpec(Get.Entity_Glyph4))
                .Cast<Structure>();
        }

        private bool AreAllGlyphsComplete()
        {
            List<Structure> list = this.GetAllGlyphs().ToList<Structure>();
            if (list.Count == 0)
            {
                return false;
            }
            foreach (Structure structure in list)
            {
                bool flag = false;
                foreach (Entity entity in Get.World.GetEntitiesAt(structure.Position))
                {
                    Item item = entity as Item;
                    if (item != null && this.IsValidItemForGlyph(item, structure))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            return true;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Item item;
    }
}