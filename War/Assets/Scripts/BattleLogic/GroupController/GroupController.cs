using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GroupElement
{
    public int ServerEntityID { get; set; }
    public BattleObjManager.E_BATTLE_OBJECT_TYPE Type { get; set; }
}
public class GroupController
{
    public enum E_FORMATION
    {
        //HORIZONTAL_LINE, 
        //VERTICAL_LINE,   
        TARGET_VERTICAL_LINE, //面朝目标纵向
        TARGET_TRANGLE        //面朝目标三角形
    }

    List<CharObj> m_charObjs = new List<CharObj>();

    public void AI_Arrive_TARGET_VERTICAL_LINE(
        GroupElement [] elments, E_FORMATION formation,
        Vector3 startPoint, Vector3 endPoint, float speed, 
        MoveSteers.StopMoveCallback callback)
    {
        Vector3 dir = (endPoint - startPoint).normalized;
        for(int i = 0; i < elments.Length; i++)
        {
            CharObj charObj = null;
            charObj = BattleObjManager.Instance.BorrowCharObj(
                elments[i].Type, elments[i].ServerEntityID, 0);
            if (charObj == null)
            {
                LogMediator.LogError("角色对象charObj为空");
            }
            m_charObjs.Add(charObj);
            charObj.AI_Arrive(startPoint + dir * i * 15f, endPoint - dir * i * 15f, speed);
        }
    }

    public void AI_Arrive_TARGET_TRANGLE(
        GroupElement[] elments, E_FORMATION formation,
        Vector3 startPoint, Vector3 endPoint, float speed,
        MoveSteers.StopMoveCallback callback)
    {
        FireRange();
    }

    void FireRange()
    {
        float fireRange = 15f;
        Vector3 startPos = new Vector3(0,0,32);
        Vector3 endPos = startPos + new Vector3(fireRange, 0f, 0f);
        LogMediator.DrawLine(startPos, endPos, Color.red, (endPos - startPos).magnitude);
        float deg = 30f;
        float radStart = Mathf.Deg2Rad * deg;
        float endStart = Mathf.Deg2Rad * (deg + 90f);
        Vector3 rayPoint1 = new Vector3(
            fireRange * Mathf.Cos(endStart),
            0,
            fireRange * Mathf.Sin(endStart));
        LogMediator.Log(rayPoint1);
        LogMediator.DrawLine(startPos, rayPoint1, Color.blue, (rayPoint1 - startPos).magnitude);

    }
    //void FireRange2()
    //{
    //    Vector3 startPos = Vector3.zero;
    //    Vector3 endPos = Vector3.zero + new Vector3(15f, 0f, 0f);

    //    Vector3 frontRayPoint = transform.position + (transform.forward * fireRange);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, fireRange);
    //    Debug.DrawLine(transform.position + new Vector3(0, 2, 0), frontRayPoint + new Vector3(0, 2, 0), Color.black);
    //    //角度转弧度
    //    float fieldOfAttackinRadians = fieldOfAttack * 3.14f / 180.0f;
    //    float fieldOfAttackinRadians2 = 3.14f / 2 + fieldOfAttackinRadians;
    //    //根据角的定义，按照顺时针方向旋转而成的角叫做负角。逆时针旋转则是正角，没有旋转叫零角。
    //    Vector3 rayPoint1 = new Vector3(
    //        fireRange * Mathf.Cos(fieldOfAttackinRadians2),
    //        0,
    //        fireRange * Mathf.Sin(fieldOfAttackinRadians2));
    //    rayPoint1 = transform.TransformPoint(rayPoint1);
    //    Debug.DrawLine(transform.position, rayPoint1 + new Vector3(0, 1, 0), Color.blue);

    //    //for (int i = 0; i < 11; i++)
    //    //for (int i = 0; i<1; i++)
    //    for (int i = 0; i < 10; i++)
    //    {
    //        RaycastHit hit;
    //        //x = sin 如果从Y的正方向往下看所构成的2维平面(X,Z)，
    //        //Z在垂直方向，X在水平方向
    //        float angle = -fieldOfAttackinRadians + fieldOfAttackinRadians * 0.2f * (float)i;
    //        Vector3 rayPoint = transform.TransformPoint(new Vector3(
    //            fireRange * Mathf.Sin(angle),
    //            0,
    //            fireRange * Mathf.Cos(angle)));


    //        if (i == 0)
    //        {
    //            Debug.DrawLine(transform.position + new Vector3(0, 1, 0), rayPoint + new Vector3(0, 1, 0), Color.green);
    //        }
    //        else
    //        {
    //            Debug.DrawLine(transform.position + new Vector3(0, 1, 0), rayPoint + new Vector3(0, 1, 0), Color.red);
    //        }

    //    }

    //    Debug.DrawLine(transform.position, frontRayPoint + new Vector3(0, 1, 0), Color.black);
    //}

    public void AI_Arrive_TARGET_TRANGLE_Old(
        GroupElement[] elments, E_FORMATION formation,
        Vector3 startPoint, Vector3 endPoint, float speed,
        MoveSteers.StopMoveCallback callback)
    {
        Vector3 dir = (endPoint - startPoint).normalized;
        for (int i = 0; i < elments.Length; i++)
        {
            CharObj charObj = null;
            charObj = BattleObjManager.Instance.BorrowCharObj(
                elments[i].Type, elments[i].ServerEntityID, 0);
            if (charObj == null)
            {
                LogMediator.LogError("角色对象charObj为空");
            }
            m_charObjs.Add(charObj);
            //charObj.AI_Arrive(startPoint + dir * i * 15f, endPoint - dir * i * 15f, speed);
        }
        Vector3 offset = Vector3.zero;
        //m_charObjs[0].AI_Arrive(startPoint, endPoint, speed);
        //offset = dir;
        //offset.x -= 15f;
        //offset.z -= 15f;
        //m_charObjs[1].AI_Arrive(startPoint, endPoint + offset, speed);
        //offset = dir;
        //offset.x += 15f;
        //offset.z -= 15f;
        //m_charObjs[2].AI_Arrive(startPoint, endPoint + offset, speed);
        LogMediator.DrawLine(startPoint, endPoint, Color.red, (endPoint - startPoint).magnitude);
        endPoint -= (dir * 15f);
        LogMediator.DrawLine(startPoint, endPoint, Color.blue, (endPoint - startPoint).magnitude);


        float deg = 45f;
        float l = 15f;
        float rad = Mathf.Deg2Rad * deg;
        float r = Mathf.Cos(rad) * l;
        float x = r * Mathf.Sin(rad);
        float z = r * Mathf.Cos(rad);
        endPoint = new Vector3(x, 0, z);
        LogMediator.Log(endPoint);
        LogMediator.DrawLine(startPoint, endPoint, Color.red, (endPoint - startPoint).magnitude);


    }

    public void Release()
    {
        for(int i = 0; i < m_charObjs.Count; i++)
        {
            BattleObjManager.Instance.ReturnCharObj(m_charObjs[i]);
        }
        m_charObjs.Clear();
        m_charObjs = null;
    }



}
