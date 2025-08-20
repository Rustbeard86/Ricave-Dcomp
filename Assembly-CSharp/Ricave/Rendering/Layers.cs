using System;
using UnityEngine;

namespace Ricave.Rendering
{
    public class Layers
    {
        public int DefaultLayer
        {
            get
            {
                return this.defaultLayer;
            }
        }

        public int PlayerLayer
        {
            get
            {
                return this.playerLayer;
            }
        }

        public int PlayerMask
        {
            get
            {
                return this.playerMask;
            }
        }

        public int FloorMarkersLayer
        {
            get
            {
                return this.floorMarkersLayer;
            }
        }

        public int FloorMarkersMask
        {
            get
            {
                return this.floorMarkersMask;
            }
        }

        public int IgnoreRaycastLayer
        {
            get
            {
                return this.ignoreRaycastLayer;
            }
        }

        public int IgnoreRaycastMask
        {
            get
            {
                return this.ignoreRaycastMask;
            }
        }

        public int ForHighlightLayer
        {
            get
            {
                return this.forHighlightLayer;
            }
        }

        public int ForIconRendererLayer
        {
            get
            {
                return this.forIconRendererLayer;
            }
        }

        public int ForIconRendererMask
        {
            get
            {
                return this.forIconRendererMask;
            }
        }

        public int ActorLayer
        {
            get
            {
                return this.actorLayer;
            }
        }

        public int ActorMask
        {
            get
            {
                return this.actorMask;
            }
        }

        public int InspectModeOnlyLayer
        {
            get
            {
                return this.inspectModeOnlyLayer;
            }
        }

        public int InspectModeOnlyMask
        {
            get
            {
                return this.inspectModeOnlyMask;
            }
        }

        public void Init()
        {
            this.defaultLayer = LayerMask.NameToLayer("Default");
            this.playerLayer = LayerMask.NameToLayer("Player");
            this.playerMask = LayerMask.GetMask(new string[] { "Player" });
            this.floorMarkersLayer = LayerMask.NameToLayer("FloorMarker");
            this.floorMarkersMask = LayerMask.GetMask(new string[] { "FloorMarker" });
            this.ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
            this.ignoreRaycastMask = LayerMask.GetMask(new string[] { "Ignore Raycast" });
            this.forHighlightLayer = LayerMask.NameToLayer("ForHighlight");
            this.forIconRendererLayer = LayerMask.NameToLayer("ForIconRenderer");
            this.forIconRendererMask = LayerMask.GetMask(new string[] { "ForIconRenderer" });
            this.actorLayer = LayerMask.NameToLayer("Actor");
            this.actorMask = LayerMask.GetMask(new string[] { "Actor" });
            this.inspectModeOnlyLayer = LayerMask.NameToLayer("InspectModeOnly");
            this.inspectModeOnlyMask = LayerMask.GetMask(new string[] { "InspectModeOnly" });
        }

        private int defaultLayer;

        private int playerLayer;

        private int playerMask;

        private int floorMarkersLayer;

        private int floorMarkersMask;

        private int ignoreRaycastLayer;

        private int ignoreRaycastMask;

        private int forHighlightLayer;

        private int forIconRendererLayer;

        private int forIconRendererMask;

        private int actorLayer;

        private int actorMask;

        private int inspectModeOnlyLayer;

        private int inspectModeOnlyMask;
    }
}