/// <summary>
/// 粒子系统回调工具类
/// author : fanzhengyong
/// date  : 2017-03-09
/// </summary>
/// 
using UnityEngine;
using System.Collections;

public class ParticleCallbackUtil : MonoBehaviour
{
    float m_interval        = 0.2f;
    float m_timer           = 0f;
    bool  m_isPlayed        = false;
    bool  m_isStoped        = false;

    ParticleSystem[] m_particleSystems;
    public delegate void PlayedCallback();
    PlayedCallback m_stopedCallback = null;

    public void Init(PlayedCallback stopedCallback)
    {
        m_stopedCallback = stopedCallback;
    }

    public void Release()
    {
        m_particleSystems = null;
        m_stopedCallback  = null;
    }

    public void SetState()
    {
        SetPlayed();
        SetStoped();
    }

    void SetPlayed()
    {
        if (m_isPlayed)
        {
            return;
        }

        //有一个开始了就算开始
        for (int i = 0; i < m_particleSystems.Length; i++)
        {
            if (m_particleSystems[i].isPlaying)
            {
                m_isPlayed = true;
                return;
            }
        }
    }

    void SetStoped()
    {
        //没有开始就没有结束
        if (!m_isPlayed)
        {
            m_isStoped = false;
            return;
        }

        //有一个没结束都算没结束
        m_isStoped = false;
        for (int i = 0; i < m_particleSystems.Length; i++)
        {
            if (!m_particleSystems[i].isStopped)
            {
                return;
            }
        }
        m_isStoped = true;
    }

    void InvokeStopedCallback()
    {
        if ((m_isStoped)
            && (m_stopedCallback != null)
            )
        {
            m_stopedCallback();
        }
    }

    //--------Unity------------------------

    void Awake()
    {
        m_particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer < m_interval)
        {
            return;
        }
        m_timer = 0f;

        SetState();
        InvokeStopedCallback();
    }

    void OnDestroy()
    {
        Release();
    }
}
