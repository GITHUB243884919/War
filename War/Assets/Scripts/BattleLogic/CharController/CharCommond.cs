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

    public void SetEffectDeactive()
    {
        foreach (KeyValuePair<CharController.E_COMMOND, CharParticleEffect> pair in m_charParticleEffects)
        {
            if ((pair.Value != null)
                && (pair.Value.ParticleSystem != null)
            )
            {
                //pair.Value.ParticleSystem.transform.
                //    parent.gameObject.SetActive(false);

                //ParticleSystem.EmissionModule em = pair.Value.ParticleSystem.emission;
                //em.enabled = false;
                pair.Value.ParticleSystem.Stop();
            }
        }
    }

    protected void ShowEffect(CharController.E_COMMOND cmd)
    {
        CharParticleEffect effect = m_charParticleEffects[cmd];
        if (effect == null)
        {
            Debug.LogError("没有这Commond的特效定义 " + cmd.ToString());
            return;
        }

        if (effect.ParticleSystem == null)
        {
            GameObject particleObj = BattleObjManager.Instance.
                BorrowParticleObj(effect.ResPath);
            particleObj.transform.position = Vector3.zero;
            particleObj.transform.Rotate(new Vector3(0f, -90f, 0f));

            Transform effectPoint = m_cctr.Transform.Find(effect.PointPath);
            particleObj.transform.SetParent(effectPoint, false);

            effect.ParticleSystem = particleObj.GetComponentInChildren<ParticleSystem>();
            effect.ParticleSystem.Play();
            //for test begin
            BattleObjManager.Instance.EffectCount++;
            //for test end
            return;
        }

        //effect.ParticleSystem.transform.parent.gameObject.SetActive(true);
        //ParticleSystem.EmissionModule em = effect.ParticleSystem.emission;
        //em.enabled = true;
        effect.ParticleSystem.Play();
        //for test begin
        BattleObjManager.Instance.EffectCount++;
        //for test end

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
