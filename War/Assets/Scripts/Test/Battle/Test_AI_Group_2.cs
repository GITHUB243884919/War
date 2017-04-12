using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Test_AI_Group_2 : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
    }

    public void BtnClick()
    {
        CreateGroupCharObjs();
    }

    void CreateGroupCharObjs()
    {
        GroupCharObjsElement[] elements = new GroupCharObjsElement[3];
        elements[0] = new GroupCharObjsElement();
        elements[0].ServerEntityID = 0;
        elements[0].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[1] = new GroupCharObjsElement();
        elements[1].ServerEntityID = 1;
        elements[1].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[2] = new GroupCharObjsElement();
        elements[2].ServerEntityID = 2;
        elements[2].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        GroupCharObjsController groupCtr = new GroupCharObjsController();
        GroupCharObjsController.E_FORMATION_TYPE formationType =
            //GroupCharObjsController.E_FORMATION_TYPE.TARGET_HORIZONTAL_LINE;
            GroupCharObjsController.E_FORMATION_TYPE.TARGET_VERTICAL_LINE;
        groupCtr.Init(elements, formationType, new Vector3(32f, 0f, 32f));

    }

}
