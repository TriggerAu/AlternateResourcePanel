using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;
using KSP.Localization;

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

        // Localization Strings
        private static string SETTINGSECTION = Localizer.Format("#ARP_LOC_067");
        private static string RATES = Localizer.Format("#ARP_LOC_068");
        private static string CALCBY = Localizer.Format("#ARP_LOC_069");
        private static string RATESTYLE = Localizer.Format("#ARP_LOC_070");
        private static string SHOWRATECHANGE = Localizer.Format("#ARP_LOC_071");
        private static string SHOWRATEFORPART = Localizer.Format("#ARP_LOC_072");
        private static string REALTIME = Localizer.Format("#ARP_LOC_073");
        private static string STAGEBARS = Localizer.Format("#ARP_LOC_074");
        private static string BARSPOS = Localizer.Format("#ARP_LOC_075");
        private static string ENABLED = Localizer.Format("#ARP_LOC_076");
        private static string SINGLEBAR = Localizer.Format("#ARP_LOC_077");
        private static string ONLEFT = Localizer.Format("#ARP_LOC_078");
        private static string ONRIGHT = Localizer.Format("#ARP_LOC_079");
        private static string RESOURCES = Localizer.Format("#ARP_LOC_080");
        private static string CONFIGRESOURCESETTING = Localizer.Format("#ARP_LOC_081");
        private static string SHOWWINDOWALARM = Localizer.Format("#ARP_LOC_082");
        private static string POPWINDOW = Localizer.Format("#ARP_LOC_083");
        private static string STYLING = Localizer.Format("#ARP_LOC_084");
        private static string BUTTON = Localizer.Format("#ARP_LOC_085");
        private static string NOTINSTALLTOOLBAR = Localizer.Format("#ARP_LOC_086");
        private static string OPENBROWSER = Localizer.Format("#ARP_LOC_087");
        private static string HIDEWHNESHOW = Localizer.Format("#ARP_LOC_088");
        private static string HIDEARP = Localizer.Format("#ARP_LOC_089");
        private static string RESTOREDONCHANGE = Localizer.Format("#ARP_LOC_090");
        private static string ICONSETS = Localizer.Format("#ARP_LOC_091");
        private static string SELECTICONORDER = Localizer.Format("#ARP_LOC_092");
        private static string SEPARATOR = Localizer.Format("#ARP_LOC_093");
        private static string PADDING = Localizer.Format("#ARP_LOC_094");
        private static string EMPTIES = Localizer.Format("#ARP_LOC_095");
        private static string HIDEEMPTYRES = Localizer.Format("#ARP_LOC_096");
        private static string HIDEFULLRES = Localizer.Format("#ARP_LOC_097");
        private static string AFTER = Localizer.Format("#ARP_LOC_098");
        private static string SECS = Localizer.Format("#ARP_LOC_099");
        private static string VISUALS = Localizer.Format("#ARP_LOC_100");
        private static string DISABLEHOVER = Localizer.Format("#ARP_LOC_101");
        private static string LOCKPOS = Localizer.Format("#ARP_LOC_102");
        private static string SAVEPOS = Localizer.Format("#ARP_LOC_103");
        private static string RESETPOS = Localizer.Format("#ARP_LOC_104");
        private static string ALARMS = Localizer.Format("#ARP_LOC_105");
        private static string VOLUME = Localizer.Format("#ARP_LOC_106");
        private static string LEVEL = Localizer.Format("#ARP_LOC_107");
        private static string ENABLEALARMS = Localizer.Format("#ARP_LOC_108");
        private static string USEKSPUIVOLUME = Localizer.Format("#ARP_LOC_109");
        private static string SOUNDS = Localizer.Format("#ARP_LOC_110");
        private static string WARNING = Localizer.Format("#ARP_LOC_111");
        private static string REPEAT = Localizer.Format("#ARP_LOC_112");
        private static string ALERT = Localizer.Format("#ARP_LOC_113");
        private static string STAGING = Localizer.Format("#ARP_LOC_114");
        private static string AUTO = Localizer.Format("#ARP_LOC_115");
        private static string DELAY = Localizer.Format("#ARP_LOC_116");
        private static string AUTOSTAGING = Localizer.Format("#ARP_LOC_117");
        private static string VERSIONCHECK = Localizer.Format("#ARP_LOC_118");
        private static string DAILYCHECK = Localizer.Format("#ARP_LOC_119");
        private static string NOWCHECK = Localizer.Format("#ARP_LOC_120");
        private static string LASTCHECK = Localizer.Format("#ARP_LOC_121");
        private static string CURVERSION = Localizer.Format("#ARP_LOC_122");
        private static string LASTVERSION = Localizer.Format("#ARP_LOC_123");
        private static string CHECKING = Localizer.Format("#ARP_LOC_124");
        private static string UPDATEDAVAILABLE = Localizer.Format("#ARP_LOC_125");
        private static string DOCUMENT = Localizer.Format("#ARP_LOC_126");
        private static string SOURCECODE = Localizer.Format("#ARP_LOC_127");
        private static string FORUMPAGE = Localizer.Format("#ARP_LOC_128");
        private static string CLICKHERE = Localizer.Format("#ARP_LOC_129");
        private static string TESTSOUND = Localizer.Format("#ARP_LOC_130");
        private static string STOPPLAYSOUND = Localizer.Format("#ARP_LOC_131");
        private static string STAGINGENABLED = Localizer.Format("#ARP_LOC_132");
        private static string MAPVIEWSTAGING = Localizer.Format("#ARP_LOC_133");
        private static string MAPVIEWSPACEBAR = Localizer.Format("#ARP_LOC_134");

        internal enum SettingsTabs
        {
            [Description("#ARP_LOC_058")] General, // General Properties
            [Description("#ARP_LOC_059")]    Styling, // Styling/Visuals
            [Description("#ARP_LOC_060")]   Alarms, // Alarm Properties
            [Description("#ARP_LOC_061")]   Staging, // Staging Controls
            [Description("#ARP_LOC_062")]   About, // About...
        }


        internal enum ButtonStyleEnum
        {
            [Description("#ARP_LOC_063")]                       Basic, // Basic button
            [Description("#ARP_LOC_064")]       Toolbar, // Common Toolbar (by Blizzy78)
            [Description("#ARP_LOC_065")]            Launcher, // KSP App Launcher Button
            [Description("#ARP_LOC_066")]        StockReplace, // Replace Stock Resources App
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
            GUILayout.Label(SETTINGSECTION, Styles.styleStageTextHead,GUILayout.Width(140)); // "Settings Section"
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
            GUILayout.Label($"{RATES}:", Styles.styleStageTextHead); // Rates
            GUILayout.Space(13);
            GUILayout.Label($"{CALCBY}:", Styles.styleStageTextHead); // Calc By
            GUILayout.Label($"{RATESTYLE}:", Styles.styleStageTextHead); // Rate Style
            GUILayout.EndVertical();
            GUILayout.BeginVertical();

            if (DrawToggle(ref settings.ShowRates, SHOWRATECHANGE, Styles.styleToggle)) // "Show Rate Change Values"
                settings.Save();
            //if (DrawToggle(ref settings.ShowRatesTimeRem, "Toggle Time Remaining", Styles.styleToggle))
            //    settings.Save();
            if (DrawToggle(ref settings.ShowRatesForParts, SHOWRATEFORPART, Styles.styleToggle)) // "Show Rates for Parts"
            {
                settings.Save();
            }

            GUILayout.BeginHorizontal();
            if (DrawToggle(ref settings.RatesUseUT, "UT", Styles.styleToggle,GUILayout.Width(60)))
                settings.Save();
            Boolean NotUT = !settings.RatesUseUT;
            if (DrawToggle(ref NotUT, REALTIME, Styles.styleToggle)) // "Real Time"
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
            GUILayout.Label($"{STAGEBARS}:", Styles.styleStageTextHead); // Stage Bars
            if (settings.SplitLastStage)
            {
                GUILayout.Space(-6);
                GUILayout.Label($"{BARSPOS}:", Styles.styleStageTextHead); // Bars Pos
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Space(2);
            if (DrawToggle(ref settings.SplitLastStage, new GUIContent(ENABLED, SINGLEBAR), Styles.styleToggle)) // "Enabled""Turn this off to show single green bars and no last stage separation."
                settings.Save();
            if (settings.SplitLastStage)
            {
                GUILayout.BeginHorizontal();
                Boolean NotRight = !settings.StageBarOnRight;
                if (DrawToggle(ref NotRight, ONLEFT, Styles.styleToggle, GUILayout.Width(90))) // "On Left"
                {
                    settings.StageBarOnRight = !settings.StageBarOnRight; 
                    settings.Save();
                }
                if (DrawToggle(ref settings.StageBarOnRight, ONRIGHT, Styles.styleToggle)) // "On Right"
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
            GUILayout.Label($"{RESOURCES}:", Styles.styleStageTextHead); // Resources
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (DrawButton(CONFIGRESOURCESETTING)) // "Configure Resource Settings"
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
            if (DrawToggle(ref settings.ShowWindowOnResourceMonitor, new GUIContent(SHOWWINDOWALARM, POPWINDOW),Styles.styleToggle)) // "Show Window on Alarm""If an alarm is exceeded then popup the window if it is not already visible. Will hide again when acknowledged."
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
            GUILayout.Label($"{STYLING}:", Styles.styleStageTextHead); // Styling
            GUILayout.Label($"{BUTTON}:", Styles.styleStageTextHead); // Button
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            ddlSettingsSkin.DrawButton();

            ddlSettingsButtonStyle.DrawButton();

            intBlizzyToolbarMissingHeight = 0;
            if (!settings.BlizzyToolbarIsAvailable)
            {
                if (settings.ButtonStyleChosen == ButtonStyleEnum.Toolbar)
                {
                    if (GUILayout.Button(new GUIContent(NOTINSTALLTOOLBAR, OPENBROWSER), Styles.styleTextCenterGreen)) // "Not Installed. Click for Toolbar Info""Click to open your browser and find out more about the Common Toolbar"
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
                if (DrawToggle(ref settings.AppLauncherMutuallyExclusive, new GUIContent(HIDEWHNESHOW, HIDEARP), Styles.styleToggle)) // "Hide when other Apps show""Hide the ARP when other stock Apps display (like the stock Resource App)"
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
                GUILayout.Label(RESTOREDONCHANGE, Styles.styleTextYellowBold); // "Stock App restored on scene change"
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
            GUILayout.Label(new GUIContent($"{ICONSETS}:",SELECTICONORDER), Styles.styleStageTextHead); // Iconsets"Select the order of priority for choosing icons. Highest priority to the left"
            GUILayout.Space(2);
            GUILayout.Label(new GUIContent($"{SEPARATOR}:",PADDING), Styles.styleStageTextHead); // Separator"Padding around resource separators"
            GUILayout.Label($"{EMPTIES}:", Styles.styleStageTextHead); // Empties
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

            if (DrawToggle(ref settings.HideEmptyResources, HIDEEMPTYRES, Styles.styleToggle)) { // "Hide Empty Resources"
                settings.Save();
            }
            if (DrawToggle(ref settings.HideFullResources, HIDEFULLRES, Styles.styleToggle)) { // "Hide Full Resources"
                settings.Save();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{AFTER}:"); // After
            settings.HideAfter = (Int32)GUILayout.HorizontalSlider(settings.HideAfter, 0, 10, GUILayout.Width(128 + mbARP.windowMain.IconAlarmOffset));
            GUILayout.Space(3);
            GUILayout.Label(String.Format("{0} {1}", settings.HideAfter, SECS)); // secs
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();



            if (settings.ButtonStyleChosen != ButtonStyleEnum.StockReplace)
            {
                //Visuals
                GUILayout.BeginHorizontal(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
                GUILayout.BeginVertical(GUILayout.Width(60));
                GUILayout.Label($"{VISUALS}:", Styles.styleStageTextHead); // Visuals
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if (DrawToggle(ref settings.DisableHover, DISABLEHOVER, Styles.styleToggle)) { // "Disable Show on Button Hover"
                    settings.Save();
                }

                //if (DrawToggle(ref settings.LockLocation, "Lock Window Position", Styles.styleToggle)) {
                if (DrawToggle(ref settings.LockLocation, LOCKPOS, Styles.styleToggle)) // "Lock Window Position"
                {
                    mbARP.windowMain.DragEnabled = !settings.LockLocation;
                    settings.Save();
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(SAVEPOS)) // "Save Position"
                {
                    settings.WindowPosition = mbARP.windowMain.WindowRect;
                    settings.Save();
                }
                if (GUILayout.Button(RESETPOS)) // "Reset Position"
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
            GUILayout.Label($"{ALARMS}:", Styles.styleStageTextHead); // Alarms
            GUILayout.Space(-5);
            GUILayout.Label($"{VOLUME}:", Styles.styleStageTextHead); // Volume
            GUILayout.Space(-3);
            GUILayout.Label($"      {LEVEL}:"); // Level
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Space(2);
            if (DrawToggle(ref settings.AlarmsEnabled, ENABLEALARMS, Styles.styleToggle)) // "Enable Alarms Functionality"
                settings.Save();
            if (DrawToggle(ref settings.AlarmsVolumeFromUI, USEKSPUIVOLUME, Styles.styleToggle)) // "Use KSP UI Volume"
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

            GUILayout.Label(SOUNDS,Styles.styleStageTextHead); // "Sounds"
            GUILayout.BeginVertical(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{WARNING}:", GUILayout.Width(70)); // Warning
            ddlSettingsAlarmsWarning.DrawButton();
            DrawTestSoundButton(mbARP.clipAlarmsWarning, settings.AlarmsWarningRepeats);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{REPEAT}:", GUILayout.Width(70)); // Repeat
            //settings.AlarmsWarningRepeats = (Int32)GUILayout.HorizontalSlider(settings.AlarmsWarningRepeats, 1, 6, GUILayout.Width(130));
            if (DrawHorizontalSlider(ref settings.AlarmsWarningRepeats, 1, 6, GUILayout.Width(130)))
                settings.Save();
            GUILayout.Space(3);
            GUILayout.Label (settings.AlarmsWarningRepeatsText);
            GUILayout.EndHorizontal();

            GUILayout.Label("", Styles.styleSeparatorH);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"{ALERT}:", GUILayout.Width(70)); // Alert
            ddlSettingsAlarmsAlert.DrawButton();
            DrawTestSoundButton(mbARP.clipAlarmsAlert, settings.AlarmsAlertRepeats);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{REPEAT}:", GUILayout.Width(70)); // Repeat
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
            GUIContent btn = new GUIContent(Resources.btnPlay, TESTSOUND); // "Test Sound"
            if (KSPAlternateResourcePanel.audioController.isClipPlaying(clip))
            {
                btn = new GUIContent(Resources.btnStop, STOPPLAYSOUND); // "StopPlaying"
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
            GUILayout.Label($"{STAGING}:", Styles.styleStageTextHead); // Staging
            //if (settings.StagingEnabled)
            //{
            //    GUILayout.Space(34);
            //    GUILayout.Label("Alt+L:", Styles.styleStageTextHead.PaddingChangeBottom(-5));
            //}
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (DrawToggle(ref settings.StagingEnabled, STAGINGENABLED, Styles.styleToggle)) // "Staging Enabled"
                settings.Save();
            if (settings.StagingEnabled)
            {
                if (DrawToggle(ref settings.StagingEnabledInMapView, MAPVIEWSTAGING, Styles.styleToggle)) // "Allow Staging in Mapview"
                    settings.Save();
                if (settings.StagingEnabledInMapView)
                    if (DrawToggle(ref settings.StagingEnabledSpaceInMapView, MAPVIEWSPACEBAR, Styles.styleToggle) ) // "Allow Space Bar in Mapview"
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
                GUILayout.Label($"{AUTO}:", Styles.styleStageTextHead); // Auto
                if (settings.AutoStagingEnabled)
                {
                    GUILayout.Space(-2);
                    GUILayout.Label($"{DELAY}:", Styles.styleStageTextHead.PaddingChangeBottom(-5)); // Delay
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if (DrawToggle(ref settings.AutoStagingEnabled, AUTOSTAGING, Styles.styleToggle)) // "Auto Staging Enabled"
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
                    GUILayout.Label(String.Format("{0:0.0} {1}", AutoStagingDelay, SECS), GUILayout.Width(50)); // sec
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
            GUILayout.Label(VERSIONCHECK, Styles.styleStageTextHead); // "Version Check"

            GUILayout.BeginVertical(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(3);
            if (DrawToggle(ref settings.DailyVersionCheck, DAILYCHECK,Styles.styleToggle)) // "Check Version Daily"
                settings.Save();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(NOWCHECK)) // "Check Version Now"
            {
                settings.VersionCheck(this, true);
                //Hide the flag as we already have the window open;
                settings.VersionAttentionFlag = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(160));
            GUILayout.Space(4);
            GUILayout.Label($"{LASTCHECK}:"); // Last Check Attempt
            GUILayout.Label($"{CURVERSION}:"); // Current Version
            GUILayout.Label($"{LASTVERSION}:"); // Last Version from Web
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label(settings.VersionCheckDate_AttemptString, Styles.styleTextGreen);
            GUILayout.Label(settings.Version, Styles.styleTextGreen);

            if (settings.VersionCheckRunning)
            {
                Int32 intDots = Convert.ToInt32(Math.Truncate(DateTime.Now.Millisecond / 250d)) + 1;
                GUILayout.Label(String.Format("{0} {1}", new String('.', intDots), CHECKING), Styles.styleTextYellowBold); // Checking
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
                if(GUILayout.Button(UPDATEDAVAILABLE, Styles.styleTextYellowBold)) // "Updated Version Available"
                    Application.OpenURL("https://github.com/TriggerAu/AlternateResourcePanel/releases"); 
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();


            //About Area
            GUILayout.BeginVertical(Styles.styleSettingsArea, GUILayout.Width(SettingsAreaWidth));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            //GUILayout.Label("Written by:", Styles.styleStageTextHead);
            GUILayout.Label($"{DOCUMENT}:", Styles.styleStageTextHead); // Documentation and Links
            GUILayout.Label($"{SOURCECODE}:", Styles.styleStageTextHead); // Source Code / Downloads
            GUILayout.Label($"{FORUMPAGE}:", Styles.styleStageTextHead); // Forum Page
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label("Trigger Au",KACResources.styleContent);
            if (GUILayout.Button(CLICKHERE, Styles.styleTextCenterGreen)) // "Click Here"
                Application.OpenURL("https://sites.google.com/site/kspalternateresourcepanel/");
            if (GUILayout.Button(CLICKHERE, Styles.styleTextCenterGreen)) // "Click Here"
                Application.OpenURL("https://github.com/TriggerAu/AlternateResourcePanel/");
            if (GUILayout.Button(CLICKHERE, Styles.styleTextCenterGreen)) // "Click Here"
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
