using System.IO;
using System.Net;
using Common;

namespace Network
{
    public enum NetworkChannelType
    {
        Connect,
        Accept,
    }

    public abstract class NChannel
    {
        public NetworkChannelType NCType { get; protected set; }

        /// <summary>
        /// 当前频道的ID
        /// </summary>
        public long Id { get; protected set; }

        /// <summary>
        /// 当前频道的内容
        /// </summary>
        public abstract MemoryStream Stream { get; }

        public int Error { get; set; }

        public IPEndPoint RemoteAddress { get; protected set; }

        public virtual void Start()
        {
        }

        protected NChannel(NetworkChannelType type)
        {
            this.Id = IdGenerater.GenerateId();
            this.NCType = type;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="stream"></param>
        public abstract void AddSend(MemoryStream stream);

        public abstract void Remove();
    }
}
