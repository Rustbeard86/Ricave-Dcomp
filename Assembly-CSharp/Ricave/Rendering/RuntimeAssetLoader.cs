using System;
using System.IO;
using System.Threading;
using Dummiesman;
using Ricave.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace Ricave.Rendering
{
    public static class RuntimeAssetLoader
    {
        public static bool TryLoad(string path, out Object asset)
        {
            if (path.NullOrEmpty() || !File.Exists(path))
            {
                asset = null;
                return false;
            }
            bool flag2;
            try
            {
                string extension = Path.GetExtension(path);
                if (extension == ".png")
                {
                    bool flag = path.Contains("/UI/") || path.StartsWith("UI/") || path.Contains("\\UI\\") || path.StartsWith("UI\\");
                    Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, !flag);
                    texture2D.LoadImage(File.ReadAllBytes(path));
                    texture2D.name = Path.GetFileNameWithoutExtension(path);
                    texture2D.filterMode = FilterMode.Bilinear;
                    texture2D.anisoLevel = (flag ? 1 : 2);
                    texture2D.wrapMode = (flag ? TextureWrapMode.Clamp : TextureWrapMode.Repeat);
                    asset = texture2D;
                    flag2 = true;
                }
                else
                {
                    if (extension == ".wav" || extension == ".ogg")
                    {
                        using (UnityWebRequest audioClip = UnityWebRequestMultimedia.GetAudioClip("file:///" + Uri.EscapeUriString(Path.GetFullPath(path)), (extension == ".ogg") ? AudioType.OGGVORBIS : AudioType.WAV))
                        {
                            audioClip.SendWebRequest();
                            int num = 10000;
                            while (!audioClip.isDone && num > 0)
                            {
                                Thread.Sleep(1);
                                num--;
                            }
                            if (audioClip.error != null)
                            {
                                Log.Error("Error while loading audio clip \"" + path + "\": " + audioClip.error, false);
                                asset = null;
                                return false;
                            }
                            asset = DownloadHandlerAudioClip.GetContent(audioClip);
                            asset.name = Path.GetFileNameWithoutExtension(path);
                            return true;
                        }
                    }
                    if (extension == ".obj")
                    {
                        GameObject gameObject = new OBJLoader().Load(path);
                        Mesh mesh = gameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
                        mesh = MeshUtility.MirrorXAxis(mesh);
                        Object.Destroy(gameObject);
                        mesh.name = Path.GetFileNameWithoutExtension(path);
                        asset = mesh;
                        flag2 = true;
                    }
                    else if (extension == ".bundle" || extension == ".unity3d")
                    {
                        AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
                        assetBundle.name = Path.GetFileNameWithoutExtension(path);
                        asset = assetBundle;
                        flag2 = true;
                    }
                    else
                    {
                        Log.Warning("Unrecognized asset extension \"" + path + "\"", false);
                        asset = null;
                        flag2 = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while loading asset \"" + path + "\"", ex);
                asset = null;
                flag2 = false;
            }
            return flag2;
        }
    }
}