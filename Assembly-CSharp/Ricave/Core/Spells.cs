using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Spells
    {
        public Actor Owner
        {
            get
            {
                return this.owner;
            }
        }

        public int Count
        {
            get
            {
                return this.spells.Count;
            }
        }

        public bool Any
        {
            get
            {
                return this.spells.Count != 0;
            }
        }

        public int MyStableHash
        {
            get
            {
                if (this.owner == null)
                {
                    return 0;
                }
                return Calc.CombineHashes<int, int>(this.owner.MyStableHash, 498142068);
            }
        }

        public List<Spell> All
        {
            get
            {
                return this.spells;
            }
        }

        public Spells()
        {
        }

        public Spells(Actor owner)
        {
            this.owner = owner;
        }

        public bool Contains(Spell spell)
        {
            return spell != null && this.spells.Contains(spell);
        }

        public bool AnyOfSpec(SpellSpec spec)
        {
            if (spec == null)
            {
                return false;
            }
            for (int i = 0; i < this.spells.Count; i++)
            {
                if (this.spells[i].Spec == spec)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddSpell(Spell spell, int insertAt = -1)
        {
            Instruction.ThrowIfNotExecuting();
            if (spell == null)
            {
                Log.Error("Tried to add null spell.", false);
                return;
            }
            if (this.Contains(spell))
            {
                Log.Error("Tried to add the same spell twice.", false);
                return;
            }
            if (insertAt >= 0)
            {
                if (insertAt > this.spells.Count)
                {
                    Log.Error(string.Concat(new string[]
                    {
                        "Tried to insert spell at index ",
                        insertAt.ToString(),
                        " but spells count is only ",
                        this.spells.Count.ToString(),
                        "."
                    }), false);
                    return;
                }
                this.spells.Insert(insertAt, spell);
            }
            else
            {
                this.spells.Add(spell);
            }
            spell.Parent = this;
            spell.OnGained();
        }

        public int RemoveSpell(Spell spell)
        {
            Instruction.ThrowIfNotExecuting();
            if (spell == null)
            {
                Log.Error("Tried to remove null spell.", false);
                return -1;
            }
            if (!this.Contains(spell))
            {
                Log.Error("Tried to remove spell but it's not here.", false);
                return -1;
            }
            int num = this.spells.IndexOf(spell);
            this.spells.RemoveAt(num);
            spell.Parent = null;
            Get.UseOnTargetUI.OnSpellRemoved(this.owner, spell);
            return num;
        }

        public void Reorder(int index1, int index2)
        {
            Instruction.ThrowIfNotExecuting();
            if (index1 < 0 || index1 >= this.spells.Count)
            {
                Log.Error("Spell index of spell 1 out of bounds while reordering.", false);
                return;
            }
            if (index2 < 0 || index2 >= this.spells.Count)
            {
                Log.Error("Spell index of spell 2 out of bounds while reordering.", false);
                return;
            }
            if (index1 == index2)
            {
                return;
            }
            this.spells.Swap<Spell>(index1, index2);
        }

        public List<ValueTuple<string, Action, string>> GetPossibleInteractions(Spell spell, out bool canAutoUseOnlyAvailableOption, bool skipNonImportant = false)
        {
            List<ValueTuple<string, Action, string>> list = new List<ValueTuple<string, Action, string>>();
            canAutoUseOnlyAvailableOption = false;
            if (spell == null)
            {
                Log.Error("Tried to get possible interactions for a null spell.", false);
                return list;
            }
            if (!this.Contains(spell))
            {
                Log.Error("Tried to get possible interactions for a spell which is not here.", false);
                return list;
            }
            if (spell.Spec.AllowUseOnSelfViaInterface && spell.UseFilter.Allows(this.owner, this.owner))
            {
                this.tmpFailReason.Clear();
                Actor actor = this.owner;
                IUsable spell2 = spell;
                Target target = this.owner;
                StringSlot stringSlot = this.tmpFailReason;
                if (actor.CanUseOn(spell2, target, null, false, stringSlot))
                {
                    list.Add(new ValueTuple<string, Action, string>(spell.UseLabel_Self, delegate
                    {
                        Inventory.UseOnSelfOrShowUsePrompt(spell);
                    }, null));
                }
                else
                {
                    list.Add(new ValueTuple<string, Action, string>(spell.UseLabel_Self, null, this.tmpFailReason.String.NullOrEmpty() ? "Disabled".Translate() : this.tmpFailReason.String));
                }
            }
            if (spell.UseRange > 0 && Get.UseOnTargetUI.TargetingUsable != spell)
            {
                list.Add(new ValueTuple<string, Action, string>(spell.UseLabel_Other, delegate
                {
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    Get.UseOnTargetUI.BeginTargeting(spell, false);
                }, null));
                canAutoUseOnlyAvailableOption = true;
            }
            if (Get.UseOnTargetUI.TargetingUsable == spell)
            {
                list.Add(new ValueTuple<string, Action, string>("StopTargeting".Translate(), delegate
                {
                    Get.Sound_CloseWindow.PlayOneShot(null, 1f, 1f);
                    Get.UseOnTargetUI.StopTargeting();
                }, null));
                canAutoUseOnlyAvailableOption = true;
            }
            return list;
        }

        [Saved]
        private Actor owner;

        [Saved(Default.New, true)]
        private List<Spell> spells = new List<Spell>();

        private StringSlot tmpFailReason = new StringSlot();
    }
}