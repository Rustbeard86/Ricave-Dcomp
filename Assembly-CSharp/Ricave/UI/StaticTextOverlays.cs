using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class StaticTextOverlays
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (this.entries.Count == 0)
            {
                return;
            }
            if (!Get.UI.WantsMouseUnlocked && KeyCodeUtility.InspectHeldDown)
            {
                return;
            }
            if (Get.WindowManager.IsOpen(Get.Window_QuestLog))
            {
                return;
            }
            if (this.entries.Count >= 2)
            {
                this.entries.Sort(StaticTextOverlays.ByDistToCamera);
            }
            for (int i = 0; i < this.entries.Count; i++)
            {
                StaticTextOverlays.Entry entry = this.entries[i];
                if (!(Get.NowControlledActor.Position == entry.entity.Position))
                {
                    Vector3 vector = ((entry.entity.Spec == Get.Entity_DailyChallengeBoard) ? entry.entity.RenderPositionComputedCenter : entry.entity.Position);
                    Vector3 vector2 = Get.Camera.WorldToScreenPoint(vector.WithAddedY(this.GetYOffset(entry.entity))) / Widgets.UIScale;
                    vector2.y = Widgets.VirtualHeight - vector2.y;
                    if (vector2.z > 0f)
                    {
                        string text = this.GetText(entry.entity);
                        if (!text.NullOrEmpty())
                        {
                            float num = 1f;
                            float magnitude = (Get.CameraPosition - entry.entity.Position).magnitude;
                            if (magnitude < 3f)
                            {
                                num = Calc.LerpDouble(0.5f, 3f, 2.3f, 1f, magnitude);
                            }
                            Item item = entry.entity as Item;
                            if (item != null && item.ForSale)
                            {
                                num *= 1.4f;
                            }
                            if (entry.entity.Spec == Get.Entity_Sign)
                            {
                                num *= 0.5f;
                            }
                            Widgets.FontSizeScalable = (int)(15f * num);
                            Widgets.Align = TextAnchor.MiddleCenter;
                            Widgets.LabelCentered(vector2, text, true, null, null, false, true, false, null);
                            Widgets.ResetAlign();
                            Widgets.ResetFontSize();
                        }
                    }
                }
            }
        }

        public void OnEntitiesVisibilityChanged(List<Entity> entitiesNoLongerSeen, List<Entity> entitiesNewlySeen)
        {
            for (int i = 0; i < entitiesNoLongerSeen.Count; i++)
            {
                if (this.HasText(entitiesNoLongerSeen[i]))
                {
                    for (int j = 0; j < this.entries.Count; j++)
                    {
                        if (this.entries[j].entity == entitiesNoLongerSeen[i])
                        {
                            this.entries.RemoveAt(j);
                            break;
                        }
                    }
                }
            }
            for (int k = 0; k < entitiesNewlySeen.Count; k++)
            {
                if (this.HasText(entitiesNewlySeen[k]))
                {
                    bool flag = false;
                    for (int l = 0; l < this.entries.Count; l++)
                    {
                        if (this.entries[l].entity == entitiesNewlySeen[k])
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        this.entries.Add(new StaticTextOverlays.Entry
                        {
                            entity = entitiesNewlySeen[k]
                        });
                    }
                }
            }
        }

        private bool HasText(Entity entity)
        {
            if (entity.Spec != Get.Entity_NewRunStaircase && entity.Spec != Get.Entity_NewRunWithSeedStaircase && entity.Spec != Get.Entity_TrainingRoomStaircase && entity.Spec != Get.Entity_QuestLog && entity.Spec != Get.Entity_StatsBoard && entity.Spec != Get.Entity_LockedDoor && entity.Spec != Get.Entity_LockedDoorTrainingRoom && entity.Spec != Get.Entity_LockedDoorHallway && entity.Spec != Get.Entity_LobbyShopkeeperCage && entity.Spec != Get.Entity_LockedPuzzleDoor && entity.Spec != Get.Entity_LobbyShopkeeper && entity.Spec != Get.Entity_Gate && entity.Spec != Get.Entity_GateReinforced && entity.Spec != Get.Entity_Spirit && entity.Spec != Get.Entity_Sign && entity.Spec != Get.Entity_DailyChallengeBoard && entity.Spec != Get.Entity_MagicMirror)
            {
                LifespanComp comp = entity.GetComp<LifespanComp>();
                if (comp == null || !comp.Props.ShowTurnsLeftAsStaticTextOverlay)
                {
                    Item item = entity as Item;
                    return item != null && item.ForSale;
                }
            }
            return true;
        }

        private string GetText(Entity entity)
        {
            if (entity.Spec == Get.Entity_NewRunStaircase)
            {
                if (this.cachedNewRunStaircaseText == null)
                {
                    this.cachedNewRunStaircaseText = string.Concat(new string[]
                    {
                        RichText.Bold("EnterDungeon".Translate()),
                        "\n",
                        RichText.Grayed("Attempts".Translate() + ": "),
                        Get.Progress.MainRuns.ToString(),
                        "\n",
                        RichText.Grayed("MaxScore".Translate() + ": "),
                        Get.Progress.MaxMainScore.ToString(),
                        "\n",
                        RichText.Grayed("MaxFloorReached".Translate() + ": "),
                        Get.Progress.MaxMainFloorReached.ToString(),
                        "\n",
                        RichText.Grayed("TotalKillCount".Translate() + ": "),
                        Get.Progress.MainKillCount.ToString()
                    });
                }
                return this.cachedNewRunStaircaseText;
            }
            if (entity.Spec == Get.Entity_NewRunWithSeedStaircase)
            {
                if (this.cachedNewRunWithSeedStaircaseText == null)
                {
                    this.cachedNewRunWithSeedStaircaseText = RichText.Bold("EnterDungeonWithSeed".Translate()) + "\n" + RichText.Grayed("ProgressDisabled".Translate());
                }
                return this.cachedNewRunWithSeedStaircaseText;
            }
            if (entity.Spec == Get.Entity_TrainingRoomStaircase)
            {
                if (this.cachedTrainingRoomStaircaseText == null)
                {
                    this.cachedTrainingRoomStaircaseText = RichText.Bold("EnterTrainingRoom".Translate()) + "\n" + RichText.Grayed("ProgressDisabled".Translate());
                }
                return this.cachedTrainingRoomStaircaseText;
            }
            if (entity.Spec == Get.Entity_QuestLog)
            {
                return "QuestLog".Translate();
            }
            if (entity.Spec == Get.Entity_StatsBoard)
            {
                return "Stats".Translate();
            }
            if (entity.Spec == Get.Entity_DailyChallengeBoard)
            {
                return "DailyChallengeBoardText".Translate().AppendedInNewLine(RichText.Grayed("{0}: ".Formatted("MaxScore".Translate())).Concatenated(Get.Progress.AllDailyChallengesBestScore.ToStringCached()));
            }
            if (entity.Spec == Get.Entity_LockedDoorHallway || entity.Spec == Get.Entity_LockedDoorTrainingRoom)
            {
                return "UnlockFor".Translate(((Structure)entity).UseEffects.Price.PriceShortRichText);
            }
            if (entity.Spec == Get.Entity_LockedDoor)
            {
                if (entity.Position == new Vector3(10f, 1f, 10f))
                {
                    return "UnlockedByQuest".Translate();
                }
                return "Locked".Translate();
            }
            else if (entity.Spec == Get.Entity_LobbyShopkeeperCage)
            {
                int num = (int)Clock.Time / 5 % 3;
                if (num == 0)
                {
                    return "Help".Translate();
                }
                if (num == 1)
                {
                    return "OverHere".Translate();
                }
                return "HelpMe".Translate();
            }
            else if (entity.Spec == Get.Entity_LobbyShopkeeper)
            {
                int num2 = (int)Clock.Time / 5 % 9;
                if (num2 == 0)
                {
                    return "LobbyShopkeeperSpeak_1".Translate();
                }
                if (num2 == 3)
                {
                    return "LobbyShopkeeperSpeak_2".Translate();
                }
                if (num2 == 6)
                {
                    return "LobbyShopkeeperSpeak_3".Translate();
                }
                return null;
            }
            else
            {
                if (entity.Spec == Get.Entity_LockedPuzzleDoor)
                {
                    if (this.cachedLockedPuzzleDoorText == null)
                    {
                        this.cachedLockedPuzzleDoorText = "Locked".Translate() + "\n" + "PuzzlePiecesCollected".Translate(Get.TotalLobbyItems.GetCount(Get.Entity_PuzzlePiece), 10);
                    }
                    return this.cachedLockedPuzzleDoorText;
                }
                if (entity.Spec == Get.Entity_Gate || entity.Spec == Get.Entity_GateReinforced)
                {
                    if (Get.RunSpec == Get.Run_Tutorial)
                    {
                        if (Get.NowControlledActor != null && Get.NowControlledActor.Position.IsAdjacent(entity.Position))
                        {
                            return "FollowTutorialToOpen".Translate();
                        }
                        return null;
                    }
                    else
                    {
                        if (Get.PlaceSpec == Get.Place_Normal)
                        {
                            return "FindLeverToOpen".Translate();
                        }
                        return null;
                    }
                }
                else
                {
                    if (entity.Spec == Get.Entity_MagicMirror)
                    {
                        return "Traits".Translate().AppendedInNewLine(RichText.Grayed("{0}: ".Formatted("UnlockedTraitsCount".Translate())).Concatenated("{0} / {1}".Formatted(Get.TraitManager.Unlocked.Count, Get.Specs.GetAll<TraitSpec>().Count)));
                    }
                    if (entity.Spec == Get.Entity_Spirit)
                    {
                        Actor actor = entity as Actor;
                        if (actor != null && actor.AttachedToChainPost != null && !actor.IsHostile(Get.MainActor))
                        {
                            int num3 = (int)Clock.Time / 5 % 3;
                            if (num3 == 0)
                            {
                                return "Help".Translate();
                            }
                            if (num3 == 1)
                            {
                                return "OverHere".Translate();
                            }
                            return "HelpMe".Translate();
                        }
                    }
                    if (entity.Spec == Get.Entity_Sign && Get.RunSpec == Get.Run_Main1 && Get.Floor == 1 && Get.TurnManager.CurrentSequence <= Get.TurnManager.InceptionSequence + 12)
                    {
                        return "ReadSignTip".Translate().FormattedKeyBindings();
                    }
                    LifespanComp comp = entity.GetComp<LifespanComp>();
                    if (comp != null && comp.Props.ShowTurnsLeftAsStaticTextOverlay)
                    {
                        string text = StringUtility.TurnsString(comp.TurnsLeft);
                        if (!comp.Props.TurnsLeftStaticTextOverlayFormat.NullOrEmpty())
                        {
                            return comp.Props.TurnsLeftStaticTextOverlayFormat.Formatted(text);
                        }
                        return text;
                    }
                    else
                    {
                        Item item = entity as Item;
                        if (item != null && item.ForSale)
                        {
                            return item.PriceTag.PriceShortRichText;
                        }
                        return null;
                    }
                }
            }
        }

        private float GetYOffset(Entity entity)
        {
            if (entity.Spec == Get.Entity_NewRunStaircase || entity.Spec == Get.Entity_NewRunWithSeedStaircase || entity.Spec == Get.Entity_TrainingRoomStaircase)
            {
                return 0.3f;
            }
            if (entity.Spec == Get.Entity_QuestLog)
            {
                return 0.5f;
            }
            if (entity.Spec == Get.Entity_StatsBoard)
            {
                return 0.5f;
            }
            if (entity.Spec == Get.Entity_DailyChallengeBoard)
            {
                return 0.38f;
            }
            if (entity.Spec == Get.Entity_LockedDoor || entity.Spec == Get.Entity_LockedDoorHallway || entity.Spec == Get.Entity_LockedDoorTrainingRoom)
            {
                return 0f;
            }
            if (entity.Spec == Get.Entity_LobbyShopkeeperCage)
            {
                return 0.55f;
            }
            if (entity.Spec == Get.Entity_LobbyShopkeeper)
            {
                return 0.49f;
            }
            if (entity.Spec == Get.Entity_LockedPuzzleDoor)
            {
                return 0f;
            }
            if (entity.Spec == Get.Entity_MagicMirror)
            {
                return 0.45f;
            }
            if (entity is Item)
            {
                return 0.2f;
            }
            if (entity is Actor)
            {
                return 0.49f;
            }
            return 0f;
        }

        private List<StaticTextOverlays.Entry> entries = new List<StaticTextOverlays.Entry>();

        private string cachedNewRunStaircaseText;

        private string cachedNewRunWithSeedStaircaseText;

        private string cachedTrainingRoomStaircaseText;

        private string cachedLockedPuzzleDoorText;

        private static readonly Comparison<StaticTextOverlays.Entry> ByDistToCamera = (StaticTextOverlays.Entry a, StaticTextOverlays.Entry b) => (b.entity.RenderPosition - Get.CameraPosition).sqrMagnitude.CompareTo((a.entity.RenderPosition - Get.CameraPosition).sqrMagnitude);

        private struct Entry
        {
            public Entity entity;
        }
    }
}