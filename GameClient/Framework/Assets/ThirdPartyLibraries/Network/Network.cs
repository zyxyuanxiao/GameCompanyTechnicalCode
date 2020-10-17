using System.Collections.Generic;
using System.Net;

namespace Network
{
    public sealed class Network
    {
        public readonly Dictionary<long, NChannel> Sessions = new Dictionary<long, NChannel>();

        public NetworkChannelType NetworkChannelType;

        public NetworkProtocol NetworkProtocol;

        public IPEndPoint RemoteAddress;

        private NService service;
        /// <summary>
        /// 初始化
        /// </summary>
        public Network(NetworkProtocol protocol = NetworkProtocol.TCP, NetworkChannelType type = NetworkChannelType.Connect)
        {
            NetThread.Run(ThreadUpdate);
            if (protocol == NetworkProtocol.TCP)
            {
                this.service = new TCPService();
            }
            else
            {
                this.service = new KCPService();
            }
        }

        /// <summary>
        /// 子线程处理,按照1秒30帧来进行更新
        /// 这个线程更新的方法里面使用的数据结构是需要带有线程安全的(ConcurrentQueue)
        /// </summary>
        private void ThreadUpdate()
        {
            //OneThreadSynchronizationContext.Instance.Post();//发送消息
        }

        public void Dispose()
        {
            NetThread.Abort();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void Send()
        {

        }
    }
}
