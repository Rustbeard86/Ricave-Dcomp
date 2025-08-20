using System;
using System.Runtime.CompilerServices;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public static class BodyPartUtility
    {
        public static Vector2 RaycastHitPointToUV(Actor actor, Vector3 raycastHit)
        {
            GameObject gameObject = actor.GameObject;
            if (gameObject == null)
            {
                return default(Vector2);
            }
            Vector3 vector = raycastHit - gameObject.transform.position;
            return new Vector2(Calc.Clamp01(Vector3.Dot(gameObject.transform.right, vector) / gameObject.transform.localScale.x + 0.5f), Calc.Clamp01(Vector3.Dot(gameObject.transform.up, vector) / gameObject.transform.localScale.y + 0.5f));
        }

        public static BodyPart GetBodyPartFromRaycastHit(Actor actor, GameObject hitGameObject, Vector3 raycastHit, bool allowMissing = false)
        {
            if (actor.BodyParts.Count == 0)
            {
                return null;
            }
            if (actor.Spec.Actor.Texture == null)
            {
                ActorGOC actorGOC = actor.ActorGOC;
                if (actorGOC == null)
                {
                    return null;
                }
                return actorGOC.GetBodyPart(hitGameObject, allowMissing);
            }
            else
            {
                if (actor.Spec.Actor.BodyMap == null)
                {
                    return null;
                }
                Vector2 vector = BodyPartUtility.RaycastHitPointToUV(actor, raycastHit);
                BodyPart bodyPart;
                if (BodyPartUtility.TryGetBodyPartAt(actor, vector, out bodyPart, allowMissing) || BodyPartUtility.TryGetBodyPartAt(actor, vector + new Vector2(0.009765625f, 0f), out bodyPart, allowMissing) || BodyPartUtility.TryGetBodyPartAt(actor, vector + new Vector2(-0.009765625f, 0f), out bodyPart, allowMissing) || BodyPartUtility.TryGetBodyPartAt(actor, vector + new Vector2(0f, 0.009765625f), out bodyPart, allowMissing) || BodyPartUtility.TryGetBodyPartAt(actor, vector + new Vector2(0f, -0.009765625f), out bodyPart, allowMissing) || BodyPartUtility.TryGetBodyPartAt(actor, vector + new Vector2(0.0390625f, 0f), out bodyPart, allowMissing) || BodyPartUtility.TryGetBodyPartAt(actor, vector + new Vector2(-0.0390625f, 0f), out bodyPart, allowMissing) || BodyPartUtility.TryGetBodyPartAt(actor, vector + new Vector2(0f, 0.0390625f), out bodyPart, allowMissing) || BodyPartUtility.TryGetBodyPartAt(actor, vector + new Vector2(0f, -0.0390625f), out bodyPart, allowMissing))
                {
                    return bodyPart;
                }
                return null;
            }
        }

        public static bool DidRaycastHitActor(Actor actor, Vector3 raycastHit, bool allowMissingParts = true)
        {
            BodyPartUtility.<> c__DisplayClass5_0 CS$<> 8__locals1;
            CS$<> 8__locals1.texture = actor.Spec.Actor.Texture;
            if (CS$<> 8__locals1.texture == null)
			{
                return true;
            }
            Vector2 vector = BodyPartUtility.RaycastHitPointToUV(actor, raycastHit);
            return vector.x >= 0f && vector.y >= 0f && vector.x <= 1f && vector.y <= 1f && (BodyPartUtility.< DidRaycastHitActor > g__IsNonTransparentPixelAt | 5_0(vector, ref CS$<> 8__locals1) || (BodyPartUtility.< DidRaycastHitActor > g__IsNonTransparentPixelAt | 5_0(vector + new Vector2(0.01953125f, 0f), ref CS$<> 8__locals1) && BodyPartUtility.< DidRaycastHitActor > g__IsNonTransparentPixelAt | 5_0(vector + new Vector2(-0.01953125f, 0f), ref CS$<> 8__locals1)) || (BodyPartUtility.< DidRaycastHitActor > g__IsNonTransparentPixelAt | 5_0(vector + new Vector2(0f, 0.01953125f), ref CS$<> 8__locals1) && BodyPartUtility.< DidRaycastHitActor > g__IsNonTransparentPixelAt | 5_0(vector + new Vector2(0f, -0.01953125f), ref CS$<> 8__locals1)) || (BodyPartUtility.< DidRaycastHitActor > g__IsNonTransparentPixelAt | 5_0(vector + new Vector2(-0.01953125f, -0.01953125f), ref CS$<> 8__locals1) && BodyPartUtility.< DidRaycastHitActor > g__IsNonTransparentPixelAt | 5_0(vector + new Vector2(0.01953125f, 0.01953125f), ref CS$<> 8__locals1)) || (BodyPartUtility.< DidRaycastHitActor > g__IsNonTransparentPixelAt | 5_0(vector + new Vector2(0.01953125f, -0.01953125f), ref CS$<> 8__locals1) && BodyPartUtility.< DidRaycastHitActor > g__IsNonTransparentPixelAt | 5_0(vector + new Vector2(-0.01953125f, 0.01953125f), ref CS$<> 8__locals1)));
        }

        public static Color GetBodyPartBodyMapColor(BodyPart bodyPart)
        {
            if (bodyPart == null)
            {
                return BodyPartUtility.ColorToIndex[0];
            }
            int num = bodyPart.Actor.BodyParts.IndexOf(bodyPart);
            if (num < 0)
            {
                Log.Error("Called GetBodyPartBodyMapColor() but this body part doesn't belong to this actor.", false);
                return BodyPartUtility.ColorToIndex[0];
            }
            return BodyPartUtility.ColorToIndex[num + 1];
        }

        private static bool TryGetBodyPartAt(Actor actor, Vector2 uv, out BodyPart bodyPart, bool allowMissing = false)
        {
            int num;
            if (!BodyPartUtility.TryGetBodyPartIndexAt(actor.Spec.Actor.BodyMap, uv, out num))
            {
                bodyPart = null;
                return false;
            }
            if (num < 0)
            {
                bodyPart = null;
                return true;
            }
            if (num >= actor.BodyParts.Count)
            {
                Log.Error(string.Concat(new string[]
                {
                    "Body map for ",
                    (actor != null) ? actor.ToString() : null,
                    " gave body part index ",
                    num.ToString(),
                    " which is out of bounds for this actor."
                }), true);
                bodyPart = null;
                return false;
            }
            if (!allowMissing && actor.BodyParts[num].IsMissing)
            {
                bodyPart = null;
                return false;
            }
            bodyPart = actor.BodyParts[num];
            return true;
        }

        private static bool TryGetBodyPartIndexAt(Texture2D bodyMap, Vector2 uv, out int bodyPartIndex)
        {
            int num = Calc.Clamp((int)((float)bodyMap.width * uv.x), 0, bodyMap.width - 1);
            int num2 = Calc.Clamp((int)((float)bodyMap.height * uv.y), 0, bodyMap.height - 1);
            Color pixel = bodyMap.GetPixel(num, num2);
            if (pixel.a < 0.5f)
            {
                bodyPartIndex = -1;
                return false;
            }
            pixel.a = 1f;
            for (int i = 0; i < BodyPartUtility.ColorToIndex.Length; i++)
            {
                if (pixel == BodyPartUtility.ColorToIndex[i])
                {
                    bodyPartIndex = i - 1;
                    return true;
                }
            }
            bodyPartIndex = -1;
            return false;
        }

        [CompilerGenerated]
        internal static bool <DidRaycastHitActor>g__IsNonTransparentPixelAt|5_0(Vector2 uv, ref BodyPartUtility.<>c__DisplayClass5_0 A_1)
		{
            int num = (int)((float)A_1.texture.width * uv.x);
        int num2 = (int)((float)A_1.texture.height * uv.y);
			return num >= 0 && num2 >= 0 && num<A_1.texture.width && num2<A_1.texture.height && A_1.texture.GetPixel(num, num2).a > 0.05f;
		}

		public const int MaxBodyParts = 7;

        public const int MaxBodyPartsIncludingNoPart = 8;

        public static readonly Color32[] ColorToIndex = new Color32[]
        {
            new Color32(0, 0, 0, byte.MaxValue),
            new Color32(byte.MaxValue, 0, 0, byte.MaxValue),
            new Color32(0, byte.MaxValue, 0, byte.MaxValue),
            new Color32(0, 0, byte.MaxValue, byte.MaxValue),
            new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue),
            new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue),
            new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue),
            new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)
        };
    }
}