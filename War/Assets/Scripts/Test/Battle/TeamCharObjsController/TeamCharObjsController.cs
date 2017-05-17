using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using E_GROUP_COMMOND = GroupCommondFormationParam.E_GROUP_COMMOND;

public class TeamCharObjsController
{

    public List<GroupCharObjsController> m_groups =
        new List<GroupCharObjsController>();

    public void Init(int paramID, E_GROUP_COMMOND groupCommond,
        Vector3 start, Vector3 lookAt)
    {
        //整个team有一个阵型，计算出组成team的每个Group的center
        //每个group进行阵型走位，各个的center是刚才算出的center
        
        GroupCharObjsController group_1 = new GroupCharObjsController();
        group_1.Init(300, E_GROUP_COMMOND.IDLE,
            new Vector3(32, 0, 32), new Vector3(10, 0, 10));

        GroupCharObjsController group_2 = new GroupCharObjsController();
        group_2.Init(300, E_GROUP_COMMOND.IDLE,
            new Vector3(32, 0, 32), new Vector3(10, 0, 10));

        GroupCharObjsController group_3 = new GroupCharObjsController();
        group_3.Init(300, E_GROUP_COMMOND.IDLE,
            new Vector3(32, 0, 32), new Vector3(10, 0, 10));

    }
}
