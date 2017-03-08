using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scene_Commond_Test_2 : MonoBehaviour
{
    bool m_withEffect = true;
    int m_effectCycle = 0;
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
    }



    public void BtnClick()
    {
        StartCoroutine("CreateTanks");
    }

    IEnumerator CreateTanks()
    {
        //bool m_withEffect = true;
        yield return null;
        for (int i = 0; i < 1; i++)
        //for (int i = 0; i < 1; i++)
        {

            CreateTank_Arrive_Attack(i, true, true);
        }
    }
    void CreateTank_Arrive_Attack(int entityID, bool withAI, bool withEffect)
    {
        BattleObjManager.E_BATTLE_OBJECT_TYPE type
            = BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK;
        //int serverEntityID = entityID;
        int serverEntityType = 1;

        Vector3 startPos = new Vector3(128f, 0f, 128f);
        Vector3 endPos   = new Vector3(320f, 0f, 320f);
        //假定服务器规定这段路必须time秒走完
        float   time     = 30f;
        float   speed    = (endPos - startPos).magnitude / time;
        CharObj obj      = BattleObjManager.Instance.BorrowCharObj(
            type, entityID, serverEntityType);

        if (withAI)
        {
            //obj.AI_Position(startPos);
            obj.AI_Arrive(startPos, endPos, speed);
            //obj.AI_Attack(startPos, new Vector3(320f, 0f, 0f), 3f);
        }

        if (m_withEffect)
        //if (withEffect)
        {
            obj.AI_Attack(startPos, endPos, 1f);
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
