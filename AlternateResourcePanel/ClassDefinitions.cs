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
                this.Remove(ResourceID);
        }
    }


    /// <summary>
    /// Details about a specific resource. 
    /// All Gets from this class should be straight from memory and all input work via Set functions that are called in the Repeating function 
    /// </summary>
    internal class ARPResource
    {
        internal ARPResource(PartResourceDefinition ResourceDefinition)
        {
            this.ResourceDef = ResourceDefinition;
        }

        internal PartResourceDefinition ResourceDef;

        internal Double Amount {get; set;}
        internal String AmountFormatted { get { return DisplayValue(this.Amount); } }
        internal Double MaxAmount{get; set;}
        internal String MaxAmountFormatted { get { return DisplayValue(this.MaxAmount); } }

        internal void ResetAmounts()
        {
            this.Amount = 0;
            this.MaxAmount = 0;
        }

        internal enum MonitorType
        {
            Alert,
            Warn,
            None
        }

        internal delegate void MonitorChanged(ARPResource sender, MonitorType alarmType, Boolean TurnedOn,Boolean Acknowledged);
        public event MonitorChanged OnMonitorStateChange;
        private Boolean _MonitorWarning = true;
        internal Boolean MonitorWarning
        {
            get { return _MonitorWarning; }
            set
            {
                Boolean oldValue = _MonitorWarning;
                _MonitorWarning = value;
                if (oldValue != value) 
                {
                    if (value) AlarmAcknowledged = false;
                    if (OnMonitorStateChange != null)
                        OnMonitorStateChange(this, MonitorType.Warn, value,_AlarmAcknowledged);
                }
            }
        }
        private Boolean _MonitorAlert = true;
        internal Boolean MonitorAlert
        {
            get { return _MonitorAlert; }
            set
            {
                Boolean oldValue = _MonitorAlert;
                _MonitorAlert = value;
                if (oldValue != value)
                {
                    if (value) AlarmAcknowledged = false;
                    if (OnMonitorStateChange!=null)
                        OnMonitorStateChange(this, MonitorType.Alert, value,_AlarmAcknowledged);
                }
            }
        }

        internal MonitorType MonitorWorstHealth
        {
            get
            {
                if (_MonitorAlert)
                    return MonitorType.Alert;
                else if (_MonitorWarning)
                    return MonitorType.Warn;
                else
                    return MonitorType.None;
            }
        }

        internal delegate void AlarmAcknowledgedEvent(ARPResource sender);
        public event AlarmAcknowledgedEvent OnAlarmAcknowledged;

        private Boolean _AlarmAcknowledged = true;
        internal Boolean AlarmAcknowledged { get {
            return _AlarmAcknowledged;
        }
        set
            {
                _AlarmAcknowledged = value;
                if (value)
                {
                    if (OnAlarmAcknowledged!=null)
                        OnAlarmAcknowledged(this);
                }
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

        internal static String DisplayValue(Double Amount)
        {
            String strFormat = "{0:0}";
            if (Amount < 100)
                strFormat = "{0:0.00}";
            return String.Format(strFormat, Amount);
        }
        internal static String DisplayRateValue(Double Amount)
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

    internal class ARPResourceList: Dictionary<Int32,ARPResource>
    {
        //Should we be storing the UT values in here somewhere instead of referencing back to a static object???
        private Double RepeatingWorkerUTPeriod;

        internal ARPResourceList(ResourceUpdate UpdateType)
        {
            this.UpdateType = UpdateType;
        }

        private ResourceUpdate _UpdateType;

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

        internal ARPResource AddResource(PartResource ResourceToAdd)
        {
            if (!UpdatingList) throw new SystemException("List is additive and Updating Flag has not been set");
            Int32 ResourceID = ResourceToAdd.info.id;
            if (!this.ContainsKey(ResourceID))
            {
                this.Add(ResourceID, new ARPResource(ResourceToAdd.info));
                this[ResourceID].OnMonitorStateChange += ARPResourceList_OnMonitorStateChange;
                this[ResourceID].OnAlarmAcknowledged += ARPResourceList_OnAlarmAcknowledged;
            }
            return this[ResourceID];
        }

        public event ARPResource.MonitorChanged OnMonitorStateChange;
        public event ARPResource.AlarmAcknowledgedEvent OnAlarmAcknowledged;

        void ARPResourceList_OnMonitorStateChange(ARPResource sender, ARPResource.MonitorType alarmType, bool TurnedOn,Boolean Acknowledged)
        {
            if (OnMonitorStateChange != null)
                OnMonitorStateChange(sender, alarmType, TurnedOn,Acknowledged);
        }
        void ARPResourceList_OnAlarmAcknowledged(ARPResource sender)
        {
            if (OnAlarmAcknowledged != null)
                OnAlarmAcknowledged(sender);
        }

        internal Boolean UnacknowledgedAlarms(Dictionary<Int32,ResourceSettings> ResourceList)
        {
            Boolean blnReturn = false;
            foreach (ARPResource r in this.Values)
            {
                if (!r.AlarmAcknowledged && ResourceList[r.ResourceDef.id].AlarmEnabled)
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
            ARPResource r = AddResource(Resource);

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

            return r;
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
}