public interface ICommandFourthLevel
{
        
}

public interface ICommandFourthReceiver : IIdentityCard
{
    //接收命令,在此方法中,做出命令的处理方式
    void ReceiverCommand(ICommandFourthLevel command);
}