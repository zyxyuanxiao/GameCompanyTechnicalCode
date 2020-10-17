using System.Net;

namespace Network
{
    public enum NetworkProtocol
    {
        KCP,
        TCP,
    }

    /// <summary>
    /// 一个服务,管理多个频道.比如TCP服务,会有多个连接频道
    /// 需要有连接,等待接收,断开,等多个抽象函数
    /// </summary>
    public abstract class NService
    {
        /// <summary>
        /// 根据 Ip,port 等,进行连接服务器
        /// </summary>
        /// <param name="iPEndPoint"></param>
        /// <returns></returns>
        public abstract NChannel AddConnectChannel(IPEndPoint iPEndPoint);

        /// <summary>
        /// 根据频道Id进行查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract NChannel Query(long id);

        /// <summary>
        /// 根据频道ID进行删除
        /// </summary>
        /// <param name="id"></param>
        public abstract void Remove(long id);

        protected abstract void UpdateAccept();

        public abstract void Update();

        public abstract void Dispose();
    }
}
