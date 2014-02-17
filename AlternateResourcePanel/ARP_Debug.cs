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


            //UIPartActionWindow win = new UIPartActionWindow();
            //win.Setup(FlightGlobals.ActiveVessel.Parts[0], UIPartActionWindow.DisplayType.ResourceOnly, UI_Scene.Flight);
            //win.UpdateWindow();



            ////UIPartActionController ct = UIPartActionController.Instance;
            ////String s = "";
            ////foreach (int i1 in ct.resourcesShown)
            ////{
            ////    s+= i1.ToString() + ",";
            ////}
            ////DebugLogFormatted(s);

            //return;

            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                //p.SetHighlightColor(Color.red);
                //p.SetHighlightType(Part.HighlightType.AlwaysOn);
                //p.SetHighlight(true);

                //GUILayout.Label(string.Format("{0}-{1}-{2}-{3}", p.name, p.Actions.Count, p.Events.Count, p.Fields.Count));
                foreach (PartModule pm in p.Modules)
                {
                    //GUILayout.Label(string.Format("{0}-{1}-{2}-{3}", pm.moduleName, pm.Actions.Count, pm.Events.Count, pm.Fields.Count));

                    foreach (BaseAction kspa in pm.Actions)
                    {
                        DebugLogFormatted("{0}-{1}-Action-{2}", p.name, pm.moduleName, kspa.guiName);
                    }
                    foreach (BaseEvent kspe in pm.Events)
                    {
                        DebugLogFormatted("{0}-{1}-Event-{2}-{3}", p.name, pm.moduleName, kspe.guiName, kspe.guiActive);
                    }
                    foreach (BaseField kspf in pm.Fields)
                    {
                        DebugLogFormatted("{0}-{1}-Field-{2}-{3}", p.name, pm.moduleName, kspf.guiName, kspf.guiActive);
                    }

                }

                //foreach (BaseEvent e in p.Events)
                //{
                //    GUILayout.Label(string.Format("{0}-{1}", e.GUIName, e.active));
                //}
                //p.highlightColor = Color.red;
                //p.highlightType = Part.HighlightType.AlwaysOn;

                //foreach (BaseAction a in p.Actions)
                //{
                //    a.active = true;
                //    DebugLogFormatted(a.name);
                //}
            }
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

        internal static int intTest = 0;
        internal static int intTest2 = 0;
        internal static int intTest3 = 0;
        internal static int intTest4 = 0;

        internal static int intTest5 = 0;
        internal static int intTest6 = 0;
        internal static int intTest7 = 0;
        internal static int intTest8 = 0;

        internal float fltTest1 = 0;
        internal float fltTest2 = 0;

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

            //intTest5 = Convert.ToInt32(GUILayout.TextField(intTest5.ToString()));
            //intTest6 = Convert.ToInt32(GUILayout.TextField(intTest6.ToString()));
            //intTest7 = Convert.ToInt32(GUILayout.TextField(intTest7.ToString()));
            //intTest8 = Convert.ToInt32(GUILayout.TextField(intTest8.ToString()));

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //GUILayout.Label(fltTest1.ToString("0.000"));
            //GUILayout.Label(fltTest2.ToString("0.000"));

            //GUILayout.Label(Drawing.ToString());
            //GUILayout.Label(rectPanel.Contains(Event.current.mousePosition).ToString());
            //GUILayout.Label(BlizzyToolbarIsAvailable.ToString());
            //GUILayout.Label(UseBlizzyToolbarIfAvailable.ToString());
            //GUILayout.Label((BlizzyToolbarIsAvailable && UseBlizzyToolbarIfAvailable).ToString());
            //GUILayout.Label((!(BlizzyToolbarIsAvailable && UseBlizzyToolbarIfAvailable)).ToString());

            //GUILayout.Label(IsMouseOver().ToString());
            //GUILayout.Label(HoverOn.ToString());
            //GUILayout.Label(ToggleOn.ToString());
            //GUILayout.Label(Drawing.ToString());

            //GUILayout.Label(lstResources.Count.ToString());
            //GUILayout.Label(UTUpdatePassed.ToString("0.0000"));
            //GUILayout.Label(fltTooltipTime.ToString());
            //GUILayout.Label(strToolTipText);

            GUILayout.Label(Camera.main.transform.position.magnitude.ToString());
            GUILayout.Label(behavrun.TotalMilliseconds.ToString());

            GUILayout.Label(vectVesselCOMScreen.x.ToString());
            GUILayout.Label(vectVesselCOMScreen.y.ToString());

            foreach (int it in SelectedResources.Keys)
            {
                GUILayout.Label(String.Format("{0}-{1}-{2}",it,SelectedResources[it].AllVisible,SelectedResources[it].LastStageVisible));
            }

            foreach (arpPartWindow pw in lstPartWindows)
            {
                GUILayout.Label(string.Format("{0}-{1}-{2}-{3}-{4}", pw.Part.partInfo.title,pw.WindowID,
                                Math.Round(pw.PartScreenPos.x,0), Math.Round(pw.PartScreenPos.y,0), Math.Round(pw.PartScreenPos.z,0)));

                //GUILayout.Label(string.Format("{0}",Vector3.Angle( pw.Part.transform.up,Camera.main.transform.up)));

                GUILayout.Label(string.Format("{0}-{1}-{2}", pw.LeftSide, Math.Round(pw.PartScreenPos.x, 0), vectVesselCOMScreen.x + pw.SideThreshold));
                
                //GUILayout.Label(String.Format("{0}-{1}", Camera.main.WorldToViewportPoint(pw.Part.transform.position).x, Camera.main.WorldToViewportPoint(pw.Part.transform.position).y));
                


                //GUILayout.Label(string.Format("{0},{1},{2}", pw.Part.CoMOffset.magnitude, pw.Part.transform.position.magnitude,pw.Part.collider.bounds.extents.magnitude));

                //foreach (KeyValuePair<int,arpResource> ri in pw.ResourceList)
                //{
                //    GUILayout.Label(string.Format("    {0}-{1}-{2}", ri.Value.Resource.name, ri.Value.Amount, ri.Value.MaxAmount));
                //}
            }

            //Part p2 = FlightGlobals.ActiveVessel.Parts[0];
            //GUILayout.Label(Camera.main.WorldToScreenPoint(p2.transform.position).x.ToString());
            //GUILayout.Label(Camera.main.WorldToScreenPoint(p2.transform.position).y.ToString());
            //GUILayout.Label(Camera.main.WorldToScreenPoint(p2.transform.position).z.ToString());
            //p2 = FlightGlobals.ActiveVessel.Parts[2];
            //GUILayout.Label(Camera.main.WorldToScreenPoint(p2.transform.position).x.ToString());
            //GUILayout.Label(Camera.main.WorldToScreenPoint(p2.transform.position).y.ToString());
            //GUILayout.Label(Camera.main.WorldToScreenPoint(p2.transform.position).z.ToString());
            //GUILayout.Label(lstStages.LastStage.ToString());
            //GUILayout.Label(p2.GetInstanceID().ToString());
            //GUILayout.Label(p2.protoPartRef.uid.ToString());

            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
            //    //p.SetHighlightColor(Color.red);
            //    //p.SetHighlightType(Part.HighlightType.AlwaysOn);
            //    //p.SetHighlight(true);

            //    GUILayout.Label(string.Format("{0}-{1}-{2}-{3}", p.name,p.Actions.Count, p.Events.Count, p.Fields.Count));
            //    foreach (PartModule pm in p.Modules)
            //    {
            //        GUILayout.Label(string.Format("{0}-{1}-{2}-{3}", pm.moduleName, pm.Actions.Count, pm.Events.Count, pm.Fields.Count));
            //    }
                
            //    //foreach (BaseEvent e in p.Events)
            //    //{
            //    //    GUILayout.Label(string.Format("{0}-{1}", e.GUIName, e.active));
            //    //}
            //    //p.highlightColor = Color.red;
            //    //p.highlightType = Part.HighlightType.AlwaysOn;

            //    //foreach (BaseAction a in p.Actions)
            //    //{
            //    //    a.active = true;
            //    //    DebugLogFormatted(a.name);
            //    //}
            //}

            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
#endif
}
