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
    //到达区，距离多少算到了;
    public float               m_nearDistance     = 0.1f;
    private Vector3            m_force            = Vector3.zero;
    private Vector3            m_toTarget         = Vector3.zero;

    public MoveSteerForArrive(MoveSteers steers)
        : base(steers)
    {
    }

    public override void Init()
    {
        Active = false;
    }

    public override Vector3 Force()
    {
        if (!Active)
        {
            LogMediator.Log("MoveSteerForArrive 未激活");
            return Vector3.zero;
        }

        m_toTarget = m_steers.m_positionData.TargetForArrive -
            m_steers.m_positionData.Transform.position;
        m_force = m_toTarget.normalized * m_steers.m_positionData.SpeedForArrive;
        if (m_force.magnitude > m_toTarget.magnitude)
        {
            m_force = m_toTarget;
        }

        if (m_toTarget.magnitude <= m_nearDistance)
        {
            m_force         = Vector3.zero;
            m_steers.Arrived = true;
        }

        return m_force;
    }
}
