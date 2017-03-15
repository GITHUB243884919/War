/// <summary>
/// 死亡动画执行完后的操作
/// author : fanzhengyong
/// date  : 2017-03-13
/// </summary>
using UnityEngine;
using System.Collections;

public class CharObjUSMBForDeadExit : StateMachineBehaviour
{
    public CharController           m_cctr         = null;
    public CharController.E_COMMOND m_commond;
    public CharObj                  m_deadChangObj = null;
    public CharController.E_COMMOND m_changCommond;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("OnStateExit");
        //结束特效播放
        //隐身
        if (m_cctr != null)
        {
            m_cctr.Commond(m_commond);
        }
        m_cctr.Deactive();
        
        //救护车出场
        if (m_deadChangObj != null)
        {
            m_deadChangObj.CharController.Commond(m_changCommond);
        }
    }

}
