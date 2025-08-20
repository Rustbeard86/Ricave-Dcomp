using System;
using Ricave.Core;

namespace Ricave.Tests
{
    public class Elem
    {
        [Saved]
        public Elem self;

        [Saved]
        public int sth;

        [Saved]
        public string otherstr;
    }
}