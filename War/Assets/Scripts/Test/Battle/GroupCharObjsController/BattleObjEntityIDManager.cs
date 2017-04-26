/// <summary>
/// 战斗对象实体ID管理器
/// author: fanzhengyong
/// date: 2017-04-26
/// 
/// 提供生成实体ID的能力
/// int.MinValue~int.MaxValue
/// 循环使用
/// </summary>

using UnityEngine;
using System.Collections;

public class BattleObjEntityIDManager
{
    private static BattleObjEntityIDManager s_instance = null;
    private int m_entityID = int.MinValue;
    public BattleObjEntityIDManager Instance 
    {
        get 
        {
            if (s_instance == null)
            {
                s_instance = new BattleObjEntityIDManager();
            }
            return s_instance;
        }
    }

    public int GenEntityID()
    {
        int entityID = 0;
        if (m_entityID < int.MaxValue)
        {
            m_entityID++;
        }
        else
        {
            m_entityID = int.MinValue;
        }

        entityID = m_entityID;
        return entityID;
    }
}
