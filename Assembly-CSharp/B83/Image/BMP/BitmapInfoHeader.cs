using System;
using UnityEngine;

namespace B83.Image.BMP
{
    public struct BitmapInfoHeader
    {
        public int absWidth
        {
            get
            {
                return Mathf.Abs(this.width);
            }
        }

        public int absHeight
        {
            get
            {
                return Mathf.Abs(this.height);
            }
        }

        public uint size;

        public int width;

        public int height;

        public ushort nColorPlanes;

        public ushort nBitsPerPixel;

        public BMPComressionMode compressionMethod;

        public uint rawImageSize;

        public int xPPM;

        public int yPPM;

        public uint nPaletteColors;

        public uint nImportantColors;
    }
}