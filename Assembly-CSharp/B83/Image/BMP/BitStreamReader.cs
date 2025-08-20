using System;
using System.IO;

namespace B83.Image.BMP
{
    public class BitStreamReader
    {
        public BitStreamReader(BinaryReader aReader)
        {
            this.m_Reader = aReader;
        }

        public BitStreamReader(Stream aStream)
            : this(new BinaryReader(aStream))
        {
        }

        public byte ReadBit()
        {
            if (this.m_Bits <= 0)
            {
                this.m_Data = this.m_Reader.ReadByte();
                this.m_Bits = 8;
            }
            byte data = this.m_Data;
            int num = this.m_Bits - 1;
            this.m_Bits = num;
            return (byte)((data >> (num & 31)) & 1);
        }

        public ulong ReadBits(int aCount)
        {
            ulong num = 0UL;
            if (aCount <= 0 || aCount > 32)
            {
                throw new ArgumentOutOfRangeException("aCount", "aCount must be between 1 and 32 inclusive");
            }
            for (int i = aCount - 1; i >= 0; i--)
            {
                num |= (ulong)this.ReadBit() << i;
            }
            return num;
        }

        public void Flush()
        {
            this.m_Data = 0;
            this.m_Bits = 0;
        }

        private BinaryReader m_Reader;

        private byte m_Data;

        private int m_Bits;
    }
}