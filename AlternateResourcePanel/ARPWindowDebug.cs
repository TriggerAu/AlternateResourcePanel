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

        public Int32 intTest1 = 339;
        public Int32 intTest2=0;
        public Int32 intTest3=299;
        public Int32 intTest4 = 20;
        public static Int32 intTest5 = 10;

        //ApplicationLauncherButton origResButton=null;
        //Boolean Clicked = false;
        internal override void DrawWindow(int id)
        {

            ;

            

            mbARP.windowMainResetPos = new Rect(Screen.width - intTest1 - (GameSettings.UI_SCALE_APPS * 42), intTest2, intTest3, intTest4);

            //GUILayout.Label(Drawing.RectTest.ToString());
            //GUILayout.Label(Drawing.RectTest2.ToString());
            //DrawTextBox(ref Drawing.RateYOffset);
            DrawTextBox(ref intTest1);
            DrawTextBox(ref intTest2);
            DrawTextBox(ref intTest3);
            DrawTextBox(ref intTest4);
            DrawTextBox(ref intTest5);

            //mbARP.windowResourceConfig.vectMonTypeOffset = new Vector2(intTest1,intTest2); //Vector2(8, 56);
            //mbARP.windowResourceConfig.vectDisplayAsOffset = new Vector2(intTest3,intTest4);

            
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
            GUILayout.Label(String.Format("UT Passed: {0}s", mbARP.RepeatingWorkerUTPeriod.ToString()));
            GUILayout.Label(String.Format("RT Passed: {0}s", (mbARP.RepeatingWorkerUTPeriod * TimeWarp.CurrentRate).ToString()));

            GUILayout.Label(String.Format("Draw Settings Duration: {0:0.00}ms", mbARP.windowSettings.DrawWindowInternalDuration.TotalMilliseconds));
            GUILayout.Label(String.Format("Draw Main Duration: {0:0.00}ms", mbARP.windowMain.DrawWindowInternalDuration.TotalMilliseconds));

            //Boolean blnControl = false;
            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
            //    foreach (ModuleCommand mc in p.Modules.OfType<ModuleCommand>())
            //    {
            //        if (mc.State == ModuleCommand.ControlSourceState.Good)
            //            blnControl = true;
            //    }
            //}
            //GUILayout.Label(String.Format("CommandState:{0}", blnControl));
            GUILayout.Label(String.Format("vesseliscontrollable:{0}", mbARP.blnVesselIsControllable));

            //if (mbARP.lstPartWindows != null)
            //{
            //    foreach (ARPPartWindow pw in mbARP.lstPartWindows.Values)
            //    {
            //        //    GUILayout.Label(string.Format("{0}-{1}-{2}-{3}-{4}", pw.PartRef.partInfo.title, pw.WindowID,
            //        GUILayout.Label(string.Format("{0}-{1}", (pw.PartRef.transform.position).ToString(), pw.WindowRect));
            //    }
            //}

            //for (int i = 1; i < mbARP.lstResourcesVesselPerStage.Keys.Max(); i++)
            //{
            //    GUILayout.Label(String.Format("{0}-{1}",i,mbARP.lstResourcesVesselPerStage[i].Values.Count));
            //}

            //GUILayout.Label(String.Format("AutostaginTermAt: {0}", mbARP.AutoStagingTerminateAt));
            //GUILayout.Label(String.Format("lstLastStageEngineModules: {0}", mbARP.lstLastStageEngineModules.Count));
            //GUILayout.Label(String.Format("lstLastStageEngineFXModules: {0}", mbARP.lstLastStageEngineFXModules.Count));

            //GUILayout.Label(String.Format("AutostaginTriggeredAt: {0}", mbARP.AutoStagingTerminateAt));
            //GUILayout.Label(String.Format("lstStage Engine count: {0}", mbARP.lstPartsLastStageEngines.Count));
            //GUILayout.Label(String.Format("LastStage Resource Sum: {0}", mbARP.lstResourcesLastStage.Sum(r => r.Value.Amount)));
            
            

            //Stuff for extra staging

            //if (GUILayout.Button("Save Parts")){
            //    System.IO.File.Delete(String.Format("{0}/AllStages2.csv", Resources.PathPlugin));
            //    System.IO.File.AppendAllText(String.Format("{0}/AllStages2.csv", Resources.PathPlugin),
            //                String.Format("StageOffset,StageBefore,StageAfter,DecoupledAt,Resource,Amount\r\n"),
            //                Encoding.ASCII);
            //    foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //    {
            //        foreach (PartResource pr in p.Resources)
            //        {
            //            System.IO.File.AppendAllText(String.Format("{0}/AllStages2.csv", Resources.PathPlugin),
            //                String.Format("{0},{1},{2},{3},{4},{5}\r\n",p.stageOffset,p.stageBefore,p.stageAfter,p.DecoupledAt(),pr.info.name,pr.amount),
            //                Encoding.ASCII);
            //        }
            //        //GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", item.getFlameoutState, item.getIgnitionState, item.EngineIgnited, item.isOperational, item.staged, item.status));
            //    }

            //}

            //GUILayout.Label(String.Format("IntakeAir Requested:{0}",mbARP.IntakeAirRequested));

            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
            //    foreach (ModuleEngines pe in p.Modules.OfType<ModuleEngines>())
            //    {
            //        foreach (Propellant pep in pe.propellants.Where(prop => prop.name == "IntakeAir"))
            //        {
            //            GUILayout.Label(String.Format("{0}-{1}:{2}",p.partInfo.name,pep.name,pep.currentRequirement));
            //        }
            //    }
            //    foreach (ModuleEnginesFX pe in p.Modules.OfType<ModuleEnginesFX>())
            //    {
            //        foreach (Propellant pep in pe.propellants.Where(prop => prop.name == "IntakeAir"))
            //        {
            //            GUILayout.Label(String.Format("FX:{0}-{1}:{2}", p.partInfo.name, pep.name, pep.currentRequirement));
            //        }
            //    }
            //}
            //Stuff for TAC Life Support
            //foreach (ARPResource Res in mbARP.lstResourcesVessel.Values)
            //{
            //    GUILayout.Label(String.Format("{2}:{0:0}|{1}", Math.Abs(Res.Amount / Res.Rate), Drawing.FormatTime(Math.Abs(Res.Amount / Res.Rate)),Res.ResourceDef.name.Substring(0,3)));


            ////    GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", r.ResourceDef.name, String.Format("{0} / {1} T", r.AmountFormatted, r.MaxAmountFormatted), r.AmountFormatted, r.Rate, r.RateFormatted, r.IsEmpty, r.EmptyAt.ToString("HH:mm:ss"), r.IsFull, r.FullAt.ToString("HH:mm:ss")));  //, r.RateFormatted2, r.RateSamples.Count));

            //////    if (r.Rate!=0)
            //////    GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", r.ResourceDef.name, r.AmountFormatted, r.MaxAmountFormatted, r.Rate, Math.Abs(r.Amount / r.Rate),Drawing.FormatTime(Math.Abs(r.Amount / r.Rate))));  //, r.RateFormatted2, r.RateSamples.Count));
            //////    //    //GUILayout.Label(String.Format("{0}-{1}-{2}-{3:0}-{4}-{5}", r.ResourceDef.name, r.AmountFormatted, r.MaxAmountFormatted, r.Amount / r.MaxAmount * 100, KSPAlternateResourcePanel.settings.Resources[r.ResourceDef.id].MonitorWarningLevel, r.MonitorWarning));  //, r.RateFormatted2, r.RateSamples.Count));
            //////    //GUILayout.Label(String.Format("{0}-{1}-{2}", r.ResourceDef.name, r.MonitorState, r.AlarmState));  //, r.RateFormatted2, r.RateSamples.Count));
            //}


            //if (GUILayout.Button("SetTrue"))
            //{
            //    //mbARP.btnAppLauncher.toggleButton.onTrue();
            //    mbARP.btnAppLauncher.SetTrue(true);
            //}
            //if (GUILayout.Button("SetFalse"))
            //{
            //    //mbARP.btnAppLauncher.toggleButton.onFalse();
            //    mbARP.btnAppLauncher.SetFalse(true);
            //    //if (origResButton==null)
            //}


                        //GUILayout.Label(mbARP.btnAppLauncher.State.ToString());

            //if(GUILayout.Button("Replace"))
            //{
            //    //if (origResButton==null)
            //    //    origResButton = (ApplicationLauncherButton)Instantiate(ResourceDisplay.Instance.appLauncherButton);

            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onDisable();
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onHover = mbARP.btnAppLauncher.toggleButton.onHover;
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onHoverOut = mbARP.btnAppLauncher.toggleButton.onHoverOut;
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onTrue = mbARP.btnAppLauncher.toggleButton.onTrue;
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onFalse = mbARP.btnAppLauncher.toggleButton.onFalse;
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onEnable = mbARP.btnAppLauncher.toggleButton.onEnable;
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onDisable = mbARP.btnAppLauncher.toggleButton.onDisable;
            //    ResourceDisplay.Instance.appLauncherButton.SetTexture(Resources.texAppLaunchIcon);

            //}
            //if (GUILayout.Button("Revert"))
            //{
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onHover = origResButton.toggleButton.onHover;
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onHoverOut = origResButton.toggleButton.onHoverOut;
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onTrue = origResButton.toggleButton.onTrue;
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onFalse = origResButton.toggleButton.onFalse;
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onEnable = origResButton.toggleButton.onEnable;
            //    ResourceDisplay.Instance.appLauncherButton.toggleButton.onDisable = origResButton.toggleButton.onDisable;
            //}
            //ApplicationLauncherButton[] lstButtons = KSPAlternateResourcePanel.FindObjectsOfType<ApplicationLauncherButton>();
            //if (lstButtons!=null){
            //GUILayout.Label(String.Format("Buttons:{0}",lstButtons.Length));
            //foreach (ApplicationLauncherButton item in lstButtons)
            //{
            //    try
            //    {
            //        GUILayout.Label(String.Format("{0}", item.name));
            //        GUILayout.Label(String.Format("{0}", item.tag));
            //        GUILayout.Label(String.Format("{0}", item.container.Text));
            //    }
            //    catch (Exception)
            //    {
                    
            //    }
            //}
            //}

            //GUILayout.Label(String.Format("{0}", settings.ButtonStyleChosen));
            //GUILayout.Label(String.Format("{0}", settings.ButtonStyleToDisplay));


            //GUILayout.Label(String.Format("{0}",mbARP.windowResourceConfig.MousePosition));
            //GUILayout.Label(String.Format("{0}", mbARP.windowResourceConfig.MousePosition + new Vector2(mbARP.windowDebug.intTest1, mbARP.windowDebug.intTest2)));

            //GUILayout.Label(String.Format("Over:{0}", mbARP.windowResourceConfig.resourceOver == null ? "None" : mbARP.windowResourceConfig.resourceOver.name));
            //GUILayout.Label(String.Format("OverIcon:{0}", mbARP.windowResourceConfig.iconOver == null ? "None" : mbARP.windowResourceConfig.iconOver.name));
            //if (mbARP.windowResourceConfig.resourceDrag != null)
            //{
            //    GUILayout.Label(String.Format("ResDrag:{0}", mbARP.windowResourceConfig.resourceDrag.name));
            //    GUILayout.Label(String.Format("Reorder:{0}", mbARP.windowResourceConfig.DropWillReorderList));
            //    GUILayout.Label(String.Format("resourceOverUpper:{0}", mbARP.windowResourceConfig.resourceOverUpper));
            //    GUILayout.Label(String.Format("resourceInsertIndex:{0}", mbARP.windowResourceConfig.resourceInsertIndex));
                
            //}

            //GUILayout.Label(String.Format("Scroll-up/down:{0}/{1}", mbARP.windowResourceConfig.blnScrollUp, mbARP.windowResourceConfig.blnScrollDown));
            //GUILayout.Label(String.Format("ScrollPos:{0}", mbARP.windowResourceConfig.ScrollPosition));

            //foreach (ARPWindowResourceConfig.ResourcePosition item in mbARP.windowResourceConfig.lstResPositions)
            //{
            //    GUILayout.Label(String.Format("{0}({1})-{2}", item.name, item.id, item.resourceRect));
            //}

            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
            //    foreach (PartResource pr in p.Resources)
            //    {
            //        if (pr.info.name == "ElectricCharge")
            //        {
            //            GUILayout.Label(String.Format("{0}-{1}-{2}", p.partInfo.name,pr.flowMode,pr.flowState));
                        
            //        }
            //    }
            //}

            //GUILayout.Label(InputLockManager.IsLocked(ControlTypes.STAGING).ToString());
            //foreach (KeyValuePair<string,ulong> item in InputLockManager.lockStack)
            //{
            //    GUILayout.Label(String.Format("{0},{1}", item.Key.ToString(), item.Value.ToString()));
            //}
            //if (GUILayout.Button("Lock"))
            //{
            //    if (InputLockManager.IsLocked(ControlTypes.STAGING))
            //        if (InputLockManager.lockStack.ContainsKey("manualStageLock"))
            //            InputLockManager.RemoveControlLock("manualStageLock");
            //    else
            //        InputLockManager.SetControlLock(ControlTypes.STAGING, "manualStageLock");
            //}

            //foreach (Part item in FlightGlobals.ActiveVessel.Parts)
            //{
            //    GUILayout.Label(String.Format("{0}-{1}", item.partInfo.name,item.partInfo.typeDescription));
            //    //GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", item.getFlameoutState, item.getIgnitionState, item.EngineIgnited, item.isOperational, item.staged, item.status));
            //}

            //foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            //{
            //    foreach (PartModule item in p.Modules)
            //    {
            //        GUILayout.Label(String.Format("{0}", item.ClassName));

            //        if (item.ClassName=="ModuleEnginesFX")
            //        {
            //            ModuleEnginesFX a = (ModuleEnginesFX)item;
            //            GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", a.getFlameoutState, a.getIgnitionState, a.EngineIgnited, a.isOperational, a.staged, a.status));
            //        }
            //    }
            //}


            //GUILayout.Label(KSPAlternateResourcePanel.HoverOn.ToString());
            //GUILayout.Label(KSPAlternateResourcePanel.ShowAll.ToString());

            GUILayout.Label(String.Format("Transfers: {0}", mbARP.lstTransfers.Count));
            foreach (ARPTransfer item in mbARP.lstTransfers)
            {
                GUILayout.Label(String.Format("T:{0}-{1}-{2}", item.partID,item.ResourceID,item.transferState));
            }

            //if (GUILayout.Button("AAA"))
            //{
            //    LogFormatted("{0}",mbARP.lstTransfers.Any(x => x.partID == intTest4));
            //    foreach (ARPTransfer a in mbARP.lstTransfers)
            //    {
            //        LogFormatted("{0}-{1}-{2}-{3}-{4}",intTest4, a.partID, a.part.GetInstanceID(), a.ResourceID, a.resource.id);
            //    }
            //}
            //foreach (IGrouping<Int32,ARPTransfer> item in mbARP.lstTransfers.Where(x => x.Active).GroupBy(x => x.ResourceID))
            //{
            //    GUILayout.Label(item.Key.ToString());
            //}
            //GUILayout.Label(mbARP.TestTrans);

            //foreach (String item in mbARP.lstString)
            //{
            //    GUILayout.Label(item);

            //}
            //#region Auto Staging
            //GUILayout.Label(FlightGlobals.ActiveVessel.ctrlState.mainThrottle.ToString());

            //GUILayout.Label(String.Format("en:{0} delay:{1}, {2:0.0}", settings.AutoStagingEnabled, settings.AutoStagingDelayInTenths, ((Double)settings.AutoStagingDelayInTenths / 10)));
            //GUILayout.Label(String.Format("Arm:{0}-run:{1}", mbARP.AutoStagingArmed, mbARP.AutoStagingRunning));

            //GUILayout.Label(String.Format("{0}", Staging.StageCount));
            //GUILayout.Label(String.Format("Staging:{0}-Stop{1}", Staging.CurrentStage, mbARP.AutoStagingTerminateAt));

            //GUILayout.Label(String.Format("{0}", KSP.UI.Screens.StageManager.StageCount));
            //GUILayout.Label(String.Format("Staging:{0}-Stop{1}", KSP.UI.Screens.StageManager.CurrentStage, mbARP.AutoStagingTerminateAt));
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
            //    //    GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}", r.ResourceDef.name, r.AmountFormatted, r.MaxAmountFormatted, r.RateFormatted, r.IsEmpty, r.EmptyAt.ToString("HH:mm:ss"), r.IsFull, r.FullAt.ToString("HH:mm:ss")));  //, r.RateFormatted2, r.RateSamples.Count));

            //    //    //GUILayout.Label(String.Format("{0}-{1}-{2}-{3:0}-{4}-{5}", r.ResourceDef.name, r.AmountFormatted, r.MaxAmountFormatted, r.Amount / r.MaxAmount * 100, KSPAlternateResourcePanel.settings.Resources[r.ResourceDef.id].MonitorWarningLevel, r.MonitorWarning));  //, r.RateFormatted2, r.RateSamples.Count));
            //    GUILayout.Label(String.Format("{0}-{1}-{2}", r.ResourceDef.name, r.MonitorState, r.AlarmState));  //, r.RateFormatted2, r.RateSamples.Count));
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
            //    //GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}", r.ResourceDef.name, r.AmountFormatted, r.MaxAmountFormatted, r.RateFormatted, r.AmountLastFormatted));  //, r.RateFormatted2, r.RateSamples.Count));
            ////    GUILayout.Label(String.Format("{0}-{1}-{2}-{3}-{4}", r.ResourceDef.name,settings.ResourcesToSplitFlowDisabled.Contains(r.ResourceDef.name), r.AmountFormatted, mbARP.lstResourcesLastStage[r.ResourceDef.id].AmountFormatted,r.ResourceDef.resourceFlowMode));  //, r.RateFormatted2, r.RateSamples.Count));
            //    GUILayout.Label(String.Format("{0}-{1}", r.ResourceDef.name,r.Amount));
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