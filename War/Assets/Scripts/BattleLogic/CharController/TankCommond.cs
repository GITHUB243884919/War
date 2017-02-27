/// <summary>
/// 坦克的命令
/// author : fanzhengyong
/// date  : 2017-02-22
/// 
using UnityEngine;
using System.Collections;

public class TankCommond : CharCommond
{
	public TankCommond(CharController cctr)
        :base(cctr){}

    private Transform m_trsEffect = null;
    public override void Init()
    {
        m_cctr.RegCommond(CharController.E_COMMOND.IDLE, Idle);
        m_cctr.RegCommond(CharController.E_COMMOND.ATTACK, Attack);
        m_cctr.RegCommond(CharController.E_COMMOND.ATTACKED, Attacked);
    }

    public void Idle()
    {
        Debug.Log("Idle");
    }

    public void Attack()
    {
        Debug.Log("Attack");
        if (m_trsEffect == null)
        {
            //m_trsEffect
            //BattleObjManager.Instance.BorrowParticleObj(effectPath);
        }
        //m_cctr.Transform.Find("Bone01/Bone02/Dummy01/Tank_dapao_fire");
    }

    public void Attacked()
    {
        Debug.Log("Attacked");
    }

	public override void Update() 
    {
	
	}

    public override void FixedUpdate()
    {
        //base.FixedUpdate();
    }
}
