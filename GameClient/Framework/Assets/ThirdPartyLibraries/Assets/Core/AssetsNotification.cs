using UnityEngine;
using System;
using LitJson;

namespace GameAssets
{
    public enum IAssetsNotificationType
    {
        None = 0,
        Info,//信息

        BeginReadConfig,//开始读取配置文件
        ReadConfigFailed,//读取配置失败了
        ReadConfigSucceed,//读取配置成功了
        
        BeginRequestVersionConfig,//请求版本配置文件
        RequestVersionConfigFailed,//请求版本配置文件
        RequestVersionConfigSucceed,//请求版本配置文件
        
        BeginDownloadFile,//开始下载所有 AB
        DownloadFileFailed,//下载一个 AB失败
        DownloadFileSucceed,//下载 所有AB成功
        
        BeginUnZipFiles,//开始解压所有被压缩的文件
        UnZipFilesFailed,//解压缩失败
        UnZipFilesSucceed,//解压缩所有文件成功
    }
    
    /// <summary>
    /// 本模块与其他模块的交互
    /// 以通知的方式进行,如果还需要其他类型的信息,在这个地方进行扩容
    /// </summary>
    public static class AssetsNotification
    {
        public delegate void MessageCallback(IAssetsNotificationType notificationType, string json);
        
        /// <summary>
        /// UI 模块与,资源管理模块都需要清晰的知道当前在本模块内出现的问题
        /// </summary>
        public static event MessageCallback AssetsMessageReceived;
        
        /// <summary>
        /// 发送通知的对象
        /// </summary>
        private static JsonData jsonData = new JsonData();

        /// <summary>
        /// 广播本模块的信息
        /// json = {"message":"信息"}
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="s"></param>
        public static void Broadcast(IAssetsNotificationType notificationType = IAssetsNotificationType.None, string s = "")
        {
            jsonData["message"] = s;
            AssetsMessageReceived?.Invoke(notificationType,jsonData.ToJson());
        }
    }
}