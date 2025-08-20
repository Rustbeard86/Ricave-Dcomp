using System;
using UnityEngine;

namespace Ricave.Core
{
    public class ShopkeeperGreetingManager
    {
        public void OnPlayerMoved()
        {
            if (this.greeted || !Get.World.AnyEntityOfSpec(Get.Entity_Shop))
            {
                return;
            }
            Entity entity = Get.World.GetEntitiesOfSpec(Get.Entity_Shop)[0];
            Vector3Int position = Get.NowControlledActor.Position;
            if (position.GetGridDistance(entity.Position) > 2)
            {
                return;
            }
            Vector3Int directionCardinal = entity.DirectionCardinal;
            Vector3Int vector3Int = directionCardinal.RightDir();
            if ((position == entity.Position + directionCardinal || position == entity.Position + directionCardinal + vector3Int || position == entity.Position + directionCardinal - vector3Int || position == entity.Position + directionCardinal * 2 || position == entity.Position + directionCardinal * 2 + vector3Int || position == entity.Position + directionCardinal * 2 - vector3Int) && LineOfSight.IsLineOfSight(position, entity.Position))
            {
                Get.Sound_ShopkeeperGreeting.PlayOneShot(new Vector3?(entity.Position), 1f, 1f);
                this.greeted = true;
            }
        }

        [Saved]
        private bool greeted;
    }
}