using System.Collections;

namespace GameAssets
{
    public class PrepareDatas : IBusiness
    {
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsConfig.OneFrame;
            //检查这个路径
            AssetsConfig.QueryLocalFilePath();
            
            
            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "<color=cyan>========================>LoadAllFiles 结束<========================</color>");
        }
    }
}