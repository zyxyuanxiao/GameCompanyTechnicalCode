using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LogSystem
{
    public class LogManager : MonoBehaviour
    {
        #region 加载或者销毁LogObject

        /// <summary>
        /// 此对象会接收所有的 log 信息
        /// </summary>
        public const string LOG_GO_NAME = "LogObject";

        /// <summary>
        /// 加载SDKGameObject
        /// </summary>
        public static void Initialize()
        {
            GameObject go = new GameObject(LOG_GO_NAME);
            go.AddComponent<LogManager>();
            DontDestroyOnLoad(go);
        }

        /// <summary>
        /// 销毁LogManagerObject
        /// </summary>
        public static void UnInitialize()
        {
            if (Instance != null)
            {
                DestroyImmediate(Instance.gameObject, true);
            }
        }

        #endregion

        #region 日志管理者对象生命周期以及初始化

        private static LogManager m_instance;
        
        
        private static int         mainThreadId;
        public static bool IsMainThread
        {
            get { return System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId; }
        } 
        
        public static LogManager Instance //这个方法可能在子线程中调用,需要判断在子线程中不能使用 Unity 的 API
        {
            get
            {
                if (m_instance == null && IsMainThread) m_instance = FindObjectOfType<LogManager>();
                return m_instance;
            }
        }

        [HideInInspector] public List<ILoggerInterface> Services;

        private void Awake()
        {
            m_instance = this;
            mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;//主线程

            Services = new List<ILoggerInterface>();
            
            Services.Add(new LogCacheData());
            Services.Add(new LogFps());
            Services.Add(new LogReporter());
            Services.Add(new LogView());
            Services.Add(new LogFile());

            foreach (var service in Services)
            {
                service.Initialize();
            }
        }

        private void Start()
        {
            LogManager.GetLogFile().CheckFile();
        }

        private void OnEnable()
        {
            LogCacheData CacheData = LogManager.GetLogCacheData();
            if (CacheData.logs.Count == 0) CacheData.clear();
        }

        private void Update()
        {
            foreach (var service in Services)
            {
                service.Update();
            }
            
            LogCacheData CacheData = LogManager.GetLogCacheData();
            
            //画圆手势,当你画出圆圈,则展示出 Log 界面
            if (LogEvents.IsGestureDone())
            {
                CacheData.IsShowGUI = true;
            }
            
            if (CacheData.IsShowGUI) //当IsShowGUI为 false,并且手势画圆才可以展示出 GUI
            {
                CacheData.IsShowGUI = false;
                AddDrawGUI();
            }
        }
        

        private void OnDestroy()
        {
            foreach (var service in Services)
            {
                service.UnInitialize();
            }
            Services.Clear();
            Services = null;
        }

        /// <summary>
        /// 显示 Log GUI
        /// </summary>
        public void AddDrawGUI()
        {
            LogDrawGUIContainer logDrawGui = Instance.gameObject.GetComponent<LogDrawGUIContainer>();
            if (logDrawGui == null) logDrawGui = Instance.gameObject.AddComponent<LogDrawGUIContainer>();
        }

        /// <summary>
        /// 隐藏 Log GUI
        /// </summary>
        public void DestroyDrawGUI()
        {
            LogDrawGUIContainer logDrawGui = gameObject.GetComponent<LogDrawGUIContainer>();
            if (logDrawGui != null)
            {
                DestroyImmediate(logDrawGui,true);
            }
        }
        #endregion

        #region 获取各个模块的服务

        public static LogCacheData GetLogCacheData()
        {
            return (LogCacheData) Instance.Services[0];
        }
        public static LogFps GetLogFps()
        {
            return (LogFps) Instance.Services[1];
        }
        public static LogReporter GetLogReporter()
        {
            return (LogReporter) Instance.Services[2];
        }
        public static LogView GetLogView()
        {
            return (LogView) Instance.Services[3];
        }
        public static LogFile GetLogFile()
        {
            return (LogFile) Instance.Services[4];
        }

        #endregion
        
    }
}

