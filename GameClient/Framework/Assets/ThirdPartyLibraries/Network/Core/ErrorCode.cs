namespace Network
{
    /// <summary>
    /// 客户端自动义网络码,用来处理错误事件以及紧急事件
    /// </summary>
    public enum ErrorCode : byte
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        UnKnow = 0,

        /// <summary>
        /// 建立连接超时,不是接收数据包超时
        /// </summary>
        ConnectionTimeOut,
        
        /// <summary>
        /// 建立连接失败
        /// </summary>
        ConnectionFail,
        
        /// <summary>
        /// 连接过程中被迫断开连接
        /// </summary>
        BreakConnection,
        
        /// <summary>
        /// 发送包超时
        /// </summary>
        SendPackageTimeOut,
        
        /// <summary>
        /// 接收包超时
        /// </summary>
        ReceivePackageTimeOut,
    }
}