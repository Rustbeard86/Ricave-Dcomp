using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Confirmation : Window
    {
        protected override string Title
        {
            get
            {
                return this.customTitle ?? base.Title;
            }
        }

        public Window_Confirmation(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Widgets.Label(base.MainArea(inRect), this.text, true, null, null, false);
            bool flag;
            if (this.onlyOKButton)
            {
                flag = base.DoBottomButton("OK".Translate(), inRect, 0, 1, false, null);
            }
            else
            {
                if (base.DoBottomButton("No".Translate(), inRect, 0, 2, false, null))
                {
                    Get.WindowManager.Close(this, true);
                    if (this.onRejected != null)
                    {
                        try
                        {
                            this.onRejected();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error while executing confirmation window onRejected.", ex);
                        }
                    }
                }
                flag = base.DoBottomButton("Yes".Translate(), inRect, 1, 2, false, null);
            }
            if (flag)
            {
                Get.WindowManager.Close(this, true);
                if (this.onConfirmed != null)
                {
                    try
                    {
                        this.onConfirmed();
                    }
                    catch (Exception ex2)
                    {
                        Log.Error("Error while executing confirmation window onConfirmed.", ex2);
                    }
                }
            }
        }

        public void Init(string text, Action onConfirmed, bool onlyOKButton = false, string customTitle = null, Action onRejected = null)
        {
            this.text = text;
            this.onConfirmed = onConfirmed;
            this.onlyOKButton = onlyOKButton;
            this.customTitle = customTitle;
            this.onRejected = onRejected;
        }

        private string text;

        private Action onConfirmed;

        private Action onRejected;

        private bool onlyOKButton;

        private string customTitle;
    }
}