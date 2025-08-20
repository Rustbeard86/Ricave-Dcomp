using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class BurrowManager
    {
        public List<BurrowManager.BurrowedActor> BurrowedActors
        {
            get
            {
                return this.burrowedActors;
            }
        }

        public void AddBurrowedActor(Actor actor)
        {
            if (actor == null)
            {
                Log.Error("Tried to add null burrowed actor.", false);
                return;
            }
            this.burrowedActors.Add(new BurrowManager.BurrowedActor(actor, Get.TurnManager.CurrentSequence, actor.Position));
        }

        public ValueTuple<BurrowManager.BurrowedActor, int> RemoveBurrowedActor(Actor actor)
        {
            for (int i = 0; i < this.burrowedActors.Count; i++)
            {
                if (this.burrowedActors[i].Actor == actor)
                {
                    BurrowManager.BurrowedActor burrowedActor = this.burrowedActors[i];
                    this.burrowedActors.RemoveAt(i);
                    return new ValueTuple<BurrowManager.BurrowedActor, int>(burrowedActor, i);
                }
            }
            Log.Error("Could not find burrowed actor to remove.", false);
            return default(ValueTuple<BurrowManager.BurrowedActor, int>);
        }

        public void InsertBurrowedActor(BurrowManager.BurrowedActor burrowedActor, int index)
        {
            if (burrowedActor == null)
            {
                Log.Error("Tried to insert null burrowed actor.", false);
                return;
            }
            if (index < 0 || index > this.burrowedActors.Count)
            {
                Log.Error("Tried to insert burrowed actor at invalid index.", false);
                return;
            }
            this.burrowedActors.Insert(index, burrowedActor);
        }

        [Saved(Default.New, true)]
        private List<BurrowManager.BurrowedActor> burrowedActors = new List<BurrowManager.BurrowedActor>();

        public class BurrowedActor
        {
            public Actor Actor
            {
                get
                {
                    return this.actor;
                }
            }

            public int BurrowSequence
            {
                get
                {
                    return this.burrowSequence;
                }
            }

            public Vector3Int BurrowedAt
            {
                get
                {
                    return this.burrowedAt;
                }
            }

            protected BurrowedActor()
            {
            }

            public BurrowedActor(Actor actor, int burrowSequence, Vector3Int burrowedAt)
            {
                this.actor = actor;
                this.burrowSequence = burrowSequence;
                this.burrowedAt = burrowedAt;
            }

            [Saved]
            private Actor actor;

            [Saved]
            private int burrowSequence;

            [Saved]
            private Vector3Int burrowedAt;
        }
    }
}