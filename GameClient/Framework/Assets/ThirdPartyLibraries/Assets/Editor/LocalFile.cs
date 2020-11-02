using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameAssets
{
    /// <summary>
    /// 在本地Assets/的路径之后,游戏文件夹内的一些文件资源,被统一抽象化为  LocalFile
    /// </summary>
    [Serializable]
    public class LocalFile
    {
        [Tooltip("资源类型名名")]
        public string FileType = string.Empty;

        [Tooltip("搜索当前文件夹下的通配符")] 
        public string SearchPattern = string.Empty;

        [Tooltip("在本地Assets/的路径,从其中进行查找文件资源进行打热更包,初始化后需要自行设置")]
        public List<string> LocalFilePaths = new List<string>();

        //获取文件夹内的所有文件资源        
        public static List<string> QueryFilePath(string searchPattern)
        {
            string path = Application.dataPath.Replace("\\", "/") + "/" + FileFilter.BuildAsset + "/";
            string[] localFilePaths = Directory.GetFiles(path, searchPattern.ToLower(), SearchOption.AllDirectories);
            List<string> filePaths = new List<string>();
            foreach (string item in localFilePaths)
            {
                string temp = item.Replace("\\", "/");
                if (Directory.Exists(item)) continue; //如果是文件夹则跳出
                temp = item.Replace(Application.dataPath, "Assets");
                if (!FileFilter.QueryFileToAB(temp)) continue; //不通过则跳出
                var ext = Path.GetExtension(temp).ToLower();
                if ((ext == ".fbx" || ext == ".anim") && !temp.Contains(ext)) continue; //fbx或者anim都不打进游戏内
                filePaths.Add(temp.Replace("\\", "/"));
            }

            return filePaths;
        }

        public static Dictionary<string, string> QueryRules()
        {
            Dictionary<string, string> assetTypes = new Dictionary<string, string>();
            assetTypes.Add("Png", FileFilter.Png);
            assetTypes.Add("TTF", FileFilter.TTF);
            assetTypes.Add("Asset", FileFilter.Asset);
            assetTypes.Add("Hdr", FileFilter.Hdr);
            assetTypes.Add("Shader", FileFilter.Shader);
            assetTypes.Add("Controller", FileFilter.Controller);
            assetTypes.Add("Mat", FileFilter.Material);
            assetTypes.Add("Prefab", FileFilter.Prefab);
            assetTypes.Add("Unity", FileFilter.Unity);
            assetTypes.Add("Spriteatlas", FileFilter.Spriteatlas);
            return assetTypes;
        }

    }
}