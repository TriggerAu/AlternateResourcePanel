using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using KSP;
using UnityEngine;

namespace KSPAlternateResourcePanel
{
    public partial class KSPAlternateResourcePanel
    {

        static Boolean DrawStuffConfigured = false;

        public void SetupDrawStuff()
        {
            GUI.skin = HighLogic.Skin;
            InitStyles();
            DrawStuffConfigured = true;
        }

        public static Texture2D texPanel = new Texture2D(16, 16, TextureFormat.ARGB32, false);

        public static Texture2D texBarBlue = new Texture2D(14, 14, TextureFormat.ARGB32, false);
        public static Texture2D texBarBlue_Back = new Texture2D(14, 14, TextureFormat.ARGB32, false);
        public static Texture2D texBarGreen = new Texture2D(14, 14, TextureFormat.ARGB32, false);
        public static Texture2D texBarGreen_Back = new Texture2D(14, 14, TextureFormat.ARGB32, false);

        public static Texture2D btnChevronUp = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnChevronDown = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        public static Texture2D btnSettingsAttention = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        public static Texture2D txtTooltipBackground = new Texture2D(9, 9);//, TextureFormat.ARGB32, false);

        public static Dictionary<String, Texture2D> texIconsKSPARP;
        public static Dictionary<String, Texture2D> texIconsPlayer;

        void LoadTextures()
        {
            DebugLogFormatted("Loading Textures");

            texIconsKSPARP = LoadIconDictionary("Icons");
            DebugLogFormatted("KSPARP Icons Loaded: {0}",texIconsKSPARP.Count.ToString());
            texIconsPlayer = LoadIconDictionary("Icons-Player");
            DebugLogFormatted("Player Icons Loaded: {0}", texIconsPlayer.Count.ToString());

            LoadImageIntoTexture2(ref texPanel, "img_PanelBack.png");

            LoadImageIntoTexture2(ref texBarBlue, "img_BarBlue.png");
            LoadImageIntoTexture2(ref texBarBlue_Back, "img_BarBlue_Back.png");
            LoadImageIntoTexture2(ref texBarGreen, "img_BarGreen.png");
            LoadImageIntoTexture2(ref texBarGreen_Back, "img_BarGreen_Back.png");

            LoadImageIntoTexture2(ref btnChevronUp, "img_buttonChevronUp.png");
            LoadImageIntoTexture2(ref btnChevronDown, "img_buttonChevronDown.png");

            LoadImageIntoTexture2(ref btnSettingsAttention, "img_buttonSettingsAttention.png");

            LoadImageIntoTexture2(ref txtTooltipBackground, "txt_TooltipBackground.png");
        }

        private static Dictionary<String, Texture2D> LoadIconDictionary(String IconFolderName)
        {
            Dictionary<String, Texture2D> dictReturn = new Dictionary<string, Texture2D>();
            Texture2D texLoading;

            String strIconPath = string.Format("{0}/{1}", PlugInPath, IconFolderName);
            if (Directory.Exists(strIconPath))
            {
                FileInfo[] fileIcons = new System.IO.DirectoryInfo(strIconPath).GetFiles("*.png");
                foreach (FileInfo fileIcon in fileIcons)
                {
                    texLoading = new Texture2D(32, 16, TextureFormat.ARGB32, false);
                    if (LoadImageIntoTexture2(ref texLoading, fileIcon.Name, strIconPath))
                        dictReturn.Add(fileIcon.Name.ToLower().Replace(".png", ""), texLoading);
                }
            }
            return dictReturn;
        }

        public static GUIStyle styleButton;
        public static GUIStyle styleButtonMain;
        public static GUIStyle styleButtonSettings;

        public static GUIStyle stylePanel;

        public static GUIStyle styleBarName;
        public static GUIStyle styleBarDef;

        public static GUIStyle styleBarBlue;
        public static GUIStyle styleBarBlue_Back;
        public static GUIStyle styleBarBlue_Thin;
        public static GUIStyle styleBarGreen;
        public static GUIStyle styleBarGreen_Back;
        public static GUIStyle styleBarGreen_Thin;

        public static GUIStyle styleBarText;
        public static GUIStyle styleBarRateText;

        public static GUIStyle styleStageText;
        public static GUIStyle styleStageTextHead;
        public static GUIStyle styleStageButton;

        public static GUIStyle styleTooltipStyle;

        public static GUIStyle styleToggle;

        public static GUIStyle styleSettingsArea;

        void InitStyles()
        {
            DebugLogFormatted("Configuring Styles");
            styleButton = new GUIStyle(GUI.skin.button);
            styleButton.normal.background = HighLogic.Skin.button.normal.background;
            styleButton.hover.background = HighLogic.Skin.button.hover.background;
            styleButton.normal.textColor = new Color(207, 207, 207);
            styleButton.fontStyle = FontStyle.Normal;
            styleButton.fixedHeight = 18;
            //styleButton.alignment = TextAnchor.MiddleCenter;

            styleButtonMain = new GUIStyle(styleButton);
            styleButtonMain.fixedHeight = 20;

            styleButtonSettings = new GUIStyle(styleButton);
            styleButtonSettings.padding = new RectOffset(1, 1, 1, 1);
            styleButtonSettings.fixedWidth = 40;

            stylePanel = new GUIStyle();
            stylePanel.border = new RectOffset(6, 6, 6, 6);
            stylePanel.normal.background = texPanel;
            stylePanel.padding = new RectOffset(8, 4, 7, 0);

            styleBarName = new GUIStyle() { fixedHeight = 16, fixedWidth = 32 };
            styleBarName.normal.textColor = Color.white;
            styleBarName.alignment = TextAnchor.MiddleCenter;

            styleBarDef = new GUIStyle(GUI.skin.box);
            styleBarDef.border = new RectOffset(2, 2, 2, 2);
            styleBarDef.normal.textColor = Color.white;

            styleBarBlue = new GUIStyle(styleBarDef);
            styleBarBlue.normal.background = texBarBlue;
            styleBarBlue_Back = new GUIStyle(styleBarDef);
            styleBarBlue_Back.normal.background = texBarBlue_Back;
            styleBarBlue_Thin = new GUIStyle(styleBarBlue);
            styleBarBlue_Thin.border = new RectOffset(0, 0, 0, 0);
            styleBarGreen = new GUIStyle(styleBarDef);
            styleBarGreen.normal.background = texBarGreen;
            styleBarGreen_Back = new GUIStyle(styleBarDef);
            styleBarGreen_Back.normal.background = texBarGreen_Back;
            styleBarGreen_Thin = new GUIStyle(styleBarGreen);
            styleBarGreen_Thin.border = new RectOffset(0, 0, 0, 0);

            styleBarText = new GUIStyle(GUI.skin.label);
            styleBarText.fontSize = 12;
            styleBarText.alignment = TextAnchor.MiddleCenter;
            styleBarText.normal.textColor = new Color(255, 255, 255, 0.8f);
            styleBarText.wordWrap = false;

            styleBarRateText = new GUIStyle(styleBarText);
            styleBarRateText.alignment = TextAnchor.MiddleRight;



            styleStageText = new GUIStyle(GUI.skin.label);
            styleStageText.normal.textColor = new Color(207, 207, 207);
            styleStageText.wordWrap = false;

            styleStageTextHead = new GUIStyle(styleStageText);
            styleStageTextHead.fontStyle = FontStyle.Bold;
            styleStageTextHead.wordWrap = false;

            styleStageButton = new GUIStyle(styleButton);

            styleToggle = new GUIStyle(HighLogic.Skin.toggle);
            styleToggle.normal.textColor = new Color(207, 207, 207);
            styleToggle.fixedHeight = 20;
            styleToggle.padding = new RectOffset(6, 0, -2, 0);

            styleSettingsArea = new GUIStyle(HighLogic.Skin.textArea);
            styleSettingsArea.padding = new RectOffset(0, 0, 0, 4);

            styleTooltipStyle = new GUIStyle();
            styleTooltipStyle.fontSize = 12;
            styleTooltipStyle.normal.textColor = new Color32(207, 207, 207, 255);
            styleTooltipStyle.stretchHeight = true;
            styleTooltipStyle.wordWrap = true;
            styleTooltipStyle.normal.background = txtTooltipBackground;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            styleTooltipStyle.border = new RectOffset(3, 3, 3, 3);
            styleTooltipStyle.padding = new RectOffset(4, 4, 6, 4);
            styleTooltipStyle.alignment = TextAnchor.MiddleCenter;

        }




        //public static Byte[] LoadFileToArray(String Filename)
        //{
        //    Byte[] arrBytes;

        //    arrBytes = KSP.IO.File.ReadAllBytes<KSPAlternateResourcePanel>(Filename);

        //    return arrBytes;
        //}
        public static Byte[] LoadFileToArray2(String Filename)
        {
            Byte[] arrBytes;

            arrBytes = System.IO.File.ReadAllBytes(Filename);

            return arrBytes;
        }

        //public static void SaveFileFromArray(Byte[] data, String Filename)
        //{
        //    KSP.IO.File.WriteAllBytes<KSPAlternateResourcePanel>(data, Filename);
        //}


        //public static Boolean LoadImageIntoTexture(ref Texture2D tex, String FileName)
        //{
        //    Boolean blnReturn = false;
        //    try
        //    {
        //        //DebugLogFormatted("Loading {0}", FileName);
        //        tex.LoadImage(LoadFileToArray(FileName));
        //        blnReturn = true;
        //    }
        //    catch (Exception)
        //    {
        //        DebugLogFormatted("Failed to load (are you missing a file):{0}", FileName);
        //    }
        //    return blnReturn;
        //}

        public static Boolean LoadImageIntoTexture2(ref Texture2D tex, String FileName,String FolderPath="")
        {
            //DebugLogFormatted("{0},{1}",FileName, FolderPath);
            Boolean blnReturn = false;
            try
            {
                if (FolderPath == "") FolderPath = PlugInPath;
                //DebugLogFormatted("Loading {0}", FileName);
                tex.LoadImage(LoadFileToArray2(string.Format("{0}/{1}", FolderPath, FileName)));
                blnReturn = true;
            }
            catch (Exception)
            {
                DebugLogFormatted("Failed to load (are you missing a file):{0}", FileName);
            }
            return blnReturn;
        }

    }
}
