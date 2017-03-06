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

public class CharParticleEffect
{
    //特效资源路径
    private string m_resPath   = null;

    //特效挂点transform路径
    private string m_pointPath = null;

    //特效上的粒子特效组件
    public ParticleSystem ParticleSystem {get; set;}

    public CharParticleEffect(string resPath, string pointPath)
    {
        m_resPath   = resPath;
        m_pointPath = pointPath;
    }

    public string ResPath 
    { 
        get 
        { 
            return m_resPath;
        }
    }

    public string PointPath
    {
        get
        {
            return m_pointPath;
        }
    }
}

public abstract class CharCommond 
{
    public CharController m_cctr;
    public CharCommond(CharController ctr)
    {
        if (ctr == null)
        {
            Debug.LogWarning("CharController 为空");
            return;
        }

        m_cctr = ctr;
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
