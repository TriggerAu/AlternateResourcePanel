using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPAlternateResourcePanel
{
    //internal class ARPPart : Part
    //{
    //    internal ARPPart(Part p)
    //    {
    //        this.ID = this.GetInstanceID();
    //        this.SetDecoupledAt();
    //        //this.Resources = new ARPResourceList();
    //    }

    //    internal Int32 ID { get; private set; }
    //    internal Part PartRef;

    //    internal Int32 DecoupledAt { get; private set; }
    //    internal void SetDecoupledAt()
    //    {
    //        this.DecoupledAt = this.PartRef.DecoupledAt();
    //    }

    //    internal PartResourceList Resources
    //    {
    //        get { return this.PartRef.Resources; }
    //    }

    //    //internal ARPResourceList Resources;


    //    //internal void UpdateAll()
    //    //{
    //    //    //this.SetDecoupledAt();
    //    //    foreach (ARPResource ri in this.Resources.Values)
    //    //    {

    //    //    }
    //    //}
    //}

    internal class ARPPartDef
    {
        internal ARPPartDef(Part p) { part = p; DecoupledAt = p.DecoupledAt(); }
        internal Part part;
        internal Int32 DecoupledAt;
    }
    internal class ARPPartDefList : List<ARPPartDef> {

        internal Boolean LastStageIsResourceOnlyAndEmpty()
        {
            Int32 LastStage = GetLastStage();

            //check each part in the stage
            foreach (ARPPartDef p in this.Where(pa=>pa.DecoupledAt==LastStage)) {

                //if theres an engine then ignore this case
                if (p.part.Modules.OfType<ModuleEngines>().Any() || p.part.Modules.OfType<ModuleEnginesFX>().Any()){
                    return false;
                }
                //if theres any resource then ignore this case
                foreach (PartResource r in p.part.Resources)
            	{
                    if (r.amount>0)
                        return false;
	            }
            }
            return true;
            //return !HasEngine && !HasFuel;
        }


        internal Int32 GetLastStage()
        {
            if (this.Count > 0)
                return this.Max(x => x.DecoupledAt);
            else return -1;
        }
    }

    internal class ARPPartList: List<Part>
    {
        //internal Int32 LastStage { get { return this.Max(x => x.DecoupledAt()); } }

        //internal List<Part> PartList
        //{
        //    get
        //    {
        //        return this.Select<ARPPart,Part>(x => x.PartRef).ToList<Part>();
        //    }
        //}
    }

    internal class PartResourceVisible
    {
        internal Boolean AllVisible;
        internal Boolean LastStageVisible;
    }

    internal class PartResourceVisibleList:Dictionary<int, PartResourceVisible>
    {
        internal void TogglePartResourceVisible(int ResourceID, Boolean LastStage = false)
        {
            //Are we editing this resource?
            if (this.ContainsKey(ResourceID))
            {
                if (!LastStage)
                    this[ResourceID].AllVisible = !this[ResourceID].AllVisible;
                else
                    this[ResourceID].LastStageVisible = !this[ResourceID].LastStageVisible;
            }
            else
            {
                //Or adding a new one
                if (!LastStage)
                    this.Add(ResourceID, new PartResourceVisible() { AllVisible = true });
                else
                    this.Add(ResourceID, new PartResourceVisible() { LastStageVisible = true });
            }

            //If they are both false then remove the resource from the list
            if (!(this[ResourceID].AllVisible || this[ResourceID].LastStageVisible))
            {
                this.Remove(ResourceID);
                if (ResourceRemoved != null)
                    ResourceRemoved(ResourceID);
            }
        }

        internal delegate void ResourceRemovedHandler(Int32 ResourceID);
        internal event ResourceRemovedHandler ResourceRemoved;
    }


    /// <summary>
    /// Details about a specific resource. 
    /// All Gets from this class should be straight from memory and all input work via Set functions that are called in the Repeating function 
    /// </summary>
    public class ARPResource
    {
        internal ARPResource(PartResourceDefinition ResourceDefinition, ResourceSettings ResourceConfig)
        {
            this.ResourceDef = ResourceDefinition;
            this.ResourceConfig = ResourceConfig;
        }

        public PartResourceDefinition ResourceDef { get; private set; }
        public ResourceSettings ResourceConfig { get; private set; }

        private Double _Amount;
        internal Double Amount
        {
            get { return _Amount; }
            set
            {
                Double oldValue = _Amount;
                _Amount = value;
                //if (oldValue != value)
                //{
                //    if (value <= 0)
                //    {
                //        IsEmpty = true;
                //        EmptyAt = DateTime.Now;
                //        //IsFull = false;
                //    }
                //    //else if (value >= MaxAmount) {
                //    //    MonoBehaviourExtended.LogFormatted("Full:{0}-{1}", value, MaxAmount);
                //    //    IsFull = true;
                //    //    FullAt = DateTime.Now;
                //    //    IsEmpty = false;
                //    //}
                //    else {
                //        IsEmpty = false;
                //        //IsFull = false;
                //    }
                //}
            }
        }

        internal void CalcEmptyandFull()
        {
            IsEmpty = (Amount <= 0);
            IsFull = (Amount >= MaxAmount);

            //EmptyAt=DateTime.FromFileTime(0);
            //EmptyAt.ToFileTime()=0;

            if (IsEmpty) {
                if (EmptyAt == new DateTime())
                    EmptyAt = DateTime.Now;
            } else {
                EmptyAt = new DateTime();
            }

            if (IsFull) {
                if (FullAt == new DateTime())
                    FullAt = DateTime.Now;
            } else {
                FullAt = new DateTime();
            }
        }


        internal Boolean IsEmpty = false;
        internal Boolean IsFull = false;

        //internal Double Amount {get; set;}
        internal Double MaxAmount{get; set;}
        public Double AmountValue { get { return Amount; } }
        public String AmountFormatted { get { return DisplayValue(this.Amount); } }
        public Double MaxAmountValue { get { return MaxAmount; } }
        public String MaxAmountFormatted { get { return DisplayValue(this.MaxAmount); } }

        internal DateTime EmptyAt { get; set; }
        internal DateTime FullAt = new DateTime();// { get; set; }

        internal void ResetAmounts()
        {
            this.Amount = 0;
            this.MaxAmount = 0;
        }

        public enum MonitorStateEnum
        {
            None,
            Warn,
            Alert,
        }

        public enum AlarmStateEnum
        {
            None,
            Unacknowledged,
            Acknowledged,
        }

        private MonitorStateEnum _MonitorState;
        public MonitorStateEnum MonitorState
        {
            get { return _MonitorState; }
            private set
            {
                MonitorStateEnum oldValue = _MonitorState;
                _MonitorState=value;
                if (oldValue!=value)
                {
                    if (value>oldValue)
                    {
                        //if severity increased then unacknowledge the state
                        if (ResourceConfig.AlarmEnabled && KSPAlternateResourcePanel.settings.AlarmsEnabled)
                            AlarmState = AlarmStateEnum.Unacknowledged;
                    }
                    else if (value== MonitorStateEnum.None)
                    {
                        //Shortcut the alarmstate if the monitors all turned off
                        _AlarmState = AlarmStateEnum.None;
                    }
                    //MonoBehaviourExtended.LogFormatted_DebugOnly("ResMON-{0}:{1}->{2} ({3})", this.ResourceDef.name, oldValue, value, this.AlarmState);
                    if (OnMonitorStateChanged != null)
                        OnMonitorStateChanged(this, oldValue, value,AlarmState);
                }
            }
        }

        private AlarmStateEnum _AlarmState;
        public AlarmStateEnum AlarmState
        {
            get { return _AlarmState; }
            private set
            {
                AlarmStateEnum oldValue = _AlarmState;
                _AlarmState = value;
                if (oldValue!=value)
                {
                    //MonoBehaviourExtended.LogFormatted_DebugOnly("ResALARM-{0}:{1}->{2} ({3})", this.ResourceDef.name, oldValue, value, this.MonitorState);
                    if (value != AlarmStateEnum.Unacknowledged && IsEmpty)
                        EmptyAt = DateTime.Now;

                    if (value != AlarmStateEnum.Unacknowledged && IsFull)
                        FullAt = DateTime.Now;

                    if (OnAlarmStateChanged != null)
                        OnAlarmStateChanged(this, oldValue, value,MonitorState);
                }
            }
        }

        internal void SetAlarmAcknowledged()
        {
            if (this.AlarmState == AlarmStateEnum.Unacknowledged)
                this.AlarmState = AlarmStateEnum.Acknowledged;
        }


        internal delegate void MonitorStateChangedHandler(ARPResource sender, MonitorStateEnum oldValue,MonitorStateEnum newValue,AlarmStateEnum AlarmState);
        internal event MonitorStateChangedHandler OnMonitorStateChanged;

        internal delegate void AlarmStateChangedHandler(ARPResource sender, AlarmStateEnum oldValue, AlarmStateEnum newValue,MonitorStateEnum MonitorState);
        internal event AlarmStateChangedHandler OnAlarmStateChanged;

        internal void SetMonitors()
        {
            Double rPercent = (this.Amount / this.MaxAmount) * 100;

            if ((ResourceConfig.MonitorDirection == ResourceSettings.MonitorDirections.Low && rPercent <= ResourceConfig.MonitorAlertLevel) ||
                (ResourceConfig.MonitorDirection == ResourceSettings.MonitorDirections.High && rPercent >= ResourceConfig.MonitorAlertLevel))
            {
                this.MonitorState = MonitorStateEnum.Alert;
            }
            else if ((ResourceConfig.MonitorDirection == ResourceSettings.MonitorDirections.Low && rPercent <= ResourceConfig.MonitorWarningLevel) ||
                (ResourceConfig.MonitorDirection == ResourceSettings.MonitorDirections.High && rPercent >= ResourceConfig.MonitorWarningLevel))
            {
                this.MonitorState = MonitorStateEnum.Warn;
            }
            else
            {
                this.MonitorState = MonitorStateEnum.None;
            }
        }

        internal Double AmountLast { get; private set; }
        internal String AmountLastFormatted { get { return DisplayValue(this.Amount); } }
        internal void SetLastAmount()
        {
            this.AmountLast = this.Amount;
        }

        internal Double Rate {get; private set;}
        internal void SetRate(Double UTPeriod)
        {
            if (UTPeriod>0)
                this.Rate = ((this.AmountLast - this.Amount) / UTPeriod);
            //    //Remmed out code for sampling idea - doesn't work for resourcelist when it is additive
            //    //r.RateSamples.Enqueue(new RateRecord(KSPAlternateResourcePanel.UTUpdate, Resource.amount));
            //    //r.SetRate2();
        }

        internal String RateFormatted
        {
            get
            {
                return DisplayRateValue(this.Rate);
            }
        }
        internal String RateFormattedAbs
        {
            get
            {
                return DisplayRateValue(Math.Abs(this.Rate));
            }
        }

        internal String DisplayValue(Double AmountToDisplay)
        {
            Double Amount = AmountToDisplay;
            if (ResourceConfig.DisplayValueAs == ResourceSettings.DisplayUnitsEnum.Tonnes)
                Amount = AmountToDisplay * this.ResourceDef.density;
            else if (ResourceConfig.DisplayValueAs == ResourceSettings.DisplayUnitsEnum.Kilograms)
                Amount = AmountToDisplay * this.ResourceDef.density * 1000;
            
            //Format string - Default
            String strFormat = "{0:0}";
            if (ResourceConfig.DisplayValueAs == ResourceSettings.DisplayUnitsEnum.Tonnes && Math.Abs(Amount) < 1)
                strFormat = "{0:0.000}";
            else if (Math.Abs(Amount) < 100)
            {
                strFormat = "{0:0.00}";
            }
            else if (ResourceConfig.DisplayValueAs == ResourceSettings.DisplayUnitsEnum.Tonnes && Math.Abs(Amount) < 1000)
            {
                strFormat = "{0:0.0}";
            }

            //handle the miniature negative value that gets rounded to 0 by string format
            if ((String.Format(strFormat, Amount) == "0.00" || String.Format(strFormat, Amount) == "0.000") && Amount < 0)
                strFormat = "-" + strFormat;

            //Handle large values
            if (Amount<10000)
                return String.Format(strFormat, Amount);
            else if (Amount < 1000000)
                return String.Format("{0:0.0}K", Amount / 1000);
            else 
                return String.Format("{0:0.0}M", Amount / 1000000);

        }
        internal String DisplayRateValue(Double Amount, Boolean HighPrecisionBelowOne=false)
        {
            if (Amount == 0) return "-";
            return DisplayValue(Amount);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////
        // This code is for a sampling idea - not implemented yet
        // Should reduce major spikes, but trade off is slower response in values
        ///////////////////////////////////////////////////////////////////////////////////////////////
        //internal Double Rate2 { get; private set; }
        //internal Double SetRate2()
        //{
        //    if (RateSamples.Count > 0)
        //    {
        //        RateRecord newest = RateSamples.OrderBy(x => x.UT).Last();
        //        RateRecord oldest = RateSamples.OrderBy(x => x.UT).First();
        //        Double AmountChanged = (newest.Amount - oldest.Amount) / (newest.UT - oldest.UT);
        //        this.Rate2 = AmountChanged;
        //        return AmountChanged;
        //    }
        //    else
        //    {
        //        this.Rate2 = 0;
        //        return 0;
        //    }
        //}

        //internal String RateFormatted2
        //{
        //    get
        //    {
        //        return DisplayValue(this.Rate2);
        //    }
        //}
        //internal LimitedQueue<RateRecord> RateSamples;

        //private Int32 _RateSamplesLimit = 2;
        //public Int32 RateSamplesLimit
        //{
        //    get { return _RateSamplesLimit; }
        //    set
        //    {
        //        _RateSamplesLimit = value;
        //        //set the limited queue length when tis changes
        //        RateSamples.Limit = _RateSamplesLimit;
        //    }
        //}
        ///////////////////////////////////////////////////////////////////////////////////////////////
    }


    ///// <summary>
    ///// Simple Class that stores the amount of a Resource at a recorded UT
    ///// </summary>
    //internal class RateRecord
    //{
    //    internal RateRecord(Double UT,Double Amount)
    //    {
    //        this.UT=UT;
    //        this.Amount=Amount;
    //    }

    //    internal Double UT;
    //    internal Double Amount;
    //}

    public class ARPResourceList: Dictionary<Int32,ARPResource>
    {
        //Should we be storing the UT values in here somewhere instead of referencing back to a static object???
        private Double RepeatingWorkerUTPeriod;

        internal ARPResourceList(ResourceUpdate UpdateType, Dictionary<Int32,ResourceSettings> ResourceConfigs)
        {
            this.UpdateType = UpdateType;
            this.ResourceConfigs = ResourceConfigs;
        }

        private ResourceUpdate _UpdateType;
        private Dictionary<Int32, ResourceSettings> ResourceConfigs;

        /// <summary>
        /// This boolean flag controls whether the ResourceList can be updated or not
        /// This is important for additive lists where we dont want to do the Rate Calcs till all the resources have been added
        /// </summary>
        internal ResourceUpdate UpdateType{
            get {return _UpdateType;}   
            set {
                UpdatingList = (value== ResourceUpdate.SetValues);
                _UpdateType = value;
            }
        }

        internal Boolean UpdatingList { get; private set; }
        internal void StartUpdatingList(Boolean CalcRates, Double RepeatingWorkerUTPeriod)
        { 
            UpdatingList = true;
            this.RepeatingWorkerUTPeriod = RepeatingWorkerUTPeriod;
            foreach (ARPResource r in this.Values)
            {
                if (CalcRates)
                {
                    r.SetLastAmount();
                }
                r.ResetAmounts();
            }
        }
        internal void EndUpdatingList(Boolean CalcRates)
        {
            UpdatingList = false;
            
            this.CalcEmptyandFulls();

            if (CalcRates)
                this.CalcListRates();
        }

        internal void CleanResources(List<Int32> ExistingIDs)
        {
            List<Int32> IDsToRemove = this.Keys.Except(ExistingIDs).ToList<Int32>();
            foreach (Int32 rID in IDsToRemove)
            {
                MonoBehaviourExtended.LogFormatted_DebugOnly("Removing Resource-{0}",rID);
                this.Remove(rID);
            }
        }

        internal ARPResource AddResource(PartResource ResourceToAdd,out Boolean NewResource)
        {
            if (!UpdatingList) throw new SystemException("List is additive and Updating Flag has not been set");
            Int32 ResourceID = ResourceToAdd.info.id;
            if (!this.ContainsKey(ResourceID))
            {
                this.Add(ResourceID, new ARPResource(ResourceToAdd.info, ResourceConfigs[ResourceID]));

                //set the initial alarm states before enabling the events
                NewResource = true;

                this[ResourceID].OnMonitorStateChanged += ARPResourceList_OnMonitorStateChanged;
                this[ResourceID].OnAlarmStateChanged+=ARPResourceList_OnAlarmStateChanged;
            }
            else { NewResource = false; }
            return this[ResourceID];
        }

        internal event ARPResource.MonitorStateChangedHandler OnMonitorStateChanged;
        internal event ARPResource.AlarmStateChangedHandler OnAlarmStateChanged;

        void ARPResourceList_OnMonitorStateChanged(ARPResource sender, ARPResource.MonitorStateEnum oldValue, ARPResource.MonitorStateEnum newValue,ARPResource.AlarmStateEnum AlarmState)
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("LISTMon-{0}:{1}->{2} ({3})", sender.ResourceDef.name, oldValue, newValue, sender.AlarmState);
            if (OnMonitorStateChanged != null)
                OnMonitorStateChanged(sender, oldValue, newValue, AlarmState);
        }

        void ARPResourceList_OnAlarmStateChanged(ARPResource sender, ARPResource.AlarmStateEnum oldValue, ARPResource.AlarmStateEnum newValue,ARPResource.MonitorStateEnum MonitorState)
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("LISTAck-{0}:{1}->{2} ({3})", sender.ResourceDef.name, oldValue, newValue, sender.MonitorState);
            if (OnAlarmStateChanged != null) 
                OnAlarmStateChanged(sender, oldValue, newValue,MonitorState);
        }

        internal Boolean UnacknowledgedAlarms()
        {
            Boolean blnReturn = false;
            foreach (ARPResource r in this.Values)
            {
                if (r.AlarmState== ARPResource.AlarmStateEnum.Unacknowledged && ResourceConfigs[r.ResourceDef.id].AlarmEnabled)
                {
                    blnReturn = true;
                    break;
                }
            }
            return blnReturn;
        }

        internal void SetUTPeriod(Double UTPeriod) { RepeatingWorkerUTPeriod = UTPeriod; }
        internal ARPResource UpdateResource(PartResource Resource,Boolean CalcRates=false)
        {
            if (!UpdatingList) throw new SystemException("List is additive and Updating Flag has not been set");
            //Get the Resource (or create it if needed)
            Boolean NewResource = false;
            ARPResource r = AddResource(Resource,out NewResource);

            //Are we adding or setting the amounts
            switch (UpdateType)
            {
                case ResourceUpdate.AddValues:
                    r.Amount += Resource.amount;
                    r.MaxAmount += Resource.maxAmount;
                    break;
                case ResourceUpdate.SetValues:
                    if (CalcRates)
                        r.SetLastAmount();
                    r.Amount = Resource.amount;
                    r.MaxAmount = Resource.maxAmount;
                    if (CalcRates)
                        r.SetRate(RepeatingWorkerUTPeriod);
                    break;
                default:
                    throw new SystemException("Invalid ResourceUpdate Type");
            }
            
            if (NewResource)
            {
                r.SetMonitors();
                r.SetAlarmAcknowledged();
            }

            return r;
        }

        internal void CalcEmptyandFulls()
        {
            foreach (ARPResource r in this.Values)
            {
                r.CalcEmptyandFull();
            }
        }


        internal void CalcListRates()
        {
            foreach (ARPResource r in this.Values)
            {
                r.SetRate(RepeatingWorkerUTPeriod);
            }
        }


        /// <summary>
        /// How to set the amounts in a resourceupdate
        /// </summary>
        internal enum ResourceUpdate
        {
            /// <summary>
            /// Add new values to existing amounts
            /// </summary>
            AddValues,
            /// <summary>
            /// Set the values to the provided ones
            /// </summary>
            SetValues
        }

    }

    internal class ARPTransfer
    {
        internal ARPTransfer()
        { }
        internal ARPTransfer(Part p,PartResourceDefinition RD,TransferStateEnum State)
        {
            this.part = p;
            this.resource = RD;
            this.transferState = State;
            this.Active = false;
        }

        internal Part part;
        internal PartResourceDefinition resource;
        internal TransferStateEnum transferState;

        internal Boolean Active;
        internal Single RatePerSec;

        internal Int32 partID { get { return part.GetInstanceID(); } }
        internal Int32 ResourceID { get { return resource.id; } }
    }

    internal enum TransferStateEnum
    {
        None,
        In,
        Out,
    }

    internal class ARPTransferList : List<ARPTransfer>
    {
        internal Single RatePerSecMax{get{ return this.Max(t => t.RatePerSec); }}

        internal void AddItem(Part p,PartResourceDefinition RD,TransferStateEnum State)
        {
            this.Add(new ARPTransfer(p, RD, State));
        }

        internal Boolean ItemExists(Int32 PartID, Int32 ResourceID)
        {
            return this.Any(x => (x.partID == PartID) && (x.ResourceID == ResourceID));
        }

        internal ARPTransfer GetItem(Int32 PartID,Int32 ResourceID)
        {
            return this.FirstOrDefault(x => (x.partID == PartID) && (x.ResourceID == ResourceID));
        }

        internal void SetStateNone(Int32 ResourceID)
        {
            foreach (ARPTransfer t in this.Where(x => x.ResourceID == ResourceID))
            {
                t.transferState = TransferStateEnum.None;
            }
        }
        internal void SetStateNone(Int32 ResourceID,TransferStateEnum State)
        {
            foreach (ARPTransfer t in this.Where(x => x.ResourceID == ResourceID && x.transferState==State))
            {
                t.transferState = TransferStateEnum.None;
            }
        }

        internal void RemoveItem(Int32 PartID, Int32 ResourceID)
        {
            ARPTransfer toRemove = GetItem(PartID, ResourceID);
            if (toRemove != null)
                this.Remove(toRemove);
        }

        internal void RemovePartItems(Int32 PartID)
        {
            List<ARPTransfer> toRemove = this.Where(x=>x.partID==PartID).ToList();
            foreach (ARPTransfer item in toRemove)
            {
                this.Remove(item);
            }
        }
        internal void RemoveResourceItems(Int32 ResourceID)
        {
            List<ARPTransfer> toRemove = this.Where(x => x.ResourceID == ResourceID).ToList();
            foreach (ARPTransfer item in toRemove)
            {
                this.Remove(item);
            }
        }


    }

}