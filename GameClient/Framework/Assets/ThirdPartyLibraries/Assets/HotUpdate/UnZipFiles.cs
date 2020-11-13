using System.Collections;

namespace GameAssets
{
    public class UnZipFiles : IBusiness
    {
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsConfig.OneFrame;
            Progress = 0;
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginReadConfig,
                "开始读取 VersionConfig.json 并转成 VersionConfig 对象");
            
        }
    }
}