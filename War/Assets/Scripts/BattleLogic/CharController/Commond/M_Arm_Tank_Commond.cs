/// <summary>
/// M_Arm_Tank坦克的命令
/// author : fanzhengyong
/// date  : 2017-03-09
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class M_Arm_Tank_Commond : CharCommond
{
    private CharObjUSMBForDeadExit CharObjUSMBForDeadExit { get; set; }

    public M_Arm_Tank_Commond(CharController cctr)
        :base(cctr){}

    public override void Init()
    {
        InitCommond();
        InitPath();
    }

    public void OnIdle()
    {
        Debug.Log("M_Arm_Tank_Commond.Idle");
    }

    public void OnOpen()
    {
        Debug.Log("M_Arm_Tank_Commond.Open");
    }


    public void OnAttack()
    {
        //Debug.Log("M_Arm_Tank_Commond.Attack " + Time.realtimeSinceStartup);

        CharController.E_COMMOND cmd = CharController.E_COMMOND.ATTACK;
        m_cctr.Animator.speed = 1f;
        m_cctr.Animator.SetTrigger("Fire");
        ActiveEffect(cmd);
    }

    public void OnAttacked()
    {
        Debug.Log("M_Arm_Tank_Commond.Attacked");
        CharController.E_COMMOND cmd    = CharController.E_COMMOND.ATTACKED;
        ActiveEffect(cmd);
    }


    /// <summary>
    /// 坦克死亡后变成救护车开走
    /// </summary>
    public void OnDead()
    {
        Debug.Log("M_Arm_Tank_Commond.OnDead");
        //设置隐身参数
        if (CharObjUSMBForDeadExit == null)
        {
            CharObjUSMBForDeadExit = m_cctr.Animator.GetBehaviour<CharObjUSMBForDeadExit>();
        }

        m_cctr.m_positionData.TargetForPosition = CharObj.INIT_POS;
        CharObjUSMBForDeadExit.m_cctr    = m_cctr;
        CharObjUSMBForDeadExit.m_commond = CharController.E_COMMOND.POSITION;
        //设置变身参数
        BattleObjManager.E_BATTLE_OBJECT_TYPE type = m_cctr.DeadChangeObjType;
        int serverEntityID = m_cctr.DeadChangeEntityID;

        CharObjUSMBForDeadExit.m_deadChangObj = BattleObjManager.Instance.BorrowCharObj(
            type, serverEntityID, 1);
        CharObjUSMBForDeadExit.m_deadChangObj.CharController.m_positionData.TargetForPosition
            = m_cctr.DeadPosition;
        CharObjUSMBForDeadExit.m_deadChangObj.CharController.m_positionData.TargetForArrive
            = m_cctr.DeadTarget;
        CharObjUSMBForDeadExit.m_deadChangObj.CharController.m_positionData.SpeedForArrive
            = m_cctr.DeadMoveSpeed;
        CharObjUSMBForDeadExit.m_changCommond = CharController.E_COMMOND.ARRIVE;

        //自身动画
        m_cctr.Animator.speed = 1f;
        m_cctr.Animator.SetTrigger("Die");
    }

    public override void MoveEffect()
    {
        ActiveEffect(CharController.E_COMMOND.ARRIVE);
    }

    public override void StopEffect()
    {
        DeactiveEffect(CharController.E_COMMOND.ARRIVE);
    }

	public override void Update() 
    {
	
	}

    public override void FixedUpdate()
    {

    }

    public override void Release()
    {
        base.Release();
        CharObjUSMBForDeadExit = null;
    }

    private void InitCommond()
    {
        m_cctr.RegCommond(CharController.E_COMMOND.IDLE,     OnIdle);
        m_cctr.RegCommond(CharController.E_COMMOND.OPEN,     OnOpen);
        m_cctr.RegCommond(CharController.E_COMMOND.ATTACK,   OnAttack);
        m_cctr.RegCommond(CharController.E_COMMOND.ATTACKED, OnAttacked);
        m_cctr.RegCommond(CharController.E_COMMOND.DEAD,     OnDead);
    }

    private void InitPath()
    {

    }

}
