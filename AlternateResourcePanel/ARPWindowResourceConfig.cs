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
        internal Vector2 vectMonTypeOffset = new Vector2(8, 56);
        Int32 ResourceToShowAlarm;
        Int32 ResourceToShowAlarmChanger = 0;
        Boolean ResourceToShowAlarmChangeNeeded = false;

        DropDownList ddlMonType;

        internal override void Awake()
        {
            settings = KSPAlternateResourcePanel.settings;
            WindowRect = new Rect(300, 0, 410, WindowHeight);

            ddlMonType = new DropDownList(EnumExtensions.ToEnumDescriptions<ResourceSettings.MonitorDirections>(),this);
            ddlMonType.SetListBoxOffset(vectMonTypeOffset-ScrollPosition);

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

            if (Event.current.type == EventType.Repaint)
                lstResPositions = new List<ResourcePosition>();
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

                    Rect IconRect;
                    if (Event.current.type == EventType.Repaint)
                    {
                        IconRect = GUILayoutUtility.GetLastRect();
                        //float intOffset = 0;
                        //if (lstResPositions.Count != 0)
                        //    intOffset = lstResPositions.Last().resourceRect.y + 22;
                        //Rect resResourcePos = new Rect(mbARP.windowDebug.intTest1, mbARP.windowDebug.intTest2 + intOffset, 0, 0);
                        lstResPositions.Add(new ResourcePosition(item.id, item.name, IconRect, ScrollAreaWidth - 20, (ResourceToShowAlarm == item.id)));
                    }
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

                    if (Event.current.type == EventType.Repaint)
                    {
                        Rect IconRect = new Rect(lstResPositions.Last().iconRect) { y = lstResPositions.Last().iconRect.y + 22, width = 120 + 36 };
                        lstResPositions.Add(new ResourcePosition(item.id, "Separator", IconRect, ScrollAreaWidth - 20, (ResourceToShowAlarm == item.id)));
                    }
                }
                if (GUILayout.Button(new GUIContent("S","Add Separator"), GUILayout.Width(21)))
                {
                    AddSeparatorAtEnd();
                    MoveResource(settings.Resources.Count - 1, i + 1);
                }
                if (i > 0) {
                    if (GUILayout.Button("↑", GUILayout.Width(21)))
                    {
                        SwapResource(i - 1, i);
                    }
                } else {
                    GUILayout.Space(21 + 4);
                }
                if (i < settings.Resources.Count - 1) {
                    if (GUILayout.Button("↓", GUILayout.Width(21)))
                    {
                        SwapResource(i, i+1);
                    }
                } else {
                    GUILayout.Space(21 + 4);
                }

                if (!item.IsSeparator)
                {
                    if (GUILayout.Button(settings.Resources[item.id].Visibility.ToString(), GUILayout.Width(75)))
                    {
                        settings.Resources[item.id].Visibility = settings.Resources[item.id].Visibility.Next();
                    }
                    if (GUILayout.Button(string.Format("{0}/{1}", settings.Resources[item.id].MonitorWarningLevel, settings.Resources[item.id].MonitorAlertLevel), GUILayout.Width(58)))
                    {
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
                    if (GUILayout.Button("Delete", GUILayout.Width(75 + 58 + 4)))
                    {
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
                    ddlMonType.SetListBoxOffset(vectMonTypeOffset - ScrollPosition);
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
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Stage Bars:", temp, GUILayout.Width(120));
                    DrawToggle(ref settings.Resources[item.id].SplitLastStage, "Split Enabled", Styles.styleToggle);
                    GUILayout.EndHorizontal();
                    if (settings.Resources[item.id].SplitLastStage && (
                        PartResourceLibrary.Instance.resourceDefinitions[item.id].resourceFlowMode == ResourceFlowMode.ALL_VESSEL ||
                        PartResourceLibrary.Instance.resourceDefinitions[item.id].resourceFlowMode == ResourceFlowMode.STAGE_PRIORITY_FLOW)
                        ) { 
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
                AddSeparatorAtEnd();
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

            // Draw the Yellow insertion strip
            if(DraggingResource && resourceOver!=null)
            {
                Rect rectResMove = new Rect(4,
                    resourceOver.resourceRect.y + 49 - ScrollPosition.y,
                    378,9);
                GUI.Box(rectResMove, new GUIContent(Resources.texResourceMove),new GUIStyle());
            }

            //Do the mouse checks
            IconMouseEvents();

            //Disable the Window from dragging if we are dragging a resource
            DragEnabled = !DraggingResource;
        }

        private void AddSeparatorAtEnd()
        {
            Int32 SepID = 0;
            Int32 SepAttempts = 0;
            while (SepID == 0 || settings.Resources.ContainsKey(SepID))
            {
                SepID = UnityEngine.Random.Range(1, 100);
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

        void IconMouseEvents()
        {
            //Mouse position relatiuve to the window
            MousePosition = Event.current.mousePosition;

            //If the Mouse is inside teh scroll window
            if (MousePosition.y > 54 && MousePosition.y < 374)
            {
                //check what we are over
                resourceOver = lstResPositions.FirstOrDefault(x => x.resourceRect.Contains(MousePosition + ScrollPosition - new Vector2(8, 54)));
                iconOver = lstResPositions.FirstOrDefault(x => x.iconRect.Contains(MousePosition + ScrollPosition + -new Vector2(8, 54)));
            }
            else { resourceOver = null; iconOver = null; }

            //did we click on an Icon
            if (Event.current.type == EventType.mouseDown && iconOver!=null)
            {
                LogFormatted_DebugOnly("Drag Start");
                resourceDrag = iconOver;
                DraggingResource = true;
            }
            //did we release the mouse
            if (Event.current.type == EventType.mouseUp)
            {
                if (resourceOver != null)
                {
                    //And dropped on a resource
                    LogFormatted_DebugOnly("Drag Stop:{0}-{1}-{2}", resourceOver == null ? "None" : lstResPositions.FindIndex(x => x.id == resourceOver.id).ToString(), resourceOver == null ? "" : settings.Resources[resourceOver.id].name, resourceDrag.name);

                    MoveResource(lstResPositions.FindIndex(x => x.id == resourceDrag.id), lstResPositions.FindIndex(x => x.id == resourceOver.id));
                }
                //disable draggingh flag
                DraggingResource = false;
            }

            //If we are dragging and in the bottom or top area then scrtoll the list
            if(DraggingResource && rectScrollBottom.Contains(MousePosition))
                ScrollPosition.y += (Time.deltaTime * 40);
            if(DraggingResource && rectScrollTop.Contains(MousePosition))
                ScrollPosition.y -= (Time.deltaTime * 40);
        }

        //Outside the window draw routine so it acts anywhere on the screen
        internal override void OnGUIEvery()
        {
            base.OnGUIEvery();

            //disable resource dragging if we mouseup outside the window
            if (Event.current.type == EventType.mouseUp) DraggingResource = false;

            //If we are dragging, show what we are dragging
            if (DraggingResource && resourceDrag!=null)
            {
                //set the Style
                GUIStyle styleResMove = SkinsLibrary.CurrentTooltip;
                styleResMove.alignment = TextAnchor.MiddleLeft;

                //set and draw the text like a tooltip
                String Message = "  Moving";
                if (resourceDrag.name == "Separator") Message += " Separator";
                Rect LabelPos = new Rect(Input.mousePosition.x-5,Screen.height-Input.mousePosition.y-5,120,22);
                GUI.Label(LabelPos, Message, SkinsLibrary.CurrentTooltip);
                
                //If its a resourcethen draw the icon too
                if (resourceDrag.name != "Separator")
                {
                    GUIContent contIcon = Drawing.GetResourceIcon(resourceDrag.name); ;
                    Rect ResPos = new Rect(Input.mousePosition.x + 55, Screen.height - Input.mousePosition.y-2, 32, 16);
                    GUI.Box(ResPos, contIcon, new GUIStyle());
                }
                //On top of everything
                GUI.depth = 0;
            }
        }

        /// <summary>
        /// List of Resource positions in the window
        /// </summary>
        internal List<ResourcePosition> lstResPositions = new List<ResourcePosition>();
        /// <summary>
        /// where the mouse is
        /// </summary>
        public Vector2 MousePosition;
        //are we currently dragging a resource
        public Boolean DraggingResource = false;

        /// <summary>
        /// Resource the mouse is over
        /// </summary>
        internal ResourcePosition resourceOver = null;
        /// <summary>
        /// Icon the Mouse is Over
        /// </summary>
        internal ResourcePosition iconOver = null;
        /// <summary>
        /// Resource that is being Dragged
        /// </summary>
        internal ResourcePosition resourceDrag = null;

        //Rects to define where the scroll detection should be
        internal Rect rectScrollBottom = new Rect(8, 360, 394, 15);
        internal Rect rectScrollTop = new Rect(8, 60, 394, 15);

        /// <summary>
        /// Object to hold info for the drag and drop resource functionality
        /// </summary>
        internal class ResourcePosition
        {
            internal ResourcePosition(Int32 id, String name, Rect iconRect,Int32 width,Boolean Expanded)
            {
                this.id = id;
                this.name=name;
                this.iconRect = iconRect;
                this._resWidth = width;
                this._expanded = Expanded;
            }

            internal Rect iconRect { get; private set; }
            internal Int32 id { get; private set; }
            internal String name { get; private set; }

            Int32 _resWidth;
            Boolean _expanded;

            /// <summary>
            /// Rectangle defining the position of the resource in the scrollview
            /// </summary>
            internal Rect resourceRect
            {
                get
                {
                    Rect tmp = new Rect(iconRect);
                    tmp.width = _resWidth;
                    tmp.height = _expanded?204:22;
                    return tmp;
                }
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

        private void MoveResource(Int32 indexFrom, Int32 InsertBeforeIndex)
        {
            LogFormatted_DebugOnly("Move Resource from {0} to {1}", indexFrom, InsertBeforeIndex);
            //do a swap for each pair in between moving the item to its new spot
            if (InsertBeforeIndex > indexFrom)
            {
                for (int i = indexFrom; i < InsertBeforeIndex-1; i++)
                {
                    LogFormatted_DebugOnly("Swap Resource - {0}<->{1}", i, i+1);
                    SwapResource(i, i + 1);
                }
            }
            else
            {
                for (int i = indexFrom; i > InsertBeforeIndex; i--)
                {
                    SwapResource(i, i - 1);
                }
            }
        }
    }
}
