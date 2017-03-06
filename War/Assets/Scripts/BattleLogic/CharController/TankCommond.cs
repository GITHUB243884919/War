/// <summary>
/// 坦克的命令
/// author : fanzhengyong
/// date  : 2017-02-22
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TankCommond : CharCommond
{
    private Dictionary<CharController.E_COMMOND, CharParticleEffect> m_charParticleEffects
        = new Dictionary<CharController.E_COMMOND, CharParticleEffect>();
	
    public TankCommond(CharController cctr)
        :base(cctr){}

    public override void Init()
    {
        InitCommond();
        InitPath();
    }

    public override void SetEffectInActive()
    {
        foreach (KeyValuePair<CharController.E_COMMOND, CharParticleEffect> pair in m_charParticleEffects)
        {
            if ((pair.Value != null)
                && (pair.Value.ParticleSystem != null)
            )
            {
                pair.Value.ParticleSystem.transform.
                    parent.gameObject.SetActive(false);

                ParticleSystem.EmissionModule em = pair.Value.ParticleSystem.emission;
                em.enabled = false;
                pair.Value.ParticleSystem.Stop();
            }
        }

    }

    public void OnIdle()
    {
        Debug.Log("TankCommond.Idle");
    }

    public void OnAttack()
    {
        //Debug.Log("TankCommond.Attack" + Time.realtimeSinceStartup);

        CharController.E_COMMOND cmd = CharController.E_COMMOND.ATTACK;
        ShowEffect(cmd);

    }

    public void OnAttacked()
    {
        Debug.Log("TankCommond.Attacked");
        CharController.E_COMMOND cmd    = CharController.E_COMMOND.ATTACKED;
        ShowEffect(cmd);
    }


    /// <summary>
    /// 坦克死亡后变成救护车开走
    /// </summary>
    public void OnDead()
    {
        Debug.Log("TankCommond.OnDead");
        //自身爆炸
        //自身隐身
        //取出变身后的救护车对象
        BattleObjManager.E_BATTLE_OBJECT_TYPE type = m_cctr.DeadChangeObjType;
        int serverEntityID = m_cctr.DeadChangeEntityID;
        CharObj obj = BattleObjManager.Instance.BorrowCharObj(
            type, serverEntityID, 1);

        obj.AI_Arrive(m_cctr.DeadPosition, m_cctr.DeadTarget, m_cctr.DeadMoveSpeed);
    }

	public override void Update() 
    {
	
	}

    public override void FixedUpdate()
    {

    }

    private void InitCommond()
    {
        m_cctr.RegCommond(CharController.E_COMMOND.IDLE,     OnIdle);
        m_cctr.RegCommond(CharController.E_COMMOND.ATTACK,   OnAttack);
        m_cctr.RegCommond(CharController.E_COMMOND.ATTACKED, OnAttacked);
        m_cctr.RegCommond(CharController.E_COMMOND.DEAD,     OnDead);
    }

    private void InitPath()
    {
        CharParticleEffect effectAttack   = new CharParticleEffect(
            "Tank/Prefab/Tank_tanke_fire", "Bone01/Bone02/Dummy01");
        m_charParticleEffects.Add(CharController.E_COMMOND.ATTACK,
            effectAttack);

        CharParticleEffect effectAttacked = new CharParticleEffect(
            "Tank/Prefab/Tank_tanke_hit", "Bone01/Bone02/Dummy01");
        m_charParticleEffects.Add(CharController.E_COMMOND.ATTACKED,
            effectAttacked);
    }

    private void ShowEffect(CharController.E_COMMOND cmd)
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

        effect.ParticleSystem.transform.parent.gameObject.SetActive(true);
        ParticleSystem.EmissionModule em = effect.ParticleSystem.emission;
        em.enabled = true;
        effect.ParticleSystem.Play();
        //for test begin
        BattleObjManager.Instance.EffectCount++;
        //for test end

    }
}
