using System;

namespace Ricave.Core
{
    public static class Maker
    {
        public static bool MakingEntity
        {
            get
            {
                return Maker.makingEntity;
            }
        }

        public static bool MakingSpell
        {
            get
            {
                return Maker.makingSpell;
            }
        }

        public static Entity Make(EntitySpec entitySpec, Action<Entity> extraSetup = null, bool assignRampUp = false, bool addActorConditionsForDifficulty = false, bool allowRandomRampUpVariation = true)
        {
            return Maker.Make<Entity>(entitySpec, extraSetup, assignRampUp, addActorConditionsForDifficulty, allowRandomRampUpVariation);
        }

        public static T Make<T>(EntitySpec entitySpec, Action<T> extraSetup = null, bool assignRampUp = false, bool addActorConditionsForDifficulty = false, bool allowRandomRampUpVariation = true) where T : Entity
        {
            Instruction.ThrowIfExecuting();
            bool flag = Maker.makingEntity;
            Maker.makingEntity = true;
            T t2;
            try
            {
                T t = (T)((object)Activator.CreateInstance(entitySpec.EntityClass, new object[] { entitySpec }));
                if (assignRampUp)
                {
                    Item item = t as Item;
                    if (item != null)
                    {
                        item.RampUp = RampUpUtility.GenerateRandomRampUpFor(item, allowRandomRampUpVariation);
                    }
                    else
                    {
                        Actor actor = t as Actor;
                        if (actor != null)
                        {
                            actor.RampUp = RampUpUtility.GenerateRandomRampUpFor(actor, allowRandomRampUpVariation);
                        }
                    }
                }
                if (addActorConditionsForDifficulty)
                {
                    Actor actor2 = t as Actor;
                    if (actor2 != null)
                    {
                        DifficultyUtility.AddConditionsForDifficulty(actor2);
                    }
                }
                if (assignRampUp || addActorConditionsForDifficulty)
                {
                    Actor actor3 = t as Actor;
                    if (actor3 != null)
                    {
                        actor3.CalculateInitialHPManaAndStamina();
                    }
                }
                if (extraSetup != null)
                {
                    extraSetup(t);
                }
                t2 = t;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while instantiating entity of spec " + ((entitySpec != null) ? entitySpec.ToString() : null) + ".", ex);
            }
            finally
            {
                Maker.makingEntity = flag;
            }
            return t2;
        }

        public static Action Make(ActionSpec actionSpec)
        {
            return Maker.Make<Action>(actionSpec);
        }

        public static T Make<T>(ActionSpec actionSpec) where T : Action
        {
            Instruction.ThrowIfExecuting();
            T t;
            try
            {
                t = (T)((object)Activator.CreateInstance(actionSpec.ActionClass, new object[] { actionSpec }));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while instantiating action of spec " + ((actionSpec != null) ? actionSpec.ToString() : null) + ".", ex);
            }
            return t;
        }

        public static Condition Make(ConditionSpec conditionSpec)
        {
            return Maker.Make<Condition>(conditionSpec);
        }

        public static T Make<T>(ConditionSpec conditionSpec) where T : Condition
        {
            Instruction.ThrowIfExecuting();
            T t;
            try
            {
                t = (T)((object)Activator.CreateInstance(conditionSpec.ConditionClass, new object[] { conditionSpec }));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while instantiating condition of spec " + ((conditionSpec != null) ? conditionSpec.ToString() : null) + ".", ex);
            }
            return t;
        }

        public static UseEffect Make(UseEffectSpec useEffectSpec)
        {
            return Maker.Make<UseEffect>(useEffectSpec);
        }

        public static T Make<T>(UseEffectSpec useEffectSpec) where T : UseEffect
        {
            Instruction.ThrowIfExecuting();
            T t;
            try
            {
                t = (T)((object)Activator.CreateInstance(useEffectSpec.UseEffectClass, new object[] { useEffectSpec }));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while instantiating use effect of spec " + ((useEffectSpec != null) ? useEffectSpec.ToString() : null) + ".", ex);
            }
            return t;
        }

        public static Spell Make(SpellSpec spellSpec)
        {
            Instruction.ThrowIfExecuting();
            bool flag = Maker.makingSpell;
            Maker.makingSpell = true;
            Spell spell;
            try
            {
                spell = new Spell(spellSpec);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while instantiating spell of spec " + ((spellSpec != null) ? spellSpec.ToString() : null) + ".", ex);
            }
            finally
            {
                Maker.makingSpell = flag;
            }
            return spell;
        }

        public static WorldSituation Make(WorldSituationSpec worldSituationSpec)
        {
            return Maker.Make<WorldSituation>(worldSituationSpec);
        }

        public static T Make<T>(WorldSituationSpec worldSituationSpec) where T : WorldSituation
        {
            Instruction.ThrowIfExecuting();
            T t;
            try
            {
                t = (T)((object)Activator.CreateInstance(worldSituationSpec.WorldSituationClass, new object[] { worldSituationSpec }));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while instantiating world situation of spec " + ((worldSituationSpec != null) ? worldSituationSpec.ToString() : null) + ".", ex);
            }
            return t;
        }

        private static bool makingEntity;

        private static bool makingSpell;
    }
}