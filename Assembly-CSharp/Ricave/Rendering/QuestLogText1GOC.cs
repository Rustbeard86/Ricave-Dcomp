using System;
using Ricave.Core;
using TMPro;
using UnityEngine;

namespace Ricave.Rendering
{
    public class QuestLogText1GOC : MonoBehaviour
    {
        private void Start()
        {
            base.GetComponent<TextMeshPro>().text = "QuestLogBoardText".Translate();
        }
    }
}