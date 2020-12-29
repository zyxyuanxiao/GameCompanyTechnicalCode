using UnityEngine;
using System.Collections.Generic;
using System;

public class LogModule 
{
    public enum LogModuleCode
    {
        ResourceSys = 1,
        AssetBundleStatus,
        ResRefStatus,
        AsyncLoadingProcessState,

        GDataLog,
    }

    public static LogModule Instance
    {
        get
        {
            if (_inst == null)
                _inst = new LogModule();
            return _inst;
        }
    }


    public void SetLogModuleStatus(int code, bool enable)
    {
        m_status[(LogModuleCode)code] = enable;
    }

    [LuaInterface.NoToLuaAttribute]
    public void SetLogModuleStatus(LogModuleCode code, bool enable)
    {
        m_status[code] = enable;
    }

    public void Trace(LogModuleCode code, string msg)
    {
        bool enable = false;
        if(m_status.TryGetValue(code, out enable))
        {
            if(enable)
                Debug.LogFormat("[{0}] [{1}]: {2}. \n {3}", DateTime.Now.ToString(), code.ToString(), msg, StackTraceUtility.ExtractStackTrace());
        }
        else
        {
            Debug.LogErrorFormat("Trace with inlalid LogModuleCode: {0}", code);
        }
    }
    
    private LogModule()
    {
        m_status = new Dictionary<LogModuleCode, bool>();
        string[] names = Enum.GetNames(typeof(LogModuleCode));
        foreach (var name in names)
        {
            LogModuleCode code = (LogModuleCode)Enum.Parse(typeof(LogModuleCode), name);
            m_status.Add(code, false);
        }
    }

    private static LogModule _inst;

    private Dictionary<LogModuleCode, bool> m_status;
}
