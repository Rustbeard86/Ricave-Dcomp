using System;

namespace Ricave.Core
{
    public abstract class Spec
    {
        public string SpecID
        {
            get
            {
                return this.specID;
            }
        }

        public string Label
        {
            get
            {
                return this.label;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public int MyStableHash
        {
            get
            {
                int? num = this.myStableHash;
                if (num == null)
                {
                    int? num2 = (this.myStableHash = new int?(this.specID.StableHashCode() ^ base.GetType().Name.StableHashCode()));
                    return num2.Value;
                }
                return num.GetValueOrDefault();
            }
        }

        public Mod ModSource
        {
            get
            {
                return this.modSource;
            }
            set
            {
                this.modSource = value;
            }
        }

        public string FileSourceRelPath
        {
            get
            {
                return this.fileSourceRelPath;
            }
            set
            {
                this.fileSourceRelPath = value;
            }
        }

        public string LabelCap
        {
            get
            {
                if (this.labelCapCached == null && this.label != null)
                {
                    this.labelCapCached = this.label.CapitalizeFirst();
                }
                return this.labelCapCached;
            }
        }

        public virtual void Destroy()
        {
            this.specID += "_DESTROYED";
            if (this.label == null)
            {
                this.label = "[DESTROYED]";
            }
            else
            {
                this.label += " [DESTROYED]";
            }
            this.labelCapCached = null;
        }

        public virtual void OnActiveLanguageChanged()
        {
        }

        public override string ToString()
        {
            return this.specID;
        }

        [Saved("UnnamedSpec", false)]
        private string specID = "UnnamedSpec";

        [Saved]
        [Translatable]
        private string label;

        [Saved]
        [Translatable]
        private string description;

        private Mod modSource;

        private string fileSourceRelPath;

        private string labelCapCached;

        private int? myStableHash;
    }
}