using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Spawn : UseEffect
    {
        public EntitySpec EntitySpec
        {
            get
            {
                return this.entitySpec;
            }
        }

        public List<EntitySpec> EntitySpecs
        {
            get
            {
                return this.entitySpecs;
            }
        }

        public bool RequireFloor
        {
            get
            {
                return this.requireFloor;
            }
        }

        public bool NeverUseNearMode
        {
            get
            {
                return this.neverUseNearMode;
            }
        }

        public bool SetRampUp
        {
            get
            {
                return this.setRampUp;
            }
        }

        public bool AtSource
        {
            get
            {
                return this.atSource;
            }
        }

        public VisualEffectSpec VisualEffectAtSpawnedPos
        {
            get
            {
                return this.visualEffectAtSpawnedPos;
            }
        }

        public SoundSpec SoundAtSpawnedPos
        {
            get
            {
                return this.soundAtSpawnedPos;
            }
        }

        public List<Condition> AddConditions
        {
            get
            {
                return this.addConditions;
            }
        }

        public bool MaxOneFollower
        {
            get
            {
                return this.maxOneFollower;
            }
        }

        public override bool AoEHandledManually
        {
            get
            {
                return true;
            }
        }

        protected override bool CanWriteAoERadius
        {
            get
            {
                int? aoERadius = base.AoERadius;
                int num = 1;
                return !((aoERadius.GetValueOrDefault() == num) & (aoERadius != null));
            }
        }

        public override Texture2D Icon
        {
            get
            {
                return this.MainEntitySpecForIcon.IconAdjusted;
            }
        }

        public override Color IconColor
        {
            get
            {
                return this.MainEntitySpecForIcon.IconColorAdjusted;
            }
        }

        public override string LabelBase
        {
            get
            {
                string text;
                if (this.entitySpec != null)
                {
                    text = base.Spec.LabelFormat.Formatted(this.entitySpec);
                }
                else if (!this.entitySpecs.NullOrEmpty<EntitySpec>())
                {
                    if (this.entitySpecsLabelCached == null)
                    {
                        this.entitySpecsLabelCached = StringUtility.ToCommaListOr(this.entitySpecs.Select<EntitySpec, string>((EntitySpec x) => RichText.Label(x, false)));
                    }
                    text = base.Spec.LabelFormat.Formatted(this.entitySpecsLabelCached);
                }
                else
                {
                    text = "";
                }
                string text2 = text;
                string text3 = ((this.SpawnedEntityLifespan > 0) ? RichText.Turns(" ({0})".Formatted(StringUtility.TurnsString(this.SpawnedEntityLifespan))) : "");
                int? aoERadius = base.AoERadius;
                int num = 1;
                return StringUtility.Concat(text2, text3, ((aoERadius.GetValueOrDefault() == num) & (aoERadius != null)) ? " {0}".Formatted("AroundTarget".Translate()) : "");
            }
        }

        private int SpawnedEntityLifespan
        {
            get
            {
                foreach (EntityCompProps entityCompProps in this.MainEntitySpecForIcon.AllCompProps)
                {
                    LifespanCompProps lifespanCompProps = entityCompProps as LifespanCompProps;
                    if (lifespanCompProps != null)
                    {
                        return lifespanCompProps.LifespanTurns;
                    }
                }
                return -1;
            }
        }

        private EntitySpec MainEntitySpecForIcon
        {
            get
            {
                if (this.entitySpec != null)
                {
                    return this.entitySpec;
                }
                if (!this.entitySpecs.NullOrEmpty<EntitySpec>())
                {
                    return this.entitySpecs[0];
                }
                return null;
            }
        }

        protected UseEffect_Spawn()
        {
        }

        public UseEffect_Spawn(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public UseEffect_Spawn(UseEffectSpec spec, EntitySpec entitySpec, bool requireFloor, float chance = 1f, int usesLeft = 0)
            : base(spec, chance, usesLeft, null, false)
        {
            this.entitySpec = entitySpec;
            this.requireFloor = requireFloor;
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_Spawn useEffect_Spawn = (UseEffect_Spawn)clone;
            useEffect_Spawn.entitySpec = this.entitySpec;
            useEffect_Spawn.entitySpecs = this.entitySpecs;
            useEffect_Spawn.requireFloor = this.requireFloor;
            useEffect_Spawn.neverUseNearMode = this.neverUseNearMode;
            useEffect_Spawn.setRampUp = this.setRampUp;
            useEffect_Spawn.atSource = this.atSource;
            useEffect_Spawn.visualEffectAtSpawnedPos = this.visualEffectAtSpawnedPos;
            useEffect_Spawn.soundAtSpawnedPos = this.soundAtSpawnedPos;
            useEffect_Spawn.addConditions = this.addConditions;
            useEffect_Spawn.maxOneFollower = this.maxOneFollower;
        }

        public override bool PreventEntireUse(Actor user, IUsable usable, Target target, StringSlot outReason = null)
        {
            if (this.maxOneFollower && user != null && user.IsPlayerParty)
            {
                foreach (Actor actor in Get.World.Actors)
                {
                    if (actor.AI == Get.AI_PlayerFollower && ((this.entitySpec != null && actor.Spec == this.entitySpec) || (this.entitySpecs != null && this.entitySpecs.Contains(actor.Spec))))
                    {
                        if (outReason != null)
                        {
                            outReason.Set("CantSummonFollowerAlreadyExists".Translate(actor.Spec));
                        }
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Vector3Int vector3Int = (this.atSource ? UseEffect_Sound.GetSourcePos(user, usable, target) : target.Position);
            if (base.AoERadius == null)
            {
                Entity toSpawn = null;
                Vector3Int spawnPos = default(Vector3Int);
                if (this.neverUseNearMode)
                {
                    if (this.entitySpec != null)
                    {
                        if (SpawnPositionFinder.CanSpawnAt(this.entitySpec, vector3Int, null, false, null))
                        {
                            toSpawn = this.MakeEntity(this.entitySpec);
                            spawnPos = vector3Int;
                            goto IL_01B5;
                        }
                        goto IL_01B5;
                    }
                    else
                    {
                        if (this.entitySpecs == null)
                        {
                            goto IL_01B5;
                        }
                        using (IEnumerator<EntitySpec> enumerator = this.entitySpecs.InRandomOrder<EntitySpec>().GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                EntitySpec entitySpec = enumerator.Current;
                                if (SpawnPositionFinder.CanSpawnAt(entitySpec, vector3Int, null, false, null))
                                {
                                    toSpawn = this.MakeEntity(entitySpec);
                                    spawnPos = vector3Int;
                                }
                            }
                            goto IL_01B5;
                        }
                    }
                }
                EntitySpec entitySpec2;
                if (this.entitySpec != null)
                {
                    toSpawn = this.MakeEntity(this.entitySpec);
                    spawnPos = SpawnPositionFinder.Near(vector3Int, toSpawn, false, false, null);
                }
                else if (this.entitySpecs != null && this.entitySpecs.TryGetRandomElement<EntitySpec>(out entitySpec2))
                {
                    toSpawn = this.MakeEntity(entitySpec2);
                    spawnPos = SpawnPositionFinder.Near(vector3Int, toSpawn, false, false, null);
                }
            IL_01B5:
                if (toSpawn != null)
                {
                    foreach (Instruction instruction in InstructionSets_Entity.Spawn(toSpawn, spawnPos, null))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                    Actor spawnedActor = toSpawn as Actor;
                    if (spawnedActor != null && this.addConditions != null)
                    {
                        foreach (Condition condition in this.addConditions)
                        {
                            Condition condition2 = condition.Clone();
                            foreach (Instruction instruction2 in InstructionSets_Misc.AddCondition(condition2, spawnedActor.Conditions, false, false))
                            {
                                yield return instruction2;
                            }
                            enumerator2 = null;
                        }
                        List<Condition>.Enumerator enumerator3 = default(List<Condition>.Enumerator);
                    }
                    foreach (Instruction instruction3 in this.ExtraEffectsAtSpawnedPos(spawnPos))
                    {
                        yield return instruction3;
                    }
                    enumerator2 = null;
                    spawnedActor = null;
                }
                toSpawn = null;
                spawnPos = default(Vector3Int);
            }
            else
            {
                using (IEnumerator<Vector3Int> enumerator4 = this.GetAoEPositionsForSpawn(user, vector3Int, usable).GetEnumerator())
                {
                    while (enumerator4.MoveNext())
                    {
                        UseEffect_Spawn.<> c__DisplayClass50_0 CS$<> 8__locals1 = new UseEffect_Spawn.<> c__DisplayClass50_0();
                        CS$<> 8__locals1.pos = enumerator4.Current;
                        Entity toSpawn = null;
                        EntitySpec entitySpec3;
                        if (this.entitySpec != null)
                        {
                            toSpawn = this.MakeEntity(this.entitySpec);
                        }
                        else if (this.entitySpecs != null && this.entitySpecs.Where<EntitySpec>((EntitySpec x) => SpawnPositionFinder.CanSpawnAt(x, CS$<> 8__locals1.pos, null, false, null)).TryGetRandomElement<EntitySpec>(out entitySpec3))
                        {
                            toSpawn = this.MakeEntity(entitySpec3);
                        }
                        foreach (Instruction instruction4 in InstructionSets_Entity.Spawn(toSpawn, CS$<> 8__locals1.pos, null))
                        {
                            yield return instruction4;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                        Actor spawnedActor = toSpawn as Actor;
                        if (spawnedActor != null && this.addConditions != null)
                        {
                            foreach (Condition condition3 in this.addConditions)
                            {
                                Condition condition4 = condition3.Clone();
                                foreach (Instruction instruction5 in InstructionSets_Misc.AddCondition(condition4, spawnedActor.Conditions, false, false))
                                {
                                    yield return instruction5;
                                }
                                enumerator2 = null;
                            }
                            List<Condition>.Enumerator enumerator3 = default(List<Condition>.Enumerator);
                        }
                        foreach (Instruction instruction6 in this.ExtraEffectsAtSpawnedPos(CS$<> 8__locals1.pos))
                        {
                            yield return instruction6;
                        }
                        enumerator2 = null;
                        toSpawn = null;
                        spawnedActor = null;
                        CS$<> 8__locals1 = null;
                    }
                }
                IEnumerator<Vector3Int> enumerator4 = null;
            }
            yield break;
            yield break;
        }

        private IEnumerable<Vector3Int> GetAoEPositionsForSpawn(Actor user, Vector3Int root, IUsable usable)
        {
            Vector3Int gravity = ((user != null) ? user.Gravity : Vector3Int.down);
            UseEffect_Spawn.tmpAoEArea.Clear();
            AoEUtility.GetAoEArea(root, this, usable, UseEffect_Spawn.tmpAoEArea, null);
            foreach (Vector3Int vector3Int in UseEffect_Spawn.tmpAoEArea)
            {
                if (!this.requireFloor || Get.CellsInfo.IsFloorUnderNoActors(vector3Int, gravity))
                {
                    if (this.entitySpec != null)
                    {
                        if (!SpawnPositionFinder.CanSpawnAt(this.entitySpec, vector3Int, null, false, null))
                        {
                            continue;
                        }
                    }
                    else if (this.entitySpecs != null)
                    {
                        bool flag = false;
                        for (int i = 0; i < this.entitySpecs.Count; i++)
                        {
                            if (SpawnPositionFinder.CanSpawnAt(this.entitySpecs[i], vector3Int, null, false, null))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            continue;
                        }
                    }
                    yield return vector3Int;
                }
            }
            List<Vector3Int>.Enumerator enumerator = default(List<Vector3Int>.Enumerator);
            yield break;
            yield break;
        }

        private Entity MakeEntity(EntitySpec specToUse)
        {
            return Maker.Make(specToUse, delegate (Entity x)
            {
                if (this.setRampUp)
                {
                    Actor actor = x as Actor;
                    if (actor != null)
                    {
                        actor.RampUp = RampUpUtility.GenerateRandomRampUpFor(actor, false);
                        DifficultyUtility.AddConditionsForDifficulty(actor);
                        actor.CalculateInitialHPManaAndStamina();
                        return;
                    }
                    Item item = x as Item;
                    if (item != null)
                    {
                        item.RampUp = RampUpUtility.GenerateRandomRampUpFor(item, false);
                    }
                }
            }, false, false, true);
        }

        private IEnumerable<Instruction> ExtraEffectsAtSpawnedPos(Vector3Int pos)
        {
            if (this.visualEffectAtSpawnedPos != null)
            {
                yield return new Instruction_VisualEffect(this.visualEffectAtSpawnedPos, pos);
            }
            if (this.soundAtSpawnedPos != null)
            {
                yield return new Instruction_Sound(this.soundAtSpawnedPos, new Vector3?(pos), 1f, 1f);
            }
            yield break;
        }

        [Saved]
        private EntitySpec entitySpec;

        [Saved]
        private List<EntitySpec> entitySpecs;

        [Saved]
        private bool requireFloor;

        [Saved]
        private bool neverUseNearMode;

        [Saved]
        private bool setRampUp;

        [Saved]
        private bool atSource;

        [Saved]
        private VisualEffectSpec visualEffectAtSpawnedPos;

        [Saved]
        private SoundSpec soundAtSpawnedPos;

        [Saved(Default.Null, true)]
        private List<Condition> addConditions;

        [Saved]
        private bool maxOneFollower;

        private string entitySpecsLabelCached;

        private static List<Vector3Int> tmpAoEArea = new List<Vector3Int>();
    }
}