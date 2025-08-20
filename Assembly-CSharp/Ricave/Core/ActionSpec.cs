using System;

namespace Ricave.Core
{
    public class ActionSpec : Spec
    {
        public Type ActionClass
        {
            get
            {
                return this.actionClass;
            }
        }

        public string DescriptionVerbose
        {
            get
            {
                return this.descriptionVerbose;
            }
        }

        [Saved(typeof(Action), false)]
        private Type actionClass = typeof(Action);

        [Saved]
        [Translatable]
        private string descriptionVerbose;
    }
}