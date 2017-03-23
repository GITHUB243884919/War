/// <summary>
/// 协程中的等待缓存
/// author : fanzhengyong
/// date  : 2017-03-22
/// 
/// 用WaitInCoroutine.XX
/// 代替 WaitForEndOfFrame，WaitForFixedUpdate， WaitForSeconds
/// </summary>
/// 
using System.Collections.Generic;
using UnityEngine;

public static class WaitInCoroutine
{
    public static bool        s_enabled          = true;
    public static int         s_internalCounter  = 0;
    static WaitForEndOfFrame  s_endOfFrame       = new WaitForEndOfFrame();
    static WaitForFixedUpdate s_fixedUpdate      = new WaitForFixedUpdate();
    static Dictionary<float, WaitForSeconds>     s_waitForSeconds
        = new Dictionary<float, WaitForSeconds>();

    public static WaitForEndOfFrame WaitForEndOfFrame()
    {
        s_internalCounter++;
        if (s_enabled)
        {
            return s_endOfFrame;
        }
        return new WaitForEndOfFrame(); 
    }

    public static WaitForFixedUpdate WaitForFixedUpdate()
    {
        s_internalCounter++;
        if (s_enabled)
        {
            return s_fixedUpdate;
        }
        return new WaitForFixedUpdate();
    }

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        s_internalCounter++;

        float _seconds = seconds;
        if (_seconds < 0f)
        {
            Debug.LogError("WaitForSeconds seconds < 0");
            _seconds = 0f;
        }

        if (!s_enabled)
        {
            return new WaitForSeconds(seconds);
        }

        WaitForSeconds wfs;
        if (!s_waitForSeconds.TryGetValue(seconds, out wfs))
        {
            wfs = new WaitForSeconds(seconds);
            s_waitForSeconds.Add(seconds, wfs);
        }
         
        return wfs;
    }

    public static void Release()
    {
        s_endOfFrame  = null;
        s_fixedUpdate = null;
        
        s_waitForSeconds.Clear();
        s_waitForSeconds = null;
    }
}