using UnityEngine;
using System.Collections;

using E_GROUP_TYPE = GroupFormationParamManager.E_GROUP_TYPE;
using E_BATTLE_OBJECT_TYPE = BattleObjManager.E_BATTLE_OBJECT_TYPE;
using E_FORMATION_TYPE = GroupCharObjsController.E_FORMATION_TYPE;

public static class ReadGroupFormationMediator
{

    public static GroupFormationParam Read(E_GROUP_TYPE type)
    {
        GroupFormationParam param = null;
        switch(type)
        {
            case E_GROUP_TYPE.TANKS:
                {
                    GroupFormationParam _param = new CycleFormationParam();
                    ReadTanks(ref _param);
                    param = _param;
                }
                break;
            case E_GROUP_TYPE.SOLDIERS:
                {
                    GroupFormationParam _param = new CycleFormationParam();
                    ReadSoldier(ref _param);
                    param = _param;
                }
                break;
            default:
                break;
        }

        return param;
    }

    private static void ReadTanks(ref GroupFormationParam param)
    {
        //多少对象
        param.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_TANK);
        param.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_TANK);
        param.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_TANK);

        //布阵用参数
        ((CycleFormationParam)param).IdleFormation =
            E_FORMATION_TYPE.TARGET_VERTICAL_LINE;
        ((CycleFormationParam)param).IdleRadius = 3f;

        ((CycleFormationParam)param).MoveFormation =
            E_FORMATION_TYPE.TARGET_VERTICAL_LINE;
        ((CycleFormationParam)param).MoveRadius = 3f;

        ((CycleFormationParam)param).AttackFormation =
            E_FORMATION_TYPE.TARGET_CYCLE;
        ((CycleFormationParam)param).MoveRadius = 3f;
    }

    private static void ReadSoldier(ref GroupFormationParam param)
    {
        param.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS);
        param.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS);
        param.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS);
        param.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS);
        param.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS);

        //阵型参数
        ((CycleFormationParam)param).IdleFormation = E_FORMATION_TYPE.TARGET_CYCLE_CENTER;
        ((CycleFormationParam)param).IdleRadius = 3f;

        ((CycleFormationParam)param).MoveFormation = E_FORMATION_TYPE.TARGET_VERTICAL_LINE;
        ((CycleFormationParam)param).MoveRadius = 3f;

        ((CycleFormationParam)param).AttackFormation = E_FORMATION_TYPE.TARGET_CYCLE;
        ((CycleFormationParam)param).MoveRadius = 3f;
    }
}
