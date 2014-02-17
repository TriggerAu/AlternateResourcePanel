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
        private static int _WindowMainID = 0;
        private static Rect _WindowMainRect = new Rect(Screen.width - 298, 19, 299, 20);
        private static int _WindowMainHeight = 30;

        private void FillWindow(int WindowHandle)
        {
            GUILayout.BeginVertical();

            float fltBarRemainRatio;
            float fltBarRemainStageRatio;

            //What will the height of the panel be
            _WindowMainHeight = ((lstResources.Count + 1) * intLineHeight) + 12;

            Rect rectBar;
            //Now draw the main panel
            for (int i = 0; i < lstResources.Count; i++)
            {
                if (i > 0) GUILayout.Space(4);
                GUILayout.BeginHorizontal();

                //add title
                DrawResourceIcon(lstResources[i].Resource.name);
                GUILayout.Space(4);

                //set ration for remaining resource value
                fltBarRemainRatio = (float)lstResources[i].Amount / (float)lstResources[i].MaxAmount;

                //For resources with no stage specifics
                if (lstResources[i].Resource.resourceFlowMode == ResourceFlowMode.ALL_VESSEL)
                {
                    //full width bar
                    if (DrawBar(i, styleBarGreen_Back, out rectBar, 245))
                        TogglePartResourceVisible(lstResources[i].Resource.id);
                    if ((rectBar.width * fltBarRemainRatio) > 1)
                        DrawBarScaled(rectBar, i, styleBarGreen, styleBarGreen_Thin, fltBarRemainRatio);

                    //add amounts
                    DrawUsage(rectBar, i, lstResources[i].Amount, lstResources[i].MaxAmount);
                    //add rate
                    if (blnShowInstants) DrawRate(rectBar, i, lstResources[i].Rate);
                }
                else
                {
                    //need full Vessel and current stage bars
                    if (DrawBar(i, styleBarGreen_Back, out rectBar ,120))
                        TogglePartResourceVisible(lstResources[i].Resource.id);
                    if ((rectBar.width * fltBarRemainRatio) > 1)
                        DrawBarScaled(rectBar, i, styleBarGreen, styleBarGreen_Thin, fltBarRemainRatio);

                    //add amounts
                    DrawUsage(rectBar, i, lstResources[i].Amount, lstResources[i].MaxAmount);
                    //add rate
                    if (blnShowInstants) DrawRate(rectBar, i, lstResources[i].Rate);

                    GUILayout.Space(1);
                    ////get last stage of this resource and set it
                    arpResource resTemp = lstResourcesLastStage.FirstOrDefault(x => x.Resource.id == lstResources[i].Resource.id);
                    if (resTemp != null)
                    {
                        fltBarRemainStageRatio = (float)resTemp.Amount / (float)resTemp.MaxAmount;
                        if (DrawBar(i, styleBarBlue_Back, out rectBar, 120))
                            TogglePartResourceVisible(lstResources[i].Resource.id,true);
                        if ((rectBar.width * fltBarRemainStageRatio) > 1)
                            DrawBarScaled(rectBar, i, styleBarBlue, styleBarBlue_Thin, fltBarRemainStageRatio);

                        //add amounts
                        DrawUsage(rectBar, i, resTemp.Amount, resTemp.MaxAmount);
                        //add rate
                        if (blnShowInstants) DrawRate(rectBar, i, resTemp.Rate);
                    }
                }


                GUILayout.EndHorizontal();

            }
            GUILayout.BeginHorizontal();
            ////STAGING STUFF
            if (blnStaging)
            {
                GUILayout.Label("Stage:", styleStageTextHead, GUILayout.Width(60));
                GUIStyle styleStageNum = new GUIStyle(styleStageTextHead);
                GUIContent contStageNum = new GUIContent(Staging.CurrentStage.ToString());
                //styleStageNum.normal.textColor=new Color(173,43,43);
                //GUIContent contStageNum = new GUIContent(Staging.CurrentStage.ToString(),"NO Active Engines");
                //if (THERE ARE ACTIVE ENGINES IN STAGE)
                //{
                //contStageNum.tooltip="Active Engines";
                styleStageNum.normal.textColor = new Color(117, 206, 60);
                //}

                GUILayout.Label(contStageNum, styleStageNum, GUILayout.Width(40));

                if (blnStagingInMapView || !MapView.MapIsEnabled)
                {
                    if (GUILayout.Button("Activate Stage", styleStageButton, GUILayout.Width(100)))
                        Staging.ActivateNextStage();
                }

            }

            //rectChevron.y = rectPanel.y + rectPanel.height - rectChevron.height - 2;
            GUILayout.FlexibleSpace();
            GUIContent btnMinMax = new GUIContent(btnChevronDown, "Show Settings...");
            if (ShowSettings) { btnMinMax.image = btnChevronUp; btnMinMax.tooltip = "Hide Settings"; }
            if (GUILayout.Button(btnMinMax, styleButtonSettings))
            {
                ShowSettings = !ShowSettings;
                SaveConfig();
            }

            GUILayout.EndHorizontal();


            GUILayout.EndVertical();

            SetTooltipText();
            if (!blnLockLocation)
                GUI.DragWindow();
        }

 
    }
}
