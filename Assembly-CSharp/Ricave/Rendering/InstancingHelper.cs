using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ricave.Rendering
{
    public static class InstancingHelper
    {
        public static void DrawInstanced(Mesh mesh, Material material, List<Vector3Int> positions, List<Matrix4x4[]> matrixArrays, Quaternion rotation, Vector3 scale)
        {
            InstancingHelper.<> c__DisplayClass1_0 CS$<> 8__locals1;
            CS$<> 8__locals1.mesh = mesh;
            CS$<> 8__locals1.material = material;
            if (positions.Count == 0)
            {
                return;
            }
            CS$<> 8__locals1.instancingSupported = Sys.InstancingSupported;
            int num = 0;
            int num2 = 0;
            if (matrixArrays.Count == 0)
            {
                matrixArrays.Add(new Matrix4x4[8]);
            }
            for (int i = 0; i < positions.Count; i++)
            {
                Matrix4x4[] array = matrixArrays[num];
                InstancingHelper.InsertInArray(ref array, num2, Matrix4x4.TRS(positions[i], rotation, scale));
                matrixArrays[num] = array;
                num2++;
                if (num2 == 1000)
                {
                    InstancingHelper.< DrawInstanced > g__DrawMeshes | 1_0(array, 1000, ref CS$<> 8__locals1);
                    num++;
                    num2 = 0;
                    if (num >= matrixArrays.Count)
                    {
                        matrixArrays.Add(new Matrix4x4[8]);
                    }
                }
            }
            if (num2 != 0)
            {
                InstancingHelper.< DrawInstanced > g__DrawMeshes | 1_0(matrixArrays[num], num2, ref CS$<> 8__locals1);
            }
        }

        private static void InsertInArray(ref Matrix4x4[] array, int index, Matrix4x4 element)
        {
            while (index >= array.Length)
            {
                Array.Resize<Matrix4x4>(ref array, Math.Min((int)((float)array.Length * 1.5f) + 1, 1000));
            }
            array[index] = element;
        }

        [CompilerGenerated]
        internal static void <DrawInstanced>g__DrawMeshes|1_0(Matrix4x4[] array, int count, ref InstancingHelper.<>c__DisplayClass1_0 A_2)
		{
			if (A_2.instancingSupported)
			{
				Graphics.DrawMeshInstanced(A_2.mesh, 0, A_2.material, array, count, null, ShadowCastingMode.Off, false);
				return;
			}
			for (int i = 0; i<count; i++)
			{
				Graphics.DrawMesh(A_2.mesh, array[i], A_2.material, 0, null, 0, null, ShadowCastingMode.Off, false);
			}
		}

		private const int LimitPerArray = 1000;
	}
}