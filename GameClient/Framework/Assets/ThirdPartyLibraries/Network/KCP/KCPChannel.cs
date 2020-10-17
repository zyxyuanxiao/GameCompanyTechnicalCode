using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    public struct WaitSendBuffer
    {
        public byte[] Bytes;
        public int Length;

        public WaitSendBuffer(byte[] bytes, int length)
        {
            this.Bytes = bytes;
            this.Length = length;
        }
    }

    public sealed class KCPChannel : NChannel
    {
        #region 变量
        private Socket socket;

        //Kcp的所有API
        private IntPtr kcp;

        //发送的数据
        private readonly Queue<WaitSendBuffer> sendBuffer = new Queue<WaitSendBuffer>();

        //是否连接
        private bool isConnected;

        //服务器的IP
        private readonly IPEndPoint remoteEndPoint;

        //最后一次接受的时间
        private uint lastRecvTime;

        //创建此频道的时间
        private readonly uint createTime;

        //此频道的发送的内容
        private readonly MemoryStream memoryStream = null;
        public override MemoryStream Stream => this.memoryStream;

        //远程连接的ID
        public uint RemoteConnectionId { get; private set; }

        //本地连接的ID
        public uint LocalConnectionId { get; private set; }
        #endregion

        //初始化
        public KCPChannel(uint localConnectionId, uint remoteConnectionId) : base(NetworkChannelType.Accept)
        {
            this.LocalConnectionId = localConnectionId;
            this.RemoteConnectionId = remoteConnectionId;

        }




        public override void AddSend(MemoryStream stream)
        {

        }

        public override void Remove()
        {

        }
    }
}
