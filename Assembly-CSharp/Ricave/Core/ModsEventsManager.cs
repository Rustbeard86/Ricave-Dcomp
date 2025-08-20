using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class ModsEventsManager
    {
        public ModsEventsManager()
        {
            this.CreateEventSubscribersLists();
            this.CreateEventWithInstructionsSubscribersLists();
        }

        private void CreateEventSubscribersLists()
        {
            int maxValue = EnumUtility.GetMaxValue<ModEventType>();
            this.eventSubscribers = new List<Action<object>>[maxValue + 1];
            for (int i = 0; i < this.eventSubscribers.Length; i++)
            {
                this.eventSubscribers[i] = new List<Action<object>>();
            }
        }

        private void CreateEventWithInstructionsSubscribersLists()
        {
            int maxValue = EnumUtility.GetMaxValue<ModEventWithInstructionsType>();
            this.eventWithInstructionsSubscribers = new List<Func<object, IEnumerable<Instruction>>>[maxValue + 1];
            for (int i = 0; i < this.eventWithInstructionsSubscribers.Length; i++)
            {
                this.eventWithInstructionsSubscribers[i] = new List<Func<object, IEnumerable<Instruction>>>();
            }
        }

        public void AskAllActiveModsToSubscribeToEvents()
        {
            this.ClearEventSubscribers();
            foreach (Mod mod in Get.ModManager.ActiveMods)
            {
                try
                {
                    mod.GetOrCreateEventsHandler().SubscribeToEvents(this);
                }
                catch (Exception ex)
                {
                    Log.Error("Error in ModEventsHandler.SubscribeToEvents().", ex);
                }
            }
        }

        public void ClearEventSubscribers()
        {
            List<Action<object>>[] array = this.eventSubscribers;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Clear();
            }
            List<Func<object, IEnumerable<Instruction>>>[] array2 = this.eventWithInstructionsSubscribers;
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i].Clear();
            }
        }

        public void CallEvent(ModEventType eventType, object arg = null)
        {
            List<Action<object>> list = this.eventSubscribers[(int)eventType];
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    list[i](arg);
                }
                catch (Exception ex)
                {
                    Log.Error("Error in mod for event type " + eventType.ToString() + ".", ex);
                }
            }
        }

        public IEnumerable<Instruction> CallEvent(ModEventWithInstructionsType eventType, object arg = null)
        {
            if (this.eventWithInstructionsSubscribers[(int)eventType].Count != 0)
            {
                return this.CallEventImpl(eventType, arg);
            }
            return Enumerable.Empty<Instruction>();
        }

        private IEnumerable<Instruction> CallEventImpl(ModEventWithInstructionsType eventType, object arg = null)
        {
            List<Func<object, IEnumerable<Instruction>>> subscribers = this.eventWithInstructionsSubscribers[(int)eventType];
            int num;
            for (int i = 0; i < subscribers.Count; i = num + 1)
            {
                IEnumerable<Instruction> enumerable = null;
                try
                {
                    enumerable = subscribers[i](arg);
                }
                catch (Exception ex)
                {
                    Log.Error("Error in mod for event type " + eventType.ToString() + ".", ex);
                }
                if (enumerable != null)
                {
                    foreach (Instruction instruction in enumerable)
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
                num = i;
            }
            yield break;
            yield break;
        }

        public void Subscribe(ModEventType eventType, Action<object> action)
        {
            if (action == null)
            {
                Log.Error("Tried to add null action for event type " + eventType.ToString() + " in ModsEventsManager.", false);
                return;
            }
            this.eventSubscribers[(int)eventType].Add(action);
        }

        public void Subscribe(ModEventWithInstructionsType eventType, Func<object, IEnumerable<Instruction>> action)
        {
            if (action == null)
            {
                Log.Error("Tried to add null action for event type " + eventType.ToString() + " in ModsEventsManager.", false);
                return;
            }
            this.eventWithInstructionsSubscribers[(int)eventType].Add(action);
        }

        private List<Action<object>>[] eventSubscribers;

        private List<Func<object, IEnumerable<Instruction>>>[] eventWithInstructionsSubscribers;
    }
}