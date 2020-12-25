using System.Collections.Generic;
using UnityEngine;
using GameAssets;

/// <summary>
/// 没有热更,就不需要这个模块的资源加载
/// 只需要把所有的东西都塞进 Resources/StreamingAssets 里面就行了,根本没有必要用到资源管理.
/// 目前采用的机制是将热更的代码常驻于游戏内存中,游戏期间也可能需要热更,边玩边下
/// 
/// 1. 从沙河空间里面读取配置文件,如果没有则从 StreamingAssets 里面读取,读取完毕;
/// 2. 再进行文件的存在判断,如果沙盒空间没有 ab 文件,则从 StreamingAssets 拷贝到沙盒空间,压缩文件直接解压缩
///    再进行文件信息的校验,如果本地文件校验不成功,就删除某个文件,或者从StreamingAssets里面进行 copy,或者从网络下载
/// 3. 从网络上面下载配置文件,如果相同则跳过,如果不同则记录缺省,不同的值进行替换,然后下载;
///    下载一个包,将版本配置文件,信息检查文件更新一次,防止网络直接被杀死,所有数据要从头开始下载的情况.不支持下载大文件的断点下载
/// 4. 解压压缩包到本地,再次将所有的文件信息输出到一个文件内,等待下次使用.如果期间出现了
/// </summary>
public sealed class HotUpdateManager : IManager
{
    private static List<IBusiness> huBusiness = new List<IBusiness>()
    {
        new ReadConfig(),
        new CheckAllFiles(),
        new DownloadAssets(),
        new PrepareBeginGame(),
    };
    
    public void Awake()
    {
        AssetsHelper.UpdatePath();//初始化文件夹
    }
    
    public void Start()
    {
        if (ConfigManager.GameConfig.StartHotUpdate)
        {
            GameManager.QueryManager<ProcessManager>().AddExecute(TaskProcessLayer.HotUpdate, hotUpdateEnd);
        }
        else
        {
            hotUpdateEnd();
        }
    }

    public void OnDestroy()
    {
        huBusiness.Clear();
        huBusiness = null;
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
