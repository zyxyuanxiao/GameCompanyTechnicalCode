using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameAssets
{
    public class UnZipFiles : IBusiness
    {
        public int Progress { get; set; }
        public IEnumerator Work()
        {
            yield return AssetsConfig.OneFrame;
            Progress = 0;
            AssetsNotification.Broadcast(IAssetsNotificationType.BeginUnZipFiles,
                "开始解压缩文件");
            Dictionary<string, File_V_MD5> zipFiles = null;
            if (AssetsConfig.VersionConfig.FileInfos!=null  && AssetsConfig.VersionConfig.FileInfos.Count > 0)
            {
                zipFiles = new Dictionary<string,File_V_MD5>();
                foreach (var item in AssetsConfig.VersionConfig.FileInfos)
                {
                    if (Path.GetExtension(item.Key).ToLower().Contains("zip"))
                    {
                        zipFiles.Add(item.Key,item.Value);
                    }
                }
            }

            yield return AssetsConfig.OneFrame;
            if (zipFiles !=null && zipFiles.Count>0)
            {
                ZipResult zipResult = new ZipResult();
                foreach (var item in zipFiles)
                {
                    string zipPath = AssetsConfig.QueryDownloadFilePath(item.Key);//路径
                    string targetPath = AssetsConfig.QueryLocalFilePath();
                    LZ4Helper.Decompress(zipPath,targetPath ,ref zipResult,true);
                    yield return AssetsConfig.OneFrame;
                }
            }

            AssetsNotification.Broadcast(IAssetsNotificationType.UnZipFilesSucceed,
                "解压缩文件全部完成");
            //全部解压完成
            yield return AssetsConfig.OneFrame;
            
            AssetsNotification.Broadcast(IAssetsNotificationType.Info,
                "<color=cyan>========================>UnZipFiles 结束<========================</color>");
        }
    }
}