using Network;

public sealed class NetworkManager : IManager, IUpdate
{
    private Network.Network network;
    public long InstanceId { get; set; }


    public void Awake()
    {
        this.network = new Network.Network(NetworkProtocol.TCP, NetworkChannelType.Connect);
    }

    public void Start()
    {

    }

    public void OnDestroy()
    {
        this.network.Dispose();
    }

    public void Update()
    {
        //子线程的方法,使用线程安全的queue存起来,并在主线程中执行
        OneThreadSynchronizationContext.Instance.Update();
    }
}
