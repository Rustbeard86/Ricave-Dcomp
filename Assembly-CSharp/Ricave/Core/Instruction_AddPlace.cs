using System;

namespace Ricave.Core
{
    public class Instruction_AddPlace : Instruction
    {
        public Place Place
        {
            get
            {
                return this.place;
            }
        }

        protected Instruction_AddPlace()
        {
        }

        public Instruction_AddPlace(Place place)
        {
            this.place = place;
        }

        protected override void DoImpl()
        {
            Get.PlaceManager.AddPlace(this.place, -1);
        }

        protected override void UndoImpl()
        {
            Get.PlaceManager.RemovePlace(this.place);
        }

        [Saved]
        private Place place;
    }
}