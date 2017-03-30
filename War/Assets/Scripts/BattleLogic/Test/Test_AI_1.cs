using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Test_AI_1 : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
    }

    public void BtnClick()
    {
        StartCoroutine("CreateCharObjs");
    }

    IEnumerator CreateCharObjs()
    {
        yield return null;

        //for (int i = 0; i < 128; i++)
        //for (int i = 0; i < 1; i++)
        //{

            //CreateTank_Arrive_Attack(i, true, true);
        //}
        

        //CreateTank_Dead(1, 2);
        //CreateTank_Arrive_Attack_LookAt(false, true);

        int entityID = 0;
        BattleObjManager.E_BATTLE_OBJECT_TYPE type
            = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        //BattleObjManager.E_BATTLE_OBJECT_TYPE type
        //    = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_AIRPLANE_01;

        //BattleObjManager.E_BATTLE_OBJECT_TYPE type
        //    = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS;

        //BattleObjManager.E_BATTLE_OBJECT_TYPE type
        //    = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ARTILLERY;
        
        //BattleObjManager.E_BATTLE_OBJECT_TYPE type
        //    = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCAR;

        //BattleObjManager.E_BATTLE_OBJECT_TYPE type
        //    = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_BUILD_JEEP;

        CharObj obj = BattleObjManager.Instance.BorrowCharObj(
            type, entityID, 1);
        CharObjAI(obj, CharController.E_COMMOND.ARRIVE);
        //CharObjAI(obj, CharController.E_COMMOND.ATTACK);
        //CharObjAI(obj, CharController.E_COMMOND.DEAD);
        //CharObjAI(obj, CharController.E_COMMOND.OPEN);

        //CharController.E_COMMOND switchCmd = CharController.E_COMMOND.ATTACK;
        //CharController.E_COMMOND switchCmd = CharController.E_COMMOND.DEAD;
        //StartCoroutine(SwitchAI(obj, switchCmd));
    }

    IEnumerator SwitchAI(CharObj obj, CharController.E_COMMOND cmd)
    {
        //obj.GameObject.transform.LookAt(new Vector3(32, 0f, 0f));
        yield return new WaitForSeconds(5f);
        CharObjAI(obj, cmd);
    }

    void CharObjAI(CharObj obj, CharController.E_COMMOND cmd)
    {
        //Vector3 startPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        Vector3 startPos = new Vector3(20f, 0f, 20f);
        //Vector3 endPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        Vector3 endPos = new Vector3(30f, 0f, 30f);
        //假定服务器规定这段路必须time秒走完
        float time = 3f;
        float speed = (endPos - startPos).magnitude / time;

        LogMediator.Log("speed " + speed);
        switch (cmd)
        {
            case CharController.E_COMMOND.ARRIVE:
                obj.AI_Arrive(startPos, endPos, speed);
                break;
            case CharController.E_COMMOND.ATTACK:
                obj.AI_Attack(startPos, endPos, 1f);
                break;
            case CharController.E_COMMOND.DEAD:
                //obj.AI_Position(startPos);
    /// <param name="deadChangeEntityID">变身后的实体ID，服务器定义。坦克死亡后变成救护车</param>
    /// <param name="deadChangeObjType">实体的类型，比如救护车类型定义</param>
    /// <param name="deadPosition">死亡位置，在哪里死的</param>
    /// <param name="deadTarget">死亡后跑到哪里去</param>
    /// <param name="deadMoveSpeed">跑的速度</param>
                obj.AI_Dead(2, BattleObjManager.E_BATTLE_OBJECT_TYPE.M_BUILD_JEEP, 
                    startPos, endPos, 2f);
                break;
            case CharController.E_COMMOND.OPEN:
                obj.AI_Open(startPos, endPos);
                break;
            default:
                break;
        }
    }
    void CreateCharObj(BattleObjManager.E_BATTLE_OBJECT_TYPE type, 
        int entityID, CharController.E_COMMOND cmd)
    {
        CharObj obj = BattleObjManager.Instance.BorrowCharObj(
            type, entityID, 1);
        //Vector3 startPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        Vector3 startPos = new Vector3(16f, 0f, 16f);
        Vector3 endPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        //假定服务器规定这段路必须time秒走完
        float time = 300f;
        float speed = (endPos - startPos).magnitude / time;
        switch (cmd)
        {
            case CharController.E_COMMOND.ARRIVE:
                obj.AI_Arrive(startPos, endPos, speed);
                break;
            default:
                break;
        }
        
    }

    

    void CreateTank_Arrive_Attack(int entityID, bool withAI, bool withEffect)
    {
        BattleObjManager.E_BATTLE_OBJECT_TYPE type
            = BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK;
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

        //if (withEffect)
        {
            float waitseconds = 0f;
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
