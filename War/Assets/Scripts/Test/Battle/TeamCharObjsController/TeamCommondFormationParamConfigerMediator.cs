/// <summary>
/// Group队形命令参数读取中介
/// author: fanzhengyong
/// date: 2017-05-18
/// 
/// group用逗号分开的字符串
/// 每个Commond对应的队形参数是数字，分别对应个参数表的ID
/// </summary>

using UnityEngine;
using System.Collections;

using E_GROUP_COMMOND = GroupCommondFormationParam.E_GROUP_COMMOND;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

public static class TeamCommondFormationParamConfigerMediator
{
    public static TeamCommondFormationParam[]
        GetTeamCommondFormationParams()
    {
        TeamCommondFormationParam[] _params = null;
        _params = new TeamCommondFormationParam[3];

        TeamCommondFormationParam soldiers = new TeamCommondFormationParam();
        soldiers.ParamID = 1000;
        soldiers.SetGroup("100");

        _params[0] = soldiers;

        TeamCommondFormationParam tanks = new TeamCommondFormationParam();
        tanks.ParamID = 2000;
        tanks.SetGroup("200");
        _params[1] = tanks;

        TeamCommondFormationParam artillerys = new TeamCommondFormationParam();
        artillerys.ParamID = 3000;
        artillerys.SetGroup("300,300,300");
        artillerys.AddFormationParam(E_GROUP_COMMOND.IDLE, 12);
        artillerys.AddFormationParam(E_GROUP_COMMOND.ARRIVE, 12);
        artillerys.AddFormationParam(E_GROUP_COMMOND.ATTACK, 22);
        artillerys.AddFormationParam(E_GROUP_COMMOND.SKILL, 22);
        _params[2] = artillerys;

        return _params;
    }
}
