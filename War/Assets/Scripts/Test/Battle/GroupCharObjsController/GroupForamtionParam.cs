/// <summary>
/// 队形参数基类
/// author: fanzhengyong
/// date: 2017-05-11
/// 
/// 派生类记录参数，每个派生类对应一种队形（E_FORMATION_TYPE）
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using E_FORMATION_TYPE = GroupCharObjsController.E_FORMATION_TYPE;
using E_BATTLE_OBJECT_TYPE = BattleObjManager.E_BATTLE_OBJECT_TYPE;

public abstract class GroupFormationParam
{
    //参数编号
    public int ParamID { get; set; }

    //队形类型
    public E_FORMATION_TYPE FormationType { get; set; }
}

public class TargetVerticalLineFormationParam : GroupFormationParam
{
    public TargetVerticalLineFormationParam()
    {
        FormationType = E_FORMATION_TYPE.TARGET_VERTICAL_LINE;
    }
    
    public float Radius { get; set; }
}

public class TargetCycleFormationParam : GroupFormationParam
{
    public TargetCycleFormationParam()
    {
        FormationType = E_FORMATION_TYPE.TARGET_CYCLE;
    }

    public float Radius { get; set; }
}

public class TargetCycleCenterFormationParam : GroupFormationParam
{
    public TargetCycleCenterFormationParam()
    {
        FormationType = E_FORMATION_TYPE.TARGET_CYCLE_CENTER;
    }

    public float Radius { get; set; }
}

public class TargetAttachCaptionFormationParam : GroupFormationParam
{
    public TargetAttachCaptionFormationParam()
    {
        FormationType = E_FORMATION_TYPE.TARGET_ATTACH_CAPTION;
    }

    public float Radius { get; set; }

    //用英文逗号(，)分割的挂点名称（英文）
    public string AttachPoints { get; set; }
}

/// <summary>
/// 单个队形参数（GroupFormationParam）的管理类
/// 
/// author: fanzhengyong
/// date: 2017-05-11
/// 
/// 游戏中一共有多少种队形，相同的队形由于参数不同可能会重复
/// </summary>
public class GroupFormationParamManager
{
    private static GroupFormationParamManager s_instance = null;
    public static GroupFormationParamManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new GroupFormationParamManager();
                s_instance.Init();
            }

            return s_instance;
        }
    }

    Dictionary<int, GroupFormationParam> m_params =
        new Dictionary<int, GroupFormationParam>();

    void Init()
    {
        GroupFormationParam[] lines =
            GroupFormationParamConfigerMediator.GetTargetVerticalLineFormationParams();
        for(int i = 0; i < lines.Length; i++)
        {
            AddParam(lines[i].ParamID, lines[i]);
        }

        GroupFormationParam[] cycles =
            GroupFormationParamConfigerMediator.GetTargetCycleFormationParams();
        for (int i = 0; i < cycles.Length; i++)
        {
            AddParam(cycles[i].ParamID, cycles[i]);
        }

        GroupFormationParam[] cycleCenters =
            GroupFormationParamConfigerMediator.GetTargetCycleCenterFormationParams();
        for (int i = 0; i < cycleCenters.Length; i++)
        {
            AddParam(cycleCenters[i].ParamID, cycleCenters[i]);
        }

        GroupFormationParam[] attachs =
            GroupFormationParamConfigerMediator.GetTargetAttachCaptionFormationParams();
        for (int i = 0; i < cycleCenters.Length; i++)
        {
            AddParam(attachs[i].ParamID, attachs[i]);
        }
    }

    void AddParam(int paramID, GroupFormationParam param)
    {
        bool retCode = false;
        GroupFormationParam _param = null;

        retCode = m_params.TryGetValue(paramID, out _param);
        if (retCode)
        {
            Debug.LogError("GroupFormationParam 重复");
            return;
        }

        m_params.Add(paramID, param);
    }

    public GroupFormationParam GetParam(int paramID)
    {
        GroupFormationParam param = null;

        m_params.TryGetValue(paramID, out param);
        
        return param;
    }

    public void Release()
    {
        if (m_params != null)
        {
            m_params.Clear();
        }
        m_params = null;
    }
}
