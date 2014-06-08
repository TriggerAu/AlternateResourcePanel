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
    class ARPWindowResourceConfig : MonoBehaviourWindowPlus
    {
        internal KSPAlternateResourcePanel mbARP;
        internal Settings settings;

        Int32 WindowHeight = 400;
        Int32 ScrollAreaWidth = 395;

        internal Vector2 ScrollPosition = new Vector2();
        Int32 ResourceToShowAlarm;
        Int32 ResourceToShowAlarmChanger = 0;
        Boolean ResourceToShowAlarmChangeNeeded = false;

        DropDownList ddlMonType;

        internal override void Awake()
        {
            settings = KSPAlternateResourcePanel.settings;
            WindowRect = new Rect(300, 0, 410, WindowHeight);

            ddlMonType = new DropDownList(EnumExtensions.ToEnumDescriptions<ResourceSettings.MonitorDirections>(),this);
            ddlMonType.SetListBoxOffset(new Vector2(8,56));

            ddlMonType.OnSelectionChanged += ddlMonType_OnSelectionChanged;
            ddlManager.AddDDL(ddlMonType);
        }

        internal override void OnDestroy()
        {
            ddlMonType.OnSelectionChanged -= ddlMonType_OnSelectionChanged;
        }

        void ddlMonType_OnSelectionChanged(MonoBehaviourWindowPlus.DropDownList sender, int OldIndex, int NewIndex)
        {
            settings.Resources[ResourceToShowAlarm].MonitorDirection = (ResourceSettings.MonitorDirections)NewIndex;

        }


        internal override void OnGUIOnceOnly()
        {
            ddlManager.DropDownGlyphs = new GUIContentWithStyle(Resources.btnDropDown, Styles.styleDropDownGlyph);
            ddlManager.DropDownSeparators = new GUIContentWithStyle("", Styles.styleSeparatorV);
        }

        internal override void DrawWindow(int id)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Configure Resources",Styles.styleTextYellowBold);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close")) this.Visible = false;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(2);
            GUILayout.Label("Icon", Styles.styleStageTextHead,GUILayout.Width(32));
            GUILayout.Label("Name", Styles.styleStageTextHead, GUILayout.Width(120));
            GUILayout.Label("Position", Styles.styleStageTextHead, GUILayout.Width(56));
            GUILayout.Space(6);
            GUILayout.Label("Visibility", Styles.styleStageTextHead, GUILayout.Width(80));
            GUILayout.Label("Monitor", Styles.styleStageTextHead, GUILayout.Width(60));
            GUILayout.EndHorizontal();

            Int32 SepToDelete = 0;

            Int32 NameWidth = 120;

            Styles.styleResourceSettingsArea.fixedWidth = ScrollAreaWidth - 23;

            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, new GUIStyle(), GUILayout.Width(ScrollAreaWidth));
            //wrapper for preventing horiz scroll
            GUILayout.BeginVertical(GUILayout.Width(ScrollAreaWidth - 23));
            //foreach (PartResourceDefinition item in PartResourceLibrary.Instance.resourceDefinitions)
            for (int i = 0; i < settings.Resources.Count; i++)
            {
                ResourceSettings item = settings.Resources.Values.ElementAt(i);
                if (ResourceToShowAlarm == item.id)
                {
                    GUILayout.Space(4);
                    GUILayout.BeginVertical(Styles.styleResourceSettingsArea);
                    NameWidth = 119;
                }
                GUILayout.BeginHorizontal();

                if (!item.IsSeparator) {
                    GUILayout.BeginVertical();
                    GUILayout.Space(6);
                    Drawing.DrawResourceIcon(item.name);
                    GUILayout.EndVertical();
                    //GUILayout.Label(item.name, GUILayout.Width(NameWidth));
                    if (GUILayout.Button(item.name, SkinsLibrary.CurrentSkin.label, GUILayout.Width(NameWidth))) {
                        if (ResourceToShowAlarmChanger == item.id)
                            ResourceToShowAlarmChanger = 0;
                        else
                        {
                            ResourceToShowAlarmChanger = item.id;
                            ddlMonType.SelectedIndex = (Int32)settings.Resources[ResourceToShowAlarmChanger].MonitorDirection;
                        }
                        ResourceToShowAlarmChangeNeeded = true;
                    }
                } else {
                    GUILayout.BeginVertical();
                    GUILayout.Space(13);
                    GUILayout.Label("", Styles.styleSeparatorH, GUILayout.Width(120+36));
                    GUILayout.Space(7);
                    GUILayout.EndVertical();
                }
                if (i > 0) {
                    if (GUILayout.Button("↑", GUILayout.Width(28))) {
                        SwapResource(i - 1, i);
                    }
                } else {
                    GUILayout.Space(28 + 4);
                }
                if (i < settings.Resources.Count - 1) {
                    if (GUILayout.Button("↓", GUILayout.Width(28)))
                    {
                        SwapResource(i, i+1);
                    }
                } else {
                    GUILayout.Space(28 + 4);
                }

                if (!item.IsSeparator)
                {
                    if (GUILayout.Button(settings.Resources[item.id].Visibility.ToString(), GUILayout.Width(80))) {
                        settings.Resources[item.id].Visibility = settings.Resources[item.id].Visibility.Next();
                    }
                    if (GUILayout.Button(string.Format("{0}/{1}", settings.Resources[item.id].MonitorWarningLevel, settings.Resources[item.id].MonitorAlertLevel), GUILayout.Width(60))) {
                        if (ResourceToShowAlarmChanger == item.id) 
                            ResourceToShowAlarmChanger = 0; 
                        else
                        {
                            ResourceToShowAlarmChanger = item.id;
                            ddlMonType.SelectedIndex = (Int32)settings.Resources[ResourceToShowAlarmChanger].MonitorDirection;
                        }
                        ResourceToShowAlarmChangeNeeded = true;
                    }
                }
                else
                {
                    if (GUILayout.Button("Delete", GUILayout.Width(80 + 60 + 4))) {
                        SepToDelete = item.id;
                    }
                }
                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();

                if (ResourceToShowAlarm==item.id)
                {
                    GUILayout.BeginVertical(GUILayout.Height(40),GUILayout.Width(ScrollAreaWidth-20));
                    GUILayout.Space(4);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Monitoring Levels:",Styles.styleStageTextHead, GUILayout.Width(151));
                    //GUILayout.Label("Monitoring Type:", GUILayout.Width(mbARP.windowDebug.intTest1));
                    ddlMonType.DrawButton();
                    GUILayout.Space(4);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Warning Level:",GUILayout.Width(90));
                    settings.Resources[item.id].MonitorWarningLevel = (Int32)Math.Round(GUILayout.HorizontalSlider(settings.Resources[item.id].MonitorWarningLevel, 0, 100,GUILayout.Width(220)));
                    GUILayout.Label(settings.Resources[item.id].MonitorWarningLevel.ToString() + "%",GUILayout.Width(35));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Alert Level:", GUILayout.Width(90));
                    settings.Resources[item.id].MonitorAlertLevel = (Int32)Math.Round(GUILayout.HorizontalSlider(settings.Resources[item.id].MonitorAlertLevel, 0, 100, GUILayout.Width(220)));
                    GUILayout.Label(settings.Resources[item.id].MonitorAlertLevel.ToString() + "%", GUILayout.Width(35));
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    if (settings.Resources[item.id].MonitorDirection == ResourceSettings.MonitorDirections.Low)
                        settings.Resources[item.id].MonitorWarningLevel = Mathf.Clamp(settings.Resources[item.id].MonitorWarningLevel,
                            settings.Resources[item.id].MonitorAlertLevel, 100);
                    else
                        settings.Resources[item.id].MonitorWarningLevel = Mathf.Clamp(settings.Resources[item.id].MonitorWarningLevel,
                            0, settings.Resources[item.id].MonitorAlertLevel);

                    GUIStyle temp = new GUIStyle(Styles.styleStageTextHead);
                    temp.padding.top = 0;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Alarms for this:", temp, GUILayout.Width(120));
                    DrawToggle(ref settings.Resources[item.id].AlarmEnabled, "Alarm Enabled", Styles.styleToggle);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Empty Behaviour:", temp, GUILayout.Width(120));
                    DrawToggle(ref settings.Resources[item.id].HideWhenEmpty, "Hide When Empty", Styles.styleToggle);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Full Behaviour:", temp, GUILayout.Width(120));
                    DrawToggle(ref settings.Resources[item.id].HideWhenFull, "Hide When Full", Styles.styleToggle);
                    GUILayout.EndHorizontal();
                    if (PartResourceLibrary.Instance.resourceDefinitions[item.id].resourceFlowMode== ResourceFlowMode.ALL_VESSEL ||
                        PartResourceLibrary.Instance.resourceDefinitions[item.id].resourceFlowMode == ResourceFlowMode.STAGE_PRIORITY_FLOW) { 
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Split Behaviour:", temp, GUILayout.Width(120));
                        DrawToggle(ref settings.Resources[item.id].ShowReserveLevels, new GUIContent("Show Reserve Levels","instead of Whole Vessel/Last Stage split"), Styles.styleToggle);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(3);
                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();


            GUILayout.EndVertical();

            if (SepToDelete != 0)
                settings.Resources.Remove(SepToDelete);

            GUILayout.BeginHorizontal();
            if (DrawButton("Add Separator"))
            {
                Int32 SepID = 0;
                Int32 SepAttempts=0;
                while (SepID==0 || settings.Resources.ContainsKey(SepID))
                {
                    SepID= UnityEngine.Random.Range(1,100);
                    SepAttempts++;
                    if (SepAttempts > 100) 
                        break;
                }
                if (SepAttempts > 100)
                {
                    ScreenMessages.PostScreenMessage("Unable to find a new Separator ID", 3, ScreenMessageStyle.UPPER_RIGHT);
                }
                settings.Resources.Add(SepID, new ResourceSettings() { id = SepID, IsSeparator = true });
            }
            if (DrawButton("Sort Groups"))
            {
                SortGroups();
            }

            if (DrawButton("Save"))
            {
                settings.Save();
            }
            GUILayout.EndHorizontal();

            if (ResourceToShowAlarmChangeNeeded)
            {
                ResourceToShowAlarmChangeNeeded = false;
                ResourceToShowAlarm = ResourceToShowAlarmChanger;
            }
        }

        private void SortGroups()
        {
            List<ResourceSettings> lstToSort = new List<ResourceSettings>();
            List<ResourceSettings> lstFinal = new List<ResourceSettings>();
            for (int i = 0; i < settings.Resources.Count; i++)
            {
                if (i==0 || settings.Resources.ElementAt(i).Value.IsSeparator)
                {
                    //Sort the current list
                    if (lstToSort.Count>0)
                    {
                        lstFinal.AddRange(lstToSort.OrderBy(x => x.name));
                    }
                    //reset the sort list
                    lstToSort = new List<ResourceSettings>();
                }
                lstToSort.Add(settings.Resources.ElementAt(i).Value);
            }
            if (lstToSort.Count>0)
            {
                lstFinal.AddRange(lstToSort.OrderBy(x => x.name));
            }
            LogFormatted("Sorted Resource Groups");
            settings.Resources = lstFinal.ToDictionary(x => x.id);
        }

        private void SwapResource(Int32 indexFirst, Int32 indexSecond)
        {
            List<ResourceSettings> lstTemp = settings.Resources.Values.ToList();
            ResourceSettings tempItem = lstTemp[indexFirst];
            lstTemp[indexFirst] = lstTemp[indexSecond];
            lstTemp[indexSecond] = tempItem;
            settings.Resources= lstTemp.ToDictionary(x => x.id);
        }
    
    }
}
