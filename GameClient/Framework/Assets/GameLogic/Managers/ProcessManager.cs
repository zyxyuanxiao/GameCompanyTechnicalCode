using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 流程管理者,游戏启动后,第一个干活的类
/// 采用 任务队列 设计模式
/// 一个任务即一个 IProcess
/// </summary>
public partial class ProcessManager : IManager
{
    public void Awake()
    {
        //初始化流程任务,次流程只限于在启动 APP 的时候初始化使用,启动游戏-结束/重启游戏之间只会启动一次
        AddExecuteTasks(ProcessLayer.Init);
    }

    public void Start()
    {
        
    }
    
    public void OnDestroy()
    {

    }

    public void Update()
    {
        if (taskActions == null || taskActions.Count <= 0) return;
        TaskAction ta = taskActions[0];
        if (ta.Status == 1)
        {
            ta.Status = 2;
            ta.TAction?.Invoke();
        }
        else if (ta.Status == 2)
        {
            
        }
        else if (ta.Status == 3)
        {
            taskActions.Remove(ta);
            ta.EndAction?.Invoke();
        }
    }
}