using System;

namespace Ricave.Core
{
    public class AINode_IsBestWeaponRanged : AINode_Conditional
    {
        protected override bool ConditionMet(Actor actor)
        {
            IUsable weaponToUse = AIUtility_Attack.GetWeaponToUse(actor, true);
            return weaponToUse != null && weaponToUse.UseRange >= 2;
        }
    }
}