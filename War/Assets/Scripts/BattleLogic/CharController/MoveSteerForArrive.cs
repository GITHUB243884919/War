/// <summary>
/// 移动行为：根据速度到达指定目标
/// 
/// autor: fanzhengyong
/// date: 2017-02-22 
/// </summary>
using UnityEngine;
using System.Collections;

public class MoveSteerForArrive : MoveSteer
{
    private bool m_isPlanar = true;

    //到达区，距离多少算到了，可以设置为0;
    public float m_nearDistance = 0.5f;

    public Vector3 m_startPos;

    //目标位置
    public Vector3 m_endPos;

    //容器
    private MoveSteers m_steers;

    public delegate void DelegateArrived();
    public DelegateArrived m_arrivedCallback = null;

    private float m_speed = 5.0f;
    private Vector3 m_force = Vector3.zero;
    private Vector3 m_uForce = Vector3.zero;
    public CharController m_ctr;

    public MoveSteerForArrive(MoveSteers steers, CharController ctr)
    {
        m_steers = steers;
        m_ctr = ctr;
    }

    public override void Init()
    {
        //Debug.Log("MoveSteerForArrive Init");
        //m_startPos = m_ctr.transform.position;
        //m_endPos = m_ctr.TargetForArrive;
        //Vector3 toTarget = m_endPos - m_startPos;
        //m_force = toTarget.normalized * m_speed;
        //m_ctr.transform.LookAt(m_endPos);
        Active = false;
    }

    public override Vector3 Force()
    {
        if (!Active)
        {
            Debug.Log("MoveSteerForArrive 未激活");
            return Vector3.zero;
        }

        Vector3 toTarget = Vector3.zero;
        //if (m_force == Vector3.zero)
        {
            m_startPos = m_ctr.transform.position;
            m_endPos = m_ctr.TargetForArrive;
            toTarget = m_endPos - m_startPos;
            m_force = toTarget.normalized * m_speed;
            m_ctr.transform.LookAt(m_endPos);
            //Debug.Log("m_force == Vector3.zero "
            //    + " m_startPos " + m_startPos
            //    + " m_endPos " + m_endPos
            //    + " m_force " + m_force
            //);
            
        }

        if (m_isPlanar)
        {
            toTarget.y = 0;
        }

        float distance = toTarget.magnitude;

        //到了
        if (distance <= m_nearDistance)
        {
            m_force = Vector3.zero;
            Active = false;
            m_steers.Active = false;

            //Debug.Log("Arrive 到了终点 " + m_endPos + " time " + Time.realtimeSinceStartup);

            if (m_arrivedCallback != null)
            {
                m_arrivedCallback();
            }
        }
        //Debug.Log("BattleAIArrive2 Force " + force);
        return m_force;
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(target, slowDownDistance);
    }
}
