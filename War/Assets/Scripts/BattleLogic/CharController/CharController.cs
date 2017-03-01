/// <summary>
/// 适用于主要由外部进行控制的角色控制类实现
/// author : fanzhengyong
/// date  : 2017-02-22
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharController : MonoBehaviour 
{
    //角色能执行的命令
    public enum E_COMMOND
    {
        NONE,     //空命令，什么都不做。

        //位置相关
        POSITION, //直接定位到某个位置,放在一个看不见的地方就实现了隐藏Vector3(0f, -10f, 0f)
        LOOKAT,   //面朝一个方向，提供目标向量
        ARRIVE,   //移动到某个位置，提供起点，终点，速度
        STOPMOVE, //停止移动

        //特殊类型
        WAIT,     //等待，需要提供等待多少秒和等待完了后执行什么（E_COMMOND类型）。如果只是等待那么后续传NONE

        //以下是动画相关
        IDLE,     //待机
        ATTACK,   //攻击
        ATTACKED, //受击
        DEAD      //死亡
    }

    public int ServerEntityID { get; set; }
    
    [HideInInspector]
    public BattleObjManager.E_BATTLE_OBJECT_TYPE m_charType;
    public BattleObjManager.E_BATTLE_OBJECT_TYPE CharType 
    {
        get
        {
            return m_charType;
        }
        set 
        {
            m_commond = CharCommondFactory.CreateCommond(this, value);
            m_commond.Init();
        }
    }

    public delegate void CommondCallback();

    public Dictionary<E_COMMOND, CommondCallback> m_commondCallbacks =
        new Dictionary<E_COMMOND, CommondCallback>();

    private CharCommond        m_commond = null;
    private MoveSteers         m_steers  = null;

    //位置数据
    public Vector3     TargetForPosition    { get; set; }
    public Vector3     TargetForArrive      { get; set; }
    public float       SpeedForArrive       { get; set; }
    public Vector3     TargetForLookAt      { get; set; }

    //等待数据
    public float       WaitForSeconds       { get; set; }
    public E_COMMOND   WaitForCommond       { get; set; }

    //死亡数据
    //变身后的实体ID，服务器定义。坦克死亡后变成救护车
    public int         DeadChangeEntityID   { get; set; }
    //死亡位置，在哪里死的
    public Vector3     DeadPosition         { get; set; }
    //死亡后跑到哪里去
    public Vector3     DeadTarget           { get; set; }
    //跑的速度
    public float       DeadMoveSpeed        { get; set; }
    //实体的类型，比如救护车类型定义
    public BattleObjManager.E_BATTLE_OBJECT_TYPE DeadChangeObjType { get; set; }


    public GameObject  GameObject           { get; private set; }
    public Transform   Transform            { get; private set; }
    public Animator    Animator             { get; private set; }

    //for test begin
    public float StartArrive { get; set; }
    //for test end


    private void Init()
    {
        GameObject = gameObject;
        Transform  = transform;
        Animator   = GetComponent<Animator>();

        RegCommond(E_COMMOND.NONE, OnNone);
        
        //与位置相关
        RegCommond(E_COMMOND.POSITION,   OnPositon);
        RegCommond(E_COMMOND.LOOKAT,     OnLookAt);
        RegCommond(E_COMMOND.ARRIVE,     OnArrive);
        RegCommond(E_COMMOND.STOPMOVE,   OnStopMove);

        //等待
        WaitForCommond = E_COMMOND.NONE;
        RegCommond(E_COMMOND.WAIT, OnWait);
        
        //m_commond  = new TankCommond(this);
        //m_commond.Init();
        //m_commond = CharCommondFactory.CreateCommond(this, m_charType);
        //m_commond.Init();

        m_steers   = new MoveSteers(this);
        m_steers.Init();
    }

    private void Realse()
    {
        GameObject = null;
        Transform  = null;
        Animator   = null;

        m_commondCallbacks.Clear();
    }

    public void Commond(E_COMMOND commond)
    {
        CommondCallback callback = null;
        m_commondCallbacks.TryGetValue(commond, out callback);
        if (callback == null)
        {
            Debug.LogWarning("CharController不存在Commond " + commond);
            return;
        }
        //Debug.Log("Commond = " + commond + " " + TargetForArrive 
        //    + " Time " + Time.realtimeSinceStartup);
        callback();
    }

    public void RegCommond(E_COMMOND commond, CommondCallback callback)
    {
        CommondCallback _callback = null;
        m_commondCallbacks.TryGetValue(commond, out _callback);
        if (_callback != null)
        {
            Debug.LogWarning("CharController已存在Commond " + commond);
            return;
        }

        m_commondCallbacks.Add(commond, callback);
    }

    private void OnNone()
    {
        Debug.Log("OnNone " + Time.realtimeSinceStartup);
    }

    private void OnPositon()
    {
        Debug.Log("OnPositon");
        Transform.position = TargetForPosition;
    }

    private void OnLookAt()
    {
        Debug.Log("OnLookAt");
        Transform.LookAt(TargetForLookAt);
    }

    private void OnArrive()
    {
        //Debug.Log("OnArrive " + Time.realtimeSinceStartup);
        StartArrive = Time.realtimeSinceStartup;
        //先定位到起点
        OnPositon();
        //转向
        Transform.LookAt(TargetForArrive);
        MoveSteer arrive = m_steers.m_steers[MoveSteers.E_STEER_TYPE.ARRIVE];
        arrive.Active    = true;
        m_steers.Active  = true;
    }

    private void OnStopMove()
    {
        MoveSteer arrive = m_steers.m_steers[MoveSteers.E_STEER_TYPE.ARRIVE];
        arrive.Active    = false;
        m_steers.Active  = false;
    }

    private void OnWait()
    {
        Debug.Log("OnWait " + Time.realtimeSinceStartup);
        StartCoroutine("WaitTimer");
    }

    IEnumerator WaitTimer()
    {
        yield return new WaitForSeconds(WaitForSeconds);
        Commond(WaitForCommond);
    }

    //Unity
	void Awake () 
    {
        Init();
	}
	
	void Update () 
    {
        if (m_commond != null)
        {
            m_commond.Update();
        }
        
        if (m_steers.Active)
        {
            m_steers.Update();
        }
	}

    void FixedUpdate()
    {
        if (m_steers.Active)
        {
            m_steers.FixedUpdate();
        }
    }

    void OnDestroy()
    {
        Realse();
    }
}
