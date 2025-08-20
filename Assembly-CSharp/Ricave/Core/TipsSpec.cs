using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class TipsSpec : Spec
    {
        public List<string> Tips
        {
            get
            {
                return this.tips;
            }
        }

        [Saved(Default.New, false)]
        [Translatable]
        private List<string> tips = new List<string>();
    }
}