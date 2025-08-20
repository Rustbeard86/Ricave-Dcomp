using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ricave.Core
{
    public static class TranslatableUtility
    {
        public static void ClearCache()
        {
            TranslatableUtility.cachedTranslatableAttributes.Clear();
        }

        public static Translatable GetTranslatableAttribute(this FieldInfo fieldInfo)
        {
            Translatable translatable;
            if (TranslatableUtility.cachedTranslatableAttributes.TryGetValue(fieldInfo, out translatable))
            {
                return translatable;
            }
            object[] customAttributes = fieldInfo.GetCustomAttributes(false);
            for (int i = 0; i < customAttributes.Length; i++)
            {
                Translatable translatable2 = customAttributes[i] as Translatable;
                if (translatable2 != null)
                {
                    TranslatableUtility.cachedTranslatableAttributes.Add(fieldInfo, translatable2);
                    return translatable2;
                }
            }
            TranslatableUtility.cachedTranslatableAttributes.Add(fieldInfo, null);
            return null;
        }

        private static Dictionary<FieldInfo, Translatable> cachedTranslatableAttributes = new Dictionary<FieldInfo, Translatable>();
    }
}