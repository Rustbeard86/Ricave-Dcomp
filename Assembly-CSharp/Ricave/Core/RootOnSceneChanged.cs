using System;

namespace Ricave.Core
{
    public class RootOnSceneChanged
    {
        public string RunToLoad
        {
            get
            {
                return this.runToLoad;
            }
        }

        public RunConfig RunToStart
        {
            get
            {
                return this.runToStart;
            }
        }

        private RootOnSceneChanged()
        {
        }

        public static RootOnSceneChanged Nothing()
        {
            return new RootOnSceneChanged();
        }

        public static RootOnSceneChanged LoadRun(string runToLoad)
        {
            return new RootOnSceneChanged
            {
                runToLoad = runToLoad
            };
        }

        public static RootOnSceneChanged StartNewRun(RunConfig config)
        {
            return new RootOnSceneChanged
            {
                runToStart = config
            };
        }

        private string runToLoad;

        private RunConfig runToStart;
    }
}