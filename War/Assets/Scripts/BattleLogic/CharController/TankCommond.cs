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
    string m_effectResPath = null;
    //特效transform路径
    private string m_effectTrsPath = null;
    //特效挂点transform路径
    private string m_effectPointTrsPath = null;
    //
    private Transform m_effectTrs = null;
    //
    private Transform m_effectPointTrs = null;

	public TankCommond(CharController cctr)
        :base(cctr){}


    public override void Init()
    {
        InitCommond();
        InitPath();
    }

    public void Idle()
    {
        Debug.Log("Idle");
    }

    public void Attack()
    {
        Debug.Log("Attack");
        //查找特效transform路径
        m_effectTrs = m_cctr.Transform.Find(m_effectTrsPath);
        //
        if(m_effectTrs == null)
        {
            GameObject particleObj = BattleObjManager.Instance.
                BorrowParticleObj(m_effectResPath);
            particleObj.transform.Rotate(new Vector3(0f, -90f, 0f));
            if (m_effectPointTrs == null)
            {
                m_effectPointTrs = m_cctr.Transform.Find(m_effectPointTrsPath);
            }
            particleObj.transform.SetParent(m_effectPointTrs, false);
        }
        //m_cctr.Animator.SetTrigger("Fire");

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

    }

    private void InitCommond()
    {
        m_cctr.RegCommond(CharController.E_COMMOND.IDLE, Idle);
        m_cctr.RegCommond(CharController.E_COMMOND.ATTACK, Attack);
        m_cctr.RegCommond(CharController.E_COMMOND.ATTACKED, Attacked);
    }

    private void InitPath()
    {
        m_effectResPath = "Effect/Tank/Prefab/Tank_dapao_fire";
        m_effectTrsPath = "Bone01/Bone02/Dummy01/Tank_dapao_fire";
        m_effectPointTrsPath = "Bone01/Bone02/Dummy01";
    }
}
