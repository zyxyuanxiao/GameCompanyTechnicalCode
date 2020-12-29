using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common;
using LitJson;
using UnityEngine;

namespace DLCAssets
{
    /// <summary>
    /// 路径,后缀,配置,辅助
    /// </summary>
    public static class AssetsHelper
    {
        /// <summary>
        /// 一帧
        /// </summary>
        public static WaitForEndOfFrame OneFrame = new WaitForEndOfFrame();
        
        /// <summary>
        /// AssetBundle 的后缀名
        /// </summary>
        public static readonly string Extension = ".unity3d";
        
        /// <summary>
        /// 下载VersionConfig的 URL,下载完毕之后,有一系列数据,包括VersionConfig对象文本,需要下载的资源URL配置
        /// </summary>
        public static string DownloadURL = string.Empty;

        public static string ASSETROOT_LASTWRITETIME_KEY = Tool.QueryPlatform() + "_ASSETROOT_LASTWRITETIME_KEY";

        
        public static void UpdatePath()
        {
            string path = Application.persistentDataPath + "/" + Tool.QueryPlatform();
            //创建沙盒空间的文件夹
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        #region File Directory Helper

        /// <summary>
        /// 文件是否存在,主要是使用了 c#的 API,要去掉 UNITY 的 API 需要的头 file://
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FileExists(string path)
        {
            path = CSharpFilePath(path);
            if (File.Exists(path)) return true;
            return false;
        }
        
        /// <summary>
        /// 文件夹是否存在,主要是使用了 c#的 API,要去掉 UNITY 的 API 需要的头 file://
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DirectoryExists(string path)
        {
            path = CSharpFilePath(path);
            if (Directory.Exists(path)) return true;
            return false;
        }
        
        /// <summary>
        /// 文件删除,主要是使用了 c#的 API,要去掉 UNITY 的 API 需要的头 file://
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void FileDelete(string path)
        {
            path = CSharpFilePath(path);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        
        /// <summary>
        /// 文件删除,主要是使用了 c#的 API,要去掉 UNITY 的 API 需要的头 file://
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void FileMove(string sourceFileName, string destFileName)
        {
            sourceFileName = CSharpFilePath(sourceFileName);
            destFileName = CSharpFilePath(destFileName);
            if (File.Exists(destFileName))File.Delete(destFileName);
            File.Move(sourceFileName,destFileName);
        }

        public static string CSharpFilePath(string path)
        {
            return path.Replace(UnityFilePath(), "");
        }
        
                
        /// <summary>
        /// 这个方法是为了 UNITY 的 API 而设计,C#原生的 API 是不需要这个头的
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string UnityFilePath(string path = "")
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    return "file://" + path;
                case RuntimePlatform.OSXPlayer :
                    return "file://" + path;
                case RuntimePlatform.OSXEditor:
                    return "file://" + path;
                case RuntimePlatform.WindowsPlayer:
                    return "file://" + path;
                case RuntimePlatform.WindowsEditor:
                    return "file://" + path;
                default:
                    return path;
            }
        }
        
        #endregion
        
        
        // /// <summary>
        // /// 查找 AB 的路径
        // /// </summary>
        // /// <param name="path"></param>
        // /// <returns></returns>
        // public static string QueryLocalABPath(string path = "")
        // {
        //     string lp = LOCAL_PATH_TO_URL(Application.persistentDataPath.Replace("\\", "/")) + "/" +
        //                 QueryPlatform() + "/" +
        //                 "AssetBundles" + "/" +
        //                 path;
        //     if (File.Exists(lp))return lp;//如果在沙盒空间下查找到了此文件,则直接返回
        //     
        //     string sp = LOCAL_PATH_TO_URL(Application.streamingAssetsPath.Replace("\\", "/")) + "/" +
        //                 QueryPlatform() + "/" +
        //                 "AssetBundles" + "/" +
        //                 path;
        //     if (File.Exists(lp))return lp;//如果在沙盒空间下没有查找到此文件,则在StreamingAssets路径下查找,查找到了就返回
        //     
        //     return lp;//如果这 2 个地方都没有查找到,则返回沙盒空间下的路径
        // }

        /// <summary>
        /// 远程文件的配置路径,需要拼接,具体的拼接方式需要视具体情况而定
        /// 一般情况下,打包的所有文件全部放入一个文件夹中,不分子文件夹,所以此网络 URL 可以正常下载
        /// </summary>
        public static string QueryDownloadFileURL(string fileName)
        {
            return DownloadURL + "/" + Tool.QueryPlatform() + "/" + fileName;
        }
        
        /// <summary>
        /// 本地文件的下载路径,下载的文件,需要从这个地方拷贝到其他地方
        /// </summary>
        public static string QueryDownloadFilePath(string abName)
        {
            string path = CSharpFilePath(QueryLocalFilePath()) + "Download/";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path + abName;
        }

        /// <summary>
        /// 每个APP的本地 沙盒空间 + 平台路径
        /// </summary>
        /// <returns></returns>
        public static string QueryLocalFilePath(string path = "")
        {
            return UnityFilePath(Application.persistentDataPath.Replace("\\", "/")) + "/" +
                   Tool.QueryPlatform() +"/" +
                   path;
        }
        
        /// <summary>
        /// streamingAssetsPath 的路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string QueryStreamingFilePath(string path = "")
        {
            return UnityFilePath(Application.streamingAssetsPath.Replace("\\", "/")) + "/" +
                   Tool.QueryPlatform() + "/" +
                   path;
        }
        
        #region Zip
        
        public static void DecompressBinary(string zipPath)
        {
            //每次都将压缩文件解压一遍,防止
            zipPath = CSharpFilePath(zipPath);
            if (Path.GetExtension(zipPath).ToLower().Contains("zip"))//如果是 zip 则需要解压
            {
                ZipResult zipResult = new ZipResult();
                string targetPath = CSharpFilePath(QueryLocalFilePath());
                LZ4Helper.Decompress(zipPath,targetPath,ref  zipResult,true); 
            }
        }
        

        #endregion
        
        #region Config
                
        /// <summary>
        /// 从本地加载的VersionConfig数据,全局使用这一份即可
        /// </summary>
        public static VersionConfig VersionConfig;
        
        /// <summary>
        /// 本次打包的配置版本文件,
        ///
        /// 编辑器下运行,路径是:Assets/StreamingAssets/VersionConfig.json,
        ///
        /// 如果此文件随整包打出去,是需要解压到 APP 下的 Application.persistentDataPath 文件夹下,然后再进行读取
        /// 如果是热更包,是跟随热更情况,先下载此热更配置文件,然后根据热更配置文件进行下载 AB 包
        /// 
        /// </summary>
        public static readonly string VersionConfigName = "VersionConfig.json";
        
        /// <summary>
        /// 此文件信息必要进行加密与解密操作
        /// </summary>
        public static Dictionary<string,FileInfoConfig> FileInfoConfigs;
        
        public static readonly string FileInfoConfigName = "FileInfoConfig.json";

        public static void WriteVersionConfigToFile()
        {
            UpdatePath();
            string path = CSharpFilePath(QueryLocalFilePath(VersionConfigName));
            File.WriteAllText(path,JsonMapper.ToJson(VersionConfig));
        }
        public static void WriteFileInfoConfigsToFile()
        {
            if (FileInfoConfigs == null || FileInfoConfigs.Count<=0) return;
            UpdatePath();
            string path = CSharpFilePath(QueryLocalFilePath(FileInfoConfigName));
            File.WriteAllText(path,JsonMapper.ToJson(FileInfoConfigs));
        }
        #endregion
    }
}