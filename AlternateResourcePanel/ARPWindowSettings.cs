using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPAlternateResourcePanel
{
    class  ARPWindowSettings : MonoBehaviourWindowPlus
    {
        internal KSPAlternateResourcePanel mbARP;
        internal Settings settings;

        internal DropDownList ddlSettingsTab;
        private DropDownList ddlSettingsRateStyle;
        private DropDownList ddlSettingsSkin;
        private DropDownList ddlSettingsAlarmsWarning;
        private DropDownList ddlSettingsAlarmsAlert;

        internal enum SettingsTabs
        {
            [Description("General Properties")] General,
            [Description("Styling/Visuals")]    Styling,
            [Description("Alarm Properties")]   Alarms,
            [Description("Staging Controls")]   Staging,
            [Description("About...")]   About,
        }


        internal enum ButtonStyleEnum
        {
            [Description("Basic button")]                       Basic,
            [Description("Common Toolbar (by Blizzy78)")]       Toolbar,
            [Description("KSP App Launcher Button")]            Launcher,
            [Description("Replace Stock Resources App")]        StockReplace,
        }

        private DropDownList ddlSettingsButtonStyle;
        
        internal Single WindowHeight;
        Int32 MinWindowHeight = 136;
        Int32 SettingsAreaWidth = 314; //284;

        internal override void OnAwake()
        {
            settings = KSPAlternateResourcePanel.settings;

            TooltipMouseOffset = new Vector2d(-10, 10);

            ddlSettingsTab = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<SettingsTabs>(),this);

            ddlSettingsSkin = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<Settings.DisplaySkin>(), (Int32)settings.SelectedSkin,this);
            ddlSettingsSkin.OnSelectionChanged += ddlSettingsSkin_SelectionChanged;

            ddlSettingsAlarmsWarning = LoadSoundsList(Resources.clipAlarms.Keys.ToArray(),settings.AlarmsWarningSound);
            ddlSettingsAlarmsAlert = LoadSoundsList(Resources.clipAlarms.Keys.ToArray(), settings.AlarmsAlertSound);
            ddlSettingsAlarmsWarning.OnSelectionChanged += ddlSettingsAlarmsWarning_OnSelectionChanged;
            ddlSettingsAlarmsAlert.OnSelectionChanged += ddlSettingsAlarmsAlert_OnSelectionChanged;

            ddlSettingsRateStyle = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<Settings.RateDisplayEnum>(),(Int32)settings.RateDisplayType, this);
            ddlSettingsRateStyle.OnSelectionChanged += ddlSettingsRateStyle_OnSelectionChanged;

            ddlSettingsButtonStyle = new DropDownList(KSPPluginFramework.EnumExtensions.ToEnumDescriptions<ButtonStyleEnum>(), (Int32)settings.ButtonStyleChosen, this);
            ddlSettingsButtonStyle.OnSelectionChanged += ddlSettingsButtonStyle_OnSelectionChanged; 

            ddlManager.AddDDL(ddlSettingsButtonStyle);
            ddlManager.AddDDL(ddlSettingsAlarmsWarning);
            ddlManager.AddDDL(ddlSettingsAlarmsAlert);
            ddlManager.AddDDL(ddlSettingsRateStyle);
            ddlManager.AddDDL(ddlSettingsTab);
            ddlManager.AddDDL(ddlSettingsSkin);
        }

        internal override void OnDestroy()
        {
            ddlSettingsSkin.OnSelectionChanged -= ddlSettingsSkin_SelectionChanged;
            ddlSettingsAlarmsWarning.OnSelectionChanged -= ddlSettingsAlarmsWarning_OnSelectionChanged;
            ddlSettingsAlarmsAlert.OnSelectionChanged -= ddlSettingsAlarmsAlert_OnSelectionChanged;
        }

        void ddlSettingsAlarmsAlert_OnSelectionChanged(MonoBehaviourWindowPlus.DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.AlarmsAlertSound = ddlSettingsAlarmsAlert.SelectedValue;
            if (settings.AlarmsAlertSound == "None")
                mbARP.clipAlarmsAlert = null;
            else
                mbARP.clipAlarmsAlert = Resources.clipAlarms[settings.AlarmsAlertSound];
            settings.Save();
        }

        void ddlSettingsAlarmsWarning_OnSelectionChanged(MonoBehaviourWindowPlus.DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.AlarmsWarningSound = ddlSettingsAlarmsWarning.SelectedValue;
            if (settings.AlarmsWarningSound == "None")
                mbARP.clipAlarmsWarning = null;
            else
                mbARP.clipAlarmsWarning = Resources.clipAlarms[settings.AlarmsWarningSound];
            settings.Save();
        }

        void ddlSettingsSkin_SelectionChanged(DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.SelectedSkin = (Settings.DisplaySkin)NewIndex;
            SkinsLibrary.SetCurrent(settings.SelectedSkin.ToString());
            settings.Save();
        }

        void ddlSettingsRateStyle_OnSelectionChanged(MonoBehaviourWindowPlus.DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.RateDisplayType = (Settings.RateDisplayEnum)NewIndex;
            settings.Save();
        }
        void ddlSettingsButtonStyle_OnSelectionChanged(MonoBehaviourWindowPlus.DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.ButtonStyleChosen = (ButtonStyleEnum)NewIndex;
            settings.Save();

            //destroy Old Objects
            switch ((ButtonStyleEnum)OldIndex)
	        {
                case ButtonStyleEnum.Toolbar:
                    mbARP.DestroyToolbarButton(mbARP.btnToolbar);
                    break;
                case ButtonStyleEnum.Launcher:
                    mbARP.DestroyAppLauncherButton();
                    break;
                case ButtonStyleEnum.StockReplace:
                    mbARP.windowMain.DragEnabled = !settings.LockLocation;
                    mbARP.windowMain.WindowRect = settings.WindowPosition;
                    mbARP.SceneChangeRequiredToRestoreResourcesApp = true;
                    break;
            }

            //Create New ones
            switch ((ButtonStyleEnum)NewIndex)
	        {
                case ButtonStyleEnum.Toolbar:
                    mbARP.btnToolbar = mbARP.InitToolbarButton();
                    break;
                case ButtonStyleEnum.Launcher:
                    mbARP.btnAppLauncher = mbARP.InitAppLauncherButton();
                    mbARP.AppLauncherToBeSetTrueAttemptDate = DateTime.Now;
                    mbARP.AppLauncherToBeSetTrue = true;
                    break;
                case ButtonStyleEnum.StockReplace:
                    mbARP.btnAppLauncher = mbARP.InitAppLauncherButton();
                    StartCoroutine(mbARP.ReplaceStockAppButton());
                    mbARP.windowMain.DragEnabled=false;
                    mbARP.windowMain.WindowRect = new Rect(mbARP.windowMainResetPos);
                    mbARP.SceneChangeRequiredToRestoreResourcesApp = false;
                    mbARP.AppLauncherToBeSetTrueAttemptDate = DateTime.Now;
                    mbARP.AppLauncherToBeSetTrue = true;
                    break;
            }
        }


        private DropDownList LoadSoundsList(String[] Names, String Selected)
        {
            DropDownList retDDl = new DropDownList(Names,this);
            
            if(Names.Contains(Selected))
            {
                retDDl.SelectedIndex = Array.FindIndex(Names, x => x == Selected);
            }
            return retDDl;
        }

        private Int32 intBlizzyToolbarMissingHeight = 0;

        internal override void DrawWindow(int id)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Settings Section", Styles.styleStageTextHead,GUILayout.Width(140));
            GUILayout.Space(5);
            ddlSettingsTab.DrawButton();
            GUILayout.Space(4);
            GUILayout.EndHorizontal();

            SettingsAreaWidth = 314; //284;
            if (settings.AlarmsEnabled)
                SettingsAreaWidth += mbARP.windowMain.IconAlarmOffset;
            //GUILayout.Box("", Styles.styleSeparatorH, GUILayout.Height(2));

            switch ((SettingsTabs)ddlSettingsTab.SelectedIndex)
            {
                case SettingsTabs.General:
                    WindowHeight = 212 + ((settings.SplitLastStage)?20:0);// 180; //160 ;// MinWindowHeight;
                    DrawWindow_General();
                    break;
                case SettingsTabs.Styling:
                    WindowHeight = 284 + intStylingWindowHeightAdjustments;// 281; //241; //174;
                    DrawWindow_Styling();
                    break;
                case SettingsTabs.Alarms:
                    WindowHeight = 246;
                    DrawWindow_Alarms();
                    break;
                case SettingsTabs.Staging:
                    WindowHeight = MinWindowHeight + ((settings.StagingEnabled)?17:0);// 153;// 173 ;//136;
                    DrawWindow_Staging();
                    break;
                case SettingsTabs.About:
                    WindowHeight = 246 + (settings.VersionAvailable?24:0);
                    DrawWindow_About();
                    break;
            }
            if (WindowHeight < MinWindowHeight) WindowHeight = MinWindowHeight;
            GUILayout.EndVertical();
        }


        private void DrawWindow_General()
        {
            GUILayout.BeginHorizontal(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Space(2);
            GUILayout.Label("Rates:", Styles.styleStageTextHead);
            GUILayout.Space(13);
            GUILayout.Label("Calc By:", Styles.styleStageTextHead);
            GUILayout.Label("Rate Style:", Styles.styleStageTextHead);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();

            if (DrawToggle(ref settings.ShowRates, "Show Rate Change Values", Styles.styleToggle))
                settings.Save();
            //if (DrawToggle(ref settings.ShowRatesTimeRem, "Toggle Time Remaining", Styles.styleToggle))
            //    settings.Save();
            if (DrawToggle(ref settings.ShowRatesForParts, "Show Rates for Parts", Styles.styleToggle))
            {
                settings.Save();
            }

            GUILayout.BeginHorizontal();
            if (DrawToggle(ref settings.RatesUseUT, "UT", Styles.styleToggle,GUILayout.Width(60)))
                settings.Save();
            Boolean NotUT = !settings.RatesUseUT;
            if (DrawToggle(ref NotUT, "Real Time", Styles.styleToggle))
            {
                settings.RatesUseUT=!settings.RatesUseUT;
                settings.Save();
            }
            GUILayout.EndHorizontal();

            ddlSettingsRateStyle.DrawButton();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginVertical(GUILayout.Width(60));

            GUILayout.Space(2);
            GUILayout.Label("Stage Bars:", Styles.styleStageTextHead);
            if (settings.SplitLastStage)
            {
                GUILayout.Space(-6);
                GUILayout.Label("Bars Pos:", Styles.styleStageTextHead);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Space(2);
            if (DrawToggle(ref settings.SplitLastStage, new GUIContent("Enabled", "Turn this off to show single green bars and no last stage separation."), Styles.styleToggle))
                settings.Save();
            if (settings.SplitLastStage)
            {
                GUILayout.BeginHorizontal();
                Boolean NotRight = !settings.StageBarOnRight;
                if (DrawToggle(ref NotRight, "On Left", Styles.styleToggle, GUILayout.Width(90)))
                {
                    settings.StageBarOnRight = !settings.StageBarOnRight; 
                    settings.Save();
                }
                if (DrawToggle(ref settings.StageBarOnRight, "On Right", Styles.styleToggle))
                {
                    settings.Save();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Space(2);
            GUILayout.Label("Resources:", Styles.styleStageTextHead);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (DrawButton("Configure Resource Settings"))
            {
                mbARP.windowResourceConfig.Visible = !mbARP.windowResourceConfig.Visible;
                if (mbARP.windowResourceConfig.Visible)
                {
                    mbARP.windowResourceConfig.WindowRect.y = mbARP.windowMain.WindowRect.y;
                    if (mbARP.windowMain.WindowRect.x + (mbARP.windowMain.WindowRect.width / 2) > (Screen.width/2))
                        mbARP.windowResourceConfig.WindowRect.x = mbARP.windowMain.WindowRect.x - mbARP.windowResourceConfig.WindowRect.width;
                    else
                        mbARP.windowResourceConfig.WindowRect.x = mbARP.windowMain.WindowRect.x + mbARP.windowMain.WindowRect.width;
                }
            }
            if (DrawToggle(ref settings.ShowWindowOnResourceMonitor, new GUIContent("Show Window on Alarm", "If an alarm is exceeded then popup the window if it is not already visible. Will hide again when acknowledged."),Styles.styleToggle))
                settings.Save();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

        }

        Int32 intStylingWindowHeightAdjustments=0;
        private void DrawWindow_Styling()
        {
            //Styling
            GUILayout.BeginHorizontal(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth), GUILayout.Height(54));

            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Space(2); //to even up the text
            GUILayout.Label("Styling:", Styles.styleStageTextHead);
            GUILayout.Label("Button:", Styles.styleStageTextHead);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            ddlSettingsSkin.DrawButton();

            ddlSettingsButtonStyle.DrawButton();

            intBlizzyToolbarMissingHeight = 0;
            if (!settings.BlizzyToolbarIsAvailable)
            {
                if (settings.ButtonStyleChosen == ButtonStyleEnum.Toolbar)
                {
                    if (GUILayout.Button(new GUIContent("Not Installed. Click for Toolbar Info", "Click to open your browser and find out more about the Common Toolbar"), Styles.styleTextCenterGreen))
                        Application.OpenURL("http://forum.kerbalspaceprogram.com/threads/60863");
                    intBlizzyToolbarMissingHeight=18;
                }
                //if (DrawToggle(ref settings.UseBlizzyToolbarIfAvailable, new GUIContent("Use Common Toolbar", "Choose to use the Common  Toolbar or the native KSP ARP button"), Styles.styleToggle))
                //{
                //    if (settings.BlizzyToolbarIsAvailable)
                //    {
                //        if (settings.UseBlizzyToolbarIfAvailable)
                //            mbARP.btnToolbar = mbARP.InitToolbarButton();
                //        else
                //            mbARP.DestroyToolbarButton(mbARP.btnToolbar);
                //    }
                //    settings.Save();
                //}
            }
            if (settings.ButtonStyleToDisplay == ButtonStyleEnum.Launcher)
            {
                if (DrawToggle(ref settings.AppLauncherMutuallyExclusive, new GUIContent("Hide when other Apps show", "Hide the ARP when other stock Apps display (like the stock Resource App)"), Styles.styleToggle))
                {
                    mbARP.AppLauncherButtonMutuallyExclusive(settings.AppLauncherMutuallyExclusive);

                    //mbARP.btnAppLauncher.SetTrue();
                    //Something needed here so the change sticks immediately - currently need to toggle the buttons once for it all to be in sync
                    //if (settings.AppLauncherMutuallyExclusive)
                    //{
                    //    //set other apps hidden
                    //    ApplicationLauncherButton[] lstButtons = KSPAlternateResourcePanel.FindObjectsOfType<ApplicationLauncherButton>();
                    //    foreach (ApplicationLauncherButton item in lstButtons)
                    //    {
                    //        if (item!=mbARP.btnAppLauncher)
                    //                item.SetFalse();
                    //    }
                    //}
                    settings.Save();
                }
                GUILayout.Space(4);
            }
            if (mbARP.SceneChangeRequiredToRestoreResourcesApp)
            {
                GUILayout.Space(-4);
                GUILayout.Label("Stock App restored on scene change", Styles.styleTextYellowBold);
                GUILayout.Space(4);
            }

            //else
            //{
            //    if (GUILayout.Button(new GUIContent("Click for Common Toolbar Info", "Click to open your browser and find out more about the Common Toolbar"), Styles.styleTextCenterGreen))
            //        Application.OpenURL("http://forum.kerbalspaceprogram.com/threads/60863");
            //}
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //Icons
            GUILayout.BeginHorizontal(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Label(new GUIContent("Iconsets:","Select the order of priority for choosing icons. Highest priority to the left"), Styles.styleStageTextHead);
            GUILayout.Space(2);
            GUILayout.Label(new GUIContent("Separator:","Padding around resource separators"), Styles.styleStageTextHead);
            GUILayout.Label("Empties:", Styles.styleStageTextHead);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            for (int i = 0; i < settings.lstIconOrder.Count; i++)
            {
                if (i > 0)
                {
                    if (GUILayout.Button("<->", GUILayout.Width(30)))
                    {
                        String strTemp = settings.lstIconOrder[i];
                        settings.lstIconOrder[i] = settings.lstIconOrder[i - 1];
                        settings.lstIconOrder[i - 1] = strTemp;
                        Resources.SetIconOrder(settings);
                        settings.Save();
                    }
                }
                GUILayout.Label(Resources.IconOrderContent(settings.lstIconOrder[i]), Styles.styleTextCenter, GUILayout.Width(40));
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            settings.SpacerPadding = (Int32)GUILayout.HorizontalSlider(settings.SpacerPadding, 0, 5, GUILayout.Width(168+mbARP.windowMain.IconAlarmOffset));
            GUILayout.Space(3);
            GUILayout.Label(String.Format("{0}px",settings.SpacerPadding));
            GUILayout.EndHorizontal();

            if (DrawToggle(ref settings.HideEmptyResources, "Hide Empty Resources", Styles.styleToggle)) {
                settings.Save();
            }
            if (DrawToggle(ref settings.HideFullResources, "Hide Full Resources", Styles.styleToggle)) {
                settings.Save();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("After:");
            settings.HideAfter = (Int32)GUILayout.HorizontalSlider(settings.HideAfter, 0, 10, GUILayout.Width(128 + mbARP.windowMain.IconAlarmOffset));
            GUILayout.Space(3);
            GUILayout.Label(String.Format("{0} secs", settings.HideAfter));
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();



            if (settings.ButtonStyleChosen != ButtonStyleEnum.StockReplace)
            {
                //Visuals
                GUILayout.BeginHorizontal(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
                GUILayout.BeginVertical(GUILayout.Width(60));
                GUILayout.Label("Visuals:", Styles.styleStageTextHead);
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if (DrawToggle(ref settings.DisableHover, "Disable Show on Button Hover", Styles.styleToggle)) {
                    settings.Save();
                }

                //if (DrawToggle(ref settings.LockLocation, "Lock Window Position", Styles.styleToggle)) {
                if (DrawToggle(ref settings.LockLocation, "Lock Window Position", Styles.styleToggle))
                {
                    mbARP.windowMain.DragEnabled = !settings.LockLocation;
                    settings.Save();
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save Position"))
                {
                    settings.WindowPosition = mbARP.windowMain.WindowRect;
                    settings.Save();
                }
                if (GUILayout.Button("Reset Position"))
                    mbARP.blnResetWindow = true;
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            //Work out window height
            intStylingWindowHeightAdjustments = intBlizzyToolbarMissingHeight;
            if (settings.ButtonStyleToDisplay == ButtonStyleEnum.Launcher)
                intStylingWindowHeightAdjustments += 19;
            else if (settings.ButtonStyleToDisplay == ButtonStyleEnum.StockReplace)
                intStylingWindowHeightAdjustments -= 68;

            if (mbARP.SceneChangeRequiredToRestoreResourcesApp)
            {
                if (settings.ButtonStyleToDisplay == ButtonStyleEnum.Launcher || settings.ButtonStyleToDisplay == ButtonStyleEnum.StockReplace)
                    intStylingWindowHeightAdjustments += 25;
                else
                    intStylingWindowHeightAdjustments += 16;
            }

        }


        private void DrawWindow_Alarms()
        {

            GUILayout.BeginHorizontal(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginVertical(GUILayout.Width(70));
            GUILayout.Space(2);
            GUILayout.Label("Alarms:", Styles.styleStageTextHead);
            GUILayout.Space(-5);
            GUILayout.Label("Volume:", Styles.styleStageTextHead);
            GUILayout.Space(-3);
            GUILayout.Label("      Level:");
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Space(2);
            if (DrawToggle(ref settings.AlarmsEnabled, "Enable Alarms Functionality", Styles.styleToggle))
                settings.Save();
            if (DrawToggle(ref settings.AlarmsVolumeFromUI, "Use KSP UI Volume", Styles.styleToggle))
            {
                settings.Save();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (settings.AlarmsVolumeFromUI)
                GUILayout.HorizontalSlider((Int32)(GameSettings.UI_VOLUME*100), 0, 100,GUILayout.Width(160));
            else
                settings.AlarmsVolume = GUILayout.HorizontalSlider(settings.AlarmsVolume * 100, 0, 100, GUILayout.Width(160)) / 100;
            GUILayout.Label(KSPAlternateResourcePanel.audioController.VolumePct.ToString() + "%");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Label("Sounds",Styles.styleStageTextHead);
            GUILayout.BeginVertical(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Warning:", GUILayout.Width(70));
            ddlSettingsAlarmsWarning.DrawButton();
            DrawTestSoundButton(mbARP.clipAlarmsWarning, settings.AlarmsWarningRepeats);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Repeat:", GUILayout.Width(70));
            //settings.AlarmsWarningRepeats = (Int32)GUILayout.HorizontalSlider(settings.AlarmsWarningRepeats, 1, 6, GUILayout.Width(130));
            if (DrawHorizontalSlider(ref settings.AlarmsWarningRepeats, 1, 6, GUILayout.Width(130)))
                settings.Save();
            GUILayout.Space(3);
            GUILayout.Label (settings.AlarmsWarningRepeatsText);
            GUILayout.EndHorizontal();

            GUILayout.Label("", Styles.styleSeparatorH);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Alert:", GUILayout.Width(70));
            ddlSettingsAlarmsAlert.DrawButton();
            DrawTestSoundButton(mbARP.clipAlarmsAlert, settings.AlarmsAlertRepeats);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Repeat:", GUILayout.Width(70));
            //settings.AlarmsAlertRepeats = (Int32)GUILayout.HorizontalSlider(settings.AlarmsAlertRepeats, 1, 6, GUILayout.Width(130));
            if (DrawHorizontalSlider(ref settings.AlarmsAlertRepeats,1,6, GUILayout.Width(130)))
                settings.Save();
            GUILayout.Space(3);
            GUILayout.Label(settings.AlarmsAlertRepeatsText);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DrawTestSoundButton(AudioClip clip, Int32 Repeats)
        {
            Boolean blnStop = false;
            GUIContent btn = new GUIContent(Resources.btnPlay, "Test Sound");
            if (KSPAlternateResourcePanel.audioController.isClipPlaying(clip))
            {
                btn = new GUIContent(Resources.btnStop, "StopPlaying");
                blnStop=true;
            }
            if (GUILayout.Button(btn, GUILayout.Width(20)))
            {
                if (blnStop)
                    KSPAlternateResourcePanel.audioController.Stop();
                else
                    KSPAlternateResourcePanel.audioController.Play(clip, Repeats);
            }
        }

        private void DrawWindow_Staging()
        {
            //Staging
            GUILayout.BeginHorizontal(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth), GUILayout.Height(64));//, GUILayout.Height(84));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Label("Staging:", Styles.styleStageTextHead);
            //if (settings.StagingEnabled)
            //{
            //    GUILayout.Space(34);
            //    GUILayout.Label("Alt+L:", Styles.styleStageTextHead.PaddingChangeBottom(-5));
            //}
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (DrawToggle(ref settings.StagingEnabled, "Staging Enabled", Styles.styleToggle)) 
                settings.Save();
            if (settings.StagingEnabled)
            {
                if (DrawToggle(ref settings.StagingEnabledInMapView, "Allow Staging in Mapview", Styles.styleToggle))
                    settings.Save();
                if (settings.StagingEnabledInMapView)
                    if (DrawToggle(ref settings.StagingEnabledSpaceInMapView, "Allow Space Bar in Mapview", Styles.styleToggle) )
                        settings.Save();
                //if (DrawToggle(ref settings.StagingIgnoreStageLock, "Ignore Keyboard Stage Lock", Styles.styleToggle))
                //    settings.Save();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //AutoStaging
            if (settings.StagingEnabled)
            {
                GUILayout.BeginHorizontal(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth), GUILayout.Height(50));
                GUILayout.BeginVertical(GUILayout.Width(60));
                GUILayout.Label("Auto:", Styles.styleStageTextHead);
                if (settings.AutoStagingEnabled)
                {
                    GUILayout.Space(-2);
                    GUILayout.Label("Delay:", Styles.styleStageTextHead.PaddingChangeBottom(-5));
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if (DrawToggle(ref settings.AutoStagingEnabled, "Auto Staging Enabled", Styles.styleToggle))
                    settings.Save();
                if (settings.AutoStagingEnabled)
                {
                    Single AutoStagingDelay = (Single)settings.AutoStagingDelayInTenths / 10;
                    GUILayout.BeginHorizontal();
                    if (DrawHorizontalSlider(ref AutoStagingDelay, 0.1f, 3f, GUILayout.Width(148 + mbARP.windowMain.IconAlarmOffset)))
                    {
                        settings.AutoStagingDelayInTenths = (Int32)(AutoStagingDelay * 10);
                        settings.Save();
                    }
                    GUILayout.Label(String.Format("{0:0.0} sec", AutoStagingDelay), GUILayout.Width(50));
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        private void DrawWindow_About()
        {
            //Update check Area to Add
            //Update Check Area
            GUILayout.Label("Version Check", Styles.styleStageTextHead);

            GUILayout.BeginVertical(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(3);
            if (DrawToggle(ref settings.DailyVersionCheck, "Check Version Daily",Styles.styleToggle))
                settings.Save();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Check Version Now"))
            {
                settings.VersionCheck(this, true);
                //Hide the flag as we already have the window open;
                settings.VersionAttentionFlag = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(160));
            GUILayout.Space(4);
            GUILayout.Label("Last Check Attempt:");
            GUILayout.Label("Current Version:");
            GUILayout.Label("Last Version from Web:");
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label(settings.VersionCheckDate_AttemptString, Styles.styleTextGreen);
            GUILayout.Label(settings.Version, Styles.styleTextGreen);

            if (settings.VersionCheckRunning)
            {
                Int32 intDots = Convert.ToInt32(Math.Truncate(DateTime.Now.Millisecond / 250d)) + 1;
                GUILayout.Label(String.Format("{0} Checking", new String('.', intDots)), Styles.styleTextYellowBold);
            }
            else
            {
                if (settings.VersionAvailable)
                    GUILayout.Label(String.Format("{0} @ {1}", settings.VersionWeb, settings.VersionCheckDate_SuccessString), Styles.styleTextYellowBold);
                else
                    GUILayout.Label(String.Format("{0} @ {1}", settings.VersionWeb, settings.VersionCheckDate_SuccessString), Styles.styleTextGreen);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (settings.VersionAvailable)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(80);
                if(GUILayout.Button("Updated Version Available", Styles.styleTextYellowBold))
                    Application.OpenURL("https://github.com/TriggerAu/AlternateResourcePanel/releases"); 
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();


            //About Area
            GUILayout.BeginVertical(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            //GUILayout.Label("Written by:", Styles.styleStageTextHead);
            GUILayout.Label("Documentation and Links:", Styles.styleStageTextHead);
            GUILayout.Label("Source Code / Downloads:", Styles.styleStageTextHead);
            GUILayout.Label("Forum Page:", Styles.styleStageTextHead);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label("Trigger Au",KACResources.styleContent);
            if (GUILayout.Button("Click Here", Styles.styleTextCenterGreen))
                Application.OpenURL("https://sites.google.com/site/kspalternateresourcepanel/");
            if (GUILayout.Button("Click Here", Styles.styleTextCenterGreen))
                Application.OpenURL("https://github.com/TriggerAu/AlternateResourcePanel/");
            if (GUILayout.Button("Click Here", Styles.styleTextCenterGreen))
                Application.OpenURL("http://forum.kerbalspaceprogram.com/threads/60227-KSP-Alternate-Resource-Panel");

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }



        internal override void OnGUIOnceOnly()
        {
            ddlManager.DropDownGlyphs = new GUIContentWithStyle(Resources.btnDropDown, Styles.styleDropDownGlyph);
            ddlManager.DropDownSeparators = new GUIContentWithStyle("", Styles.styleSeparatorV);
        }

        internal void UpdateWindowRect()
        {
            Rect rectWindowMain = mbARP.windowMain.WindowRect;
            Boolean blnKSPStyle = (SkinsLibrary.CurrentSkin.name == "Default");
            Rect rectWindowSettings = new Rect(rectWindowMain) { y = rectWindowMain.y + rectWindowMain.height - 2, height = WindowHeight };

            if (!blnKSPStyle) { rectWindowSettings.y += 1; }

            //Nowfit it in the screen and move it around as the main window moves around 
            if (Screen.height < rectWindowSettings.y + rectWindowSettings.height)
            {
                if (0 < rectWindowSettings.x - rectWindowSettings.width)
                {
                    rectWindowSettings.x = rectWindowMain.x - rectWindowMain.width + 2;
                    if (!blnKSPStyle) rectWindowSettings.x -= 1;
                    rectWindowSettings.y = Mathf.Clamp(rectWindowMain.y, 0, Screen.height - rectWindowSettings.height + 1);
                }
                else //if (Screen.width < SettingsWindowPos.x + SettingsWindowPos.width)
                {
                    rectWindowSettings.x = rectWindowMain.x + rectWindowMain.width - 2;
                    if (!blnKSPStyle) rectWindowSettings.x += 1;
                    rectWindowSettings.y = Mathf.Clamp(rectWindowMain.y, 0, Screen.height - rectWindowSettings.height + 1);
                }
            }
            this.WindowRect = rectWindowSettings.ClampToScreen(ClampToScreenOffset);
        }

        //internal GUIStyle CreateSolidBoxStyle(Color32 TextColor, Color32 BackgroundColor, RectOffset Padding)
        //{
        //    GUIStyle styleReturn = new GUIStyle();
        //    styleReturn.normal.background = CreateBoxTexture(BackgroundColor);
        //    return styleReturn;
        //}

        //internal GUIStyle CreateBoxStyle(Int32 Width, Int32 Height, Color32 Background, Color32 Border, Int32 BorderWidth = 1)
        //{
        //    GUIStyle styleReturn = new GUIStyle();
        //    styleReturn.normal.background = CreateBoxTexture(Width, Height, Background, Border, BorderWidth);
        //    styleReturn.border = new RectOffset(BorderWidth, BorderWidth, BorderWidth, BorderWidth);
        //    styleReturn.padding = new RectOffset(BorderWidth, BorderWidth, BorderWidth, BorderWidth);
        //    return styleReturn;
        //}

        //internal Texture2D CreateBoxTexture(Int32 Width, Int32 Height, Color32 Background)
        //{
        //    return CreateBoxTexture(Width, Height, Background, Background,0);
        //}
        //internal Texture2D CreateBoxTexture(Int32 Width, Int32 Height, Color32 Background, Color32 Border, Int32 BorderWidth = 1)
        //{
        //    //clamp the borderwidth value
        //    BorderWidth = Mathf.Clamp(BorderWidth, 0, 100);
        //    Texture2D texReturn = new Texture2D(Width, Height, TextureFormat.ARGB32, false);

        //    for (int i = 0; i < texReturn.width; i++)
        //    {
        //        for (int k = 0; k < texReturn.height; k++)
        //        {
        //            if (i<=BorderWidth || i>(texReturn.width-BorderWidth) ||
        //                k<=BorderWidth || k>(texReturn.height-BorderWidth))
        //            {
        //                texReturn.SetPixel(i,k,Border);
        //            } else {
        //                texReturn.SetPixel(i,k,Background);
        //            }
        //        }
        //    }
        //    texReturn.Apply();
        //    return texReturn;
        //}
    }
}
