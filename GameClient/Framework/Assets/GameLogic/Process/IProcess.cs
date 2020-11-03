using System.Collections;

/// <summary>
/// 流程接口,所有的流程需要自行创建继承此接口
/// layer > ID
/// </summary>
public interface IProcess
{
    //任务流的层次,为 0 表示需要在初始化的时候执行,其他时候需要执行其他层次的任务流,下一个层级进行加 1
    ProcessLayer Layer { get; }

    //此 ID 表示任务的优先级,从 0 开始,逐步加 1
    byte ID { get; }

    //延迟几帧执行
    byte DelayFrame { get; }

    //工作方法
    IEnumerator Work();

    //任务执行完毕的标志位
    bool isDone { get; set; }

    //将所有任务数据进行充值
    void Reset();
}