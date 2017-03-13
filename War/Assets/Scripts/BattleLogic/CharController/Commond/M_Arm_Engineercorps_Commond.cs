/// <summary>
/// M_Arm_Engineercorps的命令
/// author : fanzhengyong
/// date  : 2017-03-09
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class M_Arm_Engineercorps_Commond : CharCommond
{
    public M_Arm_Engineercorps_Commond(CharController cctr)
        :base(cctr){}

    public override void Init()
    {
        InitCommond();
        InitPath();
    }

    public void OnIdle()
    {
        Debug.Log("M_Arm_Engineercorps_Commond.Idle");
    }

    public void OnAttack()
    {
        //Debug.Log("M_Arm_Engineercorps_Commond.Attack " + Time.realtimeSinceStartup);

        CharController.E_COMMOND cmd = CharController.E_COMMOND.ATTACK;
        m_cctr.Animator.speed = 1f;
        m_cctr.Animator.SetTrigger("Fire");
        ActiveEffect(cmd);
    }

    public void OnAttacked()
    {
        Debug.Log("M_Arm_Engineercorps_Commond.Attacked");
        CharController.E_COMMOND cmd    = CharController.E_COMMOND.ATTACKED;
        ActiveEffect(cmd);
    }


    /// <summary>
    /// 士兵死亡后变成救护车开走
    /// </summary>
    public void OnDead()
    {
        Debug.Log("M_Arm_Engineercorps_Commond.OnDead");
        
        //设置隐身参数
        CharObjUSMBehaviour USMBehaviour
            = m_cctr.Animator.GetBehaviour<CharObjUSMBehaviour>();
        m_cctr.TargetForPosition = m_cctr.HidePosition;
        USMBehaviour.m_cctr = m_cctr;
        USMBehaviour.m_commond = CharController.E_COMMOND.POSITION;
        //设置变身参数
        BattleObjManager.E_BATTLE_OBJECT_TYPE type = m_cctr.DeadChangeObjType;
        int serverEntityID = m_cctr.DeadChangeEntityID;

        USMBehaviour.m_deadChangObj = BattleObjManager.Instance.BorrowCharObj(
            type, serverEntityID, 1);
        USMBehaviour.m_deadChangObj.CharController.TargetForPosition
            = m_cctr.DeadPosition;
        USMBehaviour.m_deadChangObj.CharController.TargetForArrive
            = m_cctr.DeadTarget;
        USMBehaviour.m_deadChangObj.CharController.SpeedForArrive
            = m_cctr.DeadMoveSpeed;
        USMBehaviour.m_changCommond = CharController.E_COMMOND.ARRIVE;

        //自身动画
        m_cctr.Animator.speed = 1f;
        m_cctr.Animator.SetTrigger("Die");

    }

    public override void MoveAnimator()
    {
        //base.MoveAnimator();
        //m_cctr.Animator.SetBool("Move", true);
        m_cctr.Animator.speed = 1f;
        m_cctr.Animator.SetTrigger("Move");
    }

    public override void StopAnimator()
    {
        //base.StopAnimator();
        m_cctr.Animator.speed = 0f;
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
