/// <summary>
/// 轰炸行为：自由落体
/// 
/// author: fanzhengyong
/// date: 2017-04-05
/// 
/// 名字中包含Static的意思是目标是静止的
/// </summary>
using UnityEngine;
using System.Collections;

public class MoveSteerForStaticBomb : MoveSteer
{
    //到达区，距离多少算到了;
    public float               m_nearDistance     = 0.1f;
    private Vector3            m_force            = Vector3.zero;
    private Vector3            m_toTarget         = Vector3.zero;

    private static readonly float    G            = 9.8f;
    private float              m_speed            = 0f;
    private float              m_lastForceTime    = 0f;
    private float              m_interval         = 0f;
    private Vector3            m_toTargetDir      = Vector3.zero;

    public MoveSteerForStaticBomb(MoveSteers steers)
        : base(steers)
    {
    }

    public override void Init()
    {
        Active = false;

        //m_toTarget = m_steers.m_positionData.TargetForArrive -
        //    m_steers.m_positionData.Transform.position;
        //m_toTargetDir = m_toTarget.normalized;
    }

    public override Vector3 Force()
    {
        if (!Active)
        {
            //LogMediator.Log("MoveSteerForStaticBomb 未激活");
            return Vector3.zero;
        }

        if (m_lastForceTime == 0f)
        {
            m_lastForceTime = Time.realtimeSinceStartup;
            return Vector3.zero;
        }
        m_toTarget = m_steers.m_positionData.TargetForArrive -
            m_steers.m_positionData.Transform.position;
        m_toTargetDir = m_toTarget.normalized;


        m_interval  = Time.realtimeSinceStartup - m_lastForceTime;
        m_speed    += G * m_interval;
        m_force     = m_toTargetDir * m_speed;
        m_lastForceTime = Time.realtimeSinceStartup;

        m_toTarget  = m_steers.m_positionData.TargetForArrive -
            m_steers.m_positionData.Transform.position;

        if (m_toTarget.magnitude <= m_nearDistance)
        {
            m_force = Vector3.zero;
            m_steers.Arrived = true;
        }

        if (ConditionStopMoveInMoveSteers())
        {
            m_force = Vector3.zero;
            m_steers.Arrived = true;
        }

        return m_force;
    }

    public override bool ConditionStopMoveInMoveSteers()
    {
        bool result = false;
        //Debug.Log(m_steers.m_positionData != null);
        //Debug.Log(m_steers.m_positionData.Transform != null);
        result = m_steers.m_positionData.Transform.position.y 
            <= m_steers.m_positionData.TargetForArrive.y;

        return result;
    }
}
