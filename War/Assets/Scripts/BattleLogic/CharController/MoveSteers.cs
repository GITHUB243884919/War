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
        ARRIVE
    }

    public Dictionary<E_STEER_TYPE, MoveSteer> m_steers =
        new Dictionary<E_STEER_TYPE, MoveSteer>();

    public float m_interval = 0.2f;
    public bool m_isPlanar = true;
    public float m_timer = 0.0f;
    public bool m_displayTrack = true;
    public Vector3 m_moveDistance;
    public Vector3 m_steeringForce;
    public CharController m_ctr;

    public bool Active { get; set; }

    public MoveSteers(CharController ctr)
    {
        m_ctr = ctr;
    }

    public void Init()
    {
        //Debug.Log("MoveSteers Init");
        Active = false;
        m_timer = 0.0f;

        MoveSteer steerArrive = new MoveSteerForArrive(this, m_ctr);
        m_steers.Add(E_STEER_TYPE.ARRIVE, steerArrive);
        steerArrive.Init();
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
        Profiler.BeginSample("开始计算所有给力函数");
        foreach (KeyValuePair<E_STEER_TYPE, MoveSteer> pair in m_steers)
        {
            if (pair.Value.Active)
            {
                m_steeringForce += pair.Value.Force() * pair.Value.m_weight;
            }
        }
        Profiler.EndSample();
    }

    public void FixedUpdate()
    {
        if (!Active)
        {
            return;
        }

        m_moveDistance = m_steeringForce * Time.fixedDeltaTime;
        m_ctr.transform.position += m_moveDistance;

        if (m_displayTrack)
        {
            Debug.DrawLine(m_ctr.transform.position,
                m_ctr.transform.position + m_moveDistance, Color.red, 30.0f);
        }
    }
}
