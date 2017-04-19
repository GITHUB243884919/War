using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UF_FrameWork;

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
        //HORIZONTAL_LINE, 
        //VERTICAL_LINE,  
        TARGET_HORIZONTAL_LINE, //面朝目标横向一字型
        TARGET_VERTICAL_LINE,   //面朝目标纵向一字型
        TARGET_TRANGLE,         //面朝目标三角形
        TARGET_CYCLE            //面朝目标散开
    }

    public static readonly int MIN_CHAROBJ_COUNT = 2;

    List<CharObj> m_charObjs = new List<CharObj>();
    Dictionary<int, GroupCharObjsElement> m_elments 
        = new Dictionary<int, GroupCharObjsElement>();

    bool m_isCached = false;
    public void Init(GroupCharObjsElement[] elments, E_FORMATION_TYPE formation, Vector3 orginPoint)
    {
        //Debug.Log("GroupCharObjsController Init " 
        //    + m_charObjs.Count + " " + m_elments.Count);

        bool retCode = false;

        CacheObjs(elments);

        Vector3[] positions = null;
        retCode = GetFormationPositions(
            m_charObjs.Count, formation, true,
            orginPoint, out positions);
        if (!retCode)
        {
            Debug.LogError("获取阵型坐标失败" + formation.ToString());
            return;
        }

        //Debug.Log("直接形成初始阵型");
        for(int i = 0; i < m_charObjs.Count; i++)
        {
            //m_charObjs[i].AI_Position(positions[i]);
            m_charObjs[i].AI_LookAt(positions[i], orginPoint);
        }
    }

    public void CacheObjs(GroupCharObjsElement[] elments)
    {
        //m_charObjs.Clear();
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

    public bool GetFormationPositions(
        int count, E_FORMATION_TYPE formation, bool isStandCenter, 
        Vector3 target, out Vector3 [] positions)
    {
        bool result = false;

        positions       = null;
        float radius    = 3f;
        Vector3 center  = target - target.normalized * radius;
        float targetRad = GeometryUtil.TwoPointAngleRad2D(center, target);
        float targetDeg = targetRad * Mathf.Rad2Deg;
        float angleDeg  = 0f;

        //count必须大于2
        if (count < MIN_CHAROBJ_COUNT)
        {
            Debug.LogError("阵型需要至少" + MIN_CHAROBJ_COUNT + "个对象");
            return result;
        }
        List<Vector3> points = new List<Vector3>();
        switch (formation)
        {
            case E_FORMATION_TYPE.TARGET_HORIZONTAL_LINE:
                //先算出横向两个点的坐标
                angleDeg = 90f;
                Vector3 p1 = Vector3.zero;
                Vector3 p2 = Vector3.zero;
                p1 = GeometryUtil.PositionInCycleByAngleDeg2D(center, radius, targetDeg + angleDeg);
                p2 = GeometryUtil.PositionInCycleByAngleDeg2D(center, radius, targetDeg - angleDeg);
                points.Add(p1);
                points.Add(p2);
                Debug.DrawLine(center, target, Color.blue, (target - center).magnitude);
                //Debug.Log((target - center).magnitude);
                Debug.DrawLine(center, p1, Color.red, (p1 - center).magnitude);
                //Debug.Log((p1 - center).magnitude);
                Debug.DrawLine(center, p2, Color.cyan, (p2 - center).magnitude);
                //Debug.Log((p2 - center).magnitude);
                //其他点的坐标按个数平分
                break;
            case E_FORMATION_TYPE.TARGET_VERTICAL_LINE:
                //目标点即是第一个点
                //第二个点是转了180
                angleDeg = 180f;
                p1 = GeometryUtil.PositionInCycleByAngleDeg2D(center, radius, targetDeg + angleDeg);
                Vector3 spaceDir = (p1 - target).normalized;
                float spaceOffset = (p1 - target).magnitude / (count - 1);
                points.Add(target);
                for (int i = 1; i < count; i++)
                {
                    points.Add(target + spaceDir * spaceOffset * i);
                }
                break;
            case E_FORMATION_TYPE.TARGET_TRANGLE:
                Vector3 tragleP1 = target;

                Vector3 tragleP2 = GeometryUtil.PositionInCycleByAngleDeg2D(
                    center, radius, targetDeg + 120f);
                Vector3 tragleP3 = GeometryUtil.PositionInCycleByAngleDeg2D(
                    center, radius, targetDeg - 120f);
                points.Add(tragleP1);
                points.Add(tragleP2);
                points.Add(tragleP3);

                //float tempDeg = 360 / count;
                //for(int i = 0; i < count; i++)
                //{
                //    Vector3 p = GeometryUtil.PositionInCycleByAngleDeg2D(
                //        center, radius, targetDeg + i * tempDeg);
                //    points.Add(p);
                //}
                break;
            case E_FORMATION_TYPE.TARGET_CYCLE:
                Debug.Log(E_FORMATION_TYPE.TARGET_CYCLE.ToString());
                float tempDeg = 360 / count;
                for(int i = 0; i < count; i++)
                {
                    Vector3 p = GeometryUtil.PositionInCycleByAngleDeg2D(
                        center, radius, targetDeg + i * tempDeg);
                    points.Add(p);
                }
                break;
            default:
                Debug.LogError("没有这种队形的实现" + formation.ToString());
                break;

        }

        positions = points.ToArray();

        result = true;
        return result;
    }

    public void SwitchFormation(E_FORMATION_TYPE formation, Vector3 target)
    {
        bool retCode = false;

        Vector3[] positions = null;
        retCode = GetFormationPositions(
            m_charObjs.Count, formation, true,
            target, out positions);
        if (!retCode)
        {
            Debug.LogError("获取阵型坐标失败" + formation.ToString());
            return;
        }

        for (int i = 0; i < m_charObjs.Count; i++)
        {
            Debug.Log("移动到指定位置");
            CharObj charObj = m_charObjs[i];
            m_charObjs[i].AI_Arrive(m_charObjs[i].GameObject.transform.position,
                positions[i], 2f, 
                delegate() 
                {
                    Debug.Log("Group执行OnArrived");
                    OnArrived(charObj, target);
                });
        }
    }

    public void OnArrived(CharObj charObj, Vector3 target)
    {
        bool retCode = false;

        Debug.Log(charObj.ServerEntityID + " 到了");
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
        charObj.AI_LookAt(charObj.GameObject.transform.position, target);
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
        if (!allArrived)
        {
            Debug.Log("还有没到的");
        }
        else
        {
            Debug.Log("都到了");
            //都到了就设置没到达
            foreach (var e in m_elments.Values)
            {
                e.Arrived = false;
            }
            //foreach (GroupCharObjsElement e in m_elments.Values)
            //{
            //    //e.Arrived = false;
            //    Debug.Log(e.Arrived);
            //}
        }
    }

    void Test_5()
    {
        Vector3 target = new Vector3(10f, 0f, 32f);
        float radius = 15f;
        float angleDeg = 45f;
        Vector3 center = target - target.normalized * radius;
        float targetRad = GeometryUtil.TwoPointAngleRad2D(center, target);
        float targetDeg = targetRad * Mathf.Rad2Deg;
        Vector3 p1 = Vector3.zero;
        Vector3 p2 = Vector3.zero;
        p1 = GeometryUtil.PositionInCycleByAngleDeg2D(center, radius, targetDeg + angleDeg);
        p2 = GeometryUtil.PositionInCycleByAngleRad2D(center, radius, targetRad - angleDeg * Mathf.Deg2Rad);

        Debug.DrawLine(center, target, Color.red, (target - center).magnitude);
        Debug.Log((target - center).magnitude);
        Debug.DrawLine(center, p1, Color.red, (p1 - center).magnitude);
        Debug.Log((p1 - center).magnitude);
        Debug.DrawLine(center, p2, Color.red, (p2 - center).magnitude);
        Debug.Log((p2 - center).magnitude);
    }

    public void Release()
    {
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
