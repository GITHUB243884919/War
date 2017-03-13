/// <summary>
/// 角色对象定义(坦克，士兵等)
/// 
/// author: fanzhengyong
/// date: 2017-02-24
/// 
/// 包含了其AI的接口
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharObj
{
    //服务器定义的唯一编号和类型编号
    public int             ServerEntityID { get; set; }
    //public int ServerEntityType { get; set; }

    //角色对应的prefab 坦克，士兵之类的prefab
    public GameObject      GameObject     { get; set; }

    public CharController  CharController { get; set; }

    //对象类型
    public BattleObjManager.E_BATTLE_OBJECT_TYPE Type { get; set; }

    //子对象的pool，<instanceID, path>
    //这里只缓存名字(路径)
    private Dictionary<int, string> m_childsPools =
        new Dictionary<int, string>();

    public CharObj()
    {
        GameObject     = null;
        CharController = null;
    }

    /// <summary>
    /// 检查是否合法
    /// 必须持有GameObject和CharController属性
    /// 如果没有CharController就从GameOject取
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        bool result = false;

        if (GameObject == null)
        {
            Debug.LogError("CharObj对象上没有持有 GameObject,请联系BattleObjManager作者");
            return result;
        }

        if (CharController == null)
        {
            CharController = GameObject.GetComponent<CharController>();
        }
        if (CharController == null)
        {
            Debug.LogError("CharObj对象上没有持有 CharController组件,请联系BattleObjManager作者");
            return result;
        }

        result = true;

        return result;
    }

    /// <summary>
    /// 设置成不活跃
    /// </summary>
    public void Deactive()
    {
        CharController.Deactive();
    }

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
    public void AI_Position(Vector3 position)
    {
        CharControllerMediator.AI_Position(CharController, position);
    }

    /// <summary>
    /// 展开
    /// </summary>
    /// <param name="cctr">在哪里展开</param>
    /// <param name="position">面朝哪里</param>
    /// <param name="lookAt"></param>
    public void AI_Open(Vector3 position, Vector3 lookAt)
    {
        CharControllerMediator.AI_Open(CharController, position, lookAt);
    }

    /// <summary>
    /// 到达
    /// </summary>
    /// <param name="startPoint">起点</param>
    /// <param name="endPoint">终点</param>
    /// <param name="speed">速度</param>
    public void AI_Arrive(Vector3 startPoint, Vector3 endPoint, float speed)
    {
        CharControllerMediator.AI_Arrive(CharController, startPoint, endPoint, speed);
    }

    /// <summary>
    /// 攻击
    /// </summary>
    /// <param name="position">自身位置</param>
    /// <param name="target">目标</param>
    /// <param name="waitSeconds">等待时间（秒）</param>
    public void AI_Attack(Vector3 position, Vector3 target, float waitSeconds)
    {
        //Debug.Log("CharObj AI_Attack " + waitSeconds);
        CharControllerMediator.AI_Attack(CharController, position, target, waitSeconds);
    }

    /// <summary>
    /// 受击
    /// </summary>
    /// <param name="position">在哪里的挨的打</param>
    public void AI_Attacked(Vector3 position)
    {
        CharControllerMediator.AI_Attacked(CharController, position);
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    public void AI_StopMove()
    {
        CharControllerMediator.AI_StopMove(CharController);
    }

    /// <summary>
    /// 死亡，坦克变身为救护车，救护车移动到某点
    /// 如果没有变身功能的死亡，deadPosition必须填正确，其他参数只要符合参数类型即可，底层代码会忽略这些参数
    /// </summary>
    /// <param name="deadChangeEntityID">变身后的实体ID，服务器定义。坦克死亡后变成救护车</param>
    /// <param name="deadChangeObjType">实体的类型，比如救护车类型定义</param>
    /// <param name="deadPosition">死亡位置，在哪里死的</param>
    /// <param name="deadTarget">死亡后跑到哪里去</param>
    /// <param name="deadMoveSpeed">跑的速度</param>
    public void AI_Dead(int deadChangeEntityID, BattleObjManager.E_BATTLE_OBJECT_TYPE deadChangeObjType,
        Vector3 deadPosition, Vector3 deadTarget, float deadMoveSpeed)
    {
        CharControllerMediator.AI_Dead(CharController, deadChangeEntityID, deadChangeObjType,
            deadPosition, deadTarget, deadMoveSpeed);
    }
    //API for AI end
}
