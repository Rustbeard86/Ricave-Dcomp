using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class TiledDecalsSpec : Spec, ISaveableEventsReceiver
    {
        public string MaterialPath
        {
            get
            {
                return this.materialPath;
            }
        }

        public int Order
        {
            get
            {
                return this.order;
            }
        }

        public bool AllowOnCeiling
        {
            get
            {
                return this.allowOnCeiling;
            }
        }

        public bool AllowOnFloor
        {
            get
            {
                return this.allowOnFloor;
            }
        }

        public bool AllowOnWalls
        {
            get
            {
                return this.allowOnWalls;
            }
        }

        public bool OnlyIfForced
        {
            get
            {
                return this.onlyIfForced;
            }
        }

        public bool DisallowInLobby
        {
            get
            {
                return this.disallowInLobby;
            }
        }

        public float TexCoordsScale
        {
            get
            {
                return this.texCoordsScale;
            }
        }

        public List<EntitySpec> CanAppearOnEntities
        {
            get
            {
                return this.canAppearOnEntities;
            }
        }

        public bool CanTransitionToFilledEntities
        {
            get
            {
                return this.canTransitionToFilledEntities;
            }
        }

        public Material Material
        {
            get
            {
                return this.material;
            }
        }

        public bool CanAppearOnEntity(EntitySpec entitySpec)
        {
            return this.canAppearOnEntitiesHashSet.Contains(entitySpec);
        }

        public void OnLoaded()
        {
            this.canAppearOnEntitiesHashSet = new HashSet<EntitySpec>(this.canAppearOnEntities);
            if (!this.materialPath.NullOrEmpty())
            {
                this.material = Assets.Get<Material>(this.materialPath);
                return;
            }
            Log.Error("TiledDecalsSpec " + base.SpecID + " has no material path.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string materialPath;

        [Saved]
        private int order;

        [Saved]
        private bool allowOnCeiling;

        [Saved]
        private bool allowOnFloor;

        [Saved]
        private bool allowOnWalls;

        [Saved]
        private bool onlyIfForced;

        [Saved]
        private bool disallowInLobby;

        [Saved(0.77f, false)]
        private float texCoordsScale = 0.77f;

        [Saved]
        private List<EntitySpec> canAppearOnEntities;

        [Saved(true, false)]
        private bool canTransitionToFilledEntities = true;

        private Material material;

        private HashSet<EntitySpec> canAppearOnEntitiesHashSet;
    }
}