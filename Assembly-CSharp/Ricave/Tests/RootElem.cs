using System;
using System.Collections.Generic;
using Ricave.Core;

namespace Ricave.Tests
{
    public class RootElem
    {
        [Saved]
        public string str;

        [Saved]
        public Dictionary<string, Elem> dict;
    }
}