using System;

namespace Ricave.Core
{
    public class Instruction_AddPlaceLink : Instruction
    {
        public PlaceLink PlaceLink
        {
            get
            {
                return this.placeLink;
            }
        }

        protected Instruction_AddPlaceLink()
        {
        }

        public Instruction_AddPlaceLink(PlaceLink placeLink)
        {
            this.placeLink = placeLink;
        }

        protected override void DoImpl()
        {
            Get.PlaceManager.AddLink(this.placeLink, -1);
        }

        protected override void UndoImpl()
        {
            Get.PlaceManager.RemoveLink(this.placeLink);
        }

        [Saved]
        private PlaceLink placeLink;
    }
}