using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 设计模式方式采取manager of managers
/// 不采用观察者模式 bind设计模式,这个方式在全局细分上面有很大问题,即不同问题需要不同的设计模式对应
/// 业务模块采取MVC设计模式
/// 高性能部分采取ECS设计模式
/// </summary>
public abstract class BaseManager : MonoBehaviour
{
    private Dictionary<Type, IManager> dictManager;
    private IManager[] _managers; 

    //一帧,协程中使用
    public static WaitForEndOfFrame OneFrame = new WaitForEndOfFrame();
    
    #region 生命周期

    protected virtual void Awake()
    {
        dictManager= new Dictionary<Type, IManager>();
        this.AddInitManager();
        //使用数组,提高查询效率
        _managers = new IManager[dictManager.Count];
        int i = 0;
        foreach (var item in dictManager)
        {
            item.Value.Awake();
            _managers[i] = item.Value;
            i++;
        }
    }


    protected virtual void Start()
    {
        for (int i = 0; i < _managers.Length; i++) _managers[i].Start();
    }

    protected virtual void Update()
    {
        // yield return GameManager.OneFrame;
        for (int i = 0; i < _managers.Length; i++) _managers[i].Update();
    }

    protected virtual void OnDestroy()
    {
        for (int i = 0; i < _managers.Length; i++) _managers[i].OnDestroy();
        _managers = null;
    }

    protected virtual void OnApplicationQuit()
    {
        
    }

    #endregion

    /// <summary>
    /// 获取当前的Dll中所有继承自IManager的管理者,并注册进字典管理者
    /// 并且形成互不干扰的模块.比如某一模块,必须依赖另一个模块启动,这是错误的
    /// </summary>
    private void AddInitManager()
    {
        Assembly assembly = typeof(BaseManager).Assembly;
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsClass && typeof(IManager).IsAssignableFrom(type))
            {
                //创建实例,并添加到管理者集合中
                IManager manager = Activator.CreateInstance(type) as IManager;
                //manager.ID = IdGenerater.GenerateId();
                dictManager.Add(type, manager);
            }
        }
    }

    /// <summary>
    /// 获取管理者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T QueryManager<T>() where T : IManager
    {
        if (dictManager.TryGetValue(typeof(T), out IManager manager))
        {
            return (T)manager;
        }
        return default;
    }
}
