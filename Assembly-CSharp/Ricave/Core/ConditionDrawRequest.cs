using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public struct ConditionDrawRequest : ITipSubject
    {
        public string LabelCap
        {
            get
            {
                if (this.labelCapGetter == null)
                {
                    return this.labelCap;
                }
                return this.labelCapGetter();
            }
        }

        public Func<string> TipGetter
        {
            get
            {
                return this.tipGetter;
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
                return this.iconColor;
            }
        }

        public float TimeStartedAffectingActor
        {
            get
            {
                return this.timeStartedAffectingActor;
            }
        }

        public int TurnsLeft
        {
            get
            {
                return this.turnsLeft;
            }
        }

        string ITipSubject.Description
        {
            get
            {
                return this.tipGetter();
            }
        }

        public ConditionDrawRequest(Condition condition)
        {
            this.labelCap = null;
            this.labelCapGetter = condition.CachedLabelCapGetter;
            this.tipGetter = condition.CachedTipGetter;
            this.icon = condition.Icon;
            this.iconColor = condition.IconColor;
            this.timeStartedAffectingActor = condition.TimeStartedAffectingActor;
            this.turnsLeft = condition.TurnsLeft;
            if (condition is Condition_Berserker)
            {
                this.timeStartedAffectingActor = -99999f;
            }
        }

        public ConditionDrawRequest(string labelCap, Func<string> tipGetter, Texture2D icon, Color iconColor, float timeStartedAffectingActor = -99999f)
        {
            this.labelCap = labelCap;
            this.labelCapGetter = null;
            this.tipGetter = tipGetter;
            this.icon = icon;
            this.iconColor = iconColor;
            this.timeStartedAffectingActor = timeStartedAffectingActor;
            this.turnsLeft = 0;
        }

        private string labelCap;

        private Func<string> labelCapGetter;

        private Func<string> tipGetter;

        private Texture2D icon;

        private Color iconColor;

        private float timeStartedAffectingActor;

        private int turnsLeft;
    }
}