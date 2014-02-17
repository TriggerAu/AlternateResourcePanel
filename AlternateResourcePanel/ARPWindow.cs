using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPAlternateResourcePanel
{
    class ARPWindow: MonoBehaviourWindow
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
        internal Int32 Icon2BarOffset = 40;
        internal Int32 Icon2StageBarOffset = 40 + 125;

        internal override void DrawWindow(Int32 id)
        {
            SetLocalVariables();

            GUILayout.BeginVertical();
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
                        GUILayout.Space(3);
                        GUILayout.Label("", Styles.styleSeparatorH, GUILayout.Width(WindowRect.width - 15), GUILayout.Height(2));
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
                            switch (lstResources[ResourceID].MonitorWorstHealth)
                            {
                                case ARPResource.MonitorType.Alert:
                                    if (lstResources[ResourceID].AlarmAcknowledged || DateTime.Now.Millisecond < 500)
                                        contAlarm.image = Resources.btnAlarmAlert;
                                    break;
                                case ARPResource.MonitorType.Warn:
                                    if (lstResources[ResourceID].AlarmAcknowledged || DateTime.Now.Millisecond < 500)
                                        contAlarm.image = Resources.btnAlarmWarn;
                                    break;
                                default:
                                    break;
                            }
                        }
                        //Draw the button - if the alarm is unacknowledged then acknowledge, else toggle alarm status
                        if (GUILayout.Button(contAlarm, Styles.styleAlarmButton))
                        {
                            if (!lstResources[ResourceID].AlarmAcknowledged)
                                lstResources[ResourceID].AlarmAcknowledged = true;
                            else
                                settings.Resources[ResourceID].AlarmEnabled = !settings.Resources[ResourceID].AlarmEnabled;
                        }
                    }
                    //Is this resource selected
                    Boolean Highlight = SelectedResources.ContainsKey(ResourceID) && SelectedResources[ResourceID].AllVisible;
                    //For resources with no stage specifics
                    if (lstResources[ResourceID].ResourceDef.resourceFlowMode == ResourceFlowMode.ALL_VESSEL)
                    {
                        //full width bar
                        rectBar = Drawing.CalcBarRect(rectIcon, Icon2BarOffset, 245, 15);
                        if (Drawing.DrawResourceBar(rectBar, lstResources[ResourceID], Styles.styleBarGreen_Back, Styles.styleBarGreen, Styles.styleBarGreen_Thin, settings.ShowRates, Highlight))
                            //MonoBehaviourExtended.LogFormatted_DebugOnly("Clicked");
                            SelectedResources.TogglePartResourceVisible(ResourceID);
                    }
                    else
                    {
                        //need full Vessel and current stage bars
                        rectBar = Drawing.CalcBarRect(rectIcon, Icon2BarOffset, 120, 15);
                        if (Drawing.DrawResourceBar(rectBar, lstResources[ResourceID], Styles.styleBarGreen_Back, Styles.styleBarGreen, Styles.styleBarGreen_Thin, settings.ShowRates, Highlight))
                            SelectedResources.TogglePartResourceVisible(ResourceID);

                        //get last stage of this resource and set it
                        if (lstResourcesLastStage.ContainsKey(ResourceID))
                        {
                            Highlight = SelectedResources.ContainsKey(ResourceID) && SelectedResources[ResourceID].LastStageVisible;
                            rectBar = Drawing.CalcBarRect(rectIcon, Icon2StageBarOffset, 120, 15);
                            if (Drawing.DrawResourceBar(rectBar, lstResourcesLastStage[ResourceID], Styles.styleBarBlue_Back, Styles.styleBarBlue, Styles.styleBarBlue_Thin, settings.ShowRates, Highlight))
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
                GUILayout.Label("Stage:", Styles.styleStageTextHead, GUILayout.Width(60));
                GUIStyle styleStageNum = new GUIStyle(Styles.styleStageTextHead);
                GUIContent contStageNum = new GUIContent(Staging.CurrentStage.ToString());
                //styleStageNum.normal.textColor=new Color(173,43,43);
                //GUIContent contStageNum = new GUIContent(Staging.CurrentStage.ToString(),"NO Active Engines");
                //if (THERE ARE ACTIVE ENGINES IN STAGE)
                //{
                //contStageNum.tooltip="Active Engines";
                styleStageNum.normal.textColor = new Color(117, 206, 60);
                //}

                GUILayout.Label(contStageNum, styleStageNum, GUILayout.Width(40));

                if (settings.StagingEnabledInMapView || !MapView.MapIsEnabled)
                {
                    if (GUILayout.Button("Activate Stage","ButtonGeneral", GUILayout.Width(100)))
                        Staging.ActivateNextStage();
                }

            }

            //Settings Toggle button
            GUILayout.FlexibleSpace();
            GUIContent btnMinMax = new GUIContent(Resources.btnChevronDown, "Show Settings...");
            if (windowSettings.Visible) { 
                btnMinMax.image = Resources.btnChevronUp; btnMinMax.tooltip = "Hide Settings";
            }
            else if (settings.VersionAttentionFlag && DateTime.Now.Millisecond < 500) {
                btnMinMax.image = Resources.btnSettingsAttention; 
            }
            
            if (settings.VersionAttentionFlag) btnMinMax.tooltip = "Updated Version Available - " + btnMinMax.tooltip;
            
            if (GUILayout.Button(btnMinMax,"ButtonSettings"))
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

    }

}
