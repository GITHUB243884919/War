/// <summary>
/// 队形命令参数基
/// author: fanzhengyong
/// date: 2017-05-11
/// 
/// 队形命令包含攻击，待机的队形参数
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using E_BATTLE_OBJECT_TYPE = BattleObjManager.E_BATTLE_OBJECT_TYPE;

public class GroupCommondFormationParam
{
    public int ParamID { get; set; }

    public enum E_GROUP_COMMOND
    {
        IDLE,
        MOVE,
        ATTACK,
        SKILL
    }

    //public GroupFormationParam IdleParam;
    //public GroupFormationParam MoveParam;
    //public GroupFormationParam AttackParam;
    //public GroupFormationParam SkillParam;
    Dictionary<E_GROUP_COMMOND, GroupFormationParam> m_params =
        new Dictionary<E_GROUP_COMMOND, GroupFormationParam>();

    //用英文逗号(，)分割的对象编号（数字）
    public string CharObjs { get; set; }

    public void AddParam(E_GROUP_COMMOND groupCommond, GroupFormationParam param)
    {
        GroupFormationParam _param = null;
        bool retCode = false;

        retCode = m_params.TryGetValue(groupCommond, out _param);
        if (retCode)
        {
            Debug.LogError("参数重复");
            return;
        }

        m_params.Add(groupCommond, param);
    }

    public E_BATTLE_OBJECT_TYPE[] GetCharObjTypes()
    {
        bool retCode = false;

        E_BATTLE_OBJECT_TYPE[] types = null;
        string[] objTypes = CharObjs.Split(',');
        types = new E_BATTLE_OBJECT_TYPE[objTypes.Length];
        for (int i = 0; i < objTypes.Length; i++)
        {
            int nParam = 0;
            try
            {
                nParam = Convert.ToInt32(objTypes[i]);
                retCode = true;
            }
            catch (Exception)
            {
                retCode = false;
            }

            if (!retCode)
            {
                Debug.LogError("参数非法");
                break;
            }

            retCode = Enum.IsDefined(typeof(E_BATTLE_OBJECT_TYPE), nParam);
            if (!retCode)
            {
                Debug.LogError("参数非法");
                break;
            }

            types[i] = (E_BATTLE_OBJECT_TYPE)Enum.Parse(typeof(E_BATTLE_OBJECT_TYPE), objTypes[i]);
        }

        return types;
    }
}

public class GroupCommondFormationParamManager
{
    private static GroupCommondFormationParamManager s_instance = null;
    public static GroupCommondFormationParamManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new GroupCommondFormationParamManager();
            }

            return s_instance;
        }
    }

    Dictionary<int, GroupCommondFormationParam> m_params =
        new Dictionary<int, GroupCommondFormationParam>();

    public void AddParam(int paramID, GroupCommondFormationParam param)
    {
        bool retCode = false;
        GroupCommondFormationParam _param = null;

        retCode = m_params.TryGetValue(paramID, out _param);
        if (retCode)
        {
            Debug.LogError("GroupCommondFormationParam 重复");
            return;
        }

        m_params.Add(paramID, param);
    }

    public GroupCommondFormationParam GetParam(int paramID)
    {
        GroupCommondFormationParam param = null;

        m_params.TryGetValue(paramID, out param);

        return param;
    }
}

