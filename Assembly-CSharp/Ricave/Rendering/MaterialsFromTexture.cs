using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class MaterialsFromTexture
    {
        public static Material Get(Texture texture)
        {
            Material material;
            if (MaterialsFromTexture.materials.TryGetValue(texture, out material))
            {
                return material;
            }
            Material material2 = Object.Instantiate<Material>(MaterialsFromTexture.BaseMaterial);
            material2.mainTexture = texture;
            MaterialsFromTexture.materials.Add(texture, material2);
            return material2;
        }

        private static Dictionary<Texture, Material> materials = new Dictionary<Texture, Material>();

        private static readonly Material BaseMaterial = Assets.Get<Material>("Materials/Misc/MaterialFromTextureBase");

        public static readonly Material InvisibleMaterial = MaterialsFromTexture.Get(Assets.Get<Texture>("Textures/UI/Transparent"));
    }
}