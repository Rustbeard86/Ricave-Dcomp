using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class RoomPart_RoomFeatures : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            int num = 0;
            Func<RoomFeatureSpec, bool> <> 9__0;
            while (num < this.maxCount && !memory.debugStopFillingRooms)
            {
                bool flag = false;
                IEnumerable<RoomFeatureSpec> all = Get.Specs.GetAll<RoomFeatureSpec>();
                Func<RoomFeatureSpec, bool> func;
                if ((func = <> 9__0) == null)
                {
                    func = (<> 9__0 = delegate (RoomFeatureSpec x)
                    {
                        if (x.Category == this.category && (memory.roomFeaturesGenerated.Count(x) < x.MaxPerMap || (x == Get.RoomFeature_Spikes && Get.DungeonModifier_MoreTraps.IsActiveAndAppliesToCurrentRun())) && room.RoomFeaturesGenerated.Count(x) < x.MaxPerRoom)
                        {
                            if (x.RequiredFloor != null)
                            {
                                int floor = memory.config.Floor;
                                int? requiredFloor = x.RequiredFloor;
                                if (!((floor == requiredFloor.GetValueOrDefault()) & (requiredFloor != null)))
                                {
                                    return false;
                                }
                            }
                            if (memory.config.Floor >= x.MinFloor && (x.OnlyIfQuestActive == null || x.OnlyIfQuestActive.IsActive()))
                            {
                                return x.ExcludedRunSpecs == null || !x.ExcludedRunSpecs.Contains(Get.RunSpec);
                            }
                        }
                        return false;
                    });
                }
                foreach (RoomFeatureSpec roomFeatureSpec in from x in all.Where<RoomFeatureSpec>(func).InRandomOrder<RoomFeatureSpec>((RoomFeatureSpec x) => x.RoomFeature.SelectionWeight)
                                                            orderby x.Priority descending
                                                            select x)
                {
                    try
                    {
                        if (roomFeatureSpec.RoomFeature.TryGenerate(room, memory))
                        {
                            flag = true;
                            memory.roomFeaturesGenerated.Add(roomFeatureSpec);
                            room.OnRoomFeatureGenerated(roomFeatureSpec);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error while generating room feature.", ex);
                    }
                }
                if (!flag)
                {
                    break;
                }
                num++;
            }
        }

        [Saved]
        private RoomFeatureCategory category;

        [Saved(1, false)]
        private int maxCount = 1;
    }
}