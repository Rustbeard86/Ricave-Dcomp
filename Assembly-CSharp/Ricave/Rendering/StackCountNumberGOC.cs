using System;
using Ricave.Core;
using TMPro;
using UnityEngine;

namespace Ricave.Rendering
{
    public class StackCountNumberGOC : MonoBehaviour
    {
        private void Start()
        {
            this.SetText();
        }

        private void OnEnable()
        {
            this.SetText();
        }

        public void SetSpecificStackCount(int stackCount)
        {
            this.specificStackCount = new int?(stackCount);
        }

        private void SetText()
        {
            if (this.specificStackCount != null)
            {
                int? num = this.specificStackCount;
                int num2 = 1;
                if ((num.GetValueOrDefault() == num2) & (num != null))
                {
                    base.gameObject.SetActive(false);
                    return;
                }
                base.gameObject.SetActive(true);
                base.GetComponent<TextMeshPro>().text = "x{0}".Formatted(this.specificStackCount.Value.ToStringCached());
                return;
            }
            else
            {
                ItemGOC componentInParent = base.GetComponentInParent<ItemGOC>();
                if (!(componentInParent == null) && componentInParent.Item != null && componentInParent.Item.StackCount != 1)
                {
                    base.gameObject.SetActive(true);
                    base.GetComponent<TextMeshPro>().text = "x{0}".Formatted(componentInParent.Item.StackCount.ToStringCached());
                    return;
                }
                if (componentInParent == null || componentInParent.Item == null)
                {
                    return;
                }
                base.gameObject.SetActive(false);
                return;
            }
        }

        private int? specificStackCount;
    }
}