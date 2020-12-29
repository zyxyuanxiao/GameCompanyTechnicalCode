using System.Collections;

namespace DLCAssets
{
    /* *
     * 热更流程,热更过后启动 Lua,启动登录界面
     * 1:读取资源配置表
     * 2:下载资源文件,先下载 Version.json 配置文件,再对比本地的 Version.json 配置文件
     * 3:解压文件
     * 4:检查所有本地文件
     * 5:准备数据,开始游戏
     * 
     * */
    public interface IBusiness
    {
        int Progress { get; set; }
        IEnumerator Work();
    }
}