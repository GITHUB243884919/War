/// <summary>
/// 战斗场景示范类。
/// author : fanzhengyong
/// date  : 2017-02-22
/// 
/// 从服务器拿到需要生成坦克的下行消息后，从这个这个类取对象。
/// 从对象的go上取控制组件CharController
/// 调用CharController的方法执行对应的AI，比如移动
/// 
/// 两个重要的接口 取和还
///    public ActiveBattleModelObject2 BorrowBattleModelObj(
///         BattleScene2.E_BATTLE_OBJECT_TYPE type,
///         int serverEntityID, 
///         int serverEntityType)
///         
///    public void ReturnBattleModelObj(ActiveBattleModelObject2 obj)
/// 不能在外面删除,不用了就还回来！！！
///            
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleScene2 : MonoBehaviour 
{
    public enum E_BATTLE_OBJECT_TYPE
    {
        TANK,
        SOLDIER
    }

    private static BattleScene2 s_Instance = null;
    public static BattleScene2 Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(BattleScene2))
                        as BattleScene2;
                if (s_Instance == null)
                    Debug.Log("场景中没有持有BattleScene2的对象");
            }
            return s_Instance;
        }
    }

    private void Init()
    {
        Debug.Log("begin QObjPoolForMeshBakerMgr Init " + Time.realtimeSinceStartup);
        QObjPoolForMeshBakerMgr.Instance.Init();
        Debug.Log("end QObjPoolForMeshBakerMgr Init " + Time.realtimeSinceStartup);
        ActiveBattleModelObjectCache2.Instance.Init();
        Debug.Log("ActiveBattleModelObjectCache2 Init");
    }

    /// <summary>
    /// 在场景取一个对象必须是已知类型，服务器的两个编号的
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serverEntityID"></param>
    /// <param name="serverEntityType"></param>
    /// <returns></returns>
    public ActiveBattleModelObject2 BorrowBattleModelObj(BattleScene2.E_BATTLE_OBJECT_TYPE type,
        int serverEntityID, int serverEntityType)
    {
        ActiveBattleModelObject2 activeObj = null;

        //先从缓存中取
        activeObj = ActiveBattleModelObjectCache2.Instance.Find(serverEntityID);
        if (activeObj != null)
        {
            return activeObj;
        }

        //缓存中没有才从对象池中取
        GameObject obj = QObjPoolForMeshBakerMgr.Instance.BorrowObj(type);

        //从对象池中取出的对象要放入缓存中
        ActiveBattleModelObjectCache2.Instance.Add(serverEntityID, 
            obj, type, out activeObj);

        return activeObj;
    }

    public void ReturnBattleModelObj(ActiveBattleModelObject2 obj)
    {
        //先从缓存中移除
        ActiveBattleModelObjectCache2.Instance.Remove(obj);
        //再还给对象池
        QObjPoolForMeshBakerMgr.Instance.ReturnObj(obj.GameObject, obj.Type);
    }

    void Start()
    {
        Debug.Log("BattleScene2 Start " + Time.realtimeSinceStartup);
        Init();
    } 

	void Update() 
    {
	
	}
}
