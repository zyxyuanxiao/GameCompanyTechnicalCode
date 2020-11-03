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
    protected class TaskAction
    {
        public int ID;
        public int Status;//状态:1:表示已装载,2:表示执行中,3:表示执行完毕
        public Action TAction;
        public Action EndAction;
    }

    private List<IProcess> tasks = new List<IProcess>();

    private List<TaskAction> taskActions = new List<TaskAction>();
    

    /// <summary>
    /// 传入这次执行的动态任务流的层次,根据层次进行初始化,然后执行
    /// </summary>
    /// <param name="layer">任务流的层次</param>
    public void AddExecuteTasks(ProcessLayer layer,Action end = null)
    {
        TaskAction ta = new TaskAction()
        {
            ID = taskActions.Count,
            Status = 1,
            TAction = () =>
            {
                AddTaskProcessAndExecute(layer);
                GameManager.Instance.StartCoroutine(UpdateTaskProcess());
            }
        };
        taskActions.Add(ta);
    }
    
    
    /// <summary>
    /// 将不同层次的任务流添加进容器中,然后在协程中执行
    /// </summary>
    /// <param name="layer"></param>
    private void AddTaskProcessAndExecute(ProcessLayer layer)
    {
        //注册任务流程
        Assembly assembly = typeof(ProcessManager).Assembly;
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsClass && typeof(IProcess).IsAssignableFrom(type))
            {
                //创建实例,并添加到管理者集合中
                IProcess process = Activator.CreateInstance(type) as IProcess;
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
        TaskAction ta = taskActions[0];
        ta.Status = 2;

        yield return GameManager.OneFrame;
        // 执行任务
        foreach (IProcess process in tasks)
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
        foreach (IProcess process in tasks)
        {
            process.Reset();
        }
        yield return GameManager.OneFrame;
        tasks.Clear();
        yield return GameManager.OneFrame;
        
        ta.Status = 3;
        Debug.Log("任务结束2");
    }
}
