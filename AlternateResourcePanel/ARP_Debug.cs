using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

namespace KSPAlternateResourcePanel
{
#if DEBUG
    public partial class KSPAlternateResourcePanel
    {
        public void DebugActionTimed(GameScenes loadedscene)
        {
            DebugLogFormatted("Timed Debug Action Initiated");

        }
 
        Boolean blnTriggerFlag = false;
        public void DebugActionTriggered(GameScenes loadedscene)
        {
            DebugLogFormatted("Manual Debug Action Initiated");

            blnTriggerFlag = true;

            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                ////Find the decoupler that is related to this stage
                //decoupleAt = CalcDecoupleStage(p);

                //foreach (PartResource pr in p.Resources)
                //{
                //    stgTemp = lstStages.FirstOrDefault(x => x.Number == decoupleAt);
                //    if (stgTemp == null)
                //    {
                //        stgTemp = new arpStage() { Number = decoupleAt };
                //        lstStages.Add(stgTemp);
                //    }

                //    resStageTemp = stgTemp.ResourceList.FirstOrDefault(x => x.Resource.id == pr.info.id);
                //    if (resStageTemp == null)
                //    {
                //        resStageTemp = new arpResource () { Resource = pr.info };
                //        stgTemp.ResourceList.Add(resStageTemp);
                //    }
                //    resStageTemp.Amount+=pr.amount;
                //    resStageTemp.MaxAmount+=pr.maxAmount;

                //    resTemp = lstResources.FirstOrDefault(x => x.Resource.id == pr.info.id);
                //    if (resTemp == null)
                //    {
                //        resTemp = new arpResource() { Resource = pr.info };
                //        lstResources.Add(resStageTemp);
                //    }
                //    resTemp.Amount += pr.amount;
                //    resTemp.MaxAmount += pr.maxAmount;
                //}


                DebugLogFormatted("{0},{1}", p.ClassName, p.name);
                foreach (PartResource pr in p.Resources)
                {
                    DebugLogFormatted(pr.resourceName);
                }

                foreach (PartModule pm in p.Modules)
                {
                    DebugLogFormatted(pm.ClassName);
                    switch (pm.ClassName)
                    {
                        case "ModuleAlternator":
                            foreach (ModuleResource mr in ((ModuleAlternator)pm).outputResources)
                            {
                                DebugLogFormatted("{0},{1},{2},{3},{4},{5},{6}", mr.amount, mr.available, mr.currentAmount, mr.currentRequest, mr.id, mr.name, mr.rate);
                            }
                            break;
                        case "ModuleGenerator":
                            foreach (ModuleGenerator.GeneratorResource gr in ((ModuleGenerator)pm).outputList)
                            {
                                DebugLogFormatted("{0},{1},{2},{3},{4},{5}", gr.currentAmount, gr.currentRequest, gr.id, gr.isDeprived, gr.name, gr.rate);
                            }
                            break;
                        case "ModuleLight":
                            ModuleLight ml = ((ModuleLight)pm);
                            DebugLogFormatted("{0},{1}", ml.resourceAmount, ml.resourceName);
                            break;
                        case "ModuleDeployableSolarPanel":
                            DebugLogFormatted("{0},{1},{2}", ((ModuleDeployableSolarPanel)pm).chargeRate, ((ModuleDeployableSolarPanel)pm).flowRate, ((ModuleDeployableSolarPanel)pm).powerCurve.Evaluate(((ModuleDeployableSolarPanel)pm).flowRate));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        int intTest = 0;
        int intTest2 = 0;
        int intTest3 = 0;
        int intTest4 = 0;

        int intTest5 = 0;
        int intTest6 = 0;
        int intTest7 = 0;
        int intTest8 = 0;

        float fltTest1 = 0;
        float fltTest2 = 0;

        public void FillDebugWindow(int WindowID)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("test1:");
            GUILayout.Label("test2:");
            GUILayout.Label("test3:");
            GUILayout.Label("test4:");

            GUILayout.EndVertical();
            GUILayout.BeginVertical();

            intTest = Convert.ToInt32(GUILayout.TextField(intTest.ToString()));
            intTest2 = Convert.ToInt32(GUILayout.TextField(intTest2.ToString()));
            intTest3 = Convert.ToInt32(GUILayout.TextField(intTest3.ToString()));
            intTest4 = Convert.ToInt32(GUILayout.TextField(intTest4.ToString()));

            intTest5 = Convert.ToInt32(GUILayout.TextField(intTest5.ToString()));
            intTest6 = Convert.ToInt32(GUILayout.TextField(intTest6.ToString()));
            intTest7 = Convert.ToInt32(GUILayout.TextField(intTest7.ToString()));
            intTest8 = Convert.ToInt32(GUILayout.TextField(intTest8.ToString()));

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //GUILayout.Label(fltTest1.ToString("0.000"));
            //GUILayout.Label(fltTest2.ToString("0.000"));

            //GUILayout.Label(Drawing.ToString());
            //GUILayout.Label(rectPanel.Contains(Event.current.mousePosition).ToString());
            GUILayout.Label(BlizzyToolbarIsAvailable.ToString());
            GUILayout.Label(UseBlizzyToolbarIfAvailable.ToString());
            GUILayout.Label((BlizzyToolbarIsAvailable && UseBlizzyToolbarIfAvailable).ToString());
            GUILayout.Label((!(BlizzyToolbarIsAvailable && UseBlizzyToolbarIfAvailable)).ToString());

            GUILayout.Label(IsMouseOver().ToString());
            GUILayout.Label(HoverOn.ToString());
            GUILayout.Label(ToggleOn.ToString());
            GUILayout.Label(Drawing.ToString());

            GUILayout.Label(lstResources.Count.ToString());
            //GUILayout.Label(UTUpdatePassed.ToString("0.0000"));
            //GUILayout.Label(fltTooltipTime.ToString());
            //GUILayout.Label(strToolTipText);

            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
#endif
}
