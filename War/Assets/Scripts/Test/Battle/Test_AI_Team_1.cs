using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        TestDBManager();
        
    }

    public void BtnClick()
    {
        CreateTeamCharObjs();
    }

    void CreateTeamCharObjs()
    {
        m_teamCtr.Init(1, 3000, E_GROUP_COMMOND.ATTACK,
            new Vector3(32, 0, 32), new Vector3(64, 0, 45));
        m_teamCtr.AI_Arrive(new Vector3(32, 0, 32), new Vector3(64, 0, 45), 1f);

        //m_teamCtr.Init(1, 3000, E_GROUP_COMMOND.ARRIVE,
        //    new Vector3(32, 0, 32), new Vector3(64, 0, 45));
        //m_teamCtr.AI_Arrive_New(new Vector3(32, 0, 32), new Vector3(64, 0, 45), 1f);
        //StartCoroutine(TransformFormation2());
    }

    IEnumerator TransformFormation()
    {
        Debug.Log("等待变阵");
        yield return new WaitForSeconds(2);
        Debug.Log("开始变阵");
        GroupFormationParam formationParam = 
            m_teamCtr.Param.GetFormationParam(E_GROUP_COMMOND.ATTACK);
        m_teamCtr.TransformFormation(E_GROUP_COMMOND.ATTACK,
            new Vector3(32, 0, 32), new Vector3(64, 0, 45), null);

    }

    IEnumerator TransformFormation2()
    {
        Debug.Log("等待变阵");
        yield return new WaitForSeconds(60);
        Debug.Log("开始变阵");
        m_teamCtr.AI_TransformFormation(E_GROUP_COMMOND.ATTACK,
            new Vector3(64, 0, 45));

    }

    void TestDBManager()
    {
        TeamConfigerManager.getInstance().Init("Data/TeamConfiger");
        List<TeamConfigerData> datas = null;
        //datas = TeamConfigerManager.getInstance().GetAllData();
        datas = TeamConfigerManager.getInstance().Values;
        TeamConfigerData data = TeamConfigerManager.getInstance().Get(3000);
        return;
    }
}
