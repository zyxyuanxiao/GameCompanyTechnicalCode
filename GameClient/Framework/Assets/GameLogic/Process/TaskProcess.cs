using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 任务流需要预先写好代码才可以执行
/// 这个动态任务流执行,达到可以复用任务流的目的
/// </summary>
public partial class ProcessManager
{
    protected enum ETasksStatus
    {
        Add = 0,//已装载任务队列
        Executing,//任务流执行中
        End,//任务流已结束
    }
    
    protected class TaskAction
    {
        public int ID;
        public ETasksStatus Status;//状态:1:表示已装载,2:表示执行中,3:表示执行完毕
        public Action EndAction;//任务流执行完毕之后的回调,有就执行回调,没有就不执行
        public Action CoroutineAction;
    }
    
    /// <summary>
    /// 当用户不停的向 任务流队列 里面添加 任务流, 需要使用队列做好规划,先进先出,必须在主线程中
    /// 任务流不应该很多,目前的定义是任务流是同步执行的
    /// </summary>
    private Queue<TaskAction> taskQueue = new Queue<TaskAction>(5);

    /// <summary>
    /// 任务流使用协程,默认最多 10 个流程类,太多的流程类不适合短任务流
    /// </summary>
    private List<ITaskProcess> tasks = new List<ITaskProcess>(10);

    /// <summary>
    /// 传入这次执行的动态任务流的层次以及任务流执行完毕的方法,根据层次进行初始化,然后执行,任务流执行完毕之后,调用回调
    /// </summary>
    /// <param name="layer">任务流的层次</param>
    public void AddExecute(TaskProcessLayer layer,Action end = null)
    {
        TaskAction ta = new TaskAction()
        {
            ID = taskQueue.Count,
            Status = ETasksStatus.Add,
            EndAction = end,
            CoroutineAction = () =>
            {
                AddTaskProcessAndExecute(layer);
                GameManager.Instance.StartCoroutine(UpdateTaskProcess());
            }
        };
        taskQueue.Enqueue(ta);
    }
    
    
    /// <summary>
    /// 将不同层次的任务流添加进容器中,然后在协程中执行
    /// </summary>
    /// <param name="layer"></param>
    private void AddTaskProcessAndExecute(TaskProcessLayer layer)
    {
        //注册任务流程
        Assembly assembly = typeof(ProcessManager).Assembly;
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsClass && typeof(ITaskProcess).IsAssignableFrom(type))
            {
                //创建实例,并添加到管理者集合中
                ITaskProcess process = Activator.CreateInstance(type) as ITaskProcess;
                if (process.Layer == layer)
                {
                    // Debug.Log(process.ToString());
                    tasks.Add(process);
                }
            }
        }
        tasks.Sort((x,y) =>x.ID.CompareTo(y.ID));
    }

    /// <summary>
    /// 执行任务
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateTaskProcess()
    {
        TaskAction ta = taskQueue.Peek();
        ta.Status = ETasksStatus.Executing;

        yield return GameManager.OneFrame;
        // 执行任务
        foreach (ITaskProcess process in tasks)
        {
            yield return process.Work();
            while (!process.IsDone)
            {
                for (int i = 0; i < process.DelayFrame; i++)
                {
                    yield return GameManager.OneFrame;
                }
            }
            yield return GameManager.OneFrame;
        }
        yield return GameManager.OneFrame;
        //重置所有任务
        foreach (ITaskProcess taskProcess in tasks)
        {
            taskProcess.Reset();
        }
        yield return GameManager.OneFrame;
        tasks.Clear();
        yield return GameManager.OneFrame;
        
        ta.Status = ETasksStatus.End;
    }
}
