using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class NextTurnsUI
    {
        private float StartX
        {
            get
            {
                return Widgets.VirtualWidth - 10f - 272f - 35f;
            }
        }

        private float StartY
        {
            get
            {
                return Widgets.VirtualHeight - 10f - 34f;
            }
        }

        public int LastFrameDisplayed
        {
            get
            {
                return this.lastFrameDisplayed;
            }
        }

        private bool AnyAdjacentHostileActor
        {
            get
            {
                foreach (Actor actor in Get.VisibilityCache.ActorsSeen_Unordered)
                {
                    if (!actor.IsNowControlledActor && actor.Position.IsAdjacent(Get.NowControlledActor.Position) && actor.IsHostile(Get.NowControlledActor))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.UpdateGlobalAlpha();
            }
            if (Get.UI.InventoryOpen)
            {
                return;
            }
            if (Event.current.type == EventType.Repaint)
            {
                this.CheckUpdateEntries();
            }
            if (this.globalAlpha <= 0f)
            {
                return;
            }
            this.lastFrameDisplayed = Clock.Frame;
            GUIExtra.DrawRoundedRect(new Rect(this.StartX, this.StartY, 34f, 34f), new Color(1f, 1f, 1f, 0.3f * this.globalAlpha), true, true, true, true, null);
            for (int i = 0; i < this.fadingOutEntries.Count; i++)
            {
                this.DrawEntry(this.fadingOutEntries[i]);
            }
            for (int j = 0; j < this.entries.Count; j++)
            {
                this.DrawEntry(this.entries[j]);
            }
        }

        private void UpdateGlobalAlpha()
        {
            bool flag = Get.NowControlledActor.Spawned && ((KeyCodeUtility.InspectHeldDown && Get.VisibilityCache.AnyNonNowControlledActorSeen && !Get.UI.InventoryOpen) || this.AnyAdjacentHostileActor);
            if (flag)
            {
                this.globalAlpha = Calc.StepTowards(this.globalAlpha, 1f, Clock.DeltaTime * 6f);
                return;
            }
            this.globalAlpha = Calc.StepTowards(this.globalAlpha, 0f, Clock.DeltaTime * 6f);
        }

        private void DrawEntry(NextTurnsUI.Entry entry)
        {
            Rect rect = RectUtility.CenteredAt(entry.curPos.CurrentValue, 34f);
            if (!Widgets.ScreenRect.Overlaps(rect))
            {
                return;
            }
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            Get.Tooltips.RegisterTip(rect, entry.actor, null, null);
            GUI.color = entry.actor.IconColor.WithAlphaFactor(entry.curAlpha.CurrentValue * this.globalAlpha);
            GUI.DrawTexture(rect, entry.actor.Icon);
            GUI.color = Color.white;
            if (entry.actor.ConditionsAccumulated.AnyOfSpec(Get.Condition_Sleeping))
            {
                GUI.color = new Color(1f, 1f, 1f, entry.curAlpha.CurrentValue * this.globalAlpha);
                GUI.DrawTexture(rect.ContractedByPct(0.1f), Get.Condition_Sleeping.Icon);
                GUI.color = Color.white;
            }
            if (Get.InteractionManager.PointedAtEntity == entry.actor || Get.SeenEntitiesDrawer.MouseoverEntity == entry.actor)
            {
                GUIExtra.DrawRectOutline(rect.ContractedBy(1f), SeenEntitiesDrawer.PointedAtEntityOutlineColor.WithAlphaFactor(entry.curAlpha.CurrentValue * this.globalAlpha), 2f);
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < this.entries.Count; i++)
            {
                NextTurnsUI.Entry entry = this.entries[i];
                entry.curPos.SetTarget(Vector2.Lerp(entry.curPos.Target, this.GetTargetPos(i), 0.2f));
                entry.curAlpha.SetTarget(Calc.Lerp(entry.curAlpha.Target, this.GetTargetAlpha(i), 0.2f));
                this.entries[i] = entry;
            }
            for (int j = 0; j < this.fadingOutEntries.Count; j++)
            {
                NextTurnsUI.Entry entry2 = this.fadingOutEntries[j];
                entry2.curPos.SetTarget(Vector2.Lerp(entry2.curPos.Target, entry2.posStartedFadingOut.WithAddedY(30f), 0.2f));
                entry2.curAlpha.SetTarget(Calc.Lerp(entry2.curAlpha.Target, 0f, 0.2f));
                this.fadingOutEntries[j] = entry2;
            }
            for (int k = this.fadingOutEntries.Count - 1; k >= 0; k--)
            {
                if (this.fadingOutEntries[k].curAlpha.CurrentValue < 0.001f)
                {
                    this.fadingOutEntries.RemoveAt(k);
                }
            }
        }

        private void CheckUpdateEntries()
        {
            this.CalculateNextActorTurns();
            if (this.nextActorTurns.Count == 0)
            {
                for (int i = this.entries.Count - 1; i >= 0; i--)
                {
                    NextTurnsUI.Entry entry = this.entries[i];
                    entry.posStartedFadingOut = entry.curPos.CurrentValue;
                    this.fadingOutEntries.Add(entry);
                    this.entries.RemoveAt(i);
                }
                return;
            }
            if (this.entries.Count == 0)
            {
                for (int j = 0; j < this.nextActorTurns.Count; j++)
                {
                    this.entries.Add(new NextTurnsUI.Entry
                    {
                        curPos = new InterpolatedVector2(this.GetTargetPos(j).WithAddedY(-30f)),
                        curAlpha = new InterpolatedFloat(0f),
                        actor = this.nextActorTurns[j]
                    });
                }
                return;
            }
            if (this.nextActorTurns[0] != this.entries[0].actor || (this.entries.Count >= 2 && this.nextActorTurns[1] != this.entries[1].actor))
            {
                int num = 1;
                while (num <= 4 && this.entries.Count > num)
                {
                    bool flag = true;
                    for (int k = 0; k < this.entries.Count - num; k++)
                    {
                        if (this.nextActorTurns[k] != this.entries[k + num].actor)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        for (int l = 0; l < num; l++)
                        {
                            if (this.entries.Count == 0)
                            {
                                break;
                            }
                            this.entries.RemoveAt(0);
                        }
                        break;
                    }
                    num++;
                }
            }
            int m = 0;
            while (m < 13)
            {
                if (m >= this.entries.Count)
                {
                    goto IL_027E;
                }
                if (this.entries[m].actor != this.nextActorTurns[m])
                {
                    if (m != 0)
                    {
                        NextTurnsUI.Entry entry2 = this.entries[m];
                        entry2.posStartedFadingOut = entry2.curPos.CurrentValue;
                        this.fadingOutEntries.Add(entry2);
                    }
                    this.entries.RemoveAt(m);
                    if (m >= this.entries.Count || this.entries[m].actor != this.nextActorTurns[m])
                    {
                        goto IL_027E;
                    }
                }
            IL_02F9:
                m++;
                continue;
            IL_027E:
                NextTurnsUI.Entry entry3 = new NextTurnsUI.Entry
                {
                    curPos = new InterpolatedVector2(this.GetTargetPos(m).WithAddedY(-30f)),
                    curAlpha = new InterpolatedFloat(0f),
                    actor = this.nextActorTurns[m]
                };
                if (m < this.entries.Count)
                {
                    this.entries.Insert(m, entry3);
                    goto IL_02F9;
                }
                this.entries.Add(entry3);
                goto IL_02F9;
            }
            for (int n = this.entries.Count - 1; n >= 13; n--)
            {
                NextTurnsUI.Entry entry4 = this.entries[n];
                entry4.posStartedFadingOut = entry4.curPos.CurrentValue;
                this.fadingOutEntries.Add(entry4);
                this.entries.RemoveAt(n);
            }
        }

        private void CalculateNextActorTurns()
        {
            this.tmpActors.Clear();
            this.nextActorTurns.Clear();
            this.tmpSeenActors.Clear();
            this.tmpSeenActors.AddRange(Get.VisibilityCache.ActorsSeen_Ordered);
            foreach (Actor actor in Get.PlayerParty)
            {
                if (!this.tmpSeenActors.Contains(actor) && actor.Spawned)
                {
                    this.tmpSeenActors.Add(actor);
                }
            }
            for (int i = 0; i < this.tmpSeenActors.Count; i++)
            {
                this.tmpActors.Add(new ValueTuple<Actor, int, int>(this.tmpSeenActors[i], this.tmpSeenActors[i].Sequence, Get.TurnManager.SequenceablesInOrder.IndexOf(this.tmpSeenActors[i])));
            }
            if (this.tmpActors.Count == 0)
            {
                return;
            }
            for (int j = 0; j < 13; j++)
            {
                ValueTuple<Actor, int, int> valueTuple = this.tmpActors[0];
                for (int k = 1; k < this.tmpActors.Count; k++)
                {
                    if (this.tmpActors[k].Item2 < valueTuple.Item2 || (this.tmpActors[k].Item2 == valueTuple.Item2 && this.tmpActors[k].Item3 < valueTuple.Item3))
                    {
                        valueTuple = this.tmpActors[k];
                    }
                }
                this.nextActorTurns.Add(valueTuple.Item1);
                for (int l = 0; l < this.tmpActors.Count; l++)
                {
                    ValueTuple<Actor, int, int> valueTuple2 = this.tmpActors[l];
                    if (valueTuple2.Item1 == valueTuple.Item1)
                    {
                        this.tmpActors[l] = new ValueTuple<Actor, int, int>(valueTuple2.Item1, valueTuple2.Item2 + valueTuple2.Item1.SequencePerTurn, valueTuple2.Item3);
                        break;
                    }
                }
            }
        }

        public void OnActorTookTurn(Actor actor)
        {
            if (this.entries.Count != 0 && this.entries[0].actor == actor && Get.TurnManager.GetSequenceableWithLowestSequence(false) != actor)
            {
                this.entries.RemoveAt(0);
            }
        }

        public void OnEntityDeSpawned(Entity entity)
        {
            Actor actor = entity as Actor;
            if (actor != null)
            {
                for (int i = this.entries.Count - 1; i >= 0; i--)
                {
                    if (this.entries[i].actor == actor)
                    {
                        NextTurnsUI.Entry entry = this.entries[i];
                        entry.posStartedFadingOut = entry.curPos.CurrentValue;
                        this.fadingOutEntries.Add(entry);
                        this.entries.RemoveAt(i);
                    }
                }
            }
        }

        public void OnRewindToRewindPoint()
        {
            this.CheckUpdateEntries();
            this.fadingOutEntries.Clear();
            for (int i = 0; i < this.entries.Count; i++)
            {
                NextTurnsUI.Entry entry = this.entries[i];
                entry.curPos = new InterpolatedVector2(this.GetTargetPos(i).MovedBy(-34f, 0f));
                entry.curAlpha = new InterpolatedFloat(this.GetTargetAlpha(i));
                this.entries[i] = entry;
            }
        }

        private float GetTargetAlpha(int index)
        {
            return Calc.Clamp(1f - (float)index / 8f, 0.15f, 1f);
        }

        private Vector2 GetTargetPos(int index)
        {
            return new Vector2(this.StartX + 39f * (float)index + 17f, this.StartY + 17f);
        }

        private List<NextTurnsUI.Entry> entries = new List<NextTurnsUI.Entry>();

        private List<NextTurnsUI.Entry> fadingOutEntries = new List<NextTurnsUI.Entry>();

        private List<Actor> nextActorTurns = new List<Actor>();

        private float globalAlpha;

        private int lastFrameDisplayed = -99999;

        private const float RightMargin = 10f;

        private const float BottomMargin = 10f;

        private const int RowEntriesCount_Visible = 8;

        private const int RowEntriesCount_Simulated = 13;

        private const float EntrySize = 34f;

        private const float SpaceBetweenEntries = 5f;

        private const float AnimationYOffset = 30f;

        private List<ValueTuple<Actor, int, int>> tmpActors = new List<ValueTuple<Actor, int, int>>();

        private List<Actor> tmpSeenActors = new List<Actor>();

        private struct Entry
        {
            public InterpolatedVector2 curPos;

            public InterpolatedFloat curAlpha;

            public Actor actor;

            public Vector2 posStartedFadingOut;
        }
    }
}