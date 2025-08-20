using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffects : ISaveableEventsReceiver
    {
        public IUsable Parent
        {
            get
            {
                return this.parent;
            }
        }

        public bool Any
        {
            get
            {
                return this.useEffects.Count != 0;
            }
        }

        public int MyStableHash
        {
            get
            {
                if (this.parent == null)
                {
                    return 0;
                }
                return Calc.CombineHashes<int, int>(this.parent.MyStableHash, 1306812);
            }
        }

        public List<UseEffect> All
        {
            get
            {
                return this.useEffects;
            }
        }

        public PriceTag Price
        {
            get
            {
                UseEffect_Pay useEffect_Pay = this.GetFirstOfSpec(Get.UseEffect_Pay) as UseEffect_Pay;
                if (useEffect_Pay == null)
                {
                    return null;
                }
                return useEffect_Pay.Price;
            }
        }

        public List<UseEffectDrawRequest> AllDrawRequests
        {
            get
            {
                this.tmpDrawRequests.Clear();
                for (int i = 0; i < this.useEffects.Count; i++)
                {
                    UseEffectDrawRequest? useEffectDrawRequest = this.useEffects[i].ToDrawRequestOrNull();
                    if (useEffectDrawRequest != null)
                    {
                        this.tmpDrawRequests.Add(useEffectDrawRequest.Value);
                    }
                }
                return this.tmpDrawRequests;
            }
        }

        protected UseEffects()
        {
        }

        public UseEffects(IUsable parent)
        {
            this.parent = parent;
        }

        public bool Contains(UseEffect useEffect)
        {
            return useEffect != null && this.useEffects.Contains(useEffect);
        }

        public bool Contains_ProbablyAt(UseEffect useEffect, int probablyAt)
        {
            return useEffect != null && this.useEffects.Contains_ProbablyAt(useEffect, probablyAt);
        }

        public void AddUseEffect(UseEffect useEffect, int insertAt = -1)
        {
            Instruction.ThrowIfNotExecuting();
            if (useEffect == null)
            {
                Log.Error("Tried to add null use effect.", false);
                return;
            }
            if (this.Contains(useEffect))
            {
                Log.Error("Tried to add the same use effect twice.", false);
                return;
            }
            if (insertAt >= 0)
            {
                if (insertAt > this.useEffects.Count)
                {
                    Log.Error(string.Concat(new string[]
                    {
                        "Tried to insert use effect at index ",
                        insertAt.ToString(),
                        " but use effects count is only ",
                        this.useEffects.Count.ToString(),
                        "."
                    }), false);
                    return;
                }
                this.useEffects.Insert(insertAt, useEffect);
            }
            else
            {
                this.useEffects.Add(useEffect);
            }
            useEffect.Parent = this;
        }

        public int RemoveUseEffect(UseEffect useEffect)
        {
            Instruction.ThrowIfNotExecuting();
            if (useEffect == null)
            {
                Log.Error("Tried to remove null use effect.", false);
                return -1;
            }
            if (!this.Contains(useEffect))
            {
                Log.Error("Tried to remove use effect but it's not here.", false);
                return -1;
            }
            int num = this.useEffects.IndexOf(useEffect);
            this.useEffects.RemoveAt(num);
            useEffect.Parent = null;
            return num;
        }

        public void Clear()
        {
            foreach (UseEffect useEffect in this.All.ToTemporaryList<UseEffect>())
            {
                this.RemoveUseEffect(useEffect);
            }
        }

        public void AddClonedFrom(UseEffects other)
        {
            if (other == null)
            {
                Log.Error("Tried to add cloned UseEffects from null UseEffects.", false);
                return;
            }
            if (other == this)
            {
                Log.Error("Tried to add cloned UseEffects from self.", false);
                return;
            }
            List<UseEffect> all = other.All;
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i] == null)
                {
                    Log.Error("Tried to add null UseEffect in AddClonedFrom().", false);
                }
                else
                {
                    this.AddUseEffect(all[i].Clone(), -1);
                }
            }
        }

        public UseEffect GetFirstOfSpec(UseEffectSpec spec)
        {
            for (int i = 0; i < this.useEffects.Count; i++)
            {
                if (this.useEffects[i].Spec == spec)
                {
                    return this.useEffects[i];
                }
            }
            return null;
        }

        public bool AnyOfSpec(UseEffectSpec spec)
        {
            return this.GetFirstOfSpec(spec) != null;
        }

        public bool AnyPreventsEntireUse(Actor user, Target target, StringSlot outReason = null)
        {
            for (int i = 0; i < this.useEffects.Count; i++)
            {
                if (this.useEffects[i].PreventEntireUse(user, this.parent, target, outReason))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.useEffects.RemoveAll((UseEffect x) => x.Spec == null) != 0)
            {
                Log.Error("Removed some use effects with null spec from UseEffects.", false);
            }
        }

        [Saved]
        private IUsable parent;

        [Saved(Default.New, true)]
        private List<UseEffect> useEffects = new List<UseEffect>();

        private List<UseEffectDrawRequest> tmpDrawRequests = new List<UseEffectDrawRequest>();
    }
}