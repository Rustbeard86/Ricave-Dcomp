using System;
using Ricave.Core;

namespace Ricave.Tests
{
    public static class Test_Saveable
    {
        public static void Do()
        {
            RootElem rootElem = new RootElem();
            SaveLoadManager.Load(rootElem, "test.xml", "root");
            Log.Message(rootElem.dict["elem"].self.self.self.sth.ToString());
            Log.Message(rootElem.dict["elem"].self.self.self.otherstr);
        }

        public static void TestMissingRef()
        {
            Test_Before test_Before = new Test_Before();
            test_Before.first = new Foo1Inherit
            {
                sth = 15,
                foo = new Foo2
                {
                    bar = 10
                }
            };
            test_Before.second = test_Before.first;
            test_Before.second2 = test_Before.first;
            test_Before.third = test_Before.first.foo;
            SaveLoadManager.Save(test_Before, "test.xml", "root");
            Test_After test_After = new Test_After();
            SaveLoadManager.Load(test_After, "test.xml", "root");
            string[] array = new string[10];
            array[0] = "Loaded: ";
            int num = 1;
            Type type = test_After.second.GetType();
            array[num] = ((type != null) ? type.ToString() : null);
            array[2] = " ";
            int num2 = 3;
            Foo1 second = test_After.second;
            array[num2] = ((second != null) ? new int?(second.sth) : null).ToString();
            array[4] = " ";
            int num3 = 5;
            Foo2 third = test_After.third;
            array[num3] = ((third != null) ? new int?(third.bar) : null).ToString();
            array[6] = " ";
            array[7] = (test_After.second.foo == test_After.third).ToString();
            array[8] = " ";
            array[9] = (test_After.second == test_After.second2).ToString();
            Log.Message(string.Concat(array));
        }
    }
}