//命令接口,具体需要执行命令的类
public interface ICommand
{
    bool IsGlobal { get; }//是否是全局命令,全局命令只能由
    uint Layer { get; }//当前命令需要发送的级别
}

/**
 * 命令管理者
 * 分级概念,中央集权,热插拔模式设计
 *      一级管理者:皇帝      GameManager  永生
 *      二级管理者:中央官员   各类型的 Managers,由 GameManager 控制其生命周期
 *      三级业务员:各地官员   第三方库,大型模块,具体固定的模块,必要玩法,必须实现的模块
 *      四级协作员:村长      具体模块,小型模块组成大型模块(易推翻,模块)
 *      五级办事人:公民      具体小模块的实现人员(随时被推翻)
 * 每个级别都是一个具体的命令管理者,用的时候需要添加:
 * 命令架构的要求:
 *      下级命令只能向下发送,不能向上发送;
 *      下级人员必须执行上级命令;
 *      普通上级命令只能下发到下级命令,不能越级发送;
 *      越级命令需要经由下级发送,即命令的发送必须一层一层进行;
 *      命令接收者只能接上级命令;
 *      全部接收者执行的命令必须由一级管理者发送,其他管理者不能发送全局命令;
 *      凡是承担管理者职务的,都有级别,
 *      上下多条命令是需要接收命令人员亲自解析的,即上级知道发送的命令,下级不知道发送什么命令,下级需要做命令的处理方式.
 *      上级控制下级的生命周期,不能越级控制.
 */
public interface ICommandManager : IIdentityCard
{
    
}

