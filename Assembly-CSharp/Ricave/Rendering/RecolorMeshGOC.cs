using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Rendering
{
    public class RecolorMeshGOC : MonoBehaviour
    {
        private void Start()
        {
            MeshFilter component = base.GetComponent<MeshFilter>();
            if (component == null)
            {
                return;
            }
            component.mesh = this.GetRecoloredMesh(component.sharedMesh);
        }

        private Mesh GetRecoloredMesh(Mesh mesh)
        {
            Color color;
            Color color2;
            Color color3;
            this.GetWantedColors(out color, out color2, out color3);
            Mesh mesh2;
            if (RecolorMeshGOC.cachedRecoloredMeshes.TryGetValue(new ValueTuple<Mesh, ValueTuple<Color, Color, Color>>(mesh, new ValueTuple<Color, Color, Color>(color, color2, color3)), out mesh2))
            {
                return mesh2;
            }
            mesh2 = Object.Instantiate<Mesh>(mesh);
            mesh2.name = "RecoloredMesh";
            RecolorMeshGOC.tmpColors.Clear();
            mesh2.GetColors(RecolorMeshGOC.tmpColors);
            if (RecolorMeshGOC.tmpColors.Count == 0)
            {
                int i = 0;
                int vertexCount = mesh.vertexCount;
                while (i < vertexCount)
                {
                    RecolorMeshGOC.tmpColors.Add(color);
                    i++;
                }
            }
            else
            {
                int j = 0;
                int count = RecolorMeshGOC.tmpColors.Count;
                while (j < count)
                {
                    Color color4 = RecolorMeshGOC.tmpColors[j];
                    if (color4 == Color.red)
                    {
                        RecolorMeshGOC.tmpColors[j] = color;
                    }
                    else if (color4 == Color.green)
                    {
                        RecolorMeshGOC.tmpColors[j] = color2;
                    }
                    else if (color4 == Color.blue)
                    {
                        RecolorMeshGOC.tmpColors[j] = color3;
                    }
                    j++;
                }
            }
            mesh2.SetColors(RecolorMeshGOC.tmpColors);
            RecolorMeshGOC.cachedRecoloredMeshes.Add(new ValueTuple<Mesh, ValueTuple<Color, Color, Color>>(mesh, new ValueTuple<Color, Color, Color>(color, color2, color3)), mesh2);
            return mesh2;
        }

        private void GetWantedColors(out Color c1, out Color c2, out Color c3)
        {
            if (this.materials.Count > 0)
            {
                c1 = this.materials[0].color;
            }
            else
            {
                c1 = Color.white;
            }
            if (this.materials.Count > 1)
            {
                c2 = this.materials[1].color;
            }
            else
            {
                c2 = Color.white;
            }
            if (this.materials.Count > 2)
            {
                c3 = this.materials[2].color;
                return;
            }
            c3 = Color.white;
        }

        [SerializeField]
        private List<Material> materials = new List<Material>();

        private static Dictionary<ValueTuple<Mesh, ValueTuple<Color, Color, Color>>, Mesh> cachedRecoloredMeshes = new Dictionary<ValueTuple<Mesh, ValueTuple<Color, Color, Color>>, Mesh>();

        private static List<Color> tmpColors = new List<Color>(50);
    }
}