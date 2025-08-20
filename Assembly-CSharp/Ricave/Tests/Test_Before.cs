using System;
using Ricave.Core;

namespace Ricave.Tests
{
    public class Test_Before
    {
        [Saved]
        public Foo1 first;

        [Saved]
        public Foo2 third;

        [Saved]
        public Foo1 second;

        [Saved]
        public Foo1 second2;
    }
}