using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Conditions : ISaveableEventsReceiver
    {
        public Entity Parent
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
                return this.conditions.Count != 0;
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
                return Calc.CombineHashes<int, int>(this.parent.MyStableHash, 81320908);
            }
        }

        public List<Condition> All
        {
            get
            {
                return this.conditions;
            }
        }

        public List<ConditionDrawRequest> AllDrawRequests
        {
            get
            {
                this.tmpDrawRequests.Clear();
                for (int i = 0; i < this.conditions.Count; i++)
                {
                    ConditionDrawRequest? conditionDrawRequest = this.conditions[i].ToDrawRequestOrNull();
                    if (conditionDrawRequest != null)
                    {
                        this.tmpDrawRequests.Add(conditionDrawRequest.Value);
                    }
                }
                return this.tmpDrawRequests;
            }
        }

        protected Conditions()
        {
        }

        public Conditions(Entity parent)
        {
            this.parent = parent;
        }

        public bool Contains(Condition condition)
        {
            return condition != null && this.conditions.Contains(condition);
        }

        public bool Contains_ProbablyAt(Condition condition, int probablyAt)
        {
            return condition != null && this.conditions.Contains_ProbablyAt(condition, probablyAt);
        }

        public void AddCondition(Condition condition, int insertAt = -1)
        {
            Instruction.ThrowIfNotExecuting();
            if (condition == null)
            {
                Log.Error("Tried to add null condition.", false);
                return;
            }
            if (this.Contains(condition))
            {
                Log.Error("Tried to add the same condition twice.", false);
                return;
            }
            if (insertAt >= 0)
            {
                if (insertAt > this.conditions.Count)
                {
                    Log.Error(string.Concat(new string[]
                    {
                        "Tried to insert condition at index ",
                        insertAt.ToString(),
                        " but conditions count is only ",
                        this.conditions.Count.ToString(),
                        "."
                    }), false);
                    return;
                }
                this.conditions.Insert(insertAt, condition);
            }
            else
            {
                this.conditions.Add(condition);
            }
            condition.Parent = this;
            ConditionsAccumulated.SetCachedConditionsDirty();
            if (condition.SeeRangeOffset != 0 || condition.SeeRangeCap < 10)
            {
                Actor affectedActor = this.GetAffectedActor();
                if (affectedActor != null)
                {
                    Get.VisibilityCache.OnSeeRangeChanged(affectedActor);
                }
            }
        }

        public int RemoveCondition(Condition condition)
        {
            Instruction.ThrowIfNotExecuting();
            if (condition == null)
            {
                Log.Error("Tried to remove null condition.", false);
                return -1;
            }
            if (!this.Contains(condition))
            {
                Log.Error("Tried to remove condition but it's not here.", false);
                return -1;
            }
            Actor affectedActor = this.GetAffectedActor();
            int num = this.conditions.IndexOf(condition);
            this.conditions.RemoveAt(num);
            condition.Parent = null;
            ConditionsAccumulated.SetCachedConditionsDirty();
            if (affectedActor != null && (condition.SeeRangeOffset != 0 || condition.SeeRangeCap < 10))
            {
                Get.VisibilityCache.OnSeeRangeChanged(affectedActor);
            }
            return num;
        }

        public void Clear()
        {
            foreach (Condition condition in this.All.ToTemporaryList<Condition>())
            {
                this.RemoveCondition(condition);
            }
        }

        public void AddClonedFrom(Conditions other)
        {
            if (other == null)
            {
                Log.Error("Tried to add cloned Conditions from null Conditions.", false);
                return;
            }
            if (other == this)
            {
                Log.Error("Tried to add cloned Conditions from self.", false);
                return;
            }
            List<Condition> all = other.All;
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i] == null)
                {
                    Log.Error("Tried to add null Condition in AddClonedFrom().", false);
                }
                else
                {
                    this.AddCondition(all[i].Clone(), -1);
                }
            }
        }

        public Condition GetFirstOfSpec(ConditionSpec spec)
        {
            for (int i = 0; i < this.conditions.Count; i++)
            {
                if (this.conditions[i].Spec == spec)
                {
                    return this.conditions[i];
                }
            }
            return null;
        }

        public bool AnyOfSpec(ConditionSpec spec)
        {
            return this.GetFirstOfSpec(spec) != null;
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.conditions.RemoveAll((Condition x) => x.Spec == null) != 0)
            {
                Log.Error("Removed some conditions with null spec from Conditions.", false);
            }
        }

        [Saved]
        private Entity parent;

        [Saved(Default.New, true)]
        private List<Condition> conditions = new List<Condition>();

        private List<ConditionDrawRequest> tmpDrawRequests = new List<ConditionDrawRequest>();
    }
}