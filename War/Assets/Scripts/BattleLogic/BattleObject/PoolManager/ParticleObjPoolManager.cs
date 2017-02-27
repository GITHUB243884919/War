/// <summary>
/// 战斗中粒子对象管理器,用于角色的特效
/// author: fanzhengyong
/// date: 2017-02-27
/// 
/// 持有多个对象池，按资源路径放到map中
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleObjPoolManager
{
    private Dictionary<string, QObjPool<GameObject>> m_pools =
        new Dictionary<string, QObjPool<GameObject>>();

    private static ParticleObjPoolManager s_Instance = null;
    public static ParticleObjPoolManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new ParticleObjPoolManager();
            }
            return s_Instance;
        }
    }

    public void Init()
    {

    }

    public GameObject BorrowObj(string path)
    {
        bool retCode = false;
        GameObject obj = null;
        QObjPool<GameObject> pool = null;
        m_pools.TryGetValue(path, out pool);
        if (pool == null)
        {
            //没有对应的pool就创建一个
            QObjPoolForParticle creator =
                new QObjPoolForParticle(path, 32);
            pool = new QObjPool<GameObject>();
            retCode = pool.Init(creator, null);
            if (!retCode)
            {
                Debug.LogError("QObjPool 初始失败 " + path);
                return obj;
            }
        }

        obj = pool.BorrowObj();

        return obj;
    }

    public void ReturnObj(GameObject obj, string path)
    {
        QObjPool<GameObject> pool = null;
        m_pools.TryGetValue(path, out pool);
        if (pool == null)
        {
            Debug.LogWarning(path + " 对象不在对象池管理范围内");
            return;
        }

        pool.ReturnObj(obj);
    }

}
