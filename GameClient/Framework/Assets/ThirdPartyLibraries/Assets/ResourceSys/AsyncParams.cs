using System;
using System.Collections.Generic;


public class AsyncParams
{
    public string m_Url;
    public Type m_Type;
    public Action<uint, UnityEngine.Object> m_OnLoadFinished;
    public uint m_ResId;

    private static Queue<AsyncParams> mParamsPool = new Queue<AsyncParams>();

    public AsyncParams(string url, Type type, Action<uint, UnityEngine.Object> onLoadFinished, uint resId)
    {
        Init(url, type, onLoadFinished, resId);
    }

    public void Init(string url, Type type, Action<uint, UnityEngine.Object> onLoadFinished, uint resId)
    {
        this.m_Url = url;
        this.m_Type = type;
        this.m_OnLoadFinished = onLoadFinished;
        this.m_ResId = resId;
    }

    public bool isEquals(AsyncParams otherAsyncParams)
    {
        if (this.m_Url == otherAsyncParams.m_Url && this.m_Type == otherAsyncParams.m_Type)
        {
            return true;
        }

        return false;
    }

    public void Reset()
    {
        m_Url = null;
        m_Type = null;
        m_OnLoadFinished = null;
        m_ResId = 0;
    }

    public static AsyncParams Dequeue(string url, Type type, Action<uint, UnityEngine.Object> onLoadFinished,
        uint resId)
    {
        if (mParamsPool.Count == 0)
        {
            return new AsyncParams(url, type, onLoadFinished, resId);
        }
        else
        {
            AsyncParams item = mParamsPool.Dequeue();
            item.Init(url, type, onLoadFinished, resId);
            return item;
        }
    }

    public static void Enqueue(AsyncParams item)
    {
        item.Reset();
        mParamsPool.Enqueue(item);
    }
}