using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class Structure : Entity, IUsable, ITipSubject
    {
        public UseEffects UseEffects
        {
            get
            {
                return this.useEffects;
            }
        }

        public StructureGOC StructureGOC
        {
            get
            {
                return (StructureGOC)base.EntityGOC;
            }
        }

        public List<Entity> InnerEntities
        {
            get
            {
                return this.innerEntities;
            }
        }

        public bool SpawnedWithSpecificRotationUnsaved
        {
            get
            {
                return this.spawnedWithSpecificRotationUnsaved;
            }
        }

        UseEffects IUsable.MissUseEffects
        {
            get
            {
                return null;
            }
        }

        public string UseLabel_Self
        {
            get
            {
                return UsableUtility.CheckAppendDotsToUseLabel("UseLabel_Self".Translate(), this);
            }
        }

        public string UseLabel_Other
        {
            get
            {
                return "UseLabel_Other".Translate();
            }
        }

        public string UseDescriptionFormat_Self
        {
            get
            {
                return base.Spec.Structure.UseDescriptionFormatKey_Self.Translate();
            }
        }

        public string UseDescriptionFormat_Other
        {
            get
            {
                return base.Spec.Structure.UseDescriptionFormatKey_Other.Translate();
            }
        }

        public TargetFilter UseFilter
        {
            get
            {
                return base.Spec.Structure.UseFilter;
            }
        }

        TargetFilter IUsable.UseFilterAoE
        {
            get
            {
                return base.Spec.Structure.UseFilterAoE ?? base.Spec.Structure.UseFilter;
            }
        }

        public int UseRange
        {
            get
            {
                return base.Spec.Structure.UseRange;
            }
        }

        public int? LastUsedToRewindTimeSequence
        {
            get
            {
                return this.lastUsedToRewindTimeSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastUsedToRewindTimeSequence = value;
            }
        }

        public int? LastUseSequence
        {
            get
            {
                return this.lastUseSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastUseSequence = value;
            }
        }

        int? IUsable.CanRewindTimeEveryTurns
        {
            get
            {
                return null;
            }
        }

        public float SequencePerUseMultiplier
        {
            get
            {
                return base.Spec.Structure.SequencePerUseMultiplier;
            }
        }

        public int ManaCost
        {
            get
            {
                return base.Spec.Structure.ManaCost;
            }
        }

        public int StaminaCost
        {
            get
            {
                return base.Spec.Structure.StaminaCost;
            }
        }

        public int CooldownTurns
        {
            get
            {
                return base.Spec.Structure.CooldownTurns;
            }
        }

        float IUsable.MissChance
        {
            get
            {
                return 0f;
            }
        }

        float IUsable.CritChance
        {
            get
            {
                return 0f;
            }
        }

        public UsePrompt UsePrompt
        {
            get
            {
                if (Get.Skill_Lockpicking.IsUnlocked() && SkillUtility.IsLockpickable(this))
                {
                    return null;
                }
                return base.Spec.Structure.UsePrompt;
            }
        }

        public override string Label
        {
            get
            {
                return StringUtility.Concat(base.Label, LastUsedToRewindTimeUtility.LastUsedToRewindTimeRichString(this));
            }
        }

        public override string Description
        {
            get
            {
                if (base.Spec == Get.Entity_MemoryPiece1Status)
                {
                    if (Get.TotalLobbyItems.Any(Get.Entity_MemoryPiece1))
                    {
                        return "MemoryPiece1".Translate();
                    }
                }
                else if (base.Spec == Get.Entity_MemoryPiece2Status)
                {
                    if (Get.TotalLobbyItems.Any(Get.Entity_MemoryPiece2))
                    {
                        return "MemoryPiece2".Translate();
                    }
                }
                else if (base.Spec == Get.Entity_MemoryPiece3Status)
                {
                    if (Get.TotalLobbyItems.Any(Get.Entity_MemoryPiece3))
                    {
                        return "MemoryPiece3".Translate();
                    }
                }
                else if (base.Spec == Get.Entity_MemoryPiece4Status && Get.TotalLobbyItems.Any(Get.Entity_MemoryPiece4))
                {
                    return "MemoryPiece4".Translate();
                }
                return base.Description;
            }
        }

        protected Structure()
        {
        }

        public Structure(EntitySpec spec)
            : base(spec)
        {
            this.useEffects = new UseEffects(this);
            UseEffects defaultUseEffects = spec.Structure.DefaultUseEffects;
            if (defaultUseEffects != null)
            {
                this.useEffects.AddClonedFrom(defaultUseEffects);
            }
            List<EntitySpec> defaultLoot = spec.Structure.DefaultLoot;
            if (defaultLoot != null)
            {
                foreach (EntitySpec entitySpec in defaultLoot)
                {
                    this.innerEntities.Add(Maker.Make(entitySpec, null, false, false, true));
                }
            }
        }

        public Structure(string specID, int instanceID, int stableID, Vector3Int pos, Quaternion rot, Vector3 scale)
            : base(specID, instanceID, stableID, pos, rot, scale)
        {
            this.useEffects = new UseEffects(this);
        }

        public bool CanUse_ExtraInstanceSpecificChecks(Actor user, Vector3Int? assumeUserPos = null, bool assumeAnyUserPos = false, StringSlot outReason = null)
        {
            if (!base.Spawned)
            {
                return false;
            }
            if (assumeUserPos == null && !assumeAnyUserPos && !user.Spawned)
            {
                return false;
            }
            if (!assumeAnyUserPos)
            {
                Vector3Int vector3Int = assumeUserPos ?? user.Position;
                if (vector3Int.GetGridDistance(base.Position) > base.Spec.Structure.UseRange)
                {
                    if (outReason != null)
                    {
                        outReason.Set("GetCloser".Translate());
                    }
                    return false;
                }
                if (!user.Sees(base.Position, new Vector3Int?(vector3Int)) || !LineOfSight.IsLineOfFire(vector3Int, base.Position))
                {
                    if (outReason != null)
                    {
                        outReason.Set("NoLineOfSight".Translate());
                    }
                    return false;
                }
            }
            if (base.Spec.Structure.AllowUseOnlyIfNotSeenByHostiles && UsableUtility.SeenByAnyHostile(user))
            {
                if (outReason != null)
                {
                    outReason.Set("SeenByHostiles".Translate());
                }
                return false;
            }
            if (base.Spec.Structure.AllowUseOnlyIfNoBoss && (!user.IsMainActor || !Get.Skill_Lockpicking.IsUnlocked()))
            {
                bool flag = false;
                List<Actor> actors = Get.World.Actors;
                for (int i = 0; i < actors.Count; i++)
                {
                    if (actors[i].IsBoss)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    if (outReason != null)
                    {
                        outReason.Set("BossStillAlive".Translate());
                    }
                    return false;
                }
            }
            return true;
        }

        public override void OnSpawned(bool canAutoRotate)
        {
            base.OnSpawned(canAutoRotate);
            if (canAutoRotate)
            {
                this.spawnedWithSpecificRotationUnsaved = false;
                this.AutoRotate();
                return;
            }
            this.spawnedWithSpecificRotationUnsaved = true;
        }

        public void AutoRotate()
        {
            Quaternion? autoRotateRotation = this.GetAutoRotateRotation();
            if (autoRotateRotation != null)
            {
                base.Rotation = autoRotateRotation.Value;
            }
        }

        public Quaternion? GetAutoRotateRotation()
        {
            return Structure.GetEntitySpecAutoRotateRotation(base.Spec, base.Position);
        }

        public static Quaternion? GetEntitySpecAutoRotateRotation(EntitySpec spec, Vector3Int position)
        {
            Structure.<> c__DisplayClass62_0 CS$<> 8__locals1;
            CS$<> 8__locals1.position = position;
            CS$<> 8__locals1.spec = spec;
            if (CS$<> 8__locals1.spec.Structure.AutoRotateRandomly)
			{
                return new Quaternion?(Quaternion.Euler(0f, Rand.RangeSeeded(0f, 360f, Calc.CombineHashes<int, int, int>(CS$<> 8__locals1.position.GetHashCode(), CS$<> 8__locals1.spec.MyStableHash, 536580123)), 0f));
            }
            if (CS$<> 8__locals1.spec.Structure.AutoRotateRandomlySnap90)
			{
                Vector3Int position2 = CS$<> 8__locals1.position;
                return new Quaternion?(Quaternion.Euler(0f, (float)((position2.x + position2.y + position2.z) % 4) * 90f, 0f));
            }
            if (CS$<> 8__locals1.spec.Structure.IsDoor)
			{
                Vector3Int vector3Int = CS$<> 8__locals1.position + Vector3IntUtility.East;
                Vector3Int vector3Int2 = CS$<> 8__locals1.position + Vector3IntUtility.West;
                Vector3Int vector3Int3 = CS$<> 8__locals1.position + Vector3IntUtility.North;
                Vector3Int vector3Int4 = CS$<> 8__locals1.position + Vector3IntUtility.South;
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsPermanentWall | 62_4(vector3Int) && Structure.< GetEntitySpecAutoRotateRotation > g__IsPermanentWall | 62_4(vector3Int2))
                {
                    return new Quaternion?(QuaternionUtility.Right);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsPermanentWall | 62_4(vector3Int3) && Structure.< GetEntitySpecAutoRotateRotation > g__IsPermanentWall | 62_4(vector3Int4))
                {
                    return new Quaternion?(Quaternion.identity);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsWall | 62_5(vector3Int) && Structure.< GetEntitySpecAutoRotateRotation > g__IsWall | 62_5(vector3Int2))
                {
                    return new Quaternion?(QuaternionUtility.Right);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsWall | 62_5(vector3Int3) && Structure.< GetEntitySpecAutoRotateRotation > g__IsWall | 62_5(vector3Int4))
                {
                    return new Quaternion?(Quaternion.identity);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsPermanentWall | 62_4(vector3Int) || Structure.< GetEntitySpecAutoRotateRotation > g__IsPermanentWall | 62_4(vector3Int2))
                {
                    return new Quaternion?(QuaternionUtility.Right);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsPermanentWall | 62_4(vector3Int3) || Structure.< GetEntitySpecAutoRotateRotation > g__IsPermanentWall | 62_4(vector3Int4))
                {
                    return new Quaternion?(Quaternion.identity);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsWall | 62_5(vector3Int) || Structure.< GetEntitySpecAutoRotateRotation > g__IsWall | 62_5(vector3Int2))
                {
                    return new Quaternion?(QuaternionUtility.Right);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsWall | 62_5(vector3Int3) || Structure.< GetEntitySpecAutoRotateRotation > g__IsWall | 62_5(vector3Int4))
                {
                    return new Quaternion?(Quaternion.identity);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsImpassable | 62_6(vector3Int) && Structure.< GetEntitySpecAutoRotateRotation > g__IsImpassable | 62_6(vector3Int2))
                {
                    return new Quaternion?(QuaternionUtility.Right);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsImpassable | 62_6(vector3Int3) && Structure.< GetEntitySpecAutoRotateRotation > g__IsImpassable | 62_6(vector3Int4))
                {
                    return new Quaternion?(Quaternion.identity);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsImpassable | 62_6(vector3Int) || Structure.< GetEntitySpecAutoRotateRotation > g__IsImpassable | 62_6(vector3Int2))
                {
                    return new Quaternion?(QuaternionUtility.Right);
                }
                if (!Structure.< GetEntitySpecAutoRotateRotation > g__IsImpassable | 62_6(vector3Int3))
                {
                    Structure.< GetEntitySpecAutoRotateRotation > g__IsImpassable | 62_6(vector3Int4);
                }
                return new Quaternion?(Quaternion.identity);
            }

            else
            {
                if (CS$<> 8__locals1.spec.Structure.AutoRotateTowardsFree)
				{
                    for (int i = 0; i < 5; i++)
                    {
                        bool flag = i == 0;
                        bool flag2 = i <= 1;
                        bool flag3 = i == 2;
                        bool flag4 = i == 4;
                        if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_0(Vector3IntUtility.North, flag2, flag3, flag4, flag, ref CS$<> 8__locals1))
                        {
                            return new Quaternion?(Quaternion.identity);
                        }
                        if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_0(Vector3IntUtility.East, flag2, flag3, flag4, flag, ref CS$<> 8__locals1))
                        {
                            return new Quaternion?(QuaternionUtility.Right);
                        }
                        if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_0(Vector3IntUtility.South, flag2, flag3, flag4, flag, ref CS$<> 8__locals1))
                        {
                            return new Quaternion?(QuaternionUtility.Back);
                        }
                        if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_0(Vector3IntUtility.West, flag2, flag3, flag4, flag, ref CS$<> 8__locals1))
                        {
                            return new Quaternion?(QuaternionUtility.Left);
                        }
                    }
                    return new Quaternion?(Quaternion.identity);
                }
                if (CS$<> 8__locals1.spec.Structure.AutoRotateAwayFromWall || CS$<> 8__locals1.spec.Structure.IsLadder || CS$<> 8__locals1.spec.Structure.AutoRotateConnectToSameSpec)
				{
                    if (CS$<> 8__locals1.spec.Structure.IsLadder)
					{
                        Vector3Int vector3Int5 = CS$<> 8__locals1.position.Above();
                        if (vector3Int5.InBounds())
                        {
                            Entity firstEntityOfSpecAt = Get.World.GetFirstEntityOfSpecAt(vector3Int5, CS$<> 8__locals1.spec);
                            if (firstEntityOfSpecAt != null)
                            {
                                return new Quaternion?(firstEntityOfSpecAt.Rotation);
                            }
                        }
                        Vector3Int vector3Int6 = CS$<> 8__locals1.position.Below();
                        if (vector3Int6.InBounds())
                        {
                            Entity firstEntityOfSpecAt2 = Get.World.GetFirstEntityOfSpecAt(vector3Int6, CS$<> 8__locals1.spec);
                            if (firstEntityOfSpecAt2 != null)
                            {
                                return new Quaternion?(firstEntityOfSpecAt2.Rotation);
                            }
                        }
                    }
                    if (CS$<> 8__locals1.spec.Structure.AutoRotateConnectToSameSpec)
					{
                        for (int j = 0; j < Vector3IntUtility.DirectionsXZCardinal.Length; j++)
                        {
                            Vector3Int vector3Int7 = CS$<> 8__locals1.position + Vector3IntUtility.DirectionsXZCardinal[j];
                            if (vector3Int7.InBounds())
                            {
                                Entity firstEntityOfSpecAt3 = Get.World.GetFirstEntityOfSpecAt(vector3Int7, CS$<> 8__locals1.spec);
                                if (firstEntityOfSpecAt3 != null && ((vector3Int7.x != CS$<> 8__locals1.position.x && (firstEntityOfSpecAt3.DirectionCardinal == Vector3IntUtility.Left || firstEntityOfSpecAt3.DirectionCardinal == Vector3IntUtility.Right)) || (vector3Int7.z != CS$<> 8__locals1.position.z && (firstEntityOfSpecAt3.DirectionCardinal == Vector3IntUtility.Forward || firstEntityOfSpecAt3.DirectionCardinal == Vector3IntUtility.Back))))
								{
                            return new Quaternion?(firstEntityOfSpecAt3.Rotation);
                        }
                    }
                }
                Vector3Int vector3Int8 = CS$<> 8__locals1.position.Above();
                if (vector3Int8.InBounds())
                {
                    Entity firstEntityOfSpecAt4 = Get.World.GetFirstEntityOfSpecAt(vector3Int8, CS$<> 8__locals1.spec);
                    if (firstEntityOfSpecAt4 != null)
                    {
                        return new Quaternion?(firstEntityOfSpecAt4.Rotation);
                    }
                }
                Vector3Int vector3Int9 = CS$<> 8__locals1.position.Below();
                if (vector3Int9.InBounds())
                {
                    Entity firstEntityOfSpecAt5 = Get.World.GetFirstEntityOfSpecAt(vector3Int9, CS$<> 8__locals1.spec);
                    if (firstEntityOfSpecAt5 != null)
                    {
                        return new Quaternion?(firstEntityOfSpecAt5.Rotation);
                    }
                }
            }
            for (int k = 0; k < 3; k++)
            {
                bool flag5 = k == 0;
                bool flag6 = k == 1;
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_1(Vector3IntUtility.South, flag5, flag6, ref CS$<> 8__locals1))
                {
                    return new Quaternion?(Quaternion.identity);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_1(Vector3IntUtility.West, flag5, flag6, ref CS$<> 8__locals1))
                {
                    return new Quaternion?(QuaternionUtility.Right);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_1(Vector3IntUtility.North, flag5, flag6, ref CS$<> 8__locals1))
                {
                    return new Quaternion?(QuaternionUtility.Back);
                }
                if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_1(Vector3IntUtility.East, flag5, flag6, ref CS$<> 8__locals1))
                {
                    return new Quaternion?(QuaternionUtility.Left);
                }
            }
            return new Quaternion?(Quaternion.identity);
        }
				if (CS$<>8__locals1.spec.Structure.AutoRotateTowardsCorner)
				{
					for (int l = 0; l< 3; l++)
					{
						bool flag7 = l == 0;
        bool flag8 = l == 1;
						if (Structure.<GetEntitySpecAutoRotateRotation>g__IsGood|62_2(Vector3IntUtility.South, Vector3IntUtility.West, flag7, flag8, ref CS$<>8__locals1))
						{
							return new Quaternion? (Quaternion.identity);
						}
						if (Structure.<GetEntitySpecAutoRotateRotation>g__IsGood|62_2(Vector3IntUtility.West, Vector3IntUtility.North, flag7, flag8, ref CS$<>8__locals1))
						{
							return new Quaternion? (QuaternionUtility.Right);
						}
if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_2(Vector3IntUtility.North, Vector3IntUtility.East, flag7, flag8, ref CS$<> 8__locals1))
{
    return new Quaternion?(QuaternionUtility.Back);
}
if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_2(Vector3IntUtility.East, Vector3IntUtility.South, flag7, flag8, ref CS$<> 8__locals1))
{
    return new Quaternion?(QuaternionUtility.Left);
}
					}
					return new Quaternion?(Quaternion.identity);
				}
				if (CS$<> 8__locals1.spec.Structure.IsStairs)
				{
    for (int m = 0; m < 4; m++)
    {
        bool flag9 = m == 0 || m == 1;
        bool flag10 = m == 0;
        bool flag11 = m < 3;
        if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_3(Vector3IntUtility.North, flag9, flag10, flag11, ref CS$<> 8__locals1))
        {
            return new Quaternion?(Quaternion.identity);
        }
        if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_3(Vector3IntUtility.East, flag9, flag10, flag11, ref CS$<> 8__locals1))
        {
            return new Quaternion?(QuaternionUtility.Right);
        }
        if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_3(Vector3IntUtility.South, flag9, flag10, flag11, ref CS$<> 8__locals1))
        {
            return new Quaternion?(QuaternionUtility.Back);
        }
        if (Structure.< GetEntitySpecAutoRotateRotation > g__IsGood | 62_3(Vector3IntUtility.West, flag9, flag10, flag11, ref CS$<> 8__locals1))
        {
            return new Quaternion?(QuaternionUtility.Left);
        }
    }
    return new Quaternion?(Quaternion.identity);
}
return null;
			}
		}

		[CompilerGenerated]
internal static bool < GetEntitySpecAutoRotateRotation > g__IsPermanentWall | 62_4(Vector3Int at)

        {
    return !at.InBounds() || Get.CellsInfo.AnyPermanentFilledImpassableAt(at);
}

[CompilerGenerated]
internal static bool < GetEntitySpecAutoRotateRotation > g__IsWall | 62_5(Vector3Int at)

        {
    return !at.InBounds() || Get.CellsInfo.AnyFilledImpassableAt(at);
}

[CompilerGenerated]
internal static bool < GetEntitySpecAutoRotateRotation > g__IsImpassable | 62_6(Vector3Int at)

        {
    return !at.InBounds() || !Get.CellsInfo.CanPassThroughNoActors(at);
}

[CompilerGenerated]
internal static bool < GetEntitySpecAutoRotateRotation > g__IsGood | 62_0(Vector3Int dir, bool requireFaceAwayFromPermanentWall, bool requireFaceAwayFromWall, bool assumeNonFilledAreFree, bool requireFloorUnder, ref Structure.<> c__DisplayClass62_0 A_5)

        {
    Vector3Int vector3Int = A_5.position + dir;
    if (!vector3Int.InBounds())
    {
        return false;
    }
    if (A_5.spec.Structure.AutoRotateTowardsFree_inRoomBounds && !vector3Int.InRoomsBounds())
    {
        return false;
    }
    if (!Get.CellsInfo.CanPassThroughNoActors(vector3Int) && (!assumeNonFilledAreFree || Get.CellsInfo.IsFilled(vector3Int)))
    {
        return false;
    }
    if (requireFloorUnder && !Get.CellsInfo.IsFloorUnderNoActors(vector3Int))
    {
        return false;
    }
    if (requireFaceAwayFromPermanentWall)
    {
        Vector3Int vector3Int2 = A_5.position - dir;
        if (!vector3Int2.InBounds() || !Get.CellsInfo.AnyPermanentFilledImpassableAt(vector3Int2))
        {
            return false;
        }
    }
    if (requireFaceAwayFromWall)
    {
        Vector3Int vector3Int3 = A_5.position - dir;
        if (!vector3Int3.InBounds() || !Get.CellsInfo.AnyFilledImpassableAt(vector3Int3))
        {
            return false;
        }
    }
    return true;
}

[CompilerGenerated]
internal static bool < GetEntitySpecAutoRotateRotation > g__IsGood | 62_1(Vector3Int dir, bool requirePermanentWall, bool requireWall, ref Structure.<> c__DisplayClass62_0 A_3)

        {
    Vector3Int vector3Int = A_3.position + dir;
    return vector3Int.InBounds() && !Get.CellsInfo.CanPassThroughNoActors(vector3Int) && (!requirePermanentWall || Get.CellsInfo.AnyPermanentFilledImpassableAt(vector3Int)) && (!requireWall || Get.CellsInfo.AnyFilledImpassableAt(vector3Int));
}

[CompilerGenerated]
internal static bool < GetEntitySpecAutoRotateRotation > g__IsGood | 62_2(Vector3Int dir1, Vector3Int dir2, bool requirePermanentWall, bool requireWall, ref Structure.<> c__DisplayClass62_0 A_4)

        {
    Vector3Int vector3Int = A_4.position + dir1;
    Vector3Int vector3Int2 = A_4.position + dir2;
    return vector3Int.InBounds() && vector3Int2.InBounds() && !Get.CellsInfo.CanPassThroughNoActors(vector3Int) && !Get.CellsInfo.CanPassThroughNoActors(vector3Int2) && (!requirePermanentWall || (Get.CellsInfo.AnyPermanentFilledImpassableAt(vector3Int) && Get.CellsInfo.AnyPermanentFilledImpassableAt(vector3Int2))) && (!requireWall || (Get.CellsInfo.AnyFilledImpassableAt(vector3Int) && Get.CellsInfo.AnyFilledImpassableAt(vector3Int2)));
}

[CompilerGenerated]
internal static bool < GetEntitySpecAutoRotateRotation > g__IsGood | 62_3(Vector3Int dir, bool requireWall, bool requireFreeEntrance, bool requireFloorUnderDest, ref Structure.<> c__DisplayClass62_0 A_4)

        {
    Vector3Int vector3Int = A_4.position.Above() + dir;
    if (!vector3Int.InBounds() || !Get.CellsInfo.CanPassThroughNoActors(vector3Int))
    {
        return false;
    }
    Vector3Int vector3Int2 = A_4.position - dir;
    if (requireFloorUnderDest)
    {
        if (!Get.CellsInfo.IsFloorUnderNoActors(vector3Int))
        {
            return false;
        }
    }
    else if (vector3Int2.InBounds() && !Get.CellsInfo.IsFloorUnderNoActors(vector3Int2))
    {
        return false;
    }
    return (!requireWall || Get.CellsInfo.AnyFilledImpassableAt(A_4.position + dir)) && (!requireFreeEntrance || (vector3Int2.InBounds() && Get.CellsInfo.CanPassThroughNoActors(vector3Int2)));
}

[Saved]
private UseEffects useEffects;

[Saved(Default.New, true)]
private List<Entity> innerEntities = new List<Entity>();

[Saved]
private int? lastUsedToRewindTimeSequence;

[Saved]
private int? lastUseSequence;

private bool spawnedWithSpecificRotationUnsaved;
	}
}