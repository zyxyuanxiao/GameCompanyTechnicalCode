using System.Collections.Generic;
using System;

namespace Best
{
    //public interface IObjectPoolItem : UnInitializable
    //{
    //    void Initialize();
    //}
    //public interface IObjectPoolItem<P1> : UnInitializable
    //{
    //    void Initialize(P1 p);
    //}

    //public interface IObjectPoolItem<P1, P2> : UnInitializable
    //{
    //    void Initialize(P1 p1, P2 p2);
    //}

    //public interface IObjectPoolItem<P1, P2, P3> : UnInitializable
    //{
    //    void Initialize(P1 p1, P2 p2, P3 p3);
    //}

    //public interface IObjectPoolItem<P1, P2, P3, P4> : UnInitializable
    //{
    //    void Initialize(P1 p1, P2 p2, P3 p3, P4 p4);
    //}

    //public interface IObjectPoolItem<P1, P2, P3, P4, P5> : UnInitializable
    //{
    //    void Initialize(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);
    //}

    //public interface IObjectPoolItem<P1, P2, P3, P4, P5, P6> : UnInitializable
    //{
    //    void Initialize(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6);
    //}

    //public interface IObjectPoolItem<P1, P2, P3, P4, P5, P6, P7> : UnInitializable
    //{
    //    void Initialize(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7);
    //}

    //public interface IObjectPoolItem<P1, P2, P3, P4, P5, P6, P7, P8> : UnInitializable
    //{
    //    void Initialize(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8);
    //}

    //public sealed class ObjectPool<T> : ObjectPoolImpl<T> where T : IObjectPoolItem, new()
    //{
    //    public ObjectPool(uint capacity) : base(capacity)
    //    {
    //    }

    //    public T Get()
    //    {
    //        T ret;
    //        if (m_pool.Count > 0)
    //            ret = m_pool.Pop();
    //        else
    //            ret = new T();
    //        ret.Initialize();
    //        return ret;
    //    }
    //}

    //public sealed class ObjectPool<T, P1> : ObjectPoolImpl<T> where T : IObjectPoolItem<P1>, new()
    //{
    //    public ObjectPool(uint capacity) : base(capacity)
    //    {
    //    }

    //    public T Get(P1 p)
    //    {
    //        T ret;
    //        if (m_pool.Count > 0)
    //            ret = m_pool.Pop();
    //        else
    //            ret = new T();
    //        ret.Initialize(p);
    //        return ret;
    //    }
    //}

    //public sealed class ObjectPool<T, P1, P2> : ObjectPoolImpl<T> where T : IObjectPoolItem<P1, P2>, new()
    //{
    //    public ObjectPool(uint capacity) : base(capacity)
    //    {
    //    }

    //    public T Get(P1 p1, P2 p2)
    //    {
    //        T ret;
    //        if (m_pool.Count > 0)
    //            ret = m_pool.Pop();
    //        else
    //            ret = new T();
    //        ret.Initialize(p1, p2);
    //        return ret;
    //    }
    //}

    //public sealed class ObjectPool<T, P1, P2, P3> : ObjectPoolImpl<T> where T : IObjectPoolItem<P1, P2, P3>, new()
    //{
    //    public ObjectPool(uint capacity) : base(capacity)
    //    {
    //    }

    //    public T Get(P1 p1, P2 p2, P3 p3)
    //    {
    //        T ret;
    //        if (m_pool.Count > 0)
    //            ret = m_pool.Pop();
    //        else
    //            ret = new T();
    //        ret.Initialize(p1, p2, p3);
    //        return ret;
    //    }
    //}

    //public sealed class ObjectPool<T, P1, P2, P3, P4> : ObjectPoolImpl<T> where T : IObjectPoolItem<P1, P2, P3, P4>, new()
    //{
    //    public ObjectPool(uint capacity) : base(capacity)
    //    {
    //    }

    //    public T Get(P1 p1, P2 p2, P3 p3, P4 p4)
    //    {
    //        T ret;
    //        if (m_pool.Count > 0)
    //            ret = m_pool.Pop();
    //        else
    //            ret = new T();
    //        ret.Initialize(p1, p2, p3, p4);
    //        return ret;
    //    }
    //}

    //public sealed class ObjectPool<T, P1, P2, P3, P4, P5> : ObjectPoolImpl<T> where T : IObjectPoolItem<P1, P2, P3, P4, P5>, new()
    //{
    //    public ObjectPool(uint capacity) : base(capacity)
    //    {
    //    }

    //    public T Get(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    //    {
    //        T ret;
    //        if (m_pool.Count > 0)
    //            ret = m_pool.Pop();
    //        else
    //            ret = new T();
    //        ret.Initialize(p1, p2, p3, p4, p5);
    //        return ret;
    //    }
    //}

    //public sealed class ObjectPool<T, P1, P2, P3, P4, P5, P6> : ObjectPoolImpl<T> where T : IObjectPoolItem<P1, P2, P3, P4, P5, P6>, new()
    //{
    //    public ObjectPool(uint capacity) : base(capacity)
    //    {
    //    }

    //    public T Get(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    //    {
    //        T ret;
    //        if (m_pool.Count > 0)
    //            ret = m_pool.Pop();
    //        else
    //            ret = new T();
    //        ret.Initialize(p1, p2, p3, p4, p5, p6);
    //        return ret;
    //    }
    //}

    //public sealed class ObjectPool<T, P1, P2, P3, P4, P5, P6, P7> : ObjectPoolImpl<T> where T : IObjectPoolItem<P1, P2, P3, P4, P5, P6, P7>, new()
    //{
    //    public ObjectPool(uint capacity) : base(capacity)
    //    {
    //    }

    //    public T Get(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    //    {
    //        T ret;
    //        if (m_pool.Count > 0)
    //            ret = m_pool.Pop();
    //        else
    //            ret = new T();
    //        ret.Initialize(p1, p2, p3, p4, p5, p6, p7);
    //        return ret;
    //    }
    //}

    //public sealed class ObjectPool<T, P1, P2, P3, P4, P5, P6, P7, P8> : ObjectPoolImpl<T> where T : IObjectPoolItem<P1, P2, P3, P4, P5, P6, P7, P8>, new()
    //{
    //    public ObjectPool(uint capacity) : base(capacity)
    //    {
    //    }

    //    public T Get(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    //    {
    //        T ret;
    //        if (m_pool.Count > 0)
    //            ret = m_pool.Pop();
    //        else
    //            ret = new T();
    //        ret.Initialize(p1, p2, p3, p4, p5, p6, p7, p8);
    //        return ret;
    //    }
    //}


    //#region Impl
    //public class ObjectPoolImpl<T> where T :UnInitializable
    //{
    //    public uint Capacity { get; private set; }

    //    public void Return(T item)
    //    {
    //        if (m_pool.Count < Capacity)
    //        {
    //            item.UnInitialize();
    //            m_pool.Push(item);
    //        }
    //    }

    //    protected ObjectPoolImpl(uint capacity)
    //    {
    //        Capacity = capacity;
    //        m_pool = new Stack<T>();
    //    }

    //    protected Stack<T> m_pool;
    //}

    //public interface UnInitializable
    //{
    //    void UnInitialize();
    //}
    //#endregion
}
