using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public static class NameGenerator
    {
        public static string Name(bool forSpirit = false)
        {
            string text = "";
            for (int i = 0; i < 200; i++)
            {
                text = NameGenerator.GenerateRandom(forSpirit);
                if (text.Length <= 20 && (i >= 150 || !NameGenerator.NameUsed(text)))
                {
                    break;
                }
            }
            Get.RunInfo.UsedNames.Add(text);
            return text;
        }

        private static bool NameUsed(string name)
        {
            if (name.NullOrEmpty())
            {
                return false;
            }
            if (Get.RunInfo.UsedNames.Contains(name))
            {
                return true;
            }
            using (List<TotalKillCounter.KilledBoss>.Enumerator enumerator = Get.TotalKillCounter.KilledBosses.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Name == name)
                    {
                        return true;
                    }
                }
            }
            using (List<TotalKillCounter.KilledBoss>.Enumerator enumerator = Get.KillCounter.KilledBosses.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Name == name)
                    {
                        return true;
                    }
                }
            }
            using (List<Actor>.Enumerator enumerator2 = Get.World.Actors.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.Name == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool TryGenerateGhostName(out string name)
        {
            if ((from x in (from x in Get.TotalKillCounter.KilledBosses
                            where x.ActorSpec != Get.Entity_Googon && x.ActorSpec != Get.Entity_Demon
                            select x.Name).Concat<string>(from x in Get.KillCounter.KilledBosses
                                                          where x.ActorSpec != Get.Entity_Googon && x.ActorSpec != Get.Entity_Demon
                                                          select x.Name)
                 where !x.NullOrEmpty() && x.Length <= 20 && !NameGenerator.GhostNameUsed(x)
                 select x).TryGetRandomElement<string>(out name))
            {
                Get.RunInfo.UsedGhostNames.Add(name);
                return true;
            }
            return false;
        }

        private static bool GhostNameUsed(string name)
        {
            return Get.RunInfo.UsedGhostNames.Contains(name);
        }

        private static string GenerateRandom(bool forSpirit = false)
        {
            if (forSpirit)
            {
                return "SpiritName".Translate(NameGenerator.GetRandomPart("SpiritNameGenerator").CapitalizeFirst());
            }
            return NameGenerator.GetRandomPart("NameGenerator1").CapitalizeFirst() + " " + NameGenerator.GetRandomPart("NameGenerator2").CapitalizeFirst() + NameGenerator.GetRandomPart("NameGenerator3");
        }

        private static string GetRandomPart(string key)
        {
            string text;
            if (key.TranslateSplit(",").TryGetRandomElement<string>(out text))
            {
                return text;
            }
            return "";
        }

        private const int Tries = 200;

        private const int MaxLength = 20;
    }
}