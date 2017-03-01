/// <summary>
/// 坦克的命令
/// author : fanzhengyong
/// date  : 2017-02-22
/// 
using UnityEngine;
using System.Collections;

public class TankCommond : CharCommond
{
    //特效资源路径
    private string         m_effectResPath      = null;
    //特效挂点transform路径
    private string         m_effectPointPath    = null;
    private ParticleSystem m_particleSystem     = null;
	public TankCommond(CharController cctr)
        :base(cctr){}

    public override void Init()
    {
        InitCommond();
        InitPath();
    }

    public void OnIdle()
    {
        Debug.Log("TankCommond.Idle");
    }

    public void OnAttack()
    {
        //Debug.Log("TankCommond.Attack" + Time.realtimeSinceStartup);
        if (m_particleSystem == null)
        {
            GameObject particleObj = BattleObjManager.Instance.
                BorrowParticleObj(m_effectResPath);
            particleObj.transform.position = Vector3.zero;
            particleObj.transform.Rotate(new Vector3(0f, -90f, 0f));

            Transform effectPoint = m_cctr.Transform.Find(m_effectPointPath);
            particleObj.transform.SetParent(effectPoint, false);

            m_particleSystem = particleObj.GetComponentInChildren<ParticleSystem>();
            m_particleSystem.Play();
            //for test begin
            BattleObjManager.Instance.EffectCount++;
            //for test end
            return;
        }
        m_particleSystem.Play();
        //for test begin
        BattleObjManager.Instance.EffectCount++;
        //for test end
    }

    public void OnAttacked()
    {
        Debug.Log("TankCommond.Attacked");
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

        obj.Arrive(m_cctr.DeadPosition, m_cctr.DeadTarget, m_cctr.DeadMoveSpeed);
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
        m_effectResPath      = "Effect/Tank/Prefab/Tank_dapao_fire";
        m_effectPointPath    = "Bone01/Bone02/Dummy01";
    }
}
