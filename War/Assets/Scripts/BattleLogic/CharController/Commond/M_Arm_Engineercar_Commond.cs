/// <summary>
/// M_Arm_Engineercar_Commond的命令
/// author : fanzhengyong
/// date  : 2017-03-09
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class M_Arm_Engineercar_Commond : CharCommond
{
    public M_Arm_Engineercar_Commond(CharController cctr)
        :base(cctr){}

    public override void Init()
    {
        InitCommond();
        InitPath();
    }

    public void OnIdle()
    {
        Debug.Log("M_Arm_Engineercar_Commond.Idle");
    }

    public void OnAttack()
    {
        //Debug.Log("M_Arm_Engineercar_Commond.Attack " + Time.realtimeSinceStartup);

        CharController.E_COMMOND cmd = CharController.E_COMMOND.ATTACK;
        m_cctr.Animator.SetTrigger("Fire");
        ActiveEffect(cmd);
    }

    public void OnAttacked()
    {
        Debug.Log("M_Arm_Engineercar_Commond.Attacked");
        CharController.E_COMMOND cmd    = CharController.E_COMMOND.ATTACKED;
        ActiveEffect(cmd);
    }


    /// <summary>
    /// 坦克死亡后变成救护车开走
    /// </summary>
    public void OnDead()
    {
        Debug.Log("M_Arm_Engineercar_Commond.OnDead");
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

    }

}
