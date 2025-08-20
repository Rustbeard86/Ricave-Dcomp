using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class CompsCollection<BaseCompType>
    {
        public List<BaseCompType> AllComps
        {
            get
            {
                return this.list;
            }
        }

        public CompType GetComp<CompType>() where CompType : class, BaseCompType
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                CompType compType = this.list[i] as CompType;
                if (compType != null)
                {
                    return compType;
                }
            }
            return default(CompType);
        }

        public bool HasComp<CompType>() where CompType : class, BaseCompType
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                if (this.list[i] is CompType)
                {
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            Instruction.ThrowIfNotExecuting();
            this.list.Clear();
        }

        public void Add(BaseCompType comp)
        {
            Instruction.ThrowIfNotExecuting();
            if (comp == null)
            {
                Log.Error("Tried to add null comp.", false);
                return;
            }
            if (this.list.Contains(comp))
            {
                Log.Error("Tried to add the same comp twice.", false);
                return;
            }
            this.list.Add(comp);
        }

        public void Remove(BaseCompType comp)
        {
            Instruction.ThrowIfNotExecuting();
            if (comp == null)
            {
                Log.Error("Tried to remove null comp.", false);
                return;
            }
            if (!this.list.Contains(comp))
            {
                Log.Error("Tried to remove comp but it's not here.", false);
                return;
            }
            this.list.Remove(comp);
        }

        [Saved(Default.New, true)]
        private List<BaseCompType> list = new List<BaseCompType>();
    }
}