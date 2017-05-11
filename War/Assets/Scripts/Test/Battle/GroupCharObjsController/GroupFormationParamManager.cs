//using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
using System;

using E_FORMATION_TYPE = GroupCharObjsController.E_FORMATION_TYPE;

public class CycleFormationParam : GroupFormationParam
{
    public E_FORMATION_TYPE IdleFormation { get; set; }
    public float IdleRadius { get; set; }

    public E_FORMATION_TYPE MoveFormation { get; set; }
    public float MoveRadius { get; set; }

    public E_FORMATION_TYPE AttackFormation { get; set; }
    public float AttackRadius { get; set; }


    public List<BattleObjManager.E_BATTLE_OBJECT_TYPE> m_objTypes =
        new List<BattleObjManager.E_BATTLE_OBJECT_TYPE>();

    //public GroupCharObjsController.E_FORMATION_TYPE m_formationType;

    //public void Init(string objs, float radius)
    //{
    //    string[] split = objs.Split(',');
    //}



}


public class GroupFormationParamManager
{
    public enum E_GROUP_TYPE
    {
        TANKS,
        SOLDIERS
    }

    private static GroupFormationParamManager s_instance = null;
    public static GroupFormationParamManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new GroupFormationParamManager();
            }

            return s_instance;
        }
    }

    public Dictionary<E_GROUP_TYPE, GroupFormationParam> m_params
        = new Dictionary<E_GROUP_TYPE, GroupFormationParam>();

    public void Init()
    {
        bool retCode = false;
        foreach (E_GROUP_TYPE groupType in Enum.GetValues(typeof(E_GROUP_TYPE)))
        {
            GroupFormationParam param = null;
            retCode = m_params.TryGetValue(groupType, out param);
            if (!retCode)
            {
                //param = ReadGroupCharObjsParamMediator.Read(groupType);
            }
        }
    }
}
