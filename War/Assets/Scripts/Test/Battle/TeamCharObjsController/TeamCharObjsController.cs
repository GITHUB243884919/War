using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using E_GROUP_COMMOND = GroupCommondFormationParam.E_GROUP_COMMOND;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

public class TeamCharObjsController
{

    public List<GroupCharObjsController> m_groups =
        new List<GroupCharObjsController>();

    public TeamCommondFormationParam Param { get; set; }

    public void Init(int paramID, E_GROUP_COMMOND groupCommond,
        Vector3 start, Vector3 lookAt)
    {
        //整个team有一个阵型，计算出组成team的每个Group的center
        //每个group进行阵型走位，各个的center是刚才算出的center

        TeamCommondFormationParam Param =
            TeamCommondFormationParamManager.Instance.GetParam(paramID);

        GroupFormationParam formationParam = Param.GetFormationParam(
            groupCommond);

        int[] groupIDs = Param.GetGroup();
        List<Vector3> teamPoints = new List<Vector3>();
        formationParam.SetFormationPositions(groupIDs.Length, start, 45f, lookAt, ref teamPoints);

        if (groupIDs.Length != teamPoints.Count)
        {
            Debug.LogError("!!!!!!!!!!!!!");
            return;
        }

        for(int i = 0; i < groupIDs.Length; i++)
        {
            GroupCharObjsController group = new GroupCharObjsController();
            group.Init(groupIDs[i], groupCommond, teamPoints[i], lookAt);
        }

    }
}
