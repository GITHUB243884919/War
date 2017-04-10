/// <summary>
/// NCAI  对象Bomb的控制器实现类
/// author : fanzhengyong
/// date  : 2017-04-10
/// </summary>
using UnityEngine;
using System.Collections;

#if _WAR_TEST_
using Debug = LogMediator;
#endif

public class NCAIBombController : NCAIController
{
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
        Debug.Log("Bomb 命中目标");
    }
}
