using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class SaveLoadManager_Load
    {
        public SpecsToResolve Load(object start, string filePath, string rootXmlNodeName, bool loadingSpecs, out string version)
        {
            XmlDocument xmlDocument = new XmlDocument();
            Profiler.Begin("Read XML from file");
            xmlDocument.Load(filePath);
            Profiler.End();
            return this.Load(start, xmlDocument, rootXmlNodeName, loadingSpecs, out version);
        }

        public SpecsToResolve Load(object start, XmlDocument doc, string rootXmlNodeName, bool loadingSpecs, out string version)
        {
            this.loadingSpecs = loadingSpecs;
            this.toNotify.Clear();
            this.referenceable.Clear();
            this.assignViaXPathLater.Clear();
            this.specsToResolve = new SpecsToResolve();
            this.listsToFilterNulls.Clear();
            this.dictionariesToFilterNulls.Clear();
            this.setsToFilterNulls.Clear();
            this.xmlDoc = doc;
            this.root = this.xmlDoc.SelectSingleNode(rootXmlNodeName);
            if (this.root == null)
            {
                throw new LoadingException("Could not find root node \"" + rootXmlNodeName + "\".");
            }
            XmlAttribute xmlAttribute = this.root.Attributes["Version"];
            if (xmlAttribute != null && xmlAttribute.Value != null)
            {
                version = xmlAttribute.Value.Trim();
                if (version != App.Version)
                {
                    Log.Warning(string.Concat(new string[]
                    {
                        "Loading ",
                        start.GetType().Name,
                        " from different version. Loading: ",
                        version,
                        " Current: ",
                        App.Version
                    }), false);
                }
            }
            else
            {
                version = null;
            }
            this.LoadIntoObject(ref start, this.root, rootXmlNodeName, delegate (object x)
            {
                start = x;
            });
            if (this.assignViaXPathLater.Any<KeyValuePair<string, ValueTuple<Action<object>, Type>>>((KeyValuePair<string, ValueTuple<Action<object>, Type>> x) => !this.referenceable.ContainsKey(x.Key)))
            {
                this.assignViaXPathLater.StableSort<KeyValuePair<string, ValueTuple<Action<object>, Type>>, string>((KeyValuePair<string, ValueTuple<Action<object>, Type>> x) => x.Key);
                for (int i = 0; i < 100000; i++)
                {
                    if (i == 99999)
                    {
                        Log.Error("Potentially infinite loop in SaveLoadManager_Load.", false);
                        break;
                    }
                    try
                    {
                        if (!this.TryRecoverFromMissingAssignViaXPathLater())
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error while recovering from Refs pointing at missing objects.", ex);
                        break;
                    }
                }
            }
            for (int j = 0; j < this.assignViaXPathLater.Count; j++)
            {
                object obj;
                if (!this.referenceable.TryGetValue(this.assignViaXPathLater[j].Key, out obj))
                {
                    Log.Error("XML error. Tried to use Ref pointing at element with xpath \"" + this.assignViaXPathLater[j].Key + "\", but object associated with this XML element is not a class or hasn't been loaded.", false);
                }
                else
                {
                    try
                    {
                        this.assignViaXPathLater[j].Value.Item1(obj);
                    }
                    catch (Exception ex2)
                    {
                        Log.Error("Error while resolving Ref.", ex2);
                    }
                }
            }
            foreach (ValueTuple<string, IList> valueTuple in this.listsToFilterNulls)
            {
                string item = valueTuple.Item1;
                IList item2 = valueTuple.Item2;
                bool flag = false;
                for (int k = item2.Count - 1; k >= 0; k--)
                {
                    if (item2[k] == null)
                    {
                        if (!flag)
                        {
                            flag = true;
                            Log.Warning("Filtered some nulls from " + item + " while loading.", false);
                        }
                        item2.RemoveAt(k);
                    }
                }
            }
            foreach (ValueTuple<string, IDictionary> valueTuple2 in this.dictionariesToFilterNulls)
            {
                string item3 = valueTuple2.Item1;
                IDictionary item4 = valueTuple2.Item2;
                List<object> list = null;
                foreach (object obj2 in item4.Keys)
                {
                    if (item4[obj2] == null)
                    {
                        if (list == null)
                        {
                            list = new List<object>();
                        }
                        list.Add(obj2);
                    }
                }
                if (!list.NullOrEmpty<object>())
                {
                    Log.Warning("Filtered some nulls from " + item3 + " while loading.", false);
                    foreach (object obj3 in list)
                    {
                        item4.Remove(obj3);
                    }
                }
            }
            foreach (ValueTuple<string, IEnumerable> valueTuple3 in this.setsToFilterNulls)
            {
                string item5 = valueTuple3.Item1;
                IEnumerable item6 = valueTuple3.Item2;
                bool flag2 = false;
                using (IEnumerator enumerator3 = item6.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        if (enumerator3.Current == null)
                        {
                            if (!flag2)
                            {
                                Log.Warning("Filtered some nulls from " + item5 + " while loading.", false);
                            }
                            CollectionsUtility.RemoveNullFromIEnumerableHashSet(item6);
                            break;
                        }
                    }
                }
            }
            if (this.specsToResolve.IsEmpty)
            {
                this.NotifyLoaded();
            }
            else
            {
                this.specsToResolve.NotifyLoadedAfterResolved(this.toNotify);
            }
            return this.specsToResolve;
        }

        private void NotifyLoaded()
        {
            if (this.toNotify.ContainsDuplicates<ISaveableEventsReceiver>())
            {
                Log.Error("Load manager error: toNotify contains duplicates.", false);
            }
            for (int i = 0; i < this.toNotify.Count; i++)
            {
                try
                {
                    this.toNotify[i].OnLoaded();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in OnLoaded().", ex);
                }
            }
            this.toNotify.Clear();
        }

        private void LoadIntoObject(ref object obj, XmlNode xmlNode, string xpath, Action<object> objAssigner)
        {
            if (obj == null)
            {
                return;
            }
            Type type = obj.GetType();
            bool isValueType = type.IsValueTypeCached();
            if (!isValueType)
            {
                this.referenceable.Add(xpath, obj);
            }
            ISaveableEventsReceiver saveableEventsReceiver = obj as ISaveableEventsReceiver;
            ISaveableEventsReceiver saveableEventsReceiver2;
            if (saveableEventsReceiver != null && ((this.loadingSpecs && xmlNode.ParentNode == this.root) || !(obj is Spec)))
            {
                saveableEventsReceiver2 = saveableEventsReceiver;
            }
            else
            {
                saveableEventsReceiver2 = null;
            }
            if (this.TryProcessPrimitiveObject(ref obj, xmlNode, xpath, objAssigner))
            {
                if (saveableEventsReceiver2 != null)
                {
                    this.toNotify.Add(saveableEventsReceiver2);
                }
                return;
            }
            object objNonRef = obj;
            Dictionary<string, XmlNode> dictionary = new Dictionary<string, XmlNode>();
            foreach (object obj2 in xmlNode.ChildNodes)
            {
                XmlNode xmlNode2 = (XmlNode)obj2;
                if (xmlNode2.NodeType == XmlNodeType.Element)
                {
                    dictionary.Add(xmlNode2.Name, xmlNode2);
                }
            }
            FieldInfo[] allSavedFields = type.GetAllSavedFields();
            for (int i = 0; i < allSavedFields.Length; i++)
            {
                FieldInfo field = allSavedFields[i];
                XmlNode xmlNode3;
                if (!dictionary.TryGetValue(field.Name, out xmlNode3))
                {
                    if (this.TryAssignDefaultSavedAttrValue(field, obj) && isValueType)
                    {
                        objAssigner(objNonRef);
                    }
                }
                else
                {
                    dictionary.Remove(field.Name);
                    FieldInfo fieldLocal = field;
                    string innerXpath = xpath + "/" + field.Name;
                    bool filterNulls = field.GetSavedAttribute().FilterNulls;
                    try
                    {
                        this.LoadObject(xmlNode3, innerXpath, field.FieldType, delegate (object x)
                        {
                            if (filterNulls)
                            {
                                IDictionary dictionary2 = x as IDictionary;
                                if (dictionary2 != null)
                                {
                                    this.dictionariesToFilterNulls.Add(new ValueTuple<string, IDictionary>(field.Name, dictionary2));
                                }
                                else if (x != null && x.GetType().IsHashSetCached())
                                {
                                    this.setsToFilterNulls.Add(new ValueTuple<string, IEnumerable>(field.Name, (IEnumerable)x));
                                }
                                else
                                {
                                    IList list = x as IList;
                                    if (list != null)
                                    {
                                        this.listsToFilterNulls.Add(new ValueTuple<string, IList>(field.Name, list));
                                    }
                                }
                            }
                            this.LoadedObjectHasChanged(fieldLocal.GetValue(objNonRef), x, innerXpath);
                            fieldLocal.SetValue(objNonRef, x);
                            if (isValueType)
                            {
                                objAssigner(objNonRef);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error("XML error. Error while processing XML node \"" + innerXpath + "\".", ex);
                    }
                }
            }
            foreach (KeyValuePair<string, XmlNode> keyValuePair in dictionary)
            {
                Log.Error("XML error. Node \"" + keyValuePair.Key + "\" doesn't correspond to any field in " + type.Name, false);
            }
            if (saveableEventsReceiver2 != null)
            {
                this.toNotify.Add(saveableEventsReceiver2);
            }
        }

        private void LoadObject(XmlNode newXmlNode, string xpath, Type defaultType, Action<object> objAssigner)
        {
            if (newXmlNode.NodeType != XmlNodeType.Element)
            {
                Log.Error("XML error. Node \"" + newXmlNode.Name + "\" is not an element node.", false);
                return;
            }
            if (newXmlNode.Attributes != null && newXmlNode.Attributes["Ref"] != null)
            {
                if (defaultType.IsValueTypeCached())
                {
                    Log.Error("XML error. Tried to use Ref on value type " + defaultType.Name + ".", false);
                    return;
                }
                if (defaultType.IsSpecCached())
                {
                    Log.Error("XML error. Tried to use Ref on Spec. Specs are already handled in a special way and are treated similar to Refs.", false);
                    return;
                }
                if (newXmlNode.NodeType != XmlNodeType.Element || !((XmlElement)newXmlNode).IsEmpty)
                {
                    Log.Error("XML error. XML node with \"Ref\" attribute must be an empty element node.", false);
                    return;
                }
                string innerText = newXmlNode.Attributes["Ref"].InnerText;
                this.assignViaXPathLater.Add(new KeyValuePair<string, ValueTuple<Action<object>, Type>>(innerText, new ValueTuple<Action<object>, Type>(objAssigner, defaultType)));
                return;
            }
            else
            {
                if (newXmlNode.Attributes != null && newXmlNode.Attributes["Null"] != null && newXmlNode.Attributes["Null"].Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    objAssigner(null);
                    return;
                }
                Type type;
                if (newXmlNode.Attributes != null && newXmlNode.Attributes["Type"] != null)
                {
                    string innerText2 = newXmlNode.Attributes["Type"].InnerText;
                    type = TypeUtility.GetType(innerText2);
                    if (type == null)
                    {
                        Log.Error("XML error. Could not find type named \"" + innerText2 + "\" (specified using the Type attribute).", false);
                        return;
                    }
                }
                else if (this.loadingSpecs && newXmlNode.ParentNode != null && newXmlNode.ParentNode.ParentNode == null && defaultType.IsSpecCached())
                {
                    type = TypeUtility.GetType(newXmlNode.Name);
                    if (type == null)
                    {
                        Log.Error("XML error. Could not find spec type named \"" + newXmlNode.Name + "\" (type name came from node name).", false);
                        return;
                    }
                }
                else
                {
                    type = defaultType;
                }
                object obj = TypeUtility.Instantiate(type);
                objAssigner(obj);
                this.LoadIntoObject(ref obj, newXmlNode, xpath, objAssigner);
                objAssigner(obj);
                return;
            }
        }

        private bool TryAssignDefaultSavedAttrValue(FieldInfo field, object obj)
        {
            Saved savedAttribute = field.GetSavedAttribute();
            if (savedAttribute == null)
            {
                return false;
            }
            field.SetValue(obj, savedAttribute.GetCachedDefaultValueOrNew(field.FieldType));
            return true;
        }

        private void LoadedObjectHasChanged(object previous, object newOne, string xpath)
        {
            if (previous == null || previous == newOne)
            {
                return;
            }
            if (previous.GetType().IsValueTypeCached())
            {
                return;
            }
            object obj;
            if (!this.referenceable.TryGetValue(xpath, out obj))
            {
                return;
            }
            if (obj != previous)
            {
                Log.Error("XML error. Previous object doesn't match.", false);
                return;
            }
            this.referenceable[xpath] = newOne;
        }

        private bool TryProcessPrimitiveObject(ref object obj, XmlNode xmlNode, string xpath, Action<object> objAssigner)
        {
            bool flag;
            try
            {
                Type type = obj.GetType();
                Type type2 = Nullable.GetUnderlyingType(type) ?? type;
                Func<XmlNode, object> func;
                if (SaveLoadManager_Load.PrimitiveFromString.TryGetValue(type2, out func))
                {
                    obj = func(xmlNode);
                    flag = true;
                }
                else
                {
                    flag = this.TryProcessList(ref obj, xmlNode, xpath, objAssigner) || this.TryProcessSpec(ref obj, xmlNode, objAssigner) || this.TryProcessType(ref obj, xmlNode, objAssigner) || this.TryProcessEnum(ref obj, xmlNode, objAssigner) || this.TryProcessDictionary(ref obj, xmlNode, xpath, objAssigner) || this.TryProcessHashSet(ref obj, xmlNode, xpath, objAssigner) || this.TryProcessArray(ref obj, xmlNode, xpath, objAssigner);
                }
            }
            catch (Exception ex)
            {
                Log.Error("XML error. Could not parse node \"" + xmlNode.InnerText.ToStringSafe().TruncateToLength(1000) + "\".", ex);
                flag = true;
            }
            return flag;
        }

        private static int ParseInt32(XmlNode xmlNode)
        {
            return int.Parse(xmlNode.InnerText);
        }

        private static long ParseInt64(XmlNode xmlNode)
        {
            return long.Parse(xmlNode.InnerText);
        }

        private static float ParseFloat(XmlNode xmlNode)
        {
            return float.Parse(xmlNode.InnerText);
        }

        private static double ParseDouble(XmlNode xmlNode)
        {
            return double.Parse(xmlNode.InnerText);
        }

        private static uint ParseUint(XmlNode xmlNode)
        {
            return uint.Parse(xmlNode.InnerText);
        }

        private static ulong ParseUlong(XmlNode xmlNode)
        {
            return ulong.Parse(xmlNode.InnerText);
        }

        private static string ParseString(XmlNode xmlNode)
        {
            return XmlUtility.Decode(WebUtility.HtmlDecode(xmlNode.InnerXml.Trim()));
        }

        private static bool ParseBool(XmlNode xmlNode)
        {
            string innerText = xmlNode.InnerText;
            if (innerText.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            if (innerText.Equals("false", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            throw new LoadingException("XML error. \"" + innerText + "\" is not a valid bool value.");
        }

        private static Vector2 ParseVector2(XmlNode xmlNode)
        {
            string text = xmlNode.InnerText;
            if (text[0] != '(' || text[text.Length - 1] != ')')
            {
                throw new LoadingException("XML error. \"" + text + "\" is not a valid Vector2 value.");
            }
            text = text.Trim(new char[] { '(', ')' });
            string[] array = text.Split(',', StringSplitOptions.None);
            if (array.Length != 2)
            {
                throw new LoadingException("XML error. \"" + text + "\" is not a valid Vector2 value.");
            }
            float num = float.Parse(array[0]);
            float num2 = float.Parse(array[1]);
            return new Vector2(num, num2);
        }

        private static Vector3 ParseVector3(XmlNode xmlNode)
        {
            return SaveLoadManager_Load.ParseVector3(xmlNode.InnerText);
        }

        private static Vector3 ParseVector3(string str)
        {
            if (str[0] != '(' || str[str.Length - 1] != ')')
            {
                throw new LoadingException("XML error. \"" + str + "\" is not a valid Vector3 value.");
            }
            str = str.Trim(new char[] { '(', ')' });
            string[] array = str.Split(',', StringSplitOptions.None);
            if (array.Length != 3)
            {
                throw new LoadingException("XML error. \"" + str + "\" is not a valid Vector3 value.");
            }
            float num = float.Parse(array[0]);
            float num2 = float.Parse(array[1]);
            float num3 = float.Parse(array[2]);
            return new Vector3(num, num2, num3);
        }

        private static Vector2Int ParseVector2Int(XmlNode xmlNode)
        {
            string text = xmlNode.InnerText;
            if (text[0] != '(' || text[text.Length - 1] != ')')
            {
                throw new LoadingException("XML error. \"" + text + "\" is not a valid Vector2Int value.");
            }
            text = text.Trim(new char[] { '(', ')' });
            string[] array = text.Split(',', StringSplitOptions.None);
            if (array.Length != 2)
            {
                throw new LoadingException("XML error. \"" + text + "\" is not a valid Vector2Int value.");
            }
            int num = int.Parse(array[0]);
            int num2 = int.Parse(array[1]);
            return new Vector2Int(num, num2);
        }

        private static Vector3Int ParseVector3Int(XmlNode xmlNode)
        {
            return SaveLoadManager_Load.ParseVector3Int(xmlNode.InnerText);
        }

        private static Vector3Int ParseVector3Int(string str)
        {
            if (str[0] != '(' || str[str.Length - 1] != ')')
            {
                throw new LoadingException("XML error. \"" + str + "\" is not a valid Vector3Int value.");
            }
            str = str.Trim(new char[] { '(', ')' });
            string[] array = str.Split(',', StringSplitOptions.None);
            if (array.Length != 3)
            {
                throw new LoadingException("XML error. \"" + str + "\" is not a valid Vector3Int value.");
            }
            int num = int.Parse(array[0]);
            int num2 = int.Parse(array[1]);
            int num3 = int.Parse(array[2]);
            return new Vector3Int(num, num2, num3);
        }

        private static IntRange ParseIntRange(XmlNode xmlNode)
        {
            string text = xmlNode.InnerText;
            if (text[0] != '(')
            {
                int num = int.Parse(text);
                return new IntRange(num, num);
            }
            if (text[text.Length - 1] != ')')
            {
                throw new LoadingException("XML error. \"" + text + "\" is not a valid IntRange value.");
            }
            text = text.Trim(new char[] { '(', ')' });
            string[] array = text.Split(',', StringSplitOptions.None);
            if (array.Length != 2)
            {
                throw new LoadingException("XML error. \"" + text + "\" is not a valid IntRange value.");
            }
            int num2 = int.Parse(array[0]);
            int num3 = int.Parse(array[1]);
            return new IntRange(num2, num3);
        }

        private static FloatRange ParseFloatRange(XmlNode xmlNode)
        {
            string text = xmlNode.InnerText;
            if (text[0] != '(')
            {
                float num = float.Parse(text);
                return new FloatRange(num, num);
            }
            if (text[text.Length - 1] != ')')
            {
                throw new LoadingException("XML error. \"" + text + "\" is not a valid FloatRange value.");
            }
            text = text.Trim(new char[] { '(', ')' });
            string[] array = text.Split(',', StringSplitOptions.None);
            if (array.Length != 2)
            {
                throw new LoadingException("XML error. \"" + text + "\" is not a valid FloatRange value.");
            }
            float num2 = float.Parse(array[0]);
            float num3 = float.Parse(array[1]);
            return new FloatRange(num2, num3);
        }

        private static Quaternion ParseQuaternion(XmlNode xmlNode)
        {
            return SaveLoadManager_Load.ParseQuaternion(xmlNode.InnerText);
        }

        private static Quaternion ParseQuaternion(string str)
        {
            if (str[0] != '(' || str[str.Length - 1] != ')')
            {
                throw new LoadingException("XML error. \"" + str + "\" is not a valid Quaternion value.");
            }
            str = str.Trim(new char[] { '(', ')' });
            string[] array = str.Split(',', StringSplitOptions.None);
            if (array.Length != 4)
            {
                throw new LoadingException("XML error. \"" + str + "\" is not a valid Quaternion value.");
            }
            float num = float.Parse(array[0]);
            float num2 = float.Parse(array[1]);
            float num3 = float.Parse(array[2]);
            float num4 = float.Parse(array[3]);
            return new Quaternion(num, num2, num3, num4);
        }

        private static Color ParseColor(XmlNode xmlNode)
        {
            string text = xmlNode.InnerText;
            if (text[0] != '(' || text[text.Length - 1] != ')')
            {
                throw new LoadingException("XML error. \"" + text + "\" is not a valid Color value.");
            }
            text = text.Trim(new char[] { '(', ')' });
            string[] array = text.Split(',', StringSplitOptions.None);
            if (array.Length != 3 && array.Length != 4)
            {
                throw new LoadingException("XML error. \"" + text + "\" is not a valid Color value.");
            }
            float num = float.Parse(array[0]);
            float num2 = float.Parse(array[1]);
            float num3 = float.Parse(array[2]);
            float num4;
            if (array.Length == 4)
            {
                num4 = float.Parse(array[3]);
            }
            else
            {
                num4 = 255f;
            }
            return new Color(num / 255f, num2 / 255f, num3 / 255f, num4 / 255f);
        }

        private static Rect ParseRect(XmlNode xmlNode)
        {
            return SaveLoadManager_Load.ParseRect(xmlNode.InnerText);
        }

        private static Rect ParseRect(string str)
        {
            if (str[0] != '(' || str[str.Length - 1] != ')')
            {
                throw new LoadingException("XML error. \"" + str + "\" is not a valid Rect value.");
            }
            str = str.Trim(new char[] { '(', ')' });
            string[] array = str.Split(',', StringSplitOptions.None);
            if (array.Length != 4)
            {
                throw new LoadingException("XML error. \"" + str + "\" is not a valid Rect value.");
            }
            float num = float.Parse(array[0]);
            float num2 = float.Parse(array[1]);
            float num3 = float.Parse(array[2]);
            float num4 = float.Parse(array[3]);
            return new Rect(num, num2, num3, num4);
        }

        private static DateTime ParseDateTime(XmlNode xmlNode)
        {
            return SaveLoadManager_Load.ParseDateTime(xmlNode.InnerText);
        }

        private static DateTime ParseDateTime(string str)
        {
            return new DateTime(long.Parse(str));
        }

        private bool TryProcessList(ref object obj, XmlNode xmlNode, string xpath, Action<object> objAssigner)
        {
            if (!(obj is IList) || !obj.GetType().IsGenericTypeCached())
            {
                return false;
            }
            IList list = (IList)obj;
            Type type = obj.GetType().GetGenericArgumentsCached()[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            List<bool> list2 = new List<bool>();
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int i = 0;
            int num = 0;
            for (int j = 0; j < childNodes.Count; j++)
            {
                if (childNodes[j].NodeType == XmlNodeType.Element)
                {
                    if (childNodes[j].Name == "_compressed")
                    {
                        int num2 = 0;
                        using (List<ValueTuple<int, object>>.Enumerator enumerator = this.Decompress(childNodes[j].InnerText).GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                ValueTuple<int, object> valueTuple = enumerator.Current;
                                while (list.Count <= valueTuple.Item1)
                                {
                                    list.Add(null);
                                    list2.Add(true);
                                }
                                list[valueTuple.Item1] = valueTuple.Item2;
                                list2[valueTuple.Item1] = false;
                                string text = xpath + "/_compressed[" + (num2 + 1).ToStringCached() + "]";
                                this.referenceable.Add(text, list[valueTuple.Item1]);
                                num2++;
                            }
                            goto IL_0385;
                        }
                    }
                    dictionary.SetOrIncrement(childNodes[j].Name, 1);
                    Type type2;
                    if (childNodes[j].Name == "li")
                    {
                        type2 = type;
                    }
                    else
                    {
                        type2 = TypeUtility.GetType(childNodes[j].Name);
                        if (type2 == null)
                        {
                            Log.Error("XML error. Could not find type named \"" + childNodes[j].Name + "\" (used as an element of a list).", false);
                            goto IL_0385;
                        }
                    }
                    int indexToInsertAt = -1;
                    while (i < list.Count)
                    {
                        if (list2[i])
                        {
                            indexToInsertAt = i;
                            break;
                        }
                        i++;
                    }
                    if (indexToInsertAt == -1)
                    {
                        list.Add(TypeUtility.GetCachedDefaultValue(type2));
                        list2.Add(false);
                        indexToInsertAt = list.Count - 1;
                    }
                    else
                    {
                        list[indexToInsertAt] = TypeUtility.GetCachedDefaultValue(type2);
                        list2[indexToInsertAt] = false;
                    }
                    int num3 = ((childNodes[j].Name == "li") ? (num + 1) : dictionary[childNodes[j].Name]);
                    string elemXpath = string.Concat(new string[]
                    {
                        xpath,
                        "/",
                        childNodes[j].Name,
                        "[",
                        num3.ToStringCached(),
                        "]"
                    });
                    try
                    {
                        this.LoadObject(childNodes[j], elemXpath, type2, delegate (object x)
                        {
                            this.LoadedObjectHasChanged(list[indexToInsertAt], x, elemXpath);
                            list[indexToInsertAt] = x;
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error("XML error. Error while loading list element \"" + elemXpath + "\".", ex);
                    }
                    if (childNodes[j].Name == "li")
                    {
                        num++;
                    }
                }
            IL_0385:;
            }
            return true;
        }

        private bool TryProcessArray(ref object obj, XmlNode xmlNode, string xpath, Action<object> objAssigner)
        {
            if (!(obj is Array))
            {
                return false;
            }
            int[] lengths = (from x in xmlNode.Attributes["Length"].Value.Split(',', StringSplitOptions.None)
                             select int.Parse(x)).ToArray<int>();
            Type elementType = obj.GetType().GetElementType();
            obj = Array.CreateInstance(elementType, lengths);
            Array array = (Array)obj;
            if (elementType == typeof(bool))
            {
                string text = xmlNode.InnerText.Trim();
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '1')
                    {
                        this.SetArrayValue(array, i, true, lengths);
                    }
                    else
                    {
                        this.SetArrayValue(array, i, false, lengths);
                    }
                }
            }
            else
            {
                XmlNodeList childNodes = xmlNode.ChildNodes;
                for (int j = 0; j < childNodes.Count; j++)
                {
                    if (childNodes[j].NodeType == XmlNodeType.Element)
                    {
                        Type type;
                        if (childNodes[j].Name == "li")
                        {
                            type = elementType;
                        }
                        else
                        {
                            type = TypeUtility.GetType(childNodes[j].Name);
                            if (type == null)
                            {
                                Log.Error("XML error. Could not find type named \"" + childNodes[j].Name + "\" (used as an element of a list).", false);
                                goto IL_0264;
                            }
                        }
                        this.SetArrayValue(array, j, TypeUtility.GetCachedDefaultValue(type), lengths);
                        int localIndex = j;
                        string elemXpath = string.Concat(new string[]
                        {
                            xpath,
                            "/",
                            childNodes[j].Name,
                            "[",
                            (localIndex + 1).ToString(),
                            "]"
                        });
                        try
                        {
                            this.LoadObject(childNodes[j], elemXpath, type, delegate (object x)
                            {
                                this.LoadedObjectHasChanged(this.GetArrayValue(array, localIndex, lengths), x, elemXpath);
                                this.SetArrayValue(array, localIndex, x, lengths);
                            });
                        }
                        catch (Exception ex)
                        {
                            Log.Error("XML error. Error while loading array element \"" + elemXpath + "\".", ex);
                        }
                    }
                IL_0264:;
                }
            }
            return true;
        }

        private int[] GetArrayIndex(int at, int[] lengths)
        {
            if (this.tmpIndex == null || this.tmpIndex.Length != lengths.Length)
            {
                this.tmpIndex = new int[lengths.Length];
            }
            int num = at;
            for (int i = lengths.Length - 1; i >= 0; i--)
            {
                this.tmpIndex[i] = num % lengths[i];
                num /= lengths[i];
            }
            return this.tmpIndex;
        }

        private void SetArrayValue(Array array, int at, object value, int[] lengths)
        {
            int[] arrayIndex = this.GetArrayIndex(at, lengths);
            array.SetValue(value, arrayIndex);
        }

        private object GetArrayValue(Array array, int at, int[] lengths)
        {
            int[] arrayIndex = this.GetArrayIndex(at, lengths);
            return array.GetValue(arrayIndex);
        }

        private bool TryProcessDictionary(ref object obj, XmlNode xmlNode, string xpath, Action<object> objAssigner)
        {
            if (!(obj is IDictionary) || !obj.GetType().IsGenericTypeCached())
            {
                return false;
            }
            IDictionary dict = (IDictionary)obj;
            Type[] genericArgumentsCached = obj.GetType().GetGenericArgumentsCached();
            Type type = genericArgumentsCached[0];
            Type type2 = genericArgumentsCached[1];
            XmlNode xmlNode2 = xmlNode.SelectSingleNode("keys");
            if (xmlNode2 != null)
            {
                XmlNode xmlNode3 = xmlNode.SelectSingleNode("values");
                if (xmlNode3 == null)
                {
                    throw new LoadingException("XML error. Dictionary has keys node but not values.");
                }
                Type listTypeWithElementType = TypeUtility.GetListTypeWithElementType(type);
                Type listTypeWithElementType2 = TypeUtility.GetListTypeWithElementType(type2);
                object obj2 = Activator.CreateInstance(listTypeWithElementType);
                object obj3 = Activator.CreateInstance(listTypeWithElementType2);
                if (!this.TryProcessList(ref obj2, xmlNode2, xpath + "/keys", null))
                {
                    throw new LoadingException("XML error. Could not process dictionary keys list.");
                }
                if (!this.TryProcessList(ref obj3, xmlNode3, xpath + "/values", null))
                {
                    throw new LoadingException("XML error. Could not process dictionary values list.");
                }
                if (((IList)obj2).Count != ((IList)obj3).Count)
                {
                    Log.Error("XML error. Dictionary keys and values list counts don't match.", false);
                }
                int num = 0;
                using (IEnumerator enumerator = ((IList)obj2).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        object obj4 = enumerator.Current;
                        if (obj4 == null)
                        {
                            Log.Error("XML error. Tried to load null dictionary key.", false);
                            num++;
                        }
                        else if (dict.Contains(obj4))
                        {
                            Log.Error("XML error. Tried to load the same dictionary key twice.", false);
                            num++;
                        }
                        else
                        {
                            dict.Add(obj4, ((IList)obj3)[num]);
                            num++;
                        }
                    }
                    return true;
                }
            }
            XmlNodeList childNodes = xmlNode.ChildNodes;
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i].NodeType == XmlNodeType.Element)
                {
                    dictionary.SetOrIncrement(childNodes[i].Name, 1);
                    object key;
                    if (type == typeof(int))
                    {
                        key = int.Parse(childNodes[i].Name);
                    }
                    else if (type == typeof(string))
                    {
                        key = childNodes[i].Name;
                    }
                    else
                    {
                        if (!(type == typeof(char)))
                        {
                            throw new LoadingException(string.Concat(new string[]
                            {
                                "XML error. Could not load \"",
                                obj.GetType().Name,
                                "\". Dictionary with key of type \"",
                                type.Name,
                                "\" is not supported."
                            }));
                        }
                        key = childNodes[i].Name[0];
                    }
                    if (dict.Contains(key))
                    {
                        string text = "XML error. Key \"";
                        object key2 = key;
                        Log.Error(text + ((key2 != null) ? key2.ToString() : null) + "\" already exists in dictionary.", false);
                    }
                    else
                    {
                        dict.Add(key, TypeUtility.GetCachedDefaultValue(type2));
                        string elemXpath = string.Concat(new string[]
                        {
                            xpath,
                            "/",
                            childNodes[i].Name,
                            "[",
                            dictionary[childNodes[i].Name].ToString(),
                            "]"
                        });
                        try
                        {
                            this.LoadObject(childNodes[i], elemXpath, type2, delegate (object x)
                            {
                                this.LoadedObjectHasChanged(dict[key], x, elemXpath);
                                dict[key] = x;
                            });
                        }
                        catch (Exception ex)
                        {
                            Log.Error("XML error. Error while loading dictionary element \"" + elemXpath + "\".", ex);
                        }
                    }
                }
            }
            return true;
        }

        private bool TryProcessHashSet(ref object obj, XmlNode xmlNode, string xpath, Action<object> objAssigner)
        {
            Type type = obj.GetType();
            if (!type.IsHashSetCached())
            {
                return false;
            }
            object obj2 = Activator.CreateInstance(TypeUtility.GetListTypeWithElementType(type.GetGenericArgumentsCached()[0]));
            if (!this.TryProcessList(ref obj2, xmlNode, xpath, null))
            {
                throw new LoadingException("XML error. Could not process HashSet list.");
            }
            MethodInfo method = type.GetMethod("Add");
            foreach (object obj3 in ((IList)obj2))
            {
                method.Invoke(obj, new object[] { obj3 });
            }
            return true;
        }

        private bool TryProcessType(ref object obj, XmlNode xmlNode, Action<object> objAssigner)
        {
            if (!(obj is Type))
            {
                return false;
            }
            Type type = TypeUtility.GetType(xmlNode.InnerText);
            if (type == null)
            {
                Log.Error("XML error. Could not find type named \"" + xmlNode.InnerText + "\".", false);
                obj = null;
                return true;
            }
            obj = type;
            return true;
        }

        private bool TryProcessEnum(ref object obj, XmlNode xmlNode, Action<object> objAssigner)
        {
            if (!(obj is Enum))
            {
                return false;
            }
            try
            {
                obj = Enum.Parse(obj.GetType(), xmlNode.InnerText);
            }
            catch
            {
                Log.Error(string.Concat(new string[]
                {
                    "XML error. Enum ",
                    obj.GetType().Name,
                    " does not have value \"",
                    xmlNode.InnerText,
                    "\"."
                }), false);
                return true;
            }
            return true;
        }

        private bool TryProcessSpec(ref object obj, XmlNode xmlNode, Action<object> objAssigner)
        {
            if (!(obj is Spec))
            {
                return false;
            }
            Type type = obj.GetType();
            if (this.loadingSpecs)
            {
                if (xmlNode.ParentNode == this.root)
                {
                    return false;
                }
                if (Get.Specs.Exists(type, xmlNode.InnerText))
                {
                    obj = Get.Specs.Get(type, xmlNode.InnerText);
                }
                else
                {
                    this.specsToResolve.Add(new SpecToResolve(type, xmlNode.InnerText, objAssigner));
                }
            }
            else if (Get.Specs.Exists(type, xmlNode.InnerText))
            {
                obj = Get.Specs.Get(type, xmlNode.InnerText);
            }
            else
            {
                Log.Error(string.Concat(new string[]
                {
                    "XML error. Could not find spec named \"",
                    xmlNode.InnerText,
                    "\" (node name \"",
                    xmlNode.Name.ToStringSafe(),
                    "\")"
                }), false);
                obj = null;
            }
            return true;
        }

        private List<ValueTuple<int, object>> Decompress(string compressed)
        {
            string[] array = compressed.Trim().Trim(';').Split(';', StringSplitOptions.None);
            List<ValueTuple<int, object>> list = new List<ValueTuple<int, object>>();
            for (int i = 0; i < array.Length; i += 7)
            {
                int num = int.Parse(array[i]);
                string text = array[i + 1];
                int num2 = int.Parse(array[i + 2]);
                int num3 = int.Parse(array[i + 3]);
                Vector3Int vector3Int = SaveLoadManager_Load.ParseVector3Int(array[i + 4]);
                Quaternion quaternion = SaveLoadManager_Load.ParseQuaternion(array[i + 5]);
                Vector3 vector = SaveLoadManager_Load.ParseVector3(array[i + 6]);
                list.Add(new ValueTuple<int, object>(num, new Structure(text, num2, num3, vector3Int, quaternion, vector)));
            }
            return list;
        }

        private bool TryRecoverFromMissingAssignViaXPathLater()
        {
            int i = 0;
            while (i < this.assignViaXPathLater.Count)
            {
                string key = this.assignViaXPathLater[i].Key;
                if (!this.referenceable.ContainsKey(key))
                {
                    XmlNode xmlNode = this.xmlDoc.SelectSingleNode(key);
                    if (xmlNode == null)
                    {
                        Log.Error("XML error. Tried to recover from Ref pointing at an element which hasn't been loaded with xpath \"" + key + "\", but could not find XML node with such xpath.", false);
                        this.assignViaXPathLater.RemoveAt(i);
                        return true;
                    }
                    ValueTuple<Action<object>, Type> value = this.assignViaXPathLater[i].Value;
                    Action<object> item = value.Item1;
                    Type item2 = value.Item2;
                    Type type;
                    if (xmlNode.Attributes != null && xmlNode.Attributes["Type"] != null)
                    {
                        string innerText = xmlNode.Attributes["Type"].InnerText;
                        type = TypeUtility.GetType(innerText);
                        if (type == null)
                        {
                            Log.Error("XML error. Could not find type named \"" + innerText + "\" (specified using the Type attribute on a Ref node with missing original object).", false);
                            this.assignViaXPathLater.RemoveAt(i);
                            return true;
                        }
                    }
                    else
                    {
                        type = item2;
                    }
                    object obj = TypeUtility.Instantiate(type);
                    item(obj);
                    this.LoadIntoObject(ref obj, xmlNode, key, item);
                    item(obj);
                    Log.Warning("Object at xpath " + key + " wasn't loaded by anything (most likely because the code has changed and this field no longer exists) yet it was referenced by another XML node, so it had to be loaded manually.", false);
                    this.assignViaXPathLater.RemoveAt(i);
                    return true;
                }
                else
                {
                    i++;
                }
            }
            return false;
        }

        private XmlDocument xmlDoc;

        private XmlNode root;

        private List<ISaveableEventsReceiver> toNotify = new List<ISaveableEventsReceiver>();

        private Dictionary<string, object> referenceable = new Dictionary<string, object>();

        private List<KeyValuePair<string, ValueTuple<Action<object>, Type>>> assignViaXPathLater = new List<KeyValuePair<string, ValueTuple<Action<object>, Type>>>();

        private bool loadingSpecs;

        private SpecsToResolve specsToResolve;

        private HashSet<ValueTuple<string, IList>> listsToFilterNulls = new HashSet<ValueTuple<string, IList>>();

        private HashSet<ValueTuple<string, IDictionary>> dictionariesToFilterNulls = new HashSet<ValueTuple<string, IDictionary>>();

        private HashSet<ValueTuple<string, IEnumerable>> setsToFilterNulls = new HashSet<ValueTuple<string, IEnumerable>>();

        private static readonly Dictionary<Type, Func<XmlNode, object>> PrimitiveFromString = new Dictionary<Type, Func<XmlNode, object>>
        {
            {
                typeof(int),
                (XmlNode x) => SaveLoadManager_Load.ParseInt32(x)
            },
            {
                typeof(long),
                (XmlNode x) => SaveLoadManager_Load.ParseInt64(x)
            },
            {
                typeof(float),
                (XmlNode x) => SaveLoadManager_Load.ParseFloat(x)
            },
            {
                typeof(double),
                (XmlNode x) => SaveLoadManager_Load.ParseDouble(x)
            },
            {
                typeof(string),
                (XmlNode x) => SaveLoadManager_Load.ParseString(x)
            },
            {
                typeof(bool),
                (XmlNode x) => SaveLoadManager_Load.ParseBool(x)
            },
            {
                typeof(uint),
                (XmlNode x) => SaveLoadManager_Load.ParseUint(x)
            },
            {
                typeof(ulong),
                (XmlNode x) => SaveLoadManager_Load.ParseUlong(x)
            },
            {
                typeof(Vector2),
                (XmlNode x) => SaveLoadManager_Load.ParseVector2(x)
            },
            {
                typeof(Vector3),
                (XmlNode x) => SaveLoadManager_Load.ParseVector3(x)
            },
            {
                typeof(Vector2Int),
                (XmlNode x) => SaveLoadManager_Load.ParseVector2Int(x)
            },
            {
                typeof(Vector3Int),
                (XmlNode x) => SaveLoadManager_Load.ParseVector3Int(x)
            },
            {
                typeof(Quaternion),
                (XmlNode x) => SaveLoadManager_Load.ParseQuaternion(x)
            },
            {
                typeof(Color),
                (XmlNode x) => SaveLoadManager_Load.ParseColor(x)
            },
            {
                typeof(Rect),
                (XmlNode x) => SaveLoadManager_Load.ParseRect(x)
            },
            {
                typeof(DateTime),
                (XmlNode x) => SaveLoadManager_Load.ParseDateTime(x)
            },
            {
                typeof(IntRange),
                (XmlNode x) => SaveLoadManager_Load.ParseIntRange(x)
            },
            {
                typeof(FloatRange),
                (XmlNode x) => SaveLoadManager_Load.ParseFloatRange(x)
            }
        };

        private int[] tmpIndex;
    }
}