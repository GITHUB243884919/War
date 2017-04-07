/// <summary>
/// Log中介
/// author: fanzhengyong
/// date: 2017-03-24
/// </summary>
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public static class LogMediator
{
    enum E_SHOW_TYPE 
    {
        NONE,
        UI      
    }
    
    private static bool        s_enable   = true;

    private static E_SHOW_TYPE s_showType = E_SHOW_TYPE.UI;

    private static GameObject  s_UIRoot   = null;
    private static GameObject  UIRoot 
    {
        get 
        {
            if (s_UIRoot != null)
            {
                return s_UIRoot;
            }

            s_UIRoot = GameObject.Find("Canvas/UI_LOG_ROOT");

            return s_UIRoot;
        }
    }

    public static void Log(object message)
    {
#if UNITY_EDITOR
        if (s_enable)
        {
            Debug.Log(message);
        }
#endif
        string str = Convert.ToString(message);
        Show(str);
    }

    public static void LogFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        if (s_enable)
        {
            Debug.LogFormat(format, args);
        }
#endif
    }

    public static void LogWarning(object message)
    {
#if UNITY_EDITOR
        if (s_enable)
        {
            Debug.LogWarning(message);
        }
#endif
        string str = Convert.ToString(message);
        Show(str);
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        if (s_enable)
        {
            Debug.LogWarningFormat(format, args);
        }
#endif
    }

    public static void LogError(object message)
    {
#if UNITY_EDITOR
        if (s_enable)
        {
            Debug.LogError(message);
        }
#endif
        string str = Convert.ToString(message);
        Show(str);
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        if (s_enable)
        {
            Debug.LogErrorFormat(format, args);
        }
#endif
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
#if UNITY_EDITOR
        if (s_enable)
        {
            Debug.DrawLine(start, end, color, duration);
        }
#endif
    }

    private static void Show(string str)
    {
        switch(s_showType)
        {
            case E_SHOW_TYPE.UI:
                ShowUI(str);
                break;
            default:
                break;
        }
    }

    private static void ShowUI(string str)
    {
        GameObject UIGo = UIRoot;
        if (UIGo == null)
        {
            return;
        }

        Text txtCom = UIGo.GetComponent<Text>();
        if (txtCom == null)
        {
            return;
        }

        txtCom.text = str;
    }
}


