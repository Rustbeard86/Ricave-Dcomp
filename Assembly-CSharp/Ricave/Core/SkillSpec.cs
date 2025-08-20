using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class SkillSpec : Spec, ITipSubject, ISaveableEventsReceiver
    {
        public string IconPath
        {
            get
            {
                return this.iconPath;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
        }

        public Color IconColor
        {
            get
            {
                return Color.white;
            }
        }

        public SkillCategory Category
        {
            get
            {
                return this.category;
            }
        }

        public List<SkillSpec> Prerequisites
        {
            get
            {
                return this.prerequisites;
            }
        }

        public Vector2 Position
        {
            get
            {
                return this.position;
            }
        }

        public bool PrerequisiteUnlocked
        {
            get
            {
                if (this.prerequisites.Count == 0)
                {
                    return true;
                }
                foreach (SkillSpec skillSpec in this.prerequisites)
                {
                    if (Get.SkillManager.IsUnlocked(skillSpec))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public string PrerequisitesLabel
        {
            get
            {
                if (this.prerequisitesLabelCached == null)
                {
                    if (this.prerequisites.Count == 0)
                    {
                        this.prerequisitesLabelCached = "NoneLower".Translate();
                    }
                    else
                    {
                        this.prerequisitesLabelCached = StringUtility.ToCommaListOr(this.prerequisites.Select<SkillSpec, string>((SkillSpec x) => x.LabelCap));
                    }
                }
                return this.prerequisitesLabelCached;
            }
        }

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
                return;
            }
            Log.Error("SkillSpec " + base.SpecID + " has no icon.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string iconPath;

        [Saved]
        private SkillCategory category;

        [Saved(Default.New, true)]
        private List<SkillSpec> prerequisites = new List<SkillSpec>();

        [Saved]
        private Vector2 position;

        private Texture2D icon;

        private string prerequisitesLabelCached;
    }
}