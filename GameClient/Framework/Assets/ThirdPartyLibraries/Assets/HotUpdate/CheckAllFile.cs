using System.Collections;
using Common;
using UnityEngine;

namespace GameAssets
{
    public class CheckAllFile : IBusiness
    {
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsConfig.OneFrame;
            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "检查所有文件最后写入的时间");
            string path = AssetsConfig.CSharpFilePath(AssetsConfig.QueryLocalFilePath());
            string s = Tool.QueryAppendDirectoryLastWriteTime(path);
            string sha1 = Tool.QuerySHA1HashOfString(s);
            PlayerPrefs.SetString(AssetsConfig.ASSETROOT_LASTWRITETIME_KEY, sha1);
            
            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "<color=cyan>========================>CheckAllFile 结束<========================</color>");
        }
    }
}