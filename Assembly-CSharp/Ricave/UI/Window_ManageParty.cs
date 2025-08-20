using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_ManageParty : Window
    {
        public Window_ManageParty(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.partyMembersInUIOrder = (from x in Get.Specs.GetAll<ChooseablePartyMemberSpec>()
                                          orderby x.UIOrder
                                          select x).ToList<ChooseablePartyMemberSpec>();
            this.partyMembersInUIOrder.Insert(0, null);
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            float num = rect.y;
            Widgets.Label(rect.x, rect.width, ref num, "ManagePartyInfo1".Translate(), true);
            num += 10f;
            GUI.color = new Color(1f, 0.7f, 0.7f);
            Widgets.Label(rect.x, rect.width, ref num, "ManagePartyInfo2".Translate(0.100000024f.ToStringPercent(false)), true);
            GUI.color = Color.white;
            num = 130f;
            Rect? rect2 = null;
            foreach (ChooseablePartyMemberSpec chooseablePartyMemberSpec in this.partyMembersInUIOrder)
            {
                Rect rect3 = new Rect(rect.x + 2f, num, rect.width - 4f, 80f);
                object obj = chooseablePartyMemberSpec == null || Get.Progress.TotalSpiritsSetFree >= chooseablePartyMemberSpec.RequiredSpirits;
                bool flag = Get.Progress.LastChosenPartyMember == chooseablePartyMemberSpec;
                EntitySpec entitySpec = ((chooseablePartyMemberSpec != null) ? chooseablePartyMemberSpec.ActorSpec : null);
                if (flag)
                {
                    rect2 = new Rect?(rect3);
                }
                GUIExtra.DrawRoundedRectBump(rect3, new Color(1f, 1f, 1f, 0.08f), false, true, true, true, true, null);
                GUIExtra.DrawHighlightIfMouseover(rect3, true, true, true, true, true);
                if (chooseablePartyMemberSpec != null)
                {
                    Get.Tooltips.RegisterTip(rect3, chooseablePartyMemberSpec.ActorSpec, null, null);
                }
                Rect rect4 = new Rect(rect3.x + 10f, rect3.y + 10f, 60f, 60f);
                if (chooseablePartyMemberSpec != null)
                {
                    GUI.color = entitySpec.IconColorAdjusted;
                    GUI.DrawTexture(rect4, entitySpec.IconAdjusted);
                    GUI.color = Color.white;
                }
                Vector2 vector = new Vector2(rect4.xMax + 10f, rect3.center.y);
                Widgets.FontSizeScalable = 20;
                Widgets.LabelCenteredV(vector, ((entitySpec != null) ? entitySpec.LabelAdjustedCap : null) ?? "None".Translate(), true, null, null, false);
                Widgets.ResetFontSize();
                object obj2 = obj;
                if (obj2 == null)
                {
                    GUIExtra.DrawRoundedRect(rect3, new Color(0.08f, 0.08f, 0.08f, 0.74f), true, true, true, true, null);
                    Widgets.FontBold = true;
                    Widgets.FontSizeScalable = 18;
                    Widgets.LabelCentered(rect3, (chooseablePartyMemberSpec.RequiredSpirits == 1) ? "PartyMemberLockedSingle".Translate() : "PartyMemberLocked".Translate(chooseablePartyMemberSpec.RequiredSpirits), true, null, null);
                    Widgets.ResetFontSize();
                    Widgets.FontBold = false;
                }
                if (obj2 != null && Widgets.ButtonInvisible(rect3, true, true))
                {
                    Get.Progress.LastChosenPartyMember = chooseablePartyMemberSpec;
                }
                num += 88f;
            }
            if (rect2 != null)
            {
                GUIExtra.DrawRoundedRectOutline(rect2.Value.ExpandedBy(2f), Color.white, 2f, true, true, true, true);
            }
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private List<ChooseablePartyMemberSpec> partyMembersInUIOrder;
    }
}