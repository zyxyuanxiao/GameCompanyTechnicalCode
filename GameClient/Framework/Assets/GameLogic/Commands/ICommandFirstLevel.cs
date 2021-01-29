using System;
using Common;

//一级命令
public interface ICommandFirstLevel : ICommand
{
    
}

public interface ICommandFirstReceiver : IIdentityCard
{
    //接收命令,在此方法中,做出命令的处理方式
    void ReceiverCommand(ICommandFirstLevel command);
}

//全局命令,是最特殊的命令,必须要求所有人做什么事情,一般情况不会发送这个命令
public struct GlobalCommand : ICommandFirstLevel
{
    public bool IsGlobal => true;
    public uint Layer => 1;

    public Object data;
}

//热更命令
public struct HotUpdateCommand : ICommandFirstLevel
{
    public bool IsGlobal => false;
    public uint Layer => 1;
}

// 初始化场景,第一次进入 GameManager 场景
public struct InitSceneCommand : ICommandFirstLevel
{
    public bool IsGlobal => false;
    public uint Layer => 1;
}

// 初始化场景,第一次进入 GameManager 场景
public struct StartLuaCommand : ICommandFirstLevel
{
    public bool IsGlobal => false;
    public uint Layer => 1;
}