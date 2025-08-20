using System;
using System.IO;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class CubemapMaker
    {
        public static void MakeCubemap(Func<Color, Color> colorProcessor = null)
        {
            CubemapMaker.wantsCubemap = true;
            CubemapMaker.colorProcessor = colorProcessor;
        }

        public static void LateUpdate()
        {
            if (!CubemapMaker.wantsCubemap)
            {
                return;
            }
            CubemapMaker.wantsCubemap = false;
            Cubemap cubemap = new Cubemap(1024, TextureFormat.RGB24, false);
            try
            {
                CubemapMaker.RenderToCubemap(cubemap);
                byte[] array = CubemapMaker.CubemapToBytes(cubemap);
                File.WriteAllBytes("Cubemap.png", array);
            }
            finally
            {
                Object.Destroy(cubemap);
            }
        }

        private static void RenderToCubemap(Cubemap cubemap)
        {
            GameObject gameObject = null;
            try
            {
                Camera main = Camera.main;
                gameObject = new GameObject("CubemapCamera");
                Camera camera = gameObject.AddComponent<Camera>();
                gameObject.transform.position = main.transform.position;
                camera.nearClipPlane = main.nearClipPlane;
                camera.farClipPlane = main.farClipPlane;
                camera.clearFlags = main.clearFlags;
                camera.backgroundColor = main.backgroundColor;
                camera.fieldOfView = 70f;
                camera.RenderToCubemap(cubemap);
            }
            finally
            {
                Object.Destroy(gameObject);
            }
        }

        private static byte[] CubemapToBytes(Cubemap cubemap)
        {
            Texture2D texture2D = null;
            byte[] array2;
            try
            {
                texture2D = new Texture2D(6144, 1024, TextureFormat.RGBA32, false);
                Color[] array = new Color[6291456];
                CubemapMaker.CopyFaceToCubemapTexture(cubemap.GetPixels(CubemapFace.PositiveX), array, 0);
                CubemapMaker.CopyFaceToCubemapTexture(cubemap.GetPixels(CubemapFace.NegativeX), array, 1024);
                CubemapMaker.CopyFaceToCubemapTexture(cubemap.GetPixels(CubemapFace.PositiveY), array, 2048);
                CubemapMaker.CopyFaceToCubemapTexture(cubemap.GetPixels(CubemapFace.NegativeY), array, 3072);
                CubemapMaker.CopyFaceToCubemapTexture(cubemap.GetPixels(CubemapFace.PositiveZ), array, 4096);
                CubemapMaker.CopyFaceToCubemapTexture(cubemap.GetPixels(CubemapFace.NegativeZ), array, 5120);
                texture2D.SetPixels(array, 0);
                array2 = texture2D.EncodeToPNG();
            }
            finally
            {
                Object.Destroy(texture2D);
            }
            return array2;
        }

        private static void CopyFaceToCubemapTexture(Color[] source, Color[] dest, int offsetX)
        {
            int num = 6144;
            for (int i = 0; i < 1024; i++)
            {
                for (int j = 0; j < 1024; j++)
                {
                    dest[(1024 - i - 1) * num + j + offsetX] = ((CubemapMaker.colorProcessor != null) ? CubemapMaker.colorProcessor(source[i * 1024 + j]) : source[i * 1024 + j]);
                }
            }
        }

        private static bool wantsCubemap;

        private static Func<Color, Color> colorProcessor;

        private const int Size = 1024;
    }
}