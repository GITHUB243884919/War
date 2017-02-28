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
        Debug.Log("Attack" + Time.realtimeSinceStartup);
        //查找特效transform路径
        if(m_effectTrs == null)
        {
            //Debug.Log("m_effectTrs == null" + m_effectTrsPath);
            GameObject particleObj = BattleObjManager.Instance.
                BorrowParticleObj(m_effectResPath);
            particleObj.transform.position = Vector3.zero;
            particleObj.transform.Rotate(new Vector3(0f, -90f, 0f));
            if (m_effectPointTrs == null)
            {
                //Debug.Log("m_cctr.Transform.position" + m_cctr.Transform.position);
                m_effectPointTrs = m_cctr.Transform.Find(m_effectPointTrsPath);
            }
            particleObj.transform.SetParent(m_effectPointTrs, false);
            m_effectTrs = particleObj.transform;
        }
        //Debug.Log("m_effectTrs " + m_effectTrs.name);
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
        //m_effectTrsPath = "Bone01/Bone02/Dummy01/Tank_dapao_fire(Clone)(Clone)";
        m_effectPointTrsPath = "Bone01/Bone02/Dummy01";
    }
}
