using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using E_GROUP_COMMOND = GroupCommondFormationParam.E_GROUP_COMMOND;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif
public class Test_AI_Group_3 : MonoBehaviour
{
    
    GroupCharObjsController m_groupCtr = null;
    CharObj m_charObj = null;
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
        m_groupCtr = new GroupCharObjsController();

        //m_charObj = BattleObjManager.Instance.BorrowCharObj(
        // BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS,
        //    0, 1);
    }

    public void BtnClick()
    {
        CreateGroupCharObjs();
    }

    void CreateGroupCharObjs()
    {

        //m_groupCtr.Init(100, E_GROUP_COMMOND.MOVE,
        //    new Vector3(32, 0, 32), new Vector3(10, 0, 10));

        //m_groupCtr.Init(100, E_GROUP_COMMOND.ATTACK,
        //    new Vector3(32, 0, 32), new Vector3(10, 0, 10));

        //m_groupCtr.Init(100, E_GROUP_COMMOND.IDLE,
        //    new Vector3(32, 0, 32), new Vector3(10, 0, 10));

        //m_groupCtr.Init(100, E_GROUP_COMMOND.IDLE,
        //    new Vector3(32, 0, 32), new Vector3(64, 0, 45));

        //m_groupCtr.Init(200, E_GROUP_COMMOND.IDLE,
        //    new Vector3(32, 0, 32), new Vector3(10, 0, 10));

        //m_groupCtr.Init(200, E_GROUP_COMMOND.ATTACK,
        //    new Vector3(32, 0, 32), new Vector3(64, 0, 45));

        m_groupCtr.Init(300, E_GROUP_COMMOND.ATTACK,
            new Vector3(32, 0, 32), new Vector3(64, 0, 45));

        //m_groupCtr.Init(300, E_GROUP_COMMOND.IDLE,
        //    new Vector3(32, 0, 32), new Vector3(10, 0, 10));

        //StartCoroutine(SwitchFormation(E_GROUP_COMMOND.ATTACK));
        //StartCoroutine(SwitchFormation(E_GROUP_COMMOND.ARRIVE));


        m_groupCtr.AI_Arrive(new Vector3(32, 0, 32),
            new Vector3(40, 0, 64), 1f);

        //CharObj charObj = BattleObjManager.Instance.BorrowCharObj(
        //    BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS,
        //    0, 1);

        //m_charObj.AI_Arrive(new Vector3(32, 0, 32), new Vector3(40, 0, 40), 1f,
        //    delegate
        //    {
        //        m_charObj.AI_Arrive(new Vector3(40, 0, 40), new Vector3(64, 0, 64), 1f);
        //        //StartCoroutine(TestSwitchCommond());
        //    });


        //m_charObj.CharController.Commond(CharController.E_COMMOND.IDLE);

        

    }

    IEnumerator TestSwitchCommond()
    {
        yield return null;
        //yield return new WaitForSeconds(0.5f);
        //m_charObj.AI_Arrive(new Vector3(32, 0, 32), new Vector3(40, 0, 40), 1f,
        //    delegate
        //    {
        //        //new WaitForSeconds(5f);
        //        m_charObj.AI_Arrive(new Vector3(40, 0, 40), new Vector3(64, 0, 64), 1f);
        //        m_charObj.CharController.Animator.SetTrigger("Move");
        //    });

        m_charObj.AI_Arrive(new Vector3(40, 0, 40), new Vector3(64, 0, 64), 1f);

    }

    public IEnumerator SwitchFormation(
        E_GROUP_COMMOND  groupCommond)
    {

        Debug.Log("3秒后开始阵型变换");
        yield return new WaitForSeconds(3f);
        Debug.Log("阵型变换中。。。。");
        m_groupCtr.TransformFormation(m_groupCtr.Param.GetFormationParam(groupCommond),
            new Vector3(32, 0, 32), new Vector3(10, 0, 10), null);
    }

    //public IEnumerator SwitchFormation(
    //    GroupCharObjsController.E_FORMATION_TYPE formation,
    //    Vector3 start, Vector3 target)
    //{

    //    Debug.Log("3秒后开始阵型变换");
    //    yield return new WaitForSeconds(3f);
    //    Debug.Log("阵型变换中。。。。");
    //    m_groupCtr.SwitchFormation(formation, start, target);
    //}

    public IEnumerator WaitJob()
    {
        Debug.Log("等待开始");
        yield return new WaitForSeconds(30f);
        Debug.Log("结束等待");
        //m_groupCtr.AI_Arrive(new Vector3(64f, 0f, 64f), new Vector3(32f, 0f, 32f), 5f,
        //    delegate()
        //    {
        //        //m_groupCtr.SwitchFormation(GroupCharObjsController.E_FORMATION_TYPE.TARGET_CYCLE,
        //        //m_groupCtr.m_center, m_groupCtr.m_lookAt);
        //        m_groupCtr.SwitchFormation(GroupCharObjsController.E_FORMATION_TYPE.TARGET_CYCLE,
        //            new Vector3(64f, 0f, 64f), new Vector3(32f, 0f, 32f));
        //    });
    }


}
