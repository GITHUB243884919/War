using UnityEngine;
using System.Collections;

/// <summary>
/// 导弹运动算法
/// 先匀速运动到两点间的一个点，这个点是整个路径中y最大的点，然后再匀速运动到目标点
/// </summary>
public class Missile_Test_1 : MonoBehaviour
{
    //目标对象
    public GameObject    m_target         = null;
    //最高点的y的偏移量
    public float         m_offsetY        = 3f;
    //速度
    public float         m_speed          = 5f;
    //最高点
    private Vector3      m_maxHightPoint  = Vector3.zero;
    //移动向量
    private Vector3      m_movement       = Vector3.zero;
    //目标距离
    private float        m_distance       = 0f;
    private float        m_nearDistance   = 1f;
    //是否抵达到最高点
    private bool         m_isArrivedMaxHightPoint = false;
    //是否抵达到目标点
    private bool         m_isArrivedTarget        = false;

    void Start()
    {
        //计算最高点
        m_maxHightPoint = m_target.transform.position - transform.position;
        Debug.Log(m_maxHightPoint);
        m_maxHightPoint = m_maxHightPoint * 0.5f;
        Debug.Log(m_maxHightPoint);
        m_maxHightPoint += transform.position;
        Debug.Log(m_maxHightPoint);
        m_maxHightPoint.y = m_maxHightPoint.y + m_offsetY;
        Debug.Log(m_maxHightPoint);

        transform.LookAt(m_maxHightPoint);
        //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //go.transform.position = m_hightPoint;
        
    }

    void FixedUpdate()
    {
        if (m_isArrivedMaxHightPoint && m_isArrivedTarget)
        {
            return;
        }

        //如果没有抵达最高点，那么持续往最高点移动
        if (!m_isArrivedMaxHightPoint)
        {
            m_distance = (m_maxHightPoint - transform.position).magnitude;
            if (m_distance <= m_nearDistance)
            {
                m_isArrivedMaxHightPoint = true;
                return;
            }
            m_movement = (m_maxHightPoint - transform.position).normalized;
            m_movement *= m_speed;
            m_movement *= Time.fixedDeltaTime;
            
            transform.LookAt(transform.position += m_movement);
            transform.position += m_movement;    
        }

        //如果抵达最高点，并且没有达到目标点，那么持续往目标点移动
        if ((!m_isArrivedTarget) && (m_isArrivedMaxHightPoint))
        {
            m_distance = (m_target.transform.position - transform.position).magnitude;
            if (m_distance <= m_nearDistance)
            {
                m_isArrivedTarget = true;
                return;
            }
            m_movement = (m_target.transform.position - transform.position).normalized;
            m_movement *= m_speed;
            m_movement *= Time.fixedDeltaTime;
            transform.LookAt(transform.position += m_movement);
            transform.position += m_movement;    

        }
    }

}