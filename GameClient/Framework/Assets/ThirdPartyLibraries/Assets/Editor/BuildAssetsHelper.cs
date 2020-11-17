using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BestHTTP;
using Common;
using LitJson;
using UnityEditor;
using UnityEngine;
using Tool = Common.Tool;

namespace GameAssets
{
    public static class BuildAssetsHelper
    {
        public static string DownloadAssetsDirectory
        {
            get
            {  
                string path = Application.dataPath.Replace("Assets", "DownloadAssets/") + 
                              Common.Tool.QueryPlatform() + "/";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        
        /// <summary>
        /// 初始化随包资源
        /// </summary>
        public static void BuildDefaultStreamingAssets()
        {
            BuildAllAssets();
        }

        /// <summary>
        /// 除了随包资源,后续生产的都是打包资源,为了增量更新资源
        /// 1. 打本地的最新版本包
        /// 2. 下载服务器上面的版本配置文件,下载完毕之后,这个配置文件表示老版的配置文件
        /// 3. 本地的版本配置文件对比老版本的配置文件,将变化的,不存在的上传至服务器,将服务器上面多余的进行删除
        /// </summary>
        public static void BuildHotUpdateAsset()
        {
            BuildAllAssets();
            //目前使用的是本地的 URL,具体到项目上面部署的情况下,再重新编写此类
            AssetsHelper.DownloadURL = Resources.Load<GameConfig>("Configs/GameConfig").QueryAddress()[0];
            string versionConfigURL = AssetsHelper.QueryDownloadFileURL(AssetsHelper.VersionConfigName);
            BestHttpHelper.GET(versionConfigURL, CheckVersionConfig);
        }
        
        /// <summary>
        /// 检查版本
        /// </summary>
        /// <param name="b"></param>
        /// <param name="s"></param>
        private static void CheckVersionConfig(bool b,string s)
        {
            if (!b || string.IsNullOrEmpty(s))
            {
                Debug.Log("资源服务器版本配置文件下载失败,说明本次直接使用全部打包,并将所有数据全部拷贝到服务器上面即可");
                BuilderMenuItems.CopyAssetBundlesToWebServer();
                return;
            }
            //资源服务器上面的版本配置文件
            Debug.Log("从资源服务器下载 VersionConfig.json 成功");
            VersionConfig remoteVersionConfig = JsonMapper.ToObject<VersionConfig>(s);
            
            string vcPath = DownloadAssetsDirectory + AssetsHelper.VersionConfigName;
            string localVC = File.ReadAllText(vcPath, Encoding.UTF8);
            VersionConfig localVersionConfig = JsonMapper.ToObject<VersionConfig>(localVC);
            
            //1. 判断版本配置的不同,以本地打包为准,
            //2. 找出不同的,不存在的,覆盖上传到服务器上面,版本配置文件也上传到服务器上面
            //3. 如果相同,则将服务器上面的单个下载文件的版本号覆盖本地文件
            List<string> copyFileNames = new List<string>();
            var localFileNames = new List<string>(localVersionConfig.FileInfos.Keys);
            foreach (string l_name in localFileNames)
            {
                File_V_MD5 localFileVMd5 = localVersionConfig.FileInfos[l_name];
                
                if (!remoteVersionConfig.FileInfos.TryGetValue(l_name,out File_V_MD5 remoteFileVMd5))
                {
                    copyFileNames.Add(l_name); //这个文件需要覆盖上传到服务器
                    continue;
                }

                if (remoteFileVMd5.MD5Hash != localFileVMd5.MD5Hash)
                {
                    Debug.Log("多少个 Hash 不同  " + l_name);
                    copyFileNames.Add(l_name); //这个文件需要覆盖上传到服务器
                }
                else
                {
                    //如果相同,则需要将版本调至与旧版本相同,不然每个客户端都会下载
                    localFileVMd5.Version = remoteFileVMd5.Version;
                }
            }

            File.WriteAllText(vcPath, JsonMapper.ToJson(localVersionConfig));
            copyFileNames.Add(AssetsHelper.VersionConfigName);
            //覆盖上传到服务器上面
            Debug.Log("覆盖上传");
            foreach (string name in copyFileNames)
            {
                Debug.Log("覆盖上传文件:"+ name);
                string localPath = DownloadAssetsDirectory + name;
                string remotePath = "";
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    remotePath = "/Library/WebServer/Documents/DownloadAssets/";
                }
                
                remotePath = remotePath + Common.Tool.QueryPlatform() + "/" + name;
                if (File.Exists(remotePath))File.Delete(remotePath);
                File.Copy(localPath,remotePath);
            }
            
            Debug.Log("覆盖上传完毕");
        }
        
        
        /// <summary>
        /// 打所有的包,最新的包,包括 ab 包与 app (执行程序)
        /// </summary>
        public static void BuildAllAssets()
        {
            //配置路径
            if (Directory.Exists(DownloadAssetsDirectory))
                Directory.Delete(DownloadAssetsDirectory, true);
            
            //根据配置文件进行初始化 AB 包的名字以及资源
            BuildAssetsConfig assetsConfig = BuildAssetsConfig.QueryAssetsConfig();
            assetsConfig.BuildAll();
            AssetDatabase.Refresh();
            
            //设置打包压缩格式
            const BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression |
                                                    BuildAssetBundleOptions.DisableWriteTypeTree |
                                                    BuildAssetBundleOptions.DeterministicAssetBundle;
            var targetPlatform = EditorUserBuildSettings.activeBuildTarget;
            //开始打包
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(
                DownloadAssetsDirectory, 
                assetsConfig.QueryAssetBundleBuilds(), 
                options, 
                targetPlatform);
            if (assetBundleManifest == null)
            {
                Debug.LogError("打包失败");
                return;
            }
            
            //输出版本配置文件
            BuildVersionConfig(assetBundleManifest);
            //将 AB 数据以及二进制数据,版本配置文件拷贝到StreamingAssets文件夹里面.
            CopyFileToStreamingAssets();
            
            EditorUtility.OpenWithDefaultApp(DownloadAssetsDirectory);
        }

        /// <summary>
        /// 根据打AB包生成的AssetBundleManifest,设置 VersionConfig
        /// </summary>
        private static void BuildVersionConfig(AssetBundleManifest assetBundleManifest)
        {
            //先设置游戏配置包,如果没有就直接获取 SVN 版本
            string version = Common.SVNHelper.GetSvnVersion();
            version = "2";
            Resources.Load<GameConfig>("Configs/GameConfig").version = version;//没有就屏蔽
            
            VersionConfig vc = new VersionConfig()
            {
                OS = Common.Tool.QueryPlatform(),
                SVNVersion = version,
                AppVersion = Application.version,
            };
            vc.FileInfos = new Dictionary<string, File_V_MD5>();

            string[] abNames = assetBundleManifest.GetAllAssetBundles();
            foreach (string name in abNames)
            {
                string abPath = DownloadAssetsDirectory + name;
                vc.FileInfos[name] = new File_V_MD5()
                    {Version = version, MD5Hash = Common.SecurityTools.GetMD5Hash(abPath)};
                
            }

            //将 assetBundleManifest 文件也装载进配置文件中
            string abm = DownloadAssetsDirectory + Tool.QueryPlatform();
            vc.FileInfos[Tool.QueryPlatform()] = new File_V_MD5()
                {Version = version, MD5Hash = Common.SecurityTools.GetMD5Hash(abm)};
            
            //将 zip 文件也装载进配置文件中
            string zip = DownloadAssetsDirectory + Tool.QueryPlatform();
            vc.FileInfos[FileFilter.AllText] = new File_V_MD5()
                {Version = version, MD5Hash = Common.SecurityTools.GetMD5Hash(zip)};
            
            string vcPath = DownloadAssetsDirectory + AssetsHelper.VersionConfigName;
            if (!File.Exists(vcPath))
            {
                using (File.Create(vcPath)) ;
            }
            // 这个后面不能加 UTF8,因为 Litjson 解析时报错
            File.WriteAllText(vcPath, JsonMapper.ToJson(vc));
        }

        
        /// <summary>
        /// 拷贝打包出的文件到游戏目录里面
        /// </summary>
        private static void CopyFileToStreamingAssets()
        {
            string[] files = Directory.GetFiles(DownloadAssetsDirectory, "*");
            string destFileName = AssetsHelper.CSharpFilePath(AssetsHelper.QueryStreamingFilePath());
            foreach (string file in files)
            {
                if (Path.GetExtension(file).ToLower().Contains("manifest"))
                {
                    continue;
                }
                File.Copy(file, destFileName + Path.GetFileName(file),true);
            }
        }

    }
}