/// <summary>
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

    //API for AI begin
    /// <summary>
    /// 瞬间定位
    /// </summary>
    /// <param name="position"></param>
    public void Position(Vector3 position)
    {
        CharController.TargetForPosition = position;
        CharController.Commond(CharController.E_COMMOND.POSITION);
    }

    /// <summary>
    /// 到达
    /// </summary>
    /// <param name="startPoint">起点</param>
    /// <param name="endPoint">终点</param>
    /// <param name="speed">速度</param>
    public void Arrive(Vector3 startPoint, Vector3 endPoint, float speed)
    {
        Debug.Log("Arrive " + startPoint + " " + endPoint + " " + Time.realtimeSinceStartup);
        CharController.TargetForPosition = startPoint;
        CharController.TargetForArrive = endPoint;
        CharController.SpeedForArrive = speed;
        CharController.Commond(CharController.E_COMMOND.ARRIVE);
    }

    /// <summary>
    /// 攻击
    /// </summary>
    /// <param name="position">自身位置</param>
    /// <param name="target">目标</param>
    /// <param name="waitSeconds">等待时间（秒）</param>
    public void Attack(Vector3 position, Vector3 target, float waitSeconds)
    {
        Debug.Log("Attack");
        
        //自己定位
        CharController.TargetForPosition = position;
        CharController.Commond(CharController.E_COMMOND.POSITION);

        //面朝目标
        CharController.TargetForLookAt = target;
        CharController.Commond(CharController.E_COMMOND.LOOKAT);
        
        //0.3秒内的等待不执行
        if (waitSeconds > 0.3f)
        {
            CharController.WaitForSeconds = waitSeconds;
            CharController.WaitForCommond = CharController.E_COMMOND.ATTACK;
            CharController.Commond(CharController.E_COMMOND.WAIT);
            return;
        }

        CharController.Commond(CharController.E_COMMOND.ATTACK);
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    public void StopMove()
    {
        CharController.Commond(CharController.E_COMMOND.STOPMOVE);
    }

    /// <summary>
    /// 死亡，坦克变身为救护车，移动到某点
    /// </summary>
    /// <param name="deadChangeEntityID">变身后的实体ID，服务器定义。坦克死亡后变成救护车</param>
    /// <param name="deadChangeObjType">实体的类型，比如救护车类型定义</param>
    /// <param name="deadPosition">死亡位置，在哪里死的</param>
    /// <param name="deadTarget">死亡后跑到哪里去</param>
    /// <param name="deadMoveSpeed">跑的速度</param>
    public void Dead(int deadChangeEntityID, BattleObjManager.E_BATTLE_OBJECT_TYPE deadChangeObjType,
        Vector3 deadPosition, Vector3 deadTarget, float deadMoveSpeed)
    {
        CharController.DeadChangeEntityID = deadChangeEntityID;
        CharController.DeadPosition       = deadPosition;
        CharController.DeadTarget         = deadTarget;
        CharController.DeadMoveSpeed      = deadMoveSpeed;
        CharController.DeadChangeObjType  = deadChangeObjType;
        CharController.Commond(CharController.E_COMMOND.DEAD);
    }
    //API for AI end




}
