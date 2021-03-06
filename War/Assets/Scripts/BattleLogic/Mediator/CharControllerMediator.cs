﻿/// <summary>
/// CharController的中介
/// 
/// author: fanzhengyong
/// date: 2017-03-08
/// 
/// 通过组合其本身正交集的Commond实现外界对角色的控制
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public static class CharControllerMediator
{
    public static void AI_Position(CharController cctr, Vector3 position)
    {
        cctr.PrepareSwitchCommond();
        cctr.m_positionData.TargetForPosition = position;
        cctr.Commond(CharController.E_COMMOND.POSITION);
    }

    public static void AI_Open(CharController cctr, Vector3 position, Vector3 lookAt)
    {
        cctr.PrepareSwitchCommond();
        cctr.m_positionData.TargetForPosition = position;
        cctr.m_positionData.TargetForLookAt = lookAt;
        cctr.Commond(CharController.E_COMMOND.POSITION);
        cctr.Commond(CharController.E_COMMOND.LOOKAT);
        cctr.Commond(CharController.E_COMMOND.OPEN);
    }

    public static void AI_Arrive(CharController cctr, Vector3 startPoint, 
        Vector3 endPoint, float speed, MoveSteers.StopMoveCallback callback = null)
    {
        //Debug.Log("Arrive " + startPoint + " " + endPoint + " " + Time.realtimeSinceStartup);
        cctr.PrepareSwitchCommond();
        cctr.m_positionData.TargetForPosition = startPoint;
        cctr.m_positionData.TargetForArrive = endPoint;
        cctr.m_positionData.SpeedForArrive = speed;
        //cctr.m_steers.m_stopMoveCallback = callback;
        cctr.m_outStopCallback = callback;
        cctr.Commond(CharController.E_COMMOND.ARRIVE);
    }

    public static void AI_Attack(CharController cctr, Vector3 position,
        Vector3 target, float waitSeconds)
    {
        //Debug.Log("Attack");
        cctr.PrepareSwitchCommond();
        //自己定位
        cctr.m_positionData.TargetForPosition = position;
        cctr.Commond(CharController.E_COMMOND.POSITION);

        //面朝目标
        cctr.m_positionData.TargetForLookAt = target;
        cctr.Commond(CharController.E_COMMOND.LOOKAT);
        //RuntimeAnimatorController bak = cctr.Animator.runtimeAnimatorController;
        //cctr.Animator.runtimeAnimatorController = null;
        //cctr.Transform.Find("Bone01/Bone02").Rotate(90f, 0f, 0f);
        //cctr.Animator.runtimeAnimatorController = bak;

        //0.3秒内的等待不执行
        if (waitSeconds > 0.3f)
        {
            cctr.WaitForSeconds = waitSeconds;
            cctr.WaitForCommond = CharController.E_COMMOND.ATTACK;
            cctr.Commond(CharController.E_COMMOND.WAIT);
            return;
        }

        cctr.Commond(CharController.E_COMMOND.ATTACK);
    }

    public static void AI_Attacked(CharController cctr, Vector3 position)
    {
        cctr.PrepareSwitchCommond();
        cctr.m_positionData.TargetForPosition = position;
        cctr.Commond(CharController.E_COMMOND.POSITION);
        cctr.Commond(CharController.E_COMMOND.ATTACKED);
    }

    public static void AI_StopMove(CharController cctr)
    {
        cctr.PrepareSwitchCommond();
        cctr.Commond(CharController.E_COMMOND.STOPMOVE);
    }

    public static void AI_Dead(CharController cctr,int deadChangeEntityID, 
        BattleObjManager.E_BATTLE_OBJECT_TYPE deadChangeObjType,
        Vector3 deadPosition, Vector3 deadTarget, float deadMoveSpeed)
    {
        //自己定位
        cctr.m_positionData.TargetForPosition = deadPosition;
        cctr.Commond(CharController.E_COMMOND.POSITION);
        //变身
        cctr.DeadChangeEntityID = deadChangeEntityID;
        cctr.DeadPosition       = deadPosition;
        cctr.DeadTarget         = deadTarget;
        cctr.DeadMoveSpeed      = deadMoveSpeed;
        cctr.DeadChangeObjType  = deadChangeObjType;
        cctr.Commond(CharController.E_COMMOND.DEAD);
    }

    public static void AI_LookAt(CharController cctr, Vector3 position, Vector3 lookAt)
    {
        cctr.m_positionData.TargetForPosition = position;
        cctr.m_positionData.TargetForLookAt = lookAt;
        cctr.Commond(CharController.E_COMMOND.POSITION);
        cctr.Commond(CharController.E_COMMOND.LOOKAT);
    }
}
