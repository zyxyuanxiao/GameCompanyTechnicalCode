﻿using System.Collections.Generic;
//using UnityEngine.Profiling;

public static class LuaProfiler
{
    public static List<string> list = new List<string>();

    public static void Clear()
    {
        list.Clear();
    }

    public static int GetID(string name)
    {
        int id = list.Count;
        list.Add(name);
        return id;
    }

    public static void BeginSample(int id)
    {
        string name = list[id];
        UnityEngine.Profiling.Profiler.BeginSample(name);
    }

    public static void EndSample()
    {
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
