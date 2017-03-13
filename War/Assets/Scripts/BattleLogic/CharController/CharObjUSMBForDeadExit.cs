/// <summary>
/// 死亡动画执行完后的操作
/// author : fanzhengyong
/// date  : 2017-03-13
/// </summary>
using UnityEngine;
using System.Collections;

public class CharObjUSMBForDeadExit : StateMachineBehaviour
{
    public CharController m_cctr;
    public CharController.E_COMMOND m_commond;
    public CharObj m_deadChangObj;
    public CharController.E_COMMOND m_changCommond;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("OnStateExit");
        //结束特效播放
        //隐身
        m_cctr.Commond(m_commond);
        //救护车出场
        m_deadChangObj.CharController.Commond(m_changCommond);
    }

}
