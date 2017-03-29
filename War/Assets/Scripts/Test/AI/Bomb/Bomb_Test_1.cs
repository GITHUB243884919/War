using UnityEngine;
using System.Collections;

/// <summary>
/// 轰炸算法实现
/// 按重力加速度往下运动
/// </summary>
public class Bomb_Test_1 : MonoBehaviour 
{
    public  Vector3 m_target = Vector3.zero;
    private float   m_speed = 0f;
    private float   m_timer = 0f;
    private float   m_interval = 0.2f;
    private Vector3 m_force = Vector3.zero;
    private Vector3 m_toTargetDir = Vector3.zero;
    private float   m_nearDistance = 0f;
    private bool    m_Active = true;
    private Vector3 m_moveDistance = Vector3.zero;
    private float   m_realInterval = 0f;
    private readonly float G = 9.8f;

    void Start()
    {
        m_toTargetDir = (m_target - transform.position).normalized;
        //transform.LookAt(m_target);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.PlayDelayed(10f);
        //如果多段音效需要增加多个AudioSource，然后用map存起来，用key去控制播放哪个。
        //比如key就是某动作的枚举编号
    }

    void Update()
    {
        if (!m_Active)
        {
            return;
        }

        m_timer += Time.deltaTime;
        if (m_timer < m_interval)
        {
            return;
        }
        m_realInterval = m_timer;
        m_timer        = 0f;

        //根据重力加速度算速度
        m_speed       += G * m_realInterval;
        m_force        = m_toTargetDir * m_speed;

        //到达目标范围内停止
        if (((m_target - transform.position)).magnitude <= m_nearDistance)
        {
            m_Active = false;
        }

        //到达目标的y值停止
        if (transform.position.y <= m_target.y)
        {
            m_Active = false;
        }
    }

    void FixedUpdate()
    {
        if (!m_Active)
        {
            return;
        }

        m_moveDistance = m_force * Time.fixedDeltaTime;
        //Debug.Log(Time.realtimeSinceStartup + " m_moveDistance " + m_moveDistance + " m_force " + m_force);
        transform.position += m_moveDistance;
        //到达目标的y值停止
        if (transform.position.y <= m_target.y)
        {
            m_Active = false;
        }
    }
}
