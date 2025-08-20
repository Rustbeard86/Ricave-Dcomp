using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ricave.Core
{
    public static class SaveableUtility
    {
        public static void ClearCache()
        {
            SaveableUtility.cachedAllSaveableFields.Clear();
            SaveableUtility.cachedSavedAttributes.Clear();
        }

        public static FieldInfo[] GetAllSavedFields(this Type type)
        {
            FieldInfo[] array;
            if (SaveableUtility.cachedAllSaveableFields.TryGetValue(type, out array))
            {
                return array;
            }
            FieldInfo[] array2 = (from x in type.GetAllFieldsIncludingBaseTypes()
                                  where x.GetSavedAttribute() != null
                                  select x).ToArray<FieldInfo>();
            SaveableUtility.cachedAllSaveableFields.Add(type, array2);
            return array2;
        }

        public static Saved GetSavedAttribute(this FieldInfo fieldInfo)
        {
            Saved saved;
            if (SaveableUtility.cachedSavedAttributes.TryGetValue(fieldInfo, out saved))
            {
                return saved;
            }
            object[] customAttributes = fieldInfo.GetCustomAttributes(false);
            for (int i = 0; i < customAttributes.Length; i++)
            {
                Saved saved2 = customAttributes[i] as Saved;
                if (saved2 != null)
                {
                    SaveableUtility.cachedSavedAttributes.Add(fieldInfo, saved2);
                    return saved2;
                }
            }
            SaveableUtility.cachedSavedAttributes.Add(fieldInfo, null);
            return null;
        }

        private static Dictionary<Type, FieldInfo[]> cachedAllSaveableFields = new Dictionary<Type, FieldInfo[]>();

        private static Dictionary<FieldInfo, Saved> cachedSavedAttributes = new Dictionary<FieldInfo, Saved>();

        public const string NullAttribute = "Null";

        public const string RefAttribute = "Ref";

        public const string TypeAttribute = "Type";

        public const string LengthAttribute = "Length";

        public const string CompressedNodeName = "_compressed";
    }
}