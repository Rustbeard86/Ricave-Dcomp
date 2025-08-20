using System;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class DungeonMapDrawerGOC : MonoBehaviour
    {
        public bool Showing
        {
            get
            {
                return this.showing;
            }
        }

        private void OnGUI()
        {
            Widgets.ApplySkin();
            GUI.depth = -75;
            if (!this.Showing)
            {
                return;
            }
            GUIExtra.DrawRect(Widgets.ScreenRect.ExpandedBy(1f), Color.black);
            DungeonMapDrawer.DrawMap(Widgets.ScreenRect, ref this.scrolled);
            Get.WindowManager.CloseAll();
            Get.UI.CloseWheelSelector(false);
            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
            {
                Event.current.Use();
            }
        }

        public void Show()
        {
            this.showing = true;
            this.scrolled = false;
            base.gameObject.SetActive(true);
            Get.UI.OnDungeonMapShowing();
        }

        public void Hide()
        {
            this.showing = false;
            this.scrolled = false;
            base.gameObject.SetActive(false);
        }

        private bool showing;

        private bool scrolled;
    }
}