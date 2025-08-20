using System;
using Ricave.Core;

namespace Ricave.Tests
{
    public class Foo1
    {
        [Saved]
        public int sth;

        [Saved]
        public Foo2 foo;
    }
}