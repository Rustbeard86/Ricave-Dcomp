using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dummiesman
{
    public class OBJLoader
    {
        private void LoadMaterialLibrary(string mtlLibPath)
        {
            if (this._objInfo != null && File.Exists(Path.Combine(this._objInfo.Directory.FullName, mtlLibPath)))
            {
                this.Materials = new MTLLoader().Load(Path.Combine(this._objInfo.Directory.FullName, mtlLibPath));
                return;
            }
            if (File.Exists(mtlLibPath))
            {
                this.Materials = new MTLLoader().Load(mtlLibPath);
                return;
            }
        }

        public GameObject Load(Stream input)
        {
            StreamReader streamReader = new StreamReader(input);
            Dictionary<string, OBJObjectBuilder> builderDict = new Dictionary<string, OBJObjectBuilder>();
            OBJObjectBuilder currentBuilder = null;
            string text = "default";
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            List<int> list3 = new List<int>();
            Action<string> action = delegate (string objectName)
            {
                if (!builderDict.TryGetValue(objectName, out currentBuilder))
                {
                    currentBuilder = new OBJObjectBuilder(objectName, this);
                    builderDict[objectName] = currentBuilder;
                }
            };
            action("default");
            CharWordReader charWordReader = new CharWordReader(streamReader, 4096);
            for (; ; )
            {
                charWordReader.SkipWhitespaces();
                if (charWordReader.endReached)
                {
                    break;
                }
                charWordReader.ReadUntilWhiteSpace();
                if (charWordReader.Is("#"))
                {
                    charWordReader.SkipUntilNewLine();
                }
                else if (this.Materials == null && charWordReader.Is("mtllib"))
                {
                    charWordReader.SkipWhitespaces();
                    charWordReader.ReadUntilNewLine();
                    string @string = charWordReader.GetString(0);
                    this.LoadMaterialLibrary(@string);
                }
                else if (charWordReader.Is("v"))
                {
                    this.Vertices.Add(charWordReader.ReadVector());
                }
                else if (charWordReader.Is("vn"))
                {
                    this.Normals.Add(charWordReader.ReadVector());
                }
                else if (charWordReader.Is("vt"))
                {
                    this.UVs.Add(charWordReader.ReadVector());
                }
                else if (charWordReader.Is("usemtl"))
                {
                    charWordReader.SkipWhitespaces();
                    charWordReader.ReadUntilNewLine();
                    string string2 = charWordReader.GetString(0);
                    text = string2;
                    if (this.SplitMode == SplitMode.Material)
                    {
                        action(string2);
                    }
                }
                else if ((charWordReader.Is("o") || charWordReader.Is("g")) && this.SplitMode == SplitMode.Object)
                {
                    charWordReader.ReadUntilNewLine();
                    string string3 = charWordReader.GetString(1);
                    action(string3);
                }
                else if (charWordReader.Is("f"))
                {
                    for (; ; )
                    {
                        bool flag;
                        charWordReader.SkipWhitespaces(out flag);
                        if (flag)
                        {
                            break;
                        }
                        int num = int.MinValue;
                        int num2 = int.MinValue;
                        int num3 = charWordReader.ReadInt();
                        if (charWordReader.currentChar == '/')
                        {
                            charWordReader.MoveNext();
                            if (charWordReader.currentChar != '/')
                            {
                                num2 = charWordReader.ReadInt();
                            }
                            if (charWordReader.currentChar == '/')
                            {
                                charWordReader.MoveNext();
                                num = charWordReader.ReadInt();
                            }
                        }
                        if (num3 > -2147483648)
                        {
                            if (num3 < 0)
                            {
                                num3 = this.Vertices.Count - num3;
                            }
                            num3--;
                        }
                        if (num > -2147483648)
                        {
                            if (num < 0)
                            {
                                num = this.Normals.Count - num;
                            }
                            num--;
                        }
                        if (num2 > -2147483648)
                        {
                            if (num2 < 0)
                            {
                                num2 = this.UVs.Count - num2;
                            }
                            num2--;
                        }
                        list.Add(num3);
                        list2.Add(num);
                        list3.Add(num2);
                    }
                    currentBuilder.PushFace(text, list, list2, list3);
                    list.Clear();
                    list2.Clear();
                    list3.Clear();
                }
                else
                {
                    charWordReader.SkipUntilNewLine();
                }
            }
            GameObject gameObject = new GameObject((this._objInfo != null) ? Path.GetFileNameWithoutExtension(this._objInfo.Name) : "WavefrontObject");
            gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
            foreach (KeyValuePair<string, OBJObjectBuilder> keyValuePair in builderDict)
            {
                if (keyValuePair.Value.PushedFaceCount != 0)
                {
                    keyValuePair.Value.Build().transform.SetParent(gameObject.transform, false);
                }
            }
            return gameObject;
        }

        public GameObject Load(Stream input, Stream mtlInput)
        {
            MTLLoader mtlloader = new MTLLoader();
            this.Materials = mtlloader.Load(mtlInput);
            return this.Load(input);
        }

        public GameObject Load(string path, string mtlPath)
        {
            this._objInfo = new FileInfo(path);
            if (!string.IsNullOrEmpty(mtlPath) && File.Exists(mtlPath))
            {
                MTLLoader mtlloader = new MTLLoader();
                this.Materials = mtlloader.Load(mtlPath);
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    return this.Load(fileStream);
                }
            }
            GameObject gameObject;
            using (FileStream fileStream2 = new FileStream(path, FileMode.Open))
            {
                gameObject = this.Load(fileStream2);
            }
            return gameObject;
        }

        public GameObject Load(string path)
        {
            return this.Load(path, null);
        }

        public SplitMode SplitMode = SplitMode.Object;

        internal List<Vector3> Vertices = new List<Vector3>();

        internal List<Vector3> Normals = new List<Vector3>();

        internal List<Vector2> UVs = new List<Vector2>();

        internal Dictionary<string, Material> Materials;

        private FileInfo _objInfo;
    }
}