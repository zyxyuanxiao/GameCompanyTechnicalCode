using System.Collections;
using GameAssets;

/**
 * 热更流程
 * 1:读取资源配置表
 * 2:下载资源文件
 * 3:解压文件
 * 4:加载本地文件
 * 5:检查所有本地文件
 *
 * HPT : HotUpdate Process Task 热更流程任务
 * 所有类以 PTH 开头
 */

//读取配置表
public sealed class HPT_ReadVersionConfig : IProcess
{
    public byte ID => 0;
    public ProcessLayer Layer => ProcessLayer.HotUpdate;
    public byte DelayFrame => 1;
    public IEnumerator Work()
    {
        yield return GameManager.OneFrame;
        this.IsDone = false;
        yield return  HotUpdate.QueryBusiness(ID).Work();
        this.IsDone = true;
    }

    public bool IsDone { get; set; }
    public void Reset()
    {
        this.IsDone = false;
    }
}

//下载资源
public sealed class HPT_DownloadAssets : IProcess
{
    public byte ID => 1;
    public ProcessLayer Layer => ProcessLayer.HotUpdate;
    public byte DelayFrame => 1;
    public IEnumerator Work()
    {
        yield return GameManager.OneFrame;
        this.IsDone = false;
        yield return  HotUpdate.QueryBusiness(ID).Work();
        this.IsDone = true;
    }

    public bool IsDone { get; set; }
    public void Reset()
    {
        this.IsDone = false;
    }
}

//解压文件
public sealed class HPT_UnZipFiles : IProcess
{
    public byte ID => 2;
    public ProcessLayer Layer => ProcessLayer.HotUpdate;
    public byte DelayFrame => 1;
    public IEnumerator Work()
    {
        yield return GameManager.OneFrame;
        this.IsDone = false;
        yield return  HotUpdate.QueryBusiness(ID).Work();
        this.IsDone = true;
    }

    public bool IsDone { get; set; }
    public void Reset()
    {
        this.IsDone = false;
    }
}

//加载本地 AB 包
public sealed class HPT_LoadLocalAB : IProcess
{
    public byte ID => 3;
    public ProcessLayer Layer => ProcessLayer.HotUpdate;
    public byte DelayFrame => 1;
    public IEnumerator Work()
    {
        yield return GameManager.OneFrame;
        this.IsDone = false;
        yield return  HotUpdate.QueryBusiness(ID).Work();
        this.IsDone = true;
    }

    public bool IsDone { get; set; }
    public void Reset()
    {
        this.IsDone = false;
    }
}

//检查文件
public sealed class HPT_CheckFiles : IProcess
{
    public byte ID => 4;
    public ProcessLayer Layer => ProcessLayer.HotUpdate;
    public byte DelayFrame => 1;
    public IEnumerator Work()
    {
        yield return GameManager.OneFrame;
        this.IsDone = false;
        yield return  HotUpdate.QueryBusiness(ID).Work();
        this.IsDone = true;
    }

    public bool IsDone { get; set; }
    public void Reset()
    {
        this.IsDone = false;
    }
}