using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace GameAssets
{
    // public class ABInfo
    // {
    //     public string Name = string.Empty;
    //     public string Md5Hash = string.Empty;
    //     public int Length = 0; //单位B
    //     public int UpdateType = 0; //更新方式，1：强制更新，2：弱更新
    //     public int ForceQuit = 0; //是否强制退出 0:非强制 1:踢用户下线 2:不踢用户下线
    //     public string Version = string.Empty; //版本号
    //     
    // }
    
    /// <summary>
    /// AB包的版本以及 MD5
    /// </summary>
    public class AB_V_MD5
    {
        public string Version;
        public string Md5Hash;
    }
    
    public class VersionConfig
    {
        public string Md5Hash = string.Empty;//本身文件的 MD5
        public string OS = string.Empty;//操作平台
        public string SVNVersion = string.Empty;//SVN版本号
        public string AppVersion = string.Empty;//游戏版本号
        /// <summary>
        /// AB包 在 APP 内的路径对应的版本号以及 MD5,不是在本地的路径
        /// </summary>
        public Dictionary<string, AB_V_MD5> ABInfos;

    }
}