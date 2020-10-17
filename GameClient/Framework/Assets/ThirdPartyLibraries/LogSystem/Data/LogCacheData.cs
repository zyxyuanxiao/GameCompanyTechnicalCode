/********************************************************************
 Date: 2020-09-23
 Name: aaaa
 author:  zhuzizheng
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LogSystem
{
    public class LogCacheData  : ILoggerInterface
    {
        public bool IsShowGUI;             //是否显示 GUI
        public bool collapse;              //是否折叠日志
        public bool clearOnNewSceneLoaded; //是否要清除新加载场景的日志
        public bool showTime;              //是否展示 Log 输出的游戏时间
        public bool showScene;             //是否展示场景
        public bool showMemory;            //是否展示Log内存
        public bool showFps;               //是否展示 FPS
        public bool showGraph;             //是否展示渲染
        public bool showLog;               //是否展示 Log
        public bool showWarning;           //是否展示警告信息
        public bool showError;             //是否展示错误

        public int numOfLogs;                 //Log 的总条目
        public int numOfLogsWarning;          //log 警告的总条目
        public int numOfLogsError;            //log 错误的总条目
        public int numOfCollapsedLogs;        //log 折叠的总条目
        public int numOfCollapsedLogsWarning; //log 警告的折叠总条目

        public int numOfCollapsedLogsError; //log 错误的折叠总条目
        //public int maxAllowedLog = 1000 ;

        public Vector2 size = new Vector2(48, 48);

        public bool showClearOnNewSceneLoadedButton;
        public bool showTimeButton;
        public bool showSceneButton;
        public bool showMemButton;
        public bool showFpsButton;
        public bool showSearchText;
        public bool showCopyButton;
        public bool showSaveButton;

        public float logsMemUsage;                                  //日志使用的内存
        public float graphMemUsage;                                 //图使用的内存
        public float TotalMemUsage => logsMemUsage + graphMemUsage; //Logs 使用的总内存大小
        public float maxSize = 40;                                  //日志超过 40 M,就开始清空

        public LogEntity selectedLog; //选中的日志
        
        //设备相关
        public string deviceModel;
        public string deviceType;
        public string deviceName;
        public string graphicsMemorySize;
        public string maxTextureSize;
        public string systemMemorySize;

        //所有的场景
        public string[] allScenes;

        //当前的场景
        public string currentScene;

        //所有的 Mono 内存
        public float GCTotalMemory;

        //从 Unity 里面接收出来的 Log 最原始的 Log
        public List<LogEntity> threadedLogs;

        //包含所有未折叠的日志
        public List<LogEntity> logs;

        //包含所有折叠日志
        public List<LogEntity> collapsedLogs;

        //包含只对用户显示的日志，例如，如果您关闭显示日志+关闭显示警告，并且您的模式是崩溃，那么这个列表将只包含崩溃错误
        public List<LogEntity> currentLog;

        //用于检查新的日志是否已经存在或新建日志
        public LogMultiKeyDictionary<string, string, LogEntity> logsDic;

        //为了节省内存
        public Dictionary<string, string> cachedString;
        
        //收集样本
        public List<Sample> samples = null;

        
        //过滤的文字
        public string filterText;

        public void Initialize()
        {
            IsShowGUI             = false;
            collapse              = true;
            clearOnNewSceneLoaded = false;
            showTime              = false;
            showScene             = false;
            showMemory            = false;
            showFps               = false;
            showGraph             = false;
            showLog               = true;
            showWarning           = true;
            showError             = true;

            numOfLogs                 = 0;
            numOfLogsWarning          = 0;
            numOfLogsError            = 0;
            numOfCollapsedLogs        = 0;
            numOfCollapsedLogsWarning = 0;
            numOfCollapsedLogsError = 0;

            size = new Vector2(48, 48);

            showClearOnNewSceneLoadedButton = true;
            showTimeButton                  = true;
            showSceneButton                 = true;
            showMemButton                   = true;
            showFpsButton                   = true;
            showSearchText                  = true;
            showCopyButton                  = true;
            showSaveButton                  = true;


            deviceModel        = SystemInfo.deviceModel.ToString();
            deviceType         = SystemInfo.deviceType.ToString();
            deviceName         = SystemInfo.deviceName.ToString();
            graphicsMemorySize = SystemInfo.graphicsMemorySize.ToString();
            maxTextureSize     = SystemInfo.maxTextureSize.ToString();
            systemMemorySize   = SystemInfo.systemMemorySize.ToString();

            allScenes = new string[SceneManager.sceneCountInBuildSettings];
            currentScene = SceneManager.GetActiveScene().name;
            
            samples = new List<Sample>();
            
            threadedLogs = new List<LogEntity>();

            logs = new List<LogEntity>();

            collapsedLogs = new List<LogEntity>();

            currentLog = new List<LogEntity>();

            logsDic = new LogMultiKeyDictionary<string, string, LogEntity>();

            cachedString = new Dictionary<string, string>();

            filterText = string.Empty;
            
            SceneManager.sceneLoaded += SceneLoaded;

        }

        public void Update()
        {
            //当前 Mono 占用的内存
            GCTotalMemory = (((float) System.GC.GetTotalMemory(false)) / 1024 / 1024);

            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex != -1 && string.IsNullOrEmpty(allScenes[sceneIndex]))
                allScenes[SceneManager.GetActiveScene().buildIndex] = SceneManager.GetActiveScene().name;
        }
        
        public void UnInitialize()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        private void SceneLoaded(Scene _null1, LoadSceneMode _null2)
        {
            if (clearOnNewSceneLoaded)
                clear();
            currentScene = SceneManager.GetActiveScene().name;
            Debug.Log("场景: " + currentScene + " 已加载");
        }

        public void clear()
        {
            logs.Clear();
            collapsedLogs.Clear();
            currentLog.Clear();
            logsDic.Clear();
            //selectedIndex = -1;
            selectedLog               = null;
            numOfLogs                 = 0;
            numOfLogsWarning          = 0;
            numOfLogsError            = 0;
            numOfCollapsedLogs        = 0;
            numOfCollapsedLogsWarning = 0;
            numOfCollapsedLogsError   = 0;
            logsMemUsage              = 0;
            graphMemUsage             = 0;
            samples.Clear();
            System.GC.Collect();
        }

    }
}