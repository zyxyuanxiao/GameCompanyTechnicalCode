using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 流程管理者,游戏启动后,第一个干活的类
/// 采用 任务队列 设计模式
/// 一个任务即一个 IProcess
/// </summary>
public partial class ProcessManager : IUpdateManager
{
    public void Awake()
    {
        //初始化流程任务,次流程只限于在启动 APP 的时候初始化使用,启动游戏-结束/重启游戏之间只会启动一次
        AddExecute(TaskProcessLayer.Init);
    }

    public void Start()
    {
        
    }
    
    public void OnDestroy()
    {

    }

    public void Update()
    {
        if (taskQueue == null || taskQueue.Count <= 0) return;
        TaskAction ta = taskQueue.Peek();
        if (ta.Status == ETasksStatus.Add)//执行任务流
        {
            ta.Status = ETasksStatus.Executing;
            ta.CoroutineAction?.Invoke();
        }
        else if (ta.Status == ETasksStatus.End)//任务流结束
        {
            taskQueue.Dequeue();
            ta.EndAction?.Invoke();
        }
    }
}