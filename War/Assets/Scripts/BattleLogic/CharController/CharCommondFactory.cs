/// <summary>
/// CharCommond的工厂类，负责根据参数创建出不同的CharCommond，Tank，Soldier...
/// author: fanzhengyong
/// date: 2017-02-27
/// 
/// 持有多个对象池，按资源路径放到map中
/// </summary>
using UnityEngine;
using System.Collections;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

public class CharCommondFactory
{
    public static CharCommond CreateCommond(CharController cctr, 
        BattleObjManager.E_BATTLE_OBJECT_TYPE charType)
    {
        CharCommond cmd = null;
        switch (charType)
        {
            case BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK:
                cmd = new TankCommond(cctr);
                break;
            case BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK:
                cmd = new M_Arm_Tank_Commond(cctr);
                break;
            case BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_AIRPLANE_01:
                cmd = new M_Arm_Airplane_01_Commond(cctr);
                break;
            case BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCAR:
                cmd = new M_Arm_Engineercar_Commond(cctr);
                break;
            case BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS:
                cmd = new M_Arm_Engineercorps_Commond(cctr);
                break;
            case BattleObjManager.E_BATTLE_OBJECT_TYPE.M_BUILD_JEEP:
                cmd = new M_Build_Jeep_Commond(cctr);
                break;
            case BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ARTILLERY:
                cmd = new M_Arm_Artillery_Commond(cctr);
                break;
            default:
                //cmd = new TankCommond(cctr);
                Debug.LogError("不存在这类CharCommond无法生成 " + charType.ToString());
                break;
        }

        return cmd;
    }
}

