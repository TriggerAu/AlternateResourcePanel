using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

namespace KSPAlternateResourcePanel
{

    //Classes for resources and stages        

    //internal class arpPart : Part
    //{
    //    internal int DecoupledAt { get; set; }

    //    internal Dictionary<int, Double> ResourceRates { get; set; }
    //}

    //internal class arpPartList:List<Part>
    //{
    //    internal int LastStage { get { return this.Max(x => x.DecoupledAt()); } }
    //}

    //internal arpPartList lstParts;
    //internal arpPartList lstPartsLast;

    /// <summary>
    /// A Resource type
    /// </summary>
    internal class arpResource
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

    internal class arpResourceList : List<arpResource>
    {
        //public double UTgrabbed; 

        //public arpResourceList()
        //{
        //    UTgrabbed = Planetarium.GetUniversalTime();
        //}
    }

    class arpStageList : List<arpStage>
    {
        internal int LastStage { get { return this.Max(x => x.Number); } }

        //public double lastGrab;

        //public arpStageList()
        //{
        //    lastGrab = Planetarium.GetUniversalTime();
        //}
    }

#region "Extensions"
    public static class PartExtensions
    {
        public static int DecoupledAt(this Part p)
        {
            return KSPAlternateResourcePanel.CalcDecoupleStage(p);
        }

    }

    public static class RectExtensions
    {
        public static void ClampToScreen(this Rect r)
        {
            r.x = Mathf.Clamp(r.x, -1, Screen.width - r.width + 1);
            r.y = Mathf.Clamp(r.y, -1, Screen.height - r.height + 1);
        }
    }
#endregion
}
