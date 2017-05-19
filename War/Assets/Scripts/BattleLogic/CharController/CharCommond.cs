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

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

public abstract class CharCommond 
{
    public CharController m_cctr;

    protected Dictionary<CharController.E_COMMOND, ParticleSystem> m_charParticleEffects
        = new Dictionary<CharController.E_COMMOND, ParticleSystem>();

    public CharCommond(CharController cctr)
    {
        if (cctr == null)
        {
            Debug.LogError("CharController 为空");
            return;
        }

        m_cctr = cctr;
    }

    public void DeactiveEffects()
    {
        foreach (ParticleSystem particleSystem in m_charParticleEffects.Values)
        {
            if (particleSystem != null)
            {
                particleSystem.Stop();
            }
        }
    }

    public void DeactiveEffect(CharController.E_COMMOND cmd)
    {
        ParticleSystem particleSystem = null;
        m_charParticleEffects.TryGetValue(cmd, out particleSystem);
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }
    }

    public void ActiveEffect(CharController.E_COMMOND cmd)
    {
        ParticleSystem particleSystem = null;
        m_charParticleEffects.TryGetValue(cmd, out particleSystem);
        if (particleSystem == null)
        {
            CharParticleConfiger cfg = CharObjParticleConfigerMediator.Instance.GetConfiger(
                m_cctr.CharObj.Type, cmd);
            if (cfg == null)
            {
                Debug.LogError(m_cctr.CharObj.Type.ToString() + 
                    " 没找到特效配置 " + cmd.ToString());
                return;
            }
            GameObject particleObj = BattleObjManager.Instance.BorrowParticleObj(cfg.ResPath);
            m_cctr.CharObj.AddToChildsPools(particleObj, cfg.ResPath);

            particleObj.transform.position = Vector3.zero;
            
            Transform effectPoint = m_cctr.Transform.Find(cfg.PointPath);
            if (effectPoint == null)
            {
                Debug.LogError(m_cctr.CharObj.Type.ToString() 
                    +  " 根据配置没有找到挂点 " + cfg.PointPath);
                return;
            }
            
            particleObj.transform.SetParent(effectPoint, false);
            
            particleSystem = particleObj.GetComponentInChildren<ParticleSystem>();
            m_charParticleEffects.Add(cmd, particleSystem);
            //particleSystem.Play();
            //return;
        }

        //effect.ParticleSystem.transform.parent.gameObject.SetActive(true);
        //ParticleSystem.EmissionModule em = effect.ParticleSystem.emission;
        //em.enabled = true;
        particleSystem.Play();
    }

    public virtual void MoveAnimator()
    {

    }

    public virtual void StopAnimator()
    {

    }

    public virtual void MoveEffect()
    {

    }

    public virtual void StopEffect()
    {

    }

    public virtual void OnArrived()
    {
        //Debug.Log("CharCommond OnArrived");
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

    public virtual void Release()
    {
        m_cctr = null;
        m_charParticleEffects.Clear();
        m_charParticleEffects = null;
    }
}
