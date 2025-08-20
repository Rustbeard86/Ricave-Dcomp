using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class UI
    {
        public WindowManager WindowManager
        {
            get
            {
                return this.windowManager;
            }
        }

        public Minimap Minimap
        {
            get
            {
                return this.minimap;
            }
        }

        public IconOverlayDrawer IconOverlayDrawer
        {
            get
            {
                return this.iconOverlayDrawer;
            }
        }

        public CrosshairDrawer CrosshairDrawer
        {
            get
            {
                return this.crosshairDrawer;
            }
        }

        public DragAndDrop DragAndDrop
        {
            get
            {
                return this.dragAndDrop;
            }
        }

        public StaticTextOverlays StaticTextOverlays
        {
            get
            {
                return this.staticTextOverlays;
            }
        }

        public FloatingTexts FloatingTexts
        {
            get
            {
                return this.floatingTexts;
            }
        }

        public Tooltips Tooltips
        {
            get
            {
                return this.tooltips;
            }
        }

        public SeenEntitiesDrawer SeenEntitiesDrawer
        {
            get
            {
                return this.seenEntitiesDrawer;
            }
        }

        public DebugUI DebugUI
        {
            get
            {
                return this.debugUI;
            }
        }

        public WheelSelector WheelSelector
        {
            get
            {
                return this.wheelSelector;
            }
        }

        public StrikeOverlays StrikeOverlays
        {
            get
            {
                return this.strikeOverlays;
            }
        }

        public UseOnTargetUI UseOnTargetUI
        {
            get
            {
                return this.useOnTargetUI;
            }
        }

        public ProgressBarDrawer ProgressBarDrawer
        {
            get
            {
                return this.progressBarDrawer;
            }
        }

        public QuestCompletedText QuestCompletedText
        {
            get
            {
                return this.questCompletedText;
            }
        }

        public MemoryPieceHintUI MemoryPieceHintUI
        {
            get
            {
                return this.memoryPieceHintUI;
            }
        }

        public WorldEventNotification WorldEventNotification
        {
            get
            {
                return this.worldEventNotification;
            }
        }

        public UnexploredRoomsDrawer UnexploredRoomsDrawer
        {
            get
            {
                return this.unexploredRoomsDrawer;
            }
        }

        public BossSlainText BossSlainText
        {
            get
            {
                return this.bossSlainText;
            }
        }

        public RewindUI RewindUI
        {
            get
            {
                return this.rewindUI;
            }
        }

        public NextTurnsUI NextTurnsUI
        {
            get
            {
                return this.nextTurnsUI;
            }
        }

        public TimeDrawer TimeDrawer
        {
            get
            {
                return this.timeDrawer;
            }
        }

        public RandomTip RandomTip
        {
            get
            {
                return this.randomTip;
            }
        }

        public PlayerHitMarkers PlayerHitMarkers
        {
            get
            {
                return this.playerHitMarkers;
            }
        }

        public ScoreGainEffects ScoreGainEffects
        {
            get
            {
                return this.scoreGainEffects;
            }
        }

        public DeathScreenDrawer DeathScreenDrawer
        {
            get
            {
                return this.deathScreenDrawer;
            }
        }

        public BossHPBar BossHPBar
        {
            get
            {
                return this.bossHPBar;
            }
        }

        public CollectedItemsDrawer CollectedItemsDrawer
        {
            get
            {
                return this.collectedItemsDrawer;
            }
        }

        public ActiveQuestsReadout ActiveQuestsReadout
        {
            get
            {
                return this.activeQuestsReadout;
            }
        }

        public HPBarOverlayDrawer HPBarOverlayDrawer
        {
            get
            {
                return this.hpBarOverlayDrawer;
            }
        }

        public PartyUI PartyUI
        {
            get
            {
                return this.partyUI;
            }
        }

        public NewImportantConditionsUI NewImportantConditionsUI
        {
            get
            {
                return this.newImportantConditionsUI;
            }
        }

        public bool InventoryOpen
        {
            get
            {
                return this.inventoryOpen;
            }
        }

        public bool WorldInputBlocked
        {
            get
            {
                return Root.ChangingScene || this.inventoryOpen || this.windowManager.AnyWindowOpen || this.IsWheelSelectorOpen || this.DeathScreenDrawer.ShouldShow || Get.DevConsole.IsOn || Get.ScreenFader.AnyActionQueued || Get.TextSequenceDrawer.Showing || Get.DungeonMapDrawer.Showing;
            }
        }

        public bool IsWheelSelectorOpen
        {
            get
            {
                return this.wheelSelector != null;
            }
        }

        public bool WantsMouseUnlocked
        {
            get
            {
                return this.inventoryOpen || this.windowManager.AnyWindowOpen || this.IsWheelSelectorOpen || Get.DevConsole.IsOn || Get.DungeonMapDrawer.Showing;
            }
        }

        public bool IsEscapeMenuOpen
        {
            get
            {
                return this.windowManager.IsOpen(Get.Window_EscapeMenu);
            }
        }

        public float LastTimeOpenedAutoOpenInUIModeWindows
        {
            get
            {
                return this.lastTimeOpenedAutoOpenInUIModeWindows;
            }
        }

        public float LastTimeClosedAutoOpenInUIModeWindows
        {
            get
            {
                return this.lastTimeClosedAutoOpenInUIModeWindows;
            }
        }

        public void Init()
        {
            this.CloseInventory(false);
            this.CloseWheelSelector(false);
            Mouse.SetCursorLocked(!this.WantsMouseUnlocked);
            this.minimap.Regenerate();
        }

        public void OnWorldAboutToRegenerate()
        {
            this.minimap.OnWorldAboutToRegenerate();
            this.iconOverlayDrawer.OnWorldAboutToRegenerate();
            this.floatingTexts.OnWorldAboutToRegenerate();
            this.seenEntitiesDrawer.OnWorldAboutToRegenerate();
            this.crosshairDrawer.Reset();
        }

        public void EarlyOnGUI()
        {
            try
            {
                Profiler.Begin("unexploredRoomsDrawer.OnGUI()");
                this.unexploredRoomsDrawer.OnGUI();
            }
            catch (Exception ex)
            {
                Log.Error("Error in unexploredRoomsDrawer.OnGUI().", ex);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.Repaint && this.showWelcomeTextsOnFrame == Clock.Frame)
            {
                this.welcomeText.Show();
                this.activeQuestsReadout.Show();
                this.memoryPieceHintUI.CheckStartShowing();
                Get.LessonDrawerGOC.OnUIInit();
                if (!Get.InLobby)
                {
                    this.randomTip.Show();
                }
                GUI.FocusControl(null);
            }
            try
            {
                Profiler.Begin("newImportantConditionsUI.OnGUI()");
                this.newImportantConditionsUI.OnGUI();
            }
            catch (Exception ex)
            {
                Log.Error("Error in newImportantConditionsUI.OnGUI().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("strikeOverlays.OnGUI()");
                this.strikeOverlays.OnGUI();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in strikeOverlays.OnGUI().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("tooltips.OnGUI()");
                this.tooltips.OnGUI();
            }
            catch (Exception ex3)
            {
                Log.Error("Error in tooltips.OnGUI().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("windowManager.CheckCloseWindowsOnClickedOutside()");
                this.windowManager.CheckCloseWindowsOnClickedOutside();
            }
            catch (Exception ex4)
            {
                Log.Error("Error in windowManager.CheckCloseWindowsOnClickedOutside().", ex4);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("hpBarOverlayDrawer.OnGUI()");
                this.hpBarOverlayDrawer.OnGUI();
            }
            catch (Exception ex5)
            {
                Log.Error("Error in hpBarOverlayDrawer.OnGUI().", ex5);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("inspectOverlaysDrawer.OnGUI()");
                this.inspectOverlaysDrawer.OnGUI();
            }
            catch (Exception ex6)
            {
                Log.Error("Error in inspectOverlaysDrawer.OnGUI().", ex6);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("bodyPartOverlayDrawer.OnGUI()");
                this.bodyPartOverlayDrawer.OnGUI();
            }
            catch (Exception ex7)
            {
                Log.Error("Error in bodyPartOverlayDrawer.OnGUI().", ex7);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("staticTextOverlays.OnGUI()");
                this.staticTextOverlays.OnGUI();
            }
            catch (Exception ex8)
            {
                Log.Error("Error in staticTextOverlays.OnGUI().", ex8);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("floatingTexts.OnGUI()");
                this.floatingTexts.OnGUI();
            }
            catch (Exception ex9)
            {
                Log.Error("Error in floatingTexts.OnGUI().", ex9);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("playerHitMarkers.OnGUI()");
                this.playerHitMarkers.OnGUI();
            }
            catch (Exception ex10)
            {
                Log.Error("Error in playerHitMarkers.OnGUI().", ex10);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("dragAndDrop.OnGUI()");
                this.dragAndDrop.OnGUI();
            }
            catch (Exception ex11)
            {
                Log.Error("Error in dragAndDrop.OnGUI().", ex11);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("nextTurnsUI.OnGUI()");
                this.nextTurnsUI.OnGUI();
            }
            catch (Exception ex12)
            {
                Log.Error("Error in nextTurnsUI.OnGUI().", ex12);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("Compass.Do()");
                Compass.Do();
            }
            catch (Exception ex13)
            {
                Log.Error("Error in Compass.Do().", ex13);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("roomLabelDrawer.OnGUI()");
                this.roomLabelDrawer.OnGUI();
            }
            catch (Exception ex14)
            {
                Log.Error("Error in roomLabelDrawer.OnGUI().", ex14);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("bossHPBar.OnGUI()");
                this.bossHPBar.OnGUI();
            }
            catch (Exception ex15)
            {
                Log.Error("Error in bossHPBar.OnGUI().", ex15);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("minimap.OnGUI()");
                this.minimap.OnGUI();
            }
            catch (Exception ex16)
            {
                Log.Error("Error in minimap.OnGUI().", ex16);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("rewindUI.OnGUI()");
                this.rewindUI.OnGUI();
            }
            catch (Exception ex17)
            {
                Log.Error("Error in rewindUI.OnGUI().", ex17);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("collectedItemsDrawer.OnGUI()");
                this.collectedItemsDrawer.OnGUI();
            }
            catch (Exception ex18)
            {
                Log.Error("Error in collectedItemsDrawer.OnGUI().", ex18);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("timeDrawer.OnGUI()");
                this.timeDrawer.OnGUI();
            }
            catch (Exception ex19)
            {
                Log.Error("Error in timeDrawer.OnGUI().", ex19);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("lobbyQuestCountLabel.OnGUI()");
                this.lobbyQuestCountLabel.OnGUI();
            }
            catch (Exception ex20)
            {
                Log.Error("Error in lobbyQuestCountLabel.OnGUI().", ex20);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("scoreGainEffects.OnGUI()");
                this.scoreGainEffects.OnGUI();
            }
            catch (Exception ex21)
            {
                Log.Error("Error in scoreGainEffects.OnGUI().", ex21);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("playerActorStatusControls.OnGUI()");
                this.playerActorStatusControls.OnGUI();
            }
            catch (Exception ex22)
            {
                Log.Error("Error in playerActorStatusControls.OnGUI().", ex22);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("partyUI.OnGUI()");
                this.partyUI.OnGUI();
            }
            catch (Exception ex23)
            {
                Log.Error("Error in partyUI.OnGUI().", ex23);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("seenEntitiesDrawer.OnGUI()");
                this.seenEntitiesDrawer.OnGUI();
            }
            catch (Exception ex24)
            {
                Log.Error("Error in seenEntitiesDrawer.OnGUI().", ex24);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("FPPQuickbarControls.OnGUI()");
                FPPQuickbarControls.OnGUI();
            }
            catch (Exception ex25)
            {
                Log.Error("Error in FPPQuickbarControls.OnGUI().", ex25);
            }
            finally
            {
                Profiler.End();
            }
            if (this.inventoryOpen)
            {
                try
                {
                    Profiler.Begin("TimelineDrawer.Do()");
                    TimelineDrawer.Do();
                    goto IL_0524;
                }
                catch (Exception ex26)
                {
                    Log.Error("Error in TimelineDrawer.Do().", ex26);
                    goto IL_0524;
                }
                finally
                {
                    Profiler.End();
                }
            }
            try
            {
                Profiler.Begin("useOnTargetUI.OnGUI()");
                this.useOnTargetUI.OnGUI();
            }
            catch (Exception ex27)
            {
                Log.Error("Error in useOnTargetUI.OnGUI().", ex27);
            }
            finally
            {
                Profiler.End();
            }
        IL_0524:
            try
            {
                Profiler.Begin("iconOverlayDrawer.OnGUI()");
                this.iconOverlayDrawer.OnGUI();
            }
            catch (Exception ex28)
            {
                Log.Error("Error in iconOverlayDrawer.OnGUI().", ex28);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("crosshairDrawer.OnGUI()");
                this.crosshairDrawer.OnGUI();
            }
            catch (Exception ex29)
            {
                Log.Error("Error in crosshairDrawer.OnGUI().", ex29);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("randomTip.OnGUI()");
                this.randomTip.OnGUI();
            }
            catch (Exception ex30)
            {
                Log.Error("Error in randomTip.OnGUI().", ex30);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("privateRoomTip.OnGUI()");
                this.privateRoomTip.OnGUI();
            }
            catch (Exception ex31)
            {
                Log.Error("Error in privateRoomTip.OnGUI().", ex31);
            }
            finally
            {
                Profiler.End();
            }
            if (this.wheelSelector != null)
            {
                try
                {
                    Profiler.Begin("wheelSelector.OnGUI()");
                    this.wheelSelector.OnGUI();
                }
                catch (Exception ex32)
                {
                    Log.Error("Error in wheelSelector.OnGUI().", ex32);
                }
                finally
                {
                    Profiler.End();
                }
            }
            try
            {
                Profiler.Begin("windowManager.OnGUI()");
                this.windowManager.OnGUI();
            }
            catch (Exception ex33)
            {
                Log.Error("Error in windowManager.OnGUI().", ex33);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("CheckToggleInventory()");
                this.CheckToggleInventory();
            }
            catch (Exception ex34)
            {
                Log.Error("Error in CheckToggleInventory().", ex34);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("welcomeText.OnGUI()");
                this.welcomeText.OnGUI();
            }
            catch (Exception ex35)
            {
                Log.Error("Error in welcomeText.OnGUI().", ex35);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("activeQuestsReadout.OnGUI()");
                this.activeQuestsReadout.OnGUI();
            }
            catch (Exception ex36)
            {
                Log.Error("Error in activeQuestsReadout.OnGUI().", ex36);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("memoryPieceHintUI.OnGUI()");
                this.memoryPieceHintUI.OnGUI();
            }
            catch (Exception ex37)
            {
                Log.Error("Error in memoryPieceHintUI.OnGUI().", ex37);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("questCompletedText.OnGUI()");
                this.questCompletedText.OnGUI();
            }
            catch (Exception ex38)
            {
                Log.Error("Error in questCompletedText.OnGUI().", ex38);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("worldEventNotification.OnGUI()");
                this.worldEventNotification.OnGUI();
            }
            catch (Exception ex39)
            {
                Log.Error("Error in worldEventNotification.OnGUI().", ex39);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("bossSlainText.OnGUI()");
                this.bossSlainText.OnGUI();
            }
            catch (Exception ex40)
            {
                Log.Error("Error in bossSlainText.OnGUI().", ex40);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("deathScreenDrawer.OnGUI()");
                this.deathScreenDrawer.OnGUI();
            }
            catch (Exception ex41)
            {
                Log.Error("Error in deathScreenDrawer.OnGUI().", ex41);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("DoMainTabButtons()");
                this.DoMainTabButtons();
            }
            catch (Exception ex42)
            {
                Log.Error("Error in DoMainTabButtons().", ex42);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("debugUI.OnGUI()");
                this.debugUI.OnGUI();
            }
            catch (Exception ex43)
            {
                Log.Error("Error in debugUI.OnGUI().", ex43);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("CheckOpenEscapeMenu()");
                this.CheckOpenEscapeMenu();
            }
            catch (Exception ex44)
            {
                Log.Error("Error in CheckOpenEscapeMenu().", ex44);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("MouseAttachmentDrawerGOC.CheckEnableDisable()");
                Get.MouseAttachmentDrawerGOC.CheckEnableDisable();
            }
            catch (Exception ex45)
            {
                Log.Error("Error in MouseAttachmentDrawerGOC.CheckEnableDisable().", ex45);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("LessonDrawerGOC.CheckEnableDisable()");
                Get.LessonDrawerGOC.CheckEnableDisable();
            }
            catch (Exception ex46)
            {
                Log.Error("Error in LessonDrawerGOC.CheckEnableDisable().", ex46);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void Update()
        {
            try
            {
                Profiler.Begin("minimap.Update()");
                this.minimap.Update();
            }
            catch (Exception ex)
            {
                Log.Error("Error in minimap.Update().", ex);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void FixedUpdate()
        {
            try
            {
                Profiler.Begin("nextTurnsUI.FixedUpdate()");
                this.nextTurnsUI.FixedUpdate();
            }
            catch (Exception ex)
            {
                Log.Error("Error in nextTurnsUI.FixedUpdate().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("crosshairDrawer.FixedUpdate()");
                this.crosshairDrawer.FixedUpdate();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in crosshairDrawer.FixedUpdate().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("seenEntitiesDrawer.FixedUpdate()");
                this.seenEntitiesDrawer.FixedUpdate();
            }
            catch (Exception ex3)
            {
                Log.Error("Error in seenEntitiesDrawer.FixedUpdate().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("iconOverlayDrawer.FixedUpdate()");
                this.iconOverlayDrawer.FixedUpdate();
            }
            catch (Exception ex4)
            {
                Log.Error("Error in iconOverlayDrawer.FixedUpdate().", ex4);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("roomLabelDrawer.FixedUpdate()");
                this.roomLabelDrawer.FixedUpdate();
            }
            catch (Exception ex5)
            {
                Log.Error("Error in roomLabelDrawer.FixedUpdate().", ex5);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("progressBarDrawer.FixedUpdate()");
                this.progressBarDrawer.FixedUpdate();
            }
            catch (Exception ex6)
            {
                Log.Error("Error in progressBarDrawer.FixedUpdate().", ex6);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void OpenWheelSelector(List<ValueTuple<string, Action>> options, ITipSubject subject, string extraText = null, bool isSpellChooser = false, bool closeIfItemNotInInventory = false)
        {
            List<WheelSelector.Option> list = new List<WheelSelector.Option>();
            for (int i = 0; i < options.Count; i++)
            {
                list.Add(new WheelSelector.Option(options[i].Item1, options[i].Item2, null, null));
            }
            this.OpenWheelSelector(list, subject, extraText, isSpellChooser, closeIfItemNotInInventory, false);
        }

        public void OpenWheelSelector(List<ValueTuple<string, Action, string>> options, ITipSubject subject, string extraText = null, bool isSpellChooser = false, bool closeIfItemNotInInventory = false)
        {
            List<WheelSelector.Option> list = new List<WheelSelector.Option>();
            for (int i = 0; i < options.Count; i++)
            {
                list.Add(new WheelSelector.Option(options[i].Item1, options[i].Item2, options[i].Item3, null));
            }
            this.OpenWheelSelector(list, subject, extraText, isSpellChooser, closeIfItemNotInInventory, false);
        }

        public void OpenWheelSelector(List<WheelSelector.Option> options, ITipSubject subject, string extraText = null, bool isSpellChooser = false, bool closeIfItemNotInInventory = false, bool isQuickbarMenu = false)
        {
            this.windowManager.CloseAll(Get.Window_EscapeMenu);
            if (this.inventoryOpen)
            {
                this.CloseInventory(true);
            }
            this.wheelSelector = new WheelSelector(options, subject, extraText, isSpellChooser, closeIfItemNotInInventory, isQuickbarMenu);
            this.useOnTargetUI.StopTargeting();
            Mouse.SetCursorLocked(false);
            Get.Sound_OpenWindow.PlayOneShot(null, 1f, 1f);
        }

        public void CloseWheelSelector(bool playCloseSound = true)
        {
            if (this.wheelSelector == null)
            {
                return;
            }
            this.wheelSelector = null;
            if (!this.WantsMouseUnlocked)
            {
                Mouse.SetCursorLocked(true);
            }
            if (playCloseSound)
            {
                Get.Sound_CloseWindow.PlayOneShot(null, 1f, 1f);
            }
        }

        public void OpenEscapeMenu()
        {
            this.windowManager.Open(Get.Window_EscapeMenu, true);
        }

        public void OnWindowOpened()
        {
            if (this.WantsMouseUnlocked)
            {
                Mouse.SetCursorLocked(false);
            }
        }

        public void OnWindowClosed()
        {
            if (!this.WantsMouseUnlocked)
            {
                Mouse.SetCursorLocked(true);
            }
        }

        public void OnDungeonMapShowing()
        {
            if (this.WantsMouseUnlocked)
            {
                Mouse.SetCursorLocked(false);
            }
        }

        public void OnBeginTargeting()
        {
            this.CloseInventory(false);
            this.CloseWheelSelector(false);
        }

        public void OnPressedQuitToMainMenuButton()
        {
            Mouse.SetCursorLocked(false);
        }

        private void OpenWindowsToAutoOpenInUIMode(bool animate)
        {
            foreach (Window window in Get.WindowManager.Windows.ToTemporaryList<Window>())
            {
                if (window.Spec.AutoOpenForMainTab != null && window.Spec.AutoOpenForMainTab != this.currentTab)
                {
                    Get.WindowManager.Close(window, false);
                }
            }
            List<WindowSpec> all = Get.Specs.GetAll<WindowSpec>();
            bool inLobby = Get.InLobby;
            for (int i = 0; i < all.Count; i++)
            {
                if (((!inLobby && all[i].AutoOpenForMainTab != null && all[i].AutoOpenForMainTab == this.currentTab) || (inLobby && all[i].AutoOpenInUIModeInLobby)) && !this.windowManager.IsOpen(all[i]))
                {
                    Window window2 = this.windowManager.Open(all[i], false);
                    if (animate)
                    {
                        window2.StartOpenAnimation();
                    }
                }
            }
        }

        private void CheckToggleInventory()
        {
            if ((Get.KeyBinding_ShowInventory.JustPressed || Get.KeyBinding_ShowInventoryAlt.JustPressed) && !Get.TextSequenceDrawer.Showing && !Get.DungeonMapDrawer.Showing)
            {
                if (!this.inventoryOpen)
                {
                    if (!Get.InLobby || Get.NowControlledActor.Position.InPrivateRoom())
                    {
                        this.inventoryOpen = true;
                        this.currentTab = Get.MainTab_Inventory;
                        this.windowManager.CloseAll(Get.Window_EscapeMenu);
                        this.CloseWheelSelector(true);
                        this.useOnTargetUI.StopTargeting();
                        this.OpenWindowsToAutoOpenInUIMode(true);
                        Mouse.SetCursorLocked(false);
                        Get.Sound_OpenInventory.PlayOneShot(null, 1f, 1f);
                        this.lastTimeOpenedAutoOpenInUIModeWindows = Clock.UnscaledTime;
                        Get.LessonManager.FinishIfCurrent(Get.Lesson_OpeningInventory);
                    }
                }
                else
                {
                    this.CloseInventory(true);
                }
            }
            if (this.inventoryOpen && KeyCodeUtility.CancelJustPressed)
            {
                this.CloseInventory(true);
                if (Event.current.type == EventType.KeyDown)
                {
                    Event.current.Use();
                }
            }
        }

        private void CheckOpenEscapeMenu()
        {
            if (Get.KeyBinding_Cancel.JustPressed && !this.IsEscapeMenuOpen && !Get.TextSequenceDrawer.Showing && !Get.DungeonMapDrawer.Showing)
            {
                this.OpenEscapeMenu();
                if (Event.current.type == EventType.KeyDown)
                {
                    Event.current.Use();
                }
            }
        }

        private void CloseInventory(bool withSound = true)
        {
            if (!this.inventoryOpen)
            {
                return;
            }
            this.inventoryOpen = false;
            this.windowManager.CloseAll();
            this.lastTimeClosedAutoOpenInUIModeWindows = Clock.UnscaledTime;
            this.mainButtonLabelsAlpha = 0f;
            if (!this.WantsMouseUnlocked)
            {
                Mouse.SetCursorLocked(true);
            }
            if (withSound)
            {
                Get.Sound_CloseInventory.PlayOneShot(null, 1f, 1f);
            }
        }

        public void ShowWelcomeTexts()
        {
            this.showWelcomeTextsOnFrame = Clock.Frame + 1;
            this.PlayWelcomeSong();
        }

        private void PlayWelcomeSong()
        {
            float num;
            if (Get.MusicManager.PlayingMusicNow)
            {
                num = 0.15f;
            }
            else
            {
                num = 1f;
                Get.MusicManager.ForceSilenceFor(5f);
                Get.AmbientSoundsManager.ForceSilenceFor(5f);
            }
            float num2;
            if (Get.InLobby)
            {
                SoundSpec sound_LobbyWelcomeSong = Get.Sound_LobbyWelcomeSong;
                num2 = num;
                sound_LobbyWelcomeSong.PlayOneShot(null, num2, 1f);
                return;
            }
            SoundSpec sound_WelcomeSong = Get.Sound_WelcomeSong;
            num2 = num;
            sound_WelcomeSong.PlayOneShot(null, num2, 1f);
        }

        public void SetCurrentTabAndOpenWindows(MainTabSpec tab)
        {
            this.currentTab = tab;
            this.OpenWindowsToAutoOpenInUIMode(false);
        }

        private void DoMainTabButtons()
        {
            if (!this.InventoryOpen || Get.InLobby)
            {
                return;
            }
            float num = Widgets.ScreenCenter.y - 331f;
            if (this.mainTabsInDisplayOrder == null)
            {
                this.mainTabsInDisplayOrder = (from x in Get.Specs.GetAll<MainTabSpec>()
                                               orderby x.Order
                                               select x).ToList<MainTabSpec>();
            }
            float num2 = Math.Max((1f - (Clock.UnscaledTime - Get.UI.LastTimeOpenedAutoOpenInUIModeWindows) / 0.036363635f) * 8f, 0f);
            float num3 = Widgets.ScreenCenter.x - 443f - 10f - 64f;
            bool flag = false;
            MainTabSpec mainTabSpec = this.currentTab;
            bool flag2 = false;
            for (int i = 0; i < this.mainTabsInDisplayOrder.Count; i++)
            {
                MainTabSpec mainTabSpec2 = this.mainTabsInDisplayOrder[i];
                if ((mainTabSpec2 != Get.MainTab_DungeonMap || Get.PlaceManager.Places.Count != 0) && (mainTabSpec2 != Get.MainTab_Skills || Get.NowControlledActor == Get.MainActor) && (mainTabSpec2 != Get.MainTab_Quests || !Get.RunConfig.IsDailyChallenge))
                {
                    if (this.currentTab == mainTabSpec2)
                    {
                        flag = true;
                    }
                    Rect rect = new Rect(num3, num + num2, 64f, 64f);
                    bool flag3 = false;
                    if (Mouse.Over(rect))
                    {
                        Get.Tooltips.RegisterTip(rect, mainTabSpec2.LabelCap.AppendedInDoubleNewLine(mainTabSpec2.Description), null);
                        flag2 = true;
                        flag3 = true;
                    }
                    Color? color;
                    if (mainTabSpec2 == this.currentTab)
                    {
                        color = new Color?(Widgets.ActiveTabColor);
                    }
                    else if (mainTabSpec2 == Get.MainTab_Skills && Get.Player.SkillPoints > 0)
                    {
                        color = new Color?(Color.white.MultipliedColor(0.2f + Calc.PulseUnscaled(2f, 0.25f)));
                    }
                    else
                    {
                        color = null;
                    }
                    Rect rect2 = rect;
                    string text = null;
                    bool flag4 = true;
                    Color? color2 = color;
                    Texture2D icon = mainTabSpec2.Icon;
                    if (Widgets.Button(rect2, text, flag4, color2, null, true, true, true, true, icon, true, true, null, false, null, null))
                    {
                        this.SetCurrentTabAndOpenWindows(mainTabSpec2);
                    }
                    if (mainTabSpec2 == Get.MainTab_Skills && Get.Player.SkillPoints != 0)
                    {
                        this.DoMainButtonNumber(rect, Get.Player.SkillPoints);
                    }
                    else if (mainTabSpec2 == Get.MainTab_Quests)
                    {
                        int num4 = Window_Quests.GetQuestsToShow().Count + Window_Quests.GetExtraQuests().Count;
                        if (num4 != 0)
                        {
                            this.DoMainButtonNumber(rect, num4);
                        }
                    }
                    if (this.mainButtonLabelsAlpha > 0f)
                    {
                        Widgets.FontSizeScalable = 17;
                        Widgets.Align = TextAnchor.MiddleRight;
                        Widgets.FontBold = flag3;
                        GUI.color = new Color(0.9f, 0.9f, 0.9f, Calc.Clamp01(this.mainButtonLabelsAlpha) * 0.55f);
                        Widgets.Label(new Rect(rect.xMin - 160f, rect.y, 150f, rect.height), mainTabSpec2.LabelCap, true, null, null, true);
                        GUI.color = Color.white;
                        Widgets.FontBold = false;
                        Widgets.ResetAlign();
                        Widgets.ResetFontSize();
                    }
                    num += 74f;
                }
            }
            if (!flag && this.currentTab != Get.MainTab_Inventory && mainTabSpec == this.currentTab)
            {
                this.SetCurrentTabAndOpenWindows(Get.MainTab_Inventory);
            }
            if (Event.current.type == EventType.Repaint)
            {
                if (flag2)
                {
                    this.mainButtonLabelsAlpha = Math.Min(this.mainButtonLabelsAlpha + Clock.UnscaledDeltaTime * 4f, 2f);
                    return;
                }
                this.mainButtonLabelsAlpha = Math.Max(this.mainButtonLabelsAlpha - Clock.UnscaledDeltaTime * 5f, -0.42f);
            }
        }

        private void DoMainButtonNumber(Rect rect, int number)
        {
            Widgets.FontSizeScalable = 17;
            Widgets.FontBold = true;
            Widgets.Align = TextAnchor.LowerRight;
            Widgets.Label(rect.MovedBy(-6f, -2f), number.ToStringCached(), true, null, null, true);
            Widgets.ResetAlign();
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
        }

        private WindowManager windowManager = new WindowManager();

        private Minimap minimap = new Minimap();

        private IconOverlayDrawer iconOverlayDrawer = new IconOverlayDrawer();

        private CrosshairDrawer crosshairDrawer = new CrosshairDrawer();

        private HPBarOverlayDrawer hpBarOverlayDrawer = new HPBarOverlayDrawer();

        private InspectOverlaysDrawer inspectOverlaysDrawer = new InspectOverlaysDrawer();

        private BodyPartOverlayDrawer bodyPartOverlayDrawer = new BodyPartOverlayDrawer();

        private DragAndDrop dragAndDrop = new DragAndDrop();

        private RoomLabelDrawer roomLabelDrawer = new RoomLabelDrawer();

        private StaticTextOverlays staticTextOverlays = new StaticTextOverlays();

        private FloatingTexts floatingTexts = new FloatingTexts();

        private Tooltips tooltips = new Tooltips();

        private SeenEntitiesDrawer seenEntitiesDrawer = new SeenEntitiesDrawer();

        private DebugUI debugUI = new DebugUI();

        private StrikeOverlays strikeOverlays = new StrikeOverlays();

        private PlayerHitMarkers playerHitMarkers = new PlayerHitMarkers();

        private UseOnTargetUI useOnTargetUI = new UseOnTargetUI();

        private ProgressBarDrawer progressBarDrawer = new ProgressBarDrawer();

        private QuestCompletedText questCompletedText = new QuestCompletedText();

        private MemoryPieceHintUI memoryPieceHintUI = new MemoryPieceHintUI();

        private WorldEventNotification worldEventNotification = new WorldEventNotification();

        private UnexploredRoomsDrawer unexploredRoomsDrawer = new UnexploredRoomsDrawer();

        private BossSlainText bossSlainText = new BossSlainText();

        private RewindUI rewindUI = new RewindUI();

        private NextTurnsUI nextTurnsUI = new NextTurnsUI();

        private TimeDrawer timeDrawer = new TimeDrawer();

        private LobbyQuestCountLabel lobbyQuestCountLabel = new LobbyQuestCountLabel();

        private RandomTip randomTip = new RandomTip();

        private PrivateRoomTip privateRoomTip = new PrivateRoomTip();

        private ActiveQuestsReadout activeQuestsReadout = new ActiveQuestsReadout();

        private WelcomeText welcomeText = new WelcomeText();

        private ScoreGainEffects scoreGainEffects = new ScoreGainEffects();

        private PlayerActorStatusControls playerActorStatusControls = new PlayerActorStatusControls();

        private DeathScreenDrawer deathScreenDrawer = new DeathScreenDrawer();

        private BossHPBar bossHPBar = new BossHPBar();

        private CollectedItemsDrawer collectedItemsDrawer = new CollectedItemsDrawer();

        private PartyUI partyUI = new PartyUI();

        private NewImportantConditionsUI newImportantConditionsUI = new NewImportantConditionsUI();

        private bool inventoryOpen;

        private int showWelcomeTextsOnFrame = -1;

        private MainTabSpec currentTab = Get.MainTab_Inventory;

        private WheelSelector wheelSelector;

        private float lastTimeOpenedAutoOpenInUIModeWindows = -99999f;

        private float lastTimeClosedAutoOpenInUIModeWindows = -99999f;

        private List<MainTabSpec> mainTabsInDisplayOrder;

        private float mainButtonLabelsAlpha;
    }
}