using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ricave.Core
{
    public static class TypeUtility
    {
        public static List<Assembly> ActiveAssemblies
        {
            get
            {
                if (!TypeUtility.cachedAssemblies.Any())
                {
                    TypeUtility.cachedAssemblies.Add(Assembly.GetExecutingAssembly());
                    foreach (Mod mod in Get.ModManager.ActiveMods)
                    {
                        TypeUtility.cachedAssemblies.AddRange(mod.Assemblies);
                    }
                }
                return TypeUtility.cachedAssemblies;
            }
        }

        public static List<Type> AllTypes
        {
            get
            {
                if (!TypeUtility.cachedAllTypes.Any())
                {
                    foreach (Assembly assembly in TypeUtility.ActiveAssemblies)
                    {
                        try
                        {
                            TypeUtility.cachedAllTypes.AddRange(assembly.GetTypes());
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            string text = "Error while getting types from an assembly ";
                            Assembly assembly2 = assembly;
                            Log.Error(text + ((assembly2 != null) ? assembly2.ToString() : null), ex);
                        }
                    }
                }
                return TypeUtility.cachedAllTypes;
            }
        }

        public static void ClearCache()
        {
            TypeUtility.cachedAllFieldsIncludingBaseTypes.Clear();
            TypeUtility.cachedTypes.Clear();
            TypeUtility.cachedTypeAndBaseTypes.Clear();
            TypeUtility.cachedConstructors.Clear();
            TypeUtility.cachedAssemblies.Clear();
            TypeUtility.cachedAllTypes.Clear();
            TypeUtility.cachedSubclasses.Clear();
            TypeUtility.cachedNameOrFullName.Clear();
            TypeUtility.cachedRecursivelySaveableTypes.Clear();
            TypeUtility.cachedDefaultValuesForValueTypes.Clear();
            TypeUtility.cachedFields.Clear();
            TypeUtility.cachedIsHashSet.Clear();
            TypeUtility.cachedIsValueType.Clear();
            TypeUtility.cachedGenericArguments.Clear();
            TypeUtility.cachedIsGenericType.Clear();
            TypeUtility.cachedIsPrimitive.Clear();
            TypeUtility.cachedIsEnum.Clear();
            TypeUtility.cachedIsSpec.Clear();
            TypeUtility.cachedListTypeWithElementType.Clear();
            TypeUtility.cachedTypeNameHashes.Clear();
        }

        public static Type GetType(string typeName)
        {
            if (typeName.NullOrEmpty())
            {
                return null;
            }
            Type type;
            if (TypeUtility.cachedTypes.TryGetValue(typeName, out type))
            {
                return type;
            }
            Type typeRaw = TypeUtility.GetTypeRaw(typeName);
            TypeUtility.cachedTypes.Add(typeName, typeRaw);
            return typeRaw;
        }

        private static Type GetTypeRaw(string typeName)
        {
            if (typeName.NullOrEmpty())
            {
                return null;
            }
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }
            for (int i = 0; i < TypeUtility.IgnoredNamespaces.Length; i++)
            {
                type = Type.GetType(TypeUtility.IgnoredNamespaces[i] + "." + typeName);
                if (type != null)
                {
                    return type;
                }
            }
            foreach (Assembly assembly in TypeUtility.ActiveAssemblies)
            {
                type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
                for (int j = 0; j < TypeUtility.IgnoredNamespaces.Length; j++)
                {
                    type = assembly.GetType(TypeUtility.IgnoredNamespaces[j] + "." + typeName);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        public static Type[] AllSubclasses(this Type type)
        {
            Type[] array;
            if (TypeUtility.cachedSubclasses.TryGetValue(type, out array))
            {
                return array;
            }
            Type[] array2 = TypeUtility.AllTypes.Where<Type>((Type x) => x.IsSubclassOf(type)).ToArray<Type>();
            TypeUtility.cachedSubclasses.Add(type, array2);
            return array2;
        }

        public static ConstructorInfo GetDefaultConstructorCached(this Type type)
        {
            ConstructorInfo constructorInfo;
            if (TypeUtility.cachedConstructors.TryGetValue(type, out constructorInfo))
            {
                return constructorInfo;
            }
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            TypeUtility.cachedConstructors.Add(type, constructor);
            return constructor;
        }

        public static List<Type> GetTypeAndBaseTypesExceptObject(this Type type)
        {
            List<Type> list;
            if (TypeUtility.cachedTypeAndBaseTypes.TryGetValue(type, out list))
            {
                return list;
            }
            List<Type> list2 = new List<Type>();
            Type type2 = type;
            do
            {
                list2.Add(type2);
                type2 = type2.BaseType;
            }
            while (type2 != null && type2 != typeof(object));
            TypeUtility.cachedTypeAndBaseTypes.Add(type, list2);
            return list2;
        }

        public static FieldInfo[] GetAllFieldsIncludingBaseTypes(this Type type)
        {
            FieldInfo[] array;
            if (TypeUtility.cachedAllFieldsIncludingBaseTypes.TryGetValue(type, out array))
            {
                return array;
            }
            List<Type> typeAndBaseTypesExceptObject = type.GetTypeAndBaseTypesExceptObject();
            List<FieldInfo> list = new List<FieldInfo>(10);
            for (int i = typeAndBaseTypesExceptObject.Count - 1; i >= 0; i--)
            {
                foreach (FieldInfo fieldInfo in TypeUtility.GetFields(typeAndBaseTypesExceptObject[i]))
                {
                    bool flag = false;
                    for (int k = 0; k < list.Count; k++)
                    {
                        if (list[k].DeclaringType == fieldInfo.DeclaringType && list[k].Name == fieldInfo.Name)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        list.Add(fieldInfo);
                    }
                }
            }
            FieldInfo[] array2 = list.ToArray();
            TypeUtility.cachedAllFieldsIncludingBaseTypes.Add(type, array2);
            return array2;
        }

        public static string NameOrFullName(this Type type)
        {
            string text;
            if (TypeUtility.cachedNameOrFullName.TryGetValue(type, out text))
            {
                return text;
            }
            string @namespace = type.Namespace;
            for (int i = 0; i < TypeUtility.IgnoredNamespaces.Length; i++)
            {
                if (!(@namespace != TypeUtility.IgnoredNamespaces[i]))
                {
                    string name = type.Name;
                    TypeUtility.cachedNameOrFullName.Add(type, name);
                    return name;
                }
            }
            string fullName = type.FullName;
            TypeUtility.cachedNameOrFullName.Add(type, fullName);
            return fullName;
        }

        public static bool IsRecursivelySaveableType(this Type type)
        {
            bool flag;
            if (TypeUtility.cachedRecursivelySaveableTypes.TryGetValue(type, out flag))
            {
                return flag;
            }
            string @namespace = type.Namespace;
            bool flag2 = @namespace == "Ricave.Core" || @namespace == "Ricave.UI" || @namespace == "Ricave.Rendering" || @namespace == "Mods" || (@namespace != null && @namespace.StartsWith("Mods."));
            TypeUtility.cachedRecursivelySaveableTypes.Add(type, flag2);
            return flag2;
        }

        public static object GetCachedDefaultValue(Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (!type.IsValueTypeCached())
            {
                return null;
            }
            object obj;
            if (TypeUtility.cachedDefaultValuesForValueTypes.TryGetValue(type, out obj))
            {
                return obj;
            }
            obj = Activator.CreateInstance(type, true);
            TypeUtility.cachedDefaultValuesForValueTypes.Add(type, obj);
            return obj;
        }

        public static object Instantiate(Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (type.IsValueTypeCached())
            {
                return Activator.CreateInstance(Nullable.GetUnderlyingType(type) ?? type, true);
            }
            if (type == typeof(string))
            {
                return "";
            }
            if (type == typeof(Type))
            {
                return typeof(object);
            }
            if (type.IsArray)
            {
                int[] array = new int[type.GetArrayRank()];
                return Array.CreateInstance(type.GetElementType(), array);
            }
            if (type.IsAbstract)
            {
                Log.Error("Tried to instantiate object of abstract type \"" + type.Name + "\".", false);
                return null;
            }
            if (type.GetDefaultConstructorCached() == null)
            {
                Log.Error("Tried to instantiate object of type without default constructor \"" + type.Name + "\".", false);
                return null;
            }
            return Activator.CreateInstance(type, true);
        }

        private static FieldInfo[] GetFields(Type type)
        {
            FieldInfo[] array;
            if (TypeUtility.cachedFields.TryGetValue(type, out array))
            {
                return array;
            }
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            TypeUtility.cachedFields.Add(type, fields);
            return fields;
        }

        public static bool IsHashSetCached(this Type type)
        {
            bool flag;
            if (TypeUtility.cachedIsHashSet.TryGetValue(type, out flag))
            {
                return flag;
            }
            bool flag2 = type.IsGenericType && typeof(HashSet<>).IsAssignableFrom(type.GetGenericTypeDefinition());
            TypeUtility.cachedIsHashSet.Add(type, flag2);
            return flag2;
        }

        public static bool IsValueTypeCached(this Type type)
        {
            bool flag;
            if (TypeUtility.cachedIsValueType.TryGetValue(type, out flag))
            {
                return flag;
            }
            bool isValueType = type.IsValueType;
            TypeUtility.cachedIsValueType.Add(type, isValueType);
            return isValueType;
        }

        public static Type[] GetGenericArgumentsCached(this Type type)
        {
            Type[] genericArguments;
            if (TypeUtility.cachedGenericArguments.TryGetValue(type, out genericArguments))
            {
                return genericArguments;
            }
            genericArguments = type.GetGenericArguments();
            TypeUtility.cachedGenericArguments.Add(type, genericArguments);
            return genericArguments;
        }

        public static bool IsGenericTypeCached(this Type type)
        {
            bool flag;
            if (TypeUtility.cachedIsGenericType.TryGetValue(type, out flag))
            {
                return flag;
            }
            bool isGenericType = type.IsGenericType;
            TypeUtility.cachedIsGenericType.Add(type, isGenericType);
            return isGenericType;
        }

        public static bool IsPrimitiveCached(this Type type)
        {
            bool flag;
            if (TypeUtility.cachedIsPrimitive.TryGetValue(type, out flag))
            {
                return flag;
            }
            bool isPrimitive = type.IsPrimitive;
            TypeUtility.cachedIsPrimitive.Add(type, isPrimitive);
            return isPrimitive;
        }

        public static bool IsEnumCached(this Type type)
        {
            bool flag;
            if (TypeUtility.cachedIsEnum.TryGetValue(type, out flag))
            {
                return flag;
            }
            bool isEnum = type.IsEnum;
            TypeUtility.cachedIsEnum.Add(type, isEnum);
            return isEnum;
        }

        public static bool IsSpecCached(this Type type)
        {
            bool flag;
            if (TypeUtility.cachedIsSpec.TryGetValue(type, out flag))
            {
                return flag;
            }
            bool flag2 = typeof(Spec).IsAssignableFrom(type);
            TypeUtility.cachedIsSpec.Add(type, flag2);
            return flag2;
        }

        public static Type GetListTypeWithElementType(Type type)
        {
            Type type2;
            if (TypeUtility.cachedListTypeWithElementType.TryGetValue(type, out type2))
            {
                return type2;
            }
            type2 = typeof(List<>).MakeGenericType(new Type[] { type });
            TypeUtility.cachedListTypeWithElementType.Add(type, type2);
            return type2;
        }

        public static int TypeNameStableHashCode(this Type type)
        {
            if (type == null)
            {
                return 0;
            }
            int num;
            if (TypeUtility.cachedTypeNameHashes.TryGetValue(type, out num))
            {
                return num;
            }
            int num2 = type.Name.StableHashCode();
            TypeUtility.cachedTypeNameHashes.Add(type, num2);
            return num2;
        }

        public static void PreJITMethods(Assembly assembly)
        {
            if (TypeUtility.preJITed)
            {
                return;
            }
            TypeUtility.preJITed = true;
            Profiler.Begin("Pre-JIT methods");
            try
            {
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (!methodInfo.IsAbstract && !methodInfo.ContainsGenericParameters)
                        {
                            try
                            {
                                RuntimeMethodHandle methodHandle = methodInfo.MethodHandle;
                                RuntimeHelpers.PrepareMethod(methodHandle);
                                methodHandle.GetFunctionPointer();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Error while pre-JITing method \"" + methodInfo.Name + "\".", ex);
                            }
                        }
                    }
                }
            }
            finally
            {
                Profiler.End();
            }
        }

        private static Dictionary<Type, FieldInfo[]> cachedAllFieldsIncludingBaseTypes = new Dictionary<Type, FieldInfo[]>(100);

        private static Dictionary<string, Type> cachedTypes = new Dictionary<string, Type>(100);

        private static Dictionary<Type, List<Type>> cachedTypeAndBaseTypes = new Dictionary<Type, List<Type>>(100);

        private static Dictionary<Type, ConstructorInfo> cachedConstructors = new Dictionary<Type, ConstructorInfo>(100);

        private static List<Assembly> cachedAssemblies = new List<Assembly>();

        private static List<Type> cachedAllTypes = new List<Type>(100);

        private static Dictionary<Type, Type[]> cachedSubclasses = new Dictionary<Type, Type[]>(100);

        private static Dictionary<Type, string> cachedNameOrFullName = new Dictionary<Type, string>(100);

        private static Dictionary<Type, bool> cachedRecursivelySaveableTypes = new Dictionary<Type, bool>(100);

        private static Dictionary<Type, object> cachedDefaultValuesForValueTypes = new Dictionary<Type, object>(100);

        private static Dictionary<Type, FieldInfo[]> cachedFields = new Dictionary<Type, FieldInfo[]>(100);

        private static Dictionary<Type, bool> cachedIsHashSet = new Dictionary<Type, bool>(100);

        private static Dictionary<Type, bool> cachedIsValueType = new Dictionary<Type, bool>(100);

        private static Dictionary<Type, Type[]> cachedGenericArguments = new Dictionary<Type, Type[]>(100);

        private static Dictionary<Type, bool> cachedIsGenericType = new Dictionary<Type, bool>(100);

        private static Dictionary<Type, bool> cachedIsPrimitive = new Dictionary<Type, bool>(100);

        private static Dictionary<Type, bool> cachedIsEnum = new Dictionary<Type, bool>(100);

        private static Dictionary<Type, bool> cachedIsSpec = new Dictionary<Type, bool>(100);

        private static Dictionary<Type, Type> cachedListTypeWithElementType = new Dictionary<Type, Type>(100);

        private static Dictionary<Type, int> cachedTypeNameHashes = new Dictionary<Type, int>();

        public const string CoreNamespace = "Ricave.Core";

        public const string UINamespace = "Ricave.UI";

        public const string RenderingNamespace = "Ricave.Rendering";

        public const string ModsNamespace = "Mods";

        public const string ModsNamespaceWithDot = "Mods.";

        private static readonly string[] IgnoredNamespaces = new string[] { "Ricave.Core", "Ricave.UI", "Ricave.Rendering", "System", "System.Collections", "System.Collections.Generic" };

        private static bool preJITed = false;
    }
}