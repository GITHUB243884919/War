/// <summary>
/// NCAI  对象Bomb的控制器实现类
/// author : fanzhengyong
/// date  : 2017-04-10
/// </summary>
using UnityEngine;
using System.Collections;
using UF_FrameWork;
//#if _WAR_TEST_
//using Debug = LogMediator;
//#endif

public class NCAIBombController : NCAIController
{
    ParticleSystem m_ps = null;
    ParticleCallbackUtil m_pscbUtil = null;

    public override void InitSteers()
    {
        m_steers = new MoveSteers();
        m_steerTypes = new MoveSteers.E_STEER_TYPE[1]{
            MoveSteers.E_STEER_TYPE.STATIC_BOMB
        };

        m_steers.Init(m_steerTypes, OnArrived);
    }

    public override void OnArrived()
    {
        //Debug.Log("Bomb 命中目标");
        if (m_ps == null)
        {
            m_ps = gameObject.GetComponentInChildren<ParticleSystem>();
        }
        if (m_pscbUtil == null)
        {
            m_pscbUtil = gameObject.GetComponent<ParticleCallbackUtil>();
            m_pscbUtil.Init(Finished);
        }
        m_ps.Play();
        AudioManager.Instance.PlaySound(
            "Audio/bomb1", transform.position);
        //Debug.Log("Bomb 命中目标 播放特效");
    }

    public override void Release()
    {
        base.Release();
        m_ps = null;
        m_pscbUtil = null;
    }

    public void Finished()
    {
        //Debug.Log("特效播放完毕 炸弹消失");
        GameObject.Destroy(gameObject, 0.1f);
    }
}
