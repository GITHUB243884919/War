/// <summary>
/// 坦克的命令，这个应该被抽象。
/// author : fanzhengyong
/// date  : 2017-02-22
/// 
using UnityEngine;
using System.Collections;

public class TankCommond 
{
    public CharController m_ctr;
	public TankCommond(CharController ctr)
    {
        if (ctr == null)
        {
            Debug.LogWarning("CharController 为空");
            return;
        }

        m_ctr = ctr;
    }

    public void Init()
    {
        m_ctr.RegCommond(CharController.E_COMMOND.IDLE, Idle);
        m_ctr.RegCommond(CharController.E_COMMOND.ATTACK, Attack);
        m_ctr.RegCommond(CharController.E_COMMOND.ATTACKED, Attacked);
    }

    public void Idle()
    {
        Debug.Log("Idle");
    }

    public void Attack()
    {
        Debug.Log("Attack");
    }

    public void Attacked()
    {
        Debug.Log("Attacked");
    }

	public void Update () 
    {
	
	}
}
