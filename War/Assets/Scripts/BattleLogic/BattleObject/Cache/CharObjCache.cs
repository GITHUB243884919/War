/// <summary>
/// 角色对象缓存。
/// author: fanzhengyong
/// date: 2017-02-24 
/// 
/// 这里存的都是已经从对象池中取过的对象。
/// 内部主要使用map存储，key是服务器的实体编号。
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharObjCache
{
    private Dictionary<int, CharObj> m_cache
       = new Dictionary<int, CharObj>();

    private static CharObjCache s_Instance = null;
    public static CharObjCache Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new CharObjCache();
            }
            return s_Instance;
        }
    }

    public Dictionary<int, CharObj> Cache { get { return m_cache; } }

    public void Init() { }

    public bool Add(CharObj charObj)
    {
        bool result = false;

        if (charObj == null)
        {
            return result;
        }

        CharObj _charObj;
        m_cache.TryGetValue(charObj.ServerEntityID, out _charObj);
        if (_charObj != null)
        {
            result = true;
            return result;
        }

        m_cache.Add(charObj.ServerEntityID, charObj);

        result = true;
        return result;
    }

    public bool Remove(CharObj charObj)
    {
        bool result = false;

        if (charObj == null)
        {
            return result;
        }

        CharObj _charObj = null;
        m_cache.TryGetValue(charObj.ServerEntityID, out charObj);
        if (_charObj == null)
        {
            result = true;
            return result;
        }

        m_cache[charObj.ServerEntityID] = null;
        m_cache.Remove(charObj.ServerEntityID);

        result = true;
        return result;
    }

    public CharObj Find(int serverEntityID)
    {
        //Debug.Log("Find " + serverEntityID);
        CharObj charObj = null;
        m_cache.TryGetValue(serverEntityID, out charObj);

        return charObj;
    }

    
}
