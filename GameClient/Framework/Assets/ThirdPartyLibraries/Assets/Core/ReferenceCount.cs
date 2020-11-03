using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 引用计数,每个 AssetBundle 都需要一个引用计数类
/// </summary>
public abstract class ReferenceCount
{
    public string name { get; set; }
    
    /// <summary>
    /// 引用次数
    /// </summary>
    public int refCount { get; private set; }
    
    
    /// <summary>
    /// 释放
    /// </summary>
    public virtual void Release()
    {
        refCount--;
        if (refCount < 0)
        {
            Debug.LogErrorFormat("Release: {0} refCount < 0", name);
        } 
    }
    
    /// <summary>
    /// 引用次数+1
    /// </summary>
    public virtual void Retain()
    {
        refCount++;
    }
    
}
