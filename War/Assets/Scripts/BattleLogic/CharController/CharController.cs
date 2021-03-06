﻿/// <summary>
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
        OPEN,     //展开
        ATTACK,   //攻击
        ATTACKED, //受击
        DEAD      //死亡
    }

    private CharObj m_charObj = null;
    public CharObj  CharObj 
    {
        get
        {
            return m_charObj;
        }
        set
        {
            m_charObj = value;
            if (
                (m_commond == null)
                && (m_charObj != null)
            )
            {
                m_commond = CharCommondFactory.CreateCommond(this, m_charObj.Type);
                m_commond.Init();
                m_commond.StopAnimator();
                m_commond.StopEffect();
            }
        }
    }

    public delegate void CommondCallback();

    public Dictionary<E_COMMOND, CommondCallback> m_commondCallbacks =
        new Dictionary<E_COMMOND, CommondCallback>();


    public MoveSteers.StopMoveCallback m_outStopCallback = null;

    private CharCommond        m_commond = null;
    public  MoveSteers         m_steers  = null;

    //位置数据
    public PositionData m_positionData = new PositionData();
    public PositionData PositionData{ get{return m_positionData;} }

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

    public void Init(CharObj charObj)
    {
        GameObject = gameObject;
        PositionData.GameObject = GameObject;
        Transform  = transform;
        PositionData.Transform = Transform;
        Animator   = GetComponent<Animator>();
        CharObj    = charObj;

        RegCommond(E_COMMOND.NONE, OnNone);
        
        //与位置相关
        RegCommond(E_COMMOND.POSITION,   OnPositon);
        RegCommond(E_COMMOND.LOOKAT,     OnLookAt);
        RegCommond(E_COMMOND.ARRIVE,     OnArrive);
        RegCommond(E_COMMOND.STOPMOVE,   OnStopMove);

        //等待
        WaitForCommond = E_COMMOND.NONE;
        RegCommond(E_COMMOND.WAIT, OnWait);

        m_steers = new MoveSteers();
        MoveSteers.E_STEER_TYPE [] steerType;
        steerType = new MoveSteers.E_STEER_TYPE [1]{
            MoveSteers.E_STEER_TYPE.ARRIVE
        };
        //m_steers.Init(steerType, PositionData, m_commond.OnArrived);
        m_steers.Init(steerType, PositionData);
    }

    /// <summary>
    /// 不活跃操作。
    /// 停止一切MoveSteer的AI
    /// 停止一切协程
    /// 停止一切计时器
    /// </summary>
    public void Deactive()
    {
        StopCoroutine(WaitTimer(WaitForCommond, WaitForSeconds));
        if (m_commond != null)
        {
            m_commond.DeactiveEffects();
        }
        OnStopMove();

        m_positionData.TargetForPosition  = Vector3.zero;
        m_positionData.TargetForArrive    = Vector3.zero;
        m_positionData.SpeedForArrive     = 0f;
        m_positionData.TargetForLookAt    = Vector3.zero;    
        WaitForSeconds     = 0f; 
        WaitForCommond     = E_COMMOND.NONE;
        DeadChangeEntityID = 0;
        DeadPosition       = Vector3.zero;
        DeadTarget         = Vector3.zero;
        DeadMoveSpeed      = 0f;  
    }

    /// <summary>
    /// 切换命令时的准备
    /// 停止协程
    /// 停止动画
    /// 停止特效
    /// </summary>
    public void PrepareSwitchCommond()
    {
        //StopCoroutine(WaitTimer(WaitForCommond, WaitForSeconds));
        //if (Animator != null)
        //{
        //    Animator.speed = 0f;
        //}
        //if (m_commond != null)
        //{
        //    m_commond.DeactiveEffects();
        //}
    }

    private void Release()
    {
        GameObject = null;
        Transform  = null;
        Animator   = null;
        CharObj    = null;

        if (m_steers != null)
        {
            m_steers.Release();
            m_steers = null;
        }

        if (m_commondCallbacks != null)
        {
            m_commondCallbacks.Clear();
            m_commondCallbacks = null;
        }
    }

    public void Commond(E_COMMOND commond)
    {
        CommondCallback callback = null;
        m_commondCallbacks.TryGetValue(commond, out callback);
        if (callback == null)
        {
            LogMediator.LogWarning("CharController不存在Commond " + commond);
            return;
        }
        //LogMediator.Log("Commond = " + commond + " " + TargetForArrive 
        //    + " Time " + Time.realtimeSinceStartup);
        callback();
    }

    public void RegCommond(E_COMMOND commond, CommondCallback callback)
    {
        CommondCallback _callback = null;
        m_commondCallbacks.TryGetValue(commond, out _callback);
        if (_callback != null)
        {
            LogMediator.LogWarning("CharController已存在Commond " + commond);
            return;
        }

        m_commondCallbacks.Add(commond, callback);
    }

    private void OnNone()
    {
        LogMediator.Log("OnNone " + Time.realtimeSinceStartup);
    }

    private void OnPositon()
    {
        //LogMediator.Log("OnPositon");
        Transform.position = m_positionData.TargetForPosition;
    }

    private void OnLookAt()
    {
        //LogMediator.Log("OnLookAt " + TargetForLookAt);
        Transform.LookAt(m_positionData.TargetForLookAt);
    }

    private void OnArrive()
    {
        //先定位到起点
        OnPositon();

        //转向
        //LogMediator.Log("OnArrive LookAt " + TargetForArrive);
        Transform.LookAt(m_positionData.TargetForArrive);

        //特效
        //LogMediator.Log(CharType.ToString());
        m_commond.MoveEffect();
        //动画
        m_commond.MoveAnimator();

        if (m_positionData.SpeedForArrive == 0f)
        {
            LogMediator.LogWarning("速度为0，不会执行 " +
                MoveSteers.E_STEER_TYPE.ARRIVE.ToString());
            return;
        }

        //if (m_positionData.TargetForArrive == m_positionData.TargetForPosition)
        //{
        //    LogMediator.LogWarning("起点和终点相同，不会执行 " + 
        //        MoveSteers.E_STEER_TYPE.ARRIVE.ToString());
        //    return;
        //}
        //Debug.Log("Add stop callback");
        m_steers.m_stopMoveCallback  = null;
        m_steers.m_stopMoveCallback += m_commond.OnArrived;
        m_steers.m_stopMoveCallback += m_outStopCallback;
        m_steers.Active  = true;
    }

    private void OnStopMove()
    {
        //LogMediator.Log("CharController OnStopMove");
        if (m_steers == null)
        {
            return;
        }

        if (m_steers != null)
        {
            m_steers.Active = false;
        }

        if (m_commond != null)
        {
            m_commond.StopAnimator();
            m_commond.StopEffect();
        }
    }

    private void OnWait()
    {
        //Debug.Log("OnWait " + Time.realtimeSinceStartup);
        StartCoroutine(WaitTimer(WaitForCommond, WaitForSeconds));
    }

    IEnumerator WaitTimer(E_COMMOND waitForCommond, float waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);
        //Debug.Log("WaitTimer " + waitForCommond.ToString() + " " + waitForSeconds);

        Commond(waitForCommond);
    }

    //Unity
	void Awake () 
    {
        //Init();
	}
	
	void Update () 
    {
        if (m_commond != null)
        {
            m_commond.Update();
        }

        if ((m_steers != null) && (m_steers.Active))
        {
            m_steers.Update();
        }
	}

    void FixedUpdate()
    {
        if ((m_steers != null) && (m_steers.Active))
        {
            m_steers.FixedUpdate();
        }
    }

    void OnDestroy()
    {
        Release();
    }
}
