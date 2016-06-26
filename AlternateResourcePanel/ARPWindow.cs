using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using KSP.UI.Screens;
using UnityEngine;
using KSPPluginFramework;

namespace KSPAlternateResourcePanel
{
    class ARPWindow: MonoBehaviourWindowPlus
    {
        //internal ARPWindow()
        //{
        //    //WindowContent = new GUIContent("HEADER");
        //    WindowRect = new Rect(100, 100, 100, 100);
        //}
        //internal ARPWindow(String text) : base(text)
        //{
        //    WindowRect = new Rect(100, 100, 100, 100);
        //}

        //TODO: Look at using this
        //  http://answers.unity3d.com/questions/445444/add-component-in-one-line-with-parameters.html

        //Parent Objects
        internal KSPAlternateResourcePanel mbARP;
        private ARPWindowSettings windowSettings;
        private Settings settings;

        //Working Objects
        private ARPResourceList lstResources;
        private ARPResourceList lstResourcesLastStage;
        private PartResourceVisibleList SelectedResources;
        internal Int32 intLineHeight = 20;

        internal override void Awake()
        {
            TooltipMouseOffset = new Vector2d(-10, 10);

            onWindowMoveComplete += ARPWindow_onWindowMoveComplete;
        }
        internal override void OnDestroy()
        {
            base.OnDestroy();

            onWindowMoveComplete -= ARPWindow_onWindowMoveComplete;
        }

        void ARPWindow_onWindowMoveComplete(MonoBehaviourWindow sender)
        {
            settings.WindowPosition = WindowRect;
            settings.Save();
        }

        private void SetLocalVariables()
        {
            windowSettings = mbARP.windowSettings;
            settings = KSPAlternateResourcePanel.settings;
            lstResources = mbARP.lstResourcesVessel;
            lstResourcesLastStage = mbARP.lstResourcesLastStage;
            SelectedResources = mbARP.SelectedResources;
        }

        //public Rect rectIcon;
        internal Int32 IconAlarmOffset = 12;
        internal Int32 Icon2BarOffset_Left = 40;
        internal Int32 Icon2BarOffset_Right = 40 + 140;

        internal override void DrawWindow(Int32 id)
        {
            SetLocalVariables();

            try { GUILayout.BeginVertical(); }
            catch (Exception) { LogFormatted("DrawWindow: GUILayout not ready yet"); return; }

            if (mbARP.lstResourcesToDisplay.Count == 0)
            {
                GUILayout.Label("No current resources configured to display");
            }
            else
            {
                Int32 ResourceID;
                Rect rectBar;
                for (int i = 0; i < mbARP.lstResourcesToDisplay.Count; i++)
                {
                    ResourceID = mbARP.lstResourcesToDisplay[i];
                    //Is it a separator - draw and skip?
                    if (ResourceID == 0)
                    {
                        GUILayout.Space(3 + settings.SpacerPadding);
                        GUILayout.Label("", Styles.styleSeparatorH, GUILayout.Width(WindowRect.width - 15), GUILayout.Height(2));
                        GUILayout.Space(settings.SpacerPadding);
                        continue;
                    }

                    //add space at top of window
                    if (i > 0) GUILayout.Space(4);

                    GUILayout.BeginHorizontal();

                    //add title
                    Rect rectIcon = Drawing.DrawResourceIcon(lstResources[ResourceID].ResourceDef.name);

                    //If the global alarms setting is on
                    if (settings.AlarmsEnabled)
                    {
                        GUILayout.Space(1);
                        //work out the alarm ico to display
                        GUIContent contAlarm = new GUIContent(Resources.btnAlarm);
                        if (settings.Resources[ResourceID].AlarmEnabled)
                        {
                            contAlarm.image = Resources.btnAlarmEnabled;
                            switch (lstResources[ResourceID].MonitorState)
                            {
                                case ARPResource.MonitorStateEnum.Alert:
                                    if (lstResources[ResourceID].AlarmState!= ARPResource.AlarmStateEnum.Unacknowledged || DateTime.Now.Millisecond < 500)
                                        contAlarm.image = Resources.btnAlarmAlert;
                                    break;
                                case ARPResource.MonitorStateEnum.Warn:
                                    if (lstResources[ResourceID].AlarmState != ARPResource.AlarmStateEnum.Unacknowledged || DateTime.Now.Millisecond < 500)
                                        contAlarm.image = Resources.btnAlarmWarn;
                                    break;
                                default:
                                    break;
                            }
                        }
                        //Draw the button - if the alarm is unacknowledged then acknowledge, else toggle alarm status
                        if (GUILayout.Button(contAlarm, Styles.styleAlarmButton))
                        {
                            if (lstResources[ResourceID].AlarmState == ARPResource.AlarmStateEnum.Unacknowledged)
                                lstResources[ResourceID].SetAlarmAcknowledged();
                            else
                                settings.Resources[ResourceID].AlarmEnabled = !settings.Resources[ResourceID].AlarmEnabled;
                        }
                    }
                    //Is this resource selected
                    Boolean Highlight = SelectedResources.ContainsKey(ResourceID) && SelectedResources[ResourceID].AllVisible;

                    //For resources with no stage specifics - or the Resources is set to split display and values are different for all vessel and flow enabled ones
                    if (!settings.SplitLastStage || !lstResources[ResourceID].ResourceConfig.SplitLastStage || settings.ShowBase ||
                        (lstResources[ResourceID].ResourceDef.resourceFlowMode == ResourceFlowMode.ALL_VESSEL ||
                            lstResources[ResourceID].ResourceDef.resourceFlowMode == ResourceFlowMode.STAGE_PRIORITY_FLOW) &&
                        (( !settings.Resources[ResourceID].ShowReserveLevels )
                            || (!lstResourcesLastStage.ContainsKey(ResourceID)))
                        )
                    {
                        //full width bar
                        rectBar = Drawing.CalcBarRect(rectIcon, Icon2BarOffset_Left, 275, 15);  //was 245
                        if (Drawing.DrawResourceBar(rectBar, lstResources[ResourceID], Styles.styleBarGreen_Back, Styles.styleBarGreen, Styles.styleBarGreen_Thin, settings.ShowRates, Highlight, Styles.styleBarHighlight))
                            //MonoBehaviourExtended.LogFormatted_DebugOnly("Clicked");
                            SelectedResources.TogglePartResourceVisible(ResourceID);
                    }
                    else
                    {
                        Int32 FullVesselBarOffset = settings.StageBarOnRight ? Icon2BarOffset_Left : Icon2BarOffset_Right;
                        Int32 StageBarOffset = settings.StageBarOnRight ? Icon2BarOffset_Right : Icon2BarOffset_Left;

                        //need full Vessel and current stage bars
                        rectBar = Drawing.CalcBarRect(rectIcon, FullVesselBarOffset, 135, 15);  //was 120
                        if (Drawing.DrawResourceBar(rectBar, lstResources[ResourceID], Styles.styleBarGreen_Back, Styles.styleBarGreen, Styles.styleBarGreen_Thin, settings.ShowRates, Highlight, Styles.styleBarHighlight))
                            SelectedResources.TogglePartResourceVisible(ResourceID);

                        //get last stage of this resource and set it
                        if (lstResourcesLastStage.ContainsKey(ResourceID))
                        {
                            Highlight = SelectedResources.ContainsKey(ResourceID) && SelectedResources[ResourceID].LastStageVisible;
                            rectBar = Drawing.CalcBarRect(rectIcon, StageBarOffset, 135, 15);  //was 120
                            if (Drawing.DrawResourceBar(rectBar, lstResourcesLastStage[ResourceID], Styles.styleBarBlue_Back, Styles.styleBarBlue, Styles.styleBarBlue_Thin, settings.ShowRates, Highlight, Styles.styleBarHighlight))
                                SelectedResources.TogglePartResourceVisible(ResourceID, true);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.BeginHorizontal();
            ////STAGING STUFF
            if (settings.StagingEnabled)
            {
                ////Keyboard Stage Lock
                //GUIContent contLock = new GUIContent(Resources.btnUnlock, "Keyboard Staging Unlocked\r\nClick to toggle");
                //if (FlightInputHandler.fetch.stageLock)
                //    contLock = new GUIContent(Resources.btnLock, "Keyboard Staging Locked\r\nClick to toggle");
                //if (GUILayout.Button(contLock, Styles.styleAlarmButton))
                //    FlightInputHandler.fetch.stageLock = !FlightInputHandler.fetch.stageLock;

                if (!settings.AutoStagingEnabled)
                {
                    //GUILayout.Label("Stage:", Styles.styleStageTextHead, GUILayout.Width(50));
                    if (GUILayout.Button("Stage:", Styles.styleStageTextHead, GUILayout.Width(50)))
                        settings.AutoStagingEnabled = !settings.AutoStagingEnabled;
                    GUIStyle styleStageNum = new GUIStyle(Styles.styleStageTextHead);
                    GUIContent contStageNum = new GUIContent(StageManager.CurrentStage.ToString());
                    //styleStageNum.normal.textColor=new Color(173,43,43);
                    //GUIContent contStageNum = new GUIContent(StageManager.CurrentStage.ToString(),"NO Active Engines");
                    //if (THERE ARE ACTIVE ENGINES IN STAGE)
                    //{
                    //contStageNum.tooltip="Active Engines";
                    styleStageNum.normal.textColor = new Color(117, 206, 60);
                    //}

                    GUILayout.Label(contStageNum, styleStageNum, GUILayout.Width(20));

                    if (settings.StagingEnabledInMapView || !MapView.MapIsEnabled)
                    {
                        if (mbARP.blnVesselIsControllable) {
                            if (GUILayout.Button("Activate Stage", "ButtonGeneral", GUILayout.Width(100)))
                                StageManager.ActivateNextStage();
                            GUILayout.Space(21 + IconAlarmOffset);
                            //GUILayout.Space(51 + IconAlarmOffset);
                        }
                        else {
                            GUILayout.Label("No Vessel Control", GUILayout.Width(120));
                            GUILayout.Space(1 + IconAlarmOffset);
                            //GUILayout.Space(31 + IconAlarmOffset);
                        }
                    }
                    //GUILayout.Space(48 + IconAlarmOffset);
                    //GUILayout.Space(21 + IconAlarmOffset);
                }
                else
                {
                    if (GUILayout.Button("Auto:", Styles.styleStageTextHead, GUILayout.Width(50)))
                        settings.AutoStagingEnabled = !settings.AutoStagingEnabled;
                    GUILayout.Label(StageManager.CurrentStage.ToString(),Styles.styleStageTextHead, GUILayout.Width(20));
                    GUILayout.Label("to", Styles.styleStageTextHead, GUILayout.Width(30));
                    GUILayout.Label(mbARP.AutoStagingTerminateAt.ToString(), Styles.styleStageTextHead, GUILayout.Width(30));
                    DrawHorizontalSlider(ref mbARP.AutoStagingTerminateAt, 0, mbARP.AutoStagingMaxStage);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();

                    String  strButtonArm = "Arm";
                    GUIStyle styleArm = new GUIStyle(SkinsLibrary.CurrentSkin.button) { fontStyle = FontStyle.Bold };
                    if (mbARP.AutoStagingArmed)
                    {
                        strButtonArm = "Disarm";
                        styleArm.normal.textColor = styleArm.hover.textColor = new Color32(207,31,31,255) ;
                    }
                    else
                    {
                        styleArm.normal.textColor = styleArm.hover.textColor = new Color32(183,254,0,255);
                    }

                    if (GUILayout.Button(strButtonArm,styleArm ,GUILayout.Width(75)))
                        mbARP.AutoStagingArmed = !mbARP.AutoStagingArmed;
                    
                    GUIStyle StatusStyle = new GUIStyle(SkinsLibrary.CurrentSkin.label) ;
                    StatusStyle.normal.textColor = mbARP.AutoStagingStatusColor;
                    //GUILayout.Label(mbARP.AutoStagingStatus, StatusStyle, GUILayout.Width(147 + IconAlarmOffset));
                    GUILayout.Label(mbARP.AutoStagingStatus, StatusStyle, GUILayout.Width(120 + IconAlarmOffset));
                    //GUILayout.Label(mbARP.AutoStagingStatus, StatusStyle, GUILayout.Width(150 + IconAlarmOffset));
                }
            }
            else
            {
                //GUILayout.Space(234 + IconAlarmOffset);
                GUILayout.Space(207 + IconAlarmOffset);
                //GUILayout.Space(237 + IconAlarmOffset);
            }

            // ShowBase Button
            Boolean blnShowBase = settings.ShowBase;
            if (DrawToggle(ref blnShowBase, new GUIContent(Resources.btnViewBaseActive, blnShowBase ? "Back to Vessel Display" : "Show Base Display\r\nAll resource within 2km of active vessel"), SkinsLibrary.GetStyle(SkinsLibrary.CurrentSkin, "ButtonToggle").PaddingChange(1), GUILayout.Width(23)))
            {
                settings.ShowBase = blnShowBase;
            }
            //if (GUILayout.Button(new GUIContent(settings.ShowBase ? Resources.btnViewBaseActive : Resources.btnViewBase, "Toggle Vessel/Base Display"), SkinsLibrary.CurrentSkin.button.PaddingChange(1), GUILayout.Width(23)))
            //{
            //    settings.ShowBase = !settings.ShowBase;
            //}

            // ShowTime Button
            Boolean blnShowTimeRem = settings.ShowTimeRem;
            if (DrawToggle(ref blnShowTimeRem, new GUIContent(Resources.btnViewTimes, blnShowTimeRem ? "Hide time Remianing": "Show Time Remaining"), SkinsLibrary.GetStyle(SkinsLibrary.CurrentSkin,"ButtonToggle").PaddingChange(1), GUILayout.Width(23))) {
                settings.ShowTimeRem = blnShowTimeRem;
            }
            //if (GUILayout.Button(new GUIContent(Resources.btnViewTimes, "Toggle Time Remaining"), SkinsLibrary.CurrentSkin.button.PaddingChange(1), GUILayout.Width(23)))
            //{
            //    settings.ShowTimeRem = !settings.ShowTimeRem;
            //}

            // ShowAll Button
            Boolean blnToggleHidden = KSPAlternateResourcePanel.ShowAll;
            if (DrawToggle(ref blnToggleHidden, new GUIContent(Resources.btnViewAll, "Toggle Hidden Resources"), SkinsLibrary.GetStyle(SkinsLibrary.CurrentSkin, "ButtonToggle").PaddingChange(1), GUILayout.Width(23)))
            {
                KSPAlternateResourcePanel.ShowAll = blnToggleHidden;
            }
            //if (GUILayout.Button(new GUIContent(Resources.btnViewAll, "Toggle Hidden Resources"), SkinsLibrary.CurrentSkin.button.PaddingChange(1), GUILayout.Width(23)))
            //{
            //    KSPAlternateResourcePanel.ShowAll = !KSPAlternateResourcePanel.ShowAll;
            //}

            //Settings Toggle button
            GUIContent btnMinMax = new GUIContent(Resources.btnChevronDown, "Show Settings...");
            if (windowSettings.Visible) { 
                btnMinMax.image = Resources.btnChevronUp; btnMinMax.tooltip = "Hide Settings";
            }
            else if (settings.VersionAttentionFlag && DateTime.Now.Millisecond < 500) {
                btnMinMax.image = Resources.btnSettingsAttention; 
            }
            
            if (settings.VersionAttentionFlag) btnMinMax.tooltip = "Updated Version Available - " + btnMinMax.tooltip;
            
            if (GUILayout.Button(btnMinMax,"ButtonSettings",GUILayout.Width(23)))
            {
                windowSettings.Visible = !windowSettings.Visible;
                if (windowSettings.Visible && settings.VersionAttentionFlag)
                {
                    windowSettings.ddlSettingsTab.SelectedIndex = (Int32)ARPWindowSettings.SettingsTabs.About;
                    settings.VersionAttentionFlag = false;
                }
                settings.Save();
            }

            GUILayout.EndHorizontal();

            //End window layout
            GUILayout.EndVertical();

            //If settings window is visible then position it accordingly
            if (mbARP.windowSettings.Visible)
                mbARP.windowSettings.UpdateWindowRect();
        }

        internal override void OnGUIEvery()
        {
            base.OnGUIEvery();
        }
    }

}
