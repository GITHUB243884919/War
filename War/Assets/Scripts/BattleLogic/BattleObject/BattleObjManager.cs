/// <summary>
/// 战斗对象管理
/// author : fanzhengyong
/// date  : 2017-02-24
/// 
/// 从服务器拿到需要生成坦克的下行消息后，从这个这个类取对象。
/// 从对象的go上取控制组件CharController
/// 调用CharController的方法执行对应的AI，比如移动
/// 
/// CharObjCache 
/// CharObjPoolManager
/// BNGObjPoolManager
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleObjManager : MonoBehaviour 
{
    public enum E_BATTLE_OBJECT_TYPE
    {
        TANK,
        SOLDIER
    }
    //for test begin
    public int EffectCount { get; set; }
    //for test end
    private static BattleObjManager s_Instance = null;
    public static BattleObjManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(BattleObjManager))
                        as BattleObjManager;
                if (s_Instance == null)
                {
                    Debug.LogError("场景中没有持有BattleObjManager的对象");
                }
            }
            return s_Instance;
        }
    }

    private void Init()
    {
        //for test begin
        EffectCount = 0;
        //for test end
        //Debug.Log("begin CharObjPoolManager Init " + Time.realtimeSinceStartup);
        CharObjPoolManager.Instance.Init();
        //Debug.Log("end CharObjPoolManager Init " + Time.realtimeSinceStartup);
        CharObjCache.Instance.Init();
        //Debug.Log("CharObjCache Init");
        //BNGObjPoolManager.Instance.Init();
        //Debug.Log("BNGObjPoolManager Init");
        ParticleObjPoolManager.Instance.Init();
        //Debug.Log("ParticleObjPoolManager Init");
    }

    /// <summary>
    /// 在场景取一个对象必须是已知类型，服务器的两个编号的
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serverEntityID"></param>
    /// <param name="serverEntityType"></param>
    /// <returns></returns>
    public CharObj BorrowCharObj(BattleObjManager.E_BATTLE_OBJECT_TYPE type,
        int serverEntityID, int serverEntityType)
    {
        bool    retCode = false;
        CharObj obj     = null;

        //Debug.Log("BorrowCharObj " + serverEntityID);
        //先从缓存中取
        obj = CharObjCache.Instance.Find(serverEntityID);
        if (obj != null)
        {
            //Debug.Log("缓存中找到 " + serverEntityID);
            obj.InActive();
            return obj;
        }

        //缓存中没有才从对象池中取
        obj = CharObjPoolManager.Instance.BorrowObj(type);
        if (obj == null)
        {
            Debug.LogError("CharObjPoolManager.Instance.BorrowObj 取到空Obj " + type.ToString());
            return obj;
        }

        retCode = obj.IsValid();
        if (!retCode)
        {
            Debug.LogError("取到CharObj非法，请联系BattleObjManager作者");
            return obj;
        }

        //为CharObj打上身份证号:）
        obj.ServerEntityID                = serverEntityID;
        obj.CharController.ServerEntityID = serverEntityID;
        obj.CharController.CharType       = type;

        //从对象池中取出的对象要放入缓存中
        CharObjCache.Instance.Add(obj);

        return obj;
    }

    public void ReturnCharObj(CharObj obj)
    {
        obj.InActive();
        //先从缓存中移除
        CharObjCache.Instance.Remove(obj);
        //再还给对象池
        CharObjPoolManager.Instance.ReturnObj(obj);
    }

    //public GameObject BorrowBNGObj(string path)
    //{
    //    GameObject obj = null;
    //    obj = BNGObjPoolManager.Instance.BorrowObj(path);
    //    return obj;
    //}

    //public void ReturnBNGObj(GameObject obj, string path)
    //{
    //    BNGObjPoolManager.Instance.ReturnObj(obj, path);
    //}

    public GameObject BorrowParticleObj(string path)
    {
        GameObject obj = null;
        obj = ParticleObjPoolManager.Instance.BorrowObj(path);
        return obj;
    }

    public void ReturnParticleObj(GameObject obj, string path)
    {
        ParticleObjPoolManager.Instance.ReturnObj(obj, path);
    }

    //Unity
    void Start()
    {
        //Debug.Log("BattleObjManager Start " + Time.realtimeSinceStartup);
        Init();
    }

    //for test begin
    float timer    = 0f;
    float interval = 0.2f;
    //for test end
	void Update() 
    {
        //for test begin
        timer += Time.deltaTime;
        if (timer < interval)
        {
            return;
        }
        timer = 0;

        if (EffectCount % 32 == 0)
        {
            Debug.Log("EffectCount " + EffectCount);
        }
        //for test end
	}
}
