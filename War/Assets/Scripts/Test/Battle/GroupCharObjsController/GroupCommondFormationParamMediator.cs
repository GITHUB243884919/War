using UnityEngine;
using System.Collections;

/// <summary>
/// 队形命令参数读取中介
/// author: fanzhengyong
/// date: 2017-05-12
/// 
/// 对象类型用逗号分开的字符串
/// 每个Commond对应的队形参数是数字，分别对应个参数表的ID
/// </summary>
public static class GroupCommondFormationParamMediator
{
    public static GroupCommondFormationParam[]
        GetGroupCommondFormationParams()
    {
        GroupCommondFormationParam[] _params = null;
        _params = new GroupCommondFormationParam[3];

        GroupCommondFormationParam soldiers = new GroupCommondFormationParam();
        soldiers.ParamID = 100;
        soldiers.SetCharObjTypesParam("5,5,5,5,5");
        soldiers.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.IDLE, 30);
        soldiers.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.MOVE, 10);
        soldiers.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.ATTACK, 20);
        soldiers.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.SKILL, 20);
        _params[0] = soldiers;

        GroupCommondFormationParam tanks = new GroupCommondFormationParam();
        tanks.ParamID = 200;
        tanks.SetCharObjTypesParam("2,2,2");
        tanks.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.IDLE, 11);
        tanks.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.MOVE, 11);
        tanks.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.ATTACK, 21);
        tanks.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.SKILL, 21);
        _params[1] = tanks;


        GroupCommondFormationParam artillerys = new GroupCommondFormationParam();
        artillerys.ParamID = 300;
        artillerys.SetCharObjTypesParam("7,5,5");
        artillerys.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.IDLE, 40);
        artillerys.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.MOVE, 40);
        artillerys.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.ATTACK, 41);
        artillerys.AddFormationParam(
            GroupCommondFormationParam.E_GROUP_COMMOND.SKILL, 41);
        _params[1] = artillerys;

        return _params;
    }
}
