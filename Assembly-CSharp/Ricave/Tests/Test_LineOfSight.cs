using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Tests
{
    public static class Test_LineOfSight
    {
        public static void Do()
        {
            Log.Message("Running LOS checks.");
            Test_LineOfSight.Clear();
            for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
            {
                Vector3Int vector3Int = new Vector3Int(2, 2, 2);
                Vector3Int vector3Int2 = vector3Int + Vector3IntUtility.DirectionsAndInside[i];
                TestUtility.CheckTrue(LineOfSight.IsLineOfSight(vector3Int, vector3Int2), null);
                TestUtility.CheckTrue(LineOfSight.IsLineOfSight(vector3Int2, vector3Int), null);
                TestUtility.CheckTrue(LineOfSight.IsLineOfSight(vector3Int, vector3Int2 + Vector3IntUtility.DirectionsAndInside[i]), null);
            }
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 0));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 2)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 2), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 0, 1), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 2)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 0, 2), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(2, 0, 0), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 0, 1)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(2, 0, 1), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 0, 2)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(2, 0, 2), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 0));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 0, 1));
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 2)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 2), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 1)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(1, 0, 1), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 2)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(1, 0, 2), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(2, 0, 0), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 0, 1)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(2, 0, 1), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 0, 2)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(2, 0, 2), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 0, 1));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 0, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 0));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 1));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 2)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 2), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 2)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(1, 0, 2), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(2, 0, 0), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 0, 1)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(2, 0, 1), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 0, 2)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(2, 0, 2), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 0));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(0, 1, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 0), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 1, 0));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 0, 0), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 0), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 0));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 1, 0));
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 0), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 2, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(2, 2, 0), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 2, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(1, 2, 0), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(0, 2, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 2, 0), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 0));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(2, 2, 2)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(2, 2, 2), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 0, 1));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 1));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 1, 0));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 1, 0));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 1, 1));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 0));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 0, 1));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 0));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 0, 1));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 1, 0));
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 1, 0));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 1, 1));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 2, 1)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(1, 2, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 1, 0));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 1, 1));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 1));
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 0));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 0, 1));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 1));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 1, 0));
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            Test_LineOfSight.Clear();
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 0, 0));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 0, 1));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(1, 1, 0));
            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(0, 1, 1));
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1)), null);
            TestUtility.CheckTrue(!LineOfSight.IsLineOfSight(new Vector3Int(1, 1, 1), new Vector3Int(0, 0, 0)), null);
            Log.Message("Done");
        }

        private static void Clear()
        {
            foreach (Entity entity in Get.World.AllEntities.ToTemporaryList<Entity>())
            {
                entity.DeSpawn(false);
            }
        }
    }
}