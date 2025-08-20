using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Ricave.Core
{
    public class Specs
    {
        public List<Spec> AllSpecs
        {
            get
            {
                return this.specs;
            }
        }

        public T Get<T>(string specID) where T : Spec
        {
            return (T)((object)this.Get(typeof(T), specID));
        }

        public Spec Get(Type type, string specID)
        {
            if (type == null)
            {
                Log.Error("Tried to get spec of null type.", false);
                return null;
            }
            if (specID == null)
            {
                Log.Error("Tried to get spec with null specID.", false);
                return null;
            }
            Specs.SpecsOfType specsOfType;
            if (!this.byType.TryGetValue(type, out specsOfType))
            {
                Log.Error(string.Concat(new string[]
                {
                    "Could not find spec named ",
                    specID,
                    " of type ",
                    (type != null) ? type.ToString() : null,
                    " (such type never added)."
                }), false);
                return null;
            }
            Spec spec;
            if (specsOfType.byName.TryGetValue(specID, out spec))
            {
                return spec;
            }
            Log.Error(string.Concat(new string[]
            {
                "Could not find spec named ",
                specID,
                " of type ",
                (type != null) ? type.ToString() : null,
                "."
            }), false);
            return null;
        }

        public bool TryGet<T>(string specID, out T spec) where T : Spec
        {
            Spec spec2;
            if (this.TryGet(typeof(T), specID, out spec2))
            {
                spec = (T)((object)spec2);
                return true;
            }
            spec = default(T);
            return false;
        }

        public bool TryGet(Type type, string specID, out Spec spec)
        {
            if (type == null || specID == null)
            {
                spec = null;
                return false;
            }
            Specs.SpecsOfType specsOfType;
            if (this.byType.TryGetValue(type, out specsOfType))
            {
                return specsOfType.byName.TryGetValue(specID, out spec);
            }
            spec = null;
            return false;
        }

        public List<T> GetAll<T>() where T : Spec
        {
            Specs.SpecsOfType specsOfType;
            if (this.byType.TryGetValue(typeof(T), out specsOfType))
            {
                if (specsOfType.specsListCorrectType == null)
                {
                    specsOfType.specsListCorrectType = new List<T>();
                    for (int i = 0; i < specsOfType.specs.Count; i++)
                    {
                        specsOfType.specsListCorrectType.Add(specsOfType.specs[i]);
                    }
                    this.byType[typeof(T)] = specsOfType;
                }
                return (List<T>)specsOfType.specsListCorrectType;
            }
            return EmptyLists<T>.Get();
        }

        public List<Spec> GetAll(Type type)
        {
            if (type == null)
            {
                Log.Error("Tried to get all specs of null type.", false);
                return EmptyLists<Spec>.Get();
            }
            Specs.SpecsOfType specsOfType;
            if (this.byType.TryGetValue(type, out specsOfType))
            {
                return specsOfType.specs;
            }
            return EmptyLists<Spec>.Get();
        }

        public bool Exists<T>(string specID)
        {
            return this.Exists(typeof(T), specID);
        }

        public bool Exists(Type type, string specID)
        {
            Specs.SpecsOfType specsOfType;
            return !(type == null) && specID != null && this.byType.TryGetValue(type, out specsOfType) && specsOfType.byName.ContainsKey(specID);
        }

        public bool Exists(Spec spec)
        {
            return this.specsHashSet.Contains(spec);
        }

        public void Add(Spec spec)
        {
            if (spec == null)
            {
                Log.Error("Tried to add null spec.", false);
                return;
            }
            if (this.specsHashSet.Contains(spec))
            {
                Log.Error("Tried to add the same spec twice.", false);
                return;
            }
            string specID = spec.SpecID;
            if (specID.NullOrEmpty())
            {
                Log.Error("Tried to add spec with null or empty specID.", false);
                return;
            }
            Specs.SpecsOfType specsOfType;
            if (this.byType.TryGetValue(spec.GetType(), out specsOfType))
            {
                if (specsOfType.byName.ContainsKey(specID))
                {
                    Log.Error("Tried to add spec with specID " + specID + " but another spec instance exists which uses the same specID.", false);
                    return;
                }
                specsOfType.specs.Add(spec);
                specsOfType.byName.Add(specID, spec);
                if (specsOfType.specsListCorrectType != null)
                {
                    specsOfType.specsListCorrectType.Add(spec);
                }
            }
            else
            {
                Specs.SpecsOfType specsOfType2 = default(Specs.SpecsOfType);
                specsOfType2.specs = new List<Spec>();
                specsOfType2.specs.Add(spec);
                specsOfType2.byName = new Dictionary<string, Spec>();
                specsOfType2.byName.Add(specID, spec);
                this.byType.Add(spec.GetType(), specsOfType2);
            }
            this.specs.Add(spec);
            this.specsHashSet.Add(spec);
        }

        public void Remove(Spec spec)
        {
            if (spec == null)
            {
                Log.Error("Tried to remove null spec.", false);
                return;
            }
            if (!this.specsHashSet.Contains(spec))
            {
                Log.Error("Tried to remove spec but it's not here.", false);
                return;
            }
            string specID = spec.SpecID;
            Specs.SpecsOfType specsOfType = this.byType[spec.GetType()];
            specsOfType.specs.Remove(spec);
            specsOfType.byName.Remove(specID);
            if (specsOfType.specsListCorrectType != null)
            {
                specsOfType.specsListCorrectType.Remove(spec);
            }
            this.specs.Remove(spec);
            this.specsHashSet.Remove(spec);
        }

        public void DestroyAndClear()
        {
            foreach (Spec spec in this.specs)
            {
                try
                {
                    spec.Destroy();
                }
                catch (Exception ex)
                {
                    Log.Error("Error while destroying spec.", ex);
                }
            }
            this.Clear();
        }

        public void Clear()
        {
            this.specs.Clear();
            this.specsHashSet.Clear();
            this.byType.Clear();
        }

        public void LoadAll()
        {
            if (this.specs.Count != 0)
            {
                Log.Error("Called Specs.LoadAll() but some specs are already loaded. They should be cleared first.", false);
                return;
            }
            ConcurrentBag<ValueTuple<XmlDocument, Mod, string>> loadedXmlDocs = new ConcurrentBag<ValueTuple<XmlDocument, Mod, string>>();
            Thread thread = new Thread(delegate
            {
                foreach (ValueTuple<string, Mod, string> valueTuple3 in FilePaths.AllSpecFiles)
                {
                    string item4 = valueTuple3.Item1;
                    Mod item5 = valueTuple3.Item2;
                    string item6 = valueTuple3.Item3;
                    try
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(item4);
                        loadedXmlDocs.Add(new ValueTuple<XmlDocument, Mod, string>(xmlDocument, item5, item6));
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Could not load or parse XML for specs from \"" + item4 + "\".", ex);
                    }
                }
            });
            thread.Start();
            SpecsToResolve specsToResolve = new SpecsToResolve();
            while (thread.IsAlive || !loadedXmlDocs.IsEmpty)
            {
                ValueTuple<XmlDocument, Mod, string> valueTuple;
                if (loadedXmlDocs.TryTake(out valueTuple))
                {
                    ValueTuple<XmlDocument, Mod, string> valueTuple2 = valueTuple;
                    XmlDocument item = valueTuple2.Item1;
                    Mod item2 = valueTuple2.Item2;
                    string item3 = valueTuple2.Item3;
                    specsToResolve.MergeAndClaim(this.LoadFrom(item, item2, item3));
                }
            }
            specsToResolve.Resolve();
            this.specs.StableSort<Spec, string>((Spec x) => x.SpecID);
            foreach (KeyValuePair<Type, Specs.SpecsOfType> keyValuePair in this.byType)
            {
                keyValuePair.Value.specs.StableSort<Spec, string>((Spec x) => x.SpecID);
            }
            this.BindGetSpecs();
        }

        private SpecsToResolve LoadFrom(XmlDocument doc, Mod modSource, string fileSourceRelPath)
        {
            List<Spec> list = new List<Spec>();
            SpecsToResolve specsToResolve = SaveLoadManager.LoadSpecs(list, doc, "Specs");
            for (int i = 0; i < list.Count; i++)
            {
                list[i].ModSource = modSource;
                list[i].FileSourceRelPath = fileSourceRelPath;
                this.Add(list[i]);
            }
            return specsToResolve;
        }

        private void BindGetSpecs()
        {
            foreach (FieldInfo fieldInfo in typeof(Get).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                Type fieldType = fieldInfo.FieldType;
                if (fieldType.IsSpecCached())
                {
                    int num = fieldInfo.Name.IndexOf('_');
                    if (num < 0)
                    {
                        Log.Error("Get field called \"" + fieldInfo.Name + "\" doesn't contain an underscore. Can't bind spec.", false);
                    }
                    else
                    {
                        string text = fieldInfo.Name.Substring(0, num);
                        string text2 = text + "Spec";
                        if (fieldType.Name != text2)
                        {
                            Log.Error(string.Concat(new string[] { "Get field called \"", fieldInfo.Name, "\" has ", text, " in its name, but its type is really ", fieldType.Name, "." }), false);
                        }
                        else
                        {
                            string text3 = fieldInfo.Name.Substring(num + 1);
                            fieldInfo.SetValue(null, this.Get(fieldType, text3));
                        }
                    }
                }
            }
        }

        private List<Spec> specs = new List<Spec>(100);

        private HashSet<Spec> specsHashSet = new HashSet<Spec>(100);

        private Dictionary<Type, Specs.SpecsOfType> byType = new Dictionary<Type, Specs.SpecsOfType>(10);

        private struct SpecsOfType
        {
            public List<Spec> specs;

            public Dictionary<string, Spec> byName;

            public IList specsListCorrectType;
        }
    }
}