using System;
using UnityEngine;

namespace Ricave.Core
{
    public class SavedCameraPosition
    {
        public Quaternion? ActorTargetRot
        {
            get
            {
                return this.actorTargetRot;
            }
            set
            {
                this.actorTargetRot = value;
            }
        }

        public Quaternion? CameraTargetRot
        {
            get
            {
                return this.cameraTargetRot;
            }
            set
            {
                this.cameraTargetRot = value;
            }
        }

        public void OnWorldGenerated()
        {
            this.actorTargetRot = null;
            this.cameraTargetRot = null;
        }

        [Saved]
        private Quaternion? actorTargetRot;

        [Saved]
        private Quaternion? cameraTargetRot;
    }
}