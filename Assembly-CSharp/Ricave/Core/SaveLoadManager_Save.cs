using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class SaveLoadManager_Save
    {
        public void Save(object start, string filePath, string rootXmlNodeName)
        {
            if (start == null)
            {
                throw new SavingException("Tried to save null root object. This is not supported.");
            }
            this.Clear();
            this.Save(start, start.GetType(), rootXmlNodeName, rootXmlNodeName, 0, false);
            if (this.contentsToString == null || this.contents.Length > this.contentsToString.Length)
            {
                this.contentsToString = new char[this.contents.Length * 2];
            }
            this.contents.CopyTo(0, this.contentsToString, 0, this.contents.Length);
            Profiler.Begin("Write to file");
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.Write(this.contentsToString, 0, this.contents.Length);
            }
            Profiler.End();
            for (int i = this.toNotify.Count - 1; i >= 0; i--)
            {
                try
                {
                    this.toNotify[i].OnSaved();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in OnSaved().", ex);
                }
            }
            this.Clear();
        }

        private void Clear()
        {
            this.contents.Length = 0;
            this.visited.Clear();
            this.referenceable.Clear();
            this.toNotify.Clear();
        }

        private void Save(object obj, Type defaultType, string nodeName, string xpath, int indent, bool filterCollectionNulls)
        {
            string text = ((indent == 0) ? SaveLoadManager_Save.VersionHeader : "");
            if (obj == null)
            {
                this.AppendIndent(indent);
                this.contents.Append("<").Append(nodeName).Append(text)
                    .Append(" ")
                    .Append("Null")
                    .AppendLine("=\"true\" />");
                return;
            }
            Type type = obj.GetType();
            bool flag = type.IsValueTypeCached();
            string text2;
            if (!flag && this.referenceable.TryGetValue(obj, out text2))
            {
                this.AppendIndent(indent);
                this.contents.Append("<").Append(nodeName).Append(text)
                    .Append(" ")
                    .Append("Ref")
                    .Append("=\"")
                    .Append(text2)
                    .Append("\" ");
                if (obj != null && type != defaultType)
                {
                    this.contents.Append("Type").Append("=\"").Append(type.NameOrFullName())
                        .Append("\" ");
                }
                this.contents.AppendLine("/>");
                return;
            }
            if (this.TryAppendStringRepresentation(obj, null))
            {
                this.AppendIndent(indent);
                this.contents.Append("<").Append(nodeName).Append(text)
                    .Append(">");
                this.TryAppendStringRepresentation(obj, this.contents);
                this.contents.Append("</").Append(nodeName).AppendLine(">");
                return;
            }
            if (!flag && !this.visited.Add(obj))
            {
                Log.Error("Tried to save the same field twice. This should never happen as it can cause infinite loops. nodeName=" + nodeName, false);
                return;
            }
            if (!flag)
            {
                this.referenceable.Add(obj, xpath);
            }
            ISaveableEventsReceiver saveableEventsReceiver = obj as ISaveableEventsReceiver;
            if (saveableEventsReceiver != null)
            {
                this.toNotify.Add(saveableEventsReceiver);
            }
            Array array = obj as Array;
            if (array != null)
            {
                this.SaveArray(array, nodeName, xpath, indent);
                return;
            }
            IDictionary dictionary = obj as IDictionary;
            if (dictionary != null)
            {
                this.SaveDictionary(dictionary, nodeName, xpath, indent, filterCollectionNulls);
                return;
            }
            if (type.IsHashSetCached())
            {
                this.SaveHashSet((IEnumerable)obj, nodeName, xpath, indent, filterCollectionNulls);
                return;
            }
            IList list = obj as IList;
            if (list != null)
            {
                this.SaveList(list, nodeName, xpath, indent, filterCollectionNulls);
                return;
            }
            if (type.IsRecursivelySaveableType())
            {
                this.AppendIndent(indent);
                this.contents.Append("<").Append(nodeName).Append(text);
                if (type != defaultType)
                {
                    this.contents.Append(" ").Append("Type").Append("=\"")
                        .Append(type.NameOrFullName())
                        .Append("\"");
                }
                this.contents.AppendLine(">");
                foreach (FieldInfo fieldInfo in type.GetAllSavedFields())
                {
                    object value = fieldInfo.GetValue(obj);
                    Saved savedAttribute = fieldInfo.GetSavedAttribute();
                    if (!savedAttribute.DefaultValueEquals(value, fieldInfo.FieldType))
                    {
                        bool flag2 = this.CaresAboutXPath(value);
                        this.Save(value, fieldInfo.FieldType, fieldInfo.Name, flag2 ? (xpath + "/" + fieldInfo.Name) : null, indent + 1, savedAttribute.FilterNulls);
                    }
                }
                this.AppendIndent(indent);
                this.contents.Append("</").Append(nodeName).AppendLine(">");
                return;
            }
            string text3 = "Don't know how to save an object of type \"";
            Type type2 = type;
            Log.Error(text3 + ((type2 != null) ? type2.ToString() : null) + "\". nodeName=" + nodeName, false);
        }

        private bool CaresAboutXPath(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Type type = obj.GetType();
            if (!type.IsValueTypeCached())
            {
                return true;
            }
            Type type2 = Nullable.GetUnderlyingType(type) ?? type;
            return !SaveLoadManager_Save.PrimitiveToString.ContainsKey(type2) && !type2.IsPrimitiveCached() && !type2.IsEnumCached();
        }

        private void SaveArray(Array array, string nodeName, string xpath, int indent)
        {
            Type elementType = array.GetType().GetElementType();
            this.tmpArrayLengthSb.Length = 0;
            for (int i = 0; i < array.Rank; i++)
            {
                if (i != 0)
                {
                    this.tmpArrayLengthSb.Append(", ");
                }
                this.tmpArrayLengthSb.Append(array.GetLength(i));
            }
            this.AppendIndent(indent);
            this.contents.Append("<").Append(nodeName).Append(" ")
                .Append("Length")
                .Append("=\"")
                .Append(this.tmpArrayLengthSb)
                .AppendLine("\">");
            if (elementType == typeof(bool))
            {
                this.AppendIndent(indent + 1);
                foreach (object obj in array)
                {
                    this.contents.Append(((bool)obj) ? "1" : "0");
                }
                this.contents.AppendLine();
            }
            else
            {
                int num = -1;
                int num2 = 0;
                using (IEnumerator enumerator = array.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current != null)
                        {
                            num = num2;
                        }
                        num2++;
                    }
                }
                num2 = 0;
                foreach (object obj2 in array)
                {
                    if (num2 > num)
                    {
                        break;
                    }
                    bool flag = this.CaresAboutXPath(obj2);
                    this.Save(obj2, elementType, "li", flag ? (xpath + "/li[" + (num2 + 1).ToStringCached() + "]") : null, indent + 1, false);
                    num2++;
                }
            }
            this.AppendIndent(indent);
            this.contents.Append("</").Append(nodeName).AppendLine(">");
        }

        private void SaveDictionary(IDictionary dictionary, string nodeName, string xpath, int indent, bool filterCollectionNulls)
        {
            this.AppendIndent(indent);
            this.contents.Append("<").Append(nodeName).AppendLine(">");
            Type[] genericArgumentsCached = dictionary.GetType().GetGenericArgumentsCached();
            Type type = genericArgumentsCached[0];
            Type type2 = genericArgumentsCached[1];
            Type listTypeWithElementType = TypeUtility.GetListTypeWithElementType(type);
            Type listTypeWithElementType2 = TypeUtility.GetListTypeWithElementType(type2);
            IList list;
            if (this.workingLists.TryGetValue(listTypeWithElementType, out list))
            {
                this.workingLists.Remove(listTypeWithElementType);
                list.Clear();
            }
            else
            {
                list = (IList)Activator.CreateInstance(listTypeWithElementType);
            }
            IList list2;
            if (this.workingLists.TryGetValue(listTypeWithElementType2, out list2))
            {
                this.workingLists.Remove(listTypeWithElementType2);
                list2.Clear();
            }
            else
            {
                list2 = (IList)Activator.CreateInstance(listTypeWithElementType2);
            }
            bool flag = false;
            foreach (object obj in dictionary.Keys)
            {
                object obj2 = dictionary[obj];
                if (filterCollectionNulls && obj2 == null)
                {
                    if (!flag)
                    {
                        flag = true;
                        Log.Warning("Filtered some nulls from " + nodeName + " while saving.", false);
                    }
                }
                else
                {
                    list.Add(obj);
                    list2.Add(obj2);
                }
            }
            this.SaveList(list, "keys", xpath + "/keys", indent + 1, false);
            this.SaveList(list2, "values", xpath + "/values", indent + 1, false);
            list.Clear();
            if (!this.workingLists.ContainsKey(listTypeWithElementType))
            {
                this.workingLists.Add(listTypeWithElementType, list);
            }
            list2.Clear();
            if (!this.workingLists.ContainsKey(listTypeWithElementType2))
            {
                this.workingLists.Add(listTypeWithElementType2, list2);
            }
            this.AppendIndent(indent);
            this.contents.Append("</").Append(nodeName).AppendLine(">");
        }

        private void SaveHashSet(IEnumerable hashSet, string nodeName, string xpath, int indent, bool filterCollectionNulls)
        {
            Type listTypeWithElementType = TypeUtility.GetListTypeWithElementType(hashSet.GetType().GetGenericArgumentsCached()[0]);
            IList list;
            if (this.workingLists.TryGetValue(listTypeWithElementType, out list))
            {
                this.workingLists.Remove(listTypeWithElementType);
                list.Clear();
            }
            else
            {
                list = (IList)Activator.CreateInstance(listTypeWithElementType);
            }
            foreach (object obj in hashSet)
            {
                list.Add(obj);
            }
            this.SaveList(list, nodeName, xpath, indent, filterCollectionNulls);
            list.Clear();
            if (!this.workingLists.ContainsKey(listTypeWithElementType))
            {
                this.workingLists.Add(listTypeWithElementType, list);
            }
        }

        private void SaveList(IList list, string nodeName, string xpath, int indent, bool filterCollectionNulls)
        {
            if (list.Count == 0)
            {
                this.AppendIndent(indent);
                this.contents.Append("<").Append(nodeName).AppendLine(" />");
                return;
            }
            Type type = list.GetType().GetGenericArgumentsCached()[0];
            bool flag = type.IsValueTypeCached();
            this.AppendIndent(indent);
            this.contents.Append("<").Append(nodeName).AppendLine(">");
            bool flag2 = this.TryAppendCompressedListElements(list, null);
            if (flag2)
            {
                this.AppendIndent(indent + 1);
                this.contents.Append("<").Append("_compressed").AppendLine(">");
                this.TryAppendCompressedListElements(list, this.contents);
                this.contents.AppendLine();
                this.AppendIndent(indent + 1);
                this.contents.Append("</").Append("_compressed").AppendLine(">");
            }
            bool flag3 = false;
            int num = 0;
            int num2 = 0;
            int i = 0;
            int count = list.Count;
            while (i < count)
            {
                object obj = list[i];
                if (filterCollectionNulls && obj == null)
                {
                    if (!flag3)
                    {
                        flag3 = true;
                        Log.Warning("Filtered some nulls from " + nodeName + " while saving.", false);
                    }
                }
                else if (flag2 && this.ShouldBeCompressed(obj))
                {
                    if (!flag)
                    {
                        string text = xpath + "/_compressed[" + (num + 1).ToStringCached() + "]";
                        this.referenceable.Add(obj, text);
                    }
                    num++;
                }
                else
                {
                    string text2 = (this.CaresAboutXPath(obj) ? (xpath + "/li[" + (num2 + 1).ToStringCached() + "]") : null);
                    this.Save(obj, type, "li", text2, indent + 1, false);
                    num2++;
                }
                i++;
            }
            this.AppendIndent(indent);
            this.contents.Append("</").Append(nodeName).AppendLine(">");
        }

        private void AppendIndent(int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                this.contents.Append('\t');
            }
        }

        private bool TryAppendStringRepresentation(object obj, StringBuilder sb)
        {
            Type type = obj.GetType();
            Type type2 = Nullable.GetUnderlyingType(type) ?? type;
            Action<object, StringBuilder> action;
            if (SaveLoadManager_Save.PrimitiveToString.TryGetValue(type2, out action))
            {
                if (sb != null)
                {
                    action(obj, sb);
                }
                return true;
            }
            if (type2.IsPrimitiveCached() || type2.IsEnumCached())
            {
                if (sb != null)
                {
                    sb.Append(obj);
                }
                return true;
            }
            Type type3 = obj as Type;
            if (type3 != null)
            {
                if (sb != null)
                {
                    sb.Append(type3.NameOrFullName());
                }
                return true;
            }
            Spec spec = obj as Spec;
            if (spec != null)
            {
                if (sb != null)
                {
                    sb.Append(spec.SpecID);
                }
                return true;
            }
            return false;
        }

        private bool TryAppendCompressedListElements(IList list, StringBuilder sb)
        {
            if (list.Count < 30)
            {
                return false;
            }
            if (list.GetType().GetGenericArgumentsCached()[0] != typeof(Entity))
            {
                return false;
            }
            bool flag = false;
            int num = 0;
            foreach (object obj in list)
            {
                if (!this.ShouldBeCompressed(obj))
                {
                    num++;
                }
                else
                {
                    if (sb == null)
                    {
                        return true;
                    }
                    Entity entity = (Entity)obj;
                    sb.Append(num.ToStringCached()).Append(";").Append(entity.Spec.SpecID)
                        .Append(";")
                        .Append(entity.InstanceID.ToStringCached())
                        .Append(";")
                        .Append(entity.MyStableHash.ToStringCached())
                        .Append(";");
                    sb.Append("(").Append(entity.Position.x.ToStringCached()).Append(",")
                        .Append(entity.Position.y.ToStringCached())
                        .Append(",")
                        .Append(entity.Position.z.ToStringCached())
                        .Append(")");
                    sb.Append(";");
                    sb.Append("(").Append(entity.Rotation.x.ToStringCachedNoRounding()).Append(",")
                        .Append(entity.Rotation.y.ToStringCachedNoRounding())
                        .Append(",")
                        .Append(entity.Rotation.z.ToStringCachedNoRounding())
                        .Append(",")
                        .Append(entity.Rotation.w.ToStringCachedNoRounding())
                        .Append(")");
                    sb.Append(";");
                    sb.Append("(").Append(entity.Scale.x.ToStringCachedNoRounding()).Append(",")
                        .Append(entity.Scale.y.ToStringCachedNoRounding())
                        .Append(",")
                        .Append(entity.Scale.z.ToStringCachedNoRounding())
                        .Append(")");
                    sb.Append(";");
                    flag = true;
                    num++;
                }
            }
            return flag;
        }

        private bool ShouldBeCompressed(object obj)
        {
            Structure structure = obj as Structure;
            return structure != null && obj.GetType() == typeof(Structure) && structure.Spawned && structure.AllComps.Count == 0 && !structure.UseEffects.Any && structure.InnerEntities.Count == 0 && structure.LastUsedToRewindTimeSequence == null && structure.LastUseSequence == null && structure.HP == 0;
        }

        private StringBuilder contents = new StringBuilder(524288);

        private HashSet<object> visited = new HashSet<object>();

        private Dictionary<object, string> referenceable = new Dictionary<object, string>();

        private List<ISaveableEventsReceiver> toNotify = new List<ISaveableEventsReceiver>(128);

        private char[] contentsToString;

        private Dictionary<Type, IList> workingLists = new Dictionary<Type, IList>();

        private static readonly Dictionary<Type, Action<object, StringBuilder>> PrimitiveToString = new Dictionary<Type, Action<object, StringBuilder>>
        {
            {
                typeof(int),
                delegate(object x, StringBuilder sb)
                {
                    sb.Append(((int)x).ToStringCached());
                }
            },
            {
                typeof(float),
                delegate(object x, StringBuilder sb)
                {
                    sb.Append(((float)x).ToStringCachedNoRounding());
                }
            },
            {
                typeof(bool),
                delegate(object x, StringBuilder sb)
                {
                    sb.Append(((bool)x) ? "true" : "false");
                }
            },
            {
                typeof(string),
                delegate(object x, StringBuilder sb)
                {
                    sb.Append(WebUtility.HtmlEncode((string)x));
                }
            },
            {
                typeof(Vector2),
                delegate(object x, StringBuilder sb)
                {
                    Vector2 vector = (Vector2)x;
                    sb.Append("(").Append(vector.x.ToStringCachedNoRounding()).Append(",")
                        .Append(vector.y.ToStringCachedNoRounding())
                        .Append(")");
                }
            },
            {
                typeof(Vector3),
                delegate(object x, StringBuilder sb)
                {
                    Vector3 vector2 = (Vector3)x;
                    sb.Append("(").Append(vector2.x.ToStringCachedNoRounding()).Append(",")
                        .Append(vector2.y.ToStringCachedNoRounding())
                        .Append(",")
                        .Append(vector2.z.ToStringCachedNoRounding())
                        .Append(")");
                }
            },
            {
                typeof(Vector2Int),
                delegate(object x, StringBuilder sb)
                {
                    Vector2Int vector2Int = (Vector2Int)x;
                    sb.Append("(").Append(vector2Int.x.ToStringCached()).Append(",")
                        .Append(vector2Int.y.ToStringCached())
                        .Append(")");
                }
            },
            {
                typeof(Vector3Int),
                delegate(object x, StringBuilder sb)
                {
                    Vector3Int vector3Int = (Vector3Int)x;
                    sb.Append("(").Append(vector3Int.x.ToStringCached()).Append(",")
                        .Append(vector3Int.y.ToStringCached())
                        .Append(",")
                        .Append(vector3Int.z.ToStringCached())
                        .Append(")");
                }
            },
            {
                typeof(Quaternion),
                delegate(object x, StringBuilder sb)
                {
                    Quaternion quaternion = (Quaternion)x;
                    sb.Append("(").Append(quaternion.x.ToStringCachedNoRounding()).Append(",")
                        .Append(quaternion.y.ToStringCachedNoRounding())
                        .Append(",")
                        .Append(quaternion.z.ToStringCachedNoRounding())
                        .Append(",")
                        .Append(quaternion.w.ToStringCachedNoRounding())
                        .Append(")");
                }
            },
            {
                typeof(Color),
                delegate(object x, StringBuilder sb)
                {
                    Color color = (Color)x;
                    sb.Append("(").Append(((int)(color.r * 255f)).ToStringCached()).Append(",")
                        .Append(((int)(color.g * 255f)).ToStringCached())
                        .Append(",")
                        .Append(((int)(color.b * 255f)).ToStringCached())
                        .Append(")");
                }
            },
            {
                typeof(Rect),
                delegate(object x, StringBuilder sb)
                {
                    Rect rect = (Rect)x;
                    sb.Append("(").Append(rect.x.ToStringCachedNoRounding()).Append(",")
                        .Append(rect.y.ToStringCachedNoRounding())
                        .Append(",")
                        .Append(rect.width.ToStringCachedNoRounding())
                        .Append(",")
                        .Append(rect.height.ToStringCachedNoRounding())
                        .Append(")");
                }
            },
            {
                typeof(DateTime),
                delegate(object x, StringBuilder sb)
                {
                    sb.Append(((DateTime)x).Ticks);
                }
            },
            {
                typeof(IntRange),
                delegate(object x, StringBuilder sb)
                {
                    IntRange intRange = (IntRange)x;
                    sb.Append("(").Append(intRange.from.ToStringCached()).Append(",")
                        .Append(intRange.to.ToStringCached())
                        .Append(")");
                }
            },
            {
                typeof(FloatRange),
                delegate(object x, StringBuilder sb)
                {
                    FloatRange floatRange = (FloatRange)x;
                    sb.Append("(").Append(floatRange.from.ToStringCachedNoRounding()).Append(",")
                        .Append(floatRange.to.ToStringCachedNoRounding())
                        .Append(")");
                }
            }
        };

        private const int MinCountToCompressEntities = 30;

        private static readonly string VersionHeader = " Version=\"" + App.Version + "\"";

        private StringBuilder tmpArrayLengthSb = new StringBuilder();
    }
}