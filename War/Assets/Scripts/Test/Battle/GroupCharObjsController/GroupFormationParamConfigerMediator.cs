/// <summary>
/// 各个GroupFormationParam配置表读取中介
/// 
/// author: fanzhengyong
/// date: 2017-05-12
/// 
/// 提供读取各个队形配置表的能力
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;

using E_FORMATION_TYPE = GroupCharObjsController.E_FORMATION_TYPE;
using E_BATTLE_OBJECT_TYPE = BattleObjManager.E_BATTLE_OBJECT_TYPE;
public static class GroupFormationParamConfigerMediator
{
    public static GroupFormationParam[]
        GetTargetVerticalLineFormationParams()
    {
        TargetVerticalLineFormationParam[] lines = null;

        lines = new TargetVerticalLineFormationParam[2];

        TargetVerticalLineFormationParam soldierLine =
            new TargetVerticalLineFormationParam();
        soldierLine.ParamID = 10;
        soldierLine.Radius = 3f;
        lines[0] = soldierLine;

        TargetVerticalLineFormationParam tankLine =
            new TargetVerticalLineFormationParam();
        tankLine.ParamID = 11;
        tankLine.Radius = 3f;
        lines[1] = tankLine;

        return lines;
    }

    public static GroupFormationParam[]
        GetTargetCycleFormationParams()
    {
        TargetCycleFormationParam[] cycles = null;
        cycles = new TargetCycleFormationParam[2];

        TargetCycleFormationParam soldierCycle =
            new TargetCycleFormationParam();
        soldierCycle.ParamID = 20;
        soldierCycle.Radius = 3;
        cycles[0] = soldierCycle;

        TargetCycleFormationParam tankCycle =
            new TargetCycleFormationParam();
        tankCycle.ParamID = 21;
        tankCycle.Radius = 3;
        cycles[1] = tankCycle;

        return cycles;
    }

    public static GroupFormationParam[]
        GetTargetCycleCenterFormationParams()
    {
        TargetCycleCenterFormationParam[] cycleCenters = null;
        cycleCenters = new TargetCycleCenterFormationParam[1];

        TargetCycleCenterFormationParam soldier =
           new TargetCycleCenterFormationParam();

        soldier.ParamID = 30;
        soldier.Radius = 3f;
        cycleCenters[0] = soldier;

        return cycleCenters;
    }

    public static GroupFormationParam[]
        GetTargetAttachCaptionFormationParams()
    {
        TargetAttachCaptionFormationParam[] attachs = null;
        attachs = new TargetAttachCaptionFormationParam[2];

        TargetAttachCaptionFormationParam idle =
            new TargetAttachCaptionFormationParam();
        idle.ParamID = 40;
        idle.Radius = 3f;
        idle.AttachPoints = "Move_Attach_Point_1,Move_Attach_Point_2";
        attachs[0] = idle;

        TargetAttachCaptionFormationParam attack =
            new TargetAttachCaptionFormationParam();
        attack.ParamID = 41;
        attack.Radius = 3f;
        attack.AttachPoints = "Battle_Attach_Point_1,Battle_Attach_Point_2";
        attachs[1] = attack;

        return attachs;
    }


}
