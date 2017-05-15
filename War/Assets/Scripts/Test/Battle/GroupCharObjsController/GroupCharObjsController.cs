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
        TARGET_ATTACH_CAPTION,   //面朝目标并基于队长模型上的挂点
    }

    public static readonly int MIN_CHAROBJ_COUNT = 2;

    //CharObj对象缓存
    List<CharObj> m_charObjs = new List<CharObj>();

    //CharObj对象元素缓存（里面不包含对象，初始时只包含对象的ID）
    Dictionary<int, GroupCharObjsElement> m_elments 
        = new Dictionary<int, GroupCharObjsElement>();

    bool m_isCached = false;

    //队形形成的点的集合
    List<Vector3> m_formationPoints = new List<Vector3>();

    //阵型的中心点
    public Vector3 m_center = Vector3.zero;
    //阵型的朝向
    public Vector3 m_lookAt = Vector3.zero;
    //阵型的半径
    public float   m_radius = 5f;
    //阵型的类型
    public E_FORMATION_TYPE m_formation 
        = E_FORMATION_TYPE.NONE;
    //行走的的阵型
    public E_FORMATION_TYPE m_arriveFormation
        = E_FORMATION_TYPE.TARGET_VERTICAL_LINE;

    public delegate void ArrivedCallback();
    public ArrivedCallback m_arrivedCallback = null;

    /// <summary>
    /// 带参数表接口
    /// </summary>
    /// <param name="paramID"></param>
    /// <param name="groupCommond"></param>
    /// <param name="start"></param>
    /// <param name="lookAt"></param>
    public void Init(int paramID, E_GROUP_COMMOND groupCommond,
        Vector3 start, Vector3 lookAt)
    {
        GroupCommondFormationParam param =
            GroupCommondFormationParamManager.Instance.GetParam(paramID);

        E_BATTLE_OBJECT_TYPE [] objTypes = param.GetCharObjTypesParam();

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

        Init(elements, param.GetFormationParam(groupCommond), start, lookAt);

    }

    public void Init(GroupCharObjsElement[] elements, 
        GroupFormationParam groupFormationParam, Vector3 start, Vector3 lookAt)
    {

        CacheObjs(elements);

        SetFormationPositions(m_charObjs, groupFormationParam,
            start, lookAt);

        for (int i = 0; i < m_charObjs.Count; i++)
        {
            m_charObjs[i].AI_LookAt(m_formationPoints[i], lookAt);
        }
    }

    public void SetFormationPositions(
        List<CharObj> charObjs, GroupFormationParam groupFormationParam,
        Vector3 center,  Vector3 lookAt)
    {
        m_center = center;
        m_lookAt = lookAt;
        m_formation = groupFormationParam.FormationType;

        float lookAtRad = GeometryUtil.TwoPointAngleRad2D(center, lookAt);
        float lookAtDeg = lookAtRad * Mathf.Rad2Deg;

        groupFormationParam.SetFormationPositions(charObjs, center, lookAtDeg, 
            lookAt, ref m_formationPoints);

    }

    public void Init(GroupCharObjsElement[] elements, 
        E_FORMATION_TYPE formation, Vector3 start, Vector3 lookAt)
    {
        bool retCode = false;

        CacheObjs(elements);

        retCode = SetFormationPositions(
            m_charObjs.Count, formation, true,
            start, lookAt);
        if (!retCode)
        {
            Debug.LogError("获取阵型坐标失败" + formation.ToString());
            return;
        }

        //Debug.Log("直接形成初始阵型");
        for (int i = 0; i < m_charObjs.Count; i++)
        {
            m_charObjs[i].AI_LookAt(m_formationPoints[i], lookAt);
        }
    }

    public void AI_Arrive(Vector3 start, Vector3 target, float speed, ArrivedCallback callback)
    {
        //查看阵型是否是一字型
        if (m_formation == m_arriveFormation)
        {
            m_arrivedCallback = callback;
            for (int i = 0; i < m_charObjs.Count; i++)
            {
                Vector3 _start = start + (m_charObjs[i].GameObject.transform.position - m_center);
                Vector3 _target = target + (_start - m_center);
                CharObj charObj = m_charObjs[i];
                m_charObjs[i].AI_Arrive(_start, _target, speed,
                    delegate()
                    {
                        Debug.Log("AI_Arrive 中执行CheckArrived " + Time.realtimeSinceStartup + " " + target);
                        CheckArrived(charObj, target - start);
                    });
            }
        }
        else
        {
            //先变成一字型
            //在执行Arrive
            Debug.Log("不是一字型，要变成一字型" + m_center);
            
            //SwitchFormation(m_arriveFormation, m_center, m_center + m_center.normalized * m_radius);
            SwitchFormation(m_arriveFormation, start, target);
            //m_arrivedCallback = delegate()
            //{
            //    Debug.Log("一字型ok");
            //};
            m_arrivedCallback = delegate()
            {
                m_arrivedCallback = callback;
                for (int i = 0; i < m_charObjs.Count; i++)
                {
                    Vector3 _start = start + (m_charObjs[i].GameObject.transform.position - m_center);
                    Vector3 _target = target + (_start - m_center);
                    CharObj charObj = m_charObjs[i];
                    m_charObjs[i].AI_Arrive(_start, _target, speed,
                        delegate()
                        {
                            CheckArrived(charObj, target - start);
                        });
                }
            };
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

    public bool SetFormationPositions(
        int count, E_FORMATION_TYPE formation, bool isStandCenter, 
        Vector3 center,  Vector3 lookAt)
    {
        Debug.Log("获得阵型");
        bool result = false;

        m_center    = center;
        m_lookAt    = lookAt;
        m_formation = formation;

        //float radius = 3f;
        float lookAtRad = GeometryUtil.TwoPointAngleRad2D(center, lookAt);
        float lookAtDeg = lookAtRad * Mathf.Rad2Deg;
        
        switch(formation)
        {
            case E_FORMATION_TYPE.TARGET_VERTICAL_LINE:
                Formation_TargetVerticalLine(center, m_radius, lookAtDeg, lookAt, 
                    count, ref m_formationPoints);
                break;
            case E_FORMATION_TYPE.TARGET_CYCLE:
                Formation_TargetCycle(center, m_radius, lookAtDeg, lookAt,
                    count, ref m_formationPoints);
                break;
            case E_FORMATION_TYPE.TARGET_CYCLE_CENTER:
                Formation_TargetCycleCenter(center, m_radius, lookAtDeg, lookAt,
                    count, ref m_formationPoints);
                break;
            default:
                Debug.LogError("没有这种阵型的实现 " + formation.ToString());
                break;

        }

        result = true;
        return result;
    }

    public void CheckArrived(CharObj charObj, Vector3 lookAt)
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
            //都到了就设置没到达
            foreach (var e in m_elments.Values)
            {
                e.Arrived = false;
            }
            if (m_arrivedCallback != null)
            {
                m_arrivedCallback();
                m_arrivedCallback = null;
            }
        }
    }

    public void SwitchFormation(E_FORMATION_TYPE formation, Vector3 center, Vector3 lookAt)
    {
        Debug.Log("切换阵型" + formation.ToString());
        bool retCode = false;

        retCode = SetFormationPositions(
            m_charObjs.Count, formation, true,
            center, lookAt);
        if (!retCode)
        {
            Debug.LogError("获取阵型坐标失败" + formation.ToString());
            return;
        }
        Debug.Log(center + " " + lookAt);
        for (int i = 0; i < m_charObjs.Count; i++)
        {
            //Debug.Log("移动到指定位置 " + m_formationPoints[i]);
            CharObj charObj = m_charObjs[i];
            //m_charObjs[i].AI_Arrive(m_charObjs[i].GameObject.transform.position,
            //    lookAt - (m_formationPoints[i] - center), 2f,
            m_charObjs[i].AI_Arrive(m_charObjs[i].GameObject.transform.position,
                m_formationPoints[i], 2f,
                delegate()
                {
                    Debug.Log("SwitchFormation 中 Group执行CheckArrived" + lookAt);
                    CheckArrived(charObj, lookAt);
                });
        }
    }

    void Formation_TargetVerticalLine(Vector3 orgin, float radius, 
        float targetDeg, Vector3 target, int count,
        ref List<Vector3> formationPoints)
    {
        formationPoints.Clear();
        float angleDeg = 180f;
        Vector3 dir = (target - orgin).normalized;
        Vector3 fristPoint = orgin + dir * radius;
        Vector3 lastPoint = GeometryUtil.PositionInCycleByAngleDeg2D(orgin, radius, targetDeg + angleDeg);
        Debug.DrawLine(fristPoint, lastPoint, Color.blue, (fristPoint - lastPoint).magnitude);

        float spaceOffset = (lastPoint - fristPoint).magnitude / (count - 1);
        for (int i = 0; i < count; i++)
        {
            formationPoints.Add(fristPoint - dir * spaceOffset * i);
        }
    }

    void Formation_TargetCycle(Vector3 center, float radius, 
        float orginDeg, Vector3 target, int count,
        ref List<Vector3> formationPoints)
    {
        formationPoints.Clear();
        float tempDeg = 360 / count;
        for (int i = 0; i < count; i++)
        {
            Vector3 p = GeometryUtil.PositionInCycleByAngleDeg2D(
                center, radius, orginDeg + i * tempDeg);
            formationPoints.Add(p);
        }
    }

    void Formation_TargetCycleCenter(Vector3 center, float radius,
        float orginDeg, Vector3 target, int count,
        ref List<Vector3> formationPoints)
    {
        formationPoints.Clear();
        int _count = count - 1;
        float tempDeg = 360 / _count;
        for (int i = 0; i < _count; i++)
        {
            Vector3 p = GeometryUtil.PositionInCycleByAngleDeg2D(
                center, radius, orginDeg + i * tempDeg);
            formationPoints.Add(p);
        }
        formationPoints.Add(center);
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


    }
}
