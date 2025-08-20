using System;
using System.IO;
using UnityEngine;

namespace Dummiesman
{
    public static class DDSLoader
    {
        public static Texture2D Load(Stream ddsStream)
        {
            byte[] array = new byte[ddsStream.Length];
            ddsStream.Read(array, 0, (int)ddsStream.Length);
            return DDSLoader.Load(array);
        }

        public static Texture2D Load(string ddsPath)
        {
            return DDSLoader.Load(File.ReadAllBytes(ddsPath));
        }

        public static Texture2D Load(byte[] ddsBytes)
        {
            Texture2D texture2D2;
            try
            {
                if (ddsBytes[4] != 124)
                {
                    throw new Exception("Invalid DDS header. Structure length is incrrrect.");
                }
                byte b = ddsBytes[87];
                if (b != 49 && b != 53)
                {
                    throw new Exception("Cannot load DDS due to an unsupported pixel format. Needs to be DXT1 or DXT5.");
                }
                int num = (int)ddsBytes[13] * 256 + (int)ddsBytes[12];
                int num2 = (int)ddsBytes[17] * 256 + (int)ddsBytes[16];
                bool flag = ddsBytes[28] > 0;
                TextureFormat textureFormat = ((b == 49) ? TextureFormat.DXT1 : TextureFormat.DXT5);
                int num3 = 128;
                byte[] array = new byte[ddsBytes.Length - num3];
                Buffer.BlockCopy(ddsBytes, num3, array, 0, ddsBytes.Length - num3);
                Texture2D texture2D = new Texture2D(num2, num, textureFormat, flag);
                texture2D.LoadRawTextureData(array);
                texture2D.Apply();
                texture2D2 = texture2D;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while loading DirectDraw Surface: " + ex.Message);
            }
            return texture2D2;
        }
    }
}