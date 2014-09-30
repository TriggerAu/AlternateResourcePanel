using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

using KSPARPAPITester_ARPWrapper;

namespace KSPARPAPITester
{
    [KSPAddon(KSPAddon.Startup.Flight, false),
    WindowInitials(Visible=true,Caption="KSP ARP API Tester",DragEnabled=true)]
    public class KSPARPAPITester : MonoBehaviourWindow
    {
        internal override void Start()
        {
            LogFormatted("Start");
            ARPWrapper.InitKSPARPWrapper();

            ARPWrapper.KSPARP.onMonitorStateChanged += KSPARP_onMonitorStateChanged;
            ARPWrapper.KSPARP.onAlarmStateChanged += KSPARP_onAlarmStateChanged;
        }

        void KSPARP_onMonitorStateChanged(ARPWrapper.KSPARPAPI.MonitorStateChangedEventArgs e)
        {
            LogFormatted("{0}:{1}->{2} ({3})", e.resource.ResourceDef.name, e.oldValue, e.newValue, e.AlarmState);
        }

        void KSPARP_onAlarmStateChanged(ARPWrapper.KSPARPAPI.AlarmStateChangedEventArgs e)
        {
            LogFormatted("{0}:{1}", e.resource.ResourceDef.name,e.newValue);
        }

        internal override void Awake()
        {
            WindowRect = new Rect(600, 100, 300, 200);
        }

        internal override void OnDestroy()
        {

        }

        internal override void DrawWindow(int id)
        {
            GUILayout.Label("Assembly: " + ARPWrapper.AssemblyExists.ToString());
            GUILayout.Label("Instance: " + ARPWrapper.InstanceExists.ToString());
            GUILayout.Label("APIReady: " + ARPWrapper.APIReady.ToString());
            
            if (ARPWrapper.APIReady)
            {
                foreach (ARPWrapper.ARPResource r in ARPWrapper.KSPARP.VesselResources.Values)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(String.Format("{0}:  {1}/{2}",r.ResourceDef.name,r.AmountFormatted,r.MaxAmountFormatted),GUILayout.Width(200));
                    if (r.AlarmState == ARPWrapper.ARPResource.AlarmStateEnum.Unacknowledged)
                    {
                        if (GUILayout.Button("Acknowledge"))
                        {
                            ARPWrapper.KSPARP.AcknowledgeAlarm(r.ResourceDef.id);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
