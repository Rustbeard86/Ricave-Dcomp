using System;
using Ricave.Core;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Ricave.Rendering
{
    public class MainSceneObjects
    {
        public RootGOC RootGOC
        {
            get
            {
                return this.rootGOC;
            }
        }

        public FPPControllerGOC FPPControllerGOC
        {
            get
            {
                return this.fppControllerGOC;
            }
        }

        public FancyOutlineGOC FancyOutlineGOC
        {
            get
            {
                return this.fancyOutlineGOC;
            }
        }

        public MouseAttachmentDrawerGOC MouseAttachmentDrawerGOC
        {
            get
            {
                return this.mouseAttachmentDrawerGOC;
            }
        }

        public LessonDrawerGOC LessonDrawerGOC
        {
            get
            {
                return this.lessonDrawerGOC;
            }
        }

        public Camera Camera
        {
            get
            {
                return this.camera;
            }
        }

        public Camera FPPItemCamera
        {
            get
            {
                return this.fppItemCamera;
            }
        }

        public Camera HighlightCamera
        {
            get
            {
                return this.highlightCamera;
            }
        }

        public GameObject RuntimeSpecialContainer
        {
            get
            {
                return this.runtimeSpecialContainer;
            }
        }

        public GameObject RuntimeSectionsContainer
        {
            get
            {
                return this.runtimeSectionsContainer;
            }
        }

        public GameObject AudioSourceContainer
        {
            get
            {
                return this.audioSourceContainer;
            }
        }

        public GameObject CameraOffset
        {
            get
            {
                return this.cameraOffset;
            }
        }

        public GameObject PlayerGravity
        {
            get
            {
                return this.playerGravity;
            }
        }

        public ScreenFaderGOC ScreenFader
        {
            get
            {
                return this.screenFader;
            }
        }

        public BlackBarsGOC BlackBars
        {
            get
            {
                return this.blackBars;
            }
        }

        public TextSequenceDrawerGOC TextSequenceDrawer
        {
            get
            {
                return this.textSequenceDrawer;
            }
        }

        public DungeonMapDrawerGOC DungeonMapDrawer
        {
            get
            {
                return this.dungeonMapDrawer;
            }
        }

        public VignetteAndChromaticAberration Vignette
        {
            get
            {
                return this.vignette;
            }
        }

        public BlurOptimized Blur
        {
            get
            {
                return this.blur;
            }
        }

        public ColorCorrectionCurves Grayscale
        {
            get
            {
                return this.grayscale;
            }
        }

        public MotionBlur MotionBlur
        {
            get
            {
                return this.motionBlur;
            }
        }

        public ParticleSystem HighSpeedParticles
        {
            get
            {
                return this.highSpeedParticles;
            }
        }

        public DevConsoleGOC DevConsole
        {
            get
            {
                return this.devConsole;
            }
        }

        public MainSceneObjects()
        {
            this.Assign();
        }

        public void OnSceneChanged()
        {
            this.Assign();
        }

        private void Assign()
        {
            this.rootGOC = GameObject.Find("Root").GetComponent<RootGOC>();
            GameObject gameObject = GameObject.Find("FPPController");
            this.fppControllerGOC = ((gameObject == null) ? null : gameObject.GetComponent<FPPControllerGOC>());
            this.mouseAttachmentDrawerGOC = GameObject.Find("MouseAttachment").GetComponent<MouseAttachmentDrawerGOC>();
            this.mouseAttachmentDrawerGOC.gameObject.SetActive(false);
            this.lessonDrawerGOC = GameObject.Find("LessonDrawer").GetComponent<LessonDrawerGOC>();
            this.lessonDrawerGOC.gameObject.SetActive(false);
            this.camera = Camera.main;
            Get.CacheReferences();
            GameObject gameObject2 = GameObject.Find("FPPItemCamera");
            this.fppItemCamera = ((gameObject2 == null) ? null : gameObject2.GetComponent<Camera>());
            if (gameObject2 != null)
            {
                gameObject2.SetActive(false);
            }
            GameObject gameObject3 = GameObject.Find("HighlightCamera");
            this.highlightCamera = gameObject3.GetComponent<Camera>();
            this.fancyOutlineGOC = gameObject3.GetComponent<FancyOutlineGOC>();
            gameObject3.SetActive(false);
            this.runtimeSpecialContainer = GameObject.Find("Runtime_Special");
            this.runtimeSectionsContainer = GameObject.Find("Runtime_Sections");
            this.audioSourceContainer = GameObject.Find("Runtime_AudioSource");
            this.cameraOffset = GameObject.Find("CameraOffset");
            this.playerGravity = GameObject.Find("PlayerGravity");
            GameObject gameObject4 = GameObject.Find("ScreenFader");
            this.screenFader = ((gameObject4 != null) ? gameObject4.GetComponent<ScreenFaderGOC>() : null);
            GameObject gameObject5 = GameObject.Find("BlackBars");
            this.blackBars = ((gameObject5 != null) ? gameObject5.GetComponent<BlackBarsGOC>() : null);
            if (this.blackBars != null)
            {
                this.blackBars.gameObject.SetActive(false);
            }
            GameObject gameObject6 = GameObject.Find("TextSequenceDrawer");
            this.textSequenceDrawer = ((gameObject6 != null) ? gameObject6.GetComponent<TextSequenceDrawerGOC>() : null);
            this.textSequenceDrawer.gameObject.SetActive(false);
            GameObject gameObject7 = GameObject.Find("DungeonMapDrawer");
            this.dungeonMapDrawer = ((gameObject7 != null) ? gameObject7.GetComponent<DungeonMapDrawerGOC>() : null);
            this.dungeonMapDrawer.gameObject.SetActive(false);
            GameObject gameObject8 = GameObject.Find("DevConsole");
            this.devConsole = ((gameObject8 == null) ? null : gameObject8.GetComponent<DevConsoleGOC>());
            this.vignette = this.camera.GetComponent<VignetteAndChromaticAberration>();
            this.blur = this.camera.GetComponent<BlurOptimized>();
            this.grayscale = this.camera.GetComponent<ColorCorrectionCurves>();
            this.motionBlur = this.camera.GetComponent<MotionBlur>();
            GameObject gameObject9 = GameObject.Find("HighSpeed");
            this.highSpeedParticles = ((gameObject9 != null) ? gameObject9.GetComponent<ParticleSystem>() : null);
        }

        private RootGOC rootGOC;

        private FPPControllerGOC fppControllerGOC;

        private FancyOutlineGOC fancyOutlineGOC;

        private MouseAttachmentDrawerGOC mouseAttachmentDrawerGOC;

        private LessonDrawerGOC lessonDrawerGOC;

        private Camera camera;

        private Camera fppItemCamera;

        private Camera highlightCamera;

        private GameObject runtimeSpecialContainer;

        private GameObject runtimeSectionsContainer;

        private GameObject audioSourceContainer;

        private GameObject cameraOffset;

        private GameObject playerGravity;

        private ScreenFaderGOC screenFader;

        private BlackBarsGOC blackBars;

        private TextSequenceDrawerGOC textSequenceDrawer;

        private DungeonMapDrawerGOC dungeonMapDrawer;

        private VignetteAndChromaticAberration vignette;

        private BlurOptimized blur;

        private ColorCorrectionCurves grayscale;

        private MotionBlur motionBlur;

        private ParticleSystem highSpeedParticles;

        private DevConsoleGOC devConsole;

        public const string PlaySceneName = "Play";

        public const string MainMenuSceneName = "MainMenu";
    }
}