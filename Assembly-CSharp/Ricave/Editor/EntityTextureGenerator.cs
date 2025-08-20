using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Editor
{
    public class EntityTextureGenerator : MonoBehaviour
    {
        private void Start()
        {
            QualitySettings.antiAliasing = 0;
            foreach (object obj in this.actors.transform)
            {
                Transform transform = (Transform)obj;
                if (transform.gameObject.activeInHierarchy)
                {
                    transform.gameObject.SetActive(false);
                    this.actorsToDo.Add(transform.gameObject);
                }
            }
            foreach (object obj2 in this.items.transform)
            {
                Transform transform2 = (Transform)obj2;
                if (transform2.gameObject.activeInHierarchy)
                {
                    transform2.gameObject.SetActive(false);
                    this.itemsToDo.Add(transform2.gameObject);
                }
            }
            for (int i = 0; i < this.actorsToDo.Count; i++)
            {
                this.actorsToDo[i].SetActive(true);
                this.itemName = null;
                this.actorName = this.actorsToDo[i].name;
                this.SaveTexture();
                this.SaveBodyMap();
                this.actorsToDo[i].SetActive(false);
            }
            for (int j = 0; j < this.itemsToDo.Count; j++)
            {
                this.itemsToDo[j].SetActive(true);
                this.itemName = this.itemsToDo[j].name;
                this.actorName = null;
                this.SaveTexture();
                this.itemsToDo[j].SetActive(false);
            }
            Log.Message("Done");
        }

        private void SaveTexture()
        {
            Camera component = base.GetComponent<Camera>();
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = component.targetTexture;
            component.Render();
            Texture2D texture2D = new Texture2D(component.targetTexture.width, component.targetTexture.height);
            texture2D.ReadPixels(new Rect(0f, 0f, (float)component.targetTexture.width, (float)component.targetTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;
            if (!this.itemName.NullOrEmpty())
            {
                this.DoOutline(texture2D, false, 17, false);
            }
            else
            {
                this.DoOutline(texture2D, false, 14, false);
            }
            this.MultByUniversal(texture2D);
            if (!this.itemName.NullOrEmpty())
            {
                Texture2D texture2D2 = this.Rescale(texture2D, 512);
                Object.Destroy(texture2D);
                texture2D = texture2D2;
            }
            byte[] array = texture2D.EncodeToPNG();
            Texture2D texture2D3 = this.GenerateIcon(texture2D);
            byte[] array2 = texture2D3.EncodeToPNG();
            Object.Destroy(texture2D);
            Object.Destroy(texture2D3);
            if (!this.actorName.NullOrEmpty())
            {
                File.WriteAllBytes("Assets/Resources/Textures/Actors/" + this.actorName + ".png", array);
                File.WriteAllBytes("Assets/Resources/Textures/UI/Actors/" + this.actorName + "_Icon.png", array2);
                return;
            }
            File.WriteAllBytes("Assets/Resources/Textures/Items/" + this.itemName + ".png", array);
            File.WriteAllBytes("Assets/Resources/Textures/UI/Items/" + this.itemName + "_Icon.png", array2);
        }

        private void SaveBlur()
        {
            Camera component = base.GetComponent<Camera>();
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = component.targetTexture;
            component.Render();
            Texture2D texture2D = new Texture2D(component.targetTexture.width, component.targetTexture.height);
            texture2D.ReadPixels(new Rect(0f, 0f, (float)component.targetTexture.width, (float)component.targetTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;
            this.DoOutline(texture2D, false, 25, true);
            byte[] array = texture2D.EncodeToPNG();
            Object.Destroy(texture2D);
            File.WriteAllBytes("Assets/Resources/Textures/Actors/" + this.actorName + "Blur.png", array);
        }

        private void SaveBodyMap()
        {
            foreach (Renderer renderer in this.actors.GetComponentsInChildren<Renderer>())
            {
                bool flag = false;
                for (int j = 0; j < BodyPartUtility.ColorToIndex.Length; j++)
                {
                    if (renderer.gameObject.name == j.ToString())
                    {
                        renderer.material.color = BodyPartUtility.ColorToIndex[j];
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    renderer.enabled = false;
                }
            }
            Camera component = base.GetComponent<Camera>();
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = component.targetTexture;
            component.Render();
            Texture2D texture2D = new Texture2D(component.targetTexture.width, component.targetTexture.height);
            texture2D.ReadPixels(new Rect(0f, 0f, (float)component.targetTexture.width, (float)component.targetTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;
            this.DoOutline(texture2D, true, 14, false);
            byte[] array = texture2D.EncodeToPNG();
            Object.Destroy(texture2D);
            File.WriteAllBytes("Assets/Resources/Textures/Actors/" + this.actorName + "BodyMap.png", array);
        }

        private void DoOutline(Texture2D texture, bool closestColor, int size, bool setAllToWhite = false)
        {
            Color[] pixels = texture.GetPixels();
            Color[] newPixels = (Color[])pixels.Clone();
            int width = texture.width;
            int height = texture.width;
            Parallel.For(0, width * height, delegate (int i)
            {
                int num = i % width;
                int num2 = i / width;
                if (pixels[i].a > 0f)
                {
                    if (setAllToWhite)
                    {
                        newPixels[i] = Color.white.WithAlpha(pixels[i].a);
                    }
                    return;
                }
                float num3 = 9999f;
                Color color = default(Color);
                for (int j = num - size; j <= num + size; j++)
                {
                    for (int k = num2 - size; k <= num2 + size; k++)
                    {
                        if (j >= 0 && k >= 0 && j < width && k < height && pixels[k * width + j].a > 0f)
                        {
                            float magnitude = (new Vector2((float)num, (float)num2) - new Vector2((float)j, (float)k)).magnitude;
                            if (magnitude < num3)
                            {
                                num3 = magnitude;
                                color = pixels[k * width + j];
                            }
                        }
                    }
                }
                if (num3 < (float)size)
                {
                    float num4 = num3 / (float)size;
                    Color color2;
                    if (setAllToWhite)
                    {
                        color2 = Color.white;
                    }
                    else if (closestColor)
                    {
                        color2 = color;
                    }
                    else
                    {
                        color2 = Color.black;
                    }
                    newPixels[i] = color2;
                }
            });
            texture.SetPixels(newPixels);
            texture.Apply();
        }

        private void MultByUniversal(Texture2D texture)
        {
            Color[] pixels = texture.GetPixels();
            Color[] newPixels = (Color[])pixels.Clone();
            Color[] universalPixels = this.universalTexture.GetPixels();
            int width = texture.width;
            int width2 = texture.width;
            Parallel.For(0, width * width2, delegate (int i)
            {
                int num = i % width;
                int num2 = i / width;
                if (pixels[i].a <= 0f)
                {
                    return;
                }
                newPixels[i] = pixels[i] * universalPixels[i];
            });
            texture.SetPixels(newPixels);
            texture.Apply();
        }

        private Texture2D GenerateIcon(Texture2D tex)
        {
            return this.Rescale(tex, 256);
        }

        private Texture2D Rescale(Texture2D tex, int toSize)
        {
            RenderTexture renderTexture = new RenderTexture(toSize, toSize, 24);
            RenderTexture.active = renderTexture;
            Graphics.Blit(tex, renderTexture);
            Texture2D texture2D = new Texture2D(toSize, toSize);
            texture2D.ReadPixels(new Rect(0f, 0f, (float)toSize, (float)toSize), 0, 0);
            texture2D.Apply();
            RenderTexture.active = null;
            Object.Destroy(renderTexture);
            return texture2D;
        }

        public Texture2D universalTexture;

        public GameObject actors;

        public GameObject items;

        private string actorName;

        private string itemName;

        private List<GameObject> actorsToDo = new List<GameObject>();

        private List<GameObject> itemsToDo = new List<GameObject>();
    }
}