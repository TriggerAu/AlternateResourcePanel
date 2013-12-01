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
        
        //Classes for resources and stages        
        /// <summary>
        /// A Resource type
        /// </summary>
        class arpResource
        {
            public PartResourceDefinition Resource { get; set; }
            public Double Amount { get; set; }
            public Double MaxAmount { get; set; }

            public Double Rate { get; set; }
        }
        
        /// <summary>
        /// Stage based on decouplers, not stage view
        /// </summary>
        class arpStage
        {
            public arpResourceList ResourceList = new arpResourceList();
            public int Number;
        }


        class arpResourceList : List<arpResource>
        {
            //public double UTgrabbed; 

            //public arpResourceList()
            //{
            //    UTgrabbed = Planetarium.GetUniversalTime();
            //}
        }

        class arpStageList : List<arpStage>
        {
            //public double lastGrab;

            //public arpStageList()
            //{
            //    lastGrab = Planetarium.GetUniversalTime();
            //}
        }

        //Here's the work to do
        private arpStageList lstStages = new arpStageList();
        private arpResourceList lstResources = new arpResourceList();
        private arpResourceList lstResourcesLastStage = new arpResourceList();

        private double UTUpdateLast = 0;
        private double UTUpdate = 0;
        private double UTUpdatePassed = 0;

        public void BehaviourUpdate()
        {
            //Dont bother doing the work if the panel is not displayed
            if (!(HoverOn || ToggleOn))
                return;

            Vessel active = FlightGlobals.ActiveVessel;

            arpResourceList lstLastResources = lstResources;
            arpResourceList lstLastResourcesLastStage = lstResourcesLastStage;
            UTUpdate = Planetarium.GetUniversalTime();
            UTUpdatePassed = UTUpdate - UTUpdateLast;

            lstStages = new arpStageList();
            lstResources = new arpResourceList();
            lstResourcesLastStage = new arpResourceList();

            arpStage stgTemp;
            int decoupleAt;
            //for each type of resource
            foreach (Part p in active.Parts)
	        {
                //Find the decoupler that is related to this stage
                decoupleAt = CalcDecoupleStage(p);

                foreach (PartResource pr in p.Resources)
	            {
                    //Build the stages based on decouplers
                    stgTemp = lstStages.FirstOrDefault(x => x.Number == decoupleAt);
                    if (stgTemp == null)
                    {
                        stgTemp = new arpStage() { Number = decoupleAt };
                        lstStages.Add(stgTemp);
                    }

                    //Add the resources to stages
                    AddResourceToList(ref stgTemp.ResourceList, pr);

                    //Now to the list of resources
                    AddResourceToList(ref lstResources, pr);
	            }

	        }

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

            ////This is the stuff to update rates
            //if (UTUpdate!=0)
            //{
            //    CalculateRates(lstLastResources, lstLastResources);

            //    CalculateRates(lstResourcesLastStage, lstLastResourcesLastStage);
            //}
            //UTUpdateLast = UTUpdate;


            //if (tmeOutput.AddSeconds(10) < DateTime.Now)
            //{
            //    DebugLogFormatted("{0}", Staging.lastStage, Staging.CurrentStage);

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
            //    //DebugLogFormatted("CurrentStage:{0}", Staging.CurrentStage);
            //    tmeOutput = DateTime.Now;
            //}
    
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


        private int DecoupledInStage(Part part , int stage = -1)
        {

            if (IsDecoupler(part))
            {
                if (part.inverseStage > stage)
                {
                    stage = part.inverseStage;
                }
            }

            if (part.parent != null)
            {
                stage = DecoupledInStage(part.parent, stage);
            }

            return stage;
        }
        private bool IsDecoupler(Part part)
        {
            return part is Decoupler || part is RadialDecoupler || part.Modules.OfType<ModuleDecouple>().Count() > 0 || part.Modules.OfType<ModuleAnchoredDecoupler>().Count() > 0;
        }

        private int CalcDecoupleStage(Part pTest)
        {
            int stageOut = 0;

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
