using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

using ARPToolbarWrapper;

namespace KSPAlternateResourcePanel
{
    [KSPAddon(KSPAddon.Startup.Flight,false)]
    public class KSPAlternateResourcePanel : MonoBehaviourExtended
    {
        //windows
        internal ARPWindow windowMain;
        internal ARPWindowSettings windowSettings;
        internal ARPWindowResourceConfig windowResourceConfig;

        //variables
        internal PartResourceVisibleList SelectedResources;

        internal Int32 LastStage;
        internal ARPResourceList lstResourcesVessel;
        internal ARPResourceList lstResourcesLastStage;

        /// <summary>
        /// ResourceIDs of ones to show in window
        /// </summary>
        internal List<Int32> lstResourcesToDisplay;

        internal ARPPartWindowList lstPartWindows;
        
        /// <summary>
        /// Vector to the screenposition of the vessels center of mass
        /// </summary>
        internal Vector3 vectVesselCOMScreen;

        internal static Settings settings;
        
        internal IButton btnToolbar = null;

        /// <summary>flag to reset Window Position</summary>
        internal Boolean blnResetWindow = false;

        internal static AudioController audioController;
        internal AudioClip clipAlarmsWarning;
        internal AudioClip clipAlarmsAlert;

        internal override void Awake()
        {
            LogFormatted("Awakening the AlternateResourcePanel (ARP)");

            LogFormatted("Loading Settings");
            settings = new Settings("settings.cfg");
            if (!settings.Load())
                LogFormatted("Settings Load Failed");

            //Ensure settings.resources contains all the resources in the loaded game
            VerifyResources();

            //get the sounds and set things up
            Resources.LoadSounds();
            InitAudio();

            //Get whether the toolbar is there
            settings.BlizzyToolbarIsAvailable = ToolbarManager.ToolbarAvailable;
            //if requested use that button
            if (settings.BlizzyToolbarIsAvailable && settings.UseBlizzyToolbarIfAvailable)
                btnToolbar = InitToolbarButton();

            //init the global variables
            lstPartWindows = new ARPPartWindowList();
            lstResourcesVessel = new ARPResourceList(ARPResourceList.ResourceUpdate.AddValues);
            lstResourcesLastStage = new ARPResourceList(ARPResourceList.ResourceUpdate.AddValues);

            lstResourcesToDisplay = new List<Int32>();
            SelectedResources = new PartResourceVisibleList();

            lstResourcesVessel.OnMonitorStateChange += lstResourcesVessel_OnMonitorStateChange;
            lstResourcesVessel.OnAlarmAcknowledged += lstResourcesVessel_OnAlarmAcknowledged;

            //init the windows
            InitMainWindow();
            InitSettingsWindow();
            InitResourceConfigWindow();
            InitDebugWindow();

            //plug us in to the draw queue and start the worker
            RenderingManager.AddToPostDrawQueue(1, DrawGUI);
            StartRepeatingWorker(10);

            //register for stage separation events - so we can cancel the noise on a sep
            GameEvents.onStageActivate.Add(OnStageActivate);

            //do the daily version check if required
            if (settings.DailyVersionCheck)
                settings.VersionCheck(false);
        }

        internal override void OnDestroy()
        {
            LogFormatted("Destroying the AlternateResourcePanel (ARP)");

            lstResourcesVessel.OnMonitorStateChange -= lstResourcesVessel_OnMonitorStateChange;
            lstResourcesVessel.OnAlarmAcknowledged -= lstResourcesVessel_OnAlarmAcknowledged;

            RenderingManager.RemoveFromPostDrawQueue(1, DrawGUI);

            DestroyToolbarButton(btnToolbar);
        }

        //use this to trigger a clean up of sound at the end of teh repeating worker loop
        Boolean StageCheckAlarmAudio = false;
        void OnStageActivate(Int32 StageNum)
        {
            StageCheckAlarmAudio = true;
        }

        void lstResourcesVessel_OnMonitorStateChange(ARPResource sender, ARPResource.MonitorType alarmType, bool TurnedOn,Boolean AlarmAcknowledged)
        {
            //Play a sound if necessary
            if (TurnedOn && !AlarmAcknowledged && settings.AlarmsEnabled && settings.Resources[sender.ResourceDef.id].AlarmEnabled)
            {
                switch (alarmType)
                {
                    case ARPResource.MonitorType.Alert:
                        if (clipAlarmsAlert != null) 
                            KSPAlternateResourcePanel.audioController.Play(clipAlarmsAlert, settings.AlarmsAlertRepeats);
                        break;
                    case ARPResource.MonitorType.Warn:
                        if (clipAlarmsAlert != null) 
                            KSPAlternateResourcePanel.audioController.Play(clipAlarmsWarning, settings.AlarmsWarningRepeats);
                        break;
                }
            }
        }
        void lstResourcesVessel_OnAlarmAcknowledged(ARPResource sender)
        {
            KSPAlternateResourcePanel.audioController.Stop();
        }

        /// <summary>
        /// This will add any missing resources to the settings objects befoe we run so we have records for all
        /// </summary>
        private void VerifyResources()
        {
            Boolean AddedResources = false;
            foreach (PartResourceDefinition item in PartResourceLibrary.Instance.resourceDefinitions)
            {
                if (!settings.Resources.ContainsKey(item.id))
                {
                    LogFormatted_DebugOnly("Adding Resource to Settings - {0}", item.name);
                    AddedResources = true;
                    settings.Resources.Add(item.id, new ResourceSettings() { id = item.id, name = item.name });
                }
            }
            if (AddedResources) settings.Save();
        }

        private void InitAudio()
        {
            audioController = AddComponent<AudioController>();
            audioController.mbARP = this;
            audioController.Init();
        }

        private void InitMainWindow()
        {
            // windowMain = gameObject.AddComponent<ARPWindow>();
            windowMain = AddComponent<ARPWindow>();
            windowMain.mbARP = this;
            windowMain.WindowRect = settings.WindowPosition;
            windowMain.DragEnabled = !settings.LockLocation;
            windowMain.ClampToScreenOffset = new RectOffset(-1, -1, -1, -1);
            windowMain.TooltipsEnabled = true;
        }

        private void InitSettingsWindow()
        {
            // windowMain = gameObject.AddComponent<ARPWindow>();
            windowSettings = AddComponent<ARPWindowSettings>();
            windowSettings.mbARP = this;
            windowSettings.Visible = false;
            windowSettings.DragEnabled = true;
            windowSettings.ClampToScreenOffset = new RectOffset(-1, -1, -1, -1);
            windowSettings.TooltipsEnabled = true;
        }

        private void InitResourceConfigWindow()
        {
            // windowMain = gameObject.AddComponent<ARPWindow>();
            windowResourceConfig = AddComponent<ARPWindowResourceConfig>();
            windowResourceConfig.mbARP = this;
            windowResourceConfig.Visible = false;
            windowResourceConfig.DragEnabled = true;
            windowResourceConfig.ClampToScreenOffset = new RectOffset(-1, -1, -1, -1);
        }


#if DEBUG
        internal ARPWindowDebug windowDebug;
#endif
        [System.Diagnostics.Conditional("DEBUG")]
        private void InitDebugWindow()
        {
#if DEBUG
            windowDebug = AddComponent<ARPWindowDebug>();
            windowDebug.mbARP = this;
            windowDebug.Visible = true;
            windowDebug.WindowRect = new Rect(0, 50, 300, 200);
            windowDebug.DragEnabled = true;
#endif
        }


        internal override void OnGUIOnceOnly()
        {
            //Get the textures we need into Textures
            Resources.LoadTextures();

            //Set up the Styles
            Styles.InitStyles();

            //Set up the Skins
            Styles.InitSkins();

            //Set the current Skin
            SkinsLibrary.SetCurrent(settings.SelectedSkin.ToString());
        }

        //Positio of screen button
        static Rect rectButton = new Rect(Screen.width - 109, 0, 80, 30);
        //Hover Status for mouse
        internal static Boolean HoverOn = false;

        void DrawGUI()
        {
            //Draw the button - if we arent using blizzy's toolbar
            if (!(settings.BlizzyToolbarIsAvailable && settings.UseBlizzyToolbarIfAvailable))
            {
                if (GUI.Button(rectButton, "Alternate", SkinsLibrary.CurrentSkin.GetStyle("ButtonMain")))
                {
                    settings.ToggleOn = !settings.ToggleOn;
                    settings.Save();
                }
            }

            //Test for mouse over any component - do this on repaint so it doesn't do it on layout and cause grouping errors
            if (Event.current.type == EventType.Repaint)
                HoverOn = IsMouseOver();

            //Are we drawing the main window - hovering, or toggled or alarmsenabled and an unackowledged alarm - and theres resources
            if ((HoverOn || settings.ToggleOn || (settings.AlarmsEnabled && lstResourcesVessel.UnacknowledgedAlarms(settings.Resources))) && (lstResourcesVessel.Count > 0))
            {
                windowMain.Visible = true;
                if (blnResetWindow)
                {
                    windowMain.WindowRect = new Rect(Screen.width - 298, 19, 299, 20);
                    blnResetWindow = false;
                    settings.WindowPosition = windowMain.WindowRect;
                    settings.Save();
                }
            }
            else
            {
                windowMain.Visible = false;
                windowSettings.Visible = false;
            }
        }

        /// <summary>
        /// Is the mouse ver the main or settings windows or the button
        /// </summary>
        /// <returns></returns>
        public Boolean IsMouseOver()
        {
            if ((settings.BlizzyToolbarIsAvailable && settings.UseBlizzyToolbarIfAvailable))
                return MouseOverToolbarBtn || (windowMain.Visible && windowMain.WindowRect.Contains(Event.current.mousePosition));

            //are we painting?
            Boolean blnRet = Event.current.type == EventType.Repaint;

            //And the mouse is over the button
            blnRet = blnRet && rectButton.Contains(Event.current.mousePosition);

            //mouse in main window
            blnRet = blnRet || (windowMain.Visible && windowMain.WindowRect.Contains(Event.current.mousePosition));

            ////or, the form was on the screen and the mouse is over that rectangle
            //blnRet = blnRet || (Drawing && rectPanel.Contains(Event.current.mousePosition));

            ////or, the settings form was on the screen and the mouse is over that rectangle
            blnRet = blnRet || (windowSettings.Visible && windowSettings.WindowRect.Contains(Event.current.mousePosition));

            return blnRet;

        }


        /// <summary>
        /// Heres where the heavy lifting should occur
        /// </summary>
        internal override void RepeatingWorker()
        {
            Vessel active = FlightGlobals.ActiveVessel;
            LastStage = GetLastStage(active.parts);

            //trigger the start loop and store the UT that has passed - only calc rates where necessary
            lstResourcesVessel.StartUpdatingList(settings.ShowRates, RepeatingWorkerUTPeriod);
            lstResourcesLastStage.StartUpdatingList(settings.ShowRates, RepeatingWorkerUTPeriod);

            List<Int32> ActiveResources = new List<Int32>();
            //Now loop through and update em
            foreach (Part p in active.parts)
            {
                foreach (PartResource pr in p.Resources)
                {
                    //store a list of all resources in vessel so we can nuke resources from the other lists later
                    if (!ActiveResources.Contains(pr.info.id)) ActiveResources.Add(pr.info.id);
                    
                    //update the resource in the vessel list
                    lstResourcesVessel.UpdateResource(pr);

                    //is the part decoupled in the last stage
                    Boolean DecoupledInLastStage = (p.DecoupledAt()==LastStage);
                    if (DecoupledInLastStage)
                    {
                        lstResourcesLastStage.UpdateResource(pr);
                    }

                    //is the resource in the selected list
                    if (SelectedResources.ContainsKey(pr.info.id) && SelectedResources[pr.info.id].AllVisible)
                        lstPartWindows.AddPartWindow(p, pr,this);
                    else if (SelectedResources.ContainsKey(pr.info.id) && SelectedResources[pr.info.id].LastStageVisible && DecoupledInLastStage)
                        lstPartWindows.AddPartWindow(p, pr,this);
                    else if (lstPartWindows.ContainsKey(p.GetInstanceID()))
                    {
                        //or not,but the window is in the list
                        if (lstPartWindows[p.GetInstanceID()].ResourceList.ContainsKey(pr.info.id))
                            lstPartWindows[p.GetInstanceID()].ResourceList.Remove(pr.info.id);
                    }
                }
            }

            //Destroy the windows that have no resources selected to display
            lstPartWindows.CleanWindows();
            
            //Remove Resources that no longer exist in vessel
            lstResourcesVessel.CleanResources(ActiveResources);
            lstResourcesLastStage.CleanResources(ActiveResources);
            
            //Finalise the list updates - calc rates and set alarm flags
            lstResourcesVessel.EndUpdatingList(settings.ShowRates);
            lstResourcesLastStage.EndUpdatingList(settings.ShowRates);

            //Set the alarm flags
            foreach (ARPResource r in lstResourcesVessel.Values)
            {
                ResourceSettings rToCheck = settings.Resources[r.ResourceDef.id];
                Double rPercent = (r.Amount / r.MaxAmount)*100;

                if ((rToCheck.MonitorDirection == ResourceSettings.MonitorDirections.Low && rPercent <= rToCheck.MonitorAlertLevel) ||
                    (rToCheck.MonitorDirection == ResourceSettings.MonitorDirections.High && rPercent >= rToCheck.MonitorAlertLevel))
                {
                    r.MonitorAlert = true;
                    r.MonitorWarning = true;
                }
                else if ((rToCheck.MonitorDirection == ResourceSettings.MonitorDirections.Low && rPercent <= rToCheck.MonitorWarningLevel) ||
                    (rToCheck.MonitorDirection == ResourceSettings.MonitorDirections.High && rPercent >= rToCheck.MonitorWarningLevel))
                {
                    r.MonitorWarning = true;
                    r.MonitorAlert = false;
                }
                else
                {
                    r.MonitorWarning = false;
                    r.MonitorAlert = false;
                }
            }


            //work out if we have to kill the audio
            if (StageCheckAlarmAudio)
            {
                StageCheckAlarmAudio = false;
                if (KSPAlternateResourcePanel.audioController.isPlaying())
                {
                    Boolean AudioShouldBePlaying = false;
                    foreach (ARPResource r in lstResourcesVessel.Values)
                    {
                        if (!r.AlarmAcknowledged && r.MonitorWorstHealth!= ARPResource.MonitorType.None)
                        {
                            AudioShouldBePlaying = true;
                            break;
                        }
                    }
                    if (!AudioShouldBePlaying)
                        KSPAlternateResourcePanel.audioController.Stop();
                }

            }


            //Build the displayList
            lstResourcesToDisplay = new List<Int32>();
            for (int i = 0; i < settings.Resources.Count; i++)
            {
                Int32 ResourceID = (Int32)settings.Resources.Keys.ElementAt(i);

                if (settings.Resources[ResourceID].IsSeparator)
                {
                    if (lstResourcesToDisplay.Count > 0 && lstResourcesToDisplay.Last() != 0) //Dont add double seps
                        lstResourcesToDisplay.Add(0);
                    continue;
                }

                //skip to next item if this one is a) Hidden b) Set to threshold and not flagged
                if (!lstResourcesVessel.ContainsKey(ResourceID)) continue;
                if (settings.Resources[ResourceID].Visibility == ResourceSettings.VisibilityTypes.Hidden)
                    continue;
                else if (settings.Resources[ResourceID].Visibility == ResourceSettings.VisibilityTypes.Threshold)
                    if (!(lstResourcesVessel[ResourceID].MonitorAlert || lstResourcesVessel[ResourceID].MonitorWarning))
                        continue;

                //if we get this far then add it to the list
                lstResourcesToDisplay.Add(ResourceID);

            }

            //remove starting or ending seps
            if (lstResourcesToDisplay.Count > 0)
            {
                while (lstResourcesToDisplay.First() == 0)
                    lstResourcesToDisplay.RemoveAt(0);
                while (lstResourcesToDisplay.Last() == 0)
                    lstResourcesToDisplay.RemoveAt(lstResourcesToDisplay.Count - 1);
            }

            //Calc window widths/heights
            windowMain.WindowRect.width = 299;
            windowMain.Icon2BarOffset = 40;
            windowMain.Icon2StageBarOffset = 40 + 125;
            if (settings.AlarmsEnabled)
            {
                windowMain.WindowRect.width += windowMain.IconAlarmOffset;
                windowMain.Icon2BarOffset += windowMain.IconAlarmOffset;
                windowMain.Icon2StageBarOffset += windowMain.IconAlarmOffset;
            }

            if (lstResourcesToDisplay.Count == 0)
                windowMain.WindowRect.height = (2 * windowMain.intLineHeight) + 16;
            else
                windowMain.WindowRect.height = ((lstResourcesToDisplay.Count + 1) * windowMain.intLineHeight) - (lstResourcesToDisplay.Count(x => x == 0) * 15) + 12;

        }

        internal override void Update()
        {
            //Activate Stage via Space Bar in MapView
            if (settings.StagingEnabledSpaceInMapView && MapView.MapIsEnabled && windowMain.Visible && Input.GetKey(KeyCode.Space))
                Staging.ActivateNextStage();
        }

        internal override void LateUpdate()
        {
            //position the part windows
            if (lstPartWindows.Count > 0)
            {
                vectVesselCOMScreen = FlightCamera.fetch.mainCamera.WorldToScreenPoint(FlightGlobals.ActiveVessel.findWorldCenterOfMass());

                DateTime dteStart = DateTime.Now;

                List<ARPPartWindow> LeftWindows = lstPartWindows.Values.Where(x => x.LeftSide).OrderByDescending(x => x.PartScreenPos.y).ToList();
                if (LeftWindows.Count > 0)
                {
                    Double LeftPos = lstPartWindows.Values.Where(x => x.LeftSide).Min(x => x.PartScreenPos.x) - ARPPartWindow.WindowOffset - (ARPPartWindow.WindowWidth / 2);
                    foreach (ARPPartWindow pwTemp in LeftWindows)
                    {
                        pwTemp.WindowRect.y = Screen.height - (float)pwTemp.PartScreenPos.y - (pwTemp.WindowRect.height / 2);   //this sets an initial y used for sorting later
                        pwTemp.WindowRect.x = (Int32)LeftPos;
                    }
                    UpdateWindowYs(LeftWindows);
                }
                List<ARPPartWindow> RightWindows = lstPartWindows.Values.Where(x => !x.LeftSide).OrderByDescending(x => x.PartScreenPos.y).ToList();
                if (RightWindows.Count > 0)
                {
                    Double RightPos = (float)lstPartWindows.Values.Where(x => !x.LeftSide).Max(x => x.PartScreenPos.x) + ARPPartWindow.WindowOffset - (ARPPartWindow.WindowWidth / 2);
                    foreach (ARPPartWindow pwTemp in RightWindows)
                    {
                        pwTemp.WindowRect.y = Screen.height - (float)pwTemp.PartScreenPos.y - (pwTemp.WindowRect.height / 2); //this sets an initial y used for sorting later
                        pwTemp.WindowRect.x = (Int32)RightPos;
                    }
                    UpdateWindowYs(RightWindows);
                }

                //Now update the lines
                //foreach (ARPPartWindow pwTemp in lstPartWindows.Values)
                //    pwTemp.UpdateLinePos();
            }
        }

        /// <summary>
        /// Worker that spaces out the windows in the Y axis to pad em
        /// </summary>
        /// <param name="ListOfWindows"></param>
        private static void UpdateWindowYs(List<ARPPartWindow> ListOfWindows)
        {
            Int32 MiddleNum = ListOfWindows.Count() / 2;
            for (int i = MiddleNum + 1; i < ListOfWindows.Count(); i++)
            {
                if (ListOfWindows[i - 1].ScreenNextWindowY > ListOfWindows[i].WindowRect.y)
                    ListOfWindows[i].WindowRect.y = ListOfWindows[i - 1].ScreenNextWindowY;
            }
            for (int i = MiddleNum - 1; i >= 0; i--)
            {
                if (ListOfWindows[i].ScreenNextWindowY > ListOfWindows[i + 1].WindowRect.y)
                    ListOfWindows[i].WindowRect.y = ListOfWindows[i + 1].WindowRect.y - ListOfWindows[i].WindowRect.height - ARPPartWindow.WindowSpaceOffset;
            }
        }

    
        /// <summary>
        /// Should be self explanatory
        /// </summary>
        /// <param name="Parts"></param>
        /// <returns>Largest Stage # in Parts list</returns>
        private Int32 GetLastStage(List<Part> Parts)
        {
            if (Parts.Count>0)
                return Parts.Max(x => x.DecoupledAt());
            else return -1;
        }

        #region Toolbar Stuff
        /// <summary>
        /// initialises a Toolbar Button for this mod
        /// </summary>
        /// <returns>The ToolbarButtonWrapper that was created</returns>
        internal IButton InitToolbarButton()
        {
            IButton btnReturn;
            try
            {
                LogFormatted("Initialising the Toolbar Icon");
                btnReturn = ToolbarManager.Instance.add(_ClassName, "btnToolbarIcon");
                SetToolbarIcon(btnReturn);
                btnReturn.ToolTip = "Alternate Resource Panel";
                btnReturn.OnClick +=(e) =>
                {
                    settings.ToggleOn = !settings.ToggleOn;
                    SetToolbarIcon(e.Button);
                    MouseOverToolbarBtn = true;
                    settings.Save();
                };
                btnReturn.OnMouseEnter += btnReturn_OnMouseEnter;
                btnReturn.OnMouseLeave += btnReturn_OnMouseLeave;
            }
            catch (Exception ex)
            {
                btnReturn = null;
                LogFormatted("Error Initialising Toolbar Button: {0}", ex.Message);
            }
            return btnReturn;
        }

        private static void SetToolbarIcon(IButton btnReturn)
        {
            //if (settings.ToggleOn) 
                //btnReturn.TexturePath = "TriggerTech/KSPAlternateResourcePanel/ToolbarIcons/KSPARPa_On";
            //else
                btnReturn.TexturePath = "TriggerTech/KSPAlternateResourcePanel/ToolbarIcons/KSPARPa";
        }

        void btnReturn_OnMouseLeave(MouseLeaveEvent e)
        {
            MouseOverToolbarBtn = false;
        }

        internal Boolean MouseOverToolbarBtn=false;
        void btnReturn_OnMouseEnter(MouseEnterEvent e)
        {
            MouseOverToolbarBtn=true;
        }

        /// <summary>
        /// Destroys theToolbarButtonWrapper object
        /// </summary>
        /// <param name="btnToDestroy">Object to Destroy</param>
        internal void DestroyToolbarButton(IButton btnToDestroy)
        {
            if (btnToDestroy != null)
            {
                LogFormatted("Destroying Toolbar Button");
                btnToDestroy.Destroy();
            }
            btnToDestroy = null;
        }
        #endregion

    }

    //[KSPAddon(KSPAddon.Startup.MainMenu, false)]
    //public class DDLTest:MonoBehaviourWindowPlus
    //{
    //    DropDownList ddltest;

    //    internal override void Awake()
    //    {
    //        WindowRect = new Rect(300, 0, 300, 200);
    //        Visible = true;

    //        List<String> Test = new List<String>() { "Option 1", "Option 2", "Option 3", "Option 4"};

    //        ddltest = new DropDownList(Test,this);

    //        ddltest.OnSelectionChanged += ddltest_OnSelectionChanged;
    //        ddlManager.Add(ddltest);
    //    }

    //    void ddltest_OnSelectionChanged(MonoBehaviourWindowPlus.DropDownList sender, int OldIndex, int NewIndex)
    //    {
    //        ScreenMessages.PostScreenMessage(ddltest.SelectedValue, 3, ScreenMessageStyle.UPPER_RIGHT);
    //    }

    //    internal override void OnGUIOnceOnly()
    //    {
    //        ddlManager.DropDownGlyphs = new GUIContentWithStyle(Resources.btnDropDown, Styles.styleDropDownGlyph);
    //        ddlManager.DropDownSeparators = new GUIContentWithStyle("", Styles.styleSeparatorV);
    //    }

    //    internal override void DrawWindow(int id)
    //    {
    //        if (GUILayout.Button("Add"))
    //        {
    //            ddltest.Items.Add("Option " + (ddltest.Items.Count + 1).ToString());
    //        }
    //        if (GUILayout.Button("Remove"))
    //        {
    //            ddltest.Items.RemoveAt(ddltest.Items.Count-1);
    //        }

    //        ddltest.DrawButton();

    //        GUILayout.Label(String.Format("{0}",ddltest.SelectedIndex));
    //        if(ddltest.ListVisible)
    //            GUILayout.Label(((Int32)Math.Floor((Event.current.mousePosition.y - ddltest.rectListBox.y) / (ddltest.rectListBox.height / ddltest.ListPageLength))).ToString());
    //    }
    //}

//#if DEBUG
//    //This will kick us into the save called default and set the first vessel active
//    [KSPAddon(KSPAddon.Startup.Flight, false)]
//    public class Debug_Drawline : MonoBehaviourExtended
//    {
//        internal override void OnGUIEvery()
//        {
//            Vector2 pointA = new Vector2(Screen.width / 2, Screen.height / 2);
//            Vector2 pointB = Event.current.mousePosition;
//            Drawing.DrawLine(pointA, pointB, 3);
//        }
//    }
//#endif

#if DEBUG
    //This will kick us into the save called default and set the first vessel active
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Debug_AutoLoadPersistentSaveOnStartup : MonoBehaviour
    {
        //use this variable for first run to avoid the issue with when this is true and multiple addons use it
        public static bool first = true;
        public void Start()
        {
            //only do it on the first entry to the menu
            if (first)
            {
                first = false;
                HighLogic.SaveFolder = "default";
                var game = GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);
                if (game != null && game.flightState != null && game.compatible)
                {
                    FlightDriver.StartAndFocusVessel(game, 0);
                }
                //CheatOptions.InfiniteFuel = true;
            }
        }
    }
#endif
}