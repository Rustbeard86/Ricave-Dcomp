using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class SectionRendererGOC : MonoBehaviour
    {
        public CombinedFogOfWarMeshes CombinedFogOfWarMeshes
        {
            get
            {
                return this.combinedFogOfWarMeshes;
            }
        }

        public CombinedMeshes CombinedMeshes
        {
            get
            {
                return this.combinedMeshes;
            }
        }

        public CombinedTiledDecalsMeshes CombinedTiledDecalsMeshes
        {
            get
            {
                return this.combinedTiledDecalsMeshes;
            }
        }

        public SplatsManager SplatsManager
        {
            get
            {
                return this.splatsManager;
            }
        }

        private void Awake()
        {
            this.combinedFogOfWarMeshes.Init();
        }

        private void Update()
        {
            Profiler.Begin("combinedMeshes");
            try
            {
                this.combinedMeshes.Update();
            }
            finally
            {
                Profiler.End();
            }
            Profiler.Begin("combinedFogOfWarMeshes");
            try
            {
                this.combinedFogOfWarMeshes.Update();
            }
            finally
            {
                Profiler.End();
            }
            Profiler.Begin("combinedTiledDecalsMeshes");
            try
            {
                this.combinedTiledDecalsMeshes.Update();
            }
            finally
            {
                Profiler.End();
            }
            Profiler.Begin("splatsManager");
            try
            {
                this.splatsManager.Update();
            }
            finally
            {
                Profiler.End();
            }
        }

        private CombinedFogOfWarMeshes combinedFogOfWarMeshes = new CombinedFogOfWarMeshes();

        private CombinedMeshes combinedMeshes = new CombinedMeshes();

        private CombinedTiledDecalsMeshes combinedTiledDecalsMeshes = new CombinedTiledDecalsMeshes();

        private SplatsManager splatsManager = new SplatsManager();
    }
}