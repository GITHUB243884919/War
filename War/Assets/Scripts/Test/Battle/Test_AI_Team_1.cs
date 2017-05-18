﻿using UnityEngine;
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
    }

    public void BtnClick()
    {
        CreateTeamCharObjs();
    }

    void CreateTeamCharObjs()
    {
        m_teamCtr.Init(3000, E_GROUP_COMMOND.ARRIVE,
            new Vector3(32, 0, 32), new Vector3(64, 0, 45));

        StartCoroutine(TransformFormation());
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
}
