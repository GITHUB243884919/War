/// <summary>
/// Team控制类
/// author : fanzhengyong
/// date  : 2017-05-19
/// 
/// Team,Group,CharObj三者的关系
/// 
/// Team是由一个或者多个Group组成
/// Group是由一个或者CharObj组成
/// Team对应了一支部队，但归根结底还是由CharObj组成
/// 目前步兵Team由5名士兵组成，坦克Team由3辆坦克组成，炮兵Team相对特殊。
/// 炮兵Team由3个炮兵Group组成，而每个炮兵Group是由1辆炮车和2名士兵组成。
/// TeamCharObjsController完全是因为炮兵Team而开发的。因为实际上是有两层
/// 队形逻辑。Group是一层，有自己队形，Team同样也是。
/// </summary>

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

    public List<Vector3> m_formationPoints = new List<Vector3>();

    public List<bool> m_blackBoard = new List<bool>();

    //当前的Team的阵型ID
    public int TeamFormationParamID { get; set; }

    public Vector3 m_center = Vector3.zero;

    public Vector3 m_lookAt = Vector3.zero;

    public float   m_radius = 0f;

    public TeamCommondFormationParam Param { get; set; }

    public void Init(int paramID, E_GROUP_COMMOND groupCommond,
        Vector3 start, Vector3 lookAt)
    {
        Param = TeamCommondFormationParamManager.Instance.GetParam(paramID);

        m_groupIDs = Param.GetGroup();

        //这里获得是Team的队形参数
        GroupFormationParam formationParam = Param.GetFormationParam(
            groupCommond);

        if (formationParam != null)
        {
            SetFormationPositions(formationParam, start, lookAt, ref m_formationPoints);
        }
        
        for (int i = 0; i < m_groupIDs.Length; i++)
        {
            m_blackBoard.Add(false);
        }

        //把team的队形点集合作为group的center进行各个group的初始化
        if (m_groupIDs.Length > 1)
        {
            for (int i = 0; i < m_groupIDs.Length; i++)
            {
                GroupCharObjsController group = new GroupCharObjsController();
                group.Init(m_groupIDs[i], groupCommond, m_formationPoints[i], lookAt);
                m_groups.Add(group);
            }
        }
        else
        {
            Debug.Log("本Team只有一个Group");
            GroupCharObjsController group = new GroupCharObjsController();
            group.Init(m_groupIDs[0], groupCommond, start, lookAt);
            m_groups.Add(group);
            group.Param.GetFormationParam(groupCommond);
            TeamFormationParamID = group.Param.GetFormationParam(groupCommond).ParamID;
            m_formationPoints = group.m_formationPoints;
            m_center = group.m_center;
            m_radius = group.m_radius;
            //m_lookAt
        }

    }

    public void AI_Arrive(Vector3 start, Vector3 target, float speed)
    {
        //所有对象完成Arrive后的回调：要变成Idle阵型
        ArrivedCallback arrivedCallback = delegate()
        {
            Debug.Log("所有group完成Arrive后要变成Idle阵型");
            TransformFormation(E_GROUP_COMMOND.ATTACK,
                target, target + target.normalized * m_radius, null);
        };

        //阵型变换完成后的回调：执行Group中单个对象的Arrive
        ArrivedCallback transformedCallback = delegate()
        {
            Debug.Log("所有group变成Arrive阵型变化后要依次执行Arrive");
            Arrive(start, target, speed, arrivedCallback);
        };

        //不是ARRIVE的阵型先变成ARRIVE的阵型
        GroupFormationParam arriveParam = Param.GetFormationParam(E_GROUP_COMMOND.ARRIVE);
        if (arriveParam == null)
        {
            arriveParam = m_groups[0].Param.GetFormationParam(E_GROUP_COMMOND.ARRIVE);
            if (arriveParam == null)
            {
                Debug.LogError("没有找到本Team的Arrive阵型参数");
                return;
            }
        }
        //Debug.Log(arriveParam.ParamID);
        if (TeamFormationParamID != arriveParam.ParamID)
        {
            Debug.Log("不是ARRIVE的阵型先变成ARRIVE的阵型" + m_center);
            TransformFormation(E_GROUP_COMMOND.ARRIVE,
                m_center, target, transformedCallback);
        }
        else
        {
            Arrive(start, target, speed, arrivedCallback);
        }
    }

    public void Arrive(Vector3 start, Vector3 target, float speed, ArrivedCallback callback)
    {
        for (int i = 0; i < m_groups.Count; i++)
        {
            Vector3 _start = start + (m_groups[i].m_center - m_center);
            Vector3 _target = target + (_start - m_center);
            m_groups[i].AI_Arrive(_start, _target, speed, callback);
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
        if (formationParam != null)
        {
            SetFormationPositions(formationParam, center, lookAt, ref m_formationPoints);
        }
        else
        {
            formationParam = m_groups[0].Param.GetFormationParam(commond);
        }
        
        Debug.Log("team切换到Commond对应的阵型 " + formationParam.ParamID);
        //Debug.Log("m_groups.Count " + m_groups.Count);

        //每个group进行阵型变换，group的TransformFormation会执行单个charObj的阵型走位
        if (m_groups.Count > 1)
        {
            for (int i = 0; i < m_groups.Count; i++)
            {
                GroupFormationParam _formationParam = m_groups[i].Param.GetFormationParam(commond);
                Debug.Log("group " + m_groups[i].Param.ParamID +
                    "　开始执行变换到阵型　" + _formationParam.ParamID);
                GroupCharObjsController group = m_groups[i];
                m_groups[i].TransformFormation(_formationParam, m_formationPoints[i], lookAt,
                    delegate()
                    {
                        //Debug.Log("Group " + group.Param.ParamID + "走位完成 ");
                        CheakAndSetBlackboard(callback);
                    });
            }
        }
        else
        {
            GroupFormationParam _formationParam = m_groups[0].Param.GetFormationParam(commond);
            Debug.Log("group " + m_groups[0].Param.ParamID +
                "　开始执行变换到阵型　" + _formationParam.ParamID);
            GroupCharObjsController group = m_groups[0];
            m_groups[0].TransformFormation(_formationParam, m_groups[0].m_center, lookAt,
                delegate()
                {
                    //Debug.Log("Group " + group.Param.ParamID + "走位完成 ");
                    CheakAndSetBlackboard(callback);
                });

        }

    }

    public void CheakAndSetBlackboard(ArrivedCallback callback)
    {
        int idx = 0;
        for (int i = 0; i < m_blackBoard.Count; i++)
        {
            idx = i;
            if (!m_blackBoard[idx])
            {
                m_blackBoard[idx] = true;
                break;
            }
        }

        if (idx == (m_groupIDs.Length - 1))
        {
            for (int i = 0; i < m_blackBoard.Count; i++)
            {
                m_blackBoard[i] = false;
            }
            
            Debug.Log("所有Group的走位完成");

            if (callback != null)
            {
                callback();
            }
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
