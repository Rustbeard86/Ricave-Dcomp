using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class StructureGOC : EntityGOC
    {
        public Structure Structure
        {
            get
            {
                return (Structure)base.Entity;
            }
        }

        public int BevelsMask
        {
            get
            {
                return this.bevelsMask;
            }
        }

        public List<bool> Bevels
        {
            get
            {
                return this.bevels;
            }
        }

        public override void OnEntitySpawned()
        {
            base.OnEntitySpawned();
            if (!(base.Entity is Structure))
            {
                Log.Error("StructureGOC spawned with a non-Structure entity.", false);
            }
        }

        public override bool UpdateAppearance()
        {
            StructureGOC.<> c__DisplayClass18_0 CS$<> 8__locals1;
            CS$<> 8__locals1.<> 4__this = this;
            bool flag = base.UpdateAppearance();
            flag |= this.UpdateFloorAltMaterial();
            flag |= this.UpdateWallAltMaterial();
            CS$<> 8__locals1.useBevels = base.gameObject.CompareTag("HasBevels") || base.gameObject.CompareTag("HasBevelsSide");
            if (CS$<> 8__locals1.useBevels)
			{
                if (!this.upDownBevelsMeshRendererQueried)
                {
                    foreach (object obj in base.transform)
                    {
                        Transform transform = (Transform)obj;
                        if (transform.CompareTag("HasBevelsUpDown"))
                        {
                            MeshFilter component = transform.GetComponent<MeshFilter>();
                            if (PrimitiveMeshes.IsCubeUpDown(component.sharedMesh))
                            {
                                this.upDownBevelsMeshFilter = component;
                                break;
                            }
                        }
                    }
                    this.upDownBevelsMeshRendererQueried = true;
                }
                if (this.bevels == null)
                {
                    this.bevels = new List<bool>(12);
                }
                else
                {
                    this.bevels.Clear();
                }
                StructureGOC.CalculateBevels(base.Entity.Position, this.bevels);
                int num = CubesWithBevels.BevelsListToMask(this.bevels);
                if (this.bevelsMask != num)
                {
                    this.bevelsMask = num;
                    flag = true;
                    if (this.mainMeshFilter == null)
                    {
                        this.mainMeshFilter = base.GetComponent<MeshFilter>();
                    }
                    if (base.gameObject.CompareTag("HasBevelsSide"))
                    {
                        Mesh mesh;
                        this.mainMeshFilter.mesh = CubesWithBevels.GetSplit(this.bevels, true, true, true, true, true, true, out mesh);
                        if (this.upDownBevelsMeshFilter != null)
                        {
                            this.upDownBevelsMeshFilter.mesh = mesh;
                        }
                    }
                    else
                    {
                        this.mainMeshFilter.mesh = CubesWithBevels.Get(this.bevels, true, true, true, true, true, true);
                    }
                    if (this.beveledMeshBoxCollider == null)
                    {
                        this.beveledMeshBoxCollider = base.GetComponent<BoxCollider>();
                    }
                    if (this.beveledMeshMeshCollider == null)
                    {
                        this.beveledMeshMeshCollider = base.GetComponent<MeshCollider>();
                    }
                    if (this.bevelsMask == 0)
                    {
                        this.beveledMeshBoxCollider.size = Vector3.one;
                        this.beveledMeshMeshCollider.sharedMesh = null;
                    }
                    else
                    {
                        this.beveledMeshBoxCollider.size = Vector3.zero;
                        this.beveledMeshMeshCollider.sharedMesh = CubesWithBevels.Get(this.bevels, true, true, true, true, true, true);
                    }
                }
            }
            if ((this.Structure.Spec.Structure.HideAdjacentFacesSameSpec || this.Structure.Spec.Structure.HideAdjacentFacesUnfoggedOrNotFilled) | CS$<> 8__locals1.useBevels)
			{
                Quaternion rotation = base.Entity.Rotation;
                foreach (object obj2 in base.transform)
                {
                    StructureGOC.<> c__DisplayClass18_1 CS$<> 8__locals2;
                    CS$<> 8__locals2.child = (Transform)obj2;
                    MeshFilter meshFilter;
                    if (CS$<> 8__locals2.child.TryGetComponent<MeshFilter>(out meshFilter) && meshFilter.sharedMesh.vertexCount == 4)
					{
                        Vector3 vector = rotation * CS$<> 8__locals2.child.localPosition;
                        if (vector.x < -0.1f)
                        {
                            Vector3Int vector3Int = base.Entity.Position + Vector3Int.left;
                            flag |= StructureGOC.< UpdateAppearance > g__SetFaceActive | 18_1(!this.< UpdateAppearance > g__AnythingHidesFaceAt | 18_0(vector3Int, ref CS$<> 8__locals1), ref CS$<> 8__locals2);
                        }
                        else if (vector.x > 0.1f)
                        {
                            Vector3Int vector3Int2 = base.Entity.Position + Vector3Int.right;
                            flag |= StructureGOC.< UpdateAppearance > g__SetFaceActive | 18_1(!this.< UpdateAppearance > g__AnythingHidesFaceAt | 18_0(vector3Int2, ref CS$<> 8__locals1), ref CS$<> 8__locals2);
                        }
                        else if (vector.z < -0.1f)
                        {
                            Vector3Int vector3Int3 = base.Entity.Position + Vector3IntUtility.Back;
                            flag |= StructureGOC.< UpdateAppearance > g__SetFaceActive | 18_1(!this.< UpdateAppearance > g__AnythingHidesFaceAt | 18_0(vector3Int3, ref CS$<> 8__locals1), ref CS$<> 8__locals2);
                        }
                        else if (vector.z > 0.1f)
                        {
                            Vector3Int vector3Int4 = base.Entity.Position + Vector3IntUtility.Forward;
                            flag |= StructureGOC.< UpdateAppearance > g__SetFaceActive | 18_1(!this.< UpdateAppearance > g__AnythingHidesFaceAt | 18_0(vector3Int4, ref CS$<> 8__locals1), ref CS$<> 8__locals2);
                        }
                        else if (vector.y < -0.1f)
                        {
                            Vector3Int vector3Int5 = base.Entity.Position + Vector3Int.down;
                            flag |= StructureGOC.< UpdateAppearance > g__SetFaceActive | 18_1(!this.< UpdateAppearance > g__AnythingHidesFaceAt | 18_0(vector3Int5, ref CS$<> 8__locals1), ref CS$<> 8__locals2);
                        }
                        else if (vector.y > 0.1f)
                        {
                            Vector3Int vector3Int6 = base.Entity.Position + Vector3Int.up;
                            flag |= StructureGOC.< UpdateAppearance > g__SetFaceActive | 18_1(!this.< UpdateAppearance > g__AnythingHidesFaceAt | 18_0(vector3Int6, ref CS$<> 8__locals1), ref CS$<> 8__locals2);
                        }
                    }
                }
            }
            return flag;
        }

        public static void CalculateBevels(Vector3Int at, List<bool> outBevels)
        {
            outBevels.Clear();
            outBevels.Add(false);
            outBevels.Add(false);
            outBevels.Add(false);
            outBevels.Add(false);
            outBevels.Add(StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Back) && StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Back + Vector3IntUtility.Left) && StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Left));
            outBevels.Add(StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Left) && StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Left + Vector3IntUtility.Forward) && StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Forward));
            outBevels.Add(StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Forward) && StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Forward + Vector3IntUtility.Right) && StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Right));
            outBevels.Add(StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Right) && StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Right + Vector3IntUtility.Back) && StructureGOC.< CalculateBevels > g__IsNotFilledAt | 19_0(at + Vector3IntUtility.Back));
            outBevels.Add(false);
            outBevels.Add(false);
            outBevels.Add(false);
            outBevels.Add(false);
        }

        private bool UpdateFloorAltMaterial()
        {
            if (StructureGOC.FloorMaterial == null)
            {
                StructureGOC.FloorMaterial = Resources.Load<Material>("Materials/Structures/Floor");
            }
            if (StructureGOC.FloorAltMaterial == null)
            {
                StructureGOC.FloorAltMaterial = Resources.Load<Material>("Materials/Structures/FloorAlt");
            }
            bool flag = false;
            if (this.Structure.Spec.Structure.IsFilled)
            {
                bool flag2 = (this.Structure.Position.x + this.Structure.Position.z) % 2 == 0;
                foreach (MeshRenderer meshRenderer in base.MeshRenderers)
                {
                    if (flag2)
                    {
                        if (meshRenderer.sharedMaterial == StructureGOC.FloorMaterial)
                        {
                            meshRenderer.sharedMaterial = StructureGOC.FloorAltMaterial;
                            flag = true;
                        }
                    }
                    else if (meshRenderer.sharedMaterial == StructureGOC.FloorAltMaterial)
                    {
                        meshRenderer.sharedMaterial = StructureGOC.FloorMaterial;
                        flag = true;
                    }
                }
            }
            return flag;
        }

        private bool UpdateWallAltMaterial()
        {
            if (StructureGOC.WallMaterial == null)
            {
                StructureGOC.WallMaterial = Resources.Load<Material>("Materials/Structures/Wall");
            }
            if (StructureGOC.WallAltMaterial == null)
            {
                StructureGOC.WallAltMaterial = Resources.Load<Material>("Materials/Structures/WallAlt");
            }
            bool flag = false;
            if (this.Structure.Spec.Structure.IsFilled)
            {
                Vector3Int vector3Int = this.Structure.Position + Vector3IntUtility.Left;
                Vector3Int vector3Int2 = this.Structure.Position + Vector3IntUtility.Right;
                bool flag2 = (!vector3Int.InBounds() || Get.CellsInfo.IsFilled(vector3Int)) && (!vector3Int2.InBounds() || Get.CellsInfo.IsFilled(vector3Int2));
                foreach (MeshRenderer meshRenderer in base.MeshRenderers)
                {
                    if (flag2)
                    {
                        if (meshRenderer.sharedMaterial == StructureGOC.WallMaterial)
                        {
                            meshRenderer.sharedMaterial = StructureGOC.WallAltMaterial;
                            flag = true;
                        }
                    }
                    else if (meshRenderer.sharedMaterial == StructureGOC.WallAltMaterial)
                    {
                        meshRenderer.sharedMaterial = StructureGOC.WallMaterial;
                        flag = true;
                    }
                }
            }
            return flag;
        }

        [CompilerGenerated]
        private bool <UpdateAppearance>g__AnythingHidesFaceAt|18_0(Vector3Int pos, ref StructureGOC.<>c__DisplayClass18_0 A_2)
		{
			return pos.InBounds() && ((A_2.useBevels && this.bevelsMask == 0) || (this.Structure.Spec.Structure.HideAdjacentFacesSameSpec && Get.World.AnyEntityOfSpecAt(pos, base.Entity.Spec)) || ((this.Structure.Spec.Structure.HideAdjacentFacesUnfoggedOrNotFilled | A_2.useBevels) && (!Get.FogOfWar.IsFogged(pos) || !Get.CellsInfo.AnyFilledCantSeeThroughAt(pos))));
		}

    [CompilerGenerated]
    internal static bool <UpdateAppearance>g__SetFaceActive|18_1(bool val, ref StructureGOC.<>c__DisplayClass18_1 A_1)
		{
			if (A_1.child.gameObject.activeSelf == val)
			{
				return false;
			}
A_1.child.gameObject.SetActive(val);
return true;
		}

		[CompilerGenerated]
internal static bool < CalculateBevels > g__IsNotFilledAt | 19_0(Vector3Int c)

        {
    return c.InBounds() && !Get.CellsInfo.IsFilled(c) && !Get.CellsInfo.AnyBlocksBevels(c);
}

private int bevelsMask;

private List<bool> bevels;

private MeshFilter mainMeshFilter;

private MeshFilter upDownBevelsMeshFilter;

private BoxCollider beveledMeshBoxCollider;

private MeshCollider beveledMeshMeshCollider;

private bool upDownBevelsMeshRendererQueried;

private static Material FloorMaterial;

private static Material FloorAltMaterial;

private static Material WallMaterial;

private static Material WallAltMaterial;
	}
}