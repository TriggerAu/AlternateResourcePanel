using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

using KSPAlternateResourcePanel;

namespace KSPAlternateResourcePanel
{
    public partial class KSPAlternateResourcePanel 
    {
        arpPartWindowList lstPartWindows = new arpPartWindowList();
        Dictionary<int, PartResourceVisible> SelectedResources = new Dictionary<int, PartResourceVisible>();

        internal static Vector3 vectVesselCOMScreen;

        void TogglePartResourceVisible(int ResourceID,Boolean LastStage=false){
            //Are we editing this resource?
            if (SelectedResources.ContainsKey(ResourceID))
            {
                if (!LastStage)
                    SelectedResources[ResourceID].AllVisible = !SelectedResources[ResourceID].AllVisible;
                else
                    SelectedResources[ResourceID].LastStageVisible = !SelectedResources[ResourceID].LastStageVisible;
            }
            else
            {
                //Or adding a new one
                if (!LastStage)
                    SelectedResources.Add(ResourceID, new PartResourceVisible() { AllVisible = true });
                else
                     SelectedResources.Add(ResourceID, new PartResourceVisible() { LastStageVisible = true });
            }

            //If they are both false then remove the resource from the list
            if (!(SelectedResources[ResourceID].AllVisible || SelectedResources[ResourceID].LastStageVisible))
                SelectedResources.Remove(ResourceID);
         }

        void AddResourceToPartWindow(Part p, PartResource pr)
        {
            //Are we adding all resources or just the last stage
            if (SelectedResources[pr.info.id].AllVisible || (SelectedResources[pr.info.id].LastStageVisible && (p.DecoupledAt() == lstStages.LastStage)))
            {
                //find an existing window
                arpPartWindow pwTemp = lstPartWindows.FirstOrDefault(x => x.Part.GetInstanceID() == p.GetInstanceID());
                if (pwTemp == null)
                {
                    //Create a new one as there is none in the list
                    pwTemp = new arpPartWindow() { Part = p };
                    lstPartWindows.Add(pwTemp);
                }

                //Adjust this resourceID
                AddResourceToList(ref pwTemp.ResourceList, pr);
            }
        }

        void RemoveResourceFromPartWindow(Part p,PartResource pr)
        {
            arpPartWindow pwTemp = lstPartWindows.FirstOrDefault(x => x.Part.GetInstanceID() == p.GetInstanceID());
            if(pwTemp!=null && pwTemp.ResourceList.ContainsKey(pr.info.id))
            {
                pwTemp.ResourceList.Remove(pr.info.id);
            }
            if (pwTemp.ResourceList.Count < 1)
            {
                pwTemp.DestroyLineRenderer();
                lstPartWindows.Remove(pwTemp);
            }
        }

        void DrawPartWindows()
        {
            //leftside right side layout.....

            foreach (arpPartWindow pwTemp in lstPartWindows)
            {
               
                
                //if (pwTemp.PartScreenPos.x>Screen.width/2)
                //    pwTemp.Window.x = (float)pwTemp.PartScreenPos.x + 200;
                //else
                //    pwTemp.Window.x = (float)pwTemp.PartScreenPos.x - 200;
                //FlightGlobals.ActiveVessel.CoM.

                Vector3 partsub = new Vector3(0, 0, -2);
                pwTemp.linePart2Window.SetPosition(0, Camera.main.ScreenToWorldPoint(new Vector3((float)Math.Floor(pwTemp.Window.x+(pwTemp.Window.width/2)),(float)Math.Floor(Screen.height-pwTemp.Window.y-(pwTemp.Window.height/2)),(float)Math.Floor(pwTemp.PartScreenPos.z-(float)intTest4/10))));
                pwTemp.linePart2Window.SetPosition(1, Camera.main.ScreenToWorldPoint(pwTemp.PartScreenPos + partsub));

                float Scale = (float)intTest3 / 1000 * Camera.main.transform.position.magnitude;

                pwTemp.linePart2Window.SetWidth((float)intTest2 / 1000 * Scale, (float)intTest / 1000 * Scale);
                pwTemp.linePart2Window.enabled = true;

                //pwTemp.Window.ClampToScreen();
                GUILayout.Window(pwTemp.WindowID, pwTemp.Window, pwTemp.FillWindow, "", stylePartWindowPanel);

            }
        }
    }

    internal class PartResourceVisible
    {
        internal Boolean AllVisible;
        internal Boolean LastStageVisible;
    }

    internal class arpPartWindowList: List<arpPartWindow>
    {
        /// <summary>
        /// Reset values to 0;
        /// </summary>
        internal void ClearValues()
        {
            foreach (arpPartWindow pwTemp in this)
            {
                foreach (int key in pwTemp.ResourceList.Keys)
            	{
                    pwTemp.ResourceList[key].Amount=0;
                    pwTemp.ResourceList[key].MaxAmount=0;
                    pwTemp.ResourceList[key].Rate=0;
	            }
            }
        }

        //remove windows for parts that are no longer attached
        internal void ClearWindows()
        {
            arpPartWindowList lstremove = new arpPartWindowList();
            foreach (arpPartWindow pwTemp in this)
            {
                if (!FlightGlobals.ActiveVessel.Parts.Any(x=>x.GetInstanceID()==pwTemp.Part.GetInstanceID()))
                    lstremove.Add(pwTemp);
            }
            foreach (arpPartWindow pwTemp in lstremove)
            {
                KSPAlternateResourcePanel.DebugLogFormatted("Removing Window");
                pwTemp.DestroyLineRenderer();
                this.Remove(pwTemp);
                
            }
        }
    }

    internal class arpPartWindow
    {
        int LineHeight=20;
        internal float SideThreshold = 10;

        //Link to the part
        internal Part Part 
            {get;set;}
        //where the part is on the screen
        internal Vector3d PartScreenPos
        { 
            get {
                return Camera.main.WorldToScreenPoint(this.Part.transform.position);
            }
        }

        //What resources are we displaying for this part
        internal Dictionary<int,arpResource> ResourceList = new Dictionary<int,arpResource>();
        
        //Where are we drawing it...
        private Rect _Window;
        internal Rect Window
        {
            get {
                _Window.width = 169;
                _Window.height = ((this.ResourceList.Count + 1) * this.LineHeight) + 1;
                _Window.y =  Screen.height - (float)this.PartScreenPos.y - (_Window.height / 2);
                
                //_Window.x =  //where to position the resource
                if (this.LeftSide && (this.PartScreenPos.x > (KSPAlternateResourcePanel.vectVesselCOMScreen.x + SideThreshold)))
                    this.LeftSide = false;
                else if (!this.LeftSide && (this.PartScreenPos.x < (KSPAlternateResourcePanel.vectVesselCOMScreen.x - SideThreshold)))
                    this.LeftSide = true;

                float PartWindowOffset = 200;
                if (this.LeftSide)
                    PartWindowOffset *= -1;
                _Window.x = (float)this.PartScreenPos.x + PartWindowOffset - (_Window.width/2);
                
                return _Window; 
            }
            //set { _Window = value; }
        }
        internal int WindowID;

        internal Boolean LeftSide;

        GameObject lineObj;
        internal LineRenderer linePart2Window;
        internal Color LineColor;

        public arpPartWindow()
        {
            _Window = new Rect();
            WindowID= KSPAlternateResourcePanel.rnd.Next(1000, 2000000);
            LineColor = Color.white;
            LeftSide = true;

            lineObj = new GameObject(string.Format("LineObj-{0}", WindowID));
            linePart2Window = lineObj.AddComponent<LineRenderer>(); 
            linePart2Window.material = new Material(Shader.Find("KSP/Emissive/Diffuse"));
            linePart2Window.renderer.castShadows = false;
            linePart2Window.renderer.receiveShadows = false;
            linePart2Window.material.color = LineColor;
            linePart2Window.material.SetColor("_EmissiveColor", LineColor);

            linePart2Window.SetVertexCount(2);
        }

        internal void DestroyLineRenderer()
        {
            KSPAlternateResourcePanel.DebugLogFormatted("Killing line Renderer");
            linePart2Window.SetVertexCount(0);
            linePart2Window.enabled = false;
            GameObject.Destroy(lineObj);
            GameObject.Destroy(linePart2Window);
            linePart2Window = null;
        }

        internal void FillWindow(int WindowID)
        {
            Rect rectBar;
            float fltBarRemainRatio;

            GUILayout.BeginVertical();
            GUILayout.Label(this.Part.partInfo.title, KSPAlternateResourcePanel.stylePartWindowHead ,GUILayout.Height(18));
            int i = 0;
            foreach (int key in this.ResourceList.Keys)
            {
                GUILayout.Space(2); 
                if (i > 0) GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                //add title
                KSPAlternateResourcePanel.DrawResourceIcon(this.ResourceList[key].Resource.name);
                GUILayout.Space(2);

                //set ration for remaining resource value
                fltBarRemainRatio = (float)this.ResourceList[key].Amount / (float)this.ResourceList[key].MaxAmount;
                KSPAlternateResourcePanel.DrawBar(i, KSPAlternateResourcePanel.styleBarGreen_Back, out rectBar, 120);
                if ((rectBar.width * fltBarRemainRatio) > 1)
                    KSPAlternateResourcePanel.DrawBarScaled(rectBar, i, KSPAlternateResourcePanel.styleBarGreen, KSPAlternateResourcePanel.styleBarGreen_Thin, fltBarRemainRatio);

                //add amounts
                KSPAlternateResourcePanel.DrawUsage(rectBar, i, this.ResourceList[key].Amount, this.ResourceList[key].MaxAmount,true);
                GUILayout.EndHorizontal();
                i++;
            }
            GUILayout.EndVertical();
        }

    }
}
