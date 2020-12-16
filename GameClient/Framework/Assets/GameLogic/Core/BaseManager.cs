using System;
using System.Collections.Generic;
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
    //严格控制这个地方的内存大小与性能
    private static Dictionary<Type,IManager>_managers = new Dictionary<Type,IManager>();//方便并且性能需求不高
    private static IFixedUpdate[] _fixedUpdate = new IFixedUpdate[1]; //性能需求高,并且清楚知道有多少个 IFixedUpdate
    private static IUpdate[] _update = new IUpdate[3]; //性能需求高,并且清楚知道有多少个 IUpdate
    private static ILateUpdate[] _lateUpdate = new ILateUpdate[1]; //性能需求高,并且清楚知道有多少个 ILateUpdate

    //一帧,协程中使用
    public static WaitForEndOfFrame OneFrame = new WaitForEndOfFrame();
    
    #region 生命周期

    protected virtual void Awake()
    {
        //当前线程的地区设置为美国，避免因为切换不同地区，数字，日期时间，字符串匹配的结果不一样
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        this.AddInitManager();
        foreach (var item in _managers.Values)
        {
            item.Awake();
        }
    }


    protected virtual void Start()
    {
        foreach (var item in _managers.Values)
        {
            item.Start();
        }
    }
    
    protected virtual void FixedUpdate()
    {
        for (int i = 0; i < _fixedUpdate.Length; i++) _fixedUpdate[i].FixedUpdate();
    }
    
    protected virtual void Update()
    {
        for (int i = 0; i < _update.Length; i++) _update[i].Update();
    }

    protected virtual void LateUpdate()
    {
        for (int i = 0; i < _lateUpdate.Length; i++) _lateUpdate[i].LateUpdate();
    }

    protected virtual void OnDestroy()
    {
        foreach (var item in _managers.Values)
        {
            item.OnDestroy();
        }
        _managers = null;
        _fixedUpdate = null;
        _update = null;
        _lateUpdate = null;
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
        int index_F = 0;
        int index_U = 0;
        int index_L = 0;
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsClass && typeof(IManager).IsAssignableFrom(type))
            {
                //创建实例,并添加到管理者集合中
                IManager manager = Activator.CreateInstance(type) as IManager;
                //manager.ID = IdGenerater.GenerateId();
                _managers.Add(type,manager);
                if (typeof(IFixedUpdate).IsAssignableFrom(type))
                {
                    _fixedUpdate[index_F] = manager as IFixedUpdate;
                    index_F++;
                }
                if (typeof(IUpdate).IsAssignableFrom(type))
                {
                    _update[index_U] = manager as IUpdate;
                    index_U++;
                }
                if (typeof(ILateUpdate).IsAssignableFrom(type))
                {
                    _lateUpdate[index_L]=manager as ILateUpdate;
                    index_L++;
                }
            }
        }
    }

    /// <summary>
    /// 获取管理者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T QueryManager<T>() where T : IManager
    {
        if (_managers.TryGetValue(typeof(T), out IManager manager))
        {
            return (T)manager;
        }
        return default;
    }
}
