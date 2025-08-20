using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class LessonManager
    {
        public LessonSpec CurrentLesson
        {
            get
            {
                if (this.Finished)
                {
                    return null;
                }
                LessonSpec lessonSpec = this.cachedOrderedLessons[this.currentLessonIndex];
                if (lessonSpec.InLobbyOnly && !Get.InLobby)
                {
                    return null;
                }
                if (Get.InLobby && !lessonSpec.InLobbyOnly)
                {
                    return null;
                }
                return lessonSpec;
            }
        }

        public bool Finished
        {
            get
            {
                return this.currentLessonIndex >= this.cachedOrderedLessons.Count;
            }
        }

        public bool DespawnedGate
        {
            get
            {
                return this.despawnedGate;
            }
        }

        public bool TutorialAreaPassed
        {
            get
            {
                return this.tutorialAreaPassed;
            }
        }

        public LessonManager()
        {
            this.cachedOrderedLessons = new List<LessonSpec>();
            this.cachedOrderedLessons.AddRange(Get.Specs.GetAll<LessonSpec>());
            this.cachedOrderedLessons.StableSort<LessonSpec, float>((LessonSpec x) => x.Order);
        }

        public void FinishIfCurrent(LessonSpec lessonSpec)
        {
            if (this.CurrentLesson == lessonSpec)
            {
                this.currentLessonIndex++;
                Get.LessonDrawerGOC.OnLessonFinished();
                this.despawnedGate = false;
            }
        }

        public void OnTutorialAreaPassed()
        {
            this.tutorialAreaPassed = true;
        }

        public void OnTutorialGateDespawned()
        {
            this.despawnedGate = true;
        }

        public void ResetLessons()
        {
            this.currentLessonIndex = 0;
        }

        [Saved]
        private int currentLessonIndex;

        [Saved]
        private bool tutorialAreaPassed;

        [Saved]
        private bool despawnedGate;

        private List<LessonSpec> cachedOrderedLessons;
    }
}