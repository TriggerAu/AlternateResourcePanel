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
        //StageGroup Button
        //Autostage option
        //Settings??
        //Remember toggle state/settings

        //GlobalSettings
        private Boolean HoverOn = false;    //Are we hovering on something to draw the screen
        private Boolean ToggleOn = false;   //Has the draw been toggled on
        private Boolean Drawing = false;    //Am I drawing the window
        private Boolean DrawingSettings = false;    //Am I drawing the Settings Bit
        private static String _ClassName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name;

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

            //Load Textures
            LoadTextures();

            //Load Settings here?
            LoadSettings();

            //Add it to the queue
            DebugLogFormatted("Adding to DrawQueue");
            RenderingManager.AddToPostDrawQueue(0, DrawGUI);

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
        }

        
        /// <summary>
        /// Update Function - Happens on every frame - this is where behavioural stuff is typically done 
        /// </summary>
        public void Update()
        {
            //TODO:Disable this one
            //if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.F8))
            //    DebugActionTriggered(HighLogic.LoadedScene);

            //Activate Stage via Space Bar in MapView
            if (blnStagingSpaceInMapView && MapView.MapIsEnabled && Drawing && Input.GetKey(KeyCode.Space))
                Staging.ActivateNextStage();

        }

        int intLineHeight = 20;
        static Rect rectButton = new Rect(Screen.width - 109, 0, 80, 20);
        static Rect rectPanel = new Rect(Screen.width - 298, 19, 299, 20);
        static Rect rectBarWide = new Rect(Screen.width - 250, 26, 245, 15);
        static Rect rectBarLeft = new Rect(Screen.width - 250,26, 120, 15);
        static Rect rectBarRight = new Rect(Screen.width - 125, 26, 120, 15);
        static Rect rectPanelLabel = new Rect(Screen.width - 290,18, 32, 16 );

        static Rect rectChevron = new Rect(Screen.width - 20, 19, 20, 20);
        static Rect rectSettingsPanel = new Rect(rectPanel);
        static Rect rectSettingsStaging;

        Boolean blnStaging = false;
        Boolean blnStagingInMapView = false;
        Boolean blnStagingSpaceInMapView = false;

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

            //Draw the button
            
            if (GUI.Button(rectButton, "Alternate",styleButton))
            {
                ToggleOn = !ToggleOn;
                SaveSettings();
            }

            //Test for moue over any component
            HoverOn = IsMouseOver();

            if (HoverOn || ToggleOn)
            {
                //Are there any resources left?
                if (lstResources.Count > 0)
                {
                    //Flag that we are drawing on the screen
                    Drawing = true;

                    float fltBarY ;
                    float fltBarRemainRatio;
                    float fltBarRemainStageRatio;

                    //What will the height of the panel be
                    rectPanel.height = (lstResources.Count + 1) * intLineHeight + 10;

                    //Are we drawing the settings panel - draw this first so it's behind the main panel
                    if (DrawingSettings)
                    {
                        rectSettingsPanel.height = 70;
                        rectSettingsPanel.y = rectPanel.y + rectPanel.height - 6;
                        GUI.Box(rectSettingsPanel, "", stylePanel);

                        rectSettingsStaging = new Rect(rectSettingsPanel.x+60,rectSettingsPanel.y+2,150,20);

                        GUI.Label(new Rect(rectSettingsPanel.x+5,rectSettingsPanel.y+5,80,22),"Staging:",styleStageTextHead);
                        blnStaging = GUI.Toggle(rectSettingsStaging, blnStaging, "Staging Enabled", styleToggle);
                        if (blnStaging)
                        {
                            blnStagingInMapView = GUI.Toggle(new Rect(rectSettingsStaging) { y = rectSettingsStaging.y + 20 }, blnStagingInMapView, "Allow Staging in Mapview", styleToggle);
                            if (blnStagingInMapView)
                                blnStagingSpaceInMapView = GUI.Toggle(new Rect(rectSettingsStaging) { y = rectSettingsStaging.y + 40 }, blnStagingSpaceInMapView, "Allow Space Bar in Mapview", styleToggle);
                        }

                    }
                    
                    //Now draw the main panel
                    GUI.Box(rectPanel, "", stylePanel);
                    for (int i = 0; i < lstResources.Count; i++)
                    {
                        //set y location of first bar
                        fltBarY = rectBarWide.y + (i * intLineHeight);
                        //set ration for remaining resource value
                        fltBarRemainRatio = (float)lstResources[i].Amount / (float)lstResources[i].MaxAmount;
                        
                        //For resources with no stage specifics
                        if (lstResources[i].Resource.resourceFlowMode == ResourceFlowMode.ALL_VESSEL)
                        {
                            //full width bar
                            DrawBar(rectBarWide, i, styleBarGreen_Back);
                            if ((rectBarLeft.width * fltBarRemainRatio) > 1)
                                DrawBarScaled(rectBarWide, i, styleBarGreen, styleBarGreen_Thin, fltBarRemainRatio);

                            //add amounts
                            GUI.Label(new Rect(rectBarWide) { y = fltBarY + intTest3 }, string.Format("{0} / {1}", DisplayValue(lstResources[i].Amount), DisplayValue(lstResources[i].MaxAmount)), styleBarText);
                            ////add rate
                            //GUI.Label(new Rect(rectBarWide) { y = fltBarY + intTest3 }, string.Format("({0:0.00})", lstResources[i].Rate), styleBarRateText);
                        }
                        else
                        {
                            //need full Vessel and current stage bars
                            DrawBar(rectBarLeft, i, styleBarGreen_Back);
                            if ((rectBarLeft.width * fltBarRemainRatio) > 1)
                                DrawBarScaled(rectBarLeft, i, styleBarGreen, styleBarGreen_Thin, fltBarRemainRatio);

                            //add amounts
                            GUI.Label(new Rect(rectBarLeft) { y = fltBarY + intTest3 }, string.Format("{0} / {1}", DisplayValue(lstResources[i].Amount), DisplayValue(lstResources[i].MaxAmount)), styleBarText);
                            ////add rate
                            //GUI.Label(new Rect(rectBarLeft) { y = fltBarY + intTest3 }, string.Format("({0:0.00})", lstResources[i].Rate), styleBarRateText);

                            //get last stage of this resource and set it
                            arpResource resTemp = lstResourcesLastStage.FirstOrDefault(x => x.Resource.id == lstResources[i].Resource.id);
                            if (resTemp != null)
                            {
                                fltBarRemainStageRatio = (float)resTemp.Amount / (float)resTemp.MaxAmount;
                                DrawBar(rectBarRight, i, styleBarBlue_Back);
                                if ((rectBarRight.width * fltBarRemainStageRatio) > 1)
                                    DrawBarScaled(rectBarRight, i, styleBarBlue, styleBarBlue_Thin, fltBarRemainStageRatio);

                                //add amounts
                                GUI.Label(new Rect(rectBarRight) { y = fltBarY + intTest3 }, string.Format("{0} / {1}", DisplayValue(resTemp.Amount), DisplayValue(resTemp.MaxAmount)), styleBarText);
                                ////add rate
                                //GUI.Label(new Rect(rectBarRight) { y = fltBarY + intTest3 }, string.Format("({0:0.00})", resTemp.Rate), styleBarRateText);
                            }
                        }

                        //add title
                        GUIContent contLabel;
                        Rect rectLabel = new Rect(rectPanelLabel) { y = fltBarY , x = rectPanelLabel.x  };
                        switch (lstResources[i].Resource.name.ToLower())
                        {
                            case "electriccharge": contLabel = new GUIContent(texEC); break;
                            case "liquidfuel": contLabel = new GUIContent(texLF); break;
                            case "oxidizer": contLabel = new GUIContent(texOX); break;
                            case "monopropellant": contLabel = new GUIContent(texMP); break;
                            case "solidfuel": contLabel = new GUIContent(texSF); break;
                            case "xenongas": contLabel = new GUIContent(texXe); break;
                            case "intakeair": contLabel = new GUIContent(texIA); break;
                            default:
                                contLabel = new GUIContent(System.Text.RegularExpressions.Regex.Replace(lstResources[i].Resource.name, "[^A-Z]", ""));
                                if (contLabel.text.Length < 2) contLabel.text = lstResources[i].Resource.name.Substring(0, 2);
                                break;
                        }
                        contLabel.tooltip = lstResources[i].Resource.name;
                        GUI.Label(rectLabel, contLabel, styleBarName);

                    }

                    //STAGING STUFF
                    if (blnStaging)
                    {
                        GUI.Label(new Rect(rectPanel.x + 5, rectPanel.y + rectPanel.height - 22, 100, 22), "Stage:", styleStageTextHead);
                        GUIStyle styleStageNum = new GUIStyle(styleStageTextHead);
                        GUIContent contStageNum = new GUIContent(Staging.CurrentStage.ToString());
                        //styleStageNum.normal.textColor=new Color(173,43,43);
                        //GUIContent contStageNum = new GUIContent(Staging.CurrentStage.ToString(),"NO Active Engines");
                        //if (THERE ARE ACTIVE ENGINES IN STAGE)
                        //{
                            //contStageNum.tooltip="Active Engines";
                            styleStageNum.normal.textColor = new Color(117,206,60);
                        //}

                        GUI.Label(new Rect(rectPanel.x + 70, rectPanel.y + rectPanel.height - 22, 100, 22), contStageNum, styleStageNum);

                        if (blnStagingInMapView || !MapView.MapIsEnabled)
                        {
                            if (GUI.Button(new Rect(rectPanel.x + 100, rectPanel.y + rectPanel.height - 22, 100, 18), "Activate Stage", styleStageButton))
                                Staging.ActivateNextStage();
                        }

                    }

                    rectChevron.y = rectPanel.y + rectPanel.height - rectChevron.height-2;

                    GUIContent btnMinMax = new GUIContent(btnChevronDown,"Show Settings...");
                    if (DrawingSettings) { btnMinMax.image = btnChevronUp; btnMinMax.tooltip = "Hide Settings"; }
                    if (GUI.Button(rectChevron, btnMinMax, styleButton))
                    {
                        DrawingSettings = !DrawingSettings;
                        SaveSettings();
                    }

                }

            }
            else
            {
                Drawing = false;
                DrawingSettings = false;
            }

            SetTooltipText();
            DrawToolTip();

            //_WindowDebugRect = GUILayout.Window(_WindowDebugID, _WindowDebugRect, FillDebugWindow, "Debug");

        }
        //private static int _WindowDebugID = 12345;
        //private static Rect _WindowDebugRect = new Rect(Screen.width-800,130,400,200);



        private void DrawBar(Rect rectStart,int Row,GUIStyle Style,float xOffset=0,float yOffset=0 )
        {
            Rect rectTemp = new Rect(rectStart);
            rectTemp.x += xOffset;
            rectTemp.y += (Row * intLineHeight) + yOffset;
            GUI.Label(rectTemp, "", Style);
        }

        private void DrawBarScaled(Rect rectStart, int Row, GUIStyle Style, GUIStyle StyleNarrow, float Scale, float xOffset = 0, float yOffset = 0)
        {
            Rect rectTemp = new Rect(rectStart);
            rectTemp.x += xOffset;
            rectTemp.y += (Row * intLineHeight) + yOffset;
            Math.Ceiling( rectTemp.width *= Scale);
            if (rectTemp.width <= 2) Style = StyleNarrow;
            GUI.Label(rectTemp, "", Style);
        }

        private string DisplayValue(Double Amount)
        {
            String strFormat = "{0:0}";
            if (Amount<100) 
                strFormat = "{0:0.00}";
            return string.Format(strFormat, Amount);
        }
       
        private Boolean IsMouseOver() 
        {
            //are we painting?
            Boolean blnRet = Event.current.type == EventType.Repaint;

            //And the mouse is over the button
            blnRet = blnRet && rectButton.Contains(Event.current.mousePosition);

            //or, the form was on the screen and the mouse is over that rectangle
            blnRet = blnRet || (Drawing && rectPanel.Contains(Event.current.mousePosition));

            //or, the settings form was on the screen and the mouse is over that rectangle
            blnRet = blnRet || (DrawingSettings && rectSettingsPanel.Contains(Event.current.mousePosition));

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
            if (strToolTipText != "" && (fltTooltipTime < fltMaxToolTipTime))
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

        
        /// <summary>
        /// Some Structured logging to the debug file
        /// </summary>
        /// <param name="Message"></param>
        public static void DebugLogFormatted(String Message, params object[] strParams)
        {
            Message = String.Format(Message, strParams);
            String strMessageLine = String.Format("{0},{2},{1}", DateTime.Now, Message, _ClassName);
            Debug.Log(strMessageLine);

        }
    }

    
    //[KSPAddon(KSPAddon.Startup.MainMenu, false)]
    //public class Debug_AutoLoadPersistentSaveOnStartup : MonoBehaviour
    //{
    //    public static bool first = true;
    //    public void Start()
    //    {
    //        if (first)
    //        {
    //            first = false;
    //            HighLogic.SaveFolder = "default";
    //            var game = GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);
    //            if (game != null && game.flightState != null && game.compatible)
    //            {
    //                FlightDriver.StartAndFocusVessel(game, 6);
    //            }
    //            //CheatOptions.InfiniteFuel = true;
    //        }
    //    }
    //}

}
