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
                if (KSPAlternateResourcePanel.settings.ButtonStyleChosen==ARPWindowSettings.ButtonStyleEnum.Launcher)
                {
                    btnAppLauncher = InitAppLauncherButton();
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


                ApplicationLauncher.Instance.EnableMutuallyExclusive(retButton);

                //appButton = ApplicationLauncher.Instance.AddApplication(
                //    onAppLaunchToggleOn, onAppLaunchToggleOff,
                //    onAppLaunchHoverOn, onAppLaunchHoverOff,
                //    null, null,
                //    (Texture)Resources.texAppLaunchIcon);
                //appButton.VisibleInScenes = ApplicationLauncher.AppScenes.FLIGHT;


                if (KSPAlternateResourcePanel.settings.ToggleOn)
                    retButton.toggleButton.SetTrue();
            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("Failed to set up App Launcher Button\r\n{0}",ex.Message);
                retButton = null;
            }
            return retButton;
        }

        internal void DestroyAppLauncherButton()
        {
            if (btnAppLauncher != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(btnAppLauncher);
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
