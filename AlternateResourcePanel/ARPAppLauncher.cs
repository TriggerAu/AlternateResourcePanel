using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPAlternateResourcePanel
{
    public partial class KSPAlternateResourcePanel
    {
        void OnGUIAppLauncherReady()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("AppLauncherReady");
            if (ApplicationLauncher.Ready)
            {
                if (KSPAlternateResourcePanel.settings.ButtonStyleChosen==ARPWindowSettings.ButtonStyleEnum.Launcher || 
                    KSPAlternateResourcePanel.settings.ButtonStyleChosen==ARPWindowSettings.ButtonStyleEnum.StockReplace )
                {
                    btnAppLauncher = InitAppLauncherButton();

                    if (KSPAlternateResourcePanel.settings.ButtonStyleChosen == ARPWindowSettings.ButtonStyleEnum.StockReplace)
                    {
                        ReplaceStockAppButton();
                    }
                }
            }
            else { MonoBehaviourExtended.LogFormatted("App Launcher-Not Actually Ready"); }
        }

        void OnGameSceneLoadRequestedForAppLauncher(GameScenes SceneToLoad)
        {
            LogFormatted_DebugOnly("GameSceneLoadRequest");
            DestroyAppLauncherButton();
        }
        internal ApplicationLauncherButton btnAppLauncher = null;
        internal Boolean SceneChangeRequiredToRestoreResourcesApp=false;

        internal ApplicationLauncherButton InitAppLauncherButton()
        {
            ApplicationLauncherButton retButton = null;

            try
            {
                retButton = ApplicationLauncher.Instance.AddModApplication(
                    onAppLaunchToggleOn, onAppLaunchToggleOff,
                    onAppLaunchHoverOn, onAppLaunchHoverOff,
                    null, null,
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
                    (Texture)Resources.texAppLaunchIcon);

                AppLauncherButtonMutuallyExclusive(settings.AppLauncherMutuallyExclusive);

                //appButton = ApplicationLauncher.Instance.AddApplication(
                //    onAppLaunchToggleOn, onAppLaunchToggleOff,
                //    onAppLaunchHoverOn, onAppLaunchHoverOff,
                //    null, null,
                //    (Texture)Resources.texAppLaunchIcon);
                //appButton.VisibleInScenes = ApplicationLauncher.AppScenes.FLIGHT;

            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("Failed to set up App Launcher Button\r\n{0}",ex.Message);
                retButton = null;
            }
            return retButton;
        }

        internal void AppLauncherButtonMutuallyExclusive(Boolean Enable)
        {
            if (btnAppLauncher == null) return;
            if (Enable)
            {
                MonoBehaviourExtended.LogFormatted("Setting Mutually Exclusive");
                ApplicationLauncher.Instance.EnableMutuallyExclusive(btnAppLauncher);
            }
            else
            {
                MonoBehaviourExtended.LogFormatted("Clearing Mutually Exclusive");
                ApplicationLauncher.Instance.DisableMutuallyExclusive(btnAppLauncher);
            }
        }

        //internal ApplicationLauncherButton btnAppLauncher2 = null;

        //internal ApplicationLauncherButton InitAppLauncherButton2()
        //{
        //    ApplicationLauncherButton retButton = null;

        //    try
        //    {
        //        retButton = ApplicationLauncher.Instance.AddApplication(
        //            onAppLaunchToggleOn, onAppLaunchToggleOff,
        //            onAppLaunchHoverOn, onAppLaunchHoverOff,
        //            null, null,
        //            (Texture)Resources.texAppLaunchIcon);
        //        retButton.VisibleInScenes = ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW;


        //        //ApplicationLauncher.Instance.EnableMutuallyExclusive(retButton);

        //        //appButton = ApplicationLauncher.Instance.AddApplication(
        //        //    onAppLaunchToggleOn, onAppLaunchToggleOff,
        //        //    onAppLaunchHoverOn, onAppLaunchHoverOff,
        //        //    null, null,
        //        //    (Texture)Resources.texAppLaunchIcon);
        //        //appButton.VisibleInScenes = ApplicationLauncher.AppScenes.FLIGHT;


        //        if (KSPAlternateResourcePanel.settings.ToggleOn)
        //            retButton.toggleButton.SetTrue();
        //    }
        //    catch (Exception ex)
        //    {
        //        MonoBehaviourExtended.LogFormatted("Failed to set up App Launcher Button\r\n{0}", ex.Message);
        //        retButton = null;
        //    }
        //    return retButton;
        //}

        internal void DestroyAppLauncherButton()
        {
            if (btnAppLauncher != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(btnAppLauncher);
            }
        }

        internal Boolean StockAppToBeHidden = false;
        internal DateTime StockAppToBeHiddenAttemptDate;
        internal void ReplaceStockAppButton()
        {
            if (ResourceDisplay.Instance.appLauncherButton == null)
            {
                if (!StockAppToBeHidden)
                    StockAppToBeHiddenAttemptDate = DateTime.Now;
                StockAppToBeHidden = true;

                if (StockAppToBeHiddenAttemptDate.AddSeconds(5) < DateTime.Now)
                {
                    StockAppToBeHidden = false;
                    LogFormatted("Unable to Swap the ARP App for the Stock Resource App - tried for 5 secs");
                }
            }
            else
            {
                StockAppToBeHidden = false;
                ResourceDisplay.Instance.appLauncherButton.toggleButton.onDisable();

                ResourceDisplay.Instance.appLauncherButton.toggleButton.onHover = btnAppLauncher.toggleButton.onHover;
                ResourceDisplay.Instance.appLauncherButton.toggleButton.onHoverOut = btnAppLauncher.toggleButton.onHoverOut;
                ResourceDisplay.Instance.appLauncherButton.toggleButton.onTrue = btnAppLauncher.toggleButton.onTrue;
                ResourceDisplay.Instance.appLauncherButton.toggleButton.onFalse = btnAppLauncher.toggleButton.onFalse;
                ResourceDisplay.Instance.appLauncherButton.toggleButton.onEnable = btnAppLauncher.toggleButton.onEnable;
                ResourceDisplay.Instance.appLauncherButton.toggleButton.onDisable = btnAppLauncher.toggleButton.onDisable;
                ResourceDisplay.Instance.appLauncherButton.SetTexture(Resources.texAppLaunchIcon);

                try
                {
                    ApplicationLauncher.Instance.RemoveModApplication(btnAppLauncher);
                }
                catch (Exception)
                {

                }
                windowMain.DragEnabled = false;
                windowMain.WindowRect = new Rect(windowMainResetPos);
            }
        }

        void onAppLaunchToggleOn() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("TOn");
            KSPAlternateResourcePanel.settings.ToggleOn = true; 
            settings.Save(); 
        }
        void onAppLaunchToggleOff() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("TOff");
            KSPAlternateResourcePanel.settings.ToggleOn = false; 
            settings.Save(); 
        }
        void onAppLaunchHoverOn() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("HovOn");
            MouseOverAppLauncherBtn = true;
        }
        void onAppLaunchHoverOff() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("HovOff");
            MouseOverAppLauncherBtn = false; 
        }

        Boolean MouseOverAppLauncherBtn = false;
    }
}
