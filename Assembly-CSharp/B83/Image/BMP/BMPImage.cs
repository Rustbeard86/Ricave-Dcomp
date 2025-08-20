using System;
using System.Collections.Generic;
using UnityEngine;

namespace B83.Image.BMP
{
    public class BMPImage
    {
        public Texture2D ToTexture2D()
        {
            Texture2D texture2D = new Texture2D(this.info.absWidth, this.info.absHeight);
            texture2D.SetPixels32(this.imageData);
            texture2D.Apply();
            return texture2D;
        }

        public BMPFileHeader header;

        public BitmapInfoHeader info;

        public uint rMask = 16711680U;

        public uint gMask = 65280U;

        public uint bMask = 255U;

        public uint aMask;

        public List<Color32> palette;

        public Color32[] imageData;
    }
}