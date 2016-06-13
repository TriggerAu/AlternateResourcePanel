using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPAlternateResourcePanel
{
    class Drawing
    {
        internal static Rect DrawResourceIcon(String ResourceName)
        {
            GUIContent contLabel;
            contLabel = GetResourceIcon(ResourceName);

            contLabel.tooltip = ResourceName;
            GUILayout.Label(contLabel, Styles.styleBarName, GUILayout.ExpandWidth(false));

            //return the rect that is the position of the Icon
            return GUILayoutUtility.GetLastRect();
        }

        internal static GUIContent GetResourceIcon(String ResourceName)
        {
            GUIContent contLabel;
            if (Resources.dictFirst.ContainsKey(ResourceName.ToLower()))
            {
                contLabel = new GUIContent(Resources.dictFirst[ResourceName.ToLower()]);
            }
            else if (Resources.dictSecond.ContainsKey(ResourceName.ToLower()))
            {
                contLabel = new GUIContent(Resources.dictSecond[ResourceName.ToLower()]);
            }
            else if (Resources.dictThird.ContainsKey(ResourceName.ToLower()))
            {
                contLabel = new GUIContent(Resources.dictThird[ResourceName.ToLower()]);
            }
            else
            {
                contLabel = new GUIContent(System.Text.RegularExpressions.Regex.Replace(ResourceName, "[^A-Z]", ""));
                if (ResourceName.Length < 5)
                    contLabel.text = ResourceName;
                else if (contLabel.text.Length < 2)
                    contLabel.text = ResourceName.Substring(0, 3) + "...";
            }
            return contLabel;
        }


        internal static Boolean DrawFlowControlButton(Rect rectBar, Boolean FlowState)
        {
            Boolean blnReturn = false;

            rectBar.x += rectBar.width + 5;
            rectBar.y -= 2;
            rectBar.width = 16;
            rectBar.height = 20;

            //Set Texture here based on flowstate

            GUIContent btnFlow ;
            if (FlowState)
                btnFlow = Resources.guiFlowEnabled;
            else
                btnFlow = Resources.guiFlowDisabled;

            GUIStyle g = new GUIStyle(SkinsLibrary.CurrentSkin.button);
            g.padding = new RectOffset(-2,-3,-2,-2);

            if (GUI.Button(rectBar, btnFlow,g))
                blnReturn = true;

            return blnReturn;
        }

        internal static Boolean DrawResourceBar(Rect rectBar, ARPResource Res, GUIStyle styleBack, GUIStyle styleFront, GUIStyle styleFrontThin, Boolean ShowRates, Boolean Highlight, GUIStyle HighlightStyle)
        {
            Boolean blnReturn = false;
            Single fltBarRemainRatio = (float)Res.Amount / (float)Res.MaxAmount;

            //drawhighlight border
            if (Highlight)
            {
                Rect rectHighlight = new Rect(rectBar);
                rectHighlight.x -= 2; rectHighlight.y -= 2;
                rectHighlight.width += 4; rectHighlight.height += 4;
                GUI.Box(rectHighlight, "", HighlightStyle);
            }


            //blnReturn = Drawing.DrawBar(styleBack, out rectBar, Width);
            blnReturn = GUI.Button(rectBar, "", styleBack);

            if ((rectBar.width * fltBarRemainRatio) > 1)
                Drawing.DrawBarScaled(rectBar, Res ,styleFront, styleFrontThin, fltBarRemainRatio);

            if (!KSPAlternateResourcePanel.settings.ShowTimeRem)
            {
                ////add amounts
                Drawing.DrawUsage(rectBar, Res, ShowRates);
                ////add rate
                if (ShowRates) Drawing.DrawRate(rectBar, Res);
            }
            else
            {
                Drawing.DrawTimeRemaining(rectBar, Res);
            }

            return blnReturn;
        }

        internal static Rect CalcBarRect(Rect rectIcon, Int32 Icon2BarOffset, Int32 Width,Int32 Height)
        {
            Rect rectReturn = new Rect()
            {
                x = rectIcon.x + Icon2BarOffset,
                y = rectIcon.y,
                width = Width,
                height = Height
            };
            return rectReturn;
        }
        
        //internal static Boolean DrawBar(GUIStyle Style, out Rect BarRect, int Width = 0, int Height = 0)
        //{
        //    Boolean blnReturn = false;
        //    List<GUILayoutOption> Options = new List<GUILayoutOption>();
        //    if (Width == 0) Options.Add(GUILayout.ExpandWidth(true));
        //    else Options.Add(GUILayout.Width(Width));
        //    if (Height != 0) Options.Add(GUILayout.Height(Height));

        //    //GUILayout.Label("", Style, Options.ToArray());
        //    if (GUILayout.Button("", Style, Options.ToArray()))
        //        blnReturn = true;
        //    BarRect = GUILayoutUtility.GetLastRect();

        //    return blnReturn;
        //}

        //private void DrawBar(Rect rectStart, int Row, GUIStyle Style)
        //{
        //    GUI.Label(rectStart, "", Style);
        //}

        internal static void DrawBarScaled(Rect rectStart, ARPResource Res, GUIStyle Style, GUIStyle StyleNarrow, float Scale)
        {
            Rect rectTemp = new Rect(rectStart);
            rectTemp.width = (float)Math.Ceiling(rectTemp.width = rectTemp.width * Scale);
            if (rectTemp.width <= 2) Style = StyleNarrow;
            GUI.Label(rectTemp, "", Style);
        }

        internal static void DrawUsage(Rect rectStart, ARPResource Res, Boolean ShowRates, Boolean IgnoreInstants = false)
        {
            Rect rectTemp = new Rect(rectStart) { y = rectStart.y - 1, height = 18 };

            if (ShowRates && !IgnoreInstants && (rectStart.width < 180)) rectTemp.width = (rectTemp.width * 2 / 3);

            String strUsageString = String.Format("{0} / {1}", Res.AmountFormatted, Res.MaxAmountFormatted);;
            switch (Res.ResourceConfig.DisplayValueAs)
            {
                case ResourceSettings.DisplayUnitsEnum.Units:                                       break;
                case ResourceSettings.DisplayUnitsEnum.Tonnes:      strUsageString += " T";         break;
                case ResourceSettings.DisplayUnitsEnum.Kilograms:   strUsageString += " Kg";        break;
                case ResourceSettings.DisplayUnitsEnum.Liters:      strUsageString += " L";         break;
                default:                                                                            break;
            }
            GUI.Label(rectTemp, strUsageString, Styles.styleBarText);
        }

        //internal static Double RatePercent;
        internal static void DrawRate(Rect rectStart, ARPResource Res)
        {
            Rect rectTemp = new Rect(rectStart) { width = rectStart.width - 2, height = 18,y = rectStart.y - 1 };
            String strLabel = "";
            switch (KSPAlternateResourcePanel.settings.RateDisplayType)
            {
                case Settings.RateDisplayEnum.Default:
                    GUI.Label(rectTemp, string.Format("({0})", Res.RateFormatted), Styles.styleBarRateText);
                    break;
                case Settings.RateDisplayEnum.LeftRight:
                    //Int32 Arrows=1;
                    //RatePercent = Math.Abs(Res.Rate) / Res.MaxAmount * 100;
                    //if (RatePercent < KSPAlternateResourcePanel.APIInstance.windowDebug.intTest1)
                    //    Arrows = 1;
                    //else if (RatePercent < KSPAlternateResourcePanel.APIInstance.windowDebug.intTest2)
                    //    Arrows = 2;
                    //else if (RatePercent < KSPAlternateResourcePanel.APIInstance.windowDebug.intTest2)
                    //    Arrows = 3;
                    //else
                    //    Arrows = 4;
                                        
                    //if (Res.Rate < 0)
                    //    strLabel = new String('>', Arrows);
                    //else if (Res.Rate == 0)
                    //    strLabel = "---";
                    //else 
                    //    strLabel = new String('<', Arrows);

                    if (Res.Rate < 0)
                        strLabel = ">>>";
                    else if (Res.Rate == 0)
                        strLabel = "---";
                    else 
                        strLabel = "<<<";
                    GUI.Label(rectTemp, string.Format("{0}", strLabel), Styles.styleBarRateText);
                    break;
                case Settings.RateDisplayEnum.LeftRightPlus:
                    strLabel = Res.RateFormattedAbs;
                    if (Res.Rate < 0)
                        strLabel += " >";
                    else if (Res.Rate == 0)
                        strLabel = "---";
                    else
                        strLabel = "< " + strLabel;
                    GUI.Label(rectTemp, string.Format("{0}", strLabel), Styles.styleBarRateText);
                    break;
                case Settings.RateDisplayEnum.UpDown:
                case Settings.RateDisplayEnum.UpDownPlus:
                    if (Res.Rate < 0)
                        GUI.Label(rectTemp, Resources.texRateUp, Styles.styleBarRateText);
                    else if (Res.Rate > 0)
                        GUI.Label(rectTemp, Resources.texRateDown, Styles.styleBarRateText);

                    if (KSPAlternateResourcePanel.settings.RateDisplayType == Settings.RateDisplayEnum.UpDownPlus)
                    {
                        rectTemp = new Rect(rectTemp) { width = rectTemp.width - 10 };
                        GUI.Label(rectTemp, string.Format("{0}", Res.RateFormattedAbs), Styles.styleBarRateText);
                    }
                    break;
                default:
                    GUI.Label(rectTemp, string.Format("({0})", Res.RateFormatted), Styles.styleBarRateText);
                    break;
            }

        }

        internal static void DrawTimeRemaining(Rect rectStart, ARPResource Res)
        {
            Rect rectTemp = new Rect(rectStart) { y = rectStart.y - 1, height = 18 };

            if (Res.Rate < 0) {
                GUI.Label(rectTemp, Resources.texRateUp, Styles.styleBarRateText);

                Double TimeRemaining = Math.Abs((Res.MaxAmount - Res.Amount) / Res.Rate);
                String strTime = FormatTime(TimeRemaining);
                GUI.Label(rectTemp, strTime, Styles.styleBarText);
            }
            else if (Res.Rate > 0) {
                GUI.Label(rectTemp, Resources.texRateDown, Styles.styleBarRateText);

                Double TimeRemaining = Math.Abs(Res.Amount / Res.Rate);
                String strTime = FormatTime(TimeRemaining);
                GUI.Label(rectTemp, strTime, Styles.styleBarText);
            }
            else
            {
                GUI.Label(rectTemp, "---", Styles.styleBarText);
            }

            //GUI.Label(rectTemp, string.Format("{0} / {1}", Res.AmountFormatted, Res.MaxAmountFormatted), Styles.styleBarText);

        }

        internal static String FormatTime(Double TimeInSecs)
        {
            String strReturn = "";

            Double Second = TimeInSecs % 60;
            Double Minute = Math.Truncate((TimeInSecs / 60) % 60);
            Double HourRaw = TimeInSecs / 60 / 60;
            
            Double Hour;
            if (GameSettings.KERBIN_TIME)
                Hour = Math.Truncate(HourRaw % 6);
            else
                Hour = Math.Truncate(HourRaw % 24);

            Double Day;
            if (GameSettings.KERBIN_TIME)
                Day = Math.Truncate((HourRaw % (426 * 6)) / 6);
            else
                Day = Math.Truncate((HourRaw % (365 * 24)) / 24);

            Double Year;
            if (GameSettings.KERBIN_TIME)
                Year = Math.Truncate((HourRaw / (426 * 6)));
            else
                Year = Math.Truncate((HourRaw / (365 * 24)));

            if (Year > 0)
                strReturn = String.Format("{0:0}y {1:0}d {2:00}:{3:00}:{4:00}", Year, Day, Hour, Minute, Second);
            else if (Day > 0)
                strReturn = String.Format("{0:0}d {1:00}:{2:00}:{3:00}", Day, Hour, Minute, Second);
            else if (Hour > 0)
                strReturn = String.Format("{1:0}:{2:00}:{3:00}", Day, Hour, Minute, Second);
            else 
                strReturn = String.Format("{2:0}:{3:00.0}", Day, Hour, Minute, Second);
            return strReturn;
        }



        //****************************************************************************************************
        //  static function DrawLine(rect : Rect) : void
        //  static function DrawLine(rect : Rect, color : Color) : void
        //  static function DrawLine(rect : Rect, width : float) : void
        //  static function DrawLine(rect : Rect, color : Color, width : float) : void
        //  static function DrawLine(Vector2 pointA, Vector2 pointB) : void
        //  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color) : void
        //  static function DrawLine(Vector2 pointA, Vector2 pointB, width : float) : void
        //  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color, width : float) : void
        //  
        //  Draws a GUI line on the screen.
        //  
        //  DrawLine makes up for the severe lack of 2D line rendering in the Unity runtime GUI system.
        //  This function works by drawing a 1x1 texture filled with a color, which is then scaled
        //   and rotated by altering the GUI matrix.  The matrix is restored afterwards.
        //****************************************************************************************************

        public static Texture2D lineTex;

        public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
        public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
        public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
        public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
        public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
        public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            // Save the current GUI matrix and color, since we're going to make changes to it.
            Matrix4x4 matrix = GUI.matrix;
            Color savedColor = GUI.color;

            // Generate a single pixel texture if it doesn't exist
            if (!lineTex) { lineTex = new Texture2D(1, 1); lineTex.SetPixel(0, 0, color); lineTex.Apply(); }
            // and set the GUI color to the color parameter
            GUI.color = color;

            // Determine the angle of the line.
            Single angle = Vector3.Angle(pointB - pointA, Vector2.right);

            // Vector3.Angle always returns a positive number.
            // If pointB is above pointA, then angle needs to be negative.
            if (pointA.y > pointB.y) { angle = -angle; }

            // Set the rotation for the line. The angle was calculated with pointA as the origin.
            GUIUtility.RotateAroundPivot(angle, pointA);

            // Finally, draw the actual line. we've rotated the GUI, so now we draw the length and width
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, (pointA-pointB).magnitude, width), lineTex);

            // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
            GUI.matrix = matrix;
            GUI.color = savedColor;
        }

        //public static void DrawLine_Orig(Vector2 pointA, Vector2 pointB, Color color, float width)
        //{
        //    // Save the current GUI matrix, since we're going to make changes to it.
        //    Matrix4x4 matrix = GUI.matrix;

        //    // Generate a single pixel texture if it doesn't exist
        //    if (!lineTex) { lineTex = new Texture2D(1, 1); }

        //    // Store current GUI color, so we can switch it back later,
        //    // and set the GUI color to the color parameter
        //    Color savedColor = GUI.color;
        //    GUI.color = color;

        //    // Determine the angle of the line.
        //    Single angle = Vector3.Angle(pointB - pointA, Vector2.right);

        //    // Vector3.Angle always returns a positive number.
        //    // If pointB is above pointA, then angle needs to be negative.
        //    if (pointA.y > pointB.y) { angle = -angle; }

        //    // Use ScaleAroundPivot to adjust the size of the line.
        //    // We could do this when we draw the texture, but by scaling it here we can use
        //    //  non-integer values for the width and length (such as sub 1 pixel widths).
        //    // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
        //    //  is centered on the origin at pointA.
        //    GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));

        //    // Set the rotation for the line.
        //    //  The angle was calculated with pointA as the origin.
        //    GUIUtility.RotateAroundPivot(angle, pointA);

        //    // Finally, draw the actual line.
        //    // We're really only drawing a 1x1 texture from pointA.
        //    // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
        //    //  render with the proper width, length, and angle.
        //    GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);

        //    // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
        //    GUI.matrix = matrix;
        //    GUI.color = savedColor;
        //}

    }


}
