using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class PistonComp : EntityComp
    {
        public new PistonCompProps Props
        {
            get
            {
                return (PistonCompProps)base.Props;
            }
        }

        public int TurnsPassed
        {
            get
            {
                return this.turnsPassed;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.turnsPassed = value;
            }
        }

        public bool Active
        {
            get
            {
                return !this.retracted;
            }
        }

        public bool Retracted
        {
            get
            {
                return this.retracted;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.retracted = value;
            }
        }

        private bool WouldLikeToBeActive
        {
            get
            {
                int num = this.Props.TurnsNotRetracted + this.Props.TurnsRetracted;
                return (this.turnsPassed + this.Props.TurnsNotRetracted) % num < this.Props.TurnsNotRetracted;
            }
        }

        protected PistonComp()
        {
        }

        public PistonComp(Entity parent)
            : base(parent)
        {
            if (!(parent is SequenceableStructure))
            {
                Log.Warning("PistonComp requires entity of type SequenceableStructure. Entity: " + ((parent != null) ? parent.ToString() : null), false);
            }
        }

        public override IEnumerable<Instruction> MakeResolveStructureInstructions()
        {
            bool prevActive = this.WouldLikeToBeActive;
            yield return new Instruction_Piston_ChangeTurnsPassed(this, 1);
            Vector3Int inFront = base.Parent.Position + base.Parent.DirectionCardinal;
            if (prevActive != this.WouldLikeToBeActive && inFront.InBounds())
            {
                bool canPlaySound = Get.NowControlledActor.Spawned && base.Parent.Position.GetGridDistance(Get.NowControlledActor.Position) <= 4;
                if (this.retracted)
                {
                    bool flag = false;
                    foreach (Entity entity2 in Get.World.GetEntitiesAt(inFront))
                    {
                        if (!(entity2 is Item) && !(entity2 is Ethereal) && (entity2.Spec.IsPermanent || (entity2.Spec.IsStructure && entity2.Spec.Structure.IsFilled) || entity2.Spec == Get.Entity_InvisibleBlocker))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        yield return new Instruction_Piston_SetRetracted(this, false);
                        foreach (Instruction instruction in InstructionSets_Entity.Spawn(Maker.Make(Get.Entity_InvisibleBlocker, null, false, false, true), inFront, null))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                        int num;
                        for (int i = 0; i < 3; i = num + 1)
                        {
                            foreach (Entity entity in Get.World.GetEntitiesAt(inFront).ToTemporaryList<Entity>())
                            {
                                if (entity.Spawned && !(entity.Position != inFront) && !(entity is Ethereal) && entity.Spec != Get.Entity_InvisibleBlocker)
                                {
                                    foreach (Instruction instruction2 in InstructionSets_Entity.Push(entity, base.Parent.DirectionCardinal, 1, false, false, false))
                                    {
                                        yield return instruction2;
                                    }
                                    enumerator2 = null;
                                    if (entity.Spawned && entity.Position == inFront)
                                    {
                                        if (entity.HP > 0)
                                        {
                                            int crushDamage = DamageUtility.ApplyDamageProtectionAndClamp(entity, 4, Get.DamageType_Crush);
                                            foreach (Instruction instruction3 in InstructionSets_Entity.Damage(entity, crushDamage, Get.DamageType_Crush, null, new Vector3Int?(base.Parent.Position), false, false, null, null, false, true))
                                            {
                                                yield return instruction3;
                                            }
                                            enumerator2 = null;
                                            if (crushDamage > 0)
                                            {
                                                yield return new Instruction_Sound(Get.Sound_LoseHealthGeneric, new Vector3?(entity.Position), 1f, 1f);
                                            }
                                        }
                                        else if (entity.Spec.IsStructure && (entity.Spec.Structure.Fragile || entity.Spec.Structure.Flammable))
                                        {
                                            foreach (Instruction instruction4 in InstructionSets_Entity.Destroy(entity, null, null))
                                            {
                                                yield return instruction4;
                                            }
                                            enumerator2 = null;
                                        }
                                        if (entity.Spawned)
                                        {
                                            Vector3Int firstDirToTry = (Rand.Bool ? base.Parent.DirectionCardinal.RightDir() : (-base.Parent.DirectionCardinal.RightDir()));
                                            Vector3Int secondDirToTry = -firstDirToTry;
                                            foreach (Instruction instruction5 in InstructionSets_Entity.Push(entity, firstDirToTry, 1, false, false, false))
                                            {
                                                yield return instruction5;
                                            }
                                            enumerator2 = null;
                                            if (entity.Spawned && entity.Position == inFront)
                                            {
                                                foreach (Instruction instruction6 in InstructionSets_Entity.Push(entity, secondDirToTry, 1, false, false, false))
                                                {
                                                    yield return instruction6;
                                                }
                                                enumerator2 = null;
                                            }
                                            if (entity.Spawned && entity.Position == inFront && (base.Parent.DirectionCardinal == Vector3IntUtility.Up || base.Parent.DirectionCardinal == Vector3IntUtility.Down))
                                            {
                                                foreach (Instruction instruction7 in InstructionSets_Entity.Push(entity, firstDirToTry.RightDir(), 1, false, false, false))
                                                {
                                                    yield return instruction7;
                                                }
                                                enumerator2 = null;
                                                if (entity.Spawned && entity.Position == inFront)
                                                {
                                                    foreach (Instruction instruction8 in InstructionSets_Entity.Push(entity, -firstDirToTry.RightDir(), 1, false, false, false))
                                                    {
                                                        yield return instruction8;
                                                    }
                                                    enumerator2 = null;
                                                }
                                            }
                                            if (entity.Spawned && entity.Position == inFront)
                                            {
                                                foreach (Instruction instruction9 in InstructionSets_Entity.Destroy(entity, null, null))
                                                {
                                                    yield return instruction9;
                                                }
                                                enumerator2 = null;
                                            }
                                            firstDirToTry = default(Vector3Int);
                                            secondDirToTry = default(Vector3Int);
                                        }
                                    }
                                    entity = null;
                                }
                            }
                            List<Entity>.Enumerator enumerator3 = default(List<Entity>.Enumerator);
                            num = i;
                        }
                    }
                    else
                    {
                        canPlaySound = false;
                    }
                }
                else
                {
                    if (base.Parent.DirectionCardinal == Vector3IntUtility.Up)
                    {
                        Vector3Int secondDirToTry = base.Parent.Position + base.Parent.DirectionCardinal * 2;
                        if (secondDirToTry.InBounds())
                        {
                            bool flag2 = false;
                            foreach (Entity entity3 in Get.World.GetEntitiesAt(inFront))
                            {
                                if (!entity3.Spec.CanPassThrough && entity3.Spec != Get.Entity_InvisibleBlocker)
                                {
                                    flag2 = true;
                                    break;
                                }
                            }
                            if (!flag2)
                            {
                                foreach (Entity entity4 in Get.World.GetEntitiesAt(secondDirToTry).ToTemporaryList<Entity>())
                                {
                                    if (entity4.Spawned && !(entity4.Position != secondDirToTry) && entity4.Spec != Get.Entity_InvisibleBlocker && (!entity4.Spec.IsStructure || !entity4.Spec.Structure.AttachesToAnything) && (!entity4.Spec.IsStructure || entity4.Spec.Structure.FallBehavior != StructureFallBehavior.None) && !entity4.Spec.IsEthereal)
                                    {
                                        foreach (Instruction instruction10 in InstructionSets_Entity.Move(entity4, inFront, true, false))
                                        {
                                            yield return instruction10;
                                        }
                                        IEnumerator<Instruction> enumerator2 = null;
                                    }
                                }
                                List<Entity>.Enumerator enumerator3 = default(List<Entity>.Enumerator);
                            }
                        }
                        secondDirToTry = default(Vector3Int);
                    }
                    Entity firstEntityOfSpecAt = Get.World.GetFirstEntityOfSpecAt(inFront, Get.Entity_InvisibleBlocker);
                    if (firstEntityOfSpecAt != null)
                    {
                        foreach (Instruction instruction11 in InstructionSets_Entity.DeSpawn(firstEntityOfSpecAt, false))
                        {
                            yield return instruction11;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                    }
                    yield return new Instruction_Piston_SetRetracted(this, true);
                }
                if (canPlaySound)
                {
                    if (prevActive)
                    {
                        if (this.Props.RetractSound != null)
                        {
                            yield return new Instruction_Sound(this.Props.RetractSound, new Vector3?(base.Parent.Position), 1f, 1f);
                        }
                    }
                    else if (this.Props.AppearSound != null)
                    {
                        yield return new Instruction_Sound(this.Props.AppearSound, new Vector3?(base.Parent.Position), 1f, 1f);
                    }
                }
            }
            yield break;
            yield break;
        }

        [Saved]
        private int turnsPassed;

        [Saved(true, false)]
        private bool retracted = true;

        private const int CrushDamage = 4;
    }
}