using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public struct UseEffectDrawRequest : ITipSubject
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

        public List<UseEffectDrawRequest> InnerUseEffects
        {
            get
            {
                return this.innerUseEffects;
            }
        }

        string ITipSubject.Description
        {
            get
            {
                return this.tipGetter();
            }
        }

        public UseEffectDrawRequest(UseEffect useEffect)
        {
            this.labelCap = null;
            this.labelCapGetter = useEffect.CachedLabelCapGetter;
            this.tipGetter = useEffect.CachedTipGetter;
            this.icon = useEffect.Icon;
            this.iconColor = useEffect.IconColor;
            UseEffect_Spawn useEffect_Spawn = useEffect as UseEffect_Spawn;
            if (useEffect_Spawn != null && useEffect_Spawn.EntitySpec != null && useEffect_Spawn.EntitySpec.IsStructure && (useEffect_Spawn.EntitySpec.Structure.AutoUseOnActorsPassing || useEffect_Spawn.EntitySpec.Structure.AutoUseOnDestroyed) && useEffect_Spawn.EntitySpec.Structure.DefaultUseEffects != null && useEffect_Spawn.EntitySpec.Structure.DefaultUseEffects.Any && useEffect_Spawn.EntitySpec.Structure.IsCollidingFloorEmplacement)
            {
                this.innerUseEffects = useEffect_Spawn.EntitySpec.Structure.DefaultUseEffects.AllDrawRequests;
                return;
            }
            this.innerUseEffects = null;
        }

        public UseEffectDrawRequest(string labelCap, Func<string> tipGetter, Texture2D icon, Color iconColor)
        {
            this.labelCap = labelCap;
            this.labelCapGetter = null;
            this.tipGetter = tipGetter;
            this.icon = icon;
            this.iconColor = iconColor;
            this.innerUseEffects = null;
        }

        private string labelCap;

        private Func<string> labelCapGetter;

        private Func<string> tipGetter;

        private Texture2D icon;

        private Color iconColor;

        private List<UseEffectDrawRequest> innerUseEffects;
    }
}