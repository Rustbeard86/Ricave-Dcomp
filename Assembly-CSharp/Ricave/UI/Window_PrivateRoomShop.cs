using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_PrivateRoomShop : Window
    {
        public Window_PrivateRoomShop(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.selling.Clear();
            foreach (EntitySpec entitySpec in Get.Specs.GetAll<EntitySpec>())
            {
                if (entitySpec.IsStructure && entitySpec.Structure.PrivateRoomPriceTag != null)
                {
                    this.selling.Add(entitySpec);
                }
            }
            this.selling.StableSort<EntitySpec, bool, int>((EntitySpec x) => x.Structure.PrivateRoomPriceTag.ItemSpec == Get.Entity_Diamond, (EntitySpec x) => x.Structure.PrivateRoomPriceTag.Count);
        }

        protected override void DoWindowContents(Rect inRect)
        {
            int num = 0;
            Rect rect = Widgets.BeginScrollView(inRect, null);
            using (List<EntitySpec>.Enumerator enumerator = this.selling.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    EntitySpec spec = enumerator.Current;
                    PriceTag privateRoomPriceTag = spec.Structure.PrivateRoomPriceTag;
                    Rect rect2 = new Rect(rect.x, (float)(num * 75), rect.width, 75f);
                    GUIExtra.DrawRoundedRectBump(rect2, this.BackgroundColor.Lighter(0.04f), false, true, true, true, true, null);
                    GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
                    Rect rect3 = rect2.ContractedBy(20f);
                    Rect rect4 = new Rect(rect3.x, rect3.y, rect3.height, rect3.height);
                    GUI.color = spec.IconColor;
                    GUI.DrawTexture(rect4, spec.Icon);
                    GUI.color = Color.white;
                    Rect rect5 = new Rect(rect4.xMax + 5f, rect3.y, rect3.width - rect4.width - 5f, rect3.height);
                    Widgets.Align = TextAnchor.MiddleLeft;
                    Widgets.Label(rect5, RichText.Label(spec, false), true, null, null, false);
                    Widgets.ResetAlign();
                    Rect rect6 = new Rect(rect3.xMax - 100f, rect3.y, 100f, 29f);
                    if (privateRoomPriceTag.CanAfford(Get.MainActor))
                    {
                        if (Widgets.Button(rect6, "Buy".Translate(), true, null, null, true, true, true, true, null, true, true, null, false, null, null))
                        {
                            ActionViaInterfaceHelper.TryDo(() => new Action_BuyPrivateRoomStructure(Get.Action_BuyPrivateRoomStructure, spec));
                        }
                    }
                    else
                    {
                        Widgets.DisabledButton(rect6, "Buy".Translate());
                    }
                    Widgets.Align = TextAnchor.UpperRight;
                    Widgets.Label(rect5.MovedBy(0f, 28f), privateRoomPriceTag.PriceRichText, true, null, null, false);
                    Widgets.ResetAlign();
                    if (!Mouse.Over(rect6))
                    {
                        Get.Tooltips.RegisterTip(rect2, spec, null, null);
                    }
                    num++;
                }
            }
            Widgets.EndScrollView(inRect, (float)(num * 75), false, false);
        }

        private List<EntitySpec> selling = new List<EntitySpec>();

        private const int RowHeight = 75;

        private const float BuyButtonWidth = 100f;

        private const float BuyButtonHeight = 29f;
    }
}