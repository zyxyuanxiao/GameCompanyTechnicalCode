using System.Collections;

/**
 * 热更流程
 * 1:读取资源配置表
 * 2:下载资源文件
 * 3:解压文件
 * 4:加载本地文件
 * 5:检查所有本地文件
 */


public sealed class ReadVersionConfig : IProcess
{
    public byte ID => 0;
    public ProcessLayer Layer => ProcessLayer.HotUpdate;
    public byte DelayFrame => 1;
    public IEnumerator Work()
    {
        yield return GameManager.OneFrame;
        this.isDone = false;
        
        this.isDone = true;
    }

    public bool isDone { get; set; }
    public void Reset()
    {
        this.isDone = false;
    }
}