/// <summary>
/// 移动行为容器
/// autor: fanzhengyong
/// date: 2017-02-22 
/// 
/// 负责计算出容器内有效的（可能是多个）行为类形成的合力，最终对其形成作用
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveSteers
{
    public enum E_STEER_TYPE
    {
        NONE,
        ARRIVE,        //到达
        STATIC_BOMB    //静态轰炸
    }

    public Dictionary<E_STEER_TYPE, MoveSteer> m_steers =
        new Dictionary<E_STEER_TYPE, MoveSteer>();

    public delegate void  StopMoveCallback();
    public StopMoveCallback m_stopMoveCallback;

    public float          m_interval      = 0.2f;
    public float          m_timer         = 0.0f;
    public bool           m_displayTrack  = false;
    public Vector3        m_moveDistance  = Vector3.zero;
    public Vector3        m_steeringForce = Vector3.zero;
    public PositionData   m_positionData  = null;

    private bool m_arrived = false;
    public  bool Arrived 
    { 
        get 
        {
            return m_arrived;
        }
        set 
        {
            if (m_arrived == value)
            {
                return;
            }
            m_arrived = value;
            
            if (!m_arrived)
            {
                return;
            }

            WhenArrived();
        } 
    }

    private bool  m_active = false;
    public  bool  Active 
    {
        get 
        {
            return m_active;
        }
        set 
        {
            if (m_active == value)
            {
                return;
            }
            m_active = value;

            foreach (MoveSteer moveSteer in m_steers.Values)
            {
                moveSteer.Active = value;
            }

        } 
    }

    public void Init(E_STEER_TYPE[] types, PositionData positionData, StopMoveCallback callback)
    {
        //Debug.Log("MoveSteers Init");
        Active  = false;
        m_timer = 0.0f;

        m_positionData = positionData;
        m_stopMoveCallback += callback;

        for (int i = 0; i < types.Length; i++)
        {
            MoveSteer steer = MoveSteerFactory.Create(this, types[i]);
            steer.Init();
            m_steers.Add(E_STEER_TYPE.ARRIVE, steer);
        }
    }

    public void Update()
    {
        if (!Active)
        {
            return;
        }

        m_timer += Time.deltaTime;
        if (m_timer < m_interval)
        {
            return;
        }
        m_timer = 0;

        m_steeringForce = Vector3.zero;
        //Profiler.BeginSample("开始计算所有给力函数");
        foreach (MoveSteer moveSteer in m_steers.Values)
        {
            if (moveSteer.Active)
            {
                m_steeringForce += moveSteer.Force() * moveSteer.m_weight;
            }
        }
        //Profiler.EndSample();
    }

    public void FixedUpdate()
    {
        if (!Active)
        {
            return;
        }

        if (CheckConditionStopMove())
        {
            Active = false;
        }

        m_moveDistance = m_steeringForce * Time.fixedDeltaTime;
        m_positionData.Transform.position += m_moveDistance;

        if (m_displayTrack)
        {
            LogMediator.DrawLine(m_positionData.Transform.position,
                m_positionData.Transform.position + m_moveDistance, Color.red, 30.0f);
        }
    }

    public void Release()
    {
        foreach(MoveSteer moveSteer in m_steers.Values)
        {
            moveSteer.Release();
        }
        m_steers.Clear();
        m_steers = null;

        m_positionData.Release();
        m_positionData = null;

        m_stopMoveCallback = null;
    }

    public void WhenArrived()
    {
        Active = false;

        if (m_stopMoveCallback != null)
        {
            m_stopMoveCallback();
        }
    }

    public bool CheckConditionStopMove()
    {
        bool retCode = false;
        bool result = false;

        foreach (MoveSteer moveSteer in m_steers.Values)
        {
            retCode = moveSteer.ConditionStopMoveInMoveSteers();
            if (retCode)
            {
                result = retCode;
                break;
            }
        }

        return result;
    }
}
