using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static Texture2D texPanel = new Texture2D(16,16,TextureFormat.ARGB32, false);

        public static Texture2D texBarBlue = new Texture2D(14, 14, TextureFormat.ARGB32, false);
        public static Texture2D texBarBlue_Back = new Texture2D(14, 14, TextureFormat.ARGB32, false);
        public static Texture2D texBarGreen = new Texture2D(14, 14, TextureFormat.ARGB32, false);
        public static Texture2D texBarGreen_Back = new Texture2D(14, 14, TextureFormat.ARGB32, false);

        public static Texture2D texEC = new Texture2D(32, 16, TextureFormat.ARGB32, false);
        public static Texture2D texKe = new Texture2D(32, 16, TextureFormat.ARGB32, false);
        public static Texture2D texLF = new Texture2D(32, 16, TextureFormat.ARGB32, false);
        public static Texture2D texMP = new Texture2D(32, 16, TextureFormat.ARGB32, false);
        public static Texture2D texO2 = new Texture2D(32, 16, TextureFormat.ARGB32, false);
        public static Texture2D texOX = new Texture2D(32, 16, TextureFormat.ARGB32, false);
        public static Texture2D texSF = new Texture2D(32, 16, TextureFormat.ARGB32, false);
        public static Texture2D texXe = new Texture2D(32, 16, TextureFormat.ARGB32, false);
        public static Texture2D texIA = new Texture2D(32, 16, TextureFormat.ARGB32, false);

        public static Texture2D btnChevronUp = new Texture2D(17, 16, TextureFormat.ARGB32, false);
        public static Texture2D btnChevronDown = new Texture2D(17, 16, TextureFormat.ARGB32, false);

        public static Texture2D txtTooltipBackground = new Texture2D(9, 9);//, TextureFormat.ARGB32, false);

        void LoadTextures()
        {
            DebugLogFormatted("Loading Textures");

            LoadImageIntoTexture(ref texPanel,"img_PanelBack.png");

            LoadImageIntoTexture(ref texBarBlue, "img_BarBlue.png");
            LoadImageIntoTexture(ref texBarBlue_Back, "img_BarBlue_Back.png");
            LoadImageIntoTexture(ref texBarGreen, "img_BarGreen.png");
            LoadImageIntoTexture(ref texBarGreen_Back, "img_BarGreen_Back.png");

            LoadImageIntoTexture(ref texEC, "img_EC.png");
            LoadImageIntoTexture(ref texKe, "img_Ke.png");
            LoadImageIntoTexture(ref texLF, "img_LF.png");
            LoadImageIntoTexture(ref texMP, "img_MP.png");
            LoadImageIntoTexture(ref texO2, "img_O2.png");
            LoadImageIntoTexture(ref texOX, "img_OX.png");
            LoadImageIntoTexture(ref texSF, "img_SF.png");
            LoadImageIntoTexture(ref texXe, "img_Xe.png");
            LoadImageIntoTexture(ref texIA, "img_IA.png");

            LoadImageIntoTexture(ref btnChevronUp, "img_buttonChevronUp.png");
            LoadImageIntoTexture(ref btnChevronDown, "img_buttonChevronDown.png");

            LoadImageIntoTexture(ref txtTooltipBackground, "txt_TooltipBackground.png");
        }

        public static GUIStyle styleButton;

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

        void InitStyles()
        {
            DebugLogFormatted("Configuring Styles");
            styleButton = new GUIStyle(GUI.skin.button);
            styleButton.normal.background = HighLogic.Skin.button.normal.background;
            styleButton.hover.background = HighLogic.Skin.button.hover.background;
            styleButton.normal.textColor = new Color(207, 207, 207);
            styleButton.fontStyle = FontStyle.Normal;
            //styleButton.alignment = TextAnchor.MiddleCenter;

            stylePanel = new GUIStyle();
            stylePanel.border = new RectOffset(6,6,6,6);
            stylePanel.normal.background = texPanel;

            styleBarName = new GUIStyle() { fixedHeight = 16, fixedWidth = 32 };
            styleBarName.normal.textColor = Color.white;
            styleBarName.alignment = TextAnchor.MiddleCenter;

            styleBarDef = new GUIStyle(GUI.skin.box);
            styleBarDef.border = new RectOffset(2,2,2,2);
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

            styleBarRateText = new GUIStyle(styleBarText);
            styleBarRateText.alignment = TextAnchor.MiddleRight;



            styleStageText = new GUIStyle(GUI.skin.label);
            styleStageText.normal.textColor = new Color(207, 207, 207);

            styleStageTextHead = new GUIStyle(styleStageText);
            styleStageTextHead.fontStyle = FontStyle.Bold;

            styleStageButton = new GUIStyle(styleButton);

            styleToggle = new GUIStyle(HighLogic.Skin.toggle);
            styleToggle.normal.textColor = new Color(207, 207, 207);


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

        


        public static Byte[] LoadFileToArray(String Filename)
        {
            Byte[] arrBytes;

            arrBytes = KSP.IO.File.ReadAllBytes<KSPAlternateResourcePanel>(Filename);

            return arrBytes;
        }

        public static void SaveFileFromArray(Byte[] data, String Filename)
        {
            KSP.IO.File.WriteAllBytes<KSPAlternateResourcePanel>(data, Filename);
        }


        public static void LoadImageIntoTexture(ref Texture2D tex, String FileName)
        {

            try
            {
                //DebugLogFormatted("Loading {0}", FileName);
                tex.LoadImage(LoadFileToArray(FileName));
            }
            catch (Exception)
            {
                DebugLogFormatted("Failed to load (are you missing a file):{0}", FileName);
            }
        }


        private void LoadSettings()
        {
            DebugLogFormatted("Loading Config...");
            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KSPAlternateResourcePanel>();
            configfile.load();

            blnStaging = configfile.GetValue<Boolean>("Staging", false);
            blnStagingInMapView = configfile.GetValue<Boolean>("StagingInMapView", false);
            blnStagingSpaceInMapView = configfile.GetValue<Boolean>("StagingSpaceInMapView", false);

            ToggleOn = configfile.GetValue<Boolean>("ToggleOn", false);

            DebugLogFormatted("Config Loaded Successfully");
        }

        private void SaveSettings()
        {
            DebugLogFormatted("Saving Config...");

            KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<KSPAlternateResourcePanel >();
            configfile.load();

            configfile.SetValue("Staging", blnStaging);
            configfile.SetValue("StagingInMapView", blnStagingInMapView);
            configfile.SetValue("StagingSpaceInMapView", blnStagingSpaceInMapView);

            configfile.SetValue("ToggleOn", ToggleOn);

            configfile.save();
            DebugLogFormatted("Saved Config");
        }
    }
}
