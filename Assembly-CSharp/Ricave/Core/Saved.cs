using System;
using Steamworks;
using UnityEngine;

namespace Ricave.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class Saved : Attribute
    {
        public Default DefaultValueKind
        {
            get
            {
                return this.defaultValueKind;
            }
        }

        public object SpecificDefaultValue
        {
            get
            {
                return this.specificDefaultValue;
            }
        }

        public bool FilterNulls
        {
            get
            {
                return this.filterNulls;
            }
        }

        public Saved()
        {
        }

        public Saved(Default defaultValueKind, bool filterNulls = false)
        {
            this.defaultValueKind = defaultValueKind;
            this.filterNulls = filterNulls;
        }

        public Saved(object specificDefaultValue, bool filterNulls = false)
        {
            this.defaultValueKind = Default.SpecificValue;
            this.specificDefaultValue = specificDefaultValue;
            this.filterNulls = filterNulls;
        }

        public bool DefaultValueEquals(object obj, Type fieldType)
        {
            if (!fieldType.IsValueTypeCached())
            {
                return obj == null && (this.defaultValueKind == Default.Null || (this.defaultValueKind == Default.SpecificValue && this.specificDefaultValue == null));
            }
            return object.Equals(obj, this.GetCachedDefaultValueOrNew(fieldType));
        }

        public object GetCachedDefaultValueOrNew(Type fieldType)
        {
            switch (this.defaultValueKind)
            {
                case Default.Null:
                    return TypeUtility.GetCachedDefaultValue(fieldType);
                case Default.New:
                    return TypeUtility.Instantiate(fieldType);
                case Default.Vector2_One:
                    return Saved.Vector2OneBoxed;
                case Default.Vector3_One:
                    return Saved.Vector3OneBoxed;
                case Default.Vector3Int_Down:
                    return Saved.Vector3IntDownBoxed;
                case Default.Quaternion_Identity:
                    return Saved.QuaternionIdentityBoxed;
                case Default.Color_White:
                    return Saved.ColorWhiteBoxed;
                case Default.FloatRange_One:
                    return Saved.FloatRangeOneBoxed;
                case Default.PublishedFiledId_Invalid:
                    return Saved.PublishedFiledIdInvalidBoxed;
                case Default.SpecificValue:
                    return this.specificDefaultValue;
                default:
                    return TypeUtility.GetCachedDefaultValue(fieldType);
            }
        }

        private Default defaultValueKind;

        private object specificDefaultValue;

        private bool filterNulls;

        private static readonly object Vector2OneBoxed = Vector2.one;

        private static readonly object Vector3OneBoxed = Vector3.one;

        private static readonly object Vector3IntDownBoxed = Vector3IntUtility.Down;

        private static readonly object QuaternionIdentityBoxed = Quaternion.identity;

        private static readonly object ColorWhiteBoxed = Color.white;

        private static readonly object FloatRangeOneBoxed = new FloatRange(1f, 1f);

        private static readonly object PublishedFiledIdInvalidBoxed = PublishedFileId_t.Invalid.m_PublishedFileId;
    }
}