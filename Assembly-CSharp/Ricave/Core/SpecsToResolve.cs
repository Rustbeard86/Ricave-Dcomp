using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class SpecsToResolve
    {
        public bool IsEmpty
        {
            get
            {
                return this.specs.Count == 0 && this.toNotifyAfterResolved.Count == 0;
            }
        }

        public void Add(SpecToResolve spec)
        {
            this.specs.Add(spec);
        }

        public void NotifyLoadedAfterResolved(List<ISaveableEventsReceiver> toNotify)
        {
            this.toNotifyAfterResolved.AddRange(toNotify);
            toNotify.Clear();
        }

        public void MergeAndClaim(SpecsToResolve other)
        {
            if (other == null)
            {
                return;
            }
            this.specs.AddRange(other.specs);
            other.specs.Clear();
            this.toNotifyAfterResolved.AddRange(other.toNotifyAfterResolved);
            other.toNotifyAfterResolved.Clear();
        }

        public void Clear()
        {
            this.specs.Clear();
        }

        public void Resolve()
        {
            for (int i = this.specs.Count - 1; i >= 0; i--)
            {
                this.Resolve(this.specs[i]);
                this.specs.RemoveAt(i);
            }
            for (int j = 0; j < this.toNotifyAfterResolved.Count; j++)
            {
                try
                {
                    this.toNotifyAfterResolved[j].OnLoaded();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in OnLoaded().", ex);
                }
            }
            this.toNotifyAfterResolved.Clear();
        }

        private void Resolve(SpecToResolve toResolve)
        {
            Spec spec;
            if (Get.Specs.TryGet(toResolve.SpecType, toResolve.SpecID, out spec))
            {
                toResolve.Assign(spec);
                return;
            }
            string[] array = new string[5];
            array[0] = "Could not resolve spec named \"";
            array[1] = toResolve.SpecID;
            array[2] = "\" of type ";
            int num = 3;
            Type specType = toResolve.SpecType;
            array[num] = ((specType != null) ? specType.ToString() : null);
            array[4] = ".";
            Log.Error(string.Concat(array), false);
        }

        private List<SpecToResolve> specs = new List<SpecToResolve>();

        private List<ISaveableEventsReceiver> toNotifyAfterResolved = new List<ISaveableEventsReceiver>();
    }
}