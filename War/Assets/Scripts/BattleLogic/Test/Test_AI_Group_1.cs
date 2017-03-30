using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Test_AI_Group_1 : MonoBehaviour
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
        GroupController groupController = new GroupController();
        GroupElement [] elements = new GroupElement[3];
        elements[0] = new GroupElement();
        elements[0].ServerEntityID = 0;
        elements[0].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[1] = new GroupElement();
        elements[1].ServerEntityID = 1;
        elements[1].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[2] = new GroupElement();
        elements[2].ServerEntityID = 2;
        elements[2].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;
        Vector3 startPos = new Vector3(32f, 0f, 32f);
        //Vector3 endPos = new Vector3(Random.Range(10, 310), 0f, Random.Range(10, 310));
        Vector3 endPos = new Vector3(160f, 0f, 160f);
        //假定服务器规定这段路必须time秒走完
        float time = 15f;
        float speed = (endPos - startPos).magnitude / time;
        groupController.AI_Arrive(elements, 
            GroupController.E_FORMATION.TARGET_VERTICAL_LINE,
            startPos, endPos, speed, null);
    }

    void ArriveCallback()
    {
        LogMediator.Log("ArriveCallback");
    }
}
