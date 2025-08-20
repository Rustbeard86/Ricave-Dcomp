using System;

namespace Ricave.Core
{
    public class Instruction_RemovePlaceLink : Instruction
    {
        public PlaceLink PlaceLink
        {
            get
            {
                return this.placeLink;
            }
        }

        protected Instruction_RemovePlaceLink()
        {
        }

        public Instruction_RemovePlaceLink(PlaceLink placeLink)
        {
            this.placeLink = placeLink;
        }

        protected override void DoImpl()
        {
            this.removedFromIndex = Get.PlaceManager.RemoveLink(this.placeLink);
        }

        protected override void UndoImpl()
        {
            Get.PlaceManager.AddLink(this.placeLink, this.removedFromIndex);
        }

        [Saved]
        private PlaceLink placeLink;

        [Saved]
        private int removedFromIndex;
    }
}