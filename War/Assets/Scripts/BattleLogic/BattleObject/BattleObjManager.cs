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
                    Debug.LogError("场景中没有持有BattleObjManager的对象");
            }
            return s_Instance;
        }
    }

    private void Init()
    {
        Debug.Log("begin CharObjPoolManager Init " + Time.realtimeSinceStartup);
        CharObjPoolManager.Instance.Init();
        Debug.Log("end CharObjPoolManager Init " + Time.realtimeSinceStartup);
        CharObjCache.Instance.Init();
        Debug.Log("CharObjCache Init");
        //BNGObjPoolManager.Instance.Init();
        //Debug.Log("BNGObjPoolManager Init");
        ParticleObjPoolManager.Instance.Init();
        Debug.Log("ParticleObjPoolManager Init");
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
        CharObj charObj = null;

        //先从缓存中取
        charObj = CharObjCache.Instance.Find(serverEntityID);
        if (charObj != null)
        {
            return charObj;
        }

        //缓存中没有才从对象池中取
        charObj = CharObjPoolManager.Instance.BorrowObj(type);
        if (charObj == null)
        {
            Debug.LogError("CharObjPoolManager.Instance.BorrowObj 取到空Obj" + type);
            return charObj;
        }
        
        //对象池中取的要打上ServerEntityID
        charObj.CharController = charObj.GameObject.GetComponent<CharController>();
        if (charObj.CharController == null)
        {
            //注意，这里没有取到组件，但是已经从pool借走了！！！
            Debug.LogError("对象上没有持有 CharController组件");
            return charObj;

        }
        charObj.CharController.ServerEntityID = serverEntityID;

        //从对象池中取出的对象要放入缓存中
        CharObjCache.Instance.Add(charObj);

        return charObj;
    }

    public void ReturnBattleModelObj(CharObj obj)
    {
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

    void Start()
    {
        Debug.Log("BattleObjManager Start " + Time.realtimeSinceStartup);
        Init();
    } 

	void Update() 
    {
	
	}
}
