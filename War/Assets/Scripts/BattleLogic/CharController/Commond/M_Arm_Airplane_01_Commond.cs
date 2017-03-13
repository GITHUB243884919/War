/// <summary>
/// M_Arm_Airplane_01的命令
/// author : fanzhengyong
/// date  : 2017-03-09
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class M_Arm_Airplane_01_Commond : CharCommond
{
    public M_Arm_Airplane_01_Commond(CharController cctr)
        :base(cctr){}

    public override void Init()
    {
        InitCommond();
        InitPath();
    }

    public void OnIdle()
    {
        Debug.Log("M_Arm_Airplane_01_Commond.Idle");
    }

    public void OnOpen()
    {
        Debug.Log("M_Arm_Airplane_01_Commond.Open");
    }

    public void OnAttack()
    {
        //Debug.Log("M_Arm_Airplane_01_Commond.Attack " + Time.realtimeSinceStartup);

        CharController.E_COMMOND cmd = CharController.E_COMMOND.ATTACK;
        m_cctr.Animator.SetTrigger("Fire");
        ActiveEffect(cmd);
    }

    public void OnAttacked()
    {
        Debug.Log("M_Arm_Airplane_01_Commond.Attacked");
        CharController.E_COMMOND cmd    = CharController.E_COMMOND.ATTACKED;
        ActiveEffect(cmd);
    }


    /// <summary>
    /// 飞机死亡是动画
    /// </summary>
    public void OnDead()
    {
        Debug.Log("M_Arm_Airplane_01_Commond.OnDead");
        m_cctr.Animator.SetTrigger("Die");
        m_cctr.TargetForPosition = m_cctr.HidePosition;
        //m_cctr.Commond(CharController.E_COMMOND.POSITION);
        m_cctr.WaitForCommond = CharController.E_COMMOND.POSITION;
        m_cctr.WaitForSeconds = 5f;
        m_cctr.Commond(CharController.E_COMMOND.WAIT);
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
        m_cctr.RegCommond(CharController.E_COMMOND.OPEN,     OnOpen);
        m_cctr.RegCommond(CharController.E_COMMOND.ATTACK,   OnAttack);
        m_cctr.RegCommond(CharController.E_COMMOND.ATTACKED, OnAttacked);
        m_cctr.RegCommond(CharController.E_COMMOND.DEAD,     OnDead);
    }

    private void InitPath()
    {

    }

}
