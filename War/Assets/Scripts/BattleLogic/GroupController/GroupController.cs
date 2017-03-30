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
    }

    List<CharObj> m_charObjs = new List<CharObj>();

    public void AI_Arrive(
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
