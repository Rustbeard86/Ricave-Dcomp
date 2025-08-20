using System;

namespace Ricave.Core
{
    public class Instruction_Descend : Instruction
    {
        protected Instruction_Descend()
        {
        }

        public Instruction_Descend(Place nextPlace, PlaceLink usedLink, bool usedDescendTrigger)
        {
            this.nextPlace = nextPlace;
            this.usedLink = usedLink;
            this.usedDescendTrigger = usedDescendTrigger;
            if (usedLink != null && usedLink.To != nextPlace)
            {
                Log.Error("Used link destination place doesn't match nextPlace.", false);
            }
        }

        protected override void DoImpl()
        {
            int worldSeed = WorldGenUtility.CreateSeedForWorld(Get.RunSeed, this.nextPlace);
            Get.ScreenFader.FadeOutAndExecute(delegate
            {
                Run run = Get.Run;
                Place place = this.nextPlace;
                run.ReloadScene(RunOnSceneChanged.GenerateWorld(new WorldConfig(((place != null) ? place.Spec.WorldSpec : null) ?? Get.RunSpec.DefaultWorldSpec, worldSeed, this.nextPlace, this.usedLink, null, this.usedDescendTrigger)), !this.usedDescendTrigger);
            }, new float?(2.5f), false, true, false);
        }

        protected override void UndoImpl()
        {
            Log.Error("Tried to undo descend instruction.", false);
        }

        [Saved]
        private Place nextPlace;

        [Saved]
        private PlaceLink usedLink;

        [Saved]
        private bool usedDescendTrigger;
    }
}