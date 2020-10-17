using System;
using System.Diagnostics;

namespace LogSystem
{
  public class rdtDebug
  {
    public static rdtDebug.LogLevel s_logLevel = rdtDebug.LogLevel.Info;

    public static void Debug(object instance, string fmt, params object[] args)
    {
      rdtDebug.Log(instance, rdtDebug.LogLevel.Debug, fmt, args);
    }

    public static void Debug(string fmt, params object[] args)
    {
      rdtDebug.Log(rdtDebug.LogLevel.Debug, fmt, args);
    }

    public static void Info(object instance, string fmt, params object[] args)
    {
      rdtDebug.Log(instance, rdtDebug.LogLevel.Info, fmt, args);
    }

    public static void Info(string fmt, params object[] args)
    {
      rdtDebug.Log(rdtDebug.LogLevel.Info, fmt, args);
    }

    public static void Warning(string fmt, params object[] args)
    {
      rdtDebug.Log(rdtDebug.LogLevel.Warning, fmt, args);
    }

    public static void Warning(object instance, string fmt, params object[] args)
    {
      rdtDebug.Log(instance, rdtDebug.LogLevel.Warning, fmt, args);
    }

    public static void Error(string fmt, params object[] args)
    {
      rdtDebug.Log(rdtDebug.LogLevel.Error, fmt, args);
    }

    public static void Error(object instance, string fmt, params object[] args)
    {
      rdtDebug.Log(instance, rdtDebug.LogLevel.Error, fmt, args);
    }

    public static void Error(object instance, Exception e, string fmt, params object[] args)
    {
      rdtDebug.Log(instance, e, rdtDebug.LogLevel.Error, fmt, args);
    }

    public static void Log(
      object instance,
      Exception e,
      rdtDebug.LogLevel l,
      string fmt,
      params object[] args)
    {
      Exception exception = e.InnerException != null ? e.InnerException : e;
      rdtDebug.Log(instance, l, fmt + " " + (object) exception + " " + e.StackTrace, args);
    }

    public static void Log(object instance, rdtDebug.LogLevel l, string fmt, params object[] args)
    {
      if (l < rdtDebug.s_logLevel)
        return;
      string str = instance.GetType().Name + ": " + string.Format(fmt, args);
      if (l == rdtDebug.LogLevel.Error)
        UnityEngine.Debug.LogError((object) str);
      else if (l == rdtDebug.LogLevel.Warning)
        UnityEngine.Debug.LogWarning((object) str);
      else
        UnityEngine.Debug.Log((object) str);
    }

    public static void Log(rdtDebug.LogLevel l, string fmt, params object[] args)
    {
      if (l < rdtDebug.s_logLevel)
        return;
      string str = string.Format(fmt, args);
      if (l == rdtDebug.LogLevel.Error)
        UnityEngine.Debug.LogError((object) str);
      else if (l == rdtDebug.LogLevel.Warning)
        UnityEngine.Debug.LogWarning((object) str);
      else
        UnityEngine.Debug.Log((object) str);
    }

    [Conditional("DEBUG")]
    public static void Assert(bool condition)
    {
      if (condition)
        return;
      UnityEngine.Debug.LogError((object) "Assert failed");
    }

    public enum LogLevel
    {
      Debug,
      Info,
      Warning,
      Error,
    }
  }
}
