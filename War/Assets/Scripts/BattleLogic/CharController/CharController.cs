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
    public int ServerEntityID { get; set; }
    public enum E_COMMOND
    {
        NONE,
        //以下是位置相关
        POSITION, //直接定位到某个位置,放在一个看不见的地方就实现了隐藏Vector3(0f, -10f, 0f)
        ARRIVE,   //移动到某个位置，提供终点

        //特殊类型
        WAIT,     //等待，需要提供等待多少秒和等待完了后执行什么（E_COMMOND）。如果只是等待那么后续传NONE
        
        //以下是动画相关
        IDLE,
        ATTACK,
        ATTACKED
    }

    public delegate void CommondCallback();

    public Dictionary<E_COMMOND, CommondCallback> m_commondCallbacks =
        new Dictionary<E_COMMOND, CommondCallback>();

    TankCommond m_commond;
    MoveSteers  m_steers;
    //位置数据
    public Vector3     TargetForPosition    { get; set; }
    public Vector3     TargetForArrive      { get; set; }

    //等待数据
    public float       WaitForSecond        { get; set; }
    public E_COMMOND   WaitForCommond       { get; set; }

    public GameObject  GameObject           { get; private set; }
    public Transform   Transform            { get; private set; }
    public Animator    Animator             { get; private set; }

    private void Init()
    {
        GameObject = gameObject;
        Transform  = transform;
        Animator   = GetComponent<Animator>();

        RegCommond(E_COMMOND.NONE, OnNone);
        
        //与位置相关
        RegCommond(E_COMMOND.POSITION, OnPositon);
        RegCommond(E_COMMOND.ARRIVE, OnArrive);

        //等待
        WaitForCommond = E_COMMOND.NONE;
        RegCommond(E_COMMOND.WAIT, OnWait);
        
        m_commond  = new TankCommond(this);
        m_commond.Init();

        m_steers   = new MoveSteers(this);
        m_steers.Init();
    }

    private void Realse()
    {
        GameObject = null;
        Transform  = null;
        Animator   = null;

        //foreach (KeyValuePair<E_COMMOND, CommondCallback> pair 
        //    in m_commondCallbacks)
        //{
        //    m_commondCallbacks[pair.Key] = null;
        //}
        m_commondCallbacks.Clear();
        //m_commondCallbacks = null;
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
        Transform.position = TargetForPosition;
    }

    private void OnArrive()
    {
        MoveSteer arrive = m_steers.m_steers[MoveSteers.E_STEER_TYPE.ARRIVE];
        arrive.Active    = true;
        m_steers.Active  = true;
    }

    private void OnWait()
    {
        Debug.Log("OnWait " + Time.realtimeSinceStartup);
        StartCoroutine("WaitTimer");
    }

    IEnumerator WaitTimer()
    {
        yield return new WaitForSeconds(WaitForSecond);
        Commond(WaitForCommond);
    }

    //Unity
	void Awake () 
    {
        Init();
	}
	
	void Update () 
    {
        m_commond.Update();
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
