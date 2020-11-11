using System.Collections;

namespace GameAssets
{
    /// <summary>
    /// 第一步 先下载远程配置文件 VersionConfig,
    /// 第二步 对比当前的配置文件与远程配置文件,那些包的 MD5 更新了,或者哪些包不存在,则传入下载队列.
    /// 循环下载队列,当下载完一个资源,将下载的这个资源生成MD5,再与远程配置文件的 MD5 对比,对比正常,则下载正确,否则重新传入下载队列.
    /// 每次下载正确时,将数据添加进本地的 VersionConfig 对象里面
    /// 第三步 全部下载结束时,将VersionConfig对象的数据,全部重新写入文件
    /// </summary>
    public class DownloadAssets : IBusiness
    {
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsConfig.OneFrame;
            
            
        }
    }
}