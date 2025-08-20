using System;

namespace Ricave.Core
{
    public class RunOnSceneChanged
    {
        public WorldConfig WorldToGenerate
        {
            get
            {
                return this.worldToGenerate;
            }
        }

        private RunOnSceneChanged()
        {
        }

        public static RunOnSceneChanged GenerateWorld(WorldConfig worldToGenerate)
        {
            return new RunOnSceneChanged
            {
                worldToGenerate = worldToGenerate
            };
        }

        private WorldConfig worldToGenerate;
    }
}