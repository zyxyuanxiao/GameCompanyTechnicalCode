using UnityEngine;
using System;

namespace GameAssets
{
    public enum IAssetsNotificationType
    {
        Info = 0,//信息
        UIPopUpInfo,//弹窗正常信息
        UIPopUpError,//弹窗错误信息
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
        /// 广播本模块的信息
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="info"></param>
        public static void Broadcast(IAssetsNotificationType notificationType = IAssetsNotificationType.Info, string json = "")
        {
            AssetsMessageReceived?.Invoke(notificationType,json);
        }
    }
}