using System.Collections;
using GameAssets;

/**
 * 热更流程
 * 1:读取资源配置表
 * 2:下载资源文件,先下载 Version.json 配置文件,再对比本地的 Version.json 配置文件
 * 3:解压文件
 * 4:加载本地文件
 * 5:检查所有本地文件
 *
 * HUT : Hot Update Task 热更流程任务
 * 所有类以 HUT 开头
 */

//读取配置表
public sealed class HUT_ReadVersionConfig : ITaskProcess
{
    public TaskProcessLayer Layer => TaskProcessLayer.HotUpdate;
    public byte ID => 0;
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