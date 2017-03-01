using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scene_Commond_Test : MonoBehaviour
{

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
    }



    public void BtnClick()
    {
        //Debug.Log("Begin click " + Time.realtimeSinceStartup);
        StartCoroutine("CreateTanks");
        //for (int i = 0; i < 128; i++)
        //{
        //    //CreateTank(i, false, false);
        //    //CreateTank(i, true, false);
        //    CreateTank(i, true, true);

        //}
        //Debug.Log("End click " + Time.realtimeSinceStartup);

    }

    IEnumerator CreateTanks()
    {
        yield return null;
        for (int i = 0; i < 128; i++)
        //for (int i = 0; i < 1; i++)
        {
            //CreateTank_Arrive_Attack(i, false, false);
            //CreateTank_Arrive_Attack(i, true, false);
            CreateTank_Arrive_Attack(i, true, true);
        }

        //CreateTank_Dead(1, 2);
    }
    void CreateTank_Arrive_Attack(int entityID, bool withAI, bool withEffect)
    {
        BattleObjManager.E_BATTLE_OBJECT_TYPE type
            = BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK;
        int serverEntityID = entityID;
        int serverEntityType = 1;

        Vector3 startPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        Vector3 endPos   = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        //假定服务器规定这段路必须time秒走完
        float   time     = 30f;
        float   speed    = (endPos - startPos).magnitude / time;
        CharObj obj      = BattleObjManager.Instance.BorrowCharObj(
            type, serverEntityID, serverEntityType);

        if (withAI)
        {
            obj.Arrive(startPos, endPos, speed);
        }

        if (withEffect)
        {
            obj.Attack(startPos, endPos, 5f);
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
        obj.Position(startPos);
        //obj.CharController.WaitForSeconds = 5f;
        //obj.CharController.WaitForCommond = CharController.E_COMMOND.NONE;
        obj.Dead(deadEntityID, type, startPos, endPos, speed);


    }

}
