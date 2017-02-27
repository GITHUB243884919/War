/// <summary>
/// 战斗中模型使用的对象池管理器
/// author: fanzhengyong
/// date: 2017-02-22 
/// 
/// 持有多个对象池，这些对象都是meshbaker合并后的，分别用于坦克，士兵。。。。
/// 取用对象BorrowObj，归还用ReturnObj，
/// BorrowObj的GameObject不能删除！！！
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QObjPoolForMeshBakerMgr
{
    Dictionary<BattleScene2.E_BATTLE_OBJECT_TYPE, QObjPool<GameObject>> m_pools =
        new Dictionary<BattleScene2.E_BATTLE_OBJECT_TYPE, QObjPool<GameObject>>();

    private static QObjPoolForMeshBakerMgr s_Instance = null;
    public static QObjPoolForMeshBakerMgr Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new QObjPoolForMeshBakerMgr();
            }
            return s_Instance;
        }
    }

    public  void Init()
    {
        int countTank = 32;
        string[] pathsTank = new string[4] 
        {
            "TankRuntime_Bake/TankMeshBaker",
            "TankRuntime_Bake/TankRuntime_Bake-mat",
            "TankRuntime_Bake/TankRuntime_Bake",
            "TankRuntime_Bake/Tank_Seed"
        };
        Debug.Log("Init Tank pool");
        InitBattleObjPool(pathsTank,
            BattleScene2.E_BATTLE_OBJECT_TYPE.TANK, countTank);

        //int countSoldier = 32;
        //string[] pathsSoldier = new string[4] 
        //{
        //    "SoldierRuntime_Bake/SoldierMeshBaker",
        //    "SoldierRuntime_Bake/SoldierRuntime_Bake-mat",
        //    "SoldierRuntime_Bake/SoldierRuntime_Bake",
        //    "SoldierRuntime_Bake/Soldier_Seed"
        //};
        //Debug.Log("Init Soldier pool");
        //InitBattleObjPool(pathsSoldier,
        //    BattleScene2.E_BATTLE_OBJECT_TYPE.SOLDIER, countSoldier);
    }

    public GameObject BorrowObj(BattleScene2.E_BATTLE_OBJECT_TYPE type)
    {
        GameObject obj = null;
        QObjPool<GameObject> pool = null;
        m_pools.TryGetValue(type, out pool);
        if (pool == null)
        {
            return obj;
        }

        obj = pool.BorrowObj();

        return obj;
    }

    public void ReturnObj(GameObject obj, BattleScene2.E_BATTLE_OBJECT_TYPE type)
    {
        QObjPool<GameObject> pool = null;
        m_pools.TryGetValue(type, out pool);
        if (pool == null)
        {
            return;
        }

        pool.ReturnObj(obj);
    }

    private void InitBattleObjPool(string[] paths, BattleScene2.E_BATTLE_OBJECT_TYPE type, int count)
    {
        QObjCreatorFactory<GameObject> creatorFactory = new QObjCreatorFactoryForMeshBaker(
            paths, BattleScene2.E_BATTLE_OBJECT_TYPE.TANK, count);

        QObjPool<GameObject> pool = new QObjPool<GameObject>();
        pool.Init(null, creatorFactory);

        m_pools.Add(type, pool);
    }
}
