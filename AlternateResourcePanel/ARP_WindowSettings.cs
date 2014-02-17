using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

namespace KSPAlternateResourcePanel
{
    public partial class KSPAlternateResourcePanel
    {
        private static int _WindowSettingsID = 0;
        private static Rect _WindowSettingsRect = new Rect(Screen.width - 298, 200, 299, 200);
        private static int _WindowSettingsHeight = 238;

        private Rect SetSettingsPosition()
        {
            //_WindowSettingsHeight = 230 + intTest;
            Rect SettingsWindowPos = new Rect(_WindowMainRect) { y = _WindowMainRect.y + _WindowMainRect.height - 2, height = _WindowSettingsHeight };

            if (!blnKSPStyle) SettingsWindowPos.y += 1;

            //Nowfit it in the screen and move it around as the main window moves around 
            if (Screen.height < SettingsWindowPos.y + SettingsWindowPos.height)
            {
                if (0 < SettingsWindowPos.x - SettingsWindowPos.width)
                {
                    SettingsWindowPos.x = _WindowMainRect.x - _WindowMainRect.width + 2;
                    if (!blnKSPStyle) SettingsWindowPos.x -= 1;
                    SettingsWindowPos.y = Mathf.Clamp(_WindowMainRect.y, 0, Screen.height - SettingsWindowPos.height + 1);
                }
                else //if (Screen.width < SettingsWindowPos.x + SettingsWindowPos.width)
                {
                    SettingsWindowPos.x = _WindowMainRect.x + _WindowMainRect.width - 2;
                    if (!blnKSPStyle) SettingsWindowPos.x += 1;
                    SettingsWindowPos.y = Mathf.Clamp(_WindowMainRect.y, 0, Screen.height - SettingsWindowPos.height + 1);
                }
            }
            return SettingsWindowPos;
        }

        private void FillSettingsWindow(int windowHandle)
        {
            //styleSettingsArea.padding = new RectOffset(intTest, intTest2, intTest3, intTest4);
            //styleToggle.padding = new RectOffset(intTest5, intTest6, intTest7, intTest8);

            //General
            GUILayout.BeginHorizontal(styleSettingsArea, GUILayout.Width(282));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Label("General:", styleStageTextHead);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            blnShowInstants = GUILayout.Toggle(blnShowInstants, "Show Rate Change Values", styleToggle);

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //Styling
            GUILayout.BeginHorizontal(styleSettingsArea, GUILayout.Width(282), GUILayout.Height(0));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Label("Styling:", styleStageTextHead);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (DrawToggle(ref blnKSPStyle, "KSP Panels", styleToggle, GUILayout.Width(100)))
            {
                SetGUIStyles();
                if (!blnKSPStyle) blnKSPStyleButtons = false;
                SaveConfig();
            }
            if (!blnKSPStyle)
            {
                if (DrawToggle(ref blnKSPStyleButtons, "KSP Buttons", styleToggle))
                {
                    SetButtonStyles();
                    SaveConfig();
                }
            }
            GUILayout.EndHorizontal();

            if (BlizzyToolbarIsAvailable)
            {
                if (DrawToggle(ref UseBlizzyToolbarIfAvailable, new GUIContent("Use Common Toolbar", "Choose to use the Common  Toolbar or the native KSP ARP button"), styleToggle))
                {
                    if (BlizzyToolbarIsAvailable)
                    {
                        if (UseBlizzyToolbarIfAvailable)
                            btnToolbar = InitToolbarButton();
                        else
                            DestroyToolbarButton(btnToolbar);
                    }
                    SaveConfig();
                }
            }
            else
            {
                //GUILayout.BeginHorizontal();
                //GUILayout.Label("Get the Common Toolbar:", styleStageTextHead);
                //GUILayout.FlexibleSpace();
                //if (GUILayout.Button("Click here", styleTextCenterGreen))
                //    Application.OpenURL("http://forum.kerbalspaceprogram.com/threads/60863");
                //GUILayout.EndHorizontal();


                if (GUILayout.Button(new GUIContent("Click for Common Toolbar Info", "Click to open your browser and find out more about the Common Toolbar"), styleTextCenterGreen))
                    Application.OpenURL("http://forum.kerbalspaceprogram.com/threads/60863");
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //Visuals
            GUILayout.BeginHorizontal(styleSettingsArea, GUILayout.Width(282));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Label("Visuals:", styleStageTextHead);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (DrawToggle(ref blnLockLocation, "Lock Window Position", styleToggle))
                SaveConfig();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Position", styleButton))
                SaveConfig();
            if (GUILayout.Button("Reset Position", styleButton))
                blnResetWindow = true;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //Icons
            GUILayout.BeginHorizontal(styleSettingsArea, GUILayout.Width(282));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Label("Icons:", styleStageTextHead);
            GUILayout.EndVertical();

            for (int i = 0; i < lstIconOrder.Count; i++)
            {
                if (i > 0)
                {
                    if (GUILayout.Button("<->", styleButton, GUILayout.Width(30)))
                    {
                        String strTemp = lstIconOrder[i];
                        lstIconOrder[i] = lstIconOrder[i - 1];
                        lstIconOrder[i - 1] = strTemp;
                        SetIconOrder();
                        SaveConfig();
                    }
                }
                GUILayout.Label(IconOrderContent(lstIconOrder[i]), styleTextCenter, GUILayout.Width(40));
            }
            GUILayout.EndHorizontal();

            //Staging
            GUILayout.BeginHorizontal(styleSettingsArea, GUILayout.Width(282), GUILayout.Height(64));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Label("Staging:", styleStageTextHead);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            blnStaging = GUILayout.Toggle(blnStaging, "Staging Enabled", styleToggle);
            if (blnStaging)
            {
                blnStagingInMapView = GUILayout.Toggle(blnStagingInMapView, "Allow Staging in Mapview", styleToggle);
                if (blnStagingInMapView)
                    blnStagingSpaceInMapView = GUILayout.Toggle(blnStagingSpaceInMapView, "Allow Space Bar in Mapview", styleToggle);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            SetTooltipText();
        }


    }
}
