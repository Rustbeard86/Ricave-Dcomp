using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class LobbyQuestCountLabel
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (!Get.InLobby)
            {
                return;
            }
            int availableToTakeQuestsCount = Get.QuestManager.AvailableToTakeQuestsCount;
            if (availableToTakeQuestsCount <= 0)
            {
                return;
            }
            Rect rect = new Rect(Widgets.VirtualWidth - 192f - 30f, 350f, 192f, 57f);
            string text = ((availableToTakeQuestsCount == 1) ? "OneQuestAvailable".Translate() : "QuestsAvailable".Translate(availableToTakeQuestsCount));
            GUI.color = new Color(0.85f, 0.85f, 0.85f);
            Widgets.FontSizeScalable = 22;
            Widgets.Align = TextAnchor.UpperRight;
            Widgets.Label(rect, text, true, "QuestsAvailableTip".Translate(), null, false);
            Widgets.ResetAlign();
            Widgets.ResetFontSize();
            GUI.color = Color.white;
        }

        private const int Width = 192;

        private const int FontSize = 22;

        private const int Pad = 30;
    }
}