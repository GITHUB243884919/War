﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scene_Commond_Test : MonoBehaviour
{
    bool m_withEffect = false;
    int m_effectCycle = 0;
    bool m_wait = false;
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
    }



    public void BtnClick()
    {
        //Debug.Log("Begin click " + Time.realtimeSinceStartup);
        StartCoroutine("CreateTanks");
        //Profiler.BeginSample("xxxxxxxxxxxxxxxxxxxxxx");
        //Debug.Log(Time.realtimeSinceStartup);
        //for (int i = 0; i < 128; i++)
        //{
        //    //CreateTank(i, false, false);
        //    //CreateTank(i, true, false);
        //    CreateTank_Arrive_Attack(i, true, true);
        //}
        //Debug.Log(Time.realtimeSinceStartup);
        //Profiler.EndSample();
        //Debug.Log("End click " + Time.realtimeSinceStartup);
        //m_effectCycle++;
        //if (m_effectCycle > 1)
        //{
        //    //m_withEffect = false;
        //    m_wait = true;
        //}

    }

    IEnumerator CreateTanks()
    {
        //bool m_withEffect = true;
        yield return null;
        //CreateTank_Arrive_Attack(1, true, false);
        //CreateTank_Arrive_Attack(1, true, false);
        for (int i = 0; i < 128; i++)
        //for (int i = 0; i < 1; i++)
        {
            //CreateTank_Arrive_Attack(i, false, false);
            CreateTank_Arrive_Attack(i, true, false);
            //CreateTank_Arrive_Attack(i, true, true);
            //yield return null;
        }
        m_effectCycle++;
        if (m_effectCycle > 1)
        {
            //m_withEffect = false;
            m_wait = true;
        }
#if _WAR_TEST_
        //MeshBakerClearManager.Instance.Realse();
#endif

        //CreateTank_Dead(1, 2);
        //CreateTank_Arrive_Attack_LookAt(false, true);
        //yield return new WaitForSeconds(5f);
        //Debug.Log("begin return " + Time.realtimeSinceStartup);
        //BattleObjManager.Instance.ReturnAllBorrowCharObjs();
        //Debug.Log("end return " + Time.realtimeSinceStartup);
    }
    void CreateTank_Arrive_Attack(int entityID, bool withAI, bool withEffect)
    {
        BattleObjManager.E_BATTLE_OBJECT_TYPE type
            = BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK;
            //= BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS;
            //= BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ARTILLERY;
        //int serverEntityID = entityID;
        int serverEntityType = 1;

        Vector3 startPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        Vector3 endPos   = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        //假定服务器规定这段路必须time秒走完
        float   time     = 30f;
        float   speed    = (endPos - startPos).magnitude / time;
        CharObj obj      = BattleObjManager.Instance.BorrowCharObj(
            type, entityID, serverEntityType);

        if (withAI)
        {
            //obj.Position(startPos);
            obj.AI_Arrive(startPos, endPos, speed);
        }

        if (m_withEffect)
        //if (withEffect)
        {
            float waitseconds = 0f;
            if (m_wait)
            {
                waitseconds = 5.0f;
            }
            if (entityID < 32)
            {
                obj.AI_Attack(startPos, endPos, waitseconds);
                //obj.AI_Attacked(startPos);
            }
            else if (entityID < 64 )
            {
                obj.AI_Attack(startPos, endPos, waitseconds);
            }
            else
            {
                obj.AI_Attack(startPos, endPos, waitseconds);
                //obj.AI_Attacked(startPos);
            }
        }

    }

    void CreateTank_Dead(int entityID, int deadEntityID)
    {
        BattleObjManager.E_BATTLE_OBJECT_TYPE type 
            = BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK;
        int serverEntityID = entityID;
        int serverEntityType = 1;

        CharObj obj = BattleObjManager.Instance.BorrowCharObj(
            type, serverEntityID, serverEntityType);

        Vector3 startPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        Vector3 endPos   = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        //假定服务器规定这段路必须time秒走完
        float   time     = 30f;
        float   speed    = (endPos - startPos).magnitude / time;
        obj.AI_Position(startPos);
        //obj.CharController.WaitForSeconds = 5f;
        //obj.CharController.WaitForCommond = CharController.E_COMMOND.NONE;
        obj.AI_Dead(deadEntityID, type, startPos, endPos, speed);
    }

    void CreateTank_Arrive_Attack_LookAt(bool withAI, bool withEffect)
    {
        BattleObjManager.E_BATTLE_OBJECT_TYPE type
            = BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK;
        int serverEntityID = 1;
        int serverEntityType = 1;

        Vector3 startPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        Vector3 endPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        //假定服务器规定这段路必须time秒走完
        float time = 30f;
        float speed = (endPos - startPos).magnitude / time;
        CharObj obj = BattleObjManager.Instance.BorrowCharObj(
            type, serverEntityID, serverEntityType);
        obj.AI_Position(startPos);
        Camera.main.GetComponent<Camera>().transform.LookAt(obj.GameObject.transform.position);
        Camera.main.GetComponent<Camera>().fieldOfView = 12f;
        if (withAI)
        {
            obj.AI_Arrive(startPos, endPos, speed);
        }

        if (withEffect)
        {
            obj.AI_Attack(startPos, endPos, 5f);
        }

    }


}
