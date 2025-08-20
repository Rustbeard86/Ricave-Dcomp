using System;

namespace Ricave.Core
{
    public class Instruction_RemovePlace : Instruction
    {
        public Place Place
        {
            get
            {
                return this.place;
            }
        }

        protected Instruction_RemovePlace()
        {
        }

        public Instruction_RemovePlace(Place place)
        {
            this.place = place;
        }

        protected override void DoImpl()
        {
            this.removedFromIndex = Get.PlaceManager.RemovePlace(this.place);
        }

        protected override void UndoImpl()
        {
            Get.PlaceManager.AddPlace(this.place, this.removedFromIndex);
        }

        [Saved]
        private Place place;

        [Saved]
        private int removedFromIndex;
    }
}