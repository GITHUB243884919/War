/// <summary>
/// Log中介
/// author: fanzhengyong
/// date: 2017-03-24
/// </summary>
using UnityEngine;
using System.Collections;

public static class LogMediator
{
    private static bool m_enable = true;
    public static void Log(object message)
    {
#if UNITY_EDITOR
        if (m_enable)
        {
            Debug.Log(message);
        }
#endif
    }

    public static void LogFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        if (m_enable)
        {
            Debug.LogFormat(format, args);
        }
#endif
    }

    public static void LogWarning(object message)
    {
#if UNITY_EDITOR
        if (m_enable)
        {
            Debug.LogWarning(message);
        }
#endif
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        if (m_enable)
        {
            Debug.LogWarningFormat(format, args);
        }
#endif
    }

    public static void LogError(object message)
    {
#if UNITY_EDITOR
        if (m_enable)
        {
            Debug.LogError(message);
        }
#endif
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        if (m_enable)
        {
            Debug.LogErrorFormat(format, args);
        }
#endif
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
#if UNITY_EDITOR
        if (m_enable)
        {
            Debug.DrawLine(start, end, color, duration);
        }
#endif
    }
}
