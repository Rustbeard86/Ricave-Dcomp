using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class LightComp : EntityComp
    {
        public new LightCompProps Props
        {
            get
            {
                return (LightCompProps)base.Props;
            }
        }

        private Color AnimatedColor
        {
            get
            {
                Color color = (this.Props.RandomColor ? LightComp.RandomColors[base.Parent.MyStableHash % LightComp.RandomColors.Length] : this.Props.Color);
                if (this.Props.Flicker)
                {
                    return color.MultipliedColor(Calc.Lerp(0.8f, 1.2f, Noise.PerlinNoise(8f + Clock.Time * 1.7f, 13f + (float)base.Parent.InstanceID * 99f)));
                }
                return color;
            }
        }

        private Vector3 AnimatedPosition
        {
            get
            {
                Actor actor = base.Parent as Actor;
                Vector3 vector;
                if (actor != null && actor.IsNowControlledActor)
                {
                    vector = Get.CameraPosition;
                }
                else if ((!this.Props.CastShadows || base.Parent is Actor) && base.Parent.GameObject != null)
                {
                    vector = base.Parent.GameObject.transform.TransformPoint(this.Props.PositionOffset);
                }
                else
                {
                    vector = base.Parent.Position + base.Parent.Rotation * this.Props.PositionOffset;
                }
                return vector;
            }
        }

        private Quaternion AnimatedRotation
        {
            get
            {
                if (this.Props.RotationSpeed == 0f)
                {
                    return Quaternion.identity;
                }
                return Quaternion.Euler(0f, this.Props.RotationSpeed * Clock.Time % 360f, 0f);
            }
        }

        protected LightComp()
        {
        }

        public LightComp(Entity parent)
            : base(parent)
        {
        }

        public override void Update()
        {
            base.Update();
            this.lightGOC.gameObject.transform.SetPositionAndRotation(this.AnimatedPosition, this.AnimatedRotation);
            this.lightGOC.color = this.AnimatedColor;
        }

        public override void OnSpawned()
        {
            base.OnSpawned();
            this.CreateLightGOC();
            Get.LightManager.RegisterLight(this.lightGOC, () => this.AnimatedColor, () => this.AnimatedPosition, () => this.AnimatedRotation, base.Parent, this.Props.OnlyIfEntityVisible, this.Props.FadeOutOnEntityDespawned);
        }

        public override void OnDeSpawned()
        {
            base.OnDeSpawned();
            Get.LightManager.DeregisterLight(this.lightGOC);
            this.lightGOC = null;
        }

        public override void OnSetGameObjectAfterLoading()
        {
            base.OnSetGameObjectAfterLoading();
            this.CreateLightGOC();
            Get.LightManager.RegisterLight(this.lightGOC, () => this.AnimatedColor, () => this.AnimatedPosition, () => this.AnimatedRotation, base.Parent, this.Props.OnlyIfEntityVisible, this.Props.FadeOutOnEntityDespawned);
        }

        private void CreateLightGOC()
        {
            if (this.lightGOC != null)
            {
                Log.Error("Called CreateLightGOC() but the light GOC is already created.", false);
                return;
            }
            GameObject gameObject = new GameObject();
            gameObject.name = "Light";
            gameObject.transform.SetParent(Get.RuntimeSpecialContainer.transform, false);
            gameObject.transform.SetPositionAndRotation(this.AnimatedPosition, this.AnimatedRotation);
            this.lightGOC = gameObject.AddComponent<Light>();
            this.lightGOC.type = this.Props.LightType;
            this.lightGOC.range = this.Props.Range;
            this.lightGOC.color = this.AnimatedColor;
            this.lightGOC.intensity = this.Props.Intensity;
            this.lightGOC.bounceIntensity = this.Props.BounceIntensity;
            this.lightGOC.shadows = this.Props.ShadowType;
            this.lightGOC.cookie = this.Props.CookieTexture;
            this.lightGOC.shadows = (this.Props.CastShadows ? LightShadows.Soft : LightShadows.None);
        }

        private Light lightGOC;

        private static readonly Color[] RandomColors = new Color[]
        {
            new Color(0.8f, 0.35f, 0.7f),
            new Color(0.31f, 0.77f, 0.38f),
            new Color(0.31f, 0.77f, 0.76f),
            new Color(0.31f, 0.31f, 0.77f),
            new Color(0.77f, 0.57f, 0.31f)
        };
    }
}