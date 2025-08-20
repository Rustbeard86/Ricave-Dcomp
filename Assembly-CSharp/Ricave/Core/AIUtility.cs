using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class AIUtility
    {
        public static IEnumerable<Actor> GetJustSeenHostiles(Actor by)
        {
            List<Actor> actors = Get.World.Actors;
            int num;
            for (int i = 0; i < actors.Count; i = num + 1)
            {
                if (actors[i] != by && by.IsHostile(actors[i]) && AIUtility.SeesOrJustSeen(by, actors[i]))
                {
                    yield return actors[i];
                }
                num = i;
            }
            yield break;
        }

        public static IEnumerable<Actor> GetJustSeenAllies(Actor by)
        {
            if (by.Faction == null)
            {
                yield break;
            }
            List<Actor> actors = Get.World.Actors;
            int num;
            for (int i = 0; i < actors.Count; i = num + 1)
            {
                if (actors[i] != by && actors[i].Faction == by.Faction && !by.IsHostile(actors[i]) && AIUtility.SeesOrJustSeen(by, actors[i]))
                {
                    yield return actors[i];
                }
                num = i;
            }
            yield break;
        }

        public static bool SeesOrJustSeen(Actor finder, Actor seen)
        {
            return finder.Sees(seen, null) || finder.AIMemory.AwareOfPost.Contains(seen);
        }

        public static bool AnyAdjacentHostileActorCanTouchMeAt(Vector3Int pos, Actor forActor)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsAndInside[i];
                if (vector3Int.InBounds())
                {
                    List<Entity> entitiesAt = Get.World.GetEntitiesAt(vector3Int);
                    for (int j = 0; j < entitiesAt.Count; j++)
                    {
                        Actor actor = entitiesAt[j] as Actor;
                        if (actor != null && actor != forActor && actor.IsHostile(forActor) && Get.World.CanTouch(vector3Int, pos, actor))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool AnyAdjacentHostileActorCanTouchMeAndCanAttackMeBeforeMyTurnAt(Vector3Int pos, Actor forActor)
        {
            for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
            {
                Vector3Int vector3Int = pos + Vector3IntUtility.DirectionsAndInside[i];
                if (vector3Int.InBounds())
                {
                    List<Entity> entitiesAt = Get.World.GetEntitiesAt(vector3Int);
                    for (int j = 0; j < entitiesAt.Count; j++)
                    {
                        Actor actor = entitiesAt[j] as Actor;
                        if (actor != null && actor != forActor && actor.IsHostile(forActor) && Get.World.CanTouch(vector3Int, pos, actor) && AIUtility.WillMakeMoveBeforeMe(actor, forActor))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool WillMakeMoveBeforeMe(Actor other, Actor me)
        {
            return Get.TurnManager.HasSequencePriorityOver(other.Sequence, Get.TurnManager.GetSequenceableOrder(other), me.Sequence + me.SequencePerTurn, Get.TurnManager.GetSequenceableOrder(me));
        }

        public static bool WillMakeAtLeastTwoMovesBeforeMe(Actor other, Actor me)
        {
            return Get.TurnManager.HasSequencePriorityOver(other.Sequence + other.SequencePerTurn, Get.TurnManager.GetSequenceableOrder(other), me.Sequence + me.SequencePerTurn, Get.TurnManager.GetSequenceableOrder(me));
        }

        public static int CountMovesBeforeMe(Actor other, Actor me)
        {
            TurnManager turnManager = Get.TurnManager;
            int sequence = other.Sequence;
            int sequencePerTurn = other.SequencePerTurn;
            int sequenceableOrder = Get.TurnManager.GetSequenceableOrder(other);
            int sequence2 = me.Sequence;
            int sequencePerTurn2 = me.SequencePerTurn;
            int sequenceableOrder2 = Get.TurnManager.GetSequenceableOrder(me);
            int num = 0;
            for (int i = 0; i < 100; i++)
            {
                if (!turnManager.HasSequencePriorityOver(sequence + sequencePerTurn * i, sequenceableOrder, sequence2 + sequencePerTurn2, sequenceableOrder2))
                {
                    return num;
                }
                num = i + 1;
            }
            Log.Warning("CountMovesBeforeMe() reached its iterations limit.", true);
            return num;
        }

        public static Texture2D GetIcon(this PartyMemberAIMode mode)
        {
            if (mode == PartyMemberAIMode.Follow)
            {
                return AIUtility.FollowAIIcon;
            }
            if (mode != PartyMemberAIMode.Stay)
            {
                return null;
            }
            return AIUtility.StayAIIcon;
        }

        private static readonly Texture2D FollowAIIcon = Assets.Get<Texture2D>("Textures/UI/PartyMemberAIMode/Follow");

        private static readonly Texture2D StayAIIcon = Assets.Get<Texture2D>("Textures/UI/PartyMemberAIMode/Stay");
    }
}