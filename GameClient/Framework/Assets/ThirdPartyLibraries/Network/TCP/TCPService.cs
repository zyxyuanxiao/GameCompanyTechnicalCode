using System.Net;

namespace Network
{
    public sealed class TCPService : NService
    {
        public override NChannel AddConnectChannel(IPEndPoint iPEndPoint)
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public override NChannel Query(long id)
        {
            throw new System.NotImplementedException();
        }

        public override void Remove(long id)
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateAccept()
        {
            throw new System.NotImplementedException();
        }
    }
}
