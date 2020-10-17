/********************************************************************
 Date: 2020-09-23
 Name: LogGUI
 author:  zhuzizheng
*********************************************************************/

using UnityEngine;

namespace LogSystem
{
    public class LogGUI
    {
        public Texture2D clearImage;           // clear.png
        public Texture2D collapseImage;        //collapse.png
        public Texture2D clearOnNewSceneImage; //clearOnSceneLoaded.png
        public Texture2D showTimeImage;        //time_1.png
        public Texture2D showSceneImage; //UnityIcon
        public Texture2D userImage; //user.png
        public Texture2D showMemoryImage; //memory.png
        public Texture2D softwareImage; //software.png
        public Texture2D dateImage;    //date.png
        public Texture2D showFpsImage; //fps.png
        public Texture2D infoImage;
        public Texture2D saveLogsImage;
        public Texture2D searchImage; //search.png
        public Texture2D copyImage; //copy.png
        public Texture2D closeImage; //close.png
        public Texture2D chartImage;

        public Texture2D buildFromImage;    //buildFrom.png
        public Texture2D systemInfoImage;   //ComputerIcon.png
        public Texture2D graphicsInfoImage; //graphicCard.png
        public Texture2D backImage; //back.png

        public Texture2D logImage; //log_icon.png
        public Texture2D warningImage; //warning_icon.png
        public Texture2D errorImage;   //error_icon.png

        public Texture2D barImage; //bar.png
        public Texture2D button_activeImage; //button_active.png
        public Texture2D even_logImage; //even_log.png
        public Texture2D odd_logImage;  //odd_log.png
        public Texture2D selectedImage;


        public Texture2D scroller_down_arraw;
        public Texture2D scroller_horizental_back;
        public Texture2D scroller_horizental_thumb;
        public Texture2D scroller_left_arraw;
        public Texture2D scroller_right_arraw;
        public Texture2D scroller_up_arraw;
        public Texture2D scroller_vertical_back;
        public Texture2D scroller_vertical_thumb;

        public GUISkin reporterScrollerSkin;

        // gui
        public GUIContent clearContent;
        public GUIContent collapseContent;
        public GUIContent clearOnNewSceneContent;
        public GUIContent showTimeContent;
        public GUIContent showSceneContent;
        public GUIContent userContent;
        public GUIContent showMemoryContent;
        public GUIContent softwareContent;
        public GUIContent dateContent;

        public GUIContent showFpsContent;

        public GUIContent graphContent;
        public GUIContent infoContent;
        public GUIContent saveLogsContent;
        public GUIContent searchContent;
        public GUIContent copyContent;
        public GUIContent closeContent;

        public GUIContent buildFromContent;
        public GUIContent systemInfoContent;
        public GUIContent graphicsInfoContent;
        public GUIContent backContent;

        //GUIContent cameraContent;

        public GUIContent logContent;
        public GUIContent warningContent;
        public GUIContent errorContent;
        public GUIStyle   barStyle;
        public GUIStyle   buttonActiveStyle;

        public GUIStyle nonStyle;
        public GUIStyle lowerLeftFontStyle;
        public GUIStyle backStyle;
        public GUIStyle evenLogStyle;
        public GUIStyle oddLogStyle;
        public GUIStyle logButtonStyle;
        public GUIStyle selectedLogStyle;
        public GUIStyle selectedLogFontStyle;
        public GUIStyle stackLabelStyle;
        public GUIStyle scrollerStyle;
        public GUIStyle searchStyle;
        public GUIStyle sliderBackStyle;
        public GUIStyle sliderThumbStyle;
        public GUISkin  toolbarScrollerSkin;
        public GUISkin  logScrollerSkin;
        public GUISkin  graphScrollerSkin;

        
        public Vector2 size = new Vector2(48, 48);
        
        public void LoadStyle()
        {
            reporterScrollerSkin = ScriptableObject.CreateInstance<GUISkin>();
            reporterScrollerSkin.font = Font.CreateDynamicFontFromOSFont(Font.GetOSInstalledFontNames()[0],25);
            reporterScrollerSkin.horizontalScrollbar.normal.background = LogIcon.scroller_horizental_backImage();
            reporterScrollerSkin.horizontalScrollbarThumb.normal.background = LogIcon.scroller_horizental_thumbImage();
            reporterScrollerSkin.horizontalScrollbarLeftButton.normal.background = LogIcon.scroller_left_arrawImage();
            reporterScrollerSkin.horizontalScrollbarRightButton.normal.background = LogIcon.scroller_right_arrawImage();
            reporterScrollerSkin.verticalScrollbar.normal.background = LogIcon.scroller_vertical_backImage();
            reporterScrollerSkin.verticalScrollbarThumb.normal.background = LogIcon.scroller_vertical_thumbImage();
            reporterScrollerSkin.verticalScrollbarUpButton.normal.background = LogIcon.scroller_up_arrawImage();
            reporterScrollerSkin.verticalScrollbarDownButton.normal.background = LogIcon.scroller_down_arrawImage();

            
            int paddingX = (int) (size.x * 0.2f);
            int paddingY = (int) (size.y * 0.2f);
            nonStyle                   = new GUIStyle();
            nonStyle.clipping          = TextClipping.Clip;
            nonStyle.border            = new RectOffset(0, 0, 0, 0);
            nonStyle.normal.background = null;
            nonStyle.fontSize          = (int) (size.y / 2);
            nonStyle.alignment         = TextAnchor.MiddleCenter;

            lowerLeftFontStyle                   = new GUIStyle();
            lowerLeftFontStyle.clipping          = TextClipping.Clip;
            lowerLeftFontStyle.border            = new RectOffset(0, 0, 0, 0);
            lowerLeftFontStyle.normal.background = null;
            lowerLeftFontStyle.fontSize          = (int) (size.y / 2);
            lowerLeftFontStyle.fontStyle         = FontStyle.Bold;
            lowerLeftFontStyle.alignment         = TextAnchor.LowerLeft;


            barStyle                   = new GUIStyle();
            barStyle.border            = new RectOffset(1, 1, 1, 1);
            barStyle.normal.background = barImage;
            barStyle.active.background = button_activeImage;
            barStyle.alignment         = TextAnchor.MiddleCenter;
            barStyle.margin            = new RectOffset(0, 0, 0, 0);

            //barStyle.padding = new RectOffset(paddingX,paddingX,paddingY,paddingY); 
            //barStyle.wordWrap = true ;
            barStyle.clipping = TextClipping.Clip;
            barStyle.fontSize = (int) (size.y);


            buttonActiveStyle                   = new GUIStyle();
            buttonActiveStyle.border            = new RectOffset(1, 1, 1, 1);
            buttonActiveStyle.normal.background = button_activeImage;
            buttonActiveStyle.alignment         = TextAnchor.MiddleCenter;
            buttonActiveStyle.margin            = new RectOffset(1, 1, 1, 1);
            //buttonActiveStyle.padding = new RectOffset(4,4,4,4);
            buttonActiveStyle.fontSize = (int) (size.y / 2);

            backStyle                   = new GUIStyle();
            backStyle.normal.background = even_logImage;
            backStyle.clipping          = TextClipping.Clip;
            backStyle.fontSize          = (int) (size.y / 2);

            evenLogStyle                   = new GUIStyle();
            evenLogStyle.normal.background = even_logImage;
            evenLogStyle.fixedHeight       = size.y;
            evenLogStyle.clipping          = TextClipping.Clip;
            evenLogStyle.alignment         = TextAnchor.UpperLeft;
            evenLogStyle.imagePosition     = ImagePosition.ImageLeft;
            evenLogStyle.fontSize          = (int) (size.y / 2);
            //evenLogStyle.wordWrap = true;

            oddLogStyle                   = new GUIStyle();
            oddLogStyle.normal.background = odd_logImage;
            oddLogStyle.fixedHeight       = size.y;
            oddLogStyle.clipping          = TextClipping.Clip;
            oddLogStyle.alignment         = TextAnchor.UpperLeft;
            oddLogStyle.imagePosition     = ImagePosition.ImageLeft;
            oddLogStyle.fontSize          = (int) (size.y / 2);
            //oddLogStyle.wordWrap = true ;

            logButtonStyle = new GUIStyle();
            //logButtonStyle.wordWrap = true;
            logButtonStyle.fixedHeight = size.y;
            logButtonStyle.clipping    = TextClipping.Clip;
            logButtonStyle.alignment   = TextAnchor.UpperLeft;
            //logButtonStyle.imagePosition = ImagePosition.ImageLeft ;
            //logButtonStyle.wordWrap = true;
            logButtonStyle.fontSize = (int) (size.y / 2);
            logButtonStyle.padding  = new RectOffset(paddingX, paddingX, paddingY, paddingY);

            selectedLogStyle                   = new GUIStyle();
            selectedLogStyle.normal.background = selectedImage;
            selectedLogStyle.fixedHeight       = size.y;
            selectedLogStyle.clipping          = TextClipping.Clip;
            selectedLogStyle.alignment         = TextAnchor.UpperLeft;
            selectedLogStyle.normal.textColor  = Color.white;
            //selectedLogStyle.wordWrap = true;
            selectedLogStyle.fontSize = (int) (size.y / 2);

            selectedLogFontStyle                   = new GUIStyle();
            selectedLogFontStyle.normal.background = selectedImage;
            selectedLogFontStyle.fixedHeight       = size.y;
            selectedLogFontStyle.clipping          = TextClipping.Clip;
            selectedLogFontStyle.alignment         = TextAnchor.UpperLeft;
            selectedLogFontStyle.normal.textColor  = Color.white;
            //selectedLogStyle.wordWrap = true;
            selectedLogFontStyle.fontSize = (int) (size.y / 2);
            selectedLogFontStyle.padding  = new RectOffset(paddingX, paddingX, paddingY, paddingY);

            stackLabelStyle          = new GUIStyle();
            stackLabelStyle.wordWrap = true;
            stackLabelStyle.fontSize = (int) (size.y / 2);
            stackLabelStyle.padding  = new RectOffset(paddingX, paddingX, paddingY, paddingY);

            scrollerStyle                   = new GUIStyle();
            scrollerStyle.normal.background = barImage;

            searchStyle           = new GUIStyle();
            searchStyle.clipping  = TextClipping.Clip;
            searchStyle.alignment = TextAnchor.LowerCenter;
            searchStyle.fontSize  = (int) (size.y / 2);
            searchStyle.wordWrap  = true;


            sliderBackStyle                   = new GUIStyle();
            sliderBackStyle.normal.background = barImage;
            sliderBackStyle.fixedHeight       = size.y;
            sliderBackStyle.border            = new RectOffset(1, 1, 1, 1);

            sliderThumbStyle                   = new GUIStyle();
            sliderThumbStyle.normal.background = selectedImage;
            sliderThumbStyle.fixedWidth        = size.x;

            GUISkin skin = reporterScrollerSkin;

            toolbarScrollerSkin                                      = (GUISkin) GameObject.Instantiate(skin);
            toolbarScrollerSkin.verticalScrollbar.fixedWidth         = 0f;
            toolbarScrollerSkin.horizontalScrollbar.fixedHeight      = 0f;
            toolbarScrollerSkin.verticalScrollbarThumb.fixedWidth    = 0f;
            toolbarScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

            logScrollerSkin                                      = (GUISkin) GameObject.Instantiate(skin);
            logScrollerSkin.verticalScrollbar.fixedWidth         = size.x * 2f;
            logScrollerSkin.horizontalScrollbar.fixedHeight      = 0f;
            logScrollerSkin.verticalScrollbarThumb.fixedWidth    = size.x * 2f;
            logScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

            graphScrollerSkin                                      = (GUISkin) GameObject.Instantiate(skin);
            graphScrollerSkin.verticalScrollbar.fixedWidth         = 0f;
            graphScrollerSkin.horizontalScrollbar.fixedHeight      = size.x * 2f;
            graphScrollerSkin.verticalScrollbarThumb.fixedWidth    = 0f;
            graphScrollerSkin.horizontalScrollbarThumb.fixedHeight = size.x * 2f;
            //inGameLogsScrollerSkin.verticalScrollbarThumb.fixedWidth = size.x * 2;
            //inGameLogsScrollerSkin.verticalScrollbar.fixedWidth = size.x * 2;
        }

        public void LoadGUIContent()
        {
            clearContent    = new GUIContent("", clearImage,    "清除 Clear logs");
            collapseContent = new GUIContent("", collapseImage, "折叠 Collapse logs");
            clearOnNewSceneContent =
                new GUIContent("", clearOnNewSceneImage, "跳场景清除 Clear logs on new scene loaded");
            showTimeContent   = new GUIContent("", showTimeImage,   "显示或者隐藏游戏时间 Show Hide Time");
            showSceneContent  = new GUIContent("", showSceneImage,  "显示或者隐藏场景 Show Hide Scene");
            showMemoryContent = new GUIContent("", showMemoryImage, "显示或者隐藏Log占用的内存 Show Hide Memory");
            softwareContent   = new GUIContent("", softwareImage,   "Software");
            dateContent       = new GUIContent("", dateImage,       "Date");
            showFpsContent    = new GUIContent("", showFpsImage,    "Show Hide fps");
            infoContent = new GUIContent("", infoImage, "Information about application");
            graphContent = new GUIContent("", chartImage, "Information about application");
            saveLogsContent   = new GUIContent("", saveLogsImage,   "保存 Save logs to device");
            searchContent     = new GUIContent("", searchImage,     "搜索 Search for logs");
            copyContent       = new GUIContent("", copyImage,       "拷贝 Copy log to clipboard");
            closeContent      = new GUIContent("", closeImage,      "关闭 Hide logs");
            userContent       = new GUIContent("", userImage,       "User");

            buildFromContent    = new GUIContent("", buildFromImage,    "Build From");
            systemInfoContent   = new GUIContent("", systemInfoImage,   "System Info");
            graphicsInfoContent = new GUIContent("", graphicsInfoImage, "Graphics Info");
            backContent         = new GUIContent("", backImage,         "Back");


            //snapshotContent = new GUIContent("",images.cameraImage,"show or hide logs");
            logContent     = new GUIContent("", logImage,     "show or hide logs");
            warningContent = new GUIContent("", warningImage, "show or hide warnings");
            errorContent   = new GUIContent("", errorImage,   "show or hide errors");
        }

        public void LoadImages()
        {
            clearImage           = LogIcon.clearImage();
            collapseImage        = LogIcon.collapseImage();
            clearOnNewSceneImage = LogIcon.clearOnSceneLoadedImage();
            showTimeImage        = LogIcon.timer_1Image();
            showSceneImage       = LogIcon.UnityIconImage();
            userImage            = LogIcon.userImage();
            showMemoryImage      = LogIcon.memoryImage();
            softwareImage        = LogIcon.softwareImage();
            dateImage            = LogIcon.dateImage();
            showFpsImage         = LogIcon.fpsImage();
            infoImage            = LogIcon.infoImage();
            saveLogsImage        = LogIcon.SaveImage();
            searchImage          = LogIcon.searchImage();
            copyImage            = LogIcon.copyImage();
            closeImage           = LogIcon.closeImage();

            chartImage = LogIcon.chartImage();

            buildFromImage    = LogIcon.buildFromImage();
            systemInfoImage   = LogIcon.ComputerIconImage();
            graphicsInfoImage = LogIcon.graphicCardImage();
            backImage         = LogIcon.backImage();

            logImage     = LogIcon.log_iconImage();
            warningImage = LogIcon.warning_iconImage();
            errorImage   = LogIcon.error_iconImage();

            barImage           = LogIcon.barImage();
            button_activeImage = LogIcon.button_activeImage();
            even_logImage      = LogIcon.even_logImage();
            odd_logImage       = LogIcon.odd_logImage();
            selectedImage      = LogIcon.selectedImage();

            scroller_down_arraw       = LogIcon.scroller_down_arrawImage();
            scroller_horizental_back  = LogIcon.scroller_horizental_backImage();
            scroller_horizental_thumb = LogIcon.scroller_horizental_thumbImage();
            scroller_left_arraw       = LogIcon.scroller_left_arrawImage();
            scroller_right_arraw      = LogIcon.scroller_right_arrawImage();
            scroller_up_arraw         = LogIcon.scroller_up_arrawImage();
            scroller_vertical_back    = LogIcon.scroller_vertical_backImage();
            scroller_vertical_thumb   = LogIcon.scroller_vertical_thumbImage();
        }
    }
}