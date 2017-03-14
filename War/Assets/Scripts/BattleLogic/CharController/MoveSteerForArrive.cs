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
    private bool               m_isPlanar        = true;

    //到达区，距离多少算到了，可以设置为0;
    public float               m_nearDistance    = 0.1f;
    //steer容器
    private MoveSteers         m_steers           = null;

    private Vector3            m_force            = Vector3.zero;
    public CharController      m_cctr             = null;
    public delegate void       DelegateArrived();
    public DelegateArrived     m_arrivedCallback = null;
    public MoveSteerForArrive(MoveSteers steers, CharController cctr)
    {
        m_steers = steers;
        m_cctr   = cctr;
    }

    public override void Init()
    {
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
        Vector3 startPos = m_cctr.Transform.position;
        Vector3 endPos   = m_cctr.TargetForArrive;
        toTarget         = endPos - startPos;
        m_force          = toTarget.normalized * m_cctr.SpeedForArrive;

        //if (m_isPlanar)
        //{
        //    toTarget.y = m_cctr.OffsetY.y;
        //}

        float distance = toTarget.magnitude;

        //到了
        if (distance <= m_nearDistance)
        {
            m_force         = Vector3.zero;
            Active          = false;
            m_steers.Active = false;
            //m_cctr.m_commond
            m_cctr.Commond(CharController.E_COMMOND.STOPMOVE);
            //Debug.Log("Arrive 到了终点 " + endPos + " cost seconds " +
            //    (Time.realtimeSinceStartup - m_cctr.StartArrive));

            if (m_arrivedCallback != null)
            {
                m_arrivedCallback();
            }
        }
        return m_force;
    }

    void OnDrawGizmos()
    {

    }
}
