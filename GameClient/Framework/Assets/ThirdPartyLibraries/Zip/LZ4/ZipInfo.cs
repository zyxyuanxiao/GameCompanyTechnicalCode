
using System.Collections.Generic;
using System;

[Serializable]
public class ZipInfo 
{
    public List<string> allFileNames = new List<string>();
    public List<int> allFileSizes = new List<int>();

    public void Clear()
    {
        allFileSizes.Clear();
        allFileNames.Clear();
    }

    #region pool  --对象池
    private static SelfObjectPool<ZipInfo> mZipInfoPool = new SelfObjectPool<ZipInfo>(5);
    public static ZipInfo Get()
    {
        var ret = mZipInfoPool.CreateObject<ZipInfo>();
        ret.Clear();
        return ret;
    }

    public static void Recovery(ZipInfo zipInfo)
    {
        mZipInfoPool.RecoverObject(zipInfo);
    }
    #endregion
}

public class SelfObjectPool<T> where T : class
{
    List<T> m_lstObject = new List<T>();
    int m_nThresholdValue = 50;
    public SelfObjectPool(int threshold = 50)
    {
        m_nThresholdValue = threshold;
    }

    public int Threshold
    {
        get
        {
            return m_nThresholdValue;
        }
        set
        {
            m_nThresholdValue = value;
        }
    }
        
    public void PreCreateObject<T1>(int num) where T1 : class,new()
    {
        if (num > m_nThresholdValue)
            num = m_nThresholdValue;
        for (int i = 0;i<num;++i)
        {
            T1 ret = new T1();
            m_lstObject.Add(ret as T);
        }
    }

    public T1 CreateObject<T1>() where T1 : class,new()
    {
        if (m_lstObject.Count > 0)
        {
            T1 ret = m_lstObject[m_lstObject.Count - 1] as T1;
            m_lstObject.RemoveAt(m_lstObject.Count - 1);
            return ret;
        }
        return new T1();
    }

    public void RecoverObject(T obj)
    {
        if (obj != null && m_lstObject.Count < m_nThresholdValue)
        {
            m_lstObject.Add(obj);
        }
        else 
        {
            obj = null;
        }
    }

    public void CheckThreshold()
    {
        if (m_lstObject.Count > m_nThresholdValue)
        {
            m_lstObject.RemoveRange(0, m_nThresholdValue - m_lstObject.Count);
        }
    }

    public void ClearPool()
    {
        m_lstObject.Clear();
    }

    public int Length() 
    {
        return m_lstObject.Count;
    }
}