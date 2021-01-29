public interface ICommandSecondLevel
{
        
}

public interface ICommandSecondReceiver : IIdentityCard
{
    //接收命令,在此方法中,做出命令的处理方式
    void ReceiverCommand(ICommandSecondLevel command);
}