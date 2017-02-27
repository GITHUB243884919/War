/// <summary>
/// 战斗中模型使用的对象池缓存。
/// author: fanzhengyong
/// date: 2017-02-22 
/// 
/// 之所以Active为前缀，意思是这里存的都是已经从对象池中取过的对象。
/// 内部主要使用map存储，key是服务器的实体编号。
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActiveBattleModelObject2
{
    public BattleScene2.E_BATTLE_OBJECT_TYPE Type { get; set; }

    //服务器定义的唯一编号和类型编号
    public int ServerEntityID { get; set; }
    //public int ServerEntityType { get; set; }

    public GameObject GameObject { get; set; }
}

public class ActiveBattleModelObjectCache2
{
    private Dictionary<int, ActiveBattleModelObject2> m_cache
       = new Dictionary<int, ActiveBattleModelObject2>();

    private static ActiveBattleModelObjectCache2 s_Instance = null;
    public static ActiveBattleModelObjectCache2 Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new ActiveBattleModelObjectCache2();
            }
            return s_Instance;
        }
    }
    public void Init() { }

    public bool Add(int serverEntityID, GameObject obj, 
        BattleScene2.E_BATTLE_OBJECT_TYPE type, out ActiveBattleModelObject2 activeObj)
    {
        bool result = false;
        activeObj = null;

        if (obj == null)
        {
            return result;
        }

        m_cache.TryGetValue(serverEntityID, out activeObj);
        if (activeObj != null)
        {
            result = true;
            return result;
        }

        activeObj = new ActiveBattleModelObject2();
        activeObj.ServerEntityID = serverEntityID;
        activeObj.GameObject = obj;
        //activeObj.ServerEntityType = type;
        activeObj.Type = type;

        m_cache.Add(serverEntityID, activeObj);

        result = true;
        return result;
    }

    public bool Remove(ActiveBattleModelObject2 activeObj)
    {
        bool result = false;

        if (activeObj == null)
        {
            return result;
        }

        ActiveBattleModelObject2 _activeObj = null;
        m_cache.TryGetValue(activeObj.ServerEntityID, out activeObj);
        if (_activeObj == null)
        {
            result = true;
            return result;
        }

        m_cache[activeObj.ServerEntityID] = null;
        m_cache.Remove(activeObj.ServerEntityID);

        result = true;
        return result;
    }

    public ActiveBattleModelObject2 Find(int serverEntityID)
    {
        ActiveBattleModelObject2 activeObj = null;
        m_cache.TryGetValue(serverEntityID, out activeObj);

        return activeObj;
    }
}
