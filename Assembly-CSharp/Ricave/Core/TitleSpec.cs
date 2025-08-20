using System;

namespace Ricave.Core
{
    public class TitleSpec : Spec
    {
        public string LabelFormat
        {
            get
            {
                return this.labelFormat;
            }
        }

        [Saved]
        [Translatable]
        private string labelFormat;
    }
}