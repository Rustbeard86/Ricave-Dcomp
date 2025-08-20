using System;

namespace Ricave.Core
{
    public static class WorldGenUtility
    {
        public static bool ShouldUseReinforcedGates
        {
            get
            {
                if (Get.Floor < 8)
                {
                    int floor = Get.Floor;
                    int? floorCount = Get.RunSpec.FloorCount;
                    return (floor == floorCount.GetValueOrDefault()) & (floorCount != null);
                }
                return true;
            }
        }

        public static MiniChallenge MiniChallengeForCurrentWorld
        {
            get
            {
                Rand.PushState(Calc.CombineHashes<int, int>(Get.WorldGenMemory.config.WorldSeed, 316588310));
                MiniChallenge miniChallenge;
                if (Rand.Chance(0.12f))
                {
                    miniChallenge = MiniChallenge.None;
                }
                else if (Get.RunSpec.RoomsPerFloor <= 6)
                {
                    miniChallenge = Rand.Element<MiniChallenge>(MiniChallenge.GreenKeyFragments, MiniChallenge.AncientMechanisms);
                }
                else
                {
                    miniChallenge = Rand.Element<MiniChallenge>(MiniChallenge.AncientDevices, MiniChallenge.GreenKeyFragments, MiniChallenge.AncientMechanisms);
                }
                Rand.PopState();
                return miniChallenge;
            }
        }

        public static Item GenerateMiniChallengeReward()
        {
            return ItemGenerator.Reward(true);
        }

        public static int CreateSeedForWorld(int runSeed, Place place)
        {
            int num = runSeed;
            if (place != null)
            {
                num = Calc.CombineHashes<int, int>(num, place.MyStableHash);
            }
            return num;
        }
    }
}