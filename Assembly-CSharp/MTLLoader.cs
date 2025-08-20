using System;
using System.Collections.Generic;
using System.IO;
using Dummiesman;
using UnityEngine;

public class MTLLoader
{
    public virtual Texture2D TextureLoadFunction(string path, bool isNormalMap)
    {
        foreach (string text in this.SearchPaths)
        {
            string text2 = Path.Combine((this._objFileInfo != null) ? text.Replace("%FileName%", Path.GetFileNameWithoutExtension(this._objFileInfo.Name)) : text, path);
            if (File.Exists(text2))
            {
                Texture2D texture2D = ImageLoader.LoadTexture(text2);
                if (isNormalMap)
                {
                    texture2D = ImageUtils.ConvertToNormalMap(texture2D);
                }
                return texture2D;
            }
        }
        return null;
    }

    private Texture2D TryLoadTexture(string texturePath, bool normalMap = false)
    {
        texturePath = texturePath.Replace('\\', Path.DirectorySeparatorChar);
        texturePath = texturePath.Replace('/', Path.DirectorySeparatorChar);
        return this.TextureLoadFunction(texturePath, normalMap);
    }

    private int GetArgValueCount(string arg)
    {
        uint num = < PrivateImplementationDetails >.ComputeStringHash(arg);
        if (num <= 1657636085U)
        {
            if (num > 93314553U)
            {
                if (num != 1456304657U)
                {
                    if (num != 1473082276U)
                    {
                        if (num != 1657636085U)
                        {
                            return -1;
                        }
                        if (!(arg == "-o"))
                        {
                            return -1;
                        }
                    }
                    else if (!(arg == "-t"))
                    {
                        return -1;
                    }
                }
                else if (!(arg == "-s"))
                {
                    return -1;
                }
                return 3;
            }
            if (num != 92475910U)
            {
                if (num != 93314553U)
                {
                    return -1;
                }
                if (!(arg == "-bm"))
                {
                    return -1;
                }
            }
            else
            {
                if (!(arg == "-mm"))
                {
                    return -1;
                }
                return 2;
            }
        }
        else if (num <= 2397666853U)
        {
            if (num != 2280877111U)
            {
                if (num != 2397666853U)
                {
                    return -1;
                }
                if (!(arg == "-texres"))
                {
                    return -1;
                }
            }
            else if (!(arg == "-clamp"))
            {
                return -1;
            }
        }
        else if (num != 3939001348U)
        {
            if (num != 3980883183U)
            {
                if (num != 3997660802U)
                {
                    return -1;
                }
                if (!(arg == "-blendu"))
                {
                    return -1;
                }
            }
            else if (!(arg == "-blendv"))
            {
                return -1;
            }
        }
        else if (!(arg == "-imfchan"))
        {
            return -1;
        }
        return 1;
    }

    private int GetTexNameIndex(string[] components)
    {
        for (int i = 1; i < components.Length; i++)
        {
            int argValueCount = this.GetArgValueCount(components[i]);
            if (argValueCount < 0)
            {
                return i;
            }
            i += argValueCount;
        }
        return -1;
    }

    private float GetArgValue(string[] components, string arg, float fallback = 1f)
    {
        string text = arg.ToLower();
        for (int i = 1; i < components.Length - 1; i++)
        {
            string text2 = components[i].ToLower();
            if (text == text2)
            {
                return OBJLoaderHelper.FastFloatParse(components[i + 1]);
            }
        }
        return fallback;
    }

    private string GetTexPathFromMapStatement(string processedLine, string[] splitLine)
    {
        int texNameIndex = this.GetTexNameIndex(splitLine);
        if (texNameIndex < 0)
        {
            Debug.LogError("texNameCmpIdx < 0 on line " + processedLine + ". Texture not loaded.");
            return null;
        }
        int num = processedLine.IndexOf(splitLine[texNameIndex]);
        return processedLine.Substring(num);
    }

    public Dictionary<string, Material> Load(Stream input)
    {
        StringReader stringReader = new StringReader(new StreamReader(input).ReadToEnd());
        Dictionary<string, Material> dictionary = new Dictionary<string, Material>();
        Material material = null;
        for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                string text2 = text.Clean();
                string[] array = text2.Split(' ', StringSplitOptions.None);
                if (array.Length >= 2 && text2[0] != '#')
                {
                    if (array[0] == "newmtl")
                    {
                        string text3 = text2.Substring(7);
                        Material material2 = new Material(Shader.Find("Standard (Specular setup)"))
                        {
                            name = text3
                        };
                        dictionary[text3] = material2;
                        material = material2;
                    }
                    else if (!(material == null))
                    {
                        if (array[0] == "Kd" || array[0] == "kd")
                        {
                            Color color = material.GetColor("_Color");
                            Color color2 = OBJLoaderHelper.ColorFromStrArray(array, 1f);
                            material.SetColor("_Color", new Color(color2.r, color2.g, color2.b, color.a));
                        }
                        else if (array[0] == "map_Kd" || array[0] == "map_kd")
                        {
                            string texPathFromMapStatement = this.GetTexPathFromMapStatement(text2, array);
                            if (texPathFromMapStatement != null)
                            {
                                Texture2D texture2D = this.TryLoadTexture(texPathFromMapStatement, false);
                                material.SetTexture("_MainTex", texture2D);
                                if (texture2D != null && (texture2D.format == TextureFormat.DXT5 || texture2D.format == TextureFormat.ARGB32))
                                {
                                    OBJLoaderHelper.EnableMaterialTransparency(material);
                                }
                                if (Path.GetExtension(texPathFromMapStatement).ToLower() == ".dds")
                                {
                                    material.mainTextureScale = new Vector2(1f, -1f);
                                }
                            }
                        }
                        else if (array[0] == "map_Bump" || array[0] == "map_bump")
                        {
                            string texPathFromMapStatement2 = this.GetTexPathFromMapStatement(text2, array);
                            if (texPathFromMapStatement2 != null)
                            {
                                Texture2D texture2D2 = this.TryLoadTexture(texPathFromMapStatement2, true);
                                float argValue = this.GetArgValue(array, "-bm", 1f);
                                if (texture2D2 != null)
                                {
                                    material.SetTexture("_BumpMap", texture2D2);
                                    material.SetFloat("_BumpScale", argValue);
                                    material.EnableKeyword("_NORMALMAP");
                                }
                            }
                        }
                        else if (array[0] == "Ks" || array[0] == "ks")
                        {
                            material.SetColor("_SpecColor", OBJLoaderHelper.ColorFromStrArray(array, 1f));
                        }
                        else if (array[0] == "Ka" || array[0] == "ka")
                        {
                            material.SetColor("_EmissionColor", OBJLoaderHelper.ColorFromStrArray(array, 0.05f));
                            material.EnableKeyword("_EMISSION");
                        }
                        else if (array[0] == "map_Ka" || array[0] == "map_ka")
                        {
                            string texPathFromMapStatement3 = this.GetTexPathFromMapStatement(text2, array);
                            if (texPathFromMapStatement3 != null)
                            {
                                material.SetTexture("_EmissionMap", this.TryLoadTexture(texPathFromMapStatement3, false));
                            }
                        }
                        else if (array[0] == "d" || array[0] == "Tr")
                        {
                            float num = OBJLoaderHelper.FastFloatParse(array[1]);
                            if (array[0] == "Tr")
                            {
                                num = 1f - num;
                            }
                            if (num < 1f - Mathf.Epsilon)
                            {
                                Color color3 = material.GetColor("_Color");
                                color3.a = num;
                                material.SetColor("_Color", color3);
                                OBJLoaderHelper.EnableMaterialTransparency(material);
                            }
                        }
                        else if (array[0] == "Ns" || array[0] == "ns")
                        {
                            float num2 = OBJLoaderHelper.FastFloatParse(array[1]);
                            num2 /= 1000f;
                            material.SetFloat("_Glossiness", num2);
                        }
                    }
                }
            }
        }
        return dictionary;
    }

    public Dictionary<string, Material> Load(string path)
    {
        this._objFileInfo = new FileInfo(path);
        this.SearchPaths.Add(this._objFileInfo.Directory.FullName);
        Dictionary<string, Material> dictionary;
        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
            dictionary = this.Load(fileStream);
        }
        return dictionary;
    }

    public List<string> SearchPaths = new List<string>
    {
        "%FileName%_Textures",
        string.Empty
    };

    private FileInfo _objFileInfo;
}