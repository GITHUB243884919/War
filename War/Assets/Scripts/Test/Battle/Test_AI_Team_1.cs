using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using E_GROUP_COMMOND = GroupCommondFormationParam.E_GROUP_COMMOND;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif
public class Test_AI_Team_1 : MonoBehaviour
{
    
    TeamCharObjsController m_teamCtr = null;
    CharObj m_charObj = null;
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
        m_teamCtr = new TeamCharObjsController();

        //m_charObj = BattleObjManager.Instance.BorrowCharObj(
        // BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS,
        //    0, 1);
    }

    public void BtnClick()
    {
        CreateTeamCharObjs();
    }

    void CreateTeamCharObjs()
    {
        m_teamCtr.Init(3000, E_GROUP_COMMOND.IDLE,
            new Vector3(32, 0, 32), new Vector3(64, 0, 45));
    }

}
