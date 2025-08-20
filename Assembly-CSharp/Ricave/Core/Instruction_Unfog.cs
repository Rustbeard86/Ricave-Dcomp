using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_Unfog : Instruction
    {
        public List<Vector3Int> Cells
        {
            get
            {
                return this.cells;
            }
        }

        protected Instruction_Unfog()
        {
        }

        public Instruction_Unfog(List<Vector3Int> cells)
        {
            this.cells = cells;
        }

        protected override void DoImpl()
        {
            Get.FogOfWar.Unfog(this.cells);
        }

        protected override void UndoImpl()
        {
            Get.FogOfWar.SetFogged(this.cells);
        }

        [Saved]
        private List<Vector3Int> cells;
    }
}