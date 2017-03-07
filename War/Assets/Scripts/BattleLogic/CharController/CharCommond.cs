/// <summary>
/// 角色能被执行的commond的抽象类
/// 
/// author: fanzhengyong
/// date: 2017-02-24
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharParticleEffect
{
    //特效上的粒子特效组件
    public ParticleSystem  ParticleSystem { get; set; }
    public string          ResPath        { get; set; }
    public string          PointPath      { get; set; }
    public CharParticleEffect(string resPath, string pointPath)
    {
        ResPath   = resPath;
        PointPath = pointPath;
    }
}

public abstract class CharCommond 
{
    public CharController m_cctr;

    protected Dictionary<CharController.E_COMMOND, CharParticleEffect> m_charParticleEffects
        = new Dictionary<CharController.E_COMMOND, CharParticleEffect>();

    public CharCommond(CharController cctr)
    {
        if (cctr == null)
        {
            Debug.LogWarning("CharController 为空");
            return;
        }

        m_cctr = cctr;
    }

    public virtual void SetEffectInActive()
    {

    }

    public virtual void Init()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }
}
