/// <summary>
/// M_Arm_Airplane_01 飞机的命令
/// author : fanzhengyong
/// date  : 2017-03-09
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

public class M_Arm_Airplane_01_Commond : CharCommond
{
    private CharObjUSMBForDeadExit CharObjUSMBForDeadExit { get; set; }

    public M_Arm_Airplane_01_Commond(CharController cctr)
        :base(cctr){}

    public override void Init()
    {
        InitCommond();
        InitPath();
        m_cctr.m_positionData.Offset = new Vector3(0f, 2.8f, 0f);
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

        //CharController.E_COMMOND cmd = CharController.E_COMMOND.ATTACK;
        //m_cctr.Animator.speed = 1f;
        //m_cctr.Animator.SetTrigger("Fire");
        //ActiveEffect(cmd);
        GameObject go = ResourcesManagerMediator.
            GetGameObjectFromResourcesManager("NCAIObj/Daodan_0_1");
        go.transform.position = m_cctr.PositionData.Transform.position;
        NCAIController ctr = go.GetComponent<NCAIController>();
        PositionData data = new PositionData();
        
        data.TargetForArrive = m_cctr.m_positionData.TargetForLookAt 
            - m_cctr.m_positionData.Offset;
        ctr.AI(data);
    }

    public void OnAttacked()
    {
        Debug.Log("M_Arm_Airplane_01_Commond.Attacked");
        //CharController.E_COMMOND cmd    = CharController.E_COMMOND.ATTACKED;
        //ActiveEffect(cmd);
    }

    /// <summary>
    /// 飞机死亡是动画
    /// </summary>
    public void OnDead()
    {
        Debug.Log("M_Arm_Airplane_01_Commond.OnDead");
        //设置隐身参数
        //if (CharObjUSMBForDeadExit == null)
        //{
        //    CharObjUSMBForDeadExit = m_cctr.Animator.GetBehaviour<CharObjUSMBForDeadExit>();
        //}

        //m_cctr.m_positionData.TargetForPosition = CharObj.INIT_POS;
        //CharObjUSMBForDeadExit.m_cctr    = m_cctr;
        //CharObjUSMBForDeadExit.m_commond = CharController.E_COMMOND.POSITION;
        //m_cctr.Animator.speed = 1f;
        //m_cctr.Animator.SetTrigger("Die");
    }

    public override void MoveAnimator()
    {
        m_cctr.Animator.speed = 1f;
        m_cctr.Animator.SetTrigger("Move");
    }

    public override void StopAnimator()
    {
        //base.StopAnimator();
        if (m_cctr.Animator != null)
        {
            m_cctr.Animator.speed = 0f;
        }
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
