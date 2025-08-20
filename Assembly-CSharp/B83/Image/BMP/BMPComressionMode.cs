using System;

namespace B83.Image.BMP
{
    public enum BMPComressionMode
    {
        BI_RGB,

        BI_RLE8,

        BI_RLE4,

        BI_BITFIELDS,

        BI_JPEG,

        BI_PNG,

        BI_ALPHABITFIELDS,

        BI_CMYK = 11,

        BI_CMYKRLE8,

        BI_CMYKRLE4
    }
}