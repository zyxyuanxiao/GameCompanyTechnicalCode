public interface ICommandFifthLevel : ICommand
{
    bool IsGlobal { get; set; }//是否是全局命令,全局命令只能由
    bool Layer { get; set; }
}

public interface ICommandFifthReceiver : IIdentityCard
{
    //接收命令,在此方法中,做出命令的处理方式
    void ReceiverCommand(ICommandFifthLevel command);
}