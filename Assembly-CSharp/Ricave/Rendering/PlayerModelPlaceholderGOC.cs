using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class PlayerModelPlaceholderGOC : MonoBehaviour
    {
        public void ResolveModel()
        {
            foreach (object obj in Get.PlayerModel.Model.transform)
            {
                Transform transform = (Transform)obj;
                Object.Instantiate<GameObject>(transform.gameObject, base.transform.parent).name = transform.name;
            }
            Object.Destroy(base.gameObject);
        }
    }
}