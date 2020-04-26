using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPAlternateResourcePanel
{
    class ARPPartWindow : MonoBehaviourWindow
    {
        //internal ARPPartWindow(Part Part)
        //{
        //    this.Part = Part;
        //    this.ResourceList = new ARPResourceList(ARPResourceList.ResourceUpdate.SetValues);
        //}

        //TODO: Look at using this
        //  http://answers.unity3d.com/questions/445444/add-component-in-one-line-with-parameters.html

        /// <summary>
        /// Have to take this out of the constructor as the object must be created using gameObject.addComponent<>
        /// </summary>
        /// <param name="Part">Part that this window is displaying info for</param>
        internal void Init(Part Part,KSPAlternateResourcePanel mbARP)
        {
            this.mbARP = mbARP;
            this.settings = KSPAlternateResourcePanel.settings;
            this.PartRef = Part;
            this.ResourceList = new ARPResourceList(ARPResourceList.ResourceUpdate.SetValues,settings.Resources);
            this.LeftSide = this.PartScreenPos.x < (mbARP.vectVesselCOMScreen.x - 3);
        }

        //Details about the part attached to this window
        internal Int32 PartID { get { return PartRef.GetInstanceID(); } }
        internal Part PartRef { get; private set; }

        //What resources are we displaying for this part
        internal ARPResourceList ResourceList;

        //Parent Objects
        internal KSPAlternateResourcePanel mbARP;
        internal Settings settings;

        /// <summary>
        /// Screen location of the Part
        /// </summary>
        internal Vector3d PartScreenPos { get {
            return FlightCamera.fetch.mainCamera.WorldToScreenPoint(this.PartRef.transform.position);
        }
        }

        //Where are we drawing it...
        internal Boolean LeftSide;
        internal static Single SideThreshold = 8;
        internal static Single WindowOffset = 200;
        internal static Single WindowSpaceOffset = 5;
        internal static Single WindowWidthForBars = 169;
        internal static Single WindowWidthForFlowControl = 16;

        internal static Int32 Icon2BarOffset = 36;

        //Line stuff
        Color colorLineCurrent = new Color(207, 207, 207);
        //GameObject lineObj;
        //internal LineRenderer linePart2Window;
        //internal Vector3 vectLinePartEnd;
        //internal Vector3 vectLineWindowEnd;
        //internal Color LineColor;

        //Colors for part and line highlighting
        Color colorHighlight = new Color(0, 0, .8f, .75f);
        Color colorLineHighlight = new Color(.1f, .1f, .8f, .6f);

        /// <summary>
        /// Set the window size and whether its left/right of the vessel
        /// </summary>
        private void UpdateWindowSizeAndVariables()
        {
            Rect NewPosition = new Rect(WindowRect);
            NewPosition.width = WindowWidthForBars + WindowWidthForFlowControl;
            NewPosition.height = ((this.ResourceList.Count + 1 + this.TransfersCount ) * mbARP.windowMain.intLineHeight) + 1;

            //where to position the window
            if (this.LeftSide && (this.PartScreenPos.x > (mbARP.vectVesselCOMScreen.x + SideThreshold)))
                this.LeftSide = false;
            else if (!this.LeftSide && (this.PartScreenPos.x < (mbARP.vectVesselCOMScreen.x - SideThreshold)))
                this.LeftSide = true;

            // STore it back
            WindowRect = NewPosition;
        }

        /// <summary>
        /// Where would the next window start
        /// </summary>
        internal Single ScreenNextWindowY
        {
            get
            {
                return WindowRect.y + WindowRect.height + WindowSpaceOffset;
            }
        }

        internal override void OnAwake()
        {
            if (!MapView.MapIsEnabled)
                Visible = true;
            //CreateLineRenderer();

            //Events to subscribe
            OnMouseEnter += ARPPartWindow_OnMouseEnter;
            OnMouseLeave += ARPPartWindow_OnMouseLeave;

            SkinsLibrary.OnSkinChanged += SkinsLibrary_SkinChanged;

            MapView.OnEnterMapView += OnEnterMapView;
            MapView.OnExitMapView += OnExitMapView;
        }

        internal override void OnDestroy()
        {
            mbARP.lstTransfers.RemovePartItems(this.PartID);

            //unsubscribe events
            OnMouseEnter -= ARPPartWindow_OnMouseEnter;
            OnMouseLeave -= ARPPartWindow_OnMouseLeave;

            SkinsLibrary.OnSkinChanged -= SkinsLibrary_SkinChanged;

            MapView.OnEnterMapView -= OnEnterMapView;
            MapView.OnExitMapView -= OnExitMapView;
        }

        //Adjust the window and line visibility based on MapView
        void OnEnterMapView()
        {
            this.Visible = false;
            //DestroyLineRenderer();
        }
        void OnExitMapView()
        {
            this.Visible = true;
            //CreateLineRenderer();
        }

        //Change the windowstyle to match the mods style
        void SkinsLibrary_SkinChanged()
        {
            if (SkinsLibrary.CurrentSkin.name=="Unity")
                WindowStyle = Styles.stylePartWindowPanelUnity;
            else
                WindowStyle = Styles.stylePartWindowPanel;

        }

        //Hoverover for the part
        void ARPPartWindow_OnMouseEnter()
        {
            //linePart2Window.material.SetColor("_EmissiveColor", colorLineHighlight);
            colorLineCurrent = colorLineHighlight;
            PartRef.SetHighlightColor(colorHighlight);
            PartRef.SetHighlight(true,false);
            
        }
        void ARPPartWindow_OnMouseLeave()
        {
            //linePart2Window.material.SetColor("_EmissiveColor", new Color(207,207,207));
            colorLineCurrent = new Color(207, 207, 207);
            PartRef.SetHighlightDefault();
        }

        internal override void OnGUIOnceOnly()
        {
            //Init the skins
            SkinsLibrary_SkinChanged();
        }

        Int32 TransfersCount = 0;
        internal override void DrawWindow(int id)
        {
            TransfersCount = 0;
            if (SkinsLibrary.CurrentSkin.name=="Unity")
            {
                GUI.Box(new Rect(0, 0, WindowWidthForBars - 1,WindowRect.height- 1), "", SkinsLibrary.CurrentSkin.window);
            }

            GUILayout.BeginVertical();
            GUI.Label(new Rect(0,0,WindowWidthForBars+WindowWidthForFlowControl-1,18), this.PartRef.partInfo.title, Styles.stylePartWindowHead);
            GUILayout.Space(18);
            int i = 0;
            foreach (int key in this.ResourceList.Keys)
            {
                Boolean TransferExists = mbARP.lstTransfers.ItemExists(this.PartID, key);
                Boolean TransferActive=false;
                GUIStyle Highlight = Styles.styleBarHighlight;
                if (TransferExists)
                {
                    if (mbARP.lstTransfers.GetItem(this.PartID, key).transferState== TransferStateEnum.In)
                        Highlight = Styles.styleBarHighlightGreen;
                    else if (mbARP.lstTransfers.GetItem(this.PartID, key).transferState== TransferStateEnum.Out)
                        Highlight = Styles.styleBarHighlightRed;

                    TransferActive=mbARP.lstTransfers.GetItem(this.PartID, key).Active;
                }

                GUILayout.Space(2);
                if (i > 0) GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                //add title
                Rect rectIcon = Drawing.DrawResourceIcon(this.ResourceList[key].ResourceDef.name);
                GUILayout.Space(2);

                Rect rectBar = Drawing.CalcBarRect(rectIcon,Icon2BarOffset,120,15);
                if (Drawing.DrawResourceBar(rectBar, this.ResourceList[key], 
                    Styles.styleBarGreen_Back, Styles.styleBarGreen, Styles.styleBarGreen_Thin,
                    settings.ShowRatesForParts, TransferActive, Highlight))
                {
                    //if (this.ResourceList[key].ResourceDef.resourceTransferMode != ResourceTransferMode.NONE)
                    if (this.ResourceList[key].ResourceDef.resourceTransferMode != ResourceTransferMode.NONE &&
                            (
                            HighLogic.CurrentGame.Mode != Game.Modes.CAREER ||
                            GameVariables.Instance.UnlockedFuelTransfer(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.ResearchAndDevelopment))
                            )
                        )
                    {

                        //toggle the transfer line
                        if (TransferExists)
                        {
                            mbARP.lstTransfers.RemoveItem(this.PartID, key);
                            TransferExists = false;
                        }
                        else
                        {
                            mbARP.lstTransfers.AddItem(this.PartRef, this.ResourceList[key].ResourceDef, TransferStateEnum.None);
                        }
                    }
                }

                if (this.ResourceList[key].ResourceDef.resourceFlowMode != ResourceFlowMode.NO_FLOW)
                {
                    if (Drawing.DrawFlowControlButton(rectBar, this.PartRef.Resources.Get(key).flowState))
                        this.PartRef.Resources.Get(key).flowState = !this.PartRef.Resources.Get(key).flowState;
                }
                
                GUILayout.EndHorizontal();

                if (TransferExists)
                {
                    TransfersCount++;
                    ARPTransfer tmpTransfer = mbARP.lstTransfers.GetItem(this.PartID, key);
                    GUILayout.Space(1);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(8);
                    GUIStyle styleTransfer = new GUIStyle(SkinsLibrary.CurrentSkin.label);
                    styleTransfer.fixedHeight = 19;
                    styleTransfer.padding.top = -8;
                    styleTransfer.padding.bottom = 0;
                    //GUILayout.Label("Transfer: " + tmpTransfer.transferState, styleTransfer);
                    GUILayout.Label("Transfer: " , styleTransfer);

                    GUILayout.Space(21);


                    GUIStyle styleTransferButton = new GUIStyle(SkinsLibrary.CurrentSkin.button);
                    styleTransferButton.fixedHeight = 19;
                    styleTransferButton.fixedWidth = 40;
                    styleTransferButton.onNormal = styleTransferButton.active;
                    styleTransferButton.padding = new RectOffset(0, 0, 0, 0);
                    styleTransferButton.margin = new RectOffset(0, 0, 0, 0);


                    String strOut = (tmpTransfer.transferState == TransferStateEnum.Out && tmpTransfer.Active) ? "Stop" : "Out";
                    String strIn = (tmpTransfer.transferState == TransferStateEnum.In && tmpTransfer.Active) ? "Stop" : "In";
                    Boolean blnTempOut = GUILayout.Toggle(tmpTransfer.transferState == TransferStateEnum.Out, strOut, styleTransferButton);
                    Boolean blnTempIn = GUILayout.Toggle(tmpTransfer.transferState == TransferStateEnum.In, strIn, styleTransferButton);

                    if (blnTempOut && (tmpTransfer.transferState != TransferStateEnum.Out))
                    {
                        //if there are any transfers in place for this resource then turn off the Out
                        //if (mbARP.lstTransfers.Any(x => x.ResourceID == key && x.Active))
                        //    mbARP.lstTransfers.SetStateNone(key);
                        //else
                        //    mbARP.lstTransfers.SetStateNone(key, TransferStateEnum.Out);

                        tmpTransfer.transferState = TransferStateEnum.Out;
                    }
                    else if (blnTempIn && (tmpTransfer.transferState != TransferStateEnum.In))
                    {
                        //if there are any transfers in place for this resource then turn off the In
                        //if (mbARP.lstTransfers.Any(x => x.ResourceID == key && x.Active))
                        //    mbARP.lstTransfers.SetStateNone(key);
                        //else
                        //    mbARP.lstTransfers.SetStateNone(key, TransferStateEnum.In);

                        tmpTransfer.transferState = TransferStateEnum.In;
                    }
                    else if (!blnTempIn && !blnTempOut && (tmpTransfer.transferState != TransferStateEnum.None))
                        tmpTransfer.transferState = TransferStateEnum.None;

                    //if (tmpTransfer.Active)
                    //{
                    //    GUILayout.Space(20);
                    //    if (GUILayout.Button("Stop", styleTransferButton))
                    //    {
                    //        //mbARP.lstTransfers.SetStateNone(key);
                    //        mbARP.lstTransfers.SetStateNone(key);
                    //    }
                    //}
                    //else
                    //{
                    //    //if (GUILayout.Button("Out", styleTransferButton)) { tmpTransfer.transferState = TransferStateEnum.Out; }
                    //    //if (GUILayout.Button("In", styleTransferButton)) { tmpTransfer.transferState = TransferStateEnum.In; }
                    //    //if (GUILayout.Button("None", styleTransferButton)) { tmpTransfer.transferState = TransferStateEnum.None; }
                    //    Boolean blnTempOut = GUILayout.Toggle(tmpTransfer.transferState == TransferStateEnum.Out, "Out", styleTransferButton);
                    //    Boolean blnTempIn = GUILayout.Toggle(tmpTransfer.transferState == TransferStateEnum.In, "In", styleTransferButton);
                    //    if (blnTempOut && (tmpTransfer.transferState != TransferStateEnum.Out))
                    //    {
                    //        if (mbARP.lstTransfers.Any(x=>x.transferState== TransferStateEnum.In))
                    //            mbARP.lstTransfers.SetStateNone(key);
                    //        else
                    //            mbARP.lstTransfers.SetStateNone(key, TransferStateEnum.Out);

                    //        tmpTransfer.transferState = TransferStateEnum.Out;
                    //    }
                    //    else if (blnTempIn && (tmpTransfer.transferState != TransferStateEnum.In))
                    //    {
                    //        if (mbARP.lstTransfers.Any(x => x.transferState == TransferStateEnum.Out))
                    //            mbARP.lstTransfers.SetStateNone(key);
                    //        else
                    //            mbARP.lstTransfers.SetStateNone(key, TransferStateEnum.In);

                    //        tmpTransfer.transferState = TransferStateEnum.In;
                    //    }
                    //    else if (!blnTempIn && !blnTempOut && (tmpTransfer.transferState != TransferStateEnum.None))
                    //        tmpTransfer.transferState = TransferStateEnum.None;
                    //}
                    GUILayout.EndHorizontal();
                }

                i++;
            }
        }
        private Boolean MouseOver;

        delegate void MouseEventHandler();
        event MouseEventHandler OnMouseEnter;
        event MouseEventHandler OnMouseLeave;
        internal override void OnGUIEvery()
        {
            if (Visible && !UIHidden)
            {
                Boolean oldMouseOver = MouseOver;
                MouseOver =  (Event.current.type== EventType.repaint) && WindowRect.Contains(Event.current.mousePosition);

                if (oldMouseOver != MouseOver)
                {
                    if (MouseOver) OnMouseEnter(); else OnMouseLeave(); 
                }

                //Heres where all the line stuff goes - still need to hide it on F2
                Single WindowEndX = WindowRect.x + 5;
                if (LeftSide) WindowEndX = WindowRect.x + WindowRect.width - 5;
                Single WindowEndY = Screen.height - WindowRect.y - (WindowRect.height / 2);
                Drawing.DrawLine(new Vector2((Single)PartScreenPos.x, Screen.height - (Single)PartScreenPos.y),
                    new Vector2(WindowEndX, Screen.height - WindowEndY), colorLineCurrent, 2);
            }
            base.OnGUIEvery();
        }

        Boolean UIHidden = false;
        internal override void Update()
        {
            //detect if F@ is pressed to hide UI - this is so we can hide the lines as they are drawn in OnGUI Code
            if (Input.GetKeyDown(KeyCode.F2))
                UIHidden = !UIHidden;
        }

        internal override void LateUpdate()
        {
            UpdateWindowSizeAndVariables();
            //UpdateWindowPos();
            //UpdateLinePos();
        }

        //#region LineRenderer Stuff
        //private void CreateLineRenderer()
        //{
        //    LineColor = Color.white;
        //    lineObj = new GameObject(string.Format("LineObj-{0}", WindowID));
        //    linePart2Window = lineObj.AddComponent<LineRenderer>();
        //    linePart2Window.material = new Material(Shader.Find("KSP/Emissive/Diffuse"));
        //    linePart2Window.renderer.castShadows = false;
        //    linePart2Window.renderer.receiveShadows = false;
        //    linePart2Window.material.color = LineColor;
        //    linePart2Window.material.SetColor("_EmissiveColor", LineColor);

        //    linePart2Window.SetVertexCount(2);
        //}
        //internal void DestroyLineRenderer()
        //{
        //    LogFormatted_DebugOnly("Killing line Renderer");
        //    linePart2Window.SetVertexCount(0);
        //    linePart2Window.enabled = false;
        //    GameObject.Destroy(lineObj);
        //    GameObject.Destroy(linePart2Window);
        //    linePart2Window = null;
        //}

        //internal void UpdateLinePos()
        //{
        //    //If the lines not ready yet
        //    if (linePart2Window == null) return;

        //    Vector3 partsub = new Vector3(0, 0, -1);
        //    //this is the end in front of the part
        //    vectLinePartEnd = FlightCamera.fetch.mainCamera.ScreenToWorldPoint(PartScreenPos) + partsub;
        //    linePart2Window.SetPosition(0, FlightCamera.fetch.mainCamera.ScreenToWorldPoint(PartScreenPos + partsub));
        //    //this is the end behind the window
        //    Single WindowEndX = WindowRect.x + 5;
        //    if (LeftSide) WindowEndX = WindowRect.x + WindowRect.width - 5;
        //    Single WindowEndY = Screen.height - WindowRect.y - (WindowRect.height / 2);
        //    vectLineWindowEnd = FlightCamera.fetch.mainCamera.ScreenToWorldPoint(new Vector3(WindowEndX, WindowEndY, (float)((PartScreenPos + partsub).z)));

        //    linePart2Window.SetPosition(1, vectLineWindowEnd);

        //    float Scale = (float)50 / 1000 * (FlightCamera.fetch.transform.position - FlightGlobals.ActiveVessel.findWorldCenterOfMass()).magnitude;

        //    linePart2Window.SetWidth((float)50 / 1000 * Scale, (float)50 / 1000 * Scale);
        //    linePart2Window.enabled = true;
        //}

        //#endregion
    }

    /// <summary>
    /// Dictionary List of ARPPartWindows-with some extra functions
    ///   Key:Parts InstanceID
    /// </summary>
    internal class ARPPartWindowList: Dictionary<Int32,ARPPartWindow>
    {
        /// <summary>
        /// This stores a list of game objects that we attache the part windows to so we can destroy them when they are closed and not leak memory like a sieve
        /// </summary>
        private Dictionary<Int32, GameObject> WindowGameObjects = new Dictionary<int, GameObject>();

        /// <summary>
        /// Checks for and adds a PartWindow to the List if not there already - does NOT update resources
        /// </summary>
        /// <param name="PartToAddorUpdate">Part to Add the Window For</param>
        /// <returns>The PartWindow for this part</returns>
        internal ARPPartWindow AddPartWindow(Part PartToAddorUpdate, KSPAlternateResourcePanel mbARP)
        {
            Int32 PartID = PartToAddorUpdate.GetInstanceID();
            if (!this.ContainsKey(PartID))
            {
                MonoBehaviourExtended.LogFormatted_DebugOnly("Adding Part Window");
                WindowGameObjects.Add(PartID, new GameObject(string.Format("PartWindowObj-{0}", PartID)));
                ARPPartWindow pwNew = WindowGameObjects[PartID].AddComponent<ARPPartWindow>();
                pwNew.Init(PartToAddorUpdate,mbARP);
                this.Add(PartID, pwNew);
            }
            return this[PartID];
        }

        /// <summary>
        /// Checks for and adds a PartWindow to the List if not there already - does NOT update resources
        /// </summary>
        /// <param name="PartToAddorUpdate">Part to Add the Window For</param>
        /// <param name="PartsResource">A PartResource to Add/Update in the PartWindows details</param>
        /// <returns>The PartWindow for this part</returns>
        internal ARPPartWindow AddPartWindow(Part PartToAddorUpdate, PartResource PartsResource, KSPAlternateResourcePanel mbARP,Double Period)
        {
            //Ensure PartWindow Exists first
            ARPPartWindow pwTemp= AddPartWindow(PartToAddorUpdate,mbARP);

            //Then add the Resource
            pwTemp.ResourceList.SetUTPeriod(Period);
            pwTemp.ResourceList.UpdateResource(PartsResource,CalcRates:KSPAlternateResourcePanel.settings.ShowRatesForParts);

            return pwTemp;
        }

        /// <summary>
        /// A tiny man with a little squegee gets in his window cleaning bucket and travels up and down the PartWindowSkyScraper.
        /// Where he finds Windows for Parts that no longer exist he delicately marks them and then smashes them to pieces with
        /// an excessive amount of force and vigour
        /// </summary>
        internal void CleanWindows()
        {
            List<Int32> IDsToRemove = this.Keys.Except(FlightGlobals.ActiveVessel.parts.Select(x => x.GetInstanceID())).ToList<Int32>();
            foreach (KeyValuePair<Int32,ARPPartWindow> pwTemp in this)
            {
                if (pwTemp.Value.ResourceList.Count < 1 && !IDsToRemove.Contains(pwTemp.Key))
                    IDsToRemove.Add(pwTemp.Key);
            }

            foreach (Int32 pID in IDsToRemove)
            {
                MonoBehaviourExtended.LogFormatted_DebugOnly("Removing Part Window");
                DestroyARPPartWindow(pID);
            }
        }

        private void DestroyARPPartWindow(Int32 pID)
        {
            //Destroy the line
            //this[pID].DestroyLineRenderer();
            //hide the window or the GUI will be stranded - like linerender setting vertex to 0
            this[pID].Visible = false;
            this[pID].enabled = false;
            //kill the game object that the window is attached to
            GameObject.Destroy(WindowGameObjects[pID]);
            WindowGameObjects[pID] = null;

            //Remove this part ID and bits from the lists
            WindowGameObjects.Remove(pID);
            this.Remove(pID);
        }
    }
}