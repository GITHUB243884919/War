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

public abstract class CharCommond 
{
    public CharController m_cctr;

    protected Dictionary<CharController.E_COMMOND, ParticleSystem> m_charParticleEffects
        = new Dictionary<CharController.E_COMMOND, ParticleSystem>();

    public CharCommond(CharController cctr)
    {
        if (cctr == null)
        {
            Debug.LogWarning("CharController 为空");
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
            CharParticleConfiger cfg = CharObjParticleConfigerMediator.Instance.GetConfiger(m_cctr.CharType, cmd);
            if (cfg == null)
            {
                return;
            }
            GameObject particleObj = BattleObjManager.Instance.BorrowParticleObj(cfg.ResPath);
            m_cctr.CharObj.AddToChildsPools(particleObj, cfg.ResPath);

            particleObj.transform.position = Vector3.zero;
            particleObj.transform.Rotate(new Vector3(0f, -90f, 0f));

            Transform effectPoint = m_cctr.Transform.Find(cfg.PointPath);
            particleObj.transform.SetParent(effectPoint, false);

            particleSystem = particleObj.GetComponentInChildren<ParticleSystem>();
            m_charParticleEffects.Add(cmd, particleSystem);
            particleSystem.Play();
            //for test begin
            BattleObjManager.Instance.EffectCount++;
            //for test end
            return;
        }

        //effect.ParticleSystem.transform.parent.gameObject.SetActive(true);
        //ParticleSystem.EmissionModule em = effect.ParticleSystem.emission;
        //em.enabled = true;
        particleSystem.Play();
        //for test begin
        BattleObjManager.Instance.EffectCount++;
        //for test end

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

        foreach (CharController.E_COMMOND cmd in m_charParticleEffects.Keys)
        {
            m_charParticleEffects[cmd] = null;
        }
        m_charParticleEffects.Clear();
        m_charParticleEffects = null;
    }
}
