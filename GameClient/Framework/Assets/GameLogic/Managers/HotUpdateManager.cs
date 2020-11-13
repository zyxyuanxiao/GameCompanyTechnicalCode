using System.Collections.Generic;
using UnityEngine;
using GameAssets;

/// <summary>
/// 没有热更,不需要这个模块的资源加载
/// 只需要把所有的东西都塞进 Resources/StreamingAssets 里面就行了,根本没有必要用到资源管理.
/// 
/// </summary>
public class HotUpdateManager : IManager
{
    private static List<IBusiness> huBusiness = new List<IBusiness>()
    {
        new ReadConfig(),
        new DownloadAssets(),
        new UnZipFiles(),
        new LoadAllFiles(),
        new CheckAllFile()
    };
    
    public void Awake()
    {

    }
    
    public void Start()
    {
        GameManager.QueryManager<ProcessManager>().AddExecute(TaskProcessLayer.HotUpdate, hotUpdateEnd);
    }

    public void OnDestroy()
    {

    }
    
    
    public static IBusiness QueryBusiness(int index)
    {
        return huBusiness[index];
    }

    
    private void hotUpdateEnd()
    {
        Debug.Log("<color=green>热更流程结束</color>");
    }
}
