﻿/// <summary>
/// CharCommond的工厂类，负责根据参数创建出不同的CharCommond，Tank，Soldier...
/// author: fanzhengyong
/// date: 2017-02-27
/// 
/// 持有多个对象池，按资源路径放到map中
/// </summary>
using UnityEngine;
using System.Collections;

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
            default:
                //cmd = new TankCommond(cctr);
                Debug.LogError("不存在这类CharCommond无法生成 " + charType.ToString());
                break;
        }

        return cmd;
    }
}

