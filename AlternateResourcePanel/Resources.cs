using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPAlternateResourcePanel
{
    internal class Resources
    {
        //WHERE SHOULD THESE BE???
        internal static String PathApp = KSPUtil.ApplicationRootPath.Replace("\\", "/");
        internal static String PathTriggerTech = string.Format("{0}GameData/TriggerTech", PathApp);
        internal static String PathPlugin = string.Format("{0}/{1}", PathTriggerTech, KSPAlternateResourcePanel._AssemblyName);
        internal static String PathPluginSounds = string.Format("{0}/Sounds", PathPlugin);
        //internal static String PathPluginData = string.Format("{0}/Data", PathPlugin);
        //internal static String PathPluginTextures = string.Format("{0}/Textures", PathPlugin);

        internal static String DBPathTriggerTech = string.Format("TriggerTech");
        internal static String DBPathPlugin = string.Format("TriggerTech/{0}", KSPAlternateResourcePanel._AssemblyName);
        internal static String DBPathToolbarIcons = string.Format("{0}/ToolbarIcons", DBPathPlugin);
        internal static String DBPathTextures = string.Format("{0}/Textures", DBPathPlugin);
        internal static String DBPathPluginSounds = string.Format("{0}/Sounds", DBPathPlugin);

        internal static Texture2D texPanel;
        internal static Texture2D texBarBlue; // = new Texture2D(14, 14, TextureFormat.ARGB32, false);
        internal static Texture2D texBarBlue_Back; // = new Texture2D(14, 14, TextureFormat.ARGB32, false);
        internal static Texture2D texBarGreen; // = new Texture2D(14, 14, TextureFormat.ARGB32, false);
        internal static Texture2D texBarGreen_Back; // = new Texture2D(14, 14, TextureFormat.ARGB32, false);

        internal static Texture2D texBarHighlight; // = new Texture2D(14, 14, TextureFormat.ARGB32, false);

        internal static Texture2D btnChevronUp; // = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        internal static Texture2D btnChevronDown; // = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        internal static Texture2D btnSettingsAttention; // = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        internal static Texture2D texPartWindowHead;

        //internal static Texture2D texTooltipBackground; // = new Texture2D(9, 9);//, TextureFormat.ARGB32, false);

        internal static Texture2D btnAlarm;
        internal static Texture2D btnAlarmEnabled;
        internal static Texture2D btnAlarmWarn;
        internal static Texture2D btnAlarmAlert;

        internal static Texture2D btnDropDown;
        internal static Texture2D btnPlay;
        internal static Texture2D btnStop;

        internal static Texture2D texBox;
        internal static Texture2D texBoxUnity;

        internal static Texture2D texSeparatorV;
        internal static Texture2D texSeparatorH;

        //Icon Libraries
        internal static Dictionary<String, Texture2D> texIconsKSPARP;
        internal static Dictionary<String, Texture2D> texIconsPlayer;
        internal static Dictionary<String, Texture2D> texIconsResourceDefs;
        
        //Alarm Library
        internal static Dictionary<String, AudioClip> clipAlarms;

        internal static void LoadSounds()
        {
            MonoBehaviourExtended.LogFormatted("Loading Sounds");

            clipAlarms = new Dictionary<string, AudioClip>();
            clipAlarms.Add("None", null);
            if (Directory.Exists(PathPluginSounds))
            {
                //get all the png and tga's
                FileInfo[] fileClips = new System.IO.DirectoryInfo(PathPluginSounds).GetFiles("*.wav");

                foreach (FileInfo fileClip in fileClips)
                {
                    try
                    {
                        //load the file from the GameDB
                        AudioClip clipLoading = null;
                        if (LoadAudioClipFromGameDB(ref clipLoading, fileClip.Name))
                        {
                            String ClipKey = fileClip.Name;
                            if (ClipKey.ToLower().EndsWith(".wav"))
                                ClipKey = ClipKey.Substring(0, ClipKey.Length - 4);
                            clipAlarms.Add(ClipKey, clipLoading);
                        }
                    }
                    catch (Exception)
                    {
                        //MonoBehaviourExtended.LogFormatted("Unable to load AudioClip from GameDB:{0}/{1}", PathPluginSounds,fileClip.Name);
                    }
                }
            }
        
        }

        internal static void LoadTextures()
        {
            MonoBehaviourExtended.LogFormatted("Loading Textures");

            texIconsKSPARP = LoadIconDictionary("Icons");
            MonoBehaviourExtended.LogFormatted("KSPARP Icons Loaded: {0}", texIconsKSPARP.Count.ToString());
            texIconsPlayer = LoadIconDictionary("Icons-Player");
            MonoBehaviourExtended.LogFormatted("Player Icons Loaded: {0}", texIconsPlayer.Count.ToString());
            texIconsResourceDefs = LoadIconDictionary_Defs();
            MonoBehaviourExtended.LogFormatted("Mod Definition Icons Loaded: {0}", texIconsResourceDefs.Count.ToString());

            //Set the ordering arrays
            dictFirst = texIconsKSPARP;
            dictSecond = texIconsResourceDefs;
            dictThird = texIconsPlayer;

            LoadImageFromGameDB(ref texPanel, "img_PanelBack.png");

            LoadImageFromGameDB(ref texBarBlue, "img_BarBlue.png");
            LoadImageFromGameDB(ref texBarBlue_Back, "img_BarBlue_Back.png");
            LoadImageFromGameDB(ref texBarGreen, "img_BarGreen.png");
            LoadImageFromGameDB(ref texBarGreen_Back, "img_BarGreen_Back.png");

            LoadImageFromGameDB(ref texBarHighlight, "img_BarHighlight.tga");

            LoadImageFromGameDB(ref btnChevronUp, "img_buttonChevronUp.png");
            LoadImageFromGameDB(ref btnChevronDown, "img_buttonChevronDown.png");

            LoadImageFromGameDB(ref btnSettingsAttention, "img_buttonSettingsAttention.png");

            LoadImageFromGameDB(ref texPartWindowHead, "img_PartWindowHead.png");

            //LoadImageFromGameDB(ref texTooltipBackground, "tex_TooltipBackground.png");

            LoadImageFromGameDB(ref btnAlarm, "img_Alarm.png");
            LoadImageFromGameDB(ref btnAlarmEnabled, "img_AlarmEnabled.png");
            LoadImageFromGameDB(ref btnAlarmWarn, "img_AlarmWarn.png");
            LoadImageFromGameDB(ref btnAlarmAlert, "img_AlarmAlert.png");

            LoadImageFromGameDB(ref btnDropDown, "img_DropDown.tga");
            LoadImageFromGameDB(ref btnPlay, "img_Play.tga");
            LoadImageFromGameDB(ref btnStop, "img_Stop.tga");
            //LoadImageFromGameDB(ref btnDropDownSep, "img_DropDownSep.tga");

            //LoadImageFromGameDB(ref texDropDownListBox, "tex_DropDownListBox.tga");
            //LoadImageFromGameDB(ref texDropDownListBoxUnity, "tex_DropDownListBoxUnity.tga");

            LoadImageFromGameDB(ref texBox, "tex_Box.tga");
            LoadImageFromGameDB(ref texBoxUnity, "tex_BoxUnity.tga");

            LoadImageFromGameDB(ref texSeparatorH , "img_SeparatorHorizontal.tga");
            LoadImageFromGameDB(ref texSeparatorV, "img_SeparatorVertical.tga");
        }



        private static Dictionary<String, Texture2D> LoadIconDictionary(String IconFolderName)
        {
            Dictionary<String, Texture2D> dictReturn = new Dictionary<string, Texture2D>();
            Texture2D texLoading;

            //Where are the Icons
            String strIconPath = string.Format("{0}/{1}", PathPlugin, IconFolderName);
            String strIconDBPath = string.Format("{0}/{1}", DBPathPlugin, IconFolderName);

            if (Directory.Exists(strIconPath))
            {
                //get all the png and tga's
                FileInfo[] fileIconsPNG = new System.IO.DirectoryInfo(strIconPath).GetFiles("*.png");
                FileInfo[] fileIconsTGA = new System.IO.DirectoryInfo(strIconPath).GetFiles("*.tga");
                FileInfo[] fileIcons = fileIconsPNG.Concat(fileIconsTGA).ToArray();

                foreach (FileInfo fileIcon in fileIcons)
                {
                    try
                    {
                        //load the file from the GameDB
                        texLoading = null;
                        if (LoadImageFromGameDB(ref texLoading, fileIcon.Name, strIconDBPath))
                            dictReturn.Add(fileIcon.Name.ToLower().Replace(".png", "").Replace(".tga", ""), texLoading);
                    }
                    catch (Exception)
                    {
                        MonoBehaviourExtended.LogFormatted("Unable to load Texture from GameDB:{0}", strIconPath);
                    }
                }
            }
            return dictReturn;
        }

        /// <summary>
        /// This one gets all the icons named in the resource definitions
        /// </summary>
        /// <returns></returns>
        private static Dictionary<String, Texture2D> LoadIconDictionary_Defs()
        {
            Dictionary<String, Texture2D> dictReturn = new Dictionary<string, Texture2D>();
            Texture2D texLoading;

            //Find All the RESOURCE_DEFINITION Nodes
            ConfigNode[] cns = GameDatabase.Instance.GetConfigNodes("RESOURCE_DEFINITION");
            foreach (ConfigNode cn in cns)
            {
                if (cn.HasValue("name"))
                {
                    if (cn.HasValue("ksparpicon"))
                    {
                        //If it has a name and a ksparpicon
                        try
                        {
                            //lead the Texture from the GameDB
                            texLoading = GameDatabase.Instance.GetTexture(cn.GetValue("ksparpicon"), false);
                            if ((texLoading.width > 32) || (texLoading.height > 16))
                            {
                                MonoBehaviourExtended.LogFormatted("Texture Too Big (32x16 is limit) - w:{0} h:{1}", texLoading.width, texLoading.height);
                            }
                            else
                            {
                                dictReturn.Add(cn.GetValue("name").ToLower(), texLoading);
                            }
                        }
                        catch (Exception)
                        {
                            MonoBehaviourExtended.LogFormatted("Unable to load texture {0}-{1}", cn.GetValue("name"), cn.GetValue("ksparpicon"));
                        }
                    }
                }
            }

            return dictReturn;
        }

        internal static Dictionary<String, Texture2D> dictFirst = texIconsKSPARP;
        internal static Dictionary<String, Texture2D> dictSecond = texIconsResourceDefs;
        internal static Dictionary<String, Texture2D> dictThird = texIconsPlayer;

        internal static void SetIconOrder(Settings settings)
        {
            dictFirst = GetIconDict(settings.lstIconOrder[0]);
            dictSecond = GetIconDict(settings.lstIconOrder[1]);
            dictThird = GetIconDict(settings.lstIconOrder[2]);
        }

        internal static Dictionary<String, Texture2D> GetIconDict(String Name)
        {
            switch (Name.ToLower())
            {
                case "ksparp": return texIconsKSPARP;
                case "mod": return texIconsResourceDefs;
                case "player": return texIconsPlayer;
                default:
                    return null;
            }
        }
        internal static GUIContent IconOrderContent(String Name)
        {
            switch (Name.ToLower())
            {
                case "ksparp": return new GUIContent("ARP", "Alternate Resource Panel");
                case "mod": return new GUIContent("Mod", "Mod Resource Definition");
                case "player": return new GUIContent("Player", "Players Icons");
                default:
                    return new GUIContent("ERROR", "");
            }
        }

        #region Util Stuff
        internal static Boolean LoadImageFromGameDB(ref Texture2D tex, String FileName, String FolderPath = "")
        {
            Boolean blnReturn = false;
            try
            {
                //trim off the tga and png extensions
                if (FileName.ToLower().EndsWith(".png")) FileName = FileName.Substring(0, FileName.Length - 4);
                if (FileName.ToLower().EndsWith(".tga")) FileName = FileName.Substring(0, FileName.Length - 4); 
                //default folder
                if (FolderPath == "") FolderPath = DBPathTextures;

                //Look for case mismatches
                if (!GameDatabase.Instance.ExistsTexture(String.Format("{0}/{1}", FolderPath, FileName)))
                    throw new Exception();
                
                //now load it
                tex = GameDatabase.Instance.GetTexture(String.Format("{0}/{1}", FolderPath, FileName), false);
                blnReturn = true;
            }
            catch (Exception)
            {
                MonoBehaviourExtended.LogFormatted("Failed to load (are you missing a file - and check case):{0}/{1}", FolderPath, FileName);
            }
            return blnReturn;
        }

        internal static Boolean LoadAudioClipFromGameDB(ref AudioClip clip, String FileName, String FolderPath = "")
        {
            Boolean blnReturn = false;
            try
            {
                //trim off the tga and png extensions
                if (FileName.ToLower().EndsWith(".wav")) FileName = FileName.Substring(0, FileName.Length - 4);
                //default folder
                if (FolderPath == "") FolderPath = DBPathPluginSounds;

                //Look for case mismatches
                if (!GameDatabase.Instance.ExistsAudioClip(String.Format("{0}/{1}", FolderPath, FileName)))
                    throw new Exception();

                //now load it
                clip = GameDatabase.Instance.GetAudioClip(String.Format("{0}/{1}", FolderPath, FileName));
                blnReturn = true;
            }
            catch (Exception)
            {
                MonoBehaviourExtended.LogFormatted("Failed to load (are you missing a file - and check case):{0}/{1}", FolderPath, FileName);
            }
            return blnReturn;
        }
        #endregion
    }
}
