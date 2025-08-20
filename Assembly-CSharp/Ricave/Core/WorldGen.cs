using System;

namespace Ricave.Core
{
    public static class WorldGen
    {
        public static bool Working
        {
            get
            {
                return WorldGen.working;
            }
        }

        public static bool DoingGenPasses
        {
            get
            {
                return WorldGen.doingGenPasses;
            }
        }

        public static WorldGenMemory CurMemory
        {
            get
            {
                return WorldGen.curMemory;
            }
        }

        public static WorldGenMemory Generate(World world, WorldConfig config)
        {
            if (WorldGen.working)
            {
                Log.Error("Tried to generate world but world generation is already in process. Nested Generate() calls are not allowed.", false);
                return null;
            }
            if (config == null)
            {
                Log.Error("Tried to generate world using null config.", false);
                return null;
            }
            if (Get.World != world)
            {
                Log.Error("Get.World needs to be the same as the world being generated.", false);
                return null;
            }
            WorldGen.working = true;
            Profiler.Begin("WorldGen.Generate()");
            WorldGenMemory worldGenMemory2;
            try
            {
                string[] array = new string[5];
                array[0] = "Generating new world with generator ";
                int num = 1;
                WorldSpec spec = config.Spec;
                array[num] = ((spec != null) ? spec.ToString() : null);
                array[2] = " and seed ";
                array[3] = config.WorldSeed.ToString();
                array[4] = ".";
                Log.Message(string.Concat(array));
                WorldGenMemory worldGenMemory = new WorldGenMemory();
                WorldGen.curMemory = worldGenMemory;
                worldGenMemory.config = config;
                if (config.Spec == null)
                {
                    Log.Error("Tried to generate world using null WorldGenSpec.", false);
                    worldGenMemory2 = worldGenMemory;
                }
                else
                {
                    WorldGen.doingGenPasses = true;
                    foreach (GenPassSpec genPassSpec in config.Spec.GenPassesInOrder)
                    {
                        Rand.PushState(Calc.CombineHashes<int, int>(config.WorldSeed, genPassSpec.GenPass.SeedPart));
                        Profiler.Begin(genPassSpec.SpecID);
                        try
                        {
                            genPassSpec.GenPass.DoPass(worldGenMemory);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error while doing gen pass.", ex);
                        }
                        finally
                        {
                            Profiler.End();
                            Rand.PopState();
                        }
                    }
                    WorldGen.doingGenPasses = false;
                    WorldGen.DoFinalInit(world);
                    Get.ModsEventsManager.CallEvent(ModEventType.WorldGenerated, world);
                    worldGenMemory2 = worldGenMemory;
                }
            }
            finally
            {
                WorldGen.working = false;
                WorldGen.doingGenPasses = false;
                WorldGen.curMemory = null;
                Profiler.End();
            }
            return worldGenMemory2;
        }

        private static void DoFinalInit(World world)
        {
            Get.TurnManager.AddSequenceable(world.WorldSequenceable, 0, -1);
            foreach (SequenceableStructure sequenceableStructure in world.SequenceableStructures)
            {
                Get.TurnManager.AddSequenceable(sequenceableStructure, 0, -1);
            }
            foreach (Actor actor in world.Actors)
            {
                Get.TurnManager.AddSequenceable(actor, 0, -1);
                foreach (Condition condition in actor.ConditionsAccumulated.AllConditions)
                {
                    Get.TurnManager.AddSequenceable(condition, 0, -1);
                }
            }
            foreach (Entity entity in world.AllEntities)
            {
                try
                {
                    entity.UpdateGameObjectAppearance();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in UpdateGameObjectAppearance().", ex);
                }
            }
            if (Get.DungeonModifier_EveryoneStartsAwake.IsActiveAndAppliesToCurrentRun())
            {
                foreach (Actor actor2 in world.Actors)
                {
                    Condition firstOfSpec = actor2.Conditions.GetFirstOfSpec(Get.Condition_Sleeping);
                    if (firstOfSpec != null)
                    {
                        foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(firstOfSpec, false, false))
                        {
                            instruction.Do();
                        }
                    }
                }
            }
        }

        private static bool working;

        private static bool doingGenPasses;

        private static WorldGenMemory curMemory;
    }
}