using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions ;

using System.IO;

using KSP;
using UnityEngine;

namespace KSPAlternateResourcePanel
{
    public partial class KSPAlternateResourcePanel 
    {
        public static String AppPath = KSPUtil.ApplicationRootPath.Replace("\\", "/");
        public static String PlugInPath = string.Format(AppPath + "GameData/TriggerTech/PluginData/{0}", _ClassName);

        
        Boolean blnShowInstants = false;
        Boolean blnLockLocation = true;

        Boolean blnStaging = false;
        Boolean blnStagingInMapView = false;
        Boolean blnStagingSpaceInMapView = false;
        
        private void LoadSettings()
        {
            DebugLogFormatted("Loading Config...");
            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KSPAlternateResourcePanel>();
            configfile.load();

            ////Version check stuff
            //this.DailyVersionCheck = configfile.GetValue("DailyUpdateCheck", true);
            //try { this.VersionCheckDate_Attempt = DateTime.ParseExact(configfile.GetValue("VersionCheckDate_Attempt", ""), "yyyy-MM-dd", CultureInfo.CurrentCulture); }
            //catch (Exception) { this.VersionCheckDate_Attempt = new DateTime(); }
            //try { this.VersionCheckDate_Success = DateTime.ParseExact(configfile.GetValue("VersionCheckDate_Success", ""), "yyyy-MM-dd", CultureInfo.CurrentCulture); }
            //catch (Exception) { this.VersionCheckDate_Success = new DateTime(); }
            //this.VersionWeb = configfile.GetValue("VersionWeb", "");

            //Actual settings
            blnShowInstants = configfile.GetValue<Boolean>("ShowInstants", false);
            blnLockLocation = configfile.GetValue<Boolean>("LockLocation", true);
            _WindowMainRect = configfile.GetValue<Rect>("WindowPos", new Rect(Screen.width - 298, 19, 299, 20));

            blnStaging = configfile.GetValue<Boolean>("Staging", false);
            blnStagingInMapView = configfile.GetValue<Boolean>("StagingInMapView", false);
            blnStagingSpaceInMapView = configfile.GetValue<Boolean>("StagingSpaceInMapView", false);

            ToggleOn = configfile.GetValue<Boolean>("ToggleOn", false);

            DebugLogFormatted("Config Loaded Successfully");
        }

        //private void SaveSettings()
        //{
        //    SaveConfig();
        //    SaveUpdateCheck();
        //}

        //private void SaveUpdateCheck()
        //{
        //    DebugLogFormatted("Saving Update Check stuff...");

        //    KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KSPAlternateResourcePanel>();
        //    configfile.load();

        //    configfile.SetValue("DailyUpdateCheck", this.DailyVersionCheck);
        //    configfile.SetValue("VersionCheckDate_Attempt", this.VersionCheckDate_AttemptString);
        //    configfile.SetValue("VersionCheckDate_Success", this.VersionCheckDate_SuccessString);
        //    configfile.SetValue("VersionWeb", this.VersionWeb);

        //    configfile.save();
        //    DebugLogFormatted("Saved stuff :)");
        //}

        private void SaveConfig()
        {
            DebugLogFormatted("Saving Config...");

            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KSPAlternateResourcePanel >();
            configfile.load();

            configfile.SetValue("ShowInstants", blnShowInstants);
            configfile.SetValue("LockLocation", blnLockLocation);
            configfile.SetValue("WindowPos", _WindowMainRect);

            configfile.SetValue("Staging", blnStaging);
            configfile.SetValue("StagingInMapView", blnStagingInMapView);
            configfile.SetValue("StagingSpaceInMapView", blnStagingSpaceInMapView);

            configfile.SetValue("ToggleOn", ToggleOn);

            configfile.save();
            DebugLogFormatted("Saved Config");
        }




        //public String Version = "";
        //public String VersionWeb = "";
        //public Boolean VersionAvailable
        //{
        //    get
        //    {
        //        //todo take this out
        //        if (this.VersionWeb == "")
        //            return false;
        //        else
        //            try
        //            {
        //                //if there was a string and its version is greater than the current running one then alert
        //                System.Version vTest = new System.Version(this.VersionWeb);
        //                return (System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.CompareTo(vTest) < 0);
        //            }
        //            catch (Exception ex)
        //            {
        //                DebugLogFormatted("webversion: '{0}'", this.VersionWeb);
        //                DebugLogFormatted("Unable to compare versions: {0}", ex.Message);
        //                return false;
        //            }

        //        //return ((this.VersionWeb != "") && (this.Version != this.VersionWeb));
        //    }
        //}

        ////Are we doing daily checks
        //public Boolean DailyVersionCheck = true;
        //public String VersionCheckResult = "";
        ////attentionflag
        //public Boolean VersionAttentionFlag = false;
        ////When did we last check??
        //public DateTime VersionCheckDate_Attempt;
        //public String VersionCheckDate_AttemptString { get { return ConvertVersionCheckDateToString(this.VersionCheckDate_Attempt); } }
        //public DateTime VersionCheckDate_Success;
        //public String VersionCheckDate_SuccessString { get { return ConvertVersionCheckDateToString(this.VersionCheckDate_Success); } }

        //private String ConvertVersionCheckDateToString(DateTime Date)
        //{
        //    if (Date < DateTime.Now.AddYears(-10))
        //        return "No Date Recorded";
        //    else
        //        return String.Format("{0:yyyy-MM-dd}", Date);
        //}


        //public Boolean getLatestVersion()
        //{
        //    Boolean blnReturn = false;
        //    try
        //    {
        //        //Get the file from Codeplex
        //        this.VersionCheckResult = "Unknown - check again later";
        //        this.VersionCheckDate_Attempt = DateTime.Now;

        //        DebugLogFormatted("Reading version from Web");
        //        //Page content FormatException is |LATESTVERSION|1.2.0.0|LATESTVERSION|
        //        WWW www = new WWW("http://kerbalalarmclock.codeplex.com/wikipage?title=LatestVersion");
        //        while (!www.isDone) { }

        //        //Parse it for the version String
        //        String strFile = www.text;
        //        DebugLogFormatted("Response Length:" + strFile.Length);

        //        Match matchVersion;
        //        matchVersion = Regex.Match(strFile, "(?<=\\|LATESTVERSION\\|).+(?=\\|LATESTVERSION\\|)", System.Text.RegularExpressions.RegexOptions.Singleline);
        //        DebugLogFormatted("Got Version '" + matchVersion.ToString() + "'");

        //        String strVersionWeb = matchVersion.ToString();
        //        if (strVersionWeb != "")
        //        {
        //            this.VersionCheckResult = "Success";
        //            this.VersionCheckDate_Success = DateTime.Now;
        //            this.VersionWeb = strVersionWeb;
        //            blnReturn = true;
        //        }
        //        else
        //        {
        //            this.VersionCheckResult = "Unable to parse web service";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DebugLogFormatted("Failed to read Version info from web");
        //        DebugLogFormatted(ex.Message);

        //    }
        //    DebugLogFormatted("Version Check result:" + VersionCheckResult);
        //    return blnReturn;
        //}

        ///// <summary>
        ///// Does some logic to see if a check is needed, and returns true if there is a different version
        ///// </summary>
        ///// <param name="ForceCheck">Ignore all logic and simply do a check</param>
        ///// <returns></returns>
        //public Boolean VersionCheck(Boolean ForceCheck)
        //{
        //    Boolean blnReturn = false;
        //    Boolean blnDoCheck = false;

        //    try
        //    {
        //        if (ForceCheck)
        //        {
        //            blnDoCheck = true;
        //            DebugLogFormatted("Starting Version Check-Forced");
        //        }
        //        else if (this.VersionWeb == "")
        //        {
        //            blnDoCheck = true;
        //            DebugLogFormatted("Starting Version Check-No current web version stored");
        //        }
        //        else if (this.VersionCheckDate_Success < DateTime.Now.AddYears(-9))
        //        {
        //            blnDoCheck = true;
        //            DebugLogFormatted("Starting Version Check-No current date stored");
        //        }
        //        else if (this.VersionCheckDate_Success.Date != DateTime.Now.Date)
        //        {
        //            blnDoCheck = true;
        //            DebugLogFormatted("Starting Version Check-stored date is not today");
        //        }
        //        else
        //            DebugLogFormatted("Skipping version check");


        //        if (blnDoCheck)
        //        {
        //            getLatestVersion();
        //            SaveCheckSettings();

        //            //if theres a new version then set the flag
        //            VersionAttentionFlag = VersionAvailable;
        //        }
        //        blnReturn = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        DebugLogFormatted("Failed to run the update test");
        //        DebugLogFormatted(ex.Message);
        //    }
        //    return blnReturn;
        //}

    }
}
