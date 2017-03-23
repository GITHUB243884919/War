/// <summary>
/// Log中介
/// author: fanzhengyong
/// date: 2017-02-24
/// </summary>
using UnityEngine;
using System.Collections;

public static class LogMediator
{
    public static void Log(object message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    public static void LogFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        Debug.LogFormat(format, args);
#endif
    }

    public static void LogWarning(object message)
    {
#if UNITY_EDITOR
        Debug.LogWarning(message);
#endif
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        Debug.LogWarningFormat(format, args);
#endif
    }

    public static void LogError(object message)
    {
#if UNITY_EDITOR
        Debug.LogError(message);
#endif
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        Debug.LogErrorFormat(format, args);
#endif
    }

}
