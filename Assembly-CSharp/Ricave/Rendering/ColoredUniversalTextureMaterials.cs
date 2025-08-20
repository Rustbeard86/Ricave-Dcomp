using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class ColoredUniversalTextureMaterials
    {
        public static Material Get(Color color)
        {
            Color32 color2 = color;
            Material material;
            if (ColoredUniversalTextureMaterials.materials.TryGetValue(color2, out material))
            {
                return material;
            }
            Material material2 = Object.Instantiate<Material>(ColoredUniversalTextureMaterials.BaseMaterial);
            material2.color = color2;
            ColoredUniversalTextureMaterials.materials.Add(color2, material2);
            return material2;
        }

        private static Dictionary<Color32, Material> materials = new Dictionary<Color32, Material>();

        private static readonly Material BaseMaterial = Assets.Get<Material>("Materials/Misc/ColoredUniversalTextureBase");
    }
}