/// <summary>
/// 队形命令参数
/// author: fanzhengyong
/// date: 2017-05-11
/// 
/// 有多个什么类型的战斗对象组成Group队形，队形命令包含攻击，待机的队形参数
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
    Dictionary<E_GROUP_COMMOND, GroupFormationParam> m_formationParams =
        new Dictionary<E_GROUP_COMMOND, GroupFormationParam>();

    List<E_BATTLE_OBJECT_TYPE> m_charObjTypes = new List<E_BATTLE_OBJECT_TYPE>();

    /// <summary>
    /// 设置m_charObjTypes
    /// </summary>
    /// <param name="param">用英文逗号(，)分割的对象编号（数字）</param>
    public void SetCharObjTypesParam(string param)
    {
        if (string.IsNullOrEmpty(param))
        {
            Debug.LogError("CharObjTypes参数为空");
            return;
        }

        if (m_charObjTypes != null && m_charObjTypes.Count > 0)
        {
            m_charObjTypes.Clear();
        }

        E_BATTLE_OBJECT_TYPE [] types = ParseCharObjTypes(param);

        m_charObjTypes.AddRange(types);
    }

    public E_BATTLE_OBJECT_TYPE[] GetCharObjTypesParam()
    {
        E_BATTLE_OBJECT_TYPE[] types = null;
        
        if (m_charObjTypes!= null)
        {
            types = m_charObjTypes.ToArray();
        }

        return types;
    }

    /// <summary>
    /// 队形参数有多个，因此本函数需要调用多次
    /// </summary>
    /// <param name="groupCommond"></param>
    /// <param name="formationParamID"></param>
    public void AddFormationParam(E_GROUP_COMMOND groupCommond, int formationParamID)
    {
        bool retCode = false;

        GroupFormationParam formationParam =
            GroupFormationParamManager.Instance.GetParam(formationParamID);
        if (formationParam == null)
        {
            Debug.LogError("队形参数没有取到 " + formationParamID);
            return;
        }

        GroupFormationParam _formationParam = null;
        retCode = m_formationParams.TryGetValue(groupCommond, out _formationParam);
        if (retCode)
        {
            Debug.LogError("队形参数重复");
            return;
        }

        m_formationParams.Add(groupCommond, formationParam);
    }

    public GroupFormationParam GetFormationParam(E_GROUP_COMMOND groupCommond)
    {
        GroupFormationParam param = null;
        
        if (m_formationParams != null)
        {
            m_formationParams.TryGetValue(groupCommond, out param);
        }

        return param;
    }

    public void Release()
    {
        if (m_formationParams != null)
        {
            m_formationParams.Clear();
            m_formationParams = null;
        }
    }

    public E_BATTLE_OBJECT_TYPE[] ParseCharObjTypes(string charObjTypes)
    {
        bool retCode = false;

        E_BATTLE_OBJECT_TYPE[] types = null;
        string[] objTypes = charObjTypes.Split(',');
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
                s_instance.Init();
            }

            return s_instance;
        }
    }

    Dictionary<int, GroupCommondFormationParam> m_params =
        new Dictionary<int, GroupCommondFormationParam>();

    void Init()
    {
        GroupCommondFormationParam[] _params =
            GroupCommondFormationParamMediator.GetGroupCommondFormationParams();

        if (_params == null)
        {
            Debug.LogError("GroupCommondFormationParam 配置读取失败");
            return;
        }

        for(int i = 0; i < _params.Length; i++)
        {
            Debug.Log(_params[i].ParamID);
            AddParam(_params[i].ParamID, _params[i]);
        }
    }

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

    public void Release()
    {
        if (m_params != null)
        {
            foreach(var param in m_params.Values)
            {
                param.Release();
            }
        }
        m_params.Clear();
        m_params = null;
    }
}

