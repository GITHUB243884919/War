﻿/// <summary>
/// 角色对象数据结构(坦克，士兵等)
/// 
/// author: fanzhengyong
/// date: 2017-02-24
/// 
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharObj
{
    //服务器定义的唯一编号和类型编号
    public int ServerEntityID { get; set; }
    //public int ServerEntityType { get; set; }

    //对象类型
    public BattleObjManager.E_BATTLE_OBJECT_TYPE Type { get; set; }

    //角色对应的prefab 坦克，士兵之类的prefab
    public GameObject GameObject { get; set; }

    public CharController CharController { get; set; }

    //子对象的pool，<instanceID, path> 就是BNGObjPoolManager管理那些pool
    //这里只缓存名字(路径)
    private Dictionary<int, string> m_childsPools =
        new Dictionary<int, string>();

    public string GetChildPool(int instanceID)
    {
        string path = null;
        m_childsPools.TryGetValue(instanceID, out path);
        return path;
    }

    //AI

    /// <summary>
    /// 到达
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="speed"></param>
    public void Arrive(Vector3 startPoint, Vector3 endPoint, float speed)
    {
        Debug.Log("Arrive " + startPoint + " " + endPoint + " " + Time.realtimeSinceStartup);
        CharController.TargetForPosition = startPoint;
        CharController.TargetForArrive = endPoint;
        CharController.SpeedForArrive = speed;
        //OnArrive();
        CharController.Commond(CharController.E_COMMOND.ARRIVE);
    }

    /// <summary>
    /// 攻击
    /// </summary>
    public void Attack()
    {
        Debug.Log("Attack");
        CharController.Commond(CharController.E_COMMOND.ATTACK);
    }


}
