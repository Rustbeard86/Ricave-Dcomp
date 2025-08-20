using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class PartyUI
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseDown && Event.current.type != EventType.KeyDown && Event.current.type != EventType.MouseUp)
            {
                return;
            }
            float num = 20f;
            List<Actor> party = Get.Player.Party;
            if (party.Count >= 2)
            {
                for (int i = 0; i < party.Count; i++)
                {
                    this.DoActor(party[i], ref num, new int?(i));
                }
            }
            List<Actor> followers = Get.Player.Followers;
            for (int j = 0; j < followers.Count; j++)
            {
                this.DoActor(followers[j], ref num, null);
            }
        }

        private void DoActor(Actor actor, ref float curY, int? partyIndex)
        {
            Rect rect = new Rect(321f, curY, 168f, 50f);
            if (Get.DragAndDrop.HoveringDragged<Item>(rect, null) && actor.IsPlayerParty)
            {
                GUIExtra.DrawHighlight(rect, true, true, true, true, 1f);
            }
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            Get.Tooltips.RegisterTip(rect, actor, null, null);
            Rect rect2 = new Rect(321f, curY, 168f, 25f);
            Rect rect3 = rect2.LeftPart(rect2.height);
            GUI.color = actor.IconColor;
            GUI.DrawTexture(rect3, actor.Icon);
            GUI.color = Color.white;
            Rect rect4 = rect2.CutFromLeft(rect3.width + 5f);
            string text = actor.LabelCap.Truncate(rect4.width);
            Widgets.Align = TextAnchor.MiddleLeft;
            if (partyIndex != null)
            {
                Widgets.FontBold = true;
            }
            float x = Widgets.CalcSize(text).x;
            Widgets.Label(rect4, text, true, null, null, false);
            Widgets.FontBold = false;
            Widgets.ResetAlign();
            if (actor.Spawned && !Get.VisibilityCache.PlayerSees(actor))
            {
                Rect rect5 = rect4.ExpandedBy(0f, 200f, 0f, 0f).CutFromLeft(x + 4f).LeftPart(rect4.height)
                    .ContractedBy(1f);
                float angleXZFromCamera_Simple = Vector3Utility.GetAngleXZFromCamera_Simple(actor.RenderPositionComputedCenter);
                GUI.color = new Color(0.8f, 0.8f, 0.8f);
                GUIExtra.DrawTextureRotated(rect5, PartyUI.ArrowTex, angleXZFromCamera_Simple + 180f, null);
                GUI.color = Color.white;
            }
            Rect rect6 = new Rect(321f, curY + 25f + 3f, 168f, 22f);
            float num = (PlayerActorStatusControls.IsActorOnLowHP(actor) ? PlayerActorStatusControls.LowHPFlash : 0f);
            Get.ProgressBarDrawer.Draw(rect6, actor.HP, actor.MaxHP, TipSubjectDrawer_Entity.GetHPBarColor(actor).Lighter(num), 1f, 1f, true, false, new int?(actor.InstanceID), TipSubjectDrawer_Entity.GetHPBarValueChangeDirection(actor), Get.InteractionManager.GetLostHPRangeForUI(actor), true, true, true, true, true, false, null);
            if (partyIndex != null)
            {
                Rect rect7 = new Rect(rect.xMax, rect.y + 3f, 21f, 21f);
                Rect rect8 = rect7.ExpandedBy(2f);
                GUIExtra.DrawHighlightIfMouseover(rect8, true, true, true, true, true);
                Get.Tooltips.RegisterTip(rect8, "PartyMemberAIModeTip".Translate(), null);
                if (actor.PartyMemberAIMode == PartyMemberAIMode.Stay)
                {
                    GUI.color = new Color(1f, 0.7f, 0.7f, 0.5f);
                }
                else
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                }
                GUI.DrawTexture(rect7, actor.PartyMemberAIMode.GetIcon());
                GUI.color = Color.white;
                if (Widgets.ButtonInvisible(rect8, true, false))
                {
                    List<ValueTuple<string, Action>> list = new List<ValueTuple<string, Action>>();
                    Func<Action> <> 9__4;
                    list.Add(new ValueTuple<string, Action>("PartyMemberAIMode_Follow".Translate(), delegate
                    {
                        Func<Action> func;
                        if ((func = <> 9__4) == null)
                        {
                            func = (<> 9__4 = () => new Action_ChangePartyMemberAIMode(Get.Action_ChangePartyMemberAIMode, actor, PartyMemberAIMode.Follow));
                        }
                        ActionViaInterfaceHelper.TryDo(func);
                        Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    }));
                    Func<Action> <> 9__5;
                    list.Add(new ValueTuple<string, Action>("PartyMemberAIMode_Stay".Translate(), delegate
                    {
                        Func<Action> func2;
                        if ((func2 = <> 9__5) == null)
                        {
                            func2 = (<> 9__5 = () => new Action_ChangePartyMemberAIMode(Get.Action_ChangePartyMemberAIMode, actor, PartyMemberAIMode.Stay));
                        }
                        ActionViaInterfaceHelper.TryDo(func2);
                        Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    }));
                    Get.WindowManager.OpenContextMenu(list, "PartyMemberAIMode".Translate());
                }
            }
            if (partyIndex != null && !ControllerUtility.InControllerMode)
            {
                KeyCode partyMemberHotkey = this.GetPartyMemberHotkey(partyIndex.Value);
                GUI.color = Color.gray;
                Widgets.LabelCenteredV(rect.RightCenter().MovedBy(3f, 14f), partyMemberHotkey.ToStringCached(), true, null, null, false);
                GUI.color = Color.white;
            }
            if (actor.IsNowControlledActor)
            {
                GUIExtra.DrawRoundedRectBump(rect, new Color(0.5f, 0.5f, 0.2f, 0.3f), false, true, true, true, true, null);
            }
            else if (actor.ActorGOC != null)
            {
                float num2 = Calc.ResolveFadeInStayOut(Clock.Time - actor.ActorGOC.LastTimeLostHP, 0.03f, 0.02f, 0.26f);
                if (num2 > 0f)
                {
                    GUIExtra.DrawRoundedRectBump(rect, new Color(1f, 0.2f, 0.2f, num2 * 0.8f), false, true, true, true, true, null);
                }
            }
            if (Get.InteractionManager.PointedAtEntity == actor)
            {
                GUIExtra.DrawRoundedRectOutline(rect, SeenEntitiesDrawer.PointedAtEntityOutlineColor, 2f, true, true, true, true);
            }
            Item item = Get.DragAndDrop.ConsumeDropped<Item>(rect, null);
            if (item != null)
            {
                this.HandleDroppedItem(item, actor);
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.Over(rect))
            {
                List<ValueTuple<string, Action, string>> contextMenuOptions = SeenEntitiesDrawer.GetContextMenuOptions(actor);
                Get.WindowManager.OpenContextMenu(contextMenuOptions, actor.LabelCap);
                Event.current.Use();
            }
            if (Widgets.ButtonInvisible(rect, false, false) && Event.current.button == 0 && !actor.IsNowControlledActor)
            {
                if (partyIndex != null && actor.Spawned)
                {
                    Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                    ActionViaInterfaceHelper.TryDo(() => new Action_SwitchCurrentPartyMember(Get.Action_SwitchCurrentPartyMember, actor));
                }
                else
                {
                    Get.FPPControllerGOC.RotateToFace(actor.RenderPositionComputedCenter);
                    Get.Sound_Rotation.PlayOneShot(null, 1f, 1f);
                }
            }
            if (partyIndex != null && !actor.IsNowControlledActor && actor.Spawned && Event.current.type == EventType.KeyDown && Event.current.keyCode == this.GetPartyMemberHotkey(partyIndex.Value))
            {
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                ActionViaInterfaceHelper.TryDo(() => new Action_SwitchCurrentPartyMember(Get.Action_SwitchCurrentPartyMember, actor));
            }
            curY += 60f;
        }

        private KeyCode GetPartyMemberHotkey(int index)
        {
            if (index >= 10)
            {
                return KeyCode.None;
            }
            return KeyCode.F1 + index;
        }

        private void HandleDroppedItem(Item droppedItem, Actor targetActor)
        {
            Inventory parentInventory = droppedItem.ParentInventory;
            Actor actor = ((parentInventory != null) ? parentInventory.Owner : null);
            if (actor == null)
            {
                return;
            }
            PartyUI.TryTransferItem(actor, targetActor, droppedItem);
        }

        public static bool TryTransferItem(Actor from, Actor to, Item item)
        {
            if (from == null || to == null || !from.Spawned || !to.Spawned || !from.IsPlayerParty || !to.IsPlayerParty || from == to || !from.Inventory.Contains(item))
            {
                return false;
            }
            if (from.Inventory.Equipped.IsEquipped(item) && item.Cursed)
            {
                Get.PlayLog.Add("CannotUnequipCursedItem".Translate());
                return false;
            }
            if (!Action_Transfer.CanReach(from, to))
            {
                Get.PlayLog.Add("CantReach".Translate());
                return false;
            }
            if (!InstructionSets_Actor.HasSpaceToPickUp(to, item))
            {
                Get.PlayLog.Add("NoSpace".Translate());
                return false;
            }
            return !to.IsHostile(from) && ActionViaInterfaceHelper.TryDo(() => new Action_Transfer(Get.Action_Transfer, from, to, item));
        }

        private static readonly Texture2D ArrowTex = Assets.Get<Texture2D>("Textures/UI/Arrow");
    }
}