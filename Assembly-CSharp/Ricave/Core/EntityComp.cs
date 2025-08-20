using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public abstract class EntityComp : ISaveableEventsReceiver
    {
        public Entity Parent
        {
            get
            {
                return this.parent;
            }
        }

        public EntityCompProps Props
        {
            get
            {
                return this.props;
            }
        }

        public EntityGOC EntityGOC
        {
            get
            {
                return this.parent.EntityGOC;
            }
        }

        public virtual string ExtraTip
        {
            get
            {
                return null;
            }
        }

        protected EntityComp()
        {
        }

        public EntityComp(Entity parent)
        {
            this.parent = parent;
            this.FindProps();
        }

        public virtual void OnSpawned()
        {
        }

        public virtual void OnDeSpawned()
        {
        }

        public virtual void OnChangedGameObjectActiveStatus()
        {
        }

        public virtual void OnSetGameObjectAfterLoading()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual IEnumerable<Instruction> MakeResolveStructureInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public virtual IEnumerable<Instruction> MakeActorDestroyedHereInstructions(Actor actor)
        {
            return Enumerable.Empty<Instruction>();
        }

        public virtual IEnumerable<Instruction> MakeParentDestroyedInstructions(Vector3Int? posBeforeDestroy)
        {
            return Enumerable.Empty<Instruction>();
        }

        public void OnSaved()
        {
        }

        public virtual void OnLoaded()
        {
            this.FindProps();
        }

        private void FindProps()
        {
            List<EntityCompProps> allCompProps = this.parent.Spec.AllCompProps;
            for (int i = 0; i < allCompProps.Count; i++)
            {
                EntityCompProps entityCompProps = allCompProps[i];
                if (entityCompProps.CompClass == base.GetType())
                {
                    this.props = entityCompProps;
                    break;
                }
            }
            if (this.props == null)
            {
                Log.Error("Could not find corresponding entity comp props.", false);
            }
        }

        public override string ToString()
        {
            string name = base.GetType().Name;
            string text = " (";
            string text2;
            if (this.parent == null)
            {
                text2 = "no parent";
            }
            else
            {
                string text3 = "comp of ";
                Entity entity = this.parent;
                text2 = text3 + ((entity != null) ? entity.ToString() : null);
            }
            return name + text + text2 + ")";
        }

        [Saved]
        private Entity parent;

        private EntityCompProps props;
    }
}