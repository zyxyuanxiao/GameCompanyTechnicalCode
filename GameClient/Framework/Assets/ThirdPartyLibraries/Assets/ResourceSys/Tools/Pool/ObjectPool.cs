using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Best_Pool
{
    /** <summary>
     * objectpool用于代码对象，提供了四个可选继承接口，在不同阶段执行
     **/

    /// <summary>
    /// IPoolSpawn 会在spawn时候执行该回调
    /// </summary>
    public interface IPoolSpawn
    {
        void PoolSpawn();
    }

    /// <summary>
    /// 在池despawn时执行
    /// </summary>
    public interface IPoolDespawn
    {
        void PoolDespawn();
    }

    /// <summary>
    /// 在对象创建时执行
    /// </summary>
    public interface IPoolInstance
    {
        void PoolInstance();
    }

    /// <summary>
    /// 在对象销毁时执行
    /// </summary>
    public interface IPoolDestroy
    {
        void PoolDestroy();
    }

    public sealed class ObjectPool<T> : PoolParent<T> where T : class, new()
    {
        public ObjectPool(string tag, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null) : base(tag, limitSetter, cullSetter, preSetter)
        {
        }

        public override T CreateInstance()
        {
            T t = new T();
            if (IsDebug)
                poolDebug.CreateTimes++;
            IPoolInstance i = t as IPoolInstance;
            if (i != null)
                i.PoolInstance();
            return t;
        }

        public override void DestroyInstance(T ins)
        {
            IPoolDestroy d = ins as IPoolDestroy;
            if (IsDebug)
            {
                poolDebug.DestroyTimes++;
                if (bLogMessage)
                    Debug.LogFormat("ObjectPool {0} destroy the object type {1}. _spawned count = {2}, _despawned count={3}.",
                                    typeName, poolName, _spawned.Count, _despawned.Count);
            }
                
            if (d != null)
                d.PoolDestroy();
        }

        public override T SpawnNew()
        {
            T o = CreateInstance();
            _spawned.AddFirst(o);
            if (bLogMessage)
                Debug.Log(string.Format("ObjectPool {0}: Spawned new instance type of {1}._spawned count = {2}, _despawned count = {3}",
                                        poolName, typeName, _spawned.Count, _despawned.Count));
            return o;
        }

        public override void SendMessage(bool isSpawn, T ins)
        {
            if (isSpawn)
            {
                IPoolSpawn s = ins as IPoolSpawn;
                if (s != null)
                    s.PoolSpawn();
            }
            else
            {
                IPoolDespawn d = ins as IPoolDespawn;
                if (d != null)
                    d.PoolDespawn();
            }
        }
    }
}

