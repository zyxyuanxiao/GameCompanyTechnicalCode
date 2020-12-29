using System.Collections;
using System.Collections.Generic;

namespace GameAssets
{
    public sealed class PrepareBeginGame  : IBusiness
    {
        public int Progress { get; set; }

        /// <summary>
        /// 读本地数据生成一个 VersionConfig 对象
        /// </summary>
        public IEnumerator Work()
        {
            Progress = 0;

            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "热更最后阶段,准备开始游戏");
            AssetsHelper.WriteVersionConfigToFile();
            AssetsHelper.WriteFileInfoConfigsToFile();
            yield return AssetsHelper.OneFrame;
            Progress = 100;

            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "热更结束,展示登录界面");
        }
    }
}