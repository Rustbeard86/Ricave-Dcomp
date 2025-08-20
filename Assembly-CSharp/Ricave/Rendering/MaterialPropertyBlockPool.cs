using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class MaterialPropertyBlockPool
    {
        public static MaterialPropertyBlock Get()
        {
            if (MaterialPropertyBlockPool.pool.Count != 0)
            {
                MaterialPropertyBlock materialPropertyBlock = MaterialPropertyBlockPool.pool[MaterialPropertyBlockPool.pool.Count - 1];
                MaterialPropertyBlockPool.pool.RemoveAt(MaterialPropertyBlockPool.pool.Count - 1);
                return materialPropertyBlock;
            }
            return new MaterialPropertyBlock();
        }

        public static void Return(MaterialPropertyBlock propertyBlock)
        {
            if (propertyBlock == null)
            {
                return;
            }
            if (MaterialPropertyBlockPool.pool.Count < 100)
            {
                propertyBlock.Clear();
                MaterialPropertyBlockPool.pool.Add(propertyBlock);
            }
        }

        private static List<MaterialPropertyBlock> pool = new List<MaterialPropertyBlock>(30);

        private const int MaxCapacity = 100;
    }
}