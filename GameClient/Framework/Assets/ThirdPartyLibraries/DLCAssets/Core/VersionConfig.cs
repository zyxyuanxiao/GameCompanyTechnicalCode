using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace DLCAssets
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
    /// 
    /// </summary>
    public sealed class File_V_MD5
    {
        public string Version;
        public string MD5Hash;
    }
    
    public sealed class VersionConfig
    {
        public string OS = string.Empty;//操作平台
        public string SVNVersion = string.Empty;//SVN版本号
        public string AppVersion = string.Empty;//游戏版本号
        /// <summary>
        /// 名字:{版本,MD5},名字包含 AB 包的名字,以及 ZIP 文件的名字
        /// </summary>
        public Dictionary<string, File_V_MD5> FileInfos;
        
    }
}