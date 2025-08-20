using System;
using System.IO;
using UnityEngine;

namespace Dummiesman
{
    public class CharWordReader
    {
        public CharWordReader(StreamReader reader, int bufferSize)
        {
            this.reader = reader;
            this.bufferSize = bufferSize;
            this.buffer = new char[this.bufferSize];
            this.word = new char[this.bufferSize];
            this.MoveNext();
        }

        public void SkipWhitespaces()
        {
            while (char.IsWhiteSpace(this.currentChar))
            {
                this.MoveNext();
            }
        }

        public void SkipWhitespaces(out bool newLinePassed)
        {
            newLinePassed = false;
            while (char.IsWhiteSpace(this.currentChar))
            {
                if (this.currentChar == '\r' || this.currentChar == '\n')
                {
                    newLinePassed = true;
                }
                this.MoveNext();
            }
        }

        public void SkipUntilNewLine()
        {
            while (this.currentChar != '\0' && this.currentChar != '\n' && this.currentChar != '\r')
            {
                this.MoveNext();
            }
            this.SkipNewLineSymbols();
        }

        public void ReadUntilWhiteSpace()
        {
            this.wordSize = 0;
            while (this.currentChar != '\0' && !char.IsWhiteSpace(this.currentChar))
            {
                this.word[this.wordSize] = this.currentChar;
                this.wordSize++;
                this.MoveNext();
            }
        }

        public void ReadUntilNewLine()
        {
            this.wordSize = 0;
            while (this.currentChar != '\0' && this.currentChar != '\n' && this.currentChar != '\r')
            {
                this.word[this.wordSize] = this.currentChar;
                this.wordSize++;
                this.MoveNext();
            }
            this.SkipNewLineSymbols();
        }

        public bool Is(string other)
        {
            if (other.Length != this.wordSize)
            {
                return false;
            }
            for (int i = 0; i < this.wordSize; i++)
            {
                if (this.word[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        public string GetString(int startIndex = 0)
        {
            if (startIndex >= this.wordSize - 1)
            {
                return string.Empty;
            }
            return new string(this.word, startIndex, this.wordSize - startIndex);
        }

        public Vector3 ReadVector()
        {
            this.SkipWhitespaces();
            float num = this.ReadFloat();
            this.SkipWhitespaces();
            float num2 = this.ReadFloat();
            bool flag;
            this.SkipWhitespaces(out flag);
            float num3 = 0f;
            if (!flag)
            {
                num3 = this.ReadFloat();
            }
            return new Vector3(num, num2, num3);
        }

        public int ReadInt()
        {
            int num = 0;
            bool flag = this.currentChar == '-';
            if (flag)
            {
                this.MoveNext();
            }
            while (this.currentChar >= '0' && this.currentChar <= '9')
            {
                int num2 = (int)(this.currentChar - '0');
                num = num * 10 + num2;
                this.MoveNext();
            }
            if (!flag)
            {
                return num;
            }
            return -num;
        }

        public float ReadFloat()
        {
            bool flag = this.currentChar == '-';
            if (flag)
            {
                this.MoveNext();
            }
            float num = (float)this.ReadInt();
            if (this.currentChar == '.' || this.currentChar == ',')
            {
                this.MoveNext();
                num += this.ReadFloatEnd();
                if (this.currentChar == 'e' || this.currentChar == 'E')
                {
                    this.MoveNext();
                    int num2 = this.ReadInt();
                    num *= Mathf.Pow(10f, (float)num2);
                }
            }
            if (flag)
            {
                num = -num;
            }
            return num;
        }

        private float ReadFloatEnd()
        {
            float num = 0f;
            float num2 = 0.1f;
            while (this.currentChar >= '0' && this.currentChar <= '9')
            {
                int num3 = (int)(this.currentChar - '0');
                num += (float)num3 * num2;
                num2 *= 0.1f;
                this.MoveNext();
            }
            return num;
        }

        private void SkipNewLineSymbols()
        {
            while (this.currentChar == '\n' || this.currentChar == '\r')
            {
                this.MoveNext();
            }
        }

        public void MoveNext()
        {
            this.currentPosition++;
            if (this.currentPosition >= this.maxPosition)
            {
                if (this.reader.EndOfStream)
                {
                    this.currentChar = '\0';
                    this.endReached = true;
                    return;
                }
                this.currentPosition = 0;
                this.maxPosition = this.reader.Read(this.buffer, 0, this.bufferSize);
            }
            this.currentChar = this.buffer[this.currentPosition];
        }

        public char[] word;

        public int wordSize;

        public bool endReached;

        private StreamReader reader;

        private int bufferSize;

        private char[] buffer;

        public char currentChar;

        private int currentPosition;

        private int maxPosition;
    }
}