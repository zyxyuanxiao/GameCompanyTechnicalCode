using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


/// <summary>
/// 任务流需要预先写好代码才可以执行
/// 这个动态任务流执行,达到可以复用任务流的目的
/// </summary>
public partial class ProcessManager
{
    private SortedDictionary<int, IProcess> tasks = new SortedDictionary<int, IProcess>();

    /// <summary>
    /// 传入这次执行的动态任务流的层次,根据层次进行初始化,然后执行
    /// </summary>
    /// <param name="layer">任务流的层次</param>
    public void AddExecuteTasks(ProcessLayer layer)
    {
        AddTaskProcessAndExecute(layer);
        GameManager.Instance.StartCoroutine(UpdateTaskProcess());
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
                    tasks.Add(process.ID, process);
                }
            }
        }
    }


    private IEnumerator UpdateTaskProcess()
    {
        yield return GameManager.OneFrame;
        // 执行任务
        ICollection<IProcess> values = tasks.Values;
        foreach (IProcess process in values)
        {
            yield return process.Work();
            while (!process.isDone)
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
        foreach (IProcess process in values)
        {
            process.Reset();
        }
        yield return GameManager.OneFrame;
        tasks.Clear();
        yield return GameManager.OneFrame;
    }
}
