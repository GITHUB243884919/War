/// <summary>
/// Group的控制类
/// author: fanzhengyong
/// date: 2017-05-17
/// 
/// Group是包含CharObj，实现多个CharObj组成的Group的队形转换
/// 实现了Arrive和Attack的行为
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UF_FrameWork;

using E_GROUP_COMMOND = GroupCommondFormationParam.E_GROUP_COMMOND;
using E_BATTLE_OBJECT_TYPE = BattleObjManager.E_BATTLE_OBJECT_TYPE;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

public class GroupCharObjsElement
{
    public int ServerEntityID { get; set; }
    public BattleObjManager.E_BATTLE_OBJECT_TYPE Type { get; set; }

    public bool Arrived { get; set; }
}

public class GroupCharObjsController
{
    public enum E_FORMATION_TYPE
    {
        NONE,
        TARGET_VERTICAL_LINE,    //面朝目标纵向一字型
        TARGET_CYCLE,            //面朝目标散开
        TARGET_CYCLE_CENTER,     //面朝目标散开有一个站中间
        TARGET_ATTACH_CAPTION    //面朝目标并基于队长模型上的挂点
    }

    //CharObj对象缓存
    List<CharObj> m_charObjs = new List<CharObj>();

    //CharObj对象元素缓存（里面不包含对象，初始时只包含对象的ID）
    Dictionary<int, GroupCharObjsElement> m_elments 
        = new Dictionary<int, GroupCharObjsElement>();

    bool m_isCached = false;

    //队形形成的点的集合
    public List<Vector3> m_formationPoints = new List<Vector3>();

    //阵型的中心点
    public Vector3 m_center = Vector3.zero;
    //阵型的朝向
    public Vector3 m_lookAt = Vector3.zero;
    //阵型的半径
    public float   m_radius = 5f;

    public delegate void ArrivedCallback();

    //整个group的参数
    public GroupCommondFormationParam Param { get; set; }

    //当前的队形ID
    public int GroupFormationParamID { get; set; }
    
    /// <summary>
    /// 带参数表接口的最外层初始化函数（一级）
    /// </summary>
    /// <param name="paramID"></param>
    /// <param name="groupCommond"></param>
    /// <param name="start"></param>
    /// <param name="lookAt"></param>
    public void Init(int paramID, E_GROUP_COMMOND groupCommond,
        Vector3 start, Vector3 lookAt)
    {
        Param = GroupCommondFormationParamManager.Instance.GetParam(paramID);

        E_BATTLE_OBJECT_TYPE[] objTypes = Param.GetCharObjTypesParam();

        GroupCharObjsElement[] elements =
            new GroupCharObjsElement[objTypes.Length];

        for(int i = 0; i < objTypes.Length; i++)
        {
            GroupCharObjsElement element = new GroupCharObjsElement();
            element.ServerEntityID = 
                BattleObjEntityIDManager.Instance.GenEntityID();
            element.Type = objTypes[i];
            elements[i] = element;
        }

        Init(elements, Param.GetFormationParam(groupCommond), start, lookAt);
    }

    /// <summary>
    /// 带参数表接口的最外层初始化函数（二级）
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="groupFormationParam"></param>
    /// <param name="start"></param>
    /// <param name="lookAt"></param>
    public void Init(GroupCharObjsElement[] elements, 
        GroupFormationParam groupFormationParam, Vector3 start, Vector3 lookAt)
    {
        CacheObjs(elements);

        SetFormationPositions(groupFormationParam, start, lookAt);

        for (int i = 0; i < m_charObjs.Count; i++)
        {
            m_charObjs[i].AI_LookAt(m_formationPoints[i], lookAt);
        }
    }

    /// <summary>
    /// 设置某阵型的各个对象（CharObj）的位置
    /// </summary>
    /// <param name="charObjs"></param>
    /// <param name="groupFormationParam"></param>
    /// <param name="center"></param>
    /// <param name="lookAt"></param>
    public void SetFormationPositions(GroupFormationParam groupFormationParam,
        Vector3 center,  Vector3 lookAt)
    {
        m_center = center;
        m_lookAt = lookAt;

        GroupFormationParamID = groupFormationParam.ParamID;

        float lookAtRad = GeometryUtil.TwoPointAngleRad2D(center, lookAt);
        float lookAtDeg = lookAtRad * Mathf.Rad2Deg;

        groupFormationParam.SetFormationPositions(m_charObjs, center, lookAtDeg, 
            lookAt, ref m_formationPoints);

        m_radius = groupFormationParam.Radius;
    }

    /// <summary>
    /// 阵型变换
    /// </summary>
    /// <param name="groupFormationParam"></param>
    /// <param name="center"></param>
    /// <param name="lookAt"></param>
    public void TransformFormation(GroupFormationParam groupFormationParam,
        Vector3 center, Vector3 lookAt, ArrivedCallback callback)
    {
        Debug.Log("切换到阵型类型 " + groupFormationParam.FormationType.ToString()
            + "阵型ID " + groupFormationParam.ParamID);

        SetFormationPositions(groupFormationParam, center, lookAt);

        for (int i = 0; i < m_charObjs.Count; i++)
        {
            //Debug.Log("移动到指定位置 " + m_formationPoints[i]);
            CharObj charObj = m_charObjs[i];
            m_charObjs[i].AI_Arrive(m_charObjs[i].GameObject.transform.position,
                m_formationPoints[i], 1f,
                delegate()
                {
                    Debug.Log("TransformFormation中Group执行CheckAndSetArrived" + lookAt);
                    CheckAndSetArrived(charObj, lookAt, callback);
                });
        }
    }

    public void AI_Arrive(Vector3 start, Vector3 target, float speed)
    {
        AI_Arrive(start, target, speed, null);
    }

    public void AI_Arrive(Vector3 start, Vector3 target, float speed, ArrivedCallback callback)
    {
        //所有对象完成Arrive后的回调：要变成Idle阵型
        ArrivedCallback arrivedCallback = delegate()
        {
            Debug.Log("所有对象完成Arrive后要变成Idle阵型");
            TransformFormation(Param.GetFormationParam(E_GROUP_COMMOND.IDLE),
                target, target, callback);
        };

        //阵型变换完成后的回调：执行Group中单个对象的Arrive
        ArrivedCallback transformedCallback = delegate()
        {
            Arrive(start, target, speed, arrivedCallback);
        };

        //不是ARRIVE的阵型先变成ARRIVE的阵型
        GroupFormationParam arriveParam = Param.GetFormationParam(E_GROUP_COMMOND.ARRIVE);
        if (arriveParam == null)
        {
            Debug.LogError("没有找到本Group的Arrive阵型参数");
            return;
        }
        if (GroupFormationParamID != arriveParam.ParamID)
        {
            Debug.Log("不是ARRIVE的阵型先变成ARRIVE的阵型" + m_center);
            TransformFormation(Param.GetFormationParam(E_GROUP_COMMOND.ARRIVE),
                m_center, target, transformedCallback);
        }
        else
        {
            Arrive(start, target, speed, arrivedCallback);
        }
    }

    /// <summary>
    /// Group中单个对象依次执行各自的Arrive
    /// </summary>
    /// <param name="start"></param>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    /// <param name="callback"></param>
    public void Arrive(Vector3 start, Vector3 target, float speed, ArrivedCallback callback)
    {
        for (int i = 0; i < m_charObjs.Count; i++)
        {
            Vector3 _start = start + (m_charObjs[i].GameObject.transform.position - m_center);
            Vector3 _target = target + (_start - m_center);
            CharObj charObj = m_charObjs[i];
            m_charObjs[i].AI_Arrive(_start, _target, speed,
                delegate()
                {
                    Debug.Log("AI_Arrive 中执行CheckArrived " + Time.realtimeSinceStartup + " " + target);
                    CheckAndSetArrived(charObj, target - start, callback);
                });
        }
    }

    /// <summary>
    /// 检查并设置Group的Arrive状态，目前以下两种情况用到
    /// （1）阵型变换时候对象要走位
    /// （2）group整个执行Arrive
    /// </summary>
    /// <param name="charObj"></param>
    /// <param name="lookAt"></param>
    /// <param name="callback"></param>
    public void CheckAndSetArrived(CharObj charObj, Vector3 lookAt, ArrivedCallback callback)
    {
        bool retCode = false;

        //Debug.Log(charObj.ServerEntityID + " 到了");
        GroupCharObjsElement element = null;
        retCode = m_elments.TryGetValue(charObj.ServerEntityID, out element);
        if (!retCode)
        {
            Debug.LogError("没有找到实体ID" + charObj.ServerEntityID);
            return;
        }
        //更新自己的到达标志
        element.Arrived = true;
        //调整朝向
        charObj.AI_LookAt(charObj.GameObject.transform.position, lookAt + lookAt.normalized * (m_radius + 1f));
        //Debug.Log("xxxxxxxxxxxx" + Time.realtimeSinceStartup + " " + lookAt);
        charObj.AI_LookAt(charObj.GameObject.transform.position,
            m_lookAt + (lookAt).normalized * (m_radius + 1f));
        //检查所有的到达标志
        bool allArrived = true;
        foreach (var e in m_elments.Values)
        {
            if (!e.Arrived)
            {
                allArrived = false;
                break;
            }
        }

        if (allArrived)
        {
            Debug.Log("都到了");
            //都到了就设置没到达,相当于设置成初始的状态
            foreach (var e in m_elments.Values)
            {
                e.Arrived = false;
            }
            if (callback != null)
            {
                callback();
            }
        }
    }

    public void AI_Attack()
    {
        //BattleObjManager.Instance.StartCoroutine(Attack_1());

        //for (int i = 0; i < m_charObjs.Count; i++)
        //{
        //    BattleObjManager.Instance.StartCoroutine(Attack_2(i));
        //}

        //BattleObjManager.Instance.StartCoroutine(Attack_3());

        for (int i = 0; i < m_charObjs.Count; i++)
        {

            m_charObjs[i].AI_Attack(m_charObjs[i].GameObject.transform.position,
                m_charObjs[i].GameObject.transform.position, Random.Range(0.5f, 2.5f));

        }
    }

    public IEnumerator Attack_1()
    {
        yield return null;

        for(int i = 0; i < m_charObjs.Count; i++)
        {
            m_charObjs[i].AI_Attack(m_charObjs[i].GameObject.transform.position,
                m_charObjs[i].GameObject.transform.position, 0);
            //yield return new WaitForSeconds(Random.Range(1f, 3f));
            yield return new WaitForSeconds(2f);
        }
    }
    public IEnumerator Attack_2(int i)
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        m_charObjs[i].AI_Attack(m_charObjs[i].GameObject.transform.position,
            m_charObjs[i].GameObject.transform.position, 0);
    }

    public IEnumerator Attack_3()
    {
        //yield return new WaitForSeconds(Random.Range(1f, 3f));
        //m_charObjs[i].AI_Attack(m_charObjs[i].GameObject.transform.position,
        //    m_charObjs[i].GameObject.transform.position, 0);
        int first = Random.Range(0, m_charObjs.Count);
        Debug.Log(first);
        m_charObjs[first].AI_Attack(m_charObjs[first].GameObject.transform.position,
            m_charObjs[first].GameObject.transform.position, 0);
        for(int i = 0; i < first; i++)
        {
            m_charObjs[i].AI_Attack(m_charObjs[i].GameObject.transform.position,
                m_charObjs[i].GameObject.transform.position, 0);
            Debug.Log(i);
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }

        for (int i = first + 1; i < m_charObjs.Count; i++)
        {
            m_charObjs[i].AI_Attack(m_charObjs[i].GameObject.transform.position,
                m_charObjs[i].GameObject.transform.position, 0);
            Debug.Log(i);
            yield return new WaitForSeconds(Random.Range(1f, 1.5f));
        }

    }

    void CacheObjs(GroupCharObjsElement[] elments)
    {
        if (m_isCached)
        {
            Debug.Log("allready cached");
            return;
        }

        for (int i = 0; i < elments.Length; i++)
        {
            CharObj charObj = null;
            charObj = BattleObjManager.Instance.BorrowCharObj(
                elments[i].Type, elments[i].ServerEntityID, 0);
            if (charObj == null)
            {
                Debug.LogError("角色对象charObj为空");
            }
            m_charObjs.Add(charObj);

            elments[i].Arrived = false;
            m_elments.Add(elments[i].ServerEntityID, elments[i]);
        }

        m_isCached = true;
    }

    public void Release()
    {
        if (m_formationPoints != null)
        {
            m_formationPoints.Clear();
            m_formationPoints = null;
        }

        if (m_elments != null)
        {
            m_elments.Clear();
            m_elments = null;
        }

        if (m_charObjs != null)
        {
            for(int i = 0; i < m_charObjs.Count; i++)
            {
                BattleObjManager.Instance.ReturnCharObj(m_charObjs[i]);
            }
            m_charObjs.Clear();
            m_charObjs = null;
        }

        if (Param != null)
        {
            Param.Release();
            Param = null;
        }
    }
}
