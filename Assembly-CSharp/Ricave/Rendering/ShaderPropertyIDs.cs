using System;
using UnityEngine;

namespace Ricave.Rendering
{
    public class ShaderPropertyIDs
    {
        public int ColorID
        {
            get
            {
                return this.colorID;
            }
        }

        public int FaceColorID
        {
            get
            {
                return this.faceColorID;
            }
        }

        public int OutlineColorID
        {
            get
            {
                return this.outlineColorID;
            }
        }

        public int EmissionColorID
        {
            get
            {
                return this.emissionColorID;
            }
        }

        public int BodyMapID
        {
            get
            {
                return this.bodyMapID;
            }
        }

        public int[] PartXExistsID
        {
            get
            {
                return this.partXExistsID;
            }
        }

        public int BodyPartBodyMapColorID
        {
            get
            {
                return this.bodyPartBodyMapColorID;
            }
        }

        public int LightningStrikeID
        {
            get
            {
                return this.lightningStrikeID;
            }
        }

        public int SampleStepID
        {
            get
            {
                return this.sampleStepID;
            }
        }

        public int IterationsCountID
        {
            get
            {
                return this.iterationsCountID;
            }
        }

        public int MaxThicknessInPixelsID
        {
            get
            {
                return this.maxThicknessInPixelsID;
            }
        }

        public int SceneTexID
        {
            get
            {
                return this.sceneTexID;
            }
        }

        public int MetallicID
        {
            get
            {
                return this.metallicID;
            }
        }

        public int GlossinessID
        {
            get
            {
                return this.glossinessID;
            }
        }

        public int CustomFogColorID
        {
            get
            {
                return this.customFogColorID;
            }
        }

        public int CustomFogIntensityID
        {
            get
            {
                return this.customFogIntensityID;
            }
        }

        public int SkyColorID
        {
            get
            {
                return this.skyColorID;
            }
        }

        public int OffsetID
        {
            get
            {
                return this.offsetID;
            }
        }

        public int ScaleID
        {
            get
            {
                return this.scaleID;
            }
        }

        public int StartAngleID
        {
            get
            {
                return this.startAngleID;
            }
        }

        public int AngleSpanID
        {
            get
            {
                return this.angleSpanID;
            }
        }

        public int InnerRadiusPctID
        {
            get
            {
                return this.innerRadiusPctID;
            }
        }

        public void Init()
        {
            this.colorID = Shader.PropertyToID("_Color");
            this.faceColorID = Shader.PropertyToID("_FaceColor");
            this.outlineColorID = Shader.PropertyToID("_OutlineColor");
            this.emissionColorID = Shader.PropertyToID("_EmissionColor");
            this.bodyMapID = Shader.PropertyToID("_BodyMap");
            for (int i = 0; i < this.partXExistsID.Length; i++)
            {
                this.partXExistsID[i] = Shader.PropertyToID(string.Format("_Part{0}Exists", i + 1));
            }
            this.bodyPartBodyMapColorID = Shader.PropertyToID("_BodyPartBodyMapColor");
            this.lightningStrikeID = Shader.PropertyToID("_LightningStrike");
            this.sampleStepID = Shader.PropertyToID("_SampleStep");
            this.iterationsCountID = Shader.PropertyToID("_IterationsCount");
            this.maxThicknessInPixelsID = Shader.PropertyToID("_MaxThicknessInPixels");
            this.sceneTexID = Shader.PropertyToID("_SceneTex");
            this.metallicID = Shader.PropertyToID("_Metallic");
            this.glossinessID = Shader.PropertyToID("_Glossiness");
            this.customFogColorID = Shader.PropertyToID("_CustomFogColor");
            this.customFogIntensityID = Shader.PropertyToID("_CustomFogIntensity");
            this.skyColorID = Shader.PropertyToID("_SkyColor");
            this.offsetID = Shader.PropertyToID("_Offset");
            this.scaleID = Shader.PropertyToID("_Scale");
            this.startAngleID = Shader.PropertyToID("_StartAngle");
            this.angleSpanID = Shader.PropertyToID("_AngleSpan");
            this.innerRadiusPctID = Shader.PropertyToID("_InnerRadiusPct");
        }

        private int colorID;

        private int faceColorID;

        private int outlineColorID;

        private int emissionColorID;

        private int bodyMapID;

        private int[] partXExistsID = new int[7];

        private int bodyPartBodyMapColorID;

        private int lightningStrikeID;

        private int sampleStepID;

        private int iterationsCountID;

        private int maxThicknessInPixelsID;

        private int sceneTexID;

        private int metallicID;

        private int glossinessID;

        private int customFogColorID;

        private int customFogIntensityID;

        private int skyColorID;

        private int offsetID;

        private int scaleID;

        private int startAngleID;

        private int angleSpanID;

        private int innerRadiusPctID;
    }
}