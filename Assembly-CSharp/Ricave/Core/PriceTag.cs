using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class PriceTag
    {
        public EntitySpec ItemSpec
        {
            get
            {
                return this.itemSpec;
            }
        }

        public int Count
        {
            get
            {
                float num = 1f;
                if (this.affectedByLowerPricesSkill)
                {
                    if (Get.Skill_LowerPrices.IsUnlocked())
                    {
                        num *= 0.5f;
                    }
                    num *= Get.TraitManager.WanderersAndShopkeepersPricesFactor;
                }
                return Calc.RoundToIntHalfUp((float)this.count * num);
            }
        }

        public bool AffectedByLowerPricesSkill
        {
            get
            {
                return this.affectedByLowerPricesSkill;
            }
        }

        public string PriceRichText
        {
            get
            {
                if (this.itemSpec == Get.Entity_Gold)
                {
                    return RichText.Gold(StringUtility.GoldString(this.Count));
                }
                if (this.itemSpec.Item.LobbyItem)
                {
                    if (this.itemSpec == Get.Entity_Diamond)
                    {
                        return RichText.Diamond(StringUtility.DiamondsString(this.Count));
                    }
                    if (this.itemSpec == Get.Entity_Stardust)
                    {
                        return RichText.Stardust(StringUtility.StardustString(this.Count));
                    }
                    return RichText.Blue("{0} x{1}".Formatted(this.itemSpec.LabelAdjustedCap, this.Count.ToStringCached()));
                }
                else
                {
                    if (this.Count != 1)
                    {
                        return "{0} x{1}".Formatted(this.itemSpec, this.Count);
                    }
                    return RichText.Label(this.itemSpec, false);
                }
            }
        }

        public string PriceShortRichText
        {
            get
            {
                if (this.itemSpec == Get.Entity_Gold)
                {
                    return RichText.Gold("GoldCountShort".Translate(this.Count));
                }
                if (!this.itemSpec.Item.LobbyItem)
                {
                    return this.PriceRichText;
                }
                if (this.itemSpec == Get.Entity_Diamond)
                {
                    return RichText.Diamond("{0}▼".Formatted(this.Count));
                }
                if (this.itemSpec == Get.Entity_Stardust)
                {
                    return RichText.Stardust("{0}★".Formatted(this.Count));
                }
                return this.PriceRichText;
            }
        }

        protected PriceTag()
        {
        }

        public PriceTag(EntitySpec itemSpec, int count, bool affectedByLowerPricesSkill = false)
        {
            this.itemSpec = itemSpec;
            this.count = count;
            this.affectedByLowerPricesSkill = affectedByLowerPricesSkill;
        }

        public int HasCount(Actor actor)
        {
            if (this.itemSpec == Get.Entity_Gold && actor.IsPlayerParty)
            {
                return Get.Player.Gold;
            }
            if (!this.itemSpec.Item.LobbyItem || !actor.IsPlayerParty)
            {
                return actor.Inventory.GetCount(this.itemSpec, false);
            }
            if (Get.InLobby)
            {
                return Get.TotalLobbyItems.GetCount(this.itemSpec);
            }
            return Get.TotalLobbyItems.GetCount(this.itemSpec) + Get.ThisRunLobbyItems.GetCount(this.itemSpec);
        }

        public bool CanAfford(Actor actor)
        {
            return this.HasCount(actor) >= this.Count;
        }

        public IEnumerable<Instruction> MakePayInstructions(Actor actor, Actor giveTo = null)
        {
            if (!this.CanAfford(actor))
            {
                Log.Error("Called MakePayInstructions() but can't afford it. This should have been checked before calling this method.", false);
                yield break;
            }
            if (this.itemSpec == Get.Entity_Gold && actor.IsPlayerParty)
            {
                if (this.Count > 0)
                {
                    yield return new Instruction_ChangePlayerGold(-this.Count);
                    if (giveTo != null)
                    {
                        foreach (Instruction instruction in InstructionSets_Actor.AddToInventoryOrSpawnNear(giveTo, ItemGenerator.Gold(this.Count)))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator = null;
                    }
                }
            }
            else if (this.itemSpec.Item.LobbyItem && actor.IsPlayerParty)
            {
                if (this.Count > 0)
                {
                    yield return new Instruction_ChangeLobbyItems(this.itemSpec, -this.Count);
                }
            }
            else if (this.Count > 0)
            {
                int found = 0;
                foreach (Item item in actor.Inventory.AllItems.ToTemporaryList<Item>())
                {
                    if (item.Spec == this.itemSpec && (!item.Cursed || !actor.Inventory.Equipped.IsEquipped(item)))
                    {
                        int toTake = Math.Min(this.Count - found, item.StackCount);
                        if (giveTo != null)
                        {
                            foreach (Instruction instruction2 in InstructionSets_Actor.RemoveCountFromInventoryAndGiveTo(item, this.Count, giveTo))
                            {
                                yield return instruction2;
                            }
                            IEnumerator<Instruction> enumerator = null;
                        }
                        else
                        {
                            foreach (Instruction instruction3 in InstructionSets_Actor.RemoveCountFromInventory(item, this.Count))
                            {
                                yield return instruction3;
                            }
                            IEnumerator<Instruction> enumerator = null;
                        }
                        found += toTake;
                        if (found >= this.Count)
                        {
                            break;
                        }
                    }
                }
                List<Item>.Enumerator enumerator2 = default(List<Item>.Enumerator);
            }
            yield break;
            yield break;
        }

        [Saved]
        private EntitySpec itemSpec;

        [Saved]
        private int count;

        [Saved]
        private bool affectedByLowerPricesSkill;
    }
}