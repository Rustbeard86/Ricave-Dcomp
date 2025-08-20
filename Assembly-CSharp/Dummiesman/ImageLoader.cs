using System;
using System.IO;
using B83.Image.BMP;
using UnityEngine;

namespace Dummiesman
{
    public class ImageLoader
    {
        public static void SetNormalMap(ref Texture2D tex)
        {
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                Color color = pixels[i];
                color.r = pixels[i].g;
                color.a = pixels[i].r;
                pixels[i] = color;
            }
            tex.SetPixels(pixels);
            tex.Apply(true);
        }

        public static Texture2D LoadTexture(Stream stream, ImageLoader.TextureFormat format)
        {
            if (format == ImageLoader.TextureFormat.BMP)
            {
                return new BMPLoader().LoadBMP(stream).ToTexture2D();
            }
            if (format == ImageLoader.TextureFormat.DDS)
            {
                return DDSLoader.Load(stream);
            }
            if (format == ImageLoader.TextureFormat.JPG || format == ImageLoader.TextureFormat.PNG)
            {
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, (int)stream.Length);
                Texture2D texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(array);
                return texture2D;
            }
            if (format == ImageLoader.TextureFormat.TGA)
            {
                return TGALoader.Load(stream);
            }
            return null;
        }

        public static Texture2D LoadTexture(string fn)
        {
            if (!File.Exists(fn))
            {
                return null;
            }
            byte[] array = File.ReadAllBytes(fn);
            string text = Path.GetExtension(fn).ToLower();
            string fileName = Path.GetFileName(fn);
            Texture2D texture2D = null;
            uint num = < PrivateImplementationDetails >.ComputeStringHash(text);
            if (num <= 1388056268U)
            {
                if (num != 175576948U)
                {
                    if (num != 1128223456U)
                    {
                        if (num != 1388056268U)
                        {
                            goto IL_0219;
                        }
                        if (!(text == ".jpg"))
                        {
                            goto IL_0219;
                        }
                    }
                    else if (!(text == ".png"))
                    {
                        goto IL_0219;
                    }
                }
                else
                {
                    if (!(text == ".bmp"))
                    {
                        goto IL_0219;
                    }
                    texture2D = new BMPLoader().LoadBMP(array).ToTexture2D();
                    goto IL_022F;
                }
            }
            else if (num <= 3508679078U)
            {
                if (num != 2215576294U)
                {
                    if (num != 3508679078U)
                    {
                        goto IL_0219;
                    }
                    if (!(text == ".dds"))
                    {
                        goto IL_0219;
                    }
                    texture2D = DDSLoader.Load(array);
                    goto IL_022F;
                }
                else
                {
                    if (!(text == ".crn"))
                    {
                        goto IL_0219;
                    }
                    byte[] array2 = array;
                    ushort num2 = BitConverter.ToUInt16(new byte[]
                    {
                        array2[13],
                        array2[12]
                    }, 0);
                    ushort num3 = BitConverter.ToUInt16(new byte[]
                    {
                        array2[15],
                        array2[14]
                    }, 0);
                    byte b = array2[18];
                    global::UnityEngine.TextureFormat textureFormat;
                    if (b == 0)
                    {
                        textureFormat = global::UnityEngine.TextureFormat.DXT1Crunched;
                    }
                    else if (b == 2)
                    {
                        textureFormat = global::UnityEngine.TextureFormat.DXT5Crunched;
                    }
                    else
                    {
                        if (b != 12)
                        {
                            Debug.LogError(string.Concat(new string[]
                            {
                                "Could not load crunched texture ",
                                fileName,
                                " because its format is not supported (",
                                b.ToString(),
                                "): ",
                                fn
                            }));
                            goto IL_022F;
                        }
                        textureFormat = global::UnityEngine.TextureFormat.ETC2_RGBA8Crunched;
                    }
                    texture2D = new Texture2D((int)num2, (int)num3, textureFormat, true);
                    texture2D.LoadRawTextureData(array2);
                    texture2D.Apply(true);
                    goto IL_022F;
                }
            }
            else if (num != 4178554255U)
            {
                if (num != 4285742059U)
                {
                    goto IL_0219;
                }
                if (!(text == ".tga"))
                {
                    goto IL_0219;
                }
                texture2D = TGALoader.Load(array);
                goto IL_022F;
            }
            else if (!(text == ".jpeg"))
            {
                goto IL_0219;
            }
            texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(array);
            goto IL_022F;
        IL_0219:
            Debug.LogError("Could not load texture " + fileName + " because its format is not supported : " + fn);
        IL_022F:
            if (texture2D != null)
            {
                texture2D = ImageLoaderHelper.VerifyFormat(texture2D);
                texture2D.name = Path.GetFileNameWithoutExtension(fn);
            }
            return texture2D;
        }

        public enum TextureFormat
        {
            DDS,

            TGA,

            BMP,

            PNG,

            JPG,

            CRN
        }
    }
}