using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_DungeonMap : Window
    {
        public Window_DungeonMap(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            DungeonMapDrawer.DrawMap(inRect, ref this.scrolled);
        }

        private bool scrolled;
    }
}