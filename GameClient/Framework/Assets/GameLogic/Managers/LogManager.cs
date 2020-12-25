using System;
using UnityEngine;

public sealed class LogManager : IManager
{
    private float LastTime = 0;
    private static int LastGCCount = 0;
    public void Awake()
    {
        
    }

    public void Start()
    {
        LogSystem.GCTools.GCDone += gcNotification;
    }

    public void OnDestroy()
    {
        LogSystem.GCTools.GCDone -= gcNotification;
    }

    //如果 1 秒内高频 GC,需要输出 Log
    private void gcNotification(Int32 generation)
    {
        if (DateTime.Now.Second - LastTime <= 1)return;

        int count = (GC.CollectionCount(generation) - LastGCCount);
        if (LastTime > 0 && count > 1)
        {
            //回调的次数越多,说明 GC 的次数越多,总内存越小,说明可分配的内存块越小,正常情况下 6-10 秒左右回调一次,大小7-15MB
            Debug.LogFormat(@"<color=yellow>1秒内频繁 GC; 对第{0}代GC了{1}次;GC 总内存:{2}MB</color>",generation,count,GC.GetTotalMemory(true)/1024/1024);
        }
        
        LastGCCount = GC.CollectionCount(generation);
        LastTime = DateTime.Now.Second;
    }


}