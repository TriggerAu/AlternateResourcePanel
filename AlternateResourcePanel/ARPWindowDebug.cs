using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPAlternateResourcePanel
{
#if DEBUG
    internal class ARPWindowDebug: MonoBehaviourWindowPlus
    {

        //internal ARPWindowDebug()
        //{
        //}
        //internal ARPWindowDebug(String text)
        //    : base(text)
        //{

        //}

        //TODO: Look at using this
        //  http://answers.unity3d.com/questions/445444/add-component-in-one-line-with-parameters.html

        internal KSPAlternateResourcePanel mbARP;
        internal Settings settings;

        public Int32 intTest1=0;
        public Int32 intTest2=0;
        public Int32 intTest3=0;
        public Int32 intTest4 = 0;
        public Int32 intTest5 = 200;
        internal override void DrawWindow(int id)
        {
            //GUILayout.Label(Drawing.RectTest.ToString());
            //GUILayout.Label(Drawing.RectTest2.ToString());
            //DrawTextBox(ref Drawing.RateYOffset);
            DrawTextBox(ref intTest1);
            DrawTextBox(ref intTest2);
            DrawTextBox(ref intTest3);
            DrawTextBox(ref intTest4);
            DrawTextBox(ref intTest5);

            //if (DrawButton("KSP")) SkinsLibrary.SetCurrent( SkinsLibrary.DefSkinType.KSP);
            //if (DrawButton("Unity")) SkinsLibrary.SetCurrent(SkinsLibrary.DefSkinType.Unity);
            //if (DrawButton("UnityandKSP")) SkinsLibrary.SetCurrent("UnityStyleWKSPBtns");

            //GUIContent temp = new GUIContent("Red Text Hopefully", "Tooltip this");

            //if (GUILayout.Button(temp, "RedButton"))
            //    LogFormatted_DebugOnly("Clicked");

            //if (GUILayout.Button("Togglefixedwidthtooltip"))
            //{
            //    if (TooltipMaxWidth > 0)
            //        TooltipMaxWidth = 0;
            //    else
            //        TooltipMaxWidth = 250;
            //}

            //base.DrawWindow(id);
            GUILayout.BeginVertical();


            GUILayout.Label(String.Format("Repeating Worker: {0:0.00}ms  Config:{1}-{2}-{3}", mbARP.RepeatingWorkerDuration.TotalMilliseconds, mbARP.RepeatingWorkerRunning, mbARP.RepeatingWorkerInitialWait, mbARP.RepeatingWorkerRate));
            GUILayout.Label(String.Format("UT Passed: {0}s",mbARP.RepeatingWorkerUTPeriod.ToString()));

            GUILayout.Label(String.Format("Draw Settings Duration: {0:0.00}ms", mbARP.windowSettings.DrawWindowInternalDuration.TotalMilliseconds));
            GUILayout.Label(String.Format("Draw Main Duration: {0:0.00}ms", mbARP.windowMain.DrawWindowInternalDuration.TotalMilliseconds));

            GUILayout.Label(KSPAlternateResourcePanel.HoverOn.ToString());
            GUILayout.Label(KSPAlternateResourcePanel.ShowAll.ToString());

            GUILayout.Label(String.Format("Transfers: {0}", mbARP.lstTransfers.Count));
            foreach (ARPTransfer item in mbARP.lstTransfers)
            {
                GUILayout.Label(String.Format("T:{0}-{1}-{2}", item.partID,item.ResourceID,item.transferState));
            }
            if (GUILayout.Button("AAA"))
            {
                LogFormatted("{0}",mbARP.lstTransfers.Any(x => x.partID == intTest4));
                foreach (ARPTransfer a in mbARP.lstTransfers)
                {
                    LogFormatted("{0}-{1}-{2}-{3}-{4}",intTest4, a.partID, a.part.GetInstanceID(), a.ResourceID, a.resource.id);
                }
            }
            foreach (IGrouping<Int32,ARPTransfer> item in mbARP.lstTransfers.Where(x => x.Active).GroupBy(x => x.ResourceID))
	        {
                GUILayout.Label(item.Key.ToString());
		    }
            GUILayout.Label(mbARP.TestTrans);
            
            foreach (String item in mbARP.lstString)
            {
                GUILayout.Label(item);
                
            }
            //#region Auto Staging
            //GUILayout.Label(FlightGlobals.ActiveVessel.ctrlState.mainThrottle.ToString());

            //GUILayout.Label(String.Format("en:{0} delay:{1}, {2:0.0}", settings.AutoStagingEnabled, settings.AutoStagingDelayInTenths, ((Double)settings.AutoStagingDelayInTenths / 10)));
            //GUILayout.Label(String.Format("Arm:{0}-run:{1}", mbARP.AutoStagingArmed, mbARP.AutoStagingRunning));
            
            //GUILayout.Label(String.Format("Staging:{0}-Stop{1}", Staging.CurrentStage, mbARP.AutoStagingTerminateAt));

            //GUILayout.Label(string.Format("{0}-{1}-{2}", mbARP.AutoStagingTriggeredAt, Planetarium.GetUniversalTime(), Planetarium.GetUniversalTime() - mbARP.AutoStagingTriggeredAt));

            //GUILayout.Label(string.Format("{0}", FlightInputHandler.fetch.stageLock));

            //if (GUILayout.Button("GO BABY GO"))
            //    mbARP.AutoStagingArmed = true;

            //foreach (Part item in mbARP.lstPartsLastStageEngines)
            //{
            //    GUILayout.Label(String.Format("{0}-{1}", item.partName, item.partInfo.name));
            //    foreach (ModuleEngines me in item.Modules.OfType<ModuleEngines>())
            //    {
            //        GUILayout.Label(string.Format("   {0}-{1}-{2}", me.getFlameoutState, me.staged, me.propellants.Count));
            //        String Props = "    ";
            //        for (int i = 0; i < me.propellants.Count; i++)
            //        {
            //            Props += string.Format("{0}:{1}, ",me.propellants[i].name,me.propellants[i].id);
            //        }
            //        GUILayout.Label(Props);
            //    }
            //}
            //#endregion

            //GUILayout.Label(API.VesselResources.Count.ToString());
            //GUILayout.Label(API.AResource.ResourceDef.name + ":" + API.AResource.Amount);

            //if (GUILayout.Button("Toggle"))
            //    API.TestBool = !API.TestBool;
            //if (GUILayout.Button("AddRes"))
            //{
            //    LogFormatted(API.AResource.Amount.ToString());
            //    API.AResource.Amount += 20;
            //    LogFormatted(API.AResource.Amount.ToString());
            //}

            //GUILayout.Label(mbARP.MouseOverToolbarBtn.ToString());

            //foreach (ARPResource r in mbARP.lstResourcesVessel.Values)
            //{
            //    //GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}", r.ResourceDef.name, r.AmountFormatted, r.MaxAmountFormatted, r.RateFormatted, r.AmountLastFormatted));  //, r.RateFormatted2, r.RateSamples.Count));
            //    //GUILayout.Label(String.Format("{0}-{1}-{2}-{3:0}-{4}-{5}", r.ResourceDef.name, r.AmountFormatted, r.MaxAmountFormatted, r.Amount / r.MaxAmount * 100, KSPAlternateResourcePanel.settings.Resources[r.ResourceDef.id].MonitorWarningLevel, r.MonitorWarning));  //, r.RateFormatted2, r.RateSamples.Count));
            //    GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}", r.ResourceDef.name, r.MonitorWarning,r.MonitorAlert,r.MonitorWorstHealth,r.AlarmAcknowledged));  //, r.RateFormatted2, r.RateSamples.Count));
            //}

            //foreach (Int32 item in mbARP.lstResourcesToDisplay)
            //{
            //    if (item == 0)
            //        GUILayout.Label("SEP");
            //    else
            //        GUILayout.Label(mbARP.lstResourcesVessel[item].ResourceDef.name);
            //}


            //GUILayout.Label(FlightCamera.fetch.transform.position.ToString());
            //GUILayout.Label(FlightCamera.fetch.mainCamera.transform.position.ToString());

            //GUILayout.Label(FlightCamera.fetch.mainCamera.WorldToScreenPoint(FlightGlobals.ActiveVessel.transform.position).ToString());
            //GUILayout.Label(FlightGlobals.ActiveVessel.findWorldCenterOfMass().ToString());
            //GUILayout.Label((FlightCamera.fetch.transform.position - FlightGlobals.ActiveVessel.findWorldCenterOfMass()).ToString());
            //foreach (Camera item in FlightCamera.fetch.cameras)
            //{
            //GUILayout.Label(item.name + item.transform.position.ToString());
                
            //}
            //GUILayout.Label(String.Format("{0}", mbARP.windowMain.rectHighlight1.ToString()));
            //GUILayout.Label(String.Format("{0}", mbARP.windowSettings.WindowRect.ToString()));

            //GUILayout.Label("MBWindows: " + GameObject.FindObjectsOfType(typeof(MonoBehaviourWindow)).Length.ToString());

            //GUILayout.Label(String.Format("t1: {0:0.00}ms", mbARP.t1.TotalMilliseconds));
            //GUILayout.Label(String.Format("t1: {0:0.00}ms", mbARP.LeftPos.ToString()));
            //GUILayout.Label(String.Format("t1: {0:0.00}ms", mbARP.RightPos.ToString()));
            //GUILayout.Label(String.Format("middle: {0}", mbARP.MiddleNum));

            //foreach (int it in mbARP.SelectedResources.Keys)
            //{
            //    GUILayout.Label(String.Format("{0}-{1}-{2}", it, mbARP.SelectedResources[it].AllVisible, mbARP.SelectedResources[it].LastStageVisible));
            //}

            //GUILayout.Label(mbARP.lstPartWindows.Count.ToString());

            //if (mbARP.LeftWindows.Count > 0)
            //{
            //    foreach (ARPPartWindow pw in mbARP.LeftWindows)
            //    {
            //        GUILayout.Label(string.Format("L{0}-{1:0}-{2:0}", pw.PartRef.partInfo.title, pw.WindowRect.y, pw.ScreenNextWindowY));
            //    }
            //}
            //if (mbARP.RightWindows.Count > 0)
            //{
            //    foreach (ARPPartWindow pw in mbARP.RightWindows)
            //    {
            //        GUILayout.Label(string.Format("R{0}-{1:0}-{2:0}", pw.PartRef.partInfo.title, pw.WindowRect.y, pw.ScreenNextWindowY));
            //    }
            //}

            //foreach (ARPPartWindow pw in mbARP.lstPartWindows.Values)
            //{
            //    //    GUILayout.Label(string.Format("{0}-{1}-{2}-{3}-{4}", pw.PartRef.partInfo.title, pw.WindowID,
            //    GUILayout.Label(string.Format("{0}-{1}-{2}", (pw.PartRef.transform.position).ToString(), pw.vectLinePartEnd.ToString(), pw.vectLineWindowEnd.ToString()));
            //}
            //foreach (ARPPartWindow pw in mbARP.lstPartWindows.Values)
            //{
            //    GUILayout.Label(string.Format("{0}-{1}-{2}-{3}-{4}", pw.PartRef.partInfo.title, pw.WindowID,
                    //GUILayout.Label(string.Format("{0}-{1}-{2}",pw.PartScreenPos.ToString(),pw.vectLinePartEnd.ToString(), pw.vectLineWindowEnd.ToString()));
                    
            //    //     Math.Round(pw.PartScreenPos.x, 0), Math.Round(pw.PartScreenPos.y, 0), Math.Round(pw.PartScreenPos.z, 0)));

            //    //GUILayout.Label(string.Format("{0}-{1}-{2}", pw.LeftSide, Math.Round(pw.PartScreenPos.x, 0), vectVesselCOMScreen.x + pw.SideThreshold));




            //    //GUILayout.Label(string.Format("{0},{1},{2}", pw.Part.CoMOffset.magnitude, pw.Part.transform.position.magnitude,pw.Part.collider.bounds.extents.magnitude));

            //    foreach (KeyValuePair<int,ARPResource> ri in pw.ResourceList)
            //    {
            //        GUILayout.Label(string.Format("{0}-{1}-{2}-{3}", ri.Value.ResourceDef.name, ri.Value.Amount.ToString("0.0"), ri.Value.AmountLast.ToString("0.0"), (ri.Value.AmountLast - ri.Value.Amount)));
            //    }
            //}



            //foreach (ARPResource r in mbARP.lstResourcesVessel.Values)
            //{
            //    GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}", r.ResourceDef.name, r.AmountFormatted, r.MaxAmountFormatted, r.RateFormatted, r.AmountLastFormatted));  //, r.RateFormatted2, r.RateSamples.Count));
            //}
            //foreach (Int32 item in mbARP.ActiveResources)
            //{
            //    GUILayout.Label(item.ToString());
            //}

            //foreach (ARPResource r in mbARP.lstResourcesLastStage.Values)
            //{
            //    GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}", r.ResourceDef.name, r.AmountFormatted, r.MaxAmountFormatted, r.RateFormatted, r.AmountLastFormatted));  //, r.RateFormatted2, r.RateSamples.Count));
            //}

            //foreach (ARPResource r in KSPAlternateResourcePanel.lstResourcesLastStage)
            //{
            //    GUILayout.Label(String.Format("{0}-{1}-{2}-{3}", r.ResourceDef.name, r.Amount, r.MaxAmount, r.RateFormatted));
            //}

            //foreach (ARPPartWindow pw in KSPAlternateResourcePanel.lstPartWindows.Values)
            //{
            //    foreach (ARPResource r in pw.ResourceList.Values)
            //    {
            //        GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}", r.ResourceDef.name, r.Amount, r.MaxAmount, r.RateFormatted, r.AmountLast));  //, r.RateFormatted2, r.RateSamples.Count));
            //    }
            //}

            //foreach (Part p in KSPAlternateResourcePanel.lstParts)
            //{
            //    GUILayout.Label(String.Format("{0}-{1}", p.GetInstanceID(), p.partInfo.title));
            //}

            GUILayout.EndHorizontal();
        }
   
    }
#endif
}