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

    //对象类型集（坦克，士兵。。。。的混合，或者是单个）
    public List<E_BATTLE_OBJECT_TYPE> m_objTypes =
        new List<E_BATTLE_OBJECT_TYPE>();

    public void AddCharObjType(E_BATTLE_OBJECT_TYPE type)
    {
        m_objTypes.Add(type);
    }

    public void Release()
    {
        if (m_objTypes != null)
        {
            m_objTypes.Clear();
            m_objTypes = null;
        }
    }
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
public class GroupFormationParamManager1
{
    private static GroupFormationParamManager1 s_instance = null;
    public static GroupFormationParamManager1 Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new GroupFormationParamManager1();
            }

            return s_instance;
        }
    }

    Dictionary<int, GroupFormationParam> m_params =
        new Dictionary<int, GroupFormationParam>();

    public void AddParam(int paramID, GroupFormationParam param)
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
}

public class GroupFormationParamConfigerMediator
{
    public static TargetVerticalLineFormationParam [] GetTargetVerticalLineFormationParams()
    {
        TargetVerticalLineFormationParam [] lines = null;

        lines = new TargetVerticalLineFormationParam[3];

        TargetVerticalLineFormationParam soldierLine = 
            new TargetVerticalLineFormationParam();
        soldierLine.ParamID = 0;
        soldierLine.Radius = 3f;
        soldierLine.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS);
        soldierLine.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS);
        soldierLine.AddCharObjType(E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS);


        lines[0] = soldierLine;


        return lines;
    }

    //E_BATTLE_OBJECT_TYPE [] GetCharObjTypes(string charObjs)
    //{
    //    E_BATTLE_OBJECT_TYPE[] types = null;
    //    //string 
    //    return types;
    //}

    public static E_BATTLE_OBJECT_TYPE[] GetCharObjTypes(string charObjTypes)
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
