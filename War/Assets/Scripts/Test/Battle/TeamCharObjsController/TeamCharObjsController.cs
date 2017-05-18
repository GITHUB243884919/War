using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UF_FrameWork;

using E_GROUP_COMMOND = GroupCommondFormationParam.E_GROUP_COMMOND;
using ArrivedCallback = GroupCharObjsController.ArrivedCallback;
#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

public class TeamCharObjsController
{

    public List<GroupCharObjsController> m_groups =
        new List<GroupCharObjsController>();

    public int [] m_groupIDs = null;

    public List<Vector3> m_formationPoints =
        new List<Vector3>();

    //当前的Team的阵型ID
    public int TeamFormationParamID { get; set; }

    public Vector3 m_center = Vector3.zero;

    public Vector3 m_lookAt = Vector3.zero;

    public float   m_radius = 0f;

    public TeamCommondFormationParam Param { get; set; }

    public void Init(int paramID, E_GROUP_COMMOND groupCommond,
        Vector3 start, Vector3 lookAt)
    {
        //整个team有一个阵型，计算出组成team的每个Group的center
        //每个group进行阵型走位，各个的center是刚才算出的center

        Param = TeamCommondFormationParamManager.Instance.GetParam(paramID);

        //这里获得是Team的队形参数
        GroupFormationParam formationParam = Param.GetFormationParam(
            groupCommond);

        m_groupIDs = Param.GetGroup();
        SetFormationPositions(formationParam, start, lookAt, ref m_formationPoints);
        
        for (int i = 0; i < m_groupIDs.Length; i++)
        {
            GroupCharObjsController group = new GroupCharObjsController();
            group.Init(m_groupIDs[i], groupCommond, m_formationPoints[i], lookAt);
            m_groups.Add(group);
        }
    }

    public void SetFormationPositions(GroupFormationParam formationParam,
        Vector3 center, Vector3 lookAt, ref List<Vector3> formationPoints)
    {
        m_center = center;
        m_lookAt = lookAt;

        float lookAtRad = GeometryUtil.TwoPointAngleRad2D(center, lookAt);
        float lookAtDeg = lookAtRad * Mathf.Rad2Deg;

        TeamFormationParamID = formationParam.ParamID;

        formationParam.SetFormationPositions(m_groupIDs.Length, center, lookAtDeg, lookAt, ref formationPoints);
        if (m_groupIDs.Length != formationPoints.Count)
        {
            Debug.LogError("team的group数量不一致 " + 
                m_groupIDs.Length + " " + formationPoints.Count);
            return;
        }

        m_radius = formationParam.Radius;
    }

    public void TransformFormation(E_GROUP_COMMOND commond,
        Vector3 center, Vector3 lookAt, ArrivedCallback callback)
    {
        Debug.Log("team切换到Commond " + commond.ToString());
        
        //计算整个Team的阵型位置集，作为各Group的center
        GroupFormationParam formationParam = Param.GetFormationParam(commond);
        SetFormationPositions(formationParam, center, lookAt, ref m_formationPoints);
        Debug.Log("team切换到Commond对应的阵型 " + formationParam.ParamID);
        //Debug.Log("m_groups.Count " + m_groups.Count);

        //每个group进行阵型变换，group的TransformFormation会执行单个charObj的阵型走位
        for (int i = 0; i < m_groups.Count; i++)
        {
            GroupFormationParam _formationParam  = m_groups[i].Param.GetFormationParam(commond);
            Debug.Log("group " + m_groups[i].Param.ParamID + 
                "　开始执行变换到阵型　" + _formationParam.ParamID);
            GroupCharObjsController group = m_groups[i];
            m_groups[i].TransformFormation(_formationParam, m_formationPoints[i], lookAt, 
                delegate()
                {
                    Debug.Log("Group " + group.Param.ParamID + "走位完成 ");
                });
        }
    }

    public void Release()
    {
        if (m_groups != null)
        {
            for(int i = 0; i < m_groups.Count; i++)
            {
                m_groups[i].Release();
            }
            m_groups.Clear();
            m_groups = null;
        }

        if (m_formationPoints != null)
        {
            m_formationPoints.Clear();
            m_formationPoints = null;
        }

        if (Param != null)
        {
            Param.Release();
            Param = null;
        }
    }
}
