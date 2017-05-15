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
using UF_FrameWork;

using E_FORMATION_TYPE = GroupCharObjsController.E_FORMATION_TYPE;
using E_BATTLE_OBJECT_TYPE = BattleObjManager.E_BATTLE_OBJECT_TYPE;

public abstract class GroupFormationParam
{
    //参数编号
    public int ParamID { get; set; }

    //队形类型
    public E_FORMATION_TYPE FormationType { get; set; }

    public virtual void SetFormationPositions(List<CharObj> charObjs, Vector3 orgin,
        float targetDeg, Vector3 target, ref List<Vector3> formationPoints)
    {

    }
}

public class TargetVerticalLineFormationParam : GroupFormationParam
{
    public TargetVerticalLineFormationParam()
    {
        FormationType = E_FORMATION_TYPE.TARGET_VERTICAL_LINE;
    }
    
    public float Radius { get; set; }

    public override void SetFormationPositions(List<CharObj> charObjs, Vector3 orgin,
        float targetDeg, Vector3 target,
        ref List<Vector3> formationPoints)
    {
        formationPoints.Clear();
        float angleDeg = 180f;
        Vector3 dir = (target - orgin).normalized;
        Vector3 fristPoint = orgin + dir * Radius;
        Vector3 lastPoint = GeometryUtil.PositionInCycleByAngleDeg2D(orgin, Radius, targetDeg + angleDeg);
        Debug.DrawLine(fristPoint, lastPoint, Color.blue, (fristPoint - lastPoint).magnitude);

        float spaceOffset = (lastPoint - fristPoint).magnitude / (charObjs.Count - 1);
        for (int i = 0; i < charObjs.Count; i++)
        {
            formationPoints.Add(fristPoint - dir * spaceOffset * i);
        }
    }
}

public class TargetCycleFormationParam : GroupFormationParam
{
    public TargetCycleFormationParam()
    {
        FormationType = E_FORMATION_TYPE.TARGET_CYCLE;
    }

    public float Radius { get; set; }

    public override void SetFormationPositions(List<CharObj> charObjs, Vector3 orgin,
        float targetDeg, Vector3 target,
        ref List<Vector3> formationPoints)
    {
        formationPoints.Clear();
        float tempDeg = 360 / charObjs.Count;
        for (int i = 0; i < charObjs.Count; i++)
        {
            Vector3 p = GeometryUtil.PositionInCycleByAngleDeg2D(
                orgin, Radius, targetDeg + i * tempDeg);
            formationPoints.Add(p);
        }
    }
}

public class TargetCycleCenterFormationParam : GroupFormationParam
{
    public TargetCycleCenterFormationParam()
    {
        FormationType = E_FORMATION_TYPE.TARGET_CYCLE_CENTER;
    }

    public float Radius { get; set; }

    public override void SetFormationPositions(List<CharObj> charObjs, Vector3 orgin,
        float targetDeg, Vector3 target,
        ref List<Vector3> formationPoints)
    {
        formationPoints.Clear();
        int _count = charObjs.Count - 1;
        float tempDeg = 360 / _count;
        for (int i = 0; i < _count; i++)
        {
            Vector3 p = GeometryUtil.PositionInCycleByAngleDeg2D(
                orgin, Radius, targetDeg + i * tempDeg);
            formationPoints.Add(p);
        }
        formationPoints.Add(orgin);
    }
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

    public override void SetFormationPositions(List<CharObj> charObjs, Vector3 orgin,
        float targetDeg, Vector3 target,
        ref List<Vector3> formationPoints)
    {
        formationPoints.Clear();

        if (string.IsNullOrEmpty(AttachPoints))
        {
            Debug.LogError("队长挂点数据为空");
            return;
        }
        string [] attachPoints = AttachPoints.Split(',');
        if (attachPoints.Length != charObjs.Count - 1)
        {
            Debug.LogError("队长挂点数目不正确");
            return;
        }

        //非队长依赖挂点，所以要先把队长的位置和朝向设置好
        charObjs[0].GameObject.transform.position = orgin;
        charObjs[0].GameObject.transform.LookAt(target);
        formationPoints.Add(orgin);
        for(int i = 0; i < attachPoints.Length; i++)
        {
            Transform trs = charObjs[0].GameObject.transform.FindChild(attachPoints[i]);
            if (trs == null)
            {
                Debug.LogError("配置的挂点没找到");
                return;
            }
            formationPoints.Add(trs.position);
        }
    }
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
        for (int i = 0; i < attachs.Length; i++)
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
