/// <summary>
/// team队形命令参数
/// author: fanzhengyong
/// date: 2017-05-18
/// 
/// 有多个group组成team队形，队形命令包含攻击，待机的队形参数
/// </summary>
/// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using E_GROUP_COMMOND = GroupCommondFormationParam.E_GROUP_COMMOND;

public class TeamCommondFormationParam
{
    public int ParamID { get; set; }

    //包含的group
    List<int> m_groups = new List<int>();

    //Team的Commond应该被Group的Commond包含，因此借用Group的Commond
    Dictionary<E_GROUP_COMMOND, GroupFormationParam> m_formationParams =
        new Dictionary<E_GROUP_COMMOND, GroupFormationParam>();

    /// <summary>
    /// 用，分割开的group，可能没有逗号，也就是说这个team只包含一个group
    /// </summary>
    /// <param name="param"></param>
    public void SetGroup(string param)
    {
        if (string.IsNullOrEmpty(param))
        {
            Debug.LogError("CharObjTypes参数为空");
            return;
        }

        if (m_groups != null && m_groups.Count > 0)
        {
            m_groups.Clear();
        }

        m_groups.AddRange(ParseGroups(param));
    }

    public int [] GetGroup()
    {
        int [] groups = null;

        if (m_groups != null)
        {
            groups = m_groups.ToArray();
        }

        return groups;
    }

    public int [] ParseGroups(string groups)
    {
        bool retCode = false;

        int[] nGroups = null;
        string[] sGrpups = groups.Split(',');
        if (sGrpups.Length == 0)
        {
            int nParam = 0;
            try
            {
                nParam  = Convert.ToInt32(groups);
                retCode = true;
            }
            catch (Exception)
            {
                retCode = false;
            }

            if (!retCode)
            {
                Debug.LogError("team的groups参数非法 " + groups);
                return nGroups;
            }

            nGroups = new int[1];
            nGroups[0] = nParam;
            return nGroups;
        }
        else
        {
            nGroups = new int[sGrpups.Length];
        }

        for (int i = 0; i < sGrpups.Length; i++)
        {
            int nParam = 0;
            try
            {
                nParam = Convert.ToInt32(sGrpups[i]);
                retCode = true;
            }
            catch (Exception)
            {
                retCode = false;
            }

            if (!retCode)
            {
                Debug.LogError("team的groups参数非法 " + groups);
                break;
            }
        }

        return nGroups;
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

        if (m_groups != null)
        {
            m_groups.Clear();
            m_groups = null;
        }
    }
}


public class TeamCommondFormationParamManager
{
    private static TeamCommondFormationParamManager s_instance = null;
    public static TeamCommondFormationParamManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new TeamCommondFormationParamManager();
                //s_instance.Init();
            }

            return s_instance;
        }
    }

    Dictionary<int, TeamCommondFormationParam> m_params =
        new Dictionary<int, TeamCommondFormationParam>();

    //void Init()
    //{
    //    GroupCommondFormationParam[] _params =
    //        GroupCommondFormationParamMediator.GetGroupCommondFormationParams();

    //    if (_params == null)
    //    {
    //        Debug.LogError("GroupCommondFormationParam 配置读取失败");
    //        return;
    //    }

    //    for (int i = 0; i < _params.Length; i++)
    //    {
    //        //Debug.Log(_params[i].ParamID);
    //        AddParam(_params[i].ParamID, _params[i]);
    //    }
    //}

    public void AddParam(int paramID, TeamCommondFormationParam param)
    {
        bool retCode = false;
        TeamCommondFormationParam _param = null;

        retCode = m_params.TryGetValue(paramID, out _param);
        if (retCode)
        {
            Debug.LogError("GroupCommondFormationParam 重复");
            return;
        }

        m_params.Add(paramID, param);
    }

    public TeamCommondFormationParam GetParam(int paramID)
    {
        TeamCommondFormationParam param = null;

        m_params.TryGetValue(paramID, out param);

        return param;
    }

    public void Release()
    {
        if (m_params != null)
        {
            foreach (var param in m_params.Values)
            {
                param.Release();
            }
        }
        m_params.Clear();
        m_params = null;
    }
}

