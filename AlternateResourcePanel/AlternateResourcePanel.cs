using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

namespace KSPAlternateResourcePanel
{
    [KSPAddon(KSPAddon.Startup.Flight,false)]
    public partial class KSPAlternateResourcePanel : MonoBehaviour
    {
        //Autostage option

        //GlobalSettings
        private Boolean HoverOn = false;    //Are we hovering on something to draw the screen
        private Boolean ToggleOn = false;   //Has the draw been toggled on
        private Boolean Drawing = false;    //Am I drawing the window
        private Boolean DrawingSettings = false;    //Am I drawing the Settings Bit
        private static String _ClassName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name;

        internal static System.Random rnd;
        /// <summary>
        /// Class Initializer
        /// </summary>
        public KSPAlternateResourcePanel()
        {

        }

        
        /// <summary>
        /// Awake Event - when the DLL is loaded 
        /// </summary>
        public void Awake()
        {
            DebugLogFormatted("Awakening the {0}", _ClassName);

            //Settings s = new Settings();
            //s.Save();

            rnd = new System.Random();
            _WindowMainID = rnd.Next(1000, 2000000);
            _WindowSettingsID = rnd.Next(1000, 2000000);

            //Load Textures
            LoadTextures();

            //Load Settings here?
            LoadSettings();

            SetIconOrder();


            //Add it to the queue
            DebugLogFormatted("Adding to DrawQueue");
            RenderingManager.AddToPostDrawQueue(0, DrawGUI);

            //Common Toolbar Code
            BlizzyToolbarIsAvailable = HookToolbar();

            if (BlizzyToolbarIsAvailable && UseBlizzyToolbarIfAvailable)
            {
                btnToolbar = InitToolbarButton();
            }

            //Get the time per sec from the settings file
            SetupRepeatingFunction("BehaviourUpdate", 0.05f);
            //SetupRepeatingFunction_BehaviourUpdate(10);
        }

        /// <summary>
        /// Destroy Event - when the DLL is unloaded 
        /// </summary>
        public void OnDestroy()
        {
            DebugLogFormatted("Destroying the {0}", _ClassName);

            DestroyToolbarButton(btnToolbar);
        }


        internal ToolbarButtonWrapper btnToolbar = null;

        /// <summary>
        /// Check to see if the Toolbar is available
        /// </summary>
        /// <returns>True if the Toolbar.ToolbarManager class is loaded in an existing assembly</returns>
        internal Boolean HookToolbar()
        {
            //Is the Dll in memory
            Boolean blnReturn = ToolbarDLL.Loaded;
            DebugLogFormatted("Blizzy's Toolbar Loaded:{0}", blnReturn);
            return blnReturn;
        }

        /// <summary>
        /// initialises a Toolbar Button for this mod
        /// </summary>
        /// <returns>The ToolbarButtonWrapper that was created</returns>
        internal ToolbarButtonWrapper InitToolbarButton()
        {
            ToolbarButtonWrapper btnReturn;
            try
            {
                DebugLogFormatted("Initialising the Toolbar Icon");
                btnReturn = new ToolbarButtonWrapper(_ClassName, "btnToolbarIcon");
                btnReturn.TexturePath = "TriggerTech/ToolbarIcons/KSPARP";
                btnReturn.ToolTip = "Alternate Resource Panel";
                btnReturn.AddButtonClickHandler((e) =>
                {
                    ToggleOn = !ToggleOn;
                    SaveConfig();
                });
            }
            catch (Exception ex)
            {
                btnReturn = null;
                DebugLogFormatted("Error Initialising Toolbar Button: {0}", ex.Message);
            }
            return btnReturn;
        }

        /// <summary>
        /// Destroys theToolbarButtonWrapper object
        /// </summary>
        /// <param name="btnToDestroy">Object to Destroy</param>
        internal void DestroyToolbarButton(ToolbarButtonWrapper btnToDestroy)
        {
            if (btnToDestroy != null)
            {
                DebugLogFormatted("Destroying Toolbar Button");
                btnToDestroy.Destroy();
            }
            btnToDestroy = null;
        }

               
        /// <summary>
        /// Update Function - Happens on every frame - this is where behavioural stuff is typically done 
        /// </summary>
        public void Update()
        {
            //TODO:Disable this one
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F8))
                DebugActionTriggered(HighLogic.LoadedScene);

            //Activate Stage via Space Bar in MapView
            if (blnStagingSpaceInMapView && MapView.MapIsEnabled && Drawing && Input.GetKey(KeyCode.Space))
                Staging.ActivateNextStage();

        }

        public void OnFixedUpdate()
        {
            if (lstPartWindows.Count>0)
            {
                //raycast test from camera to each part

                //create the ray
                //Origin is camera, direction is the camera minus the part

                //test against part collider

                //test against all colliders and see if the part is the one thats hit


            }
        }

        private static Boolean ShowSettings = false;
        private static Boolean blnResetWindow = false;
        /// <summary>
        /// This is what we do every frame when the object is being drawn 
        /// We dont get here unless we are in the postdraw queue
        /// </summary>
        public void DrawGUI()
        {
            //Check for loaded Textures, etc
            if (!DrawStuffConfigured) {
                SetupDrawStuff();
            }

            //Draw the button - if we arent using blizzy's toolbar
            if (!(BlizzyToolbarIsAvailable && UseBlizzyToolbarIfAvailable))
            { 
                if (GUI.Button(rectButton, "Alternate",styleButtonMain))
                {
                    ToggleOn = !ToggleOn;
                    SaveConfig();
                }
            }

            //Test for moue over any component - do this on repaint so it doesn't do it on layout and cause grouping errors
            if (Event.current.type== EventType.Repaint)
                HoverOn = IsMouseOver();

            //Are there any resources left and the window is displayed?
            if ((HoverOn || ToggleOn) && (lstResources.Count > 0))
            {
                if (!blnKSPStyle && (styleTooltipStyle.normal.background != GUI.skin.box.normal.background))
                {
                    DebugLogFormatted("Enforcing Unity Styles on Start");
                    SetStylesUnity();
                    SetButtonStyles();
                }

                if (blnResetWindow)
                {
                    _WindowMainRect = new Rect(Screen.width - 298, 19, 299, 20);
                    if (!blnKSPStyle) _WindowMainRect.y += 1;
                    blnResetWindow = false;
                    SaveConfig();
                }

                //set up the main window and lock it in the screen
                Rect MainWindowPos = new Rect(_WindowMainRect) { height = _WindowMainHeight };
                MainWindowPos = ClampToScreen(MainWindowPos);
                _WindowMainRect = GUILayout.Window(_WindowMainID, MainWindowPos, FillWindow, "", stylePanel);

                //Flag that we are drawing on the screen
                Drawing = true;

                if (ShowSettings)
                {
                    Rect SettingsWindowPos = SetSettingsPosition();

                    _WindowSettingsRect=GUILayout.Window(_WindowSettingsID, SettingsWindowPos, FillSettingsWindow, "", stylePanel);

                    DrawingSettings = true;
                }


                if (lstPartWindows.Count>0)
                {
                    vectVesselCOMScreen = Camera.main.WorldToScreenPoint(FlightGlobals.ActiveVessel.findWorldCenterOfMass());

                    DrawPartWindows();
                }

#if DEBUG
                _WindowDebugRect = GUILayout.Window(_WindowDebugID, _WindowDebugRect, FillDebugWindow, "Debug");
#endif
            }
            else
            {
                Drawing = false;
                DrawingSettings = false;
                ShowSettings = false;
            }

            DrawToolTip();
        }


        int intLineHeight = 20;
        static Rect rectButton = new Rect(Screen.width - 109, 0, 80, 30);


        private Boolean IsMouseOver() 
        {
            if ((BlizzyToolbarIsAvailable && UseBlizzyToolbarIfAvailable))
                return false;

            //are we painting?
            Boolean blnRet = Event.current.type == EventType.Repaint;

            //And the mouse is over the button
            blnRet = blnRet && rectButton.Contains(Event.current.mousePosition);

            //mouse in main window
            blnRet = blnRet || (Drawing && _WindowMainRect.Contains(Event.current.mousePosition));

            ////or, the form was on the screen and the mouse is over that rectangle
            //blnRet = blnRet || (Drawing && rectPanel.Contains(Event.current.mousePosition));

            ////or, the settings form was on the screen and the mouse is over that rectangle
            blnRet = blnRet || (DrawingSettings && _WindowSettingsRect.Contains(Event.current.mousePosition));

            return blnRet;

        }
                        
        #region "Tooltip Work"
        //Tooltip variables
        //Store the tooltip text from throughout the code
        String strToolTipText = "";
        String strLastTooltipText = "";
        //is it displayed and where
        Boolean blnToolTipDisplayed = false;
        Rect rectToolTipPosition;
        int intTooltipVertOffset = 12;
        int intTooltipMaxWidth = 250;
        //timer so it only displays for a preriod of time
        float fltTooltipTime = 0f;
        float fltMaxToolTipTime = 15f;


        private void DrawToolTip()
        {
            //Added drawing check to turn off tooltips when window hides
            if (Drawing && (strToolTipText != "") && (fltTooltipTime < fltMaxToolTipTime))
            {
                GUIContent contTooltip = new GUIContent(strToolTipText);
                if (!blnToolTipDisplayed || (strToolTipText != strLastTooltipText))
                {
                    //reset display time if text changed
                    fltTooltipTime = 0f;
                    //Calc the size of the Tooltip
                    rectToolTipPosition = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y + intTooltipVertOffset, 0, 0);
                    float minwidth, maxwidth;
                    styleTooltipStyle.CalcMinMaxWidth(contTooltip, out minwidth, out maxwidth); // figure out how wide one line would be
                    rectToolTipPosition.width = Math.Min(intTooltipMaxWidth - styleTooltipStyle.padding.horizontal, maxwidth); //then work out the height with a max width
                    rectToolTipPosition.height = styleTooltipStyle.CalcHeight(contTooltip, rectToolTipPosition.width); // heers the result
                    //Make sure its not off the right of the screen
                    if (rectToolTipPosition.x + rectToolTipPosition.width > Screen.width) rectToolTipPosition.x = Screen.width - rectToolTipPosition.width;
                }
                //Draw the Tooltip
                GUI.Label(rectToolTipPosition, contTooltip, styleTooltipStyle);
                //On top of everything
                GUI.depth = 0;

                //update how long the tip has been on the screen and reset the flags
                fltTooltipTime += Time.deltaTime;
                blnToolTipDisplayed = true;
            }
            else
            {
                //clear the flags
                blnToolTipDisplayed = false;
            }
            if (strToolTipText != strLastTooltipText) fltTooltipTime = 0f;
            strLastTooltipText = strToolTipText;
        }

        public void SetTooltipText()
        {
            if (Event.current.type == EventType.Repaint)
            {
                strToolTipText = GUI.tooltip;
            }
        }
        #endregion


        #region "Control Drawing"

        Rect ClampToScreen(Rect r)
        {
            r.x = Mathf.Clamp(r.x, -1, Screen.width - r.width + 1);
            r.y = Mathf.Clamp(r.y, -1, Screen.height - r.height + 1);
            return r;
        }

        internal static void DrawResourceIcon(String ResourceName)
        {
            GUIContent contLabel;
            if (dictFirst.ContainsKey(ResourceName.ToLower()))
            {
                contLabel = new GUIContent(dictFirst[ResourceName.ToLower()]);
            }
            else if (dictSecond.ContainsKey(ResourceName.ToLower()))
            {
                contLabel = new GUIContent(dictSecond[ResourceName.ToLower()]);
            }
            else if (dictThird.ContainsKey(ResourceName.ToLower()))
            {
                contLabel = new GUIContent(dictThird[ResourceName.ToLower()]);
            }
            else
            {
                contLabel = new GUIContent(System.Text.RegularExpressions.Regex.Replace(ResourceName, "[^A-Z]", ""));
                if (ResourceName.Length < 5)
                    contLabel.text = ResourceName;
                else if (contLabel.text.Length < 2)
                    contLabel.text = ResourceName.Substring(0, 3) + "...";
            }

            contLabel.tooltip = ResourceName;
            GUILayout.Label(contLabel, styleBarName);
        }

        internal static Boolean DrawBar(int Row, GUIStyle Style, out Rect BarRect, int Width = 0, int Height = 0)
        {
            Boolean blnReturn = false;
            List<GUILayoutOption> Options = new List<GUILayoutOption>();
            if (Width == 0) Options.Add(GUILayout.ExpandWidth(true));
            else Options.Add(GUILayout.Width(Width));
            if (Height != 0) Options.Add(GUILayout.Height(Height));
            
            //GUILayout.Label("", Style, Options.ToArray());
            if (GUILayout.Button("", Style, Options.ToArray()))
                blnReturn = true;
            BarRect=GUILayoutUtility.GetLastRect();

            return blnReturn;
        }

        //private void DrawBar(Rect rectStart, int Row, GUIStyle Style)
        //{
        //    GUI.Label(rectStart, "", Style);
        //}

        internal static void DrawBarScaled(Rect rectStart, int Row, GUIStyle Style, GUIStyle StyleNarrow, float Scale)
        {
            Rect rectTemp = new Rect(rectStart);
            rectTemp.width = (float)Math.Ceiling(rectTemp.width = rectTemp.width * Scale);
            if (rectTemp.width <= 2) Style = StyleNarrow;
            GUI.Label(rectTemp, "", Style);
        }

        internal static void DrawUsage(Rect rectStart, int Row, double Amount, double MaxAmount,Boolean IgnoreInstants=false)
        {
            Rect rectTemp = new Rect(rectStart);

            if (blnShowInstants && !IgnoreInstants && (rectStart.width < 180)) rectTemp.width = (rectTemp.width * 2 / 3);
            rectTemp.x -= 20;
            rectTemp.width += 40;

            GUI.Label(rectTemp, string.Format("{0} / {1}", DisplayValue(Amount), DisplayValue(MaxAmount)), styleBarText);
        }

        internal static void DrawRate(Rect rectStart, int Row, double Rate)
        {
            Rect rectTemp = new Rect(rectStart) { width = rectStart.width - 2 };
            GUI.Label(rectTemp, string.Format("({0})", DisplayValue(Rate)), styleBarRateText);
        }

        internal static string DisplayValue(Double Amount)
        {
            if (Amount == 0) return "-.--";
            String strFormat = "{0:0}";
            if (Amount < 100)
                strFormat = "{0:0.00}";
            return string.Format(strFormat, Amount);
        }



        /// <summary>
        /// Draws a Toggle Button and sets the boolean variable to the state of the button
        /// </summary>
        /// <param name="blnVar">Boolean variable to set and store result</param>
        /// <param name="ButtonText"></param>
        /// <param name="style"></param>
        /// <param name="options"></param>
        /// <returns>True when the button state has changed</returns>
        internal static Boolean DrawToggle(ref Boolean blnVar, String ButtonText, GUIStyle style, params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, ButtonText, style, options);

            return ToggleResult(ref blnVar, ref  blnReturn);
        }

        internal static Boolean DrawToggle(ref Boolean blnVar, Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, image, style, options);

            return ToggleResult(ref blnVar, ref blnReturn);
        }

        internal static Boolean DrawToggle(ref Boolean blnVar, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            Boolean blnReturn = GUILayout.Toggle(blnVar, content, style, options);

            return ToggleResult(ref blnVar, ref blnReturn);
        }

        internal static Boolean ToggleResult(ref Boolean Old, ref Boolean New)
        {
            if (Old != New)
            {
                Old = New;
                DebugLogFormatted("Toggle Changed:" + New.ToString());
                return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Some Structured logging to the debug file
        /// </summary>
        /// <param name="Message"></param>
        internal static void DebugLogFormatted(String Message, params object[] strParams)
        {
            Message = String.Format(Message, strParams);
            String strMessageLine = String.Format("{0},{2},{1}", DateTime.Now, Message, _ClassName);
            Debug.Log(strMessageLine);
        }


#if DEBUG
        private static int _WindowDebugID = 12345;
        private static Rect _WindowDebugRect = new Rect(100, 130, 400, 200);
#endif

    }

#if DEBUG
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Debug_AutoLoadPersistentSaveOnStartup : MonoBehaviour
    {
        public static bool first = true;
        public void Start()
        {
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
