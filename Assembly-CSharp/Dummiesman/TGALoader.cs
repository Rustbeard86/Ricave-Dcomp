using System;
using System.IO;
using Dummiesman.Extensions;
using UnityEngine;

namespace Dummiesman
{
    public class TGALoader
    {
        private static int GetBits(byte b, int offset, int count)
        {
            return (b >> offset) & ((1 << count) - 1);
        }

        private static Color32[] LoadRawTGAData(BinaryReader r, int bitDepth, int width, int height)
        {
            Color32[] array = new Color32[width * height];
            byte[] array2 = r.ReadBytes(width * height * (bitDepth / 8));
            ImageLoaderHelper.FillPixelArray(array, array2, bitDepth / 8, true);
            return array;
        }

        private static Color32[] LoadRLETGAData(BinaryReader r, int bitDepth, int width, int height)
        {
            Color32[] array = new Color32[width * height];
            int num;
            for (int i = 0; i < array.Length; i += num)
            {
                byte b = r.ReadByte();
                int bits = TGALoader.GetBits(b, 7, 1);
                num = TGALoader.GetBits(b, 0, 7) + 1;
                if (bits == 0)
                {
                    for (int j = 0; j < num; j++)
                    {
                        Color32 color = ((bitDepth == 32) ? r.ReadColor32RGBA().FlipRB() : r.ReadColor32RGB().FlipRB());
                        array[j + i] = color;
                    }
                }
                else
                {
                    Color32 color2 = ((bitDepth == 32) ? r.ReadColor32RGBA().FlipRB() : r.ReadColor32RGB().FlipRB());
                    for (int k = 0; k < num; k++)
                    {
                        array[k + i] = color2;
                    }
                }
            }
            return array;
        }

        public static Texture2D Load(string fileName)
        {
            Texture2D texture2D;
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                texture2D = TGALoader.Load(fileStream);
            }
            return texture2D;
        }

        public static Texture2D Load(byte[] bytes)
        {
            Texture2D texture2D;
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                texture2D = TGALoader.Load(memoryStream);
            }
            return texture2D;
        }

        public static Texture2D Load(Stream TGAStream)
        {
            Texture2D texture2D;
            using (BinaryReader binaryReader = new BinaryReader(TGAStream))
            {
                binaryReader.BaseStream.Seek(2L, SeekOrigin.Begin);
                byte b = binaryReader.ReadByte();
                if (b != 10 && b != 2)
                {
                    Debug.LogError(string.Format("Unsupported targa image type. ({0})", b));
                    texture2D = null;
                }
                else
                {
                    binaryReader.BaseStream.Seek(12L, SeekOrigin.Begin);
                    short num = binaryReader.ReadInt16();
                    short num2 = binaryReader.ReadInt16();
                    int num3 = (int)binaryReader.ReadByte();
                    if (num3 < 24)
                    {
                        throw new Exception("Tried to load TGA with unsupported bit depth");
                    }
                    binaryReader.BaseStream.Seek(1L, SeekOrigin.Current);
                    Texture2D texture2D2 = new Texture2D((int)num, (int)num2, (num3 == 24) ? TextureFormat.RGB24 : TextureFormat.ARGB32, true);
                    if (b == 2)
                    {
                        texture2D2.SetPixels32(TGALoader.LoadRawTGAData(binaryReader, num3, (int)num, (int)num2));
                    }
                    else
                    {
                        texture2D2.SetPixels32(TGALoader.LoadRLETGAData(binaryReader, num3, (int)num, (int)num2));
                    }
                    texture2D2.Apply();
                    texture2D = texture2D2;
                }
            }
            return texture2D;
        }
    }
}