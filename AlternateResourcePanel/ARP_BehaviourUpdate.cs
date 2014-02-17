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
        #region "Here is where the repeating Function is started"
        public void SetupRepeatingFunction_BehaviourUpdate(int ChecksPerSec)
        {
            SetupRepeatingFunction("BehaviourUpdate", ChecksPerSec);
        }
        public void SetupRepeatingFunction(String FunctionName, int ChecksPerSec)
        {
            float Interval = 1F / ChecksPerSec;
            SetupRepeatingFunction(FunctionName, Interval);
        }
        public void SetupRepeatingFunction(String FunctionName, float SecsInterval)
        {
            Debug.Log(SecsInterval);
            if (IsInvoking(FunctionName))
            {
                DebugLogFormatted("Cancelling repeating Behaviour({0})", FunctionName);
                CancelInvoke(FunctionName);
            }
            DebugLogFormatted("Setting up repeating Behaviour({1}) every {0:0.00} Secs", SecsInterval, FunctionName);
            InvokeRepeating(FunctionName, SecsInterval, SecsInterval);
        }
        #endregion

        //Here's the work to do
        private arpStageList lstStages = new arpStageList();
        private arpResourceList lstResources = new arpResourceList();
        private arpResourceList lstResourcesLastStage = new arpResourceList();

        private double UTUpdateLast = 0;
        private double UTUpdate = 0;
        private double UTUpdatePassed = 0;


        private DateTime behav;
        private TimeSpan behavrun;
        public void BehaviourUpdate()
        {
            behav = DateTime.Now;
            //Dont bother doing the work if the panel is not displayed
            if (!(HoverOn || ToggleOn))
                return;

            Vessel active = FlightGlobals.ActiveVessel;

            //lstPartsLast = lstParts;
            arpResourceList lstLastResources = lstResources;
            arpResourceList lstLastResourcesLastStage = lstResourcesLastStage;
            UTUpdate = Planetarium.GetUniversalTime();
            UTUpdatePassed = UTUpdate - UTUpdateLast;

            lstStages = new arpStageList();
            lstResources = new arpResourceList();
            lstResourcesLastStage = new arpResourceList();

            //lstPartWindows = new List<PartWindow>();

            arpStage stgTemp;
            //for each type of resource
            foreach (Part p in active.Parts)
	        {
                foreach (PartResource pr in p.Resources)
                {
                    //Build the stages based on decouplers
                    stgTemp = lstStages.FirstOrDefault(x => x.Number == p.DecoupledAt());
                    if (stgTemp == null)
                    {
                        stgTemp = new arpStage() { Number = p.DecoupledAt() };
                        lstStages.Add(stgTemp);
                    }

                    //Add the resources to stages
                    AddResourceToList(ref stgTemp.ResourceList, pr);

                    //Now to the list of resources
                    AddResourceToList(ref lstResources, pr);
                }
	        }

            if (lstStages.Count > 0) { 
                //Now the list for the last stage before decouplers for resources where this counts
                arpStage arpStageLast = lstStages.OrderBy(x => x.Number).Last();
                arpResource resTemp;
                foreach (arpResource r in lstResources)
                {
                    if (r.Resource.resourceFlowMode != ResourceFlowMode.ALL_VESSEL)
                    {
                        resTemp = arpStageLast.ResourceList.FirstOrDefault(x => x.Resource.id == r.Resource.id);
                        if (resTemp != null)
                        {
                            lstResourcesLastStage.Add(new arpResource() {
                                                        Resource = resTemp.Resource,
                                                        Amount = resTemp.Amount,
                                                        MaxAmount = resTemp.MaxAmount
                                                        }
                                                    );
                        }

                    }
                }

                //This is the stuff to update rates
                if (UTUpdate != 0)
                {
                    CalculateRates(lstResources, lstLastResources);

                    CalculateRates(lstResourcesLastStage, lstLastResourcesLastStage);
                }
            }
            UTUpdateLast = UTUpdate;

            //Loop through the partWindows and reset the amounts/rates
            lstPartWindows.ClearValues();
            //remove any windows for parts that are no longer attached
            lstPartWindows.ClearWindows();
            //Loop through the parts to see if we are displaying partwindows
            foreach (Part p in active.Parts)
            {
                foreach (PartResource pr in p.Resources)
                {
                    //Now Check/Add PartWindow Stuff
                    if (SelectedResources.ContainsKey(pr.info.id))
                        AddResourceToPartWindow(p, pr);
                    else if (lstPartWindows.Any(x=>x.Part.GetInstanceID()==p.GetInstanceID()))
                        RemoveResourceFromPartWindow(p,pr);
                }
            }

            //if (tmeOutput.AddSeconds(10) < DateTime.Now)
            //{
            //    DebugLogFormatted("{0}-{1}", Staging.lastStage, Staging.CurrentStage);

            //    foreach (Part p in active.Parts)
            //    {
            //        DebugLogFormatted("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
            //            p.partInfo.title,
            //            p.childStageOffset,
            //        p.defaultInverseStage,
            //        p.inStageIndex,
            //        p.inverseStage,
            //        p.manualStageOffset,
            //        p.originalStage,
            //        p.stageAfter,
            //        p.stageBefore,
            //        p.stageOffset,
            //        CalcDecoupleStage(p),
            //        p.GetInstanceID().ToString()

            //            );
            //    }

            //    //work out per stage values for max and used
            //    foreach (arpStage s in lstStages)
            //    {
            //        foreach (arpResource r in s.ResourceList)
            //        {
            //            DebugLogFormatted("{0},{1},{2},{3},{4}",
            //                                s.Number,
            //                                r.Resource.name,
            //                                r.Amount,
            //                                r.MaxAmount,
            //                                r.Rate);
            //        }
            //    }

            //    //foreach (arpStage s in lstStages)
            //    //{
            //    //    foreach (arpResource r in s.ResourceList.Where(x=>x.Resource.name=="LiquidFuel"))
            //    //    {
            //    //        DebugLogFormatted("{0},{1},{2},{3}",
            //    //                            s.Number,
            //    //                            r.Resource.name,
            //    //                            r.Amount,
            //    //                            r.MaxAmount);
            //    //    }
            //    //}

            //    foreach (arpResource r in lstResources)
            //    {
            //            DebugLogFormatted("{0},{1},{2},{3},{4},{5},{6},{7}",
            //                                r.Resource.id,
            //                                r.Resource.name,
            //                                r.Resource.resourceFlowMode.ToString(),
            //                                r.Resource.color.ToString(),
            //                                r.Resource.density,
            //                                r.Amount,
            //                                r.MaxAmount,
            //                                r.Rate);

            //    }
            //    DebugLogFormatted("Last Stage");
            //    foreach (arpResource r in lstResourcesLastStage)
            //    {
            //        DebugLogFormatted("{0},{1},{2},{3},{4},{5},{6},{7}",
            //                            r.Resource.id,
            //                            r.Resource.name,
            //                            r.Resource.resourceFlowMode.ToString(),
            //                            r.Resource.color.ToString(),
            //                            r.Resource.density,
            //                            r.Amount,
            //                            r.MaxAmount,
            //                            r.Rate);

            //    }
                //DebugLogFormatted("CurrentStage:{0}", Staging.CurrentStage);
            //    tmeOutput = DateTime.Now;
            //}

            behavrun = DateTime.Now - behav;
    
        }

        private arpResource AddResourceToList(ref arpResourceList list, PartResource pr)
        {
            arpResource resTemp;
            resTemp = list.FirstOrDefault(x => x.Resource.id == pr.info.id);
            if (resTemp == null)
            {
                resTemp = new arpResource() { Resource = pr.info };
                list.Add(resTemp);
            }
            resTemp.Amount += pr.amount;
            resTemp.MaxAmount += pr.maxAmount;
            return resTemp;
        }

        private arpResource AddResourceToList(ref Dictionary<int,arpResource> list, PartResource pr)
        {
            arpResource resTemp;
            if (!list.ContainsKey(pr.info.id))
            { 
                resTemp = new arpResource() { Resource = pr.info };
                list.Add(pr.info.id, resTemp);
            }
            resTemp = list[pr.info.id];
            resTemp.Amount += pr.amount;
            resTemp.MaxAmount += pr.maxAmount;
            return resTemp;
        }

        private void CalculateRates(arpResourceList lstCurrent, arpResourceList lstLast)
        {
            foreach (arpResource r in lstCurrent)
            {
                arpResource rLast = lstLast.FirstOrDefault(x => x.Resource.id == r.Resource.id);
                if (rLast != null)
                {
                    //This is for physwarp when time sometimes doesnt pass
                    if (UTUpdatePassed == 0)
                        r.Rate = rLast.Rate;
                    else
                        r.Rate = (rLast.Amount - r.Amount) / UTUpdatePassed;
                }
                else
                {
                    r.Rate = 0;
                }
            }
        }

        private DateTime tmeOutput = DateTime.Now;


        //private int DecoupledInStage(Part part , int stage = -1)
        //{

        //    if (IsDecoupler(part))
        //    {
        //        if (part.inverseStage > stage)
        //        {
        //            stage = part.inverseStage;
        //        }
        //    }

        //    if (part.parent != null)
        //    {
        //        stage = DecoupledInStage(part.parent, stage);
        //    }

        //    return stage;
        //}
        //private bool IsDecoupler(Part part)
        //{
        //    return part is Decoupler || part is RadialDecoupler || part.Modules.OfType<ModuleDecouple>().Count() > 0 || part.Modules.OfType<ModuleAnchoredDecoupler>().Count() > 0;
        //}

        internal static int CalcDecoupleStage(Part pTest)
        {
            int stageOut = -1;

            //Is this part a decoupler
            if (pTest.Modules.OfType<ModuleDecouple>().Count() > 0 || pTest.Modules.OfType<ModuleAnchoredDecoupler>().Count() > 0)
            {
                stageOut = pTest.inverseStage;
            } 
            //if not look further up the tree
            else if (pTest.parent !=null)
            {
                stageOut = CalcDecoupleStage(pTest.parent);
            }
            return stageOut;
    }
    }
}
